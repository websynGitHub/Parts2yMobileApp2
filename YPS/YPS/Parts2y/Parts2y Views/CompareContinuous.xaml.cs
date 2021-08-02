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
    public partial class CompareContinuous : ContentPage
    {
        YPSService trackService;
        CompareContinuousViewModel Vm;

        public CompareContinuous()
        {
            try
            {
                trackService = new YPSService();
                InitializeComponent();
                Settings.scanredirectpage = "CompareContinuous";
                BindingContext = Vm = new CompareContinuousViewModel(Navigation, this, false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CompareContinuous Constructor -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                Task.Run(() => trackService.Handleexception(ex)).Wait();
            }
        }

        private async void ClearHistory(object sender, EventArgs e)
        {
            try
            {
                BindingContext = Vm = new CompareContinuousViewModel(Navigation, this, true);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClearHistory Method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "Back_Tapped Method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "ViewHistory Method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "HideHistory Method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "TotalEntryTextChanged Method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                        BindingContext = Vm = new CompareContinuousViewModel(Navigation, this, false);
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveClick Method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                //if (Settings.scanredirectpage == "CompareContinuous")
                //{
                //Vm.Scanditscan(Settings.scanQRValuecode);
                //Vm.scanset.ContinuousAfterScan = false;
                //Settings.scanredirectpage = string.Empty;
                //}
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