using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Views;
using YPS.Parts2y.Parts2y_Views;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;
using YPS.Helpers;
using YPS.CommonClasses;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using YPS.Model;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanPage : ContentPage
    {
        #region Data members declaration
        ScanPageViewModel Vm;
        YPSService yPSService;
        bool checkInternet;

        #endregion

        public ScanPage(int uploadtype, PhotoUploadModel selectedtagdata, bool isinitialphoto, AllPoData selectepotagdata)
        {
            try
            {
                yPSService = new YPSService();
                InitializeComponent();
                BindingContext = Vm = new ScanPageViewModel(Navigation, uploadtype, selectedtagdata, isinitialphoto, selectepotagdata, this);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScanPage constructor -> in ScanPage.xaml.cs " + Settings.userLoginID);
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
                        await Navigation.PopAsync(false);
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in ScanPage.xaml.cs " + Settings.userLoginID);
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
                Navigation.PopToRootAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in ScanPage.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }
    }
}