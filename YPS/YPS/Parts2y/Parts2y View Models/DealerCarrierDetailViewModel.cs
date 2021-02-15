using Newtonsoft.Json;
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
    public class DealerCarrierDetailViewModel : BaseViewModel
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
        public List<CarrierDataTotal> result { get; set; }

        public DealerCarrierDetailViewModel(INavigation _Navigation)
        {
            Navigation = _Navigation;
            BgColor = YPS.CommonClasses.Settings.Bar_Background;
            InProgressCmd = new Command(async () => await InProgress_Tap());
            CompletedCmd = new Command(async () => await Complete_Tap());
            PendingCmd = new Command(async () => await Pending_Tap());
            AllCmd = new Command(async () => await All_Tap());
            Backevnttapped = new Command(async () => await Backevnttapped_click());
            GetCarrierList();
        }
        public async Task GetCarrierList()
        {
            try
            {
                loadindicator = true;

                result = new List<CarrierDataTotal>();
                DealerDB Db = new DealerDB("dealercarrierdata");
                var current = Connectivity.NetworkAccess;
                
                if (current == NetworkAccess.Internet)
                {
                    var poData = await service.GetDealerCarrierListDetails(Settings.DealerCarrierNo);
                    var val = JsonConvert.DeserializeObject<CarrierDataTotal>(poData.ToString());
                    if (val != null)
                    {
                        if (val.status != false)
                        {
                            result.Add(val);
                            Db.SaveEachCarrierDetail(result);
                        }
                    }
                }
                else
                {
                    result = Db.GetCarrierInfo(Settings.DealerCarrierNo);
                }
                if (result.Count > 0)
                {
                    ETA_Date = result[0].Carrierinfo.Eta;
                    Carrier_No = result[0].Carrierinfo.Carrierno;
                    InVoice = result[0].Carrierinfo.Invoicenumber;
                    CarrierList = result[0].Carrierdata;
                    
                    foreach (var item in CarrierList)
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
                    INP = CarrierList.Where(x => x.IsCompleted == 2).Count();
                    Cmp = CarrierList.Where(x => x.IsCompleted == 1).Count();
                    Pending = CarrierList.Where(x => x.IsCompleted == 0).Count();
                    All = CarrierList.Count();
                    Cmptext = "COMPLETED " + "(" + Cmp.ToString() + ")";
                    INPtext = "IN PROGRESS " + "(" + INP.ToString() + ")";
                    Pendingtext = "PENDING " + "(" + Pending.ToString() + ")";
                    Alltext = "ALL\n" + "(" + All.ToString() + ")";
                    InProgressList = CarrierList;
                    loadindicator = false;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "No data available", "Ok");
                    await Navigation.PopAsync();
                    loadindicator = false;
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async Task Backevnttapped_click()
        {
            try
            {
                loadindicator = true;
                await Navigation.PopAsync();

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
                InProgressList = CarrierList.Where(x => x.IsCompleted == 2).ToList();
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
                InProgressList = CarrierList.Where(x => x.IsCompleted == 1).ToList();
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
                InProgressList = CarrierList.Where(x => x.IsCompleted == 0).ToList();
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
                InProgressList = CarrierList;
                AllTabVisibility = true;
                InProgressTabVisibility = CompleteTabVisibility = PendingTabVisibility = false;
                AllTxtColor = Settings.Bar_Background;
                InProgressTxtColor = CompletedTxtColor = PendingTxtColor = Color.Black;
            }
            catch (Exception ex)
            {

            }
        }


        private async Task VINTapped()
        {
            try
            {
                if (CarrierDetails != null)
                {
                    try
                    {
                        loadindicator = true;

                        Settings.VINNo = CarrierDetails.Vin;
                        if (Navigation.ModalStack.Count == 0 ||
                                           Navigation.ModalStack.Last().GetType() != typeof(DealerVehicleDetails))
                        {
                            await Navigation.PushAsync(new DealerVehicleDetails());
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

            }
        }

        #region Properties
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
        private string _ETA_Date;
        public string ETA_Date
        {
            get => _ETA_Date;
            set
            {
                _ETA_Date = value;
                OnPropertyChanged("ETA_Date");
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
        private List<Carrierdata> _InProgressList;
        public List<Carrierdata> InProgressList
        {
            get { return _InProgressList; }
            set
            {
                _InProgressList = value;
                OnPropertyChanged("InProgressList");
            }
        }

        private List<Carrierdata> _CarrierList;
        public List<Carrierdata> CarrierList
        {
            get { return _CarrierList; }
            set
            {
                _CarrierList = value;
                OnPropertyChanged("CarrierList");
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

        private Carrierdata _CarrierDetails;
        public Carrierdata CarrierDetails
        {
            get => _CarrierDetails;
            set
            {
                _CarrierDetails = value;
                VINTapped();
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

        //private int _INP;
        //public int INP
        //{
        //    get => _INP;
        //    set
        //    {
        //        _INP = value;
        //        OnPropertyChanged("INP");
        //    }
        //}

        //private int _Pending;
        //public int Pending
        //{
        //    get => _Pending;
        //    set
        //    {
        //        _Pending = value;
        //        OnPropertyChanged("Pending");
        //    }
        //}

        //private int _Cmp;
        //public int Cmp
        //{
        //    get => _Cmp;
        //    set
        //    {
        //        _Cmp = value;
        //        OnPropertyChanged("Cmp");
        //    }
        //}

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
        //private int _All;
        //public int All
        //{
        //    get => _All;
        //    set
        //    {
        //        _All = value;
        //        OnPropertyChanged("All");
        //    }
        //}

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
