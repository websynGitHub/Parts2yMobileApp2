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
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using YPS.Views;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class InspectionPhotoUploadViewModel : IBase
    {
        #region IComman and data members declaration
        public ICommand select_pic { set; get; }
        public ICommand upload_pic { set; get; }
        public INavigation Navigation { get; set; }
        public ICommand tap_OnImge { set; get; }
        public ICommand deleteImage { set; get; }

        YPSService trackService;
        InspectionPhotosPage pagename;
        AllPoData selectedTagData;
        Stream picStream;
        string extension = "", fileName, Mediafile, types = string.Empty, pendingTagIDs;
        int tagId, taskid;
        bool multiple_Taps, IsCarrierInsp, isInspVIN;
        InspectionConfiguration inspectionConfiguration;
        #endregion

        public InspectionPhotoUploadViewModel(INavigation _Navigation, InspectionPhotosPage page, int tagId,
            InspectionConfiguration inspectionConfiguration, string vinValue, AllPoData selectedtagdata, bool iscarrierinsp,
            bool isvininsp, string pendingtagIDs = null)
        {
            try
            {
                Navigation = _Navigation;
                pagename = page;
                selectedTagData = selectedtagdata;
                IsTagNumberVisible = isInspVIN = isvininsp;
                pendingTagIDs = pendingtagIDs;
                this.tagId = tagId;
                taskid = selectedtagdata.TaskID;
                Tagnumbers = vinValue;
                IsCarrierInsp = iscarrierinsp;
                Question = inspectionConfiguration.SerialNo + " " + inspectionConfiguration.Question;
                TaskName = selectedtagdata.TaskName;
                EventName = selectedtagdata.EventName;
                this.inspectionConfiguration = inspectionConfiguration;
                trackService = new YPSService();
                ListOfImage = new ObservableCollection<GalleryImage>();
                Task.Run(() => DynamicTextChange());
                Task.Run(() => GetInspectionPhotos());
                select_pic = new Command(async () => await SelectPic());
                upload_pic = new Command(async () => await Photo_Upload());
                deleteImage = new Command<InspectionPhotosResponseListData>(Delete_Photo);
                tap_OnImge = new Command<InspectionPhotosResponseListData>(image_tap);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "InspectionPhotoUploadViewModel constructor -> in InspectionPhotoUploadViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        private async void Delete_Photo(InspectionPhotosResponseListData inspectionPhotosResponseListData)
        {
            YPSLogger.TrackEvent("InspectionPhotoUploadViewModel.cs", " in DeleteImage method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                if (DeleteIconOpacity == 1.0)
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
                                    await GetInspectionPhotos();
                                }
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                            }
                        });
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Delete_Photo method -> in InspectionPhotoUploadViewModel.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("InspectionPhotoUploadViewModel.cs", " in image_tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                if (!multiple_Taps)
                {
                    multiple_Taps = true;
                    int photoid = data.ID;
                    var imageLists = finalPhotoListA;

                    await Navigation.PushAsync(new ImageView(imageLists, data.ID, Tagnumbers, Question, isInspVIN), false);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "image_tap method -> in InspectionPhotoUploadViewModel.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("InspectionPhotoUploadViewModel.cs ", "in SelectPic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                if (CamIconOpacity == 1.0)
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
                                    IsCamVisible = false;
                                    NoPhotos_Visibility = false;
                                    firstStack = true;
                                    SecondMainStack = true;
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
                                        IsCamVisible = false;
                                        NoPhotos_Visibility = false;
                                        firstStack = true;
                                        SecondMainStack = true;
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
                                    IsCamVisible = false;
                                    NoPhotos_Visibility = false;
                                    firstStack = true;
                                    SecondMainStack = true;
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
                                        IsCamVisible = false;
                                        NoPhotos_Visibility = false;
                                        firstStack = true;
                                        SecondMainStack = true;
                                        RowHeightOpenCam = 100;
                                        extension = "ios";
                                        int id = 0;
                                        foreach (var item in images)
                                        {
                                            var filename = item.Split('/').Last();
                                            ListOfImage.Add(new GalleryImage() { ID = id, Extension = ".jpg", ImgPath = item, Fullfilename = filename, uploadplatform = "ios" });
                                            id++;
                                        }
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
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                }

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectPic method -> in InspectionPhotoUploadViewModel.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("InspectionPhotoUploadViewModel.cs", " in Photo_Upload method " + DateTime.Now + " UserId: " + Settings.userLoginID);


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
                        IsCamVisible = true;
                        NoPhotos_Visibility = false;
                        firstStack = false;

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
                                    POTagID = IsCarrierInsp == false ? tagId : 0,
                                    TaskID = taskid,
                                    FileDescription = description_txt,
                                    QID = inspectionConfiguration.MInspectionConfigID,
                                    UserID = Settings.userLoginID
                                };
                                listOfFiles.Add(updateInspectionRequest);
                            }

                            RowHeightOpenCam = 0;
                            description_txt = string.Empty;
                            await trackService.InsertInspectionPhotosService(listOfFiles);
                            await GetInspectionPhotos();
                            await DoneClicked();
                        }
                    }
                    else
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");

                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "Photo_Upload method -> in InspectionPhotoUploadViewModel.cs " + Settings.userLoginID);
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
            try
            {
                IndicatorVisibility = true;
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (IsCarrierInsp == true)
                    {
                        var result = await trackService.GeInspectionPhotosByTask(taskid, inspectionConfiguration?.MInspectionConfigID);

                        if (result != null && result.data != null && result.data.listData != null && result.data.listData.Count > 0)
                        {
                            AStack = true;
                            //FirstMainStack = true;
                            SecondMainStack = false;
                            NoPhotos_Visibility = false;
                            firstStack = false;
                            finalPhotoListA = new ObservableCollection<InspectionPhotosResponseListData>(result.data.listData);
                        }
                        else
                        {
                            AStack = false;
                            //FirstMainStack = true;
                            NoPhotos_Visibility = true;
                            firstStack = false;
                            SecondMainStack = false;
                        }
                    }
                    else
                    {
                        var vinresult = await trackService.GeInspectionPhotosByTag(taskid, tagId, inspectionConfiguration?.MInspectionConfigID);

                        if (vinresult != null && vinresult.data != null && vinresult.data.listData != null && vinresult.data.listData.Count > 0)
                        {
                            AStack = true;
                            //FirstMainStack = true;
                            SecondMainStack = false;
                            NoPhotos_Visibility = false;
                            firstStack = false;
                            finalPhotoListA = new ObservableCollection<InspectionPhotosResponseListData>(vinresult.data.listData);
                        }
                        else
                        {
                            AStack = false;
                            //FirstMainStack = true;
                            NoPhotos_Visibility = true;
                            firstStack = false;
                            SecondMainStack = false;
                        }
                    }

                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }

                IndicatorVisibility = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetInspectionPhotos method -> in InspectionPhotoUploadViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        private async Task DoneClicked()
        {
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    //if (selectedTagData.TaskID != 0 && selectedTagData.TagTaskStatus == 0)
                    if (selectedTagData.TaskID != 0 && ((isInspVIN == false && !string.IsNullOrEmpty(pendingTagIDs)) ||
                        (isInspVIN == true && selectedTagData.TagTaskStatus == 0)))
                    {
                        TagTaskStatus tagtaskstatus = new TagTaskStatus();
                        tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                        tagtaskstatus.POTagID = !string.IsNullOrEmpty(pendingTagIDs) ? string.Join(",", pendingTagIDs) : Helperclass.Encrypt(selectedTagData.POTagID.ToString());
                        //tagtaskstatus.POTagID = Helperclass.Encrypt(selectedTagData.POTagID.ToString());
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
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked method -> in InspectionPhotoUploadViewModel.cs  " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }


        /// <summary>
        /// Dynamic text changed
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
                        var uploadBtn = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjUploadBtn.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var desc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjDesc.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();

                        labelobjUploadBtn = uploadBtn != null ? (!string.IsNullOrEmpty(uploadBtn) ? uploadBtn : labelobjUploadBtn) : labelobjUploadBtn;
                        labelobjDesc = desc != null ? (!string.IsNullOrEmpty(desc) ? desc : labelobjDesc) : labelobjDesc;
                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    DeleteIconOpacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoDelete".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    CamIconOpacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method -> in InspectionPhotoUploadViewModel.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        #region Properties

        public bool _IsTagNumberVisible;
        public bool IsTagNumberVisible
        {
            get { return _IsTagNumberVisible; }
            set
            {
                _IsTagNumberVisible = value;
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

        private double _DeleteIconOpacity = 1.0;
        public double DeleteIconOpacity
        {
            get => _DeleteIconOpacity;
            set
            {
                _DeleteIconOpacity = value;
                NotifyPropertyChanged();
            }
        }

        private double _CamIconOpacity = 1.0;
        public double CamIconOpacity
        {
            get { return _CamIconOpacity; }
            set
            {
                _CamIconOpacity = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsCamVisible = true;
        public bool IsCamVisible
        {
            get { return _IsCamVisible; }
            set
            {
                _IsCamVisible = value;
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
