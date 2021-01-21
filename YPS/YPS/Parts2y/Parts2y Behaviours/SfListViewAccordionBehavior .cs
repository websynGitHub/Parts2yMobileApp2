using Syncfusion.ListView.XForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPSePOD.ypsepod.ViewModel;

namespace YPSePOD.ypsepod.Behaviours
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class SfListViewAccordionBehavior : Behavior<ContentPage>
    {
        private SfListView listView;
        private VehicleDetailViewModel viewModel;

        public SfListViewAccordionBehavior()
        {
            try
            {
                INavigation Navigation = null;
                viewModel = new VehicleDetailViewModel(Navigation);
            }
            catch(Exception ex)
            {

            }
        }


        protected override void OnAttachedTo(ContentPage bindable)
        {
            try
            {
                base.OnAttachedTo(bindable);
                listView = bindable.FindByName<SfListView>("listView");
                viewModel.listView = listView;
                listView.BindingContext = viewModel;
               //listView.ItemsSource = viewModel.QuestionInfo;
            }
            catch (Exception ex)
            {

            }
            //listView.ItemsSource = viewModel.ContactsInfo;
        }
    }
}
