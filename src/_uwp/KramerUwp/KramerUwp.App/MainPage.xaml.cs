﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Core;
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
            player.SystemMediaTransportControls.IsEnabled = true;
            player.SystemMediaTransportControls.IsPauseEnabled = true;
            player.SystemMediaTransportControls.IsPlayEnabled = true;
            player.SystemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
        }

        private async void Player_MediaOpened(MediaPlayer sender, object args)
        {
            string title = null;
            await OnDispatcher(() => title = NowPlayingText?.Text);

            if (string.IsNullOrWhiteSpace(title))
                return;

            var updater = BackgroundMediaPlayer.Current.SystemMediaTransportControls.DisplayUpdater;
            updater.Type = MediaPlaybackType.Music;
            var properties = updater.MusicProperties;
            properties.Title = title;
            updater.Update();
        }

        private void SystemMediaTransportControls_ButtonPressed(
            Windows.Media.SystemMediaTransportControls sender,
            Windows.Media.SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            if (args.Button == SystemMediaTransportControlsButton.Pause)
            {
                BackgroundMediaPlayer.Current.Pause();
            }
            else if (args.Button == SystemMediaTransportControlsButton.Play)
            {
                BackgroundMediaPlayer.Current.Play();
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
                ShowError("Error getting episodes, please try again.");
                Debug.WriteLine(exception);
            }
        }

        private void _list_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
