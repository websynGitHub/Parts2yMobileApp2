﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    public class CVinInspectQuestionsPageViewModel : IBase
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
        public ICommand DriverSignatureCmd { get; set; }
        public ICommand HideSignaturePadCmd { get; set; }
        public QuestiionsPageHeaderData QuestiionsPageHeaderData { get; set; }
        CVinInspectQuestionsPage pageName;
        YPSService trackService;
        int tagId, taskid;
        bool isAllDone;
        List<InspectionResultsList> inspectionResultsLists;
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }
        #endregion

        public CVinInspectQuestionsPageViewModel(INavigation _Navigation, CVinInspectQuestionsPage pagename, AllPoData selectedtagdata, bool isalldone)
        {
            try
            {
                Navigation = _Navigation;
                trackService = new YPSService();
                pageName = pagename;
                isAllDone = isalldone;
                selectedTagData = selectedtagdata;
                this.tagId = selectedTagData.POTagID;
                taskid = selectedTagData.TaskID;
                TagNumber = selectedTagData.TagNumber;
                IndentCode = selectedTagData.IdentCode;
                IsConditionNameLabelVisible = string.IsNullOrEmpty(selectedTagData.ConditionName) ? false : true;
                ConditionName = selectedTagData.ConditionName;
                TaskName = selectedTagData.TaskName;
                EventName = selectedTagData.EventName;
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                QuestionClickCommand = new Command<InspectionConfiguration>(QuestionClick);
                Task.Run(() => ChangeLabel()).Wait();
                Task.Run(() => GetQuestionsLIst()).Wait();

                QuickTabCmd = new Command(QuickTabClicked);
                FullTabCmd = new Command(FullTabClicked);
                SignalTabCmd = new Command(SignTabClicked);
                DriverSignatureCmd = new Command(SignaturePadShowHide);
                HideSignaturePadCmd = new Command(SignaturePadShowHide);
                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                PartsCmd = new Command(async () => await TabChange("parts"));

                if (selectedTagData?.TaskResourceID == Settings.userLoginID)
                {
                    LoadCmd = new Command(async () => await TabChange("load"));
                }
                else
                {
                    LoadTextColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "VinInspectQuestionsPageViewModel constructor -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task GetInspSignature()
        {
            try
            {
                var result = await trackService.GetInspSignatureByTag(taskid, tagId);

                if (Settings.EntityTypeName.Trim().ToLower() == "Dealer".Trim().ToLower())
                {
                    IsDealerSignVisible = true;
                    IsOwnerSignVisible = false;
                    IsSignatureCarrierVisible = IsLoadTabVisible == true ? false : true;

                    var carrierdriverimagesign = result?.data?.listData.
                        Where(wr => wr.SignType == (int)InspectionSignatureType.CarrierDriver).Select(c => c.Signature).FirstOrDefault();

                    var vindealerimagesigncarrier = result?.data?.listData.
                        Where(wr => wr.SignType == (int)InspectionSignatureType.VinDealer).Select(c => c.Signature).FirstOrDefault();

                    CarrierDriverImageSign = carrierdriverimagesign != null ?
                        ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(carrierdriverimagesign))) : null;

                    VINDealerImageSignCarrier = vindealerimagesigncarrier != null ?
                        ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(vindealerimagesigncarrier))) : null;

                    if (selectedTagData.TagTaskStatus != 2 && ((IsLoadTabVisible == false && carrierdriverimagesign != null && vindealerimagesigncarrier != null) || (IsLoadTabVisible == true)) &&
                   ((QuickSignQuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null &&
                   FullSignQuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null) ||
                   (FullSignQuestionListCategory == null && QuickSignQuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null) ||
                   (QuickSignQuestionListCategory == null && FullSignQuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null))
                   )
                    {
                        IsDoneEnable = true;
                        DoneOpacity = 1.0;
                    }
                }
                else if (Settings.EntityTypeName.Trim().ToLower() == "Owner".Trim().ToLower())
                {
                    IsDealerSignVisible = false;
                    IsOwnerSignVisible = true;
                    IsSignatureCarrierVisible = IsLoadTabVisible == true ? false : true;

                    var driverimagesign = result?.data?.listData?.
                        Where(wr => wr.SignType == (int)InspectionSignatureType.VinDriver).
                        Select(c => c.Signature).FirstOrDefault();


                    DriverImageSign = driverimagesign != null ?
                        ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(driverimagesign))) : null;

                    if (selectedTagData.TagTaskStatus != 2 && ((IsLoadTabVisible == false && driverimagesign != null) || (IsLoadTabVisible == true)) &&
                   ((QuickSignQuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null &&
                   FullSignQuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null) ||
                   (FullSignQuestionListCategory == null && QuickSignQuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null) ||
                   (QuickSignQuestionListCategory == null && FullSignQuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null))
                   )
                    {
                        IsDoneEnable = true;
                        DoneOpacity = 1.0;
                    }
                }
                else
                {
                    IsSignatureCarrierVisible = false;

                    if ((QuickSignQuestionListCategory != null && QuickSignQuestionListCategory.Where(wr => wr.Status == 0).FirstOrDefault() == null &&
                      FullSignQuestionListCategory != null && FullSignQuestionListCategory.Where(wr => wr.Status == 0).FirstOrDefault() == null) ||
                      (FullSignQuestionListCategory == null && QuickSignQuestionListCategory != null && QuickSignQuestionListCategory.Where(wr => wr.Status == 0).FirstOrDefault() == null) ||
                      (QuickSignQuestionListCategory == null && FullSignQuestionListCategory != null && FullSignQuestionListCategory.Where(wr => wr.Status == 0).FirstOrDefault() == null) &&
                      selectedTagData.TagTaskStatus != 2)
                    {
                        IsDoneEnable = true;
                        DoneOpacity = 1.0;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetInspSignature method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async void SignaturePadShowHide(object sender)
        {
            try
            {
                var sign = sender as Label;
                var back = sender as YPS.CustomRenders.FontAwesomeIconLabel;

                if (back != null)
                {
                    SignaturePadPopup = false;
                    SignTabVisibility = true;
                }
                else
                {
                    SignaturePadPopup = true;
                    SignTabVisibility = false;
                }

                if (sign != null)
                {
                    Signature = sign.StyleId;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SignaturePadShowHide method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task TabChange(string tabname)
        {
            try
            {
                if (tabname.Trim().ToLower() == "home".Trim().ToLower())
                {
                    loadindicator = true;
                    App.Current.MainPage = new MenuPage(typeof(HomePage));
                }
                else if (tabname.Trim().ToLower() == "job".Trim().ToLower())
                {
                    loadindicator = true;

                    if (Settings.POID != 0)
                    {
                        if (Navigation.NavigationStack.Count() == 5)
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[2]);
                            Navigation.RemovePage(Navigation.NavigationStack[2]);
                        }
                        else
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                        }
                        Settings.POID = 0;
                        Settings.TaskID = 0;
                    }
                    else
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[2]);
                        Navigation.RemovePage(Navigation.NavigationStack[2]);
                    }
                    await Navigation.PopAsync();
                }
                else if (tabname.Trim().ToLower() == "parts".Trim().ToLower())
                {
                    loadindicator = true;

                    if (Settings.POID != 0)
                    {
                        if (Navigation.NavigationStack.Count() == 5)
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[3]);
                        }
                        else
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                            Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                        }
                        Settings.POID = 0;
                        Settings.TaskID = 0;
                    }
                    else
                    {
                        Navigation.RemovePage(Navigation.NavigationStack[3]);
                    }
                    await Navigation.PopAsync();
                }
                else if (tabname.Trim().ToLower() == "load".Trim().ToLower())
                {
                    ObservableCollection<AllPoData> preparelist = new ObservableCollection<AllPoData>();
                    preparelist.Add(selectedTagData);
                    await Navigation.PushAsync(new CLoadInspectionQuestionsPage(preparelist, isAllDone));
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                YPSLogger.ReportException(ex, "TabChange method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task<ObservableCollection<AllPoData>> GetUpdatedAllPOData()
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
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.TaskID == Settings.TaskID));
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
                YPSLogger.ReportException(ex, "GetUpdatedAllPOData method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
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

                IsSignQuestionListVisible = false;
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
                YPSLogger.ReportException(ex, "QuickTabClicked method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async void FullTabClicked()
        {
            try
            {
                loadindicator = true;

                await GetConfigurationResults(2);

                IsSignQuestionListVisible = false;
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
                YPSLogger.ReportException(ex, "FullTabClicked method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
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

                if (IsQuickTabVisible == true)
                {
                    await GetConfigurationResults(1);
                    QuickSignQuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionListCategory.Where(wr => wr.CategoryID == 1).ToList());
                    QuickSignQuestionListCategory.Where(wr => wr.Status == 1).ToList().ForEach(l => { l.SignQuesBgColor = Color.FromHex("#005800"); });
                }

                if (IsFullTabVisible == true)
                {
                    await GetConfigurationResults(2);
                    FullSignQuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionListCategory.Where(wr => wr.CategoryID == 2).ToList());
                    FullSignQuestionListCategory.Where(wr => wr.Status == 1).ToList().ForEach(l => { l.SignQuesBgColor = Color.FromHex("#005800"); });
                }

                await GetInspSignature();

                IsSignQuestionListVisible = true;
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
                YPSLogger.ReportException(ex, "SignTabClicked method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
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
                    QuestionsList?.All(x => { x.SelectedTagBorderColor = Color.Transparent; return true; });
                    QuestionsList?.All(x => { x.Status = 0; return true; });

                    var result = await trackService.GetInspectionResultsService(taskid, tagId);

                    if (result != null && result.data != null && result.data.listData != null)
                    {
                        inspectionResultsLists = result.data.listData;
                        QuestionsList?.Where(x => inspectionResultsLists.Any(z => z.QID == x.MInspectionConfigID)).Select(x => { x.Status = 1; return x; }).ToList();

                        QuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionsList?.Where(wr => wr.CategoryID == categoryID && wr.VersionID == Settings.VersionID).ToList());
                        QuestionListCategory.Where(wr => string.IsNullOrEmpty(wr.Area)).ToList().ForEach(s => { s.AreBgColor = Color.Transparent; });
                    }
                    else
                    {
                        QuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionsList?.Where(wr => wr.CategoryID == categoryID && wr.VersionID == Settings.VersionID).ToList());
                        QuestionListCategory.Where(wr => string.IsNullOrEmpty(wr.Area)).ToList().ForEach(s => { s.AreBgColor = Color.Transparent; });
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetConfigurationResults method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "GetQuestionsLIst method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
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
                    }
                }
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Backevnttapped_click method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async void QuestionClick(InspectionConfiguration inspectionConfiguration)
        {
            try
            {
                loadindicator = true;

                inspectionConfiguration.SelectedTagBorderColor = Settings.Bar_Background;
                await GetConfigurationResults(inspectionConfiguration.CategoryID);
                await Navigation.PushAsync(new CInspectionAnswersPage(inspectionConfiguration,
                    new ObservableCollection<InspectionConfiguration>(QuestionListCategory.Where(wr => wr.CategoryID == inspectionConfiguration.CategoryID).ToList())
                    , inspectionResultsLists, selectedTagData, true, null, this));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QuestionClick method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
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
                        var tagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.IdentCode.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ConditionName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();


                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber == null ? true : (tagnumber.Status == 1 ? true : false);
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode == null ? true : (identcode.Status == 1 ? true : false);
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname == null ? true : (conditionname.Status == 1 ? true : false);
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme == null ? true : (taskanme.Status == 1 ? true : false);
                        labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                        labelobj.EventName.Status = eventname == null ? true : (eventname.Status == 1 ? true : false);

                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    IsQuickTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "CQuickInspection".Trim().ToLower()).FirstOrDefault()) != null ? true : false;
                    IsFullTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "CFullInspection".Trim().ToLower()).FirstOrDefault()) != null ? true : false;

                    //if (Settings.VersionID == 2)
                    //{
                    IsSignatureCarrierVisible = (IsLoadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "CCarrierInspection".Trim().ToLower()).FirstOrDefault()) != null ? true : false) == true ? false : true;
                    SignTabText = IsLoadTabVisible == false ? "Checklist & Sign" : "Checklist";
                    //}

                    if (IsQuickTabVisible == false && IsFullTabVisible == false)
                    {
                        SignTabClicked();
                    }

                    if (IsQuickTabVisible == true)
                    {
                        QuickTabVisibility = true;
                        FullTabVisibility = false;
                        SignTabVisibility = false;
                    }
                    else
                    {
                        FullTabVisibility = true;
                        QuickTabVisibility = false;
                        SignTabVisibility = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabel method -> in VinInspectQuestionsPageViewModel " + Settings.userLoginID);
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
            public DashboardLabelFields TagNumber { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TagNumber"
            };
            public DashboardLabelFields TaskName { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "TaskName"
            };

            public DashboardLabelFields EventName { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "Event"
            };
            public DashboardLabelFields IdentCode { get; set; } = new DashboardLabelFields { Status = true, Name = "IdentCode" };
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

        //private base64 _SignaturePadPopup = false;
        //public bool SignaturePadPopup
        //{
        //    get { return _SignaturePadPopup; }
        //    set
        //    {
        //        _SignaturePadPopup = value;
        //        RaisePropertyChanged("SignaturePadPopup");
        //    }
        //}
        private bool _IsConditionNameLabelVisible = true;
        public bool IsConditionNameLabelVisible
        {
            get { return _IsConditionNameLabelVisible; }
            set
            {
                _IsConditionNameLabelVisible = value;
                RaisePropertyChanged("IsConditionNameLabelVisible");
            }
        }

        private bool _IsDealerSignVisible;
        public bool IsDealerSignVisible
        {
            get => _IsDealerSignVisible;
            set
            {
                _IsDealerSignVisible = value;
                RaisePropertyChanged("IsDealerSignVisible");
            }
        }

        private bool _IsOwnerSignVisible;
        public bool IsOwnerSignVisible
        {
            get => _IsOwnerSignVisible;
            set
            {
                _IsOwnerSignVisible = value;
                RaisePropertyChanged("IsOwnerSignVisible");
            }
        }

        private ImageSource _CarrierDriverImageSign;
        public ImageSource CarrierDriverImageSign
        {
            get => _CarrierDriverImageSign;
            set
            {
                _CarrierDriverImageSign = value;
                RaisePropertyChanged("CarrierDriverImageSign");
            }
        }

        private ImageSource _VINDealerImageSignCarrier;
        public ImageSource VINDealerImageSignCarrier
        {
            get => _VINDealerImageSignCarrier;
            set
            {
                _VINDealerImageSignCarrier = value;
                RaisePropertyChanged("VINDealerImageSignCarrier");
            }
        }

        private ImageSource _DriverImageSign;
        public ImageSource DriverImageSign
        {
            get => _DriverImageSign;
            set
            {
                _DriverImageSign = value;
                RaisePropertyChanged("DriverImageSign");
            }
        }

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

        private string _SignTabText = "Sign";
        public string SignTabText
        {
            get { return _SignTabText; }
            set
            {
                _SignTabText = value;
                RaisePropertyChanged("SignTabText");
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

        private bool _IsFullSignQuestionListVisible = false;
        public bool IsFullSignQuestionListVisible
        {
            get { return _IsFullSignQuestionListVisible; }
            set
            {
                _IsFullSignQuestionListVisible = value;
                RaisePropertyChanged("IsFullSignQuestionListVisible");
            }
        }

        private bool _IsQuickSignQuestionListVisible = false;
        public bool IsQuickSignQuestionListVisible
        {
            get { return _IsQuickSignQuestionListVisible; }
            set
            {
                _IsQuickSignQuestionListVisible = value;
                RaisePropertyChanged("IsQuickSignQuestionListVisible");
            }
        }

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

        private ObservableCollection<InspectionConfiguration> _QuickSignQuestionListCategory;
        public ObservableCollection<InspectionConfiguration> QuickSignQuestionListCategory
        {
            get => _QuickSignQuestionListCategory;
            set
            {
                _QuickSignQuestionListCategory = value;
                RaisePropertyChanged("QuickSignQuestionListCategory");
            }
        }

        private ObservableCollection<InspectionConfiguration> _FullSignQuestionListCategory;
        public ObservableCollection<InspectionConfiguration> FullSignQuestionListCategory
        {
            get => _FullSignQuestionListCategory;
            set
            {
                _FullSignQuestionListCategory = value;
                RaisePropertyChanged("FullSignQuestionListCategory");
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