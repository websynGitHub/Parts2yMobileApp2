using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    public class CInspectionAnswersPageViewModel : IBase
    {
        #region IComman and data members declaration

        public INavigation Navigation { get; set; }
        public ICommand ViewallClick { get; set; }
        public ICommand NextClick { get; set; }
        public ICommand PhotoClickCommand { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand InspTabCmd { set; get; }
        public ICommand QuickTabCmd { set; get; }
        public ICommand FullTabCmd { set; get; }
        public ICommand SignalTabCmd { set; get; }
        //public QuestiionsPageHeaderData QuestiionsPageHeaderData { get; set; }
        CInspectionAnswersPage pagename;
        AllPoData selectedTagData;
        YPSService trackService;
        int tagId, taskid, photoCounts;
        public bool isInspVIN, isAllDone;
        Stream picStream;
        string extension = "", Mediafile, fileName;
        int count;
        ObservableCollection<InspectionConfiguration> inspectionConfigurationList;
        ObservableCollection<InspectionPhotosResponseListData> finalPhotoListA;
        List<InspectionResultsList> inspectionResultsList;
        CLoadInspectionQuestionsViewModel CarQueVm;
        CVinInspectQuestionsPageViewModel VINQueVm;

        #endregion

        public CInspectionAnswersPageViewModel(INavigation _Navigation, CInspectionAnswersPage page, InspectionConfiguration inspectionConfiguration, ObservableCollection<InspectionConfiguration> inspectionConfigurationList,
            List<InspectionResultsList> inspectionResultsList, AllPoData selectedtagdata, bool isVINInsp,
             CLoadInspectionQuestionsViewModel carcueVm, CVinInspectQuestionsPageViewModel vinqueVm, bool isalldone = false)
        {
            try
            {
                Navigation = _Navigation;
                CarQueVm = carcueVm;
                VINQueVm = vinqueVm;
                trackService = new YPSService();
                finalPhotoListA = new ObservableCollection<InspectionPhotosResponseListData>();
                pagename = page;
                isInspVIN = isVINInsp;
                isAllDone = isalldone;
                selectedTagData = selectedtagdata;
                this.tagId = selectedtagdata.POTagID;
                taskid = selectedtagdata.TaskID;
                PONumber = selectedtagdata.PONumber;
                ShippingNumber = selectedtagdata.ShippingNumber;
                REQNo = selectedtagdata.REQNo;
                TaskName = selectedtagdata.TaskName;
                Resource = selectedtagdata.TaskResourceName;
                EventName = selectedtagdata.EventName;
                IsResourcecVisible = selectedtagdata.TaskResourceID == Settings.userLoginID ? false : true;
                TagNumber = selectedtagdata.TagNumber;
                IndentCode = selectedtagdata.IdentCode;
                IsConditionNameLabelVisible = string.IsNullOrEmpty(selectedTagData.ConditionName) ? false : true;
                ConditionName = selectedtagdata.ConditionName;
                TagNumber = selectedtagdata.TagNumber;
                IndentCode = selectedtagdata.IdentCode;
                ConditionName = selectedtagdata.ConditionName;
                InspectionConfiguration = inspectionConfiguration;
                this.inspectionConfigurationList = inspectionConfigurationList;
                this.inspectionResultsList = inspectionResultsList;
                ShowConfigurationOptions();

                ViewallClick = new Command(ViewallClickMethod);
                NextClick = new Command(NextClickMethod);
                InspTabCmd = new Command(InspTabClicked);
                QuickTabCmd = new Command(QuickTabClicked);
                FullTabCmd = new Command(FullTabClicked);
                SignalTabCmd = new Command(SignTabClicked);
                PhotoClickCommand = new Command(async () => await SelectPic());
                Backevnttapped = new Command(async () => await Backevnttapped_click());

                Task.Run(() => ChangeLabel()).Wait();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "VinInspectionAnswersPageViewModel constructor -> in VinInspectionAnswersPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async void QuickTabClicked()
        {
            try
            {
                loadindicator = true;
                IsAnswersVisible = true;
                QuickTabTextColor = Settings.Bar_Background;
                QuickTabVisibility = true;
                FullTabTextColor = Color.Black;
                FullTabVisibility = false;
                SignTabTextColor = Color.Black;
                SignTabVisibility = false;
                VINQueVm.QuickTabVisibility = true;
                VINQueVm.FullTabVisibility = false;
                VINQueVm.SignTabVisibility = false;
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QuickTabClicked method -> in VinInspectionAnswersPageViewModel " + Settings.userLoginID);
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
                IsAnswersVisible = true;
                QuickTabTextColor = Color.Black;
                QuickTabVisibility = false;
                FullTabTextColor = Settings.Bar_Background;
                FullTabVisibility = true;
                SignTabTextColor = Color.Black;
                SignTabVisibility = false;
                VINQueVm.QuickTabVisibility = false;
                VINQueVm.FullTabVisibility = true;
                VINQueVm.SignTabVisibility = false;
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FullTabClicked method -> in VinInspectionAnswersPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async void InspTabClicked()
        {
            try
            {
                loadindicator = true;
                CarQueVm.InspTabVisibility = true;
                CarQueVm.SignTabVisibility = false;
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "InspTabClicked method -> in VinInspectionAnswersPageViewModel " + Settings.userLoginID);
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
                InspTabTextColor = Color.Black;
                InspTabVisibility = false;
                QuickTabTextColor = Color.Black;
                QuickTabVisibility = false;
                FullTabTextColor = Color.Black;
                FullTabVisibility = false;
                SignTabTextColor = Settings.Bar_Background;
                SignTabVisibility = true;

                if (CarQueVm != null)
                {
                    CarQueVm.InspTabVisibility = false;
                    CarQueVm.SignTabVisibility = true;
                }

                if (VINQueVm != null)
                {
                    VINQueVm.QuickTabVisibility = false;
                    VINQueVm.FullTabVisibility = false;
                    VINQueVm.SignTabVisibility = true;
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SignTabClicked method -> in VinInspectionAnswersPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task GetInspectionPhotos()
        {
            try
            {
                loadindicator = true;
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (IsInspTabVisible == true)
                    {
                        var result = await trackService.GeInspectionPhotosByTask(taskid, InspectionConfiguration?.MInspectionConfigID);

                        if (result != null && result.data != null && result.data.listData != null && result.data.listData.Count > 0)
                        {
                            ImagesCount = result.data.listData.Count;
                        }
                        else
                        {
                            ImagesCount = 0;
                        }
                    }
                    else
                    {
                        var vinresult = await trackService.GeInspectionPhotosByTag(taskid, tagId, InspectionConfiguration?.MInspectionConfigID);

                        if (vinresult != null && vinresult.data != null && vinresult.data.listData != null && vinresult.data.listData.Count > 0)
                        {
                            ImagesCount = vinresult.data.listData.Count;
                        }
                        else
                        {
                            ImagesCount = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetInspectionPhotos method -> in VinInspectionAnswersPageViewModel " + Settings.userLoginID);
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
                labelobj = new DashboardLabelChangeClass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        //Assigning the Labels & Show/Hide the controls based on the data
                        if (isInspVIN == true)
                        {
                            var tagnumber = labelval.Where(wr => wr.FieldID == labelobj.TagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var identcode = labelval.Where(wr => wr.FieldID == labelobj.IdentCode.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var conditionname = labelval.Where(wr => wr.FieldID == labelobj.ConditionName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                            labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                            labelobj.TagNumber.Status = tagnumber == null ? false : (tagnumber.Status == 1 ? true : false);
                            labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                            labelobj.IdentCode.Status = identcode == null ? false : (identcode.Status == 1 ? true : false);
                            labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                            labelobj.ConditionName.Status = conditionname == null ? false : (conditionname.Status == 1 ? true : false);
                            labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                            labelobj.TaskName.Status = taskanme == null ? false : (taskanme.Status == 1 ? true : false);
                            labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                            labelobj.EventName.Status = eventname == null ? false : (eventname.Status == 1 ? true : false);
                            IsResourcecVisible = false;

                            if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                            {
                                IsQuickTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CQuickInspection".Trim()).FirstOrDefault()) != null ? true : false;
                                IsFullTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CFullInspection".Trim()).FirstOrDefault()) != null ? true : false;

                                var isloadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CCarrierInspection".Trim()).FirstOrDefault()) != null ? true : false;
                                SignTabText = isloadTabVisible == false ? "Checklist & Sign" : "Checklist";
                            }

                            if (selectedTagData?.TaskResourceID == Settings.userLoginID)
                            {
                                if (IsQuickTabVisible == false && IsFullTabVisible == false)
                                {
                                    SignTabClicked();
                                }
                                else if (IsQuickTabVisible == true && IsFullTabVisible == true)
                                {
                                    if (InspectionConfiguration.CategoryID == 1)
                                    {
                                        QuickTabClicked();
                                    }
                                    else if (InspectionConfiguration.CategoryID == 2)
                                    {
                                        FullTabClicked();
                                    }
                                }
                                else if (IsQuickTabVisible == true && inspectionConfigurationList[0].CategoryID == 1)
                                {
                                    IsFullTabVisible = false;
                                    QuickTabClicked();
                                }
                                else
                                {
                                    IsQuickTabVisible = false;
                                    FullTabClicked();
                                }
                            }
                            else
                            {
                                IsQuickTabVisible = true;
                                IsFullTabVisible = false;
                                SignTabText = "Checklist & Sign";
                                QuickTabClicked();
                            }
                        }
                        else
                        {
                            var poid = labelval.Where(wr => wr.FieldID == labelobj.POID.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var shippingnumber = labelval.Where(wr => wr.FieldID == labelobj.ShippingNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var reqnumber = labelval.Where(wr => wr.FieldID == labelobj.REQNo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var taskanme = labelval.Where(wr => wr.FieldID == labelobj.TaskName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var resource = labelval.Where(wr => wr.FieldID == labelobj.Resource.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                            labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                            labelobj.POID.Status = poid == null ? true : (poid.Status == 1 ? true : false);
                            labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                            labelobj.ShippingNumber.Status = shippingnumber == null ? true : (shippingnumber.Status == 1 ? true : false);
                            labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                            labelobj.REQNo.Status = reqnumber == null ? true : (reqnumber.Status == 1 ? true : false);
                            labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                            labelobj.TaskName.Status = taskanme == null ? true : (taskanme.Status == 1 ? true : false);
                            labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                            labelobj.EventName.Status = eventname == null ? true : (eventname.Status == 1 ? true : false);
                            labelobj.Resource.Name = (resource != null ? (!string.IsNullOrEmpty(resource.LblText) ? resource.LblText : labelobj.Resource.Name) : labelobj.Resource.Name) + " :";

                            //if (Settings.VersionID == 2)
                            //{
                            SignTabText = "Checklist & Sign";
                            //}

                            InspTabClicked();
                            IsInspTabVisible = true;

                            if (isAllDone == false)
                            {
                                DoneOpacity = 0.5;
                                IsDoneEnable = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabel method -> in VinInspectionAnswersPageViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on the Camera icon for uploading photo.
        /// </summary>
        /// <returns></returns>
        public async Task SelectPic()
        {
            loadindicator = true;

            try
            {
                await Navigation.PushAsync(new InspectionPhotosPage(this.tagId, InspectionConfiguration, TagNumber, selectedTagData, IsInspTabVisible == true ? true : false));
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "SelectPic method -> in VinInspectionAnswersPageViewModel.cs " + Settings.userLoginID);
            }
            finally
            {
                loadindicator = false;
            }
        }

        private async void NextClickMethod()
        {
            try
            {
                loadindicator = true;

                if (NextButtonText == "COMPLETE")
                {
                    SignTabClicked();
                }

                if (FrontRightTrue || FrontRightFalse || FrontLeftTrue || FrontLeftFalse || RearLeftTrue
                    || RearLeftFalse || RearRightTrue || RearRightFalse || PlaneTrue || PlaneFalse)
                {
                    var checkInternet = await App.CheckInterNetConnection();
                    if (checkInternet)
                    {
                        await DoneClicked();
                        UpdateInspectionRequest updateInspectionRequest = new UpdateInspectionRequest();
                        updateInspectionRequest.QID = InspectionConfiguration.MInspectionConfigID;

                        if (RearLeftTrue == true || RearLeftFalse == true)
                        {
                            updateInspectionRequest.BackLeft = RearLeftTrue ? 2 : 1;
                        }

                        if (RearRightTrue == true || RearRightFalse == true)
                        {
                            updateInspectionRequest.BackRight = RearRightTrue ? 2 : 1;
                        }

                        if (PlaneTrue == true || PlaneFalse == true)
                        {
                            updateInspectionRequest.Direct = PlaneTrue ? 2 : 1;
                        }

                        if (FrontLeftTrue == true || FrontLeftFalse == true)
                        {
                            updateInspectionRequest.FrontLeft = FrontLeftTrue ? 2 : 1;
                        }

                        if (FrontRightTrue == true || FrontRightFalse == true)
                        {
                            updateInspectionRequest.FrontRight = FrontRightTrue ? 2 : 1;
                        }

                        if (IsInspTabVisible == false)
                        {
                            updateInspectionRequest.POTagID = tagId;
                        }

                        updateInspectionRequest.TaskID = taskid;
                        updateInspectionRequest.Remarks = Remarks;
                        updateInspectionRequest.UserID = Settings.userLoginID;
                        var result = await trackService.InsertUpdateInspectionResult(updateInspectionRequest);

                        if (result != null && result.status == 1)
                        {
                            if (InspectionConfiguration.SerialNo == inspectionConfigurationList.Count)
                            {
                                NextButtonText = "COMPLETE";
                            }

                            if (InspectionConfiguration.SerialNo < inspectionConfigurationList.Count)
                            {
                                FrontRightTrue = false;
                                FrontRightFalse = false;
                                FrontLeftTrue = false;
                                FrontLeftFalse = false;
                                RearLeftTrue = false;
                                RearLeftFalse = false;
                                RearRightTrue = false;
                                RearRightFalse = false;
                                PlaneTrue = false;
                                PlaneFalse = false;
                                Remarks = string.Empty;

                                AnswersGridVisibility = false;

                                InspectionConfiguration = inspectionConfigurationList.FirstOrDefault(x => x.SerialNo == InspectionConfiguration.SerialNo + 1);

                                ShowConfigurationOptions();
                            }
                            else
                            {
                                SignTabClicked();
                            }
                        }
                    }
                }
                else
                {
                    if (InspectionConfiguration.SerialNo < inspectionConfigurationList.Count)
                    {
                        InspectionConfiguration = inspectionConfigurationList.FirstOrDefault(x => x.SerialNo == InspectionConfiguration.SerialNo + 1);
                        ShowConfigurationOptions();
                        Remarks = string.Empty;
                    }
                    else
                    {
                        SignTabClicked();
                    }
                }
                await GetInspectionPhotos();
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "NextClickMethod method -> in VinInspectionAnswersPageViewModel.cs " + Settings.userLoginID);
            }
            finally
            {
                loadindicator = false;
            }
        }

        private async void ViewallClickMethod()
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ViewallClickMethod method -> in VinInspectionAnswersPageViewModel.cs " + Settings.userLoginID);
            }
        }

        private async Task Backevnttapped_click()
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Backevnttapped_click method -> in VinInspectionAnswersPageViewModel.cs " + Settings.userLoginID);
            }
        }

        private async Task DoneClicked()
        {
            try
            {
                if (selectedTagData.TaskID != 0 && selectedTagData.TagTaskStatus == 0)
                {
                    TagTaskStatus tagtaskstatus = new TagTaskStatus();
                    tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                    tagtaskstatus.POTagID = Helperclass.Encrypt(selectedTagData.POTagID.ToString());
                    tagtaskstatus.Status = 1;
                    tagtaskstatus.CreatedBy = Settings.userLoginID;

                    var result = await trackService.UpdateTagTaskStatus(tagtaskstatus);

                    if (result.status == 1)
                    {
                        if (selectedTagData.TaskStatus == 0)
                        {
                            TagTaskStatus taskstatus = new TagTaskStatus();
                            taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                            taskstatus.TaskStatus = 1;
                            taskstatus.CreatedBy = Settings.userLoginID;

                            var taskval = await trackService.UpdateTaskStatus(taskstatus);
                        }
                        selectedTagData.TagTaskStatus = 1;
                        selectedTagData.TaskStatus = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked method -> in VinInspectionAnswersPageViewModel.cs  " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private void ShowConfigurationOptions()
        {
            try
            {
                loadindicator = true;

                FrontLeft = InspectionConfiguration.FrontLeft == 1 || InspectionConfiguration.IsFront == 1;
                FrontRight = InspectionConfiguration.FrontRight == 1;
                RearLeft = InspectionConfiguration.BackLeft == 1 || InspectionConfiguration.IsBack == 1;
                RearRight = InspectionConfiguration.BackRight == 1;
                RearLabel = InspectionConfiguration.BackRight == 1 || InspectionConfiguration.BackLeft == 1 || InspectionConfiguration.IsBack == 1;
                FrontLabel = InspectionConfiguration.FrontLeft == 1 || InspectionConfiguration.FrontRight == 1 || InspectionConfiguration.IsFront == 1;
                PlaneOptions = InspectionConfiguration.FrontLeft == 0 &&
                              InspectionConfiguration.FrontRight == 0 &&
                             InspectionConfiguration.BackLeft == 0 &&
                            InspectionConfiguration.BackRight == 0 &&
                            InspectionConfiguration.IsBack == 0 &&
                            InspectionConfiguration.IsFront == 0;
                LeftLabel = InspectionConfiguration.FrontLeft == 1 || InspectionConfiguration.BackLeft == 1;
                RightLabel = InspectionConfiguration.FrontRight == 1 || InspectionConfiguration.BackRight == 1;
                var answer = inspectionResultsList?.Find(x => x.QID == InspectionConfiguration.MInspectionConfigID);

                if (answer != null)
                {
                    ImagesCount = answer.PhotoCount;
                    Remarks = answer.Remarks;
                    if (answer.FrontLeft == 2)
                    {
                        FrontLeftTrue = true;
                        FrontLeftFalse = false;
                    }

                    if (FrontLeft && answer.FrontLeft == 1)
                    {
                        FrontLeftTrue = false;
                        FrontLeftFalse = true;
                    }

                    if (answer.FrontRight == 2)
                    {
                        FrontRightTrue = true;
                        FrontRightFalse = false;
                    }

                    if (FrontRight && answer.FrontRight == 1)
                    {
                        FrontRightTrue = false;
                        FrontRightFalse = true;
                    }

                    if (answer.BackLeft == 2)
                    {
                        RearLeftTrue = true;
                        RearLeftFalse = false;
                    }

                    if (RearLeft && answer.BackLeft == 1)
                    {
                        RearLeftTrue = false;
                        RearLeftFalse = true;
                    }

                    if (answer.BackRight == 2)
                    {
                        RearRightTrue = true;
                        RearRightFalse = false;
                    }

                    if (RearRight && answer.BackRight == 1)
                    {
                        RearRightTrue = false;
                        RearRightFalse = true;
                    }

                    if (answer.Direct == 2)
                    {
                        PlaneTrue = true;
                        PlaneFalse = false;
                    }

                    if (PlaneOptions && answer.Direct == 1)
                    {
                        PlaneTrue = false;
                        PlaneFalse = true;
                    }
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ShowConfigurationOptions method -> in VinInspectionAnswersPageViewModel.cs " + Settings.userLoginID);
            }
            finally
            {
                AnswersGridVisibility = true;
                loadindicator = false;
            }
        }

        #region Properties

        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields TagNumber { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "TagNumber"
            };
            public DashboardLabelFields IdentCode { get; set; } = new DashboardLabelFields { Status = false, Name = "IdentCode" };
            public DashboardLabelFields ConditionName { get; set; } = new DashboardLabelFields { Status = false, Name = "ConditionName" };

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

        private bool _IsAnswersVisible = true;
        public bool IsAnswersVisible
        {
            get { return _IsAnswersVisible; }
            set
            {
                _IsAnswersVisible = value;
                RaisePropertyChanged("IsAnswersVisible");
            }
        }

        private double _NextBtnOpacity = 1.0;
        public double NextBtnOpacity
        {
            get => _NextBtnOpacity;
            set
            {
                _NextBtnOpacity = value;
                NotifyPropertyChanged("NextBtnOpacity");
            }
        }

        private bool _IsNextBtnEnable = true;
        public bool IsNextBtnEnable
        {
            get => _IsNextBtnEnable;
            set
            {
                _IsNextBtnEnable = value;
                NotifyPropertyChanged("IsNextBtnEnable");
            }
        }

        private bool _IsQuickTabVisible = false;
        public bool IsQuickTabVisible
        {
            get => _IsQuickTabVisible;
            set
            {
                _IsQuickTabVisible = value;
                NotifyPropertyChanged("IsQuickTabVisible");
            }
        }

        private bool _IsFullTabVisible = false;
        public bool IsFullTabVisible
        {
            get => _IsFullTabVisible;
            set
            {
                _IsFullTabVisible = value;
                NotifyPropertyChanged("IsFullTabVisible");
            }
        }

        private bool _IsInspTabVisible = false;
        public bool IsInspTabVisible
        {
            get => _IsInspTabVisible;
            set
            {
                _IsInspTabVisible = value;
                NotifyPropertyChanged("IsInspTabVisible");
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

        private string _NextButtonText = "NEXT";
        public string NextButtonText
        {
            get => _NextButtonText;
            set
            {
                _NextButtonText = value;
                RaisePropertyChanged("NextButtonText");
            }
        }

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

        private InspectionConfiguration _InspectionConfiguration;
        public InspectionConfiguration InspectionConfiguration
        {
            get => _InspectionConfiguration;
            set
            {
                _InspectionConfiguration = value;
                RaisePropertyChanged("InspectionConfiguration");
            }
        }

        private bool _RearLeft;
        public bool RearLeft
        {
            get => _RearLeft;
            set
            {
                _RearLeft = value;
                RaisePropertyChanged("RearLeft");
            }
        }

        private bool _RearRight;
        public bool RearRight
        {
            get => _RearRight;
            set
            {
                _RearRight = value;
                RaisePropertyChanged("RearRight");
            }
        }

        private bool _PlaneOptions;
        public bool PlaneOptions
        {
            get => _PlaneOptions;
            set
            {
                _PlaneOptions = value;
                RaisePropertyChanged("PlaneOptions");
            }
        }
        private bool _FrontRight;
        public bool FrontRight
        {
            get => _FrontRight;
            set
            {
                _FrontRight = value;
                RaisePropertyChanged("FrontRight");
            }
        }

        private bool _FrontLeft;
        public bool FrontLeft
        {
            get => _FrontLeft;
            set
            {
                _FrontLeft = value;
                RaisePropertyChanged("FrontLeft");
            }
        }

        private bool _FrontLabel;
        public bool FrontLabel
        {
            get => _FrontLabel;
            set
            {
                _FrontLabel = value;
                RaisePropertyChanged("FrontLabel");
            }
        }

        private bool _RearLabel;
        public bool RearLabel
        {
            get => _RearLabel;
            set
            {
                _RearLabel = value;
                RaisePropertyChanged("RearLabel");
            }
        }

        private bool _LeftLabel;
        public bool LeftLabel
        {
            get => _LeftLabel;
            set
            {
                _LeftLabel = value;
                RaisePropertyChanged("LeftLabel");
            }
        }

        private bool _RightLabel;
        public bool RightLabel
        {
            get => _RightLabel;
            set
            {
                _RightLabel = value;
                RaisePropertyChanged("RightLabel");
            }
        }

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator;
            set
            {
                _loadindicator = value;
                RaisePropertyChanged("loadindicator");
            }
        }

        private bool _AnswersGridVisibility = true;
        public bool AnswersGridVisibility
        {
            get => _AnswersGridVisibility;
            set
            {
                _AnswersGridVisibility = value;
                RaisePropertyChanged("loadindicator");
            }
        }

        private string _Remarks;
        public string Remarks
        {
            get => _Remarks;
            set
            {
                _Remarks = value;
                RaisePropertyChanged("Remarks");
            }
        }

        private bool _FrontLeftTrue;
        public bool FrontLeftTrue
        {
            get => _FrontLeftTrue;
            set
            {
                _FrontLeftTrue = value;
                RaisePropertyChanged("FrontLeftTrue");
            }
        }

        private bool _FrontLeftFalse;
        public bool FrontLeftFalse
        {
            get => _FrontLeftFalse;
            set
            {
                _FrontLeftFalse = value;
                RaisePropertyChanged("FrontLeftFalse");
            }
        }

        private bool _FrontRightTrue;
        public bool FrontRightTrue
        {
            get => _FrontRightTrue;
            set
            {
                _FrontRightTrue = value;
                RaisePropertyChanged("FrontRightTrue");
            }
        }

        private bool _FrontRightFalse;
        public bool FrontRightFalse
        {
            get => _FrontRightFalse;
            set
            {
                _FrontRightFalse = value;
                RaisePropertyChanged("FrontRightFalse");
            }
        }

        private bool _RearLeftTrue;
        public bool RearLeftTrue
        {
            get => _RearLeftTrue;
            set
            {
                _RearLeftTrue = value;
                RaisePropertyChanged("RearLeftTrue");
            }
        }

        private bool _RearLeftFalse;
        public bool RearLeftFalse
        {
            get => _RearLeftFalse;
            set
            {
                _RearLeftFalse = value;
                RaisePropertyChanged("RearLeftFalse");
            }
        }

        private bool _RearRightTrue;
        public bool RearRightTrue
        {
            get => _RearRightTrue;
            set
            {
                _RearRightTrue = value;
                RaisePropertyChanged("RearRightTrue");
            }
        }

        private bool _RearRightFalse;
        public bool RearRightFalse
        {
            get => _RearRightFalse;
            set
            {
                _RearRightFalse = value;
                RaisePropertyChanged("RearRightFalse");
            }
        }

        private bool _PlaneTrue;
        public bool PlaneTrue
        {
            get => _PlaneTrue;
            set
            {
                _PlaneTrue = value;
                RaisePropertyChanged("PlaneTrue");
            }
        }

        private bool _PlaneFalse;
        public bool PlaneFalse
        {
            get => _PlaneFalse;
            set
            {
                _PlaneFalse = value;
                RaisePropertyChanged("PlaneFalse");
            }
        }

        private int _ImagesCount;
        public int ImagesCount
        {
            get => _ImagesCount;
            set
            {
                _ImagesCount = value;
                RaisePropertyChanged("ImagesCount");
            }
        }

        private ImageSource _ImageViewForUpload;
        public ImageSource ImageViewForUpload
        {
            get { return _ImageViewForUpload; }
            set
            {
                _ImageViewForUpload = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
    }
}
