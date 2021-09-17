using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.RestClientAPI;
using YPS.Service;
using YPS.Views;

namespace YPS
{
    public class OAuthConfig
    {
        #region Data member declaration
        static NavigationPage _NavigationPage;
        public static UserDetails User;
        YPSService service = new YPSService();
        #endregion

        public static Action SuccessfulLoginAction
        {
            get
            {
                return new Action(() =>
                {
                    try
                    {
                        if (Settings.IIJToken != null)
                        {
                            getdetails();
                        }
                        else
                        {
                            App.Current.MainPage = new MenuPage(typeof(YPS.Views.LoginPage));
                        }
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "getdetails-> in SuccessfulLoginAction " + Settings.userLoginID);
                        YPSService service = new YPSService();
                        service.Handleexception(ex);
                    }

                });
            }
        }

        public static async void getdetails()
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = null;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Settings.token_type, Settings.IIJToken);
                response = httpClient.GetAsync("https://www.auth.iij.jp/op/userinfo/me").Result;
                
                if (response.IsSuccessStatusCode)
                {
                    var DataResult = response.Content.ReadAsStringAsync().Result;
                    var USERProfile = JsonConvert.DeserializeObject<IIJUserInfo>(DataResult);
                    
                    if (USERProfile.preferred_username != null)
                    {
                        Settings.IIJLoginID = USERProfile.preferred_username;
                        redirectingApp(Settings.IIJLoginID);
                    }
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                YPSLogger.ReportException(ex, "getdetails method -> in OAuthConfig.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }

        }

        public static async void redirectingApp(string IIJLoginId)
        {
            try
            {
                YPSService service = new YPSService();
                
                if (!string.IsNullOrEmpty(IIJLoginId))
                {
                    Task.Run(async () => await Loginapicalling(IIJLoginId)).Wait();

                    if (IIJLoginId.ToLower() == EncryptManager.Decrypt(Settings.LoginID).ToLower())
                    {
                        SendTokendetails senddetails = new SendTokendetails();
                        senddetails.access_token = Settings.IIJToken;
                        senddetails.token_type = Settings.token_type;
                        senddetails.expires_in = Convert.ToInt32(Settings.expires_in);
                        senddetails.id_token = Settings.id_token;
                        senddetails.UserID = Settings.userLoginID;
                        senddetails.LoginID = Settings.LoginID;
                        var iijresponse = await service.UpdateIIJtoken(senddetails);

                        if (iijresponse != null)
                        {
                            if (iijresponse.status != 0)
                            {
                                Task.Run(async () => await CloudFolderKeyVal.GetToken()).Wait();
                                var DBresponse = await service.GetSaveUserDefaultSettings(Settings.userLoginID);

                                await CloudFolderKeyVal.CheckAndRegisterDeviceForPN();

                                if (DBresponse.status != 0)
                                {
                                    Settings.CompanyID = DBresponse.data.CompanyID;
                                    Settings.ProjectID = DBresponse.data.ProjectID;
                                    Settings.JobID = DBresponse.data.JobID;

                                    Settings.CompanySelected = DBresponse.data.CompanyName;
                                    Settings.ProjectSelected = DBresponse.data.ProjectName;
                                    Settings.JobSelected = DBresponse.data.JobNumber;
                                    App.Current.MainPage = new MenuPage(typeof(HomePage));
                                }
                                else
                                {
                                    App.Current.MainPage = new MenuPage(typeof(ProfileSelectionPage));
                                }
                            }
                            else
                            {
                                App.Current.MainPage = new MenuPage(typeof(YPS.Views.LoginPage));
                            }
                        }
                        else
                        {
                            App.Current.MainPage = new MenuPage(typeof(YPS.Views.LoginPage));
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Invalid LoginID.", "OK");
                        App.Current.MainPage = new MenuPage(typeof(YPS.Views.LoginPage));
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Invalid LoginID.", "OK");
                }
                OAuthConfig.User = null;
            }
            catch (System.Exception ex)
            {
                YPSLogger.ReportException(ex, "redirectingApp method -> in OAuthConfig.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
        }

        public static async Task Loginapicalling(string userName)
        {
            try
            {
                LoginData login = new LoginData();
                login.LoginID = EncryptManager.Encrypt(userName);
                Settings.LoginID = EncryptManager.Encrypt(userName);
                ///Calling login API to login.
                RestClient service = new RestClient();
                var result = await service.LoginRestClientFromIIJ(login);

                if (result.status != 0)
                {
                    if (result.data.UserID > 0)
                    {
                        CloudFolderKeyVal.Assignvaluestosettings(result);

                        ///Checking device type is android or iOS.
                        if (Device.RuntimePlatform == Device.Android)
                        {
                            Settings.FireBasedToken = await SecureStorage.GetAsync("Token");
                        }
                        else
                        {
                            Settings.FireBasedToken = await SecureStorage.GetAsync("iOSFireBaseToken");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string message = ex.Message;
                YPSLogger.ReportException(ex, "Loginapicalling method -> in OAuthConfig.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
        }
    }
}
