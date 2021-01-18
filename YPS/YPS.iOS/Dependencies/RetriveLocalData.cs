using System.Linq;
using YPS.CustomToastMsg;
using YPS.iOS.Dependencies;

[assembly: Xamarin.Forms.Dependency(typeof(RetriveLocalData))]
namespace YPS.iOS.Dependencies
{
    public class RetriveLocalData : ISQLite
    {
        /// <summary>
        /// deleteReadCountNmsg
        /// </summary>
        /// <param name="notifyId"></param>
        public void deleteReadCountNmsg(int notifyId)
        {
            NotifyCountDB countDB = new NotifyCountDB();
            countDB.ClearSpecificNotifications(notifyId);
            NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
            msg.ClearSpecificNotifyMsg(notifyId);
        }

        /// <summary>
        /// getNotificationCountData
        /// </summary>
        /// <returns></returns>
        public int getNotificationCountData()
        {
            NotifyCountDB Count = new NotifyCountDB();
            int notify_count = Count.GetAllNotifications().Count();
            return notify_count;
        }

        /// <summary>
        /// GetAllNotificationList
        /// </summary>
        /// <returns></returns>
        public object GetAllNotificationList()
        {
            NotifyCountDB Count = new NotifyCountDB();
            return Count.GetAllNotifications();

        }

        /// <summary>
        /// deleteAllPNdata
        /// </summary>
        public void deleteAllPNdata()
        {
            NotifyCountDB Count = new NotifyCountDB();
            Count.DeletePNCounts();
            NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
            msg.DeletePNMessages();
        }

        /// <summary>
        /// GetAllMsgsCount
        /// </summary>
        /// <param name="qid"></param>
        /// <returns></returns>
        public int GetAllMsgsCount(int qid)
        {
            NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
            return msg.GetAllNotificationMsgs().Where(x => x.QaId == qid).Count();
        }

        /// <summary>
        /// GetAllNotificationCount
        /// </summary>
        /// <returns></returns>
        public int GetAllNotificationCount()
        {
            NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
            return msg.GetAllNotificationMsgs().Count();
        }
    }
}