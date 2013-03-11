using System.Diagnostics;
using Kramer.Common.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        }

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
            var vm = new MainVm();
            await vm.Init();
        }

        private void MediaElement_OnCurrentStateChanged(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(string.Format("status: {0}", mediaElement.CurrentState));
        }

        private void itemGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var feedItem = e.ClickedItem as FeedItem;
            if (feedItem == null) return;

            mediaElement.Source = new Uri(feedItem.AudioUri, UriKind.Absolute);
        }
    }
}
