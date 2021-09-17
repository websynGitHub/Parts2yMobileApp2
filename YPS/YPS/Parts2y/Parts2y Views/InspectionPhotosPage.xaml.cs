using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectionPhotosPage : ContentPage
    {
        InspectionPhotoUploadViewModel Vm;
        YPSService trackService;

        public InspectionPhotosPage(int tagId, InspectionConfiguration inspectionConfiguration, string vinValue, AllPoData selectedtagdata, bool iscarrierinsp)
        {
            try
            {
                trackService = new YPSService();
                InitializeComponent();
                BindingContext = Vm = new InspectionPhotoUploadViewModel(Navigation, this, tagId, inspectionConfiguration, vinValue, selectedtagdata, iscarrierinsp);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "InspectionPhotosPage constructor -> in InspectionPhotosPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                Task.Run(() => trackService.Handleexception(ex)).Wait();
            }
        }

        private async void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in InspectionPhotosPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                Task.Run(() => trackService.Handleexception(ex)).Wait();
            }
        }
    }
}