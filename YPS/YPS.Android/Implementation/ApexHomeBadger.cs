using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
namespace YPS.Droid.Implementation
{
	public class ApexHomeBadger : MyBadge
	{
		private static String INTENT_UPDATE_COUNTER = "com.anddoes.launcher.COUNTER_CHANGED";
		private static String PACKAGENAME = "package";
		private static String COUNT = "count";
		private static String CLASS = "class";

		public ApexHomeBadger (Context context) : base(context)
		{
		}

		public override void executeBadge(int badgeCount)
		{
			Intent intent = new Intent(INTENT_UPDATE_COUNTER);
			intent.PutExtra(PACKAGENAME, getContextPackageName());
			intent.PutExtra(COUNT, badgeCount);
			intent.PutExtra(CLASS, getEntryActivityName());
			mContext.SendBroadcast(intent);
		}

		public override List<String>  getSupportLaunchers() {
			List<string> supportedLaunchers = new List<string> ();
			supportedLaunchers.Add ("com.anddoes.launcher");
			return supportedLaunchers;
		}
	}
}