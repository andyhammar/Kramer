using System;

namespace Kramer.Common.ViewModels
{
    public class FeedItem
    {
        public string Title { get; set; }

        public DateTime PubDate { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }
    }
}