using Android.Content;
using System.Collections.Generic;
using System;
using Android.Util;
using Android.Content.PM;
using YPS.Droid.Implementation;
using YPS.Service;
using YPS.Helpers;
using YPS.CommonClasses;

namespace YPS.Droid
{
    public class Badge
    {
        private static MyBadge mBadge;
        private static Context mContext;
        private static string LOG_TAG = "BadgeImplementation";
        private static List<String> BADGERS = new List<String>(
            new String[] { "AdwHomeBadger",
                "ApexHomeBadger",
                "AsusHomeLauncher",
                "LGHomeBadger",
                "NewHtcHomeBadger",
                "NovaHomeBadger",
                "SamsungHomeBadger",
                "SolidHomeBadger",
                "SonyHomeBadger",
                "XiaomiHomeBadger",
            }
        );

        public Badge(Context context)
        {
            try
            {
                mContext = context;
                getBadger(context);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Badge constructor  -> in Badge.cs " + Settings.userLoginID);
            }
        }

        public void count(int count)
        {
            try
            {
                mBadge.executeBadge(count);
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.Message, ex);
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "count method  -> in Badge.cs " + Settings.userLoginID);
            }
        }

        public MyBadge getBadger(Context context)
        {
            try
            {
                if (mBadge != null)
                {
                    return mBadge;
                }
                Log.Debug(LOG_TAG, "Finding badger");


                Intent intent = new Intent(Intent.ActionMain);
                intent.AddCategory(Intent.CategoryHome);
                ResolveInfo resolveInfo = context.PackageManager.ResolveActivity(intent, PackageInfoFlags.MatchDefaultOnly);
                String currentHomePackage = resolveInfo.ActivityInfo.PackageName.ToLower();

                /*if (Build.Manufacturer.ToLower() == "xiaomi") {
					mBadge = new XiaomiHomeBadger(context);
					return mBadge;
				}*/
                
                //foreach (string badgeclass in BADGERS)
                //{
                //    Type t = Type.GetType("YPS.Droid.Implementation." + badgeclass);
                //    MyBadge myObject = (MyBadge)Activator.CreateInstance(t, new Object[] { context });
                //    if (myObject.getSupportLaunchers().Contains(currentHomePackage))
                //    {
                //        mBadge = myObject;
                //    }
                //}


                if (mBadge == null)
                {
                    mBadge = new DefaultBadger(context);
                }

                Log.Debug(LOG_TAG, "Returning badger:" + mBadge.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.Message.ToString());
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "getBadger method  -> in Badge.cs " + Settings.userLoginID);
            }
            return mBadge;
        }
    }
}