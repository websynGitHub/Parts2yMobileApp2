using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using YPS.Views;

namespace YPS.ViewModel
{
    public class LoginPageViewModel : IBase
    {
        #region ICommands and variable declaration.
        public ICommand ILoginCommand { get; set; }
        bool checkInternet;
        bool ifNotRootedDevie;
        YPSService service;
        #endregion

        /// <summary>
        /// Parameter less constructor.
        /// </summary>
        public LoginPageViewModel()
        {
            YPSLogger.TrackEvent("LoginPageViewModel", "Page LoginPageViewModel method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                ILoginCommand = new Command(async () => await LoginMethod()); ///Assign LoginMethod ICommand properties
                service = new YPSService();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "LoginPageViewModel constructor -> in LoginPageViewModel " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Login" on the button and proceed login process.
        /// </summary>
        /// <returns> </returns>
        private async Task LoginMethod()
        {
            IndicatorVisibility = true;
            YPSLogger.TrackEvent("LoginPageViewModel", "in LoginMethod method " + DateTime.Now + " UserId: " + userName);
            try
            {
                /// Entry control checking is empty or not.
                if (String.IsNullOrWhiteSpace(userName))
                {
                    await App.Current.MainPage.DisplayAlert("Login", "Invalid LoginID.", "OK");
                    pwdInvalidText = "";
                    PwdError = false;
                    invalidLoginText = "Invalid LoginID.";
                    LoginIDError = true;
                }
                else if ((!Regex.IsMatch(userName, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$")))
                {
                    pwdInvalidText = "";
                    PwdError = false;
                    invalidLoginText = "Invalid LoginID.";
                    LoginIDError = true;
                    await App.Current.MainPage.DisplayAlert("Login", "Invalid LoginID.", "Ok");
                }
                else
                {
                    ///Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        LoginData login = new LoginData();
                        login.LoginID = EncryptManager.Encrypt(userName);
                        Settings.LoginID = EncryptManager.Encrypt(userName);
                        ///Calling login API to login.
                        var result = await service.LoginService(login);

                        if (result.status != 0)
                        {
                            if (result.data.UserID > 0)
                            {
                                CloudFolderKeyVal.Assignvaluestosettings(result);

                                Settings.Bar_Background = Color.FromHex(!string.IsNullOrEmpty(result.data.RoleColorCode) ? result.data.RoleColorCode : result.data.VersionColorCode);
                                if (result.data.JwToken.IsIIJEnabled)
                                {
                                    if (OAuthConfig.User == null)
                                    {
                                        App.Current.MainPage = new MenuPage(typeof(ProviderLoginPage));
                                    }
                                }
                                else
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

                                    /// Calling API to get user saved default data.
                                    var DBresponse = await service.GetSaveUserDefaultSettings(Settings.userLoginID);

                                    if (DBresponse != null)
                                    {
                                        if (DBresponse.status != 0)
                                        {
                                            Settings.CompanyID = DBresponse.data.CompanyID;
                                            Settings.ProjectID = DBresponse.data.ProjectID;
                                            Settings.JobID = DBresponse.data.JobID;
                                            Settings.SupplierID = DBresponse.data.SupplierID;
                                            Settings.CompanySelected = DBresponse.data.CompanyName;
                                            Settings.ProjectSelected = DBresponse.data.ProjectName;
                                            Settings.JobSelected = DBresponse.data.JobNumber;
                                            Settings.SupplierSelected = DBresponse.data.SupplierName;
                                            App.Current.MainPage = new MenuPage(typeof(HomePage));
                                        }
                                        else
                                        {
                                            await App.Current.MainPage.Navigation.PushAsync(new ProfileSelectionPage((int)QAType.PT));
                                        }
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alert!", "Invalid LoginID.", "OK");
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }

                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LoginMethod method -> in LoginPageViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method is used for Notification.
        /// </summary>
        private async void registerToken(object obj)
        {
            if (Settings.FireBasedToken != null)
            {
                LoginModel model = new LoginModel();

                model.UserId = Settings.userLoginID;
                model.FireBasedToken = Settings.FireBasedToken;

                var Checkdevicedata = await service.CheckDevice(model); //check device
            }
        }

        #region Properties
        string _userName;
        public string userName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyPropertyChanged();
            }
        }

        bool _LoginIDError = false;
        public bool LoginIDError
        {
            get { return _LoginIDError; }
            set
            {
                _LoginIDError = value;
                NotifyPropertyChanged();
            }
        }

        bool _PwdError = false;
        public bool PwdError
        {
            get { return _PwdError; }
            set
            {
                _PwdError = value;
                NotifyPropertyChanged();
            }
        }

        string _invalidLoginText = string.Empty;
        public string invalidLoginText
        {
            get { return _invalidLoginText; }
            set
            {
                _invalidLoginText = value;
                NotifyPropertyChanged();
            }
        }

        string _pwdInvalidText = string.Empty;
        public string pwdInvalidText
        {
            get { return _pwdInvalidText; }
            set
            {
                _pwdInvalidText = value;
                NotifyPropertyChanged();
            }
        }

        bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
