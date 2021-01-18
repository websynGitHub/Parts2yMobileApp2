using Android.App;
using Android.Views;
using Android.Widget;
using YPS.CustomToastMsg;
using YPS.Droid.Dependencies;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace YPS.Droid.Dependencies
{
    public class MessageAndroid : IToastMessage
    {
        /// <summary>
        /// This is to show alert
        /// </summary>
        /// <param name="message"></param>
        public void ShortAlert(string message)
        {
            Toast toast = Toast.MakeText(Application.Context, message, ToastLength.Short);
            toast.SetGravity(GravityFlags.Center, 0, 0);
            toast.Show();
        }
    }
}