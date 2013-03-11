using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Kramer.Common.ViewModels;
using Microsoft.Phone.BackgroundAudio;
using Microsoft.Phone.Controls;

namespace Kramer.Phone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainVm _vm;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            this.Loaded += MainPage_Loaded;
            _vm = new MainVm(new PhoneViewDispatcher());
            _vm.PropertyChanged += _vm_PropertyChanged;
            DataContext = _vm;

            BackgroundAudioPlayer.Instance.PlayStateChanged += Instance_PlayStateChanged;
        }

        void Instance_PlayStateChanged(object sender, EventArgs e)
        {
            var playerState = BackgroundAudioPlayer.Instance.PlayerState;
            Debug.WriteLine("{0:HH:mm:ss,fff} state: {1}", DateTime.Now, playerState);
            switch (playerState)
            {

                case PlayState.Unknown:
                    break;
                case PlayState.Paused:
                    break;
                case PlayState.Playing:
                case PlayState.Shutdown:
                case PlayState.Error:
                case PlayState.Stopped:
                    _vm.ClearBusy();
                    break;
                case PlayState.BufferingStarted:
                    break;
                case PlayState.BufferingStopped:
                    break;
                case PlayState.TrackReady:
                    break;
                case PlayState.TrackEnded:
                    break;
                case PlayState.Rewinding:
                    break;
                case PlayState.FastForwarding:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void _vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Items")
            {

            }
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await _vm.Init();
        }

        // Handle selection changed on ListBox
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
                return;

            var item = e.AddedItems[0];
            if (item == null || !(item is FeedItem)) return;

            MainListBox.SelectedIndex = -1;

            var feedItem = item as FeedItem;

            _vm.SetBusy(BusyMode.StartingPlay);

            BackgroundAudioPlayer.Instance.Track = new AudioTrack(
                new Uri(feedItem.AudioUri, UriKind.Absolute),
                string.Format("{0} {1}", feedItem.Author, feedItem.Title), feedItem.Content, feedItem.Author,
                null, null, EnabledPlayerControls.Pause);
            BackgroundAudioPlayer.Instance.Play();

        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}