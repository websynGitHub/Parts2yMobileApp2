using Foundation;
using UIKit;
using YPS.CustomToastMsg;
using YPS.iOS.Dependencies;
using System;
using YPS.CommonClasses;
using YPS.Helpers;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(MessageIOS))]
namespace YPS.iOS.Dependencies
{
    public class MessageIOS : IToastMessage
    {
        #region Data member
        const double SHORT_DELAY = 2.0;
        NSTimer alertDelay;
        UIAlertController alert;
        #endregion

        /// <summary>
        /// Toast message showing by this method.
        /// </summary>
        /// <param name="message"></param>
        public void ShortAlert(string message)
        {
            ShowAlert(message, SHORT_DELAY);
        }

        /// <summary>
        /// ShowAlert.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="seconds"></param>
        void ShowAlert(string message, double seconds)
        {
            try
            {
                alertDelay = NSTimer.CreateScheduledTimer(seconds, (obj) =>
                {
                    dismissMessage();
                });

                alert = UIAlertController.Create(null, message, UIAlertControllerStyle.Alert);
                alert.View.BackgroundColor = Settings.Bar_Background.ToUIColor();
                alert.View.Layer.CornerRadius=10;
                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShowAlert method-> in MessageIOS.cs" + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Toast message dismiss.
        /// </summary>
        void dismissMessage()
        {
            try
            {
                if (alert != null)
                {
                    alert.DismissViewController(true, null);
                }

                if (alertDelay != null)
                {
                    alertDelay.Dispose();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "dismissMessage method-> in MessageIOS.cs" + Settings.userLoginID);
            }
        }
    }
}