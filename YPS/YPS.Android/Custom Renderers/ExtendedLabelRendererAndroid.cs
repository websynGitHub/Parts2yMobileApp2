using Android.Content;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.CustomRender;
using YPS.Droid.Custom_Renderers;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(ExtendedLabelRendererAndroid))]
namespace YPS.Droid.Custom_Renderers
{
    public class ExtendedLabelRendererAndroid : LabelRenderer
    {
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="context"></param>
        public ExtendedLabelRendererAndroid(Context context) : base(context) { }

        /// <summary>
        /// OnElementChanged
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
                YPSLogger.ReportException(ex, "OnElementChanged method -> in OnElementPropertyChanged.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// OnElementPropertyChanged
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
                YPSLogger.ReportException(ex, "UpdateProperties method -> in OnElementPropertyChanged.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// UpdateProperties
        /// </summary>
        /// <param name="extendedLabel"></param>
        private void UpdateProperties(ExtendedLabel extendedLabel)
        {
            try
            {
                if (extendedLabel.MaxLines > 0d)
                {
                    Control.SetSingleLine(false);
                    Control.SetMaxLines(extendedLabel.MaxLines);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "UpdateProperties method -> in ExtendedLabelRendererAndroid.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }
    }
}