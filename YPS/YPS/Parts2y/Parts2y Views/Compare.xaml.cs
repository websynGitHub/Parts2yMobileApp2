using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
//using YPS.Parts2y.Parts2y_Common_Classes;
//using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Compare : ContentPage
    {
        YPSService trackService;
        CompareViewModel Vm;

        public Compare()
        {
            try
            {
                trackService = new YPSService();
                InitializeComponent();
                Settings.scanredirectpage = string.Empty;
                BindingContext = Vm = new CompareViewModel(Navigation, this, false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Compare Constructor -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                Task.Run(() => trackService.Handleexception(ex)).Wait();
            }
        }

        private async void ClearHistory(object sender, EventArgs e)
        {
            try
            {
                BindingContext = Vm = new CompareViewModel(Navigation, this, true);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClearHistory Method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
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
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped Method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private async void ViewHistory(object sender, EventArgs e)
        {
            try
            {
                Vm.showScanHistory = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ViewHistory Method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private async void HideHistory(object sender, EventArgs e)
        {
            try
            {
                Vm.showScanHistory = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HideHistory Method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private async void TotalEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Vm.TotalCount = !string.IsNullOrEmpty(e.NewTextValue) ? (int?)Convert.ToInt32(e.NewTextValue) : null;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TotalEntryTextChanged Method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private async void SaveClick(object sender, EventArgs e)
        {
            try
            {
                bool result = await App.Current.MainPage.DisplayAlert("Save rule", "Are you sure?", "Yes", "No");

                if (result)
                {
                    int? total = Vm.TotalCount;
                    string selectedrule = Vm.SelectedScanRule;


                    Vm.TotalCount = total;
                    Vm.SelectedScanRule = selectedrule;
                    bool value = await Vm.SaveConfig();

                    if (value == true)
                    {
                        BindingContext = Vm = new CompareViewModel(Navigation, this, false);
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveClick Method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                if (Settings.scanredirectpage.Trim().ToLower() == "ScanditScan".Trim().ToLower())
                {

                    Vm.Scanditscan(Settings.scanQRValuecode);
                    Settings.scanredirectpage = string.Empty;
                }
            }
            catch (Exception ex)
            {

            }

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

    }
}