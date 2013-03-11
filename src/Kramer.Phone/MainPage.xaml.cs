using System;
using System.Linq;
using System.Threading.Tasks;
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

            var feedItem = item as FeedItem;

			BackgroundAudioPlayer.Instance.Track = new AudioTrack(
                new Uri(feedItem.AudioUri, UriKind.Absolute), feedItem.Title, "ekot", "nyheter",
                new Uri("http://sverigesradio.se/img/channellogos/srlogga.png", UriKind.Absolute));

            MainListBox.SelectedIndex = -1;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }
    }

    public class PhoneViewDispatcher : IViewDispatcher
    {
        public Task RunAsync(Action action)
        {
            return Task.Factory.StartNew(() => Deployment.Current.Dispatcher.BeginInvoke(action));
        }
    }
}