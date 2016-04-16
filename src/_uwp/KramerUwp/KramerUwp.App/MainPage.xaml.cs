using System.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace KramerUwp.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

         
    }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await _vm.InitAsync();
        }

        private async void _list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await _vm.PlayAsync(e.AddedItems.FirstOrDefault() as EpisodeItemVm);
        }
    }
}
