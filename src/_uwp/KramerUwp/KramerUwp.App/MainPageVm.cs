using KramerUwp.App;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Contacts;
using Windows.Media.Audio;
using Windows.Web.Syndication;

namespace KramerUwp.App
{
    public class MainPageVm
    {
        public MainPageVm()
        {
            Episodes = new ObservableCollection<EpisodeItemVm>();
            if (DesignMode.DesignModeEnabled)
            {
                Episodes.Add(new EpisodeItemVm() { LengthInMinutes = "5", Time = "09:00" });
                Episodes.Add(new EpisodeItemVm() { LengthInMinutes = "3", Time = "15:00" });
                Episodes.Add(new EpisodeItemVm() { LengthInMinutes = "15", Time = "22:00" });
            }
        }

        public async Task Init()
        {

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

            var uri = URI;
            var client = new SyndicationClient();
            var feed = await client.RetrieveFeedAsync(new Uri(uri));

            foreach (var syndicationItem in feed.Items)
            {
                Episodes.Add(new EpisodeItemVm());
            }

        }
        public ObservableCollection<EpisodeItemVm> Episodes { get; set; }

        string example_json = "{\"copyright\":\"Copyright Sveriges Radio 2016. All rights reserved.\",\"episodes\":[{\"id\":704759,\"title\":\"Ekonyheter\",\"description\":\"Här hör du Ekots nyhetssändningar. Senaste nytt - dygnet runt, året om.\",\"url\":\"http://sverigesradio.se/sida/avsnitt/704759?programid=4540\",\"program\":{\"id\":4540,\"name\":\"Ekot\"},\"publishdateutc\":\"\\/Date(1460757600000)\\/\",\"imageurl\":\"http://sverigesradio.se/sida/images/4540/3634468_2048_1152.jpg?preset=api-default-square\",\"imageurltemplate\":\"http://sverigesradio.se/sida/images/4540/3634468_2048_1152.jpg\",\"broadcast\":{\"availablestoputc\":\"\\/Date(1463349720000)\\/\",\"playlist\":{\"duration\":120,\"publishdateutc\":\"\\/Date(1460757600000)\\/\",\"id\":5678882,\"url\":\"http://sverigesradio.se/api/radio/radio.aspx?type=broadcast&id=5678882&codingformat=.m4a&metafile=asx&quality=hi\",\"statkey\":\"/app/avsnitt/nyheter (ekot)[k(83)]/ekot[p(4540)]/ekonyheter[e(704759)]\"},\"broadcastfiles\":[{\"duration\":120,\"publishdateutc\":\"\\/Date(1460757600000)\\/\",\"id\":5678882,\"url\":\"http://sverigesradio.se/topsy/ljudfil/5678882-hi.m4a\",\"statkey\":\"/app/avsnitt/nyheter (ekot)[k(83)]/ekot[p(4540)]/ekonyheter[e(704759)]\"}]}}]}";
    }
}
