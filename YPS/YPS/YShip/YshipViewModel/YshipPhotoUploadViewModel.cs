using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
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
using YPS.Model.Yship;
using YPS.Service;
using YPS.Views;

namespace YPS.YShip.YshipViewModel
{
    public class YshipPhotoUploadViewModel : IBase
    {
        public INavigation Navigation { get; set; }

        #region ICommand declaration
        public ICommand SelectPic { set; get; }
        public ICommand UploadPic { set; get; }
        public ICommand BeforeCommand { set; get; }
        public ICommand AfterCommand { set; get; }
        public ICommand AfterOpenCommand { set; get; }
        public ICommand CloseCommand { set; get; }
        public ICommand TapOnImge { set; get; }
        public ICommand DeleteImage { set; get; }
        public ICommand HomeCommand { get; set; }
        #endregion

        #region Data members

        int photoCounts = 0, campleted, cancelled;
        string extension = string.Empty, ybkgNo, types = string.Empty, mediaFile;
        bool accessPhoto, multipleTaps;
        public string fileName;
        public Stream picStream;
        Page pageName;
        YPSService service;
        GetYshipFiles allYshipFiles;
        bool checkInternet;

        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="navigation"></param>
        /// <param name="page"></param>
        /// <param name="yshipid"></param>
        /// <param name="ybgno"></param>
        /// <param name="completed"></param>
        /// <param name="canceled"></param>
        public YshipPhotoUploadViewModel(INavigation navigation, Page page, int yshipId, string noYBkg, int completed, int canceled)
        {
            YPSLogger.TrackEvent("YshipPhotoUploadViewModel", "Page Load " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                UploadType = (int)UploadTypeEnums.GoodsPhotos_BL; /// Goods Photos Before Loading 
                Navigation = navigation;
                pageName = page;
                ybkgNo = noYBkg;
                this.campleted = completed;
                this.cancelled = canceled;
                FirstMainStack = true;
                Tagnumbers = noYBkg;
                Settings.YshipID = yshipId;

                #region Event bindings for the ICommands
                SelectPic = new Command(async () => await SelectPhoto());
                UploadPic = new Command(async () => await PhotoUpload());
                BeforeCommand = new Command(async () => await BeforePic());
                AfterCommand = new Command(async () => await AfterPic());
                AfterOpenCommand = new Command(async () => await AfterOpenPic());
                TapOnImge = new Command(ImageTap);
                DeleteImage = new Command(DeleteImg);
                HomeCommand = new Command(HomeCommandBtn);
                #endregion

                allYshipFiles = new GetYshipFiles(); /// To maintain file details like message, status & file data
                allYshipFiles.data = new YshipUploadedPhotosList<UploadFiles>(); /// To maintain uploaded photos list data
                service = new YPSService();

                GetPhotosData(Settings.YshipID); /// Fetch uploaded photos and shows in the list
                DynamicTextChange();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipPhotoUploadViewModel constructor " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on Delete icon on the photo, to delete that sinlge photo.
        /// </summary>
        /// <param name="obj"></param>
        private async void DeleteImg(object obj)
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in delete_image method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                if (!multipleTaps)
                {
                    multipleTaps = true;

                    if (accessPhoto == true)
                    {
                        await App.Current.MainPage.DisplayAlert("Message", "You can't delete this photo.", "OK");
                    }
                    else
                    {
                        bool conform = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure want to delete?", "OK", "Cancel");

                        if (conform)
                        {
                            IndicatorVisibility = true;
                            /// Verifying internet connection.
                            checkInternet = await App.CheckInterNetConnection();

                            if (checkInternet)
                            {
                                var findData = obj as UploadFiles;
                                /// Calling API to delete the selected file.
                                var uploadresult = await service.yShipDeleteFile(findData.ID, findData.yShipId, UploadType);

                                if (uploadresult != null)
                                {
                                    if (uploadresult.status != 0)
                                    {
                                        if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BL)
                                        {
                                            var item = FinalPhotoListB.Where(x => x.ID == findData.ID).FirstOrDefault();
                                            allYshipFiles.data.beforeLoading.Remove(item);
                                        }
                                        else if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AL)
                                        {
                                            var item = FinalPhotoListA.Where(x => x.ID == findData.ID).FirstOrDefault();
                                            allYshipFiles.data.afterLoading.Remove(item);
                                        }
                                        else
                                        {
                                            var item = FinalPhotoListAO.Where(x => x.ID == findData.ID).FirstOrDefault();
                                            allYshipFiles.data.afterOpening.Remove(item);
                                        }

                                        if (allYshipFiles.data.beforeLoading.Count == 0 && UploadType == (int)UploadTypeEnums.GoodsPhotos_BL)
                                        {
                                            NoPhotosVisibility = true;
                                        }
                                        else if (allYshipFiles.data.afterLoading.Count == 0 && UploadType == (int)UploadTypeEnums.GoodsPhotos_AL)
                                        {
                                            NoPhotosVisibility = true;
                                        }
                                        else if (allYshipFiles.data.afterOpening.Count == 0 && UploadType == (int)UploadTypeEnums.GoodsPhotos_AO)
                                        {

                                            NoPhotosVisibility = true;
                                        }
                                        /// Photo count update on the data grid main page after deleting a single photo.
                                        UpdateDataBackEnd();
                                        await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "OK");
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
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
                YPSLogger.ReportException(ex, "delete_image method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
                multipleTaps = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on Home icon, to redirect to home(yShip) page
        /// </summary>
        /// <param name="obj"></param>
        private async void HomeCommandBtn(object obj)
        {
            try
            {
                await Navigation.PopAsync(true);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HomeCommand_btn method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on a photo, to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void ImageTap(object obj)
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in image_tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                if (!multipleTaps)
                {
                    multipleTaps = true;
                    var data = obj as UploadFiles;

                    if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AL)
                    {
                        await Navigation.PushAsync(new ImageView(FinalPhotoListA, data.ID, ybkgNo));
                    }
                    else if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BL)
                    {
                        await Navigation.PushAsync(new ImageView(FinalPhotoListB, data.ID, ybkgNo));
                    }
                    else if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AO)
                    {
                        await Navigation.PushAsync(new ImageView(FinalPhotoListAO, data.ID, ybkgNo));
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "image_tap method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
                multipleTaps = false;
            }
        }

        /// <summary>
        /// Photo count update on the data grid main page after uploading/deleting single photo, After Loading, Before Loading & After Opening
        /// </summary>
        public void UpdateDataBackEnd()
        {
            try
            {
                //Getting the count of photos for After Loading, Before Loading & After Opening
                int al = allYshipFiles.data.afterLoading.Where(y => y.UploadType == (int)UploadTypeEnums.GoodsPhotos_AL).Count();
                int bl = allYshipFiles.data.beforeLoading.Where(y => y.UploadType == (int)UploadTypeEnums.GoodsPhotos_BL).Count();
                int ao = allYshipFiles.data.afterOpening.Where(y => y.UploadType == (int)UploadTypeEnums.GoodsPhotos_AO).Count();

                if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BL)
                {
                    MessagingCenter.Send<string, string>("BPhotoCount", "msgBL", bl.ToString());
                }
                else if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AL)
                {
                    MessagingCenter.Send("APhotoCount", "msgAL", al.ToString());
                }
                else
                {
                    MessagingCenter.Send<string, string>("APhotoCount", "msgAO", ao.ToString());
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "UpdateDataBackEnd method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on After Loading tab, to display "No photos available" if no photo 
        /// there in After Loading tab.
        /// </summary>
        /// <returns></returns>
        public async Task AfterPic()
        {
            try
            {
                UploadType = (int)UploadTypeEnums.GoodsPhotos_AL;

                if (allYshipFiles.data != null)
                {
                    AOStack = false;
                    BStack = false;

                    NoPhotosVisibility = allYshipFiles.data.afterLoading.Count != 0 ? false : true;
                    AStack = (NoPhotosVisibility) ? false : true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "afterPic method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on Before Loading tab, to display "No photos available" if no photo
        /// there in Before Loading tab. 
        /// </summary>
        /// <returns></returns>
        public async Task BeforePic()
        {
            try
            {
                UploadType = (int)UploadTypeEnums.GoodsPhotos_BL;

                if (allYshipFiles.data != null)
                {
                    AStack = false;
                    AOStack = false;
                    NoPhotosVisibility = allYshipFiles.data.beforeLoading.Count != 0 ? false : true;
                    BStack = (NoPhotosVisibility) ? false : true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "beforePic method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }

        }

        /// <summary>
        /// Gets called when clicked on After Opening tab, to display "No photos available" if no photo 
        /// there in After Opening tab.
        /// </summary>
        /// <returns></returns>
        public async Task AfterOpenPic()
        {
            try
            {
                UploadType = (int)UploadTypeEnums.GoodsPhotos_AO;

                if (allYshipFiles.data != null)
                {
                    BStack = false;
                    AStack = false;
                    NoPhotosVisibility = allYshipFiles.data.afterOpening.Count != 0 ? false : true;
                    AOStack = (NoPhotosVisibility) ? false : true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "afteropenPic method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }

        }

        /// <summary>
        /// Gets called when clicked on Upload Button, to uplaod the selected/captured photo.
        /// </summary>
        /// <returns></returns>
        public async Task PhotoUpload()
        {
            YPSLogger.TrackEvent("YshipPhotoUplodeViewModel", "in Photo_Upload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                IndicatorVisibility = true;
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();
                /// If internet connection is available.

                if (checkInternet)
                {
                    FirstMainStack = true;
                    RowHeightOpenCam = 0;
                    SecondMainStack = FirstStack = secondStack = false;
                    ListStack = true;

                    if (extension.Trim().ToLower() == ".png" || extension.Trim().ToLower() == ".jpg" || extension.Trim().ToLower() == ".jpeg" || extension.Trim().ToLower() == ".gif" || extension.Trim().ToLower() == ".bmp")
                    {
                        /// Calling the blob API to upload photo.
                        var yshipdata = await BlobUpload.YPSFileUpload(extension, picStream, Settings.YshipID, fileName, UploadType, (int)BlobContainer.cntyshipuploads, null, null);
                        var result = yshipdata as yShipFileUploadResponse;

                        if (result != null)
                        {
                            if (result.status != 0)
                            {
                                if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AL)
                                {
                                    try
                                    {
                                        allYshipFiles.data.afterLoading.Insert(0, new UploadFiles() { FileURL = result.data.FileURL, FullName = result.data.FullName, UploadedDate = result.data.UploadedDate, ID = result.data.ID, UploadType = result.data.UploadType, yShipId = result.data.yShipId });
                                        FinalPhotoListA = allYshipFiles.data.afterLoading;
                                    }
                                    catch (Exception ex)
                                    {
                                        await service.Handleexception(ex);
                                        YPSLogger.ReportException(ex, "Photo_Upload method if(UploadType == (int)UploadTypeEnums.GoodsPhotos_AL) in Photo_Upload method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                                    }
                                }
                                else if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BL)
                                {
                                    allYshipFiles.data.beforeLoading.Insert(0, new UploadFiles() { FileURL = result.data.FileURL, FullName = result.data.FullName, UploadedDate = result.data.UploadedDate, ID = result.data.ID, UploadType = result.data.UploadType, yShipId = result.data.yShipId });
                                    FinalPhotoListB = allYshipFiles.data.beforeLoading;
                                }
                                else
                                {
                                    allYshipFiles.data.afterOpening.Insert(0, new UploadFiles() { FileURL = result.data.FileURL, FullName = result.data.FullName, UploadedDate = result.data.UploadedDate, ID = result.data.ID, UploadType = result.data.UploadType, yShipId = result.data.yShipId });
                                    FinalPhotoListAO = allYshipFiles.data.afterOpening;
                                }

                                DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Message", result.message, "OK");
                            }
                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }

                        if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AL)
                        {
                            BStack = false;
                            AOStack = false;
                            NoPhotosVisibility = FinalPhotoListA.Count != 0 ? false : true;
                            AStack = NoPhotosVisibility ? false : true;
                        }
                        else if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BL)
                        {
                            AStack = false;
                            AOStack = false;
                            NoPhotosVisibility = FinalPhotoListB.Count != 0 ? false : true;
                            BStack = NoPhotosVisibility ? false : true;
                        }
                        else
                        {
                            BStack = false;
                            AStack = false;
                            NoPhotosVisibility = FinalPhotoListAO.Count != 0 ? false : true;
                            AOStack = NoPhotosVisibility ? false : true;
                        }

                        /// Photo count update on the data grid main page after uploading a single photo.
                        UpdateDataBackEnd();

                        if (Device.RuntimePlatform == Device.Android) ///Checking for OS of the device
                        {
                            if (!string.IsNullOrEmpty(mediaFile))
                            {
                                try
                                {
                                    bool b = File.Exists(mediaFile);
                                    File.Delete(mediaFile);
                                }
                                catch (Exception ex)
                                {
                                    await service.Handleexception(ex);
                                    YPSLogger.ReportException(ex, "if(Device.OS == TargetPlatform.Android) in Photo_Upload method -> in PhotoUplodeViewModel.cs " + Settings.userLoginID);
                                    IndicatorVisibility = false;
                                }
                            }
                        }
                        else
                        {
                            string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp");

                            if (Directory.Exists(pathToNewFolder))
                            {
                                try
                                {
                                    Directory.Delete(pathToNewFolder, true);
                                }
                                catch (Exception ex)
                                {
                                    await service.Handleexception(ex);
                                    YPSLogger.ReportException(ex, "if(Directory.Exists(pathToNewFolder)) in Photo_Upload method -> in PhotoUplodeViewModel.cs " + Settings.userLoginID);
                                    IndicatorVisibility = false;
                                }
                            }

                            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                            var files = Directory.GetFiles(documents);

                            if (!files.Any())
                            {
                                IndicatorVisibility = false;
                                return;
                            }

                            foreach (var file in files)
                            {
                                try
                                {
                                    File.Delete(file);
                                }
                                catch (Exception ex)
                                {
                                    await service.Handleexception(ex);
                                    YPSLogger.ReportException(ex, "foreach (var file in files) in Photo_Upload method  -> Photo_Upload method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                                    IndicatorVisibility = false;
                                }
                            }
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please upload files having extensions: .png, .jpg, .jpeg, .gif, .bmp only.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Photo_Upload method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Fetching the Photo(s) if present.
        /// </summary>
        /// <param name="yshiid_value"></param>
        /// <returns></returns>
        public async Task GetPhotosData(int yshiid_value)
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in GetPhotosData method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();
                    /// If internet connection is available.

                    if (checkInternet)
                    {
                        string uplodtype = (int)UploadTypeEnums.GoodsPhotos_BL + "," + (int)UploadTypeEnums.GoodsPhotos_AL + "," + (int)UploadTypeEnums.GoodsPhotos_AO;
                        /// Calling photo API to get all uploaded images.
                        var filesdata = await service.GetYshipfiles(yshiid_value, uplodtype);

                        if (filesdata != null)
                        {
                            if (filesdata.status != 0)
                            {
                                allYshipFiles = filesdata;
                                FinalPhotoListA = allYshipFiles.data.afterLoading;
                                FinalPhotoListB = allYshipFiles.data.beforeLoading;
                                FinalPhotoListAO = allYshipFiles.data.afterOpening;

                                if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AL)
                                {
                                    AStack = true;
                                    NoPhotosVisibility = FinalPhotoListA.Count != 0 ? false : true;
                                }
                                else if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BL)
                                {
                                    BStack = true;
                                    NoPhotosVisibility = FinalPhotoListB.Count != 0 ? false : true;
                                }
                                else
                                {
                                    AOStack = true;
                                    NoPhotosVisibility = FinalPhotoListAO.Count != 0 ? false : true;
                                }
                            }
                            else
                            {
                                NoPhotosVisibility = true;
                            }

                            //if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                            //Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin ||
                            //Settings.userRoleID == (int)UserRoles.DealerUser)
                            //{
                            //    DeleteIconStack = false;
                            //    FirstMainStack = false;
                            //}
                            //else
                            //{
                            //    if (campleted == 1 || cancelled == 1)
                            //    {
                            //        DeleteIconStack = false;
                            //        FirstMainStack = false;
                            //    }
                            //}
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "GetPhotosData method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
                finally
                {
                    IndicatorVisibility = false;
                }
            });
        }

        /// <summary>
        /// Gets called when Capturing/Selecting a photo, to upload.
        /// </summary>
        /// <returns></returns>
        public async Task SelectPhoto()
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in SelectPic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                string action = await App.Current.MainPage.DisplayActionSheet("", "Cancel", null, "Camera", "Gallery");

                if (action.Trim().ToLower() == "camera") /// When choosen to capture from Camera
                {
                    IndicatorVisibility = true;

                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
                        BtnEnable = false;
                        var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        var statusiOS = resultIOS[Permission.Camera];

                        if (statusiOS == PermissionStatus.Denied)
                        {
                            IndicatorVisibility = false;
                            var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the camera to take photos.", null, null, "Maybe Later", "Settings");

                            switch (checkSelect)
                            {
                                case "Maybe Later":
                                    break;
                                case "Settings":
                                    CrossPermissions.Current.OpenAppSettings();
                                    break;
                            }
                        }
                        else if (statusiOS == PermissionStatus.Granted)
                        {
                            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                            {
                                PhotoSize = PhotoSize.Custom,
                                CustomPhotoSize = Settings.PhotoSize,
                                CompressionQuality = Settings.CompressionQuality
                            });

                            if (file != null)
                            {
                                IndicatorVisibility = false;
                                BtnEnable = true;

                                if (photoCounts == 0)
                                {
                                    CaptchaImage1 = ImageSource.FromStream(() =>
                                    {
                                        return file.GetStreamWithImageRotatedForExternalStorage();
                                    });
                                    FirstStack = true;
                                    ListStack = false;
                                    secondStack = false;
                                }
                                else
                                {
                                    CaptchaImage2 = ImageSource.FromStream(() =>
                                    {
                                        return file.GetStreamWithImageRotatedForExternalStorage();
                                    });
                                    FirstStack = false;
                                    secondStack = true;
                                    ListStack = false;
                                }
                                closeLabelText = false;
                                RowHeightcomplete = 0;
                                NoPhotosVisibility = false;
                                FirstMainStack = false;
                                RowHeightOpenCam = 100;
                                SecondMainStack = true;

                                extension = Path.GetExtension(file.Path);
                                mediaFile = file.Path;
                                picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(file.Path);
                            }
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                    }
                    BtnEnable = true;
                }
                else if (action.Trim().ToLower() == "gallery") /// When choosen to select from Gallery
                {
                    var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                    var statusiOS = resultIOS[Permission.Photos];

                    if (statusiOS == PermissionStatus.Denied)
                    {
                        IndicatorVisibility = false;
                        var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the gallery to take photos.", null, null, "Maybe Later", "Settings");

                        switch (checkSelect)
                        {
                            case "Maybe Later":
                                break;
                            case "Settings":
                                CrossPermissions.Current.OpenAppSettings();
                                break;
                        }
                    }
                    else if (statusiOS == PermissionStatus.Granted)
                    {
                        IndicatorVisibility = true;
                        var fileOS = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                        {
                            PhotoSize = PhotoSize.Custom,
                            CustomPhotoSize = Settings.PhotoSize,
                            CompressionQuality = Settings.CompressionQuality
                        });

                        if (fileOS != null)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (photoCounts == 0)
                                {
                                    CaptchaImage1 = ImageSource.FromFile(fileOS.Path);
                                    FirstStack = true;
                                    ListStack = false;
                                    secondStack = false;
                                }
                                else
                                {
                                    CaptchaImage2 = ImageSource.FromFile(fileOS.Path);
                                    FirstStack = false;
                                    secondStack = true;
                                }
                                closeLabelText = false;
                                RowHeightcomplete = 0;
                                FirstMainStack = false;
                                RowHeightOpenCam = 100;
                                SecondMainStack = true;
                                NoPhotosVisibility = false;
                                BtnEnable = true;

                                extension = Path.GetExtension(fileOS.Path);
                                picStream = fileOS.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(fileOS.Path);
                            });

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectPic method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Dynamic text changed.
        /// </summary>
        public void DynamicTextChange()
        {
            try
            {
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var uploadBtn = labelval.Where(wr => wr.FieldID == labelobjPhoto).Select(c => c.LblText).FirstOrDefault();
                        labelobjPhoto = uploadBtn != null ? (!string.IsNullOrEmpty(uploadBtn) ? uploadBtn : labelobjPhoto) : labelobjPhoto;

                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        #region Properties

        private ImageSource _captchaImage1;
        public ImageSource CaptchaImage1
        {
            get { return _captchaImage1; }
            set
            {
                _captchaImage1 = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource _captchaImage2;
        public ImageSource CaptchaImage2
        {
            get { return _captchaImage2; }
            set
            {
                _captchaImage2 = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<UploadFiles> _FinalPhotoListB;
        public ObservableCollection<UploadFiles> FinalPhotoListB
        {
            get { return _FinalPhotoListB; }
            set
            {
                _FinalPhotoListB = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<UploadFiles> _FinalPhotoListA;
        public ObservableCollection<UploadFiles> FinalPhotoListA
        {
            get { return _FinalPhotoListA; }
            set
            {
                _FinalPhotoListA = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<UploadFiles> _FinalPhotoList_AO;
        public ObservableCollection<UploadFiles> FinalPhotoListAO
        {
            get { return _FinalPhotoList_AO; }
            set
            {
                _FinalPhotoList_AO = value;
                NotifyPropertyChanged();
            }
        }

        private bool _BStack = false;
        public bool BStack
        {
            get { return _BStack; }
            set
            {
                _BStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _AStack = false;
        public bool AStack
        {
            get { return _AStack; }
            set
            {
                _AStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _AOStack = false;
        public bool AOStack
        {
            get { return _AOStack; }
            set
            {
                _AOStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _activityIndicator = false;
        public bool activityIndicator
        {
            get { return _activityIndicator; }
            set
            {
                _activityIndicator = value;
                NotifyPropertyChanged();
            }
        }

        private bool _FirstStack = false;
        public bool FirstStack
        {
            get { return _FirstStack; }
            set
            {
                _FirstStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _secondStack = false;
        public bool secondStack
        {
            get { return _secondStack; }
            set
            {
                _secondStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ListStack = true;
        public bool ListStack
        {
            get { return _ListStack; }
            set
            {
                _ListStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _FirstMainStack = true;
        public bool FirstMainStack
        {
            get { return _FirstMainStack; }
            set
            {
                _FirstMainStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _SecondMainStack = false;
        public bool SecondMainStack
        {
            get { return _SecondMainStack; }
            set
            {
                _SecondMainStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _BtnEnable = true;
        public bool BtnEnable
        {
            get { return _BtnEnable; }
            set
            {
                _BtnEnable = value;
                NotifyPropertyChanged();
            }
        }

        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private string _description_txt = "";
        public string description_txt
        {
            get { return _description_txt; }
            set
            {
                _description_txt = value;
                NotifyPropertyChanged();
            }
        }
        public bool _NoPhotos_Visibility = false;
        public bool NoPhotosVisibility
        {
            get { return _NoPhotos_Visibility; }
            set
            {
                _NoPhotos_Visibility = value;
                NotifyPropertyChanged();
            }
        }

        public string _Tagnumbers = Settings.Tagnumbers;
        public string Tagnumbers
        {
            get { return _Tagnumbers; }
            set
            {
                _Tagnumbers = value;
                NotifyPropertyChanged();
            }
        }

        private bool _closeLabelText = false;
        public bool closeLabelText
        {
            get { return _closeLabelText; }
            set
            {
                _closeLabelText = value;
                NotifyPropertyChanged();
            }
        }

        private int _RowHeightTitle = 30;

        public int RowHeightTitle
        {
            get => _RowHeightTitle;
            set
            {
                _RowHeightTitle = value;
                NotifyPropertyChanged();
            }
        }

        private int _RowHeightTagNumber = 20;

        public int RowHeightTagNumber
        {
            get => _RowHeightTagNumber;
            set
            {
                _RowHeightTagNumber = value;
                NotifyPropertyChanged();
            }
        }

        private int _RowHeightcomplete = 0;

        public int RowHeightcomplete
        {
            get => _RowHeightcomplete;
            set
            {
                _RowHeightcomplete = value;
                NotifyPropertyChanged();
            }
        }

        private int _RowHeightOpenCam = 0;

        public int RowHeightOpenCam
        {
            get => _RowHeightOpenCam;
            set
            {
                _RowHeightOpenCam = value;
                NotifyPropertyChanged();
            }
        }

        private bool _CompletedVal = false;
        public bool CompletedVal
        {
            get { return _CompletedVal; }
            set
            {
                _CompletedVal = value;
                NotifyPropertyChanged();
            }
        }

        private bool _notCompeleteDisable = true;
        public bool notCompeleteDisable
        {
            get { return _notCompeleteDisable; }
            set
            {
                _notCompeleteDisable = value;
                NotifyPropertyChanged();
            }
        }

        private bool _NotCompletedVal = true;
        public bool NotCompletedVal
        {
            get { return _NotCompletedVal; }
            set
            {
                _NotCompletedVal = value;
                NotifyPropertyChanged();
            }
        }

        public int _UploadType;
        public int UploadType
        {
            get { return _UploadType; }
            set
            {
                _UploadType = value;
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

        private string _labelobjPhoto = "Upload";
        public string labelobjPhoto
        {
            get => _labelobjPhoto;
            set
            {
                _labelobjPhoto = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
