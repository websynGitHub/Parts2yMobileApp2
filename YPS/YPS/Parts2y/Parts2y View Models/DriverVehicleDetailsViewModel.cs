using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_Services;
using YPS.Parts2y.Parts2y_SQLITE;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;
using ZXing;
using ZXing.Net.Mobile.Forms;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Linq;


namespace YPS.Parts2y.Parts2y_View_Models
{
    public class DriverVehicleDetailsViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand ScanCommand { get; set; }
        public ICommand ScantoCommand { get; set; }
        public ICommand QrCodeCommand { get; set; }
        public ICommand VerifyCommand { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand Okaytogo { set; get; }
        public ICommand  BusTapCommand { get; set; }
        DriverVehicleDetails driverpage;
        DriverVehicleDetailsModel result;
        public Command<object> TapGestureCommand { get; set; }

        RestClient service = new RestClient();

        public DriverVehicleDetailsViewModel(INavigation _Navigation,DriverVehicleDetails driverpagecs)
        {
            try
            {

                Navigation = _Navigation;
                BgColor = YPS.CommonClasses.Settings.Bar_Background;
                ScanCommand = new Command(async () => await Scan_Tap());
                ScantoCommand = new Command(async () => await Scanto_Tap());
                QrCodeCommand = new Command(async () => await QRCode_Tap());
                VerifyCommand = new Command(async () => await Verify_Tap());
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                Okaytogo = new Command(async () => await Okaytogo_click());
                BusTapCommand = new Command(async () => await BusSchedule_Click());
                //  GetCurrentLocaation();
                driverpage = driverpagecs;
                if (Settings.IscompltedRecord == 1)
                {
                    QrCodeImageWithDetailsVisibility = true;
                    ScanImagevisable = false;
                    ScanImagestackvisable = true;
                    okaytobuttoncolor = Color.LightGray;
                    okaytobuttondisable = false;
                    BGfromtabcolor = Color.FromHex("#E9E9E9");
                    BGtotabcolor = Color.FromHex("#E9E9E9");
                }
                else
                {
                    ScanImagevisable = false;
                    ScanImagevisable1 = true;
                }
                mapstack = true;
                Dateandtime = ": " + DateTime.Now.ToString("yyyy/MM/dd hh:mm tt");
                VIN_NO = Settings.VINNo;
                HRLtext = Settings.HRLtext;
                Getvindetails();
            }
            catch (Exception ex)
            {

            }
        }

       

        private async Task BusSchedule_Click()
        {
            try
            {
                if (Navigation.ModalStack.Count == 0 ||
                                       Navigation.ModalStack.Last().GetType() != typeof(BusSchedule))
                {
                    await Navigation.PushAsync(new BusSchedule());
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task Okaytogo_click()
        {
            try
            {
                if (Settings.IscompltedRecord != 1)
                {
                    if (btntext != "DONE")
                    {
                        btntext = "DONE";
                        ScanTabVisibility = true;
                        ScanFromBorderVisibility = false;
                        ScanToBorderVisibility = true;
                        ScanFuncVisibility = PDITabVisibility = false;
                        QrCodeImageWithDetailsVisibility = false;
                        BGfromtabcolor = Color.FromHex("#E9E9E9");
                        BGtotabcolor = Color.FromHex("#E9E9E9");
                        ScanFrom_text = Settings.Scantotext;
                        ScanaFrom_Description1 = Settings.Scantodscr;
                        ZoneClusterBayCellfrom = ": "+Settings.ScanFrom_ZoneClusterBayCellto;
                        ScanImagevisable = false;
                        ScanImagevisable1 = true;
                        mapstack = true;
                        scanfromborderBG = Color.Transparent;
                        scantoborderBG = Color.FromHex("#000000");
                        mapModel md = new mapModel();
                        md.Latitude = Convert.ToDecimal(Settings.Sto_latitude);
                        md.Longitude = Convert.ToDecimal(Settings.Sto_longitude);
                        md.SlotColor = "red_marker.png";
                        //md.Name = "TO";
                        md.Name = Settings.Scantotext;
                        md.Address = Settings.Scantodscr;
                        driverpage.mapview(md);
                    }
                    else if (btntext == "DONE")
                    {
                        //CompletedVinList();
                        App.Current.MainPage = new MenuPage(typeof(Driverpage));
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async void CompletedVinList()
        {
            var Driverlist = await service.GetCompletedVinList(Settings.VINNo, Settings.HRLtext);

            var result = JsonConvert.DeserializeObject<VehicleDetailsModel>(Driverlist.ToString());
            if (result != null)
            {
                if (result.status != false)
                {
                }
            }
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
            }

        }

        public async void Getvindetails()
        {
            try
            {
                loadindicator = true;

                result = new DriverVehicleDetailsModel();
                //Vindata vindata = new Vindata();
                DriverDB Db = new DriverDB("drivervehicledetails");
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet)
                {
                    var poData = await service.GetDrivervindetails(Settings.VINNo, Settings.HRLtext);
                    var data = JsonConvert.DeserializeObject<DriverVehicleDetailsModel>(poData.ToString());
                    if (data != null)
                    {
                        if (data.status != false)
                        {
                            result = data;
                            Db.SaveDriverVehicleDetails(data);
                        }
                    }
                }
                else
                {
                    result= Db.GetDriverVehicleDetails(Settings.VINNo, Settings.HRLtext);
                }
                if (result != null)
                {
                    Model= result.Vindata.Modelname;
                    VIN_NO= result.Vindata.Vin;
                    Colour = result.Vindata.Colour;
                    LOAD =result.Vindata.Loadnumber;
                    TripNo = result.Vindata.Trip;

                    ScanFrom_text =  result.Vindata.ScanFrom_text;
                    ScanaFrom_Description1 = result.Vindata.ScanaFrom_Description;

                    ZoneClusterBayCellfrom = ": " + result.Vindata.ScanFrom_ZoneClusterBayCell;
                    VIN_NO1 = ": " + result.Vindata.Vin;
                    Dateandtime = ": " + result.Vindata.ScanFrom_DateTime;
                    Currentlocation = ": " + result.Vindata.ScanFrom_Gpslocation;


                    Settings.VINNo = result.Vindata.Vin;

                    Settings.Scandatefromtime =result.Vindata.ScanFrom_DateTime;
                    Settings.Scandatetotime = result.Vindata.ScanTo_DateTime;

                    Settings.Scanlocationfrom =  result.Vindata.ScanFrom_Gpslocation;
                    Settings.Scanlocationto =  result.Vindata.ScanTo_Gpslocation;

                    Settings.Scanfromtext =  result.Vindata.ScanFrom_text;
                    Settings.Scantotext =  result.Vindata.ScanTo_text;

                    Settings.Scanfromdscr =  result.Vindata.ScanaFrom_Description;
                    Settings.Scantodscr =  result.Vindata.ScanTo_Description;
                    Settings.ScanFrom_ZoneClusterBayCellfrom = result.Vindata.ScanFrom_ZoneClusterBayCell;
                    Settings.ScanFrom_ZoneClusterBayCellto =  result.Vindata.ScanTo_ZoneClusterBayCell;

                    Settings.Sfrom_latitude = result.Vindata.Sfrom_latitude;
                    Settings.Sfrom_longitude = result.Vindata.Sfrom_longitude;

                    Settings.Sto_latitude =  result.Vindata.Sto_latitude;
                    Settings.Sto_longitude =  result.Vindata.Sto_longitude;


                    //foreach (var item in result.Vindata)
                    //{
                    //    VIN_NO = item.Vin;
                    //    Colour = item.Colour;
                    //    CarrierNo = item.Carrier;
                    //}
                    //foreach(var items in result.ModelData)
                    //{
                    //    Model = items.ModelName;
                    //}
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "No data available", "OK");
                    await Navigation.PopAsync();

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task GetCurrentLocaation()
        {
            try
            {
                //var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                //var requestedPermissionStatus = requestedPermissions[Permission.Location];
                //var pass1 = requestedPermissions[Permission.Location];
                //if (pass1 != PermissionStatus.Denied)
                //{
                    // var location = await Geolocation.GetLastKnownLocationAsync();
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    var location1 = await Geolocation.GetLocationAsync(request);
                    if (location1 != null)
                    {
                        //Locationids.Text = ": " + location1.Latitude + ", " + location1.Longitude;
                        Currentlocation = ": " + location1.Latitude + ", " + location1.Longitude;
                    }
                    else
                    {
                        // Vm.Currentlocation = ": 17.4261295,78.4222222";
                    }
                //}
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                //Vm.Currentlocation = ": 17.4261295,78.4222222";
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Vm.Currentlocation = ": 17.4261295,78.4222222";
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Vm.Currentlocation = ": 17.4261295,78.4222222";
                // Handle permission exception
            }
            catch (Exception ex)
            {
                //  Vm.Currentlocation = ": 17.4261295,78.4222222";
                // Unable to get location
            }
            if (Currentlocation == "")
            {
                // Vm.Currentlocation = "17.4261295,78.4222222";
            }
        }

        private async Task QRCode_Tap()
        {
            try
            {
                var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                var requestedPermissionStatus = requestedPermissions[Permission.Camera];
                var pass1 = requestedPermissions[Permission.Camera];
                if (pass1 == Plugin.Permissions.Abstractions.PermissionStatus.Denied)
                {
                    var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the camera to scan.", null, null, "Maybe Later", "Settings");
                    switch (checkSelect)
                    {
                        case "Maybe Later":
                            break;
                        case "Settings":
                            CrossPermissions.Current.OpenAppSettings();
                            break;
                    }
                }
                else if (pass1 == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var ScannerPage = new ZXingScannerPage();

                    ScannerPage.OnScanResult += (result) =>
                    {
                    // Parar de escanear
                    ScannerPage.IsScanning = false;

                    // Alert com o código escaneado
                    Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PopAsync();
                            if (result != null)
                            {
                                if (Settings.VINNo == "MA3NFG81SJC-191204")
                                {
                                    QrCodeImageWithDetailsVisibility1 = true;
                                    QrCodeImageWithDetailsVisibility = false;
                                    ScanImagevisable = false;
                                    ScanImagevisable1 = true;
                                    mapstack = true;

                                }
                                else
                                {
                                    App.Current.MainPage.DisplayAlert("Alert", result.Text, "OK");
                                //ScannedValue = result.Text;
                                //Scanned_Format = result.BarcodeFormat;
                                    QrCodeImageWithDetailsVisibility = true;
                                    Dateandtime = ": " + DateTime.Now.ToString("yyyy/MMM/dd hh:mm");
                                    GetCurrentLocaation();
                                    ScanImagevisable = false;
                                    ScanImagevisable1 = true;
                                    mapstack = true;
                                }

                            //QrCodeButtonVisibility = QrCodeImageWithDetailsVisibility = TickVisibility = false;
                            //QrCodeImageVisibility = true;
                        }
                            else
                            {
                                QrCodeButtonVisibility = true;
                                QrCodeImageVisibility = QrCodeImageWithDetailsVisibility = TickVisibility = false;
                            }
                        });
                    };
                    await Navigation.PushAsync(ScannerPage);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                    //await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to scan, Please allow the camera permission in App Permission settings", "Ok");

                }
            }
            catch (Exception ex)
            {

            }
        }
        public async void GetScanData()
        {
            var poData = await service.GetScandetails("1", Settings.VINNo, Settings.HRLtext);
            var result = JsonConvert.DeserializeObject<ScanDetailsModel>(poData.ToString());
            if (result != null)
            {
                if (result.status != false)
                {

                }
            }
        }
        private async Task Scanto_Tap()
        {
            try
            {
               
                if (Settings.IscompltedRecord == 1)
                {
                    ScanTabVisibility = true;
                    ScanFuncVisibility = PDITabVisibility = false;
                    ScanToBorderVisibility = true;
                    ScanFromBorderVisibility = false;
                    QrCodeImageWithDetailsVisibility = true;
                    BGfromtabcolor = Color.FromHex("#E9E9E9");
                    BGtotabcolor = Color.FromHex("#E9E9E9");
                    btntext = "DONE";
                    mapstack = true;
                    mapModel md = new mapModel();
                    md.Latitude = Convert.ToDecimal(Settings.Sto_latitude);
                    md.Longitude = Convert.ToDecimal(Settings.Sto_longitude);
                    md.SlotColor = "red_marker.png";
                    // md.Name = "To";
                    md.Name = Settings.Scantotext;
                    md.Address= Settings.Scantodscr;
                    driverpage.mapview(md);

                    ScanFrom_text = Settings.Scantotext;
                    ScanaFrom_Description1 = Settings.Scantodscr;

                    VIN_NO1 = ": " + Settings.VINNo;
                    Dateandtime = ": " + Settings.Scandatetotime;
                    Currentlocation = ": " + Settings.Scanlocationto;
                    ZoneClusterBayCellfrom = ": " + Settings.ScanFrom_ZoneClusterBayCellto;
                    scanfromborderBG = Color.Transparent;
                    scantoborderBG = Color.FromHex("#000000");

                }

            }
            catch (Exception ex)
            {

            }
        }
        private async Task Scan_Tap()
        {
            try
            {
                mapModel md = new mapModel();
                md.Latitude = Convert.ToDecimal(Settings.Sfrom_latitude);
                md.Longitude = Convert.ToDecimal(Settings.Sfrom_longitude);
                md.SlotColor = "red_marker.png";
                md.Name = Settings.Scanfromtext;
                md.Address = Settings.Scanfromdscr;
                //md.Name = "From";
                scanfromborderBG = Color.FromHex("#000000");
                scantoborderBG = Color.Transparent;
                driverpage.mapview(md);
                if (Settings.IscompltedRecord != 1)
                {
                    if (ScanTabVisibility == true)
                    {
                        ScanTabVisibility = false;
                        ScanFuncVisibility = PDITabVisibility = true;
                        ScanFrom_text = Settings.Scanfromtext;
                        ScanaFrom_Description1 = Settings.Scanfromdscr;
                        BGfromtabcolor = Color.FromHex("#E9E9E9");
                        BGtotabcolor = Color.LightGray;
                        btntext = "OK TO GO";
                        ZoneClusterBayCellfrom = ": " + Settings.ScanFrom_ZoneClusterBayCellfrom;
                        ScanToBorderVisibility = false;
                        ScanFromBorderVisibility = true;
                        QrCodeImageWithDetailsVisibility = false;


                    }
                }
                else if (Settings.IscompltedRecord == 1)
                {
                    ScanTabVisibility = false;
                    ScanFuncVisibility = PDITabVisibility = true;
                    QrCodeImageWithDetailsVisibility = true;
                    ScanFrom_text = Settings.Scanfromtext;
                    ScanaFrom_Description1 = Settings.Scanfromdscr;
                    BGfromtabcolor = Color.FromHex("#E9E9E9");
                    BGtotabcolor = Color.FromHex("#E9E9E9");
                    VIN_NO1 = ": " + Settings.VINNo;
                    ZoneClusterBayCellfrom = ": " + Settings.ScanFrom_ZoneClusterBayCellfrom;
                    Dateandtime = ": " + Settings.Scandatefromtime;
                    Currentlocation = ": " + Settings.Scanlocationfrom;
                    
                    //mapModel md = new mapModel();
                    //md.Latitude = Convert.ToDecimal(Settings.Sfrom_latitude);
                    //md.Longitude = Convert.ToDecimal(Settings.Sfrom_longitude);
                    //md.SlotColor = "red_marker.png";
                    //md.Name = "From";
                    //driverpage.mapview(md);
                    btntext = "OK TO GO";
                    ScanToBorderVisibility = false;
                    ScanFromBorderVisibility = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task Verify_Tap()
        {
            try
            {
                scannedOn = DateTime.Now.ToString();
                StatusBg = (StatusRslt == "Verified") ? Color.FromHex("#00b300") : Color.Red;
                StatusSource = (StatusRslt == "Verified") ? "tickgreen.png" : "crossic.png";
                TickVisibility = true;
                QrCodeButtonVisibility = false;
                QrCodeImageVisibility = false;
                QrCodeImageWithDetailsVisibility = true;
            }
            catch (Exception ex)
            {

            }
        }

        #region properties
        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator;
            set
            {
                _loadindicator = value;
                NotifyPropertyChanged("loadindicator");
            }
        }

        private bool _ScanTabVisibility = false;
        public bool ScanTabVisibility
        {
            get => _ScanTabVisibility;
            set
            {
                _ScanTabVisibility = value;
                OnPropertyChanged("ScanTabVisibility");
            }
        }

        private bool _ScanImagevisable = true;
        public bool ScanImagevisable
        {
            get => _ScanImagevisable;
            set
            {
                _ScanImagevisable = value;
                OnPropertyChanged("ScanImagevisable");
            }
        }

        private bool _ScanImagevisable1 = false;
        public bool ScanImagevisable1
        {
            get => _ScanImagevisable1;
            set
            {
                _ScanImagevisable1 = value;
                OnPropertyChanged("ScanImagevisable1");
            }
        }

        private bool _mapstack = false;
        public bool mapstack
        {
            get => _mapstack;
            set
            {
                _mapstack = value;
                OnPropertyChanged("mapstack");
            }
        }

        private bool _ScanImagestackvisable = false;
        public bool ScanImagestackvisable
        {
            get => _ScanImagestackvisable;
            set
            {
                _ScanImagestackvisable = value;
                OnPropertyChanged("ScanImagestackvisable");
            }
        }

        private Color _okaytobuttoncolor = Color.FromHex("#005800");
        public Color okaytobuttoncolor
        {
            get => _okaytobuttoncolor;
            set
            {
                _okaytobuttoncolor = value;
                OnPropertyChanged("okaytobuttoncolor");
            }
        }

        private Color _scantoborderBG = Color.Transparent;
        public Color scantoborderBG
        {
            get => _scantoborderBG;
            set
            {
                _scantoborderBG = value;
                OnPropertyChanged("scantoborderBG");
            }
        }

        private Color _scanfromborderBG = Color.FromHex("#000000");
        public Color scanfromborderBG
        {
            get => _scanfromborderBG;
            set
            {
                _scanfromborderBG = value;
                OnPropertyChanged("scanfromborderBG");
            }
        }

        private bool _okaytobuttondisable = true;
        public bool okaytobuttondisable
        {
            get => _okaytobuttondisable;
            set
            {
                _okaytobuttondisable = value;
                OnPropertyChanged("okaytobuttondisable");
            }
        }

        private bool _PDITabVisibility = true;
        public bool PDITabVisibility
        {
            get => _PDITabVisibility;
            set
            {
                _PDITabVisibility = value;
                OnPropertyChanged("PDITabVisibility");
            }
        }
        private bool _LoadTabVisibility = true;
        public bool LoadTabVisibility
        {
            get => _LoadTabVisibility;
            set
            {
                _LoadTabVisibility = value;
                OnPropertyChanged("LoadTabVisibility");
            }
        }
        private bool _QrCodeImageVisibility = false;
        public bool QrCodeImageVisibility
        {
            get => _QrCodeImageVisibility;
            set
            {
                _QrCodeImageVisibility = value;
                OnPropertyChanged("QrCodeImageVisibility");
            }
        }

        private bool _QrCodeButtonVisibility = true;
        public bool QrCodeButtonVisibility
        {
            get => _QrCodeButtonVisibility;
            set
            {
                _QrCodeButtonVisibility = value;
                OnPropertyChanged("QrCodeButtonVisibility");
            }
        }
        private bool _QrCodeImageWithDetailsVisibility = false;
        public bool QrCodeImageWithDetailsVisibility
        {
            get => _QrCodeImageWithDetailsVisibility;
            set
            {
                _QrCodeImageWithDetailsVisibility = value;
                OnPropertyChanged("QrCodeImageWithDetailsVisibility");
            }
        }

        private bool _QrCodeImageWithDetailsVisibility1 = false;
        public bool QrCodeImageWithDetailsVisibility1
        {
            get => _QrCodeImageWithDetailsVisibility1;
            set
            {
                _QrCodeImageWithDetailsVisibility1 = value;
                OnPropertyChanged("QrCodeImageWithDetailsVisibility1");
            }
        }
        private bool _BusSchesulePopup = false;
        public bool BusSchesulePopup
        {
            get => _BusSchesulePopup;
            set
            {
                _BusSchesulePopup = value;
                OnPropertyChanged("BusSchesulePopup");
            }
        }
        

        //private bool isAnalyzing = true;
        //public bool IsAnalyzing
        //{
        //    get { return this.isAnalyzing; }
        //    set
        //    {
        //        if (!bool.Equals(this.isAnalyzing, value))
        //        {
        //            this.isAnalyzing = value;
        //            this.OnPropertyChanged(nameof(IsAnalyzing));
        //        }
        //    }
        //}
        //private bool isScanning = true;
        //public bool IsScanning
        //{
        //    get { return this.isScanning; }
        //    set
        //    {
        //        if (!bool.Equals(this.isScanning, value))
        //        {
        //            this.isScanning = value;
        //            this.OnPropertyChanged(nameof(IsScanning));
        //        }
        //    }
        //}

        private string _ScannedValue;
        public string ScannedValue
        {
            get => _ScannedValue;
            set
            {
                _ScannedValue = value;
                OnPropertyChanged("ScannedValue");
            }
        }

        private BarcodeFormat _Scanned_Format;
        public BarcodeFormat Scanned_Format
        {
            get => _Scanned_Format;
            set
            {
                _Scanned_Format = value;
                OnPropertyChanged("Scanned_Format");
            }
        }

        private string _ZoneClusterBayCellfrom;
        public string ZoneClusterBayCellfrom
        {
            get => _ZoneClusterBayCellfrom;
            set
            {
                _ZoneClusterBayCellfrom = value;
                OnPropertyChanged("ZoneClusterBayCellfrom");
            }
        }
        private string _StatusRslt = "Verified";
        public string StatusRslt
        {
            get => _StatusRslt;
            set
            {
                _StatusRslt = value;
                OnPropertyChanged("StatusRslt");
            }
        }
        private Color _StatusBg;
        public Color StatusBg
        {
            get => _StatusBg;
            set
            {
                _StatusBg = value;
                OnPropertyChanged("StatusBg");
            }
        }

        private Color _BGfromtabcolor = Color.FromHex("#E9E9E9");
        public Color BGfromtabcolor
        {
            get => _BGfromtabcolor;
            set
            {
                _BGfromtabcolor = value;
                OnPropertyChanged("BGfromtabcolor");
            }
        }
        private Color _BGtotabcolor = Color.LightGray;
        public Color BGtotabcolor
        {
            get => _BGtotabcolor;
            set
            {
                _BGtotabcolor = value;
                OnPropertyChanged("BGtotabcolor");
            }
        }

        private string _StatusSource;
        public string StatusSource
        {
            get => _StatusSource;
            set
            {
                _StatusSource = value;
                OnPropertyChanged("StatusSource");
            }
        }

        private string _btntext = "OK TO GO";
        public string btntext
        {
            get => _btntext;
            set
            {
                _btntext = value;
                OnPropertyChanged("btntext");
            }
        }
        private Color _TickBg;
        public Color TickBg
        {
            get => _TickBg;
            set
            {
                _TickBg = value;
                OnPropertyChanged("TickBg");
            }
        }

        private bool _TickVisibility = false;
        public bool TickVisibility
        {
            get => _TickVisibility;
            set
            {
                _TickVisibility = value;
                OnPropertyChanged("TickVisibility");
            }
        }

        private string _VIN_NO;
        public string VIN_NO
        {
            get => _VIN_NO;
            set
            {
                _VIN_NO = value;
                OnPropertyChanged("VIN_NO");
            }
        }

        private string _VIN_NO1;
        public string VIN_NO1
        {
            get => _VIN_NO1;
            set
            {
                _VIN_NO1 = value;
                OnPropertyChanged("VIN_NO1");
            }
        }

        private string _HRLtext;
        public string HRLtext
        {
            get => _HRLtext;
            set
            {
                _HRLtext = value;
                OnPropertyChanged("_HRLtext");
            }
        }

        private string _Currentlocation;
        public string Currentlocation
        {
            get => _Currentlocation;
            set
            {
                _Currentlocation = value;
                OnPropertyChanged("Currentlocation");
            }
        }

        private string _scannedOn;
        public string scannedOn
        {
            get => _scannedOn;
            set
            {
                _scannedOn = value;
                OnPropertyChanged("scannedOn");
            }
        }

        private string _Dateandtime;
        public string Dateandtime
        {
            get => _Dateandtime;
            set
            {
                _Dateandtime = value;
                OnPropertyChanged("Dateandtime");
            }
        }


        private Color _BgColor= YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }

        private bool _ScanFuncVisibility = true;
        public bool ScanFuncVisibility
        {
            get => _ScanFuncVisibility;
            set
            {
                _ScanFuncVisibility = value;
                OnPropertyChanged("ScanFuncVisibility");
            }
        }

        private bool _PDIFuncVisibility = false;
        public bool PDIFuncVisibility
        {
            get => _PDIFuncVisibility;
            set
            {
                _PDIFuncVisibility = value;
                OnPropertyChanged("PDIFuncVisibility");
            }
        }
        private bool _LoadFuncVisibility = false;
        public bool LoadFuncVisibility
        {
            get => _LoadFuncVisibility;
            set
            {
                _LoadFuncVisibility = value;
                OnPropertyChanged("LoadFuncVisibility");
            }
        }
        private string _LOAD;
        public string LOAD
        {
            get => _LOAD;
            set
            {
                _LOAD = value;
                OnPropertyChanged("LOAD");
            }
        }
        private string _TripNo;
        public string TripNo
        {
            get => _TripNo;
            set
            {
                _TripNo = value;
                OnPropertyChanged("TripNo");
            }
        }

        private string _Colour;
        public string Colour
        {
            get => _Colour;
            set
            {
                _Colour = value;
                OnPropertyChanged("Colour");
            }
        }

        private string _Model;
        public string Model
        {
            get => _Model;
            set
            {
                _Model = value;
                OnPropertyChanged("Model");
            }
        }
        private string _ScanFrom_text;
        public string ScanFrom_text
        {
            get => _ScanFrom_text;
            set
            {
                _ScanFrom_text = value;
                OnPropertyChanged("ScanFrom_text");
            }
        }


        private string _ScanaFrom_Description1;
        public string ScanaFrom_Description1
        {
            get => _ScanaFrom_Description1;
            set
            {
                _ScanaFrom_Description1 = value;
                OnPropertyChanged("ScanaFrom_Description1");
            }
        }
        private bool _ScanFromBorderVisibility = true;
        public bool ScanFromBorderVisibility
        {
            get => _ScanFromBorderVisibility;
            set
            {
                _ScanFromBorderVisibility = value;
                OnPropertyChanged("ScanFromBorderVisibility");
            }
        }
        private bool _ScanToBorderVisibility = false;
        public bool ScanToBorderVisibility
        {
            get => _ScanToBorderVisibility;
            set
            {
                _ScanToBorderVisibility = value;
                OnPropertyChanged("ScanToBorderVisibility");
            }
        }
        
            

        #endregion
    }
}
