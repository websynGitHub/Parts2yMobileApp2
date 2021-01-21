using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class BusScheduleViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand BusBackTapCommand { get; set; }
        //public ObservableCollection<LocationDetails> locationListDetails;
        //public ObservableCollection<BusDetails> BusDetailsList;
        //BusSchedule busSchedule;
        public BusScheduleViewModel(INavigation _Navigation)
        {
            Navigation = _Navigation;
            //busSchedule = busSchedulePage;
            BgColor = Settings.Bar_Background;
            //var latitudes = new List<double>();
            //var longitudes = new List<double>();
            BusBackTapCommand = new Command(async () => await BusBackTap_Click());
            //locationListDetails = new ObservableCollection<LocationDetails>();
            //BusDetailsList = new ObservableCollection<BusDetails>();
            //locationListDetails.Add(new LocationDetails { Description= "H–01–12", Address= "NEAR H/A-2 Plant-1", Position=new Position(28.497242, 77.069627) });
            //locationListDetails.Add(new LocationDetails { Description = "R07–02–01", Address = "NEAR R/T-3 Office-1", Position = new Position(28.498046, 77.068779) });
            //locationListDetails.Add(new LocationDetails { Description = "L05–01–01", Address = "NEAR EXIT GATE", Position = new Position(28.495123, 77.062196) });
            //locationListDetails.Add(new LocationDetails { Description = "H–01–04", Address = "NEAR H/O-1 Plant-3", Position = new Position(28.4968602, 77.0738165) });
            ////locationListDetails.Add(new LocationDetails { Description = "China", Address = "China", Position = new Position(35.8617, 104.1954) });
            ////locationListDetails.Add(new LocationDetails { Description = "Pakistan", Address = "Pakistan", Position = new Position(30.3753, 69.3451) });


            //BusDetailsList.Add(new BusDetails() { Bus_No = "10:00 AM", BusArriving_Time = "10:15 AM", BusDeparture_Time="10:30 AM" });
            //BusDetailsList.Add(new BusDetails() { Bus_No = "11:00 AM", BusArriving_Time = "11:10 AM", BusDeparture_Time = "11:20 AM" });
            //BusDetailsList.Add(new BusDetails() { Bus_No = "12:00 AM", BusArriving_Time = "12:30 AM", BusDeparture_Time = "12:40 AM" });
            //BusDetailsList.Add(new BusDetails() { Bus_No = "1:20 PM", BusArriving_Time = "1:40 PM", BusDeparture_Time = "1:55 PM" });
            //BusDetailsList.Add(new BusDetails() { Bus_No = "1:30 PM", BusArriving_Time = "1:35 PM", BusDeparture_Time = "1:45 PM" });
            //LocationsList = locationListDetails;
            //latitudes = LocationsList.Select(x => x.Position.Latitude).ToList();
            //longitudes= LocationsList.Select(x => x.Position.Longitude).ToList();
            //double lowestLat = latitudes.Min();
            //double highestLat = latitudes.Max();
            //double lowestLong = longitudes.Min();
            //double highestLong = longitudes.Max();
            //double finalLat = (lowestLat + highestLat) / 2;
            //double finalLong = (lowestLong + highestLong) / 2;
            //double distance = GeoCodeCalc.CalcDistance(lowestLat, lowestLong, highestLat, highestLong, GeoCodeCalcMeasurement.Kilometers);
            ////busSchedule.MapMoving(finalLat, finalLong, distance);
            // //busSchedule.StoresMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Convert.ToDouble("17.2543"), Convert.ToDouble(" 78.6808")), Distance.FromMiles(10)));
        }
        private async Task BusBackTap_Click()
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {

            }
        }

        //private ObservableCollection<LocationDetails> _LocationsList;
        //public ObservableCollection<LocationDetails> LocationsList
        //{
        //    get => _LocationsList;
        //    set
        //    {
        //        _LocationsList = value;
        //        OnPropertyChanged("LocationsList");
        //    }
        //}
        private Color _BgColor;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }
        private bool _PopUpVisibility = false;
        public bool PopUpVisibility
        {
            get => _PopUpVisibility;
            set
            {
                _PopUpVisibility = value;
                OnPropertyChanged("PopUpVisibility");
            }
        }
        private string _BusDetails_No;
        public string BusDetails_No
        {
            get => _BusDetails_No;
            set
            {
                _BusDetails_No = value;
                OnPropertyChanged("BusDetails_No");
            }
        }
        private string _BusDetails_address;
        public string BusDetails_address
        {
            get => _BusDetails_address;
            set
            {
                _BusDetails_address = value;
                OnPropertyChanged("BusDetails_address");
            }
        }
        private ObservableCollection<BusDetails> _ListOfBuses;
        public ObservableCollection<BusDetails> ListOfBuses
        {
            get => _ListOfBuses;
            set
            {
                _ListOfBuses = value;
                OnPropertyChanged("ListOfBuses");
            }
        }
        
    }
}
