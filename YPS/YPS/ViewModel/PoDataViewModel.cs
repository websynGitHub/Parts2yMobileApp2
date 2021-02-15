using Newtonsoft.Json;
using Syncfusion.SfDataGrid.XForms;
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
using YPS.Service;
using YPS.Views;
using static YPS.Model.SearchModel;
using static YPS.Models.ChatMessage;

namespace YPS.ViewModel
{
    public class PoDataViewModel : INotifyPropertyChanged
    {
        #region IComman and data members declaration
        public INavigation Navigation { get; set; }
        public ICommand tap_InitialCameraCommand { set; get; }
        public ICommand tap_OnMessageCommand { set; get; }
        public ICommand tap_InitialFileUploadCommand { set; get; }
        public ICommand tap_PrinterCommand { get; set; }
        public ICommand tap_OnChat { get; set; }
        public ICommand tap_OnCamB { get; set; }
        public ICommand tap_OnCamA { get; set; }
        public ICommand tap_FileUpload { get; set; }
        public ICommand ChooseColumns_Tap { get; set; }
        public ICommand btn_close { get; set; }
        public ICommand btn_done { get; set; }
        public ICommand profile_Change_Tap { get; set; }
        public ICommand IsRequired_Tap_IC { get; set; }
        public ICommand IsNotRequired_Tap_IC { get; set; }
        public ICommand tap_OnFilterCommand { get; set; }
        public ICommand tab_RefreshPage { get; set; }
        public ICommand tab_ClearSearch { get; set; }
        public ICommand ShowPageSizePickerItems { get; set; }

        YPSService trackService;
        MainPage pagename;
        private bool avoidMutiplePageOpen = false;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="_Navigation"></param>
        /// <param name="page"></param>
        public PoDataViewModel(INavigation _Navigation, MainPage page)
        {
            try
            {
                YPSLogger.TrackEvent("PoDataViewModel", "page loading " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Navigation = _Navigation;
                pagename = page;
                trackService = new YPSService();
                Settings.QAType = (int)QAType.PT;
                var cID = Settings.CompanyID;
                var pID = Settings.ProjectID;
                var jID = Settings.JobID;
                DataGrid = true;
                profileSettingVisible = true;
                mainStack = true;

                #region Assigning methods to the respective ICommands
                tab_ClearSearch = new Command(async () => await ClearSearch());
                tap_OnChat = new Command(tap_eachMessage);
                tap_OnCamB = new Command(tap_eachCamB);
                tap_OnCamA = new Command(tap_eachCamA);
                tap_FileUpload = new Command(tap_eachFileUpload);
                btn_close = new Command(closeBtn);
                btn_done = new Command(doneBtn);
                IsRequired_Tap_IC = new Command(IsRequired_Tap);
                IsNotRequired_Tap_IC = new Command(IsNotRequired_Tap);
                #endregion
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "PoDataViewModel constructor -> in PoDataViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method is called when clicked on PhotoRequired icon. 
        /// </summary>
        /// <param name="obj"></param>
        public async void IsRequired_Tap(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in IsRequired_Tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {

                var dataGrid = obj as SfDataGrid;
                var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                var uniq = data.GroupBy(x => x.POShippingNumber);

                if (uniq.Count() >= 2)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please select any one group.");
                }
                else if (uniq.Count() == 0)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please select tag.");
                }
                else if (uniq.Count() == 1)
                {
                    var requiredDataCount = from s in data
                                            where s.TagAPhotoCount > 0 || s.TagBPhotoCount > 0 || s.ISPhotoClosed == 1
                                            select s;
                    if (requiredDataCount.Count() > 0)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Photo upload is already created.");
                    }
                    else
                    {
                        List<PhotoTag> lstVal = new List<PhotoTag>();
                        string poTagIDs = "";
                        foreach (var podata in uniq)
                        {

                            foreach (var item in podata)
                            {
                                poTagIDs += item.POTagID + ",";
                                lstVal.Add(new PhotoTag() { POTagID = item.POTagID });
                            }
                        }
                        poTagIDs = poTagIDs.TrimEnd(',');

                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            if (!String.IsNullOrEmpty(poTagIDs))
                            {
                                YPSService yPSService = new YPSService();
                                var result = await yPSService.IsRequiredOrNotReuired(poTagIDs, 1);

                                if (result != null)
                                {
                                    if (result.message == "Required")
                                    {
                                        var updateCounts = PoDataCollections
                                       .Where(x => x.POTagID == (lstVal.Where(k => k.POTagID == x.POTagID).Select(p => p.POTagID).FirstOrDefault()))
                                       .Select(c =>
                                       {
                                           c.cameImage = "minus.png";
                                           return c;
                                       }).ToList();
                                    }
                                }

                            }

                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "IsRequired_Tap method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is called when clicked on PhotoNotRequired icon.
        /// </summary>
        /// <param name="obj"></param>
        public async void IsNotRequired_Tap(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in IsNotRequired_Tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var dataGrid = obj as SfDataGrid;
                    var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                    var uniq = data.GroupBy(x => x.POShippingNumber);

                    if (uniq.Count() >= 2)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select any one group.");
                    }
                    else if (uniq.Count() == 0)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select tag.");
                    }
                    else if (uniq.Count() == 1)
                    {
                        var requiredDataCount = from s in data
                                                where s.TagAPhotoCount > 0 || s.TagBPhotoCount > 0 || s.ISPhotoClosed == 1
                                                select s;
                        if (requiredDataCount.Count() > 0)
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Photo upload is already created.");
                        }
                        else
                        {
                            List<PhotoTag> lstVal = new List<PhotoTag>();
                            string poTagIDs = "";
                            foreach (var podata in uniq)
                            {
                                foreach (var item in podata)
                                {
                                    poTagIDs += item.POTagID + ",";
                                    lstVal.Add(new PhotoTag() { POTagID = item.POTagID });
                                }
                            }
                            poTagIDs = poTagIDs.TrimEnd(',');

                            if (!String.IsNullOrEmpty(poTagIDs))
                            {
                                YPSService yPSService = new YPSService();
                                var result = await yPSService.IsRequiredOrNotReuired(poTagIDs, 0);

                                if (result != null)
                                {
                                    if (result.message == "Not Required")
                                    {

                                        var updateCounts = PoDataCollections
                                         .Where(x => x.POTagID == (lstVal.Where(k => k.POTagID == x.POTagID).Select(p => p.POTagID).FirstOrDefault()))
                                         .Select(c =>
                                         {
                                             c.cameImage = "cross.png";
                                             return c;
                                         }).ToList();
                                    }
                                }
                            }
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
                YPSLogger.ReportException(ex, "IsNotRequired_Tap method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is called when clicked on done button from choose column popup.
        /// </summary>
        /// <param name="obj"></param>
        public async void doneBtn(object obj)
        {
            activityIndicator = true;
            popup = false;
            try
            {
                var dataGrid = obj as SfDataGrid;

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    var columnstatusdata = Settings.alllabeslvalues.Where(wh => wh.VersionID == Settings.VersionID && wh.LanguageID == Settings.LanguageID).ToList();

                    foreach (var statusitem in columnstatusdata)
                    {
                        foreach (var item in showColumns)
                        {
                            if (statusitem.FieldID == item.mappingText)
                            {

                                if (item.check == true)
                                {
                                    if (dataGrid.Columns[item.mappingText].IsHidden == true)
                                        dataGrid.Columns[item.mappingText].IsHidden = false;
                                }
                                else
                                {
                                    if (dataGrid.Columns[item.mappingText].IsHidden == false)
                                        dataGrid.Columns[item.mappingText].IsHidden = true;
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                }

                ObservableCollection<ColumnInfoSave> SaveColumns = new ObservableCollection<ColumnInfoSave>();

                foreach (var item in showColumns)
                {
                    SaveColumns.Add(new ColumnInfoSave() { Column = item.mappingText, Value = item.check == false ? false : true });
                }

                SaveUserDefaultSettingsModel defaultData = new SaveUserDefaultSettingsModel();
                defaultData.UserID = Settings.userLoginID;
                defaultData.VersionID = Settings.VersionID;
                defaultData.TagColumns = JsonConvert.SerializeObject(SaveColumns); //showColumns
                var responseData = await trackService.SaveUserPrioritySetting(defaultData);
                SearchDisable = true;
                RefreshDisable = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "doneBtn method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            activityIndicator = false;
        }

        /// <summary>
        /// This method is called when clicked on the settings icon from bottom menu.
        /// </summary>
        /// <param name="obj"></param>
        public async void profile_Tap(object obj)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                try
                {
                    activityIndicator = true;
                    Settings.mutipleTimeClick = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        await Navigation.PushAsync(new ProfileSelectionPage((int)QAType.PT));
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "profile_Tap method -> in PoDataViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method is called when clicked on close button from choose column popup.
        /// </summary>
        /// <param name="obj"></param>
        private void closeBtn(object obj)
        {
            popup = false;
            SearchDisable = true;
            RefreshDisable = true;
        }

        /// <summary>
        /// This method is called when clicked on Choose Columns.
        /// </summary>
        public void ChooseColumns()
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                try
                {
                    CheckToolBarPopUpHideOrShow();
                    Settings.mutipleTimeClick = true;

                    popup = true;
                    SearchDisable = false;
                    RefreshDisable = false;
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "ChooseColumns method -> in PoDataViewModel! " + Settings.userLoginID);
                    trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                }
            }
        }

        #region for each time 
        /// <summary>
        /// This method is called when clicked on camera icon form data grid, to view After Packing photo(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachCamA(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachCamA method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            if (!avoidMutiplePageOpen)
            {
                avoidMutiplePageOpen = true;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var allPo = obj as AllPoData;

                    if (allPo.imgCamOpacityA != 0.5)
                    {
                        activityIndicator = true;
                        try
                        {
                            if (allPo.cameImage == "Chatcamera.png")
                            {
                                Settings.AphotoCount = allPo.TagAPhotoCount;
                                Settings.currentPuId = allPo.PUID;
                                await Navigation.PushAsync(new PhotoUpload(null, allPo, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, allPo.photoTickVisible));
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamA method -> in PoDataViewModel! " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                        }
                        activityIndicator = false;
                    }
                }
                else
                {
                    activityIndicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                avoidMutiplePageOpen = false;
            }
        }

        /// <summary>
        /// This method is called when clicked on camera icon form data grid, to view Before Packing photo(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachCamB(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachCamB method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            if (!avoidMutiplePageOpen)
            {
                avoidMutiplePageOpen = true;
                var allPo = obj as AllPoData;
                bool checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    if (allPo.imgCamOpacityB != 0.5)
                    {
                        activityIndicator = true;
                        try
                        {
                            if (allPo.cameImage == "Chatcamera.png")
                            {
                                Settings.currentPuId = allPo.PUID;
                                Settings.BphotoCount = allPo.TagBPhotoCount;
                                await Navigation.PushAsync(new PhotoUpload(null, allPo, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, allPo.photoTickVisible));
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamB method -> in PoDataViewModel! " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                        }
                        activityIndicator = false;
                    }
                }
                else
                {
                    activityIndicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                avoidMutiplePageOpen = false;
            }
        }

        /// <summary>
        /// This method is called when clicked on chat icon form data grid, to view chat(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachMessage(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachMessage method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            activityIndicator = true;

            if (!avoidMutiplePageOpen)
            {
                avoidMutiplePageOpen = true;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    try
                    {
                        var allPo = obj as AllPoData;

                        if (allPo.chatImage != "minus.png")
                        {
                            await Navigation.PushAsync(new QnAlistPage(allPo.POID, allPo.POTagID, Settings.QAType));
                        }
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "tap_eachMessage method -> in PoDataViewModel! " + Settings.userLoginID);
                        var trackResult = await trackService.Handleexception(ex);
                    }
                }
                else
                {
                    activityIndicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                avoidMutiplePageOpen = false;
            }
            activityIndicator = false;
        }

        /// <summary>
        /// This method is called when clicked on attachment icon form data grid, to view uploaded file(s).
        /// </summary>
        /// <param name="obj"></param>
        public async void tap_eachFileUpload(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachFileUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            activityIndicator = true;

            if (!avoidMutiplePageOpen)
            {
                avoidMutiplePageOpen = true;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    try
                    {
                        var allPo = obj as AllPoData;

                        if (allPo.fileImage != "minus.png")
                        {
                            Settings.currentFuId = allPo.FUID;
                            Settings.FilesCount = allPo.TagFilesCount;
                            await Navigation.PushAsync(new FileUpload(null, allPo.POID, allPo.FUID, "fileUpload", allPo.fileTickVisible));
                        }
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "tap_eachFileUpload method -> in PoDataViewModel! " + Settings.userLoginID);
                        var trackResult = await trackService.Handleexception(ex);
                    }
                }
                else
                {
                    activityIndicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                avoidMutiplePageOpen = false;
                activityIndicator = false;
            }
        }
        #endregion

        #region for bottom bar 
        /// <summary>
        /// This method is called when clicked on camera icon from bottom menu, to start uploading photo(s).
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_InitialCamera(object sender)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in tap_InitialCamera method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        {
                            var dataGrid = sender as SfDataGrid;
                            var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                            var uniq = data.GroupBy(x => x.POShippingNumber);

                            if (uniq.Count() >= 2)
                            {
                                //Please select any one group.
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select tags under same po.");
                            }
                            else if (uniq.Count() == 0)
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s) to start upload photo(s).");
                            }
                            else if (uniq.Count() == 1)
                            {
                                var restricData = data.Where(r => r.cameImage == "cross.png").ToList();

                                if (restricData.Count > 0)
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Photos not required to upload for the selected tag(s).");
                                }
                                else
                                {
                                    PhotoUploadModel selectedTagsData = new PhotoUploadModel();

                                    foreach (var podata in uniq)
                                    {
                                        var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                        selectedTagsData.POID = d.POID;
                                        List<PhotoTag> lstdat = new List<PhotoTag>();

                                        foreach (var item in podata)
                                        {
                                            if (item.TagAPhotoCount == 0 && item.TagBPhotoCount == 0 && item.PUID == 0)
                                            {
                                                PhotoTag tg = new PhotoTag();

                                                if (item.POTagID != 0)
                                                {
                                                    tg.POTagID = item.POTagID;
                                                    Settings.Tagnumbers = item.TagNumber;
                                                    lstdat.Add(tg);
                                                }
                                            }
                                            else
                                            {
                                                selectedTagsData.alreadyExit = "alreadyExit";
                                                break;
                                            }
                                        }
                                        selectedTagsData.photoTags = lstdat;
                                        Settings.currentPoTagId_Inti = lstdat;
                                    }

                                    if (!String.IsNullOrEmpty(selectedTagsData.alreadyExit))
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("Photo upload is already started for the selected tag(s).");
                                    }
                                    else
                                    {
                                        if (selectedTagsData.photoTags.Count != 0)
                                        {
                                            await Navigation.PushAsync(new PhotoUpload(selectedTagsData, null, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false));
                                        }
                                        else
                                        {
                                            DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
                                        }
                                    }
                                }
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
                    YPSLogger.ReportException(ex, "tap_InitialCamera method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    activityIndicator = false;
                    Settings.mutipleTimeClick = false;
                }
            }
        }

        /// <summary>
        /// This method is called when clicked on chat icon from bottom menu, to start chat.
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_OnMessage(object sender)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in tap_OnMessage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        {
                            var dataGrid = sender as SfDataGrid;
                            var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                            var uniq = data.GroupBy(x => x.POShippingNumber);

                            if (uniq.Count() >= 2)
                            {
                                //Please select any one group.
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select tags from same po.");
                            }
                            else if (uniq.Count() == 0)
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s) to start conversation.");
                            }
                            else if (uniq.Count() == 1)
                            {
                                ChatData selectedTagsData = new ChatData();

                                foreach (var podata in uniq)
                                {
                                    var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                    selectedTagsData.POID = d.POID;
                                    List<Tag> lstdat = new List<Tag>();

                                    foreach (var item in podata)
                                    {
                                        Tag tg = new Tag();

                                        if (item.POTagID != 0)
                                        {
                                            tg.POTagID = item.POTagID;
                                            lstdat.Add(tg);
                                        }
                                    }
                                    selectedTagsData.tags = lstdat;
                                }
                                if (selectedTagsData.tags.Count != 0)
                                {
                                    Settings.ChatuserCountImgHide = 1;
                                    await Navigation.PushAsync(new ChatUsers(selectedTagsData, true));
                                }
                                else
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("No tags available");
                                }
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
                    YPSLogger.ReportException(ex, "tap_OnMessage method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method is called when clicked on file icon from bottom menu, to start uploading file(s).
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_InitialFileUpload(object sender)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in tap_InitialFileUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        {
                            try
                            {
                                var dataGrid = sender as SfDataGrid;
                                var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                                var uniq = data.GroupBy(x => x.POShippingNumber);

                                if (uniq.Count() >= 2)
                                {
                                    //Please select any one group.
                                    DependencyService.Get<IToastMessage>().ShortAlert("File upload is already marked as completed for the selected tag(s).");
                                }
                                else if (uniq.Count() == 0)
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s) to start upload file(s).");
                                }
                                else if (uniq.Count() == 1)
                                {
                                    StartUploadFileModel selectedTagsData = new StartUploadFileModel();
                                    foreach (var podata in uniq)
                                    {
                                        var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                        selectedTagsData.POID = d.POID;

                                        List<FileTag> lstdat = new List<FileTag>();
                                        foreach (var item in podata)
                                        {
                                            FileTag tg = new FileTag();
                                            if (item.TagFilesCount == 0 && item.FUID == 0)
                                            {
                                                if (item.POTagID != 0)
                                                {
                                                    tg.POTagID = item.POTagID;
                                                    lstdat.Add(tg);
                                                }
                                            }
                                            else
                                            {
                                                selectedTagsData.alreadyExit = "alreadyExit";
                                                break;
                                            }

                                        }
                                        selectedTagsData.FileTags = Settings.currentPoTagId_Inti_F = lstdat;
                                    }

                                    if (!String.IsNullOrEmpty(selectedTagsData.alreadyExit))
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("File upload is already started for the selected tag(s).");
                                    }
                                    else
                                    {
                                        if (selectedTagsData.FileTags.Count != 0)
                                        {
                                            await Navigation.PushAsync(new FileUpload(selectedTagsData, 0, 0, "initialFile", false));
                                        }
                                        else
                                        {
                                            DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                YPSLogger.ReportException(ex, "tap_InitialFileUpload method -> in PoDataViewModel! " + Settings.userLoginID);
                                var trackResult = await trackService.Handleexception(ex);
                            }
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_InitialFileUpload method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method is called when clicked on PrintTag icon from bottom menu.
        /// </summary>
        /// <param name="obj"></param>
        public async void tap_Printer(object obj)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in tap_Printer method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    activityIndicator = true;
                    Settings.mutipleTimeClick = true;

                    if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    {
                        if (Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                        {
                        }
                        else
                        {
                            var dataGrid = obj as SfDataGrid;
                            var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                            var uniq = data.GroupBy(x => x.POShippingNumber);

                            if (uniq.Count() >= 2)
                            {
                                //Please select any one group.
                                DependencyService.Get<IToastMessage>().ShortAlert("please select tag(s) under same po.");
                            }
                            else if (uniq.Count() == 0)
                            {
                                //Please select tag(s) to download the report
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s).");
                            }
                            else if (uniq.Count() == 1)
                            {
                                var checkInternet = await App.CheckInterNetConnection();
                                if (checkInternet)
                                {
                                    string poTagID = "";
                                    foreach (var podata in uniq)
                                    {
                                        var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                        foreach (var item in podata)
                                        {
                                            poTagID += item.POTagID + ",";
                                        }
                                    }
                                    poTagID = poTagID.TrimEnd(',');

                                    if (poTagID != "0")
                                    {
                                        YPSService pSService = new YPSService();
                                        var printResult = await pSService.PrintPDF(poTagID);

                                        PrintPDFModel printPDFModel = new PrintPDFModel();

                                        if (printResult.status != 0 && printResult != null)
                                        {
                                            var bArray = printResult.data;
                                            byte[] bytes = Convert.FromBase64String(bArray);
                                            printPDFModel.bArray = bytes;
                                            printPDFModel.FileName = "PrintTag" + "_" + String.Format("{0:yyyyMMMdd_hh-mm-ss}", DateTime.Now) + ".pdf";
                                            printPDFModel.PDFFileTitle = "Print Tag";

                                            switch (Device.RuntimePlatform)
                                            {
                                                case Device.iOS:
                                                    if (await FileManager.ExistsAsync(printPDFModel.FileName) == false)
                                                    {
                                                        await FileManager.GetByteArrayData(printPDFModel);
                                                    }

                                                    var url = FileManager.GetFilePathFromRoot(printPDFModel.FileName);
                                                    DependencyService.Get<NewOpenPdfI>().passPath(url);
                                                    break;
                                                case Device.Android:
                                                    await Navigation.PushAsync(new PdfViewPage(printPDFModel));
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong!");
                                        }
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
                                    }
                                }
                                else
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_Printer method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    activityIndicator = false;
                    Settings.mutipleTimeClick = false;
                }
            }
        }
        #endregion

        /// <summary>
        /// This method is called when clicked on search icon, there on ToolBar.
        /// </summary>
        public async void FilterClicked()
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in FilterClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (Settings.filterPageCount == 0)
                        {
                            Settings.filterPageCount = 1;
                            await Navigation.PushAsync(new FilterData());
                        }
                    }
                    else
                    {
                        activityIndicator = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "FilterClicked method-> in PoDataViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    activityIndicator = false;
                    Settings.mutipleTimeClick = false;
                }
            }
        }

        bool iscalled = false;

        /// <summary>
        /// This method is called when clicked on Refresh Page, from more options.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshPage()
        {

            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in FilterClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    CheckToolBarPopUpHideOrShow();
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    SearchDisable = false;
                    App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                    SearchDisable = true;
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "RefreshPage -> in PoDataViewModel.cs " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This methid is called when clicked on Archived Chat, from more options.
        /// </summary>
        public async void ArchivedChats()
        {
            try
            {
                CheckToolBarPopUpHideOrShow();
                await Navigation.PushAsync(new QnAlistPage(0, 0, (int)QAType.PT));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ArchivedChats method -> in PoDataViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        bool isloading = false;

        /// <summary>
        /// This method gets called when clicked on "Clear Search" label.
        /// </summary>
        /// <returns></returns>
        public async Task ClearSearch()
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in clearData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        await SaveAndClearSearch(false);

                        if (NoRecordsLbl == true)
                        {
                            pagename.BindGridData(false, false);
                        }
                        else if (NoRecordsLbl == false)
                        {
                            pagename.BindGridData(true, false);
                        }

                        isloading = true;
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "ClearSearch method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method is to clear the search criteria values and save to DB.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private async Task SaveAndClearSearch(bool val)
        {
            SendPodata SaveUserDS = new SendPodata();
            SearchPassData defaultData = new SearchPassData();
            try
            {
                //Key
                SaveUserDS.PONumber = Settings.PONumber = string.Empty;
                SaveUserDS.REQNo = Settings.REQNo = string.Empty;
                SaveUserDS.ShippingNo = Settings.ShippingNo = string.Empty;
                SaveUserDS.DisciplineID = Settings.DisciplineID = 0;
                SaveUserDS.ELevelID = Settings.ELevelID = 0;
                SaveUserDS.ConditionID = Settings.ConditionID = 0;
                SaveUserDS.ExpeditorID = Settings.ExpeditorID = 0;
                SaveUserDS.PriorityID = Settings.PriorityID = 0;
                SaveUserDS.TagNo = Settings.TAGNo = string.Empty;
                SaveUserDS.IdentCode = Settings.IdentCodeNo = string.Empty;
                SaveUserDS.BagNo = Settings.BagNo = string.Empty;
                defaultData.CompanyID = Settings.CompanyID;
                defaultData.UserID = Settings.userLoginID;
                defaultData.SearchCriteria = JsonConvert.SerializeObject(SaveUserDS);
                var responseData = await trackService.SaveSerchvaluesSetting(defaultData);

                if (val == true)
                {
                    SearchResultGet(SaveUserDS);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveAndClearSearch method -> in PoDataViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is for checking if the search value(s) has any value or is empty. 
        /// </summary>
        /// <returns></returns>
        public async Task CheckingSearchValues()
        {
            try
            {
                if ((!String.IsNullOrEmpty(Settings.PONumber) || !String.IsNullOrEmpty(Settings.REQNo) || !String.IsNullOrEmpty(Settings.ShippingNo) ||
                   Settings.DisciplineID != 0 || Settings.ELevelID != 0 || Settings.ConditionID != 0 || Settings.ExpeditorID != 0
                   || Settings.PriorityID != 0 || !String.IsNullOrEmpty(Settings.TAGNo) || !String.IsNullOrEmpty(Settings.IdentCodeNo) || !String.IsNullOrEmpty(Settings.BagNo)) && Settings.SearchWentWrong == false)
                {
                    ClearSearchLbl = true;
                }
                else if ((Settings.LocationPickupID != 0 || Settings.LocationPOLID != 0 || Settings.LocationPODID != 0 || Settings.LocationDeliverPlaceID != 0) && Settings.SearchWentWrong == false)
                {
                    ClearSearchLbl = true;
                }
                else if ((!String.IsNullOrEmpty(Settings.DeliveryFrom) || !String.IsNullOrEmpty(Settings.ETDFrom) || !String.IsNullOrEmpty(Settings.ETAFrom) ||
                    !String.IsNullOrEmpty(Settings.OnsiteFrom) || !String.IsNullOrEmpty(Settings.DeliveryTo) || !String.IsNullOrEmpty(Settings.ETDTo) ||
                    !String.IsNullOrEmpty(Settings.ETATo) || !String.IsNullOrEmpty(Settings.OnsiteTo)) && Settings.SearchWentWrong == false)
                {
                    ClearSearchLbl = true;
                }
                else
                {
                    ClearSearchLbl = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckingSearchValues method -> in PoDataViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        #region TagColumns Binding
        /// <summary>
        /// This method binds the tags columns to the grid. 
        /// </summary>
        /// <returns></returns>
        public async Task TagColumnsBinding()
        {
            try
            {
                var getpoData = await trackService.GetUserPrioritySettings(Settings.userLoginID);

                if (getpoData != null)
                {
                    if (getpoData.status != 0)
                    {
                        var searchTags = JsonConvert.DeserializeObject<List<ColumnInfo>>(getpoData.data.TagColumns);
                        Settings.VersionID = getpoData.data.VersionID;

                        if (searchTags != null)
                        {
                            ObservableCollection<ColumnInfo> columnsData = new ObservableCollection<ColumnInfo>();

                            foreach (var name in searchTags)
                            {
                                columnsData.Add(new ColumnInfo() { mappingText = name.Column, headerText = name.Column, Value = name.Value });
                                showColumns = columnsData;
                            }
                        }
                        else
                        {
                            showColumns = null;
                        }
                    }
                    else
                    {
                        showColumns = null;

                        if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0 && Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        {
                            pagename.GetBottomMenVal();
                        }
                    }
                }
                else
                {
                    showColumns = null;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TagColumnsBinding method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }
        #endregion

        /// <summary>
        /// This method gets the search result based on search values.
        /// </summary>
        /// <param name="sendPodata"></param>
        public async void SearchResultGet(SendPodata sendPodata)
        {
            try
            {
                var Serchdata = await trackService.GetSearchValuesService(Settings.userLoginID);

                if (Serchdata != null)
                {
                    if (Serchdata.status == 1)
                    {
                        if (!string.IsNullOrEmpty(Serchdata.data.SearchCriteria))
                        {
                            var searchC = JsonConvert.DeserializeObject<SendPodata>(Serchdata.data.SearchCriteria);

                            if (searchC != null)
                            {
                                //Key
                                sendPodata.PONumber = Settings.PONumber = searchC.PONumber;
                                sendPodata.REQNo = Settings.REQNo = searchC.REQNo;
                                sendPodata.ShippingNo = Settings.ShippingNo = searchC.ShippingNo;
                                sendPodata.DisciplineID = Settings.DisciplineID = searchC.DisciplineID;
                                sendPodata.ELevelID = Settings.ELevelID = searchC.ELevelID;
                                sendPodata.ConditionID = Settings.ConditionID = searchC.ConditionID;
                                sendPodata.ExpeditorID = Settings.ExpeditorID = searchC.ExpeditorID;
                                sendPodata.PriorityID = Settings.PriorityID = searchC.PriorityID;
                                sendPodata.TagNo = Settings.TAGNo = searchC.TagNo;
                                sendPodata.IdentCode = Settings.IdentCodeNo = searchC.IdentCode;
                                sendPodata.BagNo = Settings.BagNo = searchC.BagNo;
                                sendPodata.yBkgNumber = Settings.Ybkgnumber = searchC.yBkgNumber;

                                Settings.SearchWentWrong = false;
                            }

                        }
                        else
                        {
                            await SaveAndClearSearch(true);
                        }
                    }
                    else
                    {
                        Settings.SearchWentWrong = true;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SearchResultGet method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to remember user details.
        /// </summary>
        /// <returns></returns>
        public async Task RememberUserDetails()
        {
            try
            {
                RememberPwdDB Db = new RememberPwdDB();
                var user = Db.GetUserDetails();

                if (user.Count == 0)
                {
                    RememberPwd saveData = new RememberPwd();
                    saveData.encUserId = EncryptManager.Encrypt(Convert.ToString(Settings.userLoginID));
                    saveData.encLoginID = Settings.LoginID;
                    saveData.encUserRollID = EncryptManager.Encrypt(Convert.ToString(Settings.userRoleID));
                    saveData.encSessiontoken = Settings.Sessiontoken;
                    saveData.AndroidVersion = Settings.AndroidVersion;
                    saveData.iOSversion = Settings.iOSversion;
                    saveData.IIJEnable = Settings.IsIIJEnabled;
                    saveData.IsPNEnabled = Settings.IsPNEnabled;
                    saveData.IsEmailEnabled = Settings.IsEmailEnabled;
                    Db.SaveUserPWd(saveData);

                    #region selected profile
                    if (!String.IsNullOrEmpty(Settings.CompanySelected))
                    {
                        CompanyName = Settings.CompanySelected;
                    }

                    if (!String.IsNullOrEmpty(Settings.Projectelected) || !String.IsNullOrEmpty(Settings.JobSelected))
                    {
                        if (Settings.SupplierSelected == "ALL")
                        {
                            ProNjobName = Settings.Projectelected + "/" + Settings.JobSelected;
                        }
                        else
                        {
                            var pNjobName = Settings.Projectelected + "/" + Settings.JobSelected + "/" + Settings.SupplierSelected;
                            string trimpNjobName = pNjobName.TrimEnd('/');
                            ProNjobName = trimpNjobName;
                        }
                    }
                    #endregion
                }
                else
                {
                    var DBresponse = await trackService.GetSaveUserDefaultSettings(Settings.userLoginID);

                    if (DBresponse != null)
                    {
                        if (DBresponse.status == 1)
                        {
                            Settings.VersionID = DBresponse.data.VersionID;
                            CompanyName = Settings.CompanySelected = DBresponse.data.CompanyName;

                            if (DBresponse.data.SupplierName == "")
                            {
                                ProNjobName = DBresponse.data.ProjectName + "/" + DBresponse.data.JobNumber;
                                Settings.Projectelected = DBresponse.data.ProjectName;
                                Settings.JobSelected = DBresponse.data.JobNumber;
                                Settings.CompanyID = DBresponse.data.CompanyID;
                                Settings.ProjectID = DBresponse.data.ProjectID;
                                Settings.JobID = DBresponse.data.JobID;
                                Settings.SupplierSelected = DBresponse.data.SupplierName;
                                Settings.SupplierID = DBresponse.data.SupplierID;
                            }
                            else
                            {
                                ProNjobName = DBresponse.data.ProjectName + "/" + DBresponse.data.JobNumber + "/" + DBresponse.data.SupplierName;
                                Settings.Projectelected = DBresponse.data.ProjectName;
                                Settings.JobSelected = DBresponse.data.JobNumber;
                                Settings.CompanyID = DBresponse.data.CompanyID;
                                Settings.ProjectID = DBresponse.data.ProjectID;
                                Settings.JobID = DBresponse.data.JobID;
                                Settings.SupplierSelected = DBresponse.data.SupplierName;
                                Settings.SupplierID = DBresponse.data.SupplierID;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RememberUserDetails method -> in PoDataViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method get all label texts, used in the app.
        /// </summary>
        public async void GetallApplabels()
        {
            try
            {
                if (Settings.alllabeslvalues == null || Settings.alllabeslvalues.Count == 0)
                {
                    var lblResult = await trackService.GetallApplabelsService();

                    if (lblResult != null && lblResult.data != null)
                    {
                        Settings.alllabeslvalues = lblResult.data.ToList();
                        var datavalues = Settings.alllabeslvalues.Where(x => x.VersionID == Settings.VersionID && x.LanguageID == Settings.LanguageID).ToList();
                        Settings.Companylabel = datavalues.Where(x => x.FieldID == Settings.Companylabel1).Select(m => m.LblText).FirstOrDefault();
                        Settings.projectlabel = datavalues.Where(x => x.FieldID == Settings.projectlabel1).Select(x => x.LblText).FirstOrDefault();
                        Settings.joblabel = datavalues.Where(x => x.FieldID == Settings.joblabel1).Select(x => x.LblText).FirstOrDefault();
                        Settings.supplierlabel = datavalues.Where(x => x.FieldID == Settings.supplierlabel1).Select(x => x.LblText).FirstOrDefault();

                        if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        {
                            pagename.GetBottomMenVal();
                        }
                    }
                    else
                    {
                        //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                    }
                }
                else
                {
                    if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                    {
                        var datavalues = Settings.alllabeslvalues.Where(x => x.VersionID == Settings.VersionID && x.LanguageID == Settings.LanguageID).ToList();

                        Settings.Companylabel = datavalues.Where(x => x.FieldID == Settings.Companylabel1).Select(m => m.LblText).FirstOrDefault();
                        Settings.projectlabel = datavalues.Where(x => x.FieldID == Settings.projectlabel1).Select(x => x.LblText).FirstOrDefault();
                        Settings.joblabel = datavalues.Where(x => x.FieldID == Settings.joblabel1).Select(x => x.LblText).FirstOrDefault();
                        Settings.supplierlabel = datavalues.Where(x => x.FieldID == Settings.supplierlabel1).Select(x => x.LblText).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetallApplabels method -> in PoDataViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to show/hide the toolbar.
        /// </summary>
        public void CheckToolBarPopUpHideOrShow()
        {
            if (ToolBarItemPopUp == false)
            {
                MoreItemIconColor = Color.Gray;
                ToolBarItemPopUp = true;
            }
            else
            {
                MoreItemIconColor = Color.White;
                ToolBarItemPopUp = false;
            }
        }

        /// <summary>
        /// This method is for getting PN count.
        /// </summary>
        public async void GetPNCount()
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in GetPNCount method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var result = await trackService.GetNotifyHistory(Settings.userLoginID);

                    if (result != null)
                    {
                        if (result.status != 0)
                        {
                            if (result.data.Count() > 0)
                            {
                                NotifyCountTxt = result.data[0].listCount.ToString();
                                Settings.notifyCount = result.data[0].listCount;
                            }
                            else
                            {
                                NotifyCountTxt = "0";
                                Settings.notifyCount = 0;
                            }
                        }
                        else if (result.message == "No Data Found")
                        {
                            NotifyCountTxt = "0";
                            Settings.notifyCount = 0;
                        }
                    }
                }
                else
                {
                    // DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetPNCount method -> in PoDataViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
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
        public string _PageSizeFromDDL;
        public string PageSizeFromDDL
        {
            get => _PageSizeFromDDL;
            set
            {
                _PageSizeFromDDL = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<AllPoData> _PoData;
        public ObservableCollection<AllPoData> PoDataCollections
        {
            get { return _PoData; }
            set
            {
                _PoData = value;
                RaisePropertyChanged("PoDataCollections");
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

        private bool _Archivedchat = false;
        public bool Archivedchat
        {
            get { return _Archivedchat; }
            set
            {
                _Archivedchat = value;
                RaisePropertyChanged("Archivedchat");
            }
        }

        private bool _SearchDisable = false;
        public bool SearchDisable
        {
            get { return _SearchDisable; }
            set
            {
                _SearchDisable = value;
                RaisePropertyChanged("SearchDisable");
            }
        }

        private bool _profileSettingVisible = false;
        public bool profileSettingVisible
        {
            get { return _profileSettingVisible; }
            set
            {
                _profileSettingVisible = value;
                RaisePropertyChanged("profileSettingVisible");
            }
        }

        private bool _activityIndicator;
        public bool activityIndicator
        {
            get { return _activityIndicator; }
            set
            {
                _activityIndicator = value;
                RaisePropertyChanged("activityIndicator");
            }
        }

        private bool _popup = false;
        public bool popup
        {
            get { return _popup; }
            set
            {
                _popup = value;
                NotifyPropertyChanged();
            }
        }

        private bool _columnsStack = false;
        public bool columnsStack
        {
            get { return _columnsStack; }
            set
            {
                _columnsStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _mainStack = true;
        public bool mainStack
        {
            get { return _mainStack; }
            set
            {
                _mainStack = value;
                NotifyPropertyChanged();
            }
        }

        private string _boolImage = "unchecked_ic.png";
        public string boolImage
        {
            get { return _boolImage; }
            set
            {
                _boolImage = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ColumnInfo> _showColumns;
        public ObservableCollection<ColumnInfo> showColumns
        {
            get { return _showColumns; }
            set
            {
                _showColumns = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ColumnInfo> _showColumnsstatus;
        public ObservableCollection<ColumnInfo> showColumnsstatus
        {
            get { return _showColumnsstatus; }
            set
            {
                _showColumnsstatus = value;
                NotifyPropertyChanged();
            }
        }

        private bool _collapseVisible = true;
        public bool collapseVisible
        {
            get { return _collapseVisible; }
            set
            {
                _collapseVisible = value;
                NotifyPropertyChanged();
            }
        }

        private string _CompanyName;
        public string CompanyName
        {
            get { return _CompanyName; }
            set
            {
                _CompanyName = value;
                NotifyPropertyChanged();
            }
        }

        private string _ProNjobName;
        public string ProNjobName
        {
            get { return _ProNjobName; }
            set
            {
                _ProNjobName = value;
                NotifyPropertyChanged();
            }
        }

        private string _NotifyCountTxt;
        public string NotifyCountTxt
        {
            get { return _NotifyCountTxt; }
            set
            {
                _NotifyCountTxt = value;
                NotifyPropertyChanged();
            }
        }

        private bool _noRecordsLbl = false;
        public bool NoRecordsLbl
        {
            get { return _noRecordsLbl; }
            set
            {
                _noRecordsLbl = value;
                NotifyPropertyChanged();
            }
        }

        private int _PageCount;
        public int PageCount
        {
            get { return _PageCount; }
            set
            {
                _PageCount = value;
                RaisePropertyChanged("PageCount");
            }
        }

        private bool _DataGrid = false;
        public bool DataGrid
        {
            get { return _DataGrid; }
            set
            {
                _DataGrid = value;
                NotifyPropertyChanged();
            }
        }

        private bool _DataGridShowColumn = true;
        public bool DataGridShowColumn
        {
            get { return _DataGridShowColumn; }
            set
            {
                _DataGridShowColumn = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ClearSearchLbl = false;
        public bool ClearSearchLbl
        {
            get { return _ClearSearchLbl; }
            set
            {
                _ClearSearchLbl = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsShipPrinterhide = false;
        public bool IsShipPrinterhide
        {
            get { return _IsShipPrinterhide; }
            set
            {
                _IsShipPrinterhide = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsPNenable = false;
        public bool IsPNenable
        {
            get { return _IsPNenable; }
            set
            {
                _IsPNenable = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ToolBarItemPopUp = false;
        public bool ToolBarItemPopUp
        {
            get { return _ToolBarItemPopUp; }
            set
            {
                _ToolBarItemPopUp = value;
                NotifyPropertyChanged();
            }
        }

        private Color _MoreItemIconColor = Color.White;
        public Color MoreItemIconColor
        {
            get => _MoreItemIconColor;
            set
            {
                _MoreItemIconColor = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }

    public class ModelForIsPhotoRequired
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }
}
