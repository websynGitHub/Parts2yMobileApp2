using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Model
{
    public class UpdateInspectionRequest
    {
        public int ID { get; set; }
        public int POTagID { get; set; }
        public int TaskID { get; set; }
        public int QID { get; set; }
        public int FrontLeft { get; set; }
        public int FrontRight { get; set; }
        public int BackLeft { get; set; }
        public int BackRight { get; set; }
        public int Direct { get; set; }
        public string Remarks { get; set; }
        public string FileDescription { get; set; }
        public string FileName { get; set; }
        public string FileURL { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string CreatedDate { get; set; }
        public int PhotoCount { get; set; }
        public string PalletNo_L2 { get; set; }
        public string ExpiryDate { get; set; }
        public string Attributes { get; set; }
        public decimal? InnerQty {get; set; }
        public int additional_entry { get; set; }
    }

    public class UpdateInsertInspectionResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<UpdateInspectionRequest> data { get; set; }
    }
}
