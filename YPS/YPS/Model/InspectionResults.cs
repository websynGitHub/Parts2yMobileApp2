using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Model
{
    public class InspectionResults
    {
        public string message { get; set; }
        public int status { get; set; }
        public InspectionResultsData data { get; set; }
    }

    public class InspectionResultsRespnse
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }

    public class InspectionResultsData
    {
        public List<InspectionResultsList> listData { get; set; }
        public int listCount { get; set; }
    }

    public class InspectionResultsList
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
        public object FileName { get; set; }
        public object FilrURl { get; set; }
        public int UserID { get; set; }
        public int PhotoCount { get; set; }
        public int SignType { get; set; }
        public string Signature { get; set; }

        public string PalletNo_L2 { get; set; }
        public string ExpiryDate { get; set; }
        public string Attributes { get; set; }
        public double? InnerQty { get; set; }
        public int additional_entry { get; set; }

    }
}
