using SQLite;

namespace YPS.CustomToastMsg
{
    public interface ISQLite
    {
        int getNotificationCountData();

        void deleteReadCountNmsg(int NotifyId);

        object GetAllNotificationList();

        void deleteAllPNdata();

        int GetAllMsgsCount(int qid);
        int GetAllNotificationCount();
    }

    public interface IRememberSQLIte
    {
        SQLiteConnection GetConnection();
    }
}
