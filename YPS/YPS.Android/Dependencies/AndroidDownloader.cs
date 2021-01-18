using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Android.Content;
using Xamarin.Forms;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.CommonClasses;
using YPS.Service;

[assembly: Dependency(typeof(AndroidDownloader))]
namespace YPS.Droid.Dependencies
{
    public class AndroidDownloader : IDownloader
    {
        public event EventHandler<DownloadEventArgs> OnFileDownloaded;

        /// <summary>
        /// This method will download file from server and saved in mobile.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="folder"></param>
        public void DownloadFile(string url, string folder)
        {
            try
            {
                Context context = Forms.Context;
                var path = global::Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                string pathToNewFolder = Path.Combine(path, folder + "/Downloads");
                Directory.CreateDirectory(pathToNewFolder);
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                string pathToNewFile = Path.Combine(pathToNewFolder, String.Format("{0:yyyyMMMdd_hh-mm-ss}", DateTime.Now) + "_" + Path.GetFileName(url));
                webClient.DownloadFileAsync(new Uri(url), pathToNewFile);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DownloadFile method -> in AndroidDownloader.cs (Droid) " + Settings.userLoginID);
                if (OnFileDownloaded != null)
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
                YPSService trackService = new YPSService();
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (OnFileDownloaded != null)
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
            }
            else
            {
                if (OnFileDownloaded != null)
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(true));
            }
        }
    }
}