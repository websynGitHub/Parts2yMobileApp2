using SQLite;
using System;
using System.IO;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.iOS.Dependencies;
using YPS.Service;

[assembly: Dependency(typeof(RememberSQLite))]
namespace YPS.iOS.Dependencies
{
    class RememberSQLite : IRememberSQLIte
    {
        public SQLiteConnection GetConnection()
        {
            try
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
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "GetConnection method -> in RememberSQLite.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }
    }
}