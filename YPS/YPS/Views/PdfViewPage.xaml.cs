﻿using System;
using Xamarin.Forms;
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
            YPSLogger.TrackEvent("PdfViewPage", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Settings.currentPage = "PdfViewPage";
                myBArray = printPDFModel.bArray;
                _printPDFModel = printPDFModel;
                setTitle.Text = printPDFModel.PDFFileTitle;

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PdfViewPage Constructor -> in PdfViewPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "OnAppearing Constructor -> in PdfViewPage.cs " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Home" icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToHome_Tapped(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }
    }
}