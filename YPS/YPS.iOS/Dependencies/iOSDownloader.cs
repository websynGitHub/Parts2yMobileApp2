using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.iOS.Dependencies;

[assembly: Dependency(typeof(iOSDownloader))]
namespace YPS.iOS.Dependencies
{
    public class iOSDownloader : IDownloader
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
                string pathToNewFolder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), folder);
                Directory.CreateDirectory(pathToNewFolder);
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                string pathToNewFile = Path.Combine(pathToNewFolder, String.Format("{0:yyyyMMMdd_hh-mm-ss}", DateTime.Now) + "_" + Path.GetFileName(url));
                webClient.DownloadFileAsync(new Uri(url), pathToNewFile);
            }
            catch (Exception ex)
            {
                if (OnFileDownloaded != null)
                {
                    OnFileDownloaded.Invoke(this, new DownloadEventArgs(false));
                    YPSLogger.ReportException(ex, "DownloadFile method-> in iOSDownloader.cs" + Settings.userLoginID);
                }
            }
        }

        /// <summary>
        /// Completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DownloadFile method-> in iOSDownloader.cs" + Settings.userLoginID);
            }

        }
    }
}