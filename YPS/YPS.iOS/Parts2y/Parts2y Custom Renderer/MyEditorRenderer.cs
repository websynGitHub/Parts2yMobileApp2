using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.iOS.Parts2y.Parts2y_Custom_Renderer;
using YPS.Parts2y.Parts2y_Custom_Renderers;

[assembly: ExportRenderer(typeof(MyEditor), typeof(MyEditorRenderer))]
namespace YPS.iOS.Parts2y.Parts2y_Custom_Renderer
{
    public class MyEditorRenderer : EditorRenderer
    {
        private string Placeholder { get; set; }
        public bool IsOnChatView { get; private set; }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            var element = this.Element as MyEditor;

            if (Control != null && element != null)
            {
                //Control.KeyboardType = UIKeyboardType.ASCIICapable;
                if (this.Element.Keyboard == Keyboard.Default)
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
        }
        protected void AddDoneButton()
        {

            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, delegate
            {
                this.Control.ResignFirstResponder();
                var baseEntry = this.Element.GetType();
                ((IEditorController)Element).SendCompleted();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            this.Control.InputAccessoryView = toolbar;
        }
    }
}
