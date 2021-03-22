using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Model
{
    public class PhotoRepoModel
    {
        [PrimaryKey, AutoIncrement]
        public int PhotoID { get; set; }
        public int UserID { get; set; }
        public string PhotoURL { get; set; }
        public string FullFileName { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string CreatedDate { get; set; }
        public bool ShowAndHideDescr { get; set; }
        public bool IsSelected { get; set; }
    }
}
