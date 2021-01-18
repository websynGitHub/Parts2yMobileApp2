using Foundation;
using UIKit;
using Xamarin.Forms;
using YPS.Helpers;
using YPS.iOS.Dependencies;
using YPS.CommonClasses;
using System;
using YPS.Service;

[assembly: Dependency(typeof(PdfPrintFile))]
namespace YPS.iOS.Dependencies
{
    public class PdfPrintFile : NewOpenPdfI
    {
        /// <summary>
        /// This is to pass pdf path, to preview it.
        /// </summary>
        /// <param name="pathpdf"></param>
        public void passPath(string pathpdf)
        {
            try
            {
                var PreviewController = UIDocumentInteractionController.FromUrl(NSUrl.FromFilename(pathpdf));
                PreviewController.Delegate = new UIDocumentInteractionControllerDelegateClass(UIApplication.SharedApplication.KeyWindow.RootViewController);
                
                Device.BeginInvokeOnMainThread(() =>
                {
                    PreviewController.PresentPreview(true);
                });
            }
            catch(Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "passPath method -> in PdfPrintFile.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
           
        }
    }
}