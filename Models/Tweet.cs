using System;
using System.Collections.Generic;

namespace startupTweetMiner.Models
{
    public struct Tweet
    {
        public string Handle { get; set; }

        public string Location { get; set; }

        public string AuthorId { get; set; }

        public string UserName { get; set; }

        public string TweetId { get; set; }

        public string Text { get; set; }

        public string CreatedAt { get; set; }

        public string Language { get; set; }

        public List<Url> MediaData { get; set; }

        public string MediaDataUrl { get; set; }

        public string MediaTitle { get; set; }

        public string MediaDescription { get; set; }

        public string ReferencedTweetId { get; set; }

        public string ReferencedTweetType { get; set; }

        public List<ReferencedTweet> ReferencedTweets { get; set; }

        public int RetweetCount { get; set; }

        public int ReplyCount { get; set; }

        public int LikeCount { get; set; }

        public int QuoteCount { get; set; }
    }

    public struct Entity
    {
        public string Handle { get; set; }

        public string Location { get; set; }

        public string AuthorId { get; set; }
    }

    public struct ReferencedTweet
    {
        public string Id { get; set; }

        public string Type { get; set; }
    }

    public struct Url
    {
        public string url { get; set; }
        public string title { get; set; }
        public string description { get; set; }
    }
}
