using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.iOS.Parts2y.Parts2y_Custom_Renderer;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(MyNavgtnPageRenderer))]
namespace YPS.iOS.Parts2y.Parts2y_Custom_Renderer
{
    public class MyNavgtnPageRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var att = new UITextAttributes();
                att.Font = UIFont.FromName("Lato-Bold.ttf", 20);
                UINavigationBar.Appearance.SetTitleTextAttributes(att);
            }
        }
    }
}