using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PdfViewPage : ContentPage
    {
        #region Data member
        private readonly PrintPDFModel _printPDFModel;
        Byte[] myBArray;
        YPSService service = new YPSService();
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="printPDFModel"></param>
        public PdfViewPage(PrintPDFModel printPDFModel)
        {
            InitializeComponent();

            YPSLogger.TrackEvent("PdfViewPage.xaml.cs", " Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                headerpart.BackgroundColor = Settings.Bar_Background;
                Settings.currentPage = "PdfViewPage";
                myBArray = printPDFModel.bArray;
                _printPDFModel = printPDFModel;
                setTitle.Text = printPDFModel.PDFFileTitle;

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PdfViewPage constructor -> in PdfViewPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when page is appearing.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (await FileManager.ExistsAsync(_printPDFModel.FileName) == false)
                {
                    await FileManager.GetByteArrayData(_printPDFModel);
                }
                PdfDocView.Uri = FileManager.GetFilePathFromRoot(_printPDFModel.FileName);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing constructor -> in PdfViewPage.xaml.cs " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on back icon, to redirect to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackTapped(object sender, EventArgs e)
        {
            YPSLogger.TrackEvent("PdfViewPage.xaml.cs", " In AddToChatClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Settings.mutipleTimeClick = false;
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BackTapped method -> in PdfViewPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Home" icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToHome_Tapped(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GoToHome_Tapped method -> in PdfViewPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
    }
}