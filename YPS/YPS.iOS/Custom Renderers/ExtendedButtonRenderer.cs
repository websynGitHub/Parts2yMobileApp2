using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CustomRender;
using YPS.iOS.Custom_Renderers;
using CoreGraphics;
using System.Drawing;
using System.ComponentModel;
using YPS.CustomRenders;

[assembly: ExportRenderer(typeof(EllipButton), typeof(EllipsExtendedRenderer))]
namespace YPS.iOS.Custom_Renderers
{
    public class EllipsExtendedRenderer : ButtonRenderer
    {

        private EllipButton TypedElement => this.Element as EllipButton;
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            TypedElement.CornerRadius = 20;
            TypedElement.BorderWidth = 1;
            TypedElement.WidthRequest = 120;
            TypedElement.HeightRequest = 50;
        }

        private void ButtonShape(System.Drawing.RectangleF rec)
        {
            UIButton btn;
            if (TypedElement != null)
            {
                //using (var background = new GradientDrawable())
                //{


                //}
            }
        }
    }
}