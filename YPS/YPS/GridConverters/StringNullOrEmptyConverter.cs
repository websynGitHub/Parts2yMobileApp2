using System;
using System.Globalization;
using Xamarin.Forms;

namespace YPS.GridConverters
{
    public class StringNullOrEmptyConverter :IValueConverter
    {
        public object IsNullValue { get; set; }

        public object IsNotNullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == "" ||value==null? IsNullValue : IsNotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}