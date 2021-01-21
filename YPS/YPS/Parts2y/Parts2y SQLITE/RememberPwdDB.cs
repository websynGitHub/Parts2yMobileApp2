using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Models;

namespace YPS.Parts2y.Parts2y_SQLITE
{
    #region Remember me for Parst2y
    public class RememberPwdDB
    {
        private SQLiteConnection _sqlconnection;
        public RememberPwdDB()
        {
            try
            {
                _sqlconnection = DependencyService.Get<ISqlite>().GetConnection();
                _sqlconnection.CreateTable<Parts2y_Models.UserDetails>();
            }
            catch (Exception ex)
            {

            }
        }
        public void SaveUserPWd(Parts2y_Models.UserDetails user)
        {
            try
            {
                _sqlconnection.Insert(user);

            }
            catch (Exception ex)
            {

            }
        }
        public List<Parts2y_Models.UserDetails> GetUserDetails()
        {
            try
            {
                return (from t in _sqlconnection.Table<Parts2y_Models.UserDetails>() select t).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void ClearUserDetail(int id)
        {
            try
            {
                _sqlconnection.Query<Parts2y_Models.UserDetails>("Delete From [UserDetails] Where ID=?", id);
            }
            catch (Exception ex)
            {

            }
            //  _sqlconnection.Delete<RememberPwd>(id);
        }
        #endregion
    }
}
