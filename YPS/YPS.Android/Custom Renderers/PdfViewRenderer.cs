using Android.Content;
using Android.Print;
using Android.Webkit;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using YPS.CustomRenders;
using YPS.Droid.Custom_Renderers;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

[assembly: ExportRenderer(typeof(PdfView), typeof(PdfViewRenderer))]
namespace YPS.Droid.Custom_Renderers
{
    public class PdfViewRenderer : WebViewRenderer
    {
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="context"></param>
        public PdfViewRenderer(Context context) : base(context) { }

        internal class PdfWebChromeClient : WebChromeClient
        {
            public override bool OnJsAlert(global::Android.Webkit.WebView view, string url, string message, JsResult result)
            {
                try
                {
                    if (message != "PdfViewer_app_scheme:print")
                    {
                        return base.OnJsAlert(view, url, message, result);
                    }

                    using (var printManager = Forms.Context.GetSystemService(global::Android.Content.Context.PrintService) as PrintManager)
                    {
                        printManager?.Print(FileName, new FilePrintDocumentAdapter(FileName, Uri), null);
                    }
                    result.Cancel();
                    return true;
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "LoadFile method -> in PdfViewRenderer.cs " + Settings.userLoginID);
                    YPSService service = new YPSService();
                    service.Handleexception(ex);
                    return false;
                }
            }

            public string Uri { private get; set; }

            public string FileName { private get; set; }
        }

        /// <summary>
        /// Gets called when any changes occur to the PdfView.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.WebView> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (e.NewElement == null)
                {
                    return;
                }

                var pdfView = Element as PdfView;

                if (pdfView == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(pdfView.Uri) == false)
                {
                    Control.SetWebChromeClient(new PdfWebChromeClient
                    {
                        Uri = pdfView.Uri,
                        FileName = GetFileNameFromUri(pdfView.Uri)
                    });
                }

                Control.Settings.AllowFileAccess = true;
                Control.Settings.AllowUniversalAccessFromFileURLs = true;
                LoadFile(pdfView.Uri);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LoadFile method -> in PdfViewRenderer.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is to get file name from.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static string GetFileNameFromUri(string uri)
        {
            try
            {
                var lastIndexOf = uri?.LastIndexOf("/", StringComparison.InvariantCultureIgnoreCase);
                return lastIndexOf > 0 ? uri.Substring(lastIndexOf.Value, uri.Length - lastIndexOf.Value) : string.Empty;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LoadFile method -> in PdfViewRenderer.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
                return "";
            }
        }

        /// <summary>
        /// Gets called when any chnages to property occur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);
                if (e.PropertyName != PdfView.UriProperty.PropertyName)
                {
                    return;
                }
                var pdfView = Element as PdfView;

                if (pdfView == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(pdfView.Uri) == false)
                {
                    Control.SetWebChromeClient(new PdfWebChromeClient
                    {
                        Uri = pdfView.Uri,
                        FileName = GetFileNameFromUri(pdfView.Uri)
                    });
                }

                LoadFile(pdfView.Uri);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnElementPropertyChanged method -> in PdfViewRenderer.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is to load file.
        /// </summary>
        /// <param name="uri"></param>
        private void LoadFile(string uri)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(uri))
                {
                    return;
                }
                Control.LoadUrl($"file:///android_asset/pdfjs/web/viewer.html?file=file://{uri}");
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LoadFile method -> in PdfViewRenderer.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }
    }
}