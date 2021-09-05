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
    public partial class KRPartsInspectionQuestionsPage : ContentPage
    {
        KRPartsInspectionQuestionsViewModel Vm;
        AllPoData selectedTagData;
        YPSService service;
        bool isAllDone;

        public KRPartsInspectionQuestionsPage(AllPoData selectedtagdata, bool isalldone)
        {
            try
            {
                service = new YPSService();
                InitializeComponent();
                Settings.IsRefreshPartsPage = true;
                selectedTagData = selectedtagdata;
                BindingContext = Vm = new KRPartsInspectionQuestionsViewModel(Navigation, this, selectedTagData, isalldone);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "KRPartsInspectionQuestionsPage Constructor -> in KRPartsInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => service.Handleexception(ex)).Wait();
            }
        }


        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (Vm.selectedTagData?.TaskResourceID == Settings.userLoginID)
                {
                    if (Vm.QuickTabVisibility == true)
                    {
                        Vm.QuickTabClicked();
                    }
                    else if (Vm.FullTabVisibility == true)
                    {
                        Vm.FullTabClicked();
                    }
                    else if (Vm.SignTabVisibility == true)
                    {
                        Vm.SignTabClicked();
                    }
                }
                else
                {
                    if (Vm.SignTabVisibility == true)
                    {
                        Vm.SignTabClicked();
                    }
                    else
                    {
                        //Vm.LoadTextColor = Color.Gray;
                        //Vm.IsQuickTabVisible = true;
                        //Vm.IsFullTabVisible = false;
                        //Vm.SignTabVisibility = false;
                        Task.Run(() => Vm.QuickTabClicked()).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing OnAppearing -> in KRPartsInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }


        private async void DoneClicked(object sender, EventArgs e)
        {
            try
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
                        selectedTagData.TagTaskStatus = 2;

                        if (selectedTagData.TaskStatus == 0)
                        {
                            ObservableCollection<AllPoData> podate = await Vm.GetUpdatedAllPOData();
                            TagTaskStatus taskstatus = new TagTaskStatus();
                            taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                            taskstatus.TaskStatus = (podate?.Where(wr => wr.TagTaskStatus != 2).FirstOrDefault() != null) ? 1 : 2;
                            taskstatus.CreatedBy = Settings.userLoginID;

                            var taskval = await service.UpdateTaskStatus(taskstatus);
                            selectedTagData.TaskStatus = 1;
                        }

                        await Vm.TabChange("job");
                        DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked Method -> in KRPartsInspectionQuestionsPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

    }
}