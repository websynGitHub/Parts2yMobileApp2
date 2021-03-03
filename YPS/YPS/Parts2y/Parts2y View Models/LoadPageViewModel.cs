using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class LoadPageViewModel : IBase
    {
        YPSService service;
        public INavigation Navigation { get; set; }
        bool checkInternet;
        int puid, poid, selectiontype_index, photoCounts = 0;
        AllPoData selectedTagData;
        Stream picStream;
        string extension = "", Mediafile, fileName;
        public Command select_pic { set; get; }
        public Command CloseCommand { set; get; }
        public Command upload_pic { set; get; }
        public ICommand ViewPhotoDetailsCmd { set; get; }
        public ICommand DeleteImageCmd { set; get; }
        public Command HomeCommand { get; set; }

        public LoadPageViewModel(INavigation navigation, AllPoData selectedtagdata, LoadPage pagename)
        {
            try
            {
                service = new YPSService();
                Navigation = navigation;
                IsNoPhotoTxt = true;
                selectedTagData = selectedtagdata;
                Tagnumbers = selectedtagdata.TagNumber;
                select_pic = new Command(async () => await SelectPic());
                upload_pic = new Command(async () => await UploadPhoto());
                //CloseCommand = new Command(async () => await ClosePic());
                ViewPhotoDetailsCmd = new Command(ViewPhotoDetails);
                DeleteImageCmd = new Command(DeleteImage);
                HomeCommand = new Command(HomeCommand_btn);

                if (selectedTagData.TagTaskStatus == 2)
                {
                    NotDoneVal = false;
                    DoneBtnOpacity = 0.5;
                }

                GetPhotosData(selectedtagdata.POTagID);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task MarkAsDone()
        {
            try
            {
                YPSLogger.TrackEvent("MarkAsDone", "in image_tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
                IndicatorVisibility = true;

                if (selectedTagData.TaskID != 0 && selectedTagData.TagTaskStatus != 2)
                {
                    TagTaskStatus tagtaskstatus = new TagTaskStatus();
                    tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                    tagtaskstatus.POTagID = Helperclass.Encrypt(selectedTagData.POTagID.ToString());
                    tagtaskstatus.Status = 2;
                    tagtaskstatus.CreatedBy = Settings.userLoginID;

                    var result = await service.UpdateTagTaskStatus(tagtaskstatus);

                    NotDoneVal = false;
                    DoneBtnOpacity = 0.5;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "image_tap method -> in LoadPageViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            IndicatorVisibility = false;
        }


        /// <summary>
        /// Get the existing uploaded photo(s).
        /// </summary>
        /// <param name="potagid"></param>
        /// <returns></returns>
        public async Task GetPhotosData(int potagid)
        {

            YPSLogger.TrackEvent("LoadPageViewModel", "in GetPhotosData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IndicatorVisibility = true;
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        /// Calling photo API to get photos.
                        var result = await service.GetLoadPhotos(potagid);

                        if (result != null && result.status != 0 && result.data.Count != 0)
                        {
                            IsPhotosListVisible = true;
                            IsPhotosListStackVisible = true;
                            IsNoPhotoTxt = false;
                            closeLabelText = true;

                            LoadPhotosList = new ObservableCollection<LoadPhotoModel>(result.data);
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
                    YPSLogger.ReportException(ex, "GetPhotosData method -> in LoadPageViewModel " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
                IndicatorVisibility = false;
            });
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void ViewPhotoDetails(object obj)
        {
            YPSLogger.TrackEvent("LoadPageViewModel", "in image_tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var data = obj as LoadPhotoModel;
                int photoid = Convert.ToInt32(data.PhotoID);
                var des = LoadPhotosList.Where(x => x.PhotoID == photoid).FirstOrDefault();
                var imageLists = LoadPhotosList;

                await Navigation.PushAsync(new ImageView(imageLists, photoid, Tagnumbers));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "image_tap method -> in LoadPageViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }


        /// <summary>
        /// Gets called when clicked the delete icon on photo.
        /// </summary>
        /// <param name="obj"></param>
        private async void DeleteImage(object obj)
        {
            YPSLogger.TrackEvent("LoadPageViewModel", "in DeleteImage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            IndicatorVisibility = true;
            try
            {
                //if (Access_Photo == true)
                //{
                //    await App.Current.MainPage.DisplayAlert("Message", "You can't able to delete this photo.", "OK");
                //}
                //else
                //{
                bool conform = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure want to delete?", "OK", "Cancel");

                if (conform)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        IndicatorVisibility = true;
                        try
                        {
                            /// Verifying internet connection.
                            checkInternet = await App.CheckInterNetConnection();

                            if (checkInternet)
                            {
                                var findData = obj as LoadPhotoModel;
                                /// Calling photo delete API to delete files based on a photo id.
                                var uploadresult = await service.DeleteLoadImageService(findData.PhotoID);

                                if (uploadresult != null)
                                {
                                    if (uploadresult.status != 0)
                                    {
                                        var item = LoadPhotosList.Where(x => x.PhotoID == findData.PhotoID).FirstOrDefault();
                                        LoadPhotosList.Remove(item);

                                        if (LoadPhotosList.Count == 0)
                                        {
                                            IsNoPhotoTxt = true;
                                        }

                                        await GetPhotosData(selectedTagData.POTagID);

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
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "delete_image method from if(conform) -> in LoadPageViewModel " + Settings.userLoginID);
                            await service.Handleexception(ex);
                        }

                        IndicatorVisibility = false;
                    });
                }
                //}
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DeleteImage method -> in LoadPageViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on Upload Button.
        /// </summary>
        /// <returns></returns>
        public async Task UploadPhoto()
        {
            YPSLogger.TrackEvent("LoadPageViewModel", "in UploadPhoto method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                IndicatorVisibility = true; ;
                try
                {
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        IsPhotoUploadIconVisible = true;
                        RowHeightOpenCam = 0;
                        IsUploadStackVisible = IsImageViewForUploadVisible = false;
                        IsPhotosListStackVisible = true;

                        if (extension.Trim().ToLower() == ".png" || extension.Trim().ToLower() == ".jpg" || extension.Trim().ToLower() == ".jpeg" || extension.Trim().ToLower() == ".gif" || extension.Trim().ToLower() == ".bmp")
                        {
                            DescriptionText = Regex.Replace(DescriptionText, @"\s+", " ");

                            if (selectedTagData != null && selectedTagData.POTagID != 0)
                            {
                                /// Calling the blob API to upload photo. 
                                var initialdata = await BlobUpload.YPSFileUpload(extension, picStream, selectedTagData.POTagID, fileName, (int)UploadTypeEnums.TagLoadPhotos, (int)BlobContainer.cnttagphotos, null, null, DescriptionText);

                                var initialresult = initialdata as LoadPhotosUploadResponse;
                                if (initialresult != null)
                                {
                                    if (initialresult.status != 0)
                                    {
                                        selectiontype_index = 1;

                                        //if (initialresult.data.Count != 0)
                                        //{
                                        //    var result = initialresult.photoTags.Select(x => x.TagNumber).ToList();
                                        //    Settings.Tagnumbers = String.Join(" , ", result);
                                        //}

                                        //if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AP)
                                        //{
                                        //    AllPhotosData.data = new UploadedPhotosList<CustomPhotoModel>() { Aphotos = { new CustomPhotoModel() { PhotoURL = initialresult.data.photo.PhotoURL, PhotoDescription = initialresult.data.photo.PhotoDescription, PhotoID = initialresult.data.photo.PhotoID, UploadType = UploadType/* uploadType*/, FullName = initialresult.data.photo.FullName, CreatedDate = initialresult.data.photo.CreatedDate } } };
                                        //    finalPhotoListA = AllPhotosData.data.Aphotos;
                                        //    AStack = true;
                                        //    BStack = false;
                                        //    AfterPackingTextColor = Color.Green;
                                        //    BeforePackingTextColor = Color.Black;
                                        //}

                                        DependencyService.Get<IToastMessage>().ShortAlert("Success."); ;
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                }
                            }

                            DescriptionText = string.Empty;

                            await GetPhotosData(selectedTagData.POTagID);

                            //UpdateDataBackEnd();

                            //if (AllPhotosData.data.Aphotos.Count == 0 && AllPhotosData.data.BPhotos.Count == 0)
                            //{
                            //    closeLabelText = false;
                            //    RowHeightcomplete = 0;
                            //}
                            //else
                            //{
                            //    if (Settings.userRoleID == (int)UserRoles.SupplierAdmin ||
                            //        Settings.userRoleID == (int)UserRoles.SupplierUser)
                            //    {
                            //        if (isuploadcompleted == true)
                            //        {
                            //            DeleteIconStack = false;
                            //            closeLabelText = false;
                            //            RowHeightcomplete = 0;
                            //        }
                            //    }
                            //    else
                            //    {
                            //        closeLabelText = true;
                            //        RowHeightcomplete = 50;
                            //    }
                            //}

                            if (Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                                    Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser ||
                                    Settings.userRoleID == (int)UserRoles.LogisticsAdmin || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                            {
                                closeLabelText = true;
                                //RowHeightcomplete = 0;
                                DeleteIconStack = false;
                            }

                            if (Device.RuntimePlatform == Device.Android)
                            {

                                if (!string.IsNullOrEmpty(Mediafile))
                                {
                                    try
                                    {
                                        bool b = File.Exists(Mediafile);
                                        File.Delete(Mediafile);
                                    }
                                    catch (Exception ex)
                                    {
                                        await service.Handleexception(ex);
                                        YPSLogger.ReportException(ex, "while deleting the Mediafile in Photo_Upload method -> in LoadPageViewModel " + Settings.userLoginID);
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
                                        YPSLogger.ReportException(ex, "while deleting the folder in Photo_Upload method -> in LoadPageViewModel " + Settings.userLoginID);
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
                                        YPSLogger.ReportException(ex, "while deleting the each files in Parts2y folder -> Photo_Upload method -> in LoadPageViewModel " + Settings.userLoginID);
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
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "UploadPhoto method -> in LoadPageViewModel " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
                finally
                {
                    IndicatorVisibility = false;
                }
            });
        }


        /// <summary>
        /// Gets called when clicked on the Camera icon for uploading photo.
        /// </summary>
        /// <returns></returns>
        public async Task SelectPic()
        {
            YPSLogger.TrackEvent("LoadPageViewModel", "in SelectPic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                string action = await App.Current.MainPage.DisplayActionSheet("", "Cancel", null, "Camera", "Gallery");

                if (action == "Camera")
                {
                    IndicatorVisibility = true;

                    /// Checking camera is available or not in in mobile.
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
                        //btnenable = false;
                        /// Request permission a user to allowed take photos from the camera.
                        var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        var statusiOS = resultIOS[Permission.Camera];

                        /// Checking permission is allowed or denied by the user to access the photo from mobile.
                        if (statusiOS == PermissionStatus.Denied)
                        {
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
                                //btnenable = true;

                                if (photoCounts == 0)
                                {
                                    ImageViewForUpload = ImageSource.FromStream(() =>
                                    {
                                        return file.GetStreamWithImageRotatedForExternalStorage();
                                    });

                                    //firstStack = true;
                                    closeLabelText = false;
                                    //RowHeightcomplete = 0;
                                    //listStack = false;
                                    //secondStack = false;
                                    //NoPhotos_Visibility = false;
                                    IsUploadStackVisible = true;
                                }
                                else
                                {
                                    //CaptchaImage2 = ImageSource.FromStream(() =>
                                    //{
                                    //    return file.GetStreamWithImageRotatedForExternalStorage();
                                    //});

                                    //firstStack = false;
                                    //secondStack = true;
                                    closeLabelText = false;
                                    //listStack = false;
                                    //RowHeightcomplete = 0;
                                    //NoPhotos_Visibility = false;

                                }

                                extension = Path.GetExtension(file.Path);
                                Mediafile = file.Path;
                                picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(file.Path);
                                //FirstMainStack = false;
                                IsPhotoUploadIconVisible = false;
                                IsPhotosListVisible = false;
                                IsPhotosListStackVisible = false;
                                RowHeightOpenCam = 100;
                                //SecondMainStack = true;
                            }
                        }
                        else
                        {
                            //btnenable = true;
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                    }
                    //btnenable = true;
                }
                else if (action == "Gallery")
                {
                    /// Request permission a user to allowed take photos from the gallery.
                    var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                    var statusiOS = resultIOS[Permission.Photos];

                    /// Checking permission is allowed or denied by the user to access the photo from mobile.
                    if (statusiOS == PermissionStatus.Denied)
                    {
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
                                    ImageViewForUpload = ImageSource.FromFile(fileOS.Path);
                                    //firstStack = true;
                                    //listStack = false;
                                    //secondStack = false;
                                    IsUploadStackVisible = true;
                                }
                                else
                                {
                                    //CaptchaImage2 = ImageSource.FromFile(fileOS.Path);
                                    //firstStack = false;
                                    //secondStack = true;
                                }

                                closeLabelText = false;
                                IsPhotoUploadIconVisible = false;
                                //RowHeightcomplete = 0;
                                //FirstMainStack = false;
                                RowHeightOpenCam = 100;
                                //SecondMainStack = true;
                                //NoPhotos_Visibility = false;
                                //btnenable = true;
                                IsPhotosListVisible = false;
                                IsPhotosListStackVisible = false;
                                extension = Path.GetExtension(fileOS.Path);
                                picStream = fileOS.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(fileOS.Path);
                            });
                        }
                    }
                    else
                    {
                        //btnenable = true;
                    }
                }
                IndicatorVisibility = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectPic method -> in LoadPageViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked the delete icon on photo.
        /// </summary>
        /// <param name="obj"></param>
        //private async void delete_image(object obj)
        //{
        //    YPSLogger.TrackEvent("LoadPageViewModel", "in delete_image method " + DateTime.Now + " UserId: " + Settings.userLoginID);

        //    IndicatorVisibility = true;
        //    try
        //    {
        //        if (!multiple_Taps)
        //        {
        //            multiple_Taps = true;

        //            if (Access_Photo == true)
        //            {
        //                await App.Current.MainPage.DisplayAlert("Message", "You can't able to delete this photo.", "OK");
        //            }
        //            else
        //            {
        //                bool conform = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure want to delete?", "OK", "Cancel");

        //                if (conform)
        //                {
        //                    Device.BeginInvokeOnMainThread(async () =>
        //                    {
        //                        IndicatorVisibility = true;
        //                        try
        //                        {
        //                            /// Verifying internet connection.
        //                            checkInternet = await App.CheckInterNetConnection();

        //                            if (checkInternet)
        //                            {
        //                                var findData = obj as CustomPhotoModel;
        //                                /// Calling photo delete API to delete files based on a photo id.
        //                                var uploadresult = await service.DeleteImageService(findData.PhotoID);

        //                                if (uploadresult != null)
        //                                {
        //                                    if (uploadresult.status != 0)
        //                                    {
        //                                        if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BP)
        //                                        {
        //                                            var item = finalPhotoListB.Where(x => x.PhotoID == findData.PhotoID).FirstOrDefault();
        //                                            AllPhotosData.data.BPhotos.Remove(item);

        //                                            if (AllPhotosData.data.BPhotos.Count == 0)
        //                                            {
        //                                                NoPhotos_Visibility = true;
        //                                            }
        //                                        }
        //                                        else
        //                                        {
        //                                            var item = finalPhotoListA.Where(x => x.PhotoID == findData.PhotoID).FirstOrDefault();
        //                                            AllPhotosData.data.Aphotos.Remove(item);

        //                                            if (AllPhotosData.data.Aphotos.Count == 0)
        //                                            {
        //                                                NoPhotos_Visibility = true;
        //                                            }
        //                                        }

        //                                        UpdateDataBackEnd();

        //                                        if (AllPhotosData.data.Aphotos.Count == 0 && AllPhotosData.data.BPhotos.Count == 0)
        //                                        {
        //                                            closeLabelText = false;
        //                                            RowHeightcomplete = 0;
        //                                        }
        //                                        await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "OK");
        //                                    }
        //                                    else
        //                                    {
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                }
        //                            }
        //                            else
        //                            {
        //                                DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            YPSLogger.ReportException(ex, "delete_image method from if(conform) -> in LoadPageViewModel " + Settings.userLoginID);
        //                            await service.Handleexception(ex);
        //                        }

        //                        IndicatorVisibility = false;
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YPSLogger.ReportException(ex, "delete_image method -> in LoadPageViewModel " + Settings.userLoginID);
        //        await service.Handleexception(ex);
        //    }
        //    finally
        //    {
        //        IndicatorVisibility = false;
        //    }
        //    multiple_Taps = false;
        //}

        /// <summary>
        /// Gets called when clicked on Complete RadioButton
        /// </summary>
        /// <returns></returns>
        //public async Task ClosePic()
        //{
        //    YPSLogger.TrackEvent("LoadPageViewModel", "in ClosePic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
        //    IndicatorVisibility = true;

        //    try
        //    {
        //        NotCompletedVal = false;
        //        CompleteBtnOpacity = 0.5;

        //        bool result = await App.Current.MainPage.DisplayAlert("Complete", "Are you sure?", "Yes", "No");

        //        if (result)
        //        {
        //            /// Verifying internet connection.
        //            checkInternet = await App.CheckInterNetConnection();

        //            if (checkInternet)
        //            {
        //                /// Calling ClosePhoto API to close upload photo.
        //                var closePhotoResult = await service.ClosePhotoData(poid, puid);

        //                if (closePhotoResult.status != 0 || closePhotoResult != null)
        //                {
        //                    MessagingCenter.Send<string, string>("PhotoComplted", "showtickMark", puid.ToString());

        //                    switch (Device.RuntimePlatform)
        //                    {
        //                        case Device.iOS:
        //                            await App.Current.MainPage.DisplayAlert("Completed", "Success.", "Close");
        //                            break;
        //                        case Device.Android:
        //                            DependencyService.Get<IToastMessage>().ShortAlert("Success.");
        //                            break;
        //                    }
        //                    await Navigation.PopAsync(true);
        //                }
        //                else
        //                {
        //                }
        //            }
        //            else
        //            {
        //                DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
        //            }
        //        }
        //        else
        //        {
        //            CompletedVal = false;
        //            NotCompletedVal = true;
        //            CompleteBtnOpacity = 1.0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YPSLogger.ReportException(ex, "ClosePic method -> in LoadPageViewModel " + Settings.userLoginID);
        //        await service.Handleexception(ex);
        //    }
        //    finally
        //    {
        //        IndicatorVisibility = false;
        //    }
        //}

        private async void HomeCommand_btn(object obj)
        {
            try
            {
                //await Navigation.PopAsync(true);
                App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HomeCommand_btn method -> in LoadPageViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }

        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        //private async void image_tap(object obj)
        //{
        //    YPSLogger.TrackEvent("LoadPageViewModel", "in image_tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
        //    IndicatorVisibility = true;

        //    try
        //    {
        //        if (!multiple_Taps)
        //        {
        //            multiple_Taps = true;
        //            var data = obj as CustomPhotoModel;
        //            int photoid = Convert.ToInt32(data.PhotoID);
        //            var des = (UploadType == (int)UploadTypeEnums.GoodsPhotos_AP) ? finalPhotoListA.Where(x => x.PhotoID == photoid).FirstOrDefault() : finalPhotoListB.Where(x => x.PhotoID == photoid).FirstOrDefault();
        //            var imageLists = UploadType == (int)UploadTypeEnums.GoodsPhotos_AP ? finalPhotoListA : finalPhotoListB;

        //            foreach (var items in imageLists)
        //            {
        //                if (items.PhotoDescription.Length > 150)
        //                {
        //                    items.ShowAndHideDescr = true;
        //                    items.ShowAndHideBtn = true;
        //                    items.ShowAndHideBtnEnable = true;
        //                }
        //                else if (items.PhotoDescription.Length > 0)
        //                {
        //                    items.ShowAndHideDescr = true;
        //                }
        //            }
        //            await Navigation.PushAsync(new ImageView(imageLists, photoid, Tagnumbers));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YPSLogger.ReportException(ex, "image_tap method -> in LoadPageViewModel " + Settings.userLoginID);
        //        await service.Handleexception(ex);
        //    }
        //    finally
        //    {
        //        IndicatorVisibility = false;
        //    }
        //    multiple_Taps = false;
        //}

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
                YPSLogger.ReportException(ex, "DynamicTextChange method in LoadPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
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
        }
        public class LabelAndActionFields : IBase
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

        private ObservableCollection<LoadPhotoModel> _LoadPhotosList;
        public ObservableCollection<LoadPhotoModel> LoadPhotosList
        {
            get { return _LoadPhotosList; }
            set
            {
                _LoadPhotosList = value;
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

        public string _Tagnumbers;
        public string Tagnumbers
        {
            get { return _Tagnumbers; }
            set
            {
                _Tagnumbers = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsNoPhotoTxt = false;
        public bool IsNoPhotoTxt
        {
            get { return _IsNoPhotoTxt; }
            set
            {
                _IsNoPhotoTxt = value;
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


        private bool _IsUploadStackEnable = true;
        public bool IsUploadStackEnable
        {
            get { return _IsUploadStackEnable; }
            set
            {
                _IsUploadStackEnable = value;
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

        private double _DoneBtnOpacity { set; get; } = 1.0;
        public double DoneBtnOpacity
        {
            get
            {
                return _DoneBtnOpacity;
            }
            set
            {
                _DoneBtnOpacity = value;
                RaisePropertyChanged("DoneBtnOpacity");
            }
        }

        private bool _NotDoneVal = true;
        public bool NotDoneVal
        {
            get { return _NotDoneVal; }
            set
            {
                _NotDoneVal = value;
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

        private bool _IsPhotoUploadIconEnable = true;
        public bool IsPhotoUploadIconEnable
        {
            get { return _IsPhotoUploadIconEnable; }
            set
            {
                _IsPhotoUploadIconEnable = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
