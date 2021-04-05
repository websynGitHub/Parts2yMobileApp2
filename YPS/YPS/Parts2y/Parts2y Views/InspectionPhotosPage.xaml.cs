using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectionPhotosPage : ContentPage
    {
        InspectionPhotoUploadViewModel Vm;
        public InspectionPhotosPage(int tagId, InspectionConfiguration inspectionConfiguration, string vinValue, AllPoData selectedtagdata, bool iscarrierinsp)
        {
            InitializeComponent();
            BindingContext = Vm = new InspectionPhotoUploadViewModel(Navigation, this, tagId, inspectionConfiguration, vinValue, selectedtagdata, iscarrierinsp);
        }

        private async void Back_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}