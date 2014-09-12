using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Kramer.Common.Api;
using Kramer.Common.Extensions;
using Kramer.Common.Settings;
using Newtonsoft.Json;

namespace Kramer.Common.ViewModels
{
    public class MainVm : BindableBase
    {
        private readonly IViewDispatcher _viewDispatcher;
        private readonly IPlayService _playService;
        private readonly IAppSettings _appSettings;

        public MainVm(IViewDispatcher viewDispatcher, IPlayService playService, IAppSettings appSettings)
        {
            _viewDispatcher = viewDispatcher;
            _playService = playService;
            _appSettings = appSettings;
            PropertyChanged += MainVm_PropertyChanged;
        }

        void MainVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Items")
            {
                TryAutoPlay();
            }
        }

        public async Task Init()
        {
            SetBusy(BusyMode.LoadingData);

            //todo [Andreas Hammar 2014-07-05 01:23]: read from settings

            var untilDate = DateTime.Now.Date.AddDays(1);
            var fromDate = untilDate.AddDays(-3);
            
            //ekonomiekot
            //var URI = "http://api.sr.se/api/v2/episodes/index?programid=178&fromdate={0}&todate={1}&urltemplateid=3&audioquality=hi&pagination=false&format=json";
            
            //ekot
            var URI = "http://api.sr.se/api/v2/episodes/index?programid=4540&fromdate={0}&todate={1}&urltemplateid=3&audioquality=hi&pagination=false&format=json";
            
            var url =
                string.Format(
                    URI,
                    fromDate.ToSwedishDate(),
                    untilDate.ToSwedishDate());
            await GetFeedAsync(new Uri(url, UriKind.Absolute));

            ClearBusy();
        }

        private void TryAutoPlay()
        {
            try
            {
                if (!_appSettings.IsAutoPlaying)
                    return;

                var eligableDateTimeCutoff = DateTime.Now.AddHours(-_appSettings.AutoPlayMaxAgeInHours);
                var newEnoughItems = Items.Where(x => x.Date > eligableDateTimeCutoff);

                var itemsWithinCorrectRange = newEnoughItems.Where(x =>
                {
                    var durationInMinutes = TimeSpan.Parse("00:" + x.Duration).TotalMinutes;
                    return _appSettings.AutoPlayRangeMinInMinutes <= durationInMinutes
                           && _appSettings.AutoPlayRangeMaxInMinutes >= durationInMinutes;
                });
                var item = itemsWithinCorrectRange.FirstOrDefault();

                if (item == null)
                    return;

                _playService.PlayItem(item);

            }
            catch (Exception exc)
            {
                //todo [Andreas Hammar 2014-07-05 01:44]: log error somehow
                return;
            }
        }

        public IEnumerable<FeedItem> Items { get; set; }

        public bool IsBusy { get; set; }
        
        public string StatusText { get; set; }

        public string ErrorText { get; set; }

        private async Task GetFeedAsync(Uri uri)
        {
            var request = WebRequest.CreateHttp(uri);
            //request.Method = "GET";

            try
            {
                WebResponse response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

                var rootObject = await DeserializeResponse(response);

                PopulateItems(rootObject);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                ErrorText =
                    "Kunde inte hämta sändningar. Vänligen se till att du har nätanslutning och försök igen.";
            }
        }

        private void PopulateItems(RootObject root)
        {
            var feedItems = root.episodes.Select(CreateItem);
            _viewDispatcher.RunAsync(() =>
                                     Items = new ObservableCollection<FeedItem>(feedItems));

        }

        private FeedItem CreateItem(Episode episode)
        {
            var localTime = episode.publishdateutc.ToLocalTime();
            var broadcastfile = episode.broadcast.broadcastfiles.First();
            return new FeedItem
                {
                    AudioUri = broadcastfile.url,
                    Title = localTime.ToSwedishTime(),
                    Date = localTime,
                    Author = episode.program.name,
                    Content = episode.title,
                    Duration = broadcastfile.duration.ToMinSecString()
                };
        }

        //private void Callback(IAsyncResult ar)
        //{
        //    var request = ar.AsyncState as WebRequest;
        //    var response = request.EndGetResponse(ar);

        //    var result = string.Empty;

        //    DeserializeResponse(response);
        //}

        private async Task<RootObject> DeserializeResponse(WebResponse response)
        {
            string result;
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = await reader.ReadToEndAsync();
            }
            var deserialized = JsonConvert.DeserializeObject<RootObject>(result);
            return deserialized;

        }


        //private DispatchedHandler ShowError(Exception exception)
        //{
        //    return () => ShowError(exception.Message);
        //}

        //private void ShowError(string message)
        //{
        //    var dlg = new MessageDialog(message);
        //    dlg.ShowAsync();
        //}
        public void ClearBusy()
        {
            StatusText = null;
            IsBusy = false;
        }

        public void SetBusy(BusyMode busyMode)
        {
            IsBusy = true;
            switch (busyMode)
            {
                case BusyMode.StartingPlay:
                    StatusText = "Startar spelning...";
                    break;
                case BusyMode.LoadingData:
                    StatusText = "Hämtar sändningar...";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("busyMode");
            }
        }
    }
}