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
    public partial class PhotoRepoPage : ContentPage
    {
        #region Data members declaration
        PhotoRepoPageViewModel Vm;
        YPSService yPSService;
        bool checkInternet;
        SendPodata sendPodata = new SendPodata();
        #endregion

        public PhotoRepoPage()
        {
            try
            {

                InitializeComponent();
                yPSService = new YPSService();
                BindingContext = Vm = new PhotoRepoPageViewModel(Navigation, this);

                if (Device.RuntimePlatform == Device.iOS)// for adjusting the display as per the notch
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 20;
                    headerpart.Padding = safeAreaInset;
                }

                Vm.IndicatorVisibility = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PhotoRepoPage constructor -> in PhotoRepoPage.xaml.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
                Vm.IndicatorVisibility = false;
            }
            Vm.IndicatorVisibility = false;
        }

        protected async override void OnAppearing()
        {
            try
            {
                Vm.IndicatorVisibility = true;
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in PhotoRepoPage.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
                Vm.IndicatorVisibility = false;
            }
            Vm.IndicatorVisibility = false;
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

                if (Vm.IsRepoaPage == true)
                {
                    await Navigation.PopAsync();
                }
                else
                {
                    //Vm.UploadViewContentVisible = true;
                    //Vm.IsPhotoUploadIconVisible = true;
                    //Vm.POTagLinkContentVisible = false;
                    //Vm.IsRepoaPage = true;

                    //await Vm.GetPhotosData();
                    //if (Settings.POID > 0)
                    //{
                    //    Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                    //    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                    //    Navigation.RemovePage(Navigation.NavigationStack[0]);

                    Settings.POID = 0;
                    Vm.taskID = 0;
                    //    //await Navigation.PopAsync();
                    //}
                    //else
                    //{
                    Vm.UploadViewContentVisible = true;
                    Vm.IsPhotoUploadIconVisible = true;
                    Vm.POTagLinkContentVisible = false;
                    Vm.IsRepoaPage = true;
                    Vm.Title = "Photo Repo";
                    await Vm.GetPhotosData();
                    //}
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in PhotoRepoPage.xaml.cs " + Settings.userLoginID);
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
                YPSLogger.TrackEvent("PhotoUpload.xaml.cs", "in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

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
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.POID == Settings.POID && wr.TaskID == Vm.taskID));
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
            }
            finally
            {
                Vm.IndicatorVisibility = false;
            }
            return AllPoDataList;
        }
    }
}