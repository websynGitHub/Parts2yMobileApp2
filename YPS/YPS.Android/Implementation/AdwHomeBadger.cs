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
    public class AdwHomeBadger : MyBadge
    {
        public static String INTENT_UPDATE_COUNTER = "org.adw.launcher.counter.SEND";
        public static String PACKAGENAME = "PNAME";
        public static String COUNT = "COUNT";

        public AdwHomeBadger(Context context) : base(context)
        {
        }

        public override void executeBadge(int badgeCount)
        {
            try
            {
                Intent intent = new Intent(INTENT_UPDATE_COUNTER);
                intent.PutExtra(PACKAGENAME, getContextPackageName());
                intent.PutExtra(COUNT, badgeCount);
                mContext.SendBroadcast(intent);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "executeBadge method -> in AdwHomeBadger.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        public override List<String> getSupportLaunchers()
        {
            try
            {
                List<string> supportedLaunchers = new List<string>();
                supportedLaunchers.Add("org.adw.launcher");
                supportedLaunchers.Add("org.adwfreak.launcher");
                return supportedLaunchers;
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "getSupportLaunchers method -> in AdwHomeBadger.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }
    }
}

