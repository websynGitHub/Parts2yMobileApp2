using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;

namespace YPS.CustomToastMsg
{
    public class PhotoRepoSQlLite
    {
        private SQLiteConnection _sqlconnection;

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public PhotoRepoSQlLite()
        {
            try
            {
                /// Getting conection and creating table
                _sqlconnection = DependencyService.Get<IRememberSQLIte>().GetConnection();
                _sqlconnection.CreateTable<PhotoRepoModel>();
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "PhotoRepoSQlLite Constructor -> in PhotoRepoSQlLite.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Add new user to DB.
        /// </summary>
        /// <param name="user"></param>
        public void SavePhoto(PhotoRepoModel photodetails)
        {
            try
            {
                _sqlconnection.Insert(photodetails);
            }
            catch (Exception ex)
            {
                YPSService trackService = new YPSService();
                YPSLogger.ReportException(ex, "SavePhoto method -> in PhotoRepoSQlLite.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns></returns>
        public List<PhotoRepoModel> GetPhotosOfUser()
        {
            try
            {
                return (from t in _sqlconnection.Table<PhotoRepoModel>() where t.UserID == Settings.userLoginID select t).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        /// <returns></returns>
        public void DeleteOfUser(int photoid)
        {
            try
            {
                if (photoid != 0)
                {
                    _sqlconnection.Table<PhotoRepoModel>().Where(wr => wr.PhotoID == photoid &&
       wr.UserID == Settings.userLoginID).Delete();
                }
                else
                {
                    _sqlconnection.Table<PhotoRepoModel>().Where(wr => wr.UserID == Settings.userLoginID).Delete();
                }

            }
            catch (Exception ex)
            {

            }
        }

    }
}
