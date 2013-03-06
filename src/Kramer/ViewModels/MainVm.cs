using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Kramer.Common;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.Web.Syndication;

namespace Kramer.ViewModels
{
    public class MainVm : BindableBase
    {
        public MainVm()
        {
            Items = new ObservableCollection<FeedItem>();
        }

        public async Task Init()
        {
            await GetFeedAsync("http://feeds.reuters.com/reuters/topNews");
        }

        public ObservableCollection<FeedItem> Items { get; set; }

        private async Task GetFeedAsync(string feedUriString)
        {
            // using Windows.Web.Syndication;
            var client = new SyndicationClient();
            var feedUri = new Uri(feedUriString);

            try
            {
                SyndicationFeed feed = await client.RetrieveFeedAsync(feedUri);

                foreach (SyndicationItem item in feed.Items)
                {
                    FeedItem feedItem = new FeedItem();
                    feedItem.Title = item.Title.Text;
                    feedItem.PubDate = item.PublishedDate.DateTime;
                    //feedItem.Author = item.Authors[0].Name.ToString();
                    if (feed.SourceFormat == SyndicationFormat.Atom10)
                    {
                        feedItem.Content = item.Content.Text;
                    }
                    else if (feed.SourceFormat == SyndicationFormat.Rss20)
                    {
                        feedItem.Content = item.Summary.Text;
                    }
                    CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                                                         () => Items.Add(feedItem));

                }
            }
            catch (Exception ex)
            {
                CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, ShowError(ex));

            }
        }

        private DispatchedHandler ShowError(Exception exception)
        {
            return () => ShowError(exception.Message);
        }

        private void ShowError(string message)
        {
            var dlg = new MessageDialog(message);
            dlg.ShowAsync();
        }
    }
}