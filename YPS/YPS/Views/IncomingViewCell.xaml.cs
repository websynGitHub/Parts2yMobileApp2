using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Service;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingViewCell : ViewCell
    {
        IDownloader downloader = DependencyService.Get<IDownloader>();

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        public IncomingViewCell()
        {
            try
            {
                InitializeComponent();
                YPSLogger.TrackEvent("IncomingViewCell", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

                if (Settings.Text != null)
                {
                    nameIncomingPhoto.IsVisible = false;
                    nameIncoming.IsVisible = true;
                    nameDocumentIncoming.IsVisible = false;
                    nameIncoming.Text = Settings.Text;
                }

                if (Settings.Image != null)
                {
                    nameIncoming.IsVisible = false;
                    nameIncomingPhoto.IsVisible = true;
                    nameDocumentIncoming.IsVisible = false;
                    nameIncomingPhoto.Source = Settings.Image;
                }

                if (Settings.ChatDocument != null)
                {
                    nameIncomingPhoto.IsVisible = false;
                    nameIncoming.IsVisible = false;
                    nameDocumentIncoming.IsVisible = true;
                    DownloadFile.ClassId = Settings.ChatDocument;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "IncomingViewCell constructor -> in IncomingViewCell.xaml.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on the download text and icon to download the file in mobile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DownloadFileIcon_Tapped(object sender, EventArgs e)
        {
            try
            {
                var items = (StackLayout)sender;
                /// Checking photo permission is allowed or denied by the user. 
                var requestedPermissionsPhoto = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                var requestedPermissionStatusPhoto = requestedPermissionsPhoto[Permission.Photos];
                /// Checking storage permission is allowed or denied by the user.
                var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                var requestedPermissionStatus = requestedPermissions[Permission.Storage];
                var photoPermission = requestedPermissionsPhoto[Permission.Photos];
                var storagePermission = requestedPermissions[Permission.Storage];

                if (photoPermission != PermissionStatus.Denied && storagePermission != PermissionStatus.Denied)
                {
                    BusyIndicator.IsRunning = true;
                    BusyIndicator.IsVisible = true;
                    /// Verifying internet connection.
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        downloader.DownloadFile(items.ClassId, "Parts2y");
                        downloader.OnFileDownloaded += OnFileDownloaded;
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
                YPSLogger.ReportException(ex, "DownloadFileIcon_Tapped method-> in IncomingViewCell.xaml.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
            finally
            {
                BusyIndicator.IsRunning = false;
                BusyIndicator.IsVisible = false;
            }
        }

        /// <summary>
        /// Gets called when file download successfully to show alert message file is download successfully.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnFileDownloaded(object sender, DownloadEventArgs e)
        {
            try
            {
                if (e.FileSaved)
                {
                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            await App.Current.MainPage.DisplayAlert("Download", "Successful file saved to Parts2y/Downloads", "Close");
                            break;
                        case Device.Android:
                            DependencyService.Get<IToastMessage>().ShortAlert("Successful file saved to Parts2y/Downloads.");
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
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnFileDownloaded method-> in IncomingViewCell.xaml.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
            finally
            {
            }
        }
    }
}