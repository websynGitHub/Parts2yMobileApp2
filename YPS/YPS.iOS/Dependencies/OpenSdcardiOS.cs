using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using System;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.iOS.Dependencies;
using YPS.Service;

[assembly: Dependency(typeof(OpenSdcardiOS))]
namespace YPS.iOS.Dependencies
{
    public class OpenSdcardiOS : IOpenSdCard
    {
        public async void openSdCard()
        {
            try
            {
                FileData fileData1 = await CrossFilePicker.Current.PickFile();

                if (fileData1 == null)
                    return; // user canceled file picking
            }
            catch(Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "openSdCard method -> in OpenSdcardiOS.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}