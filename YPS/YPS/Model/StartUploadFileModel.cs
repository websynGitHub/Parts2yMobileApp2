using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YPS.Model
{
    public class FileTag
    {
        public int FUID { get; set; }
        public int POTagID { get; set; }
        public string TagNumber { get; set; }
        public int TaskID { get; set; }
        public int TagTaskStatus { get; set; }
        public int TaskStatus { get; set; }
    }

    public class MyFile
    {
        public int FUID { get; set; }
        public int FileID { get; set; }
        public string FileDescription { get; set; }
        public string FileName { get; set; }
        public string RoleName { get; set; }
        public string FileURL { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int listCount { get; set; }
        public int RowNumber { get; set; }
        public string GivenName { get; set; }
        public bool HideDownloadFileIc { get; set; }
        public bool HideDeleteIc { get; set; }
        public string ImageURL { get; set; }
        public int UploadType { get; set; }
    }
    public class MyData
    {
        public ObservableCollection<FileTag> fileTags { get; set; }
        public ObservableCollection<MyFile> files { get; set; }

        public MyData()
        {
            fileTags = new ObservableCollection<FileTag>();
            files = new ObservableCollection<MyFile>();
        }
    }

    public class GetStartUploadFileModel
    {

        public string message { get; set; }
        public int status { get; set; }
        public MyData data { get; set; }
    }

    public class StartUploadFileModel
    {
        public int FUID { get; set; }
        public int POID { get; set; }
        public List<FileTag> FileTags { get; set; }
        public MyFile file { get; set; }
        public int CreatedBy { get; set; }
        public string CheckFileUploadType { get; set; }
        public string alreadyExit { get; set; } = "";
    }
    public class DeleteFile
    {
        public string message { get; set; }
        public string data { get; set; }
        public int status { get; set; }
    }

    public class CloseResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }

    public class RootObject
    {
        public string message { get; set; }
        public int status { get; set; }
        public StartUploadFileModel data { get; set; }
    }

    public class SecondRootObject
    {
        public string message { get; set; }
        public int status { get; set; }
        public MyFile data { get; set; }
    }

    #region PL Upload , Get PL Files and Delete PL File

    public class PLFileUpload
    {
        public int ID { get; set; }
        public int POID { get; set; }
        public string FileDescription { get; set; }
        public string FileURL { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int listCount { get; set; }
        public int RowNumber { get; set; }
        public string GivenName { get; set; }
        public string FileName { get; set; }
        public string ImageURL { get; set; }
        public bool HideDeleteIc { get; set; } = false;
    }

    public class PLFileUploadResult
    {
        public string message { get; set; }
        public int status { get; set; }
        public PLFileUpload data { get; set; }
    }
    public class GetPLFileUploadData
    {
        public string message { get; set; }
        public int status { get; set; }
        public ObservableCollection<PLFileUpload> data { get; set; }

        public GetPLFileUploadData()
        {
            data = new ObservableCollection<PLFileUpload>();
        }
    }

    public class PLDeleteFileResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public string data { get; set; }
    }
    #endregion
}
