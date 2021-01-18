using SQLite;

namespace YPS.iOS.Dependencies
{
    class NotificationModels{}
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
    }
}