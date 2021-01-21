using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YPS.Parts2y.Parts2y_Models
{


    public class LoadList
    {
        //[PrimaryKey]
        public int ID { get; set; }
        public string Loadnumber { get; set; }
        public int UserId { get; set; }
        public int LoadID { get; set; }
        public string Invoicenumber { get; set; }
        public string Carrierno { get; set; }
        public string Eta { get; set; }
        public string Ata { get; set; }
        public int IsCompleted { get; set; }
        public string Status_icon { get; set; }

        //commented by sindhu because of not using anywhere
       // public Color Icon_Color { get; set; }
       //changed to string from color because of saving in sqlite
        public string bgteamcolor { get; set; }

        //public object LoadList { get; set; }
        //public object LoadDetailsInfo { get; set; }
        //public object VinDetailsInfo { get; set; }
    }

    public class AllData
    {
        //[PrimaryKey]
        //public string Id { get; set; }
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }

        public bool status { get; set; }
        public string TransportRep { get; set; }
        public string Email { get; set; }
        public string Jobcount { get; set; }
        public string Entityname { get; set; }
        public DateTime date { get; set; } = DateTime.Now;

        [TextBlob("LoadListBlobbed")]
        public List<LoadList> LoadList { get; set; }

        public string LoadListBlobbed { get; set; }
        public AllData()
        {
            LoadList = new List<LoadList>();
            //ID = new Guid().ToString();

        }
    }



    public class Dealerdata
    {
        public int UserId { get; set; }
        public int LoadID { get; set; }
        // public object Loadnumber { get; set; }
        public string Loadnumber { get; set; }
        public string Invoicenumber { get; set; }
        public string Carrierno { get; set; }
        public string Eta { get; set; }
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
        public string Status_icon { get; set; }
       // public Color Icon_Color { get; set; }
        public string bgteamcolor { get; set; }
    }

    public class DealerAllData
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public bool status { get; set; }
        public string Dealername { get; set; }
        public string Email { get; set; }
        public string Entityname { get; set; }
        public string DealerId { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
        public string Delivery { get; set; }
        [TextBlob("DealerDataBlobbed")]
        public List<Dealerdata> Dealerdata { get; set; }
        public string DealerDataBlobbed { get; set; }

        public DealerAllData()
        {
            Dealerdata = new List<Dealerdata>();

        }
    }



}
