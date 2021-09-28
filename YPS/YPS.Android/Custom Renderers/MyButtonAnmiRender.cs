using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using YPS.CustomRenders;
using YPS.Droid.Custom_Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Button = Xamarin.Forms.Button;
using YPS.CustomRender;

//[assembly: ExportRenderer(typeof(MyButton), typeof(MyButtonAnmiRender))]
namespace YPS.Droid.Custom_Renderers
{

    public class MyButtonAnmiRender : ButtonRenderer
    {
        private MyButton TypedElement => this.Element as MyButton;

        public MyButtonAnmiRender(Context context) : base(context)
        {
        }

        /// <summary>
        /// Gets called when any changes occur to the MyButton.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
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

        /// <summary>
        /// Gets called when any changes occur to the MyButton's property.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals(VisualElement.BackgroundColorProperty.PropertyName) ||
                e.PropertyName.Equals(Button.CornerRadiusProperty.PropertyName) ||
                e.PropertyName.Equals(Button.BorderWidthProperty.PropertyName))
            {
                this.UpdateBackground();
            }
        }


        /// <summary>
        /// This method applies style to MyButton.
        /// </summary>
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