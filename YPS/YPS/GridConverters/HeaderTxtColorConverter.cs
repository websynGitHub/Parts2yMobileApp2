using System;
using System.Globalization;
using Xamarin.Forms;

namespace YPS.GridConverters
{
    public class HeaderTxtColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Color.White;
        }
    }
}
