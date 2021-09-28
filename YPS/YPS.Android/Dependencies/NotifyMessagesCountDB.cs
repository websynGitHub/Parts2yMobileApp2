using SQLite;
using System.Collections.Generic;
using System.Linq;
using System;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

namespace YPS.Droid.Dependencies
{
    public class NotifyMessagesCountDB
    {
        private SQLiteConnection _sqlconnection;
        YPSService trackService;

        /// <summary>
        /// Getting conection and creating table.
        /// </summary>
        public NotifyMessagesCountDB()
        {
            try
            {
                YPSService trackService;
                _sqlconnection = new SQLiteConnection(SQLite_Android.DbFilePath);
                _sqlconnection.CreateTable<NotifyMessagesCount>();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NotifyMessagesCountDB constructor -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Add new notification messages.
        /// </summary>
        /// <param name="msgs"></param>
        public void SaveNotifyMsg(NotifyMessagesCount msgs)
        {
            try
            {
                _sqlconnection.Insert(msgs);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveNotifyMsg method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Get all notifications messages.
        /// </summary>
        /// <returns></returns>
        public List<NotifyMessagesCount> GetAllNotificationMsgs()
        {
            try
            {
                return (from t in _sqlconnection.Table<NotifyMessagesCount>() select t).ToList();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllNotificationMsgs method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }

        /// <summary>
        /// Specific notification chat messages.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<NotifyMessagesCount> SpecificNotification(int id)
        {
            try
            {
                return (from u in _sqlconnection.Table<NotifyMessagesCount>() where u.QaId == id select u).ToList();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SpecificNotification method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                return null;
            }
        }

        /// <summary>
        /// Delete specific notification chat messages.
        /// </summary>
        /// <param name="id"></param>
        public void ClearSpecificNotifyMsg(int id)
        {
            try
            {
                _sqlconnection.Query<NotifyMessagesCount>("Delete From [NotifyMessagesCount] Where QaId=?", id);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClearSpecificNotifyMsg method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Delete table.
        /// </summary>
        public void DeletePNMessages()
        {
            try
            {
                _sqlconnection.Query<NotifyMessagesCount>("Delete From [NotifyMessagesCount]");
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DeletePNMessages method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}