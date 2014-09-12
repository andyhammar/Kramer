using System;
using System.Collections.ObjectModel;
using Kramer.Common.ViewModels;

namespace Kramer.Common.Design
{
    public class MainVmDesign : MainVm
    {
        public MainVmDesign() : base(null, null, null)
        {
            Items = new ObservableCollection<FeedItem>(new[]
                {
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464105-hi.m4a", 
                            Title = "22:00",
                            Author = "Ekot",
                            Content = "Ekonyheter",
                            Date = DateTime.Now,
                            Duration = "5:12"
                        },
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464093-hi.m4a", 
                            Title = "21:00",
                            Duration = "12:00",
                            Content = "Dagens eko"
                        },
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464093-hi.m4a", 
                            Title = "21:00",
                            Duration = "12:00",
                            Content = "Dagens eko"
                        },
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464093-hi.m4a", 
                            Title = "21:00",
                            Duration = "12:00",
                            Content = "Dagens eko"
                        },
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464093-hi.m4a", 
                            Title = "21:00",
                            Duration = "12:00",
                            Content = "Dagens eko"
                        },
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464093-hi.m4a", 
                            Title = "21:00",
                            Duration = "12:00",
                            Content = "Dagens eko"
                        },
                    new FeedItem
                        {
                            AudioUri = "http://sverigesradio.se/topsy/ljudfil/4464093-hi.m4a", 
                            Title = "21:00",
                            Duration = "12:00",
                            Content = "Dagens eko"
                        }
                });
            StatusText = "playing...";
            IsBusy = true;
        }

    }
}