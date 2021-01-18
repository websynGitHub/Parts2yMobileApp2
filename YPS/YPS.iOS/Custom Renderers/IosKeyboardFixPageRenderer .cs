using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CustomRenders;
using YPS.iOS.Custom_Renderers;
using System;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

[assembly: ExportRenderer(typeof(MyContentPage), typeof(IosKeyboardFixPageRenderer))]
namespace YPS.iOS.Custom_Renderers
{
    public class IosKeyboardFixPageRenderer : PageRenderer
    {
        #region Data members
        NSObject observerHideKeyboard;
        NSObject observerShowKeyboard;
        private bool _isDisposed;
        #endregion

        public override void ViewDidLoad()
        {
            try
            {
                base.ViewDidLoad();

                var cp = Element as MyContentPage;
                cp.BackgroundColor = System.Drawing.Color.White;

                if (cp != null && !cp.CancelsTouchesInView)
                {
                    foreach (var g in View.GestureRecognizers)
                    {
                        if (YPS.CommonClasses.Settings.currentPage == "chatPage")
                        {
                            g.CancelsTouchesInView = true;
                        }
                        else
                        {
                            g.CancelsTouchesInView = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "ViewDidLoad method -> in IosKeyboardFixPageRenderer.cs" + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            try
            {
                base.ViewWillAppear(animated);

                observerHideKeyboard = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
                observerShowKeyboard = NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "ViewWillAppear method -> in IosKeyboardFixPageRenderer.cs" + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            try
            {
                base.ViewWillDisappear(animated);

                NSNotificationCenter.DefaultCenter.RemoveObserver(observerHideKeyboard);
                NSNotificationCenter.DefaultCenter.RemoveObserver(observerShowKeyboard);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "ViewWillDisappear method -> in IosKeyboardFixPageRenderer.cs" + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                _isDisposed = true;

                base.Dispose(disposing);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "Dispose method -> in IosKeyboardFixPageRenderer.cs" + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        void OnKeyboardNotification(NSNotification notification)
        {
            try
            {
                if (_isDisposed || !IsViewLoaded) return;

                var frameBegin = UIKeyboard.FrameBeginFromNotification(notification);
                var frameEnd = UIKeyboard.FrameEndFromNotification(notification);

                var page = Element as ContentPage;

                if (page != null && !(page.Content is ScrollView))
                {
                    var padding = page.Padding;
                    page.Padding = new Thickness(padding.Left, padding.Top, padding.Right, padding.Bottom + frameBegin.Top - frameEnd.Top);
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnKeyboardNotification method -> in IosKeyboardFixPageRenderer.cs" + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}


