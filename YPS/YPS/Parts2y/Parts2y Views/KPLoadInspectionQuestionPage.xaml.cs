﻿using System;
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
    public partial class KPLoadInspectionQuestionPage : ContentPage
    {
        KPLoadInspectionQuestionViewModel Vm;
        ObservableCollection<AllPoData> SelectedPodataList;
        YPSService service;

        public KPLoadInspectionQuestionPage(ObservableCollection<AllPoData> selectedpodatalist, bool isalltagsdone)
        {
            try
            {
                service = new YPSService();
                InitializeComponent();
                Settings.IsRefreshPartsPage = true;
                SelectedPodataList = selectedpodatalist;
                BindingContext = Vm = new KPLoadInspectionQuestionViewModel(Navigation, this, SelectedPodataList, isalltagsdone);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "KPLoadInspectionQuestionPage constructor -> in KPLoadInspectionQuestionPage.xaml.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "OnAppearing method -> in KPLoadInspectionQuestionPage.xaml.cs " + Settings.userLoginID);
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

                    bool makeitdone = await App.Current.MainPage.DisplayAlert("Confirm", "Make sure you have finished the inspection. \nAre you sure, you want to mark this as Done ? ", "Ok", "Cancel");

                    if (makeitdone == true && SelectedPodataList[0].TaskID != 0)
                    {
                        //if (SelectedPodataList[0].TaskStatus != 2)
                        //{
                        TagTaskStatus taskstatus = new TagTaskStatus();
                        taskstatus.TaskID = Helperclass.Encrypt(SelectedPodataList[0].TaskID.ToString());
                        taskstatus.TaskStatus = 2;
                        taskstatus.CreatedBy = Settings.userLoginID;
                        var taskval = await service.UpdateTaskStatus(taskstatus);

                        if (taskval?.status == 1)
                        {
                            TagTaskStatus tagtaskstatus = new TagTaskStatus();
                            tagtaskstatus.TaskID = Helperclass.Encrypt(SelectedPodataList.Select(c => c.TaskID).FirstOrDefault().ToString());

                            List<string> EncPOTagID = new List<string>();

                            foreach (var data in SelectedPodataList)
                            {
                                var value = Helperclass.Encrypt(data.POTagID.ToString());
                                EncPOTagID.Add(value);
                            }
                            tagtaskstatus.POTagID = string.Join(",", EncPOTagID);
                            tagtaskstatus.Status = 2;
                            tagtaskstatus.CreatedBy = Settings.userLoginID;

                            var result = await service.UpdateTagTaskStatus(tagtaskstatus);

                            await App.Current.MainPage.DisplayAlert("Done", "This job is marked as done.", "Ok");
                            await Vm.TabChange("job");
                            //DependencyService.Get<IToastMessage>().ShortAlert("This job is marked as done.");
                        }
                        //}
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
                YPSLogger.ReportException(ex, "DoneClicked method -> in KPLoadInspectionQuestionPage.xaml.cs  " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "SfButton_Clicked method -> in KPLoadInspectionQuestionPage.xaml.cs  " + Settings.userLoginID);
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
                    updateInspectionRequest.TaskID = Vm.SelectedPodataList[0].TaskID;
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
                YPSLogger.ReportException(ex, "RadioButton_Clicked method -> in KPLoadInspectionQuestionPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}