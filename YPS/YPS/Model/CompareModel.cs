using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using YPS.Service;

namespace YPS.Model
{
    public class CompareModel : IBase
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int Length { get; set; }
        public int _Status { get; set; }
        public int Status
        {
            get
            {
                return _Status;
            }
            set
            {
                this._Status = value;
                RaisePropertyChanged("Status");
            }
        }
        public int CreatedBy { get; set; }
        public string FieldID { get; set; }
        public string LblText { get; set; }
        public string Field2ID { get; set; }
        public bool IsChecked { get; set; }
    }

    public class GetScanDataModel
    {
        public List<CompareModel> ScanConfig { get; set; }
        public List<CompareModel> PolyboxRule { get; set; }
        public List<CompareModel> PolyboxStatus { get; set; }
        public List<CompareModel> PolyboxLocation { get; set; }
        public List<CompareModel> PolyboxRemarks { get; set; }
        public List<CompareModel> PrintFields { get; set; }
    }

    public class ScanConfigResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public GetScanDataModel data { get; set; }

        //public ScanConfigResponse()
        //{
        //    data = new List<GetScanDataModel>();
        //}
    }

    public class SaveScanConfigResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }

    public class GetSavedScanConfigModel
    {
        public int UserID { get; set; }
        public int ScanConfigID { get; set; }
        public int ScanCount { get; set; }
        public int PolyboxRule { get; set; }
        public int PolyboxStatus { get; set; }
        public string PolyboxLocation { get; set; }
        public int PolyboxRemarks { get; set; }
        public string PolyboxPrintFields { get; set; }
    }

    public class GetSavedConfigResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public GetSavedScanConfigModel data { get; set; }
    }

    public class CompareHistoryList
    {
        [Key]
        public int HistorySerialNo { get; set; }
        public string AValue { get; set; }
        public string BValue { get; set; }
        public string IsMatchedImg { get; set; }
    }
}
