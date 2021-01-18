using System.Linq;
using Xamarin.Forms;
using YPS.CustomToastMsg;
using YPS.Droid.Dependencies;

[assembly: Dependency(typeof(RetriveLocalData))]
namespace YPS.Droid.Dependencies
{
    public class RetriveLocalData : ISQLite
    {
        public void deleteReadCountNmsg(int notifyId)
        {
            NotifyCountDB countDB = new NotifyCountDB();
            countDB.ClearSpecificNotifications(notifyId);
            NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
            msg.ClearSpecificNotifyMsg(notifyId);
        }

        public int getNotificationCountData()
        {
            NotifyCountDB Count = new NotifyCountDB();
            int notify_count= Count.GetAllNotifications().Count();
            return notify_count;
        }

        public object GetAllNotificationList()
        {
            NotifyCountDB Count = new NotifyCountDB();
            return Count.GetAllNotifications();

        }

        public void deleteAllPNdata()
        {
            NotifyCountDB Count = new NotifyCountDB();
            Count.DeletePNCounts();
            NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
            msg.DeletePNMessages();
        }

        public int GetAllMsgsCount(int qid)
        {
            NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
            return msg.GetAllNotificationMsgs().Where(x=>x.QaId==qid).Count();
        }

        public int GetAllNotificationCount()
        {
            NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
            return msg.GetAllNotificationMsgs().Count();
        }
    }
}