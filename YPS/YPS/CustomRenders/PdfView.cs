using Xamarin.Forms;

namespace YPS.CustomRenders
{
    /// <summary>
    /// PdfView, This class is used for showing PDF files.
    /// </summary>
    public class PdfView : WebView
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(nameof(Uri), typeof(string), typeof(PdfView));

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }
    }
}
