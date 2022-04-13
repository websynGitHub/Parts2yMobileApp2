using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;
using static YPS.Model.SearchModel;
using static YPS.Models.ChatMessage;
using System.Collections.Generic;

namespace YPS.RestClientAPI
{
    public class RestClient
    {
        #region Data member declaration
        RequestProvider requestProvider;
        HttpClient httpClient;
        YPSService service;
        private string WebServiceUrl = HostingURL.WebServiceUrl;
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public RestClient()
        {
            try
            {
                requestProvider = new RequestProvider();
                httpClient = new HttpClient();
                service = new YPSService();
                #region Public Key
                httpClient = new HttpClient(new NativeMessageHandler(
                        throwOnCaptiveNetwork: false,
                        customSSLVerification: true
                        ));
                #endregion

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.access_token);
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "RestClient constructor -> in RestClient.cs" + Settings.userLoginID);
            }
        }

        #region Common API calling methods.

        /// <summary>
        /// GetNotifyHistory
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<NotifyDataModel> GetNotifyHistory(int userID)
        {
            try
            {
                string url = WebServiceUrl + "Notification/Notifications?UserID=" + userID;
                return await requestProvider.PostAsync<NotifyDataModel>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetNotifyHistory method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// DefaultSettingProfile
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<DefaultSettingModel> DefaultSettingProfile(int userId)
        {
            try
            {
                string url = WebServiceUrl + "DDLMaster/ALLDefaultSettingMasters?UserID=" + userId;
                return await requestProvider.PostAsync<DefaultSettingModel>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DefaultSettingProfile method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// StartUploadFiles
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> StartUploadFiles(StartUploadFileModel obj)
        {
            try
            {
                return await requestProvider.PostAsync<StartUploadFileModel, RootObject>(WebServiceUrl + "Upload/File/StartUpload", obj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "StartUploadFiles method -> in RestClient.cs" + Settings.userLoginID);
                return string.Empty;
            }
        }

        /// <summary>
        /// SaveUserDefaultSettingsRClient
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<SaveUDSModel> SaveUserDefaultSettingsRClient(SaveUserDefaultSettingsModel obj)
        {
            try
            {
                return await requestProvider.PostAsync<SaveUserDefaultSettingsModel, SaveUDSModel>(WebServiceUrl + "PO/SaveUserDefaultSettings", obj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "SaveUserDefaultSettingsRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        public async Task<TagTaskStatusUpdateResponse> UpdateTagTaskStatus(TagTaskStatus obj)
        {
            try
            {
                return await requestProvider.PostAsync<TagTaskStatusUpdateResponse>(WebServiceUrl + "Task/UpdateTaskTagStatus?TaskID=" + obj.TaskID + "&POTagID="
                    + obj.POTagID + "&Status=" + obj.Status + "&CreatedBy=" + obj.CreatedBy);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdateTagTaskStatus method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        public async Task<TagTaskStatusUpdateResponse> UpdateTaskStatus(TagTaskStatus obj)
        {
            try
            {
                return await requestProvider.PostAsync<TagTaskStatusUpdateResponse>(WebServiceUrl + "Task/UpdateTaskStatus?TaskID=" + obj.TaskID +
                    "&Status=" + obj.TaskStatus + "&CreatedBy=" + obj.CreatedBy);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdateTaskStatus method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// SaveUserPrioritySetting
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> SaveUserPrioritySetting(SaveUserDefaultSettingsModel obj)
        {
            try
            {
                return await requestProvider.PostAsync<SaveUserDefaultSettingsModel, object>(WebServiceUrl + "PO/SaveUserPrioritySetting", obj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "SaveUserPrioritySetting method -> in RestClient.cs" + Settings.userLoginID);
                return string.Empty;
            }
        }

        /// <summary>
        /// GetUserPrioritySettingsRClient
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseFromSaveUDSModel> GetUserPrioritySettingsRClient(int userId)
        {
            try
            {
                string url = WebServiceUrl + "PO/GetUserPrioritySettings?UserID=" + userId + "&VersionID=" + Settings.VersionID;
                return await requestProvider.PostAsync<ResponseFromSaveUDSModel>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetUserPrioritySettingsRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GetSearchValuesRC
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<SearchSetting> GetSearchValuesRC(int userId)
        {
            try
            {
                string url = WebServiceUrl + "PO/GetUserSearchSettings?UserID=" + userId
                    + "&CompanyID=" + Settings.CompanyID
                    + "&ProjectID=" + Settings.ProjectID
                    + "&JobID=" + Settings.JobID;
                return await requestProvider.PostAsync<SearchSetting>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetSerchvalues method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// SaveSerchValues
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<object> SaveSerchValues(SearchPassData data)
        {
            try
            {
                return await requestProvider.PostAsync<SearchPassData, object>(WebServiceUrl + "PO/SaveUserSearchSetting", data);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "SaveSerchvalues method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GetSaveUserDefaultSettingsRClient
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResponseFromSaveUDSModel> GetSaveUserDefaultSettingsRClient(int userId)
        {
            try
            {
                string url = WebServiceUrl + "PO/GetUserDefaultSettings?UserID=" + userId;
                return await requestProvider.PostAsync<ResponseFromSaveUDSModel>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetSaveUserDefaultSettingsRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// UploadSecondTimeFiles
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> UploadSecondTimeFiles(List<MyFile> obj)
        {
            try
            {
                return await requestProvider.PostAsync<List<MyFile>, SecondRootObject>(WebServiceUrl + "Upload/File/Upload", obj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UploadSecondTimeFiles method -> in RestClient.cs" + Settings.userLoginID);
                return string.Empty;
            }
        }

        /// <summary>
        /// GetMyFileRestClient
        /// </summary>
        /// <param name="fuid"></param>
        /// <returns></returns>
        public async Task<GetStartUploadFileModel> GetMyFileRestClient(int fuid)
        {
            try
            {
                string url = WebServiceUrl + "Upload/File/Files?FUID=" + Helperclass.Encrypt(Convert.ToString(fuid)) + "&UserID=" + Settings.userLoginID;
                return await requestProvider.PostAsync<GetStartUploadFileModel>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetMyFileRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// DeleteFileRestClient
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public async Task<DeleteFile> DeleteFileRestClient(int fileId)
        {
            try
            {
                string url = WebServiceUrl + "Upload/File/DeleteFile?UserID=" + Settings.userLoginID + "&ID=" + Helperclass.Encrypt(Convert.ToString(fileId));
                return await requestProvider.PostAsync<DeleteFile>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DeleteFileRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// PLUploadFilesRClient
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> PLUploadFilesRClient(List<PLFileUpload> obj)
        {
            try
            {
                return await requestProvider.PostAsync<List<PLFileUpload>, PLFileUploadResult>(WebServiceUrl + "Upload/PL/Upload", obj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "PLUploadFilesRClient method -> in RestClient.cs" + Settings.userLoginID);
                return string.Empty;
            }
        }

        /// <summary>
        /// GetPLUploadedFilesRClient
        /// </summary>
        /// <param name="POID"></param>
        /// <returns></returns>
        public async Task<GetPLFileUploadData> GetPLUploadedFilesRClient(int POID)
        {
            try
            {
                string url = WebServiceUrl + "Upload/PL/Files?UserID=" + Settings.userLoginID + "&POID=" + Helperclass.Encrypt(Convert.ToString(POID));
                return await requestProvider.PostAsync<GetPLFileUploadData>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetPLUploadedFilesRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }

        }

        /// <summary>
        /// DeletePLFilesRClient
        /// </summary>
        /// <param name="poID"></param>
        /// <returns></returns>
        public async Task<PLDeleteFileResponse> DeletePLFilesRClient(int poID)
        {
            try
            {
                string url = WebServiceUrl + "Upload/PL/DeleteFile?UserID=" + Settings.userLoginID + "&ID=" + Helperclass.Encrypt(Convert.ToString(poID));
                return await requestProvider.PostAsync<PLDeleteFileResponse>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DeletePLFilesRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// LoadPoDataRestClient
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<GetPoData> LoadPoDataRestClient(SendPodata obj)
        {
            try
            {
                return await requestProvider.PostAsync<SendPodata, GetPoData>(WebServiceUrl + "PO/PODataWithTags", obj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoadPoDataRestClient method -> in RestClient.cs" + Settings.userLoginID); return null;
            }
        }

        /// <summary>
        /// This event will get all drop donw values 
        /// </summary>
        /// <returns></returns>
        public async Task<GetHeaderFilter> GetHeaderFilterDataRestClient()
        {
            try
            {
                string url = WebServiceUrl + "DDLMaster/ALLFiliterMasters?UserID=" + Settings.userLoginID;
                return await requestProvider.PostAsync<GetHeaderFilter>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetHeaderFilterDataRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// This event for getting the picker values of yShip page 
        /// </summary>
        /// <returns></returns>
        public async Task<GetYshipHeaderFilter> GetyShipHeaderFilterDataRestClient()
        {
            try
            {
                string url = WebServiceUrl + "DDLMaster/ALLyShipMasters?UserID=" + Settings.userLoginID;
                return await requestProvider.PostAsync<GetYshipHeaderFilter>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetHeaderFilterDataRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GetCountryFilterDataRestClient
        /// </summary>
        /// <returns></returns>
        public async Task<DDLMasterData> GetCountryFilterDataRestClient()
        {
            try
            {
                return await requestProvider.PostAsync<DDLMasterData>(WebServiceUrl + "DDLMaster/Country/ALL?UserID=" + Settings.userLoginID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetCountryFilterDataRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// StartUploadPhotos
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> StartUploadPhotos(PhotoUploadModel obj)
        {
            try
            {
                return await requestProvider.PostAsync<PhotoUploadModel, InitialResponse>(WebServiceUrl + "Upload/Photo/StartUpload", obj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "StartUploadPhotos method -> in RestClient.cs" + Settings.userLoginID);
                return string.Empty;
            }
        }

        /// <summary>
        /// UploadPhotos
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> UploadPhotos(List<CustomPhotoModel> obj)
        {

            try
            {
                return await requestProvider.PostAsync<List<CustomPhotoModel>, SecondTimeResponse>(WebServiceUrl + "Upload/Photo/Upload", obj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UploadPhotos method -> in RestClient.cs" + Settings.userLoginID);
                return string.Empty;
            }
        }

        /// <summary>
        /// GetUploadPhotosDataRestClient
        /// </summary>
        /// <param name="puid"></param>
        /// <returns></returns>
        public async Task<PhotosList> GetUploadPhotosDataRestClient(int puid)
        {
            try
            {
                string url = WebServiceUrl + "Upload/Photo/Photos?UserID=" + Settings.userLoginID + "&PUID=" + Helperclass.Encrypt(Convert.ToString(puid));// + "&Authorization=" + "";
                return await requestProvider.PostAsync<PhotosList>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetUploadPhotosDataRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Chatusers
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="qaid"></param>
        /// <param name="qatype"></param>
        /// <returns></returns>
        public async Task<Getuserdata> Chatusers(int? poid, int? qaid, int qatype)
        {
            try
            {
                string url = WebServiceUrl + "QA/Users?POID=" + Helperclass.Encrypt(Convert.ToString(poid))
                    + "&QAID=" + Helperclass.Encrypt(Convert.ToString(qaid))
                    + "&UserID=" + Settings.userLoginID + "&QAType=" + qatype + "";
                return await requestProvider.PostAsync<Getuserdata>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "Chatusers method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Start Chat
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<GetChatData> StartChat(ChatData obj)
        {
            try
            {
                return await requestProvider.PostAsync<ChatData, GetChatData>(WebServiceUrl + "QA/Start", obj);

            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "StartChat method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// ChatConversations (POID,POTagID)
        /// </summary>
        /// <returns></returns>
        public async Task<GetQADataList> ChatConversations(int poid, int potagid, int QAType)
        {
            try
            {
                string url = WebServiceUrl + "/QA/Conversations?POID=" + Helperclass.Encrypt(Convert.ToString(poid)) + "&POTagID=" + Helperclass.Encrypt(Convert.ToString(potagid)) + "&UserID=" + Settings.userLoginID + "&QAType=" + QAType + "";
                return await requestProvider.PostAsync<GetQADataList>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatConversations method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// ArchivedChatConversations
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="QAType"></param>
        /// <returns></returns>
        public async Task<GetQADataList> ArchivedChatConversations(int userid, int QAType)
        {
            try
            {
                string url = WebServiceUrl + "/QA/ArchivedConversations?UserID=" + userid + "&QAType=" + QAType + "";
                return await requestProvider.PostAsync<GetQADataList>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatConversations method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }


        /// <summary>
        /// Conversationsclose
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="qaid"></param>
        /// <returns></returns>
        public async Task<CLoseChat> Conversationsclose(int? poid, int? qaid, int qatype)
        {
            try
            {
                string url = WebServiceUrl + "/QA/Close?QAID=" + Helperclass.Encrypt(Convert.ToString(qaid)) + "&CreatedBy=" + Settings.userLoginID + "&QAType=" + qatype + "";
                return await requestProvider.PostAsync<CLoseChat>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "Conversationsclose method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// chatHistory
        /// </summary>
        /// <param name="qaid"></param>
        /// <returns></returns>
        public async Task<GetMessages> chatHistory(int? poid, int? qaid, string PhotoType, int qatype)
        {
            try
            {
                string url = "";

                if (string.IsNullOrEmpty(PhotoType))
                {
                    if (qaid > 0)
                    {
                        url = WebServiceUrl + "/QA/Messages?POID=" + Helperclass.Encrypt(Convert.ToString(poid)) +
                        "&QAID=" + Helperclass.Encrypt(Convert.ToString(qaid)) + "&UserID=" +
                        Settings.userLoginID + "&QAType=" + qatype + "";
                    }
                    else if (qaid == 0)
                    {
                        url = WebServiceUrl + "/QA/GetWhiteBoardMessages?UserID=" + Settings.userLoginID;
                    }

                }
                else
                {
                    if (qaid > 0)
                    {
                        url = WebServiceUrl + "/QA/Messages?POID=" + Helperclass.Encrypt(Convert.ToString(poid)) + "&QAID=" + Helperclass.Encrypt(Convert.ToString(qaid)) + "&UserID=" + Settings.userLoginID + "&QAType=" + qatype + "&MessageType=" + PhotoType + "";
                    }
                    else if (qaid == 0)
                    {
                        url = WebServiceUrl + "/QA/GetWhiteBoardMessages?UserID=" + Settings.userLoginID + "&QAType=" + qatype + "&MessageType=" + PhotoType + "";
                    }
                }
                return await requestProvider.PostAsync<GetMessages>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "chatHistory method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// delete images
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteImagesRestClient(int id)
        {
            try
            {
                string url = WebServiceUrl + "Upload/Photo/DeletePhoto?UserID=" + Settings.userLoginID + "&ID=" + Helperclass.Encrypt(Convert.ToString(id));// + "&Authorization=" + "";
                return await requestProvider.PostAsync<DeleteResponse>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DeleteImagesRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// LoginRestClient
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public async Task<LoginUserData> LoginRestClient(LoginData mail)
        {
            try
            {
                string loginresult = string.Empty;
                return await requestProvider.PostAsync<LoginData, LoginUserData>(WebServiceUrl + "Login/Login", mail);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoginRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// LoginRestClient
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public async Task<LoginUserData> LoginRestClientFromIIJ(LoginData mail)
        {
            try
            {
                string loginresult = string.Empty;
                return await requestProvider.PostSettingsAsyncwithModel<LoginData, LoginUserData>(WebServiceUrl + "Login/Login", mail);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoginRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Close Photo Upload
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="puid"></param>
        /// <returns></returns>
        public async Task<ClosePhotoResponse> ClosePhoto(int poid, int puid)
        {
            try
            {
                string url = WebServiceUrl + "/Upload/Photo/ClosePhoto?UserID=" + Settings.userLoginID + "&POID=" + poid + "&PUID=" + Helperclass.Encrypt(Convert.ToString(puid));// + "&Authorization=" + "" + "";
                return await requestProvider.PostAsync<ClosePhotoResponse>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ClosePhoto method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Close file upload.
        /// </summary>
        /// <param name="POID"></param>
        /// <param name="FUID"></param>
        /// <returns></returns>
        public async Task<CloseResponse> CloseFileRClient(int POID, int FUID)
        {
            try
            {
                string url = WebServiceUrl + "Upload/File/CloseFile?UserID=" + Settings.userLoginID + "&POID=" + POID + "&FUID=" + Helperclass.Encrypt(Convert.ToString(FUID));
                return await requestProvider.PostAsync<CloseResponse>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "CloseFileRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }

        }

        /// <summary>
        /// IIJ Update
        /// </summary>
        /// <param name="iijdata"></param>
        /// <returns></returns>
        public async Task<returnTokendetails> updateIIJ(SendTokendetails iijdata)
        {
            try
            {
                return await requestProvider.PostAsync<SendTokendetails, returnTokendetails>(WebServiceUrl + "Login/UpdateIIJToken", iijdata);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "updateIIJ method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Update Title
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public async Task<CLoseChat> updatetitle(string title, int qatype)
        {
            try
            {
                TitleUpdate tu = new TitleUpdate();
                tu.Title = title;
                tu.QAtype = qatype;
                tu.QAID = Settings.QaId;
                tu.CreatedBy = Settings.userLoginID;
                return await requestProvider.PostAsync<TitleUpdate, CLoseChat>(WebServiceUrl + "QA/UpdateTitle", tu);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "updatetitle method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// UpdateUser
        /// </summary>
        /// <param name="udata"></param>
        /// <returns></returns>
        public async Task<PLDeleteFileResponse> UpdateUser(UserUpdating udata, int type)
        {
            try
            {
                PLDeleteFileResponse titleupdateresult = null;

                if (type == 1)
                {
                    titleupdateresult = await requestProvider.PostAsync<UserUpdating, PLDeleteFileResponse>(WebServiceUrl + "QA/RemoveUser", udata);
                }
                else if (type == 2)
                {
                    titleupdateresult = await requestProvider.PostAsync<UserUpdating, PLDeleteFileResponse>(WebServiceUrl + "QA/AddUser", udata); ;
                }
                return titleupdateresult;
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdateUser method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GetTimeZoneRClient
        /// </summary>
        /// <returns></returns>
        public async Task<DDLMasterData> GetTimeZoneRClient()
        {
            try
            {
                return await requestProvider.PostAsync<DDLMasterData>(WebServiceUrl + "DDLMaster/TimeZones?UserID=" + Settings.userLoginID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetTimeZoneRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GetLanguagesRC
        /// </summary>
        /// <returns></returns>
        public async Task<LanguagesModel> GetLanguagesRC()
        {
            try
            {
                return await requestProvider.PostAsync<LanguagesModel>(WebServiceUrl + "DDLMaster/Languages?UserID=" + Settings.userLoginID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetLanguagesRC method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GetProfileRClinet
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetProfile> GetProfileRClinet(int userId)
        {
            try
            {
                return await requestProvider.PostAsync<GetProfile>(WebServiceUrl + "User/GetProfile?UserID=" + userId);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetProfileRClinet method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// UpdateProfileRClinet
        /// </summary>
        /// <param name="updateProfile"></param>
        /// <returns></returns>
        public async Task<updateProfileResponse> UpdateProfileRClinet(UpdateProfileData updateProfile)
        {
            try
            {
                return await requestProvider.PostAsync<UpdateProfileData, updateProfileResponse>(WebServiceUrl + "User/UpdateProfile", updateProfile);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdateProfileRClinet method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Byte Array for Print PDF File by using poTagID
        /// </summary>
        /// <param name="poTagID"></param>
        /// <returns></returns>
        public async Task<PrintPDFModel> PrintPDFRClient(string poTagID)
        {
            try
            {
                return await requestProvider.PostAsync<PrintPDFModel>(WebServiceUrl + "PO/PrintTag?UserID=" + Settings.userLoginID + "&potagIDs=" + poTagID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "PrintPDFRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Byte Array for Print PDF File by using POID
        /// </summary>
        /// <param name="poId"></param>
        /// <returns></returns>
        public async Task<PrintPDFModel> PrintPDFByUsingOPIDRClient(int poId)
        {
            try
            {
                return await requestProvider.PostAsync<PrintPDFModel>(WebServiceUrl + "PO/PrintShippingMark?UserID=" + Settings.userLoginID + "&POID=" + poId);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "PrintPDFByUsingOPIDRClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// RClientSearchLoacation
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>

        public async Task<SearchDataRoot> RClientSearchLoacation(string LocationText, int LocationID)
        {
            try
            {
                string url;

                if (!String.IsNullOrEmpty(LocationText))
                {
                    url = WebServiceUrl + "DDLMaster/Location/Search?UserID=" + Settings.userLoginID + "&location=" + LocationText;
                }
                else
                {
                    url = WebServiceUrl + "DDLMaster/Location/Search?UserID=" + Settings.userLoginID + "&LocationId=" + LocationID;
                }
                return await requestProvider.PostAsync<SearchDataRoot>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "RClientSearchLoacation method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Chat Mail
        /// </summary>
        /// <param name="mailobj"></param>
        /// <returns></returns>
        public async Task<CMessageResponse> SendChatMail(CMesssage mailobj)
        {
            try
            {
                return await requestProvider.PostAsync<CMessageResponse>(WebServiceUrl + "QA/Email?POID=" + Helperclass.Encrypt(Convert.ToString(mailobj.POID)) + "&QAID=" + Helperclass.Encrypt(Convert.ToString(mailobj.QAID)) + "&UserID=" + Settings.userLoginID + "&Message=" + mailobj.MessageBody + "&QAType=" + mailobj.QAType);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "SendChatMail method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// JW Bearer token
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public async Task<LoginUserData> Getjwtoken()
        {
            try
            {
                LoginData tk = new LoginData();
                tk.LoginID = Settings.LoginID;
                tk.SessionToken = Settings.Sessiontoken;
                return await requestProvider.PostAsync<LoginData, LoginUserData>(WebServiceUrl + "Login/GetJWToken", tk);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "Getjwtoken method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Read notification
        /// </summary>
        /// <param name="qaid"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<Readnotification> readNotifyHistory(int qaid, int userID)
        {
            try
            {
                string url = WebServiceUrl + "Notification/Read?QAID=" + qaid + "&UserID=" + userID;
                return await requestProvider.PostAsync<Readnotification>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "readNotifyHistory method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// clearNotifyHistory
        /// </summary>
        /// <returns></returns>
        public async Task<Readnotification> clearNotifyHistory()
        {
            try
            {
                string url = WebServiceUrl + "Notification/Clear?UserID=" + Settings.userLoginID;
                return await requestProvider.PostAsync<Readnotification>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "clearNotifyHistory method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// All Labesls
        /// </summary>
        /// <returns></returns>
        public async Task<AllLabels> alllabels()
        {
            try
            {
                return await requestProvider.PostAsync<AllLabels>(WebServiceUrl + "DDLMaster/AppLabels/ALL?UserID=" + Settings.userLoginID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "alllabels method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// All action status
        /// </summary>
        /// <returns></returns>
        public async Task<ActionsForUser> AllActionStatus(int userloginID)
        {
            try
            {
                return await requestProvider.PostAsync<ActionsForUser>(WebServiceUrl + "Login/GetActionByUserId?UserID=" + userloginID.ToString());
                //return await requestProvider.PostAsync<ActionsForUser>(WebServiceUrl + "Login/GetActionByUserId?UserID=" + Settings.userLoginID.ToString());
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "AllActionStatus method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Mobile Builds
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<MobileBFinalData> GetMobileBuildsRClinet(GetMobileBData mobileBModel)
        {
            try
            {
                return await requestProvider.PostAsync<GetMobileBData, MobileBFinalData>(WebServiceUrl + "Upload/MobileBuild/SearchMobileBuilds", mobileBModel);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetMobileBuildsRClinet method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }


        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        public async Task<returnTokendetails> Logout()
        {
            try
            {
                LoginData lgdata = new LoginData();
                lgdata.LoginID = Settings.LoginID;
                lgdata.UserID = EncryptManager.Encrypt(Convert.ToString(Settings.userLoginID));
                lgdata.SessionToken = Settings.Sessiontoken;
                return await requestProvider.PostAsync<LoginData, returnTokendetails>(WebServiceUrl + "Login/Logout", lgdata, true);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "Logout method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        public async Task<ApplicationSettings> GetAllSettings()
        {
            try
            {
                string url = WebServiceUrl + "Login/GlobalSettings?SecurityCode=" + Settings.SecurityCode;
                return await requestProvider.PostSettings<ApplicationSettings>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetAllSettings method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }


        /// <summary>
        /// GetUploadedLoadPhotosRestClient
        /// </summary>
        /// <param name="potagid"></param>
        /// <returns></returns>
        public async Task<LoadPhotosListResponse> GetUploadedLoadPhotosRestClient(int taskid)
        {
            try
            {
                string url = WebServiceUrl + "Upload/LoadPhoto/Photos?TaskID=" + Helperclass.Encrypt(Convert.ToString(taskid));
                return await requestProvider.PostAsync<LoadPhotosListResponse>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetUploadedLoadPhotosRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Uploadfiles
        /// </summary>
        /// <param name="fileobj"></param>
        /// <returns></returns>
        public async Task<LoadPhotosListResponse> LoadPhotoUpload(List<LoadPhotoModel> photodata)
        {
            try
            {
                return await requestProvider.PostAsync<List<LoadPhotoModel>, LoadPhotosListResponse>(WebServiceUrl + "Upload/LoadPhoto/Upload", photodata);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoadPhotoUpload method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// delete load images
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<DeleteResponse> DeleteLoadImagesRestClient(int id)
        {
            try
            {
                string url = WebServiceUrl + "Upload/LoadPhoto/DeletePhoto?UserID=" + Settings.userLoginID + "&ID=" + Helperclass.Encrypt(Convert.ToString(id));// + "&Authorization=" + "";
                return await requestProvider.PostAsync<DeleteResponse>(url);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DeleteLoadImagesRestClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Insert or update Inspection result
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<UpdateInspectionResponse> InsertUpdateInspectionResult(UpdateInspectionRequest updateInspectionRequest)
        {
            try
            {
                return await requestProvider.PostAsync<UpdateInspectionRequest, UpdateInspectionResponse>(WebServiceUrl + "Inspection/InsertUpdateInspectionResult", updateInspectionRequest);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "InsertUpdateInspectionResult method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// DeletInspection result
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<DeletePhotoResponce> DeleteInspectionPhoto(int QID)
        {
            try
            {
                return await requestProvider.PostAsync<DeletePhotoResponce>(WebServiceUrl + "Inspection/DeleteInspectionPhoto?UserID=" + Settings.userLoginID + "&ID=" + Helperclass.Encrypt(Convert.ToString(QID)));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "InsertUpdateInspectionResult method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GeInspectionResultsClient
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<InspectionResults> GetInspectionResultsClient(int taskid, int tagId)
        {
            try
            {
                return await requestProvider.PostAsync<InspectionResults>(WebServiceUrl +
                    "Inspection/GetInspectionResultsByTag?TaskID=" + Helperclass.Encrypt(Convert.ToString(taskid))
                    + "&POTagID=" + Helperclass.Encrypt(Convert.ToString(tagId)));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GeInspectionResultsClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Ge Inspection Results for task
        /// </summary>
        /// <param name="taskid"></param>
        /// <returns></returns>
        public async Task<InspectionResults> GetInspectionResultsByTask(int taskid)
        {
            try
            {
                return await requestProvider.PostAsync<InspectionResults>(WebServiceUrl +
                    "Inspection/GetInspectionResultsByTask?TaskID=" + Helperclass.Encrypt(Convert.ToString(taskid)));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GeInspectionResultsByTask method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GeInspectionPhotosClient
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<InspectionPhotosResponse> GeInspectionPhotosByTag(int taskid, int tagId, int? questionId)
        {
            try
            {
                return await requestProvider.PostAsync<InspectionPhotosResponse>(WebServiceUrl
                    + "Inspection/GetInspectionPhotosByTag?TaskID=" + Helperclass.Encrypt(Convert.ToString(taskid))
                    + "&POTagID=" + Helperclass.Encrypt(Convert.ToString(tagId))
                    + "&QID=" + questionId);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GeInspectionResultsClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// GeInspectionPhotosClient
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<InspectionPhotosResponse> GeInspectionPhotosByTask(int tagId, int? questionId)
        {
            try
            {
                return await requestProvider.PostAsync<InspectionPhotosResponse>(WebServiceUrl
                    + "Inspection/GetInspectionPhotosByTask?TaskID=" + Helperclass.Encrypt(Convert.ToString(tagId))
                    + "&QID=" + questionId);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GeInspectionResultsClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// InsertInspectionPhotosClient
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public async Task<UpdateInsertInspectionResponse> InsertInspectionPhotosClient(List<UpdateInspectionRequest> updateInspectionRequest)
        {
            try
            {
                return await requestProvider.PostAsync<List<UpdateInspectionRequest>, UpdateInsertInspectionResponse>(WebServiceUrl + "Inspection/InsertInspectionPhotos", updateInspectionRequest);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GeInspectionResultsClient method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get All Inspection Configurations
        /// </summary>
        /// <returns></returns>
        public async Task<InspectionConfigurationsResults> GetAllMInspectionConfigurations()
        {
            try
            {
                return await requestProvider.PostAsync<InspectionConfigurationsResults>(WebServiceUrl + "Inspection/GetAllMInspectionConfigurations?UserID=" + Settings.userLoginID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetAllMInspectionConfigurations method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Scan Config
        /// </summary>
        /// <returns></returns>
        public async Task<SaveScanConfigResponse> SaveScanConfig(int companyid, int projectid, int jobid, int compareruleid, int scancount,
            int polyboxruleid, string polyboxlocname, int polyboxremarkid, int polyboxstatusid)
        {
            try
            {
                return await requestProvider.PostAsync<SaveScanConfigResponse>(WebServiceUrl +
                    "User/UpdateScanConfiguration?UserID=" + Settings.userLoginID
                     + "&CompanyID=" + companyid
                    + "&ProjectID=" + projectid
                    + "&JobID=" + jobid
                    + "&ScanConfigID=" + compareruleid
                    + "&ScanCount=" + scancount
                    + "&PolyboxRule=" + polyboxruleid
                    + "&PolyboxLocation=" + polyboxlocname
                    + "&PolyboxRemarks=" + polyboxremarkid
                    + "&PolyboxStatus=" + polyboxstatusid);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetScanConfig method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }
        
        public async Task<SaveScanConfigResponse> UpdatePrintField(string PageType,string PrintFields)
        {
            try
            {
                return await requestProvider.PostAsync<SaveScanConfigResponse>(WebServiceUrl +
                    "User/UpdatePrintFieldsByUser?UserID=" + Settings.userLoginID
                     + "&CompanyID=" + Settings.CompanyID
                    + "&ProjectID=" + Settings.ProjectID
                    + "&JobID=" + Settings.JobID
                    + "&PageType=" + PageType
                    + "&PrintFields=" + PrintFields);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdatePrintField method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get saved compare config.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<GetSavedConfigResponse> GetSaveScanConfig(int companyid, int projectid, int jobid)
        {
            try
            {
                return await requestProvider.PostAsync<GetSavedConfigResponse>(WebServiceUrl +
                    "User/GetScanConfiguration?UserID=" +
                    Helperclass.Encrypt(Settings.userLoginID.ToString())
                     + "&CompanyID=" + companyid
                    + "&ProjectID=" + projectid
                    + "&JobID=" + jobid);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetProfileRClinet method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }


        /// <summary>
        /// Get Scan Config
        /// </summary>
        /// <returns></returns>
        public async Task<ScanConfigResponse> GetScanConfig()
        {
            try
            {
                return await requestProvider.PostAsync<ScanConfigResponse>(WebServiceUrl + "Login/ScanConfig?" +
                    "UserID=" + Settings.userLoginID
                     + "&CompanyID=" + Settings.CompanyID
                    + "&ProjectID=" + Settings.ProjectID
                    + "&JobID=" + Settings.JobID
                    + "&PageType=" + "PolyboxPrint");
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetScanConfig method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Scan Config
        /// </summary>
        /// <returns></returns>
        public async Task<GetRepoPhotoResponse> GetRepoPhotos()
        {
            try
            {
                return await requestProvider.PostAsync<GetRepoPhotoResponse>(WebServiceUrl +
                    "Upload/FileRepository/Files?UserID=" + Helperclass.Encrypt(Settings.userLoginID.ToString()));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetRepoPhotos method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Uploadfiles
        /// </summary>
        /// <param name="fileobj"></param>
        /// <returns></returns>
        public async Task<GetRepoPhotoResponse> UploadRepoPhotos(List<PhotoRepoDBModel> photodata)
        {
            try
            {
                return await requestProvider.PostAsync<List<PhotoRepoDBModel>, GetRepoPhotoResponse>(WebServiceUrl +
                    "Upload/FileRepository/Upload", photodata);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoadPhotoUpload method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Delete Single Repo Photo
        /// </summary>
        /// <param name="fileobj"></param>
        /// <returns></returns>
        public async Task<GetRepoPhotoDelResponse> DeleteSingleRepoPhoto(int photoid)
        {
            try
            {
                return await requestProvider.PostAsync<GetRepoPhotoDelResponse>(WebServiceUrl +
                    "Upload/FileRepository/DeleteFile?UserID=" + Settings.userLoginID
                    + "&ID=" + Helperclass.Encrypt(photoid.ToString()));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoadPhotoUpload method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Insert Insp Signatures
        /// </summary>
        /// <param name="fileobj"></param>
        /// <returns></returns>
        public async Task<InspectionResultsRespnse> InsertUpdateSignature(InspectionResultsList inspobj)
        {
            try
            {
                return await requestProvider.PostAsync<InspectionResultsList, InspectionResultsRespnse>(WebServiceUrl +
                    "Inspection/InsertUpdateInspectionSignature", inspobj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoadPhotoUpload method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Insp Signature By Tag
        /// </summary>
        /// <param name="fileobj"></param>
        /// <returns></returns>
        public async Task<InspectionResults> GetInspSignatureByTag(int taskid, int potagid)
        {
            try
            {
                return await requestProvider.PostAsync<InspectionResults>(WebServiceUrl +
                    "Inspection/GetInspectionSignatureByTag?TaskID=" + Helperclass.Encrypt(taskid.ToString())
                    + "&POTagID=" + Helperclass.Encrypt(potagid.ToString()));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoadPhotoUpload method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Insp Signature By Task
        /// </summary>
        /// <param name="fileobj"></param>
        /// <returns></returns>
        public async Task<InspectionResults> GetInspSignatureByTask(int taskid)
        {
            try
            {
                return await requestProvider.PostAsync<InspectionResults>(WebServiceUrl +
                    "Inspection/GetInspectionSignatureByTask?TaskID=" + Helperclass.Encrypt(taskid.ToString()));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoadPhotoUpload method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Delete ALL Repo Photo
        /// </summary>
        /// <param name="fileobj"></param>
        /// <returns></returns>
        public async Task<GetRepoPhotoDelAllResponse> DeleteAllRepoPhoto()
        {
            try
            {
                return await requestProvider.PostAsync<GetRepoPhotoDelAllResponse>(WebServiceUrl +
                    "Upload/FileRepository/DeleteAllFiles?UserID=" + Helperclass.Encrypt(Settings.userLoginID.ToString()));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoadPhotoUpload method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Saved User Search Settings
        /// </summary>
        /// <returns></returns>
        public async Task<GetSearchFilterListResponse> GetSavedUserSearchSettings()
        {
            try
            {
                return await requestProvider.PostAsync<GetSearchFilterListResponse>(WebServiceUrl +
                    "PO/GetSavedUserSearchSettings?UserID=" + Settings.userLoginID
                    + "&CompanyID=" + Settings.CompanyID
                    + "&ProjectID=" + Settings.ProjectID
                    + "&JobID=" + Settings.JobID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetSavedUserSearchSetting method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Saved User Search Settings
        /// </summary>
        /// <returns></returns>
        public async Task<SearchSetting> GetSavedUserSearchSettingsByID(SearchPassData obj)
        {
            try
            {
                return await requestProvider.PostAsync<SearchSetting>(WebServiceUrl +
                    "PO/GetUserSearchSettingByID?UserID=" + Settings.userLoginID
                    + "&ID=" + Helperclass.Encrypt(Convert.ToString(obj.ID)));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetSavedUserSearchSettingsByID method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Delete User Search Filter
        /// </summary>
        /// <returns></returns>
        public async Task<SearchDataSimpleResponse> DeleteUserSearchFilter(SearchPassData obj)
        {
            try
            {
                return await requestProvider.PostAsync<SearchDataSimpleResponse>(WebServiceUrl +
                    "PO/DeleteUserSearchFilter?UserID=" + Settings.userLoginID
                    + "&ID=" + Helperclass.Encrypt(Convert.ToString(obj.ID)));
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DeleteUserSearchFilter method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        public async Task<TagTaskStatusUpdateResponse> AssignUnassignedTask(int taskid)
        {
            try
            {
                return await requestProvider.PostAsync<TagTaskStatusUpdateResponse>(WebServiceUrl + "Task/AssignJob?TaskID=" + Helperclass.Encrypt(taskid.ToString()) +
                    "&UserID=" + Settings.userLoginID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdateTaskStatus method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get polybox header details
        /// </summary>
        /// <returns></returns>
        public async Task<PolyboxValidateResponse> GetPolyboxHeaderDetails()
        {
            try
            {
                return await requestProvider.PostAsync<PolyboxValidateResponse>(WebServiceUrl + "Polybox/GetPolyboxScanDetails?UserID=" + Settings.userLoginID
                    + "&CompanyID=" + Settings.PBCompanyID
                    + "&ProjectID=" + Settings.PBProjectID
                    + "&JobID=" + Settings.PBJobID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetPolyboxHeaderDetails method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Polybox scan validation
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<PolyboxValidateResponse> PolyboxScanValidation(string tagnumber)
        {
            try
            {
                return await requestProvider.PostAsync<PolyboxValidateResponse>(WebServiceUrl +
                    "Polybox/ValidatePolyboxScanDetails?CompanyID=" + Settings.PBCompanyID
                    + "&ProjectID=" + Settings.PBProjectID
                    + "&JobID=" + Settings.PBJobID
                    + "&UserID=" + Settings.userLoginID
                    + "&TagNumber=" + tagnumber);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "PolyboxScanValidation method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Save Polybox Scan Data
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<PolyboxSaveResponse> SavePolyboxScanData(PolyBoxModel polyboxscanobj)
        {
            try
            {
                return await requestProvider.PostAsync<PolyBoxModel, PolyboxSaveResponse>
                    (WebServiceUrl +
                    "Polybox/SavePolyboxScanDetails", polyboxscanobj);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "SavePolyboxScanData method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Byte Array for Polybox PDF File by using poTagID
        /// </summary>
        /// <param name="tagnumber"></param>
        /// <returns></returns>
        public async Task<PrintPDFModel> PrintPolyboxPDF(string tagnumber)
        {
            try
            {
                return await requestProvider.PostAsync<PrintPDFModel>(WebServiceUrl
                    + "Polybox/PrintPolybox?UserID=" + Settings.userLoginID
                    + "&TagNumber=" + tagnumber
                    + "&CompanyID=" + Settings.PBCompanyID
                    + "&ProjectID=" + Settings.PBProjectID
                    + "&JobID=" + Settings.PBJobID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "PrintPolyboxPDF method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Update EventID
        /// </summary>
        /// <param name="poTagID"></param>
        /// <returns></returns>
        public async Task<PrintPDFModel> UpdateDefaultSettingByEventID()
        {
            try
            {
                return await requestProvider.PostAsync<PrintPDFModel>(WebServiceUrl
                    + "PO/UpdateDefaultSettingByEventID?UserID=" + Settings.userLoginID
                    + "&EventID=" + Settings.EventID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdateDefaultSettingByEventID method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Notification count
        /// </summary>
        /// <param name="poTagID"></param>
        /// <returns></returns>
        public async Task<NotificationcountResponse> GetCount()
        {
            try
            {
                return await requestProvider.PostAsync<NotificationcountResponse>(WebServiceUrl
                    + "Notification/NotificationCount?UserID=" + Settings.userLoginID);
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdateDefaultSettingByEventID method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }
        #endregion

        #region Without common API calling methods.

        /// <summary>
        /// Handleexception
        /// </summary>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public async Task<object> Handleexception(Exception ErrorMessage)
        {
            try
            {
                string errorMessage = string.Empty;
                var json = JsonConvert.SerializeObject(ErrorMessage, Formatting.None, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Error = (ser, er) => er.ErrorContext.Handled = true
                });
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = null;
                bool checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    response = await httpClient.PostAsync(WebServiceUrl + "Login/MobileErrorLog?UserID=" + Settings.userLoginID, httpContent);
                }
                return errorMessage;
            }
            catch (Exception ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    YPSLogger.ReportException(ex, "Handleexception method because of SSL public key mismatch-> in RequestProvider.cs" + Settings.userLoginID);
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                    return null;
                }
                #endregion
                else
                {
                    YPSLogger.ReportException(ex, "Handleexception method -> in RestClient.cs" + Settings.userLoginID);
                    await service.Handleexception(ex);
                    return null;
                }
            }
        }

        /// <summary>
        /// UpdateChatUsers / We are now not calling this api any where.currently using 'UpdateUser' method
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<string> UpdateChatUsers(ChatData obj)
        {
            try
            {
                string UpdateChatResult = string.Empty;
                var json = JsonConvert.SerializeObject(obj);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = null;

                response = await httpClient.PostAsync(WebServiceUrl + "QA/UpdateUsers", httpContent); //need to change api,i have done for only main page datagrid

                var chatResult = await response.Content.ReadAsStringAsync();
                UpdateChatResult = chatResult;
                return UpdateChatResult;

            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UpdateChatUsers method -> in RestClient.cs" + Settings.userLoginID);
                return string.Empty;
            }
        }

        /// <summary>
        /// SendMessage  we are not using this anywhere
        /// </summary>
        /// <param name="mesobj"></param>
        /// <returns></returns>
        public async Task<string> SaveMessageRestClinet(CMesssage mesobj)
        {
            try
            {
                string sendmessageresult = string.Empty;
                var json = JsonConvert.SerializeObject(mesobj);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = null;

                if (Settings.QaId > 0)
                {
                    response = await httpClient.PostAsync(WebServiceUrl + "QA/SaveMessage", httpContent);
                }
                else if (Settings.QaId == 0 || Settings.QaId == -1)
                {
                    response = await httpClient.PostAsync(WebServiceUrl + "QA/SaveWhiteBoardMessage", httpContent);
                }


                var messageResult = await response.Content.ReadAsStringAsync();
                sendmessageresult = messageResult;
                return sendmessageresult;
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "sendmessage method -> in RestClient.cs" + Settings.userLoginID);
                return string.Empty;
            }
        }

        /// <summary>
        /// CheckDevice
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<object> CheckDevice(LoginModel obj)
        {
            try
            {
                string Result = string.Empty;
                var json = JsonConvert.SerializeObject(obj);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = null;
                response = await httpClient.PostAsync(WebServiceUrl + "Notification/CheckDevice", httpContent);
                var messageResult = await response.Content.ReadAsStringAsync();
                Result = messageResult;
                return Result;
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "CheckDevice method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// RegisterNotification
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public async Task<string> RegisterNotification(DeviceRegistration dr)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dr);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await httpClient.PostAsync(WebServiceUrl + "Notification/PutRegistration", httpContent);
                var data = await result.Content.ReadAsStringAsync();
                return data;
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "RegisterNotification method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// DeleteRegistrationId
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public async Task<object> DeleteRegistrationId(DeviceRegistration dr)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dr);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await httpClient.PostAsync(WebServiceUrl + "Notification/DeleteNotification", httpContent);
                var data = await result.Content.ReadAsStringAsync();
                var taskModel = JsonConvert.DeserializeObject<DeviceRegistration>(data.ToString());
                return taskModel;
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DeleteRegistrationId method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// SaveNotification
        /// </summary>
        /// <param name="ns"></param>
        /// <returns></returns>
        public async Task<object> SaveNotification(NotificationSettings ns)
        {
            try
            {
                var json = JsonConvert.SerializeObject(ns);
                HttpContent httpContent = new StringContent(json);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await httpClient.PostAsync(WebServiceUrl + "Notification/SaveNotification", httpContent);
                var data = await result.Content.ReadAsStringAsync();
                var taskModel = JsonConvert.DeserializeObject<NotificationSettings>(data.ToString());
                return taskModel;
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "SaveNotification method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// Get Filter YShip Data, we are not using anywhere this API calling.
        /// </summary>
        /// <returns></returns>
        public async Task<string> FilterYshipDataRClient()
        {
            try
            {
                string YshipResult = string.Empty;
                string url = WebServiceUrl + "DDLMaster/LoadType/ALL";
                var response = httpClient.PostAsync(url, null).Result;
                var taskModels1 = await response.Content.ReadAsStringAsync();
                YshipResult = taskModels1;
                return taskModels1;
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "RCFilterYshipData method -> in RestClient.cs" + Settings.userLoginID);
                return null;
            }
        }
        #endregion
    }
}
