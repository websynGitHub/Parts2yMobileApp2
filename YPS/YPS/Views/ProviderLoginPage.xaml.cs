using System;
using Xamarin.Auth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProviderLoginPage : ContentPage
    {
        #region Data member declaration
        public INavigation Navigation { get; set; }
        public string ProviderName { get; set; }
        public static string pagaload = "1";
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ProviderLoginPage()
        {
            try
            {
                InitializeComponent();
                YPSLogger.TrackEvent("ProviderLoginPage.xaml.cs", " in ProviderLoginPage constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ProviderLoginPage";
                ProviderName = "IIJ";
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ProviderLoginPage constructor -> in ProviderLoginPage.xaml.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when page is appearing.
        /// </summary>
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (pagaload == "1")
                    pagaload = "2";
                else
                    pagaload = "1";
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in ProviderLoginPage.xaml.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when user is leaving the page.
        /// </summary>
        protected async override void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnDisappearing method -> in ProviderLoginPage.xaml.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
        }

        public OAuth2Authenticator LoginWithTwitter()
        {
            return null;
        }
    }
}