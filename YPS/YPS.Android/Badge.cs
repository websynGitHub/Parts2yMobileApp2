using Android.Content;
using System.Collections.Generic;
using System;
using Android.Util;
using Android.Content.PM;
using YPS.Droid.Implementation;

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
            mContext = context;
            getBadger(context);
        }

        public void count(int count)
        {
            try
            {
                mBadge.executeBadge(count);
            }
            catch (Exception e)
            {
                Log.Error(LOG_TAG, e.Message, e);
            }
        }

        public MyBadge getBadger(Context context)
        {
            if (mBadge != null)
            {
                return mBadge;
            }
            Log.Debug(LOG_TAG, "Finding badger");

            try
            {
                Intent intent = new Intent(Intent.ActionMain);
                intent.AddCategory(Intent.CategoryHome);
                ResolveInfo resolveInfo = context.PackageManager.ResolveActivity(intent, PackageInfoFlags.MatchDefaultOnly);
                String currentHomePackage = resolveInfo.ActivityInfo.PackageName.ToLower();

                /*if (Build.Manufacturer.ToLower() == "xiaomi") {
					mBadge = new XiaomiHomeBadger(context);
					return mBadge;
				}*/

                foreach (string badgeclass in BADGERS)
                {
                    Type t = Type.GetType("JSTAY.Android." + badgeclass);
                    MyBadge myObject = (MyBadge)Activator.CreateInstance(t, new Object[] { context });
                    if (myObject.getSupportLaunchers().Contains(currentHomePackage))
                    {
                        mBadge = myObject;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(LOG_TAG, ex.Message.ToString());
            }

            if (mBadge == null)
            {
                mBadge = new DefaultBadger(context);
            }

            Log.Debug(LOG_TAG, "Returning badger:" + mBadge.ToString());
            return mBadge;
        }
    }
}