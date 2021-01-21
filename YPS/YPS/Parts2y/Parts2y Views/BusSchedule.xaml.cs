using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BusSchedule : ContentPage
    {
        BusScheduleViewModel Vm;
        public  Map StoresMap;
        public string popup;
        public ObservableCollection<CustomPin> locationListDetails;
        public ObservableCollection<BusDetails> BusDetailsList;
        public BusSchedule()
        {
            InitializeComponent();
            //StoresMap = mapControl;
            BindingContext = Vm = new BusScheduleViewModel(Navigation);
            locationListDetails = new ObservableCollection<CustomPin>();
            BusDetailsList = new ObservableCollection<BusDetails>();
            MapView();
            BusList.ItemTapped += (s, e) => BusList.SelectedItem = null;

            //mapControl.MoveToRegion()
        }

        public async void MapView()
        {
            try
            {
                locationListDetails.Add(new CustomPin { Description = "H–01–12", AddressInfo = "NEAR H/A-2 Plant-1", Lattitudee=28.497242, Longitude=77.069627 });
                locationListDetails.Add(new CustomPin { Description = "R07–02–01", AddressInfo = "NEAR R/T-3 Office-1", Lattitudee=28.498046, Longitude=77.068779 });
                locationListDetails.Add(new CustomPin { Description = "L05–01–01", AddressInfo = "NEAR EXIT GATE", Lattitudee=28.495123, Longitude = 77.062196});
                locationListDetails.Add(new CustomPin { Description = "H–01–04", AddressInfo = "NEAR H/O-1 Plant-3", Lattitudee=28.4968602, Longitude = 77.0738165});

                BusDetailsList.Add(new BusDetails() { Bus_No = "10:00 AM", BusArriving_Time = "10:15 AM", BusDeparture_Time = "10:30 AM" });
                BusDetailsList.Add(new BusDetails() { Bus_No = "11:00 AM", BusArriving_Time = "11:10 AM", BusDeparture_Time = "11:20 AM" });
                BusDetailsList.Add(new BusDetails() { Bus_No = "12:00 AM", BusArriving_Time = "12:30 AM", BusDeparture_Time = "12:40 AM" });
                BusDetailsList.Add(new BusDetails() { Bus_No = "1:20 PM", BusArriving_Time = "1:40 PM", BusDeparture_Time = "1:55 PM" });
                BusDetailsList.Add(new BusDetails() { Bus_No = "1:30 PM", BusArriving_Time = "1:35 PM", BusDeparture_Time = "1:45 PM" });
                mapControl.CustomPins = new List<CustomPin>();
                var latitudes = new List<double>();
                var longitudes = new List<double>();
                if (locationListDetails.Count > 0)
                {
                    for (int i = 0; i < locationListDetails.Count; i++)
                    {
                        latitudes.Add(Convert.ToDouble(locationListDetails[i].Lattitudee));
                        longitudes.Add(Convert.ToDouble(locationListDetails[i].Longitude));
                        var pin = new CustomPin
                        {
                            Type = PinType.Place,
                            //gvmPatientAppointments = gvmPatientAppointments,
                            Position = new Position(Convert.ToDouble(locationListDetails[i].Lattitudee), Convert.ToDouble(locationListDetails[i].Longitude)),
                            Label = locationListDetails[i].Description,// + "(SGD " + ovmAppointmentSlots.AvailableSlots[i].Price + ")",
                            BindingContext = locationListDetails[i],
                            Address= locationListDetails[i].AddressInfo,


                        };
                        pin.MarkerClicked += Marker_Tapped;
                        pin.InfoWindowClicked += InfoWindow_Tapped;
                        mapControl.Pins.Add(pin);
                        mapControl.CustomPins.Add(pin);
                       // mapControl.Navigation = this.Navigation;
                    }
                }
                double lowestLat = latitudes.Min();
                double highestLat = latitudes.Max();
                double lowestLong = longitudes.Min();
                double highestLong = longitudes.Max();
                double finalLat = (lowestLat + highestLat) / 2;
                double finalLong = (lowestLong + highestLong) / 2;
                double distance = GeoCodeCalc.CalcDistance(lowestLat, lowestLong, highestLat, highestLong, GeoCodeCalcMeasurement.Miles);
                mapControl.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(finalLat, finalLong), Distance.FromKilometers(distance)));
            }
            catch (Exception ex)
            {

            }
        }
        //public async void MapMoving(double finalLat, double finalLong, double distance)
        //{
        //    try
        //    {
        //        mapControl.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(finalLat, finalLong), Distance.FromMiles(distance)));
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        private void Marker_Tapped(object sender, PinClickedEventArgs e)
        {
            try
            {
                Vm.PopUpVisibility = false;
            }
            catch (Exception ex)
            {

            }
        }

        private void InfoWindow_Tapped(object sender, PinClickedEventArgs e)
        {
            try
            {
                var data= sender as Pin;
               // bool containsInt = "your string".Any(char.IsDigit)
                Vm.BusDetails_No = data.Label;
                Vm.BusDetails_address = data.Address;
                Vm.ListOfBuses = BusDetailsList;
                double height = App.ScreenHeight / 2.4, width = App.ScreenWidth;
                popup = "Open";
                AbsoluteLayout.SetLayoutBounds(Mainframe, new Rectangle(0, 1, width, height));
                AbsoluteLayout.SetLayoutFlags(Mainframe, AbsoluteLayoutFlags.PositionProportional);
                Vm.PopUpVisibility = true;
            }
            catch (Exception ex)
            {

            }
        }
        double x = 0;
        double y = 0;
        bool gestValue = true;
        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            try
            {
                x = e.TotalX;
                y = e.TotalY;

                if (e.StatusType.Equals(GestureStatus.Running))
                {
                    if (y > 0)
                    {
                        if (gestValue)
                        {
                            double height = App.ScreenHeight / 2.4, width = App.ScreenWidth;

                            if (popup == "FullScreen")
                            {
                                AbsoluteLayout.SetLayoutBounds(Mainframe, new Rectangle(0, 1, width, height));
                                AbsoluteLayout.SetLayoutFlags(Mainframe, AbsoluteLayoutFlags.PositionProportional);
                                popup = "Open";
                            }
                            else
                            {
                                Vm.PopUpVisibility = false;
                            }
                            gestValue = false;
                            // frameMesurments.HeightRequest = App.ScreenHeight / 3;
                        }
                    }
                    else
                    {
                        if (gestValue)
                        {
                            // int height = App.ScreenHeight - 50, width = App.ScreenWidth;

                            gestValue = false;
                            popup = "FullScreen";
                            AbsoluteLayout.SetLayoutBounds(Mainframe, new Rectangle(0, 0, 1, 1));
                            AbsoluteLayout.SetLayoutFlags(Mainframe, AbsoluteLayoutFlags.All);
                        }
                    }
                }
                else if (e.StatusType.Equals(GestureStatus.Completed))
                {
                    gestValue = true;
                }
                else if (e.StatusType.Equals(GestureStatus.Canceled))
                {
                    gestValue = true;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}