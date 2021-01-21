using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        HomeViewModel Vm;
        public HomePage()
        {
            try
            {
                InitializeComponent();
                BindingContext = Vm = new HomeViewModel(Navigation);
                InPrgrs_List.ItemTapped += (s, e) => InPrgrs_List.SelectedItem = null;
            }
            catch (Exception ex)
            {

            }
        }

        //private async  void TransportReportTapped(object sender, ItemTappedEventArgs e)
        //{
        //    await Navigation.PushAsync(new TransportReportDetails());
        //}
    }
}