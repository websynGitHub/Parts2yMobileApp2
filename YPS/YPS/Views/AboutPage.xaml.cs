using System;
using Xamarin.Essentials;
using Xamarin.Forms;
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
                var currentVersion = VersionTracking.CurrentVersion;

                CVersion.Text = currentVersion;
                aboutbuild.Text = "This is a mobile solution app that allows effective communication between stakeholders in the supply chain, such as logistics, purchasing, production, sales and their suppliers and customers. This app will fill the gap between the location and the office worldwide.";

                if (currentVersion == Settings.iOSversion) { }

                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        if (currentVersion != Settings.iOSversion)
                        {
                            ios.IsVisible = true;
                            android.IsVisible = true;
                            updatetext.Text = "Update to latest version " + Settings.iOSversion + " from store";
                            updatetext.IsVisible = true;
                        }
                        else
                        {
                            ios.IsVisible = false;
                            android.IsVisible = false;
                            updatetext.IsVisible = false;
                        }

                        break;
                    case Device.Android:
                        if (currentVersion != Settings.AndroidVersion)
                        {
                            android.IsVisible = true;
                            ios.IsVisible = true;
                            updatetext.Text = "Update to latest version " + Settings.AndroidVersion + " from store";
                            updatetext.IsVisible = true;
                        }
                        else
                        {
                            android.IsVisible = false;
                            ios.IsVisible = false;
                            updatetext.IsVisible = false;

                        }
                        break;
                }
            }
            catch(Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "AboutPage constructor -> in AboutPage.cs " + Settings.userLoginID);
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