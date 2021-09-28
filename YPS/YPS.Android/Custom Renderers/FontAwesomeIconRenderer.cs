using Xamarin.Forms.Platform.Android;
using Android.Content;
using Xamarin.Forms;
using Android.Graphics;
using YPS.CustomRenders;
using YPS.Droid.Custom_Renderers;
using System;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

[assembly:ExportRenderer(typeof(FontAwesomeIconLabel),typeof(FontAwesomeIconRenderer))]
namespace YPS.Droid.Custom_Renderers
{
    public class FontAwesomeIconRenderer : LabelRenderer
    {
        private readonly Context _context;

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="context"></param>
        public FontAwesomeIconRenderer(Context context) : base(context){
            _context = context;
        }

        /// <summary>
        /// Gets called when any changes occur to the FontAwesomeIconLabel.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            try
            {
                base.OnElementChanged(e);
                if (e.OldElement == null)
                {
                    //The ttf in /Assets is CaseSensitive, so name it FontAwesome.ttf
                    Control.Typeface = Typeface.CreateFromAsset(_context.Assets, FontAwesomeIconLabel.Typeface + ".ttf");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnElementChanged method -> in FontAwesomeIconRenderer.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }
    }
}