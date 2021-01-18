using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model.Yship;
using YPS.Service;
using YPS.Model;
using System.Linq;
using YPS.CustomRenders;

namespace YPS.YShip.YshipViewModel
{
    public class YshipFileUploadViewModel : IBase
    {

        #region ICommand & data member Declaration
        string selectedDate2;
        IDownloader downloader = DependencyService.Get<IDownloader>();
        public ICommand ITabInVoice { get; set; }
        public ICommand ITabPacking { get; set; }
        public ICommand IExport { get; set; }
        public ICommand IImport { get; set; }
        public ICommand ITransmit { get; set; }
        public ICommand IDescription { get; set; }
        public ICommand IPermitsDate { set; get; }
        public ICommand IUploadFile { get; set; }
        public ICommand IDeleteFile { get; set; }
        public ICommand IDownloadFile { get; set; }
        public ICommand PickFileFromDevice { get; set; }
        YPSService service;
        public Stream picStream;
        string extension;
        GetYshipFiles getYshipFiles;
        int yShipId, completed, canceled;
        NullableDatePicker myNullableDate;
        bool checkInternet;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="yshipID"></param>
        /// <param name="completed"></param>
        /// <param name="canceled"></param>
        public YshipFileUploadViewModel(int yshipID, int completed, int canceled)
        {
            try
            {
                #region Binding methods to the respective ICommands
                ITabInVoice = new Command(async () => await TabInVoice());
                ITabPacking = new Command(async () => await TabPacking());
                IExport = new Command(async () => await TabExport());
                IImport = new Command(async () => await TabImport());
                ITransmit = new Command(async () => await TabTransmit());
                IPermitsDate = new Command(tab_PermitsDate);
                IUploadFile = new Command(async () => await UploadFile());
                IDescription = new Command(tab_yShipDescription);
                IDeleteFile = new Command(yShipDeleteFile);
                IDownloadFile = new Command(yShipDownloadFile);
                PickFileFromDevice = new Command(async () => await GetFileFromDevice());
                #endregion

                service = new YPSService();
                getYshipFiles = new GetYshipFiles();
                this.yShipId = yshipID;
                this.completed = completed;
                this.canceled = canceled;
                GetData(yshipID);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel constructor -> in YshipFileUploadModel.cs" + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on description icon and then showing related file info.
        /// </summary>
        /// <param name="obj"></param>
        private async void tab_yShipDescription(object obj)
        {
            try
            {
                IndicatorVisibility = true;
                UploadFiles data = new UploadFiles();

                if (UploadType == (int)UploadTypeEnums.Invoice)
                {
                    data = (from res in InvoiceList where res.ID == (int)obj select res).FirstOrDefault();
                    updateDescription(data.FullName, data.UploadedDate, data.AttachmentName);
                }
                else if (UploadType == (int)UploadTypeEnums.PackingList)
                {
                    data = (from res in PackingList where res.ID == (int)obj select res).FirstOrDefault();
                    updateDescription(data.FullName, data.UploadedDate, data.AttachmentName);
                }
                else if (UploadType == (int)UploadTypeEnums.ExportPermit)
                {
                    data = (from res in ExportList where res.ID == (int)obj select res).FirstOrDefault();
                    updateDescription(data.FullName, data.UploadedDate, data.AttachmentName, data.PermitNo, data.PermitDate);
                }
                else if (UploadType == (int)UploadTypeEnums.TransitPermit)
                {
                    data = (from res in TransmitList where res.ID == (int)obj select res).FirstOrDefault();
                    updateDescription(data.FullName, data.UploadedDate, data.AttachmentName, data.PermitNo, data.PermitDate);
                }
                else
                {
                    data = (from res in ImportList where res.ID == (int)obj select res).FirstOrDefault();
                    updateDescription(data.FullName, data.UploadedDate, data.AttachmentName, data.PermitNo, data.PermitDate);
                }
                PopUpForFileDes = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in tab_yShipDescription" + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method is calling from tab_yShipDescription() method to set the value on the property based on the upload type.
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="uploadedDate"></param>
        /// <param name="attachmentName"></param>
        /// <param name="permitNo"></param>
        /// <param name="permitOn"></param>
        private void updateDescription(string fullName, string uploadedDate, string attachmentName, string permitNo = null, string permitOn = null)
        {
            Uploadedby = fullName;
            UploadedDate = uploadedDate;
            FileName = attachmentName;

            if (permitNo == null)
            {
                PermitNoHeight = 0;
                PermitOnHeight = 0;
            }
            else
            {
                PermitNo = permitNo;
                PermitOn = permitOn;
            }
        }

        /// <summary>
        /// This method will fire when clicked on permits date icon to show the calendar.
        /// </summary>
        /// <param name="obj"></param>
        private void tab_PermitsDate(object obj)
        {
            myNullableDate = obj as NullableDatePicker;
            myNullableDate.Focus();
            myNullableDate.PropertyChanged += MyNullableDate_PropertyChanged;
        }

        /// <summary>
        /// This method will fire after the select date and then click on "OK" icons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyNullableDate_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var date = sender as NullableDatePicker;
            var selectedDate = String.Format("{0:dd MMM yyyy}", date.Date);
            selectedDate2 = String.Format("{0:00}/{1:00}/{2:0000}", date.Date.Month, date.Date.Day, date.Date.Year);
            PermitsDate = selectedDate;
            myNullableDate.CleanDate();
        }

        /// <summary>
        /// This method will fire when clicked on "Upload" button to upload a file by using blob.
        /// </summary>
        /// <returns></returns>
        private async Task UploadFile()
        {
            IndicatorVisibility = true;
            YPSLogger.TrackEvent("YshipFileUploadViewModel", "in UploadFile method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (SetFileName == "Please Select A File")
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please upload files having extensions: .pdf, .txt, .doc, .docx only.");
                    }
                    else if (String.IsNullOrWhiteSpace(PermitsNo) && CheckUploadType == (int)UploadTypeEnums.ExportPermit + "," + (int)UploadTypeEnums.TransitPermit + "," + (int)UploadTypeEnums.ImportPermit)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Enter your permit no.");
                    }
                    else if (PermitsDate == "Permit date" && CheckUploadType == (int)UploadTypeEnums.ExportPermit + "," + (int)UploadTypeEnums.TransitPermit + "," + (int)UploadTypeEnums.ImportPermit)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select date.");
                    }
                    else
                    {
                        string PassPermitDate = string.Empty;

                        if (PermitsDate != PermitsDateDefaultVal)
                        {
                            PassPermitDate = selectedDate2;
                        }

                        /// Calling bloab API to upload file.
                        var blobResponse = await BlobUpload.YPSFileUpload(extension, picStream, yShipId, SetFileName, UploadType, (int)BlobContainer.cntyshipuploads, null, null, "", PassPermitDate, PermitsNo);
                        var resultUF = blobResponse as yShipFileUploadResponse;

                        if (resultUF.status != 0)
                        {
                            string Icon;

                            if (resultUF.data.UploadType == (int)UploadTypeEnums.Invoice)
                            {
                                Icon = CheckExtensionOfImage(extension);
                                InvoiceList.Insert(0, new UploadFiles() { AttachmentName = resultUF.data.AttachmentName, FullName = resultUF.data.FullName, ID = resultUF.data.ID, FileURL = resultUF.data.FileURL, UploadedDate = resultUF.data.UploadedDate, MyImage = Icon });
                                getYshipFiles.data.invoice = InvoiceList;
                                SetFileName = "Please Select A File";
                                AddFolder = "ChooseFile.png";
                                IsFolderVisible = true;
                                IsCrossVisible = false;
                                NoDataFoundLbl = false;
                                InVoiceStackLayOut = true;
                            }
                            else if (resultUF.data.UploadType == (int)UploadTypeEnums.PackingList)
                            {
                                Icon = CheckExtensionOfImage(extension);
                                PackingList.Insert(0, new UploadFiles() { AttachmentName = resultUF.data.AttachmentName, FullName = resultUF.data.FullName, ID = resultUF.data.ID, FileURL = resultUF.data.FileURL, UploadedDate = resultUF.data.UploadedDate, MyImage = Icon });
                                getYshipFiles.data.packingList = PackingList;
                                SetFileName = "Please Select A File";
                                AddFolder = "ChooseFile.png";
                                IsFolderVisible = true;
                                IsCrossVisible = false;
                                NoDataFoundLbl = false;
                                PackingStackLayout = true;
                            }
                            else if (resultUF.data.UploadType == (int)UploadTypeEnums.ExportPermit)
                            {
                                Icon = CheckExtensionOfImage(extension);
                                ExportList.Insert(0, new UploadFiles() { AttachmentName = resultUF.data.AttachmentName, FullName = resultUF.data.FullName, ID = resultUF.data.ID, FileURL = resultUF.data.FileURL, UploadedDate = resultUF.data.UploadedDate, PermitNo = resultUF.data.PermitNo, PermitDate = PermitsDate, MyImage = Icon });
                                getYshipFiles.data.exportPermit = ExportList;
                                SetFileName = "Please Select A File";
                                AddFolder = "ChooseFile.png";
                                IsFolderVisible = true;
                                IsCrossVisible = false;
                                PermitsNo = null;
                                PermitsDate = PermitsDateDefaultVal;
                                NoDataFoundLbl = false;
                                ExportIsVisible = true;
                            }
                            else if (resultUF.data.UploadType == (int)UploadTypeEnums.TransitPermit)
                            {
                                Icon = CheckExtensionOfImage(extension);
                                TransmitList.Insert(0, new UploadFiles() { AttachmentName = resultUF.data.AttachmentName, FullName = resultUF.data.FullName, ID = resultUF.data.ID, FileURL = resultUF.data.FileURL, UploadedDate = resultUF.data.UploadedDate, PermitNo = resultUF.data.PermitNo, PermitDate = PermitsDate, MyImage = Icon });
                                getYshipFiles.data.transitPermit = TransmitList;
                                SetFileName = "Please Select A File";
                                AddFolder = "ChooseFile.png";
                                IsFolderVisible = true;
                                IsCrossVisible = false;
                                PermitsNo = null;
                                PermitsDate = PermitsDateDefaultVal;
                                NoDataFoundLbl = false;
                                TransmitIsVisible = true;
                            }
                            else if (resultUF.data.UploadType == (int)UploadTypeEnums.ImportPermit)
                            {
                                Icon = CheckExtensionOfImage(extension);
                                ImportList.Insert(0, new UploadFiles() { AttachmentName = resultUF.data.AttachmentName, FullName = resultUF.data.FullName, ID = resultUF.data.ID, FileURL = resultUF.data.FileURL, UploadedDate = resultUF.data.UploadedDate, PermitNo = resultUF.data.PermitNo, PermitDate = PermitsDate, MyImage = Icon });
                                getYshipFiles.data.importPermit = ImportList;
                                SetFileName = "Please Select A File";
                                AddFolder = "ChooseFile.png";
                                IsFolderVisible = true;
                                IsCrossVisible = false;
                                PermitsNo = null;
                                PermitsDate = PermitsDateDefaultVal;
                                NoDataFoundLbl = false;
                                ImportIsVisible = true;
                            }

                            DependencyService.Get<IToastMessage>().ShortAlert("Success."); ;
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Message", resultUF.message, "ok");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in UploadFile" + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method gets called clicked on the download icon to download files.
        /// </summary>
        /// <param name="sender"></param>
        private async void yShipDownloadFile(object sender)
        {
            IndicatorVisibility = true;
            YPSLogger.TrackEvent("FileUploadViewModel", "in yShipDownloadFile method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    UploadFiles downloadF = new UploadFiles();

                    /// Requesting photo permissions.
                    var requestedPPhotos = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                    var rPhotos = requestedPPhotos[Permission.Photos];
                    /// Requesting storage permissions.
                    var requestedPStorage = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    var rStorage = requestedPStorage[Permission.Storage];

                    /// Checking permissions are allowed or denied by the user to store the file and photo from mobile.
                    if (rPhotos != PermissionStatus.Denied && rStorage != PermissionStatus.Denied)
                    {
                        if (UploadType == (int)UploadTypeEnums.Invoice)
                        {
                            if (InvoiceList != null && InvoiceList.Count > 0)
                            {
                                downloadF = (from res in InvoiceList where res.FileURL == (string)sender select res).FirstOrDefault();
                                string folderName = (Device.RuntimePlatform == Device.Android) ? "Parts2y" : "Downloads";
                                downloader.DownloadFile(downloadF.FileURL, folderName);
                                downloader.OnFileDownloaded += yShipOnFileDownloaded;
                            }
                        }
                        else if (UploadType == (int)UploadTypeEnums.PackingList)
                        {
                            if (PackingList != null && PackingList.Count > 0)
                            {
                                downloadF = (from res in PackingList where res.FileURL == (string)sender select res).FirstOrDefault();
                                string folderName = (Device.RuntimePlatform == Device.Android) ? "Parts2y" : "Downloads";
                                downloader.DownloadFile(downloadF.FileURL, folderName);
                                downloader.OnFileDownloaded += yShipOnFileDownloaded;
                            }
                        }
                        else if (UploadType == (int)UploadTypeEnums.ExportPermit)
                        {
                            if (ExportList != null && ExportList.Count > 0)
                            {
                                downloadF = (from res in ExportList where res.FileURL == (string)sender select res).FirstOrDefault();
                                string folderName = (Device.RuntimePlatform == Device.Android) ? "Parts2y" : "Downloads";
                                downloader.DownloadFile(downloadF.FileURL, folderName);
                                downloader.OnFileDownloaded += yShipOnFileDownloaded;
                            }
                        }
                        else if (UploadType == (int)UploadTypeEnums.ImportPermit)
                        {
                            if (ImportList != null && ImportList.Count > 0)
                            {
                                downloadF = (from res in ImportList where res.FileURL == (string)sender select res).FirstOrDefault();
                                string folderName = (Device.RuntimePlatform == Device.Android) ? "Parts2y" : "Downloads";
                                downloader.DownloadFile(downloadF.FileURL, folderName);
                                downloader.OnFileDownloaded += yShipOnFileDownloaded;
                            }
                        }
                        else if (UploadType == (int)UploadTypeEnums.TransitPermit)
                        {
                            if (TransmitList != null && TransmitList.Count > 0)
                            {
                                downloadF = (from res in TransmitList where res.FileURL == (string)sender select res).FirstOrDefault();
                                string folderName = (Device.RuntimePlatform == Device.Android) ? "Parts2y" : "Downloads";
                                downloader.DownloadFile(downloadF.FileURL, folderName);
                                downloader.OnFileDownloaded += yShipOnFileDownloaded;
                            }
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to save files, please allow the permission in app permission setting", "OK");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in yShipDownloadFile" + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when file download successfully to show alert message file is download successfully.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void yShipOnFileDownloaded(object sender, DownloadEventArgs e)
        {
            try
            {
                if (Settings.isExpectedPublicKey == false)
                {
                    Settings.isExpectedPublicKey = true;
                    downloader.OnFileDownloaded -= yShipOnFileDownloaded;
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
                        downloader.OnFileDownloaded -= yShipOnFileDownloaded;
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
                        downloader.OnFileDownloaded -= yShipOnFileDownloaded;
                    }
                }
            }
            catch (Exception ex)
            {
                Settings.isExpectedPublicKey = true;
                YPSLogger.ReportException(ex, "yShipOnFileDownloaded method-> in FileUploadViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on delete icon to delete a single file.
        /// </summary>
        /// <param name="sender"></param>
        private async void yShipDeleteFile(object sender)
        {
            try
            {
                IndicatorVisibility = true;
                UploadFiles fileDelete = new UploadFiles();
                bool result = await App.Current.MainPage.DisplayAlert("Delete!", "Are you sure you want to delete?", "OK", "Cancel");

                if (result)
                {
                    if (UploadType == (int)UploadTypeEnums.Invoice)
                    {
                        fileDelete = (from res in InvoiceList where res.ID == (int)sender select res).FirstOrDefault();
                        /// Calling delete API to delete a single file based on file id and type for the invoice.
                        var InVoiceResponse = await service.yShipDeleteFile(fileDelete.ID, yShipId, (int)UploadTypeEnums.Invoice);

                        if (InVoiceResponse.status != 0 || InVoiceResponse != null)
                        {
                            var deleteItem = InvoiceList.Where(x => x.ID == fileDelete.ID).FirstOrDefault();
                            InvoiceList.Remove(deleteItem);
                            getYshipFiles.data.invoice = InvoiceList;
                            switch (Device.RuntimePlatform)
                            {
                                case Device.iOS:
                                    await App.Current.MainPage.DisplayAlert("Delete", "Success.", "Close");
                                    break;
                                case Device.Android:
                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    break;
                            }
                            if (getYshipFiles.data.invoice.Count() == 0)
                            {
                                InVoiceStackLayOut = false;
                                NoDataFoundLbl = true;
                            }
                        }
                        else
                        {
                        }
                    }
                    else if (UploadType == (int)UploadTypeEnums.PackingList)
                    {
                        fileDelete = (from res in PackingList where res.ID == (int)sender select res).FirstOrDefault();
                        /// Calling delete API to delete a single file based on file id and type for the packing.
                        var PackingResponse = await service.yShipDeleteFile(fileDelete.ID, yShipId, (int)UploadTypeEnums.PackingList);

                        if (PackingResponse.status != 0 || PackingResponse != null)
                        {
                            var deleteItem = PackingList.Where(x => x.ID == fileDelete.ID).FirstOrDefault();
                            PackingList.Remove(deleteItem);
                            getYshipFiles.data.packingList = PackingList;
                            switch (Device.RuntimePlatform)
                            {
                                case Device.iOS:
                                    await App.Current.MainPage.DisplayAlert("Delete", "Success.", "Close");
                                    break;
                                case Device.Android:
                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    break;
                            }
                            if (getYshipFiles.data.packingList.Count() == 0)
                            {
                                PackingStackLayout = false;
                                NoDataFoundLbl = true;
                            }
                        }
                        else
                        {
                        }
                    }
                    else if (UploadType == (int)UploadTypeEnums.ExportPermit)
                    {
                        fileDelete = (from res in ExportList where res.ID == (int)sender select res).FirstOrDefault();
                        /// Calling delete API to delete a single file based on file id and type for the export permit.
                        var ExportResponse = await service.yShipDeleteFile(fileDelete.ID, yShipId, (int)UploadTypeEnums.ExportPermit);

                        if (ExportResponse.status != 0 || ExportResponse != null)
                        {
                            var deleteItem = ExportList.Where(x => x.ID == fileDelete.ID).FirstOrDefault();
                            ExportList.Remove(deleteItem);
                            getYshipFiles.data.exportPermit = ExportList;
                            switch (Device.RuntimePlatform)
                            {
                                case Device.iOS:
                                    await App.Current.MainPage.DisplayAlert("Delete", "Success.", "Close");
                                    break;
                                case Device.Android:
                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    break;
                            }
                            if (getYshipFiles.data.exportPermit.Count() == 0)
                            {
                                ExportIsVisible = false;
                                NoDataFoundLbl = true;
                            }
                        }
                        else
                        {
                        }
                    }
                    else if (UploadType == (int)UploadTypeEnums.ImportPermit)
                    {
                        fileDelete = (from res in ImportList where res.ID == (int)sender select res).FirstOrDefault();
                        /// Calling delete API to delete a single file based on file id and type for the import permit.
                        var ImportResponse = await service.yShipDeleteFile(fileDelete.ID, yShipId, (int)UploadTypeEnums.ImportPermit);

                        if (ImportResponse.status != 0 || ImportResponse != null)
                        {
                            var deleteItem = ImportList.Where(x => x.ID == fileDelete.ID).FirstOrDefault();
                            ImportList.Remove(deleteItem);
                            getYshipFiles.data.importPermit = ImportList;
                            switch (Device.RuntimePlatform)
                            {
                                case Device.iOS:
                                    await App.Current.MainPage.DisplayAlert("Delete", "Success.", "Close");
                                    break;
                                case Device.Android:
                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    break;
                            }
                            if (getYshipFiles.data.importPermit.Count() == 0)
                            {
                                ImportIsVisible = false;
                                NoDataFoundLbl = true;
                            }
                        }
                        else
                        {
                        }
                    }
                    else if (UploadType == (int)UploadTypeEnums.TransitPermit)
                    {
                        fileDelete = (from res in TransmitList where res.ID == (int)sender select res).FirstOrDefault();
                        /// Calling delete API to delete a single file based on file id and type for the transit permit.
                        var TransmitResponse = await service.yShipDeleteFile(fileDelete.ID, yShipId, (int)UploadTypeEnums.TransitPermit);

                        if (TransmitResponse.status != 0 || TransmitResponse != null)
                        {
                            var deleteItem = TransmitList.Where(x => x.ID == fileDelete.ID).FirstOrDefault();
                            TransmitList.Remove(deleteItem);
                            getYshipFiles.data.transitPermit = TransmitList;
                            switch (Device.RuntimePlatform)
                            {
                                case Device.iOS:
                                    await App.Current.MainPage.DisplayAlert("Delete", "Success.", "Close");
                                    break;
                                case Device.Android:
                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    break;
                            }
                            if (getYshipFiles.data.transitPermit.Count() == 0)
                            {
                                TransmitIsVisible = false;
                                NoDataFoundLbl = true;
                            }
                        }
                        else
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in yShipDeleteFile" + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method is for picking file from device.
        /// </summary>
        /// <returns></returns>
        private async Task GetFileFromDevice()
        {
            IndicatorVisibility = true;
            YPSLogger.TrackEvent("YshipFileUploadViewModel", "in GetFileFromDevice method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        /// Requesting storage permissions.
                        var requestedPStorages = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                        var pCheckAndroid = requestedPStorages[Permission.Storage];

                        /// Checking permission is allowed or denied by the user to access the file from mobile for Android.
                        if (pCheckAndroid != PermissionStatus.Denied)
                        {
                            FileData fileData = await CrossFilePicker.Current.PickFile();
                            if (fileData == null)
                            {
                                IndicatorVisibility = false;
                                return; // user canceled file picking
                            }
                            string AndroidfileName = fileData.FileName;
                            string AndroidfilePath = fileData.FilePath;
                            extension = Path.GetExtension(AndroidfilePath).ToLower();

                            if (extension == "")
                            {
                                extension = Path.GetExtension(AndroidfileName).ToLower();
                            }

                            picStream = fileData.GetStream();
                            if (extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                            {
                                SetFileName = AndroidfileName;
                                AddCross = "cross.png";
                                IsCrossVisible = true;
                                IsFolderVisible = false;
                                FilePath64 = AndroidfilePath;
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .pdf, .txt, .doc, .docx only.", "OK");
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to save files, please allow the permission in app permission setting", "OK");
                        }
                        break;

                    case Device.iOS:

                        FileData fileDataForiOS = await CrossFilePicker.Current.PickFile();

                        if (fileDataForiOS == null)
                        {
                            IndicatorVisibility = false;
                            return; // user canceled file picking
                        }
                        string iOSfileName = fileDataForiOS.FileName;
                        string iOSfilePath = fileDataForiOS.FilePath;
                        extension = Path.GetExtension(iOSfilePath).ToLower();
                        picStream = fileDataForiOS.GetStream();

                        if (extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                        {
                            SetFileName = iOSfileName;
                            AddCross = "cross.png";
                            IsCrossVisible = true;
                            IsFolderVisible = false;
                            FilePath64 = iOSfilePath;
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .pdf, .txt, .doc, .docx only.", "OK");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in GetFileFromDevice " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets the uploaded files based on yShipId.
        /// </summary>
        /// <param name="yShipId"></param>
        private async void GetData(int yShipId)
        {
            try
            {
                IndicatorVisibility = true;
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();
                /// If internet connection is available.

                if (checkInternet)
                {
                    /// Calling gets API to get data and set on the list view.
                    getYshipFiles = await service.GetYshipfiles(yShipId, CheckUploadType);
                    if (getYshipFiles != null)
                    {
                        if (getYshipFiles.status != 0)
                        {
                            string icon;

                            if (CheckUploadType == (int)UploadTypeEnums.Invoice + "," + (int)UploadTypeEnums.PackingList)
                            {
                                if (getYshipFiles.data.invoice.Count != 0)
                                {
                                    ObservableCollection<UploadFiles> addInvoiceList = new ObservableCollection<UploadFiles>();
                                    foreach (var items in getYshipFiles.data.invoice)
                                    {
                                        icon = CheckExtensionOfImage(Path.GetExtension(items.FileURL).ToLower());
                                        addInvoiceList.Add(new UploadFiles() { FullName = items.FullName, ID = items.ID, FileURL = items.FileURL, UploadedDate = items.UploadedDate, MyImage = icon, AttachmentName = items.AttachmentName });
                                    }
                                    InvoiceList = addInvoiceList;
                                }

                                if (getYshipFiles.data.packingList.Count != 0)
                                {
                                    ObservableCollection<UploadFiles> addPackingList = new ObservableCollection<UploadFiles>();
                                    foreach (var items in getYshipFiles.data.packingList)
                                    {
                                        icon = CheckExtensionOfImage(Path.GetExtension(items.FileURL).ToLower());
                                        addPackingList.Add(new UploadFiles() { FullName = items.FullName, ID = items.ID, FileURL = items.FileURL, UploadedDate = items.UploadedDate, MyImage = icon, AttachmentName = items.AttachmentName });
                                    }
                                    PackingList = addPackingList;
                                }

                                if (getYshipFiles.data.invoice.Count != 0)
                                {
                                    InVoiceStackLayOut = true;
                                }
                                else
                                {
                                    NoDataFoundLbl = true;
                                    InVoiceStackLayOut = false;
                                }

                                if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                                    Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin ||
                                    Settings.userRoleID == (int)UserRoles.DealerUser)
                                {
                                    DeleteIconStack = false;
                                    BrowseFileFrm = false;
                                    BrowseFileRowHt = 0;
                                }
                                else
                                {
                                    if (completed == 1 || canceled == 1)
                                    {
                                        DeleteIconStack = false;
                                        BrowseFileFrm = false;
                                        BrowseFileRowHt = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (getYshipFiles.data.exportPermit.Count != 0)
                                {
                                    ObservableCollection<UploadFiles> addExportList = new ObservableCollection<UploadFiles>();
                                    foreach (var items in getYshipFiles.data.exportPermit)
                                    {
                                        icon = CheckExtensionOfImage(Path.GetExtension(items.FileURL).ToLower());
                                        addExportList.Add(new UploadFiles() { FullName = items.FullName, ID = items.ID, FileURL = items.FileURL, UploadedDate = items.UploadedDate, MyImage = icon, AttachmentName = items.AttachmentName, PermitNo = items.PermitNo, PermitDate = items.PermitDate });
                                    }
                                    ExportList = addExportList;
                                }

                                if (getYshipFiles.data.importPermit.Count != 0)
                                {
                                    ObservableCollection<UploadFiles> addImportList = new ObservableCollection<UploadFiles>();
                                    foreach (var items in getYshipFiles.data.importPermit)
                                    {
                                        icon = CheckExtensionOfImage(Path.GetExtension(items.FileURL).ToLower());
                                        addImportList.Add(new UploadFiles() { FullName = items.FullName, ID = items.ID, FileURL = items.FileURL, UploadedDate = items.UploadedDate, MyImage = icon, AttachmentName = items.AttachmentName, PermitNo = items.PermitNo, PermitDate = items.PermitDate });
                                    }
                                    ImportList = addImportList;
                                }

                                if (getYshipFiles.data.transitPermit.Count != 0)
                                {
                                    ObservableCollection<UploadFiles> addTransitList = new ObservableCollection<UploadFiles>();
                                    foreach (var items in getYshipFiles.data.transitPermit)
                                    {
                                        icon = CheckExtensionOfImage(Path.GetExtension(items.FileURL).ToLower());
                                        addTransitList.Add(new UploadFiles() { FullName = items.FullName, ID = items.ID, FileURL = items.FileURL, UploadedDate = items.UploadedDate, MyImage = icon, AttachmentName = items.AttachmentName, PermitNo = items.PermitNo, PermitDate = items.PermitDate });
                                    }
                                    TransmitList = addTransitList;
                                }

                                if (getYshipFiles.data.exportPermit.Count != 0)
                                {
                                    ExportIsVisible = true;
                                }
                                else
                                {
                                    NoDataFoundLbl = true;
                                    ExportIsVisible = false;
                                }

                                if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                                    Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin ||
                                    Settings.userRoleID == (int)UserRoles.DealerUser)
                                {
                                    DeleteIconStack = false;
                                    PermitsRowSpacing = 0;
                                    BrowseFileFrm = false;
                                    PermitNoDateIsVsble = false;
                                    PermitNoDateHt = 0;
                                    BrowseFileRowHtPermits = 0;
                                }
                                else
                                {
                                    if (completed == 1 || canceled == 1)
                                    {
                                        DeleteIconStack = false;
                                        PermitsRowSpacing = 0;
                                        BrowseFileFrm = false;
                                        PermitNoDateIsVsble = false;
                                        PermitNoDateHt = 0;
                                        BrowseFileRowHtPermits = 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (CheckUploadType == (int)UploadTypeEnums.Invoice + "," + (int)UploadTypeEnums.PackingList)
                            {
                                NoDataFoundLbl = true;
                                InVoiceStackLayOut = false;
                            }
                            else
                            {
                                NoDataFoundLbl = true;
                                ExportIsVisible = false;
                            }

                            if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                                Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin ||
                                Settings.userRoleID == (int)UserRoles.DealerUser)
                            {
                                //DeleteIconStack = false;
                                BrowseFileFrm = false;
                                BrowseFileRowHt = 0;
                                PermitNoDateIsVsble = false;
                                PermitNoDateHt = 0;
                                BrowseFileRowHtPermits = 0;
                            }
                            else
                            {
                                if (completed == 1 || canceled == 1)
                                {
                                    //DeleteIconStack = false;
                                    BrowseFileFrm = false;
                                    BrowseFileRowHt = 0;
                                    PermitNoDateIsVsble = false;
                                    PermitNoDateHt = 0;
                                    BrowseFileRowHtPermits = 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        //   DependencyService.Get<IToastMessage>().ShortAlert("Somethings went wrong, please try again.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in GetData " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method is for checking image and file extension types.
        /// </summary>
        /// <param name="ImageExtension"></param>
        /// <returns></returns>
        private string CheckExtensionOfImage(string ImageExtension)
        {
            string imgExtension;

            if (ImageExtension == ".pdf")
            {
                imgExtension = "PDF.png";
            }
            else if (ImageExtension == ".txt")
            {
                imgExtension = "TXT.png";
            }
            else if (ImageExtension == ".doc")
            {
                imgExtension = "DOC.png";
            }
            else if (ImageExtension == ".docx")
            {
                imgExtension = "DOCX.png";
            }
            else
            {
                imgExtension = "ChooseFile.png";
            }
            return imgExtension;
        }

        #region Tab click events/methods

        /// <summary>
        /// this gets called when clicked on InVoice tab.
        /// </summary>
        /// <returns></returns>
        private async Task TabInVoice()
        {
            try
            {
                IndicatorVisibility = true;
                SetFileName = "Please Select A File";
                AddFolder = "ChooseFile.png";
                IsFolderVisible = true;
                IsCrossVisible = false;
                TabInVoiceVisibility = true;
                TabPackingVisibility = false;
                TabOpacityInVoice = 1;
                TabOpacityPacking = 0.3;
                UploadType = (int)UploadTypeEnums.Invoice;

                if (getYshipFiles.data.invoice != null)
                {
                    PackingStackLayout = false;
                    NoDataFoundLbl = getYshipFiles.data.invoice.Count != 0 ? false : true;
                    InVoiceStackLayOut = (NoDataFoundLbl) ? false : true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in TabInVoice " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This is called when clicked on the Packing tab.
        /// </summary>
        /// <returns></returns>
        private async Task TabPacking()
        {
            try
            {
                IndicatorVisibility = true;
                SetFileName = "Please Select A File";
                AddFolder = "ChooseFile.png";
                IsFolderVisible = true;
                IsCrossVisible = false;
                TabInVoiceVisibility = false;
                TabPackingVisibility = true;
                TabOpacityInVoice = 0.3;
                TabOpacityPacking = 1;
                UploadType = (int)UploadTypeEnums.PackingList;

                if (getYshipFiles.data.packingList != null)
                {
                    InVoiceStackLayOut = false;
                    NoDataFoundLbl = getYshipFiles.data.packingList.Count != 0 ? false : true;
                    PackingStackLayout = (NoDataFoundLbl) ? false : true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in TabPacking " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This gets called when clicked on the Export tab.
        /// </summary>
        /// <returns></returns>
        private async Task TabExport()
        {
            try
            {
                SetFileName = "Please Select A File";
                AddFolder = "ChooseFile.png";
                IsFolderVisible = true;
                IsCrossVisible = false;
                PermitsNo = null;
                PermitsDate = PermitsDateDefaultVal;
                TabExportVisibility = true;
                TabImportVisibility = TabTransmitVisibility = false;
                TabOpacityExport = 1.0;
                TabOpacityImport = TabOpacityTransmit = 0.3;
                UploadType = (int)UploadTypeEnums.ExportPermit;

                if (getYshipFiles.data.exportPermit != null)
                {
                    ImportIsVisible = false;
                    TransmitIsVisible = false;
                    NoDataFoundLbl = getYshipFiles.data.exportPermit.Count != 0 ? false : true;
                    ExportIsVisible = (NoDataFoundLbl) ? false : true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in TabTransmit " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This gets called when clicked on the Import tab.
        /// </summary>
        /// <returns></returns>
        private async Task TabImport()
        {
            try
            {
                SetFileName = "Please Select A File";
                AddFolder = "ChooseFile.png";
                IsFolderVisible = true;
                IsCrossVisible = false;
                PermitsNo = null;
                PermitsDate = PermitsDateDefaultVal;
                TabImportVisibility = true;
                TabExportVisibility = TabTransmitVisibility = false;
                TabOpacityImport = 1.0;
                TabOpacityExport = TabOpacityTransmit = 0.3;
                UploadType = (int)UploadTypeEnums.ImportPermit;

                if (getYshipFiles.data.importPermit != null)
                {
                    ExportIsVisible = false;
                    TransmitIsVisible = false;
                    NoDataFoundLbl = getYshipFiles.data.importPermit.Count != 0 ? false : true;
                    ImportIsVisible = (NoDataFoundLbl) ? false : true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in TabTransmit " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This gets called when clicked on the Transmit tab.
        /// </summary>
        /// <returns></returns>
        private async Task TabTransmit()
        {
            try
            {
                SetFileName = "Please Select A File";
                AddFolder = "ChooseFile.png";
                IsFolderVisible = true;
                IsCrossVisible = false;
                PermitsNo = null;
                PermitsDate = PermitsDateDefaultVal;
                TabTransmitVisibility = true;
                TabExportVisibility = TabImportVisibility = false;
                TabOpacityTransmit = 1.0;
                TabOpacityExport = TabOpacityImport = 0.3;
                UploadType = (int)UploadTypeEnums.TransitPermit;

                if (getYshipFiles.data.transitPermit != null)
                {
                    ExportIsVisible = false;
                    ImportIsVisible = false;
                    NoDataFoundLbl = getYshipFiles.data.transitPermit.Count != 0 ? false : true;
                    TransmitIsVisible = (NoDataFoundLbl) ? false : true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipFileUploadViewModel method-> in TabTransmit " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
        #endregion

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
        private bool _InVoiceStackLayOut = true;
        public bool InVoiceStackLayOut
        {
            get => _InVoiceStackLayOut;
            set
            {
                _InVoiceStackLayOut = value;
                NotifyPropertyChanged();
            }
        }
        private bool _PackingStackLayout = false;
        public bool PackingStackLayout
        {
            get => _PackingStackLayout;
            set
            {
                _PackingStackLayout = value;
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
        private bool _BrowseFileFrm = true;
        public bool BrowseFileFrm
        {
            get => _BrowseFileFrm;
            set
            {
                _BrowseFileFrm = value;
                NotifyPropertyChanged();
            }
        }
        private int _BrowseFileRowHt = 55;
        public int BrowseFileRowHt
        {
            get => _BrowseFileRowHt;
            set
            {
                _BrowseFileRowHt = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ExportIsVisible = true;
        public bool ExportIsVisible
        {
            get => _ExportIsVisible;
            set
            {
                _ExportIsVisible = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ImportIsVisible = false;
        public bool ImportIsVisible
        {
            get => _ImportIsVisible;
            set
            {
                _ImportIsVisible = value;
                NotifyPropertyChanged();
            }
        }
        private bool _TransmitIsVisible = false;
        public bool TransmitIsVisible
        {
            get => _TransmitIsVisible;
            set
            {
                _TransmitIsVisible = value;
                NotifyPropertyChanged();
            }
        }
        private bool _PermitNoDateIsVsble = true;
        public bool PermitNoDateIsVsble
        {
            get => _PermitNoDateIsVsble;
            set
            {
                _PermitNoDateIsVsble = value;
                NotifyPropertyChanged();
            }
        }
        private int _PermitNoDateHt = 55;
        public int PermitNoDateHt
        {
            get => _PermitNoDateHt;
            set
            {
                _PermitNoDateHt = value;
                NotifyPropertyChanged();
            }
        }
        private int _PermitsRowSpacing = 4;
        public int PermitsRowSpacing
        {
            get => _PermitsRowSpacing;
            set
            {
                _PermitsRowSpacing = value;
                NotifyPropertyChanged();
            }
        }
        private int _BrowseFileRowHtPermits = 50;
        public int BrowseFileRowHtPermits
        {
            get => _BrowseFileRowHtPermits;
            set
            {
                _BrowseFileRowHtPermits = value;
                NotifyPropertyChanged();
            }
        }
        private bool _NoDataFoundLbl = false;
        public bool NoDataFoundLbl
        {
            get => _NoDataFoundLbl;
            set
            {
                _NoDataFoundLbl = value;
                NotifyPropertyChanged();
            }
        }
        private Color _TabBgColor = Color.FromHex("#269DC9");
        public Color TabBgColor
        {
            get => _TabBgColor;
            set
            {
                _TabBgColor = value;
                NotifyPropertyChanged();
            }
        }
        private bool _TabInVoiceVisibility = true;
        public bool TabInVoiceVisibility
        {
            get => _TabInVoiceVisibility;
            set
            {
                _TabInVoiceVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private bool _TabPackingVisibility = false;
        public bool TabPackingVisibility
        {
            get => _TabPackingVisibility;
            set
            {
                _TabPackingVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private bool _TabExportVisibility = true;
        public bool TabExportVisibility
        {
            get => _TabExportVisibility;
            set
            {
                _TabExportVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private bool _TabImportVisibility = false;
        public bool TabImportVisibility
        {
            get => _TabImportVisibility;
            set
            {
                _TabImportVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private bool _TabTransmitVisibility = false;
        public bool TabTransmitVisibility
        {
            get => _TabTransmitVisibility;
            set
            {
                _TabTransmitVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityInVoice = 1;
        public double TabOpacityInVoice
        {
            get => _TabOpacityInVoice;
            set
            {
                _TabOpacityInVoice = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityPacking = 0.3;
        public double TabOpacityPacking
        {
            get => _TabOpacityPacking;
            set
            {
                _TabOpacityPacking = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityExport = 1;
        public double TabOpacityExport
        {
            get => _TabOpacityExport;
            set
            {
                _TabOpacityExport = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityImport = 0.3;
        public double TabOpacityImport
        {
            get => _TabOpacityImport;
            set
            {
                _TabOpacityImport = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityTransmit = 0.3;
        public double TabOpacityTransmit
        {
            get => _TabOpacityTransmit;
            set
            {
                _TabOpacityTransmit = value;
                NotifyPropertyChanged();
            }
        }

        private string _AddFolder = "ChooseFile.png";
        public string AddFolder
        {
            get => _AddFolder;

            set
            {
                _AddFolder = value;
                NotifyPropertyChanged();
            }
        }

        private string _AddCross = "cross.png";
        public string AddCross
        {
            get => _AddCross;

            set
            {
                _AddCross = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsFolderVisible = true;
        public bool IsFolderVisible
        {
            get => _IsFolderVisible;

            set
            {
                _IsFolderVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsCrossVisible = false;
        public bool IsCrossVisible
        {
            get => _IsCrossVisible;

            set
            {
                _IsCrossVisible = value;
                NotifyPropertyChanged();
            }
        }


        #region File Upload InVoice and Packing list porperties
        private string _SetFileName = "Please Select A File";
        public string SetFileName
        {
            get => _SetFileName;
            set
            {
                _SetFileName = value;
                NotifyPropertyChanged();
            }
        }
        private string _FilePath64;
        public string FilePath64
        {
            get => _FilePath64;

            set
            {
                _FilePath64 = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<UploadFiles> _InvoiceList = new ObservableCollection<UploadFiles>();
        public ObservableCollection<UploadFiles> InvoiceList
        {
            get => _InvoiceList;
            set
            {
                _InvoiceList = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<UploadFiles> _PackingList = new ObservableCollection<UploadFiles>();
        public ObservableCollection<UploadFiles> PackingList
        {
            get => _PackingList;
            set
            {
                _PackingList = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<UploadFiles> _ExportList = new ObservableCollection<UploadFiles>();
        public ObservableCollection<UploadFiles> ExportList
        {
            get => _ExportList;
            set
            {
                _ExportList = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<UploadFiles> _ImportList = new ObservableCollection<UploadFiles>();
        public ObservableCollection<UploadFiles> ImportList
        {
            get => _ImportList;
            set
            {
                _ImportList = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<UploadFiles> _TransmitList = new ObservableCollection<UploadFiles>();
        public ObservableCollection<UploadFiles> TransmitList
        {
            get => _TransmitList;
            set
            {
                _TransmitList = value;
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
        public string _CheckUploadType;
        public string CheckUploadType
        {
            get { return _CheckUploadType; }
            set
            {
                _CheckUploadType = value;
                NotifyPropertyChanged();
            }
        }
        private string _PermitsNo;
        public string PermitsNo
        {
            get { return _PermitsNo; }
            set
            {
                _PermitsNo = value;
                NotifyPropertyChanged();
            }
        }
        public string _PermitsDate = "Permit date";
        public string PermitsDate
        {
            get { return _PermitsDate; }
            set
            {
                _PermitsDate = value;
                NotifyPropertyChanged();
            }
        }
        public string _PermitsDateDefaultVal;
        public string PermitsDateDefaultVal
        {
            get { return _PermitsDateDefaultVal; }
            set
            {
                _PermitsDateDefaultVal = value;
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
        private string _PermitNo = string.Empty;
        public string PermitNo
        {
            get { return _PermitNo; }
            set
            {
                _PermitNo = value;
                NotifyPropertyChanged();
            }
        }
        private int _PermitNoHeight = 30;
        public int PermitNoHeight
        {
            get => _PermitNoHeight;
            set
            {
                _PermitNoHeight = value;
                NotifyPropertyChanged();
            }
        }
        private int _PermitOnHeight = 30;
        public int PermitOnHeight
        {
            get => _PermitOnHeight;
            set
            {
                _PermitOnHeight = value;
                NotifyPropertyChanged();
            }
        }
        private string _PermitOn = string.Empty;
        public string PermitOn
        {
            get { return _PermitOn; }
            set
            {
                _PermitOn = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Dynamic text change
        public class fileuploadlabelchange
        {
            public string uploadBtnInvoicePL { get; set; } = "Upload";
            public string uploadeExpTrans { get; set; } = "Upload";
        }

        private fileuploadlabelchange _labelobjdefval = new fileuploadlabelchange();
        public fileuploadlabelchange labelobjdefval
        {
            get => _labelobjdefval;
            set
            {
                _labelobjdefval = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        #endregion
    }
}
