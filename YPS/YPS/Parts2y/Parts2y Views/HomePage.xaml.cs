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
using YPS.Parts2y.Parts2y_Models;
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
                InitializeComponent();
                trackService = new YPSService();
                BindingContext = Vm = new DashboardViewModel(Navigation);

                Vm.IsPNenable = Settings.IsPNEnabled;

                if (Settings.userRoleID == (int)UserRoles.SuperAdmin)
                {
                    Vm.IsPNenable = false;
                }

                MessagingCenter.Subscribe<string, string>("PushNotificationCame", "IncreaseCount", (sender, args) =>
                {
                    Vm.NotifyCountTxt = args;
                });

                Task.Run(() => Vm.RememberUserDetails()).Wait();
                Task.Run(() => Vm.GetallApplabels()).Wait();
                Task.Run(() => Vm.ChangeLabel()).Wait();
            }
            catch (Exception ex)
            {

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
                if (Vm.NotifyCountTxt == "0")
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
                YPSLogger.ReportException(ex, "Notification_Tap method -> in HomePage.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                Vm.loadindicator = false;
                //UserDialogs.Instance.HideLoading();
            }
            Vm.loadindicator = false;
        }

        protected async override void OnAppearing()
        {
            try
            {
                Settings.ShowSuccessAlert = true;

                base.OnAppearing();
                YPSLogger.TrackEvent("MainPage", "OnAppearing " + DateTime.Now + " UserId: " + Settings.userLoginID);
                //await SecureStorage.SetAsync("mainPageisOn", "1");

                Settings.countmenu = 1;

                /// Get PN count
                Vm.GetPNCount();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                Vm.loadindicator = false;
            }
            finally
            {
                Vm.loadindicator = false;
            }
        }
    }
}