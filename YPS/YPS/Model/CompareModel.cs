using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Model
{
    public class CompareModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int Length { get; set; }
        public int Status { get; set; }
        public int CreatedBy { get; set; }
    }

    public class ScanConfigResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<CompareModel> data { get; set; }
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
    }

    public class GetSavedConfigResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public GetSavedScanConfigModel data { get; set; }
    }
}
