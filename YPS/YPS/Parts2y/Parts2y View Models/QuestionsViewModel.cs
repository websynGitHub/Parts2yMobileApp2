using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
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
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }
        #endregion

        public QuestionsViewModel(INavigation _Navigation, QuestionsPage page, int tagId)
        {
            try
            {
                Navigation = _Navigation;
                pagename = page;
                this.tagId = tagId;
                trackService = new YPSService();
                QuestionsList = new ObservableCollection<InspectionConfiguration>();
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                QuestionClickCommand = new Command<InspectionConfiguration>(QuestionClick);

                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                PartsCmd = new Command(async () => await TabChange("parts"));
                LoadCmd = new Command(async () => await TabChange("load"));

                Task.Run(() => GetQuestionsLIst(tagId)).Wait();
                Task.Run(() => DynamicTextChange()).Wait();
            }
            catch(Exception ex)
            {

            }
        }


        private async Task TabChange(string tabname)
        {
            try
            {
                loadindicator = true;
                await Task.Delay(1);

                if (tabname == "home")
                {
                    App.Current.MainPage = new MenuPage(typeof(HomePage));
                }
                else if (tabname == "job")
                {
                    Navigation.RemovePage(Navigation.NavigationStack[3]);
                    Navigation.RemovePage(Navigation.NavigationStack[2]);

                    await Navigation.PopAsync();
                }
                else if (tabname == "parts")
                {
                    Navigation.RemovePage(Navigation.NavigationStack[3]);

                    await Navigation.PopAsync();
                    //await Navigation.PushAsync(new POChildListPage(await GetUpdatedAllPOData(), SendPodData));
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
            }
            loadindicator = false;
        }

        public async Task GetQuestionsLIst(int tagId)
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
                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    loadindicator = true;
                    var result = await trackService.GeInspectionResultsService(tagId);
                    loadindicator = false;
                    if (result != null && result.data != null)
                    {
                        inspectionResultsLists = result.data.listData;
                    }
                }
                QuestionsList?.All(x => { x.SelectedTagBorderColor = Color.Transparent; return true; });
                inspectionConfiguration.SelectedTagBorderColor = Color.DarkGreen;
                loadindicator = true;
                await Navigation.PushAsync(new AnswersPage(inspectionConfiguration, this.tagId, QuestionsList, inspectionResultsLists));
                loadindicator = false;

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Dynamic text changed
        /// </summary>
        public async Task DynamicTextChange()
        {
            try
            {
                labelobj.Load.Name = Settings.CompanySelected.Contains("(C)") == true ? "Insp" : "Load";
                labelobj.Parts.Name = Settings.CompanySelected.Contains("(C)") == true ? "VIN" : "Parts";
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method in QuestionsViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
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

        #region Properties for dynamic label change
        public class LabelAndActionsChangeClass
        {
            public LabelAndActionFields Load { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Load"
            };

            public LabelAndActionFields Parts { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Parts"
            };
        }
        public class LabelAndActionFields : IBase
        {
            public bool _Status;
            public bool Status
            {
                get => _Status;
                set
                {
                    _Status = value;
                    NotifyPropertyChanged();
                }
            }

            public string _Name;
            public string Name
            {
                get => _Name;
                set
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public LabelAndActionsChangeClass _labelobj = new LabelAndActionsChangeClass();
        public LabelAndActionsChangeClass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

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

        #endregion
    }
}


