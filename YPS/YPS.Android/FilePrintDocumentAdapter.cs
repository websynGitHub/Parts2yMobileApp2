using Android.OS;
using Android.Print;
using Java.IO;
using System;

namespace YPS.Droid
{
    public class FilePrintDocumentAdapter : PrintDocumentAdapter
    {
        #region Data memeber declaration
        private readonly string _fileName;
        private readonly string _filePath;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        public FilePrintDocumentAdapter(string fileName, string filePath)
        {
            _fileName = fileName;
            _filePath = filePath;
        }

        /// <summary>
        /// OnLayout
        /// </summary>
        /// <param name="oldAttributes"></param>
        /// <param name="newAttributes"></param>
        /// <param name="cancellationSignal"></param>
        /// <param name="callback"></param>
        /// <param name="extras"></param>
        public override void OnLayout(PrintAttributes oldAttributes, PrintAttributes newAttributes, CancellationSignal cancellationSignal, LayoutResultCallback callback, Bundle extras)
        {
            if (cancellationSignal.IsCanceled)
            {
                callback.OnLayoutCancelled();
                return;
            }

            callback.OnLayoutFinished(new PrintDocumentInfo.Builder(_fileName)
                .SetContentType(PrintContentType.Document)
                .Build(), true);
        }

        /// <summary>
        /// OnWrite
        /// </summary>
        /// <param name="pages"></param>
        /// <param name="destination"></param>
        /// <param name="cancellationSignal"></param>
        /// <param name="callback"></param>
        public override void OnWrite(PageRange[] pages, ParcelFileDescriptor destination, CancellationSignal cancellationSignal, WriteResultCallback callback)
        {
            try
            {
                using (InputStream input = new FileInputStream(_filePath))
                {
                    using (OutputStream output = new FileOutputStream(destination.FileDescriptor))
                    {
                        var buf = new byte[1024];
                        int bytesRead;

                        while ((bytesRead = input.Read(buf)) > 0)
                        {
                            output.Write(buf, 0, bytesRead);
                        }
                    }
                }
                callback.OnWriteFinished(new[] { PageRange.AllPages });
            }
            catch (System.IO.FileNotFoundException fileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine(fileNotFoundException);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception);
            }
        }
    }
}