using Syncfusion.XForms.Buttons;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomRenders;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoUpload : MyContentPage
    {
        #region Data Members
        PhotoUplodeViewModel vm;
        YPSService service;
        bool accessPhoto, voidAlertMessage;
        SendPodata sendPodata = new SendPodata();
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="sItems"></param>
        /// <param name="allPoData"></param>
        /// <param name="selectionType"></param>
        /// <param name="uploadType"></param>
        /// <param name="photoAccess"></param>
        public PhotoUpload(PhotoUploadModel sItems, AllPoData allPoData, string selectionType, int uploadType, bool photoAccess)
        {
            YPSLogger.TrackEvent("PhotoUpload", "Page constructer " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                InitializeComponent();

                if (Device.RuntimePlatform == Device.iOS)// for adjusting the display as per the notch
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 20;
                    headerpart.Padding = safeAreaInset;
                }

                Settings.currentPage = "PhotoUploadPage";// Setting the current page as "PhotoUploadPage" to settings
                accessPhoto = photoAccess;
                service = new YPSService();// Creating new instance of the YPSService, which is used to call AIP
                BindingContext = vm = new PhotoUplodeViewModel(Navigation, this, sItems, allPoData, selectionType, uploadType, photoAccess);

                //if (Settings.userRoleID == (int)UserRoles.SuperAdmin)
                //{
                //    PhotoUploadIcon.IsVisible = false;
                //}

                img.WidthRequest = App.ScreenWidth;
                img.HeightRequest = App.ScreenHeight - 150;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PhotoUpload constructor -> in PhotoUpload.cs  " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when page is appearing.
        /// </summary>
        protected override void OnAppearing()
        {
            YPSLogger.TrackEvent("PhotoUpload", "Page constructer " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                base.OnAppearing();
                /// For Enable and Disble master detail page menu gesture
                //(Application.Current.MainPage as YPSMasterPage).IsGestureEnabled = true;
                Settings.photoUploadPageCount = 0;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing constructor -> in PhotoUpload.cs  " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when leaving the page.
        /// </summary>
        protected override void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnDisappearing constructor -> in PhotoUpload.cs  " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when back icon is clicked, to redirect  to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (Settings.POID > 0)
                {
                    Navigation.RemovePage(Navigation.NavigationStack[1]);
                    Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                    Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                    Settings.POID = 0;
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped constructor -> in PhotoUpload.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async Task<ObservableCollection<AllPoData>> GetUpdatedAllPOData()
        {
            ObservableCollection<AllPoData> AllPoDataList = new ObservableCollection<AllPoData>();

            try
            {
                vm.IndicatorVisibility = true;
                YPSLogger.TrackEvent("PhotoUpload.xaml.cs", "in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {

                    sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;

                    var result = await service.LoadPoDataService(sendPodata);

                    if (result != null && result.data != null)
                    {
                        if (result.status != 0 && result.data.allPoData != null && result.data.allPoData.Count > 0)
                        {
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoData.Where(wr => wr.POID == Settings.POID));
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
                vm.IndicatorVisibility = false;
            }
            return AllPoDataList;
        }

        private async void SfButton_Clicked(object sender, EventArgs e)
        {
            if (!voidAlertMessage)
            {
                voidAlertMessage = true;

                try
                {
                    string text = (sender as SfButton).Text;

                    if (text.Trim().ToLower() == vm.labelobjComplete.Trim().ToLower() && accessPhoto == false)
                    {
                        await vm.ClosePic();
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "CompleteStateChanged method -> in PhotoUpload.cs  " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
                voidAlertMessage = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on Complete RadioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CompleteStateChanged(object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            if (!voidAlertMessage)
            {
                voidAlertMessage = true;

                try
                {
                    string text = (sender as SfRadioButton).Text;

                    if (e.IsChecked == true && text.Trim().ToLower() == vm.labelobjComplete.Trim().ToLower() && accessPhoto == false)
                    {
                        await vm.ClosePic();
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "CompleteStateChanged method -> in PhotoUpload.cs  " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
                voidAlertMessage = false;
            }
        }
    }
}