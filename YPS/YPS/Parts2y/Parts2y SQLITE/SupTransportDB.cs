
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using static YPS.Parts2y.Parts2y_Models.PhotoModel;

namespace YPS.Parts2y.Parts2y_SQLITE
{
    public class SupTransportDB
    {
        private SQLiteConnection _conn;
        public SupTransportDB(string table)
        {
            _conn = DependencyService.Get<ISqlite>().GetConnection();


            if (table == "vedetails")
            {
                _conn.CreateTable<VehicleDetailsModel>();
            }
            else if (table == "image")
            {
                _conn.CreateTable<CustomPhotoModel>();
            }
            else if (table == "loaddata")
            {
                _conn.CreateTable<GetLoaddata>();
            }
            else if (table == "allpodata")
            {
                _conn.CreateTable<AllData>();
            }
        }

        public void SaveTransportDetails(AllData data)
        {
            try
            {
                SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(_conn, data);
            }
            catch (Exception ex)
            {

            }
        }
        public AllData GetTransportDetails()
        {
            try
            {
                //return _sqlconnection.Table<AllData>().ToList();
                var data = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<AllData>(_conn);
                return data.Where(x => x.Email == Settings.UserMail).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region Added By Rameez
        public void SaveAndUpdateAllVeData(List<VehicleDetailsModel> veAllData)
        {
            try
            {
                var val = GetAllVeData();

                if (val.Count > 0)
                {
                    #region to update the data with updated values that is in Loacl DB (Dynamic)
                    veAllData.RemoveAt(0);
                    veAllData.Add(val[0]);
                    #endregion


                    //#region to update with the API value which is static
                    //veAllData[0].VeDetailsID = val[0].VeDetailsID;
                    //veAllData[0].vinno = val[0].vinno;
                    //#endregion
                    _conn.UpdateWithChildren(veAllData[0]);
                }
                else
                {
                    veAllData[0].vinno = Settings.VINNo;
                    _conn.InsertWithChildren(veAllData[0], false);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<VehicleDetailsModel> GetAllVeData()
        {
            List<VehicleDetailsModel> val = new List<VehicleDetailsModel>();
            try
            {
                val = _conn.GetAllWithChildren<VehicleDetailsModel>(c => c.vinno == Settings.VINNo);
                return val;
            }
            catch (Exception ex)
            {
                return val;
            }
        }

        public void UpdateAllVeData(List<VehicleDetailsModel> data)
        {
            try
            {
                _conn.UpdateWithChildren(data[0]);
            }
            catch (Exception ex)
            {

            }
        }


        public void SaveImage(ObservableCollection<CustomPhotoModel> photo)
        {
            try
            {
                _conn.InsertAll(photo);
            }
            catch (Exception ex)
            {

            }
        }

        public void SaveUpdateLoadData(List<GetLoaddata> loadData)
        {
            try
            {
                var loaddata = GetLoadData();

                if (loaddata.Count > 0)
                {


                    #region to update the data with updated values that is in Loacl DB (Dynamic)
                    loadData.RemoveAt(0);
                    loadData.Add(loaddata[0]);
                    #endregion


                    //#region to update with the API value which is static
                    //loadData[0].loadDataID = loaddata[0].loadDataID;
                    //loadData[0].loadno = loaddata[0].loadno;
                    //#endregion
                    _conn.UpdateWithChildren(loadData[0]);
                }
                else
                {
                    loadData[0].loadno = Settings.LoadNo;
                    _conn.CreateTable<GetLoaddata>();
                    _conn.InsertAllWithChildren(loadData, false);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<GetLoaddata> GetLoadData()
        {
            List<GetLoaddata> data = new List<GetLoaddata>();
            try
            {
                data = _conn.GetAllWithChildren<GetLoaddata>(wr => wr.loadno == Settings.LoadNo);
                return data;
            }
            catch (Exception ex)
            {
                return data;
            }
        }

        public async Task UpdateDisplayLoadDataList(List<GetLoaddata> loaddata)
        {
            try
            {
                _conn.UpdateWithChildren(loaddata[0]);
            }
            catch (Exception ex)
            {
            }
        }

        public List<CustomPhotoModel> GetImage()
        {
            try
            {
                var data = _conn.Table<CustomPhotoModel>().Where(wr => wr.vinno == Settings.VINNo && wr.queseq == Settings.QNo_Sequence&&wr.email==Settings.UserMail).ToList();
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion


        //public List<AllData> GetTransportDetails()
        //{

        //   return _sqlconnection.Table<AllData>().ToList();
        // //  return (from t in _sqlconnection.Table<offlineTransportReport>() select t).ToList();
        //}
    }
}
