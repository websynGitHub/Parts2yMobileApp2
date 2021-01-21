using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using SQLite;
using UIKit;
using Xamarin.Forms;
using YPS.iOS.Parts2y.SqliteiOS;
using YPS.Parts2y.Parts2y_SQLITE;

[assembly: Dependency(typeof(iOS_Sqlite))]
namespace YPS.iOS.Parts2y.SqliteiOS
{
    public class iOS_Sqlite : ISqlite
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "YPSePODDB.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); /// Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);
            /// Create the connection
            var conn = new SQLiteConnection(path);
            /// Return the database connection
            return conn;
        }
    }
}