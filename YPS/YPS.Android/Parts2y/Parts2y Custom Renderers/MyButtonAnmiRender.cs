using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.Parts2y.Parts2y_Custom_Renderers;

namespace YPS.Droid.Parts2y.Parts2y_Custom_Renderers
{
    public class MyButtonAnmiRender: ButtonRenderer
    {
        private MyButton TypedElement => this.Element as MyButton;

        public MyButtonAnmiRender(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (this.Control != null && e.NewElement != null)
            {
                this.UpdateBackground();

                if (Build.VERSION.SdkInt > BuildVersionCodes.Lollipop)
                {
                    this.Control.StateListAnimator = null;
                }
                else
                {
                    //this.Control.Elevation = 0;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals(VisualElement.BackgroundColorProperty.PropertyName) ||
                e.PropertyName.Equals(Xamarin.Forms.Button.CornerRadiusProperty.PropertyName) ||
                e.PropertyName.Equals(Xamarin.Forms.Button.BorderWidthProperty.PropertyName))
            {
                this.UpdateBackground();
            }
        }

        private void UpdateBackground()
        {
            if (this.TypedElement != null)
            {
                using (var background = new GradientDrawable())
                {
                    background.SetColor(this.TypedElement.BackgroundColor.ToAndroid());
                    background.SetStroke((int)Context.ToPixels(this.TypedElement.BorderWidth), this.TypedElement.BorderColor.ToAndroid());
                    background.SetCornerRadius(Context.ToPixels(this.TypedElement.CornerRadius));

                    // customize the button states as necessary
                    using (var backgroundStates = new StateListDrawable())
                    {
                        backgroundStates.AddState(new int[] { }, background);

                        this.Control.SetBackground(backgroundStates);
                    }
                }
            }
        }
    }
}