using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.InputMethods;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.CustomRenders;
using YPS.Droid.Custom_Renderers;

[assembly: ExportRenderer(typeof(MyEditor), typeof(MyEditorRenderer))]
namespace YPS.Droid.Custom_Renderers
{
    public class MyEditorRenderer : EditorRenderer
    {
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="context"></param>
        public MyEditorRenderer(Context context) : base(context){}
        MyEditor myEditor;
        public static void Init() { }

        /// <summary>
        /// Gets called when any changes occur to the MyEditor.
        /// </summary>
        /// <param name="e"></param>
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
               myEditor = e.NewElement as MyEditor;
                this.Control.Hint = element.Placeholder;
                Control.ImeOptions = (ImeAction)ImeFlags.NoExtractUi;
            }
        }

        /// <summary>
        /// Gets called when MyEditor is in focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Control_FocusChange(object sender, FocusChangeEventArgs e)
        {
            if (e.HasFocus)
            {
                myEditor?.NotifyKeyBoardAppear(sender, null);
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustResize);
            }
            else
            {
                myEditor?.NotifyKeyBoardDisappear(sender, null);
                (Forms.Context as Activity).Window.SetSoftInputMode(SoftInput.AdjustNothing);
            }
        }

        /// <summary>
        /// Gets called when any changes occur to the MyEditor's property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
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