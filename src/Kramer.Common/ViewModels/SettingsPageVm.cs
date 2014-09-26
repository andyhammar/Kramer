using System.Collections.Generic;
using System.Threading.Tasks;
using Kramer.Common.Settings;

namespace Kramer.Common.ViewModels
{
    public class SettingsPageVm : BindableBase
    {
        private readonly IAppSettings _appSettings;

        public SettingsPageVm(IAppSettings appSettings)
        {
            _appSettings = appSettings;

            InitAvailableDropdowns();
        }

        private void InitAvailableDropdowns()
        {
            var episodeLengths = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                episodeLengths.Add(string.Format("{0} minuter", i));
            }
            EpisodeLengths = episodeLengths;

            var episodeSearchWindowAges = new List<string>();
            for (int i = 1; i < 24; i++)
            {
                episodeSearchWindowAges.Add(string.Format("{0} {1}", i, i == 1 ? "timme" : "timmar"));
            }
            EpisodeSearchWindowAges = episodeSearchWindowAges;
        }

        public async Task Init()
        {
        }

        public IEnumerable<string> EpisodeLengths{ get; set; }
        public IEnumerable<string> EpisodeSearchWindowAges{ get; set; }

    }
}