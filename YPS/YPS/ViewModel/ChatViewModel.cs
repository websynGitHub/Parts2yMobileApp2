using YPS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using YPS.Views;
using YPS.Service;
using YPS.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using static YPS.Models.ChatMessage;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;

namespace YPS.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        #region Data members
        public YPS.ChatServices.ChatServices chatServices;
        public ObservableCollection<ChatMessageViewModel> photoList = new ObservableCollection<ChatMessageViewModel>();
        ObservableCollection<ChatMessageViewModel> chatData1 = new ObservableCollection<ChatMessageViewModel>();
        public List<ObservableGroupCollection<DateTime, ChatMessageViewModel>> groupData = new List<ObservableGroupCollection<DateTime, ChatMessageViewModel>>();
        public event PropertyChangedEventHandler propertyChanged;
        YPSService service = new YPSService();// Creating new instance of the YPSService, which is used to call API
        MediaFile file;
        Stream sImageSource;
        public Stream picStream;
        ChatPage pageName;
        int count = 0;
        bool checkMail;
        public string gMessage = string.Empty, gGroupId = string.Empty, uploadType = string.Empty,
            base64Picture = "", fileName, mediaFile, extension = "", fileNameWithoutExtention;
        #endregion

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="podata"></param>
        /// <param name="page"></param>
        public ChatViewModel(ChatData podata, ChatPage page)
        {
            YPSLogger.TrackEvent("ChatViewModel", " Page loading " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Chat = new ObservableCollection<ChatMessageViewModel>();
                Chat1 = new ObservableCollection<ObservableGroupCollection<DateTime, ChatMessageViewModel>>();
                Settings.QaId = Settings.currentChatPage = podata.QAID;
                Settings.PoId = podata.POID;
                pageName = page;
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatViewModel with 2 param constructor -> in ChatViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="qaid"></param>
        /// <param name="poid"></param>
        /// <param name="page"></param>
        public ChatViewModel(int qaid, int poid, ChatPage page)
        {
            YPSLogger.TrackEvent("ChatViewModel", " Page loading with parms " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Chat = new ObservableCollection<ChatMessageViewModel>();
                Chat1 = new ObservableCollection<ObservableGroupCollection<DateTime, ChatMessageViewModel>>();
                pageName = page;
                Settings.QaId = qaid;
                Settings.PoId = poid;
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatViewModel with 3 param constructor -> in ChatViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Called when clicked on any Q&A from Q&A list, to establish a thread connection among group of that Q&A
        /// </summary>
        public async void Connect()
        {
            YPSLogger.TrackEvent("ChatViewModel", " Connect " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                chatServices = new ChatServices.ChatServices();
                _chatMessage = new ChatMessageViewModel();
                await chatServices.Connect();
                chatServices.OnChatMessageReceived += _chatServices_OnChatMessageReceived;// Binding _chatServices_OnChatMessageReceived method to OnChatMessageReceived
            }
            catch (Exception ex)
            {
                IndicatorVisibility = false;
                YPSLogger.ReportException(ex, "Connect method -> in ChatViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// Called to receive the sent message, for displaying it in conversation tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void _chatServices_OnChatMessageReceived(object sender, ChatMessage e)
        {
            YPSLogger.TrackEvent("ChatViewModel", " _chatServices_OnChatMessageReceived " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                e.IsMine = e.UserID == Settings.userLoginID ? true : false;
                Uri uriResult;

                var obj = new ChatMessageViewModel()// Creating object of ChatMessageViewModel with data
                {
                    Name = e.Name,
                    MessagUtcDateTime = e.MessagUtcDateTime.Date,
                    MessageType = e.MessageType,
                    IsMine = e.IsMine,
                    Message = e.MessageType == "T" || e.MessageType == "E" ? e.Message : null,
                    Image = e.MessageType == "P" ? e.Message : null,
                    Document = e.MessageType == "F" ? e.Message : null,
                };

                if (Chat1.Count > 0)
                {
                    if (Chat1.LastOrDefault().LastOrDefault().MessagUtcDateTime == obj.MessagUtcDateTime)
                    {
                        Chat1.LastOrDefault().Add(obj);
                    }
                    else
                    {
                        Chat.Add(obj);
                        GroupingAndAddingMessage1();// Grouping the maeesage and adding to Message1
                    }
                }
                else
                {
                    Chat.Add(obj);
                    GroupingAndAddingMessage1();// Grouping the maeesage and adding to Message1
                }

                pageName.Scroll();// Calling the Scroll method to scroll the page and show new message

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "_chatServices_OnChatMessageReceived method -> in ChatViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// Called when clicked on any Q&A from Q&A list, to get the conversation log
        /// </summary>
        /// <param name="qaid"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<ObservableGroupCollection<DateTime, ChatMessageViewModel>>> GetChatHistory(int qaid)
        {
            YPSLogger.TrackEvent("ChatViewModel", "in GetChatHistory method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                string time;
                Settings.QaId = qaid;
                var result = await service.ChatHistory(Settings.PoId, qaid, "", Settings.QAType);// Calling The API for chat history

                bgcount = Settings.ChatUserCount = result.UserCount;

                if (result != null && (result.data != null && result.data.Count > 0))
                {
                    Chat.Clear();

                    foreach (var item in result.data)
                    {

                        Uri uriResult;
                        bgcount = Settings.ChatUserCount = item.UserCount;
                        item.IsMine = item.UserID == Settings.userLoginID ? true : false;
                        time = !string.IsNullOrEmpty(item.CreatedDate.ToString()) ? String.Format(Settings.DateFormat, item.CreatedDate) : string.Empty;


                        var obj = new ChatMessageViewModel() // Creating object of ChatMessageViewModel with data
                        {
                            Name = item.FullName,
                            MessagDateTime = item.MessageType == "E" ? time + " @" : time,
                            MessageType = item.MessageType,
                            IsMine = item.IsMine,
                            Message = item.MessageType == "T" || item.MessageType == "E" ? item.MessageBody : null,
                            Image = item.MessageType == "P" ? item.MessageBody : null,
                            Document = item.MessageType == "F" ? item.MessageBody : null,
                            MessagUtcDateTime = item.CreatedDate.Date,
                        };
                        Chat.Add(obj);// Adding the object to the Message Observable
                    }
                    GroupingAndAddingMessage1();// Grouping the maeesage and adding to Message1
                }
            }
            catch (Exception ex)
            {
                IndicatorVisibility = false;
                YPSLogger.ReportException(ex, "GetChatHistory method -> in ChatViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            IndicatorVisibility = false;

            return Chat1;

        }

        /// <summary>
        /// This method groups the messages to as a list
        /// </summary>
        private async void GroupingAndAddingMessage1()
        {
            YPSLogger.TrackEvent("ChatViewModel", " GroupingAndAddingMessage1 " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                groupData =
             Chat.OrderBy(e => e.MessagUtcDateTime)
                 .GroupBy(e => e.MessagUtcDateTime)
                 .Select(e => new ObservableGroupCollection<DateTime, ChatMessageViewModel>(e)).ToList()
                 ;

                Chat1 = new ObservableCollection<ObservableGroupCollection<DateTime, ChatMessageViewModel>>(groupData);
            }
            catch (Exception ex)
            {
                IndicatorVisibility = false;
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GroupingAndAddingMessage method -> in ChatViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Called when clicked on any photo present in the conversation, to view the photo
        /// </summary>
        /// <param name="QAID"></param>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public async Task GetPhotoData(int? QAID, string messageType)
        {
            YPSLogger.TrackEvent("ChatViewModel", "in getPhotoData method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var chatPhotos = await service.ChatHistory(Settings.PoId, Settings.QaId, Settings.PhotoOption, Settings.QAType);// Calling API for getting the photo related data
                    photoList.Clear();

                    if (chatPhotos != null)
                    {
                        foreach (var item in chatPhotos.data)
                        {
                            ChatMessageViewModel photoData = new ChatMessageViewModel() { Image = item.MessageBody, Name = item.FullName, MessagDateTime = String.Format(Settings.DateFormat, item.CreatedDate), FileNameWithoutExtention = item.FileName };
                            photoList.Add(photoData);
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetPhotoData method -> in ChatViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// Called when clicked on the send icon, to send the text as message
        /// </summary>
        bool dismissCurrentSignalRConnection = false;
        public async void ExecuteSendMessageCommand()
        {
            YPSLogger.TrackEvent("ChatViewModel", "in ExecuteSendMessageCommand method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                IndicatorVisibility = true;

                if (dismissCurrentSignalRConnection) count = 0;

                if (count == 0)
                {
                    count = 1;

                    if (!String.IsNullOrWhiteSpace(_chatMessage.Message))
                    {
                        _chatMessage.Message = _chatMessage.Message.Trim();
                        {
                            if (Chat.Count <= 0)
                            {
                                chatServices.GetChatGroupName();
                            }

                            CMesssage sendchatdata = new CMesssage();
                            sendchatdata.MessageType = "T";
                            sendchatdata.UserName = Settings.Username;
                            sendchatdata.QAID = Settings.QaId;
                            sendchatdata.MessageBody = _chatMessage.Message;
                            sendchatdata.CreatedBy = Settings.userLoginID;
                            sendchatdata.UserID = Settings.userLoginID;
                            sendchatdata.MessageBase64 = "";
                            sendchatdata.QAType = Settings.QAType;
                            sendchatdata.CreatedDate = Convert.ToDateTime(String.Format(Settings.DateFormat, DateTime.Now));
                            bool checkInternet = false;

                            checkInternet = await App.CheckInterNetConnection();
                            if (checkInternet)
                            {
                                count = 0;
                                await chatServices.Send(sendchatdata);
                                if (checkMail != true)
                                    SaveMessageInDB(sendchatdata);
                                // dismissCurrentSignalRConnection = false;
                                _chatMessage.Message = string.Empty;
                                checkMail = false;
                            }
                            else
                            {
                                count = 0;
                                dismissCurrentSignalRConnection = true;
                                await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                                //DependencyService.Get<IToastMessage>().ShortAlert("Check your internet connection.");
                            }
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Input", "Please enter text.", "Ok");
                        dismissCurrentSignalRConnection = true;
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please enter text.");
                    }
                }
            }
            catch (Exception ex)
            {
                count = 0;
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ExecuteSendMessageCommand method -> in ChatViewModel.cs " + Settings.userLoginID);
                IndicatorVisibility = false;
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// Called When a Photo is choosen to send
        /// </summary>
        public async void selectPhoto(string MediaType)
        {
            YPSLogger.TrackEvent("ChatViewModel", "in selectPhoto method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                await Task.Delay(100);
                IndicatorVisibility = true;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    string FullFilename = null;
                    CMesssage sendchatdata = new CMesssage();

                    FullFilename = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + extension;
                    BlobUpload.UploadFile(CloudFolderKeyVal.GetBlobFolderName((int)BlobContainer.cntchatfiles), FullFilename, picStream);
                    sendchatdata.MessageType = (MediaType.Trim().ToLower()) == "photo" ? "P" : "F";

                    if (Chat.Count <= 0)
                    {
                        chatServices.GetChatGroupName();
                    }

                    sendchatdata.UserName = Settings.Username;
                    sendchatdata.QAID = Settings.QaId;
                    sendchatdata.MessageBase64 = FullFilename;
                    sendchatdata.CreatedBy = Settings.userLoginID;
                    sendchatdata.UserID = Settings.userLoginID;
                    sendchatdata.MessageBody = FullFilename;
                    sendchatdata.CreatedDate = DateTime.Now;
                    sendchatdata.QAType = Settings.QAType;
                    sendchatdata.FileName = fileNameWithoutExtention;
                    await chatServices.Send(sendchatdata);
                    SaveMessageInDB(sendchatdata);

                    //Checking for device's OS
                    if (Device.OS == TargetPlatform.Android)
                    {
                        if (!string.IsNullOrEmpty(mediaFile))
                        {
                            try
                            {
                                bool b = System.IO.File.Exists(mediaFile);
                                System.IO.File.Delete(mediaFile);
                            }
                            catch (Exception ex)
                            {
                                await service.Handleexception(ex);
                                YPSLogger.ReportException(ex, "if(Device.OS == TargetPlatform.Android) block in ChatViewModel -> in selectPhoto.cs " + Settings.userLoginID);
                            }
                        }
                    }
                    else
                    {
                        string pathToNewFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "temp");

                        if (Directory.Exists(pathToNewFolder))
                        {
                            try
                            {
                                Directory.Delete(pathToNewFolder, true);
                            }
                            catch (Exception ex)
                            {
                                await service.Handleexception(ex);
                                YPSLogger.ReportException(ex, "else block in ChatViewModel -> in Delete path " + Settings.userLoginID);
                            }
                        }
                        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        var files = System.IO.Directory.GetFiles(documents);

                        if (!files.Any())
                        {
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
                                YPSLogger.ReportException(ex, "foreach(var file in files) block in selectPhoto method ->  ChatViewModel.cs" + Settings.userLoginID);
                            }
                        }
                    }
                    base64Picture = string.Empty;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }

            }
            catch (Exception ex)
            {
                IndicatorVisibility = false;
                YPSLogger.ReportException(ex, "selectPhoto method -> in ChatViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on camera icon, to Capture/Choose photo
        /// </summary>
        public async void ExecuteSendImageCommand(object commandfrom)
        {
            YPSLogger.TrackEvent("ChatViewModel", "in ExecuteSendImageCommand method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;
            try
            {
                var label = commandfrom as Label;
                try
                {
                    //Checking for the OS of teh device
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        if (label.StyleId == "photo")
                        {
                            string action = await App.Current.MainPage.DisplayActionSheet("", "Cancel", null, "Camera", "Gallery"); //Only Photo

                            if (action.Trim().ToLower() == "camera")
                            {
                                IndicatorVisibility = true;

                                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                                {
                                    // Supply media options for saving our photo after it's taken.
                                    var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                                    {
                                        PhotoSize = PhotoSize.Custom,
                                        CustomPhotoSize = Settings.PhotoSize,
                                        CompressionQuality = Settings.CompressionQuality
                                    };

                                    // Take a photo of the business receipt.
                                    var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

                                    if (file != null)
                                    {
                                        IndicatorVisibility = false;
                                        extension = System.IO.Path.GetExtension(file.Path);
                                        picStream = file.GetStreamWithImageRotatedForExternalStorage();//GetStream();
                                        fileName = System.IO.Path.GetFileName(file.Path);// Get the file name
                                        mediaFile = file.Path;
                                        fileNameWithoutExtention = Path.GetFileNameWithoutExtension(file.Path);

                                        if (picStream != null)
                                        {
                                            selectPhoto("Photo");
                                        }
                                    }
                                    else
                                    {
                                        await App.Current.MainPage.DisplayAlert("Oops", "File not available at location.", "Ok");
                                    }
                                }
                                else
                                {
                                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable.", "Ok");
                                }
                            }
                            else if (action.Trim().ToLower() == "gallery")
                            {
                                if (!CrossMedia.Current.IsPickPhotoSupported)
                                {
                                    await App.Current.MainPage.DisplayAlert("Permissions Denied", "Unable to pick photos.", "Ok");
                                    return;
                                }
                                else
                                {
                                    SendImageFromGallery();
                                }
                                IndicatorVisibility = false;
                            }
                            //else if (action.Trim().ToLower() == "document")
                            //{
                            //    var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                            //    var requestedPermissionStatus = requestedPermissions[Permission.Storage];
                            //    var pass1 = requestedPermissions[Permission.Storage];

                            //    if (pass1 != PermissionStatus.Denied)
                            //    {
                            //        FileData fileData = await CrossFilePicker.Current.PickFile();

                            //        if (fileData == null)
                            //            return; /// user canceled file picking

                            //        string AndroidfileName = fileData.FileName;
                            //        string AndroidfilePath = fileData.FilePath;
                            //        extension = Path.GetExtension(AndroidfilePath).ToLower();

                            //        if (extension == "")
                            //        {
                            //            extension = Path.GetExtension(AndroidfileName).ToLower();
                            //        }

                            //        picStream = fileData.GetStream();
                            //        IndicatorVisibility = true;
                            //        if (extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                            //        {
                            //            if (picStream != null)
                            //            {
                            //                selectPhoto("Document");
                            //            }
                            //        }
                            //        else
                            //        {
                            //            await Application.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .pdf, .txt, .doc, .docx only.", "Ok");
                            //        }
                            //    }
                            //    else
                            //    {
                            //        DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to save files, please allow the permission in app permission settings");
                            //    }
                            //}

                            if (file == null)
                            {
                                IndicatorVisibility = false;
                                return;
                            }
                        }
                        else
                        {
                            var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                            var requestedPermissionStatus = requestedPermissions[Permission.Storage];
                            var pass1 = requestedPermissions[Permission.Storage];

                            if (pass1 != PermissionStatus.Denied)
                            {
                                FileData fileData = await CrossFilePicker.Current.PickFile();

                                if (fileData == null)
                                    return; /// user canceled file picking

                                string AndroidfileName = fileData.FileName;
                                string AndroidfilePath = fileData.FilePath;
                                extension = Path.GetExtension(AndroidfilePath).ToLower();

                                if (extension == "")
                                {
                                    extension = Path.GetExtension(AndroidfileName).ToLower();
                                }

                                picStream = fileData.GetStream();
                                IndicatorVisibility = true;
                                if (extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                                {
                                    if (picStream != null)
                                    {
                                        selectPhoto("Document");
                                    }
                                }
                                else
                                {
                                    await Application.Current.MainPage.DisplayAlert("Upload", "Please upload files having extensions: .pdf, .txt, .doc, .docx only.", "Ok");
                                }
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Action denied", "You don't have permission to do this action.", "Ok");
                                //DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to save files, please allow the permission in app permission settings.");
                            }
                        }
                    }
                    else
                    {
                        if (label.StyleId == "photo")
                        {
                            string action = await App.Current.MainPage.DisplayActionSheet("Select", "Cancel", null, "Camera", "Gallery"); //Only Photo

                            switch (action)
                            {
                                case "Camera":

                                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                                    {
                                        var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                                        {

                                            PhotoSize = PhotoSize.Custom,
                                            CustomPhotoSize = Settings.PhotoSize,
                                            CompressionQuality = Settings.CompressionQuality
                                        };
                                        var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

                                        if (file != null)
                                        {
                                            extension = System.IO.Path.GetExtension(file.Path);
                                            fileName = System.IO.Path.GetFileName(file.Path);
                                            picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                            mediaFile = file.Path;
                                            fileNameWithoutExtention = Path.GetFileNameWithoutExtension(file.Path);

                                            if (picStream != null)
                                            {
                                                selectPhoto("Photo");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable.", "Ok");
                                    }

                                    break;
                                case "Gallery":
                                    SendImageFromGallery();
                                    break;
                            }
                        }
                        else
                        {
                            FileData fileDataForiOS = await CrossFilePicker.Current.PickFile();

                            if (fileDataForiOS == null)
                                return; /// user canceled file picking

                            string iOSfileName = fileDataForiOS.FileName;
                            string iOSfilePath = fileDataForiOS.FilePath;
                            extension = Path.GetExtension(iOSfilePath).ToLower();
                            picStream = fileDataForiOS.GetStream();

                            if (extension == ".pdf" || extension == ".txt" || extension == ".doc" || extension == ".docx")
                            {
                                if (picStream != null)
                                {
                                    selectPhoto("Document");
                                }
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Alert", "Please upload files having extensions: .pdf, .txt, .doc, .docx only.", "Ok");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    await service.Handleexception(ex);
                    YPSLogger.ReportException(ex, "ExecuteSendImageCommand method -> in ChatViewModel.cs " + Settings.userLoginID);
                    IndicatorVisibility = false;
                }
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ExecuteSendImageCommand method -> in ChatViewModel.cs " + Settings.userLoginID);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when choosen to send photo from Gallery
        /// </summary>
        private async void SendImageFromGallery()
        {
            YPSLogger.TrackEvent("ChatViewModel", "in SendImageFromGallery method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    PhotoSize = PhotoSize.Custom,
                    CustomPhotoSize = Settings.PhotoSize,
                    CompressionQuality = Settings.CompressionQuality
                });

                if (file == null)
                {
                    IndicatorVisibility = false;
                    return;
                }

                IndicatorVisibility = false;
                picStream = file.GetStreamWithImageRotatedForExternalStorage();
                fileName = System.IO.Path.GetFileName(file.Path);
                mediaFile = file.Path;
                extension = System.IO.Path.GetExtension(file.Path);
                fileNameWithoutExtention = Path.GetFileNameWithoutExtension(file.Path);

                if (picStream != null)
                {
                    selectPhoto("Photo");
                }
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "SendImageFromGallery method -> in ChatViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on message icon, to send the message to as email along with normal message
        /// </summary>
        public async void ExecutesendEmailImageCommand()
        {
            YPSLogger.TrackEvent("ChatViewModel", "in ExecutesendEmailImageCommand method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (App.Proxy != null)
                    {
                        if (!string.IsNullOrWhiteSpace(_chatMessage.Message))
                        {
                            _chatMessage.Message = _chatMessage.Message.Trim();
                            App.CheckingSignalRConnection();

                            if (Chat.Count <= 0)
                            {
                                chatServices.GetChatGroupName();
                            }

                            IsBusy = true;
                            CMesssage sendchatdata = new CMesssage();
                            sendchatdata.QAID = Settings.QaId;
                            sendchatdata.MessageBody = _chatMessage.Message;
                            sendchatdata.POID = Settings.PoId;
                            sendchatdata.UserID = Settings.userLoginID;
                            sendchatdata.MessageType = "E";
                            sendchatdata.QAType = Settings.QAType;
                            sendchatdata.CreatedDate = DateTime.Now;
                            YPSService service = new YPSService();
                            var ChatMailrespose = await service.Chatmail(sendchatdata);

                            if (ChatMailrespose?.status == 1)
                            {
                                checkMail = true;
                                await App.Current.MainPage.DisplayAlert("Success", "Mail send success.", "Ok");
                                //DependencyService.Get<IToastMessage>().ShortAlert("Mail send success.");
                                _chatMessage.Message = string.Empty;
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Input", "Please enter text.", "Ok");
                            //DependencyService.Get<IToastMessage>().ShortAlert("Please enter text.");
                        }
                        IsBusy = false;
                    }
                    else
                    {
                        App.CheckingSignalRConnection();
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ExecutesendEmailImageCommand method -> in ChatViewModel.cs " + Settings.userLoginID);
                IndicatorVisibility = false;
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// This is for saving the messages into the DB. 
        /// </summary>
        /// <param name="data"></param>
        public async void SaveMessageInDB(CMesssage data)
        {
            await service.SaveMessageService(data);
        }

        /// <summary>
        /// Called when property changes.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            YPSLogger.TrackEvent("ChatViewModel", "in NotifyPropertyChanged method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                if (this.propertyChanged != null)
                {
                    this.propertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NotifyPropertyChanged method -> in ChatViewModel " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        #region Properties
        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                NotifyPropertyChanged("BgColor");
            }
        }

        private ObservableCollection<ChatMessageViewModel> _chat;
        private ObservableCollection<ObservableGroupCollection<DateTime, ChatMessageViewModel>> chat1;

        public ObservableCollection<ChatMessageViewModel> Chat
        {
            get { return _chat; }
            set
            {
                _chat = value;
                OnPropertyChanged("Chat");
            }
        }
        public ObservableCollection<ObservableGroupCollection<DateTime, ChatMessageViewModel>> Chat1
        {
            get { return chat1; }
            set
            {
                chat1 = value;
                OnPropertyChanged("Chat1");
            }
        }

        private ChatMessageViewModel _chatMessage;
        public ChatMessageViewModel ChatMessage
        {
            get { return _chatMessage; }
            set
            {
                _chatMessage = value;
                OnPropertyChanged("ChatMessage");
            }
        }

        private bool _textVisible = false;
        public bool textVisible
        {
            get { return _textVisible; }
            set
            {
                _textVisible = value;
                OnPropertyChanged("textVisible");
            }
        }

        private bool _imageVisible = false;
        public bool imageVisible
        {
            get { return _imageVisible; }
            set
            {
                _imageVisible = value;
                OnPropertyChanged("imageVisible");
            }
        }
        public bool _IndicatorVisibility;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();
                OnPropertyChanged("IndicatorVisibility");
            }
        }
        private List<NameIfo> _Userlist;
        public List<NameIfo> UserListCollections
        {
            get { return _Userlist; }
            set
            {
                _Userlist = value;
                NotifyPropertyChanged();
            }
        }
        private ChatData _chatlist;
        public ChatData chatListCollections
        {
            get { return _chatlist; }
            set
            {
                _chatlist = value;
                NotifyPropertyChanged();
            }
        }

        public string _GroupName;
        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                _GroupName = value;
                NotifyPropertyChanged();
            }
        }
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
        private bool _Lbl_Enable = true;
        public bool Lbl_Enable
        {
            get { return _Lbl_Enable; }
            set
            {
                _Lbl_Enable = value;
                OnPropertyChanged("Lbl_Enable");
            }
        }

        private bool _Img_Enable = true;
        public bool Img_Enable
        {
            get { return _Img_Enable; }
            set
            {
                _Img_Enable = value;
                OnPropertyChanged("Img_Enable");
            }
        }

        private int _bgcount;
        public int bgcount
        {
            get { return _bgcount; }
            set
            {
                _bgcount = value;
                OnPropertyChanged("bgcount");
            }
        }

        private bool _BaseOnPlatformEditorShowAndroid = false;
        public bool BaseOnPlatformEditorShowAndroid
        {
            get { return _BaseOnPlatformEditorShowAndroid; }
            set
            {
                _BaseOnPlatformEditorShowAndroid = value;
                OnPropertyChanged("BaseOnPlatformEditorShowAndroid");
            }
        }

        private bool _BaseOnPlatformEditorShowiOS = false;
        public bool BaseOnPlatformEditorShowiOS
        {
            get { return _BaseOnPlatformEditorShowiOS; }
            set
            {
                _BaseOnPlatformEditorShowiOS = value;
                OnPropertyChanged("BaseOnPlatformEditorShowiOS");
            }
        }

        public ICommand ChatExit { get; set; }
        public ICommand adduserclicked { get; set; }
        public ICommand Updatetitle { get; set; }

        /// <summary>
        /// Command to Send Message
        /// </summary>
        Command _sendEmailImageCommand;
        public Command SendEmailImageCommand
        {
            get
            {
                return _sendEmailImageCommand ??
                (_sendEmailImageCommand = new Command(ExecutesendEmailImageCommand));
            }
        }

        /// <summary>
        /// Command to Send Image
        /// </summary>
        Command sendImageCommand;
        public Command SendImageCommand
        {
            get
            {
                return sendImageCommand ??
                (sendImageCommand = new Command(ExecuteSendImageCommand));
            }
        }

        /// <summary>
        /// Command to Send Message
        /// </summary>
        Command sendMessageCommand;
        public Command SendMessageCommand
        {
            get
            {
                return sendMessageCommand ??
                (sendMessageCommand = new Command(ExecuteSendMessageCommand));
            }
        }

        private bool _IsEmailenable = true;
        public bool IsEmailenable
        {
            get { return _IsEmailenable; }
            set
            {
                _IsEmailenable = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
