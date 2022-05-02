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
    public partial class PInspectionAnswersPage : ContentPage
    {
        PInspectionAnswersViewModel Vm;
        YPSService service;
        AllPoData selectedTagData;

        public PInspectionAnswersPage(InspectionConfiguration inspectionConfiguration, ObservableCollection<InspectionConfiguration> inspectionConfigurationList,
            List<InspectionResultsList> inspectionResultsLists, AllPoData selectedtagdata, bool isVINInsp,
            PPartsInspectionQuestionsViewModel PartsQueVm, PLoadInspectionQuestionsViewModel LoadQueVm, bool isalldone = false,
            string pendingTagIDs = null)
        {
            try
            {
                service = new YPSService();
                InitializeComponent();
                selectedTagData = selectedtagdata;
                BindingContext = Vm = new PInspectionAnswersViewModel(Navigation, this, inspectionConfiguration,
                    inspectionConfigurationList, inspectionResultsLists, selectedtagdata, isVINInsp, PartsQueVm, LoadQueVm, isalldone, pendingTagIDs);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PInspectionAnswersPage constructor -> in PInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => service.Handleexception(ex)).Wait();
            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                Task.Run(() => Vm.GetInspectionPhotos());
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in PInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
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

                            if (result?.status == 1)
                            {
                                if (selectedTagData.TaskStatus == 0)
                                {
                                    TagTaskStatus taskstatus = new TagTaskStatus();
                                    taskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                                    taskstatus.TaskStatus = 1;
                                    taskstatus.CreatedBy = Settings.userLoginID;

                                    var taskval = await service.UpdateTaskStatus(taskstatus);
                                }
                                await App.Current.MainPage.DisplayAlert("Done", "Marked as done.", "Ok");
                                //DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");
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

                                if (taskval?.status == 1)
                                {
                                    await App.Current.MainPage.DisplayAlert("Done", "Marked as done.", "Ok");
                                    //DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");
                                }
                            }

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
                YPSLogger.ReportException(ex, "DoneClicked method -> in PInspectionAnswersPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
        private async void PlaneradioClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (RadioButton)sender;
                switch (rb.ClassId)
                {
                    case "0":
                        Vm.PlaneTrue = true;
                        Vm.PlaneFalse = false;
                        Vm.PlaneNA = false;
                        break;
                    case "1":
                        Vm.PlaneFalse = true;
                        Vm.PlaneTrue = false;
                        Vm.PlaneNA = false;
                        break;
                    case "2":
                        Vm.PlaneTrue = false;
                        Vm.PlaneFalse = false;
                        Vm.PlaneNA = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PlaneradioClicked method -> in KPInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void RearRightClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (Plugin.InputKit.Shared.Controls.RadioButton)sender;
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
                YPSLogger.ReportException(ex, "RearRightClicked method -> in PInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void RearLeftClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (Plugin.InputKit.Shared.Controls.RadioButton)sender;
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
                YPSLogger.ReportException(ex, "RearLeftClicked method -> in PInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void FrontRightClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (Plugin.InputKit.Shared.Controls.RadioButton)sender;
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
                YPSLogger.ReportException(ex, "FrontRightClicked method -> in PInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void FrontLeftClicked(object sender, EventArgs e)
        {
            try
            {
                var rb = (Plugin.InputKit.Shared.Controls.RadioButton)sender;
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
                YPSLogger.ReportException(ex, "FrontLeftClicked method -> in PInspectionAnswersPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private void OnKeyboardAppear(object sender, EventArgs e)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    {
                        Device.BeginInvokeOnMainThread(() => controlLayout.HeightRequest = 0);
                        break;
                    }
            }
        }
        private void OnKeyboardDisppear(object sender, EventArgs e)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    {
                        Device.BeginInvokeOnMainThread(() => controlLayout.HeightRequest = 45);
                        break;
                    }
            }
        }

        private void ExpiryDateClick(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => pkExpiryDate.Focus());
        }

        private void DateSelected(object sender, DateChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                txtExpiryDate.Text = pkExpiryDate.Date.ToString("dd MMM yyyy");
                pkExpiryDate.Unfocus();
            });
        }

        private void NextQuestionClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await scrQustionView.ScrollToAsync(scrQustionView.Content as Element, ScrollToPosition.Start, true);
            });
        }

        private void InnerQuantityInput(object sender, TextChangedEventArgs e)
        {
            var input = ((Editor)sender).Text;
            try
            {
                if (e.OldTextValue.Contains('.'))
                {
                    var split = e.NewTextValue.Split('.')[1].Length;
                    ((Editor)sender).Text = !(split <= 2) ? e.NewTextValue.Remove(e.NewTextValue.Length - 1) : e.NewTextValue;
                }
            }
            catch
            {

            }
        }
    }
}