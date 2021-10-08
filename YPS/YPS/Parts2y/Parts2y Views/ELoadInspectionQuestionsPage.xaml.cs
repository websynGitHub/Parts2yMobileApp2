using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ELoadInspectionQuestionsPage : ContentPage
    {
        ELoadInspectionQuestionsViewModel Vm;
        ObservableCollection<AllPoData> SelectedPodataList;
        YPSService service;

        public ELoadInspectionQuestionsPage(ObservableCollection<AllPoData> selectedpodatalist, bool isalltagsdone)
        {
            try
            {
                service = new YPSService();
                InitializeComponent();
                Settings.IsRefreshPartsPage = true;
                SelectedPodataList = selectedpodatalist;
                BindingContext = Vm = new ELoadInspectionQuestionsViewModel(Navigation, this, SelectedPodataList, isalltagsdone);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ELoadInspectionQuestionsPage constructor -> in ELoadInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => service.Handleexception(ex));
            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (Vm.LoadInspTabVisibility == true)
                {
                    Vm.LoadInspTabClicked();
                }
                else
                {
                    Vm.SignTabClicked();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in ELoadInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void DoneClicked(object sender, EventArgs e)
        {
            try
            {
                if (SelectedPodataList[0].TaskID != 0)
                {
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (SelectedPodataList[0].TaskStatus != 2)
                        {
                            TagTaskStatus taskstatus = new TagTaskStatus();
                            taskstatus.TaskID = Helperclass.Encrypt(SelectedPodataList[0].TaskID.ToString());
                            taskstatus.TaskStatus = 2;
                            taskstatus.CreatedBy = Settings.userLoginID;
                            var taskval = await service.UpdateTaskStatus(taskstatus);

                            if (taskval?.status == 1)
                            {
                                SelectedPodataList[0].TaskID = 2;
                                await Vm.TabChange("job");
                                DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");
                            }
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked method -> in ELoadInspectionQuestionsPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}