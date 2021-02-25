using Acr.UserDialogs;
using FFImageLoading.Forms.Touch;
using Firebase.CloudMessaging;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using PanCardView.iOS;
using Plugin.Media;
using Syncfusion.SfDataGrid.XForms.iOS;
using Syncfusion.XForms.iOS.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using UIKit;
using UserNotifications;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using YPS.Views;
using ZXing.Net.Mobile.Forms;

namespace YPS.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate
    {
        #region Data member
        public static string is_param_val;
        string parameterValue = string.Empty;
        public static System.Timers.Timer timer;

        #region Jailbreak
        private Cryoprison.IJailbreakDetector jailbreakDetector;
        public static bool IsJailBroken = false;
        public static IEnumerable<string> JailBreaks = null;
        #endregion

        #region Appcenter Keys need to change every release version based on bulid type

        /// <summary>
        /// Alpha
        /// </summary>
        string Appcenter_iOS = "98470379-fccd-4f9d-8d22-8856a5af15c9";
        /// <summary>
        /// Beta
        /// </summary>
        /// <param name="Appcenter_iOS"></param>
        //string Appcenter_iOS = "37be6471-dc83-405d-b62f-e796ae44267d";///old Beta Key
        //string Appcenter_iOS = "3cb00169-1fb5-41b5-a1c6-0e3fdb108114";

        /// <summary>
        /// Production
        /// </summary>
        /// <param name="Appcenter_iOS"></param>
        //string Appcenter_iOS = "6aeae3ae-1423-41c0-b9ec-0de97e33789d";///old Production Key
        //string Appcenter_iOS = "02b039b4-862f-4236-94a7-6054b8bb0d10";

        #endregion

        #endregion

        public void DidRefreshRegistrationToken(Messaging messaging, string fcmToken)
        {
            System.Diagnostics.Debug.WriteLine($"FCM Token: {fcmToken}");
            Settings.FireBasedToken = fcmToken;
        }
        
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        string userAgent = "Mozilla / 5.0(Windows NT 6.1) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 41.0.2228.0 Safari / 537.36";
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            #region Jailbreak

            var env = Cryoprison.Factory.CreateEnvironment();

            env.Reporter.OnJailbreakReported = (id) =>
            {
                Console.WriteLine($"Jailbreak: {id ?? "<null>"}");
            };

            env.Reporter.OnExceptionReported = (message, exception) =>
            {
                Console.WriteLine($"Jailbreak Error: {message}");
                Console.WriteLine(exception.ToString());
            };

            jailbreakDetector = Cryoprison.Factory.CreateJailbreakDetector(env, simulatorFriendly: true);

            App.isJailBroken = (jailbreakDetector.Violations.Count() == 0 || (jailbreakDetector.Violations.Count() == 1 && jailbreakDetector.Violations.Contains("EMBEDDED_MOBILEPROVISION_SHOULD_BE_PRESENT"))) ? false : true;

            #endregion

            NSDictionary dictionary = NSDictionary.FromObjectAndKey(NSObject.FromObject(userAgent), NSObject.FromObject("UserAgent"));
            NSUserDefaults.StandardUserDefaults.RegisterDefaults(dictionary);
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            #region IPAPATCH
            string bundleIdentifier = NSBundle.MainBundle.BundleIdentifier;
            //if (bundleIdentifier != EncryptManager.Decrypt(HostingURL.Bdchk))
            //{
            //    App.IsIpapatch = true;
            //    System.Diagnostics.Process.GetCurrentProcess().Kill();
            //}
            #endregion

            App.ScreenHeight = (int)UIScreen.MainScreen.Bounds.Height;
            App.ScreenWidth = (int)UIScreen.MainScreen.Bounds.Width;
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjY3MTkzQDMxMzgyZTMxMmUzMFhUak9IY0JaYUsrNTlsOXZqTExUeEt3SlNvNEZ6NHcyV3ZnWm1SQlIrM0U9");///18.1.0.42
            global::Xamarin.Forms.Forms.Init();
            CachedImageRenderer.Init();
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            //Appcenter value need to change based on (Alpha,Beta,Production//reference appcenter region)
            AppCenter.Start(Appcenter_iOS, typeof(Analytics), typeof(Crashes));

            LoadApplication(new App());

            #region authorizations for app
            // Firebase component initialize
            Firebase.Core.App.Configure();
            CardsViewRenderer.Preserve();

            Syncfusion.XForms.iOS.BadgeView.SfBadgeViewRenderer.Init();
            SfCheckBoxRenderer.Init();
            SfDataGridRenderer.Init();
            Syncfusion.XForms.iOS.Border.SfBorderRenderer.Init();
            Syncfusion.ListView.XForms.iOS.SfListViewRenderer.Init();
            Syncfusion.XForms.iOS.Cards.SfCardViewRenderer.Init();
            CrossMedia.Current.Initialize();
            #endregion

            // Request Permissions  
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // iOS 10
                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
               
                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
                {
                    Console.WriteLine(granted);
                });

                // Get current notification settings
                UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
                {
                    var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
                });
                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = this;
            }
            else
            {
                // iOS 9 <=
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            Settings.AppVersion = NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleVersion")].ToString();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            timer = new Timer((int)TimeSpan.FromMinutes(Settings.Timerminutes).TotalMilliseconds);
            timer.Elapsed += new ElapsedEventHandler(IPatchCheck);
            timer.AutoReset = true;
            timer.Enabled = true;

            return base.FinishedLaunching(app, options);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            YPSService trackService = new YPSService();
            System.Exception newExc = (System.Exception)unhandledExceptionEventArgs.ExceptionObject;
            YPSLogger.ReportException(newExc, "Unhandle exception-> CurrentDomainOnUnhandledException" + Settings.userLoginID);
            trackService.Handleexception(newExc).Wait();
            UserDialogs.Instance.HideLoading();
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            YPSService trackService = new YPSService();
            System.Exception newExc = (System.Exception)unobservedTaskExceptionEventArgs.Exception;
            YPSLogger.ReportException(newExc, "Unhandle exception->TaskSchedulerOnUnobservedTaskException " + Settings.userLoginID);
            trackService.Handleexception(newExc).Wait();
            UserDialogs.Instance.HideLoading();
        }

        public void IPatchCheck(Object source, System.Timers.ElapsedEventArgs e)
        {
            string bundleIdentifier = NSBundle.MainBundle.BundleIdentifier;
            if (bundleIdentifier != EncryptManager.Decrypt(HostingURL.Bdchk))
            {
                App.IsIpapatch = true;
                Exception ex = new Exception();
                YPSLogger.ReportException(ex, "LoginMethod method -> Hacked with IPAPatch  " + Settings.Username);
                App.Current.MainPage.DisplayAlert("Hacked", "Hacked with IPAPatch", "OK");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        #region Notifications Code
        public override void DidEnterBackground(UIApplication uiApplication)
        {
            Messaging.SharedInstance.Disconnect();
        }

        #region Background blur app resume

        //UIVisualEffectView _blurWindow = null;
        //public override void OnActivated(UIApplication uiApplication)
        //{
        //    connectFCM();
        //    base.OnActivated(uiApplication);
        //    ///blur background
        //    _blurWindow?.RemoveFromSuperview();
        //    _blurWindow?.Dispose();
        //    _blurWindow = null;
        //}
        //public override void OnResignActivation(UIApplication application)
        //{
        //    base.OnResignActivation(application);

        //    using (var blurEffect = UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark))
        //    {
        //        _blurWindow = new UIVisualEffectView(blurEffect)
        //        {
        //            Frame = UIApplication.SharedApplication.KeyWindow.RootViewController.View.Bounds
        //        };
        //        UIApplication.SharedApplication.KeyWindow.RootViewController.View.AddSubview(_blurWindow);
        //    }
        //}

        #endregion


        public override async void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {

            // Get current device token
            var DeviceToken = deviceToken.Description;
            
            if (!string.IsNullOrWhiteSpace(DeviceToken))
            {
                DeviceToken = DeviceToken.Trim('<').Trim('>');
            }

            Settings.FireBasedToken = DeviceToken.Replace(" ", "");
            // Get previous device token
            var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");
            //  Settings.FireBasedToken = oldDeviceToken;
            // Has the token changed?
            if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
            {
                DeviceToken = DeviceToken.Replace(" ", "");
                await SecureStorage.SetAsync("iOSFireBaseToken", DeviceToken);
                //TODO: Put your own logic here to notify your server that the device token has changed/been created!
            }

            // Save new device token
            NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");
        }

        // iOS 9 <=, fire when recieve notification foreground
        [Foundation.Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public async override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {

            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;
            NSDictionary keyValues = aps.ObjectForKey(new NSString("alert")) as NSDictionary;

            string paramValues = string.Empty;
            if (keyValues.ContainsKey(new NSString("param")))
                paramValues = (keyValues[new NSString("param")] as NSString).ToString();

            var showNotify = paramValues.Split(';');
            string val = await SecureStorage.GetAsync("mainPageisOn");

            if (val == "1")
            {
                Settings.notifyCount = Settings.notifyCount + 1;
                MessagingCenter.Send<string, string>("PushNotificationCame", "IncreaseCount", Settings.notifyCount.ToString());
            }
        }

        //iOS 10, fire when recieve notification foreground
        [Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
        public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {

            completionHandler(UNNotificationPresentationOptions.Alert);
        }

        private void connectFCM()
        {
            Console.WriteLine("connectFCM\tEjecutandose la función.");
            
            Messaging.SharedInstance.Connect((error) =>
            {
                if (error == null)
                {
                    //TODO: Change Topic to what is required
                    Messaging.SharedInstance.Subscribe("/topics/all");
                }

                Console.WriteLine("connectFCM\t" + (error != null ? "error occured" + error.DebugDescription : "connect success"));

            });
        }

        [Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
        public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            NSDictionary aps = response.Notification.Request.Content.UserInfo.ObjectForKey(new NSString("aps")) as NSDictionary;
            NSDictionary keyValues = aps.ObjectForKey(new NSString("alert")) as NSDictionary;
            string paramValues = string.Empty;
            
            if (keyValues.ContainsKey(new NSString("param")))
                paramValues = (keyValues[new NSString("param")] as NSString).ToString();

            if (!string.IsNullOrEmpty(paramValues))
            {

                var navPages = paramValues.Split(';');
               
                if (!String.IsNullOrEmpty(navPages[0]))
                {
                    if (navPages[0] == "AddUser" || navPages[0] == "Close" || navPages[0] == "receiveMessage")
                    {
                        Settings.GetParamVal = paramValues;
                        App.Current.MainPage = new MenuPage(typeof(ChatPage));

                    }
                    else if (navPages[0] == "RemoveUser")
                    {
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
                            App.Current.MainPage = new MenuPage(typeof(HomePage));
                        }
                    }
                }
            }
        }
        #endregion
    }
}

