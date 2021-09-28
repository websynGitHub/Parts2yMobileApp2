using System;
using Xamarin.Forms;
using YPS.Service;
using YPS.Helpers;
using YPS.CommonClasses;

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
                YPSLogger.ReportException(ex, "FontAwesomeIconLabel default constructor -> in FontAwesomeIconLabel.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
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
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FontAwesomeIconLabel constructor with one parameter -> in FontAwesomeIconLabel.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }
    }
}
