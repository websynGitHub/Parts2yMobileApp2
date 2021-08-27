using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
namespace YPS.Droid.Implementation
{
	public class SolidHomeBadger : MyBadge
	{
		private static String INTENT_UPDATE_COUNTER = "com.majeur.launcher.intent.action.UPDATE_BADGE";
		private static String PACKAGENAME = "com.majeur.launcher.intent.extra.BADGE_PACKAGE";
		private static String COUNT = "com.majeur.launcher.intent.extra.BADGE_COUNT";
		private static String CLASS = "com.majeur.launcher.intent.extra.BADGE_CLASS";

		public SolidHomeBadger (Context context) : base(context)
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
			supportedLaunchers.Add ("com.majeur.launcher");
			return supportedLaunchers;
		}
	}
}

