using SQLite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace YPS.Parts2y.Parts2y_Models.SqliteModels
{
    public class LoadDetails
    {
        [PrimaryKey, AutoIncrement]
        public int loadDetailsID { get; set; }
        public int UserId { get; set; }
        public int LoadID { get; set; }
        public string Loadnumber { get; set; }
        public string Invoicenumber { get; set; }
        public string Carrierno { get; set; }
        public string Eta { get; set; }
        public string Ata { get; set; }
        public int IsCompleted { get; set; }
        public string LoadList { get; set; }
        public string LoadDetailsInfo { get; set; }
        public string VinDetailsInfo { get; set; }
        public DateTime createdOn { get; set; } = DateTime.Now;
    }

    public class LoadData
    {
        [PrimaryKey, AutoIncrement]
        public int loadDataID { get; set; }
        public int VinDetailsID { get; set; }
        public string Vin { get; set; }
        public string Trip { get; set; }
        public string PDI { get; set; }
        public string Loaded { get; set; }
        public int IsCompleted { get; set; }
        public string Call { get; set; }
        public string Eta { get; set; }
        public string Ata { get; set; }
        public string Handover_Retrieval_Loading { get; set; }
        public string Status_icon { get; set; }
        public string bgteamcolor { get; set; }
        public string loadNumber { get; set; }
        public DateTime createdOn { get; set; } = DateTime.Now;
    }

    public class AllLoadData
    {
        public bool status { get; set; }
        public LoadDetails loaddetails { get; set; }
        public List<LoadData> loaddata { get; set; }
    }
    public class VINList
    {
        public int VinDetailsID { get; set; }
        public string Vin { get; set; }
        public string Trip { get; set; }
        public string PDI { get; set; }
        public string Loaded { get; set; }
        public int IsCompleted { get; set; }
        public string Call { get; set; }
        public string Eta { get; set; }
        public string Ata { get; set; }
        public string Status_icon { get; set; }
        public Color bgteamcolor { get; set; }
        public string Handover_Retrieval_Loading { get; set; }
    }

    public class DriverVINData
    {
        public bool status { get; set; }
        public string TransportRep { get; set; }
        public string Email { get; set; }
        public string Entityname { get; set; }
        public string Jobcount { get; set; }
        public List<Vinlist> Vinlist { get; set; }
    }


    public class CarrierInfo
    {
        public int UserId { get; set; }
        public int LoadID { get; set; }
        public object Loadnumber { get; set; }
        public string Invoicenumber { get; set; }
        public string Carrierno { get; set; }
        public string Eta { get; set; }
        public object Ata { get; set; }
        public int IsCompleted { get; set; }
        public object LoadList { get; set; }
        public object LoadDetailsInfo { get; set; }
        public object VinDetailsInfo { get; set; }
        public object DealerVinDeatilsinfo { get; set; }
    }

    public class CarrierData
    {
        public string Vin { get; set; }
        public string Color { get; set; }
        public int ModelId { get; set; }
        public string Remark { get; set; }
        public int IsCompleted { get; set; }
        public string Modelname { get; set; }

        public string Status_icon { get; set; }
        public Color bgteamcolor { get; set; }
    }

    public class CarrierDataTotal
    {
        public bool status { get; set; }
        public Carrierinfo Carrierinfo { get; set; }
        public List<Carrierdata> Carrierdata { get; set; }
    }
}
