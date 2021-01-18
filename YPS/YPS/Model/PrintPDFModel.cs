using System;

namespace YPS.Model
{
    public class PrintPDFModel
    {
        public string message { get; set; }
        public int status { get; set; }
        public string data { get; set; }

        public string FileName { get; set; }
        public Byte[] bArray { get; set; }
        public string PDFFileTitle { get; set; }
    }
}
