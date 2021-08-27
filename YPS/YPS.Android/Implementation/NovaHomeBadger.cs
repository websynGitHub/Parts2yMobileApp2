using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;

namespace YPS.Droid.Implementation
{
	public class NovaHomeBadger : MyBadge
	{
		private static String CONTENT_URI = "content://com.teslacoilsw.notifier/unread_count";
		private static String COUNT = "count";
		private static String TAG = "tag";

		public NovaHomeBadger (Context context) : base(context)
		{
		}

		public override void executeBadge(int badgeCount)
		{
			try {
				ContentValues contentValues = new ContentValues();
				contentValues.Put(TAG, getContextPackageName() + "/" + getEntryActivityName());
				contentValues.Put(COUNT, badgeCount);
				mContext.ContentResolver.Insert(Android.Net.Uri.Parse(CONTENT_URI),contentValues);
			}  catch (Exception ex) {

			}
		}

		public override List<String>  getSupportLaunchers() {
			List<string> supportedLaunchers = new List<string> ();
			supportedLaunchers.Add ("com.teslacoilsw.launcher");
			return supportedLaunchers;
		}
	}
}

