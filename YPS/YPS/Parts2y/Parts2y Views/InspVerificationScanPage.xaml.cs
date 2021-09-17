using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspVerificationScanPage : ContentPage
    {
        #region Data members declaration
        InspVerificationScanViewModel Vm;
        YPSService yPSService;
        bool checkInternet;
        #endregion

        public InspVerificationScanPage(AllPoData selectedtagdata, bool isalldone)
        {
            try
            {
                yPSService = new YPSService();
                InitializeComponent();
                BindingContext = Vm = new InspVerificationScanViewModel(Navigation, selectedtagdata, isalldone, this);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "InspVerificationScanPage constructor -> in InspVerificationScanPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                Task.Run(() => yPSService.Handleexception(ex)).Wait();
            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (Vm.CanOpenScan == true)
                {
                    Vm.CanOpenScan = false;
                    Vm.IsScanPage = true;
                    await Vm.OpenScanner();
                    Vm.IsScanPage = false;
                }
                else
                {
                    if (Vm.IsScanPage == true)
                    {
                        await Navigation.PopAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in InspVerificationScanPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in InspVerificationScanPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }
    }
}