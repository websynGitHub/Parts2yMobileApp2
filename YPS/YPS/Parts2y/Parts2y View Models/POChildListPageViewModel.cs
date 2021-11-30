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
        public ICommand InProgressCmd { get; set; }
        public ICommand CompletedCmd { get; set; }
        public ICommand PendingCmd { get; set; }
        public ICommand AllCmd { set; get; }
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }
        public Command SelectTagItemCmd { set; get; }
        public Command ScanCmd { set; get; }
        public Command InspCmd { set; get; }
        SendPodata sendPodata = new SendPodata();
        public ObservableCollection<AllPoData> allPOTagData;
        POChildListPage pageName;
        int POID, TaskID;
        public static bool isalldone;

        public POChildListPageViewModel(INavigation _Navigation, ObservableCollection<AllPoData> potag, SendPodata sendpodata, POChildListPage pagename)
        {
            try
            {
                Navigation = _Navigation;
                sendPodata = sendpodata;
                pageName = pagename;
                trackService = new YPSService();

                if (potag != null)
                {
                    allPOTagData = new ObservableCollection<AllPoData>(potag);
                }

                PreparePoTagList(potag, -1);
                viewExistingFiles = new Command(ViewUploadedFiles);
                viewExistingBUPhotos = new Command(tap_eachCamB);
                viewExistingAUPhotos = new Command(tap_eachCamA);
                viewExistingChats = new Command(ViewExistingChats);
                HomeCommand = new Command(HomeCommand_btn);
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
                ScanCmd = new Command(async () => await ScanOrInsp("scan"));
                InspCmd = new Command(async () => await ScanOrInsp("insp"));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "POChildListPageViewModel constructor -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        private void TagLongPessed(object sender)
        {
            try
            {
                if (SelectedTagCount == 0)
                {
                    PoDataChildCollections.Where(wr => wr.SelectedTagBorderColor == BgColor).ToList().ForEach(l => { l.SelectedTagBorderColor = Color.Transparent; l.IsChecked = false; });
                }

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
                YPSLogger.ReportException(ex, "TagLongPessed method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task TabChange(string tabname)
        {
            try
            {
                if (tabname == "home")
                {
                    loadindicator = true;
                    await Navigation.PopToRootAsync(false);
                }
                else if (tabname == "job")
                {
                    loadindicator = true;
                    await Navigation.PopAsync(false);
                }
                else if (tabname == "load")
                {
                    if (Settings.VersionID == 1)
                    {
                        loadindicator = true;
                        await Navigation.PushAsync(new ELoadInspectionQuestionsPage(allPOTagData, isalldone), false);
                    }
                    else if (Settings.VersionID == 2)
                    {
                        loadindicator = true;
                        await Navigation.PushAsync(new CLoadInspectionQuestionsPage(allPOTagData, isalldone), false);
                    }
                    else if (Settings.VersionID == 3)
                    {
                        loadindicator = true;
                        await Navigation.PushAsync(new KRLoadInspectionQuestionsPage(allPOTagData, isalldone), false);
                    }
                    else if (Settings.VersionID == 4)
                    {
                        loadindicator = true;
                        await Navigation.PushAsync(new KPLoadInspectionQuestionPage(allPOTagData, isalldone), false);
                    }
                    else if (Settings.VersionID == 5)
                    {
                        loadindicator = true;
                        await Navigation.PushAsync(new PLoadInspectionQuestionsPage(allPOTagData, isalldone), false);
                    }
                    else
                    {
                        if (isalldone == true)
                        {
                            loadindicator = true;
                            await Navigation.PushAsync(new LoadPage(PoDataChildCollections.FirstOrDefault(), sendPodata, isalldone), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                YPSLogger.ReportException(ex, "TabChange method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task Pending_Tap()
        {
            try
            {
                loadindicator = true;

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    GetPoData result = await trackService.LoadPoDataService(sendPodata);
                    POID = (int)allPOTagData?.Select(c => c.POID).FirstOrDefault();
                    TaskID = (int)allPOTagData?.Select(c => c.TaskID).FirstOrDefault();

                    if (result?.data?.allPoDataMobile?.Count > 0)
                    {
                        PreparePoTagList(new ObservableCollection<AllPoData>(result?.data?.allPoDataMobile.Where(wr => wr.TaskID == TaskID)), 0);
                    }

                    PendingTabVisibility = true;
                    CompleteTabVisibility = InProgressTabVisibility = AllTabVisibility = false;
                    PendingTabTextColor = Settings.Bar_Background;
                    InProgressTabTextColor = CompleteTabTextColor = AllTabTextColor = Color.Black;
                    SelectedTagCountVisible = false;
                    SelectedTagCount = 0;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                YPSLogger.ReportException(ex, "Pending_Tap method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }

        }

        public async Task InProgress_Tap()
        {
            try
            {
                loadindicator = true;

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    GetPoData result = await trackService.LoadPoDataService(sendPodata);
                    POID = (int)allPOTagData?.Select(c => c.POID).FirstOrDefault();
                    TaskID = (int)allPOTagData?.Select(c => c.TaskID).FirstOrDefault();

                    if (result?.data?.allPoDataMobile?.Count > 0)
                    {
                        PreparePoTagList(new ObservableCollection<AllPoData>(result?.data?.allPoDataMobile.Where(wr => wr.TaskID == TaskID)), 1);
                    }

                    InProgressTabVisibility = true;
                    CompleteTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                    InProgressTabTextColor = Settings.Bar_Background;
                    CompleteTabTextColor = PendingTabTextColor = AllTabTextColor = Color.Black;
                    SelectedTagCountVisible = false;
                    SelectedTagCount = 0;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                YPSLogger.ReportException(ex, "InProgress_Tap method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }

        }

        public async Task Complete_Tap()
        {
            try
            {
                loadindicator = true;

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    GetPoData result = await trackService.LoadPoDataService(sendPodata);
                    POID = (int)allPOTagData?.Select(c => c.POID).FirstOrDefault();
                    TaskID = (int)allPOTagData?.Select(c => c.TaskID).FirstOrDefault();

                    if (result?.data?.allPoDataMobile?.Count > 0)
                    {
                        PreparePoTagList(new ObservableCollection<AllPoData>(result?.data?.allPoDataMobile.Where(wr => wr.TaskID == TaskID)), 2);
                    }

                    CompleteTabVisibility = true;
                    InProgressTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                    CompleteTabTextColor = Settings.Bar_Background;
                    InProgressTabTextColor = PendingTabTextColor = AllTabTextColor = Color.Black;
                    SelectedTagCountVisible = false;
                    SelectedTagCount = 0;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                YPSLogger.ReportException(ex, "Complete_Tap method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }

        }

        public async Task All_Tap()
        {
            try
            {
                loadindicator = true;

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    GetPoData result = await trackService.LoadPoDataService(sendPodata);
                    POID = (int)allPOTagData?.Select(c => c.POID).FirstOrDefault();
                    TaskID = (int)allPOTagData?.Select(c => c.TaskID).FirstOrDefault();

                    if (result?.data?.allPoDataMobile?.Count > 0)
                    {
                        PreparePoTagList(new ObservableCollection<AllPoData>(result.data.allPoDataMobile.Where(wr => wr.TaskID == TaskID)), -1);
                    }

                    AllTabVisibility = true;
                    InProgressTabVisibility = CompleteTabVisibility = PendingTabVisibility = false;
                    AllTabTextColor = Settings.Bar_Background;
                    InProgressTabTextColor = CompleteTabTextColor = PendingTabTextColor = Color.Black;
                    SelectedTagCountVisible = false;
                    SelectedTagCount = 0;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                YPSLogger.ReportException(ex, "All_Tap method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }

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
                YPSLogger.ReportException(ex, "GetRefreshedData method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            return result;
        }

        public async Task ScanOrInsp(string clicktype)
        {
            try
            {
                loadindicator = true;

                if (POTagDetail != null)
                {
                    loadindicator = true;
                    IsScanOrInspPopUpVisible = false;

                    if (clicktype.Trim().ToLower() == "scan".Trim().ToLower())
                    {
                        await Navigation.PushAsync(new InspVerificationScanPage(POTagDetail, isalldone), false);

                    }
                    else
                    {
                        if (Settings.VersionID == 1)
                        {
                            await Navigation.PushAsync(new EPartsInspectionQuestionsPage(POTagDetail, isalldone), false);
                        }
                        else if (Settings.VersionID == 2)
                        {
                            await Navigation.PushAsync(new CVinInspectQuestionsPage(POTagDetail, isalldone), false);
                        }
                        else if (Settings.VersionID == 3)
                        {
                            await Navigation.PushAsync(new KRPartsInspectionQuestionsPage(POTagDetail, isalldone), false);
                        }
                        else if (Settings.VersionID == 4)
                        {
                            await Navigation.PushAsync(new KPPartsInspectionQuestionPage(POTagDetail, isalldone), false);
                        }
                        else if (Settings.VersionID == 5)
                        {
                            await Navigation.PushAsync(new PPartsInspectionQuestionsPage(POTagDetail, isalldone), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                YPSLogger.ReportException(ex, "ScanOrInsp method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async void MoveToNextPage(object sender)
        {
            try
            {
                loadindicator = true;

                var versionname = Settings.encryVersionID;
                var versionID = Settings.VersionID;

                if (Settings.VersionID == 4 || Settings.VersionID == 3 || Settings.VersionID == 1 || Settings.VersionID == 5
                    || Settings.VersionID == 2)
                {
                    loadindicator = true;
                    SelectedTagCountVisible = false;
                    SelectedTagCount = 0;
                    PoDataChildCollections.Where(wr => wr.SelectedTagBorderColor == BgColor).ToList().ForEach(l => { l.SelectedTagBorderColor = Color.Transparent; l.IsChecked = false; });
                    POTagDetail = sender as AllPoData;
                    POTagDetail.SelectedTagBorderColor = Settings.Bar_Background;
                    TagNumber = Settings.TagNumber = POTagDetail.TagNumber;
                    IsScanOrInspPopUpVisible = true;
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
                YPSLogger.ReportException(ex, "MoveToNextPage method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on "Home" icon, to move to home page
        /// </summary>
        /// <param name="obj"></param>
        private async void HomeCommand_btn(object obj)
        {
            try
            {
                loadindicator = true;
                await Navigation.PopToRootAsync(false);
                //App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HomeCommand_btn method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = true;
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
                YPSLogger.ReportException(ex, "UpdateTabCount method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        public async Task PreparePoTagList(ObservableCollection<AllPoData> potaglist, int tagTaskStatus)
        {
            try
            {
                loadindicator = true;


                if (potaglist != null && potaglist.Count > 0)
                {
                    allPOTagData = potaglist;
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

                    if (potaglist != null && potaglist.Count() > 0)
                    {
                        foreach (var data in potaglist)
                        {
                            if (data.TagNumber != null)
                            {
                                #region Chat
                                if (data.TagQACount == 0)
                                {
                                    if (data.TagQAClosedCount > 0)
                                    {
                                        data.chatTickVisible = true;
                                    }

                                    data.IsChatsVisible = false;
                                }
                                else
                                {
                                    data.countVisible = true;
                                    data.IsChatsVisible = true;
                                }
                                #endregion

                                #region Before Photo & After Photo
                                if (data.PUID == 0)
                                {
                                    if (data.IsPhotoRequired == 0)
                                    {
                                        data.CameraIconColor = Color.Red;
                                        data.IsPhotosVisible = true;
                                        data.imgCamOpacityB = data.imgTickOpacityB = 0.5;
                                        data.imgCamOpacityA = data.imgtickOpacityA = 0.5;
                                    }
                                    else
                                    {
                                        data.IsPhotosVisible = false;
                                    }
                                }
                                else
                                {
                                    if (data.ISPhotoClosed == 1)
                                    {
                                        data.photoTickVisible = true;
                                        data.CameraIconColor = Color.Black;
                                        data.imgCamOpacityB = data.imgTickOpacityB = (data.TagBPhotoCount == 0) ? 0.5 : 1.0;
                                        data.imgCamOpacityA = data.imgtickOpacityA = (data.TagAPhotoCount == 0) ? 0.5 : 1.0;
                                    }
                                    else
                                    {
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
                                    data.IsFilesVisible = false;
                                }
                                else
                                {
                                    if (data.ISFileClosed > 0)
                                    {
                                        data.fileTickVisible = true;
                                    }
                                    else
                                    {
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

                                //data.IsTaskResourceVisible = data.TaskResourceID == Settings.userLoginID ? false : true;
                                data.IsTagDescLabelVisible = string.IsNullOrEmpty(data.IDENT_DEVIATED_TAG_DESC) ? false : true;
                                data.IsConditionNameLabelVisible = string.IsNullOrEmpty(data.ConditionName) ? false : true;
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
            finally
            {
                loadindicator = false;
            }
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Backevnttapped_click method -> in POChildListPageViewModel.cs" + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        public async void tap_InitialCamera(object sender)
        {
            try
            {
                if (!loadindicator)
                {
                    loadindicator = true;

                    YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in tap_InitialCamera method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (pageName.imgCamera.Opacity == 1.0)
                        {
                            var taglist = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                            var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                            var uniq = data.GroupBy(x => x.POShippingNumber);
                            AllPoData potagdata = new AllPoData();

                            if (data != null && data.Count == 1)
                            {
                                potagdata = data[0];
                            }

                            if (data == null || data?.Count == 0)
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select " + VinsOrParts + " to start upload photo(s).");
                            }
                            else
                            {
                                var restricData = data.Where(r => r.IsPhotoRequired == 0).ToList();

                                if (restricData.Count > 0)
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Photos not required to upload for the selected " + VinsOrParts + ".");
                                }
                                else
                                {
                                    if (data?.Count() == 1)
                                    {
                                        PhotoUploadModel selectedTagsData = new PhotoUploadModel();
                                        List<PhotoTag> lstdat = new List<PhotoTag>();

                                        foreach (var podata in uniq)
                                        {
                                            var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                            selectedTagsData.POID = d.POID;
                                            selectedTagsData.isCompleted = d.photoTickVisible;


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
                                            DependencyService.Get<IToastMessage>().ShortAlert("Photo upload is already started for the selected " + VinsOrParts + ".");
                                        }
                                        else
                                        {
                                            if (selectedTagsData.photoTags.Count != 0)
                                            {
                                                await Navigation.PushAsync(new PhotoUpload(selectedTagsData, potagdata, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false, isalldone, true), false);
                                            }
                                            else
                                            {
                                                DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("Please select only one record to upload photo(s).");
                                    }
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
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_InitialCamera method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
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
                    YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in tap_OnMessage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    loadindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (pageName.imgChat.Opacity == 1.0)
                        {
                            var taglist = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                            var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                            var uniq = data.GroupBy(x => x.POShippingNumber);

                            if (uniq.Count() == 0)
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select " + VinsOrParts + " to start conversation.");
                            }
                            else
                            {
                                ChatData selectedTagsData = new ChatData();
                                List<Tag> lstdat = new List<Tag>();

                                foreach (var podata in uniq)
                                {
                                    var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                    selectedTagsData.POID = d.POID;

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
                                    await Navigation.PushAsync(new ChatUsers(selectedTagsData, true), false);
                                }
                                else
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
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
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_OnMessage method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
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
                    YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in tap_InitialFileUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    loadindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        try
                        {
                            if (pageName.imgFileUpload.Opacity == 1.0)
                            {
                                var taglist = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                                var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                                var uniq = data.GroupBy(x => x.POShippingNumber);

                                if (data == null || data.Count() == 0)
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Please select " + VinsOrParts + " to start upload file(s).");
                                }
                                else
                                {
                                    StartUploadFileModel selectedTagsData = new StartUploadFileModel();
                                    List<FileTag> lstdat = new List<FileTag>();

                                    foreach (var podata in uniq)
                                    {
                                        var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                        selectedTagsData.POID = d.POID;

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
                                                    tg.TagNumber = item.TagNumber;
                                                    tg.IdentCode = item.IdentCode;
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
                                        DependencyService.Get<IToastMessage>().ShortAlert("File upload is already started for the selected " + VinsOrParts + ".");
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
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_InitialFileUpload method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_InitialFileUpload method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
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
                YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in MarkAsDone method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                loadindicator = true;

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (pageName.imgDone.Opacity == 1.0)
                    {
                        var val = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                        var selectedTagData = val.Where(wr => wr.IsChecked == true).ToList();

                        //if ((selectedTagData.Where(wr => wr.TagTaskStatus == 2).Count()) > 0)
                        //{
                        //    DependencyService.Get<IToastMessage>().ShortAlert("Some of the " + VinsOrParts + " are already marked as done.");
                        //}
                        //else if (selectedTagData.Count() == 0)
                        if (selectedTagData.Count() == 0)
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please select " + VinsOrParts + " to mark as done.");
                        }
                        else
                        {
                            bool makeitdone = await App.Current.MainPage.DisplayAlert("Confirm", "Make sure you have finished the inspection for the selected. \nAre you sure, you want to mark the selected as Done ? ", "Ok", "Cancel");

                            //if ((selectedTagData.Where(wr => wr.TaskID != 0 && wr.TagTaskStatus != 2).Count()) != 0 && makeitdone == true)
                            if (makeitdone == true)
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

                                if (result?.status == 1)
                                {
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

                                    if ((selectedTagData.Where(wr => wr.TaskID != 0).Count()) != 0)
                                    {
                                        TagTaskStatus taskstatus = new TagTaskStatus();
                                        taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.Select(c => c.TaskID).FirstOrDefault().ToString());
                                        taskstatus.TaskStatus = (allPOTagData?.Where(wr => wr.TagTaskStatus != 2).FirstOrDefault()) == null ? 2 : 1;
                                        taskstatus.CreatedBy = Settings.userLoginID;

                                        var taskval = await trackService.UpdateTaskStatus(taskstatus);
                                        allPOTagData.All(a => { a.TaskStatus = taskstatus.TaskStatus; return true; });
                                    }
                                    DependencyService.Get<IToastMessage>().ShortAlert("Selected " + VinsOrParts + " are marked as done.");
                                }

                                Settings.IsRefreshPartsPage = false;
                                SelectedTagCount = 0;
                                SelectedTagCountVisible = false;
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

                    YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in tap_Printer method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    if (pageName.imgPrinter.Opacity == 1.0)
                    {
                        var taglist = (obj as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                        var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                        var uniq = data.GroupBy(x => x.POShippingNumber);

                        if (uniq?.Count() == 0)
                        {
                            //Please select tag(s) to download the report
                            DependencyService.Get<IToastMessage>().ShortAlert("Please select " + VinsOrParts + ".");
                        }
                        else
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

                                    if (printResult?.status == 1)
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
                                                await Navigation.PushAsync(new PdfViewPage(printPDFModel), false);
                                                break;
                                        }
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
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_Printer method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }

        }

        private async void tap_eachCamB(object obj)
        {
            try
            {
                YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in tap_eachCamB method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var potag = obj as AllPoData;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (potag.imgCamOpacityB != 0.5)
                    {
                        loadindicator = true;

                        try
                        {
                            Settings.CanOpenScanner = false;
                            Settings.currentPuId = potag.PUID;
                            Settings.BphotoCount = potag.TagBPhotoCount;
                            await Navigation.PushAsync(new PhotoUpload(null, potag, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, potag.photoTickVisible, isalldone, true), false);
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamB method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                        }
                        finally
                        {
                            loadindicator = false;
                        }
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
                YPSLogger.ReportException(ex, "tap_eachCamB method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        /// <summary>
        /// This method is called when clicked on camera icon form data grid, to view After Packing photo(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachCamA(object obj)
        {
            YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in tap_eachCamA method " + DateTime.Now + " UserId: " + Settings.userLoginID);

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
                            Settings.CanOpenScanner = false;
                            Settings.AphotoCount = allPo.TagAPhotoCount;
                            Settings.currentPuId = allPo.PUID;
                            await Navigation.PushAsync(new PhotoUpload(null, allPo, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, allPo.photoTickVisible, isalldone, true), false);
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamA method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                            loadindicator = false;
                        }
                        finally
                        {
                            loadindicator = false;
                        }
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
                YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in ViewExistingChats method " + DateTime.Now + " UserId: " + Settings.userLoginID);
                loadindicator = true;

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    try
                    {
                        var allPo = obj as AllPoData;

                        await Navigation.PushAsync(new QnAlistPage(allPo.POID, allPo.POTagID, Settings.QAType), false);
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "ViewExistingChats method inner catch block-> in POChildListPageViewModel.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "ViewExistingChats method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            loadindicator = false;
        }

        public async void ViewUploadedFiles(object obj)
        {
            try
            {
                YPSLogger.TrackEvent("POChildListPageViewModel.cs", " in ViewUploadedFiles method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    try
                    {
                        var allPo = obj as AllPoData;

                        Settings.currentFuId = allPo.FUID;
                        Settings.FilesCount = allPo.TagFilesCount;
                        await Navigation.PushAsync(new FileUpload(null, allPo.POID, allPo.FUID, "fileUpload", allPo.fileTickVisible), false);
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "ViewUploadedFiles method inner catch block-> in POChildListPageViewModel.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "ViewUploadedFiles method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
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
                        var poid = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.POID.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        //var resource = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Resource.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var starttime = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.StartTime.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var endtime = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.EndTime.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var tagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.IdentCode.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ConditionName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var invoicenumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.InvoiceNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var tagdesc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.TagDesc.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.ShippingNumber.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var barcode1 = labelval.Where(wr => wr.FieldID.Trim().ToLower()== labelobj.Barcode1.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower()== labelobj.BagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var pending = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Pending.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var inprogress = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.Inprogress.Name.Trim().ToLower().Replace(" ", "")).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var complete = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Completed.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var all = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.All.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var home = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Home.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var jobs = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Jobs.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var parts = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Parts.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var load = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Load.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid?.Status == 1 || poid?.Status == 2 ? true : false;
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme?.Status == 1 || taskanme?.Status == 2 ? true : false;
                        //labelobj.Resource.Name = (resource != null ? (!string.IsNullOrEmpty(resource.LblText) ? resource.LblText : labelobj.Resource.Name) : labelobj.Resource.Name) + " :";
                        labelobj.StartTime.Name = (starttime != null ? (!string.IsNullOrEmpty(starttime.LblText) ? starttime.LblText : labelobj.StartTime.Name) : labelobj.StartTime.Name);
                        labelobj.StartTime.Status = starttime?.Status == 1 || starttime?.Status == 2 ? true : false;
                        labelobj.EndTime.Name = (endtime != null ? (!string.IsNullOrEmpty(endtime.LblText) ? endtime.LblText : labelobj.EndTime.Name) : labelobj.EndTime.Name);
                        labelobj.EndTime.Status = endtime?.Status == 1 || endtime?.Status == 2 ? true : false;
                        labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                        labelobj.EventName.Status = eventname?.Status == 1 || eventname?.Status == 2 ? true : false;
                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber?.Status == 1 || tagnumber?.Status == 2 ? true : false;
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode?.Status == 1 || identcode?.Status == 2 ? true : false;
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname?.Status == 1 || conditionname?.Status == 2 ? true : false;
                        labelobj.InvoiceNumber.Name = (invoicenumber != null ? (!string.IsNullOrEmpty(invoicenumber.LblText) ? invoicenumber.LblText : labelobj.InvoiceNumber.Name) : labelobj.InvoiceNumber.Name) + " :";
                        labelobj.InvoiceNumber.Status = invoicenumber?.Status == 1 || invoicenumber?.Status == 2 ? true : false;
                        labelobj.TagDesc.Name = (tagdesc != null ? (!string.IsNullOrEmpty(tagdesc.LblText) ? tagdesc.LblText : labelobj.TagDesc.Name) : labelobj.TagDesc.Name) + " :";
                        labelobj.TagDesc.Status = tagdesc?.Status == 1 || tagdesc?.Status == 2 ? true : false;
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber?.Status == 1 || shippingnumber?.Status == 2 ? true : false;
                        labelobj.Barcode1.Name = (barcode1 != null ? (!string.IsNullOrEmpty(barcode1.LblText) ? barcode1.LblText : labelobj.Barcode1.Name) : labelobj.Barcode1.Name) + " :";
                        labelobj.Barcode1.Status = barcode1?.Status == 1 || barcode1?.Status == 2 ? true : false;
                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                        labelobj.BagNumber.Status = bagnumber?.Status == 1 || bagnumber?.Status == 2 ? true : false;

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
                        labelobj.Parts.Status = parts == null ? true : (parts.Status == 1 ? true : false);
                        labelobj.Load.Status = load == null ? true : (load.Status == 1 ? true : false);
                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabel method -> in POChildListPageViewModel.cs " + Settings.userLoginID);
            }
        }

        #region Properties

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

        public string _VinsOrParts = Settings.VersionID == 2 ? "VIN(s)" : "part(s)";
        public string VinsOrParts
        {
            get => _VinsOrParts;
            set
            {
                _VinsOrParts = value;
                RaisePropertyChanged("VinsOrParts");
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
                Status = false,
                Name = "PONumber"
            };
            public DashboardLabelFields TagNumber { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "TagNumber"
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

            public DashboardLabelFields StartTime { get; set; } = new DashboardLabelFields { Status = false, Name = "Start Time" };
            public DashboardLabelFields EndTime { get; set; } = new DashboardLabelFields { Status = false, Name = "End Time" };

            public DashboardLabelFields IdentCode { get; set; } = new DashboardLabelFields { Status = false, Name = "IdentCode" };
            public DashboardLabelFields ConditionName { get; set; } = new DashboardLabelFields { Status = false, Name = "ConditionName" };
            //public LabelAndActionFields InvoiceNumber { get; set; } = new LabelAndActionFields { Status = true, Name = "InvoiceNumber" };
            public DashboardLabelFields InvoiceNumber { get; set; } = new DashboardLabelFields { Status = false, Name = "Invoice1No" };
            public DashboardLabelFields TagDesc { get; set; } = new DashboardLabelFields
            {
                Status = false,  //Name = "TagDescription"
                Name = "IDENT_DEVIATED_TAG_DESC"
            };
            public DashboardLabelFields ShippingNumber { get; set; } = new DashboardLabelFields { Status = false, Name = "Shipping Number" };
            public DashboardLabelFields Barcode1 { get; set; } = new DashboardLabelFields { Status = false, Name = "Barcode1" };
            public DashboardLabelFields BagNumber { get; set; } = new DashboardLabelFields { Status = false, Name = "BagNumber" };

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

        private bool _IsScanOrInspPopUpVisible;
        public bool IsScanOrInspPopUpVisible
        {
            get => _IsScanOrInspPopUpVisible;
            set
            {
                _IsScanOrInspPopUpVisible = value;
                RaisePropertyChanged("IsScanOrInspPopUpVisible");
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

        private string _StartTime;
        public string StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
                RaisePropertyChanged("StartTime");
            }
        }

        private string _EndTime;
        public string EndTime
        {
            get { return _EndTime; }
            set
            {
                _EndTime = value;
                RaisePropertyChanged("EndTime");
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
                    StartTime = !string.IsNullOrEmpty(value[0].StartTime) ? Convert.ToDateTime(value[0].StartTime).ToString("HH:mm") : value[0].StartTime;
                    EndTime = !string.IsNullOrEmpty(value[0].EndTime) ? Convert.ToDateTime(value[0].EndTime).ToString("HH:mm") : value[0].EndTime;
                    IsTimeGiven = string.IsNullOrEmpty(value[0].StartTime) && string.IsNullOrEmpty(value[0].EndTime) ? false : true;
                    EventName = value[0].EventName;
                    //Resource = value[0].TaskResourceName;
                    IsResourcecVisible = value[0].TaskResourceID == 0 ? false : true;
                    //IsResourcecVisible = value[0].TaskResourceID == Settings.userLoginID ? false : true;
                }
                RaisePropertyChanged("PoDataChildCollections");
            }
        }

        private bool _IsTimeGiven;
        public bool IsTimeGiven
        {
            get { return _IsTimeGiven; }
            set
            {
                _IsTimeGiven = value;
                NotifyPropertyChanged();
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
