using Syncfusion.Data;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace YPS.GridConverters
{
    public class GroupCaption2ndValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var data = value != null ? value as Group : null;
                if (data != null)
                {
                    string val = data.Key.ToString();
                    var shipnumber = val.Split('+');
                    return shipnumber[1];
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
