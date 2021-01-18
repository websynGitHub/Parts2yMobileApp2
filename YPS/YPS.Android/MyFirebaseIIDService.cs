using Android.App;
using Firebase.Iid;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.Droid;
using YPS.CommonClasses;

[assembly: Dependency(typeof(MyFirebaseIIDService))]
namespace YPS.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService 
    {
        string refreshedToken;
        const string TAG = "MyFirebaseIIDService";

        //public override async void OnTokenRefresh()
        //{
        //    refreshedToken = FirebaseInstanceId.Instance.Token;
        //    await SecureStorage.SetAsync("Token", refreshedToken);
        //}


        public override async void OnTokenRefresh()
        {
            try
            {
                refreshedToken = FirebaseInstanceId.Instance.Token;
                Settings.FireBasedToken = refreshedToken;
                await SecureStorage.SetAsync("Token", refreshedToken);
                //Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            }
            catch (System.Exception ex)
            {
                ex.ToString();
            }

        }

        public string getTokenValue()
        {
            OnTokenRefresh();
            return refreshedToken;
            //throw new NotImplementedException();
        }
    }
}







