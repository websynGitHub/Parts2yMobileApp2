using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using YPS.Model;

namespace YPS.CommonClasses
{
    public static class Settings
    {
        public static ObservableCollection<AllPoData> AllPOData { get; set; }
        public static string ScanValue { get; set; }
        //public static string ImpTxtCode { get; set; } = "TDdmwPkI+5H5TLnqUfTTow==";
        public static Color Bar_Background { get; set; } = Xamarin.Forms.Color.FromHex("#269DC9");
        public static string DateFormat { get; set; } = "{0:dd MMM yyyy HH:mm}";
        public static Color HighlightedTabTxtColor { get; set; } = Color.Green;
        public static bool EndRefresh2 { get; set; }
        public static string IIJToken { get; set; }
        public static bool IsIIJEnabled { get; set; }
        public static bool IsPNEnabled { get; set; }
        public static bool IsEmailEnabled { get; set; }
        public static string token_type { get; set; }
        public static string expires_in { get; set; }
        public static string id_token { get; set; }
        public static string IIJLoginID { get; set; }
        public static int CountRefresh { get; set; } = 0;

        public static string IIJConsumerKey { get; set; }
        public static string IIJConsumerSecret { get; set; }
        public static string AndroidVersion { get; set; }
        public static string iOSversion { get; set; }

        public static string BlobConnection { get; set; }
        public static string BlobStorageConnectionString { get; set; }
        public static int PhotoSize { get; set; }
        public static string APIURL { get; set; }
        public static int CompressionQuality { get; set; }

        public static string userid;

        public static string Username { get; set; }
        public static string encryUsername { get; set; }
        public static string SGivenName { get; set; }
        public static string encrySGivenName { get; set; }
        public static int userLoginID { get; set; }
        public static string encryUserLoginID { get; set; }
        public static int LanguageID { get; set; }
        public static string encryLanguageID { get; set; }
        public static string LanguageName { get; set; }
        public static int userRoleID { get; set; }
        public static int ISCurrentUser { get; set; }
        public static string UserMail { get; set; }
        public static string ChatClosedOrNot { get; set; }
        public static string encryUserMail { get; set; }
        public static string EntityName { get; set; }
        public static string EntityTypeName { get; set; }
        public static string encryEntityName { get; set; }
        public static string RoleName { get; set; }
        public static string encryRoleName { get; set; }
        public static string Sessiontoken { get; set; }
        public static string dateBaseOnPickerVal { get; set; }

        public static int filterPageCount { set; get; } = 0;

        public static int PoId { set; get; }
        public static int QaId { set; get; }
        public static string tagnumbers { set; get; }
        public static string chatgroupname { set; get; }
        public static string PhotoOption { get; set; } = "P";
        public static int isFinalvol { get; set; } = 0;
        public static string Tagnumbers { get; set; } = string.Empty;

        #region Default set profile

        public static string CompanySelected { get; set; }
        public static string ProjectSelected { get; set; }
        public static string JobSelected { get; set; }
        //public static string SupplierSelected { get; set; } = "ALL";


        public static int CompanyID { get; set; }
        public static int ProjectID { get; set; }
        public static int JobID { get; set; }
        //public static int SupplierID { get; set; }
        public static int FilterParentID { get; set; }

        #endregion
        public static int qaListPageCount { set; get; } = 0;
        public static int photoUploadPageCount { set; get; } = 0;
        public static int fileUploadPageCount { set; get; } = 0;
        public static int chatPageCount { set; get; } = 0;
        public static int settingPageCount { set; get; } = 0;
        public static int chatstatus { set; get; }
        public static int ChatUserCount { set; get; }
        public static int ChatuserCountImgHide { get; set; }


        public static int AphotoCount { set; get; }
        public static int BphotoCount { set; get; }
        public static int FilesCount { set; get; }
        public static int currentPuId { set; get; }
        public static int currentFuId { set; get; }

        public static List<PhotoTag> currentPoTagId_Inti { set; get; }
        public static List<FileTag> currentPoTagId_Inti_F { set; get; }

        #region conditions for refresh pages and restric multiple windows
        public static int refreshPage { set; get; } = 0;
        public static int restrictDisplay { set; get; } = 0;
        #endregion

        public static string DefaultUrl { get; set; } = "http://ypsdev.azurewebsites.net/";
        public static string currentPage { get; set; } = string.Empty;
        public static string PerviousPage { get; set; } = string.Empty;
        public static string RedirectPage { get; set; } = string.Empty;
        public static string RedirectPageQA { get; set; } = string.Empty;
        public static string RedirectPagefirsttime { get; set; } = string.Empty;
        public static bool CheckQnAClose { get; set; }
        public static bool ShowSuccessAlert { get; set; } = false;

        public static bool IsFilterreset { get; set; } = false;
        public static bool IsRefreshPartsPage { get; set; } = false;
        public static bool IsSearchClicked { get; set; }
        public static bool IsChatBackButtonVisible { get; set; }

        #region for filter properties
        public static int UserID { get; set; } = 0;
        public static string PONumber { get; set; } = string.Empty;
        public static string REQNo { get; set; } = string.Empty;
        public static string ShippingNo { get; set; } = string.Empty;
        public static int DisciplineID { get; set; } = 0;
        public static string DisciplineName { get; set; }
        public static int ELevelID { get; set; } = 0;
        public static string ELevelName { get; set; }
        public static int ConditionID { get; set; } = 0;
        public static string ConditionName { get; set; }
        public static int ExpeditorID { get; set; } = 0;
        public static string ExpeditorName { get; set; }
        public static int PriorityID { get; set; } = 0;
        public static string PriorityName { get; set; }
        public static string TAGNo { get; set; } = string.Empty;
        public static string IdentCodeNo { get; set; } = string.Empty;
        public static string BagNo { get; set; } = string.Empty;
        public static string Ybkgnumber { get; set; } = string.Empty;
        public static string TaskName { get; set; } = string.Empty;

        public static int ResourceID { get; set; } = 0;
        public static string ResourceName { get; set; }

        public static int LocationPickupID { get; set; } = 0;
        public static string LocationPickupName { get; set; } = string.Empty;
        public static int LocationPOLID { get; set; } = 0;
        public static string LocationPOLName { get; set; } = string.Empty;
        public static int LocationPODID { get; set; } = 0;
        public static string LocationPODName { get; set; } = string.Empty;
        public static int LocationDeliverPlaceID { get; set; } = 0;
        public static string LocationDeliverPlaceName { get; set; } = string.Empty;
        public static string Pickup { get; set; } = string.Empty;
        public static string POL { get; set; } = string.Empty;
        public static string POD { get; set; } = string.Empty;
        public static int Onsite { get; set; } = 0;
        public static string DeliveryFrom { get; set; } = null;
        public static string DeliveryTo { get; set; } = null;
        public static string ETDFrom { get; set; } = null;
        public static string ETDTo { get; set; } = null;
        public static string ETAFrom { get; set; } = null;
        public static string ETATo { get; set; } = null;
        public static string OnsiteFrom { get; set; } = null;
        public static string OnsiteTo { get; set; } = null;
        public static int CheckingRefreshAndReset { get; set; }
        #endregion

        public static string imageBytes { get; set; }
        public static string FireBasedToken { get; set; } = string.Empty;
        public static string AppVersion { get; set; }
        public static string access_token { get; set; }
        public static string encryAccessToken { get; set; }

        public static string HubRegisterid { get; set; }
        public static string Condition { get; set; }
        public static string Expeditor { get; set; }


        public static string DateFormatforAll { get; set; } = "MM/DD/YYYY";
        public static string DateFormatformAPI { get; set; }
        public static string TimeFormatforAPI { get; set; }
        public static string LoginID { get; set; }
        public static string LoginIDDisplay { get; set; }
        public static string encryLoginID { get; set; }
        public static string SecurityCode { get; set; } = "hEJz2uavfzgqO7aYHoNezLwD";
        public static string CertificateSecCode { get; set; } = "YPSCertificateSecCode";
        public static string WebServerPublicKey { get; set; }

        public static string blobServerPubKey { get; set; }

        public static string IIJServerPublicKey { get; set; }
        public static bool IsPinnigEnabled { get; set; } = true;

        public static List<InspectionConfiguration> allInspectionConfigurations { set; get; }


        #region push notification
        public static string GetParamVal { get; set; }
        #endregion

        #region File Upload

        public static StartUploadFileModel selectedTagsData { get; set; }
        public static string compareFileValue { get; set; }
        public static int poid { get; set; }
        public static int fuid { get; set; }
        public static bool fileAccess { get; set; }
        public static string UploadType { get; set; }
        public static int loginkey { get; set; }
        #endregion

        #region Chat Values
        public static string C_Tagnumbers { get; set; } = string.Empty;
        public static int C_PoId { set; get; }
        public static int C_QaId { set; get; }
        public static string C_ChatTitle { get; set; }
        public static string C_Chatstatus { get; set; }
        #endregion

        public static int countmenu { get; set; } = 1;
        public static string HeaderTitle { get; set; }
        public static string ChatTitle { get; set; }
        public static int Resrict_Message_center_Loop { get; set; } = 0;
        public static int currentChatPage { get; set; }
        public static int PushNotificationID { get; set; }
        public static string pushMsgs { get; set; } = "";
        public static string NotifyPage { get; set; }
        public static int notifyCount { set; get; } = 0;
        public static int QAType { set; get; }
        public static int Timerminutes { set; get; } = 1;


        public static int APhoto { get; set; }
        public static int BPhoto { get; set; }
        public static int Files { get; set; }
        public static int Chat { get; set; }
        public static bool IsAppLabelCall { get; set; }
        public static bool SearchWentWrong { get; set; }
        public static bool mutipleTimeClick { get; set; }
        public static bool isExpectedPublicKey { get; set; } = true;
        public static string Text { get; set; }
        public static string Image { get; set; }
        public static string ChatDocument { get; set; }
        public static int startPageYPS { get; set; }
        public static int startPageyShip { get; set; }
        public static int previousEndIndex { get; set; }
        public static int pageSizeYPS { get; set; } = 1000;
        public static int pageSizeyShip { get; set; }
        public static int selectedIndexyShip { get; set; }
        public static int selectedIndexYPS { get; set; }
        public static bool isSkipyShip { get; set; }
        public static bool isSkip { get; set; }
        public static int toGoPageIndex { get; set; }
        public static int toGoPageIndexyShip { get; set; }
        public static string TagNumber { get; set; }
        public static int POID { set; get; }
        public static int TaskID { set; get; }
        public static bool CanUploadPhotos { set; get; }
        public static bool CanOpenScanner { set; get; }
        public static int ScanConfigID { set; get; }
        public static int ScanCount { set; get; }

        public static string VersionName { get; set; }

        public static string scanQRValueA { get; set; } = "";
        public static string scanQRValueB { get; set; } = "";
        public static int? notifyJobCount { set; get; } = 0;

        #region For Image Display
        public static string SPhotoDescription { get; set; }
        #endregion

        #region Coach marks and Guided tour properties
        public static string CheckPage { get; set; }
        #endregion

        #region Yship Propertys
        public static int YshipID { set; get; }
        public static int VersionID { set; get; }
        public static string encryVersionID { set; get; }
        public static string Companylabel { set; get; }
        public static string projectlabel { set; get; }
        public static string joblabel { set; get; }
        public static string supplierlabel { set; get; }

        public static string Companylabel1 { set; get; } = "Company";
        public static string projectlabel1 { set; get; } = "Project";
        public static string joblabel1 { set; get; } = "Job";
        public static string supplierlabel1 { set; get; } = "SupplierCompanyName";
        public static string SetAsDefaultBtn1 { set; get; } = "SetAsDefault";
        public static string Emaillabel1 { set; get; } = "Email";
        public static string GivenNamelabel1 { set; get; } = "GivenName";
        public static string FamilyNamelabel1 { set; get; } = "FamilyName";
        public static string TimeZonelabel1 { set; get; } = "TimeZone";
        public static string Languagelabel1 { set; get; } = "Language";
        public static string UpdateBtn1 { set; get; } = "Update";

        public static List<Alllabeslvalues> alllabeslvalues { set; get; }
        public static List<ActionsForUserData> AllActionStatus { set; get; }

        public static HeaderFilter alldropdownvalues { set; get; }

        public static YshipHeaderFilter yShipDDLValues { get; set; }

        #region yShipFilterProperties
        public static int bkgConfirmIdyShip { get; set; }
        public static string bkgConfirmNameyShip { get; set; }

        public static int cancelIdyShip { get; set; }
        public static string cancelNameyShip { get; set; }


        public static int completedIdyShip { get; set; }
        public static string completedNameyShip { get; set; }


        public static int eqmtTypeIdyShip { get; set; }
        public static string eqmtTypeNameyShip { get; set; }

        public static string ybkgNumberyShip { get; set; }
        public static string yshipBkgNumberyShip { get; set; }
        public static string shippingNumberyShip { get; set; }
        public static string bkgRefNumberyShip { get; set; }

        public static int oriSearchIdyShip { get; set; } = 0;
        public static string oriSearchNameyShip { get; set; } = string.Empty;

        public static int destSearchIdyShip { get; set; } = 0;
        public static string destSearchNameyShip { get; set; } = string.Empty;

        public static string reqPickUpDateyShip { get; set; } = null;
        public static string pickUpTimeyShip { get; set; } = null;
        public static string reqDeliveryDateyShip { get; set; } = null;
        public static string deliveryTimeyShip { get; set; } = null;
        public static bool isyShipRefreshPage { get; set; } = false;

        #endregion

        #endregion

        public static string scanredirectpage { get; set; }
        public static string scanQRValuecode { get; set; } = "";
    }

    public enum BlobContainer
    {
        /// <summary>
        /// Container for Tag-Files And Tag-Photos
        /// </summary>
        cnttagfiles = 1,

        /// <summary>
        /// Container for Chat Files and Photos
        /// </summary>
        cntchatfiles = 2,

        /// <summary>
        /// Container for PL Files And PL Final Files
        /// </summary>
        cntplfiles = 3,

        /// <summary>
        /// Container for Location Files
        /// </summary>
        cntlocationfiles = 4,

        /// <summary>
        /// Container for SP Mat Files
        /// </summary>
        cntspmatfiles = 5,

        /// <summary>
        /// Container for yShip Uploads
        /// </summary>
        cntyshipuploads = 6,

        /// <summary>
        /// Container for Mobile App Builds
        /// </summary>
        cntmobilebuilds = 7,

    }


    /// <summary>
    /// ChatType
    /// </summary>
    public enum QAType
    {
        /// <summary>
        /// PO Tags QA
        /// </summary>
        PT = 1,
        /// <summary>
        /// yShip QA
        /// </summary>
        YS = 2
    }

    /// <summary>
    /// Upload Type
    /// </summary>
    public enum UploadTypeEnums
    {

        /// <summary>
        /// Goods Photos Before Packing
        /// </summary>
        GoodsPhotos_BP = 1,

        /// <summary>
        /// Goods Photos After Packing 
        /// </summary>
        GoodsPhotos_AP = 2,

        /// <summary>
        /// Invoice
        /// </summary>
        Invoice = 3,

        /// <summary>
        /// Packing List
        /// </summary>
        PackingList = 4,

        /// <summary>
        /// Goods Photos Before Loading 
        /// </summary>
        GoodsPhotos_BL = 5,

        /// <summary>
        /// Goods Photos aFTER Loading 
        /// </summary>
        GoodsPhotos_AL = 6,

        /// <summary>
        /// Goods Photos After oPENING 
        /// </summary>
        GoodsPhotos_AO = 7,

        /// <summary>
        /// Export Permit
        /// </summary>
        ExportPermit = 8,

        /// <summary>
        ///Transit Permit
        /// </summary>
        TransitPermit = 9,

        /// <summary>
        /// Import Permit
        /// </summary>
        ImportPermit = 10,

        /// <summary>
        /// Basic Details 
        /// </summary>
        Basic = 11,

        /// <summary>
        /// Location Details
        /// </summary>
        Location = 12,

        /// <summary>
        /// Shipment Details
        /// </summary>
        Shipment = 13,

        /// <summary>
        /// Other Details
        /// </summary>
        Other = 14,

        /// <summary>
        /// PickUp Drivers 
        /// </summary>
        PickUpDriver = 15,

        /// <summary>
        /// Via Drivers
        /// </summary>
        ViaDriver = 16,

        /// <summary>
        /// Destination Drivers
        /// </summary>
        DestinationDriver = 17,

        /// <summary>
        /// Tag File
        /// </summary>
        TagFile = 18,

        /// <summary>
        /// Tag File
        /// </summary>
        ChatPhoto = 19,

        /// <summary>
        /// Tag File
        /// </summary>
        PLFIle = 20,

        /// <summary>
        /// Mobile Builds
        /// </summary>
        MobileBuilds = 21,

        /// <summary>
        ///Tag Load Photos
        /// </summary>
        TagLoadPhotos = 22

    }

    public enum InspectionSignatureType
    {
        /// <summary>
        /// Vin Driver
        /// </summary>
        VinDriver = 1,

        /// <summary>
        /// Vin Supervisor
        /// </summary>
        VinSupervisor = 2,

        /// <summary>
        /// Vin Auditor
        /// </summary>
        VinAuditor = 3,

        /// <summary>
        /// Carrier Supervisor
        /// </summary>
        CarrierSupervisor = 4,

        /// <summary>
        /// Carrier Auditor
        /// </summary>
        CarrierAuditor = 5,

        /// <summary>
        /// Carrier Driver
        /// </summary>
        CarrierDriver = 6,

        /// <summary>
        /// Vin Dealer
        /// </summary>
        VinDealer = 7
    }

    /// <summary>
    /// Page Name Enums
    /// </summary>
    #region Page Name Enums

    public enum PageName
    {
        /// <summary>
        /// About Page
        /// </summary>
        AboutPage = 1,

        /// <summary>
        /// Main Page
        /// </summary>
        MainPage = 2,

        /// <summary>
        /// Chat Page
        /// </summary>
        ChatPage = 3,

        /// <summary>
        /// Chat Users page
        /// </summary>
        ChatUsers = 4,

        /// <summary>
        /// File Upload page
        /// </summary>
        FileUpload = 5,

        /// <summary>
        /// File View page
        /// </summary>
        FileView = 6,

        /// <summary>
        /// FilterData Page
        /// </summary>
        FilterData = 7,

        /// <summary>
        /// ImageView page
        /// </summary>
        ImageView = 8,

        /// <summary>
        /// MobileBuilds Page
        /// </summary>
        MobileBuildsPage = 9,

        /// <summary>
        /// NotificationList Page
        /// </summary>
        NotificationListPage = 10,

        /// <summary>
        /// PdfView Page
        /// </summary>
        PdfViewPage = 11,

        /// <summary>
        /// PhotoUpload page
        /// </summary>
        PhotoUpload = 12,

        /// <summary>
        /// ProfileSelection Page
        /// </summary>
        ProfileSelectionPage = 13,

        /// <summary>
        /// ProviderLogin Page
        /// </summary>
        ProviderLoginPage = 14,

        /// <summary>
        /// QnAlist Page
        /// </summary>
        QnAlistPage = 15,

        /// <summary>
        /// UpdateProfile Page
        /// </summary>        
        UpdateProfilePage = 16,

        /// <summary>
        /// YPSMaster Page
        /// </summary>        
        YPSMasterPage = 17,

        /// <summary>
        /// Yship Page
        /// </summary>        
        YshipPage = 18,

        /// <summary>
        /// yShipFilterData page
        /// </summary>
        yShipFilterData = 19,

        /// <summary>
        /// YShipFilter page
        /// </summary>
        YShipFilter = 20,

        /// <summary>
        /// YshipFileUpload page
        /// </summary>
        YshipFileUpload = 21,

        /// <summary>
        /// YshipPhotoUpload page
        /// </summary>
        YshipPhotoUpload = 22
    }

    #endregion

    #region ConversationType


    public enum ConversationType
    {
        /// <summary>
        /// Add
        /// </summary>
        Add = 1,

        /// <summary>
        /// Message
        /// </summary>
        Message = 2,

        /// <summary>
        /// Close
        /// </summary>
        Close = 3,

        /// <summary>
        /// Remove
        /// </summary>
        Remove = 4,

        /// <summary>
        /// 
        /// </summary>
        Start = 5
    }


    #endregion


    #region Scandit Ajay Added 23-07-2021

   
    public class ScanerSettings
    {
        public bool RotationWithDevice { get; set; }
        public bool ContinuousAfterScan { get; set; }
        public bool CanScan { get; set; }

        // Symbologies
        public bool Ean13Upc12 { get; set; }
        public bool Ean8 { get; set; }
        public bool Upce { get; set; }
        public bool TwoDigitAddOn { get; set; }
        public bool FiveDigitAddOn { get; set; }
        public bool Code11 { get; set; }
        public bool Code25 { get; set; }
        public bool Code32 { get; set; }
        public bool Code39 { get; set; }
        public bool Code93 { get; set; }
        public bool Code128 { get; set; }
        public bool Interleaved2Of5 { get; set; }
        public bool MsiPlessey { get; set; }
        public bool Gs1Databar { get; set; }
        public bool Gs1DatabarExpanded { get; set; }
        public bool Gs1DatabarLimited { get; set; }
        public bool Codabar { get; set; }
        public bool Qr { get; set; }
        public bool QrInverted { get; set; }
        public bool DataMatrix { get; set; }
        public bool DataMatrixInverted { get; set; }
        public bool DpmMode { get; set; }
        public bool Pdf417 { get; set; }
        public bool MicroPdf417 { get; set; }
        public bool Aztec { get; set; }
        public bool MaxiCode { get; set; }
        public bool Rm4scc { get; set; }
        public bool Kix { get; set; }
        public bool DotCode { get; set; }
        public bool MicroQR { get; set; }
        public bool Lapa4sc { get; set; }

        // Scanning Area
        public bool RestrictScanningArea { get; set; }
        public double HotSpotHeight { get; set; }
        public double HotSpotWidth { get; set; }
        public double HotSpotY { get; set; }

        // View Finder
        public GuiStyle GuiStyle { get; set; }
        public double ViewFinderPortraitWidth { get; set; }
        public double ViewFinderPortraitHeight { get; set; }
        public double ViewFinderLandscapeWidth { get; set; }
        public double ViewFinderLandscapeHeight { get; set; }

        // Feedback
        public bool Beep { get; set; }
        public bool Vibrate { get; set; }

        // Button Visibility
        public bool TorchButtonVisible { get; set; }
        public double TorchLeftMargin { get; set; }
        public double TorchTopMargin { get; set; }
        public CameraButton CameraButton { get; set; }

        // Camera
        public Resolution Resolution { get; set; }

        public ScanerSettings()
        {
            ResetSettings();
        }

        public void ResetSettings()
        {
            RotationWithDevice = true;
            ContinuousAfterScan = false;
            Ean13Upc12 = true;
            Ean8 = true;
            Upce = true;
            TwoDigitAddOn = true;
            FiveDigitAddOn = true;
            Code11 = true;
            Code25 = true;
            Code32 = true;
            Code39 = true;
            Code93 = true;
            Code128 = true;
            Interleaved2Of5 = true;
            MsiPlessey = true;
            Gs1Databar = true;
            Gs1DatabarExpanded = true;
            Gs1DatabarLimited = true;
            Codabar = true;
            Qr = true;
            QrInverted = false;
            DataMatrix = true;
            DataMatrixInverted = false;
            DpmMode = false;
            Pdf417 = true;
            MicroPdf417 = true;
            Aztec = true;
            MaxiCode = true;
            Rm4scc = true;
            Kix = false;
            DotCode = true;
            MicroQR = true;
            Lapa4sc = true;
            RestrictScanningArea = false;
            HotSpotHeight = 0.25F;
            HotSpotWidth = 1.0F;
            HotSpotY = 0.45F;
            GuiStyle = GuiStyle.MatrixScan;
            ViewFinderPortraitWidth = 0.9F;
            ViewFinderPortraitHeight = 0.5F;
            ViewFinderLandscapeWidth = 0.4F;
            ViewFinderLandscapeHeight = 0.4F;
            Beep = false;
            Vibrate = false;
            TorchButtonVisible = true;
            TorchLeftMargin = 15;
            TorchTopMargin = 15;
            CameraButton = CameraButton.Always;
            Resolution = Resolution.Standard;
        }
    }
    public enum GuiStyle
    {
        Frame, Laser, None, MatrixScan, LocationsOnly
    };

    public enum CameraButton
    {
        Never, OnTablet, Always
    };

    public enum Resolution
    {
        Standard, HD
    };

    #endregion
}