using System;
using System.Linq;
using Xamarin.Forms;
using YPS.CustomToastMsg;
using YPS.Droid.Dependencies;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;

[assembly: Dependency(typeof(RetriveLocalData))]
namespace YPS.Droid.Dependencies
{
    public class RetriveLocalData : ISQLite
    {
        public void deleteReadCountNmsg(int notifyId)
        {
            try
            {
                NotifyCountDB countDB = new NotifyCountDB();
                countDB.ClearSpecificNotifications(notifyId);
                NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
                msg.ClearSpecificNotifyMsg(notifyId);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "deleteReadCountNmsg method -> in RetriveLocalData.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        public int getNotificationCountData()
        {

            try
            {
                NotifyCountDB Count = new NotifyCountDB();
                int notify_count = Count.GetAllNotifications().Count();
                return notify_count;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "getNotificationCountData method -> in RetriveLocalData.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
                return 0;
            }

        }

        public object GetAllNotificationList()
        {

            try
            {
                NotifyCountDB Count = new NotifyCountDB();
                return Count.GetAllNotifications();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllNotificationList method -> in RetriveLocalData.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
                return 0;
            }
        }

        public void deleteAllPNdata()
        {
            try
            {
                NotifyCountDB Count = new NotifyCountDB();
                Count.DeletePNCounts();
                NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
                msg.DeletePNMessages();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "deleteAllPNdata method -> in RetriveLocalData.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
            }
        }

        public int GetAllMsgsCount(int qid)
        {
            try
            {
                NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
                return msg.GetAllNotificationMsgs().Where(x => x.QaId == qid).Count();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllMsgsCount method -> in RetriveLocalData.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
                return 0;
            }
        }

        public int GetAllNotificationCount()
        {
            try
            {
                NotifyMessagesCountDB msg = new NotifyMessagesCountDB();
                return msg.GetAllNotificationMsgs().Count();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllNotificationCount method -> in RetriveLocalData.cs " + Settings.userLoginID);
                YPSService service = new YPSService();
                service.Handleexception(ex);
                return 0;
            }
        }
    }
}