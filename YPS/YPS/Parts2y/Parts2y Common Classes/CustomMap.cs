using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using YPS.Parts2y.Parts2y_Models;

namespace YPS.Parts2y.Parts2y_Common_Classes
{
   public class CustomMap:Map
    {
        public List<CustomPin> CustomPins { get; set; }
        public INavigation Navigation { get; set; }

        
    }

    public class CustomPin : Pin
    {
        public string Id { get; set; }

        public string Url { get; set; }
        public int ColorNumber { get; set; }
        public string hexStrin { get; set; }

        public string AddressInfo { get; set; }
        public string Description { get; set; }

        public double Lattitudee { get; set; }
        public double Longitude { get; set; }
        //public Position Position { get; set; }

        public ObservableCollection<BusDetails> busDetailsList { get; set; }
        //public string ScanCentreImage { set; get; }
        //public vmPatientAppointments gvmPatientAppointments { get; set; }
    }

    public static class GeoCodeCalc
    {
        public const double EarthRadiusInMiles = 3956.0;
        public const double EarthRadiusInKilometers = 6367.0;

        public static double ToRadian(double val) { return val * (Math.PI / 180); }
        public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
        {
            return CalcDistance(lat1, lng1, lat2, lng2, GeoCodeCalcMeasurement.Miles);
        }

        public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeoCodeCalcMeasurement m)
        {
            double radius = GeoCodeCalc.EarthRadiusInMiles;

            if (m == GeoCodeCalcMeasurement.Kilometers) { radius = GeoCodeCalc.EarthRadiusInKilometers; }
            return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
        }
    }

    public enum GeoCodeCalcMeasurement : int
    {
        Miles = 0,
        Kilometers = 1
    }
}
