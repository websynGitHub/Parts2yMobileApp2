using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_Services;
using YPS.Parts2y.Parts2y_SQLITE;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class DriverPageViewModel : BaseViewModel
    {

        public INavigation Navigation { get; set; }
        public ICommand InProgressCmd { get; set; }
        public ICommand CompletedCmd { get; set; }
        public ICommand PendingCmd { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand AllCmd { set; get; }
        public ICommand Backevnttapped { set; get; }

        RestClient service = new RestClient();
        public int INP, Cmp, Pending, All = 0;
        Drivervindata result;
        public DriverPageViewModel(INavigation _Navigation)
        {
            Navigation = _Navigation;
            BgColor = Settings.Bar_Background;
            InProgressCmd = new Command(async () => await InProgress_Tap());
            CompletedCmd = new Command(async () => await Complete_Tap());
            PendingCmd = new Command(async () => await Pending_Tap());
            AllCmd = new Command(async () => await All_Tap());
            HomeCommand = new Command(async () => await Home_Tap());
            Backevnttapped = new Command(async () => await Backevnttapped_click());
            GetLoadnumberdata(Settings.UserMail);
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

        public async void GetLoadnumberdata(string mail)
        {
            try
            {
                loadindicator = true;

                result = new Drivervindata();
                DriverDB Db = new DriverDB("drivervindata");
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.Internet)
                {
                    var Vindata = await service.Getdrivervindata(mail);
                    var data = JsonConvert.DeserializeObject<Drivervindata>(Vindata);
                    if (data != null)
                    {
                        if (data.status != false)
                        {
                            result = data;
                            Db.SaveDriverDetails(data);
                        }
                    }
                }
                else
                {
                    result = Db.GetDriverDetails();
                }
                if (result != null)
                {
                    LoadnumberList = result.Vinlist;
                    Settings.jobcount = result.Jobcount;
                    TransportRep  = result.TransportRep;
                    JobDone =  result.Jobcount;
                    foreach (var item in LoadnumberList)
                    {
                        if (item.IsCompleted == 2)
                        {
                            item.Status_icon = Icons.Tickicon;
                        }
                        else if (item.IsCompleted == 1)
                        {
                            item.Status_icon = Icons.CheckCircle;
                        }
                        else if (item.IsCompleted == 0)
                        {
                            item.Status_icon = Icons.circle;
                        }
                        item.bgteamcolor = Settings.Bar_Background.ToHex();
                    }
                    INP = LoadnumberList.Where(x => x.IsCompleted == 2).Count();
                    Cmp = LoadnumberList.Where(x => x.IsCompleted == 1).Count();
                    Pending = LoadnumberList.Where(x => x.IsCompleted == 0).Count();
                    All = LoadnumberList.Count();
                    Cmptext = "COMPLETED " + "(" + Cmp.ToString() + ")";
                    INPtext = "IN PROGRESS " + "(" + INP.ToString() + ")";
                    Pendingtext = "PENDING " + "(" + Pending.ToString() + ")";
                    Alltext = "ALL\n" + "(" + All.ToString() + ")";
                    InProgressList = LoadnumberList;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "No data available", "OK");
                    await Navigation.PopModalAsync();

                }
                loadindicator = false;
            }
            catch (Exception ex)
            {

            }
        }
        private async Task InProgress_Tap()
        {
            try
            {
                BgtabColor1 = Color.FromHex("#E9E9E9");
                BgtabColor = BgtabColor2 = BgtabColor3 = Color.White;
                InProgressList = LoadnumberList.Where(x => x.IsCompleted == 2).ToList();
                InProgressTabVisibility = true;
                CompleteTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                InProgressTxtColor = Settings.Bar_Background;
                CompletedTxtColor = PendingTxtColor = AllTxtColor = Color.Black;
            }
            catch (Exception ex)
            {

            }
        }
        private async Task Complete_Tap()
        {
            try
            {
                BgtabColor2 = Color.FromHex("#E9E9E9");
                BgtabColor = BgtabColor1 = BgtabColor3 = Color.White;
                InProgressList = LoadnumberList.Where(x => x.IsCompleted == 1).ToList();
                CompleteTabVisibility = true;
                InProgressTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                CompletedTxtColor = Settings.Bar_Background;
                InProgressTxtColor = PendingTxtColor = AllTxtColor = Color.Black;

            }
            catch (Exception ex)
            {

            }
        }

        private async Task Pending_Tap()
        {
            try
            {
                BgtabColor3 = Color.FromHex("#E9E9E9");
                BgtabColor1 = BgtabColor = BgtabColor2 = Color.White;
                InProgressList = LoadnumberList.Where(x => x.IsCompleted == 0).ToList();
                PendingTabVisibility = true;
                CompleteTabVisibility = InProgressTabVisibility = AllTabVisibility = false;
                PendingTxtColor = Settings.Bar_Background;
                InProgressTxtColor = CompletedTxtColor = AllTxtColor = Color.Black;
            }
            catch (Exception ex)
            {

            }
        }
        private async Task All_Tap()
        {
            try
            {
                BgtabColor = Color.FromHex("#E9E9E9");
                BgtabColor1 = BgtabColor2 = BgtabColor3 = Color.White;
                InProgressList = LoadnumberList;
                AllTabVisibility = true;
                InProgressTabVisibility = CompleteTabVisibility = PendingTabVisibility = false;
                AllTxtColor = Settings.Bar_Background;
                InProgressTxtColor = CompletedTxtColor = PendingTxtColor = Color.Black;
            }
            catch (Exception ex)
            {

            }
        }
        private async void VinTapped()
        {
            try
            {
                if (VehicleDetails != null)
                {
                    loadindicator = true;

                    var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    var requestedPermissionStatus = requestedPermissions[Permission.Location];
                    var pass1 = requestedPermissions[Permission.Location];
                    if (pass1 == Plugin.Permissions.Abstractions.PermissionStatus.Denied)
                    {
                        var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the location .", null, null, "Maybe Later", "Settings");
                        switch (checkSelect)
                        {
                            case "Maybe Later":
                                break;
                            case "Settings":
                                CrossPermissions.Current.OpenAppSettings();
                                break;
                        }
                        loadindicator = false; ;
                    }
                    else if (pass1 == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                    {

                        Settings.HRLtext = VehicleDetails.Handover_Retrieval_Loading;
                        Settings.VINNo = VehicleDetails.Vin;
                        Settings.IscompltedRecord = VehicleDetails.IsCompleted;

                        if (Navigation.ModalStack.Count == 0 ||
                                      Navigation.ModalStack.Last().GetType() != typeof(DriverVehicleDetails))
                        {
                            await Navigation.PushModalAsync(new DriverVehicleDetails());
                        }
                        loadindicator = false; ;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", "Unable to load location!", "OK");
                        //await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to access location, Please allow the location permission in App Permission settings", "Ok");
                    }
                    loadindicator = false; ;
                }

            }
            catch (Exception ex)
            {

            }
        }

        private async Task Home_Tap()
        {
            try
            {
                App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {

            }
        }
        #region Properties
        private List<Vinlist> _InProgressList;
        public List<Vinlist> InProgressList
        {
            get { return _InProgressList; }
            set
            {
                _InProgressList = value;
                OnPropertyChanged("InProgressList");
            }
        }

        private List<Vinlist> _LoadnumberList;
        public List<Vinlist> LoadnumberList
        {
            get { return _LoadnumberList; }
            set
            {
                _LoadnumberList = value;
                OnPropertyChanged("LoadnumberList");
            }
        }

        private bool _InProgressTabVisibility = false;
        public bool InProgressTabVisibility
        {
            get => _InProgressTabVisibility;
            set
            {
                _InProgressTabVisibility = value;
                OnPropertyChanged("InProgressTabVisibility");
            }
        }

        private bool _CompleteTabVisibility = false;
        public bool CompleteTabVisibility
        {
            get => _CompleteTabVisibility;
            set
            {
                _CompleteTabVisibility = value;
                OnPropertyChanged("CompleteTabVisibility");
            }
        }

        private bool _PendingTabVisibility = false;
        public bool PendingTabVisibility
        {
            get => _PendingTabVisibility;
            set
            {
                _PendingTabVisibility = value;
                OnPropertyChanged("PendingTabVisibility");
            }
        }

        private bool _AllTabVisibility = true;
        public bool AllTabVisibility
        {
            get => _AllTabVisibility;
            set
            {
                _AllTabVisibility = value;
                OnPropertyChanged("AllTabVisibility");
            }
        }
        private Color _InProgressTxtColor = Color.Black;
        public Color InProgressTxtColor
        {
            get => _InProgressTxtColor;
            set
            {
                _InProgressTxtColor = value;
                OnPropertyChanged("InProgressTxtColor");
            }
        }

        private Color _CompletedTxtColor = Color.Black;
        public Color CompletedTxtColor
        {
            get => _CompletedTxtColor;
            set
            {
                _CompletedTxtColor = value;
                OnPropertyChanged("CompletedTxtColor");
            }
        }

        private Color _PendingTxtColor = Color.Black;
        public Color PendingTxtColor
        {
            get => _PendingTxtColor;
            set
            {
                _PendingTxtColor = value;
                OnPropertyChanged("PendingTxtColor");
            }
        }
        private Color _AllTxtColor = Settings.Bar_Background;
        public Color AllTxtColor
        {
            get => _AllTxtColor;
            set
            {
                _AllTxtColor = value;
                OnPropertyChanged("AllTxtColor");
            }
        }

        private Vinlist _VehicleDetails;
        public Vinlist VehicleDetails
        {
            get => _VehicleDetails;
            set
            {
                _VehicleDetails = value;
                VinTapped();
            }
        }

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

        private Color _BgtabColor = Color.FromHex("#E9E9E9");
        public Color BgtabColor
        {
            get => _BgtabColor;
            set
            {
                _BgtabColor = value;
                OnPropertyChanged("BgtabColor");
            }
        }
        private Color _BgtabColor1 = Color.White;
        public Color BgtabColor1
        {
            get => _BgtabColor1;
            set
            {
                _BgtabColor1 = value;
                OnPropertyChanged("BgtabColor1");
            }
        }
        private Color _BgtabColor2 = Color.White;
        public Color BgtabColor2
        {
            get => _BgtabColor2;
            set
            {
                _BgtabColor2 = value;
                OnPropertyChanged("BgtabColor2");
            }
        }
        private Color _BgtabColor3 = Color.White;
        public Color BgtabColor3
        {
            get => _BgtabColor3;
            set
            {
                _BgtabColor3 = value;
                OnPropertyChanged("BgtabColor3");
            }
        }

        private string _TransportRep;
        public string TransportRep
        {
            get => _TransportRep;
            set
            {
                _TransportRep = value;
                OnPropertyChanged("TransportRep");
            }
        }

        private string _JobDone;
        public string JobDone
        {
            get => _JobDone;
            set
            {
                _JobDone = value;
                OnPropertyChanged("JobDone");
            }
        }



        private string _Cmptext;
        public string Cmptext
        {
            get => _Cmptext;
            set
            {
                _Cmptext = value;
                OnPropertyChanged("Cmptext");
            }
        }

        private string _Pendingtext;
        public string Pendingtext
        {
            get => _Pendingtext;
            set
            {
                _Pendingtext = value;
                OnPropertyChanged("Pendingtext");
            }
        }

        private string _INPtext;
        public string INPtext
        {
            get => _INPtext;
            set
            {
                _INPtext = value;
                OnPropertyChanged("INPtext");
            }
        }

        private string _Alltext;
        public string Alltext
        {
            get => _Alltext;
            set
            {
                _Alltext = value;
                OnPropertyChanged("Alltext");
            }
        }

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
        #endregion
    }
}
