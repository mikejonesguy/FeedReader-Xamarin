
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Android.Support.V4.App;

namespace FeedReader
{
	public class DetailFragment : Android.Support.V4.App.Fragment
	{
		public static string HtmlKey = "HTML";
		public static string TitleKey = "TITLE";

		private WebView webView;
		private string html = "Content not available";

		public override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// get html from arguments bundle
			if (Arguments.ContainsKey (DetailFragment.HtmlKey)) {
				html = Arguments.GetString (DetailFragment.HtmlKey);
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View rootView =  inflater.Inflate(Resource.Layout.FragmentDetail, container, false);

			// add a little CSS styling to make the content look nice (images and videos fit width)
			html = "<html><head><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/>" +
				"<style>body { margin: 0px; padding: 2%; } img { width:100%; } " +
				"img[width] { width:inherit; } div.video_iframe iframe { width:100%; height:56vw; margin: auto; }" +
				"</style></head><body>\n" + html + "\n</body></html>";

			webView = rootView.FindViewById<WebView> (Resource.Id.webView);
			WebSettings settings = webView.Settings;

			// for mobile-friendly viewing
			settings.LoadWithOverviewMode = true;
			settings.UseWideViewPort = true;

			// for videos
			settings.JavaScriptEnabled = true;

			// load the html content
			webView.LoadData (html, "text/html; charset=UTF-8", "UTF-8");

			return rootView;
		}
	}
}

