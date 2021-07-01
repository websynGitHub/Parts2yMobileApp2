using Acr.UserDialogs;
using System;
using UIKit;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;

namespace YPS.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            try
            {
                UIApplication.Main(args, null, "AppDelegate");
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Main method -> in Application.cs " + Settings.userLoginID);
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
        }
    }
}
