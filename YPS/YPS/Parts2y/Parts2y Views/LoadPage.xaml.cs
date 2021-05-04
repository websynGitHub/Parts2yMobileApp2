using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YPS.Parts2y.Parts2y_Views;
using YPS.Parts2y.Parts2y_View_Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;
using Syncfusion.XForms.Buttons;
using YPS.Model;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using YPS.CustomToastMsg;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadPage : ContentPage
    {
        LoadPageViewModel Vm;
        YPSService service;
        AllPoData selectedTagData;
        public LoadPage(AllPoData selectedtagdata, SendPodata sendpodata, bool isalltasksdone)
        {
            try
            {
                InitializeComponent();
                selectedTagData = selectedtagdata;

                if (Settings.VersionID == 5 || Settings.VersionID == 1)
                {
                    loadStack.IsVisible = false;
                }

                //if (Device.RuntimePlatform == Device.iOS)
                //{
                //    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                //    safeAreaInset.Bottom = 0;
                //    safeAreaInset.Top = 20;
                //    headerpart.Padding = safeAreaInset;
                //}

                Settings.IsRefreshPartsPage = true;
                BindingContext = Vm = new LoadPageViewModel(Navigation, selectedtagdata, sendpodata, isalltasksdone, this);
                service = new YPSService();

                img.WidthRequest = App.ScreenWidth;
                img.HeightRequest = App.ScreenHeight - 150;
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Gets called when back icon is clicked, to redirect  to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped constructor -> in LoadPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void DoneClicked(object sender, EventArgs e)
        {
            try
            {
                if (selectedTagData.TaskID != 0)
                {
                    TagTaskStatus tagtaskstatus = new TagTaskStatus();
                    tagtaskstatus.TaskID = Helperclass.Encrypt(selectedTagData.TaskID.ToString());
                    tagtaskstatus.TaskStatus = 2;
                    tagtaskstatus.CreatedBy = Settings.userLoginID;

                    var result = await service.UpdateTaskStatus(tagtaskstatus);

                    if (result.status == 1)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");
                        Vm.IsPhotoUploadIconVisible = false;
                        Vm.closeLabelText = false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClicked constructor -> in LoadPage.xaml.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}