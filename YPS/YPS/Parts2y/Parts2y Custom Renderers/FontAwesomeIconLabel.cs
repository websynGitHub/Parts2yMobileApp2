using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YPS.Parts2y.Parts2y_Custom_Renderers
{
    // for reference--https://github.com/johankson/awesome
    public class FontAwesomeIconLabel:Label
    {
       public static string Typeface  => Device.RuntimePlatform == Device.Android ? "fontawesome-regular" : "FontAwesome";

        public FontAwesomeIconLabel()
        {
            FontFamily = Typeface;
        }

        public FontAwesomeIconLabel(string fontAwesomeIcon = null)
        {
            FontFamily = Typeface; //iOS is happy with this, Android needs a renderer to add ".ttf"
            Text = fontAwesomeIcon;
        }
    }
}
