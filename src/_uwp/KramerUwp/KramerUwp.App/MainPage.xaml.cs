using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
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
            player.MediaOpened += Player_MediaOpened;
            player.BufferingEnded += Player_BufferingEnded;
            player.BufferingStarted += Player_BufferingStarted;
        }

        private async void Player_MediaOpened(MediaPlayer sender, object args)
        {
            try
            {
                string title = null;
                await OnDispatcher(() => title = NowPlayingText?.Text);

                if (string.IsNullOrWhiteSpace(title))
                    return;

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }


        private async void Player_BufferingStarted(MediaPlayer sender, object args)
        {
            Debug.WriteLine("buffering started");
            await OnDispatcher(() => NowPlayingStatusText.Text = "buffering...");
        }

        private async void Player_BufferingEnded(MediaPlayer sender, object args)
        {
            Debug.WriteLine("buffering ended");
            await OnDispatcher(() => NowPlayingStatusText.Text = "");
        }

        private async void Player_CurrentStateChanged(MediaPlayer sender, object args)
        {
            Debug.WriteLine($"Player_CurrentStateChanged: {sender.CurrentState}");
            string status = string.Empty;
            switch (sender.CurrentState)
            {
                case MediaPlayerState.Closed:
                    break;
                case MediaPlayerState.Opening:
                    status = "opening...";
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

            await OnDispatcher(() => NowPlayingStatusText.Text = status);
        }

        private async Task OnDispatcher(Action action)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, action.Invoke);
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
                ApplicationData.Current.LocalSettings.Values["lastError"] = exception;
                ShowError("Error getting episodes, please try again.");

                Debug.WriteLine(exception);
                //await new MessageDialog(exception.StackTrace).ShowAsync();
            }
        }

        private void _list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_list.SelectedIndex == -1)
                    return;
                var episodeItemVm = e.AddedItems.FirstOrDefault() as EpisodeItemVm;
                _list.SelectedIndex = -1;

                if (episodeItemVm == null)
                    return;
                _vm.Play(episodeItemVm);
                var title = episodeItemVm.Title;

                NowPlayingText.Text = title;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
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

        private async void TextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var lastError = ApplicationData.Current.LocalSettings.Values["lastError"] as string;
            if (string.IsNullOrEmpty(lastError))
                return;

            await new MessageDialog(lastError, "Last swallowed error").ShowAsync();
            //var textBlock = new TextBlock();
            //var scroller = new ScrollViewer();
            //scroller.Content = textBlock;
            //textBlock.Text = lastError;
            //Grid.SetRowSpan(scroller, 5);

            //_grid.Children.Add(textBlock);
        }
    }
}
