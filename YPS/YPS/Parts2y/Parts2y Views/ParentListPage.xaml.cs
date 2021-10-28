using Acr.UserDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.Parts2y.Parts2y_View_Models;
using Xamarin.Essentials;
using YPS.Views;
using System.Collections.ObjectModel;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ParentListPage : ContentPage
    {
        #region Data members declaration
        ParentListViewModel Vm;
        YPSService trackService = new YPSService();
        Xamarin.Forms.ListView listView;
        bool fromConstructor;
        bool reloadGrid;
        bool isloading;
        bool iscalled;
        public static Timer timer;
        public static Timer loadertimer;
        int lastButtonCount;
        public bool isLoaded { get; set; } = false;
        #endregion
        public ParentListPage()
        {
            try
            {
                trackService = new YPSService();

                YPSLogger.TrackEvent("MainPage", "Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

                InitializeComponent();

                BindingContext = Vm = new ParentListViewModel(Navigation, this);

                Settings.mutipleTimeClick = false;
                bool reloadGrid = true;

                lastButtonCount = 0;
                isloading = false;
                iscalled = false;
                Settings.currentPage = "MainPage";
                Settings.PerviousPage = "MainPage1";
                Settings.countmenu = 1;
                Settings.QAType = (int)QAType.PT;

                #region Assigning method that must execute when page is loaded, for binding gestures
                loadertimer = new System.Timers.Timer();
                loadertimer.Interval = 1000;
                loadertimer.Elapsed += CreateGestureWithCommands;
                loadertimer.AutoReset = false;
                loadertimer.Enabled = true;
                #endregion
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ParentListPage constructor -> in ParentListPage.cs " + Settings.userLoginID);
                Task.Run(() => trackService.Handleexception(ex)).Wait();
            }
        }


        /// <summary>
        /// This method gets called when page is appearing.
        /// </summary>
        protected async override void OnAppearing()
        {
            try
            {
                Vm.loadingindicator = true;

                base.OnAppearing();
                YPSLogger.TrackEvent("ParentListPage.xaml.cs", " in OnAppearing method" + DateTime.Now + " UserId: " + Settings.userLoginID);

                Settings.countmenu = 1;

                Settings.mutipleTimeClick = false;

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (Settings.refreshPage == 1)
                    {
                        Settings.refreshPage = 0;

                        if (Settings.IsFilterreset == true)
                        {
                            Settings.IsFilterreset = false;
                        }

                        if (Vm.AllTabVisibility == true)
                        {
                            await Vm.All_Tap();
                        }
                        else if (Vm.CompleteTabVisibility == true)
                        {
                            await Vm.Complete_Tap();
                        }
                        else if (Vm.InProgressTabVisibility == true)
                        {
                            await Vm.InProgress_Tap();
                        }
                        else
                        {
                            await Vm.Pending_Tap();
                        }
                    }
                    Task.Run(() => Vm.ShowHideSearFilterList(false));
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }

                Vm.loadingindicator = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in ParentListPage.xaml.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                Settings.ShowSuccessAlert = false;
                Vm.loadingindicator = false;
            }
            finally
            {
                Settings.mutipleTimeClick = false;
            }
        }

        #region Add gesture after page is loaded
        /// <summary>
        /// Gets called after 1 sec. of page getting loaded, for adding gestures with command to labels & stacks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void CreateGestureWithCommands(object sender, ElapsedEventArgs args)
        {
            try
            {
                loadertimer.Enabled = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    Namefilter.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.FilterClicked),
                    });

                    refreshName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await Vm.RefreshPage()),
                    });

                    searchEngineName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await Vm.ShowHideSearFilterList(true))
                    });
                });
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "CreateGestureWithCommands -> in ParentListPage.xaml.cs " + Settings.userLoginID);
                //UserDialogs.Instance.HideLoading();
                Vm.loadingindicator = false;
            }
            Vm.loadingindicator = false;
        }
        #endregion


        private async void HideSearchFilter(object sender, EventArgs e)
        {
            try
            {
                if (Vm.AllTabVisibility == true)
                {
                    await Vm.All_Tap();
                }
                else if (Vm.CompleteTabVisibility == true)
                {
                    await Vm.Complete_Tap();
                }
                else if (Vm.InProgressTabVisibility == true)
                {
                    await Vm.InProgress_Tap();
                }
                else
                {
                    await Vm.Pending_Tap();
                }

                Vm.IsSearchFilterListVisible = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HideSearchFilter method -> in ParentListPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }
    }
}