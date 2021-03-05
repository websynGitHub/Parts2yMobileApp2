using System;
using System.Collections.ObjectModel;
using System.Linq;
using YPS.Helpers;
using YPS.Service;
using YPS.ViewModels;
using YPS.CommonClasses;
using System.Drawing;

namespace YPS.ViewModel
{
    public class FileViewViewModel : IBase
    {
        public ObservableCollection<ChatMessageViewModel> photoListData { get; set; }
        YPSService service = new YPSService();// Creating new instance of the YPSService, which is used to call API

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="selectedItem"></param>
        /// <param name="photoList"></param>
        public FileViewViewModel(ChatMessageViewModel selectedItem, ObservableCollection<ChatMessageViewModel> photoList)
        {
            try
            {
                photoListData = new ObservableCollection<ChatMessageViewModel>();
                photoListData = photoList;
                int index = photoList.IndexOf(photoList.Single(x => x.Image == selectedItem.Image));
                VisibleCardInx = index;
            }
            catch(Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "BackBtnGuidedT method -> in CoachMGuidedTViewModel.cs " + Settings.userLoginID);
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
        #endregion
    }
}
