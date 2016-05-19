
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Support.V4.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace FeedReader
{			
	public class DetailActivity : AppCompatActivity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			SetContentView (Resource.Layout.ActivityDetail);

			// setup toolbar
			var toolbar = FindViewById<Toolbar> (Resource.Id.toolbar);
			SetSupportActionBar (toolbar);
			SupportActionBar.SetHomeButtonEnabled(true);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.Title = Intent.GetStringExtra (DetailFragment.TitleKey) ?? "";

			// savedInstanceState is non-null when there is fragment state
			// saved from previous configurations of this activity
			// (e.g. when rotating the screen from portrait to landscape).
			// In this case, the fragment will automatically be re-added
			// to its container so we don't need to manually add it.
			// For more information, see the Fragments API guide at:
			//
			// http://developer.android.com/guide/components/fragments.html
			if (savedInstanceState == null) {
				// create new fragment
				DetailFragment fragment = new DetailFragment();

				// get HTML content
				Bundle arguments = new Bundle();
				string html = Intent.GetStringExtra (DetailFragment.HtmlKey) ?? "Content not available";
				arguments.PutString (DetailFragment.HtmlKey, html);
				fragment.Arguments = arguments;

				// plug it into the view hierarchy
				Android.Support.V4.App.FragmentTransaction transaction = SupportFragmentManager.BeginTransaction ();
				transaction.Add (Resource.Id.detailContainer, fragment);
				transaction.Commit ();
			}

		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			// handle home-as-up selection
			if (item.ItemId == Android.Resource.Id.Home) {
				NavigateUpTo(new Intent (this, typeof(MainActivity)));
				return true;
			}
			return base.OnOptionsItemSelected (item);
		}
	}
}

