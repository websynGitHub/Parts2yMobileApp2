using SignaturePad.Forms;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
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
    public partial class CLoadInspectionQuestionsPage : ContentPage
    {
        CLoadInspectionQuestionsViewModel Vm;
        ObservableCollection<AllPoData> SelectedPodataList;
        YPSService service;

        public CLoadInspectionQuestionsPage(ObservableCollection<AllPoData> selectedpodatalist, bool isalltagsdone)
        {
            try
            {
                service = new YPSService();
                InitializeComponent();
                Settings.IsRefreshPartsPage = true;
                SelectedPodataList = selectedpodatalist;
                BindingContext = Vm = new CLoadInspectionQuestionsViewModel(Navigation, this, SelectedPodataList, isalltagsdone);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CLoadInspectionQuestionsPage constructor -> in CLoadInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => service.Handleexception(ex));
            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (Vm.InspTabVisibility == true)
                {
                    Vm.InspTabClicked();
                }
                else
                {
                    Vm.SignTabClicked();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in CLoadInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "DoneClicked method -> in CLoadInspectionQuestionsPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void ConfirmSignatureTapped(object sender, EventArgs e)
        {
            try
            {
                byte[] result;
                var strokes = PadView.Strokes;
                Stream image = await PadView.GetImageStreamAsync(SignatureImageFormat.Png);

                if (image != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.CopyTo(ms);
                        result = ms.ToArray();
                        string base64 = Convert.ToBase64String(result);
                        
                        if (result != null)
                        {
                            InspectionResultsList inspobj = new InspectionResultsList
                            {
                                TaskID = SelectedPodataList[0].TaskID,
                                UserID = Settings.userLoginID,
                                Signature = base64,
                            };

                            if (Vm.Signature == "CBUSupervisor")
                            {
                                inspobj.SignType = (int)InspectionSignatureType.VinSupervisor;

                                var sign = await service.InsertUpdateSignature(inspobj);
                            }
                            else if (Vm.Signature == "CBUAuditor")
                            {
                                inspobj.SignType = (int)InspectionSignatureType.VinAuditor;


                                var sign = await service.InsertUpdateSignature(inspobj);
                            }
                            else if (Vm.Signature == "CarrierSupervisor")
                            {
                                inspobj.SignType = (int)InspectionSignatureType.CarrierSupervisor;


                                var sign = await service.InsertUpdateSignature(inspobj);
                            }
                            else if (Vm.Signature == "CarrierAuditor")
                            {
                                inspobj.SignType = (int)InspectionSignatureType.CarrierAuditor;


                                var sign = await service.InsertUpdateSignature(inspobj);
                            }
                            
                            await Vm.GetInspSignature();

                            Vm.SignaturePadPopup = false;
                            Vm.SignTabVisibility = true;
                            PadView.Clear();
                        }
                    }
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Alert", "Please sign before saving..", "Ok");

                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ConfirmSignatureTapped method -> in CLoadInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}