using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YPS.Droid.Parts2y.Parts2y_Dependencies;
using YPS.Parts2y.Parts2y_SQLITE;

[assembly: Dependency(typeof(RememberParts2ySQLite))]
namespace YPS.Droid.Parts2y.Parts2y_Dependencies
{
    public class RememberParts2ySQLite : ISqlite
    {
        SQLiteConnection connection = null;

        public SQLiteConnection GetConnection()
        {
            try
            {
                var dbase = "YPSePODDB.db3";
                string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData); // Documents folder
                var path = System.IO.Path.Combine(documentsPath, dbase);
                connection = new SQLiteConnection(path);
            }
            catch (Exception ex)
            {
                //YPSLogger.ReportException(ex, "GetConnection method -> in RememberSQLite.cs " + Settings.userLoginID);
                //YPSService service = new YPSService();
                //service.Handleexception(ex);
            }
            return connection;
        }
    }
}