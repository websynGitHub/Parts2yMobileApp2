using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using YPS.Service;

namespace YPS.Model
{
    public class AllPoData : IBase
    {
        #region Properties
        public string emptyCellValue { get; set; } = string.Empty;
        public bool isColumnHidden { get; set; } = true;
        public string ShippingNumber { get; set; }
        public string DisciplineName { get; set; }
        public string ELevelName { get; set; }
        public string PONumber { get; set; }
        public string REQNo { get; set; }
        public string PODescription { get; set; }
        public string SupplierCompanyName { get; set; }
        public string ManufacturerCompanyName { get; set; }
        public string CompanyName { get; set; }
        public string ProjectName { get; set; }
        public string CountryName { get; set; }
        public string PriorityName { get; set; }
        public string ExpeditorName { get; set; }
        public int PLUploadCount { get; set; }
        public int Unit { get; set; }
        public double M3 { get; set; }
        public double Kgs { get; set; }
        public int QACount { get; set; }
        public int FileCount { get; set; }
        public int BPhotoCount { get; set; }
        public int APhotoCount { get; set; }
        public int TagsPrintCount { get; set; }
        public int POID { get; set; }
        public int POTagID { get; set; }
        public string POShippingNumber { get; set; }
        public string InvoiceNumber { get; set; }


        public string TagNumber { get; set; }
        public string TagDescription { get; set; }
        //public int TagQACount { get; set; }

        private int _TagQACount;
        public int TagQACount
        {
            get
            {
                return _TagQACount;
            }
            set
            {
                this._TagQACount = value;
                RaisePropertyChanged("TagQACount");
            }
        }
        public int PUID { get; set; }
        private int _TagBPhotoCount;
        public int TagBPhotoCount
        {
            get
            {
                return _TagBPhotoCount;
            }
            set
            {
                this._TagBPhotoCount = value;
                RaisePropertyChanged("TagBPhotoCount");
            }
        }
        private int _TagAPhotoCount;
        public int TagAPhotoCount
        {
            get
            {
                return _TagAPhotoCount;
            }
            set
            {
                this._TagAPhotoCount = value;
                RaisePropertyChanged("TagAPhotoCount");
            }
        }

        private Color _SelectedTagBorderColor = Color.Transparent;
        public Color SelectedTagBorderColor
        {
            get
            {
                return _SelectedTagBorderColor;
            }
            set
            {
                this._SelectedTagBorderColor = value;
                RaisePropertyChanged("SelectedTagBorderColor");
            }
        }

        private bool _IsChecked;
        public bool IsChecked
        {
            get
            {
                return _IsChecked;
            }
            set
            {
                this._IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }


        public int FUID { get; set; }

        private int _TagFilesCount;
        public int TagFilesCount
        {
            get
            {
                return _TagFilesCount;
            }
            set
            {
                this._TagFilesCount = value;
                RaisePropertyChanged("TagFilesCount");
            }
        }

        private Color _CameraIconColor;
        public Color CameraIconColor
        {
            get
            {
                return _CameraIconColor;
            }
            set
            {
                this._CameraIconColor = value;
                RaisePropertyChanged("CameraIconColor");
            }
        }

        public int TagQAClosedCount { get; set; }
        public int ISFileClosed { get; set; }
        public int ISPhotoClosed { get; set; }
        public int ISFinalVol { get; set; }
        public int IsPhotoRequired { get; set; }
        public string ROS { get; set; }
        public int? POS { get; set; }
        public int? SUB { get; set; }
        public string SIZE1 { get; set; }
        public string SIZE2 { get; set; }
        public string SIZE3 { get; set; }
        public string SIZE4 { get; set; }
        public string COMMODITY_CODE { get; set; }
        public string IDENT_DEVIATED_TAG_DESC { get; set; }
        public string ShippingNumberEncrypted { get; set; }
        public string ConditionName { get; set; }
        public int QAClosedCount { get; set; }
        public string IsReadytoyShipEncrypted { get; set; }
        public string POAck { get; set; }
        public string Remarks { get; set; }
        public string POIDEncrypted { get; set; }
        public string IdentCode { get; set; }
        public string BagNumber { get; set; }
        public int IsReadytoyShip { get; set; }
        public int ISTagPrinted { get; set; }
        public string yBkgNumber { get; set; }
        public string TaskName { get; set; }
        public int TaskID { get; set; }
        public List<int> TaskIDList { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public int TagTaskStatus { get; set; }
        public int TaskStatus { get; set; }
        public string POTaskStatusIcon { get; set; }
        public string TagTaskStatusIcon { get; set; }

        public int TaskResourceID { get; set; }
        public string TaskResourceName { get; set; }
        public string TaskResourceRole { get; set; }

        public bool _IsTaskResourceVisible { set; get; }
        public bool IsTaskResourceVisible
        {
            get
            {
                return _IsTaskResourceVisible;
            }
            set
            {
                this._IsTaskResourceVisible = value;
                RaisePropertyChanged("IsTaskResourceVisible");
            }
        }


        private bool _IsTagDescLabelVisible { set; get; }
        public bool IsTagDescLabelVisible
        {
            get
            {
                return _IsTagDescLabelVisible;
            }
            set
            {
                this._IsTagDescLabelVisible = value;
                RaisePropertyChanged("IsTagDescLabelVisible");
            }
        }

        private bool _IsConditionNameLabelVisible { set; get; }
        public bool IsConditionNameLabelVisible
        {
            get
            {
                return _IsConditionNameLabelVisible;
            }
            set
            {
                this._IsConditionNameLabelVisible = value;
                RaisePropertyChanged("IsConditionNameLabelVisible");
            }
        }


        public bool _IsFilesVisible { set; get; }
        public bool IsFilesVisible
        {
            get
            {
                return _IsFilesVisible;
            }
            set
            {
                this._IsFilesVisible = value;
                RaisePropertyChanged("IsFilesVisible");
            }
        }

        public bool _IsPhotosVisible { set; get; }
        public bool IsPhotosVisible
        {
            get
            {
                return _IsPhotosVisible;
            }
            set
            {
                this._IsPhotosVisible = value;
                RaisePropertyChanged("IsPhotosVisible");
            }
        }

        public bool _IsChatsVisible { set; get; }
        public bool IsChatsVisible
        {
            get
            {
                return _IsChatsVisible;
            }
            set
            {
                this._IsChatsVisible = value;
                RaisePropertyChanged("IsChatsVisible");
            }
        }
        #endregion

        #region Fields for Mobile 
        private double _imgCamOpacityA { set; get; } = 1.0;
        public double imgCamOpacityA
        {
            get
            {
                return _imgCamOpacityA;
            }
            set
            {
                this._imgCamOpacityA = value;
                RaisePropertyChanged("imgCamOpacityA");
            }
        }
        private double _imgCamOpacityB { set; get; } = 1.0;
        public double imgCamOpacityB
        {
            get
            {
                return _imgCamOpacityB;
            }
            set
            {
                this._imgCamOpacityB = value;
                RaisePropertyChanged("imgCamOpacityB");
            }
        }
        private double _imgtickOpacityA { set; get; } = 1.0;
        public double imgtickOpacityA
        {
            get
            {
                return _imgtickOpacityA;
            }
            set
            {
                this._imgtickOpacityA = value;
                RaisePropertyChanged("imgtickOpacityA");
            }
        }

        private double _imgTickOpacityB { set; get; } = 1.0;
        public double imgTickOpacityB
        {
            get
            {
                return _imgTickOpacityB;
            }
            set
            {
                this._imgTickOpacityB = value;
                RaisePropertyChanged("imgTickOpacityB");
            }
        }

        public bool ForMultiCheck { set; get; } = false;
        public string chatImage { set; get; }
        public bool chatTickVisible { set; get; }
        private bool _photoTickVisible;
        public bool photoTickVisible
        {
            get
            {
                return _photoTickVisible;
            }
            set
            {
                this._photoTickVisible = value;
                RaisePropertyChanged("photoTickVisible");
            }
        }
        private bool _fileTickVisible { set; get; }
        public bool fileTickVisible
        {
            get
            {
                return _fileTickVisible;
            }
            set
            {
                this._fileTickVisible = value;
                RaisePropertyChanged("fileTickVisible");
            }
        }
        public bool countVisible { set; get; }

        private string _cameImage;
        public string cameImage
        {
            get
            {
                return _cameImage;
            }
            set
            {
                this._cameImage = value;
                RaisePropertyChanged("cameImage");
            }
        }


        private string _fileImage { set; get; }
        public string fileImage
        {
            get
            {
                return _fileImage;
            }
            set
            {
                this._fileImage = value;
                RaisePropertyChanged("fileImage");
            }
        }

        public bool _filecountVisible { set; get; }
        public bool filecountVisible
        {
            get
            {
                return _filecountVisible;
            }
            set
            {
                this._filecountVisible = value;
                RaisePropertyChanged("filecountVisible");
            }
        }
        private bool _APhotoCountVisible;
        public bool APhotoCountVisible
        {
            get
            {
                return _APhotoCountVisible;
            }
            set
            {
                this._APhotoCountVisible = value;
                RaisePropertyChanged("APhotoCountVisible");
            }
        }
        private bool _BPhotoCountVisible { set; get; }
        public bool BPhotoCountVisible
        {
            get
            {
                return _BPhotoCountVisible;
            }
            set
            {
                this._BPhotoCountVisible = value;
                RaisePropertyChanged("BPhotoCountVisible");
            }
        }
        private bool _isClosed;
        public bool IsClosed
        {
            get
            {
                return _isClosed;
            }
            set
            {
                this._isClosed = value;
                RaisePropertyChanged("IsClosed");
            }
        }


        //public string boolImage { set; get; } = "unchecked_ic.png";
        #endregion
    }

    public class GridData
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<AllPoData> data { get; set; }

        public GridData()
        {
            data = new List<AllPoData>();
        }
    }
    public class ColumnInfo
    {
        public string headerText { set; get; }
        public string mappingText { set; get; }
        public bool? check { set; get; }

        public string Column { get; set; }
        public bool Value { get; set; }
    }

    public class ColumnInfoSave
    {
        public string Column { get; set; }
        public bool Value { get; set; }
    }
    public class Header
    {
        public string REQNumber { get; set; }
        public string PONumber { get; set; }
        public string ShippingNumber { get; set; }
        public int DisciplineID { get; set; }
        public int ELevelID { get; set; }
        public string Condition { get; set; }
        public string Expeditor { get; set; }
        public int PriorityID { get; set; }
    }

    public class SendPodata
    {
        public int UserID { get; set; }
        public string PONumber { get; set; }
        public string REQNo { get; set; }
        public string ShippingNo { get; set; }
        public int DisciplineID { get; set; }
        public int ELevelID { get; set; }
        public int ConditionID { get; set; }
        public int ExpeditorID { get; set; }
        public int PriorityID { get; set; }
        public int ResourceID { get; set; }
        public string TagNo { get; set; }
        public string IdentCode { get; set; }
        public string LocationDeliverPlace { get; set; }
        public string LocationPOL { get; set; }
        public string LocationPOD { get; set; }
        public int LocationOnsite { get; set; }
        public int Onsite { get; set; }
        public int OnsitePlaceID { get; set; }
        public int StartPage { get; set; }
        public int PageSize { get; set; }
        public string BagNo { get; set; }
        public string SearchText { get; set; }
        public int Status { get; set; }
        public string yBkgNumber { get; set; }
        public string TaskName { get; set; }
    }

    public class POData
    {
        public ObservableCollection<object> poData { get; set; }
        public ObservableCollection<object> poTags { get; set; }
        public ObservableCollection<AllPoData> allPoData { get; set; }
        public int listCount { get; set; }
    }

    public class GetPoData
    {
        public string message { get; set; }
        public int status { get; set; }
        public POData data { get; set; }

        public GetPoData()
        {
            data = new POData();
        }
    }

    public class Getuserdata
    {
        public string message { get; set; }
        public int status { get; set; }
        public ObservableCollection<NameIfo> data { get; set; }

        public Getuserdata()
        {
            data = new ObservableCollection<NameIfo>();
        }
    }

    public class NameIfo
    {
        public int POID { get; set; }
        public int QAID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int Status { get; set; }
        public int UserCount { get; set; }
        public int RoleID { get; set; }
        public int ISCurrentUser { get; set; }
        public int CreatedBy { get; set; }
        public bool check { get; set; }
        public string Title { get; set; }
        public bool Iscurentuser { get; set; } = true;
        public string img { get; set; }
        public bool checkType { get; set; }
        public Xamarin.Forms.Color IconColor { get; set; }

    }
    public class users
    {
        public int Status { set; get; }
        public int UserId { set; get; }
    }

    public class photoUplodeInfo
    {
        public Xamarin.Forms.ImageSource base64img { set; get; }
    }

    public class TagTaskStatus
    {
        public string TaskID { set; get; }
        public string POTagID { set; get; }
        public int Status { set; get; }
        public int TaskStatus { set; get; }
        public int CreatedBy { set; get; }
    }

    #region For click on chat from main datagrid , to navigate 

    public class TagsData
    {
        public int POTagID { set; get; }
    }

    public class UsersData
    {
        public int UserID { set; get; }
        public int Status { set; get; }
    }
    #endregion
}
