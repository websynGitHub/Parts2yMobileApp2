using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace YPS.Parts2y.Parts2y_Models
{
    public class PhotoModel
    {
        public class CustomPhotoModel
        {
            public int PhotoID { get; set; }
            public string PhotoURL { get; set; }
            public string extension { get; set; }
            public string vinno { get; set; }
            public int queseq { get; set; }
            public string email { get; set; }
            public DateTime createdOn { get; set; } = DateTime.Now;

        }
    }
}
