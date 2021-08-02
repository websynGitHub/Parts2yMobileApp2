using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YPS.Droid;
using YPS.Parts2y.Parts2y_Views;

[assembly: Xamarin.Forms.Dependency(typeof(Archiver))]
namespace YPS.Droid
{
    public class Archiver : IArchiver
    {
        public void ArchiveText(string filename, string text)
        {
            using (var streamWriter = new StreamWriter(GetFilePath(filename), false))
            {
                streamWriter.Write(text);
            }
        }

        public string UnarchiveText(string filename)
        {
            var text = "";
            var filepath = GetFilePath(filename);
            if (File.Exists(filepath))
            {
                using (var streamReader = new StreamReader(filepath))
                {
                    text = streamReader.ReadToEnd();
                }
            }

            return text;
        }

        string GetFilePath(string filename)
        {
            var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}