using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Model.Yship;
using YPS.Service;

namespace YPS.ViewModel
{
    public class ImageViewModelView : IBase
    {
        public ObservableCollection<CustomPhotoModel> imageViews { get; set; }
        public ObservableCollection<UploadFiles> imageViews1 { get; set; }
        YPSService service = new YPSService();// Creating new instance of the YPSService, which is used to call API

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="photosList"></param>
        /// <param name="photoId"></param>
        public ImageViewModelView(ObservableCollection<CustomPhotoModel> photosList, int photoId)
        {
            try
            {
                imageViews = new ObservableCollection<CustomPhotoModel>();
                imageViews = photosList;
                pophoto = true;
                Yshipphoto = false;
                int index = photosList.IndexOf(photosList.Single(x => x.PhotoID == photoId));
                var getFirstValue = photosList.Where(X => X.PhotoID == photoId).FirstOrDefault();
                VisibleCardInx = index;
                Settings.SPhotoDescription = getFirstValue.PhotoDescription;

                DynamicTextChange(getFirstValue);
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
        /// <param name="photosList1"></param>
        /// <param name="photoId1"></param>
        public ImageViewModelView(ObservableCollection<UploadFiles> photosList1, int photoId1)
        {
            try
            {
                imageViews1 = new ObservableCollection<UploadFiles>();
                imageViews1 = photosList1;
                pophoto = false;
                Yshipphoto = true;
                int index = photosList1.IndexOf(photosList1.Single(x => x.ID == photoId1));
                var getFirstValue = photosList1.Where(X => X.ID == photoId1).FirstOrDefault();
                VisibleCardInx = index;
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ImageViewModelView constructor -> in ImageViewModelView.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method is for dynamic text change.
        /// </summary>
        /// <param name="photo"></param>
        private void DynamicTextChange(CustomPhotoModel photo)
        {
            try
            {
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> lblChangeVal = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (lblChangeVal.Count > 0)
                    {
                        photo.descriptionlbl = "Description";
                        var description = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == photo.descriptionlbl.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();

                        photo.descriptionlbl = (!string.IsNullOrEmpty(description) ? description : photo.descriptionlbl) + " : ";
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
        #endregion
    }
}
