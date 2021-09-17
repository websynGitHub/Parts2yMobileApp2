using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        #region Data members
        string appStoreUri;
        YPSService service = new YPSService();// Creating new instance of the YPSService, which is used to call AIP
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public AboutPage()
        {
            try
            {
                InitializeComponent();

                headerpart.BackgroundColor = Settings.Bar_Background;
                var currentVersion = VersionTracking.CurrentVersion;

                CVersion.Text = currentVersion;
                aboutbuild.Text = "This is a mobile solution app that allows effective communication between stakeholders in the supply chain, such as logistics, purchasing, production, sales and their suppliers and customers. This app will fill the gap between the location and the office worldwide.";

                if (currentVersion == Settings.iOSversion) { }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "AboutPage constructor -> in AboutPage.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on the back button and redirect to previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in AboutPage.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }


        /// <summary>
        /// Gets called when clicked on play store image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Playstore_Tapped(object sender, EventArgs e)
        {
            try
            {
                appStoreUri = "https://play.google.com/store/apps/details?id=com.synergies.parts2yprod";
                Device.OpenUri(new Uri(appStoreUri));
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "Playstore_Tapped method -> in AboutPage.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on Apple store image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void appstore_Tapped(object sender, EventArgs e)
        {
            try
            {
                appStoreUri = "https://apps.apple.com/us/app/parts2y/id1500708846?ls=1";
                Device.OpenUri(new Uri(appStoreUri));
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "appstore_Tapped method -> in AboutPage.cs " + Settings.userLoginID);
            }
        }
    }
}