using System;
using System.Collections.Generic;
using System.Text;
using YPS.Service;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Parts2y.Parts2y_View_Models;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using YPS.Helpers;
using System.Threading.Tasks;
using YPS.CustomToastMsg;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class ELoadInspectionQuestionsViewModel : IBase
    {
        #region IComman and data members declaration
        SendPodata sendPodata = new SendPodata();
        public INavigation Navigation { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand LoadInspTabCmd { set; get; }
        public ICommand SignalTabCmd { set; get; }
        public ICommand QuestionClickCommand { get; set; }
        ObservableCollection<AllPoData> SelectedPodataList;
        public QuestiionsPageHeaderData QuestiionsPageHeaderData { get; set; }
        ELoadInspectionQuestionsPage pageName;
        YPSService trackService;
        int taskid, pagecount;
        bool IsAllTagsDone;
        List<InspectionResultsList> inspectionResultsLists;
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }
        public ICommand SignatureCmd { get; set; }
        public ICommand HideSignaturePadCmd { get; set; }
        #endregion

        public ELoadInspectionQuestionsViewModel(INavigation _Navigation, ELoadInspectionQuestionsPage pagename, ObservableCollection<AllPoData> selectedpodatalist, bool isalltagdone)
        {
            try
            {
                Navigation = _Navigation;
                trackService = new YPSService();
                pageName = pagename;
                SelectedPodataList = selectedpodatalist;
                Settings.POID = SelectedPodataList[0].POID;
                Settings.TaskID = SelectedPodataList[0].TaskID;
                taskid = SelectedPodataList[0].TaskID;
                IsAllTagsDone = isalltagdone;
                PONumber = SelectedPodataList[0].PONumber;
                ShippingNumber = SelectedPodataList[0].ShippingNumber;
                REQNo = SelectedPodataList[0].REQNo;
                TaskName = SelectedPodataList[0].TaskName;
                EventName = SelectedPodataList[0].EventName;
                Resource = SelectedPodataList[0].TaskResourceName;
                //IsResourcecVisible = SelectedPodataList[0].TaskResourceID == Settings.userLoginID ? false : true;
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                QuestionClickCommand = new Command<InspectionConfiguration>(QuestionClick);
                LoadInspTabCmd = new Command(LoadInspTabClicked);
                SignalTabCmd = new Command(SignTabClicked);
                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                PartsCmd = new Command(async () => await TabChange("parts"));
                LoadCmd = new Command(async () => await TabChange("load"));

                ChangeLabel();
                Task.Run(GetQuestionsLIst);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ELoadInspectionQuestionsViewModel constructor -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task TabChange(string tabname)
        {
            try
            {
                loadindicator = true;

                if (tabname == "home")
                {
                    await Navigation.PopToRootAsync(false);
                }
                else if (tabname == "job")
                {
                    CommonMethods.BackClickFromInspToJobs(Navigation);
                }
                else if (tabname == "parts")
                {
                    CommonMethods.BackClickFromInspToParts(Navigation);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TabChange method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
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
                YPSLogger.TrackEvent("ELoadInspectionQuestionsViewModel.cs", " in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

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
                        if (result.status == 1 && result.data.allPoDataMobile != null && result.data.allPoDataMobile.Count > 0)
                        {
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoDataMobile.Where(wr => wr.TaskID == SelectedPodataList[0]?.TaskID));
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
                YPSLogger.ReportException(ex, "GetUpdatedAllPOData method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
            return AllPoDataList;
        }

        public async void LoadInspTabClicked()
        {
            try
            {
                loadindicator = true;

                await GetConfigurationResults(12);

                IsSignQuestionListVisible = false;
                IsQuestionListVisible = true;
                LoadInspTabTextColor = Settings.Bar_Background;
                LoadInspTabVisibility = true;
                SignTabTextColor = Color.Black;
                SignTabVisibility = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LoadInspTabClicked method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async void SignTabClicked()
        {
            try
            {
                loadindicator = true;

                await GetConfigurationResults(12);

                IsSignQuestionListVisible = true;
                IsQuestionListVisible = false;
                LoadInspTabTextColor = Color.Black;
                LoadInspTabVisibility = false;
                SignTabTextColor = Settings.Bar_Background;
                SignTabVisibility = true;

                if (IsAllTagsDone == true && QuestionListCategory.Where(wr => wr.Status == 0).FirstOrDefault() == null)
                {
                    IsDoneEnable = true;
                    DoneOpacity = 1.0;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SignTabClicked method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task GetConfigurationResults(int categoryID)
        {
            try
            {
                loadindicator = true;

                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    QuestionsList?.All(a => { a.SelectedTagBorderColor = Color.Transparent; return true; });
                    QuestionsList?.All(x => { x.Status = 0; return true; });

                    var result = await trackService.GetInspectionResultsByTask(taskid);

                    if (result != null && result.data != null && result.data.listData != null)
                    {
                        inspectionResultsLists = result.data.listData;
                        QuestionsList?.Where(x => inspectionResultsLists.Any(z => z.QID == x.MInspectionConfigID)).Select(x => { x.Status = 1; return x; }).ToList();

                        QuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionsList?.Where(wr => wr.CategoryID == categoryID && wr.VersionID == Settings.VersionID).ToList());
                    }
                    else
                    {
                        QuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionsList?.Where(wr => wr.CategoryID == categoryID && wr.VersionID == Settings.VersionID).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetConfigurationResults method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public void GetQuestionsLIst()
        {
            try
            {
                QuestionsList = new ObservableCollection<InspectionConfiguration>(Settings.allInspectionConfigurations);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetQuestionsLIst method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                CommonMethods.BackClickFromInspToParts(Navigation);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Backevnttapped_click method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async void QuestionClick(InspectionConfiguration inspectionConfiguration)
        {
            try
            {
                loadindicator = true;
                List<string> EncPOTagID = new List<string>();

                foreach (var data in SelectedPodataList.Where(wr => wr.TagTaskStatus == 0))
                {
                    var value = Helperclass.Encrypt(data.POTagID.ToString());
                    EncPOTagID.Add(value);
                }

                inspectionConfiguration.SelectedTagBorderColor = Settings.Bar_Background;
                await Navigation.PushAsync(new EInspectionAnswersPage(inspectionConfiguration, QuestionListCategory, 
                    inspectionResultsLists, SelectedPodataList[0],
                    false, null, this, IsAllTagsDone, string.Join(",", EncPOTagID)), false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QuestionClick method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
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
                        var poid = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.POID.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ShippingNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.REQNo.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var resource = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Resource.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid?.Status == 1 ? true : false;
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber?.Status == 1 ? true : false;
                        labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                        labelobj.REQNo.Status = reqnumber?.Status == 1 ? true : false;
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme?.Status == 1 ? true : false;
                        labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                        labelobj.EventName.Status = eventname?.Status == 1 ? true : false;

                        labelobj.Resource.Name = (resource != null ? (!string.IsNullOrEmpty(resource.LblText) ? resource.LblText : labelobj.Resource.Name) : labelobj.Resource.Name) + " :";

                        labelobj.Home.Name = "Home";
                        labelobj.Jobs.Name = "Job";
                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    IsQuickTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "EQuickInspection".Trim()).FirstOrDefault()) != null ? true : false;
                    IsFullTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "EFullInspection".Trim()).FirstOrDefault()) != null ? true : false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ChangeLabel method -> in ELoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        #region Properties
        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields Home { get; set; } = new DashboardLabelFields { Status = true, Name = "Home" };
            public DashboardLabelFields Jobs { get; set; } = new DashboardLabelFields { Status = true, Name = "Job" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "Parts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "Load" };

            public DashboardLabelFields POID { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "PONumber"
            };
            public DashboardLabelFields REQNo { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "REQNo"
            };
            public DashboardLabelFields ShippingNumber { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "ShippingNumber"
            };
            public DashboardLabelFields TaskName { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "TaskName"
            };

            public DashboardLabelFields Resource { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "Resource"
            };

            public DashboardLabelFields EventName { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "Event"
            };
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

        private bool _IsSignatureCarrierVisible = true;
        public bool IsSignatureCarrierVisible
        {
            get => _IsSignatureCarrierVisible;
            set
            {
                _IsSignatureCarrierVisible = value;
                RaisePropertyChanged("IsSignatureCarrierVisible");
            }
        }

        private ImageSource _AuditorImageSignCBU;
        public ImageSource AuditorImageSignCBU
        {
            get => _AuditorImageSignCBU;
            set
            {
                _AuditorImageSignCBU = value;
                RaisePropertyChanged("AuditorImageSignCBU");
            }
        }

        private ImageSource _SupervisorImageSignCBU;
        public ImageSource SupervisorImageSignCBU
        {
            get => _SupervisorImageSignCBU;
            set
            {
                _SupervisorImageSignCBU = value;
                RaisePropertyChanged("SupervisorImageSignCBU");
            }

        }

        private ImageSource _AuditorImageSignCarrier;
        public ImageSource AuditorImageSignCarrier
        {
            get => _AuditorImageSignCarrier;
            set
            {
                _AuditorImageSignCarrier = value;
                RaisePropertyChanged("AuditorImageSignCarrier");
            }
        }
        private ImageSource _SupervisorImageSignCarrier;
        public ImageSource SupervisorImageSignCarrier
        {
            get => _SupervisorImageSignCarrier;
            set
            {
                _SupervisorImageSignCarrier = value;
                RaisePropertyChanged("SupervisorImageSignCarrier");
            }

        }

        private string _Signature;
        public string Signature
        {
            get => _Signature;
            set
            {
                _Signature = value;
                RaisePropertyChanged("Signature");
            }
        }

        private bool _SignaturePadPopup = false;
        public bool SignaturePadPopup
        {
            get { return _SignaturePadPopup; }
            set
            {
                _SignaturePadPopup = value;
                RaisePropertyChanged("SignaturePadPopup");
            }
        }

        private bool _IsDoneEnable = false;
        public bool IsDoneEnable
        {
            get { return _IsDoneEnable; }
            set
            {
                _IsDoneEnable = value;
                RaisePropertyChanged("IsDoneEnable");
            }
        }

        private double _DoneOpacity = 0.5;
        public double DoneOpacity
        {
            get { return _DoneOpacity; }
            set
            {
                _DoneOpacity = value;
                RaisePropertyChanged("DoneOpacity");
            }
        }

        private bool _IsResourcecVisible = false;
        public bool IsResourcecVisible
        {
            get { return _IsResourcecVisible; }
            set
            {
                _IsResourcecVisible = value;
                NotifyPropertyChanged();
            }
        }

        private string _PONumber;
        public string PONumber
        {
            get { return _PONumber; }
            set
            {
                _PONumber = value;
                RaisePropertyChanged("PONumber");
            }
        }

        private string _ShippingNumber;
        public string ShippingNumber
        {
            get { return _ShippingNumber; }
            set
            {
                _ShippingNumber = value;
                RaisePropertyChanged("ShippingNumber");
            }
        }

        private string _REQNo;
        public string REQNo
        {
            get { return _REQNo; }
            set
            {
                _REQNo = value;
                RaisePropertyChanged("REQNo");
            }
        }

        private string _TaskName;
        public string TaskName
        {
            get { return _TaskName; }
            set
            {
                _TaskName = value;
                RaisePropertyChanged("TaskName");
            }
        }

        private string _Resource;
        public string Resource
        {
            get { return _Resource; }
            set
            {
                _Resource = value;
                RaisePropertyChanged("Resource");
            }
        }

        private string _EventName;
        public string EventName
        {
            get { return _EventName; }
            set
            {
                _EventName = value;
                RaisePropertyChanged("EventName");
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

        private bool _IsSignQuestionListVisible = false;
        public bool IsSignQuestionListVisible
        {
            get { return _IsSignQuestionListVisible; }
            set
            {
                _IsSignQuestionListVisible = value;
                RaisePropertyChanged("IsSignQuestionListVisible");
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

        private Color _LoadInspTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color LoadInspTabTextColor
        {
            get => _LoadInspTabTextColor;
            set
            {
                _LoadInspTabTextColor = value;
                NotifyPropertyChanged("LoadInspTabTextColor");
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

        private bool _LoadInspTabVisibility = true;
        public bool LoadInspTabVisibility
        {
            get => _LoadInspTabVisibility;
            set
            {
                _LoadInspTabVisibility = value;
                NotifyPropertyChanged("LoadInspTabVisibility");
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
