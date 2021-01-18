using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace YPS.Droid
{
    [Activity(Label = "Parts2y", Icon = "@drawable/ypslogo12", Theme = "@style/splashscreen", MainLauncher = true, LaunchMode = LaunchMode.SingleTask,NoHistory =true)]
    public class SplashActivity : Activity
    {
        /// <summary>
        /// Gets called before MainActivity() when app is launching.
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            /// Create your application here
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
            Finish();           
        }
    }
}