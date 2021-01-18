using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CustomRender;
using YPS.iOS.Custom_Renderers;
using System;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;


[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(ExtendedLabelRendereriOS))]
namespace YPS.iOS.Custom_Renderers
{
    public class ExtendedLabelRendereriOS : LabelRenderer
    {
        /// <summary>
        /// Gets called when any changes occur to the ExtendedLabel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            try
            {
                base.OnElementChanged(e);
                
                if (Control != null && Element != null)
                {
                    UpdateProperties((ExtendedLabel)Element);
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnElementChanged method -> in ExtendedLabelRendereriOS.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when any changes occur to the ExtendedLabel's property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);

                if (sender != null)
                {
                    UpdateProperties((ExtendedLabel)sender);
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnElementPropertyChanged method -> in ExtendedLabelRendereriOS.cs" + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is to update the property value.
        /// </summary>
        /// <param name="extendedLabel"></param>
        private void UpdateProperties(ExtendedLabel extendedLabel)
        {
            try
            {
                if (!string.IsNullOrEmpty(extendedLabel.Text))
                {
                    if (extendedLabel.MaxLines > 0)
                    {
                        Control.Lines = extendedLabel.MaxLines;
                    }

                    LayoutSubviews();
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "UpdateProperties method -> in ExtendedLabelRendereriOS.cs" + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}