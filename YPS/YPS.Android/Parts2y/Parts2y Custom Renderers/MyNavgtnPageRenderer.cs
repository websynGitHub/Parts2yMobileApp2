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
using Xamarin.Forms.Platform.Android.AppCompat;
using YPS.Droid.Parts2y.Parts2y_Custom_Renderers;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(MyNavgtnPageRenderer))]
namespace YPS.Droid.Parts2y.Parts2y_Custom_Renderers
{
    public class MyNavgtnPageRenderer: NavigationPageRenderer
    {
        public MyNavgtnPageRenderer(Context context) : base(context)
        {
        }

        private Android.Support.V7.Widget.Toolbar toolbar;

        public override void OnViewAdded(Android.Views.View child)
        {
            base.OnViewAdded(child);
            if (child.GetType() == typeof(Android.Support.V7.Widget.Toolbar))
            {
                toolbar = child as Android.Support.V7.Widget.Toolbar;
                toolbar.ChildViewAdded += Toolbar_ChildViewAdded;
                var a = toolbar.ChildCount;
            }
        }

        void Toolbar_ChildViewAdded(object sender, ChildViewAddedEventArgs e)
        {
            var view = e.Child.GetType();
            if (e.Child.GetType() == typeof(Android.Support.V7.Widget.AppCompatTextView))
            {
                var textView = (TextView)e.Child;
                var spaceFont = Typeface.CreateFromAsset(Xamarin.Forms.Forms.Context.ApplicationContext.Assets, "Lato-Bold.ttf");
                textView.TextSize = 20;
                textView.Typeface = spaceFont;
                toolbar.ChildViewAdded -= Toolbar_ChildViewAdded;
            }
        }
    }
}