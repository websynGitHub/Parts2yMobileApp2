using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using YPS.iOS;
using YPS.Parts2y.Parts2y_Views;

[assembly: Xamarin.Forms.Dependency(typeof(Archiver))]
 namespace YPS.iOS
{
    public class Archiver : IArchiver
    {
        public void ArchiveText(string filename, string text)
        {
            NSKeyedArchiver.ArchiveRootObjectToFile(NSObject.FromObject(text), GetFilePath(filename));
        }

        public string UnarchiveText(string filename)
        {
            var obj = NSKeyedUnarchiver.UnarchiveFile(GetFilePath(filename));
            if (obj == null)
            {
                return null;
            }
            else
            {
                return ((NSString)obj).ToString();
            }
        }

        string GetFilePath(string filename)
        {
            var fileManger = new NSFileManager();
            var url = fileManger.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0];
            return url.Append(filename, false).Path;
        }
    }
}