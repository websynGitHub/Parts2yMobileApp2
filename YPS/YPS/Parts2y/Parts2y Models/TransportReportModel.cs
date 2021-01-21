using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YPS.Parts2y.Parts2y_Models
{

    public class Loaddetails
    {
        public int UserId { get; set; }
        public int LoadID { get; set; }
        public string Loadnumber { get; set; }
        public string Invoicenumber { get; set; }
        public string Carrierno { get; set; }
        public string Eta { get; set; }
        public string Ata { get; set; }
        public int IsCompleted { get; set; }
        public object LoadList { get; set; }
        public object LoadDetailsInfo { get; set; }
        public object VinDetailsInfo { get; set; }
        public DateTime createdOn { get; set; }= DateTime.Now;
    }

    public class Loaddata
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
        public object Handover_Retrieval_Loading { get; set; }
        public string Status_icon { get; set; }
        public Color bgteamcolor { get; set; }
        public DateTime createOn { get; set; } = DateTime.Now;

    }

    public class GetLoaddata
    {
        [PrimaryKey, AutoIncrement]
        public int loadDataID { get; set; }
        public bool status { get; set; }

        [TextBlob("loaddetails")]
        public Loaddetails Loaddetails { get; set; }
        public string loaddetails { get; set; }

        [TextBlob("loaddata")]
        public List<Loaddata> Loaddata { get; set; }
        public string loaddata { get; set; }
        public string loadno { get; set; }
        public DateTime cteatedOn { get; set; } = DateTime.Now;
    }
    public class Vinlist
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
        public string bgteamcolor { get; set; }
        public string Handover_Retrieval_Loading { get; set; }
    }

    public class Drivervindata
    {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public bool status { get; set; }
        public string TransportRep { get; set; }
        public string Email { get; set; }
        public string Entityname { get; set; }
        public string Jobcount { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        [TextBlob("VinListBlobbed")]
        public List<Vinlist> Vinlist { get; set; }
        public string VinListBlobbed { get; set; }
        public Drivervindata()
        {
            Vinlist = new List<Vinlist>();

        }
    }


    public class Carrierinfo
    {
        public int UserId { get; set; }
        public int LoadID { get; set; }
        // public object Loadnumber { get; set; }
        public string Loadnumber { get; set; }
        public string Invoicenumber { get; set; }
        public string Carrierno { get; set; }
        public string Eta { get; set; }
        // public object Ata { get; set; }
        public string Ata { get; set; }
        public int IsCompleted { get; set; }
        public string LoadList { get; set; }
        public string LoadDetailsInfo { get; set; }
        public string VinDetailsInfo { get; set; }
        public string DealerVinDeatilsinfo { get; set; }
        //public object LoadList { get; set; }
        //public object LoadDetailsInfo { get; set; }
        //public object VinDetailsInfo { get; set; }
        //public object DealerVinDeatilsinfo { get; set; }
    }

    public class Carrierdata
    {
        public string Vin { get; set; }
        public string Color { get; set; }
        public int ModelId { get; set; }
        public string Remark { get; set; }
        public int IsCompleted { get; set; }
        public string Modelname { get; set; }

        public string Status_icon { get; set; }
        public string bgteamcolor { get; set; }
    }

    public class CarrierDataTotal
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public bool status { get; set; }
        [TextBlob("CarrierInfoBlobbed")]
        public Carrierinfo Carrierinfo { get; set; }
        public string CarrierInfoBlobbed { get; set; }
        public DateTime date { get; set; } = DateTime.Now;

        [TextBlob("CarrierDataBlobbed")]
        public List<Carrierdata> Carrierdata { get; set; }
        public CarrierDataTotal()
        {
            Carrierdata = new List<Carrierdata>();
        }
        public string CarrierDataBlobbed { get; set; }
    }
}
