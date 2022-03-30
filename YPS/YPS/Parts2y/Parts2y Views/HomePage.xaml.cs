using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;
using YPS.Views;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        DashboardViewModel Vm;
        YPSService trackService;
        bool isswing;

        public HomePage()
        {
            try
            {
                BindingContext = Vm = new DashboardViewModel(Navigation, this);

                Vm.loadindicator = true;
                trackService = new YPSService();
                InitializeComponent();

                Vm.IsPNenable = Settings.IsPNEnabled;

                MessagingCenter.Subscribe<string, string>("PushNotificationCame", "IncreaseCount", (sender, args) =>
                {
                    Vm.NotifyCountTxt = args;
                    Task.Run(MoveBell);
                });

                MessagingCenter.Subscribe<string, string>("PushNotificationCame", "IncreaseJobCount", (sender, args) =>
                {
                    Vm.JobCountText = args;
                });
            }
            catch (Exception ex)
            {
                Task.Run(() => trackService.Handleexception(ex)).Wait();
                YPSLogger.ReportException(ex, "HomePage constructor-> in HomePage.xaml.cs " + Settings.userLoginID);
            }
            finally
            {
                Vm.loadindicator = false;
            }
        }

        public async void MoveBell()
        {
            try
            {
                if (Vm.NotifyCountTxt != null && Convert.ToInt32(Vm.NotifyCountTxt) > 0)
                {
                    isswing = true;

                    while (isswing)
                    {
                        await BellIcon.RotateTo(15, 300, Easing.Linear);
                        await BellIcon.RotateTo(-15, 300, Easing.Linear);
                    }
                }
                else
                {
                    isswing = false;
                    BellIcon.Rotation = 0;
                }
            }
            catch (Exception ex)
            {
                Task.Run(() => trackService.Handleexception(ex)).Wait();
                YPSLogger.ReportException(ex, "MoveBell method -> in HomePage.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method is called when clicked on notification icon and redirect to notification page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Notification_Tap(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;

                if (Vm.NotificationListCount > 0)
                {
                    await Navigation.PushAsync(new NotificationListPage(), false);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Notification", "No notification(s) available.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("No notification(s) available.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Notification_Tap method -> in HomePage.xaml.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                Vm.loadindicator = false;
            }
            Vm.loadindicator = false;
        }

        protected async override void OnAppearing()
        {
            try
            {
                YPSLogger.TrackEvent("HomePage.xaml.cs", " in OnAppearing method" + DateTime.Now + " UserId: " + Settings.userLoginID);
                base.OnAppearing();

                Vm.loadindicator = true;
                //await Task.Run(async () =>
                //{
                    if (Settings.CanCallForSettings == true)
                    {
                        Task.Run(async () => await CloudFolderKeyVal.GetToken()).Wait();
                        Settings.CanCallForSettings = false;
                    }

                    Vm.BgColor = Settings.Bar_Background;
                    //(((App.Current.MainPage) as MenuPage).Detail as NavigationPage).BarBackgroundColor = Vm.BgColor = Settings.Bar_Background;
                    (((App.Current.MainPage) as MenuPage).FindByName("contentpage") as ContentPage).BackgroundColor = Settings.Bar_Background;

                    Settings.ShowSuccessAlert = true;
                    Settings.countmenu = 1;
                    //await SecureStorage.SetAsync("mainPageisOn", "1");
                    //Task.Run(() => Vm.GetTaskData()).Wait();
                    Task.Run(() => Vm.RememberUserDetails()).Wait();
                    //Vm.GetActionStatus();
                    //Task.Run(() => Vm.GetallApplabels()).Wait();
                    Vm.ChangeLabel();
                    Vm.GetQuestions();
                    Vm.GetCounts();
                    //Vm.GetPNCount();
                //});
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in HomePage.xaml.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                Vm.loadindicator = false;
            }
            finally
            {
                Vm.loadindicator = false;
            }
        }

        protected async override void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();
                isswing = false;
                Settings.CanCallForSettings = true;
                //await SecureStorage.SetAsync("mainPageisOn", "0");
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnDisappearing method -> in HomePage.xaml.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }
            
        private async void NavigationClick(object sender, EventArgs e)
        {
            try
            {
                ((App.Current.MainPage) as MenuPage).IsPresented = !((App.Current.MainPage) as MenuPage).IsPresented;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NavigationClick method -> in HomePage.xaml.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }


    }
}