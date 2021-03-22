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

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VinInspectionAnswersPage : ContentPage
    {
        VinInspectionAnswersPageViewModel Vm;
        YPSService service;
        AllPoData selectedTagData;

        public VinInspectionAnswersPage(InspectionConfiguration inspectionConfiguration, ObservableCollection<InspectionConfiguration> inspectionConfigurationList, List<InspectionResultsList> inspectionResultsLists, AllPoData selectedtagdata, bool isVINInsp, bool isalldone = false)
        {
            try
            {
                InitializeComponent();
                selectedTagData = selectedtagdata;
                BindingContext = Vm = new VinInspectionAnswersPageViewModel(Navigation, this, inspectionConfiguration, inspectionConfigurationList, inspectionResultsLists, selectedtagdata, isVINInsp, isalldone);

                if (Device.RuntimePlatform == Device.iOS)
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 30;
                    headerpart.Padding = safeAreaInset;
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                Task.Run(() => Vm.GetInspectionPhotos()).Wait();
            }
            catch (Exception ex)
            {

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
                YPSLogger.ReportException(ex, "DoneClicked constructor -> in VinInspectionAnswersPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}