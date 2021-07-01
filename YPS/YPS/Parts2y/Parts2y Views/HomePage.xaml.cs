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
        public HomePage()
        {
            try
            {
                BindingContext = Vm = new DashboardViewModel(Navigation);

                Vm.loadindicator = true;
                trackService = new YPSService();
                InitializeComponent();

                Vm.IsPNenable = Settings.IsPNEnabled;

                MessagingCenter.Subscribe<string, string>("PushNotificationCame", "IncreaseCount", (sender, args) =>
                {
                    Vm.NotifyCountTxt = args;
                });

                MessagingCenter.Subscribe<string, string>("PushNotificationCame", "IncreaseJobCount", (sender, args) =>
                {
                    Vm.JobCountText = args;
                });
            }
            catch (Exception ex)
            {
                Task.Run(() => trackService.Handleexception(ex)).Wait();
                YPSLogger.ReportException(ex, "HomePage Constructor-> in HomePage.xaml.cs " + Settings.userLoginID);
            }
            finally
            {
                Vm.loadindicator = false;
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
                if (string.IsNullOrEmpty(Vm.NotifyCountTxt) || Vm.NotifyCountTxt == "0")
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("There is no new notification.");
                }
                else
                {
                    await Navigation.PushAsync(new NotificationListPage());
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
                Vm.loadindicator = true;
                Settings.ShowSuccessAlert = true;

                base.OnAppearing();
                YPSLogger.TrackEvent("MainPage", "OnAppearing " + DateTime.Now + " UserId: " + Settings.userLoginID);

                Settings.countmenu = 1;
                await SecureStorage.SetAsync("mainPageisOn", "1");
                Vm.GetPNCount();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in MainPage.xaml.cs " + Settings.userLoginID);
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
            base.OnDisappearing();
            await SecureStorage.SetAsync("mainPageisOn", "0");
        }
    }
}