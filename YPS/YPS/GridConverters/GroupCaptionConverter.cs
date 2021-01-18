using Syncfusion.Data;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace YPS.GridConverters
{
    public class GroupCaptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var data = value != null ? value as Group : null;
                var tt = data.Source[0] as YPS.Model.AllPoData;
                var poShipNumber = tt.POShippingNumber.ToString();
                return data != null ? poShipNumber : null;
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
