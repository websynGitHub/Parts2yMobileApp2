using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_SQLITE;

namespace YPS.Parts2y.Parts2y_Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MenuPage : MasterDetailPage
	{
		public MenuPage (Type PageName = null)
		{
			InitializeComponent ();
            if (PageName != null)
            {
                Page displayPage = (Page)Activator.CreateInstance(PageName);
                //this.MasterBehavior = MasterBehavior.Popover;
                Detail = new NavigationPage(displayPage)
                {
                    //BackgroundColor = Color.FromHex("#0d0d0d"),
                    BarBackgroundColor = Settings.Bar_Background,
                    BarTextColor = Color.White
                };
            }
            else
            {
                Detail = new NavigationPage(new MainPage())
                {
                    //BackgroundColor = Color.FromHex("#0d0d0d"),
                    BarBackgroundColor = Color.FromHex("#000000"),
                    BarTextColor = Color.White,
                };
            }
            menuList.ItemTapped += (sender, e) => menuList.SelectedItem = null;
        }

        private async void MenuItems_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                string moveToPage = (e.Item as MenuList).Title;
                if (moveToPage == "Dashboard")
                {
                    //if (Settings.roleid == 1)
                    //{
                        App.Current.MainPage = new MenuPage(typeof(Dashboard));
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
                else if (moveToPage == "Home")
                {
                    if (Settings.roleid == 1)
                    {
                        App.Current.MainPage = new MenuPage(typeof(Driverpage));
                    }
                    else if (Settings.roleid == 2)
                    {
                        App.Current.MainPage = new MenuPage(typeof(HomePage));
                    }
                    else if (Settings.roleid == 3)
                    {
                        App.Current.MainPage = new MenuPage(typeof(DealerPage));
                    }
                }
                else if (moveToPage == "Logout")
                {
                    RememberPwdDB Db = new RememberPwdDB();
                    var user = Db.GetUserDetails().FirstOrDefault();
                    
                    if (user != null)
                    {
                        Db.ClearUserDetail(user.ID);
                    }
                    App.Current.MainPage = new NavigationPage(new LoginPage());
                }
                else if (moveToPage == "About")
                {
                   
                        if (Detail.Navigation.ModalStack.Count == 0 ||
            Detail.Navigation.ModalStack.Last().GetType() != typeof(AboutPage))
                        {
                            await Detail.Navigation.PushModalAsync(new AboutPage());
                        }
                }
                //else if(moveToPage== "TransportReportDetails")
                //{
                //    App.Current.MainPage = new MenuPage(typeof(TransportReportDetails));

                //}
                else
                {
                    App.Current.MainPage = new MenuPage(typeof(MainPage));

                }
            }
            catch(Exception ex)
            {

            }

        }
    }
}