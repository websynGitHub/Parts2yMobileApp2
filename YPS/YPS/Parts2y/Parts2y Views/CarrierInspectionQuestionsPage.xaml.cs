using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
                service = new YPSService();
                InitializeComponent();
                Settings.IsRefreshPartsPage = true;
                SelectedPodataList = selectedpodatalist;

                //if (Device.RuntimePlatform == Device.iOS)
                //{
                //    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                //    safeAreaInset.Bottom = 0;
                //    safeAreaInset.Top = 30;
                //    headerpart.Padding = safeAreaInset;
                //}

                BindingContext = Vm = new CarrierInspectionQuestionsViewModel(Navigation, this, SelectedPodataList, isalltagsdone);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CarrierInspectionQuestionsPage Constructor -> in CarrierInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
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
                //await Vm.GetConfigurationResults(3);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing Method -> in CarrierInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
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
                            SelectedPodataList[0].TaskID = 2;
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

                            //else
                            //{
                            //    //if (Vm.AuditorImageSign != null)
                            //    //{
                            //    //    byte[] Base64Stream = Convert.FromBase64String(base64);
                            //    //    Vm.result[0].signatureSupervisorBase64 = Base64Stream;
                            //    //    Vm.result[0].Vindata.PDICompleted = DateTime.Now.ToString("h:mm");
                            //    //    Vm.result[0].Vindata.Load = DateTime.Now.ToString("h:mm");
                            //    //    Vm.SupervisorImageSign = ImageSource.FromStream(() => new MemoryStream(Base64Stream));

                            //    //    if (Vm.SupervisorImageSign != null)
                            //    //    {
                            //    //        Vm.OkToLoadColor = Settings.Bar_Background;
                            //    //        Vm.OkayToGoEnable = true;
                            //    //    }
                            //    //    Vm.SignaturePadPopup = false;
                            //    //    PadView.Clear();
                            //    //}
                            //    //else
                            //    //{
                            //    //    PadView.Clear();
                            //    //    App.Current.MainPage.DisplayAlert("Alert", "Please complete Auditor's signature", "Ok");
                            //    //}
                            //}

                        }
                        else
                        {

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
                YPSLogger.ReportException(ex, "ConfirmSignatureTapped Method -> in CarrierInspectionQuestionsPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}