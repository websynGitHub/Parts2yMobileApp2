using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using System;
using YPS.Helpers;
using YPS.Service;

namespace YPS.Droid
{
    [Activity(Label = "Parts2y WS", Icon = "@drawable/ypslogo22", Theme = "@style/splashscreen", MainLauncher = true, LaunchMode = LaunchMode.SingleTask, NoHistory = true)]
    public class SplashActivity : Activity
    {
        /// <summary>
        /// Gets called before MainActivity() when app is launching.
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                base.OnCreate(bundle);
                /// Create your application here
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
                Finish();
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnCreate method -> in SplashActivity.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}