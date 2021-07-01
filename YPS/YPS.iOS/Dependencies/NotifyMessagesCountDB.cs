using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;

namespace YPS.iOS.Dependencies
{
    public class NotifyMessagesCountDB
    {
        private SQLiteConnection _sqlconnection;

        /// <summary>
        /// Getting conection and Creating table.
        /// </summary>
        public NotifyMessagesCountDB()
        {
            try
            {
                _sqlconnection = new SQLiteConnection(IOSSQLite.DbFilePath);
                _sqlconnection.CreateTable<NotifyMessagesCount>();
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "NotifyMessagesCountDB constructor -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Add new Notification Messages.
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
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "SaveNotifyMsg method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Get all Notifications  Messages.
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
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "GetAllNotificationMsgs method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                return new List<NotifyMessagesCount>();
            }
        }

        /// <summary>
        /// Specific Notification Chat Messages.
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
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "SpecificNotification method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                return new List<NotifyMessagesCount>();
            }
        }

        /// <summary>
        /// Delete specific Notification chat Messages.
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
                YPSService trackService = new YPSService();
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
                _sqlconnection.Query<NotifyCount>("Delete From [NotifyMessagesCount]");
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "DeletePNMessages method -> in NotifyMessagesCountDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}