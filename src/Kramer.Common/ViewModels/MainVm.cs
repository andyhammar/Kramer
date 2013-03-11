using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
#if DEBUG
        public MainVm()
        {
            Items = new ObservableCollection<FeedItem>(new[]
                {
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464105-hi.m4a", 
                            Title = "22:00",
                            Author = "Ekot",
                            Content = "Ekonyheter",
                            Date = DateTime.Now,
                            Duration = "5:12 min"
                        },
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464093-hi.m4a", 
                            Title = "21:00"
                        }
                });
        }
#endif

        public MainVm(IViewDispatcher viewDispatcher)
        {
            _viewDispatcher = viewDispatcher;
        }

        public async Task Init()
        {
            var untilDate = DateTime.Now.Date;
            var fromDate = untilDate.AddDays(-3);
            var url =
                string.Format(
                    "http://api.sr.se/api/v2/episodes/index?programid=83&fromdate={0}&todate={1}&urltemplateid=3&audioquality=hi&pagination=false&format=json",
                    fromDate.ToSwedishDate(),
                    untilDate.ToSwedishDate());
            await GetFeedAsync(new Uri(url, UriKind.Absolute));
        }

        public IEnumerable<FeedItem> Items { get; set; }

        private async Task GetFeedAsync(Uri uri)
        {
            var request = WebRequest.CreateHttp(uri);
            //request.Method = "GET";
            WebResponse response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);

            var rootObject = await DeserializeResponse(response);

            PopulateItems(rootObject);
            //var iar = request.BeginGetResponse(new AsyncCallback(Callback), request);



            //var result = string.Empty;
            //var deserialized = 
            //JsonConvert.DeserializeObject<RootObject>(result);
            //using (var ms = new MemoryStream())
            //{
            //    DataContractJsonSerializer djs = new DataContractJsonSerializer(typeof(Req));
            //    djs.WriteObject(ms, request);

            //    ms.Position = 0;
            //    string uriString = uri.AbsoluteUri;
            //    var sr = new StreamReader(ms);
            //    string payload = sr.ReadToEnd();
            //    ms.Position = 0;

            //    using (var stream = await request.GetRequestStreamAsync())
            //    {
            //        stream.Write(ms.ToArray(), 0, (int)ms.Length);
            //    }
            //    try
            //    {
            //        WebResponse resp = await request.GetResponseAsync();
            //        using (var responseStream = resp.GetResponseStream())
            //        {
            //            djs = new DataContractJsonSerializer(typeof(Resp));
            //            return (Resp)djs.ReadObject(responseStream);
            //        }
            //    }
            //    catch (WebException ex)
            //    {
            //        // ok, is this a transactional method?
            //        if (transactional)
            //            throw new TransactionalMethodFailedException(ex.Status, ex.Message, uriString, payload);
            //        else
            //            throw new MethodFailedException(ex.Status, ex.Message);
            //    }
            //    catch (Exception ex)
            //    {
            //        // ok, is this a transactional method?
            //        if (transactional)
            //            throw new TransactionalMethodFailedException(WebExceptionStatus.UnknownError, ex.Message, uriString, payload);
            //        else
            //            throw new MethodFailedException(WebExceptionStatus.UnknownError, ex.Message);
            //    }
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
    }
}