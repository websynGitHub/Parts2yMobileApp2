using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using YPS.Helpers;
using YPS.Service;

namespace YPS.Droid.Implementation
{
    public class SonyHomeBadger : MyBadge
    {
        private static String INTENT_ACTION = "com.sonyericsson.home.action.UPDATE_BADGE";
        private static String INTENT_EXTRA_PACKAGE_NAME = "com.sonyericsson.home.intent.extra.badge.PACKAGE_NAME";
        private static String INTENT_EXTRA_ACTIVITY_NAME = "com.sonyericsson.home.intent.extra.badge.ACTIVITY_NAME";
        private static String INTENT_EXTRA_MESSAGE = "com.sonyericsson.home.intent.extra.badge.MESSAGE";
        private static String INTENT_EXTRA_SHOW_MESSAGE = "com.sonyericsson.home.intent.extra.badge.SHOW_MESSAGE";

        public SonyHomeBadger(Context context) : base(context)
        {
        }

        public override void executeBadge(int badgeCount)
        {
            try
            {
                Intent intent = new Intent(INTENT_ACTION);
                intent.PutExtra(INTENT_EXTRA_PACKAGE_NAME, getContextPackageName());
                intent.PutExtra(INTENT_EXTRA_ACTIVITY_NAME, getEntryActivityName());
                intent.PutExtra(INTENT_EXTRA_MESSAGE, badgeCount.ToString());
                intent.PutExtra(INTENT_EXTRA_SHOW_MESSAGE, badgeCount > 0);
                mContext.SendBroadcast(intent);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "executeBadge method -> in SonyHomeBadger.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        public override List<String> getSupportLaunchers()
        {
            try
            {
                List<string> supportedLaunchers = new List<string>();
                supportedLaunchers.Add("com.sonyericsson.home");
                return supportedLaunchers;
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "getSupportLaunchers method -> in SonyHomeBadger.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }
    }
}