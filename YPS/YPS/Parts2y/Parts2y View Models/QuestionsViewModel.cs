using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class QuestionsViewModel : INotifyPropertyChanged
    {

        #region IComman and data members declaration

        public INavigation Navigation { get; set; }

        public ICommand Backevnttapped { set; get; }

        public ICommand QuestionClickCommand { get; set; }

        private bool _RefreshDisable = true;
        public bool RefreshDisable
        {
            get { return _RefreshDisable; }
            set
            {
                _RefreshDisable = value;
                RaisePropertyChanged("RefreshDisable");
            }
        }

        QuestionsPage pagename;
        YPSService trackService;
        int tagId;
        List<InspectionResultsList> inspectionResultsLists;

        #endregion

        public QuestionsViewModel(INavigation _Navigation, QuestionsPage page, int tagId, string tagNumber, string indentCode, string bagNumber)
        {
            Navigation = _Navigation;
            pagename = page;
            this.tagId = tagId;
            Task.Run(() => ChangeLabel()).Wait();
            TagNumber = tagNumber;
            IndentCode = indentCode;
            TripNumber = bagNumber;
            trackService = new YPSService();
            QuestionsList = new ObservableCollection<InspectionConfiguration>();
            Backevnttapped = new Command(async () => await Backevnttapped_click());
            QuestionClickCommand = new Command<InspectionConfiguration>(QuestionClick);
            GetQuestionsLIst();

        }

        public async void ChangeLabel()
        {
            try
            {
                labelobj = new DashboardLabelChangeClass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var tagnumber = labelval.Where(wr => wr.FieldID == labelobj.TagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID == labelobj.IdentCode.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bagnumber = labelval.Where(wr => wr.FieldID == labelobj.BagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID == labelobj.ConditionName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();


                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber == null ? true : (tagnumber.Status == 1 ? true : false);
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode == null ? true : (identcode.Status == 1 ? true : false);
                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                        labelobj.BagNumber.Status = bagnumber == null ? true : (bagnumber.Status == 1 ? true : false);
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname == null ? true : (conditionname.Status == 1 ? true : false);
                        labelobj.Load.Name = Settings.CompanySelected.Contains("(C)") == true ? "Insp" : "Load";
                        labelobj.Parts.Name = Settings.CompanySelected.Contains("(C)") == true ? "VIN" : "Parts";
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                //YPSLogger.ReportException(ex, "ChangeLabelAndShowHide method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        public async void GetConfigurationResults()
        {
            try
            {
                QuestionsList?.All(x => { x.SelectedTagBorderColor = Color.Transparent; return true; });
                QuestionsList?.All(x => { x.Status = 0; return true; });
                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    loadindicator = true;
                    var result = await trackService.GeInspectionResultsService(tagId);
                    loadindicator = false;
                    if (result != null && result.data != null && result.data.listData != null)
                    {
                        inspectionResultsLists = result.data.listData;
                        QuestionsList?.Where(x => inspectionResultsLists.Any(z => z.QID == x.MInspectionConfigID)).Select(x => { x.Status = 1; return x; }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        public void GetQuestionsLIst()
        {
            QuestionsList = new ObservableCollection<InspectionConfiguration>(Settings.allInspectionConfigurations);
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
            }
        }

        public async void QuestionClick(InspectionConfiguration inspectionConfiguration)
        {
            try
            {
                inspectionConfiguration.SelectedTagBorderColor = Color.DarkGreen;
                loadindicator = true;
                await Navigation.PushAsync(new AnswersPage(inspectionConfiguration, this.tagId, QuestionsList, inspectionResultsLists,TagNumber,IndentCode, TripNumber));
                loadindicator = false;

            }
            catch (Exception ex)
            {
            }
        }

        #region INotifyPropertyChanged Implimentation
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Properties

        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                RaisePropertyChanged("BgColor");
            }
        }

        private ObservableCollection<InspectionConfiguration> _QuestionsList;
        public ObservableCollection<InspectionConfiguration> QuestionsList
        {
            get => _QuestionsList;
            set
            {
                _QuestionsList = value;
            }
        }

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator; set
            {
                _loadindicator = value;
                RaisePropertyChanged("loadindicator");
            }
        }

        private string _TagNumber;
        public string TagNumber
        {
            get => _TagNumber;
            set
            {
                _TagNumber = value;
                RaisePropertyChanged("TagNumber");
            }
        }

        private string _IndentCode;
        public string IndentCode
        {
            get => _IndentCode; set
            {
                _IndentCode = value;
                RaisePropertyChanged("IndentCode");
            }
        }

        private string _TripNumber;
        public string TripNumber
        {
            get => _TripNumber; set
            {
                _TripNumber = value;
                RaisePropertyChanged("TripNumber");
            }
        }

        #endregion

        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields TagNumber { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TagNumber"
            };
            public DashboardLabelFields IdentCode { get; set; } = new DashboardLabelFields { Status = true, Name = "IdentCode" };
            public DashboardLabelFields BagNumber { get; set; } = new DashboardLabelFields { Status = true, Name = "BagNumber" };
            public DashboardLabelFields ConditionName { get; set; } = new DashboardLabelFields { Status = true, Name = "ConditionName" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "Parts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "Load" };

        }
        public class DashboardLabelFields : IBase
        {
            public bool Status { get; set; }
            public string Name { get; set; }
        }

        public DashboardLabelChangeClass _labelobj = new DashboardLabelChangeClass();
        public DashboardLabelChangeClass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}


