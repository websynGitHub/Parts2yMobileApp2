using System.Collections.ObjectModel;
using Xamarin.Forms;
using YPS.Service;

namespace YPS.Model
{
    public class SearchModel
    {
        public class SearchPassData : IBase
        {
            public int ID { get; set; }
            public int UserID { get; set; }
            public int CompanyID { get; set; }
            public int ProjectID { get; set; }
            public int JobID { get; set; }
            public string SearchCriteria { get; set; }
            public string yShipSearchCriteria { get; set; }
            public string SearchName { get; set; }
            public bool IsCurrentSearch { get; set; }

            public string Name { get; set; }
            public int ParentID { get; set; }
            public string DisplayText { get; set; }
            public int Status { get; set; }
            public string DisplayText1 { get; set; }

            private Color _SelectedTagBorderColor = Color.Transparent;
            public Color SelectedTagBorderColor
            {
                get
                {
                    return _SelectedTagBorderColor;
                }
                set
                {
                    this._SelectedTagBorderColor = value;
                    RaisePropertyChanged("SelectedTagBorderColor");
                }
            }
        }

        public class SearchSetting
        {
            public string message { get; set; }
            public int status { get; set; }
            public SearchPassData data { get; set; }

            public SearchSetting()
            {
                data = new SearchPassData();
            }
        }

        public class SearchDataSimpleResponse
        {
            public string message { get; set; }
            public int status { get; set; }
            public string data { get; set; }
        }

        public class GetSearchFilterListResponse
        {
            public string message { get; set; }
            public int status { get; set; }
            public ObservableCollection<SearchPassData> data { get; set; }
        }

        public class GetSearchFilterSingleResponse
        {
            public string message { get; set; }
            public int status { get; set; }
            public SearchPassData data { get; set; }
        }

        public class SearchFilterDDLmaster : IBase
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public int ParentID { get; set; }
            public string DisplayText { get; set; }
            public int Status { get; set; }
            public string DisplayText1 { get; set; }

            //private Color _SelectedTagBorderColor = Color.Transparent;
            //public Color SelectedTagBorderColor
            //{
            //    get
            //    {
            //        return _SelectedTagBorderColor;
            //    }
            //    set
            //    {
            //        this._SelectedTagBorderColor = value;
            //        RaisePropertyChanged("SelectedTagBorderColor");
            //    }
            //}
        }
    }
}
