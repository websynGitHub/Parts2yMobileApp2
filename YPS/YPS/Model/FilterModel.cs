using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace YPS.Model
{
    public class FilterModel { }
    public class DDLMasterData
    {
        public string message { get; set; }
        public int status { get; set; }
        public List<DDLmaster> data { get; set; }

        public DDLMasterData()
        {
            data = new List<DDLmaster>();
        }
    }
    public class LanguageModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string DisplayText { get; set; }
        public int Status { get; set; }
    }

    public class LanguagesModel
    {
        public string message { get; set; }
        public int status { get; set; }
        public ObservableCollection<LanguageModel> data { get; set; }
    }

    public class HeaderFilter
    {
        public List<DDLmaster> Discipline { get; set; }
        public List<DDLmaster> ELevel { get; set; }
        public List<DDLmaster> Priority { get; set; }
        public List<DDLmaster> Expeditor { get; set; }
        public List<DDLmaster> Condition { get; set; }
        public List<DDLmaster> Resource { get; set; }

        public HeaderFilter()
        {
            Discipline = new List<DDLmaster>();
            ELevel = new List<DDLmaster>();
            Priority = new List<DDLmaster>();
            Expeditor = new List<DDLmaster>();
            Condition = new List<DDLmaster>();
            Resource = new List<DDLmaster>();
        }
    }

    public class YshipHeaderFilter
    {
        public List<DDLmaster> Company { get; set; }
        public List<DDLmaster> LoadType { get; set; }
        public List<DDLmaster> ShipmentType { get; set; }
        public List<DDLmaster> ShipmentLoadedType { get; set; }
        public List<DDLmaster> ShipmentTerm { get; set; }
        public List<DDLmaster> BkgConfirmed { get; set; }

        public YshipHeaderFilter()
        {
            Company = new List<DDLmaster>();
            LoadType = new List<DDLmaster>();
            ShipmentType = new List<DDLmaster>();
            ShipmentLoadedType = new List<DDLmaster>();
            ShipmentTerm = new List<DDLmaster>();
            BkgConfirmed = new List<DDLmaster>();
        }
    }

    public class GetYshipHeaderFilter
    {
        public string message { get; set; }
        public int status { get; set; }
        public YshipHeaderFilter data { get; set; }
    }

    public class GetHeaderFilter
    {
        public string message { get; set; }
        public int status { get; set; }
        public HeaderFilter data { get; set; }
    }

    public class DDLmaster
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
        public string DisplayText { get; set; }
        public int Status { get; set; }
    }

    public class SearchData : DDLmaster
    {
        public string NationHash { get; set; }
        public string Nation { get; set; }
        public string CityHash { get; set; }
        public string City_e { get; set; }
        public string LocationHash { get; set; }
        public string Location { get; set; }
    }

    public class SearchDataRoot
    {
        public string message { get; set; }
        public int status { get; set; }
        public ObservableCollection<SearchData> data { get; set; }
    }
}
