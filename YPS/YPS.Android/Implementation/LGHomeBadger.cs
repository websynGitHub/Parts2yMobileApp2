using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;

namespace YPS.Droid.Implementation
{
	public class LGHomeBadger : MyBadge
	{
		private static String INTENT_ACTION = "android.intent.action.BADGE_COUNT_UPDATE";
		private static String INTENT_EXTRA_BADGE_COUNT = "badge_count";
		private static String INTENT_EXTRA_PACKAGENAME = "badge_count_package_name";
		private static String INTENT_EXTRA_ACTIVITY_NAME = "badge_count_class_name";

		public LGHomeBadger (Context context) : base(context)
		{
		}

		public override void executeBadge(int badgeCount)
		{
			Intent intent = new Intent(INTENT_ACTION);
			intent.PutExtra(INTENT_EXTRA_BADGE_COUNT, badgeCount);
			intent.PutExtra(INTENT_EXTRA_PACKAGENAME, getContextPackageName());
			intent.PutExtra(INTENT_EXTRA_ACTIVITY_NAME, getEntryActivityName());
			mContext.SendBroadcast(intent);
		}

		public override List<String>  getSupportLaunchers() {
			List<string> supportedLaunchers = new List<string> ();
			supportedLaunchers.Add ("com.lge.launcher");
			supportedLaunchers.Add ("com.lge.launcher2");
			return supportedLaunchers;
		}
	}
}
