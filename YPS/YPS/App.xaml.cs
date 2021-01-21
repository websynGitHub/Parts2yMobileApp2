﻿using DLToolkit.Forms.Controls;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Service;
using YPS.Views;
using YPS.Models;
using Device = Xamarin.Forms.Device;
using System.Collections.Generic;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;

[assembly: Xamarin.Forms.Xaml.XamlCompilation(Xamarin.Forms.Xaml.XamlCompilationOptions.Compile)]
namespace YPS
{
    public partial class App : Application
    {
        #region ICommands and data members declaration
        public event EventHandler<ChatMessage> OnChatMessageReceived;
        public static NavigationPage MyNavigationPage;
        static HubConnection _connection = null;
        static IHubProxy _proxy = null;
        public static string ImgBase64Format { get; set; }
        public static string type;
        public int HasInterNet { get; private set; }
        public static bool IsNetworkAccessing { get; set; }
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        public static bool Approotbeer { get; set; }
        public static bool IsIpapatch { get; set; }
        public static string is_param_val;
        public static string isNotifiedId;
        public bool IsNotified;
        public static bool isJailBroken { get; set; }
        public static System.Timers.Timer timer;
        YPSService service = new YPSService();
        #endregion

        #region Push notification
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="val"></param>
        public App(bool val)
        {
            YPSLogger.TrackEvent("App.cs", "Page Loading push notification " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {

                InitializeComponent();
                SetupCertificatePinningCheck();
                FlowListView.Init();
                MyNavigationPage = new NavigationPage();
                Current.MainPage = MyNavigationPage;

                var globalResult = Task.Run(async () => await YPSService.GetglobelSettings()).Result; ///Fetching global settings

                if (globalResult != null && globalResult.status == 1 && (globalResult.data.sSLPinningKeys != null && EncryptManager.Decrypt(globalResult.data.sSLPinningKeys.SecCode) == Settings.CertificateSecCode))
                {
                    if (globalResult.data.sSLPinningKeys.IsPinnigEnabled == true)
                    {
                        Task.Run(async () => await PinPublickey(globalResult.data.sSLPinningKeys.Keys)).Wait();
                        SetupCertificatePinningCheck();
                    }

                    var navPages = is_param_val.Split(';');

                    if (!String.IsNullOrEmpty(navPages[0]))
                    {
                        if (navPages[0] == "AddUser" || navPages[0] == "Close" || navPages[0] == "receiveMessage")
                        {
                            Settings.GetParamVal = is_param_val;
                            App.Current.MainPage = new YPSMasterPage(typeof(ChatPage));
                        }
                        else if (navPages[0] == "RemoveUser")
                        {
                            DependencyService.Get<ISQLite>().deleteReadCountNmsg(Convert.ToInt32(navPages[4]));
                            RememberPwdDB Db = new RememberPwdDB();
                            var user = Db.GetUserDetails();

                            if (user.Count == 1)
                            {
                                var userData = user.FirstOrDefault();
                                Settings.userLoginID = Convert.ToInt32(EncryptManager.Decrypt(userData.encUserId));
                                Settings.userRoleID = Convert.ToInt32(EncryptManager.Decrypt(userData.encUserRollID));
                                Settings.Sessiontoken = userData.encSessiontoken;
                                Settings.AndroidVersion = userData.AndroidVersion;
                                Settings.iOSversion = userData.iOSversion;
                                Settings.IsIIJEnabled = userData.IIJEnable;
                                App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                                App.Current.MainPage.DisplayAlert("Message", "You removed from " + " '" + navPages[7] + "' " + ", Can not see previous conversation", "OK");

                            }
                            else
                            {
                                MyNavigationPage.PushAsync(new YPS.Views.LoginPage(), true);
                            }

                        }
                    }
                }
                else
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert("SSL keys not found, closing app", "Try again.", "Close");
                        CloudFolderKeyVal.Appredirectloginwithoutlogout(false);
                    });
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "App Constructor with params  -> in App.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public App()
        {
            YPSLogger.TrackEvent("App.cs", "Page Loading " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {

                InitializeComponent();
                MyNavigationPage = new NavigationPage();
                Current.MainPage = MyNavigationPage;

                #region YPS Relatesd code
                //var globalResult = Task.Run(async () => await YPSService.GetglobelSettings()).Result; ///Fetching global settings

                //if (globalResult != null && globalResult.status == 1 && (globalResult.data.sSLPinningKeys != null && EncryptManager.Decrypt(globalResult.data.sSLPinningKeys.SecCode) == Settings.CertificateSecCode))
                //{
                //    if (globalResult.data.sSLPinningKeys.IsPinnigEnabled == true)
                //    {
                //        Task.Run(async () => await PinPublickey(globalResult.data.sSLPinningKeys.Keys)).Wait();
                //        SetupCertificatePinningCheck();
                //    }

                //    FlowListView.Init();

                //    RememberPwdDB Db = new RememberPwdDB();
                //    var user = Db.GetUserDetails();

                //    if (user.Count == 1)
                //    {
                //        var userData = user.FirstOrDefault();

                //        Settings.userLoginID = Convert.ToInt32(EncryptManager.Decrypt(userData.encUserId));
                //        Settings.userRoleID = Convert.ToInt32(EncryptManager.Decrypt(userData.encUserRollID));
                //        Settings.LoginID = userData.encLoginID;
                //        Settings.Sessiontoken = userData.encSessiontoken;
                //        Settings.AndroidVersion = userData.AndroidVersion;
                //        Settings.iOSversion = userData.iOSversion;
                //        Settings.IsIIJEnabled = userData.IIJEnable;
                //        Settings.IsPNEnabled = userData.IsPNEnabled;
                //        Settings.IsEmailEnabled = userData.IsEmailEnabled;

                //        var current = Connectivity.NetworkAccess;
                //        if (current == NetworkAccess.Internet)
                //        {
                //            Task.Run(async () => await CloudFolderKeyVal.GetToken()).Wait();
                //            if (Settings.userRoleID != 0)
                //                App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                //        }
                //        else
                //        {
                //            MyNavigationPage.PushAsync(new CheckInterNetConn());
                //        }
                //    }
                //    else
                //    {
                //        Task.Run(async () => await GlobelSettings(globalResult)).Wait();

                //        if (Settings.IsIIJEnabled)
                //        {
                //            if (OAuthConfig.User == null)
                //            {
                //                MyNavigationPage.PushAsync(new ProviderLoginPage());
                //            }
                //        }
                //        else
                //        {
                //            var current = Connectivity.NetworkAccess;
                //            if (current == NetworkAccess.Internet)
                //            {
                //                MyNavigationPage.PushAsync(new LoginPage(), true);
                //            }
                //            else
                //            {
                //                MyNavigationPage.PushAsync(new CheckInterNetConn());
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    Device.BeginInvokeOnMainThread(async () =>
                //    {
                //        await App.Current.MainPage.DisplayAlert("SSL keys not found, closing app", "Try again.", "Close");
                //        CloudFolderKeyVal.Appredirectloginwithoutlogout(false);
                //    });
                //}

                #endregion

                #region Parts2y/ePod related code
                Parts2y.Parts2y_SQLITE.RememberPwdDB Db = new Parts2y.Parts2y_SQLITE.RememberPwdDB();
                var user = Db.GetUserDetails();

                if (user.Count != 0)
                {
                    var userData = user.FirstOrDefault();
                    YPS.Parts2y.Parts2y_Common_Classes.Settings.UserName = userData.username;
                    YPS.Parts2y.Parts2y_Common_Classes.Settings.UserMail = userData.user_email;
                    YPS.Parts2y.Parts2y_Common_Classes.Settings.Entity_Name = userData.EntityName;
                    YPS.Parts2y.Parts2y_Common_Classes.Settings.roleid = userData.roleid;
                    YPS.Parts2y.Parts2y_Common_Classes.Settings.ID = userData.ID;
                    YPS.Parts2y.Parts2y_Common_Classes.Settings.Bar_Background = Color.FromHex(userData.Colorcode);

                    //if (Settings.roleid == 1)
                    //{
                    App.Current.MainPage = new MenuPage(typeof(Dashboard));
                    //}
                    //else if (Settings.roleid == 2)
                    //{
                    //    App.Current.MainPage = new MenuPage(typeof(HomePage));
                    //}
                    //else if (Settings.roleid == 3)
                    //{
                    //    App.Current.MainPage = new MenuPage(typeof(DealerPage));
                    //}
                }
                else
                {
                    MyNavigationPage.PushAsync(new Parts2y.Parts2y_Views.LoginPage(), true);

                }
                #endregion
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "App Constructor without params  -> in App.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Global Settings.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task GlobelSettings(ApplicationSettings result)
        {
            try
            {
                //bool ifNotRootedDevie;

                //if (Device.RuntimePlatform == Device.Android)
                //{
                //    ///By using dependency service, checking mobile is root or not.
                //    ifNotRootedDevie = await DependencyService.Get<IRootDetection>().CheckIfRooted();
                //}
                //else
                //{
                //    ifNotRootedDevie = true;
                //}
                App.isJailBroken = false;/// need to remove this line when run in device 
                if (!App.isJailBroken)
                {
                    App.IsIpapatch = false;/// need to remove this line when run in device 
                    if (!App.IsIpapatch)
                    {
                        App.Approotbeer = false;/// need to remove this line when run in device 
                        if (!App.Approotbeer)
                        {
                            ////ifNotRootedDevie = true;/// need to remove this line when run in device 
                            //if (ifNotRootedDevie)
                            //{

                            //var result = await YPSService.GetglobelSettings();
                            if (result != null)
                            {
                                if (result.status == 1)
                                {
                                    Settings.BlobConnection = result.data.BlobConnection;
                                    Settings.PhotoSize = result.data.PhotoSize;
                                    Settings.CompressionQuality = result.data.CompressionQuality;
                                    Settings.BlobStorageConnectionString = EncryptManager.Decrypt(Settings.BlobConnection);
                                    Settings.DateFormatformAPI = result.data.DateFormat;
                                    Settings.TimeFormatforAPI = result.data.DateTimeFormat;
                                    Settings.IIJConsumerKey = EncryptManager.Decrypt(result.data.IIJConsumerKey);
                                    Settings.IIJConsumerSecret = EncryptManager.Decrypt(result.data.IIJConsumerSecret);
                                    Settings.IsIIJEnabled = result.data.IsIIJEnabled;
                                    Settings.IsPNEnabled = result.data.IsPNEnabled;
                                    Settings.IsEmailEnabled = result.data.IsEmailEnabled;
                                    // Settings.IsIIJEnabled = true;       
                                }
                            }
                            //}
                            //else
                            //{
                            //    Exception ex = new Exception();
                            //    YPSLogger.ReportException(ex, "LoginMethod method ->Your phone is rooted , please unroot to use app in LoginPageViewModel ");
                            //    await App.Current.MainPage.DisplayAlert("Warning", "Your phone is rooted , please unroot to use app", "OK");
                            //    System.Diagnostics.Process.GetCurrentProcess().Kill();
                            //}
                        }
                        else
                        {
                            Exception ex = new Exception();
                            YPSLogger.ReportException(ex, "LoginMethod method ->Security of this device is compromised. The app will exit. in LoginPageViewModel ");
                            await App.Current.MainPage.DisplayAlert("Warning", "Security of this device is compromised. The app will exit.", "OK");
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                        }
                    }
                    else
                    {
                        Exception ex = new Exception();
                        YPSLogger.ReportException(ex, "LoginMethod method -> Hacked with IPAPatch  ");
                        await App.Current.MainPage.DisplayAlert("Hacked", "Hacked with IPAPatch", "OK");
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                }
                #region Jailbreak
                else
                {
                    Exception ex = new Exception();
                    YPSLogger.ReportException(ex, "LoginMethod method -> Jailbreak detected  ");
                    await App.Current.MainPage.DisplayAlert("Jailbreak detected", "Device is jailbroken", "Cancel");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
                #endregion

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GlobelSettings method  -> in App.cs " + Settings.userLoginID);
            }
        }

        #region SignalRConnection

        public static HubConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new HubConnection(HostingURL.HubConnectionUrl);
                }
                return _connection;
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// Checking signalR connection.
        /// </summary>
        public static void CheckingSignalRConnection()
        {
            YPSLogger.TrackEvent("App.cs", "CheckingSignalRConnection " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                ConnectionState oConnectionState = App.Connection.State;
                if (oConnectionState == ConnectionState.Disconnected || oConnectionState == ConnectionState.Reconnecting
                    || oConnectionState == ConnectionState.Connecting)
                {
                    App.Connection.Stop();
                    App.Connection.Start().Wait();
                }
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "CheckingSignalRConnection method  -> in App.cs " + Settings.userLoginID);
                App.Current.MainPage.DisplayAlert("Chat", "Looks like we weren't able to connect to our server, please try again in a few minutes.", "Ok");
            }
        }

        public static IHubProxy Proxy
        {
            get
            {
                try
                {
                    if (_proxy == null)
                    {
                        _proxy = Connection.CreateHubProxy("GroupChatHub");

                        if (Connection != null)
                        {
                            Connection.Start().Wait();
                        }
                    }
                    return _proxy;

                }
                catch (System.Exception ex)
                {
                    _proxy = null;
                    return null;
                }
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// This method is to reconnect signalR connection.
        /// </summary>
        public static void ReconnectConnectSignalR()
        {
            try
            {
                _connection = null;
                _proxy = null;
                _connection = new HubConnection(HostingURL.HubConnectionUrl);

                _proxy = _connection.CreateHubProxy("GroupChatHub");
                _connection.Start();

                Execute();
            }
            catch (Exception ex)
            {
                //YPSLogger.ReportException(ex, "Connect method -> in ChatServices.cs " + Settings.userLoginID);
                //await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Thsi method checks the signalR connection.
        /// </summary>
        /// <returns></returns>
        public static bool CheckSignalRConnection()
        {
            YPSLogger.TrackEvent("App.cs", "CheckSignalRConnection " + DateTime.Now + " UserId: " + Settings.userLoginID);
            bool connectionResult = false;

            try
            {
                if (_connection == null)
                {
                    _connection = new HubConnection(HostingURL.HubConnectionUrl);
                    _proxy = _connection.CreateHubProxy("GroupChatHub");

                    App.Connection.Stop();
                    _connection.Start();

                    if (Execute() == true) { connectionResult = true; }
                }
                else if (_connection.State == ConnectionState.Disconnected || _connection.State == ConnectionState.Connecting || _connection.State == ConnectionState.Reconnecting)
                {
                    _connection = new HubConnection(HostingURL.HubConnectionUrl);
                    _proxy = _connection.CreateHubProxy("GroupChatHub");
                    App.Connection.Stop();
                    _connection.Start();
                    if (Execute() == true) { connectionResult = true; }
                }
                else if (_connection.State == ConnectionState.Connected)
                {
                    if (_proxy == null)
                    {
                        _proxy = _connection.CreateHubProxy("GroupChatHub");
                    }
                    App.Connection.Stop();
                    _connection.Start();
                    if (Execute() == true) { connectionResult = true; }
                }
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "CheckSignalRConnection method  -> in App.cs " + Settings.userLoginID);

                connectionResult = false;
            }
            return connectionResult;
        }

        /// <summary>
        /// To check whether Signal R connection is establied or not. if connection is connected,
        /// then this method will return true otherwise return false;
        /// </summary>
        /// <returns></returns>
        private static bool Execute()
        {
            try
            {
                YPSLogger.TrackEvent("App.cs", "Execute " + DateTime.Now + " UserId: " + Settings.userLoginID);
                bool isConnectionIdAvailable = false;

                _connection.Start().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        isConnectionIdAvailable = false;
                    }
                    else
                    {
                        isConnectionIdAvailable = true;
                    }

                }).Wait();

                if (Settings.isExpectedPublicKey == false)
                {
                    Settings.isExpectedPublicKey = true;
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                    return false;
                }
                else
                {
                    return isConnectionIdAvailable;
                }
            }
            catch (Exception ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                    return false;
                }
                #endregion
                else
                {
                    YPSService service = new YPSService();
                    service.Handleexception(ex);
                    YPSLogger.ReportException(ex, "Execute method  -> in App.cs " + Settings.userLoginID);
                    return false;
                }
            }
        }
        #endregion

        /// <summary>
        /// This method checks device internet.
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> CheckInterNetConnection()
        {
            try
            {
                string CheckUrl = HostingURL.HubConnectionUrl;
                string StatusDescription = string.Empty;

                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    HttpWebRequest iNetRequest = (HttpWebRequest)WebRequest.Create(CheckUrl);

                    using (var response = (HttpWebResponse)(await iNetRequest.GetResponseAsync()))
                    {
                        StatusDescription = response.StatusDescription;
                        if (StatusDescription.ToLower() == "OK".ToLower())
                        {
                            return IsNetworkAccessing = true;
                        }
                        else
                        {
                            return IsNetworkAccessing = false;
                        }
                    }
                }
                else
                {
                    return IsNetworkAccessing = false;
                }
            }
            catch (WebException ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    YPSLogger.ReportException(ex, "CheckInterNetConnection method because of SSL public key mismatch -> in App.cs " + Settings.userLoginID);
                    //await App.Current.MainPage.DisplayAlert("Man in the middle detected", "potential security risk ahead", "Close");
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                }
                #endregion
                else
                {
                    YPSLogger.ReportException(ex, "CheckInterNetConnection method  -> in App.cs " + Settings.userLoginID);
                }
                return IsNetworkAccessing = false;
            }
        }

        /// <summary>
        /// Gets called when app is started.
        /// </summary>
        protected async override void OnStart()
        {
            /// Handle when your app starts            
            AppCenter.Start(HostingURL.Appcenter_iOS + HostingURL.Appcenter_droid, typeof(Analytics), typeof(Crashes));
            YPSLogger.TrackEvent("Appcenter:", "Logger Activated" + DateTime.Now + " UserId: " + Settings.userLoginID);
        }

        /// <summary>
        /// Gets called when app is minimised.
        /// </summary>
        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        /// <summary>
        /// Gets called when app is resume.
        /// </summary>
        protected async override void OnResume()
        {
            // Handle when your app resumes            
            bool tt = App.CheckSignalRConnection();
        }

        #region Public Key related methods
        /// <summary>
        /// Assigning the fetched public keys to Settings.
        /// </summary>
        /// <param name="publickeys"></param>
        /// <returns></returns>
        public async Task PinPublickey(List<PublicKeyData> publickeys)
        {
            try
            {
                if (!string.IsNullOrEmpty(publickeys[0].CertificateKey) && !string.IsNullOrEmpty(publickeys[1].CertificateKey) && !string.IsNullOrEmpty(publickeys[2].CertificateKey))
                {
                    Settings.WebServerPublicKey = Helperclass.Decrypt(publickeys[0].CertificateKey);
                    Settings.blobServerPubKey = Helperclass.Decrypt(publickeys[1].CertificateKey);
                    Settings.IIJServerPublicKey = Helperclass.Decrypt(publickeys[2].CertificateKey);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PinPublickey method  -> in App.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Assigning the adding/assigning the method to servercallback deligate.
        /// </summary>
        public static void SetupCertificatePinningCheck()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls
                    | SecurityProtocolType.Tls11;
                ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SetupCertificatePinningCheck method  -> in App.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method is gets called when API calling is made, to verify the Public key.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            try
            {
                Settings.isExpectedPublicKey = true;
                var certpublickeyBytes = certificate?.GetPublicKey();
                string strPublicKey = Convert.ToBase64String(certpublickeyBytes);

                if (strPublicKey == Settings.WebServerPublicKey || strPublicKey == Settings.blobServerPubKey || strPublicKey == Settings.IIJServerPublicKey)
                {
                    Settings.isExpectedPublicKey = true;
                }
                else
                {
                    Settings.isExpectedPublicKey = false;
                }
            }
            catch (Exception ex)
            {
                Settings.isExpectedPublicKey = false;
                YPSLogger.ReportException(ex, "ValidateServerCertificate method  -> in App.cs " + Settings.userLoginID);
            }
            return Settings.isExpectedPublicKey;
        }
        #endregion
    }
}
