using Syncfusion.Data;
using System;
using System.Globalization;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;

namespace YPS.GridConverters
{
    public class PlCount : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var data = value != null ? value as Group : null;

                var plCount = data.Key.ToString().Split(',');
                
                if(plCount[1] == "0")
                {
                    return null;
                }
                else
                {
                    return data != null ?  plCount[1]  : null;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PlCount-> in Convert " + Settings.userLoginID);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
