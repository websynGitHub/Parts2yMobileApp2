using System;
using System.Collections.ObjectModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomControls;
using YPS.Helpers;
using YPS.Service;
using YPS.ViewModel;
using YPS.ViewModels;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileView : ContentPage
    {
        #region Data member declaration
        public string extension;
        WebView browser;
        ZoomImage image;
        YPSService trackService = new YPSService();
        ChatMessageViewModel selectedItem1 = new ChatMessageViewModel();
        FileViewViewModel FViewVM;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <param name="PhotosData"></param>
        /// <param name="TitleChat"></param>
        /// <param name="Chattags"></param>
        public FileView(ChatMessageViewModel selectedItem, ObservableCollection<ChatMessageViewModel> PhotosData, string TitleChat, string Chattags)
        {
            try
            {
                InitializeComponent();
                YPSLogger.TrackEvent("FileView", "Page with 4 param constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "chatPage";
                Groupname.Text = TitleChat;
                Usernames.Text = Chattags;
                BindingContext = FViewVM = new FileViewViewModel(selectedItem, PhotosData);
                imageViewList.WidthRequest = App.ScreenWidth;
                imageViewList.HeightRequest = App.ScreenHeight;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FileView constructor with 4 param-> in FileView.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="url"></param>
        public FileView(string url)
        {
            YPSLogger.TrackEvent("FileView", "Page with 1 param constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Settings.currentPage = "chatPage";
                Title = "Preview";
                BackgroundColor = Color.White;
                extension = Path.GetExtension(url);
                browser = new WebView
                {
                    Source = "http://drive.google.com/viewerng/viewer?embedded=true&url=" + url,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,

                };
                Content = new StackLayout
                {

                    Children ={


                    browser,
                      }
                };
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FileView constructor with 1 param-> in FileView.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when page is appearing.
        /// </summary>
        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                (Application.Current.MainPage as YPSMasterPage).IsGestureEnabled = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in FileView.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on image to view it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageViewList_VisibleCardIndexChanged(object sender, Syncfusion.XForms.Cards.VisibleCardIndexChangedEventArgs e)
        {
            try
            {
                if (e.NewCard == null)
                {
                    imageViewList.VisibleCardIndex = 0;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ImageViewList_VisibleCardIndexChanged method -> in FileView.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }
    }
}