using System;
using System.Collections.Generic;
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
    public partial class VinInspectQuestionsPage : ContentPage
    {
        VinInspectQuestionsPageViewModel Vm;
        AllPoData selectedTagData;
        YPSService service;
        bool isAllDone;

        //public VinInspectQuestionsPage(AllPoData selectedtagdata, int tagId, string tagNumber, string indentCode, string bagNumber)
        public VinInspectQuestionsPage(AllPoData selectedtagdata, bool isalldone)
        {
            try
            {
                InitializeComponent();
                service = new YPSService();
                Settings.IsRefreshPartsPage = true;
                selectedTagData = selectedtagdata;

                if (Device.RuntimePlatform == Device.iOS)// for adjusting the display as per the notch
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 30;
                    headerpart.Padding = safeAreaInset;
                }

                BindingContext = Vm = new VinInspectQuestionsPageViewModel(Navigation, this, selectedTagData, isalldone);
            }
            catch (Exception ex)
            {

            }
        }

        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                if (Vm.QuickTabVisibility == true)
                {
                    //await Vm.GetConfigurationResults(1);
                    Vm.QuickTabClicked();
                }
                else if (Vm.FullTabVisibility == true)
                {
                    //await Vm.GetConfigurationResults(2);
                    Vm.FullTabClicked();
                }
                else if (Vm.SignTabVisibility == true)
                {
                    Vm.SignTabClicked();
                }
            }
            catch (Exception ex)
            {

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
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked constructor -> in VinInspectQuestionsPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}