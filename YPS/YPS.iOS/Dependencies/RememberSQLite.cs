using SQLite;
using System;
using System.IO;
using Xamarin.Forms;
using YPS.CustomToastMsg;
using YPS.iOS.Dependencies;

[assembly: Dependency(typeof(RememberSQLite))]
namespace YPS.iOS.Dependencies
{
    class RememberSQLite : IRememberSQLIte
    {
        public SQLiteConnection GetConnection()
        {
            var sqliteFilename = "YPS.db3";
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