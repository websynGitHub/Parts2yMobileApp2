using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;
using RadioButton = Plugin.InputKit.Shared.Controls.RadioButton;


namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VinInspectionAnswersPage : ContentPage
    {
        VinInspectionAnswersPageViewModel Vm;
        YPSService service;
        AllPoData selectedTagData;

        public VinInspectionAnswersPage(InspectionConfiguration inspectionConfiguration, ObservableCollection<InspectionConfiguration> inspectionConfigurationList, List<InspectionResultsList> inspectionResultsLists, AllPoData selectedtagdata, bool isVINInsp,
            CarrierInspectionQuestionsViewModel CarQueVm, VinInspectQuestionsPageViewModel VINQueVm, bool isalldone = false
            )
        {
            try
            {
                service = new YPSService();
                InitializeComponent();
                selectedTagData = selectedtagdata;
                BindingContext = Vm = new VinInspectionAnswersPageViewModel(Navigation, this, inspectionConfiguration, inspectionConfigurationList, inspectionResultsLists, selectedtagdata, isVINInsp, CarQueVm, VINQueVm, isalldone);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "VinInspectionAnswersPage Constructor -> in VinInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => service.Handleexception(ex)).Wait();
            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                Task.Run(() => Vm.GetInspectionPhotos()).Wait();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing Constructor -> in VinInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void DoneClicked(object sender, EventArgs e)
        {
            try
            {
                if (Vm.isInspVIN == true)
                {
                    if (selectedTagData.TaskID != 0 && selectedTagData.TagTaskStatus != 2)
                    {
                        TagTaskStatus tagtaskstatus = new TagTaskStatus();
                        tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                        tagtaskstatus.POTagID = Helperclass.Encrypt(selectedTagData.POTagID.ToString());
                        tagtaskstatus.Status = 2;
                        tagtaskstatus.CreatedBy = Settings.userLoginID;

                        var result = await service.UpdateTagTaskStatus(tagtaskstatus);

                        if (result.status == 1)
                        {
                            if (selectedTagData.TaskStatus == 0)
                            {
                                TagTaskStatus taskstatus = new TagTaskStatus();
                                taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                                taskstatus.TaskStatus = 1;
                                taskstatus.CreatedBy = Settings.userLoginID;

                                var taskval = await service.UpdateTaskStatus(taskstatus);
                            }
                            DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");
                        }
                    }
                }
                else
                {
                    if (selectedTagData.TaskID != 0)
                    {

                        if (selectedTagData.TaskStatus != 2)
                        {
                            TagTaskStatus taskstatus = new TagTaskStatus();
                            taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
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
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked Method -> in VinInspectionAnswersPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void PlaneradioClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (RadioButton)sender;
                if (rb.ClassId == "0")
                {
                    Vm.PlaneTrue = true;
                    Vm.PlaneFalse = false;
                }
                else
                {
                    Vm.PlaneFalse = true;
                    Vm.PlaneTrue = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PlaneradioClicked Method -> in VinInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void RearRightClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (RadioButton)sender;
                if (rb.ClassId == "0")
                {
                    Vm.RearRightTrue = true;
                    Vm.RearRightFalse = false;
                }
                else
                {
                    Vm.RearRightFalse = true;
                    Vm.RearRightTrue = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RearRightClicked Method -> in VinInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void RearLeftClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (RadioButton)sender;
                if (rb.ClassId == "0")
                {
                    Vm.RearLeftTrue = true;
                    Vm.RearLeftFalse = false;
                }
                else
                {
                    Vm.RearLeftTrue = false;
                    Vm.RearLeftFalse = true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RearLeftClicked Method -> in VinInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void FrontRightClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (RadioButton)sender;
                if (rb.ClassId == "0")
                {
                    Vm.FrontRightTrue = true;
                    Vm.FrontRightFalse = false;

                }
                else
                {
                    Vm.FrontRightFalse = true;
                    Vm.FrontRightTrue = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FrontRightClicked Method -> in VinInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void FrontLeftClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (RadioButton)sender;
                if (rb.ClassId == "0")
                {
                    Vm.FrontLeftTrue = true;
                    Vm.FrontLeftFalse = false;

                }
                else
                {
                    Vm.FrontLeftTrue = false;
                    Vm.FrontLeftFalse = true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FrontLeftClicked Method -> in VinInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}