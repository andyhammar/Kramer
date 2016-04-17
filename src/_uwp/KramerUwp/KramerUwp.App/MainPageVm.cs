using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.Playback;
using KramerUwp.App.Api;
using Newtonsoft.Json;

namespace KramerUwp.App
{
    public class MainPageVm
    {
        public MainPageVm()
        {
            Episodes = new ObservableCollection<EpisodeItemVm>();
            if (DesignMode.DesignModeEnabled)
            {
                Episodes.Add(new EpisodeItemVm() { LengthInMinutes = "5", Title = "09:00" });
                Episodes.Add(new EpisodeItemVm() { LengthInMinutes = "3", Title = "15:00" });
                Episodes.Add(new EpisodeItemVm() { LengthInMinutes = "15", Title = "22:00" });
            }
        }

        public async Task InitAsync()
        {
            var uri = CreateUri();

            var jsonRoot = await GetFeedAsync(uri);

            if (Episodes.Count > 0)
            {
                Episodes.Clear();
                await Task.Delay(200);
            }
            foreach (var episode in jsonRoot.episodes)
            {
                Episodes.Add(CreateItem(episode));
            }
        }


        private static string CreateUri()
        {
            var untilDate = DateTime.Now.Date.AddDays(1);
            var fromDate = untilDate.AddDays(-1);

            //ekonomiekot
            //var URI = "http://api.sr.se/api/v2/episodes/index?programid=178&fromdate={0}&todate={1}&urltemplateid=3&audioquality=hi&pagination=false&format=json";

            //ekot
            const string templateUri =
                "http://api.sr.se/api/v2/episodes/index?programid=4540&fromdate={0}&todate={1}&urltemplateid=3&audioquality=hi&pagination=false&format=json";

            var uri =
                string.Format(
                    templateUri,
                    fromDate.ToSwedishDate(),
                    untilDate.ToSwedishDate());
            return uri;
        }

        private async Task<RootObject> GetFeedAsync(string uri)
        {

            var json = await new HttpClient().GetStringAsync(uri);
            //await Task.Delay(1000); const string json = "{\"copyright\":\"Copyright Sveriges Radio 2016. All rights reserved.\",\"episodes\":[{\"id\":704759,\"title\":\"Ekonyheter\",\"description\":\"Här hör du Ekots nyhetssändningar. Senaste nytt - dygnet runt, året om.\",\"url\":\"http://sverigesradio.se/sida/avsnitt/704759?programid=4540\",\"program\":{\"id\":4540,\"name\":\"Ekot\"},\"publishdateutc\":\"\\/Date(1460757600000)\\/\",\"imageurl\":\"http://sverigesradio.se/sida/images/4540/3634468_2048_1152.jpg?preset=api-default-square\",\"imageurltemplate\":\"http://sverigesradio.se/sida/images/4540/3634468_2048_1152.jpg\",\"broadcast\":{\"availablestoputc\":\"\\/Date(1463349720000)\\/\",\"playlist\":{\"duration\":120,\"publishdateutc\":\"\\/Date(1460757600000)\\/\",\"id\":5678882,\"url\":\"http://sverigesradio.se/api/radio/radio.aspx?type=broadcast&id=5678882&codingformat=.m4a&metafile=asx&quality=hi\",\"statkey\":\"/app/avsnitt/nyheter (ekot)[k(83)]/ekot[p(4540)]/ekonyheter[e(704759)]\"},\"broadcastfiles\":[{\"duration\":120,\"publishdateutc\":\"\\/Date(1460757600000)\\/\",\"id\":5678882,\"url\":\"http://sverigesradio.se/topsy/ljudfil/5678882-hi.m4a\",\"statkey\":\"/app/avsnitt/nyheter (ekot)[k(83)]/ekot[p(4540)]/ekonyheter[e(704759)]\"}]}}]}";

            var root = JsonConvert.DeserializeObject<RootObject>(json);
            return root;
        }

        private EpisodeItemVm CreateItem(Episode episode)
        {
            var localTime = episode.publishdateutc.ToLocalTime();
            var broadcastfile = episode.broadcast.broadcastfiles.First();
            return new EpisodeItemVm()
            {
                AudioUri = broadcastfile.url,
                Title = localTime.ToSwedishTime(),
                //Date = localTime,
                //Author = episode.program.name,
                //Content = episode.title,
                LengthInMinutes = broadcastfile.duration.ToMinSecString()
            };
        }

        public ObservableCollection<EpisodeItemVm> Episodes { get; set; }

        public void Play(EpisodeItemVm episodeItemVm)
        {
            if (episodeItemVm == null)
                return;

            var player = BackgroundMediaPlayer.Current;
            player.Pause();
            player.SetUriSource(new Uri(episodeItemVm.AudioUri));
            //new MediaPlayer().SetUriSource(new Uri(episodeItemVm.AudioUri));
        }
    }


}
