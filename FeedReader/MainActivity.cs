
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Android.OS;
using Android.App;
using Android.Net;
using Android.Content;
using Android.Content.Res;
using Android.Views;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using RecyclerView = Android.Support.V7.Widget.RecyclerView;

namespace FeedReader
{
	[Activity (Label = "Feed Reader", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : AppCompatActivity, IFeedItemInterface, View.IOnLayoutChangeListener
	{
		private static string FeedUrl = "http://feeds.feedburner.com/androidcentral?format=xml";
		private RecyclerView recyclerView;
		private FeedItemAdapter adapter;
		private SwipeRefreshLayout swipeContainer;
		private XmlParser parser;
		private bool isTwoPane = false;
		private bool didAutoSelect = false;
		private FeedItem selectedItem = null;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.ActivityMain);

			// setup toolbar
			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);
			SetSupportActionBar (toolbar);
			SupportActionBar.Title = "Android Central News Feed";

			// setup recycler view
			recyclerView = FindViewById<RecyclerView> (Resource.Id.recyclerView);
			recyclerView.SetLayoutManager (new LinearLayoutManager (this));

			// need this to know when we can auto-select the first row in
			// two-pane mode
			recyclerView.AddOnLayoutChangeListener(this);

			// setup swipe-to-refresh
			swipeContainer = FindViewById<SwipeRefreshLayout> (Resource.Id.swipeContainer);
			swipeContainer.Refresh += delegate {
				GetFeedData ();
			};

			// if the detailContainer is in our layout, then we're in two-pane mode
			if (FindViewById (Resource.Id.detailContainer) != null) {
				isTwoPane = true;
			}

			// init the parser
			parser = new XmlParser (this);
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// clear selection on return from detail
			if (adapter != null && !isTwoPane) {
				adapter.NotifyDataSetChanged ();
			}

			// refresh data feed on resume
			GetFeedData ();
		}

		public void OnLayoutChange (View view, Int32 left, Int32 top, Int32 right, Int32 bottom, Int32 oldLeft, Int32 oldTop, Int32 oldRight, Int32 oldBottom)
		{
			// if we do this too early, it will crash, so we use this
			// listener to know when it's safe
			AutoSelectItem0 ();
		}

		public void ItemClicked(FeedItem item, int position)
		{
			if (position < 0) { // fix for weird bug where position comes as -1?
				position = adapter.FeedItems.IndexOf (item);
			}
			if (isTwoPane) {
				// short circuit to avoid unecessarily updating the detail 
				// fragment with the same content.
				if (item == selectedItem) {
					return;
				}
				selectedItem = item;

				// show selection state -- update the adapter so it 
				// can use a special background for the selected item
				adapter.SelectedPosition = position;
				adapter.NotifyDataSetChanged ();

				// replace the fragment
				DetailFragment fragment = new DetailFragment();
				Bundle arguments = new Bundle();
				arguments.PutString (DetailFragment.TitleKey, item.Title);
				arguments.PutString (DetailFragment.HtmlKey, item.HTML);
				fragment.Arguments = arguments;
				Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction ();
				transaction.Replace (Resource.Id.detailContainer, fragment);
				transaction.Commit ();
			} else {
				// launch the detail activity
				Intent detailIntent = new Intent (this, typeof(DetailActivity));
				detailIntent.PutExtra (DetailFragment.TitleKey, item.Title);
				detailIntent.PutExtra (DetailFragment.HtmlKey, item.HTML);
				StartActivity (detailIntent);
			}
		}

		private async void GetFeedData ()
		{
			// kick off the xml fetch/parse task
			var task = parser.GetData (FeedUrl);
			await task;
			List<FeedItem> results = task.Result;
			Console.WriteLine ("Feed fetch/parse complete");

			// stop refresh animation
			swipeContainer.Refreshing = false;

			// only create a new adapter if it doesn't already exist,
			// otherwise, just update the existing adapter
			if (adapter == null) {
				adapter = new FeedItemAdapter (results, this);
				recyclerView.SetAdapter (adapter);
			} else {
				adapter.FeedItems = results;
				adapter.NotifyDataSetChanged ();
			}
		}

		private void AutoSelectItem0 ()
		{
			if (isTwoPane && !didAutoSelect && adapter != null) {
				// remove layout changed listener now that we're done with it
				recyclerView.RemoveOnLayoutChangeListener (this);

				// set flag to make sure that we don't call this more than once
				didAutoSelect = true;

				// select the first row in the list to show in the detail pane
				recyclerView.FindViewHolderForAdapterPosition (0).ItemView.PerformClick ();
			}
		}
	}
}


