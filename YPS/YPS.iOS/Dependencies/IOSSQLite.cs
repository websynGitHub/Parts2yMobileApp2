using System;
using System.IO;

namespace YPS.iOS.Dependencies
{
    public class IOSSQLite 
    {
        public static readonly string DbFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "yps.db");
    }
}
