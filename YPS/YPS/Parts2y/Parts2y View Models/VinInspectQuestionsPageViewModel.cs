using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
//using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class VinInspectQuestionsPageViewModel : IBase
    {
        #region IComman and data members declaration
        SendPodata sendPodata = new SendPodata();
        AllPoData selectedTagData;
        public INavigation Navigation { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand QuickTabCmd { set; get; }
        public ICommand FullTabCmd { set; get; }
        public ICommand SignalTabCmd { set; get; }
        public ICommand QuestionClickCommand { get; set; }
        //public ObservableCollection<InspectionConfiguration> QuestionListCategory { get; set; }
        public QuestiionsPageHeaderData QuestiionsPageHeaderData { get; set; }



        VinInspectQuestionsPage pageName;
        YPSService trackService;
        int tagId;
        bool isAllDone;
        List<InspectionResultsList> inspectionResultsLists;
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }
        #endregion

        public VinInspectQuestionsPageViewModel(INavigation _Navigation, VinInspectQuestionsPage pagename, AllPoData selectedtagdata, bool isalldone)
        {
            try
            {
                Navigation = _Navigation;
                trackService = new YPSService();
                pageName = pagename;
                isAllDone = isalldone;
                selectedTagData = selectedtagdata;
                this.tagId = selectedTagData.POTagID;
                TagNumber = selectedTagData.TagNumber;
                IndentCode = selectedTagData.IdentCode;
                ConditionName = selectedTagData.ConditionName;
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                QuestionClickCommand = new Command<InspectionConfiguration>(QuestionClick);
                Task.Run(() => ChangeLabel()).Wait();
                Task.Run(() => GetQuestionsLIst()).Wait();


                QuickTabCmd = new Command(QuickTabClicked);
                FullTabCmd = new Command(FullTabClicked);
                SignalTabCmd = new Command(SignTabClicked);
                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                PartsCmd = new Command(async () => await TabChange("parts"));
                LoadCmd = new Command(async () => await TabChange("load"));
            }
            catch (Exception ex)
            {

            }
        }

        public async Task TabChange(string tabname)
        {
            try
            {
                if (tabname == "home")
                {
                    loadindicator = true;
                    await Task.Delay(1);
                    App.Current.MainPage = new MenuPage(typeof(HomePage));
                }
                else if (tabname == "job")
                {
                    loadindicator = true;
                    await Task.Delay(1);

                    if (Settings.POID != 0)
                    {
                        if (Navigation.NavigationStack.Count() == 5)
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[2]);
                            Navigation.RemovePage(Navigation.NavigationStack[2]);
                            //Navigation.RemovePage(Navigation.NavigationStack[2]);
                        }
                        else
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                        }
                        Settings.POID = 0;
                    }
                    else
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[2]);
                        Navigation.RemovePage(Navigation.NavigationStack[2]);
                    }
                    await Navigation.PopAsync();
                }
                else if (tabname == "parts")
                {
                    loadindicator = true;
                    await Task.Delay(1);

                    if (Settings.POID != 0)
                    {
                        if (Navigation.NavigationStack.Count() == 5)
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[3]);
                            //Navigation.RemovePage(Navigation.NavigationStack[3]);
                        }
                        else
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                        }
                        Settings.POID = 0;
                    }
                    else
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[3]);
                    }
                    await Navigation.PopAsync();
                }
                else if (tabname == "load")
                {
                    ObservableCollection<AllPoData> preparelist = new ObservableCollection<AllPoData>();
                    preparelist.Add(selectedTagData);
                    await Navigation.PushAsync(new CarrierInspectionQuestionsPage(preparelist, isAllDone));
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
            }
            loadindicator = false;
        }

        private async Task<ObservableCollection<AllPoData>> GetUpdatedAllPOData()
        {
            ObservableCollection<AllPoData> AllPoDataList = new ObservableCollection<AllPoData>();

            try
            {
                loadindicator = true;
                YPSLogger.TrackEvent("PhotoUpload.xaml.cs", "in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {

                    sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;

                    var result = await trackService.LoadPoDataService(sendPodata);

                    if (result != null && result.data != null)
                    {
                        if (result.status != 0 && result.data.allPoData != null && result.data.allPoData.Count > 0)
                        {
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.POID == Settings.POID));
                        }
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                loadindicator = false;
            }
            return AllPoDataList;
        }

        public async void QuickTabClicked()
        {
            try
            {
                loadindicator = true;

                await GetConfigurationResults(1);

                IsQuestionListVisible = true;
                QuickTabTextColor = Settings.Bar_Background;
                QuickTabVisibility = true;
                FullTabTextColor = Color.Black;
                FullTabVisibility = false;
                SignTabTextColor = Color.Black;
                SignTabVisibility = false;
            }
            catch (Exception ex)
            {

            }
            loadindicator = false;
        }

        public async void FullTabClicked()
        {
            try
            {
                loadindicator = true;

                await GetConfigurationResults(2);

                IsQuestionListVisible = true;
                QuickTabTextColor = Color.Black;
                QuickTabVisibility = false;
                FullTabTextColor = Settings.Bar_Background;
                FullTabVisibility = true;
                SignTabTextColor = Color.Black;
                SignTabVisibility = false;
            }
            catch (Exception ex)
            {

            }
            loadindicator = false;
        }

        public async void SignTabClicked()
        {
            try
            {
                loadindicator = true;
                await Task.Delay(1);

                IsQuestionListVisible = false;
                QuickTabTextColor = Color.Black;
                QuickTabVisibility = false;
                FullTabTextColor = Color.Black;
                FullTabVisibility = false;
                SignTabTextColor = Settings.Bar_Background;
                SignTabVisibility = true;
            }
            catch (Exception ex)
            {

            }
            loadindicator = false;
        }

        public async Task GetConfigurationResults(int categoryID)
        {
            try
            {
                loadindicator = true;

                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    QuestionsList?.All(x => { x.SelectedTagBorderColor = Color.Transparent; return true; });
                    QuestionsList?.All(x => { x.Status = 0; return true; });

                    var result = await trackService.GeInspectionResultsService(tagId);

                    if (result != null && result.data != null && result.data.listData != null)
                    {
                        inspectionResultsLists = result.data.listData;
                        QuestionsList?.Where(x => inspectionResultsLists.Any(z => z.QID == x.MInspectionConfigID)).Select(x => { x.Status = 1; return x; }).ToList();

                        QuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionsList?.Where(wr => wr.CategoryID == categoryID).ToList());
                    }
                    else
                    {
                        QuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionsList?.Where(wr => wr.CategoryID == categoryID).ToList());
                    }
                }
            }
            catch (Exception ex)
            {

            }
            loadindicator = false;

        }

        public void GetQuestionsLIst()
        {
            try
            {
                QuestionsList = new ObservableCollection<InspectionConfiguration>(Settings.allInspectionConfigurations);
            }
            catch (Exception ex)
            {
            }
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                //if (Navigation.NavigationStack.Count > 3)
                //{
                //    Navigation.RemovePage(Navigation.NavigationStack[3]);

                //}
                //else
                //{
                //    if (Settings.POID > 0)
                //    {
                //        Navigation.RemovePage(Navigation.NavigationStack[1]);
                //        Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                //        Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                //        Settings.POID = 0;
                //    }
                //}
                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
            }
        }

        public async void QuestionClick(InspectionConfiguration inspectionConfiguration)
        {
            try
            {
                loadindicator = true;

                inspectionConfiguration.SelectedTagBorderColor = Settings.Bar_Background;
                await Navigation.PushAsync(new VinInspectionAnswersPage(inspectionConfiguration, QuestionsList, inspectionResultsLists, selectedTagData, true));
            }
            catch (Exception ex)
            {
            }
            loadindicator = false;
        }

        public async void ChangeLabel()
        {
            try
            {
                loadindicator = true;

                labelobj = new DashboardLabelChangeClass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var tagnumber = labelval.Where(wr => wr.FieldID == labelobj.TagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID == labelobj.IdentCode.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bagnumber = labelval.Where(wr => wr.FieldID == labelobj.BagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID == labelobj.ConditionName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();


                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber == null ? true : (tagnumber.Status == 1 ? true : false);
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode == null ? true : (identcode.Status == 1 ? true : false);
                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                        labelobj.BagNumber.Status = bagnumber == null ? true : (bagnumber.Status == 1 ? true : false);
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname == null ? true : (conditionname.Status == 1 ? true : false);
                        labelobj.Load.Name = Settings.VersionID == 2 ? "Carrier" : "Load";
                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    IsQuickTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "QuickInspection".Trim()).FirstOrDefault()) != null ? true : false;
                    IsFullTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "FullInspection".Trim()).FirstOrDefault()) != null ? true : false;

                    if (Settings.VersionID == 2)
                    {
                        LoadTextColor = Color.Black;
                        IsLoadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CarrierInspection".Trim()).FirstOrDefault()) != null ? true : false;
                    }

                    if (IsQuickTabVisible == false && IsFullTabVisible == false)
                    {
                        SignTabClicked();
                    }

                    if (IsQuickTabVisible == true)
                    {
                        //QuickTabClicked();
                        QuickTabVisibility = true;
                        FullTabVisibility = false;
                        SignTabVisibility = false;
                    }
                    else
                    {
                        //FullTabClicked();
                        FullTabVisibility = true;
                        QuickTabVisibility = false;
                        SignTabVisibility = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
            }
            loadindicator = false;
        }

        #region Properties
        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields TagNumber { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TagNumber"
            };
            public DashboardLabelFields IdentCode { get; set; } = new DashboardLabelFields { Status = true, Name = "IdentCode" };
            public DashboardLabelFields BagNumber { get; set; } = new DashboardLabelFields { Status = true, Name = "BagNumber" };
            public DashboardLabelFields ConditionName { get; set; } = new DashboardLabelFields { Status = true, Name = "ConditionName" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "Parts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "Load" };

        }
        public class DashboardLabelFields : IBase
        {
            public bool Status { get; set; }
            public string Name { get; set; }
        }

        public DashboardLabelChangeClass _labelobj = new DashboardLabelChangeClass();
        public DashboardLabelChangeClass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        public Color _LoadTextColor = Color.Black;
        public Color LoadTextColor
        {
            get => _LoadTextColor;
            set
            {
                _LoadTextColor = value;
                RaisePropertyChanged("LoadTextColor");
            }
        }

        private bool _IsLoadTabVisible { set; get; } = true;
        public bool IsLoadTabVisible
        {
            get
            {
                return _IsLoadTabVisible;
            }
            set
            {
                this._IsLoadTabVisible = value;
                RaisePropertyChanged("IsLoadTabVisible");
            }
        }

        private bool _IsQuestionListVisible = true;
        public bool IsQuestionListVisible
        {
            get { return _IsQuestionListVisible; }
            set
            {
                _IsQuestionListVisible = value;
                RaisePropertyChanged("IsQuestionListVisible");
            }
        }

        private bool _IsQuickTabVisible = true;
        public bool IsQuickTabVisible
        {
            get => _IsQuickTabVisible;
            set
            {
                _IsQuickTabVisible = value;
                NotifyPropertyChanged("IsQuickTabVisible");
            }
        }
        private bool _IsFullTabVisible = true;
        public bool IsFullTabVisible
        {
            get => _IsFullTabVisible;
            set
            {
                _IsFullTabVisible = value;
                NotifyPropertyChanged("IsFullTabVisible");
            }
        }
        private bool _IsSignTabVisible = true;
        public bool IsSignTabVisible
        {
            get => _IsSignTabVisible;
            set
            {
                _IsSignTabVisible = value;
                NotifyPropertyChanged("IsSignTabVisible");
            }
        }
        private string _TagNumber;
        public string TagNumber
        {
            get => _TagNumber;
            set
            {
                _TagNumber = value;
                RaisePropertyChanged("TagNumber");
            }
        }

        private string _IndentCode;
        public string IndentCode
        {
            get => _IndentCode; set
            {
                _IndentCode = value;
                RaisePropertyChanged("IndentCode");
            }
        }

        private string _ConditionName;
        public string ConditionName
        {
            get => _ConditionName; set
            {
                _ConditionName = value;
                RaisePropertyChanged("ConditionName");
            }
        }

        private ObservableCollection<InspectionConfiguration> _QuestionListCategory;
        public ObservableCollection<InspectionConfiguration> QuestionListCategory
        {
            get => _QuestionListCategory;
            set
            {
                _QuestionListCategory = value;
                RaisePropertyChanged("QuestionListCategory");
            }
        }

        private ObservableCollection<InspectionConfiguration> _QuestionsList;
        public ObservableCollection<InspectionConfiguration> QuestionsList
        {
            get => _QuestionsList;
            set
            {
                _QuestionsList = value;
            }
        }

        private bool _RefreshDisable = true;
        public bool RefreshDisable
        {
            get { return _RefreshDisable; }
            set
            {
                _RefreshDisable = value;
                RaisePropertyChanged("RefreshDisable");
            }
        }

        private Color _QuickTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color QuickTabTextColor
        {
            get => _QuickTabTextColor;
            set
            {
                _QuickTabTextColor = value;
                NotifyPropertyChanged("QuickTabTextColor");
            }
        }


        private Color _FullTabTextColor = Color.Black;
        public Color FullTabTextColor
        {
            get => _FullTabTextColor;
            set
            {
                _FullTabTextColor = value;
                NotifyPropertyChanged("FullTabTextColor");
            }
        }

        private Color _SignTabTextColor = Color.Black;
        public Color SignTabTextColor
        {
            get => _SignTabTextColor;
            set
            {
                _SignTabTextColor = value;
                NotifyPropertyChanged("SignTabTextColor");
            }
        }

        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                NotifyPropertyChanged("BgColor");
            }
        }
        private bool _InspTabVisibility = true;
        public bool InspTabVisibility
        {
            get => _InspTabVisibility;
            set
            {
                _InspTabVisibility = value;
                NotifyPropertyChanged("InspTabVisibility");
            }
        }

        private bool _QuickTabVisibility = true;
        public bool QuickTabVisibility
        {
            get => _QuickTabVisibility;
            set
            {
                _QuickTabVisibility = value;
                NotifyPropertyChanged("QuickTabVisibility");
            }
        }
        private bool _FullTabVisibility = false;
        public bool FullTabVisibility
        {
            get => _FullTabVisibility;
            set
            {
                _FullTabVisibility = value;
                NotifyPropertyChanged("FullTabVisibility");
            }
        }
        private bool _SignTabVisibility = false;
        public bool SignTabVisibility
        {
            get => _SignTabVisibility;
            set
            {
                _SignTabVisibility = value;
                NotifyPropertyChanged("SignTabVisibility");
            }
        }

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator; set
            {
                _loadindicator = value;
                RaisePropertyChanged("loadindicator");
            }
        }
        #endregion Properties
    }
}
