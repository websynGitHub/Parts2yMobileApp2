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
using YPS.Views;
using static YPS.Models.ChatMessage;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class POChildListPageViewModel : IBase
    {
        YPSService trackService;
        public ICommand moveToPage { set; get; }
        public ICommand Backevnttapped { set; get; }
        public INavigation Navigation { get; set; }
        public ICommand viewExistingChats { get; set; }
        public ICommand viewExistingBUPhotos { get; set; }
        public ICommand viewExistingAUPhotos { get; set; }
        public ICommand viewExistingFiles { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand MoveToNextPageCmd { get; set; }
        public ICommand CheckBoxTapCmd { get; set; }
        public ICommand InProgressCmd { get; set; }
        public ICommand CompletedCmd { get; set; }
        public ICommand PendingCmd { get; set; }
        public ICommand AllCmd { set; get; }
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }
        public Command SelectTagItemCmd { set; get; }
        SendPodata sendPodata = new SendPodata();
        ObservableCollection<AllPoData> allPOTagData;
        int POID, TaskID;
        public static bool isalldone;

        public POChildListPageViewModel(INavigation _Navigation, ObservableCollection<AllPoData> potag, SendPodata sendpodata)
        {
            try
            {
                Navigation = _Navigation;
                sendPodata = sendpodata;
                trackService = new YPSService();

                if (potag != null)
                {
                    allPOTagData = new ObservableCollection<AllPoData>(potag);
                }

                Task.Run(() => PreparePoTagList(potag, -1)).Wait();
                moveToPage = new Command(RedirectToPage);
                viewExistingFiles = new Command(ViewUploadedFiles);
                viewExistingBUPhotos = new Command(tap_eachCamB);
                viewExistingAUPhotos = new Command(tap_eachCamA);
                viewExistingChats = new Command(ViewExistingChats);
                HomeCommand = new Command(HomeCommand_btn);
                CheckBoxTapCmd = new Command(CheckBoxTap);
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                InProgressCmd = new Command(async () => await InProgress_Tap());
                CompletedCmd = new Command(async () => await Complete_Tap());
                PendingCmd = new Command(async () => await Pending_Tap());
                AllCmd = new Command(async () => await All_Tap());
                SelectTagItemCmd = new Command(TagLongPessed);
                MoveToNextPageCmd = new Command(MoveToNextPage);
                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                PartsCmd = new Command(async () => await TabChange("parts"));
                LoadCmd = new Command(async () => await TabChange("load"));

                //if (Settings.VersionID == 4 || Settings.VersionID == 3)
                //{
                //    LoadTextColor = Color.Black;
                //}
                //else
                //{
                //    LoadTextColor = Color.Gray;
                //}
            }
            catch (Exception ex)
            {

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

                SelectedTagCountVisible = (SelectedTagCount = PoDataChildCollections.Where(wr => wr.IsChecked == true).ToList().Count()) > 0 ? true : false;
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
                    await Navigation.PopAsync();
                }
                else if (tabname == "load")
                {
                    if (Settings.VersionID == 2)
                    {
                        loadindicator = true;
                        await Task.Delay(1);
                        await Navigation.PushAsync(new CarrierInspectionQuestionsPage(allPOTagData, isalldone));
                    }
                    else
                    {
                        if (isalldone == true)
                        {
                            loadindicator = true;
                            await Task.Delay(1);
                            await Navigation.PushAsync(new LoadPage(PoDataChildCollections.FirstOrDefault(), sendPodata, isalldone));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
            }
            loadindicator = false;
        }

        public async Task Pending_Tap()
        {
            try
            {
                loadindicator = true;
                GetPoData result = await trackService.LoadPoDataService(sendPodata);
                POID = allPOTagData.Select(c => c.POID).FirstOrDefault();
                TaskID = allPOTagData.Select(c => c.TaskID).FirstOrDefault();

                //allPOTagData = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.TagTaskStatus == 0 && wr.POID == allPOTagData[0].POID));
                PreparePoTagList(new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.POID == POID && wr.TaskID == TaskID)), 0);

                PendingTabVisibility = true;
                CompleteTabVisibility = InProgressTabVisibility = AllTabVisibility = false;
                PendingTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = CompleteTabTextColor = AllTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {
                loadindicator = false;

            }
            loadindicator = false;

        }

        public async Task InProgress_Tap()
        {
            try
            {
                loadindicator = true;

                GetPoData result = await trackService.LoadPoDataService(sendPodata);
                POID = allPOTagData.Select(c => c.POID).FirstOrDefault();
                TaskID = allPOTagData.Select(c => c.TaskID).FirstOrDefault();

                //allPOTagData = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.TagTaskStatus == 1 && wr.POID == allPOTagData[0].POID));
                PreparePoTagList(new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.POID == POID && wr.TaskID == TaskID)), 1);

                InProgressTabVisibility = true;
                CompleteTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                InProgressTabTextColor = Settings.Bar_Background;
                CompleteTabTextColor = PendingTabTextColor = AllTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {
                loadindicator = false;

            }
            loadindicator = false;

        }

        public async Task Complete_Tap()
        {
            try
            {
                loadindicator = true;

                GetPoData result = await trackService.LoadPoDataService(sendPodata);
                POID = allPOTagData.Select(c => c.POID).FirstOrDefault();
                TaskID = allPOTagData.Select(c => c.TaskID).FirstOrDefault();

                //allPOTagData = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.TagTaskStatus == 2 && wr.POID == allPOTagData[0].POID));
                PreparePoTagList(new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.POID == POID && wr.TaskID == TaskID)), 2);

                CompleteTabVisibility = true;
                InProgressTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                CompleteTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = PendingTabTextColor = AllTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {
                loadindicator = false;

            }
            loadindicator = false;

        }

        public async Task All_Tap()
        {
            try
            {
                loadindicator = true;

                GetPoData result = await trackService.LoadPoDataService(sendPodata);
                POID = allPOTagData.Select(c => c.POID).FirstOrDefault();
                TaskID = allPOTagData.Select(c => c.TaskID).FirstOrDefault();

                PreparePoTagList(new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.POID == POID && wr.TaskID == TaskID)), -1);

                AllTabVisibility = true;
                InProgressTabVisibility = CompleteTabVisibility = PendingTabVisibility = false;
                AllTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = CompleteTabTextColor = PendingTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {
                loadindicator = false;

            }
            loadindicator = false;

        }

        public async Task<GetPoData> GetRefreshedData()
        {
            GetPoData result = new GetPoData();
            try
            {
                sendPodata.UserID = Settings.userLoginID;
                sendPodata.PageSize = Settings.pageSizeYPS;
                sendPodata.StartPage = Settings.startPageYPS;

                result = await trackService.LoadPoDataService(sendPodata);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async void CheckBoxTap(object sender)
        {
            try
            {
                var val = sender as AllPoData;
            }
            catch (Exception ex)
            {

            }
        }

        public async void MoveToNextPage(object sender)
        {
            try
            {
                var versionname = Settings.encryVersionID;
                var versionID = Settings.VersionID;

                if (Settings.VersionID != 5 && Settings.VersionID != 1)
                {
                    loadindicator = true;
                    POTagDetail = sender as AllPoData;

                    if (POTagDetail != null)
                    {
                        loadindicator = true;
                        POTagDetail.SelectedTagBorderColor = Settings.Bar_Background;
                        Settings.TagNumber = POTagDetail.TagNumber;

                        if ((Settings.VersionID == 4 ||
                            Settings.VersionID == 3) && isalldone == true)
                        {
                            await Navigation.PushAsync(new LoadPage(POTagDetail, sendPodata, isalldone));
                        }
                        else if (Settings.VersionID == 2)
                        {
                            await Navigation.PushAsync(new InspVerificationScanPage(POTagDetail, isalldone));
                            //await Navigation.PushAsync(new VinInspectQuestionsPage(POTagDetail, isalldone));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
            }
            loadindicator = false;
        }

        /// <summary>
        /// Gets called when clicked on "Home" icon, to move to home page
        /// </summary>
        /// <param name="obj"></param>
        private async void HomeCommand_btn(object obj)
        {
            try
            {
                //await Navigation.PopAsync(true);
                App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HomeCommand_btn method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }

        }

        public async Task UpdateTabCount(ObservableCollection<AllPoData> potaglist)
        {
            try
            {
                await ChangeLabel();

                #region Update status count & label text 
                PendingLabel = labelobj.Pending.Name = labelobj.Pending.Name + "(" + potaglist.Where(wr => wr.TagTaskStatus == 0).Count() + ")";
                ProgressLabel = labelobj.Inprogress.Name = labelobj.Inprogress.Name + "(" + potaglist.Where(wr => wr.TagTaskStatus == 1).Count() + ")";
                DoneLabel = labelobj.Completed.Name = labelobj.Completed.Name + "(" + potaglist.Where(wr => wr.TagTaskStatus == 2).Count() + ")";
                AllLabel = labelobj.All.Name = labelobj.All.Name + "(" + potaglist.Count + ")";
                #endregion Update status count

                isalldone = potaglist.Where(wr => wr.TagTaskStatus == 2).Count() == potaglist.Count ? true : false;
            }
            catch (Exception ex)
            {

            }
        }

        public async Task PreparePoTagList(ObservableCollection<AllPoData> potaglist, int tagTaskStatus)
        {
            try
            {
                loadindicator = true;


                if (potaglist != null && potaglist.Count > 0)
                {
                    await UpdateTabCount(potaglist);

                    if (tagTaskStatus == 0)
                    {
                        potaglist = new ObservableCollection<AllPoData>(potaglist.Where(wr => wr.TagTaskStatus == 0));
                    }
                    else if (tagTaskStatus == 1)
                    {
                        potaglist = new ObservableCollection<AllPoData>(potaglist.Where(wr => wr.TagTaskStatus == 1));
                    }
                    else if (tagTaskStatus == 2)
                    {
                        potaglist = new ObservableCollection<AllPoData>(potaglist.Where(wr => wr.TagTaskStatus == 2));
                    }

                    //else
                    //{
                    //    potaglist = new ObservableCollection<AllPoData>(potaglist.Where(wr => wr.POID == allPOTagData[0].POID));
                    //}

                    if (potaglist != null && potaglist.Count() > 0)
                    {
                        foreach (var data in potaglist)
                        {
                            if (data.TagNumber != null)
                            {
                                #region Chat
                                //if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.SuperUser || Settings.userRoleID == (int)UserRoles.SuperViewer)
                                //{
                                //    data.chatImage = "minus.png";

                                //    data.IsChatsVisible = false;
                                //}
                                //else
                                //{
                                if (data.TagQACount == 0)
                                {
                                    if (data.TagQAClosedCount > 0)
                                    {
                                        data.chatImage = "chatIcon.png";
                                        data.chatTickVisible = true;
                                    }
                                    else
                                    {
                                        data.chatImage = "minus.png";
                                    }

                                    data.IsChatsVisible = false;
                                }
                                else
                                {
                                    data.chatImage = "chatIcon.png";
                                    data.countVisible = true;
                                    data.IsChatsVisible = true;
                                }
                                //}
                                #endregion

                                #region Before Photo & After Photo
                                if (data.PUID == 0)
                                {
                                    if (data.IsPhotoRequired == 0)
                                    {
                                        data.cameImage = "cross.png";
                                        data.CameraIconColor = Color.Red;
                                        data.IsPhotosVisible = true;
                                        data.imgCamOpacityB = data.imgTickOpacityB = 0.5;
                                        data.imgCamOpacityA = data.imgtickOpacityA = 0.5;
                                    }
                                    else
                                    {
                                        data.cameImage = "minus.png";
                                        data.IsPhotosVisible = false;
                                    }

                                }
                                else
                                {
                                    if (data.ISPhotoClosed == 1)
                                    {
                                        data.cameImage = "Chatcamera.png";
                                        data.photoTickVisible = true;
                                        data.CameraIconColor = Color.Black;

                                        data.imgCamOpacityB = data.imgTickOpacityB = (data.TagBPhotoCount == 0) ? 0.5 : 1.0;
                                        data.imgCamOpacityA = data.imgtickOpacityA = (data.TagAPhotoCount == 0) ? 0.5 : 1.0;
                                    }
                                    else
                                    {
                                        data.cameImage = "Chatcamera.png";
                                        data.BPhotoCountVisible = true;
                                        data.APhotoCountVisible = true;
                                        data.CameraIconColor = Color.Black;
                                    }

                                    data.IsPhotosVisible = true;
                                }
                                #endregion

                                #region File upload 
                                if (data.TagFilesCount == 0 && data.FUID == 0)
                                {
                                    data.fileImage = "minus.png";

                                    data.IsFilesVisible = false;
                                }
                                else
                                {
                                    if (data.ISFileClosed > 0)
                                    {
                                        data.fileImage = "attachb.png";
                                        data.fileTickVisible = true;
                                    }
                                    else
                                    {
                                        data.fileImage = "attachb.png";
                                        data.filecountVisible = true;
                                    }

                                    data.IsFilesVisible = true;
                                }
                                #endregion

                                #region Status icon
                                if (data.TagTaskStatus == 0)
                                {
                                    data.TagTaskStatusIcon = Icons.Pending;
                                }
                                else if (data.TagTaskStatus == 1)
                                {
                                    data.TagTaskStatusIcon = Icons.Progress;
                                }
                                else
                                {
                                    data.TagTaskStatusIcon = Icons.Done;
                                }
                                #endregion Status icon
                            }
                            else if (data.TagNumber == null)
                            {
                                data.countVisible = false;
                                data.filecountVisible = false;
                                data.fileTickVisible = false;
                                data.chatTickVisible = false;
                                data.BPhotoCountVisible = false;
                                data.APhotoCountVisible = false;
                                data.photoTickVisible = false;
                                data.POS = null;
                                data.SUB = null;
                                IsStatusTabVisible = false;
                                //data.IS_POS = null;
                                //data.IS_SUB = null;
                                data.emptyCellValue = "No records to display";
                            }
                        }

                        PoDataChildCollections = new ObservableCollection<AllPoData>(potaglist.OrderBy(o => o.TagTaskStatus).ThenBy(tob => tob.TagNumber).ThenBy(tob => tob.IdentCode));
                        IsPOTagDataListVisible = true;
                        NoRecordsLbl = false;
                        IsStatusTabVisible = true;
                    }
                    else
                    {
                        NoRecordsLbl = true;
                        IsPOTagDataListVisible = false;
                        IsStatusTabVisible = true;
                    }
                }
                else
                {
                    await UpdateTabCount(potaglist);
                    NoRecordsLbl = true;
                    IsPOTagDataListVisible = false;
                    IsStatusTabVisible = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PreparePoTagList method -> in POChildListPageViewModel.cs" + Settings.userLoginID);
                await trackService.Handleexception(ex);
                loadindicator = false;
            }
            loadindicator = false;
        }

        public async Task Backevnttapped_click()
        {
            try
            {

                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
            }
        }

        public async void tap_InitialCamera(object sender)
        {
            try
            {
                if (!loadindicator)
                {
                    loadindicator = true;

                    YPSLogger.TrackEvent("PoDataViewModel", "in tap_InitialCamera method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        //{
                        var taglist = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                        var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                        //var data = taglist.SelectedItems.Cast<AllPoData>().ToList();
                        var uniq = data.GroupBy(x => x.POShippingNumber);
                        AllPoData potagdata = new AllPoData();

                        if (data != null && data.Count == 1)
                        {
                            potagdata = data[0];
                        }

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
                                    selectedTagsData.isCompleted = d.photoTickVisible;

                                    List<PhotoTag> lstdat = new List<PhotoTag>();

                                    foreach (var item in podata)
                                    {
                                        if (item.TagAPhotoCount == 0 && item.TagBPhotoCount == 0 && item.PUID == 0)
                                        {
                                            PhotoTag tg = new PhotoTag();

                                            if (item.POTagID != 0)
                                            {
                                                tg.POTagID = item.POTagID;
                                                tg.TaskID = item.TaskID;
                                                tg.TaskStatus = item.TaskStatus;
                                                tg.TagTaskStatus = item.TagTaskStatus;
                                                tg.TagNumber = item.TagNumber;
                                                tg.IdentCode = item.IdentCode;
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
                                        //if (selectedTagsData.photoTags.Count > 1)
                                        //{
                                        await Navigation.PushAsync(new PhotoUpload(selectedTagsData, potagdata, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false));
                                        //}
                                        //else
                                        //{
                                        //    await Navigation.PushAsync(new ScanPage((int)UploadTypeEnums.GoodsPhotos_BP, selectedTagsData, true, null));
                                        //}
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
                                    }
                                }
                            }
                        }
                        //}
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_InitialCamera method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
                loadindicator = false;
            }
            finally
            {
                loadindicator = false;
            }

        }

        public async void tap_OnMessage(object sender)
        {

            try
            {
                if (!loadindicator)
                {
                    YPSLogger.TrackEvent("PoDataViewModel", "in tap_OnMessage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    loadindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        //{
                        var taglist = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                        var data = taglist.Where(wr => wr.IsChecked == true).ToList();
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
                                        tg.TaskID = item.TaskID;
                                        tg.TaskStatus = item.TaskStatus;
                                        tg.TagTaskStatus = item.TagTaskStatus;
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
                        //}
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_OnMessage method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }

        }

        public async void tap_InitialFileUpload(object sender)
        {

            try
            {
                if (!loadindicator)
                {
                    YPSLogger.TrackEvent("PoDataViewModel", "in tap_InitialFileUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    loadindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        //{
                        try
                        {
                            var taglist = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                            var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                            //var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
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
                                                tg.TaskID = item.TaskID;
                                                tg.TaskStatus = item.TaskStatus;
                                                tg.TagTaskStatus = item.TagTaskStatus;
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
                        //}
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_InitialFileUpload method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async void MarkAsDone(object sender)
        {

            try
            {
                //if (!loadindicator)
                //{
                YPSLogger.TrackEvent("POChildListPageViewModel.cs", "in MarkAsDone method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                loadindicator = true;

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    //{
                    var val = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                    var selectedTagData = val.Where(wr => wr.IsChecked == true).ToList();

                    if ((selectedTagData.Where(wr => wr.TagTaskStatus == 2).Count()) > 0)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Some of the tag(s) are already marked as done.");
                    }
                    else if (selectedTagData.Count() == 0)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s) to mark as done.");
                    }
                    else
                    {
                        bool makeitdone = true;

                        if (Settings.VersionID == 2)
                        {
                            makeitdone = await App.Current.MainPage.DisplayAlert("Mark as done", "Make sure you have finished the inspection for selected tag(s)", "Ok", "Cancel");
                        }

                        if ((selectedTagData.Where(wr => wr.TaskID != 0 && wr.TagTaskStatus != 2).Count()) != 0 && makeitdone == true)
                        {
                            TagTaskStatus tagtaskstatus = new TagTaskStatus();
                            tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.Select(c => c.TaskID).FirstOrDefault().ToString());

                            List<string> EncPOTagID = new List<string>();

                            foreach (var data in selectedTagData)
                            {
                                var value = Helperclass.Encrypt(data.POTagID.ToString());
                                EncPOTagID.Add(value);
                            }
                            tagtaskstatus.POTagID = string.Join(",", EncPOTagID);
                            tagtaskstatus.Status = 2;
                            tagtaskstatus.CreatedBy = Settings.userLoginID;

                            var result = await trackService.UpdateTagTaskStatus(tagtaskstatus);

                            if (result.status == 1)
                            {
                                if ((selectedTagData.Where(wr => wr.TaskID != 0 && wr.TaskStatus != 1).Count()) != 0)
                                {
                                    TagTaskStatus taskstatus = new TagTaskStatus();
                                    taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.Select(c => c.TaskID).FirstOrDefault().ToString());
                                    taskstatus.TaskStatus = 1;
                                    taskstatus.CreatedBy = Settings.userLoginID;

                                    var taskval = await trackService.UpdateTaskStatus(taskstatus);
                                }
                                DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");


                            }

                            if (AllTabVisibility == true)
                            {
                                await All_Tap();
                            }
                            else if (CompleteTabVisibility == true)
                            {
                                await Complete_Tap();
                            }
                            else if (InProgressTabVisibility == true)
                            {
                                await InProgress_Tap();
                            }
                            else
                            {
                                await Pending_Tap();
                            }

                            if ((Settings.VersionID == 4 || Settings.VersionID == 3))
                            {
                                if (isalldone == true)
                                {
                                    LoadTextColor = Color.Black;
                                    MoveToNextPageCmd = new Command(MoveToNextPage);
                                }
                                else
                                {
                                    LoadTextColor = Color.Gray;
                                    MoveToNextPageCmd = null;
                                }
                            }

                            Settings.IsRefreshPartsPage = false;
                            SelectedTagCount = 0;
                            SelectedTagCountVisible = false;
                        }
                    }
                    //}
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                }
                //}
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "MarkAsDone method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async void tap_Printer(object obj)
        {
            try
            {
                if (!loadindicator)
                {
                    loadindicator = true;

                    YPSLogger.TrackEvent("PoDataViewModel", "in tap_Printer method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    //{
                    //    if (Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                    //    {
                    //    }
                    //    else
                    //    {
                    var taglist = (obj as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                    var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                    //var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
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
                    //}
                    //}
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_Printer method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }

        }

        public async void RedirectToPage(object movetopage)
        {
            loadindicator = true;
            try
            {
                var page = movetopage as ListView;

                //if (page.StyleId == "file")
                //{
                //    await App.Current.MainPage.Navigation.PushAsync(new FileUpload());
                //}
                //else if (page.StyleId == "photo")
                //{
                //    await Navigation.PushAsync(new Compare());
                //}
                //else
                //{

                //}
            }
            catch (Exception ex)
            {

            }
            loadindicator = false;
        }

        private async void tap_eachCamB(object obj)
        {
            try
            {
                YPSLogger.TrackEvent("POChildListPageViewModel", "in tap_eachCamB method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var potag = obj as AllPoData;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (potag.imgCamOpacityB != 0.5)
                    {
                        loadindicator = true;

                        try
                        {
                            if (potag.cameImage == "Chatcamera.png")
                            {
                                Settings.CanOpenScanner = false;
                                Settings.currentPuId = potag.PUID;
                                Settings.BphotoCount = potag.TagBPhotoCount;
                                //await Navigation.PushAsync(new ScanPage((int)UploadTypeEnums.GoodsPhotos_BP, null, false, potag));
                                await Navigation.PushAsync(new PhotoUpload(null, potag, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, potag.photoTickVisible));
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Photo upload has not yet been initiated, for this tag.");
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamB method -> in POChildListPageViewModel " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                        }
                        loadindicator = false;
                    }
                }
                else
                {
                    loadindicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
            }
            loadindicator = false;
        }

        /// <summary>
        /// This method is called when clicked on camera icon form data grid, to view After Packing photo(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachCamA(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachCamA method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            if (!loadindicator)
            {
                var allPo = obj as AllPoData;

                if (allPo.imgCamOpacityA != 0.5)
                {
                    loadindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {

                        try
                        {
                            if (allPo.cameImage == "Chatcamera.png")
                            {
                                Settings.CanOpenScanner = false;
                                Settings.AphotoCount = allPo.TagAPhotoCount;
                                Settings.currentPuId = allPo.PUID;
                                //await Navigation.PushAsync(new ScanPage((int)UploadTypeEnums.GoodsPhotos_AP, null, false, allPo));
                                await Navigation.PushAsync(new PhotoUpload(null, allPo, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, allPo.photoTickVisible));
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Photo upload has not yet been initiated, for this tag.");
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamA method -> in PoDataViewModel! " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                            loadindicator = false;
                        }
                        loadindicator = false;
                    }
                    else
                    {
                        loadindicator = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }

                loadindicator = false;
            }
        }

        private async void ViewExistingChats(object obj)
        {
            try
            {
                YPSLogger.TrackEvent("POChildListPageViewModel", "in ViewExistingChats method " + DateTime.Now + " UserId: " + Settings.userLoginID);
                loadindicator = true;

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
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Chat has not yet been initiated, for this tag.");
                        }
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "ViewExistingChats method -> in POChildListPageViewModel " + Settings.userLoginID);
                        var trackResult = await trackService.Handleexception(ex);
                    }
                }
                else
                {
                    loadindicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
            }
            loadindicator = false;
        }

        public async void ViewUploadedFiles(object obj)
        {
            try
            {
                YPSLogger.TrackEvent("POChildListPageViewModel", "in ViewUploadedFiles method " + DateTime.Now + " UserId: " + Settings.userLoginID);

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
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("File upload has not yet been initiated, for this tag.");
                        }
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "ViewUploadedFiles method -> in POChildListPageViewModel" + Settings.userLoginID);
                        var trackResult = await trackService.Handleexception(ex);
                    }
                }
                else
                {
                    loadindicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                loadindicator = false;
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// This is for changing the labels dynamically
        /// </summary>
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
                        //Getting Label values & Status based on FieldID
                        var poid = labelval.Where(wr => wr.FieldID == labelobj.POID.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID == labelobj.ShippingNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqnumber = labelval.Where(wr => wr.FieldID == labelobj.REQNo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID == labelobj.TaskName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var resource = labelval.Where(wr => wr.FieldID == labelobj.Resource.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var tagnumber = labelval.Where(wr => wr.FieldID == labelobj.TagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID == labelobj.IdentCode.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bagnumber = labelval.Where(wr => wr.FieldID == labelobj.BagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID == labelobj.ConditionName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var pending = labelval.Where(wr => wr.FieldID == labelobj.Pending.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var inprogress = labelval.Where(wr => wr.FieldID == labelobj.Inprogress.Name.Trim().Replace(" ", "")).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var complete = labelval.Where(wr => wr.FieldID == labelobj.Completed.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var all = labelval.Where(wr => wr.FieldID == labelobj.All.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var home = labelval.Where(wr => wr.FieldID == labelobj.Home.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var jobs = labelval.Where(wr => wr.FieldID == labelobj.Jobs.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var parts = labelval.Where(wr => wr.FieldID == labelobj.Parts.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var load = labelval.Where(wr => wr.FieldID == labelobj.Load.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid == null ? true : (poid.Status == 1 ? true : false);
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber == null ? true : (shippingnumber.Status == 1 ? true : false);
                        labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                        labelobj.REQNo.Status = reqnumber == null ? true : (reqnumber.Status == 1 ? true : false);
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme == null ? true : (taskanme.Status == 1 ? true : false);
                        labelobj.Resource.Name = (resource != null ? (!string.IsNullOrEmpty(resource.LblText) ? resource.LblText : labelobj.Resource.Name) : labelobj.Resource.Name) + " :";


                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber == null ? true : (tagnumber.Status == 1 ? true : false);
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode == null ? true : (identcode.Status == 1 ? true : false);
                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                        labelobj.BagNumber.Status = bagnumber == null ? true : (bagnumber.Status == 1 ? true : false);
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname == null ? true : (conditionname.Status == 1 ? true : false);

                        labelobj.Pending.Name = (pending != null ? (!string.IsNullOrEmpty(pending.LblText) ? pending.LblText : labelobj.Pending.Name) : labelobj.Pending.Name) + "\n";
                        labelobj.Pending.Status = pending == null ? true : (pending.Status == 1 ? true : false);
                        labelobj.Inprogress.Name = (inprogress != null ? (!string.IsNullOrEmpty(inprogress.LblText) ? inprogress.LblText : labelobj.Inprogress.Name) : labelobj.Inprogress.Name) + "\n";
                        labelobj.Inprogress.Status = inprogress == null ? true : (inprogress.Status == 1 ? true : false);
                        labelobj.Completed.Name = (complete != null ? (!string.IsNullOrEmpty(complete.LblText) ? complete.LblText : labelobj.Completed.Name) : labelobj.Completed.Name) + "\n";
                        labelobj.Completed.Status = complete == null ? true : (complete.Status == 1 ? true : false);
                        labelobj.All.Name = (all != null ? (!string.IsNullOrEmpty(all.LblText) ? all.LblText : labelobj.All.Name) : labelobj.All.Name) + "\n";
                        labelobj.All.Status = all == null ? true : (all.Status == 1 ? true : false);

                        labelobj.Home.Name = (home != null ? (!string.IsNullOrEmpty(home.LblText) ? home.LblText : labelobj.Home.Name) : labelobj.Home.Name);
                        labelobj.Home.Status = home == null ? true : (home.Status == 1 ? true : false);
                        //labelobj.Jobs.Name = (jobs != null ? (!string.IsNullOrEmpty(jobs.LblText) ? jobs.LblText : labelobj.Jobs.Name) : labelobj.Jobs.Name);
                        //labelobj.Jobs.Status = jobs == null ? true : (jobs.Status == 1 ? true : false);
                        //labelobj.Parts.Name = (parts != null ? (!string.IsNullOrEmpty(parts.LblText) ? parts.LblText : labelobj.Parts.Name) : labelobj.Parts.Name);
                        labelobj.Parts.Status = parts == null ? true : (parts.Status == 1 ? true : false);
                        //labelobj.Load.Name = Settings.CompanySelected.Contains("(C)") == true ? "Insp" : "Load";
                        labelobj.Load.Status = load == null ? true : (load.Status == 1 ? true : false);

                        labelobj.Load.Name = Settings.VersionID == 2 ? "Carrier" : "Load";
                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabelAndShowHide method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        #region Properties
        public string _SelectedPartsNo = Settings.VersionID == 2 ? "VIN(s) selected" : (Settings.VersionID == 3 || Settings.VersionID == 4) ? "Part(s) selected" : "";
        public string SelectedPartsNo
        {
            get => _SelectedPartsNo;
            set
            {
                _SelectedPartsNo = value;
                RaisePropertyChanged("SelectedPartsNo");
            }
        }

        private bool _IsLoadTabVisible { set; get; }
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

        public Color _LoadTextColor = ((Settings.VersionID == 4 || Settings.VersionID == 3) && isalldone == true) == true ? Color.Black : Color.Gray;
        public Color LoadTextColor
        {
            get => _LoadTextColor;
            set
            {
                _LoadTextColor = value;
                RaisePropertyChanged("LoadTextColor");
            }
        }

        public string _PendingLabel;
        public string PendingLabel
        {
            get => _PendingLabel;
            set
            {
                _PendingLabel = value;
                RaisePropertyChanged("PendingLabel");
            }
        }

        public string _ProgressLabel;
        public string ProgressLabel
        {
            get => _ProgressLabel;
            set
            {
                _ProgressLabel = value;
                RaisePropertyChanged("ProgressLabel");
            }
        }

        public string _DoneLabel;
        public string DoneLabel
        {
            get => _DoneLabel;
            set
            {
                _DoneLabel = value;
                RaisePropertyChanged("DoneLabel");
            }
        }

        public string _AllLabel;
        public string AllLabel
        {
            get => _AllLabel;
            set
            {
                _AllLabel = value;
                RaisePropertyChanged("AllLabel");
            }
        }

        private bool _InProgressTabVisibility = false;
        public bool InProgressTabVisibility
        {
            get => _InProgressTabVisibility;
            set
            {
                _InProgressTabVisibility = value;
                RaisePropertyChanged("InProgressTabVisibility");
            }
        }

        private bool _CompleteTabVisibility = false;
        public bool CompleteTabVisibility
        {
            get => _CompleteTabVisibility;
            set
            {
                _CompleteTabVisibility = value;
                RaisePropertyChanged("CompleteTabVisibility");
            }
        }

        private bool _PendingTabVisibility = false;
        public bool PendingTabVisibility
        {
            get => _PendingTabVisibility;
            set
            {
                _PendingTabVisibility = value;
                RaisePropertyChanged("PendingTabVisibility");
            }
        }

        private bool _AllTabVisibility = true;
        public bool AllTabVisibility
        {
            get => _AllTabVisibility;
            set
            {
                _AllTabVisibility = value;
                RaisePropertyChanged("AllTabVisibility");
            }
        }

        private string _Cmptext = "COMPLETED";
        public string Cmptext
        {
            get => _Cmptext;
            set
            {
                _Cmptext = value;
                RaisePropertyChanged("Cmptext");
            }
        }

        private string _Pendingtext = "PENDING";
        public string Pendingtext
        {
            get => _Pendingtext;
            set
            {
                _Pendingtext = value;
                RaisePropertyChanged("Pendingtext");
            }
        }

        private string _INPtext = "IN PROGRESS";
        public string INPtext
        {
            get => _INPtext;
            set
            {
                _INPtext = value;
                RaisePropertyChanged("INPtext");
            }
        }

        private string _Alltext = "ALL \n";
        public string Alltext
        {
            get => _Alltext;
            set
            {
                _Alltext = value;
                RaisePropertyChanged("Alltext");
            }
        }

        private Color _PendingTabTextColor = Color.Black;
        public Color PendingTabTextColor
        {
            get => _PendingTabTextColor;
            set
            {
                _PendingTabTextColor = value;
                RaisePropertyChanged("PendingTabTextColor");
            }
        }

        private Color _InProgressTabTextColor = Color.Black;
        public Color InProgressTabTextColor
        {
            get => _InProgressTabTextColor;
            set
            {
                _InProgressTabTextColor = value;
                RaisePropertyChanged("InProgressTabTextColor");
            }
        }
        private Color _CompleteTabTextColor = Color.Black;
        public Color CompleteTabTextColor
        {
            get => _CompleteTabTextColor;
            set
            {
                _CompleteTabTextColor = value;
                RaisePropertyChanged("CompleteTabTextColor");
            }
        }
        private Color _AllTabTextColor = Settings.Bar_Background;
        public Color AllTabTextColor
        {
            get => _AllTabTextColor;
            set
            {
                _AllTabTextColor = value;
                RaisePropertyChanged("AllTabTextColor");
            }
        }
        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields POID { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "PONumber"
            };
            public DashboardLabelFields REQNo { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "REQNo"
            };
            public DashboardLabelFields ShippingNumber { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "ShippingNumber"
            };
            public DashboardLabelFields TagNumber { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TagNumber"
            };
            public DashboardLabelFields TaskName { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TaskName"
            };

            public DashboardLabelFields Resource { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "Resource"
            };

            public DashboardLabelFields IdentCode { get; set; } = new DashboardLabelFields { Status = true, Name = "IdentCode" };
            public DashboardLabelFields BagNumber { get; set; } = new DashboardLabelFields { Status = true, Name = "BagNumber" };
            public DashboardLabelFields ConditionName { get; set; } = new DashboardLabelFields { Status = true, Name = "ConditionName" };

            public DashboardLabelFields Pending { get; set; } = new DashboardLabelFields { Status = true, Name = "Pending" };
            public DashboardLabelFields Inprogress { get; set; } = new DashboardLabelFields { Status = true, Name = "Progress" };
            public DashboardLabelFields Completed { get; set; } = new DashboardLabelFields { Status = true, Name = "Done" };
            public DashboardLabelFields All { get; set; } = new DashboardLabelFields { Status = true, Name = "All" };

            public DashboardLabelFields Home { get; set; } = new DashboardLabelFields { Status = true, Name = "Home" };
            public DashboardLabelFields Jobs { get; set; } = new DashboardLabelFields { Status = true, Name = "Job" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "Parts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "Load" };
        }
        public class DashboardLabelFields : IBase
        {
            //public bool Status { get; set; }
            //public string Name { get; set; }

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

        private AllPoData _POTagDetail;
        public AllPoData POTagDetail
        {
            get => _POTagDetail;
            set
            {
                _POTagDetail = value;
                //MoveToScan();
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

        private Color _LoadTabTxtColor = Color.Gray;
        public Color LoadTabTxtColor
        {
            get => _LoadTabTxtColor;
            set
            {
                _LoadTabTxtColor = value;
                RaisePropertyChanged("LoadTabTxtColor");
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

        private ObservableCollection<AllPoData> _PoDataChild;
        public ObservableCollection<AllPoData> PoDataChildCollections
        {
            get { return _PoDataChild; }
            set
            {
                _PoDataChild = value;
                if (value != null && value.Count > 0)
                {
                    PONumber = value[0].PONumber;
                    ShippingNumber = value[0].ShippingNumber;
                    REQNo = value[0].REQNo;
                    TaskName = value[0].TaskName;
                    Resource = value[0].TaskResourceName;
                    IsResourcecVisible = value[0].TaskResourceID == Settings.userLoginID ? false : true;

                }
                RaisePropertyChanged("PoDataChildCollections");
            }
        }

        private bool _IsResourcecVisible;
        public bool IsResourcecVisible
        {
            get { return _IsResourcecVisible; }
            set
            {
                _IsResourcecVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsPOTagDataListVisible;
        public bool IsPOTagDataListVisible
        {
            get { return _IsPOTagDataListVisible; }
            set
            {
                _IsPOTagDataListVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsStatusTabVisible;
        public bool IsStatusTabVisible
        {
            get { return _IsStatusTabVisible; }
            set
            {
                _IsStatusTabVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _noRecordsLbl;
        public bool NoRecordsLbl
        {
            get { return _noRecordsLbl; }
            set
            {
                _noRecordsLbl = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsRightArrowVisible = true;
        public bool IsRightArrowVisible
        {
            get => _IsRightArrowVisible; set
            {
                _IsRightArrowVisible = value;
                RaisePropertyChanged("IsRightArrowVisible");
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
        #endregion Properties End
    }
}
