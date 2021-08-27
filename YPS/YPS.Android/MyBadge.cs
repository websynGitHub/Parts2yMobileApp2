using Android.Content;
using System;
using System.Collections.Generic;

namespace YPS.Droid
{
    public abstract class MyBadge
    {
        private static MyBadge mBadge;
        protected static Context mContext;

        private static string LOG_TAG = "BadgeImplementation";

        public abstract void executeBadge(int badgeCount);
        public abstract List<string> getSupportLaunchers();

        public MyBadge(Context context)
        {
            mContext = context;
        }

        protected String getEntryActivityName()
        {
            ComponentName componentName = mContext.PackageManager.GetLaunchIntentForPackage(mContext.PackageName).Component;
            return componentName.ClassName;
        }

        protected String getContextPackageName()
        {
            return mContext.PackageName;
        }
    }
}