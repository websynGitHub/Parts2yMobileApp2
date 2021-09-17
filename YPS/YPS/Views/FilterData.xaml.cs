using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterData : ContentPage
    {
        #region Data member declaration
        FilterDataViewModel vm;
        YPSService yPSService;
        bool checkInternet;
        #endregion

        /// <summary>
        /// Parameterless constructor. 
        /// </summary>
        public FilterData()
        {
            try
            {
                InitializeComponent();

                YPSLogger.TrackEvent("FilterData.xaml.cs", " Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

                Settings.currentPage = "FilterDataPage";
                BindingContext = vm = new FilterDataViewModel(Navigation, this);
                yPSService = new YPSService();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FilterData constructor -> in FilterData.xaml.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on the back button and redirect to previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in FilterData.xaml.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on the home icon in filter page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToHome_Tapped(object sender, EventArgs e)
        {
            try
            {
                App.Current.MainPage = new Parts2y.Parts2y_Views.MenuPage(typeof(Parts2y.Parts2y_Views.HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GoToHome_Tapped method -> in FilterData.xaml.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        private async void search_Clicked(object sender, EventArgs e)
        {
            try
            {
                vm.IsSaveSearchContentVisible = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "search_Clicked method -> in FilterData.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }

        private async void CloseSearchFilterPopUp(object sender, EventArgs e)
        {
            try
            {
                vm.IsSaveSearchContentVisible = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CloseSearchFilterPopUp method -> in FilterData.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }
    }
}