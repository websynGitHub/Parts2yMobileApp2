using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Model
{
    public class InspectionPhotosResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public InspectionPhotosResponseData data { get; set; }
    }

    public class InspectionPhotosResponseData
    {
        public List<InspectionPhotosResponseListData> listData { get; set; }
        public int listCount { get; set; }
    }

    public class InspectionPhotosResponseListData
    {
        public int ID { get; set; }
        public int POTagID { get; set; }
        public int QID { get; set; }
        public int FrontLeft { get; set; }
        public int FrontRight { get; set; }
        public int BackLeft { get; set; }
        public int BackRight { get; set; }
        public int Direct { get; set; }
        public string Remarks { get; set; }
        public string FileName { get; set; }
        public string FileURL { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string CreatedDate { get; set; }
    }
}
