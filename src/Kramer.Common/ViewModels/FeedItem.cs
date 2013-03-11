using System;

namespace Kramer.Common.ViewModels
{
    public class FeedItem
    {
        public string Title { get; set; }

        public DateTime Date { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }

        public string AudioUri { get; set; }

        public string Duration { get; set; }
    }
}