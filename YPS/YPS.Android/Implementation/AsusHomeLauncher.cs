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
    public class AsusHomeLauncher : MyBadge
    {
        private static String INTENT_ACTION = "android.intent.action.BADGE_COUNT_UPDATE";
        private static String INTENT_EXTRA_BADGE_COUNT = "badge_count";
        private static String INTENT_EXTRA_PACKAGENAME = "badge_count_package_name";
        private static String INTENT_EXTRA_ACTIVITY_NAME = "badge_count_class_name";

        public AsusHomeLauncher(Context context) : base(context)
        {
        }

        public override void executeBadge(int badgeCount)
        {
            try
            {
                Intent intent = new Intent(INTENT_ACTION);
                intent.PutExtra(INTENT_EXTRA_BADGE_COUNT, badgeCount);
                intent.PutExtra(INTENT_EXTRA_PACKAGENAME, getContextPackageName());
                intent.PutExtra(INTENT_EXTRA_ACTIVITY_NAME, getEntryActivityName());
                intent.PutExtra("badge_vip_count", 0);
                mContext.SendBroadcast(intent);
            }
            catch (Exception ex)
            {

                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "executeBadge method -> in AsusHomeLauncher.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        public override List<String> getSupportLaunchers()
        {
            try
            {
                List<string> supportedLaunchers = new List<string>();
                supportedLaunchers.Add("com.asus.launcher");
                return supportedLaunchers;
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "getSupportLaunchers method -> in AsusHomeLauncher.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }
    }
}