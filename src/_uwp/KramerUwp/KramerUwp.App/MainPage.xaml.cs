using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media.Playback;
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
            var player = BackgroundMediaPlayer.Current;
            player.AutoPlay = true;
            player.CurrentStateChanged += Player_CurrentStateChanged;
            player.BufferingEnded += Player_BufferingEnded;
            player.BufferingStarted += Player_BufferingStarted;
        }

        private void Player_BufferingStarted(MediaPlayer sender, object args)
        {
            Debug.WriteLine("buffering started");
            NowPlayingStatusText.Text = "buffering...";
        }

        private void Player_BufferingEnded(MediaPlayer sender, object args)
        {
            Debug.WriteLine("buffering ended");
            NowPlayingStatusText.Text = "";
        }

        private void Player_CurrentStateChanged(MediaPlayer sender, object args)
        {
            string status;
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Closed:
                    break;
                case MediaPlayerState.Opening:
                    break;
                case MediaPlayerState.Buffering:
                    break;
                case MediaPlayerState.Playing:
                    break;
                case MediaPlayerState.Paused:
                    break;
                case MediaPlayerState.Stopped:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            NowPlayingStatusText.Text = status;
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
