using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
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
    public partial class PhotoRepoPage : ContentPage
    {
        #region Data members declaration
        PhotoRepoPageViewModel Vm;
        YPSService yPSService;
        bool checkInternet;
        SendPodata sendPodata = new SendPodata();
        #endregion

        public PhotoRepoPage()
        {
            try
            {
                yPSService = new YPSService();
                InitializeComponent();
                BindingContext = Vm = new PhotoRepoPageViewModel(Navigation, this);
                Vm.IndicatorVisibility = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PhotoRepoPage constructor -> in PhotoRepoPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => yPSService.Handleexception(ex)).Wait();
                Vm.IndicatorVisibility = false;
            }
            Vm.IndicatorVisibility = false;
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
                Vm.IndicatorVisibility = true;
                Navigation.PopToRootAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in PhotoRepoPage.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
                Vm.IndicatorVisibility = false;
            }
            Vm.IndicatorVisibility = false;
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            Vm.IndicatorVisibility = true;

            try
            {
                var val = sender as Plugin.InputKit.Shared.Controls.CheckBox;
                var item = (PhotoRepoModel)val.BindingContext;

                item.IsSelected = val.IsChecked == true ? true : false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectionChanged method -> in PhotoRepoPage.xaml.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
                Vm.IndicatorVisibility = false;
            }
            Vm.IndicatorVisibility = false;
        }
    }
}