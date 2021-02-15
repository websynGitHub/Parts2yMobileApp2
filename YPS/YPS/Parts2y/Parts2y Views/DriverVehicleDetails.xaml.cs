using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DriverVehicleDetails : ContentPage
    {
        DriverVehicleDetailsViewModel Vm;

        public DriverVehicleDetails()
        {
            try
            {
                InitializeComponent();
                BindingContext = Vm = new DriverVehicleDetailsViewModel(Navigation, this);
                mapModel md = new mapModel();
                md.Latitude = Convert.ToDecimal(Settings.Sfrom_latitude);
                md.Longitude = Convert.ToDecimal(Settings.Sfrom_longitude);
                md.SlotColor = "marker.png";
                //md.Name = "FROM";
                md.Name = Settings.Scanfromtext;
                md.Address = Settings.Scanfromdscr;
                mapview(md);
            }
            catch (Exception ex)
            {

            }

        }

        public async void mapview(mapModel ovmAppointmentSlots)
        {
            try
            {
                //var requestedPermissions = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                //if (requestedPermissions != PermissionStatus.Granted)
                //{
                //    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                //    {
                //        await DisplayAlert("Need location", "SmartYard need that location", "OK");
                //    }

                //    //var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                //    //requestedPermissions = results[Permission.Location];
                //    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                //    requestedPermissions = results[Permission.Location];
                //}
                //if (requestedPermissions == PermissionStatus.Granted)
                //{
                //Permission granted, do what you want do.
                var customMap = new CustomMap();
                customMap.SetBinding(CustomMap.IsVisibleProperty, "mapstack");
                customMap.MapType = MapType.Street;
                customMap.IsShowingUser = true;
                customMap.HorizontalOptions = LayoutOptions.FillAndExpand; customMap.VerticalOptions = LayoutOptions.FillAndExpand;
                MapGrid.Children.Clear();
                customMap.CustomPins = new List<CustomPin>();

                var latitudes = new List<double>();
                var longitudes = new List<double>();

                if (ovmAppointmentSlots.Latitude != 0)
                {
                    int ColorNumber = 0;
                    string HexaColor = string.Empty;
                    if (ovmAppointmentSlots.SlotColor.ToLower().Contains("marker.png"))
                    {
                        ColorNumber = 0;
                        HexaColor = "red";
                    }
                    latitudes.Add(Convert.ToDouble(ovmAppointmentSlots.Latitude));
                    longitudes.Add(Convert.ToDouble(ovmAppointmentSlots.Longitude));

                    var pin = new CustomPin
                    {
                        Type = PinType.Place,
                        //gvmPatientAppointments = gvmPatientAppointments,
                        Position = new Position(Convert.ToDouble(ovmAppointmentSlots.Latitude), Convert.ToDouble(ovmAppointmentSlots.Longitude)),
                        Label = ovmAppointmentSlots.Name,
                        Address = ovmAppointmentSlots.Address,// + "(SGD " + ovmAppointmentSlots.AvailableSlots[i].Price + ")",
                        BindingContext = ovmAppointmentSlots,
                        ColorNumber = ColorNumber,
                        // ScanCentreImage = GlobalSettings.ScanCentreIcon_UAT+ ((!String.IsNullOrEmpty(ovmAppointmentSlots.AvailableSlots[i].LogoName))? ovmAppointmentSlots.AvailableSlots[i].LogoName: "favicon.ico"),
                        hexStrin = HexaColor,

                    };
                    customMap.Pins.Clear();
                    customMap.Pins.Add(pin);
                    customMap.CustomPins.Add(pin);
                    MapGrid.Children.Add(customMap);
                    pin.MarkerClicked += Pins_Clicked;
                    //customMap.Navigation = this.Navigation;
                }
                double lowestLat = latitudes.Min();
                double highestLat = latitudes.Max();
                double lowestLong = longitudes.Min();
                double highestLong = longitudes.Max();
                double finalLat = (lowestLat + highestLat) / 2;
                double finalLong = (lowestLong + highestLong) / 2;
                double distance = GeoCodeCalc.CalcDistance(lowestLat, lowestLong, highestLat, highestLong, GeoCodeCalcMeasurement.Kilometers);


                if (ovmAppointmentSlots.Latitude == 0)
                {
                    //customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(latitudes.FirstOrDefault(), longitudes.FirstOrDefault()), Distance.FromKilometers(10)));
                    customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Convert.ToDouble("28.4977705"), Convert.ToDouble("77.0719749")), Distance.FromMeters(100)));

                }
                else
                {
                    customMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(finalLat, finalLong), Distance.FromMeters(100)));
                }
                //}
                //else
                //{
                //    await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to scan, Please allow the camera permission in App Permission settings", "Ok");
                //}
            }
            catch (Exception ex)
            {
                App.Current.MainPage.DisplayAlert("Map Page", "Something went worng  while map loading.", "ok");
            }
        }
        private async void Pins_Clicked(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {

            }
        }
        private void QrCodeCommand(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Gets called when clicked on "Home" icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToHome_Tapped(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
    }
}