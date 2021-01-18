using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace YPS.Droid.Dependencies
{
    public class NotifyMessagesCountDB
    {
        private SQLiteConnection _sqlconnection;

        /// <summary>
        /// Getting conection and creating table.
        /// </summary>
        public NotifyMessagesCountDB()
        {
            _sqlconnection = new SQLiteConnection(SQLite_Android.DbFilePath);
            _sqlconnection.CreateTable<NotifyMessagesCount>();
        }

        /// <summary>
        /// Add new notification messages.
        /// </summary>
        /// <param name="msgs"></param>
        public void SaveNotifyMsg(NotifyMessagesCount msgs)
        {
            _sqlconnection.Insert(msgs);
        }

        /// <summary>
        /// Get all notifications messages.
        /// </summary>
        /// <returns></returns>
        public List<NotifyMessagesCount> GetAllNotificationMsgs()
        {
            return (from t in _sqlconnection.Table<NotifyMessagesCount>() select t).ToList();
        }

        /// <summary>
        /// Specific notification chat messages.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<NotifyMessagesCount> SpecificNotification(int id)
        {
            return (from u in _sqlconnection.Table<NotifyMessagesCount>() where u.QaId == id select u).ToList();
        }

        /// <summary>
        /// Delete specific notification chat messages.
        /// </summary>
        /// <param name="id"></param>
        public void ClearSpecificNotifyMsg(int id)
        {
            _sqlconnection.Query<NotifyMessagesCount>("Delete From [NotifyMessagesCount] Where QaId=?", id);
        }

        /// <summary>
        /// Delete table.
        /// </summary>
        public void DeletePNMessages()
        {
            _sqlconnection.Query<NotifyMessagesCount>("Delete From [NotifyMessagesCount]");
        }
    }
}