using System;
using System.Linq;
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
                YPSLogger.TrackEvent("NotificationListPage.xaml.cs", " Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

                service = new YPSService();
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (!string.IsNullOrEmpty(Settings.GetParamVal))
                    {
                        var navPages = Settings.GetParamVal.Split(';');
                        PNredirectionwhenTapPN();
                    }
                    else
                    {
                        BindingContext = vm = new NotificationViewModel(Navigation, this);
                    }
                }
                else
                {
                    BindingContext = vm = new NotificationViewModel(Navigation, this);
                }

                list.RefreshCommand = new Command(() =>
                {
                    Task.Run(() => getPNdata()).Wait();
                    list.IsRefreshing = false;
                });
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "NotificationListPage constructor -> in NotificationListPage.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method gets called when page is appearing.
        /// </summary>
        protected async override void OnAppearing()
        {
            try
            {
                YPSLogger.TrackEvent("NotificationListPage.xaml.cs", " OnAppearing " + DateTime.Now + " UserId: " + Settings.userLoginID);

                base.OnAppearing();

                vm.loadingindicator = true;

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    await getPNdata();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
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

                if (data?.Count() > 0)
                {
                    Settings.notifyCount = (int)data[0]?.listCount;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        clearAllLbl.IsVisible = (data?.Where(wr => wr.IsRead == false).FirstOrDefault()) != null ? true : false;
                    });
                    list.ItemsSource = data;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "getPNdata method -> in NotificationListPage.xaml.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("NotificationListPage.xaml.cs", " In NotifyCountlist_ItemTapped method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                vm.loadingindicator = true;

                vm.ReadNotificationHistory((e.Item as NotifyHistory).QAID);
                int type = (e.Item as NotifyHistory).NotificationType;
                list.SelectedItem = null;

                if (type != 4)
                {
                    if (string.IsNullOrEmpty((e.Item as NotifyHistory).TagNumber))
                    {
                        tgs = (e.Item as NotifyHistory).QAID > 0 ? "--" : "";
                    }
                    else
                    {
                        tgs = (e.Item as NotifyHistory).TagNumber;
                    }
                    Settings.QAType = (e.Item as NotifyHistory).QAType;
                    Settings.GetParamVal = "test;" + (e.Item as NotifyHistory).POID.ToString() + ";" + tgs + ";" + Settings.userLoginID + ";" +
                    (e.Item as NotifyHistory).QAID + ";" + (e.Item as NotifyHistory).Status + ";" + Settings.userRoleID + ";" + (e.Item as NotifyHistory).QATitle + ";" + (e.Item as NotifyHistory).QAType;
                    Settings.IsChatBackButtonVisible = true;
                    Navigation.PushAsync(new ChatPage(), false);
                }
                else
                {
                    DisplayAlert("Message", "You have been removed from " + " '" + (e.Item as NotifyHistory).QATitle + "' " + ", and cannnot view the conversation", "Ok");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NotifyCountlist_ItemTapped method -> in NotificationListPage.xaml.cs " + Settings.userLoginID);
                var xx = service.Handleexception(ex);
            }
            finally
            {
                Settings.IsChatBackButtonVisible = false;
                vm.loadingindicator = false;
            }
        }

        public async Task PNredirectionwhenTapPN()
        {
            try
            {
                Settings.IsChatBackButtonVisible = true;
                BindingContext = vm = new NotificationViewModel(Navigation, this);
                vm.loadingindicator = true;
                Navigation.PushAsync(new ChatPage(), false);
                Settings.GetParamVal = string.Empty;

            }
            catch (Exception ex)
            {
                vm.loadingindicator = false;
                YPSLogger.ReportException(ex, "PNredirectionwhenTapPN method -> in NotificationListPage.xaml.cs " + Settings.userLoginID);
                var ex1 = service.Handleexception(ex);
            }
            finally
            {
                Settings.IsChatBackButtonVisible = false;
                vm.loadingindicator = false;
            }
        }
    }
}