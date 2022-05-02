using System;
using System.Collections.Generic;
using System.Text;
using YPS.Service;
using YPS.Parts2y.Parts2y_Views;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Model;
using YPS.Helpers;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Windows.Input;
using System.Threading.Tasks;
using YPS.CommonClasses;
using System.IO;
using YPS.CustomToastMsg;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class EInspectionAnswersViewModel : IBase
    {
        #region IComman and data members declaration
        public INavigation Navigation { get; set; }
        public ICommand ViewallClick { get; set; }
        public ICommand NextClick { get; set; }
        public ICommand PhotoClickCommand { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand LoadInspTabCmd { set; get; }
        public ICommand SignalTabCmd { set; get; }
        public ICommand QuickTabCmd { set; get; }
        public ICommand FullTabCmd { set; get; }
        public ICommand SaveClick { set; get; }

        EInspectionAnswersPage pagename;
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
        ELoadInspectionQuestionsViewModel LoadQueVm;
        EPartsInspectionQuestionsViewModel PartsQueVm;
        string pendingTagIDs;
        #endregion

        public EInspectionAnswersViewModel(INavigation _Navigation, EInspectionAnswersPage page,
            InspectionConfiguration inspectionConfiguration, ObservableCollection<InspectionConfiguration> inspectionConfigurationList,
            List<InspectionResultsList> inspectionResultsList, AllPoData selectedtagdata, bool isVINInsp,
            EPartsInspectionQuestionsViewModel partsqueVm, ELoadInspectionQuestionsViewModel loadqueVm, bool isalldone = false,
             string pendingtagIDs = null)
        {
            try
            {
                Navigation = _Navigation;
                LoadQueVm = loadqueVm;
                PartsQueVm = partsqueVm;
                trackService = new YPSService();
                finalPhotoListA = new ObservableCollection<InspectionPhotosResponseListData>();
                pagename = page;
                isInspVIN = isVINInsp;
                isAllDone = isalldone;
                selectedTagData = selectedtagdata;
                pendingTagIDs = pendingtagIDs;
                this.tagId = selectedtagdata.POTagID;
                taskid = selectedtagdata.TaskID;
                PONumber = selectedtagdata.PONumberForDisplay;
                ShippingNumber = isVINInsp == true ? selectedtagdata.ShippingNumber : selectedtagdata.ShippingNumberForDisplay;
                REQNo = selectedtagdata.REQNoForDisplay;
                TaskName = selectedtagdata.TaskName;
                Barcode1 = selectedTagData.Barcode1;
                BagNumber = selectedTagData.BagNumber;
                EventName = selectedtagdata.EventName;
                TagNumber = selectedtagdata.TagNumber;
                IndentCode = selectedtagdata.IdentCode;
                IsConditionNameLabelVisible = string.IsNullOrEmpty(selectedtagdata.ConditionName) ? false : true;
                ConditionName = selectedtagdata.ConditionName;
                InspectionConfiguration = inspectionConfiguration;
                this.inspectionConfigurationList = inspectionConfigurationList;
                this.inspectionResultsList = inspectionResultsList;
                ShowConfigurationOptions();

                SaveClick = new Command(SaveClicked);
                ViewallClick = new Command(ViewallClickMethod);
                NextClick = new Command(NextClickMethod);
                QuickTabCmd = new Command(QuickTabClicked);
                FullTabCmd = new Command(FullTabClicked);
                LoadInspTabCmd = new Command(LoadInspTabClicked);
                SignalTabCmd = new Command(SignTabClicked);
                PhotoClickCommand = new Command(async () => await SelectPic());
                Backevnttapped = new Command(async () => await Backevnttapped_click());

                ChangeLabel();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EInspectionAnswersViewModel constructor -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async void SaveClicked()
        {
            try
            {
                loadindicator = true;
                if (FrontRightTrue || FrontRightFalse || FrontLeftTrue || FrontLeftFalse || RearLeftTrue
                    || RearLeftFalse || RearRightTrue || RearRightFalse || PlaneTrue || PlaneFalse || PlaneNA)
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

                        if (PlaneTrue == true || PlaneFalse == true || PlaneNA)
                        {
                            updateInspectionRequest.Direct = PlaneTrue ? 2 : PlaneFalse ? 1 : 3;
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

                        updateInspectionRequest.PalletNo_L2 = InspectionConfiguration?.InspectionResult?.PalletNo_L2;
                        updateInspectionRequest.ExpiryDate = string.IsNullOrEmpty(InspectionConfiguration?.InspectionResult?.ExpiryDate) ? "" : InspectionConfiguration?.InspectionResult?.ExpiryDate;
                        updateInspectionRequest.Attributes = InspectionConfiguration?.InspectionResult?.Attributes;
                        updateInspectionRequest.InnerQty = InspectionConfiguration?.InspectionResult?.InnerQty;
                        var result = await trackService.InsertUpdateInspectionResult(updateInspectionRequest);

                        if (result != null && result.status == 1)
                        {
                            loadindicator = false;
                            await App.Current.MainPage.DisplayAlert("Updated", "Changed Saved .", "Ok");
                            //DependencyService.Get<IToastMessage>().ShortAlert("Updated", "Changed Saved .", "Ok");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveClicked method -> in CInspectionAnswersPageViewModel " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
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
                PartsQueVm.QuickTabVisibility = true;
                PartsQueVm.FullTabVisibility = false;
                PartsQueVm.SignTabVisibility = false;

                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QuickTabClicked method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
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
                PartsQueVm.QuickTabVisibility = false;
                PartsQueVm.FullTabVisibility = true;
                PartsQueVm.SignTabVisibility = false;
                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FullTabClicked method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async void LoadInspTabClicked()
        {
            try
            {
                loadindicator = true;
                LoadQueVm.LoadInspTabVisibility = true;
                LoadQueVm.SignTabVisibility = false;
                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LoadInspTabClicked method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
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

                if (LoadQueVm != null)
                {
                    LoadQueVm.LoadInspTabVisibility = false;
                    LoadQueVm.SignTabVisibility = true;
                }

                if (PartsQueVm != null)
                {
                    PartsQueVm.QuickTabVisibility = false;
                    PartsQueVm.FullTabVisibility = false;
                    PartsQueVm.SignTabVisibility = true;
                }

                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SignTabClicked method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
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
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetInspectionPhotos method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task ChangeLabel()
        {
            try
            {
                labelobj = new DashboardLabelChangeClass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var checklist = labelval.Where(wr => wr.FieldID == labelobj.Checklist.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var remark = labelval.Where(wr => wr.FieldID == labelobj.Remarks.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var viewall = labelval.Where(wr => wr.FieldID == labelobj.ViewAll.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var next = labelval.Where(wr => wr.FieldID == labelobj.Next.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var complete = labelval.Where(wr => wr.FieldID == labelobj.Complete.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var save = labelval.Where(wr => wr.FieldID == labelobj.Save.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var palletno = labelval.Where(wr => wr.FieldID == labelobj.PalletNo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var expirydate = labelval.Where(wr => wr.FieldID == labelobj.ExpiryDate.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var attributes = labelval.Where(wr => wr.FieldID == labelobj.Attributes.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var innerqty = labelval.Where(wr => wr.FieldID == labelobj.InnerQty.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        SignTabText = labelobj.Checklist.Name = checklist != null ? (!string.IsNullOrEmpty(checklist.LblText) ? checklist.LblText : labelobj.Checklist.Name) : labelobj.Checklist.Name;
                        labelobj.Remarks.Name = remark != null ? (!string.IsNullOrEmpty(remark.LblText) ? remark.LblText : labelobj.Remarks.Name) : labelobj.Remarks.Name;
                        labelobj.ViewAll.Name = viewall != null ? (!string.IsNullOrEmpty(viewall.LblText) ? viewall.LblText : labelobj.ViewAll.Name) : labelobj.ViewAll.Name;
                        NextButtonText = labelobj.Next.Name = next != null ? (!string.IsNullOrEmpty(next.LblText) ? next.LblText : labelobj.Next.Name) : labelobj.Next.Name;
                        labelobj.Complete.Name = complete != null ? (!string.IsNullOrEmpty(complete.LblText) ? complete.LblText : labelobj.Complete.Name) : labelobj.Complete.Name;
                        labelobj.Save.Name = save != null ? (!string.IsNullOrEmpty(save.LblText) ? save.LblText : labelobj.Save.Name) : labelobj.Save.Name;
                        labelobj.PalletNo.Name = palletno != null ? (!string.IsNullOrEmpty(palletno.LblText) ? palletno.LblText : labelobj.PalletNo.Name) : labelobj.PalletNo.Name;
                        labelobj.ExpiryDate.Name = expirydate != null ? (!string.IsNullOrEmpty(expirydate.LblText) ? expirydate.LblText : labelobj.ExpiryDate.Name) : labelobj.ExpiryDate.Name;
                        labelobj.Attributes.Name = attributes != null ? (!string.IsNullOrEmpty(attributes.LblText) ? attributes.LblText : labelobj.Attributes.Name) : labelobj.Attributes.Name;
                        labelobj.InnerQty.Name = innerqty != null ? (!string.IsNullOrEmpty(innerqty.LblText) ? innerqty.LblText : labelobj.InnerQty.Name) : labelobj.InnerQty.Name;

                        //Assigning the Labels & Show/Hide the controls based on the data
                        if (isInspVIN == true)
                        {
                            var tagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var shippingnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.ShippingNumberForParts.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var barcode1 = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Barcode1.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var bagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.BagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var quick = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Quick.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var full = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Full.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                            labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                            labelobj.TagNumber.Status = tagnumber?.Status == 1 || tagnumber?.Status == 2 ? true : false;
                            labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                            labelobj.EventName.Status = eventname?.Status == 1 || eventname?.Status == 2 ? true : false;
                            labelobj.ShippingNumberForParts.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumberForParts.Name) : labelobj.ShippingNumberForParts.Name) + " :";
                            labelobj.ShippingNumberForParts.Status = shippingnumber?.Status == 1 || shippingnumber?.Status == 2 ? true : false;
                            labelobj.Barcode1.Name = (barcode1 != null ? (!string.IsNullOrEmpty(barcode1.LblText) ? barcode1.LblText : labelobj.Barcode1.Name) : labelobj.Barcode1.Name) + " :";
                            labelobj.Barcode1.Status = barcode1?.Status == 1 || barcode1?.Status == 2 ? true : false;
                            labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                            labelobj.BagNumber.Status = bagnumber?.Status == 1 || bagnumber?.Status == 2 ? true : false;
                            labelobj.Quick.Name = quick != null ? (!string.IsNullOrEmpty(quick.LblText) ? quick.LblText : labelobj.Quick.Name) : labelobj.Quick.Name;
                            labelobj.Full.Name = full != null ? (!string.IsNullOrEmpty(full.LblText) ? full.LblText : labelobj.Full.Name) : labelobj.Full.Name;
                            labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                            labelobj.TaskName.Status = taskanme == null ? true : (taskanme.Status == 1 || taskanme.Status == 2 ? true : false);
                            IsResourcecVisible = false;

                            IsQuickTabVisible = PartsQueVm.IsQuickTabVisible;
                            IsFullTabVisible = PartsQueVm.IsFullTabVisible;

                            if (IsQuickTabVisible == false && IsFullTabVisible == false)
                            {
                                SignTabClicked();
                            }
                            else if (IsQuickTabVisible == true && IsFullTabVisible == true)
                            {
                                if (InspectionConfiguration.CategoryID == 10)
                                {
                                    QuickTabClicked();
                                }
                                else if (InspectionConfiguration.CategoryID == 11)
                                {
                                    FullTabClicked();
                                }
                            }
                            else if (IsQuickTabVisible == true &&
                                InspectionConfiguration.CategoryID == 10)
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
                            var poid = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.POID.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var shippingnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.ShippingNumber.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var reqnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.REQNo.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                            var loadinsp = labelval.Where(wr => wr.FieldID == labelobj.LoadInsp.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                            labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                            labelobj.POID.Status = poid == null ? true : (poid.Status == 1 || poid.Status == 2 ? true : false);
                            labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                            labelobj.ShippingNumber.Status = shippingnumber == null ? true : (shippingnumber.Status == 1 || shippingnumber.Status == 2 ? true : false);
                            labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                            labelobj.REQNo.Status = reqnumber == null ? true : (reqnumber.Status == 1 || reqnumber.Status == 2 ? true : false);
                            labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                            labelobj.TaskName.Status = taskanme == null ? true : (taskanme.Status == 1 || taskanme.Status == 2 ? true : false);
                            labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                            labelobj.EventName.Status = eventname == null ? true : (eventname.Status == 1 || eventname.Status == 2 ? true : false);
                            labelobj.LoadInsp.Name = loadinsp != null ? (!string.IsNullOrEmpty(loadinsp.LblText) ? loadinsp.LblText : labelobj.LoadInsp.Name) : labelobj.LoadInsp.Name;

                            LoadInspTabClicked();
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
                YPSLogger.ReportException(ex, "ChangeLabel method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
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
                await Navigation.PushAsync(new InspectionPhotosPage(this.tagId, InspectionConfiguration, TagNumber, selectedTagData,
                    IsInspTabVisible == true ? true : false, isInspVIN, pendingTagIDs), false);
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "SelectPic method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
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

                if (NextButtonText == labelobj.Complete.Name)
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

                        if (PlaneTrue == true || PlaneFalse == true || PlaneNA)
                        {
                            updateInspectionRequest.Direct = PlaneTrue ? 2 : PlaneFalse ? 1 : 3;
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

                        updateInspectionRequest.PalletNo_L2 = InspectionConfiguration?.InspectionResult?.PalletNo_L2;
                        updateInspectionRequest.ExpiryDate = string.IsNullOrEmpty(InspectionConfiguration?.InspectionResult?.ExpiryDate) ? "" : InspectionConfiguration?.InspectionResult?.ExpiryDate;
                        updateInspectionRequest.Attributes = InspectionConfiguration?.InspectionResult?.Attributes;
                        updateInspectionRequest.InnerQty = InspectionConfiguration?.InspectionResult?.InnerQty;
                        var result = await trackService.InsertUpdateInspectionResult(updateInspectionRequest);

                        if (result != null && result.status == 1)
                        {
                            if (InspectionConfiguration.SerialNo == inspectionConfigurationList.Count)
                            {
                                NextButtonText = labelobj.Complete.Name;
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
                                PlaneNA = false;
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
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
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
                YPSLogger.ReportException(ex, "NextClickMethod method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
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
                SignTabClicked();
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ViewallClickMethod method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
            }
        }

        private async Task Backevnttapped_click()
        {
            try
            {
                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Backevnttapped_click method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
            }
        }

        private async Task DoneClicked()
        {
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (selectedTagData.TaskID != 0 && ((isInspVIN == false && !string.IsNullOrEmpty(pendingTagIDs)) ||
                        (isInspVIN == true && selectedTagData.TagTaskStatus == 0)))
                    {
                        TagTaskStatus tagtaskstatus = new TagTaskStatus();
                        tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                        tagtaskstatus.POTagID = !string.IsNullOrEmpty(pendingTagIDs) ? string.Join(",", pendingTagIDs) : Helperclass.Encrypt(selectedTagData.POTagID.ToString());
                        tagtaskstatus.Status = 1;
                        tagtaskstatus.CreatedBy = Settings.userLoginID;

                        var result = await trackService.UpdateTagTaskStatus(tagtaskstatus);

                        if (result?.status == 1)
                        {
                            if (selectedTagData.TaskStatus == 0)
                            {
                                TagTaskStatus taskstatus = new TagTaskStatus();
                                taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                                taskstatus.TaskStatus = 1;
                                taskstatus.CreatedBy = Settings.userLoginID;

                                var taskval = await trackService.UpdateTaskStatus(taskstatus);
                            }
                            selectedTagData.TaskStatus = 1;
                            selectedTagData.TagTaskStatus = 1;
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
                YPSLogger.ReportException(ex, "DoneClicked method -> in EInspectionAnswersViewModel.cs  " + Settings.userLoginID);
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
                InspectionConfiguration.InspectionResult = inspectionResultsList?.Where(x => x.QID == InspectionConfiguration.MInspectionConfigID).FirstOrDefault();


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
                        PlaneNA = false;
                    }

                    if (PlaneOptions && answer.Direct == 1)
                    {
                        PlaneTrue = false;
                        PlaneFalse = true;
                        PlaneNA = false;
                    }
                    if (PlaneOptions && answer.Direct == 3)
                    {
                        PlaneTrue = false;
                        PlaneFalse = false;
                        PlaneNA = true;
                    }
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ShowConfigurationOptions method -> in EInspectionAnswersViewModel.cs " + Settings.userLoginID);
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
                Name = "Shipping Number"
            };
            public DashboardLabelFields ShippingNumberForParts { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "Shipping Number"
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
            public DashboardLabelFields Barcode1 { get; set; } = new DashboardLabelFields { Status = false, Name = "Barcode1" };
            public DashboardLabelFields BagNumber { get; set; } = new DashboardLabelFields { Status = false, Name = "BagNumber" };

            public DashboardLabelFields Quick { get; set; } = new DashboardLabelFields { Status = false, Name = "TBMQuick" };
            public DashboardLabelFields Full { get; set; } = new DashboardLabelFields { Status = false, Name = "TBMFull" };
            public DashboardLabelFields LoadInsp { get; set; } = new DashboardLabelFields { Status = false, Name = "TBMCarrierInsp" };
            public DashboardLabelFields Checklist { get; set; } = new DashboardLabelFields { Status = false, Name = "TBMChecklist" };
            public DashboardLabelFields Remarks { get; set; } = new DashboardLabelFields { Status = false, Name = "LCMRemarks" };
            public DashboardLabelFields ViewAll { get; set; } = new DashboardLabelFields { Status = false, Name = "LCMbtnViewAll" };
            public DashboardLabelFields Next { get; set; } = new DashboardLabelFields { Status = false, Name = "LCMbtnNext" };
            public DashboardLabelFields Complete { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMbtnComplete" };
            public DashboardLabelFields Save { get; set; } = new DashboardLabelFields { Status = true, Name = Settings.SaveBtn };

            public DashboardLabelFields PalletNo { get; set; } = new DashboardLabelFields { Status = false, Name = "PalletNo_L2" };
            public DashboardLabelFields ExpiryDate { get; set; } = new DashboardLabelFields { Status = false, Name = "ExpiryDate" };
            public DashboardLabelFields Attributes { get; set; } = new DashboardLabelFields { Status = false, Name = "Attributes" };
            public DashboardLabelFields InnerQty { get; set; } = new DashboardLabelFields { Status = false, Name = "InnerQty" };

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

        private string _SignTabText;
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

        private string _Barcode1;
        public string Barcode1
        {
            get { return _Barcode1; }
            set
            {
                _Barcode1 = value;
                RaisePropertyChanged("Barcode1");
            }
        }

        private string _BagNumber;
        public string BagNumber
        {
            get { return _BagNumber; }
            set
            {
                _BagNumber = value;
                RaisePropertyChanged("BagNumber");
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
        private bool planeNA;
        public bool PlaneNA
        {
            get => planeNA;
            set
            {
                planeNA = value;
                RaisePropertyChanged("PlaneNA");
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
