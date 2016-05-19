using System;

namespace FeedReader
{
	public class FeedItem
	{
		public string Title { get; set; }
		public DateTime PubDate { get; set; }
		public string HTML { get; set; }

		public FeedItem ()
		{
		}
	}
}

