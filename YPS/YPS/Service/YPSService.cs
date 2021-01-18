using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Model.Yship;
using YPS.RestClientAPI;
using YPS.ViewModel;
using YPS.YShip.YshipModel;
using static YPS.Model.SearchModel;
using static YPS.Models.ChatMessage;

namespace YPS.Service
{
    public class YPSService
    {
        /// <summary>
        /// Notification
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<NotifyDataModel> GetNotifyHistory(int userID)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetNotifyHistory(userID);
        }

        /// <summary>
        /// DefaultSettingProfile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<DefaultSettingModel> DefaultSettingProfile(int userId)
        {
            RestClient restClient = new RestClient();
            return await restClient.DefaultSettingProfile(userId);
        }

        /// <summary>
        /// Save user default profile settings
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SaveUDSModel> SaveUserDefaultSettings(SaveUserDefaultSettingsModel obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.SaveUserDefaultSettingsRClient(obj);
        }

        /// <summary>
        /// SaveUserPrioritySetting
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> SaveUserPrioritySetting(SaveUserDefaultSettingsModel obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.SaveUserPrioritySetting(obj);
        }

        /// <summary>
        /// SaveSerchvaluesSetting
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> SaveSerchvaluesSetting(SearchPassData obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.SaveSerchValues(obj);
        }

        /// <summary>
        /// Get save user default profile settings
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<ResponseFromSaveUDSModel> GetSaveUserDefaultSettings(int userID)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetSaveUserDefaultSettingsRClient(userID);
        }

        /// <summary>
        /// GetUserPrioritySettings
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<ResponseFromSaveUDSModel> GetUserPrioritySettings(int userID)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetUserPrioritySettingsRClient(userID);
        }

        /// <summary>
        /// GetSearchValuesService
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<SearchSetting> GetSearchValuesService(int userID)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetSearchValuesRC(userID);
        }

        /// <summary>
        /// Handleexception
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public async Task<object> Handleexception(Exception ErrorMessage)
        {
            bool msg = false;
            object trackData = null;
            try
            {
                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    RestClient restClient = new RestClient();
                    trackData = await restClient.Handleexception(ErrorMessage);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Please check your internet connection.", "Ok");
                    msg = true;
                }

            }
            catch (Exception ex)
            {
                if (!msg)
                    await App.Current.MainPage.DisplayAlert("Alert", "Please check your internet connection!", "Ok");
                YPSLogger.ReportException(ex, "Handleexception method-> in YPSService.cs:-> this is not an exception.if you try to hit api, when there is no internet connection then this exception will come.we can ignore this..." + Settings.userLoginID);
            }
            return trackData;
        }

        /// <summary>
        /// Get all uploaded files
        /// </summary>
        /// <param name="fuid"></param>
        /// <returns></returns>
        public async Task<GetStartUploadFileModel> GetMyFileService(int fuid)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetMyFileRestClient(fuid);
        }

        /// <summary>
        /// Delete files
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<DeleteFile> DeleteFileService(int fileId)
        {
            RestClient restClient = new RestClient();
            var delete = await restClient.DeleteFileRestClient(fileId);
            return delete;
        }

        /// <summary>
        /// GetHeaderFilterDataService
        /// </summary>
        /// <returns></returns>
        public async Task<GetHeaderFilter> GetHeaderFilterDataService()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetHeaderFilterDataRestClient();
        }

        /// <summary>
        /// GetHeaderFilterDataService
        /// </summary>
        /// <returns></returns>
        public async Task<GetYshipHeaderFilter> GetyShipHeaderFilterDataService()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetyShipHeaderFilterDataRestClient();
        }

        /// <summary>
        /// Start upload files.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> StartUploadFiles(StartUploadFileModel obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.StartUploadFiles(obj);
        }

        /// <summary>
        /// Second time upload files.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> SecondTimeFiles(MyFile obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.UploadSecondTimeFiles(obj);
        }

        /// <summary>
        /// PL upload files.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> PLUploadFile(PLFileUpload obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.PLUploadFilesRClient(obj);
        }

        /// <summary>
        /// Get PL uploaded files.
        /// </summary>
        /// <param name="POID"></param>
        /// <returns></returns>
        public async Task<GetPLFileUploadData> GetPLUploadedFiles(int POID)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetPLUploadedFilesRClient(POID);
        }

        /// <summary>
        /// Delete pl upload file.
        /// </summary>
        /// <param name="poID"></param>
        /// <returns></returns>
        public async Task<PLDeleteFileResponse> DeletePLFiles(int poID)
        {
            RestClient restClient = new RestClient();
            return await restClient.DeletePLFilesRClient(poID);
        }

        /// <summary>
        /// LoadPoDataService
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<GetPoData> LoadPoDataService(SendPodata obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.LoadPoDataRestClient(obj);
        }

        /// <summary>
        /// InitialUpload
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> InitialUpload(PhotoUploadModel obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.StartUploadPhotos(obj);
        }

        /// <summary>
        /// PhotosUpload
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> PhotosUpload(CustomPhotoModel obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.UploadPhotos(obj);
        }

        /// <summary>
        /// FinalPhotosList
        /// </summary>
        /// <param name="puid"></param>
        /// <returns></returns>
        public async Task<PhotosList> FinalPhotosList(int puid)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetUploadPhotosDataRestClient(puid);
        }

        /// <summary>
        /// Chat Users
        /// </summary>
        /// <returns></returns>
        public async Task<Getuserdata> GetChatusers(int? poid, int? qaid, int qatype)
        {
            RestClient restClient = new RestClient();
            return await restClient.Chatusers(poid, qaid, qatype);
        }

        /// <summary>
        /// Start Chat
        /// </summary>
        /// <returns></returns>
        public async Task<GetChatData> ChatStart(ChatData obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.StartChat(obj);
        }

        /// <summary>
        /// GetChatConversations 
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="potagid"></param>
        /// <returns></returns>
        public async Task<GetQADataList> GetChatConversations(int poid, int potagid, int QAType)
        {
            RestClient restClient = new RestClient();
            return await restClient.ChatConversations(poid, potagid, QAType);
        }

        /// <summary>
        /// GetArchivedChatConversations
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="qatype"></param>
        /// <returns></returns>
        public async Task<GetQADataList> GetArchivedChatConversations(int userid, int qatype)
        {
            RestClient restClient = new RestClient();
            return await restClient.ArchivedChatConversations(userid, qatype);
        }

        /// <summary>
        /// Chat Close
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="qoid"></param>
        /// <returns></returns>
        public async Task<CLoseChat> ConversationsClose(int? poid, int? qoid, int qatype)
        {
            RestClient restClient = new RestClient();
            return await restClient.Conversationsclose(poid, qoid, qatype);
        }

        /// <summary>
        /// ChatHistory
        /// </summary>
        /// <param name="qoid"></param>
        /// <returns></returns>
        public async Task<GetMessages> ChatHistory(int? poid, int? qoid, string PhotoType, int qatype)
        {
            RestClient restClient = new RestClient();
            return await restClient.chatHistory(poid, qoid, PhotoType, qatype);
        }

        /// <summary>
        /// UpdateChat users
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<string> updateChatuser(ChatData obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.UpdateChatUsers(obj);
        }

        /// <summary>
        /// Delete images
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteImageService(int id)
        {
            RestClient restClient = new RestClient();
            return await restClient.DeleteImagesRestClient(id);
        }

        /// <summary>
        /// Send Message 
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="potagid"></param>
        /// <returns></returns>
        public async Task<string> SaveMessageService(CMesssage mesobj)
        {
            string result = null;
            try
            {
                RestClient restClient = new RestClient();
                result = await restClient.SaveMessageRestClinet(mesobj);

            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;

        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public async Task<LoginUserData> LoginService(LoginData mail)
        {
            RestClient restClient = new RestClient();
            return await restClient.LoginRestClient(mail);
        }

        /// <summary>
        /// ClosePhotoData
        /// </summary>
        /// <param name="Poid"></param>
        /// <param name="Puid"></param>
        /// <returns></returns>
        public async Task<ClosePhotoResponse> ClosePhotoData(int Poid, int Puid)
        {
            RestClient restClient = new RestClient();
            return await restClient.ClosePhoto(Poid, Puid);
        }

        /// <summary>
        /// Calling close file service  
        /// </summary>
        /// <param name="POID"></param>
        /// <param name="FUID"></param>
        /// <returns></returns>
        public async Task<CloseResponse> CloseFile(int POID, int FUID)
        {
            RestClient restClient = new RestClient();
            return await restClient.CloseFileRClient(POID, FUID);
        }

        /// <summary>
        /// IIJUpdate
        /// </summary>
        /// <param name="iijdetails"></param>
        /// <returns></returns>
        public async Task<returnTokendetails> UpdateIIJtoken(SendTokendetails iijdetails)
        {
            RestClient restClient = new RestClient();
            return await restClient.updateIIJ(iijdetails);
        }

        /// <summary>
        /// Update title 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task<CLoseChat> UpdateChatTitle(string title, int qatype)
        {
            RestClient restClient = new RestClient();
            return await restClient.updatetitle(title, qatype);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="userobj"></param>
        /// <returns></returns>
        public async Task<PLDeleteFileResponse> Updateuser(UserUpdating userobj, int type)
        {
            RestClient restClient = new RestClient();
            return await restClient.UpdateUser(userobj, type);
        }

        /// <summary>
        /// GetTimeZone
        /// </summary>
        /// <returns></returns>
        public async Task<DDLMasterData> GetTimeZone()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetTimeZoneRClient();
        }

        /// <summary>
        /// GetLanguages()
        /// </summary>
        /// <returns>filterData</returns>
        public async Task<LanguagesModel> GetLanguages()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetLanguagesRC();
        }

        /// <summary>
        /// Get profile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetProfile> GetProfile(int userId)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetProfileRClinet(userId);
        }

        /// <summary>
        /// Update profile
        /// </summary>
        /// <param name="updateProfile"></param>
        /// <returns></returns>
        public async Task<updateProfileResponse> UpdateProfile(UpdateProfileData updateProfile)
        {
            RestClient restClient = new RestClient();
            return await restClient.UpdateProfileRClinet(updateProfile);
        }

        /// <summary>
        /// CheckDevice
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> CheckDevice(LoginModel obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.CheckDevice(obj);
        }

        /// <summary>
        /// RegisterNotification
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public async Task<string> RegisterNotification(DeviceRegistration dr)
        {
            RestClient restClient = new RestClient();
            return await restClient.RegisterNotification(dr);
        }

        /// <summary>
        /// DeleteRegistrationId
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public async Task<object> DeleteRegistrationId(DeviceRegistration dr)
        {
            RestClient restClient = new RestClient();
            return await restClient.DeleteRegistrationId(dr);
        }

        /// <summary>
        /// SaveNotification
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<object> SaveNotification(NotificationSettings ns)
        {
            RestClient restClient = new RestClient();
            var data = await restClient.SaveNotification(ns);
            return data;
        }

        /// <summary>
        /// Get Byte Array for Print PDF File 
        /// </summary>
        /// <param name="poTagID"></param>
        /// <returns></returns>
        public async Task<PrintPDFModel> PrintPDF(string poTagID)
        {
            RestClient restClient = new RestClient();
            return await restClient.PrintPDFRClient(poTagID); ;
        }

        /// <summary>
        /// Get Byte Array for Print PDF File 
        /// </summary>
        /// <param name="poTagID"></param>
        /// <returns></returns>
        public async Task<PrintPDFModel> PrintPDFByUsingPOID(int poId)
        {
            RestClient restClient = new RestClient();
            return await restClient.PrintPDFByUsingOPIDRClient(poId); ;
        }

        /// <summary>
        /// IsPhotoRequired
        /// </summary>
        /// <param name="potagIDs"></param>
        /// <param name="IsRequiredID"></param>
        /// <returns></returns>
        public async Task<ModelForIsPhotoRequired> IsRequiredOrNotReuired(string potagIDs, int IsRequiredID)
        {
            RestClient restClient = new RestClient();
            return await restClient.IsRequiredOrNotReuiredRClient(potagIDs, IsRequiredID); ;
        }

        /// <summary>
        /// LocationSearch
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<SearchDataRoot> SearchLocation(string LocationText = "", int LocationID = 0)
        {
            RestClient restClient = new RestClient();
            return await restClient.RClientSearchLoacation(LocationText, LocationID); ;
        }
        /// <summary>
        /// Chat Mail
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<CMessageResponse> Chatmail(CMesssage obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.SendChatMail(obj); ;
        }

        /// <summary>
        /// JW token
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public async Task<LoginUserData> getjwtoken()
        {
            RestClient restClient = new RestClient();
            return await restClient.Getjwtoken(); ;
        }

        /// <summary>
        /// GetallApplabelsService
        /// </summary>
        /// <returns></returns>
        public async Task<AllLabels> GetallApplabelsService()
        {
            RestClient restClient = new RestClient();
            return await restClient.alllabels();
        }

        /// <summary>
        /// Read notification
        /// </summary>
        /// <param name="qaid"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<Readnotification> ReadNotifyHistory(int qaid, int userid)
        {
            RestClient restClient = new RestClient();
            return await restClient.readNotifyHistory(qaid, userid); ;
        }

        /// <summary>
        /// ClearNotification
        /// </summary>
        /// <returns></returns>
        public async Task<Readnotification> clearNotifyHistory()
        {
            RestClient restClient = new RestClient();
            return await restClient.clearNotifyHistory(); ;
        }

        /// <summary>
        /// GetMobileBuilds
        /// </summary>
        /// <param name="mobileBModel"></param>
        /// <returns></returns>
        public async Task<MobileBFinalData> GetMobileBuilds(GetMobileBData mobileBModel)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetMobileBuildsRClinet(mobileBModel);
        }

        /// <summary>
        /// GetYshipData
        /// </summary>
        /// <returns></returns>
        public async Task<GetYshipData> LoadYshipData(yShipSearch yshipdata)
        {
            RestClient restClient = new RestClient();
            return await restClient.LoadYshipData(yshipdata);
        }

        /// <summary>
        /// Get YShip Filter Data
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetFilterYshipData()
        {
            RestClient restClient = new RestClient();
            return await restClient.FilterYshipDataRClient();
        }

        /// <summary>
        /// GetAllYShipPickerData
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<YShipPickerDataModel> GetAllYShipPickerData(int userId)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetAllYShipPickerDataRC(userId); ;
        }

        /// <summary>
        /// Update yShip Details
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUpdateyShipDetails> UpdateyShipDetailsService(UpdateyShipDetails obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.UpdateyShipDetailsRClient(obj); ;
        }

        /// <summary>
        /// Uploadfiles
        /// </summary>
        /// <param name="UploadFiles obj"></param>
        /// <returns></returns>
        public async Task<object> UploadFiles(UploadFiles obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.Uploadfiles(obj);
        }

        /// <summary>
        /// GetYshipfiles
        /// </summary>
        /// <param name="UploadFiles obj"></param>
        /// <returns></returns>
        public async Task<GetYshipFiles> GetYshipfiles(int yshipid, string type)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetYshiFiles(yshipid, type); ;
        }

        /// <summary>
        /// yShipDeleteFile
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="yShipId"></param>
        /// <param name="UploadType"></param>
        /// <returns></returns>
        public async Task<InVoiceDeleteFile> yShipDeleteFile(int ID, int yShipId, int UploadType)
        {
            RestClient restClient = new RestClient();
            return await restClient.RCyShipDeleteFile(ID, yShipId, UploadType); ;
        }

        /// <summary>
        /// GetyShipDetailService
        /// </summary>
        /// <param name="yShipId"></param>
        /// <param name="yBkgNo"></param>
        /// <returns></returns>
        public async Task<GetyShipDetailsResponse> GetyShipDetailService(int yShipId)
        {
            RestClient restClient = new RestClient();
            return await restClient.RCGetyShipDetails(yShipId); ;
        }

        /// <summary>
        /// CloseYShipSevice
        /// </summary>
        /// <param name="yShipId"></param>
        /// <param name="complete"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public async Task<YShipClose> CloseYShipSevice(int yShipId, int complete, int cancel)
        {
            RestClient restClient = new RestClient();
            return await restClient.RCCloseYShip(yShipId, complete, cancel); ;
        }

        /// <summary>
        /// LogoutService
        /// </summary>
        /// <returns></returns>
        public async Task<returnTokendetails> LogoutService()
        {
            RestClient restClient = new RestClient();
            return await restClient.Logout(); ;
        }

        /// <summary>
        /// GlobelSettings
        /// </summary>
        /// <returns></returns>
        public static async Task<ApplicationSettings> GetglobelSettings()
        {
            HttpClient httpClient = new HttpClient();
            string WebServiceUrl = HostingURL.WebServiceUrl;

            var response = httpClient.PostAsync(WebServiceUrl + "Login/GlobalSettings?SecurityCode=" + Settings.SecurityCode, null).Result;

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var val = JsonConvert.DeserializeObject<ApplicationSettings>(jsonResponse);
            return val;
        }
    }
}
