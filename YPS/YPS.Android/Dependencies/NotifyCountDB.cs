using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace YPS.Droid.Dependencies
{
    public class NotifyCountDB
    {
        private SQLiteConnection _sqlconnection;

        /// <summary>
        /// Getting conection and creating table.
        /// </summary>
        public NotifyCountDB()
        {
            _sqlconnection = new SQLiteConnection(SQLite_Android.DbFilePath);
            _sqlconnection.CreateTable<NotifyCount>();
        }

        /// <summary>
        /// Add new Notification to DB .
        /// </summary>
        /// <param name="count"></param>
        public void SaveNotifyCount(NotifyCount count)
        {
            _sqlconnection.Insert(count);
        }

        /// <summary>
        /// Get all Notifications.
        /// </summary>
        /// <returns></returns>
        public List<NotifyCount> GetAllNotifications()
        {
            return (from t in _sqlconnection.Table<NotifyCount>() select t).ToList();
        }

        /// <summary>
        /// Return title and qaid.
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllNotificationTitle()
        {
            return (from t in _sqlconnection.Table<NotifyCount>() select t.AllPramText).ToList();
        }

        /// <summary>
        /// Specific notification.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<NotifyCount> SpecificNotification(int id)
        {
            return _sqlconnection.Query<NotifyCount>("Select * From [NotifyCount] Where QaId=?", id);
        }

        /// <summary>
        /// Delete specific notification.
        /// </summary>
        /// <param name="id"></param>
        public void ClearSpecificNotifications(int id)
        {
            _sqlconnection.Query<NotifyCount>("Delete From [NotifyCount] Where QaId=?", id);
        }

        /// <summary>
        /// Delete table.
        /// </summary>
        public void DeletePNCounts()
        {
            _sqlconnection.Query<NotifyCount>("Delete From [NotifyCount]" );
        }
    }
}