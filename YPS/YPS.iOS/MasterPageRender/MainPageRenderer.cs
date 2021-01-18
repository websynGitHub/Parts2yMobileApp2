using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.iOS.MasterPageRender;

[assembly: ExportRenderer(typeof(MasterDetailPage), typeof(MainPageRenderer), UIUserInterfaceIdiom.Pad)]
namespace YPS.iOS.MasterPageRender
{
    class MainPageRenderer : TabletMasterDetailRenderer
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.PreferredDisplayMode = UISplitViewControllerDisplayMode.PrimaryHidden;
        }
    }
}