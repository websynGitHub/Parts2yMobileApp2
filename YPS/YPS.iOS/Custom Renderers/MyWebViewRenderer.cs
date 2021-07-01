using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using SafariServices;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using YPS.CommonClasses;
using YPS.CustomRenders;
using YPS.Helpers;
using YPS.iOS.Custom_Renderers;
using YPS.Service;

[assembly: ExportRenderer(typeof(MyWebView), typeof(MyWebViewRenderer))]
namespace YPS.iOS.Custom_Renderers
{
    public class MyWebViewRenderer : ViewRenderer<MyWebView, WKWebView>
    {
        WKWebView _wkWebView;
        protected override void OnElementChanged(ElementChangedEventArgs<MyWebView> e)
        {
            try
            {
                base.OnElementChanged(e);
                // var vc = GetVisibleViewController();
                if (Control == null)
                {
                    var config = new WKWebViewConfiguration();
                    _wkWebView = new WKWebView(Frame, config);
                    SetNativeControl(_wkWebView);
                    AddSubview(_wkWebView);
                }
                if (e.NewElement != null)
                {
                    if (Element.Html != null)
                        Control.LoadHtmlString(Element.Url, baseUrl: null);
                    else
                        //{
                        //    var sfViewController = new SFSafariViewController(new NSUrl(Element.Url));
                        //    vc.PresentViewController(sfViewController, true, null);
                        //}
                        Control.LoadRequest(new NSUrlRequest(new NSUrl(Element.Url)));
                }
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "OnElementChanged method -> in MyWebViewRenderer.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
        //UIViewController GetVisibleViewController()
        //{
        //    UIViewController viewController = null;
        //    var window = UIApplication.SharedApplication.KeyWindow;


        //    if (window != null && window.WindowLevel == UIWindowLevel.Normal)
        //        viewController = window.RootViewController;

        //    if (viewController == null)
        //    {
        //        window = UIApplication.SharedApplication.Windows.OrderByDescending(w => w.WindowLevel).FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);
        //        if (window == null)
        //            throw new InvalidOperationException("Could not find current view controller");
        //        else
        //            viewController = window.RootViewController;
        //    }

        //    while (viewController.PresentedViewController != null)
        //        viewController = viewController.PresentedViewController;


        //    return viewController;
        //}

        private class CustomWKNavigationDelegate : WKNavigationDelegate
        {
            public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
            {
                try
                {
                    var policy = WKNavigationActionPolicy.Allow;
                    if (navigationAction.NavigationType == WKNavigationType.LinkActivated)
                    {
                        var url = new NSUrl(navigationAction.Request.Url.ToString());
                        if (UIApplication.SharedApplication.CanOpenUrl(url))
                        {
                            UIApplication.SharedApplication.OpenUrl(url);
                            policy = WKNavigationActionPolicy.Cancel;
                        }
                    }
                    decisionHandler?.Invoke(policy);
                }
                catch (Exception ex)
                {
                    YPSService trackService = new YPSService();
                    YPSLogger.ReportException(ex, "DecidePolicy method -> in MyWebViewRenderer.cs " + Settings.userLoginID);
                    trackService.Handleexception(ex);
                    base.DecidePolicy(webView, navigationAction, decisionHandler);
                }
            }

            public override void DidReceiveAuthenticationChallenge(WKWebView webView, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
            {
                try
                {
                    completionHandler?.Invoke(NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, null);
                }
                catch (Exception ex)
                {
                    YPSService trackService = new YPSService();
                    YPSLogger.ReportException(ex, "DidReceiveAuthenticationChallenge method -> in MyWebViewRenderer.cs " + Settings.userLoginID);
                    trackService.Handleexception(ex);
                }
            }
        }
    }
}