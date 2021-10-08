using SignaturePad.Forms;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    public partial class CVinInspectQuestionsPage : ContentPage
    {
        CVinInspectQuestionsPageViewModel Vm;
        AllPoData selectedTagData;
        YPSService service;
        bool isAllDone;

        public CVinInspectQuestionsPage(AllPoData selectedtagdata, bool isalldone)
        {
            try
            {
                service = new YPSService();
                InitializeComponent();
                Settings.IsRefreshPartsPage = true;
                selectedTagData = selectedtagdata;
                BindingContext = Vm = new CVinInspectQuestionsPageViewModel(Navigation, this, selectedTagData, isalldone);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CVinInspectQuestionsPage constructor -> in CVinInspectQuestionsPage.xaml.cs " + Settings.userLoginID);
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
                        Task.Run(() => Vm.QuickTabClicked()).Wait();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing OnAppearing -> in CVinInspectQuestionsPage.xaml.cs " + Settings.userLoginID);
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

                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        var result = await service.UpdateTagTaskStatus(tagtaskstatus);

                        if (result?.status == 1)
                        {
                            selectedTagData.TagTaskStatus = 2;

                            if (selectedTagData.TaskStatus != 2)
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
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked method -> in CVinInspectQuestionsPage.xaml.cs  " + Settings.userLoginID);
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
                                TaskID = selectedTagData.TaskID,
                                POTagID = selectedTagData.POTagID,
                                UserID = Settings.userLoginID,
                                Signature = base64,
                            };

                            if (Vm.Signature == "Driver")
                            {
                                inspobj.SignType = (int)InspectionSignatureType.VinDriver;


                                var sign = await service.InsertUpdateSignature(inspobj);
                            }
                            else if (Vm.Signature == "CarrierDriver")
                            {
                                inspobj.SignType = (int)InspectionSignatureType.CarrierDriver;


                                var sign = await service.InsertUpdateSignature(inspobj);
                            }
                            else if (Vm.Signature == "VINDealer")
                            {
                                inspobj.SignType = (int)InspectionSignatureType.VinDealer;


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
                YPSLogger.ReportException(ex, "ConfirmSignatureTapped OnAppearing -> in CVinInspectQuestionsPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}