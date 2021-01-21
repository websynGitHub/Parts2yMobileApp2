using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;
using static YPS.Parts2y.Parts2y_Models.PhotoModel;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoUploadePOD : ContentPage
    {
        EPODPhotoUplodeViewModel Vm;
        ObservableCollection<CustomPhotoModel> addedlist = new ObservableCollection<CustomPhotoModel>();

        public PhotoUploadePOD()
        {
            InitializeComponent();
            BindingContext = Vm = new EPODPhotoUplodeViewModel(Navigation);
        }

        private void select_picclick(object sender, EventArgs e)
        {
            try
            {
                Vm.SelectPic();
            }
            catch (Exception)
            {

            }          
         
        }
    }
}