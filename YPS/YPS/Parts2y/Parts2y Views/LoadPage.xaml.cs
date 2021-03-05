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

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadPage : ContentPage
    {
        LoadPageViewModel Vm;
        YPSService service;

        public LoadPage(AllPoData selectedtagdata, SendPodata sendpodata)
        {
            try
            {
                InitializeComponent();

                if (Settings.CompanySelected.Contains("(P)") || Settings.CompanySelected.Contains("(E)"))
                {
                    loadStack.IsVisible = false;
                }

                if (Device.RuntimePlatform == Device.iOS)
                {
                    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                    safeAreaInset.Bottom = 0;
                    safeAreaInset.Top = 20;
                    headerpart.Padding = safeAreaInset;
                }

                Settings.IsRefreshPartsPage = true;
                BindingContext = Vm = new LoadPageViewModel(Navigation, selectedtagdata, sendpodata, this);
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
                YPSLogger.ReportException(ex, "Back_Tapped constructor -> in PhotoUpload.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}