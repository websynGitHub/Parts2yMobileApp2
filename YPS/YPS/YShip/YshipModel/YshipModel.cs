using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using YPS.Service;

namespace YPS.Model.Yship
{
    public class YshipModel : IBase
    {
        #region MainModel
        public int listCount { get; set; }
        public string emptyCellValue { get; set; } = string.Empty;
        private bool _IsChecked;
        public bool IsChecked
        {
            get { return _IsChecked; }
            set
            {
                _IsChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }
        public int yShipId { get; set; }
        public string yShipIdEncrypted { get; set; }
        public string yBkgNumber { get; set; }
        public string yShipBkgNumber { get; set; }
        public int CompanyID { get; set; }
        public int PLFinalId { get; set; }
        public string BkgRefNo { get; set; }
        public int BkgConfirmed { get; set; }
        public int Cancel { get; set; }
        public int Complete { get; set; }
        public object ShmtType { get; set; }
        public object EqmtType1 { get; set; }
        public int EqmtType1Qty { get; set; }
        public object EqmtType2 { get; set; }
        public object EqmtType2Qty { get; set; }
        public object EqmtType3 { get; set; }
        public object EqmtType3Qty { get; set; }
        public int TotalEqmtQty { get; set; }
        public object PickupPIC { get; set; }
        public object DestinationPIC { get; set; }
        public object Shipper { get; set; }
        public object OrgSearch { get; set; }
        public object OrgCntryCode { get; set; }
        public object OrgCntryName { get; set; }
        public object OrgLocCode { get; set; }
        public object OrgLocName { get; set; }
        public object PickupLocation { get; set; }
        public object ETD { get; set; }
        public object PickUpTime { get; set; }
        public object ShipToParty { get; set; }
        public object DestSearch { get; set; }
        public object DestCntryCode { get; set; }
        public object DestCntryName { get; set; }
        public object DestLocCode { get; set; }
        public object DestLocName { get; set; }
        public object DeliveryLocation { get; set; }
        public object ETA { get; set; }
        public object DeliveryTime { get; set; }
        public object InvNo { get; set; }
        public object CustRefNo { get; set; }
        public object ShmtTerm { get; set; }
        public object SpecialCargoHandlingInstructions { get; set; }
        public object GeneralDescription { get; set; }
        public object Currency { get; set; }
        public double TotalAmount { get; set; }
        public int TtlPkg { get; set; }
        public object PkgUnit { get; set; }
        public double TtlM3 { get; set; }
        public double TtlGrossWt { get; set; }
        public double TtlNetWt { get; set; }
        public object ShipmentLoadedBy { get; set; }
        public object PickUpHubLoc { get; set; }
        public object OtherPickupLoc { get; set; }
        public object EstPICKUPDate { get; set; }
        public object EstPICKUPtime { get; set; }
        public object ActPICKUPDate { get; set; }
        public object ActPICKUPTime { get; set; }
        public object OriginalRequestedPickupDate { get; set; }
        public object OriginalRequestedPickupTime { get; set; }
        public object ViaHubLoc { get; set; }
        public object OtherViaLoc { get; set; }
        public object ETAViaLoc { get; set; }
        public object ATAViaLoc { get; set; }
        public object FinalDestHubLoc { get; set; }
        public object OtherFinalDestLoc { get; set; }
        public object ETADest { get; set; }
        public object ATADest { get; set; }
        public object OriginalRequestedDeliveyDate { get; set; }
        public object OriginalRequestedDeliveyTime { get; set; }
        public object ShipmentOpenedby { get; set; }
        public int CreatedBy { get; set; }
        public object CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public object UpdatedDate { get; set; }
        public int IsACT { get; set; }
        public int IsBkgConfirmed { get; set; }
        public int IsOTLBkg { get; set; }
        public int IsInv_P_L { get; set; }
        public int IsESTPickUpTime { get; set; }
        public int IsATD { get; set; }
        public int IsETA { get; set; }
        public int IsATA { get; set; }
        public int IsETD { get; set; }
        public string ETD1 { get; set; }
        public int IsPickUpInfo { get; set; }
        public int IsDeliveryInfo { get; set; }
        public int IsExportBkg { get; set; }
        public int IsImportBkg { get; set; }
        public int InvoicePLCount { get; set; }
        public int PhotoCount { get; set; }
        public int PermitCount { get; set; }

        /// <summary>
        /// yShip QA Count
        /// </summary>
        private int _yShipQACount;
        public int yShipQACount
        {
            get
            {
                return _yShipQACount;
            }
            set
            {
                this._yShipQACount = value;
                RaisePropertyChanged("yShipQACount");
            }
        }

        /// <summary>
        /// yShip QA Closed Count
        /// </summary>
        public int yShipQAClosedCount { get; set; }
        public string chatImage { set; get; }
        public string ActImage { set; get; }
        public Color FlagColor { set; get; }
        public string BkgConfirmedImage { set; get; }
        public Color BkgConfirmedColor { set; get; }
        public string CancelConfirmedImage { set; get; }
        public Color CancelConfirmedColor { set; get; }
        public string CompleteConfirmedImage { set; get; }
        public Color CompleteConfirmedColor { set; get; }
        public string OTLBkgImage { set; get; }
        public Color OTLBkgColor { set; get; }
        public string IsInv_P_LImage { set; get; }
        public Color IsInv_P_LColor { set; get; }
        public string ExportBkgImage { set; get; }
        public Color ExportBkgColor { set; get; }
        public string ImportBkgImage { set; get; }
        public Color ImportBkgColor { set; get; }
        public string EstP_UpTimeImage { set; get; }
        public Color EstP_UpTimeColor { set; get; }
        public string P_UpTimeImage { set; get; }
        public Color P_UpTimeColor { set; get; }
        public string ATDImage { set; get; }
        public Color ATDColor { set; get; }
        public string ETAImage { set; get; }
        public Color ETAColor { set; get; }
        public string DeliveryInfoImage { set; get; }
        public Color DeliveryInfoColor { set; get; }
        public string ATAImage { set; get; }
        public Color ATAColor { set; get; }
        public string ETDBgColor { get; set; }
        public string ETDTextColor { get; set; }
        public bool chatTickVisible { set; get; }
        public bool countVisible { set; get; }
        public bool minusVisible { set; get; }
        public bool FIlecountVisible { set; get; }
        public bool permitminusVisible { set; get; }
        public bool permitFIlecountVisible { set; get; }
        public bool invoiceminusVisible { set; get; }
        public bool invoiceFIlecountVisible { set; get; }
        public string ShippingNumber { get; set; }
        public string Company { get; set; }
        public int OrgLocationID { get; set; }
        public int DestLocationID { get; set; }
        public object EstDeliveryDate { get; set; }
        public object EstDeliveryTime { get; set; }
        public object ActDeliveryDate { get; set; }
        public object ActDeliveryTime { get; set; }
        public int PickUpHubLocID { get; set; }
        public int FinalDestHubLocID { get; set; }
        public List<object> yshipDates { get; set; }

        #endregion
    }

    public class YShipGridData
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<YshipModel> data { get; set; }
        public YShipGridData()
        {
            data = new List<YshipModel>();
        }
    }

    public class GetYshipData
    {
        public string message { get; set; }
        public int status { get; set; }
        //public YshipData data { get; set; }
        public ObservableCollection<YshipModel> data { get; set; }
        public GetYshipData()
        {
            data = new ObservableCollection<YshipModel>();
        }
    }

    public class UploadFiles
    {
        public int ID { get; set; }
        public int yShipId { get; set; }
        public int UploadType { get; set; }
        public string AttachmentName { get; set; }
        public string PermitNo { get; set; }
        public string PermitDate { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedDate { get; set; }
        public int listCount { get; set; }
        public string GivenName { get; set; }
        public string FileURL { get; set; }
        public string FullName { get; set; }
        public string MyImage { get; set; }
    }

    public class YshipUploadedPhotosList<T>
    {
        public object allUploads { get; set; }
        public ObservableCollection<T> beforeLoading { get; set; }
        public ObservableCollection<T> afterLoading { get; set; }
        public ObservableCollection<T> afterOpening { get; set; }
        public ObservableCollection<T> invoice { get; set; }
        public ObservableCollection<T> packingList { get; set; }
        public ObservableCollection<T> exportPermit { get; set; }
        public ObservableCollection<T> transitPermit { get; set; }
        public ObservableCollection<T> importPermit { get; set; }
        public YshipUploadedPhotosList()
        {
            beforeLoading = new ObservableCollection<T>();
            afterLoading = new ObservableCollection<T>();
            afterOpening = new ObservableCollection<T>();
            invoice = new ObservableCollection<T>();
            packingList = new ObservableCollection<T>();
            exportPermit = new ObservableCollection<T>();
            transitPermit = new ObservableCollection<T>();
            importPermit = new ObservableCollection<T>();
        }
    }

    public class GetYshipFiles
    {
        public string message { get; set; }
        public int status { get; set; }
        public YshipUploadedPhotosList<UploadFiles> data { get; set; }
    }

    /// <summary>
    /// YShip File upload ressponse
    /// </summary>
    public class yShipFileUploadResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public UploadFiles data { get; set; }
    }

    public class InVoiceDeleteFile
    {
        public string message { get; set; }
        public int status { get; set; }
        public string data { get; set; }
    }

    public class yShipSearch
    {
        public int UserID { get; set; }
        public string SearchText { get; set; }
        public int StartPage { get; set; }
        public int PageSize { get; set; }
        public int Status { get; set; }
        /// <summary>
        /// yBkgNumber
        /// </summary>
        public string yBkgNumber { get; set; }

        /// <summary>
        /// yShipBkgNumber
        /// </summary>
        public string yShipBkgNumber { get; set; }

        /// <summary>
        /// ShippingNumber
        /// </summary>
        public string ShippingNumber { get; set; }

        /// <summary>
        /// BkgRefNo
        /// </summary>
        public string BkgRefNo { get; set; }

        /// <summary>
        /// BkgConfirmed
        /// </summary>
        public int BkgConfirmed { get; set; }

        /// <summary>
        /// BkgConfirmed Text
        /// </summary>
        public int BkgConfirmedText { get; set; }

        /// <summary>
        /// Cancel
        /// </summary>
        public int Cancel { get; set; }

        /// <summary>
        /// Complete
        /// </summary>
        public int Complete { get; set; }

        /// <summary>
        /// EqmtTypeID
        /// </summary>
        public int EqmtTypeID { get; set; }

        /// <summary>
        /// OrgLocationID
        /// </summary>
        public int OrgLocationID { get; set; }

        /// <summary>
        /// OrgLocationID
        /// </summary>
        public string OrgLocation { get; set; }

        /// <summary>
        /// DestLocationID
        /// </summary>
        public int DestLocationID { get; set; }

        /// <summary>
        /// DestLocationID
        /// </summary>
        public string DestLocation { get; set; }

        /// <summary>
        /// ETD
        /// </summary>
        public string ETD { get; set; }

        /// <summary>
        /// ETA
        /// </summary>
        public string ETA { get; set; }

        /// <summary>
        /// PickUpTime
        /// </summary>
        public string PickUpTime { get; set; }

        /// <summary>
        /// DeliveryTime
        /// </summary>
        public string DeliveryTime { get; set; }
    }
}
