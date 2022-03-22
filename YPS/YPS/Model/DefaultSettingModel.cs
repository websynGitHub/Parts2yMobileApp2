using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YPS.Model
{
    public class DefaultSettingModel
    {
        public string message { get; set; }
        public int status { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public ObservableCollection<DDLmaster> Company { get; set; }
        public List<DDLmaster> Project { get; set; }
        public List<DDLmaster> Job { get; set; }
        //public List<DDLmaster> Supplier { get; set; }
        public Data()
        {
            Company = new ObservableCollection<DDLmaster>();
            Project = new List<DDLmaster>();
            Job = new List<DDLmaster>();
        }
    }

    /// Save user default setting
    public class SaveUserDefaultSettingsModel
    {
        public int UserID { get; set; }
        public int CompanyID { get; set; }
        public int ProjectID { get; set; }
        public int JobID { get; set; }
        //public int SupplierID { get; set; }
        public string CompanyName { get; set; }
        public string ProjectName { get; set; }
        public string JobNumber { get; set; }
        //public string SupplierName { get; set; }
        public int VersionID { get; set; }
        public string TagColumns { get; set; }
        public string yShipColumns { get; set; }
        public string VersionColorCode { get; set; }
        public int EventID { get; set; }
    }
    public class ResponseFromSaveUDSModel
    {
        public string message { get; set; }
        public int status { get; set; }
        public SaveUserDefaultSettingsModel data { get; set; }
        public ResponseFromSaveUDSModel()
        {
            data = new SaveUserDefaultSettingsModel();
        }

    }
    public class SaveUDSModel
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }

    public class GlobalSettings
    {
        public string ApplicationURL { get; set; }
        public string APIURL { get; set; }
        public string BlobConnection { get; set; }
        public int PhotoSize { get; set; }
        public int CompressionQuality { get; set; }
        public string DateFormat { get; set; }
        public string DateTimeFormat { get; set; }
        public string IIJConsumerKey { get; set; }
        public string IIJConsumerSecret { get; set; }
        public string AndroidVersion { get; set; }
        public string iOSversion { get; set; }
        public bool IsIIJEnabled { get; set; }
        public string Parts2yLogo { get; set; }
    }

    public class token
    {
        public string Token { get; set; }
        public bool IsIIJEnabled { get; set; }
        public bool IsIIJTokenExpired { get; set; }
        public string IIJAccessToken { get; set; }
        public GlobalSettings GlobalSettings { get; set; }
    }

    public class LogData
    {
        public int UserID { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public string UserCulture { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int LanguageID { get; set; }
        public string LanguageName { get; set; }
        public object EntityID { get; set; }
        public string EntityName { get; set; }
        public string EntityTypeName { get; set; }
        public string SessionToken { get; set; }
        public token JwToken { get; set; }
        public int VersionID { get; set; }
        public string LoginID { get; set; }
        public string RoleColorCode { get; set; }
        public string VersionColorCode { get; set; }
    }
    public class LoginUserData
    {
        public string message { get; set; }
        public int status { get; set; }
        public LogData data { get; set; }
    }
    public class LoginData
    {
        public string LoginID { get; set; }
        public string UserID { get; set; }
        public string SessionToken { get; set; }
    }
    public class YPSException
    {
        public Exception Ex { get; set; }
        public string LoginID { get; set; }
        public YPSException()
        {
            Ex = new Exception();
        }
    }

    /// <summary>
    /// IIJ User Info
    /// </summary>
    public class IIJUserInfo
    {
        /// <summary>
        /// User Unique ID
        /// </summary>
        public string sub { get; set; }

        /// <summary>
        /// Full Name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Given Name
        /// </summary>
        public string given_name { get; set; }

        /// <summary>
        /// Family Name
        /// </summary>
        public string family_name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string preferred_username { get; set; }

        /// <summary>
        /// Locale
        /// </summary>
        public string locale { get; set; }

        /// <summary>
        /// Last Updated 
        /// </summary>
        public int updated_at { get; set; }

        /// <summary>
        /// given_name_kana
        /// </summary>
        public string given_name_kana { get; set; }

        /// <summary>
        /// family_name_kana
        /// </summary>
        public string family_name_kana { get; set; }
    }
    public class SendTokendetails
    {
        public int UserID { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string id_token { get; set; }
        public string LoginID { get; set; }
    }
    public class returnTokendetails
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }

    }

    public class returnnulldata
    {
        public string message { get; set; }
        public int status { get; set; }

    }
    public class Getjwtoken
    {
        public string message { get; set; }
        public int status { get; set; }
        public token data { get; set; }
    }

    public class Localsettings
    {
        Localsettings()
        {

        }
        public string ApplicationURL { get; set; }
        public string APIURL { get; set; }
        public string BlobConnection { get; set; }
        public int PhotoSize { get; set; }
        public int CompressionQuality { get; set; }
        public string DateFormat { get; set; }
        public string DateTimeFormat { get; set; }
        public bool IsIIJEnabled { get; set; }
        public string IIJConsumerKey { get; set; }
        public string IIJConsumerSecret { get; set; }
        public string Parts2yLogo { get; set; }
        public bool IsEmailEnabled { get; set; }
        public bool IsPNEnabled { get; set; }
        public SSLkeyData sSLPinningKeys { get; set; }
        public AppSettings appSettings { get; set; }
        public string ScanditKey { get; set; }
        public bool IsMobileCompareCont { get; set; }
        public bool IsMobilePolybox { get; set; }
        public string MobileScanProvider { get; set; }
        public string LastUpdatedOn { get; set; }
    }
    public class ApplicationSettings
    {
        ApplicationSettings()
        {

        }
        public string message { get; set; }
        public int status { get; set; }
        public Localsettings data { get; set; }

    }

    public class SSLkeyData
    {
        SSLkeyData()
        {

        }
        public string SecCode { get; set; }
        public List<PublicKeyData> Keys { get; set; }
        public bool IsPinnigEnabled { get; set; }
    }

    public class PublicKeyData
    {
        PublicKeyData()
        {

        }
        public string ActualUrl { get; set; }
        public string Url { get; set; }
        public string CertificateKey { get; set; }
    }

    public class AppSettings
    {
        AppSettings()
        {

        }
        public string BGImage { get; set; }
        public string Message1 { get; set; }
        public string Message2 { get; set; }
        public bool Status { get; set; }
    }
}
