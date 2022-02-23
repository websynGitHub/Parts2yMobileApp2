using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Model
{
    class PolyBoxModel
    {
        public int CompanyID { get; set; }
        public int ProjectID { get; set; }
        public int JobID { get; set; }
        public string CargoCategory1 { get; set; }
        public string BagNumber { get; set; }
        public string TQB_PkgSizeNo_L1 { get; set; }
        public string EventDT_L1 { get; set; }
        public int UserID { get; set; }
        public string Attributes { get; set; }
        public string FromLoc_L1 { get; set; }
        public string EventRemarks_L1 { get; set; }
        public string Remarks_Description { get; set; }
        public string Location_Details { get; set; }
        public string TotalPolyboxCount { get; set; }
        public string TotalScannedToday { get; set; }
        public string ISR { get; set; }
        public string TagNumber { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
