using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.iOS.MasterPageRender;
using YPS.Service;

[assembly: ExportRenderer(typeof(MasterDetailPage), typeof(MainPageRenderer), UIUserInterfaceIdiom.Pad)]
namespace YPS.iOS.MasterPageRender
{
    class MainPageRenderer : TabletMasterDetailRenderer
    {
        public override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();

                this.PreferredDisplayMode = UISplitViewControllerDisplayMode.PrimaryHidden;
            }
            catch(Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "ViewDidLoad method -> in MainPageRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}