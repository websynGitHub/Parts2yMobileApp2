using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Common.Apis;
using Android.Gms.SafetyNet;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Scottyab.Rootbeer;
using FFImageLoading.Forms.Droid;
using Firebase;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using PanCardView.Droid;
using Plugin.Media;
using Plugin.Permissions;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.Service;


namespace YPS.Droid
{
    [Activity(Label = "Parts2y WS", Icon = "@drawable/ypslogo22", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize |
        ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        #region Data members
        internal static readonly string CHANNEL_ID = "my_notification_channel";
        internal static readonly int NOTIFICATION_ID = 100;
        const string NOTIFICATION_ACTION = "ReceiverValue";
        private bool _lieAboutCurrentFocus;
        GoogleApiClient googleApiClient;
        bool approoted = false;
        public static string SafetyNetApiKey = EncryptManager.Decrypt("8QShFMkX4dpMqi7ChnEbBHkZNp7Vi8mRkjGFWepYYwDwLhEEoFcMxIPW0CDDYfYl");
        public static MainActivity mainActivity;

        #region Appcenter Keys need to change every release version based on bulid type

        /// <summary>
        /// Alpha
        /// </summary>
        //string Appcenter_droid = "3068e436-13c0-4cac-873f-687d8c0830c3";
        /// <summary>
        /// Beta
        /// </summary>
        /// <param name="Appcenter_droid"></param>
        //string Appcenter_droid = "ebfaf0cf-af6f-4f28-a6c9-642352279430";///Old beta Key
        string Appcenter_droid = "951a37fc-c630-42e1-afda-7c6644f4b418";

        /// <summary>
        /// Production
        /// </summary>
        /// <param name="Appcenter_droid"></param>
        //string Appcenter_droid = "187f2593-7ed5-4ea9-8bb8-61f03175e30f";///Old Production Key
        //string Appcenter_droid = "b3a4d5ad-e6bb-4e6e-a4b3-4b6dbdbd9ee4";

        #endregion

        bool doubleBackToExit = false;

        #endregion

        /// <summary>
        /// Gets called before App() when app is launching.
        /// </summary>
        /// <param name="savedInstanceState"></param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                UserDialogs.Init(this);
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;
                base.OnCreate(savedInstanceState);
                CrossMedia.Current.Initialize();
                CachedImageRenderer.Init();
                ZXing.Net.Mobile.Forms.Android.Platform.Init();
                initFontScale();

                global::Xamarin.Forms.Forms.Init(this, savedInstanceState);


                //Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                //Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                //Window.SetStatusBarColor(Color.FromName(Color.Gray));

                #region SafteyNet
                mainActivity = this;
                googleApiClient = new GoogleApiClient.Builder(this)
                  //ConnectionCallback interface
                  .AddApi(SafetyNetClass.API)
                  .Build();
                #endregion

                #region RootBeer
                RootBeer rootBeer = new RootBeer(this);

                if (rootBeer.IsRooted)
                {
                    App.Approotbeer = rootBeer.IsRooted;
                }
                #endregion

                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

                App.ScreenHeight = (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density);
                App.ScreenWidth = (int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density);

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjY3MTkzQDMxMzgyZTMxMmUzMFhUak9IY0JaYUsrNTlsOXZqTExUeEt3SlNvNEZ6NHcyV3ZnWm1SQlIrM0U9");
                FirebaseApp.InitializeApp(this);
                CardsViewRenderer.Preserve();

                //Appcenter value need to change based on (Alpha,Beta,Production//reference appcenter region)
                AppCenter.Start(Appcenter_droid, typeof(Analytics), typeof(Crashes));

                string parameterValue = this.Intent.GetStringExtra("param");
                string msgs2 = this.Intent.GetStringExtra("message");
                var prefs = GetSharedPreferences(this.PackageName, FileCreationMode.Private);
                string msgs = prefs.GetString("last_msg", "N/A");
                CreateNotificationChannel();

                var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

                notificationManager.CancelAll();

                string params_Val = Intent.GetStringExtra("getParam");

                if (String.IsNullOrEmpty(params_Val))
                {
                    LoadApplication(new App());
                }
                else
                {
                    var showNotify = params_Val.Split(';');
                    int PushId = Convert.ToInt32(showNotify[4]);
                    NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
                    var data = msg.SpecificNotification(PushId).LastOrDefault();
                    App.is_param_val = data == null ? params_Val : data?.AllParams;
                    LoadApplication(new App(true));
                }

                #region Background blur app resume

                //Window.SetFlags(WindowManagerFlags.Secure, WindowManagerFlags.Secure);

                #endregion

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnCreate  method-> in MainActivity.cs " + Settings.userLoginID);
            }
        }

        public void GetVersionNumber()
        {
            try
            {
                PackageInfo pInfo = Android.App.Application.Context.PackageManager.GetPackageInfo(PackageName, 0);
                CommonClasses.Settings.VersionName = pInfo.VersionName;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetVersionNumber method-> in MainActivity.cs " + Settings.userLoginID);
            }
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            try
            {
                var context = Application.Context;
                Toast.MakeText(context, "Application exception occurred. Please try again!", ToastLength.Short).Show();
                YPSService trackService = new YPSService();
                System.Exception newExc = (System.Exception)unhandledExceptionEventArgs.ExceptionObject;

                YPSLogger.ReportException(newExc, "Unhandle exception-> CurrentDomainOnUnhandledException" + Settings.userLoginID);

                UserDialogs.Instance.HideLoading();

                trackService.Handleexception(newExc).Wait();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CurrentDomainOnUnhandledException method-> in MainActivity.cs " + Settings.userLoginID);
            }
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            try
            {
                YPSService trackService = new YPSService();
                System.Exception newExc = (System.Exception)unobservedTaskExceptionEventArgs.Exception;
                YPSLogger.ReportException(newExc, "Unhandle exception->TaskSchedulerOnUnobservedTaskException " + Settings.userLoginID);
                trackService.Handleexception(newExc).Wait();
                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TaskSchedulerOnUnobservedTaskException method-> in MainActivity.cs " + Settings.userLoginID);
            }
        }

        #region Exit application on hardware button press
        /// <summary>
        /// This is to exit the application when pressed hardware button twice
        /// </summary>
        /// 
        public override void OnBackPressed()
        {
            try
            {
                if (doubleBackToExit)
                {
                    base.OnBackPressed();
                    return;
                }
                if (DoBack)
                {
                    this.doubleBackToExit = true;

                    Toast.MakeText(this, "Please click back again to exit?.", ToastLength.Short).Show();

                    new Handler().PostDelayed(() =>
                    {
                        doubleBackToExit = false;
                    }, 2000);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnBackPressed method-> in MainActivity.cs " + Settings.userLoginID);
            }
        }

        public bool DoBack
        {
            get
            {
                Xamarin.Forms.MasterDetailPage mainPage = App.Current.MainPage as Xamarin.Forms.MasterDetailPage;
                if (mainPage != null)
                {
                    bool canDoBack = mainPage.Detail.Navigation.NavigationStack.Count > 1 || mainPage.IsPresented;

                    if (canDoBack)
                    {
                        if (mainPage.Detail.Navigation.ModalStack.Count > 0)
                        {
                            mainPage.Detail.Navigation.PopModalAsync(false);
                        }
                        else if (mainPage.Detail.Navigation.NavigationStack.Count > 0)
                        {
                            if (mainPage.Detail.Navigation.NavigationStack[mainPage.Detail.Navigation.NavigationStack.Count - 1].GetType().Name.Trim().ToLower() == "PhotoUpload".Trim().ToLower())
                            {
                                CommonMethods.BackClickFromPhotoUpload(mainPage?.Detail?.Navigation, new Model.AllPoData { TaskName = SecureStorage.GetAsync("podataTastName").Result });
                            }
                            else if (mainPage.Detail.Navigation.NavigationStack[mainPage.Detail.Navigation.NavigationStack.Count - 1].GetType().Name.Trim().ToLower().Contains("Insp".Trim().ToLower()))
                            {
                                CommonMethods.BackClickFromInspToParts(mainPage?.Detail?.Navigation, new Model.AllPoData { TaskName = SecureStorage.GetAsync("podataTastName").Result });
                            }
                            else
                            {
                                if (mainPage.Detail.Navigation.NavigationStack[mainPage.Detail.Navigation.NavigationStack.Count - 2].GetType().Name.Trim().ToLower() != "ChatUsers".Trim().ToLower())
                                {
                                    mainPage.Detail.Navigation.PopAsync(false);
                                }
                            }
                        }
                        return false;
                    }
                    else
                    {
                        if (mainPage.Detail.Navigation.ModalStack.Count >= 1)
                        {
                            mainPage.Detail.Navigation.PopModalAsync(false);
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    Xamarin.Forms.Page x = App.Current.MainPage;
                    if (App.Current.MainPage.Navigation.ModalStack.Count >= 1)
                    {
                        App.Current.MainPage.Navigation.PopModalAsync(false);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        #endregion

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            bool result = false;
            try
            {
                if (Settings.currentPage == "chatPage")
                {
                    _lieAboutCurrentFocus = true;
                    result = base.DispatchTouchEvent(ev);
                    _lieAboutCurrentFocus = false;
                    return result;
                }
                else
                {
                    _lieAboutCurrentFocus = false;
                    result = base.DispatchTouchEvent(ev);
                    _lieAboutCurrentFocus = true;
                    return result;
                }
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                YPSLogger.ReportException(ex, "DispatchTouchEvent method-> in MainActivity.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
            return result;
        }

        public override View CurrentFocus
        {
            get
            {
                if (_lieAboutCurrentFocus)
                {
                    return null;
                }

                return base.CurrentFocus;
            }
        }

        protected override void OnStart()
        {
            try
            {
                base.OnStart();
                IntentFilter intentFilter = new
                   IntentFilter(NOTIFICATION_ACTION);
                //RegisterReceiver(notificationBroadcastReceiver,
                //   intentFilter);
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                YPSLogger.ReportException(ex, "OnStart method-> in MainActivity.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                Badge badge = new Badge(this);
                badge.count(0);
                Xamarin.Forms.DependencyService.Get<IPNClearClass>().CancelPush(null, 0);
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                YPSLogger.ReportException(ex, "OnResume method-> in MainActivity.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                base.OnStop();
                Badge badge = new Badge(this);
                badge.count(0);
                Xamarin.Forms.DependencyService.Get<IPNClearClass>().CancelPush(null, 0);
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                YPSLogger.ReportException(ex, "OnStop method-> in MainActivity.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        void CreateNotificationChannel()
        {
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                {
                    // Notification channels are new in API 26 (and not a part of the
                    // support library). There is no need to create a notification 
                    // channel on older versions of Android.
                    return;
                }

                var channel = new NotificationChannel(CHANNEL_ID, "FCM Notifications", NotificationImportance.High)
                {
                    Description = "Firebase Cloud Messages appear in this channel"
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CreateNotificationChannel method-> in MainActivity.cs " + Settings.userLoginID);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            try
            {
                PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                for (int i = 0; i < permissions.Length; i++)
                {
                    global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                }

                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnRequestPermissionsResult method-> in MainActivity.cs " + Settings.userLoginID);
            }
        }

        private void initFontScale()
        {
            try
            {
                Configuration configuration = Resources.Configuration;
                configuration.FontScale = (float)1;
                //0.85 small, 1 standard, 1.15 big，1.3 more bigger ，1.45 supper big 
                DisplayMetrics metrics = new DisplayMetrics();
                WindowManager.DefaultDisplay.GetMetrics(metrics);
                metrics.ScaledDensity = configuration.FontScale * metrics.Density;
                BaseContext.Resources.UpdateConfiguration(configuration, metrics);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "initFontScale method-> in MainActivity.cs " + Settings.userLoginID);
            }
        }
    }
}