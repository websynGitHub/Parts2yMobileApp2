using SQLite;
using System.Collections.Generic;
using System.Linq;

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
            _sqlconnection = new SQLiteConnection(IOSSQLite.DbFilePath);
            _sqlconnection.CreateTable<NotifyMessagesCount>();
        }

        /// <summary>
        /// Add new Notification Messages.
        /// </summary>
        /// <param name="msgs"></param>
        public void SaveNotifyMsg(NotifyMessagesCount msgs)
        {
            _sqlconnection.Insert(msgs);
        }

        /// <summary>
        /// Get all Notifications  Messages.
        /// </summary>
        /// <returns></returns>
        public List<NotifyMessagesCount> GetAllNotificationMsgs()
        {
            return (from t in _sqlconnection.Table<NotifyMessagesCount>() select t).ToList();
        }

        /// <summary>
        /// Specific Notification Chat Messages.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<NotifyMessagesCount> SpecificNotification(int id)
        {
            return (from u in _sqlconnection.Table<NotifyMessagesCount>() where u.QaId == id select u).ToList();
        }

        /// <summary>
        /// Delete specific Notification chat Messages.
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
            _sqlconnection.Query<NotifyCount>("Delete From [NotifyMessagesCount]");
        }
    }
}