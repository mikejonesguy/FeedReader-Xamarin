
using System;

namespace FeedReader
{
	// simple class to give friendly timestamps to the feed items
	// in the list view -- there are fancier versions of this that
	// use localized strings, but this is sufficient for the demo
	public class TimeAgo
	{
		public TimeAgo ()
		{
			
		}

		public static string TimeAgoString (DateTime dateTime)
		{
			DateTime now = DateTime.Now;
			TimeSpan span = now.Subtract (dateTime);
			if (span.Days >= 7) {
				return dateTime.ToString ("M/d/yyyy");
			}

			if (span.Days > 0) {
				if (span.Days < 2) {
					return "Yesterday";
				} else {
					return span.Days.ToString () + " days ago";
				}
			}

			if (span.Hours > 0) {
				if (span.Hours < 2) {
					return "1 hour ago";
				} else {
					return span.Hours.ToString () + " hours ago";
				}
			}

			if (span.Minutes > 0) {
				return span.Minutes.ToString () + " min ago";
			}

			return "Moments ago";
		}
	}
}

