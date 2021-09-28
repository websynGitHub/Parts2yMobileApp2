using Android.App;
using Android.Views;
using Android.Widget;
using System;
using YPS.CustomToastMsg;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

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
            try
            {
                Toast toast = Toast.MakeText(Application.Context, message, ToastLength.Short);
                toast.SetGravity(GravityFlags.Center, 0, 0);
                toast.Show();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShortAlert method -> in MessageAndroid.cs " + Settings.userLoginID);
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
            }
        }
    }
}