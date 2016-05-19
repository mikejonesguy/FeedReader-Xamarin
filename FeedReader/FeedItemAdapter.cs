
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Android.Support.V7.Widget;
using RecyclerView = Android.Support.V7.Widget.RecyclerView;

namespace FeedReader
{
	public class FeedItemAdapter : RecyclerView.Adapter
	{
		public List<FeedItem> FeedItems { get; set; }
		public int SelectedPosition { get; set; }
		private IFeedItemInterface clickListener = null;

		public FeedItemAdapter (List<FeedItem> items, IFeedItemInterface listener)
		{
			FeedItems = items;
			clickListener = listener;
			SelectedPosition = -1;
			HasStableIds = true;
		}

		public override RecyclerView.ViewHolder
		OnCreateViewHolder (ViewGroup parent, int viewType)
		{
			View itemView = LayoutInflater.From (parent.Context).Inflate (Resource.Layout.ListItem, parent, false);
			FeedItemViewHolder viewholder = new FeedItemViewHolder (itemView, clickListener);
			return viewholder;
		}

		public override void
		OnBindViewHolder (RecyclerView.ViewHolder holder, int position)
		{
			// populate textviews with data from the FeedItem
			FeedItemViewHolder viewHolder = holder as FeedItemViewHolder;
			FeedItem item = FeedItems.ElementAt (position);
			viewHolder.Item = item;
			viewHolder.Title.Text = item.Title;
			viewHolder.PubDate.Text = TimeAgo.TimeAgoString (item.PubDate);

			// show selected state -- this seems a lot more 
			// complicated than it used to be with ListView
			CardView cardview = viewHolder.ItemView as CardView;
			if (position == SelectedPosition) {
				cardview.SetCardBackgroundColor (FeedItemViewHolder.SelectedColor);
			} else {
				cardview.SetCardBackgroundColor (Color.White);
			}
		}

		public override int ItemCount
		{
			get { return FeedItems.Count; }
		}

		public override long GetItemId (int position)
		{
			FeedItem item = FeedItems.ElementAt (position);
			return item.Title.GetHashCode ();
		}
	}
}

