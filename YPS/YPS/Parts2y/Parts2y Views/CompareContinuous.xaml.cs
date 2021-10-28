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
                InitializeComponent();
                BindingContext = Vm = new CompareContinuousViewModel(Navigation, this, false);
                Vm.loadindicator = true;
                trackService = new YPSService();
                Settings.scanredirectpage = "CompareContinuous";
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CompareContinuous constructor -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                Task.Run(() => trackService.Handleexception(ex)).Wait();
            }
            Vm.loadindicator = false;
        }

        private async void ClearHistory(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;
                BindingContext = Vm = new CompareContinuousViewModel(Navigation, this, true);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClearHistory method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            Vm.loadindicator = false;
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
                Navigation.PopToRootAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private async void ViewHistory(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;
                Vm.showScanHistory = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ViewHistory method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            Vm.loadindicator = false;
        }

        private async void HideHistory(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;
                Vm.showScanHistory = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HideHistory method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            Vm.loadindicator = false;
        }

        private async void TotalEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Vm.TotalCount = !string.IsNullOrEmpty(e.NewTextValue) ? (int?)Convert.ToInt32(e.NewTextValue) : null;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TotalEntryTextChanged method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                    Vm.loadindicator = true;
                    await Vm.SaveConfig();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveClick method -> in CompareContinuous.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            Vm.loadindicator = false;
        }
    }
}