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
                YPSLogger.ReportException(ex, "Compare constructor -> in compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "ClearHistory method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "Back_Tapped method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "ViewHistory method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "HideHistory method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "TotalEntryTextChanged method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "SaveClick method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnDisappearing method -> in Compare.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

    }
}