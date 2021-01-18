using Syncfusion.Data;
using System;
using System.Globalization;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;

namespace YPS.GridConverters
{
    class poShipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var data = value != null ? value as Group : null;
                return data != null ? data.Key.ToString() : null;
            }
            catch (Exception ex)
            {
                Service.YPSService service = new Service.YPSService();
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "poShipConverter-> in Convert " + Settings.userLoginID);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
