using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace KramerUwp.App
{
    public class MainPageVm
    {
        public MainPageVm()
        {
            Episodes = new ObservableCollection<EpisodeItemVm>();
        }
        public ObservableCollection<EpisodeItemVm> Episodes { get; set; }
    }
}
