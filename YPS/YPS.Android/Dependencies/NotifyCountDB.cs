using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

namespace YPS.Droid.Dependencies
{
    public class NotifyCountDB
    {
        private SQLiteConnection _sqlconnection;
        YPSService trackService;
        /// <summary>
        /// Getting conection and creating table.
        /// </summary>
        public NotifyCountDB()
        {
            try
            {
                trackService = new YPSService();
                _sqlconnection = new SQLiteConnection(SQLite_Android.DbFilePath);
                _sqlconnection.CreateTable<NotifyCount>();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NotifyCountDB constructor -> in NotifyCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Add new Notification to DB .
        /// </summary>
        /// <param name="count"></param>
        public void SaveNotifyCount(NotifyCount count)
        {
            try
            {
                _sqlconnection.Insert(count);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveNotifyCount method -> in NotifyCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Get all Notifications.
        /// </summary>
        /// <returns></returns>
        public List<NotifyCount> GetAllNotifications()
        {
            try
            {
                return (from t in _sqlconnection.Table<NotifyCount>() select t).ToList();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllNotifications method -> in NotifyCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }

        /// <summary>
        /// Return title and qaid.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllNotificationTitle()
        {
            try
            {
                return (from t in _sqlconnection.Table<NotifyCount>() select t.AllPramText).ToList();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllNotificationTitle method -> in NotifyCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }

        /// <summary>
        /// Specific notification.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<NotifyCount> SpecificNotification(int id)
        {
            try
            {
                return _sqlconnection.Query<NotifyCount>("Select * From [NotifyCount] Where QaId=?", id);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SpecificNotification method -> in NotifyCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }

        /// <summary>
        /// Delete specific notification.
        /// </summary>
        /// <param name="id"></param>
        public void ClearSpecificNotifications(int id)
        {
            try
            {
                _sqlconnection.Query<NotifyCount>("Delete From [NotifyCount] Where QaId=?", id);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClearSpecificNotifications method -> in NotifyCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Delete table.
        /// </summary>
        public void DeletePNCounts()
        {
            try
            {
                _sqlconnection.Query<NotifyCount>("Delete From [NotifyCount]");
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DeletePNCounts method -> in NotifyCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}