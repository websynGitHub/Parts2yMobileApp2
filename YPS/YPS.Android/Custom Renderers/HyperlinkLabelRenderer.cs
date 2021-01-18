using Android.Content;
using Android.Text.Util;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.Android.Custom_Renderers;
using YPS.CustomRenders;

[assembly: ExportRenderer(typeof(HyperlinkLabel), typeof(HyperlinkLabelRenderer))]
namespace YPS.Android.Custom_Renderers
{
    public class HyperlinkLabelRenderer : LabelRenderer
    {
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="context"></param>
        public HyperlinkLabelRenderer(Context context) : base(context) { }

        /// <summary>
        /// Gets called when any changes occur to the HyperlinkLabel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            Linkify.AddLinks(Control, MatchOptions.All);
        }
    }
}