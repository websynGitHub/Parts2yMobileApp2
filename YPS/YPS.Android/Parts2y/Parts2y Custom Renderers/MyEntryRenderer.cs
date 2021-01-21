using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
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

[assembly: ExportRenderer(typeof(MyEntry), typeof(MyEntryRenderer))]
namespace YPS.Droid.Parts2y.Parts2y_Custom_Renderers
{
    public class MyEntryRenderer : EntryRenderer
    {
        public MyEntryRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {

                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(global::Android.Graphics.Color.Transparent);
                this.Control.SetBackgroundDrawable(gd);

                #region for password icon changes text  visibility does not change
                //this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                #endregion

                Control.FocusChange += Control_FocusChange;

                //Control.SetHintTextColor(ColorStateList.ValueOf(global::Android.Graphics.Color.Gray)); //change placeHolderColor
            }
        }

        private void Control_FocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustResize);
            else
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustNothing);
        }
    }
}