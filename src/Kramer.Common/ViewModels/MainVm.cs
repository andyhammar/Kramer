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
using Newtonsoft.Json;

namespace Kramer.Common.ViewModels
{
    public class MainVm : BindableBase
    {
        private readonly IViewDispatcher _viewDispatcher;
        
        public MainVm(IViewDispatcher viewDispatcher)
        {
            _viewDispatcher = viewDispatcher;
        }

        public async Task Init()
        {
            SetBusy(BusyMode.LoadingData);
            var untilDate = DateTime.Now.Date.AddDays(1);
            var fromDate = untilDate.AddDays(-3);
            var url =
                string.Format(
                    "http://api.sr.se/api/v2/episodes/index?programid=83&fromdate={0}&todate={1}&urltemplateid=3&audioquality=hi&pagination=false&format=json",
                    fromDate.ToSwedishDate(),
                    untilDate.ToSwedishDate());
            await GetFeedAsync(new Uri(url, UriKind.Absolute));
            ClearBusy();
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