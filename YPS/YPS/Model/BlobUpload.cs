﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model.Yship;
using YPS.Service;

namespace YPS.Model
{
    /// <summary>
    /// Upload Files to Blob  
    /// </summary>
    public class BlobUpload
    {
        // Dev Server
        //  private const string BlobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=ypsuploadsdev;AccountKey=Bq+q8c2g31dusbnpVz4phpM8DISg0mABDNMtHlsuI/uF9oZjX4ozPuDxCGHn3njR+bwrfCQB37DLhgzJI9V0Ww==;EndpointSuffix=core.windows.net";

        //UAT Server
        //private const string BlobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=azrbsa026dv00a;AccountKey=C9wWhLFOWNwCU80zWeyw4288BvvaMAD5WhLYsRxiFO8Tu084m3D3jWnFIf8oksQNMMCocC4ozGLb2OTbEq8I8Q==;EndpointSuffix=core.windows.net";

        /// <summary>
        /// Upload Stream File to Blob
        /// </summary>
        /// <param name="cntName">Blob Cnt Name </param>
        /// <param name="FileName">File Name </param>
        /// <param name="fileStream"> File Dataas Stream</param>
        public static async void UploadFile(string cntName, string FileName, Stream fileStream)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount account = CloudStorageAccount.Parse(Settings.BlobStorageConnectionString);
            // Create the blob client.
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            // Retrieve reference to a previously created container.        
            CloudBlobContainer container = serviceClient.GetContainerReference(cntName);
            //Create the container if it does not exist automatically
            // await container.CreateIfNotExistsAsync();
            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(FileName);
            try
            {
                blockBlob.UploadFromStreamAsync(fileStream).Wait();
            }
            catch (System.Exception ex)
            {
                #region Public Key
                if (Settings.isExpectedPublicKey == false || ex.Message.ToLower().Replace(" ", null) == EndpointConfiguration.message)
                {
                    Settings.isExpectedPublicKey = true;
                    YPSLogger.ReportException(ex, "UploadFile method because of SSL public key mismatch-> in RequestProvider.cs" + Settings.userLoginID);
                    //await App.Current.MainPage.DisplayAlert("Man in the middle detected", "potential security risk ahead", "Close");
                    CloudFolderKeyVal.Appredirectloginwithoutlogout(true);
                }
                #endregion
                else
                {
                    YPSService service = new YPSService();
                    await service.Handleexception(ex);
                    YPSLogger.ReportException(ex, "UploadFile constructor-> in UploadFile.cs " + Settings.userLoginID);
                }
            }
        }

        /// <summary>
        /// To Upload Files from YPS Mobile app
        /// </summary>
        /// <param name="extension">File extension </param>
        /// <param name="picStream">Image/File Stream</param>
        /// <param name="ParentId">ParentId</param>
        /// <param name="fileName"> File name</param>
        /// <param name="UploadType">Chat/Photo/File/TagFile/PLfile/Yship</param>
        /// <param name="containertype">Blob Folder Name</param>
        /// <param name="description_txt">Photo/File Description</param>
        /// <returns></returns>
        public static async Task<object> YPSFileUpload(string extension, Stream picStream, int ParentId, string fileName, int UploadType, int containertype, PhotoUploadModel filevales, StartUploadFileModel filedata = null, string description_txt = "", string permitdate = "", string permitnumber = "")
        {
            try
            {
                YPSService service = new YPSService();
                string FullFilename = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                BlobUpload.UploadFile(CloudFolderKeyVal.GetBlobFolderName(containertype), FullFilename, picStream);

                if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BP || UploadType == (int)UploadTypeEnums.GoodsPhotos_AP)
                {
                    if (ParentId != 0)
                    {
                        CustomPhotoModel phUpload = new CustomPhotoModel();
                        phUpload.PUID = ParentId;
                        phUpload.PhotoID = 0;
                        phUpload.PhotoURL = FullFilename;
                        phUpload.PhotoDescription = description_txt;
                        phUpload.FileName = fileName;
                        phUpload.CreatedBy = Settings.userLoginID;
                        phUpload.UploadType = UploadType;// uploadType;
                        phUpload.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                        phUpload.FullName = Settings.Username;
                        var data = await service.PhotosUpload(phUpload);
                        return data;
                    }
                    else
                    {
                        PhotoUploadModel DataForFileUpload = new PhotoUploadModel();
                        DataForFileUpload = filevales;
                        DataForFileUpload.CreatedBy = Settings.userLoginID;
                        Photo phUpload = new Photo();
                        phUpload.PUID = ParentId;
                        phUpload.PhotoID = 0;
                        phUpload.PhotoURL = FullFilename;
                        phUpload.PhotoDescription = description_txt;
                        phUpload.FileName = fileName;
                        phUpload.CreatedBy = Settings.userLoginID;
                        phUpload.UploadType = UploadType;// uploadType;
                        phUpload.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                        phUpload.GivenName = Settings.Username;
                        DataForFileUpload.photo = phUpload;
                        var data = await service.InitialUpload(DataForFileUpload);
                        return data;
                    }
                }
                else if (UploadType == (int)UploadTypeEnums.TagFile)
                {
                    if (ParentId != 0)
                    {
                        MyFile phUpload = new MyFile();
                        phUpload.FUID = ParentId;
                        phUpload.FileURL = FullFilename;
                        phUpload.FileName = fileName;
                        phUpload.CreatedBy = Settings.userLoginID;
                        phUpload.FileDescription = description_txt;// uploadType;
                        phUpload.CreatedDate = String.Format("{0:dd MMMM yyyy hh:mm tt}", DateTime.Now);
                        var data = await service.SecondTimeFiles(phUpload);
                        return data;
                    }
                    else
                    {
                        StartUploadFileModel DataForFileUpload = new StartUploadFileModel();
                        DataForFileUpload = filedata;
                        DataForFileUpload.CreatedBy = Settings.userLoginID;
                        MyFile phUpload = new MyFile();
                        phUpload.FUID = ParentId;
                        phUpload.FileURL = FullFilename;
                        phUpload.FileName = fileName;
                        phUpload.CreatedBy = Settings.userLoginID;
                        phUpload.FileDescription = description_txt;// uploadType;
                        phUpload.UploadType = UploadType;
                        phUpload.CreatedDate = String.Format("{0:dd MMMM yyyy hh:mm tt}", DateTime.Now);
                        DataForFileUpload.file = phUpload;
                        var data = await service.StartUploadFiles(DataForFileUpload);
                        return data;
                    }
                }
                else if (UploadType == (int)UploadTypeEnums.PLFIle)
                {
                    PLFileUpload phUpload = new PLFileUpload();
                    phUpload.POID = ParentId;
                    phUpload.FileURL = FullFilename;
                    phUpload.FileName = fileName;
                    phUpload.FileDescription = description_txt;
                    phUpload.CreatedBy = Settings.userLoginID;
                    phUpload.CreatedDate = String.Format("{0:dd MMMM yyyy hh:mm tt}", DateTime.Now);
                    var data = await service.PLUploadFile(phUpload);
                    return data;
                }
                else if (UploadType == (int)UploadTypeEnums.ChatPhoto)
                {
                    UploadFiles phUpload = new UploadFiles();
                    phUpload.yShipId = ParentId;
                    phUpload.FileURL = FullFilename;
                    phUpload.AttachmentName = FullFilename;
                    phUpload.UploadedBy = Settings.userLoginID;
                    phUpload.UploadType = UploadType;// uploadType;
                    var data = await service.UploadFiles(phUpload);
                    return data;
                }
                else if (UploadType == (int)UploadTypeEnums.TagLoadPhotos)
                {
                    LoadPhotoModel loadphoto = new LoadPhotoModel();
                    loadphoto.TaskID = ParentId;
                    loadphoto.UploadType = UploadType;
                    loadphoto.PhotoURL = FullFilename;
                    loadphoto.FileName = fileName;
                    loadphoto.PhotoDescription = description_txt;
                    loadphoto.CreatedBy = Settings.userLoginID;
                    loadphoto.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                    loadphoto.FullName = Settings.Username;
                    var data = await service.LoadPhotoUpload(loadphoto);
                    return data;
                }
                else
                {
                    UploadFiles phUpload = new UploadFiles();
                    phUpload.yShipId = ParentId;
                    phUpload.FileURL = FullFilename;
                    phUpload.AttachmentName = fileName;
                    phUpload.UploadedBy = Settings.userLoginID;
                    phUpload.FullName = Settings.Username;
                    phUpload.GivenName = Settings.SGivenName;
                    phUpload.UploadType = UploadType;
                    phUpload.PermitDate = permitdate;
                    phUpload.PermitNo = permitnumber;
                    phUpload.UploadedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                    var data = await service.UploadFiles(phUpload);
                    return data;
                }
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UploadFile method -> in UploadFile.cs " + Settings.userLoginID);
                return null;
            }
        }

        public static async Task<object> YPSInspectionFileUpload(string extension, Stream picStream, string fileName, int containertype, InspectionConfiguration inspectionConfiguration, int tagId, string description_txt = "")
        {
            try
            {
                YPSService service = new YPSService();
                string FullFilename = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                BlobUpload.UploadFile(CloudFolderKeyVal.GetBlobFolderName(containertype), FullFilename, picStream);
                UpdateInspectionRequest updateInspectionRequest = new UpdateInspectionRequest()
                {
                    BackLeft = inspectionConfiguration.BackLeft,
                    BackRight = inspectionConfiguration.BackRight,
                    Direct = inspectionConfiguration.BackRight,
                    FileName = fileName,
                    FileURL = FullFilename,
                    FrontLeft = inspectionConfiguration.FrontLeft,
                    FrontRight = inspectionConfiguration.FrontRight,
                    POTagID = tagId,
                    QID = inspectionConfiguration.MInspectionConfigID,
                    Remarks = description_txt,
                    UserID = Settings.userLoginID
                };
                var data = await service.InsertInspectionPhotosService(updateInspectionRequest);
                return data;
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "UploadFile method -> in UploadFile.cs " + Settings.userLoginID);
                return null;
            }
        }

    }
}