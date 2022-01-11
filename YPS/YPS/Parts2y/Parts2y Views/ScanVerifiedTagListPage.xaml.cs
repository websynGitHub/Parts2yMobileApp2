using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Model;
using System.Collections.ObjectModel;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanVerifiedTagListPage : ContentPage
    {
        ScanVerifiedTagListPageViewModel vm;
        YPSService service;

        public ScanVerifiedTagListPage(ObservableCollection<AllPoData> matchedtaglist,
            ObservableCollection<AllPoData> allpodata, int uploadtype)
        {
            try
            {
                InitializeComponent();
                BindingContext = vm = new ScanVerifiedTagListPageViewModel(Navigation, this,
                    matchedtaglist, allpodata, uploadtype);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScanVerifiedTagListPage constructor -> in ScanVerifiedTagListPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        private async void HomeRedirection(object sender, EventArgs e)
        {
            try
            {
                vm.IndicatorVisibility = true;
                await Navigation.PopToRootAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HomeRedirection method -> in ScanVerifiedTagListPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
            finally
            {
                vm.IndicatorVisibility = false;
            }
        }
    }
}