using SQLite;
using System.Collections.Generic;

namespace YPS.Droid.Dependencies
{
    class NotificationModels
    {
    }

    #region Properties for storing
    public class NotifyCount
    {
        [Unique]
        public int QaId { get; set; }
        public string AllPramText { get; set; }
    }

    public class NotifyMessagesCount
    {
        public int QaId { get; set; }
        public string Msg { get; set; }
        public string TypeChat { get; set; }
        public string AllParams { get; set; }
    }
    #endregion

    #region Properties for getting
    public class NotifyDataModel_Android
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<NotifyHistory_Android> data { get; set; }

        public NotifyDataModel_Android()
        {
            data = new List<NotifyHistory_Android>();
        }
    }
    public class NotifyHistory_Android
    {
        public int UserId { get; set; }
        public string Platform { get; set; }
        public string Message { get; set; }
        public string Parameter { get; set; }
        public List<string> Tags { get; set; }
        public List<string> userss { get; set; }
        public List<string> apnsuserss { get; set; }
        public int NotificationId { get; set; }
        public bool IsRead { get; set; }
        public int NotificationCount { get; set; }
        public string NotifiedOn { get; set; }
        public int listCount { get; set; }
        public int RowNumber { get; set; }
        public string UserName { get; set; }
        public string QATitle { get; set; }
        public int QAID { get; set; }
        public int POID { set; get; }
        public int Status { get; set; }
        public string UpdatedDate { get; set; }
    }
    #endregion
}