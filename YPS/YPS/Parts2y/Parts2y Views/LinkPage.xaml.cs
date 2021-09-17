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
    public partial class LinkPage : ContentPage
    {
        #region Data members declaration
        LinkPageViewModel Vm;
        YPSService yPSService;
        bool checkInternet;
        SendPodata sendPodata = new SendPodata();
        #endregion

        public LinkPage(ObservableCollection<PhotoRepoDBModel> photorepolist)
        {
            try
            {
                yPSService = new YPSService();
                InitializeComponent();
                BindingContext = Vm = new LinkPageViewModel(Navigation, photorepolist, this);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LinkPage constructor -> in LinkPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => yPSService.Handleexception(ex)).Wait();
                Vm.IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when page is appearing.
        /// </summary>
        protected async override void OnAppearing()
        {
            YPSLogger.TrackEvent("LinkPage.xaml.cs", "OnAppearing method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                base.OnAppearing();

                if (Vm.updateList == true)
                {
                    Settings.CanOpenScanner = false;
                    await Vm.ShowContentsToLink();
                    Vm.updateList = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in LinkPage.xaml.cs  " + Settings.userLoginID);
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
                Vm.IndicatorVisibility = true;
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in LinkPage.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
                Vm.IndicatorVisibility = false;
            }
            Vm.IndicatorVisibility = false;
        }
    }
}