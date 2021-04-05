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

    public class PhotoRepoDBModel
    {
        public int FileID { get; set; }
        public int CreatedBy { get; set; }
        public int ISPhotoClosed { get; set; }
        public string FileIDEncrypted { get; set; }
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public string FileUrl { get; set; }
        public string FullFileUrl { get; set; }
        public string CreatedDate { get; set; }
        public string GivenName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string EntityName { get; set; }
        public string EntityTypeName { get; set; }
        public bool ShowAndHideDescr { get; set; }
        public bool IsSelected { get; set; }
    }

    public class GetRepoPhotoResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<PhotoRepoDBModel> data { get; set; }
    }

    public class GetRepoPhotoDelResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public string data { get; set; }
    }

    public class GetRepoPhotoDelAllResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<string> data { get; set; }
    }
}
