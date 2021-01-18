using System.Collections.Generic;

namespace YPS.Model
{
    public class AllLabels
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<Alllabeslvalues> data { get; set; }
    }   
    public class Alllabeslvalues
    {
        public string FieldID { get; set; }
        public string LblCode { get; set; }
        public int VersionID { get; set; }
        public int OrderID { get; set; }
        public string LblText { get; set; }
        public int Status { get; set; }
        public int LanguageID { get; set; }
    }
}
