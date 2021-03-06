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
    public class NewHtcHomeBadger : MyBadge
    {
        public static String INTENT_UPDATE_SHORTCUT = "com.htc.launcher.action.UPDATE_SHORTCUT";
        public static String INTENT_SET_NOTIFICATION = "com.htc.launcher.action.SET_NOTIFICATION";
        public static String PACKAGENAME = "packagename";
        public static String COUNT = "count";
        public static String EXTRA_COMPONENT = "com.htc.launcher.extra.COMPONENT";
        public static String EXTRA_COUNT = "com.htc.launcher.extra.COUNT";

        public NewHtcHomeBadger(Context context) : base(context)
        {
        }

        public override void executeBadge(int badgeCount)
        {
            try
            {
                Intent intent1 = new Intent(INTENT_SET_NOTIFICATION);
                ComponentName localComponentName = new ComponentName(getContextPackageName(), getEntryActivityName());
                intent1.PutExtra(EXTRA_COMPONENT, localComponentName.FlattenToShortString());
                intent1.PutExtra(EXTRA_COUNT, badgeCount);
                mContext.SendBroadcast(intent1);

                Intent intent = new Intent(INTENT_UPDATE_SHORTCUT);
                intent.PutExtra(PACKAGENAME, getContextPackageName());
                intent.PutExtra(COUNT, badgeCount);
                mContext.SendBroadcast(intent);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "executeBadge method -> in NewHtcHomeBadger.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        public override List<String> getSupportLaunchers()
        {
            try
            {
                List<string> supportedLaunchers = new List<string>();
                supportedLaunchers.Add("com.htc.launcher");
                return supportedLaunchers;
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "getSupportLaunchers method -> in NewHtcHomeBadger.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }
    }
}

