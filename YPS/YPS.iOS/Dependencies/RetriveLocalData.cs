using System;
using System.Linq;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.iOS.Dependencies;
using YPS.Service;

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
            try
            {
                NotifyCountDB countDB = new NotifyCountDB();
                countDB.ClearSpecificNotifications(notifyId);
                NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
                msg.ClearSpecificNotifyMsg(notifyId);
            }
            catch(Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "deleteReadCountNmsg method -> in RetriveLocalData.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
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
            try
            {
                NotifyCountDB Count = new NotifyCountDB();
                Count.DeletePNCounts();
                NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
                msg.DeletePNMessages();
            }
            catch(Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "deleteAllPNdata method -> in RetriveLocalData.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
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