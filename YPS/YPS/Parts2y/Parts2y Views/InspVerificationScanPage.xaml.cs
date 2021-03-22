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

        public InspVerificationScanPage(AllPoData selectedtagdata,bool isalldone)
        {
            try
            {
                InitializeComponent();

                if (Device.RuntimePlatform == Device.iOS)
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 20;
                    headerpart.Padding = safeAreaInset;
                }


                yPSService = new YPSService();
                BindingContext = Vm = new InspVerificationScanViewModel(Navigation, selectedtagdata, isalldone, this);
            }
            catch (Exception ex)
            {

            }
        }


        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                //if (Vm.CanOpenScan == true)
                //{
                //    Vm.CanOpenScan = false;
                //    Vm.IsScanPage = true;
                //    await Vm.OpenScanner();
                //    Vm.IsScanPage = false;
                //}
                //else
                //{
                //    if (Vm.IsScanPage == true)
                //    {
                //        await Navigation.PopAsync();
                //    }
                //}
            }
            catch (Exception ex)
            {

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
                YPSLogger.ReportException(ex, "Back_Tapped method -> in ScanPage.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }
    }
}