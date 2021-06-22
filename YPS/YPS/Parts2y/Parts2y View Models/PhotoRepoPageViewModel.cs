using Newtonsoft.Json;
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
using static YPS.Model.SearchModel;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class PhotoRepoPageViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        YPSService service;
        PhotoRepoPage pagename;
        SendPodata sendPodata;
        Stream picStream;
        int puid, poid, selectiontype_index, photoCounts = 0;
        string extension = "", Mediafile, fileName, fullFilename;
        public ICommand DeleteAllCmd { set; get; }
        public ICommand MoveLinkCmd { set; get; }
        public ICommand ViewPhotoDetailsCmd { set; get; }
        public ICommand DeleteImageCmd { set; get; }
        public ICommand CheckedChangedCmd { set; get; }
        public Command select_pic { set; get; }
        public Command upload_pic { set; get; }
        public int taskID { get; set; }

        public PhotoRepoPageViewModel(INavigation navigation, PhotoRepoPage page)
        {
            try
            {
                YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in PhotoRepoPageViewModel constructor" + DateTime.Now + " UserId: " + Settings.userLoginID);

                service = new YPSService();
                Navigation = navigation;
                pagename = page;
                DeleteImageCmd = new Command(DeleteImage);
                DeleteAllCmd = new Command(DeleteImage);
                CheckedChangedCmd = new Command(CheckedChanged);
                select_pic = new Command(async () => await SelectPic());
                upload_pic = new Command(async () => await UploadPhoto());
                MoveLinkCmd = new Command(async () => await ShowContentsToLink());
                ViewPhotoDetailsCmd = new Command(ViewPhotoDetails);

                Task.Run(() => DynamicTextChange().Wait());
                Task.Run(() => GetPhotosData().Wait());
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PhotoRepoPageViewModel Constructor  -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void CheckedChanged(object sender)
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in CheckedChanged method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var val = sender as Plugin.InputKit.Shared.Controls.CheckBox;
                var item = (PhotoRepoDBModel)val.BindingContext;
                item.IsSelected = val.IsChecked == true ? true : false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckedChanged method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
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
        private async Task ShowContentsToLink()
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in ShowContentsToLink method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {

                if (RepoPhotosList != null)
                {
                    if ((RepoPhotosList.Where(wr => wr.IsSelected == true).FirstOrDefault()) != null)
                    {
                        bool move = await App.Current.MainPage.DisplayAlert("Move photo(s) for linking", "Are you sure to move the selected photo(s) for linking?", "Yes", "No");

                        if (move)
                        {
                            await Navigation.PushAsync(new LinkPage(new ObservableCollection<PhotoRepoDBModel>(RepoPhotosList.Where(wr => wr.IsSelected == true).ToList())));
                        }
                    }
                    else
                    {
                        bool move = await App.Current.MainPage.DisplayAlert("Move photo(s) for linking", "Are you sure to move all photo(s) for linking?", "Yes", "No");

                        if (move)
                        {
                            await Navigation.PushAsync(new LinkPage(RepoPhotosList));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShowContentsToLink method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
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
        private async void ViewPhotoDetails(object obj)
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in ViewPhotoDetails method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var data = obj as PhotoRepoDBModel;
                int photoid = data.FileID;
                var des = RepoPhotosList.Where(x => x.FileID == photoid).FirstOrDefault();
                var imageLists = RepoPhotosList;


                foreach (var items in imageLists)
                {
                    if (items.FileDescription.Length > 150)
                    {
                        items.ShowAndHideDescr = true;

                    }
                    else if (items.FileDescription.Length > 0)
                    {
                        items.ShowAndHideDescr = true;
                    }
                }

                await Navigation.PushAsync(new ImageView(imageLists, photoid));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ViewPhotoDetails method -> in PhotoRepoPageViewModel.cs" + Settings.userLoginID);
                await service.Handleexception(ex);
                IndicatorVisibility = false;
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
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in DeleteImage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            IndicatorVisibility = true;
            try
            {
                bool conform = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure want to delete?", "OK", "Cancel");

                if (conform)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        IndicatorVisibility = true;
                        try
                        {
                            /// Verifying internet connection.
                            var checkInternet = await App.CheckInterNetConnection();

                            if (checkInternet)
                            {
                                var data = obj as PhotoRepoDBModel;
                                bool update = false;

                                if (data != null)
                                {
                                    var response = await service.DeleteSingleRepoPhoto(data.FileID);

                                    if (response.status == 1)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "OK");
                                        update = true;
                                    }
                                }
                                else
                                {
                                    var allresponse = await service.DeleteAllRepoPhoto();

                                    if (allresponse.status == 1)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "OK");
                                        update = true;
                                    }
                                }

                                NoRecHeight = (IsNoPhotoTxt = RepoPhotosList.Count == 0 ? true : false) == true ? 30 : 0;


                                if (update)
                                {
                                    await GetPhotosData();
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
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DeleteImage method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Get the existing uploaded photo(s).
        /// </summary>
        /// <param name="potagid"></param>
        /// <returns></returns>
        public async Task GetPhotosData()
        {

            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in GetPhotosData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IndicatorVisibility = true;
                    /// Verifying internet connection.
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        var data = await service.GetRepoPhotos();

                        if (data != null && data.status == 1 && data.data.Count > 0)
                        {
                            IsPhotosListVisible = true;
                            IsPhotosListStackVisible = true;
                            IsNoPhotoTxt = false;
                            NoRecHeight = 0;
                            IsBottomButtonsVisible = true;
                            IsLinkEnable = true;
                            LinkOpacity = 1.0;
                            IsPhotoUploadIconVisible = true;
                            IsDeleteAllEnable = data.data.Count > 2 ? true : false;
                            DeleteAllOpacity = data.data.Count > 2 ? 1.0 : 0.5;

                            foreach (var val in data.data)
                            {
                                val.FullFileUrl = HostingURL.blob + "tag-files/" + val.FileUrl;
                            }

                            RepoPhotosList = new ObservableCollection<PhotoRepoDBModel>(data.data);
                        }
                        else
                        {
                            IsPhotosListVisible = false;
                            IsPhotosListStackVisible = false;
                            IsNoPhotoTxt = true;
                            NoRecHeight = 30;
                            IsBottomButtonsVisible = false;
                            IsDeleteAllEnable = IsLinkEnable = false;
                            DeleteAllOpacity = LinkOpacity = 0.5;
                            IsPhotoUploadIconVisible = true;
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "GetPhotosData method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                    await service.Handleexception(ex);
                    IndicatorVisibility = false;
                }
                IndicatorVisibility = false;
            });
        }

        /// <summary>
        /// Gets called when clicked on Upload Button.
        /// </summary>
        /// <returns></returns>
        public async Task UploadPhoto()
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in UploadPhoto method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                IndicatorVisibility = true; ;
                try
                {
                    /// Verifying internet connection.
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        RowHeightOpenCam = 50;
                        IsUploadStackVisible = IsImageViewForUploadVisible = false;
                        IsPhotosListStackVisible = true;

                        if (extension.Trim().ToLower() == ".png" || extension.Trim().ToLower() == ".jpg" || extension.Trim().ToLower() == ".jpeg" || extension.Trim().ToLower() == ".gif" || extension.Trim().ToLower() == ".bmp")
                        {
                            DescriptionText = Regex.Replace(DescriptionText, @"\s+", " ");

                            string FullFilename = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                            BlobUpload.UploadFile(CloudFolderKeyVal.GetBlobFolderName((int)BlobContainer.cnttagfiles), FullFilename, picStream);
                            selectiontype_index = 1;

                            PhotoRepoDBModel photoobj = new PhotoRepoDBModel();
                            List<PhotoRepoDBModel> photolistobj = new List<PhotoRepoDBModel>();
                            photoobj.FullName = FullFilename;
                            photoobj.FileName = fileName;
                            photoobj.FileUrl = FullFilename;
                            photoobj.FileDescription = DescriptionText;
                            photoobj.CreatedDate = String.Format(Settings.DateFormat, DateTime.Now);
                            photoobj.CreatedBy = Settings.userLoginID;
                            photolistobj.Add(photoobj);

                            var response = await service.UploadRepoPhotos(photolistobj);
                            var data = response as GetRepoPhotoResponse;

                            DescriptionText = string.Empty;

                            await GetPhotosData();

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
                                        YPSLogger.ReportException(ex, "UploadPhoto method -> in PhotoRepoPageViewModel " + Settings.userLoginID);
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
                                        YPSLogger.ReportException(ex, "while deleting the folder in Photo_Upload method -> in PhotoRepoPageViewModel " + Settings.userLoginID);
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
                                        YPSLogger.ReportException(ex, "while deleting the each files in Parts2y folder -> Photo_Upload method -> in PhotoRepoPageViewModel " + Settings.userLoginID);
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
                    YPSLogger.ReportException(ex, "UploadPhoto method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                    await service.Handleexception(ex);
                    IndicatorVisibility = false;
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
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in SelectPic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                string action = await App.Current.MainPage.DisplayActionSheet("", "Cancel", null, "Camera", "Gallery");

                if (action == "Camera")
                {
                    IndicatorVisibility = true;

                    /// Checking camera is available or not in in mobile.
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
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

                                if (photoCounts == 0)
                                {
                                    ImageViewForUpload = ImageSource.FromStream(() =>
                                    {
                                        return file.GetStreamWithImageRotatedForExternalStorage();
                                    });

                                    DeleteAllPhotos = false;
                                    IsBottomButtonsVisible = false;
                                    IsUploadStackVisible = true;
                                }
                                else
                                {
                                    DeleteAllPhotos = false;
                                    IsBottomButtonsVisible = false;
                                }

                                extension = Path.GetExtension(file.Path);
                                Mediafile = file.Path;
                                picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(file.Path);
                                IsPhotoUploadIconVisible = false;
                                IsPhotosListVisible = false;
                                IsPhotosListStackVisible = false;
                                IsNoPhotoTxt = false;
                                NoRecHeight = 0;
                                RowHeightOpenCam = 100;
                                IsImageViewForUploadVisible = true;
                            }
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
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
                                    IsUploadStackVisible = true;
                                }

                                DeleteAllPhotos = false;
                                IsNoPhotoTxt = false;
                                NoRecHeight = 0;
                                IsBottomButtonsVisible = false;
                                IsPhotoUploadIconVisible = false;
                                RowHeightOpenCam = 100;
                                IsImageViewForUploadVisible = true;
                                IsPhotosListVisible = false;
                                IsPhotosListStackVisible = false;
                                extension = Path.GetExtension(fileOS.Path);
                                picStream = fileOS.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(fileOS.Path);
                            });
                        }
                    }

                }
                IndicatorVisibility = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectPic method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
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
                YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in DynamicTextChange method " + DateTime.Now + " UserId: " + Settings.userLoginID);


                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var upload = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Upload.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var desc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == DescriptipnPlaceholder.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.Upload.Name = (upload != null ? (!string.IsNullOrEmpty(upload.LblText) ? upload.LblText : labelobj.Upload.Name) : labelobj.Upload.Name);
                        labelobj.Upload.Status = upload == null ? true : (upload.Status == 1 ? true : false);
                        DescriptipnPlaceholder = desc != null ? (!string.IsNullOrEmpty(desc) ? desc : DescriptipnPlaceholder) : DescriptipnPlaceholder;
                    }

                    if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                    {
                        DeleteIconStack = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoDelete".Trim()).FirstOrDefault()) != null ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        #region Properties
        #region Properties for dynamic label change
        public class LabelAndActionsChangeClass
        {
            public LabelAndActionFields Upload { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Upload"
            };
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

        private string _Title = "Photo Repo";
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

        private bool _IsImageViewForUploadVisible = false;
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
