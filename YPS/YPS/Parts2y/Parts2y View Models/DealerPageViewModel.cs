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
    public class DealerPageViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand InProgressCmd { get; set; }
        public ICommand CompletedCmd { get; set; }
        public ICommand PendingCmd { get; set; }
        public ICommand AllCmd { set; get; }

        RestClient service = new RestClient();
        DealerAllData result;
        public int INP, Cmp, Pending, All = 0;

        public DealerPageViewModel(INavigation _Navigation)
        {
            Navigation = _Navigation;
            BgColor = Settings.Bar_Background;
            InProgressCmd = new Command(async () => await InProgress_Tap());
            CompletedCmd = new Command(async () => await Complete_Tap());
            PendingCmd = new Command(async () => await Pending_Tap());
            AllCmd = new Command(async () => await All_Tap());
            GetAllDealerData(Settings.UserMail);
        }

        public async Task GetAllDealerData(string mail)
        {
            try
            {

                loadindicator = true;

                result = new DealerAllData();
                DealerDB Db = new DealerDB("dealeralldata");
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    var dealerData = await service.GetDealerData(mail);
                    var data = JsonConvert.DeserializeObject<DealerAllData>(dealerData.ToString());

                    if (data != null)
                    {
                        if (data.status != false)
                        {
                            result = data;
                            Db.SaveDealerDetails(data);
                        }
                    }
                }
                else
                {
                    result = Db.GetDealerDetails();
                }
                if (result != null)
                {

                    Dealer_Id = result.DealerId;
                    Dealer_Name = result.Dealername;
                    Delivery_Date = result.Delivery;
                    InProgressList = result.Dealerdata;
                    ePODList = result.Dealerdata;

                    foreach (var item in ePODList)
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

                    INP = ePODList.Where(x => x.IsCompleted == 2).Count();
                    Cmp = ePODList.Where(x => x.IsCompleted == 1).Count();
                    Pending = ePODList.Where(x => x.IsCompleted == 0).Count();
                    All = ePODList.Count();

                    Cmptext = "COMPLETED " + "(" + Cmp.ToString() + ")";
                    INPtext = "IN PROGRESS " + "(" + INP.ToString() + ")";
                    Pendingtext = "PENDING " + "(" + Pending.ToString() + ")";
                    Alltext = "ALL\n" + "(" + All.ToString() + ")";

                    InProgressList = ePODList;

                    loadindicator = false;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "No data available", "Ok");
                    await Navigation.PopModalAsync();
                    loadindicator = false;
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task InProgress_Tap()
        {
            try
            {
                InProgressList = ePODList.Where(x => x.IsCompleted == 2).ToList();
                // InProgressList = InProgressListItems;
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
                InProgressList = ePODList.Where(x => x.IsCompleted == 1).ToList();
                //InProgressList = CompletedListItems;
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
                InProgressList = ePODList.Where(x => x.IsCompleted == 0).ToList();
                //InProgressList = PendingListItems;
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
                InProgressList = ePODList;
                // InProgressList = AllListItems;
                AllTabVisibility = true;
                InProgressTabVisibility = CompleteTabVisibility = PendingTabVisibility = false;
                AllTxtColor = Settings.Bar_Background;
                InProgressTxtColor = CompletedTxtColor = PendingTxtColor = Color.Black;
            }
            catch (Exception ex)
            {

            }
        }

        private async void DealerIDTapped()
        {
            try
            {
                if (SelectedDealerId != null)
                {
                    loadindicator = true;

                    if (!string.IsNullOrEmpty(SelectedDealerId.Carrierno))
                    {
                        Settings.DealerCarrierNo = SelectedDealerId.Carrierno;
                        if (Navigation.ModalStack.Count == 0 ||
                                       Navigation.ModalStack.Last().GetType() != typeof(DealerCarrierDetails))
                        {
                            await Navigation.PushModalAsync(new DealerCarrierDetails());
                            loadindicator = false;
                        }
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

        private string _Dealer_Id;
        public string Dealer_Id
        {
            get => _Dealer_Id;
            set
            {
                _Dealer_Id = value;
                OnPropertyChanged("Dealer_Id");
            }
        }

        private string _Dealer_Name;
        public string Dealer_Name
        {
            get => _Dealer_Name;
            set
            {
                _Dealer_Name = value;
                OnPropertyChanged("Dealer_Name");
            }
        }

        private string _Delivery_Date;
        public string Delivery_Date
        {
            get => _Delivery_Date;
            set
            {
                _Delivery_Date = value;
                OnPropertyChanged("Delivery_Date");
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

        private List<Dealerdata> _InProgressList;
        public List<Dealerdata> InProgressList
        {
            get { return _InProgressList; }
            set
            {
                _InProgressList = value;
                OnPropertyChanged("InProgressList");
            }
        }

        private List<Dealerdata> _ePODList;
        public List<Dealerdata> ePODList
        {
            get { return _ePODList; }
            set
            {
                _ePODList = value;
                OnPropertyChanged("ePODList");
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
        public Dealerdata _SelectedDealerId;
        public Dealerdata SelectedDealerId
        {
            get { return _SelectedDealerId; }
            set
            {

                _SelectedDealerId = value;
                DealerIDTapped();

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
