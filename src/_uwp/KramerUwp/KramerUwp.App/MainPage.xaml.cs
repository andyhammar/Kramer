using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
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
            ClearBusy();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await TryGetEpisodesAsync();
        }

        private async Task TryGetEpisodesAsync()
        {
            try
            {
                ShowBusy("Getting episodes...");
                await _vm.InitAsync();
                ClearBusy();
            }
            catch (Exception exception)
            {
                ShowError("Error getting episodes, please try again.");
                Debug.WriteLine(exception);
            }
        }

        private async void _list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await _vm.PlayAsync(e.AddedItems.FirstOrDefault() as EpisodeItemVm);
        }

        private void ShowBusy(string text)
        {
            ProgressRing.IsActive = true;
            StatusText.Text = text;
        }

        private void ClearBusy()
        {
            StatusText.Text = string.Empty;
            ProgressRing.IsActive = false;
        }
        private void ShowError(string text)
        {
            ClearBusy();
            StatusText.Text = text;
        }

        private async void RefreshButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await TryGetEpisodesAsync();
        }
    }
}
