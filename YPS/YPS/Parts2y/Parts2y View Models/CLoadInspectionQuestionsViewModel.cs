using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class CLoadInspectionQuestionsViewModel : IBase
    {
        #region IComman and data members declaration
        SendPodata sendPodata = new SendPodata();
        public INavigation Navigation { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand InspTabCmd { set; get; }
        public ICommand SignalTabCmd { set; get; }
        public ICommand QuestionClickCommand { get; set; }
        public ObservableCollection<AllPoData> SelectedPodataList;
        public QuestiionsPageHeaderData QuestiionsPageHeaderData { get; set; }
        CLoadInspectionQuestionsPage pageName;
        YPSService trackService;
        //int tagId;
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

        public CLoadInspectionQuestionsViewModel(INavigation _Navigation, CLoadInspectionQuestionsPage pagename, ObservableCollection<AllPoData> selectedpodatalist, bool isalltagdone)
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
                PONumber = SelectedPodataList[0].PONumberForDisplay;
                ShippingNumber = SelectedPodataList[0].ShippingNumberForDisplay;
                REQNo = SelectedPodataList[0].REQNoForDisplay;
                TaskName = SelectedPodataList[0].TaskName;
                EventName = SelectedPodataList[0].EventName;
                SecureStorage.SetAsync("podataTastName", SelectedPodataList[0].TaskName);
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                QuestionClickCommand = new Command<InspectionConfiguration>(QuestionClick);
                InspTabCmd = new Command(InspTabClicked);
                SignalTabCmd = new Command(SignTabClicked);
                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                PartsCmd = new Command(async () => await TabChange("parts"));
                LoadCmd = new Command(async () => await TabChange("load"));
                SignatureCmd = new Command(SignaturePadShowHide);
                HideSignaturePadCmd = new Command(SignaturePadShowHide);

                ChangeLabel();
                Task.Run(GetQuestionsLIst);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CLoadInspectionQuestionsViewModel constructor -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task GetInspSignature()
        {
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var signature = await trackService.GetInspSignatureByTask(taskid);

                    if (signature != null && signature.status == 1)
                    {
                        var supervisorimagesignCBU = signature.data.listData.
                                                Where(wr => wr.SignType == (int)InspectionSignatureType.VinSupervisor).
                                                Select(c => c.Signature).FirstOrDefault();

                        var auditorimagesignCBU = signature.data.listData.
                          Where(wr => wr.SignType == (int)InspectionSignatureType.VinAuditor).
                          Select(c => c.Signature).FirstOrDefault();

                        var supervisorimagesignCarrier = signature.data.listData.
                            Where(wr => wr.SignType == (int)InspectionSignatureType.CarrierSupervisor).
                            Select(c => c.Signature).FirstOrDefault();

                        var auditorimagesignCarrier = signature.data.listData.
                            Where(wr => wr.SignType == (int)InspectionSignatureType.CarrierAuditor).
                            Select(c => c.Signature).FirstOrDefault();

                        SupervisorImageSignCBU = supervisorimagesignCBU != null ? ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(supervisorimagesignCBU))) :
                            null;

                        AuditorImageSignCBU = auditorimagesignCBU != null ? ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(auditorimagesignCBU))) :
                        null;

                        SupervisorImageSignCarrier = supervisorimagesignCarrier != null ?
                        ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(supervisorimagesignCarrier))) : null;

                        AuditorImageSignCarrier = auditorimagesignCarrier != null ?
                        ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(auditorimagesignCarrier))) : null;

                        //if (IsAllTagsDone == true && QuestionListCategory?.Where(wr => wr.Status == 0).FirstOrDefault() == null
                        //    && supervisorimagesignCBU != null && auditorimagesignCBU != null &&
                        //    supervisorimagesignCarrier != null && auditorimagesignCarrier != null)
                        //{
                        //    IsDoneEnable = true;
                        //    DoneOpacity = 1.0;
                        //}
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetInspSignature method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "SignaturePadShowHide method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
                    CommonMethods.BackClickFromInspToParts(Navigation, SelectedPodataList[0]);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TabChange method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
                YPSLogger.TrackEvent("CLoadInspectionQuestionsViewModel.xaml.cs", "in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;
                    sendPodata.TaskName = SelectedPodataList[0].TaskName;
                    sendPodata.EventID = -1;

                    var result = await trackService.LoadPoDataService(sendPodata);

                    if (result != null && result.data != null)
                    {
                        if (result.status == 1 && result.data.allPoDataMobile != null && result.data.allPoDataMobile.Count > 0)
                        {
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoDataMobile);

                            //AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoDataMobile.Where(wr => wr.TaskID == Settings.TaskID));
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetUpdatedAllPOData method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
            return AllPoDataList;
        }

        public async void InspTabClicked()
        {
            try
            {
                loadindicator = true;

                await GetConfigurationResults(3);

                IsSignQuestionListVisible = false;
                IsQuestionListVisible = true;
                InspTabTextColor = Settings.Bar_Background;
                InspTabVisibility = true;
                SignTabTextColor = Color.Black;
                SignTabVisibility = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "InspTabClicked method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
                await GetConfigurationResults(3);

                await GetInspSignature();

                IsSignQuestionListVisible = true;
                IsQuestionListVisible = false;
                InspTabTextColor = Color.Black;
                InspTabVisibility = false;
                SignTabTextColor = Settings.Bar_Background;
                SignTabVisibility = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SignTabClicked method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
                    QuestionsList?.All(x => { x.Direct = 0; return true; });

                    var result = await trackService.GetInspectionResultsByTask(taskid);

                    if (result != null && result.data != null && result.data.listData != null)
                    {
                        inspectionResultsLists = result.data.listData;


                        foreach (InspectionConfiguration qustlist in QuestionsList)
                        {
                            foreach (InspectionResultsList anslist in inspectionResultsLists)
                            {
                                if (qustlist?.MInspectionConfigID == anslist?.QID)
                                {
                                    qustlist.Status = 1;
                                    qustlist.Direct = anslist.Direct;
                                    break;
                                }
                            }
                        }
                        QuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionsList?.Where(wr => wr.CategoryID == categoryID && wr.VersionID == Settings.VersionID).ToList());
                    }
                    else
                    {
                        QuestionListCategory = new ObservableCollection<InspectionConfiguration>(QuestionsList?.Where(wr => wr.CategoryID == categoryID && wr.VersionID == Settings.VersionID).ToList());
                    }
                    NextScanEnable = QuestionListCategory.All(a => a.Status == 1) ? true : false;
                    NextScanOpacity = NextScanEnable == false ? 0.5 : 1.0;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetConfigurationResults method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "GetQuestionsLIst method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                CommonMethods.BackClickFromInspToParts(Navigation, SelectedPodataList[0]);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Backevnttapped_click method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
                await GetConfigurationResults(inspectionConfiguration.CategoryID);
                inspectionConfiguration.SelectedTagBorderColor = Settings.Bar_Background;
                await Navigation.PushAsync(new CInspectionAnswersPage(inspectionConfiguration, QuestionListCategory,
                    inspectionResultsLists, SelectedPodataList[0], false, this, null, IsAllTagsDone, string.Join(",", EncPOTagID)), false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QuestionClick method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
                        var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var next = labelval.Where(wr => wr.FieldID == labelobj.Next.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var scan = labelval.Where(wr => wr.FieldID == labelobj.Scan.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var viewall = labelval.Where(wr => wr.FieldID == labelobj.ViewAll.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var home = labelval.Where(wr => wr.FieldID == labelobj.Home.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var jobs = labelval.Where(wr => wr.FieldID == labelobj.Jobs.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var parts = labelval.Where(wr => wr.FieldID == labelobj.Parts.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var load = labelval.Where(wr => wr.FieldID == labelobj.Load.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var carrierinsp = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.CarrierInsp.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var checklistandsign = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ChecklistAndSign.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var signaturecbu = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.SignaturesCBU.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var signaturecarrier = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.SignatureCarrier.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var supervisor = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Supervisor.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var auditor = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Auditor.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var dealer = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Dealer.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var driver = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Driver.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var done = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Done.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid?.Status == 1 || poid?.Status == 2 ? true : false;
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber?.Status == 1 || shippingnumber?.Status == 2 ? true : false;
                        labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                        labelobj.REQNo.Status = reqnumber?.Status == 1 || reqnumber?.Status == 2 ? true : false;
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme?.Status == 1 || taskanme?.Status == 2 ? true : false;
                        labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                        labelobj.EventName.Status = eventname?.Status == 1 || eventname?.Status == 2 ? true : false;
                        labelobj.ViewAll.Name = viewall != null ? (!string.IsNullOrEmpty(viewall.LblText) ? viewall.LblText : labelobj.ViewAll.Name) : labelobj.ViewAll.Name;
                        labelobj.Scan.Name = scan != null ? (!string.IsNullOrEmpty(scan.LblText) ? scan.LblText : labelobj.Scan.Name) : labelobj.Scan.Name;
                        labelobj.Next.Name = next != null ? (!string.IsNullOrEmpty(next.LblText) ? next.LblText + " " + scan.LblText : labelobj.Next.Name + " " + labelobj.Scan.Name) : labelobj.Next.Name + " " + labelobj.Scan.Name;

                        labelobj.Home.Name = home != null ? (!string.IsNullOrEmpty(home.LblText) ? home.LblText : labelobj.Home.Name) : labelobj.Home.Name;
                        labelobj.Jobs.Name = jobs != null ? (!string.IsNullOrEmpty(jobs.LblText) ? jobs.LblText : labelobj.Jobs.Name) : labelobj.Jobs.Name;
                        labelobj.Parts.Name = parts != null ? (!string.IsNullOrEmpty(parts.LblText) ? parts.LblText : labelobj.Parts.Name) : labelobj.Parts.Name;
                        labelobj.Load.Name = load != null ? (!string.IsNullOrEmpty(load.LblText) ? load.LblText : labelobj.Load.Name) : labelobj.Load.Name;

                        labelobj.CarrierInsp.Name = carrierinsp != null ? (!string.IsNullOrEmpty(carrierinsp.LblText) ? carrierinsp.LblText : labelobj.CarrierInsp.Name) : labelobj.CarrierInsp.Name;
                        labelobj.ChecklistAndSign.Name = checklistandsign != null ? (!string.IsNullOrEmpty(checklistandsign.LblText) ? checklistandsign.LblText : labelobj.ChecklistAndSign.Name) : labelobj.ChecklistAndSign.Name;
                        labelobj.SignaturesCBU.Name = signaturecbu != null ? (!string.IsNullOrEmpty(signaturecbu.LblText) ? signaturecbu.LblText : labelobj.SignaturesCBU.Name) : labelobj.SignaturesCBU.Name;
                        labelobj.SignatureCarrier.Name = signaturecarrier != null ? (!string.IsNullOrEmpty(signaturecarrier.LblText) ? signaturecarrier.LblText : labelobj.SignatureCarrier.Name) : labelobj.SignatureCarrier.Name;
                        labelobj.Supervisor.Name = supervisor != null ? (!string.IsNullOrEmpty(supervisor.LblText) ? supervisor.LblText : labelobj.Supervisor.Name) : labelobj.Supervisor.Name;
                        labelobj.Auditor.Name = auditor != null ? (!string.IsNullOrEmpty(auditor.LblText) ? auditor.LblText : labelobj.Auditor.Name) : labelobj.Auditor.Name;
                        labelobj.Dealer.Name = dealer != null ? (!string.IsNullOrEmpty(dealer.LblText) ? dealer.LblText : labelobj.Dealer.Name) : labelobj.Dealer.Name;
                        labelobj.Driver.Name = driver != null ? (!string.IsNullOrEmpty(driver.LblText) ? driver.LblText : labelobj.Driver.Name) : labelobj.Driver.Name;
                        labelobj.Done.Name = done != null ? (!string.IsNullOrEmpty(done.LblText) ? done.LblText : labelobj.Done.Name) : labelobj.Done.Name;

                        if (Settings.EntityTypeName.Trim().ToLower() == "Dealer".Trim().ToLower())
                        {
                            SupervisorOrDriver = labelobj.Driver.Name;
                            AuditorOrDealer = labelobj.Dealer.Name;
                        }
                        else
                        {
                            SupervisorOrDriver = labelobj.Supervisor.Name;
                            AuditorOrDealer = labelobj.Auditor.Name;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ChangeLabel method -> in CLoadInspectionQuestionsViewModel.cs " + Settings.userLoginID);
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
            public DashboardLabelFields Home { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMHome" };
            public DashboardLabelFields Jobs { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMTask" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMParts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMLoad" };

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

            public DashboardLabelFields CarrierInsp { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TBMCarrierInsp"
            };
            public DashboardLabelFields ChecklistAndSign { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TBMChecklistAndSign"
            };
            public DashboardLabelFields SignaturesCBU { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMSignaturesCBU"
            };
            public DashboardLabelFields SignatureCarrier { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMSignatureCarrier"
            };
            public DashboardLabelFields Supervisor { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMSupervisor"
            };
            public DashboardLabelFields Auditor { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMAuditor"
            };
            public DashboardLabelFields Dealer { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMDealer"
            };
            public DashboardLabelFields Driver { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMDriver"
            };
            public DashboardLabelFields Done { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMbtnDone"
            };
            public DashboardLabelFields Next { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMbtnNext" };
            public DashboardLabelFields Scan { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMScan" };
            public DashboardLabelFields ViewAll { get; set; } = new DashboardLabelFields { Status = false, Name = "LCMbtnViewAll" };
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

        private double _NextScanOpacity = 0.5;
        public double NextScanOpacity
        {
            get { return _NextScanOpacity; }
            set
            {
                _NextScanOpacity = value;
                RaisePropertyChanged("NextScanOpacity");
            }
        }
        private bool _NextScanEnable;
        public bool NextScanEnable
        {
            get { return _NextScanEnable; }
            set
            {
                _NextScanEnable = value;
                RaisePropertyChanged("NextScanEnable");
            }
        }

        private string _SupervisorOrDriver;
        public string SupervisorOrDriver
        {
            get => _SupervisorOrDriver;
            set
            {
                _SupervisorOrDriver = value;
                RaisePropertyChanged("SupervisorOrDriver");
            }
        }
        //private string _SupervisorOrDriver = Settings.EntityTypeName.Trim().ToLower() == "Dealer".Trim().ToLower() ? "Driver" : "Supervisor";
        //public string SupervisorOrDriver
        //{
        //    get => _SupervisorOrDriver;
        //    set
        //    {
        //        _SupervisorOrDriver = value;
        //        RaisePropertyChanged("SupervisorOrDriver");
        //    }
        //}

        private string _AuditorOrDealer = Settings.EntityTypeName.Trim().ToLower() == "Dealer".Trim().ToLower() ? "Dealer" : "Auditor";
        public string AuditorOrDealer
        {
            get => _AuditorOrDealer;
            set
            {
                _AuditorOrDealer = value;
                RaisePropertyChanged("AuditorOrDealer");
            }
        }

        //private string _AuditorOrDealer = Settings.EntityTypeName.Trim().ToLower() == "Dealer".Trim().ToLower() ? "Dealer" : "Auditor";
        //public string AuditorOrDealer
        //{
        //    get => _AuditorOrDealer;
        //    set
        //    {
        //        _AuditorOrDealer = value;
        //        RaisePropertyChanged("AuditorOrDealer");
        //    }
        //}

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

        private bool _IsDoneEnable = true;
        public bool IsDoneEnable
        {
            get { return _IsDoneEnable; }
            set
            {
                _IsDoneEnable = value;
                RaisePropertyChanged("IsDoneEnable");
            }
        }

        private double _DoneOpacity = 1.0;
        public double DoneOpacity
        {
            get { return _DoneOpacity; }
            set
            {
                _DoneOpacity = value;
                RaisePropertyChanged("DoneOpacity");
            }
        }

        private bool _IsResourcecVisible = true;
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

        private Color _InspTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color InspTabTextColor
        {
            get => _InspTabTextColor;
            set
            {
                _InspTabTextColor = value;
                NotifyPropertyChanged("InspTabTextColor");
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
