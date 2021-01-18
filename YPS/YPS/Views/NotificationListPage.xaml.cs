using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
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
                BindingContext = vm = new NotificationViewModel();
               
                list.RefreshCommand = new Command(() =>
                {
                    getPNdata();
                    list.IsRefreshing = false;
                });
                getPNdata();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "NotificationListPage Constructor -> in NotificationListPage.cs " + Settings.userLoginID);

            }
        }

        /// <summary>
        /// This method gets PN data.
        /// </summary>
        public async void getPNdata()
        {
            try
            {
                var data = await vm.GetNotificationHistory();
                list.ItemsSource = data;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "getPNdata method -> in NotificationListPage.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
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
                vm.ReadNotificationHistory((e.Item as NotifyHistory).QAID);
                int type = (e.Item as NotifyHistory).NotificationType;
                list.SelectedItem = null;
               
                if (type != 4)
                {
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
                    Navigation.PushAsync(new ChatPage());
                }
                else
                {
                    DisplayAlert("Message", "You removed from " + " '" + (e.Item as NotifyHistory).QATitle + "' " + ", Can not see previous conversation", "OK");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NotifyCountlist_ItemTapped method -> in NotificationListPage.cs " + Settings.userLoginID);
                var xx = service.Handleexception(ex);
            }
        }
    }
}