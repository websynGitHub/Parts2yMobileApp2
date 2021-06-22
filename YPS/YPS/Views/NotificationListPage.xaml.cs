using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotificationListPage : ContentPage
    {
        #region Data member declaration
        NotificationViewModel vm;
        YPSService service;
        string tgs;
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public NotificationListPage()
        {
            try
            {
                InitializeComponent();
                YPSLogger.TrackEvent("NotificationListPage", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

                service = new YPSService();
                BindingContext = vm = new NotificationViewModel(Navigation, this);

                list.RefreshCommand = new Command(() =>
                {
                    Task.Run(() => getPNdata()).Wait();
                    list.IsRefreshing = false;
                });
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "NotificationListPage Constructor -> in NotificationListPage.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method gets called when page is appearing.
        /// </summary>
        protected async override void OnAppearing()
        {
            try
            {
                YPSLogger.TrackEvent("NotificationListPage", "OnAppearing " + DateTime.Now + " UserId: " + Settings.userLoginID);

                base.OnAppearing();

                vm.loadingindicator = true;

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    await getPNdata();
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in NotificationListPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                vm.loadingindicator = false;
            }
        }

        /// <summary>
        /// This method gets PN data.
        /// </summary>
        public async Task getPNdata()
        {
            try
            {
                vm.loadingindicator = true;
                var data = await vm.GetNotificationHistory();
                list.ItemsSource = data;
                clearAllLbl.IsVisible = data?.Count > 0 ? true : false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "getPNdata method -> in NotificationListPage.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                vm.loadingindicator = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on any item from notification list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyCountlist_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            YPSLogger.TrackEvent("ChatUsers", " In NotifyCountlist_ItemTapped method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                vm.loadingindicator = true;

                vm.ReadNotificationHistory((e.Item as NotifyHistory).QAID);
                int type = (e.Item as NotifyHistory).NotificationType;
                list.SelectedItem = null;

                if (type != 4)
                {
                    (e.Item as NotifyHistory).SelectedPNBorderColor = vm.BgColor;

                    if (string.IsNullOrEmpty((e.Item as NotifyHistory).TagNumber))
                    {
                        tgs = "--";
                    }
                    else
                    {
                        tgs = (e.Item as NotifyHistory).TagNumber;
                    }
                    Settings.QAType = (e.Item as NotifyHistory).QAType;
                    Settings.GetParamVal = "test;" + (e.Item as NotifyHistory).POID.ToString() + ";" + tgs + ";" + Settings.userLoginID + ";" +
                    (e.Item as NotifyHistory).QAID + ";" + (e.Item as NotifyHistory).Status + ";" + Settings.userRoleID + ";" + (e.Item as NotifyHistory).QATitle + ";" + (e.Item as NotifyHistory).QAType;
                    Settings.IsChatBackButtonVisible = true;
                    Navigation.PushAsync(new ChatPage());
                }
                else
                {
                    DisplayAlert("Message", "You have been removed from " + " '" + (e.Item as NotifyHistory).QATitle + "' " + ", and cannnot view the conversation", "Ok");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NotifyCountlist_ItemTapped method -> in NotificationListPage.cs " + Settings.userLoginID);
                var xx = service.Handleexception(ex);
            }
            finally
            {
                Settings.IsChatBackButtonVisible = false;
                vm.loadingindicator = false;
            }
        }
    }
}