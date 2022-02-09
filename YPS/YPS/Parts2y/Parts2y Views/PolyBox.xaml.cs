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
using Syncfusion.XForms.Buttons;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PolyBox : ContentPage
    {

        YPSService trackService;
        PolyBoxViewModel Vm;

        public PolyBox()
        {
            try
            {
                InitializeComponent();
                Settings.scanredirectpage = "Polybox";
                BindingContext = Vm = new PolyBoxViewModel(Navigation, this, false);
                Vm.loadindicator = true;
                trackService = new YPSService();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PolyBox constructor -> in PolyBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                Task.Run(() => trackService.Handleexception(ex)).Wait();
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
                YPSLogger.ReportException(ex, "Back_Tapped method -> in PlayBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        private void ConfigRadioGroupCheckedChanged(object sender, Syncfusion.XForms.Buttons.CheckedChangedEventArgs e)
        {
            try
            {
                if (e.CurrentItem.ClassId == Vm.ScanConfigResult?.data?.PolyboxStatus[0]?.ID.ToString())
                {
                    Vm.IsEmpty = true;
                    Vm.IsFull = false;
                    Vm.ConfigSelectedSataus = (int)Vm.ScanConfigResult?.data?.PolyboxStatus[0]?.ID;
                }
                else
                {
                    Vm.IsEmpty = false;
                    Vm.IsFull = true;
                    Vm.ConfigSelectedSataus = (int)Vm.ScanConfigResult?.data?.PolyboxStatus[1]?.ID;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ConfigRadioGroupCheckedChanged method -> in PlayBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

    }
}