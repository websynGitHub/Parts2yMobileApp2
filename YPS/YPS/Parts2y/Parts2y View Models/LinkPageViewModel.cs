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

        //public ObservableCollection<PhotoRepoModel> RepoPhotosList { get; set; }
        YPSService service;
        LinkPage pagename;
        SendPodata sendPodata;

        public LinkPageViewModel(INavigation navigation, ObservableCollection<PhotoRepoModel> repophotosist, LinkPage page)
        {
            try
            {
                Title = Settings.VersionID == 2 ? "VIN" : "Parts";
                service = new YPSService();
                Navigation = navigation;
                pagename = page;
                RepoPhotosList = repophotosist;
                viewExistingBUPhotos = new Command(tap_eachCamB);
                viewExistingAUPhotos = new Command(tap_eachCamA);
                LinkBeforePackingPhotoCmd = new Command(LinkPhotoToTag);
                LinkAfterPackingPhotoCmd = new Command(LinkPhotoToTag);

                Task.Run(() => DynamicTextChange().Wait());
                Task.Run(() => ShowContentsToLink().Wait());
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LinkPageViewModel constructor -> in LinkPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        private async void tap_eachCamB(object obj)
        {
            try
            {
                YPSLogger.TrackEvent("LinkPageViewModel.cs", "in tap_eachCamB method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var potag = obj as AllPoData;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (potag.imgCamOpacityB != 0.5)
                    {
                        IndicatorVisibility = true;

                        try
                        {
                            //if (potag.BPhotoCount > 0)
                            //{
                            updateList = true;
                            Settings.CanOpenScanner = true;
                            Settings.currentPuId = potag.PUID;
                            Settings.BphotoCount = potag.TagBPhotoCount;
                            if (potag.PUID == 0)
                            {
                                PhotoUploadModel selectedTagsData = new PhotoUploadModel();


                                //var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
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

                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(selectedTagsData, null, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false));
                            }
                            else
                            {
                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, potag, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, potag.photoTickVisible));
                            }

                            //}
                            //else
                            //{
                            //    DependencyService.Get<IToastMessage>().ShortAlert("No photos available.");
                            //}
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

                YPSLogger.TrackEvent("LinkPageViewModel.cs", "in tap_eachCamA method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var potag = obj as AllPoData;

                if (potag.imgCamOpacityA != 0.5)
                {
                    IndicatorVisibility = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {

                        try
                        {
                            //if (allPo.APhotoCount > 0)
                            //{
                            updateList = true;
                            Settings.CanOpenScanner = true;
                            Settings.AphotoCount = potag.TagAPhotoCount;
                            Settings.currentPuId = potag.PUID;

                            if (potag.PUID == 0)
                            {
                                PhotoUploadModel selectedTagsData = new PhotoUploadModel();


                                //var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
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

                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(selectedTagsData, null, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, false));

                            }
                            else
                            {
                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, potag, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, potag.photoTickVisible));
                            }
                            //}
                            //else
                            //{
                            //    DependencyService.Get<IToastMessage>().ShortAlert("No photos available.");
                            //}
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
            IndicatorVisibility = false;
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void LinkPhotoToTag(object sender)
        {
            YPSLogger.TrackEvent("LinkPageViewModel.cs", "in LinkPhotoToTag method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var sfbutton = sender as SfButton;
                var value = (AllPoData)sfbutton.BindingContext;
                var styleid = sfbutton.StyleId;

                if (value != null)
                {
                    var linkfor = !string.IsNullOrEmpty(value.TagNumber) ? value.TagNumber : value.IdentCode;
                    bool move = await App.Current.MainPage.DisplayAlert("Photo(s) linking", "Are you sure you want to link the photo(s) to " + linkfor + "?", "Yes", "No");

                    if (move)
                    {
                        updateList = true;

                        Settings.CanOpenScanner = true;

                        //var firstphotovalue = RepoPhotosList.FirstOrDefault();
                        var firstphotovalue = RepoPhotosList;


                        PhotoUploadModel selectedTagsData = new PhotoUploadModel();

                        taskID = value.TaskID;
                        //Settings.POID = value.POID;
                        selectedTagsData.POID = value.POID;
                        selectedTagsData.isCompleted = value.photoTickVisible;

                        List<PhotoTag> lstdat = new List<PhotoTag>();

                        if (value.TagAPhotoCount == 0 && value.TagBPhotoCount == 0 && value.PUID == 0)
                        {
                            PhotoTag tg = new PhotoTag();

                            if (value.POTagID != 0)
                            {
                                tg.POTagID = value.POTagID;
                                Settings.Tagnumbers = value.TagNumber;
                                lstdat.Add(tg);

                                selectedTagsData.photoTags = lstdat;
                                Settings.currentPoTagId_Inti = lstdat;


                                if (selectedTagsData.photoTags.Count != 0 && value.IsPhotoRequired != 0)
                                {
                                    //List<PhotoUploadModel> DataForFileUploadList = new List<PhotoUploadModel>();


                                    PhotoUploadModel DataForFileUpload = new PhotoUploadModel();
                                    DataForFileUpload = selectedTagsData;
                                    DataForFileUpload.CreatedBy = Settings.userLoginID;
                                    //DataForFileUpload.photo.FileName = firstphotovalue[0].FileName;
                                    DataForFileUpload.photos = new List<Photo>();

                                    //foreach (var iniphoto in firstphotovalue)
                                    //{
                                    Photo phUpload = new Photo();
                                    phUpload.PUID = value.PUID;
                                    phUpload.PhotoID = 0;
                                    phUpload.PhotoURL = firstphotovalue[0].FullFileName;
                                    phUpload.PhotoDescription = firstphotovalue[0].Description;
                                    phUpload.FileName = firstphotovalue[0].FileName;
                                    phUpload.CreatedBy = Settings.userLoginID;
                                    phUpload.UploadType = styleid.Trim() == "a".Trim() ? (int)UploadTypeEnums.GoodsPhotos_AP : (int)UploadTypeEnums.GoodsPhotos_BP;// uploadType;
                                    phUpload.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                                    phUpload.GivenName = Settings.Username;
                                    DataForFileUpload.photos.Add(phUpload);
                                    //}


                                    //DataForFileUploadList.Add(DataForFileUpload);

                                    var data = await service.InitialUpload(DataForFileUpload);

                                    var result = data as InitialResponse;

                                    if (result != null && result.status == 1)
                                    {
                                        #region single photo sending
                                        if (value.TagTaskStatus == 0)
                                        {
                                            TagTaskStatus tagtaskstatus = new TagTaskStatus();
                                            tagtaskstatus.TaskID = Helperclass.Encrypt(value.TaskID.ToString());
                                            tagtaskstatus.POTagID = Helperclass.Encrypt(value.POTagID.ToString());
                                            tagtaskstatus.Status = 1;
                                            tagtaskstatus.CreatedBy = Settings.userLoginID;

                                            var val = await service.UpdateTagTaskStatus(tagtaskstatus);

                                            if (result.status == 1)
                                            {
                                                if (value.TaskID == 0)
                                                {
                                                    TagTaskStatus taskstatus = new TagTaskStatus();
                                                    taskstatus.TaskID = Helperclass.Encrypt(value.TaskID.ToString());
                                                    taskstatus.TaskStatus = 1;
                                                    taskstatus.CreatedBy = Settings.userLoginID;

                                                    var taskval = await service.UpdateTaskStatus(taskstatus);
                                                }
                                            }
                                        }

                                        if (styleid != null)
                                        {
                                            value.PUID = result.data.PUID;
                                            if (styleid.Trim() == "a".Trim())
                                            {
                                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, value, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, value.photoTickVisible));
                                            }
                                            else
                                            {
                                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, value, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, value.photoTickVisible));
                                            }
                                        }
                                        #endregion single photo sending


                                        #region upload photo from second item
                                        List<PhotoRepoModel> newphotolist = new List<PhotoRepoModel>(firstphotovalue);
                                        newphotolist.RemoveAt(0);

                                        if (firstphotovalue.Count > 0)
                                        {
                                            List<CustomPhotoModel> phUploadList = new List<CustomPhotoModel>();

                                            foreach (var photo in firstphotovalue)
                                            {
                                                CustomPhotoModel photomodel = new CustomPhotoModel();
                                                photomodel.PUID = value.PUID;
                                                photomodel.PhotoID = 0;
                                                photomodel.PhotoURL = photo.FullFileName;
                                                photomodel.PhotoDescription = photo.Description;
                                                photomodel.FileName = photo.FileName;
                                                photomodel.CreatedBy = Settings.userLoginID;
                                                photomodel.UploadType = styleid.Trim() == "a".Trim() ? (int)UploadTypeEnums.GoodsPhotos_AP : (int)UploadTypeEnums.GoodsPhotos_BP;// uploadType;
                                                photomodel.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                                                photomodel.FullName = Settings.Username;
                                                phUploadList.Add(photomodel);
                                            }

                                            var dataresult = await service.PhotosUpload(phUploadList);

                                            var initialresult = dataresult as SecondTimeResponse;

                                            if (initialresult != null && initialresult.status == 1)
                                            {

                                                if (styleid != null)
                                                {
                                                    foreach (var val in initialresult.data)
                                                    {
                                                        value.PUID = val.PUID;
                                                    }

                                                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count() - 1]);

                                                    if (styleid.Trim() == "a".Trim())
                                                    {
                                                        await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, value, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, value.photoTickVisible));
                                                    }
                                                    else
                                                    {
                                                        await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, value, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, value.photoTickVisible));
                                                    }
                                                }

                                                DependencyService.Get<IToastMessage>().ShortAlert("Photo(s) linked successfully.");

                                            }
                                        }
                                        #endregion upload photo from second item

                                        //DependencyService.Get<IToastMessage>().ShortAlert("Photo(s) linked successfully.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            selectedTagsData.alreadyExit = "alreadyExit";

                            if (value.imgCamOpacityB != 0.5 && value.IsPhotoRequired != 0)
                            {
                                if (value.PUID != 0)
                                {
                                    List<CustomPhotoModel> phUploadList = new List<CustomPhotoModel>();

                                    foreach (var photo in firstphotovalue)
                                    {
                                        CustomPhotoModel phUpload = new CustomPhotoModel();
                                        phUpload.PUID = value.PUID;
                                        phUpload.PhotoID = 0;
                                        phUpload.PhotoURL = photo.FullFileName;
                                        phUpload.PhotoDescription = photo.Description;
                                        phUpload.FileName = photo.FileName;
                                        phUpload.CreatedBy = Settings.userLoginID;
                                        phUpload.UploadType = styleid.Trim() == "a".Trim() ? (int)UploadTypeEnums.GoodsPhotos_AP : (int)UploadTypeEnums.GoodsPhotos_BP;// uploadType;
                                        phUpload.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                                        phUpload.FullName = Settings.Username;
                                        phUploadList.Add(phUpload);

                                    }


                                    var data = await service.PhotosUpload(phUploadList);

                                    var initialresult = data as SecondTimeResponse;

                                    if (initialresult != null && initialresult.status == 1)
                                    {

                                        if (styleid != null)
                                        {
                                            foreach (var val in initialresult.data)
                                            {
                                                value.PUID = val.PUID;
                                            }

                                            if (styleid.Trim() == "a".Trim())
                                            {
                                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, value, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, value.photoTickVisible));
                                            }
                                            else
                                            {
                                                await Navigation.PushAsync(new YPS.Views.PhotoUpload(null, value, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, value.photoTickVisible));
                                            }
                                        }

                                        DependencyService.Get<IToastMessage>().ShortAlert("Photo(s) linked successfully.");

                                    }

                                }
                            }
                        }
                    }
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
            YPSLogger.TrackEvent("LinkPageViewModel.cs", "in ShowContentsToLink method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {

                var result = await GetAllPOData();

                if (result != null && result.data != null && result.data.allPoData != null)
                {
                    IsRepoaPage = false;
                    UploadViewContentVisible = false;
                    IsPhotoUploadIconVisible = false;
                    POTagLinkContentVisible = true;
                    var potagcolections = result.data.allPoData.Where(wr => wr.TaskID > 0 && wr.IsPhotoRequired != 0).OrderBy(o => o.TagTaskStatus).ThenBy(tob => tob.TagNumber).ThenBy(tob => tob.IdentCode).ToList();

                    foreach (var values in potagcolections)
                    {
                        #region Status icon
                        if (values.TagTaskStatus == 0)
                        {
                            values.TagTaskStatusIcon = Icons.circle;
                        }
                        else if (values.TagTaskStatus == 1)
                        {
                            values.TagTaskStatusIcon = Icons.Tickicon;
                        }
                        else
                        {
                            values.TagTaskStatusIcon = Icons.CheckCircle;
                        }
                        #endregion Status icon
                    }

                    AllPoTagCollections = new ObservableCollection<AllPoData>(potagcolections);
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
                        var done = labelval.Where(wr => wr.FieldID == labelobj.Done.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var upload = labelval.Where(wr => wr.FieldID == labelobj.Upload.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var desc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == DescriptipnPlaceholder.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var afterpacking = labelval.Where(wr => wr.FieldID == labelobj.AfterPacking.Name.Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var beforepacking = labelval.Where(wr => wr.FieldID == labelobj.BeforePacking.Name.Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Getting Label values & Status based on FieldID
                        var poid = labelval.Where(wr => wr.FieldID == labelobj.POID.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID == labelobj.ShippingNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqnumber = labelval.Where(wr => wr.FieldID == labelobj.REQNo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID == labelobj.TaskName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var tagdesc = labelval.Where(wr => wr.FieldID == labelobj.TagDesc.Name.Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var tagnumber = labelval.Where(wr => wr.FieldID == labelobj.TagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID == labelobj.IdentCode.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bagnumber = labelval.Where(wr => wr.FieldID == labelobj.BagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID == labelobj.ConditionName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid == null ? true : (poid.Status == 1 ? true : false);
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber == null ? true : (shippingnumber.Status == 1 ? true : false);
                        labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                        labelobj.REQNo.Status = reqnumber == null ? true : (reqnumber.Status == 1 ? true : false);
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme == null ? true : (taskanme.Status == 1 ? true : false);
                        labelobj.TagDesc.Name = (tagdesc != null ? (!string.IsNullOrEmpty(tagdesc.LblText) ? tagdesc.LblText : labelobj.TagDesc.Name) : labelobj.TagDesc.Name) + " :";
                        labelobj.TagDesc.Status = tagdesc == null ? true : (tagdesc.Status == 1 ? true : false);

                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber == null ? true : (tagnumber.Status == 1 ? true : false);
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode == null ? true : (identcode.Status == 1 ? true : false);
                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                        labelobj.BagNumber.Status = bagnumber == null ? true : (bagnumber.Status == 1 ? true : false);
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname == null ? true : (conditionname.Status == 1 ? true : false);


                        labelobj.AfterPacking.Name = "Link to " + (afterpacking != null ? (!string.IsNullOrEmpty(afterpacking.LblText) ? afterpacking.LblText : labelobj.AfterPacking.Name) : labelobj.AfterPacking.Name);
                        labelobj.AfterPacking.Status = afterpacking == null ? true : (afterpacking.Status == 1 ? true : false);
                        labelobj.BeforePacking.Name = "Link to " + (beforepacking != null ? (!string.IsNullOrEmpty(beforepacking.LblText) ? beforepacking.LblText : labelobj.BeforePacking.Name) : labelobj.BeforePacking.Name);
                        labelobj.BeforePacking.Status = beforepacking == null ? true : (beforepacking.Status == 1 ? true : false);

                        labelobj.Done.Name = (done != null ? (!string.IsNullOrEmpty(done.LblText) ? done.LblText : labelobj.Done.Name) : labelobj.Done.Name);
                        labelobj.Done.Status = done == null ? true : (done.Status == 1 ? true : false);
                        labelobj.Upload.Name = (upload != null ? (!string.IsNullOrEmpty(upload.LblText) ? upload.LblText : labelobj.Upload.Name) : labelobj.Upload.Name);
                        labelobj.Upload.Status = upload == null ? true : (upload.Status == 1 ? true : false);
                        DescriptipnPlaceholder = desc != null ? (!string.IsNullOrEmpty(desc) ? desc : DescriptipnPlaceholder) : DescriptipnPlaceholder;
                    }

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
                //SearchResultGet(sendPodata);

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
            public LabelAndActionFields Done { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Done"
            };

            public LabelAndActionFields Upload { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Upload"
            };

            public LabelAndActionFields Load { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Load"
            };

            public LabelAndActionFields Parts { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Parts"
            };

            public LabelAndActionFields POID { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "PONumber"
            };
            public LabelAndActionFields REQNo { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "REQNo"
            };
            public LabelAndActionFields ShippingNumber { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "ShippingNumber"
            };
            public LabelAndActionFields TagNumber { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "TagNumber"
            };
            public LabelAndActionFields TaskName { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "TaskName"
            };
            public LabelAndActionFields TagDesc { get; set; } = new LabelAndActionFields { Status = true, Name = "Tag Description" };


            public LabelAndActionFields IdentCode { get; set; } = new LabelAndActionFields { Status = true, Name = "IdentCode" };
            public LabelAndActionFields BagNumber { get; set; } = new LabelAndActionFields { Status = true, Name = "BagNumber" };
            public LabelAndActionFields ConditionName { get; set; } = new LabelAndActionFields { Status = true, Name = "ConditionName" };
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

        private string _Title = Settings.VersionID == 2 ? "VIN" : "Parts";
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

        private ObservableCollection<PhotoRepoModel> _RepoPhotosList;
        public ObservableCollection<PhotoRepoModel> RepoPhotosList
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
