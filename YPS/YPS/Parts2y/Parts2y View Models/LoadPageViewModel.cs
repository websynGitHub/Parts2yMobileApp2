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
        bool checkInternet, isAllTasksDone;
        int puid, poid, selectiontype_index, photoCounts = 0;
        AllPoData selectedTagData;
        Stream picStream;
        SendPodata SendPodData = new SendPodata();
        string extension = "", Mediafile, fileName;
        public Command select_pic { set; get; }
        public Command CloseCommand { set; get; }
        public Command upload_pic { set; get; }
        public ICommand ViewPhotoDetailsCmd { set; get; }
        public ICommand DeleteImageCmd { set; get; }
        public Command HomeCommand { get; set; }
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }

        public LoadPageViewModel(INavigation navigation, AllPoData selectedtagdata, SendPodata sendpodata, bool isalltasksdone, LoadPage pagename)
        {
            try
            {
                service = new YPSService();
                Navigation = navigation;
                SendPodData = sendpodata;
                IsNoPhotoTxt = true;
                isAllTasksDone = isalltasksdone;
                selectedTagData = selectedtagdata;
                Tagnumbers = selectedtagdata.TaskName;
                select_pic = new Command(async () => await SelectPic());
                upload_pic = new Command(async () => await UploadPhoto());
                ViewPhotoDetailsCmd = new Command(ViewPhotoDetails);
                DeleteImageCmd = new Command(DeleteImage);
                HomeCommand = new Command(HomeCommand_btn);

                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                PartsCmd = new Command(async () => await TabChange("parts"));
                LoadCmd = new Command(async () => await TabChange("load"));

                Task.Run(() => DynamicTextChange()).Wait();

                Task.Run(() => GetPhotosData(selectedtagdata.TaskID)).Wait();

                if (selectedTagData.TaskStatus == 2)
                {
                    closeLabelText = true;
                    DoneBtnOpacity = 0.5;
                    NotDoneVal = false;
                    IsPhotoUploadIconVisible = false;
                }
                else
                {
                    if (isalltasksdone == true)
                    {
                        IsPhotoUploadIconVisible = true;
                        closeLabelText = true;
                    }
                    else
                    {
                        closeLabelText = false;
                        IsPhotoUploadIconVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LoadPageViewModel constructor -> in LoadPageViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = service.Handleexception(ex);
            }
        }

        private async Task<ObservableCollection<AllPoData>> GetUpdatedAllPOData()
        {
            ObservableCollection<AllPoData> AllPoDataList = new ObservableCollection<AllPoData>();

            try
            {
                IndicatorVisibility = true;
                YPSLogger.TrackEvent("LoadPageViewModel.cs", "in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var result = await service.LoadPoDataService(SendPodData);

                    if (result != null && result.data != null)
                    {
                        if (result.status == 1 && result.data.allPoDataMobile != null && result.data.allPoDataMobile.Count > 0)
                        {
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoDataMobile.Where(wr => wr.TaskID == selectedTagData.TaskID));
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
                YPSLogger.ReportException(ex, "GetUpdatedAllPOData method -> in LoadPageViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
            return AllPoDataList;
        }

        private async Task TabChange(string tabname)
        {
            try
            {
                IndicatorVisibility = true;

                if (tabname == "home")
                {
                    await Navigation.PopToRootAsync(false);
                    //App.Current.MainPage = new MenuPage(typeof(HomePage));
                }
                else if (tabname == "job")
                {
                    Navigation.RemovePage(Navigation.NavigationStack[2]);

                    await Navigation.PopAsync(false);
                }
                else if (tabname == "parts")
                {
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TabChange method -> in LoadPageViewModel " + Settings.userLoginID);
                var trackResult = service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// Get the existing uploaded photo(s).
        /// </summary>
        /// <param name="potagid"></param>
        /// <returns></returns>
        public async Task GetPhotosData(int taskid)
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
                        var result = await service.GetLoadPhotos(taskid);

                        if (result != null && result.status == 1 && result.data.Count != 0)
                        {
                            IsPhotosListVisible = true;
                            IsPhotosListStackVisible = true;
                            IsNoPhotoTxt = false;

                            if (selectedTagData.TaskStatus != 2 && isAllTasksDone == true)
                            {
                                closeLabelText = true;
                            }
                            else
                            {
                                closeLabelText = false;
                            }

                            LoadPhotosList = new ObservableCollection<LoadPhotoModel>(result.data);
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
                finally
                {
                    IndicatorVisibility = false;
                }
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


                foreach (var items in imageLists)
                {
                    if (items.PhotoDescription.Length > 150)
                    {
                        items.ShowAndHideDescr = true;
                        items.ShowAndHideBtn = true;
                        items.ShowAndHideBtnEnable = true;
                    }
                    else if (items.PhotoDescription.Length > 0)
                    {
                        items.ShowAndHideDescr = true;
                    }
                }

                await Navigation.PushAsync(new ImageView(imageLists, photoid, Tagnumbers), false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ViewPhotoDetails method -> in LoadPageViewModel " + Settings.userLoginID);
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
                bool conform = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure want to delete?", "Ok", "Cancel");

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
                                    if (uploadresult.status == 1)
                                    {
                                        var item = LoadPhotosList.Where(x => x.PhotoID == findData.PhotoID).FirstOrDefault();
                                        LoadPhotosList.Remove(item);

                                        if (LoadPhotosList.Count == 0)
                                        {
                                            IsNoPhotoTxt = true;
                                        }

                                        await GetPhotosData(selectedTagData.TaskID);

                                        await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "Ok");
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
                            YPSLogger.ReportException(ex, "delete_image method from if(conform) -> in LoadPageViewModel " + Settings.userLoginID);
                            await service.Handleexception(ex);
                        }

                        IndicatorVisibility = false;
                    });
                }
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
                        if (isAllTasksDone == true)
                        {
                            IsPhotoUploadIconVisible = true;
                        }
                        else
                        {
                            IsPhotoUploadIconVisible = false;
                        }

                        RowHeightOpenCam = 0;
                        IsUploadStackVisible = IsImageViewForUploadVisible = false;
                        IsTabsVisible = true;
                        IsPhotosListStackVisible = true;
                        IsNoPhotoTxt = false;

                        if (extension.Trim().ToLower() == ".png" || extension.Trim().ToLower() == ".jpg" || extension.Trim().ToLower() == ".jpeg" || extension.Trim().ToLower() == ".gif" || extension.Trim().ToLower() == ".bmp")
                        {
                            DescriptionText = Regex.Replace(DescriptionText, @"\s+", " ");

                            if (selectedTagData != null && selectedTagData.POTagID != 0)
                            {
                                List<LoadPhotoModel> loadPhotosList = new List<LoadPhotoModel>();

                                LoadPhotoModel loadphoto = new LoadPhotoModel();
                                loadphoto.TaskID = selectedTagData.TaskID;
                                loadphoto.UploadType = (int)UploadTypeEnums.TagLoadPhotos;
                                loadphoto.PhotoURL = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                                loadphoto.FileName = fileName;
                                loadphoto.PhotoDescription = DescriptionText;
                                loadphoto.CreatedBy = Settings.userLoginID;
                                loadphoto.CreatedDate = String.Format(Settings.DateFormat, DateTime.Now);
                                loadphoto.FullName = Settings.Username;
                                loadphoto.PicStream = picStream;

                                loadPhotosList.Add(loadphoto);
                                /// Calling the blob API to upload photo. 
                                var initialdata = await BlobUpload.YPSFileUpload((int)UploadTypeEnums.TagLoadPhotos, (int)BlobContainer.cnttagfiles, null, null, null,
                                    null, null, loadPhotosList);

                                var initialresult = initialdata as LoadPhotosUploadResponse;
                                if (initialresult != null)
                                {
                                    if (initialresult.status == 1)
                                    {
                                        selectiontype_index = 1;


                                        if (selectedTagData.TaskID != 0 && selectedTagData.TagTaskStatus == 0)
                                        {
                                            TagTaskStatus tagtaskstatus = new TagTaskStatus();
                                            tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                                            tagtaskstatus.POTagID = Helperclass.Encrypt(selectedTagData.POTagID.ToString());
                                            tagtaskstatus.Status = 1;
                                            tagtaskstatus.CreatedBy = Settings.userLoginID;

                                            var result = await service.UpdateTagTaskStatus(tagtaskstatus);
                                        }

                                        Settings.IsRefreshPartsPage = true;
                                        bool makeitdone = await App.Current.MainPage.DisplayAlert("Success", "", "Ok", "");
                                        //DependencyService.Get<IToastMessage>().ShortAlert("Success."); ;
                                    }
                                }
                            }

                            DescriptionText = string.Empty;

                            await GetPhotosData(selectedTagData.TaskID);

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
                        /// Request permission a user to allowed take photos from the camera.
                        var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        var statusiOS = resultIOS[Permission.Camera];

                        /// Checking permission is allowed or denied by the user to access the photo from mobile.
                        if (statusiOS == PermissionStatus.Denied)
                        {
                            var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needed to access the camera.", null, null, "Maybe later", "Settings");
                            switch (checkSelect)
                            {
                                case "Maybe later":
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

                                    closeLabelText = false;
                                    IsUploadStackVisible = true;
                                    IsTabsVisible = false;
                                }
                                else
                                {
                                    closeLabelText = false;
                                }

                                extension = Path.GetExtension(file.Path);
                                Mediafile = file.Path;
                                picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(file.Path);
                                IsPhotoUploadIconVisible = false;
                                IsPhotosListVisible = false;
                                IsPhotosListStackVisible = false;
                                IsNoPhotoTxt = false;
                                IsImageViewForUploadVisible = true;
                                RowHeightOpenCam = 100;
                            }
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable", "Ok");
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
                        var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needed to access the gallery.", null, null, "Maybe later", "Settings");
                        switch (checkSelect)
                        {
                            case "Maybe later":
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
                                    IsTabsVisible = false;
                                }

                                closeLabelText = false;
                                IsPhotoUploadIconVisible = false;
                                RowHeightOpenCam = 100;
                                IsPhotosListVisible = false;
                                IsPhotosListStackVisible = false;
                                IsNoPhotoTxt = false;
                                IsImageViewForUploadVisible = true;
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
                YPSLogger.ReportException(ex, "SelectPic method -> in LoadPageViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }


        private async void HomeCommand_btn(object obj)
        {
            try
            {
                await Navigation.PopToRootAsync(false);
                //App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HomeCommand_btn method -> in LoadPageViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
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


                        labelobj.Done.Name = (done != null ? (!string.IsNullOrEmpty(done.LblText) ? done.LblText : labelobj.Done.Name) : labelobj.Done.Name);
                        labelobj.Done.Status = done == null ? true : (done.Status == 1 ? true : false);
                        labelobj.Upload.Name = (upload != null ? (!string.IsNullOrEmpty(upload.LblText) ? upload.LblText : labelobj.Upload.Name) : labelobj.Upload.Name);
                        labelobj.Upload.Status = upload == null ? true : (upload.Status == 1 ? true : false);
                        DescriptipnPlaceholder = desc != null ? (!string.IsNullOrEmpty(desc) ? desc : DescriptipnPlaceholder) : DescriptipnPlaceholder;

                        labelobj.Load.Name = Settings.VersionID == 2 ? "Insp" : "Load";
                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                    }

                    if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                    {
                        DeleteIconStack = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoDelete".Trim()).FirstOrDefault()) != null ? true : false;

                        if (isAllTasksDone == true)
                        {
                            IsPhotoUploadIconVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? true : false;
                        }
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

        private bool _IsTabsVisible = true;
        public bool IsTabsVisible
        {
            get { return _IsTabsVisible; }
            set
            {
                _IsTabsVisible = value;
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
