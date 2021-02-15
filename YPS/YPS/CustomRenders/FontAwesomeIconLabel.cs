using System;
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
            try
            {
                FontFamily = Typeface;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Paramertized constractor.
        /// </summary>
        /// <param name="fontAwesomeIcon"></param>
        public FontAwesomeIconLabel(string fontAwesomeIcon = null)
        {
            try
            {
                FontFamily = Typeface; //iOS is happy with this, Android needs a renderer to add ".ttf"
                Text = fontAwesomeIcon;
            }
            catch(Exception ex)
            {

            }
        }
    }
}
