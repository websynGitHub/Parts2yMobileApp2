using Syncfusion.XForms.Cards;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.ViewModel;
using YPS.Service;
using YPS.Model.Yship;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageView : ContentPage
    {
        #region Data member
        ImageViewModelView ImageVm;
        public ObservableCollection<UploadFiles> All_YshipphotosList = new ObservableCollection<UploadFiles>();
        YPSService service;
        #endregion


        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        /// <param name="Tags"></param>
        public ImageView(ObservableCollection<PhotoRepoModel> photosList, int photoId)
        {
            try
            {
                InitializeComponent();
                service = new YPSService();
                Groupname.Text = "Repo Photos";
                BindingContext = ImageVm = new ImageViewModelView(photosList, photoId);
                synfRepoImageViewList.WidthRequest = App.ScreenWidth;
                synfRepoImageViewList.HeightRequest = App.ScreenHeight;
                YPSLogger.TrackEvent("ImageView", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ImageViewPage";
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ImageView Constructor -> in ImageView.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        /// <param name="Tags"></param>
        public ImageView(ObservableCollection<LoadPhotoModel> photosList, int photoId, string Tags)
        {
            try
            {
                InitializeComponent();
                service = new YPSService();
                Groupname.Text = "Load Photos";
                BindingContext = ImageVm = new ImageViewModelView(photosList, photoId);
                synfLoadImageViewList.WidthRequest = App.ScreenWidth;
                synfLoadImageViewList.HeightRequest = App.ScreenHeight;
                YPSLogger.TrackEvent("ImageView", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ImageViewPage";
                Usernames.Text = Tags;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ImageView Constructor -> in ImageView.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        /// <param name="Tags"></param>
        public ImageView(ObservableCollection<CustomPhotoModel> photosList, int photoId, string Tags)
        {
            try
            {
                InitializeComponent();
                service = new YPSService();
                BindingContext = ImageVm = new ImageViewModelView(photosList, photoId);
                synfImageViewList.WidthRequest = App.ScreenWidth;
                synfImageViewList.HeightRequest = App.ScreenHeight;
                YPSLogger.TrackEvent("ImageView", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ImageViewPage";
                Usernames.Text = Tags;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ImageView Constructor -> in ImageView.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        /// <param name="Tags"></param>
        public ImageView(ObservableCollection<InspectionPhotosResponseListData> photosList, int photoId, string Tags)
        {
            try
            {
                InitializeComponent();
                service = new YPSService();
                BindingContext = ImageVm = new ImageViewModelView(photosList, photoId);
                synfImageViewList.WidthRequest = App.ScreenWidth;
                synfImageViewList.HeightRequest = App.ScreenHeight;
                YPSLogger.TrackEvent("ImageView", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ImageViewPage";
                Usernames.Text = Tags;
                Groupname.Text = "Inspection Photos";
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ImageView Constructor -> in ImageView.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
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

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        /// <param name="Tags"></param>
        public ImageView(ObservableCollection<UploadFiles> photosList, int photoId, string Tags)
        {
            try
            {
                InitializeComponent();
                service = new YPSService();
                BindingContext = ImageVm = new ImageViewModelView(photosList, photoId);
                //synfImageViewList1.WidthRequest = App.ScreenWidth;
                //synfImageViewList1.HeightRequest = App.ScreenHeight;
                YPSLogger.TrackEvent("ImageView", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ImageViewPage";
                All_YshipphotosList = photosList;
                Usernames.Text = Tags;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ImageView Constructor -> in ImageView.cs " + Settings.userLoginID);
                service.Handleexception(ex);
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
                (Xamarin.Forms.Application.Current.MainPage as MenuPage).IsGestureEnabled = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in ImageView.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when back icon is clicked.
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            (Xamarin.Forms.Application.Current.MainPage as MenuPage).IsGestureEnabled = true;
            return base.OnBackButtonPressed();
        }

        /// <summary>
        /// This method is for view image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SynfImageViewList_VisibleCardIndexChanged(object sender, VisibleCardIndexChangedEventArgs e)
        {
            try
            {
                var newCard = e.NewCard;
                var oldCard = e.OldCard;

                if (e.NewCard != null)
                {
                    var newmodelCV = (CustomPhotoModel)newCard.Content.BindingContext;
                    Settings.SPhotoDescription = newmodelCV.PhotoDescription;
                }
                if (e.NewCard == null)
                {
                    synfImageViewList.VisibleCardIndex = 0;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SynfImageViewList_VisibleCardIndexChanged method -> in ImageView.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is for viewing image.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SynfImageViewList1_VisibleCardIndexChanged(object sender, VisibleCardIndexChangedEventArgs e)
        {
            try
            {
                var newCard = e.NewCard;
                var oldCard = e.OldCard;

                if (e.NewCard == null)
                {
                    //synfImageViewList1.VisibleCardIndex = 0;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SynfImageViewList1_VisibleCardIndexChanged method -> in ImageView.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Show more", to view complete description.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMoreTextBtn(object sender, EventArgs e)
        {
            try
            {
                ImageVm.ShowMoreTextpopup = true;
                ImageVm.ShowMoreText = Settings.SPhotoDescription;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShowMoreTextBtn method -> in ImageView.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosePop(object sender, EventArgs e)
        {
            try
            {
                ImageVm.ShowMoreTextpopup = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClosePop method -> in ImageView.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
    }
}