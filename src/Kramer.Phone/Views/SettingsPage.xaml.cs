using Kramer.Common.Settings;
using Kramer.Common.ViewModels;
using Microsoft.Phone.Controls;

namespace Kramer.Phone.Views
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        private SettingsPageVm _vm;

        // Constructor
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = _vm = new SettingsPageVm(new AppSettings());
        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

    }
}
 