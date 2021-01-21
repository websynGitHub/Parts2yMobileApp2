using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_View_Models;
using static YPS.Parts2y.Parts2y_Models.PhotoModel;

namespace YPS.Parts2y.Parts2y_Models
{

    public class Vindata
    {
        public string Vin { get; set; }
        public string Loadnumber { get; set; }
        public string ScanCompleted { get; set; }
        public string ScanStatus { get; set; }
        public string PDICompleted { get; set; }
        public string Load { get; set; }
        public int ModelID { get; set; }
        public string Colour { get; set; }
        public string Carrier { get; set; }
        public string Scancolour { get; set; }
        public string Eta { get; set; }
        public string Ata { get; set; }
        public string Modelname { get; set; }
        public string ScanFrom_text { get; set; }
        public string ScanaFrom_Description { get; set; }
        public string ScanTo_text { get; set; }
        public string ScanTo_Description { get; set; }
        public string ScanFrom_Scanned { get; set; }
        public string ScanFrom_DateTime { get; set; }

        public String ScanFrom_ZoneClusterBayCell { get; set; }
        public string ScanTo_ZoneClusterBayCell { get; set; }
        public string Trip { get; set; }

        public string ScanFrom_Gpslocation { get; set; }
        public string Handover_Retrieval_Loading { get; set; }
        public string ScanOn { get; set; }
        public string ScanTo_DateTime { get; set; }
        public string ScanTo_Gpslocation { get; set; }

        public string Sfrom_latitude { get; set; }
        public string Sfrom_longitude { get; set; }
        public string Sto_latitude { get; set; }
        public string Sto_longitude { get; set; }
        public DateTime createOn { get; set; } = DateTime.Now;
    }

    public class ListOption
    {
        public int ObsID { get; set; }
        public string Observation { get; set; }

        public bool Isanswered { get; set; }

        //public bool Radio_Selected { get; set; } = false;
        public bool unSelected { get; set; } = true;
        //public bool isSelected { get; set; } = false;
    }
    public class DriverVehicleDetailsModel
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public bool status { get; set; }
        [TextBlob("VinDataBlobbed")]
        public Vindata Vindata { get; set; }
        public string VinDataBlobbed { get; set; }
        public DateTime date { get; set; } = DateTime.Now;
    }

    public class VehicleDetailsModel
    {
        [PrimaryKey, AutoIncrement]
        public int VeDetailsID { get; set; }
        public string vinno { get; set; }
        public bool status { get; set; }
        [TextBlob("BlobVinData")]
        public Vindata Vindata { get; set; }
        public string BlobVinData { get; set; }
        [TextBlob("BlobCPQuestionsdata")]
        public ObservableCollection<CPQuestionsdata> cpquestions { get; set; }
        public string BlobCPQuestionsdata { get; set; }
        [TextBlob("BlobPdiCategories")]
        public List<string> PdiCategories { get; set; }
        public string BlobPdiCategories { get; set; }
        public byte[] signatureAuditBase64 { get; set; }
        public byte[] signatureSupervisorBase64 { get; set; }
        public DateTime createOn { get; set; } = DateTime.Now;
    }

    public class ScanDetailsModel
    {
        public bool status { get; set; }
        public Vindata Vindata { get; set; }
    }




    public class CPQuestionsdata : BaseViewModel
    {
        public int PDI_Qno { get; set; }
        public string PDI_Qcat { get; set; }
        public string PDI_QestionTitle { get; set; }
        public int Seqno { get; set; }

        public string Remarks { get; set; }
        public string AnsweredRemarks { get; set; }
        public string RightSwipeText { get; set; }
        public int RightSwipeSelectedIndex { get; set; }
        public object cpquestions { get; set; }
        public Color AnsweredQstnBgColor { get; set; }
        public Color NotAnsweredBgColor { get; set; }

        private ObservableCollection<ListOption> _listOptions;

        private bool isVisible = false;
        public ObservableCollection<ListOption> listOptions
        {
            get { return _listOptions; }
            set
            {
                _listOptions = value;
                this.OnPropertyChanged("listOptions");
            }
        }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                this.OnPropertyChanged("IsVisible");
            }
        }
        public bool isAnswered { get; set; } = false;
        public DateTime createOn { get; set; } = DateTime.Now;
    }

    public class CPQuestionsslist
    {
        public ObservableCollection<CPQuestionsdata> CPQuestionsdata { get; set; }
        public object PDICategories { get; set; }
    }

    public class QuestionarieeList
    {
        public string message { get; set; }
        public int status { get; set; }
        public CPQuestionsslist CPQuestionsslist { get; set; }
    }



    public class DealerDetailsData
    {
        public string Vin { get; set; }
        public int ModelId { get; set; }
        public string Color { get; set; }
        public string Carrier { get; set; }
        public string ScanCompleted { get; set; }
        public string Inspection { get; set; }
        public string Scanstatus { get; set; }
        public string Pod { get; set; }
        public string Remarks { get; set; }
        public string ScanOn { get; set; }
        public string Modelname { get; set; }
    }




    public class DealerVinDetails
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public bool status { get; set; }
        public string vin { get; set; }
        [TextBlob("DealerDataBlobbed")]
        public DealerDetailsData Dealerdata { get; set; }
        public string DealerDataBlobbed { get; set; }
        [TextBlob("VinCheckListBlobbed")]
        public ObservableCollection<CPQuestionsdata> VinCheckList { get; set; }
        public string VinCheckListBlobbed { get; set; }
        [TextBlob("PDICategoriessBlobbed")]
        public List<string> PdiCategories { get; set; }
        public string PDICategoriessBlobbed { get; set; }
        public byte[] signatureDealerBase64 { get; set; }
        public DateTime time { get; set; } = DateTime.Now;

    }


}
