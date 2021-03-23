using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YPS.Parts2y.Parts2y_Services;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using static YPS.Parts2y.Parts2y_Models.PhotoModel;
using YPS.Views;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class InspectionPhotoUploadViewModel : YPS.Parts2y.Parts2y_Services.IBase
    {
        #region IComman and data members declaration

        public ICommand select_pic { set; get; }
        public ICommand upload_pic { set; get; }

        public INavigation Navigation { get; set; }

        public ICommand tap_OnImge { set; get; }
        public ICommand deleteImage { set; get; }

        YPSService trackService;
        InspectionPhotosPage pagename;
        Stream picStream;
        string extension = "", fileName, Mediafile, types = string.Empty;
        int tagId;
        bool multiple_Taps;
        InspectionConfiguration inspectionConfiguration;

        #endregion

        public InspectionPhotoUploadViewModel(INavigation _Navigation, InspectionPhotosPage page, int tagId, InspectionConfiguration inspectionConfiguration, string vinValue)
        {
            Navigation = _Navigation;
            pagename = page;
            this.tagId = tagId;
            Tagnumbers = vinValue;
            Question = inspectionConfiguration.MInspectionConfigID + " " + inspectionConfiguration.Question;
            this.inspectionConfiguration = inspectionConfiguration;
            trackService = new YPSService();
            ListOfImage = new ObservableCollection<GalleryImage>();
            Task.Run(() => GetInspectionPhotos()).Wait();
            select_pic = new Command(async () => await SelectPic());
            upload_pic = new Command(async () => await Photo_Upload());
            deleteImage = new Command<InspectionPhotosResponseListData>(Delete_Photo);
            tap_OnImge = new Command<InspectionPhotosResponseListData>(image_tap);

        }

        private async void Delete_Photo(InspectionPhotosResponseListData inspectionPhotosResponseListData)
        {
            YPSLogger.TrackEvent("InspectionPhotoUploadPageViewModel", "in DeleteImage method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                IndicatorVisibility = true;
                bool conform = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure want to delete?", "OK", "Cancel");
                if (conform)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        /// Verifying internet connection.
                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            var result = await trackService.DeleteInspectionPhoto(inspectionPhotosResponseListData.ID);
                            if (result != null && result.status == 1)
                            {
                                finalPhotoListA.Remove(inspectionPhotosResponseListData);
                            }
                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Photo_Upload method -> in InspectionPhotoUploadViewModel " + Settings.userLoginID);
                await trackService.Handleexception(ex);
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
        private async void image_tap(InspectionPhotosResponseListData data)
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in image_tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                if (!multiple_Taps)
                {
                    multiple_Taps = true;
                    //var data = obj as CustomPhotoModel;
                    int photoid = data.ID;
                    //var des = (UploadType == (int)UploadTypeEnums.GoodsPhotos_AP) ? finalPhotoListA.Where(x => x.PhotoID == photoid).FirstOrDefault() : finalPhotoListB.Where(x => x.PhotoID == photoid).FirstOrDefault();
                    var imageLists = finalPhotoListA;

                    //foreach (var items in imageLists)
                    //{
                    //    if (items.PhotoDescription.Length > 150)
                    //    {
                    //        items.ShowAndHideDescr = true;
                    //        items.ShowAndHideBtn = true;
                    //        items.ShowAndHideBtnEnable = true;
                    //    }
                    //    else if (items.PhotoDescription.Length > 0)
                    //    {
                    //        items.ShowAndHideDescr = true;
                    //    }
                    //}
                    await Navigation.PushAsync(new ImageView(imageLists, data.ID, Tagnumbers, Question));
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "image_tap method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
            multiple_Taps = false;
        }

        /// <summary>
        /// Gets called when clicked on the Camera icon for uploading photo.
        /// </summary>
        /// <returns></returns>
        public async Task SelectPic()
        {
            YPSLogger.TrackEvent("InspectionPhotoUploadViewModel", "in SelectPic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                string action = await App.Current.MainPage.DisplayActionSheet("", "Cancel", null, "Camera", "Gallery");

                if (action == "Camera")
                {
                    IndicatorVisibility = true;

                    /// Checking camera is available or not in in mobile.
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
                        btnenable = false;
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
                            if (Device.RuntimePlatform == Device.Android)
                            {
                                var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                                {
                                    PhotoSize = PhotoSize.Custom,
                                    CustomPhotoSize = Settings.PhotoSize,
                                    CompressionQuality = Settings.CompressionQuality
                                });
                                if (photo == null)
                                {
                                    return;
                                }
                                IndicatorVisibility = false;
                                btnenable = true;
                                AStack = false;
                                NoPhotos_Visibility = false;
                                firstStack = true;
                                SecondMainStack = true;
                                FirstMainStack = false;
                                RowHeightOpenCam = 100;

                                picStream = photo.GetStreamWithImageRotatedForExternalStorage();
                                extension = Path.GetExtension(photo.Path);
                                Mediafile = photo.Path;
                                fileName = Path.GetFileNameWithoutExtension(photo.Path);
                                ListOfImage.Clear();
                                ListOfImage.Add(new GalleryImage()
                                {
                                    ID = 0,
                                    ImgPath = Mediafile,
                                    Fullfilename = fileName,
                                    ImageOriginalStream = picStream,
                                    Extension = extension
                                });
                            }
                            else
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
                                    btnenable = true;
                                    AStack = false;
                                    NoPhotos_Visibility = false;
                                    firstStack = true;
                                    SecondMainStack = true;
                                    FirstMainStack = false;
                                    RowHeightOpenCam = 100;

                                    picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                    extension = Path.GetExtension(file.Path);
                                    Mediafile = file.Path;
                                    fileName = Path.GetFileNameWithoutExtension(file.Path);
                                    ListOfImage.Clear();
                                    ListOfImage.Add(new GalleryImage()
                                    {
                                        ID = 0,
                                        ImgPath = Mediafile,
                                        Fullfilename = fileName,
                                        ImageOriginalStream = picStream,
                                        Extension = extension
                                    });
                                }
                                else
                                {
                                    btnenable = true;
                                }
                            }

                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                        }
                        btnenable = true;
                    }
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

                        var options = new Xamarin.Essentials.PickOptions
                        {
                            PickerTitle = "Please select images",
                            FileTypes = Xamarin.Essentials.FilePickerFileType.Images,
                        };

                        if (Device.RuntimePlatform == Device.Android)
                        {
                            var fileOS = await Xamarin.Essentials.FilePicker.PickMultipleAsync(options);

                            if (fileOS != null)
                            {
                                AStack = false;
                                NoPhotos_Visibility = false;
                                firstStack = true;
                                SecondMainStack = true;
                                FirstMainStack = false;
                                RowHeightOpenCam = 100;
                                int id = 0;
                                ListOfImage.Clear();
                                foreach (var item in fileOS)
                                {
                                    picStream = await item.OpenReadAsync();
                                    extension = Path.GetExtension(item.FullPath);
                                    fileName = Path.GetFileNameWithoutExtension(item.FullPath);
                                    ListOfImage.Add(new GalleryImage()
                                    {
                                        ID = id,
                                        ImgPath = item.FullPath,
                                        Fullfilename = fileName,
                                        ImageOriginalStream = picStream,
                                        Extension = extension,
                                        uploadplatform = "Android"
                                    });
                                    id++;
                                }
                            }
                        }
                        else
                        {
                            bool imageModifiedWithDrawings = false;
                            if (imageModifiedWithDrawings)
                            {
                                await GMMultiImagePicker.Current.PickMultiImage(true);
                            }
                            else
                            {
                                await GMMultiImagePicker.Current.PickMultiImage();
                            }

                            MessagingCenter.Unsubscribe<App, List<string>>((App)Xamarin.Forms.Application.Current, "ImagesSelectediOS");
                            MessagingCenter.Subscribe<App, List<string>>((App)Xamarin.Forms.Application.Current, "ImagesSelectediOS", (s, images) =>
                            {
                                if (images.Count > 0)
                                {
                                    if (ListOfImage.Count != 0)
                                        ListOfImage.Clear();
                                    AStack = false;
                                    NoPhotos_Visibility = false;
                                    firstStack = true;
                                    SecondMainStack = true;
                                    FirstMainStack = false;
                                    RowHeightOpenCam = 100;
                                    extension = "ios";
                                    int id = 0;
                                    foreach (var item in images)
                                    {
                                        var filename = item.Split('/').Last();
                                        ListOfImage.Add(new GalleryImage() { ID = id,Extension=".jpg", ImgPath = item, Fullfilename = filename, uploadplatform = "ios" });
                                        id++;
                                    }

                                    //ImgCarouselView.ItemsSource = images;
                                    //InfoText.IsVisible = true; //InfoText is optional
                                }
                            });
                        }
                    }
                    else
                    {
                        btnenable = true;
                    }
                }
                IndicatorVisibility = false;

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectPic method -> in InspectionPhotoUploadViewModel " + Settings.userLoginID);
                await trackService.Handleexception(ex);
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
        public async Task Photo_Upload()
        {
            YPSLogger.TrackEvent("InspectionPhotoUploadViewModel", "in Photo_Upload method " + DateTime.Now + " UserId: " + Settings.userLoginID);


            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IndicatorVisibility = true;
                    /// Verifying internet connection.
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        AStack = true;
                        SecondMainStack = true;
                        NoPhotos_Visibility = false;
                        firstStack = false;
                        FirstMainStack = false;

                        if (extension.Trim().ToLower() == ".png" || extension.Trim().ToLower() == ".jpg" || extension.Trim().ToLower() == ".jpeg" || extension.Trim().ToLower() == ".gif" || extension.Trim().ToLower() == ".bmp" || extension.Trim().ToLower() == "ios")
                        {
                            description_txt = Regex.Replace(description_txt, @"\s+", " ");
                            var listOfFiles = new List<UpdateInspectionRequest>();
                            foreach (var item in ListOfImage)
                            {
                                string FullFilename = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + item.Extension;
                                BlobUpload.UploadFile(CloudFolderKeyVal.GetBlobFolderName((int)BlobContainer.cnttagfiles), FullFilename, item.ImageOriginalStream, item.ImgPath, item.uploadplatform);
                                UpdateInspectionRequest updateInspectionRequest = new UpdateInspectionRequest()
                                {
                                    BackLeft = inspectionConfiguration.BackLeft,
                                    BackRight = inspectionConfiguration.BackRight,
                                    Direct = inspectionConfiguration.BackRight,
                                    FileName = item.Fullfilename,
                                    FileURL = FullFilename,
                                    FrontLeft = inspectionConfiguration.FrontLeft,
                                    FrontRight = inspectionConfiguration.FrontRight,
                                    POTagID = tagId,
                                    FileDescription = description_txt,
                                    QID = inspectionConfiguration.MInspectionConfigID,
                                    UserID = Settings.userLoginID
                                };
                                listOfFiles.Add(updateInspectionRequest);
                            }

                            await trackService.InsertInspectionPhotosService(listOfFiles);
                            await GetInspectionPhotos();
                        }
                    }
                    else
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");

                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "Photo_Upload method -> in InspectionPhotoUploadViewModel " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    IndicatorVisibility = false;
                }
            });
        }

        private async Task GetInspectionPhotos()
        {
            IndicatorVisibility = true;
            var checkInternet = await App.CheckInterNetConnection();
            if (checkInternet)
            {
                var result = await trackService.GeInspectionPhotosService(tagId, inspectionConfiguration?.MInspectionConfigID);
                if (result != null && result.data != null && result.data.listData != null && result.data.listData.Count > 0)
                {
                    AStack = true;
                    FirstMainStack = true;
                    SecondMainStack = false;
                    NoPhotos_Visibility = false;
                    firstStack = false;
                    finalPhotoListA = new ObservableCollection<InspectionPhotosResponseListData>(result.data.listData);
                }
                else
                {
                    AStack = false;
                    FirstMainStack = true;
                    NoPhotos_Visibility = true;
                    firstStack = false;
                    SecondMainStack = false;
                }
            }

            IndicatorVisibility = false;
        }


        #region Properties

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

        public string _Question;
        public string Question
        {
            get { return _Question; }
            set
            {
                _Question = value;
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

        private ObservableCollection<InspectionPhotosResponseListData> _finalPhotoList_A;
        public ObservableCollection<InspectionPhotosResponseListData> finalPhotoListA
        {
            get { return _finalPhotoList_A; }
            set
            {
                _finalPhotoList_A = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<GalleryImage> _ListOfImage;
        public ObservableCollection<GalleryImage> ListOfImage
        {
            get { return _ListOfImage; }
            set
            {
                _ListOfImage = value;
                NotifyPropertyChanged();
            }
        }

        public bool _NoPhotos_Visibility = false;
        public bool NoPhotos_Visibility
        {
            get { return _NoPhotos_Visibility; }
            set
            {
                _NoPhotos_Visibility = value;
                NotifyPropertyChanged();
            }
        }

        private bool _firstStack = false;
        public bool firstStack
        {
            get { return _firstStack; }
            set
            {
                _firstStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _btnenable = true;
        public bool btnenable
        {
            get { return _btnenable; }
            set
            {
                _btnenable = value;
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

        private string _labelobjDesc = "Description";
        public string labelobjDesc
        {
            get => _labelobjDesc;
            set
            {
                _labelobjDesc = value;
                NotifyPropertyChanged();
            }
        }

        private string _labelobjUploadBtn = "Upload";
        public string labelobjUploadBtn
        {
            get => _labelobjUploadBtn;
            set
            {
                _labelobjUploadBtn = value;
                NotifyPropertyChanged();
            }
        }


        #endregion

    }

    public class GalleryImage
    {
        public int ID { get; set; }
        public string ImgPath { get; set; }
        public Stream ImageOriginalStream { get; set; }
        public string Fullfilename { get; set; }
        public string Extension { get; set; }

        public string uploadplatform { get; set; }///Added ajay

    }
}
