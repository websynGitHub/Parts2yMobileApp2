using Android.App;
using Android.Views;
using Android.Widget;
using System;
using YPS.CustomToastMsg;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

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
                Device.BeginInvokeOnMainThread(async () =>
                {
                    Toast toast = Toast.MakeText(Android.App.Application.Context, message, ToastLength.Short);
                    var view = toast.View;
                    toast.SetGravity(GravityFlags.Center, 0, 0);
                    toast.View.Background.SetColorFilter(Settings.Bar_Background.ToAndroid(), PorterDuff.Mode.SrcIn);
                    TextView textview = (TextView)toast.View.FindViewById(Android.Resource.Id.Message);
                    textview.SetTypeface(null, TypefaceStyle.Bold);
                    textview.SetTextColor(Android.Graphics.Color.White);
                    toast.Show();
                });
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShortAlert method -> in MessageAndroid.cs " + Settings.userLoginID);
            }
        }
    }
}