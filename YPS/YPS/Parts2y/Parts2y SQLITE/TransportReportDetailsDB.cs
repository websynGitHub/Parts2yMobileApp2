using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_Models.SqliteModels;

namespace YPS.Parts2y.Parts2y_SQLITE
{
    public class TransportReportDetailsDB
    {
        private SQLiteConnection _conn;

        public TransportReportDetailsDB(string table)
        {
            try
            {
                if (table == "loaddata")
                {
                    _conn = DependencyService.Get<ISqlite>().GetConnection();
                    _conn.CreateTable<LoadData>();
                }
                else if (table == "loaddetails")
                {
                    _conn = DependencyService.Get<ISqlite>().GetConnection();
                    _conn.CreateTable<LoadDetails>();
                }
                _conn = DependencyService.Get<ISqlite>().GetConnection();
                _conn.CreateTable<AllData>();
            }
            catch (Exception ex)
            {

            }
        }

        public void SaveLoadData(List<GetLoaddata> loadData)
        {
            try
            {
                _conn.InsertAll(loadData);
            }
            catch (Exception ex)
            {

            }
        }

        public List<AllData> GetTransportData()
        {
            try
            {
                return SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<AllData>(_conn);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<LoadData> GetLoadData(string loadNo)
        {
            try
            {
                var data = _conn.Table<LoadData>().Where(wr => wr.loadNumber == loadNo).ToList();
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void ClearTableData()
        {
            try
            {
                _conn.DeleteAll<LoadData>();
                _conn.DeleteAll<LoadDetails>();
            }
            catch (Exception ex)
            {

            }
        }

        //LOAD DETAILS

        public void SaveLoadDetails(LoadDetails loadDetails)
        {
            try
            {
                _conn.Insert(loadDetails);
            }
            catch (Exception ex)
            {

            }
        }

        public LoadDetails GetLoadDetails(string loadno)
        {
            try
            {
                var data = _conn.Table<LoadDetails>().Where(wr => wr.Loadnumber == loadno).FirstOrDefault();
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}