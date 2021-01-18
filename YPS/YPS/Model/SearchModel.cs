namespace YPS.Model
{
    public class SearchModel
    {
        public class SearchPassData
        {
            public int UserID { get; set; }
            public int CompanyID { get; set; }
            public string SearchCriteria { get; set; }
            public string yShipSearchCriteria { get; set; }
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
    }
}
