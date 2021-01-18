using System;
using System.Collections.ObjectModel;
using YPS.CommonClasses;
using YPS.Model;
using YPS.Service;
using System.IO;
using Xamarin.Forms;
using YPS.CustomToastMsg;
using YPS.Helpers;
using System.Windows.Input;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Linq;

namespace YPS.ViewModel
{
    public class MobileBuildsViewModel : IBase
    {
        #region ICommand and data members declaration.
        IDownloader downloader = DependencyService.Get<IDownloader>();
        YPSService yPSService;
        public ICommand DownloadFileICommand { get; set; }
        public ICommand DescriptionICommand { get; set; }
        bool checkInternet;
        #endregion

        /// <summary>
        /// Parameter less constructor.
        /// </summary>
        public MobileBuildsViewModel()
        {
            try
            {
                yPSService = new YPSService();
                DownloadFileICommand = new Command(DownloadFileClick);
                DescriptionICommand = new Command(DescriptionClick);
                GetMobileBuildsData();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetMobileBuildsData constructor-> in MobileBuildsViewModel " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method will get data from API and bind the list view.
        /// </summary>
        private async void GetMobileBuildsData()
        {
            try
            {
                IndicatorVisibility = true;
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    ObservableCollection<MobileBuildsModel> addItems = new ObservableCollection<MobileBuildsModel>();
                    GetMobileBData buildsModel = new GetMobileBData();
                    buildsModel.UserID = Settings.userLoginID;

                    /// Calling mobile builds API to get data.
                    var mobileData = await yPSService.GetMobileBuilds(buildsModel);

                    if (mobileData.status != 0)
                    {
                        foreach (var items in mobileData.data)
                        {
                            /// Checking file extension types.
                            string addIcon = Path.GetExtension(items.AttachmentURL).ToLower();

                            if (addIcon == ".pdf")
                            {
                                addIcon = "PDF.png";
                            }
                            else if (addIcon == ".apk")
                            {
                                addIcon = "APK.png";
                            }
                            else if (addIcon == ".ipa")
                            {
                                addIcon = "ISO.png";
                            }
                            else
                            {
                                addIcon = "ChooseFile.png";
                            }
                            addItems.Add(new MobileBuildsModel() { ID = items.ID, AppServer = items.AppServer, AttachmentName = items.AttachmentName, ImageURL = addIcon, AttachmentURL = items.AttachmentURL, AppVersion = items.AppVersion, UploadedBy = items.UploadedBy, UploadedDate = items.UploadedDate, Description = items.Description, GivenName = items.GivenName });
                        }

                        ListOfMobileBuilds = addItems;
                        HideListAndShow = true;
                        HideLabelAndShow = false;

                        if (ListOfMobileBuilds.Count == 0)
                        {
                            HideListAndShow = false;
                            HideLabelAndShow = true;
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
                YPSLogger.ReportException(ex, "GetMobileBuildsData method-> in MobileBuildsViewModel " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on description icon.
        /// </summary>
        /// <param name="sender"></param>
        private async void DescriptionClick(object sender)
        {
            IndicatorVisibility = true;
            try
            {
                if (ListOfMobileBuilds != null && ListOfMobileBuilds.Count > 0)
                {
                    var fileUpload = (from res in ListOfMobileBuilds where res.ID == (int)sender select res).FirstOrDefault();
                    FileName = fileUpload.AttachmentName;
                    AppServerLbl = fileUpload.AppServer;
                    AppVersionLbl = fileUpload.AppVersion;
                    UploadedDate = fileUpload.UploadedDate;
                    Uploadedby = fileUpload.GivenName;
                    Description = fileUpload.Description;
                }
                PopUpForFileDes = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DescriptionClick method-> in MobileBuildsViewModel " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on the download icon to download the file in mobile.
        /// </summary>
        /// <param name="sender"></param>
        private async void DownloadFileClick(object sender)
        {
            IndicatorVisibility = true;
            YPSLogger.TrackEvent("MobileBuildsViewModel", "in DownloadFileClick method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                /// Checking photo permission is allowed or denied by the user.
                var requestedPermissionsPhoto = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                var requestedPermissionStatusPhoto = requestedPermissionsPhoto[Permission.Photos];
                /// Checking storage permission is allowed or denied by the user.
                var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                var requestedPermissionStatus = requestedPermissions[Permission.Storage];
                var pass = requestedPermissionsPhoto[Permission.Photos];
                var pass1 = requestedPermissions[Permission.Storage];

                if (pass != PermissionStatus.Denied && pass1 != PermissionStatus.Denied)
                {
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (ListOfMobileBuilds != null && ListOfMobileBuilds.Count > 0)
                        {
                            var fileUpload = (from res in ListOfMobileBuilds where res.AttachmentURL == (string)sender select res).FirstOrDefault();
                            string folderName = (Device.RuntimePlatform == Device.Android) ? "Parts2y" : "Downloads";
                            downloader.DownloadFile(fileUpload.AttachmentURL, folderName);
                            downloader.OnFileDownloaded += OnFileDownloaded;
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to save files, please allow the permission in app permission settings.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DownloadFileClick method-> in FileUploadViewModel " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Get called when file download successfully to show alert message file is download successfully.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            try
            {
                if (Settings.isExpectedPublicKey == false)
                {
                    Settings.isExpectedPublicKey = true;
                    downloader.OnFileDownloaded -= OnFileDownloaded;
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                }
                else
                {
                    if (e.FileSaved)
                    {
                        switch (Device.RuntimePlatform)
                        {
                            case Device.iOS:
                                await App.Current.MainPage.DisplayAlert("Download", "Successful file saved to Parts2y/Downloads", "Close");
                                break;
                            case Device.Android:
                                DependencyService.Get<IToastMessage>().ShortAlert("Successful file saved to Parts2y/Downloads");
                                break;
                        }
                        downloader.OnFileDownloaded -= OnFileDownloaded;
                    }
                    else
                    {
                        switch (Device.RuntimePlatform)
                        {
                            case Device.iOS:
                                await App.Current.MainPage.DisplayAlert("Download", "Error while saving the file.", "Close");
                                break;
                            case Device.Android:
                                DependencyService.Get<IToastMessage>().ShortAlert("Error while saving the file.");
                                break;
                        }
                        downloader.OnFileDownloaded -= OnFileDownloaded;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnFileDownloaded method-> in FileUploadViewModel " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        #region Properties
        private ObservableCollection<MobileBuildsModel> _ListOfMobileBuilds;
        public ObservableCollection<MobileBuildsModel> ListOfMobileBuilds
        {
            get => _ListOfMobileBuilds;
            set
            {
                _ListOfMobileBuilds = value;
                NotifyPropertyChanged();
            }
        }
        private bool _HideListAndShow = false;
        public bool HideListAndShow
        {
            get => _HideListAndShow;
            set
            {
                _HideListAndShow = value;
                NotifyPropertyChanged();
            }
        }
        private bool _HideLabelAndShow = false;
        public bool HideLabelAndShow
        {
            get => _HideLabelAndShow;
            set
            {
                _HideLabelAndShow = value;
                NotifyPropertyChanged();
            }
        }
        private bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get => _IndicatorVisibility;
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private bool _PopUpForFileDes = false;
        public bool PopUpForFileDes
        {
            get => _PopUpForFileDes;
            set
            {
                _PopUpForFileDes = value;
                NotifyPropertyChanged();
            }
        }
        private string _FileName = string.Empty;
        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                NotifyPropertyChanged();
            }
        }
        private string _Uploadedby = string.Empty;
        public string Uploadedby
        {
            get { return _Uploadedby; }
            set
            {
                _Uploadedby = value;
                NotifyPropertyChanged();
            }
        }
        private string _AppServerLbl = string.Empty;
        public string AppServerLbl
        {
            get { return _AppServerLbl; }
            set
            {
                _AppServerLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _AppVersionLbl = string.Empty;
        public string AppVersionLbl
        {
            get { return _AppVersionLbl; }
            set
            {
                _AppVersionLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _UploadedDate = string.Empty;
        public string UploadedDate
        {
            get { return _UploadedDate; }
            set
            {
                _UploadedDate = value;
                NotifyPropertyChanged();
            }
        }
        private string _Description = string.Empty;
        public string Description
        {
            get { return _Description; }
            set
            {
                _Description = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
