using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomRenders;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.Views;
using YPS.YShip.YshipViewModel;

namespace YPS.YShip.YshipViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YshipPhotoUpload : MyContentPage
    {
        #region Data members declaration
        YshipPhotoUploadViewModel vm;
        YPSService service;
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        /// <param name="yshipid"></param>
        /// <param name="ybkgno"></param>
        /// <param name="completed"></param>
        /// <param name="canceled"></param>
        public YshipPhotoUpload(int yshipId, string ybkgNo, int completed, int canceled)
        {
            try
            {
                InitializeComponent();

                YPSLogger.TrackEvent("PhotoUpload", "Page constructer " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "PhotoUploadPage";
                service = new YPSService(); /// Creating new instance of the YPSService, which is used to call AIP
                BindingContext = vm = new YshipPhotoUploadViewModel(Navigation, this, yshipId, ybkgNo, completed, canceled);
                img.WidthRequest = App.ScreenWidth;
                img.HeightRequest = App.ScreenHeight - 150;

                #region Dynamic text change
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> lblChangeVal = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    var upload = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower() == "upload").Select(c => c.LblText).FirstOrDefault();

                    uploadBtn.Text = !string.IsNullOrEmpty(upload) ? upload : uploadBtn.Text;
                }
                #endregion
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YshipPhotoUpload constructor -> in PhotoUpload.cs  " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when page is loading
        /// </summary>
        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                /// Enable and Disble master detail page menu gesture
                (Application.Current.MainPage as YPSMasterPage).IsGestureEnabled = true;

                Settings.photoUploadPageCount = 0;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing constructor -> in PhotoUpload.cs  " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on the back button(Icon) and redirect to previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void BackTapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in PhotoUpload.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}