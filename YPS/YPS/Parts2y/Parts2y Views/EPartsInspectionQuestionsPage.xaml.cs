using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
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
    public partial class EPartsInspectionQuestionsPage : ContentPage
    {
        EPartsInspectionQuestionsViewModel Vm;
        AllPoData selectedTagData;
        YPSService service;
        bool isAllDone;

        public EPartsInspectionQuestionsPage(AllPoData selectedtagdata, bool isalldone)
        {
            try
            {
                service = new YPSService();
                InitializeComponent();
                Settings.IsRefreshPartsPage = true;
                selectedTagData = selectedtagdata;
                BindingContext = Vm = new EPartsInspectionQuestionsViewModel(Navigation, this, selectedTagData, isalldone);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EPartsInspectionQuestionsPage constructor -> in EPartsInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => service.Handleexception(ex)).Wait();
            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();


                //if (Vm.selectedTagData?.TaskResourceID == Settings.userLoginID)
                //{
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
                //}
                //else
                //{
                //    if (Vm.SignTabVisibility == true)
                //    {
                //        Vm.SignTabClicked();
                //    }
                //    else
                //    {
                //        Task.Run(() => Vm.QuickTabClicked()).Wait();
                //    }
                //}
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing OnAppearing -> in EPartsInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }


        private async void DoneClicked(object sender, EventArgs e)
        {
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    bool makeitdone = await App.Current.MainPage.DisplayAlert("Confirm", "Make sure you have finished the inspection. \nAre you sure, you want to mark the selected part as Done ? ", "Ok", "Cancel");

                    if (makeitdone == true && selectedTagData.TaskID != 0)
                    {
                        TagTaskStatus tagtaskstatus = new TagTaskStatus();
                        tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                        tagtaskstatus.POTagID = Helperclass.Encrypt(selectedTagData.POTagID.ToString());
                        tagtaskstatus.Status = 2;
                        tagtaskstatus.CreatedBy = Settings.userLoginID;

                        var result = await service.UpdateTagTaskStatus(tagtaskstatus);

                        if (result?.status == 1)
                        {
                            selectedTagData.TagTaskStatus = 2;

                            //if (selectedTagData.TaskStatus == 0)
                            //{
                            ObservableCollection<AllPoData> podate = await Vm.GetUpdatedAllPOData();
                            TagTaskStatus taskstatus = new TagTaskStatus();
                            taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                            taskstatus.TaskStatus = (podate?.Where(wr => wr.TagTaskStatus != 2).FirstOrDefault() != null) ? 1 : 2;
                            taskstatus.CreatedBy = Settings.userLoginID;

                            var taskval = await service.UpdateTaskStatus(taskstatus);
                            selectedTagData.TaskStatus = taskstatus.TaskStatus;
                            //}

                            await App.Current.MainPage.DisplayAlert("Done", "This part is marked as done.", "Ok");
                            await Vm.TabChange("job");
                            //DependencyService.Get<IToastMessage>().ShortAlert("This part is marked as done.");
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked method -> in EPartsInspectionQuestionsPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
        private async void NextScanClicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ScanPage(0, null, false, null), false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SfButton_Clicked method -> in EPartsInspectionQuestionsPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private void ViewAllClicked(object sender, EventArgs e)
        {
            Vm.SignTabClicked();
        }
        private async void RadioButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                var question = sender as Plugin.InputKit.Shared.Controls.RadioButton;
                if (question != null)
                {
                    int QID = Convert.ToInt32(question.ClassId);
                    UpdateInspectionRequest updateInspectionRequest = new UpdateInspectionRequest();
                    updateInspectionRequest.Direct = Convert.ToInt32(question.Value);
                    updateInspectionRequest.QID = QID;
                    updateInspectionRequest.UserID = Settings.userLoginID;
                    updateInspectionRequest.TaskID = Vm.selectedTagData.TaskID;
                    updateInspectionRequest.POTagID = Vm.selectedTagData.POTagID;
                    updateInspectionRequest.ExpiryDate = "";

                    var result = await service.InsertUpdateInspectionResult_QListPage(updateInspectionRequest);
                    if (result != null && result.status == 1)
                    {
                        Vm.QuestionListCategory?.Where(wr => wr.MInspectionConfigID == QID).Select(x => { x.Status = 1; return x; }).ToList();
                        Vm.NextScanEnable = Vm.QuestionListCategory.All(a => a.Status == 1) ? true : false;
                        Vm.NextScanOpacity = Vm.NextScanEnable == false ? 0.5 : 1.0;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RadioButton_Clicked method -> in EPartsInspectionQuestionsPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}