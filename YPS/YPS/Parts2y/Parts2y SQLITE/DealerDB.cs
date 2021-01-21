using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;

namespace YPS.Parts2y.Parts2y_SQLITE
{
    public class DealerDB
    {
        private SQLiteConnection _sqlconnection;
        public DealerDB(string table)
        {
            _sqlconnection = DependencyService.Get<ISqlite>().GetConnection();
            if(table=="dealeralldata")
            {
                _sqlconnection.CreateTable<DealerAllData>();
            }
            else if(table=="dealercarrierdata")
            {
                _sqlconnection.CreateTable<CarrierDataTotal>();
            }
            else if(table=="dealervehicledetails")
            {
                _sqlconnection.CreateTable<DealerVinDetails>();
            }
        }
        public void SaveDealerDetails(DealerAllData data)
        {
            try
            {
                SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(_sqlconnection, data);
            }
            catch (Exception ex)
            {

            }
        }
        public DealerAllData GetDealerDetails()
        {
            try
            {
                var data = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<DealerAllData>(_sqlconnection);
                return data.Where(x => x.Email == Settings.UserMail).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }

            //return _sqlconnection.Table<AllData>().ToList();

        }
        public void SaveEachCarrierDetail(List<CarrierDataTotal> carrierdata)
        {

            try
            {
                var data = GetCarrierInfo(Settings.DealerCarrierNo);

                if (data.Count>0)
                {
                    carrierdata.RemoveAt(0);
                    carrierdata.Add(data[0]);
                    _sqlconnection.UpdateWithChildren(carrierdata[0]);
                }
                else
                {
                    _sqlconnection.InsertAllWithChildren(carrierdata, false);
                }
               // SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(_sqlconnection, carrierdata);
            }
            catch (Exception ex)
            {

            }
        }
        public List<CarrierDataTotal> GetCarrierInfo(string carrierno)
        {
            try
            {
                var data= SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<CarrierDataTotal>(_sqlconnection);
                return data.Where(x => x.Carrierinfo.Carrierno == carrierno).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void UpdateAllVeData(List<DealerVinDetails> data)
        {
            try
            {
                _sqlconnection.UpdateWithChildren(data[0]);
            }
            catch (Exception ex)
            {

            }
        }

        public void SaveAndUpdatDealerVinData(List<DealerVinDetails> dealerVeAllData)
        {
            try
            {
                var val = GetAllVeData();

                if (val.Count > 0)
                {
                    dealerVeAllData.RemoveAt(0);
                    dealerVeAllData.Add(val[0]);
                    _sqlconnection.UpdateWithChildren(dealerVeAllData[0]);
                }
                else
                {
                    dealerVeAllData[0].vin = Settings.VINNo;
                    _sqlconnection.InsertWithChildren(dealerVeAllData[0], false);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<DealerVinDetails> GetAllVeData()
        {
            try
            {
                List<DealerVinDetails> val = new List<DealerVinDetails>();

                 val = _sqlconnection.GetAllWithChildren<DealerVinDetails>(c => c.vin== Settings.VINNo);
                return val;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task UpdateDisplayLoadDataList(List<CarrierDataTotal> loaddata)
        {
            try
            {
                _sqlconnection.UpdateWithChildren(loaddata[0]);
            }
            catch (Exception ex)
            {
            }
        }

    }
}
