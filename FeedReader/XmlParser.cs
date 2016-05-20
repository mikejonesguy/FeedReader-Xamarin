using Android.OS;
using Android.Net;
using Android.Content;
using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FeedReader
{
	public class XmlParser
	{

		private HttpClient httpClient;
		private Context context;

		public XmlParser (Context context)
		{
			httpClient = new HttpClient ();
			this.context = context.ApplicationContext;
		}

		// main entry method for the fetch/parse task
		public async Task<List<FeedItem>> GetData (String url)
		{
			XmlTextReader reader = null;
			List<FeedItem> res = new List<FeedItem> ();
			string path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.Personal);
			string filename = Path.Combine(path, "cached.xml");
			try
			{
				// skip this if we're offline
				if (isActiveNetwork ()) {
					var response = await httpClient.GetAsync (url);
					if (response.IsSuccessStatusCode) {
						string xml = await response.Content.ReadAsStringAsync ();
						using (var streamWriter = new StreamWriter (filename, false))
						{
							await streamWriter.WriteAsync (xml);
						}
					}
				}
			}
			catch (System.Exception sysExc)
			{
				Console.WriteLine (sysExc.Message);
			}

			// hopefully we have a cached version saved from
			// a previous run -- otherwise return an empty list
			if (!File.Exists (filename)) {
				return res;
			}

			// use a separate try/catch block here so that if the download
			// fails for any reason, we can still try to parse cached data
			try {
				// start parsing the downloaded xml
				reader = new XmlTextReader (filename);
				while (!reader.EOF) {
					reader.Read ();
					// look for <item> tags -- when we find one, we 
					// parse it separately in ReadItem()
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "item") {
						FeedItem item = ReadItem (reader);
						res.Add (item);
					}
				}
			}
			catch (System.Exception sysExc)
			{
				Console.WriteLine (sysExc.Message);
			}
			finally {
				// close the reader so that we don't
				// tie up the cached.xml file
				if (reader != null) {
					reader.Close ();
				}
			}
			
			return res;
		}

		// helper method to read individual items in the feed
		private FeedItem ReadItem(XmlTextReader reader) 
		{
			// create new item
			FeedItem item = new FeedItem ();

			// keep looping through the contents of the <item> element
			// until we hit the closing </item> tag (or the EOF)
			while (!reader.EOF && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "item")) {
				reader.Read ();
				// start of child element -- we only care about
				// title, pubDate, and content:encoded
				if (reader.NodeType == XmlNodeType.Element) {
					if (reader.Name == "title") {
						string title = reader.ReadElementContentAsString ();
						item.Title = title;
					} else if (reader.Name == "pubDate") {
						string dateString = reader.ReadElementContentAsString ();
						DateTime dateValue;
						DateTime.TryParse (dateString, out dateValue);
						item.PubDate = dateValue;
					}  else if (reader.Name == "content:encoded") {
						string html = reader.ReadElementContentAsString ();
						item.HTML = html;
					} 
				}
			}

			return item;
		}

		private bool isActiveNetwork () 
		{
			// check network connectivity
			ConnectivityManager connectivityManager = (ConnectivityManager) context.GetSystemService(Context.ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			return activeConnection != null && activeConnection.IsConnected;
		}
	}
}

