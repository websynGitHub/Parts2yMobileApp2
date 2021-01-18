using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;

namespace YPS.CommonClasses
{
    public static class FileManager
    {
        /// <summary>
        /// GetFileStreamAsync, This method is getting a file path and converting it into the stream.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static async Task<Stream> GetFileStreamAsync(string filePath)
        {
            Task<Stream> openAsync = null;
            try
            {
                openAsync  = (await FileSystem.Current.GetFileFromPathAsync(filePath))?.OpenAsync(PCLStorage.FileAccess.Read);
                if (openAsync == null)
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetFileStreamAsync method -> in FileManager.cs  " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
            return await openAsync;
        }

        /// <summary>
        /// SaveFileAsync, This method will save the file path in the device (Mobile).
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        public static async Task SaveFileAsync(string fileName, MemoryStream inputStream)
        {
            try
            {
                IFile file = await FileSystem.Current.LocalStorage.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                using (var stream = await file.OpenAsync(PCLStorage.FileAccess.ReadAndWrite))
                {
                    inputStream.WriteTo(stream);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveFileAsync method -> in FileManager.cs  " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
            
        }

        /// <summary>
        /// GetFilePathFromRoot, This method will get the file path from mobile.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFilePathFromRoot(string fileName)
        {
            string path = null;
            try
            {
                path = Path.Combine(FileSystem.Current.LocalStorage.Path, fileName);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetFilePathFromRoot method -> in FileManager.cs  " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
            return path;
        }

        /// <summary>
        /// ExistsAsync
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static async Task<bool> ExistsAsync(string fileName)
        {
            bool fileExit = false;
            try
            {
                fileExit = await FileSystem.Current.LocalStorage.CheckExistsAsync(fileName) == ExistenceCheckResult.FileExists;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ExistsAsync method -> in FileManager.cs  " + Settings.userLoginID);
                YPSService service = new YPSService();
                 await service.Handleexception(ex);
            }

            return fileExit;
        }

        /// <summary>
        /// GetByteArrayData
        /// </summary>
        /// <param name="printPDFModel"></param>
        /// <returns></returns>
        public static async Task GetByteArrayData(PrintPDFModel printPDFModel)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(printPDFModel.bArray);
                await SaveFileAsync(printPDFModel.FileName, stream);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetByteArrayData method -> in FileManager.cs  " + Settings.userLoginID);
                YPSService service = new YPSService();
                await service.Handleexception(ex);
            }
        }
    }
}
