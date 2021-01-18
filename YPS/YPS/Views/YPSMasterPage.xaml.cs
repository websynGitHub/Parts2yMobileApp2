using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YPSMasterPage : MasterDetailPage
    {
        #region Data member declaration
        bool avoidMutipleTimeOpenAlert = false;
        YPSService service;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="PageName"></param>
        public YPSMasterPage(Type PageName = null)
        {
            InitializeComponent();
            service = new YPSService();
            YPSLogger.TrackEvent("YPSMasterPage", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
            
            try
            {
                if (PageName != null)
                {
                    Page displayPage = (Page)Activator.CreateInstance(PageName);
                    this.MasterBehavior = MasterBehavior.Popover;
                    
                    Detail = new NavigationPage(displayPage)
                    {
                        BackgroundColor = Color.FromHex("#0d0d0d"),
                        BarBackgroundColor = Color.FromHex("#0d0d0d"),
                        BarTextColor = Color.White
                    };
                }
                else
                {
                    Detail = new NavigationPage(new MainPage())
                    {
                        BackgroundColor = Color.FromHex("#0d0d0d"),
                        BarBackgroundColor = Color.FromHex("#0d0d0d"),
                        BarTextColor = Color.White,
                    };
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YPSMasterPage  constructor -> in YPSMasterPage.cs " + Settings.userLoginID);
            }
            menulist.ItemTapped += (sender, e) => menulist.SelectedItem = null;
        }

        /// <summary>
        /// Gets called when clicked any item from menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuItems_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            YPSLogger.TrackEvent("YPSMasterPage", "MenuItems_ItemTapped " + DateTime.Now + " UserId: " + Settings.userLoginID);
            string moveToPage = (e.Item as MenuList).Title;
            
            if (moveToPage == "Home")
            {
                Settings.PerviousPage = "Home";
                Settings.CheckQnAClose = false;
                App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
            }
            else if (moveToPage == "yShip")
            {
                Settings.PerviousPage = "yship";
                Settings.CheckQnAClose = true;
                this.MasterBehavior = MasterBehavior.Popover;
                App.Current.MainPage = new YPSMasterPage(typeof(YshipPage));
            }
            else if (moveToPage == "About")
            {
                Settings.PerviousPage = "AboutPage";
                Settings.CheckQnAClose = true;
                this.MasterBehavior = MasterBehavior.Popover;
                App.Current.MainPage = new YPSMasterPage(typeof(AboutPage));
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

                            CloudFolderKeyVal.Appredirectlogin("Your session token expired, please login",false);
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
            //else if(moveToPage == "Guided Tour")
            //{
            //    Settings.CheckPage = "Guided Tour";
            //    App.Current.MainPage = new YPSMasterPage(typeof(CoachMAndGuidedTourP));
            //}
            //else if (moveToPage == "Mobile Apps")
            //{
            //    App.Current.MainPage = new YPSMasterPage(typeof(MobileBuildsPage));
            //}
            //else if (moveToPage == "RFID")
            //{
            //    App.Current.MainPage = new YPSMasterPage(typeof(RFID));
            //}
        }
    }
}