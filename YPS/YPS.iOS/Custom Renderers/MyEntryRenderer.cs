using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System;
using System.ComponentModel;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CommonClasses;
using YPS.CustomRender;
using YPS.Helpers;
using YPS.iOS.Custom_Renderers;
using YPS.Service;

[assembly: ExportRenderer(typeof(MyEntry), typeof(MyEntryRenderer))]
namespace YPS.iOS.Custom_Renderers
{
    public class MyEntryRenderer : EntryRenderer, IUITextViewDelegate
    {
        /// <summary>
        /// Gets called when any changes occur to the MyEntry.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            try
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
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnElementChanged method -> in MyEntryRenderer.cs " + Settings.userLoginID);
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
                    ((IEntryController)Element).SendCompleted();
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
                YPSLogger.ReportException(ex, "AddDoneButton method -> in MyEntryRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when any changes occur to the MyEntry's property
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnElementPropertyChanged method -> in MyEntryRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// To disable the Copy,Paste,Cut,Select All,Select ect. functionalities
        /// </summary>
        /// <param name="action"></param>
        /// <param name="withSender"></param>
        /// <returns></returns>
        public override bool CanPerform(Selector action, NSObject withSender)
        {
            var entry = Element as Entry;

            try
            {
                if (entry.ClassId != null && entry.ClassId.Trim().ToLower() == "login")
                {
                    NSOperationQueue.MainQueue.AddOperation(() =>
                    {
                        UIMenuController.SharedMenuController.SetMenuVisible(false, false);
                    });
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "CanPerform method -> in MyEntryRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
            return base.CanPerform(action, withSender);
        }
    }
}