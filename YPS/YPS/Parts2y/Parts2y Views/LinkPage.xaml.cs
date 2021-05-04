using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LinkPage : ContentPage
    {
        #region Data members declaration
        LinkPageViewModel Vm;
        YPSService yPSService;
        bool checkInternet;
        SendPodata sendPodata = new SendPodata();
        #endregion

        public LinkPage(ObservableCollection<PhotoRepoDBModel> photorepolist)
        {
            try
            {
                yPSService = new YPSService();
                InitializeComponent();

                BindingContext = Vm = new LinkPageViewModel(Navigation, photorepolist, this);

                //if (Device.RuntimePlatform == Device.iOS)// for adjusting the display as per the notch
                //{
                //    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                //    safeAreaInset.Bottom = 0;
                //    safeAreaInset.Top = 20;
                //    headerpart.Padding = safeAreaInset;
                //}
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LinkPage constructor -> in LinkPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => yPSService.Handleexception(ex)).Wait();
                Vm.IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when page is appearing.
        /// </summary>
        protected async override void OnAppearing()
        {
            YPSLogger.TrackEvent("LinkPage.xaml.cs", "OnAppearing method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                base.OnAppearing();

                if (Vm.updateList == true)
                {
                    Settings.CanOpenScanner = false;
                    await Vm.ShowContentsToLink();
                    Vm.updateList = false;
                }

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing Method -> in LinkPage.xaml.cs  " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on the back button and redirect to previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                Vm.IndicatorVisibility = true;
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in LinkPage.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
                Vm.IndicatorVisibility = false;
            }
            Vm.IndicatorVisibility = false;
        }

        private async Task<ObservableCollection<AllPoData>> GetUpdatedAllPOData()
        {
            ObservableCollection<AllPoData> AllPoDataList = new ObservableCollection<AllPoData>();

            try
            {
                Vm.IndicatorVisibility = true;
                YPSLogger.TrackEvent("LinkPage.xaml.cs", "in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {

                    sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;

                    var result = await yPSService.LoadPoDataService(sendPodata);

                    if (result != null && result.data != null)
                    {
                        if (result.status != 0 && result.data.allPoData != null && result.data.allPoData.Count > 0)
                        {
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.TaskID == Vm.taskID));
                        }
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetUpdatedAllPOData method -> in LinkPage.xaml.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
                Vm.IndicatorVisibility = false;
            }
            finally
            {
                Vm.IndicatorVisibility = false;
            }
            return AllPoDataList;
        }

    }
}