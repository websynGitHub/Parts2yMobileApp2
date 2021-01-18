using System;
using System.Linq;
using System.Threading.Tasks;
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
            switch (key)
            {
                case (int)BlobContainer.cnttagfiles:
                    return "tag-files";
                case (int)BlobContainer.cnttagphotos:
                    return "tag-photos";
                case (int)BlobContainer.cntchatphotos:
                    return "chat-photos";
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
                            Assignvaluestosettings(result);
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
                YPSLogger.ReportException(ex, "CloudFolderKeyVal-> in GetToken " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
                return "";

            }
        }

        public static void Assignvaluestosettings(LoginUserData result)
        {
            try
            {

                Settings.userRoleID = result.data.RoleID;
                Settings.userLoginID = result.data.UserID;
                // CrossSecureStorage.Current.SetValue("userID", result.data.UserID.ToString());
                Settings.Username = result.data.FullName;
                Settings.UserMail = result.data.Email;
                Settings.SGivenName = result.data.GivenName;
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

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CloudFolderKeyVal-> in Assignvaluestosettings " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }
        public async static void Appredirectlogin(string message, bool showalert = true)
        {
            try
            {
                YPSService service = new YPSService();
                Settings.Username = Settings.UserMail =
                Settings.SGivenName = Settings.EntityName = Settings.RoleName = Settings.access_token = Settings.JobSelected =
                Settings.BlobConnection = Settings.BlobStorageConnectionString =
                Settings.CompanySelected = Settings.SupplierSelected = Settings.Projectelected = Settings.IIJConsumerKey =
                Settings.IIJConsumerSecret = string.Empty;
                Settings.SupplierID = Settings.ProjectID = Settings.CompanyID = Settings.JobID =
                Settings.CompressionQuality = Settings.PhotoSize = Settings.userRoleID = 0;
                Settings.IsIIJEnabled = false;
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
                    App.Current.MainPage = new NavigationPage(new LoginPage());
                }
                else
                {
                    if (OAuthConfig.User == null)
                    {
                        App.Current.MainPage = new NavigationPage(new ProviderLoginPage());
                    }
                }
                //var Logoutresult = await service.LogoutService();
                return;

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CloudFolderKeyVal-> in Appredirectlogin " + Settings.userLoginID);
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
                Settings.CompanySelected = Settings.SupplierSelected = Settings.Projectelected = Settings.IIJConsumerKey =
                Settings.IIJConsumerSecret = string.Empty;
                Settings.SupplierID = Settings.ProjectID = Settings.CompanyID = Settings.JobID =
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
                YPSLogger.ReportException(ex, "CloudFolderKeyVal-> in Appredirectloginwithoutlogout " + Settings.userLoginID);
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
        #endregion


        #region DEV:

        //public static string HubConnectionUrl { get; set; } = "https://dev.parts2y.com/";
        //public static string WebServiceUrl { get; set; } = "https://dev.parts2y.com/api/";
        //public static string Appcenter_droid { get; set; } = "android=ebfaf0cf-af6f-4f28-a6c9-642352279430;";
        //public static string Appcenter_iOS { get; set; } = "ios=37be6471-dc83-405d-b62f-e796ae44267d;";
        //public static string Bdchk { get; set; } = "DprjHRPut1l2lsH4K5tRcw==";
        #endregion

        #region LIVE:
        //public static string HubConnectionUrl { get; set; } = "https://www.parts2y.com/";
        //public static string WebServiceUrl { get; set; } = "https://www.parts2y.com/api/";
        //public static string Appcenter_droid { get; set; } = "android=187f2593-7ed5-4ea9-8bb8-61f03175e30f;";
        //public static string Appcenter_iOS { get; set; } = "ios=6aeae3ae-1423-41c0-b9ec-0de97e33789d;";
        //public static string Bdchk { get; set; } = "jjWxg36jzbNTyL3cTAx+fBKuerSOTbzE8vK9hcyzKbA=";
        #endregion
    }
}
