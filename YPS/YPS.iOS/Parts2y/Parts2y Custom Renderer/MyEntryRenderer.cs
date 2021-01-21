using CoreGraphics;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.iOS.Parts2y.Parts2y_Custom_Renderer;
using YPS.Parts2y.Parts2y_Custom_Renderers;

[assembly: ExportRenderer(typeof(MyEntry), typeof(MyEntryRenderer))]

namespace YPS.iOS.Parts2y.Parts2y_Custom_Renderer
{
    public class MyEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            Control.BorderStyle = UITextBorderStyle.None;
            Control.LeftView = new UIView(new CGRect(0, 0, 15, 0));
            Control.LeftViewMode = UITextFieldViewMode.Always;
            Control.RightView = new UIView(new CGRect(0, 0, 15, 0));
            Control.RightViewMode = UITextFieldViewMode.Always;

            var element = Element as Entry;
            if (element == null)
                return;

            if (this.Element.Keyboard == Keyboard.Numeric)
                this.AddDoneButton();
            else
            {
                UIToolbar toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
                {
                    this.Control.ResignFirstResponder();
                });

                toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
                this.Control.InputAccessoryView = toolbar;
            }
        }

        protected void AddDoneButton()
        {
            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                this.Control.ResignFirstResponder();
                var baseEntry = this.Element.GetType();
                ((IEntryController)Element).SendCompleted();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            this.Control.InputAccessoryView = toolbar;
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

            base.OnElementPropertyChanged(sender, e);
        }
    }
}