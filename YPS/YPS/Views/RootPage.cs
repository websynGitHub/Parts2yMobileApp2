using YPS.Views.Menu;
using System;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using System.Linq;
using YPS.Service;
using YPS.Model;
using Plugin.DeviceInfo;
using Plugin.SecureStorage;

namespace YPS.Views
{
    public class RootPage : MasterDetailPage
    {
        MenuPage menuPage;
        public RootPage(Type PageName = null)
        {
            try
            {
                Settings.currentPage = "RootPage";
                menuPage = new MenuPage();
                menuPage.Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as YPS.Views.Menu.MenuItem);
                Master = menuPage;
                this.MasterBehavior = MasterBehavior.Popover;
                MessagingCenter.Subscribe<string>("Popup Master", "msg", (sender) =>
                {

                    IsPresented = true;
                    // do something whenever the "Hi" message is sent
                });

                if (PageName != null)
                {
                    Page displayPage = (Page)Activator.CreateInstance(PageName);

                    Detail = new NavigationPage(displayPage)
                    {
                        BackgroundColor = BackColor(),
                        BarBackgroundColor = BackColor(),
                        BarTextColor = Color.White
                    };
                }
                else
                {
                    Detail = new NavigationPage(new MainPage())
                    {
                        BackgroundColor = BackColor(),
                        BarBackgroundColor = BackColor(),
                        BarTextColor = Color.White,
                    };
                }
            }
            catch(Exception ex)
            {

            }
   }

        public Color BackColor()
        {
            return Color.FromHex("#0d0d0d");
        }
        bool avoidMutipleTimeOpenAlert = false;
        async void NavigateTo(YPS.Views.Menu.MenuItem menu)
        {
            try
            {
                menuPage.Menu.SelectedItem = null;
                var conn = await App.CheckInterNetConnection();
                if(conn)
                {
                    if (menu == null)
                        return;
                    if (menu.Title == "Settings")
                        return;
                    if (menu.Title == "Logout")
                    {
                        if(!avoidMutipleTimeOpenAlert)
                        {
                            avoidMutipleTimeOpenAlert = true;
                            var answer = await DisplayAlert("Confirm", "Are you sure you want to logout?", "Yes", "No");
                            if (answer)
                            {
                                Settings.countmenu = 1;

                                //DeviceRegistration dr = new DeviceRegistration();
                                //dr.UserId = Settings.userLoginID;
                                //// dr.LoginKey = Settings.loginkey;
                                //dr.Platform = (CrossDeviceInfo.Current.Platform.ToString() == "Android") ? "gcm" : "apns";
                                //string[] arr = new string[] { "Hai" };
                                //dr.Tags = arr;
                                //dr.HubRegistrationId = "";
                                //dr.FireBasedToken = CrossSecureStorage.Current.GetValue("Token");

                                //var service = new YPSService();
                                //await service.DeleteRegistrationId(dr);

                                //dr.PlatformHandle = Settings.DeviceToken.Trim();
                                //bool NetConnectionState = await App.CheckInterNetConnection();
                                //if (NetConnectionState)
                                //{

                                //    if (!string.IsNullOrEmpty(dr.PlatformHandle))
                                //    {
                                //        await service.DeleteRegistrationId(dr);
                                //        //await service.RegisterNotification(dr);
                                //    }

                                //}
                                //else
                                //{
                                //    await App.Current.MainPage.DisplayAlert("Alert", "Please check your internet connection", "Ok");
                                //}
                                //// UserDialogs.Instance.ShowLoading("Logging out...");
                                //var properties = App.Current.Properties;
                                //properties["txtUsername"] = string.Empty;
                                //properties["txtPassword"] = string.Empty;
                                //properties["OfflineLoginKey"] = null;
                                //properties["OfflineUserId"] = string.Empty;
                                //properties["Offlinecompanykey"] = string.Empty;
                                //properties["Offlinelastlogindate"] = string.Empty;
                                //properties["Offlineemail"] = string.Empty;
                                //properties["OfflineIsSupervisor"] = string.Empty;
                                //properties["Offlineaccess_token"] = string.Empty;
                                //Settings.ResetSettings();


                                //if (Application.Current.Properties.ContainsKey("admin"))
                                //{
                                //    App.Current.MainPage = new NavigationPage(new LoginPage());
                                //    Application.Current.Properties.Remove("admin");
                                //}
                                //else if (Application.Current.Properties.ContainsKey("staf"))
                                //{
                                //    App.Current.MainPage = new NavigationPage(new LoginPage());
                                //    Application.Current.Properties.Remove("staf");
                                //}
                                RememberPwdDB Db = new RememberPwdDB();
                                var user = Db.GetUserDetails().FirstOrDefault();

                                Db.ClearUserDetail(user.UserId);
                                DependencyService.Get<ISQLite>().deleteAllPNdata();

                                CrossSecureStorage.Current.DeleteKey("userName");
                                // var data = Db.GetUserDetails();
                                App.Current.MainPage = new NavigationPage(new LoginPage());
                            }
                            avoidMutipleTimeOpenAlert = false;
                        }
                       
                    }
                    else
                    {
                        Page displayPage = (Page)Activator.CreateInstance(menu.TargetType);
                        Detail = new NavigationPage(displayPage)
                        {
                            BackgroundColor = BackColor(),
                            BarBackgroundColor = BackColor(),
                            BarTextColor = Color.White
                        };
                    }
                    menuPage.Menu.SelectedItem = null;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                }
                
                IsPresented = false;
            }
            catch(Exception ex)
            {

            }
        }
       
    }
}
