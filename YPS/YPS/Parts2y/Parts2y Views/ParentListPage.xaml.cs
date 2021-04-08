using Acr.UserDialogs;
using Syncfusion.SfDataGrid.XForms;
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
using Syncfusion.SfDataGrid.XForms.DataPager;
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
        public List<GridColumn> columns, finalcoumns;
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
                InitializeComponent();

                BindingContext = Vm = new ParentListViewModel(Navigation, this);


                if (Settings.VersionID == 5 || Settings.VersionID == 1)
                {
                    Vm.IsLoadTabVisible = false;
                }

                if (Device.RuntimePlatform == Device.iOS)// for adjusting the display as per the notch
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 30;
                    headerpart.Padding = safeAreaInset;
                }

                Settings.mutipleTimeClick = false;
                bool reloadGrid = true;

                //Task.Run(() => PageSizeDDLBinding()).Wait();
                //dataPager.PageIndexChanged += pageIndexChanged;

                lastButtonCount = 0;
                isloading = false;
                iscalled = false;
                YPSLogger.TrackEvent("MainPage", "Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "MainPage";
                Settings.PerviousPage = "MainPage1";
                Settings.countmenu = 1;
                Settings.QAType = (int)QAType.PT;
                //Vm.IsPNenable = Settings.IsPNEnabled;

                trackService = new YPSService();


                //if (Settings.userRoleID == (int)UserRoles.SuperAdmin)
                //{
                //    //imgCamera.Opacity = imgChat.Opacity = imgFileUpload.Opacity = imgPrinter.Opacity = 0.5;
                //    //CameraLbl.Opacity = ChatLbl.Opacity = FileUploadLbl.Opacity = PrinterLbl.Opacity = 0.5;
                //    //stckCamera.GestureRecognizers.Clear();
                //    //stckFileUpload.GestureRecognizers.Clear();
                //    //stckChat.GestureRecognizers.Clear();
                //    //stckPrinter.GestureRecognizers.Clear();
                //    Vm.IsPNenable = false;
                //}


                //else if (Settings.userRoleID == (int)UserRoles.CustomerAdmin || Settings.userRoleID == (int)UserRoles.CustomerUser)
                //{
                //    picRequiredStk1.IsVisible = picRequiredStk2.IsVisible = true;
                //}

                //if (Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                //{
                //    imgPrinter.Opacity = 0.5;
                //    PrinterLbl.Opacity = 0.5;
                //    stckPrinter.GestureRecognizers.Clear();
                //}


                //if (Settings.userRoleID == (int)UserRoles.OwnerAdmin)
                //{
                //    Vm.Archivedchat = true;
                //}
                //else
                //{
                //    //SetHeightOfFrame.HeightRequest = 100;
                //}

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

            }
        }

        /// <summary>
        /// This method gets called when page index is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void pageIndexChanged(object sender, PageIndexChangedEventArgs args)
        {
            //try
            //{
            //    if (!Settings.mutipleTimeClick)
            //    {
            //        dataPager.PageIndexChanged -= pageIndexChanged;
            //        Settings.mutipleTimeClick = true;

            //        //if (args.NewPageIndex != args.OldPageIndex && reloadGrid == true)
            //        //{
            //        Settings.startPageYPS = args.NewPageIndex * Settings.pageSizeYPS;
            //        Settings.toGoPageIndex = args.NewPageIndex;
            //        await Vm.BindGridData(false, false);
            //        //}
            //        //else
            //        //{
            //        //    reloadGrid = true;
            //        //    Settings.isSkip = false;
            //        //}

            //        Vm.loadingindicator = false;
            //        Settings.mutipleTimeClick = false;
            //        dataPager.PageIndexChanged += pageIndexChanged;
            //        UserDialogs.Instance.HideLoading();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    trackService.Handleexception(ex);
            //    YPSLogger.ReportException(ex, "pageIndexChanged method -> in Main Page.cs " + Settings.userLoginID);
            //    UserDialogs.Instance.HideLoading();
            //}
        }

        /// <summary>
        /// This method is to set the values to pagesize DDL, set start page & select the button when page is loaded.
        /// </summary>
        public void PageSizeDDLBinding()
        {
            try
            {
                //Device.BeginInvokeOnMainThread(() => {
                //    //PageSize = new Picker()
                //    //{
                //    //    WidthRequest = 0,
                //    //    HorizontalOptions = LayoutOptions.FillAndExpand,
                //    //    VerticalOptions = LayoutOptions.CenterAndExpand,
                //    //    IsVisible = false,
                //    //    SelectedIndex = 0,
                //    //    TextColor = Color.Black
                //    //};

                //    PageSize = new Xamarin.Forms.Picker();

                //    PageSize = new Xamarin.Forms.Picker
                //    {
                //        WidthRequest = 0,
                //        HorizontalOptions = LayoutOptions.FillAndExpand,
                //        VerticalOptions = LayoutOptions.CenterAndExpand,
                //        IsVisible = false,
                //        SelectedIndex = 0,
                //        TextColor = Color.Black
                //    };

                //    pageSizerPickerGrid.Children.Add(PageSize);
                //    PageSize.SelectedIndexChanged += PageSizeChanged;
                //    List<int> listpagesize = new List<int>(new int[] { 5, 10, 20, 50, 100 });
                //    PageSize.ItemsSource = listpagesize;
                //    reloadGrid = false;

                //    Settings.startPageYPS = 0;
                //    Settings.toGoPageIndex = 0;
                //    PageSize.SelectedIndex = 1;
                //});
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "PageSizeDDLBinding -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method get called when page size is selected from pagesize DDL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void PageSizeChanged(object sender, EventArgs args)
        {
            //try
            //{
            //    PageSize.SelectedIndexChanged -= PageSizeChanged;

            //    if (!Settings.mutipleTimeClick)
            //    {
            //        Settings.mutipleTimeClick = true;
            //        var senderobj = sender as Xamarin.Forms.Picker;
            //        Settings.pageSizeYPS = (int)senderobj.SelectedItem;
            //        pageSizeNameYPS.Text = Convert.ToString(senderobj.SelectedItem);
            //        Settings.selectedIndexYPS = senderobj.SelectedIndex;

            //        if (reloadGrid == true)
            //        {
            //            reloadGrid = false;
            //            Settings.startPageYPS = 0;
            //            Settings.toGoPageIndex = 0;
            //            lastButtonCount = dataPager.NumericButtonCount;

            //            dataPager.MoveToPage(0);
            //            await Vm.BindGridData(false, false);
            //        }

            //        //Vm.loadingindicator = false;
            //        //UserDialogs.Instance.HideLoading();
            //        Settings.mutipleTimeClick = false;
            //    }
            //    PageSize.SelectedIndexChanged += PageSizeChanged;
            //}
            //catch (Exception ex)
            //{
            //    trackService.Handleexception(ex);
            //    YPSLogger.ReportException(ex, "PageSizeChanged -> in Main Page.cs " + Settings.userLoginID);
            //    //UserDialogs.Instance.HideLoading();
            //}
        }

        private void MoreItems_Tapped(object sender, EventArgs e)
        {
            try
            {
                Vm.CheckToolBarPopUpHideOrShow();
            }
            catch (Exception ex)
            {

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
                YPSLogger.TrackEvent("ParentListPage", "OnAppearing " + DateTime.Now + " UserId: " + Settings.userLoginID);

                Settings.countmenu = 1;

                Settings.mutipleTimeClick = false;

                if (Settings.refreshPage == 1)
                {
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        Settings.refreshPage = 0;

                        if (Settings.IsFilterreset == true)
                        {
                            Settings.IsFilterreset = false;
                            //Task.Run(() => PageSizeDDLBinding()).Wait();
                            //dataPager.PageIndexChanged += pageIndexChanged;
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
                        //await Vm.BindGridData(false, false,-1);
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                Vm.loadingindicator = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in ParentListPage.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                //UserDialogs.Instance.HideLoading();
                Settings.ShowSuccessAlert = false;
                Vm.loadingindicator = false;
            }
            finally
            {
                Settings.mutipleTimeClick = false;
                //UserDialogs.Instance.HideLoading();
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

                });
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "OnTimedEvent -> in MainPage.cs " + Settings.userLoginID);
                //UserDialogs.Instance.HideLoading();
                Vm.loadingindicator = false;
            }
            Vm.loadingindicator = false;
        }
        #endregion

        /// <summary>
        /// This method is called when leaving the page.
        /// </summary>
        protected override async void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();
            }
            catch (Exception ex)
            {
            }
        }
    }
}