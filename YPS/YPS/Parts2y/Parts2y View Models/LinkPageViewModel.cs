using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static YPS.Model.SearchModel;
using Syncfusion.Buttons;
using Syncfusion.XForms.Buttons;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class LinkPageViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        public int taskID { get; set; }
        public bool updateList { get; set; }
        public ICommand LinkBeforePackingPhotoCmd { set; get; }
        public ICommand LinkAfterPackingPhotoCmd { set; get; }
        public ICommand viewExistingBUPhotos { get; set; }
        public ICommand viewExistingAUPhotos { get; set; }
        public Command SelectTagItemCmd { set; get; }

        YPSService service;
        LinkPage pagename;
        SendPodata sendPodata;
        ObservableCollection<AllPoData> AllPoDataList;

        public LinkPageViewModel(INavigation navigation, ObservableCollection<PhotoRepoDBModel> repophotosist, LinkPage page)
        {
            try
            {
                Title = Settings.VersionID == 2 ? "Link VIN" : "Link Parts";
                service = new YPSService();
                Navigation = navigation;
                pagename = page;
                RepoPhotosList = repophotosist;
                viewExistingBUPhotos = new Command(tap_eachCamB);
                viewExistingAUPhotos = new Command(tap_eachCamA);
                LinkBeforePackingPhotoCmd = new Command(LinkPhotoToTag);
                LinkAfterPackingPhotoCmd = new Command(LinkPhotoToTag);
                SelectTagItemCmd = new Command(TagLongPessed);

                DynamicTextChange();
                ShowContentsToLink();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LinkPageViewModel constructor -> in LinkPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        private void TagLongPessed(object sender)
        {
            try
            {
                var item = sender as AllPoData;

                if (item.IsChecked == true)
                {
                    item.IsChecked = false;
                    item.SelectedTagBorderColor = Color.Transparent;
                }
                else
                {
                    item.IsChecked = true;
                    item.SelectedTagBorderColor = Settings.Bar_Background;
                }

                SelectedTagCountVisible = (SelectedTagCount = AllPoTagCollections.Where(wr => wr.IsChecked == true).ToList().Count()) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TagLongPessed method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                var trackResult = service.Handleexception(ex);
            }
        }

        private async void tap_eachCamB(object obj)
        {
            try
            {
                YPSLogger.TrackEvent("LinkPageViewModel.cs", " in tap_eachCamB method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var potag = obj as AllPoData;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (potag.imgCamOpacityB != 0.5)
                    {
                        IndicatorVisibility = true;

                        try
                        {
                            updateList = true;
                            Settings.POID = potag.POID;
                            Settings.TaskID = potag.TaskID;
                            Settings.CanOpenScanner = true;
                            Settings.currentPuId = potag.PUID;
                            Settings.BphotoCount = potag.TagBPhotoCount;
                            bool isalldone = AllPoDataList?.Where(wr => wr.TaskID == potag?.TaskID && wr.TagTaskStatus != 2).FirstOrDefault() == null ? true : false;

                            if (potag.PUID == 0)
                            {
                                PhotoUploadModel selectedTagsData = new PhotoUploadModel();
                                selectedTagsData.POID = potag.POID;
                                selectedTagsData.isCompleted = potag.photoTickVisible;

                                List<PhotoTag> lstdat = new List<PhotoTag>();

                                if (potag.TagAPhotoCount == 0 && potag.TagBPhotoCount == 0 && potag.PUID == 0)
                                {
                                    PhotoTag tg = new PhotoTag();

                                    if (potag.POTagID != 0)
                                    {
                                        tg.POTagID = potag.POTagID;
                                        tg.TaskID = potag.TaskID;
                                        tg.TaskStatus = potag.TaskStatus;
                                        tg.TagTaskStatus = potag.TagTaskStatus;
                                        tg.TagNumber = potag.TagNumber;
                                        tg.IdentCode = potag.IdentCode;
                                        Settings.Tagnumbers = potag.TagNumber;
                                        lstdat.Add(tg);
                                    }
                                }
                                else
                                {
                                    selectedTagsData.alreadyExit = "alreadyExit";
                                }

                                selectedTagsData.photoTags = lstdat;
                                Settings.currentPoTagId_Inti = lstdat;

                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(selectedTagsData, null, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false, isalldone, false), false);
                            }
                            else
                            {
                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, potag, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, potag.photoTickVisible, isalldone, false), false);
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamB method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                            var trackResult = await service.Handleexception(ex);
                        }
                        IndicatorVisibility = false;
                    }
                }
                else
                {
                    IndicatorVisibility = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_eachCamB method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// This method is called when clicked on camera icon form data grid, to view After Packing photo(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachCamA(object obj)
        {
            try
            {
                IndicatorVisibility = true;

                YPSLogger.TrackEvent("LinkPageViewModel.cs", " in tap_eachCamA method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var potag = obj as AllPoData;

                if (potag.imgCamOpacityA != 0.5)
                {
                    IndicatorVisibility = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {

                        try
                        {
                            updateList = true;
                            Settings.POID = potag.POID;
                            Settings.TaskID = potag.TaskID;
                            Settings.CanOpenScanner = true;
                            Settings.AphotoCount = potag.TagAPhotoCount;
                            Settings.currentPuId = potag.PUID;
                            bool isalldone = AllPoDataList?.Where(wr => wr.TaskID == potag?.TaskID && wr.TagTaskStatus != 2).FirstOrDefault() == null ? true : false;

                            if (potag.PUID == 0)
                            {
                                PhotoUploadModel selectedTagsData = new PhotoUploadModel();
                                selectedTagsData.POID = potag.POID;
                                selectedTagsData.isCompleted = potag.photoTickVisible;

                                List<PhotoTag> lstdat = new List<PhotoTag>();

                                if (potag.TagAPhotoCount == 0 && potag.TagBPhotoCount == 0 && potag.PUID == 0)
                                {
                                    PhotoTag tg = new PhotoTag();

                                    if (potag.POTagID != 0)
                                    {
                                        tg.POTagID = potag.POTagID;
                                        tg.TaskID = potag.TaskID;
                                        tg.TaskStatus = potag.TaskStatus;
                                        tg.TagTaskStatus = potag.TagTaskStatus;
                                        tg.TagNumber = potag.TagNumber;
                                        tg.IdentCode = potag.IdentCode;
                                        Settings.Tagnumbers = potag.TagNumber;
                                        lstdat.Add(tg);
                                    }
                                }
                                else
                                {
                                    selectedTagsData.alreadyExit = "alreadyExit";
                                }

                                selectedTagsData.photoTags = lstdat;
                                Settings.currentPoTagId_Inti = lstdat;

                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(selectedTagsData, null, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, false, isalldone, false), false);
                            }
                            else
                            {
                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, potag, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, potag.photoTickVisible, isalldone, false), false);
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamA method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                            var trackResult = await service.Handleexception(ex);
                            IndicatorVisibility = false;
                        }
                        IndicatorVisibility = false;
                    }
                    else
                    {
                        IndicatorVisibility = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_eachCamA method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void LinkPhotoToTag(object sender)
        {
            YPSLogger.TrackEvent("LinkPageViewModel.cs", " in LinkPhotoToTag method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (PackingButtonOpacity == 1.0)
                    {

                        var sfbutton = sender as SfButton;

                        if (AllPoTagCollections != null && AllPoTagCollections.Count > 0)
                        {
                            var valuewithPUID = AllPoTagCollections.Where(wr => wr.IsChecked == true && wr.PUID != 0).GroupBy(g => g.PUID).ToList();
                            var valuewithnoPUID = AllPoTagCollections.Where(wr => wr.IsChecked == true && wr.PUID == 0).ToList();
                            var styleid = sfbutton.StyleId;

                            if (valuewithPUID.Count > 0 || valuewithnoPUID.Count > 0)
                            {
                                bool move = await App.Current.MainPage.DisplayAlert("Photo(s) linking", "Are you sure you want to link the photo(s) to the selected"
                                    + (Settings.VersionID == 2 ? " VIN(s)" : " Part(s)"), "Yes", "No");

                                if (move)
                                {
                                    updateList = true;

                                    var firstphotovalue = RepoPhotosList;

                                    //var checkInternet = await App.CheckInterNetConnection();

                                    //if (checkInternet)
                                    //{
                                    if (valuewithPUID.Count > 0)
                                    {
                                        foreach (var eachpuid in valuewithPUID)
                                        {
                                            PhotoUploadModel selectedTagsData = new PhotoUploadModel();

                                            taskID = eachpuid.FirstOrDefault().TaskID;
                                            selectedTagsData.POID = eachpuid.FirstOrDefault().POID;
                                            selectedTagsData.isCompleted = eachpuid.FirstOrDefault().photoTickVisible;

                                            List<PhotoTag> lstdat = new List<PhotoTag>();


                                            selectedTagsData.alreadyExit = "alreadyExit";

                                            if (eachpuid.FirstOrDefault().imgCamOpacityB != 0.5 && eachpuid.FirstOrDefault().IsPhotoRequired != 0)
                                            {
                                                if (eachpuid.FirstOrDefault().PUID != 0)
                                                {
                                                    List<CustomPhotoModel> phUploadList = new List<CustomPhotoModel>();

                                                    foreach (var photo in firstphotovalue)
                                                    {
                                                        CustomPhotoModel phUpload = new CustomPhotoModel();
                                                        phUpload.PUID = eachpuid.FirstOrDefault().PUID;
                                                        phUpload.PhotoID = 0;
                                                        phUpload.PhotoURL = photo.FileUrl;
                                                        phUpload.PhotoDescription = photo.FileDescription;
                                                        phUpload.FileName = photo.FileName;
                                                        phUpload.CreatedBy = Settings.userLoginID;
                                                        phUpload.UploadType = styleid.Trim() == "a".Trim() ? (int)UploadTypeEnums.GoodsPhotos_AP : (int)UploadTypeEnums.GoodsPhotos_BP;// uploadType;
                                                        phUpload.CreatedDate = String.Format(Settings.DateFormat, DateTime.Now);
                                                        phUpload.FullName = Settings.Username;
                                                        phUploadList.Add(phUpload);
                                                    }

                                                    var data = await service.PhotosUpload(phUploadList);

                                                    var initialresult = data as SecondTimeResponse;

                                                    if (initialresult != null && initialresult.status == 1)
                                                    {
                                                        foreach (var val in initialresult.data)
                                                        {
                                                            eachpuid.FirstOrDefault().PUID = val.PUID;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (valuewithnoPUID.Count > 0)
                                    {
                                        foreach (var parts in valuewithnoPUID)
                                        {
                                            PhotoUploadModel selectedTagsData = new PhotoUploadModel();
                                            taskID = parts.TaskID;
                                            selectedTagsData.POID = parts.POID;
                                            selectedTagsData.isCompleted = parts.photoTickVisible;

                                            List<PhotoTag> lstdat = new List<PhotoTag>();

                                            if (parts.TagAPhotoCount == 0 && parts.TagBPhotoCount == 0 && parts.PUID == 0)
                                            {
                                                PhotoTag tg = new PhotoTag();

                                                if (parts.POTagID != 0)
                                                {
                                                    tg.POTagID = parts.POTagID;
                                                    Settings.Tagnumbers = parts.TagNumber;
                                                    lstdat.Add(tg);

                                                    selectedTagsData.photoTags = lstdat;
                                                    Settings.currentPoTagId_Inti = lstdat;

                                                    if (selectedTagsData.photoTags.Count != 0 && parts.IsPhotoRequired != 0)
                                                    {
                                                        #region single photo sending
                                                        PhotoUploadModel DataForFileUpload = new PhotoUploadModel();
                                                        DataForFileUpload = selectedTagsData;
                                                        DataForFileUpload.CreatedBy = Settings.userLoginID;
                                                        DataForFileUpload.photos = new List<Photo>();

                                                        foreach (var iniphoto in firstphotovalue)
                                                        {
                                                            Photo phUpload = new Photo();
                                                            phUpload.PUID = parts.PUID;
                                                            phUpload.PhotoID = 0;
                                                            phUpload.PhotoURL = iniphoto.FileUrl;
                                                            phUpload.PhotoDescription = iniphoto.FileDescription;
                                                            phUpload.FileName = iniphoto.FileName;
                                                            phUpload.CreatedBy = Settings.userLoginID;
                                                            phUpload.UploadType = styleid.Trim() == "a".Trim() ? (int)UploadTypeEnums.GoodsPhotos_AP : (int)UploadTypeEnums.GoodsPhotos_BP;// uploadType;
                                                            phUpload.CreatedDate = String.Format(Settings.DateFormat, DateTime.Now);
                                                            phUpload.GivenName = Settings.Username;
                                                            DataForFileUpload.photos.Add(phUpload);
                                                        }
                                                        var data = await service.InitialUpload(DataForFileUpload);
                                                        #endregion single photo sending

                                                        var result = data as InitialResponse;

                                                        if (result != null && result.status == 1)
                                                        {
                                                            #region Update task status
                                                            if (parts.TaskID != 0 && parts.TagTaskStatus == 0)
                                                            {
                                                                TagTaskStatus tagtaskstatus = new TagTaskStatus();
                                                                tagtaskstatus.TaskID = Helperclass.Encrypt(parts.TaskID.ToString());
                                                                tagtaskstatus.POTagID = Helperclass.Encrypt(parts.POTagID.ToString());
                                                                tagtaskstatus.Status = 1;
                                                                tagtaskstatus.CreatedBy = Settings.userLoginID;

                                                                var val = await service.UpdateTagTaskStatus(tagtaskstatus);

                                                                if (result?.status == 1)
                                                                {
                                                                    if (parts.TaskStatus == 0)
                                                                    {
                                                                        TagTaskStatus taskstatus = new TagTaskStatus();
                                                                        taskstatus.TaskID = Helperclass.Encrypt(parts.TaskID.ToString());
                                                                        taskstatus.TaskStatus = 1;
                                                                        taskstatus.CreatedBy = Settings.userLoginID;

                                                                        var taskval = await service.UpdateTaskStatus(taskstatus);
                                                                    }
                                                                }
                                                            }
                                                            parts.PUID = result.data.PUID;
                                                            #endregion Update task status
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    DependencyService.Get<IToastMessage>().ShortAlert("Photo(s) linked successfully.");
                                    await ShowContentsToLink();
                                    //}
                                    //else
                                    //{
                                    //    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                                    //}
                                }
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select" + (Settings.VersionID == 2 ? " VIN(s)" : " part(s)")
                                    + " to start linking photo(s).");
                            }
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LinkPhotoToTag method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        public async Task ShowContentsToLink()
        {
            YPSLogger.TrackEvent("LinkPageViewModel.cs", " in ShowContentsToLink method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var result = await GetAllPOData();

                if (result != null && result.data != null && result.data.allPoDataMobile != null)
                {
                    SelectedTagCountVisible = false;
                    IsRepoaPage = false;
                    UploadViewContentVisible = false;
                    IsPhotoUploadIconVisible = false;
                    POTagLinkContentVisible = true;
                    AllPoDataList = result.data.allPoDataMobile;
                    var potagcolections = result.data.allPoDataMobile.Where(wr => wr.TaskID > 0 && wr.IsPhotoRequired != 0
                    && wr.TaskResourceID != 0)
                        .OrderBy(o => o.EventID).ThenBy(tob => tob.TagTaskStatus).ThenBy(tob => tob.TagNumber)
                        .ThenBy(tob => tob.IdentCode).ToList();

                    foreach (var values in potagcolections)
                    {
                        #region Status icon
                        if (values.TagTaskStatus == 0)
                        {
                            values.TagTaskStatusIcon = Icons.Pending;
                        }
                        else if (values.TagTaskStatus == 1)
                        {
                            values.TagTaskStatusIcon = Icons.Progress;
                        }
                        else
                        {
                            values.TagTaskStatusIcon = Icons.Done;
                        }
                        #endregion Status icon

                        values.IsTaskResourceVisible = values.TaskResourceID == Settings.userLoginID ? false : true;
                        values.IsTagDescLabelVisible = string.IsNullOrEmpty(values.IDENT_DEVIATED_TAG_DESC) ? false : true;
                        values.IsConditionNameLabelVisible = string.IsNullOrEmpty(values.ConditionName) ? false : true;
                    }

                    AllPoTagCollections = new ObservableCollection<AllPoData>(potagcolections);

                    NoRecordsLbl = (IsLinkButtonsVisible = AllPoTagCollections != null && AllPoTagCollections.Count > 0 ? true : false) == true ? false : true;
                }
                else
                {
                    IsLinkButtonsVisible = false;
                    NoRecordsLbl = true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShowContentsToLink method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Dynamic text changed
        /// </summary>
        public async Task DynamicTextChange()
        {
            try
            {
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var desc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == DescriptipnPlaceholder.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var afterpacking = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.AfterPacking.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var beforepacking = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.BeforePacking.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Getting Label values & Status based on FieldID
                        var poid = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.POID.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var tagdesc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.TagDesc.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var resource = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Resource.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var tagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.IdentCode.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var invoicenumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.InvoiceNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ConditionName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.ShippingNumber.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid?.Status == 1 ? true : false;
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme?.Status == 1 ? true : false;
                        labelobj.TagDesc.Name = (tagdesc != null ? (!string.IsNullOrEmpty(tagdesc.LblText) ? tagdesc.LblText : labelobj.TagDesc.Name) : labelobj.TagDesc.Name) + " :";
                        labelobj.TagDesc.Status = tagdesc?.Status == 1 ? true : false;
                        labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                        labelobj.EventName.Status = eventname?.Status == 1 ? true : false;
                        labelobj.Resource.Name = (resource != null ? (!string.IsNullOrEmpty(resource.LblText) ? resource.LblText : labelobj.Resource.Name) : labelobj.Resource.Name) + " :";
                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber?.Status == 1 ? true : false;
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode?.Status == 1 ? true : false;
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname?.Status == 1 ? true : false;
                        labelobj.InvoiceNumber.Name = (invoicenumber != null ? (!string.IsNullOrEmpty(invoicenumber.LblText) ? invoicenumber.LblText : labelobj.InvoiceNumber.Name) : labelobj.InvoiceNumber.Name) + " :";
                        labelobj.InvoiceNumber.Status = invoicenumber?.Status == 1 ? true : false;
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber?.Status == 1 ? true : false;

                        labelobj.AfterPacking.Name = "To " + (afterpacking != null ? (!string.IsNullOrEmpty(afterpacking.LblText) ? afterpacking.LblText : labelobj.AfterPacking.Name) : labelobj.AfterPacking.Name);
                        labelobj.AfterPacking.Status = afterpacking == null ? true : (afterpacking.Status == 1 ? true : false);
                        labelobj.BeforePacking.Name = "To " + (beforepacking != null ? (!string.IsNullOrEmpty(beforepacking.LblText) ? beforepacking.LblText : labelobj.BeforePacking.Name) : labelobj.BeforePacking.Name);
                        labelobj.BeforePacking.Status = beforepacking == null ? true : (beforepacking.Status == 1 ? true : false);
                        DescriptipnPlaceholder = desc != null ? (!string.IsNullOrEmpty(desc) ? desc : DescriptipnPlaceholder) : DescriptipnPlaceholder;
                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    PackingButtonOpacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        public async Task<GetPoData> GetAllPOData()
        {
            GetPoData result = new GetPoData();
            try
            {
                sendPodata = new SendPodata();
                sendPodata.UserID = Settings.userLoginID;
                sendPodata.PageSize = Settings.pageSizeYPS;
                sendPodata.StartPage = Settings.startPageYPS;

                result = await service.LoadPoDataService(sendPodata);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllPOData method -> in LinkPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
            return result;
        }

        #region Properties
        #region Properties for dynamic label change
        public class LabelAndActionsChangeClass
        {
            public LabelAndActionFields POID { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "PONumber"
            };
            public LabelAndActionFields TagNumber { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "TagNumber"
            };
            public LabelAndActionFields TaskName { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "TaskName"
            };
            public LabelAndActionFields Resource { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "Resource"
            };
            public LabelAndActionFields EventName { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "Event"
            };

            public LabelAndActionFields TagDesc { get; set; } = new LabelAndActionFields
            {
                Status = false,  //Name = "TagDescription"
                Name = "IDENT_DEVIATED_TAG_DESC"
            };


            public LabelAndActionFields IdentCode { get; set; } = new LabelAndActionFields { Status = false, Name = "IdentCode" };
            public LabelAndActionFields ConditionName { get; set; } = new LabelAndActionFields { Status = false, Name = "ConditionName" };
            //public LabelAndActionFields InvoiceNumber { get; set; } = new LabelAndActionFields { Status = true, Name = "InvoiceNumber" };
            public LabelAndActionFields InvoiceNumber { get; set; } = new LabelAndActionFields { Status = false, Name = "Invoice1No" };
            public LabelAndActionFields ShippingNumber { get; set; } = new LabelAndActionFields { Status = false, Name = "Shipping Number" };
            public LabelAndActionFields BeforePacking { get; set; } = new LabelAndActionFields { Status = true, Name = "Before Packing" };
            public LabelAndActionFields AfterPacking { get; set; } = new LabelAndActionFields { Status = true, Name = "After Packing" };
        }
        public class LabelAndActionFields : IBase
        {
            public bool _Status;
            public bool Status
            {
                get => _Status;
                set
                {
                    _Status = value;
                    NotifyPropertyChanged();
                }
            }

            public string _Name;
            public string Name
            {
                get => _Name;
                set
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public LabelAndActionsChangeClass _labelobj = new LabelAndActionsChangeClass();
        public LabelAndActionsChangeClass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        private double _PackingButtonOpacity = 1.0;
        public double PackingButtonOpacity
        {
            get { return _PackingButtonOpacity; }
            set
            {
                _PackingButtonOpacity = value;
                NotifyPropertyChanged();
            }
        }

        private bool _NoRecordsLbl { set; get; }
        public bool NoRecordsLbl
        {
            get
            {
                return _NoRecordsLbl;
            }
            set
            {
                this._NoRecordsLbl = value;
                RaisePropertyChanged("NoRecordsLbl");
            }
        }

        private bool _IsLinkButtonsVisible { set; get; }
        public bool IsLinkButtonsVisible
        {
            get
            {
                return _IsLinkButtonsVisible;
            }
            set
            {
                this._IsLinkButtonsVisible = value;
                RaisePropertyChanged("IsLinkButtonsVisible");
            }
        }

        public string _SelectedPartsNo = "Record(s) selected";
        public string SelectedPartsNo
        {
            get => _SelectedPartsNo;
            set
            {
                _SelectedPartsNo = value;
                RaisePropertyChanged("SelectedPartsNo");
            }
        }

        private bool _SelectedTagCountVisible { set; get; }
        public bool SelectedTagCountVisible
        {
            get
            {
                return _SelectedTagCountVisible;
            }
            set
            {
                this._SelectedTagCountVisible = value;
                RaisePropertyChanged("SelectedTagCountVisible");
            }
        }

        private int _SelectedTagCount { set; get; }
        public int SelectedTagCount
        {
            get
            {
                return _SelectedTagCount;
            }
            set
            {
                this._SelectedTagCount = value;
                RaisePropertyChanged("SelectedTagCount");
            }
        }

        private int _RowHeightOpenCam = 50;
        public int RowHeightOpenCam
        {
            get => _RowHeightOpenCam;
            set
            {
                _RowHeightOpenCam = value;
                NotifyPropertyChanged();
            }
        }

        private int _NoRecHeight = 20;
        public int NoRecHeight
        {
            get => _NoRecHeight;
            set
            {
                _NoRecHeight = value;
                NotifyPropertyChanged();
            }
        }

        private string _Title = Settings.VersionID == 2 ? "Link VIN" : "Link Parts";
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }


        private bool _IsRepoaPage = true;
        public bool IsRepoaPage
        {
            get { return _IsRepoaPage; }
            set
            {
                _IsRepoaPage = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<AllPoData> _AllPoTagCollections;
        public ObservableCollection<AllPoData> AllPoTagCollections
        {
            get { return _AllPoTagCollections; }
            set
            {
                _AllPoTagCollections = value;
                //if (value != null && value.Count > 0)
                //{
                //    //PONumber = value[0].PONumber;
                //    //ShippingNumber = value[0].ShippingNumber;
                //    //REQNo = value[0].REQNo;
                //    //TaskName = value[0].TaskName;
                //}
                RaisePropertyChanged("AllPoTagCollections");
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

        private bool _POTagLinkContentVisible = false;
        public bool POTagLinkContentVisible
        {
            get { return _POTagLinkContentVisible; }
            set
            {
                _POTagLinkContentVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _UploadViewContentVisible = true;
        public bool UploadViewContentVisible
        {
            get { return _UploadViewContentVisible; }
            set
            {
                _UploadViewContentVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsBottomButtonsVisible = false;
        public bool IsBottomButtonsVisible
        {
            get { return _IsBottomButtonsVisible; }
            set
            {
                _IsBottomButtonsVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsImageViewForUploadVisible = true;
        public bool IsImageViewForUploadVisible
        {
            get { return _IsImageViewForUploadVisible; }
            set
            {
                _IsImageViewForUploadVisible = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PhotoRepoDBModel> _RepoPhotosList;
        public ObservableCollection<PhotoRepoDBModel> RepoPhotosList
        {
            get { return _RepoPhotosList; }
            set
            {
                _RepoPhotosList = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsNoPhotoTxt = true;
        public bool IsNoPhotoTxt
        {
            get { return _IsNoPhotoTxt; }
            set
            {
                _IsNoPhotoTxt = value;
                NotifyPropertyChanged();
            }
        }

        private string _DescriptionText = "";
        public string DescriptionText
        {
            get { return _DescriptionText; }
            set
            {
                _DescriptionText = value;
                NotifyPropertyChanged();
            }
        }

        private bool _DeleteAllPhotos = false;
        public bool DeleteAllPhotos
        {
            get { return _DeleteAllPhotos; }
            set
            {
                _DeleteAllPhotos = value;
                NotifyPropertyChanged();
            }
        }

        private string _DescriptipnPlaceholder = "Description";
        public string DescriptipnPlaceholder
        {
            get => _DescriptipnPlaceholder;
            set
            {
                _DescriptipnPlaceholder = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsUploadStackVisible = false;
        public bool IsUploadStackVisible
        {
            get { return _IsUploadStackVisible; }
            set
            {
                _IsUploadStackVisible = value;
                NotifyPropertyChanged();
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

        private bool _IsPhotoUploadIconVisible = true;
        public bool IsPhotoUploadIconVisible
        {
            get { return _IsPhotoUploadIconVisible; }
            set
            {
                _IsPhotoUploadIconVisible = value;
                NotifyPropertyChanged();
            }
        }


        private bool _DeleteIconStack = true;
        public bool DeleteIconStack
        {
            get => _DeleteIconStack;
            set
            {
                _DeleteIconStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsPhotosListStackVisible = true;
        public bool IsPhotosListStackVisible
        {
            get { return _IsPhotosListStackVisible; }
            set
            {
                _IsPhotosListStackVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsPhotosListVisible = true;
        public bool IsPhotosListVisible
        {
            get { return _IsPhotosListVisible; }
            set
            {
                _IsPhotosListVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string _ScannedCompareData;
        public string ScannedCompareData
        {
            get { return _ScannedCompareData; }
            set
            {
                _ScannedCompareData = value;
                RaisePropertyChanged("ScannedCompareData");
            }
        }

        public string _PageNextButton = "Link";
        public string PageNextButton
        {
            get { return _PageNextButton; }
            set
            {
                _PageNextButton = value;
                RaisePropertyChanged("PageNextButton");
            }
        }

        public AllPoData _ScannedAllPOData;
        public AllPoData ScannedAllPOData
        {
            get { return _ScannedAllPOData; }
            set
            {
                _ScannedAllPOData = value;
                RaisePropertyChanged("ScannedAllPOData");
            }
        }

        public string _ScannedValue;
        public string ScannedValue
        {
            get { return _ScannedValue; }
            set
            {
                _ScannedValue = value;
                RaisePropertyChanged("ScannedValue");
            }
        }

        public string _ScannedOn;
        public string ScannedOn
        {
            get { return _ScannedOn; }
            set
            {
                _ScannedOn = value;
                RaisePropertyChanged("ScannedOn");
            }
        }

        public double _LinkOpacity = 0.5;
        public double LinkOpacity
        {
            get { return _LinkOpacity; }
            set
            {
                _LinkOpacity = value;
                RaisePropertyChanged("LinkOpacity");
            }
        }

        public bool _IsLinkEnable;
        public bool IsLinkEnable
        {
            get { return _IsLinkEnable; }
            set
            {
                _IsLinkEnable = value;
                RaisePropertyChanged("IsLinkEnable");
            }
        }


        public double _DeleteAllOpacity = 0.5;
        public double DeleteAllOpacity
        {
            get { return _DeleteAllOpacity; }
            set
            {
                _DeleteAllOpacity = value;
                RaisePropertyChanged("DeleteAllOpacity");
            }
        }

        public bool _IsDeleteAllEnable;
        public bool IsDeleteAllEnable
        {
            get { return _IsDeleteAllEnable; }
            set
            {
                _IsDeleteAllEnable = value;
                RaisePropertyChanged("IsDeleteAllEnable");
            }
        }

        public string _StatusText;
        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                _StatusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

        private Color _StatusTextBgColor = Color.Gray;
        public Color StatusTextBgColor
        {
            get => _StatusTextBgColor;
            set
            {
                _StatusTextBgColor = value;
                RaisePropertyChanged("StatusTextBgColor");
            }
        }

        public string _ScannedResult;
        public string ScannedResult
        {
            get { return _ScannedResult; }
            set
            {
                _ScannedResult = value;
                RaisePropertyChanged("ScannedResult");
            }
        }

        public bool _IsScanPage;
        public bool IsScanPage
        {
            get { return _IsScanPage; }
            set
            {
                _IsScanPage = value;
                RaisePropertyChanged("IsScanPage");
            }
        }

        public bool _CanOpenScan = true;
        public bool CanOpenScan
        {
            get { return _CanOpenScan; }
            set
            {
                _CanOpenScan = value;
                RaisePropertyChanged("CanOpenScan");
            }
        }



        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                RaisePropertyChanged("IndicatorVisibility");
            }
        }
        #endregion Preopeties
    }
}
