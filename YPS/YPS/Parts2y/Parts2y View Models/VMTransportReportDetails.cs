using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

    public class VMTransportReportDetails : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand InProgressCmd { get; set; }
        public ICommand CompletedCmd { get; set; }
        public ICommand PendingCmd { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand AllCmd { set; get; }
        public ICommand Backevnttapped { set; get; }
        //List<TransportDetailsModel> InProgressListItems = new List<TransportDetailsModel>();
        //List<TransportDetailsModel> CompletedListItems = new List<TransportDetailsModel>();
        //List<TransportDetailsModel> PendingListItems = new List<TransportDetailsModel>();
        //List<TransportDetailsModel> AllListItems = new List<TransportDetailsModel>();

        RestClient service = new RestClient();
        public VMTransportReportDetails(INavigation _Navigation)
        {
            try
            {
                Navigation = _Navigation;
                BgColor = Settings.Bar_Background;
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //InProgressListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });

                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //AllListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "tick.png" });

                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });
                //CompletedListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "checkmark.png" });

                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });
                //PendingListItems.Add(new TransportDetailsModel { VINNo = "MA3NFGB1SJC-1912003", TripNo = "6019760", ETATime = "11:23", ATATime = "11:25", Status_icon = "circle.png" });

                //InProgressList = AllListItems;
                InProgressCmd = new Command(async () => await InProgress_Tap());
                CompletedCmd = new Command(async () => await Complete_Tap());
                PendingCmd = new Command(async () => await Pending_Tap());
                AllCmd = new Command(async () => await All_Tap());
                HomeCommand = new Command(async () => await Home_Tap());
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                Load_No = Settings.LoadNo;
                Carrier_No = Settings.CarrierNo;
                GetLoadnumberdata();
            }
            catch (Exception ex)
            {
            }
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                Navigation.PopModalAsync();

            }
            catch (Exception ex)
            {
            }
        }

        public async void GetLoadnumberdata()
        {
            try
            {
                loadindicator = true;

                SupTransportDB db = new SupTransportDB("loaddata");
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    var poData = await service.GetLoaddetails(Settings.LoadNo);
                    var result = JsonConvert.DeserializeObject<GetLoaddata>(poData.ToString());
                    loadData = new List<GetLoaddata>();
                    loadData.Add(result);
                    db.SaveUpdateLoadData(loadData);
                }
                else
                {
                    loadData = db.GetLoadData();
                }

                if (loadData.Count > 0)
                {
                    if (loadData[0].status != false)
                    {
                        LoadnumberList = loadData[0].Loaddata;

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
                            item.bgteamcolor = Settings.Bar_Background;
                        }
                        Load_No = loadData[0].Loaddetails.Loadnumber;
                        InVoice = loadData[0].Loaddetails.Invoicenumber;
                        Carrier_No = loadData[0].Loaddetails.Carrierno;
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
                        Navigation.PopModalAsync();
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "No data available", "OK");
                    await Navigation.PopModalAsync();
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
        private async Task InProgress_Tap()
        {
            try
            {
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

        public async void TransportReportDetailsTapped()
        {
            try
            {
                if (VehicleDetails != null)
                {
                    try
                    {
                        loadindicator = true;

                        Settings.VINNo = VehicleDetails.Vin;

                        if (Navigation.ModalStack.Count == 0 ||
                                            Navigation.ModalStack.Last().GetType() != typeof(VehicleDetails))
                        {
                            await Navigation.PushModalAsync(new VehicleDetails());
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
            }
            catch (Exception ex)
            {
                loadindicator = false;
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

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator; set
            {
                _loadindicator = value;
                OnPropertyChanged("loadindicator");
            }
        }

        public List<GetLoaddata> loadData { get; set; }

        private List<Loaddata> _InProgressList;
        public List<Loaddata> InProgressList
        {
            get { return _InProgressList; }
            set
            {
                _InProgressList = value;
                OnPropertyChanged("InProgressList");
            }
        }

        private List<Loaddata> _LoadnumberList;
        public List<Loaddata> LoadnumberList
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

        private Loaddata _VehicleDetails;
        public Loaddata VehicleDetails
        {
            get => _VehicleDetails;
            set
            {
                _VehicleDetails = value;
                TransportReportDetailsTapped();
            }
        }

        private string _Load_No;
        public string Load_No
        {
            get => _Load_No;
            set
            {
                _Load_No = value;
                OnPropertyChanged("Load_No");
            }
        }
        private string _Carrier_No;
        public string Carrier_No
        {
            get => _Carrier_No;
            set
            {
                _Carrier_No = value;
                OnPropertyChanged("Carrier_No");
            }
        }
        private string _InVoice;
        public string InVoice
        {
            get => _InVoice;
            set
            {
                _InVoice = value;
                OnPropertyChanged("InVoice");
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

        private int _INP;
        public int INP
        {
            get => _INP;
            set
            {
                _INP = value;
                OnPropertyChanged("INP");
            }
        }

        private int _Pending;
        public int Pending
        {
            get => _Pending;
            set
            {
                _Pending = value;
                OnPropertyChanged("Pending");
            }
        }

        private int _Cmp;
        public int Cmp
        {
            get => _Cmp;
            set
            {
                _Cmp = value;
                OnPropertyChanged("Cmp");
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
        private int _All;
        public int All
        {
            get => _All;
            set
            {
                _All = value;
                OnPropertyChanged("All");
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
        #endregion
    }

}

