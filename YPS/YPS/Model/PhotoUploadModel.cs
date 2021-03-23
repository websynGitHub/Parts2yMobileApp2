using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace YPS.Model
{
    public class PhotoUploadModel
    {
        public int PUID { get; set; }
        public int POID { get; set; }
        public List<PhotoTag> photoTags { get; set; }
        public List<Photo> photos { get; set; }
        public Photo photo { get; set; }
        public int CreatedBy { get; set; }
        public string alreadyExit { get; set; } = "";
        public bool isCompleted { get; set; }
        public string RoleName { get; set; }

        public PhotoUploadModel()
        {
            photoTags = new List<PhotoTag>();
            photos = new List<Photo>();
            photo = new Photo();
        }
    }

    public class Photo
    {
        public int PUID { get; set; }
        public int PhotoID { get; set; }
        public int UploadType { get; set; }
        public string PhotoDescription { get; set; }
        public string FileName { get; set; }
        public string PhotoURL { get; set; }
        public int CreatedBy { get; set; }
        public int ISPhotoClosed { get; set; }
        public string CreatedDate { get; set; }
        public string GivenName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public Stream PicStream { get; set; }
    }

    public class PhotoTag
    {
        public int PUID { get; set; }
        public int POTagID { get; set; }
        public string TagNumber { get; set; }
        public string IdentCode { get; set; }
        public int ISPhotoClosed { get; set; }
        public int TaskID { get; set; }
        public int TagTaskStatus { get; set; }
        public int TaskStatus { get; set; }
    }


    public class InitialResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public PhotoUploadModel data { get; set; }
    }

    public class SecondTimeResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        //public CustomPhotoModel data { get; set; }
        public List<CustomPhotoModel> data { get; set; }
    }

    public class CustomPhotoModel
    {
        public int PUID { get; set; }
        public int PhotoID { get; set; }
        public int UploadType { get; set; }
        public string PhotoDescription { get; set; }
        public string FileName { get; set; }
        public string PhotoURL { get; set; }
        public int CreatedBy { get; set; }
        public int ISPhotoClosed { get; set; }
        public string CreatedDate { get; set; }
        public string GivenName { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public string EntityName { get; set; }
        public bool ShowAndHideBtn { get; set; }
        public bool ShowAndHideDescr { get; set; }
        public bool ShowAndHideBtnEnable { get; set; }
        public string descriptionlbl { get; set; } = "Description";
        public Stream PicStream { get; set; }
    }

    public class UploadedPhotosList<T>
    {
        public List<PhotoTag> photoTags { get; set; }
        public ObservableCollection<T> Aphotos { get; set; }
        public ObservableCollection<T> BPhotos { get; set; }
        public UploadedPhotosList()
        {
            Aphotos = new ObservableCollection<T>();
            BPhotos = new ObservableCollection<T>();
            photoTags = new List<PhotoTag>();
        }
    }

    public class PhotosList
    {
        public string message { get; set; }
        public int status { get; set; }
        public UploadedPhotosList<CustomPhotoModel> data { get; set; }
    }

    public class DeleteResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public string data { get; set; }
    }

    public class ClosePhotoResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }


    public class LoadPhotoModel
    {
        public int TaskID { get; set; }
        public int PoTagID { get; set; }
        public int PhotoID { get; set; }
        public string PhotoIDEncrypted { get; set; }
        public string descriptionlbl { get; set; } = "Description";
        public int UploadType { get; set; }
        public string PhotoDescription { get; set; }
        public string FileName { get; set; }
        public string RoleName { get; set; }
        public string EntityName { get; set; }
        public string PhotoURL { get; set; }
        public int CreatedBy { get; set; }
        public int ISPhotoClosed { get; set; }
        public string CreatedDate { get; set; }
        public string GivenName { get; set; }
        public string FullName { get; set; }
        public bool ShowAndHideBtn { get; set; }
        public bool ShowAndHideDescr { get; set; }
        public bool ShowAndHideBtnEnable { get; set; }
        public Stream PicStream { get; set; }

        //public string RoleName { get; set; }
    }

    public class LoadPhotosUploadResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public LoadPhotoModel data { get; set; }
    }

    public class LoadPhotosListResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public ObservableCollection<LoadPhotoModel> data { get; set; }
    }
}

