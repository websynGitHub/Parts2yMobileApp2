using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace YPS.Parts2y.Parts2y_Models
{
    class CompareModel
    {
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
