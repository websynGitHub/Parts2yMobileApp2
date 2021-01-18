using SQLite;
using System;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.Service;

[assembly: Dependency(typeof(RememberSQLite))]
namespace YPS.Droid.Dependencies
{
    public class RememberSQLite : IRememberSQLIte
    {
        SQLiteConnection connection = null;

        /// <summary>
        /// Gets connection.
        /// </summary>
        /// <returns></returns>
        public SQLiteConnection GetConnection()
        {
            try
            {
                var dbase = "YpsRemember.db3";
                string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData); // Documents folder
                var path = System.IO.Path.Combine(documentsPath, dbase);
                connection = new SQLiteConnection(path);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetConnection method -> in RememberSQLite.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
            return connection;
        }
    }
}