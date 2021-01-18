using Android.Content;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.CustomRender;
using YPS.Droid.Custom_Renderers;

[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(ExtendedLabelRendererAndroid))]
namespace YPS.Droid.Custom_Renderers
{
    public class ExtendedLabelRendererAndroid : LabelRenderer
    {
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="context"></param>
        public ExtendedLabelRendererAndroid(Context context) : base(context) {}

        /// <summary>
        /// OnElementChanged
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element != null)
            {
                UpdateProperties((ExtendedLabel)Element);
            }
        }

        /// <summary>
        /// OnElementPropertyChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (sender != null)
            {
                UpdateProperties((ExtendedLabel)sender);
            }
        }

        /// <summary>
        /// UpdateProperties
        /// </summary>
        /// <param name="extendedLabel"></param>
        private void UpdateProperties(ExtendedLabel extendedLabel)
        {
            if (extendedLabel.MaxLines > 0d)
            {
                Control.SetSingleLine(false);
                Control.SetMaxLines(extendedLabel.MaxLines);
            }
        }
    }
}