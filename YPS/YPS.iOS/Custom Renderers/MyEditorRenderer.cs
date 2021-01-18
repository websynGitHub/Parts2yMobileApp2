using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CustomRenders;
using YPS.iOS.Custom_Renderers;
using System;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;


[assembly: ExportRenderer(typeof(MyEditor), typeof(MyEditorRenderer))]
namespace YPS.iOS.Custom_Renderers
{
    public class MyEditorRenderer : EditorRenderer
    {
        public bool IsOnChatView { get; private set; }

        /// <summary>
        /// Gets called when any changes occur to the MyEditor.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            try
            {
                base.OnElementChanged(e);

                var element = this.Element as MyEditor;

                if (Control != null && element != null)
                {
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
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnElementChanged method -> in MyEditorRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// AddDoneButton
        /// </summary>
        protected void AddDoneButton()
        {
            try
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
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "AddDoneButton method -> in MyEditorRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}
