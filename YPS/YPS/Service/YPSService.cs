using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.RestClientAPI;
using YPS.ViewModel;
using static YPS.Model.SearchModel;
using static YPS.Models.ChatMessage;
using System.Collections.Generic;

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
        /// Update task status of tag
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<TagTaskStatusUpdateResponse> UpdateTagTaskStatus(TagTaskStatus obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.UpdateTagTaskStatus(obj);
        }

        /// <summary>
        /// Update task status
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<TagTaskStatusUpdateResponse> UpdateTaskStatus(TagTaskStatus obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.UpdateTaskStatus(obj);
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
                    msg = true;
                    RestClient restClient = new RestClient();
                    trackData = await restClient.Handleexception(ErrorMessage);
                }
                else
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Please check your internet connection.", "Ok");
                    });
                    msg = true;
                }

            }
            catch (Exception ex)
            {
                if (!msg)
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Please check your internet connection.", "Ok");
                    });
                    YPSLogger.ReportException(ex, "Handleexception method -> in YPSService.cs -> this exception gets logged, when there is no internet connection and you try to hit api. We can ignore this..." + Settings.userLoginID);
                }
                else
                {
                    YPSLogger.ReportException(ex, "Handleexception method -> in YPSService.cs, UserID: " + Settings.userLoginID);
                }
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
        public async Task<object> SecondTimeFiles(List<MyFile> obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.UploadSecondTimeFiles(obj);
        }

        /// <summary>
        /// PL upload files.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> PLUploadFile(List<PLFileUpload> obj)
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
        public async Task<object> PhotosUpload(List<CustomPhotoModel> obj)
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
        /// GetallApplabelsService
        /// </summary>
        /// <returns></returns>
        public async Task<ActionsForUser> GetallActionStatusService(int userloginID)
        {
            RestClient restClient = new RestClient();
            return await restClient.AllActionStatus(userloginID);
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
        /// Get YShip Filter Data
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetFilterYshipData()
        {
            RestClient restClient = new RestClient();
            return await restClient.FilterYshipDataRClient();
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
            try
            {
                HttpClient httpClient = new HttpClient();
                string WebServiceUrl = HostingURL.WebServiceUrl;

                var response = httpClient.PostAsync(WebServiceUrl + "Login/GlobalSettings?SecurityCode=" + Settings.SecurityCode, null).Result;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var val = JsonConvert.DeserializeObject<ApplicationSettings>(jsonResponse);
                return val;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetglobelSettings method  -> in YPSService.cs-Exception=" + ex.Message + Settings.userLoginID);
                return null;
            }

        }

        /// <summary>
        /// FinalPhotosList
        /// </summary>
        /// <param name="puid"></param>
        /// <returns></returns>
        public async Task<LoadPhotosListResponse> GetLoadPhotos(int potagid)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetUploadedLoadPhotosRestClient(potagid);
        }

        /// <summary>
        /// Uploadfiles
        /// </summary>
        /// <param name="UploadFiles obj"></param>
        /// <returns></returns>
        public async Task<LoadPhotosListResponse> LoadPhotoUpload(List<LoadPhotoModel> loadphotodata)
        {
            RestClient restClient = new RestClient();
            return await restClient.LoadPhotoUpload(loadphotodata);
        }

        /// <summary>
        /// Delete load images
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteLoadImageService(int id)
        {
            RestClient restClient = new RestClient();
            return await restClient.DeleteLoadImagesRestClient(id);
        }

        /// <summary>
        /// GeInspectionResultsService
        /// </summary>
        /// <param name="taskid"></param>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<InspectionResults> GetInspectionResultsService(int taskid, int tagId)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetInspectionResultsClient(taskid, tagId);
        }

        /// <summary>
        /// GeInspectionResultsService
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<InspectionResults> GetInspectionResultsByTask(int taskid)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetInspectionResultsByTask(taskid);
        }

        /// <summary>
        /// GeInspectionPhotosService
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<InspectionPhotosResponse> GeInspectionPhotosByTag(int taskid, int tagId, int? questionId)
        {
            RestClient restClient = new RestClient();
            return await restClient.GeInspectionPhotosByTag(taskid, tagId, questionId);
        }

        /// <summary>
        /// GeInspectionPhotosService
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<InspectionPhotosResponse> GeInspectionPhotosByTask(int taskid, int? questionId)
        {
            RestClient restClient = new RestClient();
            return await restClient.GeInspectionPhotosByTask(taskid, questionId);
        }

        /// <summary>
        /// InsertInspectionPhotosService
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<UpdateInsertInspectionResponse> InsertInspectionPhotosService(List<UpdateInspectionRequest> updateInspectionRequest)
        {
            RestClient restClient = new RestClient();
            return await restClient.InsertInspectionPhotosClient(updateInspectionRequest);
        }

        /// <summary>
        /// Insert or Update Inspection Result
        /// </summary>
        /// <param name="updateInspectionRequest"></param>
        /// <returns></returns>
        public async Task<UpdateInspectionResponse> InsertUpdateInspectionResult(UpdateInspectionRequest updateInspectionRequest)
        {
            RestClient restClient = new RestClient();
            return await restClient.InsertUpdateInspectionResult(updateInspectionRequest);
        }

        /// <summary>
        /// Delete inspection Photos
        /// </summary>
        /// <param name="updateInspectionRequest"></param>
        /// <returns></returns>
        public async Task<DeletePhotoResponce> DeleteInspectionPhoto(int QID)
        {
            RestClient restClient = new RestClient();
            return await restClient.DeleteInspectionPhoto(QID);
        }

        /// <summary>
        /// GetAllMInspectionConfigurations
        /// </summary>
        /// <returns></returns>
        public async Task<InspectionConfigurationsResults> GetAllMInspectionConfigurations()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetAllMInspectionConfigurations();
        }

        /// <summary>
        /// Get Scan Config
        /// </summary>
        /// <returns></returns>
        public async Task<SaveScanConfigResponse> SaveScanConfig(int compareruleid = 0,
            int? scancount = 0,
             int polyboxruleid = 0, string polyboxlocname = "", int polyboxremarkid = 0,
             int polyboxstatusid = 0, string polyboxprintfields = "")
        {
            RestClient restClient = new RestClient();
            return await restClient.SaveScanConfig(compareruleid, Convert.ToInt32(scancount),
                polyboxruleid, polyboxlocname, polyboxremarkid, polyboxstatusid, polyboxprintfields);
        }


        /// <summary>
        /// Get saved compare config.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetSavedConfigResponse> GetSaveScanConfig()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetSaveScanConfig();
        }
        /// <summary>
        /// Get polybox header details
        /// </summary>
        /// <returns></returns>
        public async Task<PolyboxValidateResponse> GetPolyboxHeaderDetails()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetPolyboxHeaderDetails();
        }

        /// <summary>
        /// Get Scan Config
        /// </summary>
        /// <returns></returns>
        public async Task<ScanConfigResponse> GetScanConfig()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetScanConfig();
        }

        /// <summary>
        /// Get Repo Photos
        /// </summary>
        /// <returns></returns>
        public async Task<GetRepoPhotoResponse> GetRepoPhotos()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetRepoPhotos();
        }

        /// <summary>
        /// Upload Repo Photos
        /// </summary>
        /// <returns></returns>
        public async Task<GetRepoPhotoResponse> UploadRepoPhotos(List<PhotoRepoDBModel> repophotolist)
        {
            RestClient restClient = new RestClient();
            return await restClient.UploadRepoPhotos(repophotolist);
        }

        /// <summary>
        /// Delete Single Repo Photo
        /// </summary>
        /// <returns></returns>
        public async Task<GetRepoPhotoDelResponse> DeleteSingleRepoPhoto(int photoid)
        {
            RestClient restClient = new RestClient();
            return await restClient.DeleteSingleRepoPhoto(photoid);
        }

        /// <summary>
        /// Insert Insp Signatures
        /// </summary>
        /// <returns></returns>
        public async Task<InspectionResultsRespnse> InsertUpdateSignature(InspectionResultsList inspobj)
        {
            RestClient restClient = new RestClient();
            return await restClient.InsertUpdateSignature(inspobj);
        }

        /// <summary>
        /// Get Insp Signature By Task
        /// </summary>
        /// <returns></returns>
        public async Task<InspectionResults> GetInspSignatureByTag(int taskid, int potagid)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetInspSignatureByTag(taskid, potagid);
        }

        /// <summary>
        /// Get Insp Signature By Task
        /// </summary>
        /// <returns></returns>
        public async Task<InspectionResults> GetInspSignatureByTask(int taskid)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetInspSignatureByTask(taskid);
        }


        /// <summary>
        /// Delete Single Repo Photo
        /// </summary>
        /// <returns></returns>
        public async Task<GetRepoPhotoDelAllResponse> DeleteAllRepoPhoto()
        {
            RestClient restClient = new RestClient();
            return await restClient.DeleteAllRepoPhoto();
        }

        /// <summary>
        /// Get Saved User Search Settings
        /// </summary>
        /// <returns></returns>
        public async Task<GetSearchFilterListResponse> GetSavedUserSearchSettings()
        {
            RestClient restClient = new RestClient();
            return await restClient.GetSavedUserSearchSettings();
        }

        /// <summary>
        /// Get Saved User Search Settings by ID
        /// </summary>
        /// <returns></returns>
        public async Task<SearchSetting> GetSavedUserSearchSettingsByID(SearchPassData obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.GetSavedUserSearchSettingsByID(obj);
        }

        /// <summary>
        /// Delete User Search Filter
        /// </summary>
        /// <returns></returns>
        public async Task<SearchDataSimpleResponse> DeleteUserSearchFilter(SearchPassData obj)
        {
            RestClient restClient = new RestClient();
            return await restClient.DeleteUserSearchFilter(obj);
        }

        /// <summary>
        /// Assign, Unassigned Task
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<TagTaskStatusUpdateResponse> AssignUnassignedTask(int taskid)
        {
            RestClient restClient = new RestClient();
            return await restClient.AssignUnassignedTask(taskid);
        }

        /// <summary>
        /// Polybox scan validation
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<PolyboxValidateResponse> PolyboxScanValidation(string tagnumber)
        {
            RestClient restClient = new RestClient();
            return await restClient.PolyboxScanValidation(tagnumber);
        }

        /// <summary>
        /// Save Polybox Scan Data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<PolyboxSaveResponse> SavePolyboxScanData(PolyBoxModel polyboxscanobj)
        {
            RestClient restClient = new RestClient();
            return await restClient.SavePolyboxScanData(polyboxscanobj);
        }

        /// <summary>
        /// Get Byte Array for Polybox PDF File
        public async Task<PrintPDFModel> PrintPolyboxPDF(string tagnumber)
        {
            RestClient restClient = new RestClient();
            return await restClient.PrintPolyboxPDF(tagnumber); ;
        }

    }
}
