using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
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
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;

namespace YPS.ViewModel
{
    public class FileUploadViewModel : IBase
    {
        #region ICommands, variables declaration.
        IDownloader downloader = DependencyService.Get<IDownloader>();
        public ICommand ICommandPickFile { get; set; }
        public ICommand ICommandFileUpload { get; set; }
        public ICommand ICommandDeleteFile { get; set; }
        public ICommand ICommandDownloadFile { get; set; }
        public ICommand ICommandDescription { get; set; }
        public ICommand ICommandHome { get; set; }

        YPSService service;
        StartUploadFileModel DataForFileUpload;
        MyFile fileUpload;
        PLFileUpload plFileUploadData;
        public Stream picStream;
        MediaFile file;
        bool fileclosed;
        string UploadType, extension;
        int UploadTypevalue;
        int myFuid, poIdForClose; bool access;
        bool checkInternet;
        INavigation Navigation { get; set; }
        #endregion

        /// <summary>
        /// Parameterized contructor.
        /// </summary>
        /// <param name="navigation"></param>
        /// <param name="selectedTagsData"></param>
        /// <param name="poid"></param>
        /// <param name="fuid"></param>
        /// <param name="compareFileValue"></param>
        /// <param name="fileAccess"></param>
        public FileUploadViewModel(INavigation navigation, StartUploadFileModel selectedTagsData, int poid, int fuid, string compareFileValue, bool fileAccess)
        {
            YPSLogger.TrackEvent("FileUploadViewModel", " page load method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                #region Binding events to respective ICommand properties & new instance creation
                Navigation = navigation;
                ICommandPickFile = new Command(async () => await PickFileFromDevice());
                ICommandFileUpload = new Command(async () => await FileUpload());
                ICommandDeleteFile = new Command(DeleteFileClick);
                ICommandDownloadFile = new Command(DownloadFileClick);
                ICommandDescription = new Command(DescriptionClick);
                ICommandHome = new Command(Home_btn);
                ListOfFile = new ObservableCollection<MyFile>();
                DataForFileUpload = new StartUploadFileModel();
                fileUpload = new MyFile();
                service = new YPSService();
                fileclosed = fileAccess;
                plFileUploadData = new PLFileUpload();
                PLListOfFile = new ObservableCollection<PLFileUpload>();
                #endregion

                /// Start file upload that means initail file
                if (compareFileValue.ToLower().Trim() == "initialfile")
                {
                    UploadType = "initialFile";
                    UploadTypevalue = (int)UploadTypeEnums.TagFile;
                    DataForFileUpload = selectedTagsData;
                    poIdForClose = selectedTagsData.POID;
                    RowHeightUploadFileTitle = 40;
                    RowHeightTagNumber = 0;
                    HideListAndShow = false;
                    HideLabelAndShow = true;
                    closeLabelText = false;
                    RowHeightcomplete = 0;

                    tagNumbers = string.Join(" | ", selectedTagsData.FileTags.Select(c => c.TagNumber));

                    if (string.IsNullOrEmpty(tagNumbers))
                    {
                        tagNumbers = string.Join(" | ", selectedTagsData.FileTags.Select(c => c.IdentCode));
                    }

                    if (selectedTagsData.FileTags.Count > 0)
                    {
                        selectedTagsData.FileTags = selectedTagsData.FileTags.Select(c =>
                        {
                            c.TagNumber = null;
                            return c;
                        }).ToList();
                    }
                }
                else if (compareFileValue.ToLower().Trim() == "fileupload") /// After one file uploaded then this condition will execute.
                {
                    UploadType = "fileUpload";
                    myFuid = fuid;
                    poIdForClose = poid;
                    UploadTypevalue = (int)UploadTypeEnums.TagFile;

                    if (fileAccess)
                    {
                        FrameForChooseFile = false;
                        FrameForUploadFile = false;
                        closeLabelText = CompletedVal = true;
                        notCompeleteDisable = NotCompletedVal = false;
                        CompleteBtnOpacity = 0.5;
                        RowHeightcomplete = 50;
                        access = fileAccess;
                        RowHeightChooseFile = 0;
                        RowHeightUploadFile = 0;
                    }

                    GetMyFile(fuid); /// To fetch files based on file upload id.
                }
                else /// PL File file upload.
                {
                    IndicatorVisibility = true;
                    UploadType = "plFile";
                    UploadTypevalue = (int)UploadTypeEnums.PLFIle;
                    tagNumbers = selectedTagsData.alreadyExit;
                    closeLabelText = false;
                    RowHeightcomplete = 0;
                    plFileUploadData.POID = poid;

                    if (Settings.isFinalvol == 1)
                    {
                        RowHeightChooseFile = 0;
                        RowHeightUploadFile = 0;
                        FrameForUploadFile = false;
                        FrameForChooseFile = false;
                    }
                    else
                    {
                        RowHeightChooseFile = 50;
                        RowHeightUploadFile = 80;
                        FrameForUploadFile = true;
                        FrameForChooseFile = true;
                    }

                    GetPLMyFile(poid); /// To fetch PL files based on the POID. 
                }
                DynamicTextChange();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "FileUploadViewModel constructor-> in FileUploadViewModel.cs " + Settings.userLoginID);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on "Home" icon to redirect to home page.
        /// </summary>
        /// <param name="obj"></param>
        private async void Home_btn(object obj)
        {
            try
            {
                IndicatorVisibility = true;
                await Navigation.PopToRootAsync(false);
                //App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Home_btn method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when "Completed" or "Not completed" radio button is selected.
        /// </summary>
        /// <returns></returns>
        public async Task close_btn()
        {
            YPSLogger.TrackEvent("FileUploadViewModel", "in close_btn method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                NotCompletedVal = false;
                CompleteBtnOpacity = 0.5;


                bool result = await App.Current.MainPage.DisplayAlert("Complete", "Are you sure?", "Yes", "No");

                if (result)
                {
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (myFuid != 0 && poIdForClose != 0)
                        {
                            /// Calling CloseFile API to close file upload and passing two parameters "POID" and "FUID".
                            var cloaseResponse = await service.CloseFile(poIdForClose, myFuid);

                            if (cloaseResponse != null)
                            {
                                /// If file close succeessfully.
                                if (cloaseResponse.status == 1)
                                {
                                    access = true;
                                    ///MessagingCenter used for update main page grid data file count.
                                    MessagingCenter.Send<string, string>("FileComplted", "showFileAsComplete", myFuid.ToString());

                                    switch (Device.RuntimePlatform)
                                    {
                                        case Device.iOS:
                                            await App.Current.MainPage.DisplayAlert("Completed", "Success.", "Close");
                                            break;
                                        case Device.Android:
                                            DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                            break;
                                    }
                                    await Navigation.PopAsync(false);
                                }
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "Please upload a file before click on save button.", "OK");
                        }
                    }
                    else
                    {
                        CompletedVal = false;
                        NotCompletedVal = true;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                else
                {
                    CompletedVal = false;
                    NotCompletedVal = true;
                    CompleteBtnOpacity = 1.0;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "close_btn method-> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on "Upload" button to upload file.
        /// </summary>
        /// <returns></returns>
        private async Task FileUpload()
        {
            YPSLogger.TrackEvent("FileUploadViewModel.cs", " in FileUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (UploadBtnOpacity == 1.0)
                    {
                        if (SetFileName.ToLower().Trim() != "please choose a file")
                        {
                            if (UploadType.ToLower().Trim() == "initialfile" && myFuid == 0)
                            {
                                List<MyFile> phUploadList = new List<MyFile>();

                                StartUploadFileModel DataForFileUploadObj = new StartUploadFileModel();
                                DataForFileUploadObj = DataForFileUpload;
                                DataForFileUploadObj.CreatedBy = Settings.userLoginID;

                                MyFile phUpload = new MyFile();
                                phUpload.FUID = DataForFileUpload.FUID;
                                phUpload.FileURL = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                                phUpload.FileName = Path.GetFileNameWithoutExtension(FilePath64);
                                phUpload.CreatedBy = Settings.userLoginID;
                                phUpload.FileDescription = FileDescription;// uploadType;
                                phUpload.UploadType = UploadTypevalue;
                                phUpload.CreatedDate = String.Format(Settings.DateFormat, DateTime.Now);
                                phUpload.PicStream = picStream;
                                DataForFileUploadObj.files.Add(phUpload);

                                /// Calling the blob API to initial upload file.
                                var returnData = await BlobUpload.YPSFileUpload(UploadTypevalue, (int)BlobContainer.cnttagfiles, null, null, DataForFileUploadObj,
                                    null, null, null, "", "");

                                if (returnData != null)
                                {
                                    var FinalReturnData = returnData as RootObject;
                                    //myFuid = FinalReturnData.data.FUID;
                                    myFuid = FinalReturnData.data.files[0].FUID;

                                    if (FinalReturnData?.status == 1)
                                    {
                                        /// Checking image and file extention.
                                        string initialIcon = CheckExtensionOfImage(Path.GetExtension(FilePath64).ToLower());

                                        foreach (var file in DataForFileUpload.files)
                                        {
                                            ListOfFile.Insert(0, new MyFile() { FileName = file.FileName, GivenName = Settings.SGivenName, ImageURL = initialIcon, FileID = file.FileID, FileDescription = file.FileDescription, FileURL = file.FileURL, CreatedDate = file.CreatedDate, HideDeleteIc = true, HideDownloadFileIc = true, RoleName = file.RoleName, EntityName = file.EntityName });
                                        }

                                        HideListAndShow = true;
                                        HideLabelAndShow = false;
                                        /// MessagingCenter used for update main page grid data file count.
                                        MessagingCenter.Send<string, string>("FilesCountI", "msgFileInitial", ListOfFile.Count().ToString() + "," + myFuid);
                                        SetFileName = "Please choose a file";
                                        AddFolder = "ChooseFile.png";
                                        IsFolderVisible = true;
                                        IsCrossVisible = false;
                                        FileDescription = "";

                                        if (ListOfFile.Count() == 0)
                                        {
                                            ListOfFile = null;
                                            HideListAndShow = closeLabelText = false;
                                            HideLabelAndShow = true;
                                            RowHeightcomplete = 0;
                                        }

                                        foreach (var items in DataForFileUpload.FileTags)
                                        {
                                            if (items.TaskID != 0 && items.TagTaskStatus == 0)
                                            {
                                                TagTaskStatus tagtaskstatus = new TagTaskStatus();
                                                tagtaskstatus.TaskID = Helperclass.Encrypt(items.TaskID.ToString());
                                                tagtaskstatus.POTagID = Helperclass.Encrypt(items.POTagID.ToString());
                                                tagtaskstatus.Status = 1;
                                                tagtaskstatus.CreatedBy = Settings.userLoginID;

                                                var result = await service.UpdateTagTaskStatus(tagtaskstatus);

                                                if (result?.status == 1)
                                                {
                                                    if (items.TaskID != 0 && items.TaskStatus == 0)
                                                    {
                                                        TagTaskStatus taskstatus = new TagTaskStatus();
                                                        taskstatus.TaskID = Helperclass.Encrypt(items.TaskID.ToString());
                                                        taskstatus.TaskStatus = 1;
                                                        taskstatus.CreatedBy = Settings.userLoginID;

                                                        var taskval = await service.UpdateTaskStatus(taskstatus);
                                                    }
                                                }
                                            }
                                        }

                                        Settings.IsRefreshPartsPage = true;
                                        DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                        UploadType = "fileUpload";
                                    }
                                }
                            }
                            else if (UploadType == "fileUpload")
                            {
                                List<MyFile> phUploadlist = new List<MyFile>();

                                MyFile phUpload = new MyFile();
                                phUpload.FUID = myFuid;
                                phUpload.FileURL = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                                phUpload.FileName = Path.GetFileNameWithoutExtension(FilePath64);
                                phUpload.CreatedBy = Settings.userLoginID;
                                phUpload.FileDescription = FileDescription;// uploadType;
                                phUpload.CreatedDate = String.Format(Settings.DateFormat, DateTime.Now);
                                phUpload.PicStream = picStream;

                                phUploadlist.Add(phUpload);

                                /// Calling the blob API to upload file.
                                var returnplFileData = await BlobUpload.YPSFileUpload(UploadTypevalue,
                                    (int)BlobContainer.cnttagfiles, null, null, null, phUploadlist);

                                if (returnplFileData != null)
                                {
                                    var response = returnplFileData as SecondRootObject;

                                    if (response?.status == 1)
                                    {
                                        /// Checking image and file extention.
                                        string FileIcon = CheckExtensionOfImage(Path.GetExtension(FilePath64).ToLower());

                                        foreach (var val in response.data)
                                        {
                                            ListOfFile.Insert(0, new MyFile() { FileName = val.FileName, GivenName = Settings.SGivenName, ImageURL = FileIcon, FileID = val.FileID, FileDescription = val.FileDescription, FileURL = val.FileURL, CreatedDate = val.CreatedDate, HideDeleteIc = true, HideDownloadFileIc = true, RoleName = val.RoleName, EntityName = val.EntityName });
                                        }

                                        HideListAndShow = true;
                                        HideLabelAndShow = false;
                                        SetFileName = "Please choose a file";
                                        AddFolder = "ChooseFile.png";
                                        IsFolderVisible = true;
                                        IsCrossVisible = false;
                                        FileDescription = "";
                                        /// MessagingCenter used for update main page grid data file count.
                                        MessagingCenter.Send<string, string>("FilesCounts", "msgF", ListOfFile.Count().ToString() + "," + myFuid);

                                        Settings.IsRefreshPartsPage = true;
                                        DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    }
                                }
                            }
                            else
                            {
                                List<PLFileUpload> pluploadList = new List<PLFileUpload>();

                                PLFileUpload phUpload = new PLFileUpload();
                                phUpload.POID = plFileUploadData.POID;
                                phUpload.FileURL = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                                phUpload.FileName = Path.GetFileNameWithoutExtension(FilePath64);
                                phUpload.FileDescription = FileDescription;
                                phUpload.CreatedBy = Settings.userLoginID;
                                phUpload.CreatedDate = String.Format(Settings.DateFormat, DateTime.Now);
                                phUpload.PicStream = picStream;

                                pluploadList.Add(phUpload);

                                /// Calling the blob API to PL upload file.
                                var returnplData = await BlobUpload.YPSFileUpload(UploadTypevalue, (int)BlobContainer.cntplfiles, null, null, null, null, pluploadList);

                                if (returnplData != null)
                                {
                                    var finalplData = returnplData as PLFileUploadResult;

                                    if (finalplData?.status == 1)
                                    {
                                        /// Checking image and file extention.
                                        string PLIcon = CheckExtensionOfImage(Path.GetExtension(FilePath64).ToLower());
                                        foreach (var data in finalplData.data)
                                        {
                                            PLListOfFile.Insert(0, new PLFileUpload() { FileName = data.FileName, GivenName = Settings.SGivenName, FileURL = data.FileURL, ImageURL = PLIcon, ID = data.ID, FileDescription = data.FileDescription, CreatedDate = data.CreatedDate, HideDeleteIc = true, RoleName = data.RoleName, EntityName = data.EntityName });
                                        }
                                        PLHideListAndShow = true;
                                        HideLabelAndShow = false;
                                        SetFileName = "Please choose a file";
                                        AddFolder = "ChooseFile.png";
                                        IsFolderVisible = true;
                                        IsCrossVisible = false;
                                        FileDescription = "";
                                        DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    }
                                }
                            }

                            if (UploadType.ToLower().Trim() == "initialfile" || UploadType == "fileUpload")
                            {
                                closeLabelText = true;
                                RowHeightcomplete = 50;
                            }
                            else
                            {
                                closeLabelText = false;
                                RowHeightcomplete = 0;
                            }
                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please upload files having extensions:.png, .jpg, .gif, .bmp, .jpeg, .pdf, .txt, .doc, .docx only.");
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FileUpload method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method will get uploaded files from API.  
        /// </summary>
        /// <param name="fUID"></param>
        private void GetMyFile(int fUID)
        {
            YPSLogger.TrackEvent("FileUploadViewModel.cs ", " in GetMyFile method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            Device.BeginInvokeOnMainThread(async () =>
            {
                IndicatorVisibility = true;
                try
                {
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (Convert.ToString(fUID) != null)
                        {
                            /// Calling the API with "FUID" parameter.
                            var finalResult = await service.GetMyFileService(fUID);
                            ObservableCollection<MyFile> addItems = new ObservableCollection<MyFile>();

                            if (finalResult != null)
                            {
                                if (finalResult.status == 1)
                                {
                                    if (finalResult.data.fileTags.Count() != 0)
                                    {
                                        var result = finalResult.data.fileTags.Select(x => x.TagNumber).ToList();
                                        string result1 = String.Join(" | ", result);
                                        tagNumbers = result1;
                                    }
                                    string icon;

                                    foreach (var items in finalResult.data.files)
                                    {
                                        /// Checking image and file extention. 
                                        icon = CheckExtensionOfImage(Path.GetExtension(items.FileURL).ToLower());

                                        if (access)
                                        {
                                            addItems.Add(new MyFile() { FileName = items.FileName, GivenName = items.GivenName, ImageURL = icon, FileID = items.FileID, FileDescription = items.FileDescription, FileURL = items.FileURL, CreatedDate = items.CreatedDate, HideDeleteIc = false, HideDownloadFileIc = true, RoleName = items.RoleName, EntityName = items.EntityName });
                                        }
                                        else
                                        {
                                            addItems.Add(new MyFile() { FileName = items.FileName, GivenName = items.GivenName, ImageURL = icon, FileID = items.FileID, FileDescription = items.FileDescription, FileURL = items.FileURL, CreatedDate = items.CreatedDate, HideDeleteIc = true, HideDownloadFileIc = true, RoleName = items.RoleName, EntityName = items.EntityName });
                                        }
                                    }
                                    ListOfFile = addItems;
                                    HideListAndShow = true;
                                    HideLabelAndShow = false;

                                    if (fileclosed == true)
                                    {
                                        closeLabelText = true;
                                        RowHeightcomplete = 50;
                                    }
                                    else
                                    {
                                        closeLabelText = true;
                                        RowHeightcomplete = 50;
                                    }

                                    if (ListOfFile.Count() == 0)
                                    {
                                        HideListAndShow = false;
                                        HideLabelAndShow = true;

                                        if (fileclosed == true)
                                        {
                                            closeLabelText = false;
                                            RowHeightcomplete = 0;
                                        }
                                        else
                                        {
                                            closeLabelText = false;
                                            RowHeightcomplete = 0;
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
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "GetMyFile method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
                finally
                {
                    IndicatorVisibility = false;
                }
            });
        }

        /// <summary>
        /// This method will get PL uploaded files from API. 
        /// </summary>
        /// <param name="PoID"></param>
        private async void GetPLMyFile(int PoID)
        {
            await Task.Delay(100);
            IndicatorVisibility = true;
            YPSLogger.TrackEvent("FileUploadViewModel.cs ", "in GetPLMyFile method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (Convert.ToString(PoID) != null)
                    {
                        /// Calling PL/Files API with "POID".
                        var PlData = await service.GetPLUploadedFiles(PoID);
                        ObservableCollection<PLFileUpload> addItems = new ObservableCollection<PLFileUpload>();

                        if (PlData != null)
                        {
                            if (PlData.status == 1)
                            {
                                foreach (var items in PlData.data)
                                {
                                    /// Checking image and file extention.
                                    string PLicon = CheckExtensionOfImage(Path.GetExtension(items.FileURL).ToLower());

                                    if (Settings.isFinalvol == 1)
                                    {
                                        addItems.Add(new PLFileUpload() { FileName = items.FileName, GivenName = Settings.SGivenName, FileURL = items.FileURL, ImageURL = PLicon, ID = items.ID, FileDescription = items.FileDescription, CreatedDate = items.CreatedDate, HideDeleteIc = false, RoleName = items.RoleName, EntityName = items.EntityName });
                                    }
                                    else
                                    {
                                        addItems.Add(new PLFileUpload() { FileName = items.FileName, GivenName = Settings.SGivenName, FileURL = items.FileURL, ImageURL = PLicon, ID = items.ID, FileDescription = items.FileDescription, CreatedDate = items.CreatedDate, HideDeleteIc = true, RoleName = items.RoleName, EntityName = items.EntityName });
                                    }
                                }
                                PLListOfFile = addItems;
                                HideLabelAndShow = false;
                                PLHideListAndShow = true;
                            }
                            else
                            {
                                PLHideListAndShow = false;
                                HideLabelAndShow = true;
                            }
                        }
                        else
                        {
                            PLHideListAndShow = false;
                            HideLabelAndShow = true;
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
                YPSLogger.ReportException(ex, "GetPLMyFile method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on description icon to show file info in PopUp.
        /// </summary>
        /// <param name="sender"></param>
        private async void DescriptionClick(object sender)
        {
            IndicatorVisibility = true;

            try
            {
                if (UploadType.ToLower().Trim() == "initialfile" || UploadType.ToLower().Trim() == "fileupload")
                {
                    if (ListOfFile != null && ListOfFile.Count > 0)
                    {
                        fileUpload = (from res in ListOfFile where res.FileID == (int)sender select res).FirstOrDefault();
                        Uploadedby = fileUpload.GivenName;
                        RoleName = fileUpload.RoleName;
                        EntityName = fileUpload.EntityName;
                        UploadedDate = fileUpload.CreatedDate;
                        FileName = fileUpload.FileName;
                        Description = fileUpload.FileDescription;
                    }
                }
                else
                {
                    if (PLListOfFile != null && PLListOfFile.Count > 0)
                    {
                        var plFileUpload = (from res in PLListOfFile where res.ID == (int)sender select res).FirstOrDefault();
                        Uploadedby = plFileUpload.GivenName;
                        RoleName = plFileUpload.RoleName;
                        EntityName = plFileUpload.EntityName;
                        UploadedDate = plFileUpload.CreatedDate;
                        FileName = plFileUpload.FileName;
                        Description = plFileUpload.FileDescription;
                    }
                }
                PopUpForFileDes = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DescriptionClick method-> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
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
            YPSLogger.TrackEvent("FileUploadViewModel.cs ", " in DownloadFileClick method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
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
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();
                    /// If internet connection is available.

                    if (checkInternet)
                    {
                        if (UploadType.ToLower().Trim() == "initialfile" || UploadType.ToLower().Trim() == "fileupload")
                        {
                            if (ListOfFile != null && ListOfFile.Count > 0)
                            {
                                fileUpload = (from res in ListOfFile where res.FileURL == (string)sender select res).FirstOrDefault();
                                string folderName = (Device.RuntimePlatform == Device.Android) ? "Parts2y" : "Downloads";
                                downloader.DownloadFile(fileUpload.FileURL, folderName);
                                downloader.OnFileDownloaded += OnFileDownloaded;
                            }
                        }
                        else
                        {
                            if (PLListOfFile != null && PLListOfFile.Count > 0)
                            {
                                var plFileUpload = (from res in PLListOfFile where res.FileURL == (string)sender select res).FirstOrDefault();
                                downloader.DownloadFile(plFileUpload.FileURL, "Parts2y");
                                downloader.OnFileDownloaded += OnFileDownloaded;
                            }
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
                YPSLogger.ReportException(ex, "DownloadFileClick method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when file is download successfully to show alert message file is download successfully.
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
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnFileDownloaded method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on delete icon to delete single file.
        /// </summary>
        /// <param name="sender"></param>
        private async void DeleteFileClick(object sender)
        {
            YPSLogger.TrackEvent("FileUploadViewModel.cs ", "in DeleteFileClick method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (UploadType.ToLower().Trim() == "initialfile" || UploadType.ToLower().Trim() == "fileupload")
                    {
                        if (OpacityDeleteButton == 1.0)
                        {
                            MyFile fileUpload = null;

                            if (ListOfFile != null && ListOfFile.Count > 0)
                            {
                                bool result = await App.Current.MainPage.DisplayAlert("Delete", "Are you sure you want to delete?", "OK", "Cancel");
                                if (result)
                                {
                                    IndicatorVisibility = true;
                                    fileUpload = (from res in ListOfFile where res.FileID == (int)sender select res).FirstOrDefault();
                                    /// Calling DeleteFile API with "FileID".
                                    var deleteFile = await service.DeleteFileService(fileUpload.FileID);

                                    if (deleteFile?.status == 1)
                                    {
                                        var deleteItem = ListOfFile.Where(x => x.FileID == fileUpload.FileID).ToList();

                                        if (deleteItem.Count == 1)
                                        {
                                            ListOfFile.Remove(deleteItem.FirstOrDefault());
                                            MessagingCenter.Send<string, string>("FilesCount", "deleteFile", ListOfFile.Count().ToString() + "," + myFuid);

                                            switch (Device.RuntimePlatform)
                                            {
                                                case Device.iOS:
                                                    await App.Current.MainPage.DisplayAlert("Delete", "Success.", "Close");
                                                    break;
                                                case Device.Android:
                                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                                    break;
                                            }
                                            if (ListOfFile.Count() == 0)
                                            {
                                                HideListAndShow = false;
                                                HideLabelAndShow = true;
                                                closeLabelText = false;
                                                RowHeightcomplete = 0;
                                            }
                                        }
                                    }
                                    IndicatorVisibility = false;
                                }
                            }
                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                        }
                    }
                    else
                    {
                        if (PLOpacityDeleteButton == 1.0)
                        {
                            PLFileUpload PLfileUpload = null;

                            if (PLListOfFile != null && PLListOfFile.Count > 0)
                            {
                                bool result = await App.Current.MainPage.DisplayAlert("Alert", "Are you sure you want to delete?", "OK", "Cancel");

                                if (result)
                                {
                                    IndicatorVisibility = true;
                                    PLfileUpload = (from res in PLListOfFile where res.ID == (int)sender select res).FirstOrDefault();
                                    /// Calling DeletePLFile API with "POID".
                                    var PlDeleteResponse = await service.DeletePLFiles(PLfileUpload.ID);

                                    if (PlDeleteResponse?.status == 1)
                                    {
                                        var delete_Item = PLListOfFile.Where(x => x.ID == PLfileUpload.ID).ToList();

                                        if (delete_Item.Count == 1)
                                        {
                                            PLListOfFile.Remove(delete_Item.FirstOrDefault());

                                            switch (Device.RuntimePlatform)
                                            {
                                                case Device.iOS:
                                                    await App.Current.MainPage.DisplayAlert("Delete", "Success.", "Close");
                                                    break;
                                                case Device.Android:
                                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                                    break;
                                            }

                                            if (PLListOfFile.Count() == 0)
                                            {
                                                PLHideListAndShow = false;
                                                HideLabelAndShow = true;
                                                closeLabelText = false;
                                                RowHeightcomplete = 0;
                                            }
                                        }
                                    }
                                    IndicatorVisibility = false;
                                }
                            }
                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
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
                YPSLogger.ReportException(ex, "DeleteFileClick method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on "Please choose a file" layout to take photo and file from mobile. 
        /// </summary>
        /// <returns></returns>
        private async Task PickFileFromDevice()
        {
            YPSLogger.TrackEvent("FileUploadViewModel.cs ", "in MPickFileFromDevice method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                if (UploadBtnOpacity == 1.0)
                {
                    switch (Device.RuntimePlatform)
                    {
                        case Device.Android:

                            var selectOptionsForAndroid = await App.Current.MainPage.DisplayActionSheet("Select", "Cancel", null, "Photos", "Files");

                            switch (selectOptionsForAndroid)
                            {
                                case "Photos":
                                    /// Checking permission is allowed or denied by the user to access the photo from mobile for Android.
                                    var requestedPermissionsPhoto = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                                    var pass = requestedPermissionsPhoto[Permission.Photos];

                                    if (pass != PermissionStatus.Denied)
                                    {
                                        file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                                        {
                                            PhotoSize = PhotoSize.Custom,
                                            CustomPhotoSize = Settings.PhotoSize,
                                            CompressionQuality = Settings.CompressionQuality,
                                        });

                                        if (file == null)
                                        {
                                            return;
                                        }

                                        closeLabelText = false;
                                        RowHeightcomplete = 0;
                                        string photoName = Path.GetFileName(file.Path);
                                        extension = Path.GetExtension(file.Path);
                                        picStream = file.GetStream();
                                        IndicatorVisibility = true;

                                        if (extension == ".png" || extension == ".jpg" || extension == ".gif" || extension == ".bmp" || extension == ".jpeg" || extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                                        {
                                            SetFileName = photoName;
                                            AddCross = "cross.png";
                                            IsCrossVisible = true;
                                            IsFolderVisible = false;
                                            FilePath64 = file.Path;
                                        }
                                        else
                                        {
                                            await Application.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .png, .jpg, .gif, .bmp, .jpeg, .pdf, .txt, .doc, .docx only.", "OK");
                                        }
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to save files, please allow the permission in app permission settings.");
                                    }
                                    IndicatorVisibility = false;
                                    break;

                                case "Files":

                                    /// Checking permission is allowed or denied by the user to access the file from mobile for Android.
                                    var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                                    var requestedPermissionStatus = requestedPermissions[Permission.Storage];
                                    var pass1 = requestedPermissions[Permission.Storage];

                                    if (pass1 != PermissionStatus.Denied)
                                    {
                                        FileData fileData = await CrossFilePicker.Current.PickFile();

                                        if (fileData == null)
                                        {
                                            return; /// user canceled file picking
                                        }

                                        closeLabelText = false;
                                        RowHeightcomplete = 0;
                                        string AndroidfileName = fileData.FileName;
                                        string AndroidfilePath = fileData.FilePath;
                                        extension = Path.GetExtension(AndroidfilePath).ToLower();

                                        if (extension == "")
                                        {
                                            extension = Path.GetExtension(AndroidfileName).ToLower();
                                        }

                                        picStream = fileData.GetStream();
                                        IndicatorVisibility = true;

                                        if (extension == ".png" || extension == ".jpg" || extension == ".gif" || extension == ".bmp" || extension == ".jpeg" || extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                                        {
                                            SetFileName = AndroidfileName;
                                            AddCross = "cross.png";
                                            IsCrossVisible = true;
                                            IsFolderVisible = false;
                                            FilePath64 = AndroidfilePath;
                                        }
                                        else
                                        {
                                            await Application.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .png, .jpg, .gif, .bmp, .jpeg, .pdf, .txt, .doc, .docx only.", "OK");
                                        }
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to save files, please allow the permission in app permission settings.");
                                    }
                                    IndicatorVisibility = false;
                                    break;
                            }
                            break;

                        case Device.iOS:

                            var selectOptions = await App.Current.MainPage.DisplayActionSheet("Select", "Cancel", null, "Photos", "Files");

                            switch (selectOptions)
                            {
                                case "Photos":
                                    /// Checking permission is allowed or denied by the user to access the photo from mobile for iOS.
                                    var statusiOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                                    var pass = statusiOS[Permission.Photos];

                                    if (pass == PermissionStatus.Denied)
                                    {
                                        if (Application.Current.Properties.ContainsKey("CheckingPermissionDenied"))
                                        {
                                            var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission needs to access the photo gallery", null, null, "Maybe Later", "Settings");
                                            switch (checkSelect)
                                            {
                                                case "Maybe Later":

                                                    break;
                                                case "Settings":
                                                    CrossPermissions.Current.OpenAppSettings();
                                                    break;
                                            }
                                        }

                                        Application.Current.Properties["CheckingPermissionDenied"] = "1";
                                        await App.Current.SavePropertiesAsync();
                                    }
                                    else if (pass == PermissionStatus.Granted)
                                    {
                                        file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                                        {
                                            PhotoSize = PhotoSize.Custom,
                                            CustomPhotoSize = Settings.PhotoSize,
                                            CompressionQuality = Settings.CompressionQuality
                                        });

                                        if (file == null)
                                            return;

                                        closeLabelText = false;
                                        RowHeightcomplete = 0;
                                        string iOSPhotoName = Path.GetFileName(file.Path);

                                        extension = Path.GetExtension(file.Path);
                                        picStream = file.GetStream();
                                        IndicatorVisibility = true;

                                        if (extension == ".png" || extension == ".jpg" || extension == ".gif" || extension == ".bmp" || extension == ".jpeg" || extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                                        {
                                            SetFileName = iOSPhotoName;
                                            AddCross = "cross.png";
                                            IsCrossVisible = true;
                                            IsFolderVisible = false;
                                            FilePath64 = file.Path;
                                        }
                                        else
                                        {
                                            await Application.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .png, .jpg, .gif, .bmp, .jpeg, .pdf, .txt, .doc, .docx only.", "OK");
                                        }

                                        IndicatorVisibility = false;
                                    }
                                    break;

                                case "Files":


                                    FileData fileDataForiOS = await CrossFilePicker.Current.PickFile();

                                    if (fileDataForiOS == null)
                                        return; /// user canceled file picking

                                    closeLabelText = false;
                                    RowHeightcomplete = 0;
                                    string iOSfileName = fileDataForiOS.FileName;
                                    string iOSfilePath = fileDataForiOS.FilePath;
                                    extension = Path.GetExtension(iOSfilePath).ToLower();
                                    picStream = fileDataForiOS.GetStream();
                                    IndicatorVisibility = true;

                                    if (extension == ".png" || extension == ".jpg" || extension == ".gif" || extension == ".bmp" || extension == ".jpeg" || extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                                    {
                                        SetFileName = iOSfileName;
                                        AddCross = "cross.png";
                                        IsCrossVisible = true;
                                        IsFolderVisible = false;
                                        FilePath64 = iOSfilePath;
                                    }
                                    else
                                    {
                                        await Application.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .png, .jpg, .gif, .bmp, .jpeg, .pdf, .txt, .doc, .docx only.", "OK");
                                    }
                                    IndicatorVisibility = false;
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "MPickFileFromDevice method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is for checking image and file extension types.
        /// </summary>
        /// <param name="ImageExtension"></param>
        /// <returns></returns>
        private string CheckExtensionOfImage(string ImageExtension)
        {
            string imgExtension = "";

            try
            {
                if (ImageExtension == ".png")
                {
                    imgExtension = "PNG.png";
                }
                else if (ImageExtension == ".jpg")
                {
                    imgExtension = "JPG.png";
                }
                else if (ImageExtension == ".gif")
                {
                    imgExtension = "GIF.png";
                }
                else if (ImageExtension == ".bmp")
                {
                    imgExtension = "BMP.png";
                }
                else if (ImageExtension == ".jpeg")
                {
                    imgExtension = "JPEG.png";
                }
                else if (ImageExtension == ".pdf")
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
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckExtensionOfImage method-> in FileUploadViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
                imgExtension = "";
            }
            return imgExtension;
        }

        /// <summary>
        /// This method is for dynamic text change.
        /// </summary>
        private async void DynamicTextChange()
        {
            try
            {
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        labelobjFile = new labelchangeclass();

                        var uploadBtn = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjFile.upload.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var description = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjFile.description.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var uploadedBy = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjFile.uploadedBy.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var entityname = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjFile.entityname.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var uploadedDate = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjFile.uploadeddate.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var uploadFileName = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjFile.filename.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var complete = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjFile.complete.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var notcomplete = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjFile.notcommplete.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();

                        labelobjFile.upload = uploadBtn != null ? (!string.IsNullOrEmpty(uploadBtn) ? uploadBtn : labelobjFile.upload) : labelobjFile.upload;
                        labelobjFile.description = description != null ? (!string.IsNullOrEmpty(description) ? description : labelobjFile.description) : labelobjFile.description;
                        labelobjFile.descriptionlbl = (description != null ? (!string.IsNullOrEmpty(description) ? description : labelobjFile.descriptionlbl) : labelobjFile.descriptionlbl);
                        labelobjFile.uploadedBy = (uploadedBy != null ? (!string.IsNullOrEmpty(uploadedBy) ? uploadedBy : labelobjFile.uploadedBy) : labelobjFile.uploadedBy);
                        labelobjFile.entityname = (entityname != null ? (!string.IsNullOrEmpty(entityname) ? entityname : labelobjFile.entityname) : labelobjFile.entityname);
                        labelobjFile.uploadeddate = (uploadedDate != null ? (!string.IsNullOrEmpty(uploadedDate) ? uploadedDate : labelobjFile.uploadeddate) : labelobjFile.uploadeddate);
                        labelobjFile.filename = (uploadFileName != null ? (!string.IsNullOrEmpty(uploadFileName) ? uploadFileName : labelobjFile.filename) : labelobjFile.filename);
                        labelobjFile.complete = (complete != null ? (!string.IsNullOrEmpty(complete) ? complete : "Complete") : "Complete");
                        labelobjFile.notcommplete = (notcomplete != null ? (!string.IsNullOrEmpty(notcomplete) ? "Not " + notcomplete : "Not Complete") : "Not Complete");
                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    PLOpacityDeleteButton = Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PLUploadDelete").FirstOrDefault() != null ? 1.0 : 0.5;
                    OpacityDeleteButton = Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "FileDelete").FirstOrDefault() != null ? 1.0 : 0.5;
                    var hidepluploadButton = Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PLUpload").FirstOrDefault() != null ? true : false;
                    var hideploadButton = Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "FileUpload").FirstOrDefault() != null ? true : false;

                    if (hidepluploadButton == false && hideploadButton == false)
                    {
                        //ICommandPickFile = null;
                        HideUploadButton = false;
                        UploadBtnOpacity = 0.5;
                    }
                    else
                    {
                        if (hidepluploadButton == false && UploadType == "plFile")
                        {
                            HideUploadButton = false;
                            //ICommandPickFile = null;
                            UploadBtnOpacity = 0.5;
                        }
                        else if (hideploadButton == false && UploadType == "fileUpload")
                        {
                            HideUploadButton = false;
                            //ICommandPickFile = null;
                            UploadBtnOpacity = 0.5;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method -> in FileUploadViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        #region Properties

        public double _UploadBtnOpacity = 1.0;
        public double UploadBtnOpacity
        {
            get { return _UploadBtnOpacity; }
            set
            {
                _UploadBtnOpacity = value;
                NotifyPropertyChanged();
            }
        }

        public double _PLOpacityDeleteButton = 1.0;
        public double PLOpacityDeleteButton
        {
            get { return _PLOpacityDeleteButton; }
            set
            {
                _PLOpacityDeleteButton = value;
                NotifyPropertyChanged();
            }
        }

        public double _OpacityDeleteButton = 1.0;
        public double OpacityDeleteButton
        {
            get { return _OpacityDeleteButton; }
            set
            {
                _OpacityDeleteButton = value;
                NotifyPropertyChanged();
            }
        }

        public bool _HideUploadButton = true;
        public bool HideUploadButton
        {
            get { return _HideUploadButton; }
            set
            {
                _HideUploadButton = value;
                NotifyPropertyChanged();
            }
        }

        public bool _HidePLUploadButton = true;
        public bool HidePLUploadButton
        {
            get { return _HidePLUploadButton; }
            set
            {
                _HidePLUploadButton = value;
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

        private string _SetFileName = "Please choose a file";
        public string SetFileName
        {
            get => _SetFileName;
            set
            {
                _SetFileName = value;
                NotifyPropertyChanged();
            }
        }

        private string __FileDescription;
        public string FileDescription
        {
            get => __FileDescription;
            set
            {
                __FileDescription = value;
                NotifyPropertyChanged();
            }
        }

        private string _tagNumbers;
        public string tagNumbers
        {
            get => _tagNumbers;
            set
            {
                _tagNumbers = value;
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

        private bool _PLHideListAndShow = false;
        public bool PLHideListAndShow
        {
            get => _PLHideListAndShow;
            set
            {
                _PLHideListAndShow = value;
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
        private bool _FrameForChooseFile = true;
        public bool FrameForChooseFile
        {
            get => _FrameForChooseFile;
            set
            {
                _FrameForChooseFile = value;
                NotifyPropertyChanged();
            }
        }
        private bool _FrameForUploadFile = true;
        public bool FrameForUploadFile
        {
            get => _FrameForUploadFile;
            set
            {
                _FrameForUploadFile = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<MyFile> _ListOfFile;
        public ObservableCollection<MyFile> ListOfFile
        {
            get => _ListOfFile;
            set
            {
                _ListOfFile = value;
                NotifyPropertyChanged();
            }
        }

        private labelchangeclass _labelobjFile = new labelchangeclass();
        public labelchangeclass labelobjFile
        {
            get => _labelobjFile;
            set
            {
                _labelobjFile = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PLFileUpload> _PLListOfFile;
        public ObservableCollection<PLFileUpload> PLListOfFile
        {
            get => _PLListOfFile;
            set
            {
                _PLListOfFile = value;
                NotifyPropertyChanged();
            }
        }

        //Properties for Grid hide and show
        private int _RowHeightChooseFile = 50;
        public int RowHeightChooseFile
        {
            get => _RowHeightChooseFile;
            set
            {
                _RowHeightChooseFile = value;
                NotifyPropertyChanged();
            }
        }
        private int _RowHeightUploadFile = 80;
        public int RowHeightUploadFile
        {
            get => _RowHeightUploadFile;
            set
            {
                _RowHeightUploadFile = value;
                NotifyPropertyChanged();
            }
        }
        private int _RowHeightcomplete = 50;
        public int RowHeightcomplete
        {
            get => _RowHeightcomplete;
            set
            {
                _RowHeightcomplete = value;
                NotifyPropertyChanged();
            }
        }
        private int _RowHeightUploadFileTitle = 30;
        public int RowHeightUploadFileTitle
        {
            get => _RowHeightUploadFileTitle;
            set
            {
                _RowHeightUploadFileTitle = value;
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
        private bool _closeLabelText = true;
        public bool closeLabelText
        {
            get { return _closeLabelText; }
            set
            {
                _closeLabelText = value;
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

        private double _CompleteBtnOpacity { set; get; } = 1.0;
        public double CompleteBtnOpacity
        {
            get
            {
                return _CompleteBtnOpacity;
            }
            set
            {
                _CompleteBtnOpacity = value;
                RaisePropertyChanged("CompleteBtnOpacity");
            }
        }

        private string _RoleName;
        public string RoleName
        {
            get { return _RoleName; }
            set
            {
                _RoleName = value;
                NotifyPropertyChanged();
            }
        }

        private string _EntityName;
        public string EntityName
        {
            get { return _EntityName; }
            set
            {
                _EntityName = value;
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

        #region Assigning the values that nneded to be compared for dynamic text change.
        public class labelchangeclass
        {
            public string description { get; set; } = "Description";
            public string descriptionlbl { get; set; } = "Description";
            public string upload { get; set; } = "Upload";
            public string uploadedBy { get; set; } = "Created By";
            public string uploadeddate { get; set; } = "Created Date";
            public string entityname { get; set; } = "Company";
            public string filename { get; set; } = "File Name";
            public string complete { get; set; } = "LCUploadComplete";
            public string notcommplete { get; set; } = "LCUploadNotComplete ";
        }
        #endregion
        #endregion
    }
}
