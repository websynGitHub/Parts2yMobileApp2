using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CustomRenders;
using YPS.iOS.Custom_Renderers;
using System;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

[assembly: ExportRenderer(typeof(HyperlinkLabel), typeof(HyperlinkLabelRenderer))]
namespace YPS.iOS.Custom_Renderers
{
    public class HyperlinkLabelRenderer : LabelRenderer
    {
        /// <summary>
        /// Gets called when any changes occur to the HyperlinkLabel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            try
            {
                base.OnElementChanged(e);

                Control.UserInteractionEnabled = true;

                Control.TextColor = UIColor.Blue;

                var gesture = new UITapGestureRecognizer();

                gesture.AddTarget(() =>
                {
                    var url = new NSUrl("https://" + Control.Text);

                    if (UIApplication.SharedApplication.CanOpenUrl(url))
                        UIApplication.SharedApplication.OpenUrl(url);
                });

                Control.AddGestureRecognizer(gesture);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnElementChanged method -> in HyperlinkLabelRenderer.cs" + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}