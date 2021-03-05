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
                    Detail = new NavigationPage(displayPage)
                    {
                        //BackgroundColor = Color.FromHex("#0d0d0d"),
                        //BackgroundColor = YPS.CommonClasses.Settings.Bar_Background,
                        BarBackgroundColor = YPS.CommonClasses.Settings.Bar_Background,
                        BarTextColor = Color.White
                    };
                }
                else
                {
                    Detail = new NavigationPage(new HomePage())
                    {
                        //BackgroundColor = Color.FromHex("#0d0d0d"),
                        //BackgroundColor = YPS.CommonClasses.Settings.Bar_Background,
                        BarBackgroundColor = YPS.CommonClasses.Settings.Bar_Background,
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
                        App.Current.MainPage = new MenuPage(typeof(HomePage));
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