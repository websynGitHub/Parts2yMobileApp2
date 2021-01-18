using System.Collections.ObjectModel;

namespace YPS.Model
{
    public class MobileBuildsModel
    {
        public int ID { get; set; }
        public int UploadType { get; set; }
        public int AppServerID { get; set; }
        public string AppVersion { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentURL { get; set; }
        public string Description { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedDate { get; set; }
        public int listCount { get; set; }
        public string GivenName { get; set; }
        public string AppServer { get; set; }
        public string ImageURL { get; set; }
    }

    public class MobileBFinalData
    {
        public string message { get; set; }
        public int status { get; set; }
        public ObservableCollection<MobileBuildsModel> data { get; set; }

    }
    public class GetMobileBData
    {
        public int UserID { get; set; }
        public string SearchText { get; set; }
        public int StartPage { get; set; }
        public int PageSize { get; set; }
        public int Status { get; set; }
    }
}
