using System.Diagnostics;
using Kramer.Common;
using Kramer.Common.ViewModels;
using System;
using System.Collections.Generic;
using Windows.Media;
using Windows.UI.ApplicationSettings;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace Kramer
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class GroupedItemsPage : Kramer.Common.LayoutAwarePage
    {
        public GroupedItemsPage()
        {
            this.InitializeComponent();
            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            MediaControl.StopPressed += MediaControl_StopPressed;
            MediaControl.SoundLevelChanged += MediaControl_SoundLevelChanged;

            mediaElement.CurrentStateChanged += MediaElement_CurrentStateChanged;
            mediaElement.MediaFailed += mediaElement_MediaFailed;

                        SettingsPane.GetForCurrentView().CommandsRequested += GroupedItemsPage_CommandsRequested;

        }


        void GroupedItemsPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            SettingsHelper.AddSettingsCommands(args);
        }
        

        #region audio magic

        private void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            HandlePlayPauseToggle();
        }

        public void HandlePlayPauseToggle()
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (mediaElement.CurrentState ==
                        MediaElementState.Stopped ||
                        mediaElement.CurrentState == MediaElementState.Paused ||
                        mediaElement.CurrentState == MediaElementState.Closed)
                    {
                        StartMedia();
                    }
                    else
                    {
                        StopMedia();
                    }
                });

        }

        private void MediaControl_SoundLevelChanged(object sender, object e)
        {
            var soundLevelState = MediaControl.SoundLevel;

            var shouldStartPlayingNow = GetShouldStartPlayingNow(soundLevelState);

            Debug.WriteLine(string.Format("previous soundLevel:    {0}", _lastSoundLevelState));
            Debug.WriteLine(string.Format("was previously playing: {0}", _lastMediaStateUponSoundLevelChangeWasPlaying));
            Debug.WriteLine(string.Format("new soundlevel:         {0}", soundLevelState));
            Debug.WriteLine(string.Format("shouldPlayNow:          {0}", shouldStartPlayingNow));

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    UpdateLastMediaStates(soundLevelState);

                    if (soundLevelState == SoundLevel.Muted)
                    {
                        StopMedia();
                    }
                    else if (shouldStartPlayingNow)
                    {
                        StartMedia();
                    }
                });
        }

        private bool GetShouldStartPlayingNow(SoundLevel soundLevelState)
        {
            return soundLevelState == SoundLevel.Full &&
                   _lastSoundLevelState == SoundLevel.Muted &&
                   _lastMediaStateUponSoundLevelChangeWasPlaying;
        }

        public void StartMedia()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_audioUri)) return;

                mediaElement.Source = new Uri(_audioUri);
                mediaElement.Play();
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.ToString());
                throw;
            }
        }

        public void StopMedia()
        {
            mediaElement.Stop();
        }

        private void UpdateLastMediaStates(SoundLevel soundLevelState)
        {
            if (_lastSoundLevelState == SoundLevel.Full)
                _lastMediaStateUponSoundLevelChangeWasPlaying = mediaElement.CurrentState.IsAPlayingState();
            _lastSoundLevelState = soundLevelState;
        }

        private void MediaControl_StopPressed(object sender, object e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, StopMedia);
        }

        private void MediaControl_PlayPressed(object sender, object e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, StartMedia);
        }

        private void MediaControl_PausePressed(object sender, object e)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, StopMedia);
        }

        private void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            var mediaElement = sender as MediaElement;
            if (mediaElement == null)
                return;
            if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                //StartFetchCurrentSongTimer();
                //StartFetchScheduleTimer();
            }
            else if (mediaElement.CurrentState == MediaElementState.Paused ||
                     mediaElement.CurrentState == MediaElementState.Stopped ||
                     mediaElement.CurrentState == MediaElementState.Closed)
            {
                //StopFetchCurrentSongTimer();
                //StopFetchScheduleTimer();
                //ResetNowPlayingInfo();
            }
        }

        private void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ShowNetworkErrorMessage();
        }

        private void ShowNetworkErrorMessage()
        {
            new MessageDialog(
                "Kunde inte spela ljudströmmen, vänligen kontrollera att du är nätansluten och försök igen.").ShowAsync();
            //ToastHelper.ShowToast(
            //    AppRes.Get("NoNetworkWarningTitle"),
            //    AppRes.Get("NoNetworkWarningText"));
        }

        private bool _lastMediaStateUponSoundLevelChangeWasPlaying;
        private SoundLevel _lastSoundLevelState;
        private string _audioUri;
        private MainVm _vm;

        #endregion

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _vm = new MainVm(new Win8ViewDispatcher());
            _vm.PropertyChanged += vm_PropertyChanged;
            DataContext = _vm;
            await _vm.Init();


        }

        void vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ErrorText")
            {
                new MessageDialog(_vm.ErrorText).ShowAsync();
            }
        }

        private void MediaElement_OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(string.Format("status: {0}", mediaElement.CurrentState));
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var feedItem = e.ClickedItem as FeedItem;
            if (feedItem == null) return;

            string uriString = feedItem.AudioUri;
            _audioUri = uriString;
            mediaElement.Source = new Uri(uriString, UriKind.Absolute);
        }
    }

    public static class MediaElementStateExtensions
    {
        public static bool IsAPlayingState(this MediaElementState state)
        {
            return state == MediaElementState.Opening
                || state == MediaElementState.Buffering
                || state == MediaElementState.Playing;
        }
    }
}
