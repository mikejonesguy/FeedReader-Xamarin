
using System;

using Android.Widget;
using Android.Views;
using Android.Graphics;
using Android.Support.V7.Widget;
using RecyclerView = Android.Support.V7.Widget.RecyclerView;

namespace FeedReader
{
	public interface IFeedItemInterface
	{
		void ItemClicked(FeedItem item, int position);
	}

	public class FeedItemViewHolder : RecyclerView.ViewHolder
	{
		// color for rows that are selected in two-pane mode
		public static Color SelectedColor = Color.ParseColor("#ededed");

		public FeedItem Item { get; set; }
		public TextView Title { get; private set; }
		public TextView PubDate { get; private set; }
		private IFeedItemInterface clickListener;

		public FeedItemViewHolder (View itemView, IFeedItemInterface listener) : base (itemView)
		{
			// Locate and cache view references:
			Title = itemView.FindViewById<TextView> (Resource.Id.titleView);
			PubDate = itemView.FindViewById<TextView> (Resource.Id.pubDateView);

			// setup click listener -> MainActivity.ItemClicked()
			clickListener = listener;
			itemView.Clickable = true;
			itemView.Click += delegate {
				CardView cardview = itemView as CardView;
				// set the color here in addition to setting it in the adapter
				// to avoid flicker
				cardview.SetCardBackgroundColor (FeedItemViewHolder.SelectedColor);
				clickListener.ItemClicked (Item, this.AdapterPosition);
			};
		}
	}
}

