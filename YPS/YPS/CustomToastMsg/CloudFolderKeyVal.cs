using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.Views;

namespace YPS.CustomToastMsg
{
    public static class CloudFolderKeyVal
    {
        public static string GetBlobFolderName(int key)
        {
            try
            {
                switch (key)
                {
                    case (int)BlobContainer.cnttagfiles:
                        return "tag-files";
                    case (int)BlobContainer.cntchatfiles:
                        return "chat-files";
                    case (int)BlobContainer.cntplfiles:
                        return "pl-files";
                    case (int)BlobContainer.cntyshipuploads:
                        return "yship-uploads";
                    default:
                        return "";
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetBlobFolderName method -> in CloudFolderKeyVal.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
                return "";
            }

        }

        public async static Task<string> GetToken()
        {
            try
            {
                LoginUserData result = null;
                YPSService service = new YPSService();
                //Settings.LoginID = await SecureStorage.GetAsync("LoginID"); //CrossSecureStorage.Current.GetValue("userName");
                result = await service.getjwtoken();
                if (result != null)
                {
                    //result = JsonConvert.DeserializeObject<LoginUserData>(tokendata);
                    if (result != null)
                    {
                        if (result.status == 1)
                        {
                            await Assignvaluestosettings(result);
                            // await SecureStorage.SetAsync("userID", result.data.UserID.ToString());
                            // await SecureStorage.SetAsync("userName", result.data.Email);
                            // await SecureStorage.SetAsync("LoginID", result.data.LoginID);

                            #region IIJ redirect old


                            //if (result.data.IsIIJEnabled && result.data.IsIIJTokenExpired)
                            //{
                            //    if (OAuthConfig.User == null)
                            //    {
                            //        try
                            //        {
                            //            //App.Current.MainPage = new RootPage(typeof(ProviderLoginPage));
                            //            await App.Current.MainPage.DisplayAlert("Alert", "Your yID token expired, please login", "Ok");
                            //            App.Current.MainPage = new YPSMasterPage(typeof(LoginPage));
                            //            Settings.Username = Settings.UserMail =
                            //            Settings.SGivenName = Settings.EntityName = Settings.RoleName = Settings.access_token = Settings.JobSelected =
                            //            Settings.BlobConnection = Settings.BlobStorageConnectionString =
                            //            Settings.CompanySelected = Settings.SupplierSelected = Settings.LoginID= Settings.Projectelected = string.Empty;
                            //            Settings.SupplierID = Settings.ProjectID = Settings.CompanyID = Settings.JobID =
                            //            Settings.CompressionQuality = Settings.PhotoSize = Settings.userLoginID = Settings.userRoleID = 0;
                            //            RememberPwdDB Db = new RememberPwdDB();
                            //            var user = Db.GetUserDetails().FirstOrDefault();
                            //            if (user != null)
                            //            {
                            //                Db.ClearUserDetail(user.UserId);
                            //            }
                            //            DependencyService.Get<ISQLite>().deleteAllPNdata();
                            //            SecureStorage.Remove("userName");
                            //            SecureStorage.Remove("LoginID");
                            //            SecureStorage.Remove("userID");
                            //            if (Settings.alllabeslvalues != null)
                            //            {
                            //                Settings.alllabeslvalues.Clear();
                            //            }
                            //            return "";
                            //        }catch(Exception ex)
                            //        {
                            //            return "";
                            //        }

                            //    }
                            //}
                            #endregion
                        }
                    }
                    //    }
                    //    else
                    //    {
                    //        // Appredirectlogin("Your yID token expired, please login");
                    //    }
                    //}
                    //else
                    //{
                    //    Appredirectlogin("Your session token expired, please login");
                    //}
                }
                return "";

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetToken method -> in CloudFolderKeyVal.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
                return "";

            }
        }

        public async static Task Assignvaluestosettings(LoginUserData result)
        {
            try
            {

                Settings.userRoleID = result.data.RoleID;
                Settings.userLoginID = result.data.UserID;
                // CrossSecureStorage.Current.SetValue("userID", result.data.UserID.ToString());
                Settings.Username = result.data.FullName;
                Settings.UserMail = result.data.Email;
                Settings.SGivenName = result.data.GivenName;
                Settings.LoginIDDisplay = result.data.LoginID;
                Settings.EntityName = result.data.EntityName;
                Settings.RoleName = result.data.RoleName;
                Settings.access_token = result.data.JwToken.Token;
                Settings.Sessiontoken = result.data.SessionToken;
                Settings.BlobConnection = result.data.JwToken.GlobalSettings.BlobConnection;
                Settings.PhotoSize = result.data.JwToken.GlobalSettings.PhotoSize;
                Settings.CompressionQuality = result.data.JwToken.GlobalSettings.CompressionQuality;
                Settings.BlobStorageConnectionString = EncryptManager.Decrypt(Settings.BlobConnection);
                Settings.VersionID = result.data.VersionID;
                Settings.LanguageID = result.data.LanguageID;
                Settings.LanguageName = result.data.LanguageName;
                Settings.DateFormatformAPI = result.data.JwToken.GlobalSettings.DateFormat;
                Settings.TimeFormatforAPI = result.data.JwToken.GlobalSettings.DateTimeFormat;
                Settings.LoginID = EncryptManager.Encrypt(result.data.LoginID);
                Settings.IIJConsumerKey = EncryptManager.Decrypt(result.data.JwToken.GlobalSettings.IIJConsumerKey);
                Settings.IIJConsumerSecret = EncryptManager.Decrypt(result.data.JwToken.GlobalSettings.IIJConsumerSecret);
                Settings.AndroidVersion = result.data.JwToken.GlobalSettings.AndroidVersion;
                Settings.iOSversion = result.data.JwToken.GlobalSettings.iOSversion;
                Settings.EntityTypeName = result.data.EntityTypeName;

                if (!string.IsNullOrEmpty(result.data.RoleColorCode) || !string.IsNullOrEmpty(result.data.VersionColorCode))
                {
                    Settings.Bar_Background = Color.FromHex(!string.IsNullOrEmpty(result.data.RoleColorCode) ? result.data.RoleColorCode : result.data.VersionColorCode);
                    Settings.RoleColorCode = result.data.RoleColorCode;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Assignvaluestosettings method -> in CloudFolderKeyVal.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        public async static Task CheckAndRegisterDeviceForPN()
        {
            YPSService service = new YPSService();

            try
            {
                Task.Run(async () => await CloudFolderKeyVal.GetToken()).Wait();

                if (Device.RuntimePlatform == Device.Android)
                {
                    Settings.FireBasedToken = await SecureStorage.GetAsync("Token");
                }
                else
                {
                    Settings.FireBasedToken = await SecureStorage.GetAsync("iOSFireBaseToken");
                }

                if (Settings.FireBasedToken != null)
                {
                    LoginModel model = new LoginModel();

                    model.UserId = Settings.userLoginID;
                    model.FireBasedToken = Settings.FireBasedToken;
                    model.DeviceModel = DeviceInfo.Model;
                    model.DeviceUUID = DeviceInfo.Idiom.ToString();
                    model.DevicePlatform = DeviceInfo.Platform.ToString();
                    model.DeviceVersion = DeviceInfo.Version.ToString();

                    var Checkdevicedata = await service.CheckDevice(model); //check device
                    var checkDevice = JsonConvert.DeserializeObject<PushNotifyModel>(Checkdevicedata.ToString());

                    if (checkDevice.status == 0)
                    {
                        DeviceRegistration dr = new DeviceRegistration();

                        dr.UserId = Settings.userLoginID;
                        dr.FireBasedToken = Settings.FireBasedToken;
                        dr.Platform = (DeviceInfo.Platform.ToString() == "Android") ? "gcm" : "apns";
                        dr.HubRegistrationId = "";

                        //Id we are geeting by using Xamarin.Forms.DeviceInfo so that ID taken line of code commented.
                        string[] arr = new string[] { "Hai" };
                        dr.Tags = arr;

                        bool NetConnectionState = await App.CheckInterNetConnection();

                        if (NetConnectionState)
                        {
                            var aa = await service.RegisterNotification(dr); //put register
                            var registered = JsonConvert.DeserializeObject<PushNotifyModel>(aa);

                            if (registered.status == 1)
                            {
                                Settings.HubRegisterid = registered.message; //getting registration id 

                                NotificationSettings notificationsetting1 = new NotificationSettings();

                                notificationsetting1.FireBasedToken = Settings.FireBasedToken.Trim();
                                notificationsetting1.is_login_active = true;
                                notificationsetting1.CreatedUTCDateTime = DateTime.Now;
                                notificationsetting1.ModifiedUTCDateTime = DateTime.Now;
                                //GeneratedAppId we are geeting by using Xamarin.Forms.DeviceInfo so that GenerateAppId take line of code commented.
                                notificationsetting1.ModelName = DeviceInfo.Model;
                                notificationsetting1.Platform = (DeviceInfo.Platform.ToString() == "Android") ? "gcm" : "apns";
                                notificationsetting1.Version = DeviceInfo.VersionString;
                                notificationsetting1.HubRegistrationId = Settings.HubRegisterid;
                                notificationsetting1.IsNotificationRequired = true;
                                notificationsetting1.UserID = Settings.userLoginID;
                                notificationsetting1.Appversion = Settings.AppVersion;
                                notificationsetting1.LoginKey = 1;
                                notificationsetting1.ModelID = "";

                                var datasaveDeviceIds = await service.SaveNotification(notificationsetting1);
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "Please check your internet connection", "Ok");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckAndRegisterDeviceForPN method -> in CloudFolderKeyVal.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        public async static Task Appredirectlogin(string message, bool showalert = true)
        {
            try
            {
                YPSService service = new YPSService();
                Settings.Username = Settings.UserMail =
                Settings.SGivenName = Settings.EntityName = Settings.RoleName = Settings.access_token = Settings.JobSelected =
                Settings.BlobConnection = Settings.BlobStorageConnectionString =
                Settings.CompanySelected = Settings.ProjectSelected = Settings.IIJConsumerKey =
                Settings.IIJConsumerSecret = string.Empty;
                Settings.ProjectID = Settings.CompanyID = Settings.JobID =
                Settings.CompressionQuality = Settings.PhotoSize = Settings.userRoleID = 0;
                Settings.IsSearchClicked = Settings.IsIIJEnabled = false;
                RememberPwdDB Db = new RememberPwdDB();
                var user = Db.GetUserDetails().FirstOrDefault();

                if (user != null)
                {
                    Db.ClearUserDetail(user.UserId);
                }

                DependencyService.Get<ISQLite>().deleteAllPNdata();
                // SecureStorage.Remove("userName");
                // SecureStorage.Remove("LoginID");
                // SecureStorage.Remove("userID");

                if (Settings.alllabeslvalues != null)
                {
                    Settings.alllabeslvalues.Clear();
                }
                //if (showalert)
                //   await App.Current.MainPage.DisplayAlert("Alert", message, "Ok");
                Task.Run(async () => await service.LogoutService()).Wait();
                Settings.userLoginID = 0;
                Settings.LoginID = Settings.Sessiontoken = string.Empty;

                var globalResult = Task.Run(async () => await YPSService.GetglobelSettings()).Result;
                await App.GlobelSettings(globalResult);

                if (!Settings.IsIIJEnabled)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        App.Current.MainPage = new NavigationPage(new LoginPage());
                    });
                }
                else
                {
                    if (OAuthConfig.User == null)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage = new NavigationPage(new ProviderLoginPage());
                        }
                        );
                    }
                }
                //var Logoutresult = await service.LogoutService();
                return;

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Appredirectlogin method -> in CloudFolderKeyVal.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
        }

        public async static void Appredirectloginwithoutlogout(bool isMITM = false)
        {
            try
            {
                YPSService service = new YPSService();
                Settings.Username = Settings.UserMail =
                Settings.SGivenName = Settings.EntityName = Settings.RoleName = Settings.access_token = Settings.JobSelected =
                Settings.BlobConnection = Settings.BlobStorageConnectionString =
                Settings.CompanySelected = Settings.ProjectSelected = Settings.IIJConsumerKey =
                Settings.IIJConsumerSecret = string.Empty;
                Settings.ProjectID = Settings.CompanyID = Settings.JobID =
                Settings.CompressionQuality = Settings.PhotoSize = Settings.userRoleID = 0;
                Settings.IsEmailEnabled = true;
                RememberPwdDB Db = new RememberPwdDB();
                var user = Db.GetUserDetails().FirstOrDefault();

                if (user != null)
                {
                    Db.ClearUserDetail(user.UserId);
                }
                DependencyService.Get<ISQLite>().deleteAllPNdata();
                // SecureStorage.Remove("userName");
                // SecureStorage.Remove("LoginID");
                // SecureStorage.Remove("userID");
                if (Settings.alllabeslvalues != null)
                {
                    Settings.alllabeslvalues.Clear();
                }

                if (isMITM)
                {
                    await App.Current.MainPage.DisplayAlert("Man in the middle detected", "potential security risk ahead", "Close");
                    // await Task.Delay(3000);
                }
                System.Diagnostics.Process.GetCurrentProcess().Kill();

                // App.Current.MainPage = new YPSMasterPage(typeof(LoginPage));

                //Task.Run(async () => await service.LogoutService()).Wait();
                Settings.userLoginID = 0;
                Settings.LoginID = Settings.Sessiontoken = string.Empty;
                //var Logoutresult = await service.LogoutService();
                return;

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Appredirectloginwithoutlogout method -> in CloudFolderKeyVal.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
        }
    }

    public static class HostingURL
    {
        /// We have to change appcenter key before publish.

        #region Local devpit
        public static string WebServiceUrl { get; set; } = "https://ypsdev.devtpit.com/api/";
        public static string HubConnectionUrl { get; set; } = "https://ypsdev.devtpit.com/";
        public static string Appcenter_droid { get; set; } = "android=3068e436-13c0-4cac-873f-687d8c0830c3;";
        public static string Appcenter_iOS { get; set; } = "ios=98470379-fccd-4f9d-8d22-8856a5af15c9;";
        public static string Bdchk { get; set; } = "BjFAg4x7vCGTpW3dALvutCVBSpuI7d6rb+GuOgl/574=";
        public static string blob { get; set; } = "https://ypsuploadsdev.blob.core.windows.net/";
        public static string scandItLicencekey = "";
        #endregion

        #region DEV:
        //public static string HubConnectionUrl { get; set; } = "https://dev.parts2y.com/";
        //public static string WebServiceUrl { get; set; } = "https://dev.parts2y.com/api/";
        //public static string Appcenter_droid { get; set; } = "android=ebfaf0cf-af6f-4f28-a6c9-642352279430;";
        //public static string Appcenter_iOS { get; set; } = "ios=37be6471-dc83-405d-b62f-e796ae44267d;";
        ////public static string Bdchk { get; set; } = "DprjHRPut1l2lsH4K5tRcw==";///old key 
        //public static string Bdchk { get; set; } = "BzrCZM9KiA31bPvjUJJNmA==";
        //public static string blob { get; set; } = "https://azrbsa026dv00a.blob.core.windows.net/";
        //public static string scandItLicencekey = "";
        #endregion

        #region LIVE:
        //public static string HubConnectionUrl { get; set; } = "https://www.parts2y.com/";
        //public static string WebServiceUrl { get; set; } = "https://www.parts2y.com/api/";
        //public static string Appcenter_droid { get; set; } = "android=b3a4d5ad-e6bb-4e6e-a4b3-4b6dbdbd9ee4;";
        //public static string Appcenter_iOS { get; set; } = "ios=6aeae3ae-1423-41c0-b9ec-0de97e33789d;";
        //public static string Bdchk { get; set; } = "TXXv/K4nwyXFxNfZDiFY4pTAH4PhtGwuuMq5br5uqJw=";
        /////public static string Bdchk { get; set; } = "jjWxg36jzbNTyL3cTAx+fBKuerSOTbzE8vK9hcyzKbA=";//old key
        //public static string blob { get; set; } = "https://azrbea026pr00.blob.core.windows.net/";
        //public static string scandItLicencekey = "";//Key to be added for LIVE.
        #endregion
    }
}
