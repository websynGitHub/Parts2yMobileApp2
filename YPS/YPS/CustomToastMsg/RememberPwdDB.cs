using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;

namespace YPS.CustomToastMsg
{
    public class RememberPwdDB
    {
        private SQLiteConnection _sqlconnection;

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public RememberPwdDB()
        {
            try
            {
                /// Getting conection and creating table
                _sqlconnection = DependencyService.Get<IRememberSQLIte>().GetConnection();
                _sqlconnection.CreateTable<RememberPwd>();
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "RememberPwdDB constructor -> in rememberPwsDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Add new user to DB.
        /// </summary>
        /// <param name="user"></param>
        public void SaveUserPWd(RememberPwd user)
        {
            try
            {
                _sqlconnection.Insert(user);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "SaveUserPWd method -> in rememberPwsDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// UpdatePWd
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ID"></param>
        public void UpdatePWd(RememberPwd user, int ID)
        {
            try
            {
                var data = _sqlconnection?.Table<RememberPwd>()?.ToList();
                var getUpdate = data?.Where(wr => (Convert.ToInt32(EncryptManager.Decrypt(wr.encUserId))) == ID).FirstOrDefault();
                getUpdate.tagnamelabel = user?.tagnamelabel;
                getUpdate.descrptionlabel = user?.descrptionlabel;
                getUpdate.GivenName = user?.GivenName;
                getUpdate.SelectedScanRule = user?.SelectedScanRule;
                getUpdate.EnteredScanTotal = user?.EnteredScanTotal;
                _sqlconnection?.Update(getUpdate);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "UpdatePWd method -> in rememberPwsDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns></returns>
        public List<RememberPwd> GetUserDetails()
        {
            return (from t in _sqlconnection.Table<RememberPwd>() select t).ToList();
        }

        /// <summary>
        /// Delete specific user.
        /// </summary>
        /// <param name="id"></param>
        public void ClearUserDetail(int id)
        {
            try
            {
                _sqlconnection.Query<RememberPwd>("Delete From [RememberPwd] Where UserId=?", id);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "ClearUserDetail method -> in rememberPwsDB.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }
    }
}
