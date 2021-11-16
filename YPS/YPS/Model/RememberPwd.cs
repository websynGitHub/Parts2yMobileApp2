using SQLite;
using System.Drawing;

namespace YPS.Model
{
    public class RememberPwd
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int UserId { get; set; }
        public string Email { get; set; }
        public string LoginID { get; set; }
        public string UserName { get; set; }
        public int UserRollID { get; set; }
        public string GivenName { get; set; }
        public string EntityName { get; set; }
        public string RoleName { get; set; }
        public string BlobConnection { get; set; }
        public int PhotoSize { get; set; }
        public string APIURL { get; set; }
        public int CompressionQuality { get; set; }

        public string Companylabel { set; get; }
        public string projectlabel { set; get; }
        public string joblabel { set; get; }
        public string supplierlabel { set; get; }
        public string tagnamelabel { set; get; }
        public string descrptionlabel { set; get; }
        public string AccessToken { set; get; }
        public int Languageid { set; get; }
        public string DateFormat { get; set; }
        public string DateTimeFormat { get; set; }

        public string encSessiontoken { get; set; }
        public string SelectedScanRule { get; set; }
        public int? EnteredScanTotal { get; set; }
        //public int BgColor { get; set; }
        public string BgColor { get; set; }
        public string ScanditKey { get; set; }
        public bool IsMobileCompareCont { get; set; }
        public string RoleColorCode { get; set; }

        /// <summary>
        /// Encrypted
        /// </summary>
        public string encVersionId { get; set; }
        public string encUserId { get; set; }
        public string encEmail { get; set; }
        public string encLoginID { get; set; }
        public string encUserName { get; set; }
        public string encUserRollID { get; set; }
        public string encGivenName { get; set; }
        public string encEntityName { get; set; }
        public string encRoleName { get; set; }
        public string encBlobConnection { get; set; }
        public string encPhotoSize { get; set; }
        public string encCompressionQuality { get; set; }
        public string encAccessToken { set; get; }
        public string AndroidVersion { get; set; }
        public string iOSversion { get; set; }
        public bool IIJEnable { get; set; }
        public bool IsPNEnabled { get; set; }
        public bool IsEmailEnabled { get; set; }
        public string encLanguageID { get; set; }
    }

    public class notifyMsgList
    {
        [Unique]
        public int QaId { get; set; }
        public string AllPramText { get; set; }
        public string title { set; get; }
    }
}
