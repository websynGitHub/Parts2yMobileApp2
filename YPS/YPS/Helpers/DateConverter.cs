using System;
using Xamarin.Forms;

namespace YPS.Helpers
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            DateTime dateTime = (DateTime)value;

            if (dateTime.Equals(DateTime.Today))
            {
                return "Today";
            }
            return dateTime.ToString("dd MMMM yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo cultureInfo)
        {
            return null;
        }
    }
}
