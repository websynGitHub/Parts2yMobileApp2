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
    public partial class CarrierInspectionQuestionsPage : ContentPage
    {
        CarrierInspectionQuestionsViewModel Vm;
        ObservableCollection<AllPoData> SelectedPodataList;
        YPSService service;

        public CarrierInspectionQuestionsPage(ObservableCollection<AllPoData> selectedpodatalist, bool isalltagsdone)
        {
            try
            {
                InitializeComponent();
                service = new YPSService();
                Settings.IsRefreshPartsPage = true;
                SelectedPodataList = selectedpodatalist;

                if (Device.RuntimePlatform == Device.iOS)
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 30;
                    headerpart.Padding = safeAreaInset;
                }

                BindingContext = Vm = new CarrierInspectionQuestionsViewModel(Navigation, this, SelectedPodataList, isalltagsdone);
            }
            catch (Exception ex)
            {

            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                await Vm.GetConfigurationResults(3);
            }
            catch (Exception ex)
            {

            }
        }


        private async void DoneClicked(object sender, EventArgs e)
        {
            try
            {

                if (SelectedPodataList[0].TaskID != 0)
                {

                    if (SelectedPodataList[0].TaskStatus != 2)
                    {
                        TagTaskStatus taskstatus = new TagTaskStatus();
                        taskstatus.TaskID = Helperclass.Encrypt(SelectedPodataList[0].TaskID.ToString());
                        taskstatus.TaskStatus = 2;
                        taskstatus.CreatedBy = Settings.userLoginID;

                        var taskval = await service.UpdateTaskStatus(taskstatus);

                        if (taskval.status == 1)
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked constructor -> in CarrierInspectionQuestionsPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}