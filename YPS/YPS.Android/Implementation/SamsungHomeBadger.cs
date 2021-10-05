using System;
using System.Collections.Generic;

using Android.Content;
using Android.Database;
using Android.Net;
using YPS.Helpers;
using YPS.Service;

namespace YPS.Droid.Implementation
{
    public class SamsungHomeBadger : MyBadge
    {
        private static String INTENT_ACTION = "content://com.sec.badge/apps";
        private static String INTENT_EXTRA_BADGE_COUNT = "badgecount";
        private static String INTENT_EXTRA_PACKAGENAME = "package";
        private static String INTENT_EXTRA_ACTIVITY_NAME = "class";

        public SamsungHomeBadger(Context context) : base(context)
        {
        }

        public override void executeBadge(int badgeCount)
        {
            try
            {
                ContentResolver localContentResolver = mContext.ContentResolver;
                Android.Net.Uri localUri = Android.Net.Uri.Parse(INTENT_ACTION);
                ContentValues localContentValues = new ContentValues();
                localContentValues.Put(INTENT_EXTRA_PACKAGENAME, getContextPackageName());
                localContentValues.Put(INTENT_EXTRA_ACTIVITY_NAME, getEntryActivityName());
                localContentValues.Put(INTENT_EXTRA_BADGE_COUNT, badgeCount);
                String str = "package=? AND class=?";
                String[] arrayOfString = new String[2];
                arrayOfString[0] = getContextPackageName();
                arrayOfString[1] = getEntryActivityName();

                int update = localContentResolver.Update(localUri, localContentValues, str, arrayOfString);

                if (update == 0)
                {
                    localContentResolver.Insert(localUri, localContentValues);
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error("CHECK", "Samsung : " + ex.Message);
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "executeBadge method -> in SamsungHomeBadger.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }


        public override List<String> getSupportLaunchers()
        {
            try
            {
                List<string> supportedLaunchers = new List<string>();
                supportedLaunchers.Add("com.sec.android.app.launcher");
                supportedLaunchers.Add("com.sec.android.app.twlauncher");
                return supportedLaunchers;
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "getSupportLaunchers method -> in SamsungHomeBadger.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }
    }
}

