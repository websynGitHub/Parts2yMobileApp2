using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;

namespace YPS.ViewModel
{
    public class ImageViewModelView : IBase
    {
        INavigation Navigation;
        public ICommand HomeCommand { get; set; }
        public ObservableCollection<CustomPhotoModel> imageViews { get; set; }
        public ObservableCollection<PhotoRepoDBModel> repophotoimageViews { get; set; }
        public ObservableCollection<LoadPhotoModel> loadphotoimageViews { get; set; }
        public ObservableCollection<InspectionPhotosResponseListData> inspectionImageViews { get; set; }
        YPSService service = new YPSService();// Creating new instance of the YPSService, which is used to call API


        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        public ImageViewModelView(INavigation _Navigation, ObservableCollection<PhotoRepoDBModel> photosList, int photoId)
        {
            try
            {
                Navigation = _Navigation;
                repophotoimageViews = new ObservableCollection<PhotoRepoDBModel>();
                repophotoimageViews = photosList;
                IsRepoPhotosVisible = true;
                pophoto = false;
                Yshipphoto = false;
                IsInspectionPhotosVisible = false;
                int index = photosList.IndexOf(photosList.Single(x => x.FileID == photoId));
                var getFirstValue = photosList.Where(X => X.FileID == photoId).FirstOrDefault();
                VisibleCardInx = index;
                Settings.SPhotoDescription = getFirstValue.FileDescription;
                HomeCommand = new Command(HomeCommand_btn);

                DynamicTextChange();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ImageViewModelView constructor -> in ImageViewModelView.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        public ImageViewModelView(INavigation _Navigation, ObservableCollection<LoadPhotoModel> photosList, int photoId)
        {
            try
            {
                Navigation = _Navigation;
                loadphotoimageViews = new ObservableCollection<LoadPhotoModel>();
                loadphotoimageViews = photosList;
                IsLoadPhotosVisible = true;
                IsInspectionPhotosVisible = false;
                IsRepoPhotosVisible = false;
                pophoto = false;
                Yshipphoto = false;
                int index = photosList.IndexOf(photosList.Single(x => x.PhotoID == photoId));
                var getFirstValue = photosList.Where(X => X.PhotoID == photoId).FirstOrDefault();
                VisibleCardInx = index;
                Settings.SPhotoDescription = getFirstValue.PhotoDescription;
                HomeCommand = new Command(HomeCommand_btn);

                DynamicTextChange();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ImageViewModelView constructor -> in ImageViewModelView.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        public ImageViewModelView(INavigation _Navigation, ObservableCollection<InspectionPhotosResponseListData> photosList, int photoId)
        {
            try
            {
                Navigation = _Navigation;
                inspectionImageViews = new ObservableCollection<InspectionPhotosResponseListData>();
                inspectionImageViews = photosList;
                IsInspectionPhotosVisible = true;
                IsLoadPhotosVisible = false;
                IsRepoPhotosVisible = false;
                pophoto = false;
                Yshipphoto = false;
                int index = photosList.IndexOf(photosList.Single(x => x.ID == photoId));
                var getFirstValue = photosList.Where(X => X.ID == photoId).FirstOrDefault();
                VisibleCardInx = index;
                Settings.SPhotoDescription = getFirstValue.Remarks;
                HomeCommand = new Command(HomeCommand_btn);

                DynamicTextChange();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ImageViewModelView constructor -> in ImageViewModelView.cs " + Settings.userLoginID);
            }
        }


        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        public ImageViewModelView(INavigation _Navigation, ObservableCollection<CustomPhotoModel> photosList, int photoId)
        {
            try
            {
                Navigation = _Navigation;
                imageViews = new ObservableCollection<CustomPhotoModel>();
                imageViews = photosList;
                pophoto = true;
                Yshipphoto = false;
                IsInspectionPhotosVisible = false;
                IsRepoPhotosVisible = false;
                int index = photosList.IndexOf(photosList.Single(x => x.PhotoID == photoId));
                var getFirstValue = photosList.Where(X => X.PhotoID == photoId).FirstOrDefault();
                VisibleCardInx = index;
                Settings.SPhotoDescription = getFirstValue.PhotoDescription;
                HomeCommand = new Command(HomeCommand_btn);

                DynamicTextChange();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ImageViewModelView constructor -> in ImageViewModelView.cs " + Settings.userLoginID);
            }
        }


        /// <summary>
        /// Gets called when clicked on "Home" icon, to move to home page
        /// </summary>
        /// <param name="obj"></param>
        private async void HomeCommand_btn(object obj)
        {
            try
            {
                 Navigation.PopToRootAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HomeCommand_btn method -> in ImageViewModelView.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }

        }

        /// <summary>
        /// This method is for dynamic text change.
        /// </summary>
        /// <param name="photo"></param>
        private void DynamicTextChange()
        {
            try
            {
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> lblChangeVal = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (lblChangeVal.Count > 0)
                    {
                        var description = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == DescriptionLabel.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();

                        DescriptionLabel = (!string.IsNullOrEmpty(description) ? description : DescriptionLabel) + " : ";
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method -> in ImageViewModelView.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        #region Properties
        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                RaisePropertyChanged("BgColor");
            }
        }

        private string _DescriptionLabel = "Description";
        public string DescriptionLabel
        {
            get
            {
                return _DescriptionLabel;
            }
            set
            {
                _DescriptionLabel = value;
                NotifyPropertyChanged();
            }
        }


        private int _VisibleCardInx;
        public int VisibleCardInx
        {
            get
            {
                return _VisibleCardInx;
            }
            set
            {
                _VisibleCardInx = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ShowMoreTextpopup = false;
        public bool ShowMoreTextpopup
        {
            get { return _ShowMoreTextpopup; }
            set
            {
                _ShowMoreTextpopup = value;
                NotifyPropertyChanged();
            }
        }
        private string _ShowMoreTex;
        public string ShowMoreText
        {
            get { return _ShowMoreTex; }
            set
            {
                _ShowMoreTex = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ShowAndHideBtn = false;
        public bool ShowAndHideBtn
        {
            get { return _ShowAndHideBtn; }
            set
            {
                _ShowAndHideBtn = value;
                NotifyPropertyChanged();
            }
        }

        private bool _Yshipphoto = false;
        public bool Yshipphoto
        {
            get { return _Yshipphoto; }
            set
            {
                _Yshipphoto = value;
                NotifyPropertyChanged();
            }
        }
        private bool _pophoto = false;
        public bool pophoto
        {
            get { return _pophoto; }
            set
            {
                _pophoto = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsLoadPhotosVisible = false;
        public bool IsLoadPhotosVisible
        {
            get { return _IsLoadPhotosVisible; }
            set
            {
                _IsLoadPhotosVisible = value;
                NotifyPropertyChanged();
            }
        }
        private bool _IsRepoPhotosVisible = false;
        public bool IsRepoPhotosVisible
        {
            get { return _IsRepoPhotosVisible; }
            set
            {
                _IsRepoPhotosVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsInspectionPhotosVisible = false;
        public bool IsInspectionPhotosVisible
        {
            get { return _IsInspectionPhotosVisible; }
            set
            {
                _IsInspectionPhotosVisible = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
