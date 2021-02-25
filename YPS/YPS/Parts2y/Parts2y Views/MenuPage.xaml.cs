using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.CustomToastMsg;
using YPS.Service;
using YPS.Views;
using YPS.CommonClasses;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : MasterDetailPage
    {
        #region Data member declaration
        bool avoidMutipleTimeOpenAlert = false;
        YPSService service;
        #endregion

        public MenuPage(Type PageName = null)
        {
            try
            {
                InitializeComponent();
                if (PageName != null)
                {
                    Page displayPage = (Page)Activator.CreateInstance(PageName);
                    this.MasterBehavior = MasterBehavior.Popover;

                    //if (PageName.Name == "ProfileSelectionPage")
                    //{
                    //    Detail = new NavigationPage(new ProfileSelectionPage((int)QAType.PT))
                    //    {
                    //        //BackgroundColor = Color.FromHex("#0d0d0d"),
                    //        BarBackgroundColor = Color.Green,
                    //        BarTextColor = Color.White
                    //    };
                    //}
                    //else if (PageName.Name == "QnAlistPage")
                    //{
                    //    Detail = new NavigationPage(new QnAlistPage(0, 0, (int)QAType.PT))
                    //    {
                    //        //BackgroundColor = Color.FromHex("#0d0d0d"),
                    //        BarBackgroundColor = Color.Green,
                    //        BarTextColor = Color.White
                    //    };
                    //}
                    //else
                    //{
                    Detail = new NavigationPage(displayPage)
                    {
                        //BackgroundColor = Color.FromHex("#0d0d0d"),
                        BarBackgroundColor = Color.DarkGreen,
                        BarTextColor = Color.White
                    };
                    //}
                }
                else
                {
                    Detail = new NavigationPage(new HomePage())
                    {
                        //BackgroundColor = Color.FromHex("#0d0d0d"),
                        BarBackgroundColor = Color.DarkGreen,
                        BarTextColor = Color.White,
                    };
                }
                menuList.ItemTapped += (sender, e) => menuList.SelectedItem = null;
            }
            catch (Exception ex)
            {

            }

        }

        private async void MenuItems_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    string moveToPage = (e.Item as MenuList).Title;

                    if (moveToPage == "Home")
                    {
                        //if (Settings.roleid == 1)
                        //{
                        App.Current.MainPage = new MenuPage(typeof(HomePage));
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
                    else if (moveToPage == "Archive")
                    {
                        if (Detail.Navigation.ModalStack.Count == 0 ||
             Detail.Navigation.ModalStack.Last().GetType() != typeof(QnAlistPage))
                        {
                            await Detail.Navigation.PushModalAsync(new QnAlistPage(0, 0, (int)QAType.PT));
                        }
                        this.MasterBehavior = MasterBehavior.Popover;
                        IsPresented = false;
                    }
                    else if (moveToPage == "Settings")
                    {
                        //App.Current.MainPage = new MenuPage(typeof(ProfileSelectionPage));
                        if (Detail.Navigation.ModalStack.Count == 0 ||
            Detail.Navigation.ModalStack.Last().GetType() != typeof(ProfileSelectionPage))
                        {
                            await Detail.Navigation.PushModalAsync(new ProfileSelectionPage((int)QAType.PT));
                        }
                        this.MasterBehavior = MasterBehavior.Popover;
                        IsPresented = false;
                    }
                    else if (moveToPage == "Logout")
                    {
                        if (!avoidMutipleTimeOpenAlert)
                        {
                            avoidMutipleTimeOpenAlert = true;
                            var answer = await DisplayAlert("Confirm", "Are you sure you want to logout?", "Yes", "No");
                            if (answer)
                            {
                                try
                                {

                                    CloudFolderKeyVal.Appredirectlogin("Your session token expired, please login", false);
                                    //var Logoutresult = await service.LogoutService();
                                    //Settings.Username = Settings.UserMail =
                                    //Settings.SGivenName = Settings.EntityName = Settings.RoleName = Settings.access_token = Settings.JobSelected =
                                    //Settings.BlobConnection = Settings.BlobStorageConnectionString =
                                    //Settings.CompanySelected = Settings.LoginID = Settings.SupplierSelected = Settings.Projectelected = string.Empty;
                                    //Settings.SupplierID = Settings.ProjectID = Settings.CompanyID = Settings.JobID =
                                    //Settings.CompressionQuality = Settings.PhotoSize = Settings.userLoginID = Settings.userRoleID = 0;
                                    //RememberPwdDB Db = new RememberPwdDB();
                                    //var user = Db.GetUserDetails().FirstOrDefault();
                                    //if (user != null)
                                    //{
                                    //    Db.ClearUserDetail(user.UserId);
                                    //}
                                    //DependencyService.Get<ISQLite>().deleteAllPNdata();
                                    ////SecureStorage.Remove("userName");
                                    ////SecureStorage.Remove("LoginID");
                                    ////SecureStorage.Remove("userID");
                                    //App.Current.MainPage = new NavigationPage(new LoginPage());
                                    //if (Settings.alllabeslvalues != null)
                                    //{
                                    //    Settings.alllabeslvalues.Clear();
                                    //}
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            avoidMutipleTimeOpenAlert = false;
                        }
                    }
                    else if (moveToPage == "About")
                    {

                        CommonClasses.Settings.PerviousPage = "AboutPage";
                        CommonClasses.Settings.CheckQnAClose = true;
                        if (Detail.Navigation.ModalStack.Count == 0 ||
           Detail.Navigation.ModalStack.Last().GetType() != typeof(YPS.Views.AboutPage))
                        {
                            await Detail.Navigation.PushModalAsync(new YPS.Views.AboutPage());
                        }
                        this.MasterBehavior = MasterBehavior.Popover;
                        IsPresented = false;
                    }
                    else
                    {
                        App.Current.MainPage = new MenuPage(typeof(MainPage));
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}