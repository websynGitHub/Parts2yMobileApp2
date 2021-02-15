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
        public ICommand MoveToScanCmd { get; set; }
        public ICommand CheckBoxTapCmd { get; set; }


        public POChildListPageViewModel(INavigation _Navigation, ObservableCollection<AllPoData> potag)
        {
            try
            {
                Navigation = _Navigation;
                Task.Run(() => ChangeLabel()).Wait();
                Task.Run(() => PreparePoTagList(potag)).Wait();
                //PoDataChildCollections = new ObservableCollection<AllPoData>(potag);
                moveToPage = new Command(RedirectToPage);
                viewExistingFiles = new Command(ViewUploadedFiles);
                viewExistingBUPhotos = new Command(tap_eachCamB);
                viewExistingAUPhotos = new Command(tap_eachCamA);
                viewExistingChats = new Command(ViewExistingChats);
                HomeCommand = new Command(HomeCommand_btn);
                CheckBoxTapCmd = new Command(CheckBoxTap);
                Backevnttapped = new Command(async () => await Backevnttapped_click());
            }
            catch (Exception ex)
            {

            }
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

        public async void MoveToScan()
        {
            try
            {
                loadindicator = true;

                if (POTagDetail != null)
                {
                    try
                    {
                        loadindicator = true;
                        POTagDetail.SelectedTagBorderColor = Color.DarkGreen;
                        await Navigation.PushAsync(new DriverVehicleDetails());
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        loadindicator = false;
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

        public async void PreparePoTagList(ObservableCollection<AllPoData> potaglist)
        {
            try
            {
                loadindicator = false;

                if (potaglist != null && potaglist.Count > 0)
                {
                    foreach (var data in potaglist)
                    {
                        if (data.TagNumber != null)
                        {
                            #region Chat
                            if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.SuperUser || Settings.userRoleID == (int)UserRoles.SuperViewer)
                            {
                                data.chatImage = "minus.png";

                                data.IsChatsVisible = false;
                            }
                            else
                            {
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
                            }
                            #endregion

                            #region Before Photo & After Photo
                            if (data.PUID == 0)
                            {
                                if (data.IsPhotoRequired == 0)
                                {
                                    data.cameImage = "cross.png";
                                }
                                else
                                {
                                    data.cameImage = "minus.png";
                                }

                                data.IsPhotosVisible = false;
                            }
                            else
                            {
                                if (data.ISPhotoClosed == 1)
                                {
                                    data.cameImage = "Chatcamera.png";
                                    data.photoTickVisible = true;

                                    data.imgCamOpacityB = data.imgTickOpacityB = (data.TagBPhotoCount == 0) ? 0.5 : 1.0;
                                    data.imgCamOpacityA = data.imgtickOpacityA = (data.TagAPhotoCount == 0) ? 0.5 : 1.0;
                                }
                                else
                                {
                                    data.cameImage = "Chatcamera.png";
                                    data.BPhotoCountVisible = true;
                                    data.APhotoCountVisible = true;
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
                            //data.IS_POS = null;
                            //data.IS_SUB = null;
                            data.emptyCellValue = "No records to display";
                        }
                    }
                }
                PoDataChildCollections = potaglist;
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
                        if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        {
                            var taglist = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                            var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                            //var data = taglist.SelectedItems.Cast<AllPoData>().ToList();
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
                        if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        {
                            var taglist = (sender as CollectionView).ItemsSource as ObservableCollection<AllPoData>;
                            var data = taglist.Where(wr => wr.IsChecked == true).ToList();
                            //var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
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
                        if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        {
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

        public async void tap_Printer(object obj)
        {
            try
            {
                if (!loadindicator)
                {
                    loadindicator = true;

                    YPSLogger.TrackEvent("PoDataViewModel", "in tap_Printer method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    {
                        if (Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                        {
                        }
                        else
                        {
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
                                Settings.currentPuId = potag.PUID;
                                Settings.BphotoCount = potag.TagBPhotoCount;
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
                loadindicator = true;

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var allPo = obj as AllPoData;

                    if (allPo.imgCamOpacityA != 0.5)
                    {
                        try
                        {
                            if (allPo.cameImage == "Chatcamera.png")
                            {
                                Settings.AphotoCount = allPo.TagAPhotoCount;
                                Settings.currentPuId = allPo.PUID;
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
                        }
                        loadindicator = false;
                    }
                }
                else
                {
                    loadindicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
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
                        //Getting Label values & Status based on FieldID
                        var poid = labelval.Where(wr => wr.FieldID == labelobj.POID.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID == labelobj.ShippingNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqnumber = labelval.Where(wr => wr.FieldID == labelobj.REQNo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var tagdescription = labelval.Where(wr => wr.FieldID == labelobj.TagDescription.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID == labelobj.IdentCode.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var elevel = labelval.Where(wr => wr.FieldID == labelobj.Elevel.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID == labelobj.ConditionName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();


                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) ;
                        labelobj.POID.Status = poid == null ? true : (poid.Status == 1 ? true : false);
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name);
                        labelobj.ShippingNumber.Status = shippingnumber == null ? true : (shippingnumber.Status == 1 ? true : false);
                        labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name);
                        labelobj.REQNo.Status = reqnumber == null ? true : (reqnumber.Status == 1 ? true : false);

                        labelobj.TagDescription.Name = (tagdescription != null ? (!string.IsNullOrEmpty(tagdescription.LblText) ? tagdescription.LblText : labelobj.TagDescription.Name) : labelobj.TagDescription.Name) + " :";
                        labelobj.TagDescription.Status = tagdescription == null ? true : (tagdescription.Status == 1 ? true : false);
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode == null ? true : (identcode.Status == 1 ? true : false);
                        labelobj.Elevel.Name = (elevel != null ? (!string.IsNullOrEmpty(elevel.LblText) ? elevel.LblText : labelobj.Elevel.Name) : labelobj.Elevel.Name) + " :";
                        labelobj.Elevel.Status = elevel == null ? true : (elevel.Status == 1 ? true : false);
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname == null ? true : (conditionname.Status == 1 ? true : false);

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
            public DashboardLabelFields TagDescription { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TagDescription"
            };
            public DashboardLabelFields IdentCode { get; set; } = new DashboardLabelFields { Status = true, Name = "IdentCode" };
            public DashboardLabelFields Elevel { get; set; } = new DashboardLabelFields { Status = true, Name = "ELevelName" };
            public DashboardLabelFields ConditionName { get; set; } = new DashboardLabelFields { Status = true, Name = "ConditionName" };

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

        private AllPoData _POTagDetail;
        public AllPoData POTagDetail
        {
            get => _POTagDetail;
            set
            {
                _POTagDetail = value;
                MoveToScan();
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
                }
                RaisePropertyChanged("PoDataChildCollections");
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
