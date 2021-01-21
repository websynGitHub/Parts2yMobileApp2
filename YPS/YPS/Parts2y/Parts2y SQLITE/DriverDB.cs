using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;

namespace YPS.Parts2y.Parts2y_SQLITE
{
    public class DriverDB
    {
        private SQLiteConnection _sqlconnection;
        public DriverDB(string table)
        {
            _sqlconnection = DependencyService.Get<ISqlite>().GetConnection();
            if (table == "drivervindata")
            {
                _sqlconnection.CreateTable<Drivervindata>();
            }
            else if (table == "drivervehicledetails")
            {
                _sqlconnection.CreateTable<DriverVehicleDetailsModel>();
            }

        }
        public void SaveDriverDetails(Drivervindata data)
        {
            try
            {
                SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(_sqlconnection, data);
            }
            catch (Exception ex)
            {

            }
        }
        public Drivervindata GetDriverDetails()
        {
            try
            {
                var data = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<Drivervindata>(_sqlconnection);
                return data.Where(x => x.Email == Settings.UserMail).FirstOrDefault();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public void SaveDriverVehicleDetails(DriverVehicleDetailsModel vehicledata)
        {
            try
            {
                SQLiteNetExtensions.Extensions.WriteOperations.InsertWithChildren(_sqlconnection, vehicledata);
            }
            catch (Exception ex)
            {

            }
        }
        public DriverVehicleDetailsModel GetDriverVehicleDetails(string vinno,string HRLText)
        {
            try
            {
                var data = SQLiteNetExtensions.Extensions.ReadOperations.GetAllWithChildren<DriverVehicleDetailsModel>(_sqlconnection);
                return data.Where(x => x.Vindata.Vin==vinno&&x.Vindata.Handover_Retrieval_Loading==HRLText).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
