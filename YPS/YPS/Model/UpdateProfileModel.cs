namespace YPS.Model
{
    public class UpdateProfileData
    {
        public int UserID { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullName { get; set; }
        public int ISAssigJob { get; set; }
        public string Email { get; set; }
        public string LoginID { get; set; }
        public int Status { get; set; }
        public string UserCulture { get; set; }
        public string JobAccess { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int EntityID { get; set; }
        public string EntityName { get; set; }
        public int LanguageID { get; set; }
        public string LanguageName { get; set; }
        public string SessionID { get; set; }
        public int CreatedBy { get; set; }
        public int ResultCount { get; set; }
        public bool IsIIJEnabled { get; set; }
        public string IIJAccessToken { get; set; }
        public string ExpiryDate { get; set; }
        public bool IsIIJTokenExpired { get; set; }
        public string JWToken { get; set; }
        public string ICNumber { get; set; }
        public string MobileNumber { get; set; }
        public string AltMobileNum { get; set; }
        public string VehicleNumPlate { get; set; }
        public string TrailerNumPlate { get; set; }
        public string ContainerNumber { get; set; }
    }
    public class GetProfile
    {
        public string message { get; set; }
        public int status { get; set; }
        public UpdateProfileData data { get; set; }
    }
    public class updateProfileResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }
}
