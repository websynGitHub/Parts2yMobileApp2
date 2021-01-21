using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YPS.Droid.Parts2y.Parts2y_Custom_Renderers;
using YPS.Parts2y.Parts2y_Custom_Renderers;
using Android.Graphics;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Views.InputMethods;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(MyEditor), typeof(MyEditorRenderer))]
namespace YPS.Droid.Parts2y.Parts2y_Custom_Renderers
{
    public class MyEditorRenderer : EditorRenderer
    {
        public MyEditorRenderer(Context context) : base(context)
        {

        }
        public static void Init() { }
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                Control.Background = null;

                var layoutParams = new MarginLayoutParams(Control.LayoutParameters);
                layoutParams.SetMargins(0, 0, 0, 0);
                LayoutParameters = layoutParams;
                Control.LayoutParameters = layoutParams;
                //Control.SetPadding(5, 0, 5, 0);
                //SetPadding(5, 0, 5, 0);

                GradientDrawable gd = new GradientDrawable();
                gd.SetStroke(2, global::Android.Graphics.Color.Transparent);
                gd.SetCornerRadius(10);
                this.Control.SetPadding(20, 2, 20, 0);
                this.Control.SetBackgroundDrawable(gd);

                Control.FocusChange += Control_FocusChange;
            }

            if (e.NewElement != null)
            {
                var element = e.NewElement as MyEditor;
                this.Control.Hint = element.Placeholder;

                //Control.ImeOptions = ImeAction.Done;
                //Control.SetImeActionLabel("Done", ImeAction.Done);
                Control.ImeOptions = (ImeAction)ImeFlags.NoExtractUi;
                //Control.SetRawInputType(InputTypes.TextFlagCapSentences);
                //NumberFlagDecimal
            }
        }
        private void Control_FocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustResize);
            else
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustNothing);
        }

        protected override void OnElementPropertyChanged(
        object sender,
        PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == MyEditor.PlaceholderProperty.PropertyName)
            {
                var element = this.Element as MyEditor;
                this.Control.Hint = element.Placeholder;

            }
        }
    }
}