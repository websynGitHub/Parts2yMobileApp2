using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class QuestionsViewModel : INotifyPropertyChanged
    {

        #region IComman and data members declaration
        SendPodata sendPodata = new SendPodata();
        public INavigation Navigation { get; set; }

        public ICommand Backevnttapped { set; get; }

        public ICommand QuestionClickCommand { get; set; }
        public QuestiionsPageHeaderData QuestiionsPageHeaderData { get; set; }

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

        QuestionsPage pagename;
        YPSService trackService;
        int tagId;
        List<InspectionResultsList> inspectionResultsLists;
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }
        #endregion

        public QuestionsViewModel(INavigation _Navigation, QuestionsPage page, int tagId, string tagNumber, string indentCode, string bagNumber, QuestiionsPageHeaderData questiionsPageHeaderData)
        {
            Navigation = _Navigation;
            pagename = page;
            this.tagId = tagId;
            Task.Run(() => ChangeLabel()).Wait();
            QuestiionsPageHeaderData = questiionsPageHeaderData;
            TagNumber = tagNumber;
            IndentCode = indentCode;
            TripNumber = bagNumber;
            trackService = new YPSService();
            QuestionsList = new ObservableCollection<InspectionConfiguration>();
            Backevnttapped = new Command(async () => await Backevnttapped_click());
            QuestionClickCommand = new Command<InspectionConfiguration>(QuestionClick);
            GetQuestionsLIst();

            HomeCmd = new Command(async () => await TabChange("home"));
            JobCmd = new Command(async () => await TabChange("job"));
            PartsCmd = new Command(async () => await TabChange("parts"));
            LoadCmd = new Command(async () => await TabChange("load"));
        }

        private async Task TabChange(string tabname)
        {
            try
            {
                loadindicator = true;
                await Task.Delay(1);

                if (tabname == "home")
                {
                    App.Current.MainPage = new MenuPage(typeof(HomePage));
                }
                else if (tabname == "job")
                {
                    if (Navigation.NavigationStack.Count > 3)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[3]);
                        Navigation.RemovePage(Navigation.NavigationStack[2]);

                    }
                    else
                    {
                        if (Settings.POID > 0)
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                            Settings.POID = 0;
                            Settings.TaskID = 0;
                        }
                    }

                    await Navigation.PopAsync();
                }
                else if (tabname == "parts")
                {
                    if (Navigation.NavigationStack.Count > 3)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[3]);

                    }
                    else
                    {
                        if (Settings.POID > 0)
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                            Settings.POID = 0;
                            Settings.TaskID = 0;
                        }
                    }

                    await Navigation.PopAsync();
                    //await Navigation.PushAsync(new POChildListPage(await GetUpdatedAllPOData(), SendPodData));
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
                YPSLogger.TrackEvent("QuestionsViewModel.xaml.cs", "in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

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
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.POID == Settings.POID && wr.TaskID == Settings.TaskID));
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

        public async void ChangeLabel()
        {
            try
            {
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
                        labelobj.Load.Name = Settings.VersionID == 2 ? "Insp" : "Load";
                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                //YPSLogger.ReportException(ex, "ChangeLabelAndShowHide method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        public async void GetConfigurationResults()
        {
            try
            {
                QuestionsList?.All(x => { x.SelectedTagBorderColor = Color.Transparent; return true; });
                QuestionsList?.All(x => { x.Status = 0; return true; });
                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    loadindicator = true;
                    var result = await trackService.GeInspectionResultsService(tagId);
                    loadindicator = false;
                    if (result != null && result.data != null && result.data.listData != null)
                    {
                        inspectionResultsLists = result.data.listData;
                        QuestionsList?.Where(x => inspectionResultsLists.Any(z => z.QID == x.MInspectionConfigID)).Select(x => { x.Status = 1; return x; }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void GetQuestionsLIst()
        {
            QuestionsList = new ObservableCollection<InspectionConfiguration>(Settings.allInspectionConfigurations);
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                if (Navigation.NavigationStack.Count > 3)
                {
                    Navigation.RemovePage(Navigation.NavigationStack[3]);

                }
                else
                {
                    if (Settings.POID > 0)
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[1]);
                        Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                        Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                        Settings.POID = 0;
                        Settings.TaskID = 0;
                    }
                }
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
                inspectionConfiguration.SelectedTagBorderColor = YPS.CommonClasses.Settings.Bar_Background;
                loadindicator = true;
                await Navigation.PushAsync(new AnswersPage(inspectionConfiguration, this.tagId, QuestionsList, inspectionResultsLists, TagNumber, IndentCode, TripNumber, QuestiionsPageHeaderData));
                loadindicator = false;

            }
            catch (Exception ex)
            {
            }
        }

        #region INotifyPropertyChanged Implimentation
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Properties

        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                RaisePropertyChanged("BgColor");
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

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator; set
            {
                _loadindicator = value;
                RaisePropertyChanged("loadindicator");
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

        private string _TripNumber;
        public string TripNumber
        {
            get => _TripNumber; set
            {
                _TripNumber = value;
                RaisePropertyChanged("TripNumber");
            }
        }

        #endregion

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
    }
}


