using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YPS.CustomToastMsg;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

[assembly: Dependency(typeof(PNCancelService))]
namespace YPS.Droid.Dependencies
{
    public class PNCancelService : IPNClearClass
    {
        public void CancelPush(string tag, int id)
        {
            try
            {
                var pNManager = NotificationManagerCompat.From(Android.App.Application.Context);

                if (id > 0)
                {
                    pNManager.Cancel(tag, id);
                }
                else
                {
                    pNManager.CancelAll();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CancelPush method -> in PNCancelService.cs " + Settings.userLoginID);
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
            }
        }
    }
}