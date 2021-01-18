using Xamarin.Forms;

namespace YPS.CustomRenders
{
    public class FontAwesomeIconLabel : Label
    {
        public static string Typeface => Device.RuntimePlatform == Device.Android ? "fontawesome-regular" : "FontAwesome";
        
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public FontAwesomeIconLabel()
        {
            FontFamily = Typeface;
        }

        /// <summary>
        /// Paramertized constractor.
        /// </summary>
        /// <param name="fontAwesomeIcon"></param>
        public FontAwesomeIconLabel(string fontAwesomeIcon = null)
        {
            FontFamily = Typeface; //iOS is happy with this, Android needs a renderer to add ".ttf"
            Text = fontAwesomeIcon;
        }
    }
}
