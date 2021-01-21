using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Models.SqliteModels
{
    public class LocalVehicleDetailsModel
    {
        public class VinData
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


        }

        public class ListOfOption
        {
            public int queNo { get; set; }
            public int ObsID { get; set; }
            public string Observation { get; set; }

            public bool Isanswered { get; set; }
            public bool unSelected { get; set; } = true;
            public string vinno { get; set; }
            public DateTime createOn { get; set; } = DateTime.Now;

            public static explicit operator ListOfOption(List<ObservableCollection<ListOption>> v)
            {
                throw new NotImplementedException();
            }
        }


        public class AllVehicleDetails
        {
            public bool status { get; set; }
            public Vindata Vindata { get; set; }
            public ObservableCollection<CPQuestions> cpquestions { get; set; }
            public List<CPQuestions2> cpquestions2 { get; set; }
            public List<string> PdiCategories { get; set; }
        }

        public class ScanDetails
        {
            public bool status { get; set; }
            public Vindata Vindata { get; set; }
        }

        public class CPQuestions
        {
            public int PDI_Qno { get; set; }
            public string PDI_Qcat { get; set; }
            public string PDI_QestionTitle { get; set; }
            public int Seqno { get; set; }

            public string Remarks { get; set; }
            public string AnsweredRemarks { get; set; }
            public string RightSwipeText { get; set; }
            public int RightSwipeSelectedIndex { get; set; }
            public string cpquestions { get; set; }
            public string AnsweredQstnBgColor { get; set; }
            public string NotAnsweredBgColor { get; set; }


            // public ObservableCollection<ListOfOption> _listOptions { get; set; }

            public bool isVisible = false;
            public string vinno { get; set; }
            public DateTime createdOn { get; set; } = DateTime.Now;
        }

        public class CPQuestions2
        {
            public int PDI_Qno { get; set; }
            public string PDI_Qcat { get; set; }
            public string PDI_QestionTitle { get; set; }
            public int Seqno { get; set; }

            public string Remarks { get; set; }
            public string AnsweredRemarks { get; set; }
            public string RightSwipeText { get; set; }
            public int RightSwipeSelectedIndex { get; set; }
            public string cpquestions { get; set; }
            public string AnsweredQstnBgColor { get; set; }
            public string NotAnsweredBgColor { get; set; }
            public bool isVisible = false;
            public string vinno { get; set; }
            public DateTime createdOn { get; set; } = DateTime.Now;
            [TextBlob("blobtextlistofoptions")]
            public ObservableCollection<ListOfOption> listOptions { get; set; }
            public string blobtextlistofoptions { get; set; }
        }
        public class CPQuestionssList
        {
            public List<CPQuestions> CPQuestionsdata { get; set; }
            public List<ListOfOption> optionList { get; set; }
            public object PDICategories { get; set; }
        }

        public class CPQuestionssList2
        {
            public List<CPQuestions2> CPQuestionsdata { get; set; }
            public object PDICategories { get; set; }
        }
        public class QuestionariesList
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
            public object Remarks { get; set; }
            public string ScanOn { get; set; }
            public string Modelname { get; set; }
        }




        public class DealerVinDetails
        {
            public bool status { get; set; }
            public DealerDetailsData Dealerdata { get; set; }
            public ObservableCollection<CPQuestionsdata> VinCheckList { get; set; }
            public List<string> PdiCategories { get; set; }
        }


    }
}
