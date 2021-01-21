using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.Droid.Parts2y.Parts2y_Custom_Renderers;
using YPS.Parts2y.Parts2y_Custom_Renderers;

[assembly: ExportRenderer(typeof(FontAwesomeIconLabel), typeof(FontAwesomeIconRenderer))]
namespace YPS.Droid.Parts2y.Parts2y_Custom_Renderers
{
    public class FontAwesomeIconRenderer : LabelRenderer
    {
        private readonly Context _context;

        public FontAwesomeIconRenderer(Context context) : base(context)
        {
            _context = context;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                //The ttf in /Assets is CaseSensitive, so name it FontAwesome.ttf
                Control.Typeface = Typeface.CreateFromAsset(_context.Assets, FontAwesomeIconLabel.Typeface + ".ttf");
            }
        }
    }
}