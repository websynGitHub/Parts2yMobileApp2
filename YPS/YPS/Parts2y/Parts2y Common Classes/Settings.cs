using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YPS.Parts2y.Parts2y_Common_Classes
{
    public static class Settings
    {
        public static string WebServiceUrl = "https://ypsepod.azurewebsites.net/api/";
        public static string LoadNo { get; set; }
        public static string CarrierNo { get; set; }
        public static string VINNo { get; set; }
        public static int ID { get; set; }
        public static Color Bar_Background { get; set; }
        public static string UserName { get; set; }
        public static string UserMail { get; set; }
        public static string Entity_Name { get; set; }
        public static int roleid { get; set; }
        public static string jobcount { get; set; }
        public static string HRLtext { get; set; }
        public static string Scantotext { get; set; }
        public static string Scantodscr { get; set; }
        public static string Scanfromtext { get; set; }
        public static string Scanfromdscr { get; set; }
        public static string Scandatefromtime { get; set; }
        public static string Scandatetotime { get; set; }
        public static string Scanlocationto { get; set; }
        public static string Scanlocationfrom { get; set; }
        public static string ScanFrom_ZoneClusterBayCellfrom { get; set; }
        public static string ScanFrom_ZoneClusterBayCellto { get; set; }
        public static int QNo_Sequence { get; set; }
        public static int IscompltedRecord { get; set; }
        public static string ScanCompleted { get; set; }
        public static string PDICompleted { get; set; }
        public static string LoadCompleted { get; set; }

        public static string Sfrom_latitude { get; set; }
        public static string Sfrom_longitude { get; set; }
        public static string Sto_latitude { get; set; }
        public static string Sto_longitude { get; set; }

        public static string DealerCarrierNo { get; set; }

        public static string VersionName { get; set; }

        public static bool LoadTickVisibility { get; set; } = false;
        public static bool isScanCompleted { get; set; } = false;
        public static bool isPDICompleted { get; set; } = false;
        public static bool isLoadCompleted { get; set; } = false;

        public static string scanQRValueA{ get; set; } = "";
        public static string scanQRValueB { get; set; } = "";


    }
}
