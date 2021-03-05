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
    public class AnswersPageViewModel : INotifyPropertyChanged
    {
        #region IComman and data members declaration

        public INavigation Navigation { get; set; }

        public ICommand ViewallClick { get; set; }

        public ICommand NextClick { get; set; }


        public ICommand Backevnttapped { set; get; }
        AnswersPage pagename;
        YPSService trackService;
        int tagId;
        ObservableCollection<InspectionConfiguration> inspectionConfigurationList;
        List<InspectionResultsList> inspectionResultsList;


        #endregion

        public AnswersPageViewModel(INavigation _Navigation, AnswersPage page, InspectionConfiguration inspectionConfiguration, int tagId, ObservableCollection<InspectionConfiguration> inspectionConfigurationList, List<InspectionResultsList> inspectionResultsList)
        {
            Backevnttapped = new Command(async () => await Backevnttapped_click());
            Navigation = _Navigation;
            pagename = page;
            this.tagId = tagId;
            InspectionConfiguration = inspectionConfiguration;
            this.inspectionConfigurationList = inspectionConfigurationList;
            this.inspectionResultsList = inspectionResultsList;
            trackService = new YPSService();
            ShowConfigurationOptions();
            ViewallClick = new Command(ViewallClickMethod);
            NextClick = new Command(NextClickMethod);

        }

        private async void NextClickMethod()
        {
            if(NextButtonText== "COMPLETE")
            {
                await Navigation.PopAsync();
            }


            if (FrontRightTrue || FrontRightFalse || FrontLeftTrue || FrontLeftFalse || RearLeftTrue || RearLeftFalse || RearRightTrue || RearRightFalse || PlaneTrue || PlaneFalse)
            {
                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    UpdateInspectionRequest updateInspectionRequest = new UpdateInspectionRequest();
                    updateInspectionRequest.BackLeft = RearLeftTrue ? 1 : 0;
                    updateInspectionRequest.QID = InspectionConfiguration.MInspectionConfigID;
                    updateInspectionRequest.BackRight = RearRightTrue ? 1 : 0;
                    updateInspectionRequest.Direct = PlaneTrue ? 1 : 0;
                    updateInspectionRequest.FrontLeft = FrontLeftTrue ? 1 : 0;
                    updateInspectionRequest.FrontRight = FrontRightTrue ? 1 : 0;
                    updateInspectionRequest.POTagID = tagId;
                    updateInspectionRequest.Remarks = Remarks;
                    updateInspectionRequest.UserID = Settings.userLoginID;
                    loadindicator = true;
                    var result = await trackService.InsertUpdateInspectionResult(updateInspectionRequest);
                    loadindicator = false;
                    if (result != null && result.status == 1)
                    {
                        if (InspectionConfiguration.MInspectionConfigID == inspectionConfigurationList.Count)
                        {
                            NextButtonText = "COMPLETE";
                        }

                        if (InspectionConfiguration.MInspectionConfigID < inspectionConfigurationList.Count)
                        {
                            FrontRightTrue = false;
                            FrontRightFalse = false;
                            FrontLeftTrue = false;
                            FrontLeftFalse = false;
                            RearLeftTrue = false;
                            RearLeftFalse = false;
                            RearRightTrue = false;
                            RearRightFalse = false;
                            PlaneTrue = false;
                            PlaneFalse = false;
                            InspectionConfiguration = inspectionConfigurationList.FirstOrDefault(x => x.MInspectionConfigID == InspectionConfiguration.MInspectionConfigID + 1);
                            Remarks = string.Empty;
                            ShowConfigurationOptions();
                        }

                        
                    }
                }
            }
            else
            {
                if (InspectionConfiguration.MInspectionConfigID < inspectionConfigurationList.Count)
                {
                    InspectionConfiguration = inspectionConfigurationList.FirstOrDefault(x => x.MInspectionConfigID == InspectionConfiguration.MInspectionConfigID + 1);
                    ShowConfigurationOptions();
                    Remarks = string.Empty;
                }
            }
        }
        private async void ViewallClickMethod()
        {
            await Navigation.PopAsync();
        }

        private async Task Backevnttapped_click()
        {
            try
            {
                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
            }
        }

        private void ShowConfigurationOptions()
        {
            FrontLeft = InspectionConfiguration.FrontLeft == 1 || InspectionConfiguration.IsFront == 1;
            FrontRight = InspectionConfiguration.FrontRight == 1;
            RearLeft = InspectionConfiguration.BackLeft == 1 || InspectionConfiguration.IsBack == 1;
            RearRight = InspectionConfiguration.BackRight == 1;
            RearLabel = InspectionConfiguration.BackRight == 1 || InspectionConfiguration.BackLeft == 1 || InspectionConfiguration.IsFront == 1;
            FrontLabel = InspectionConfiguration.FrontLeft == 1 || InspectionConfiguration.FrontRight == 1 || InspectionConfiguration.IsFront == 1;
            PlaneOptions = InspectionConfiguration.FrontLeft == 0 &&
                          InspectionConfiguration.FrontRight == 0 &&
                         InspectionConfiguration.BackLeft == 0 &&
                        InspectionConfiguration.BackRight == 0 &&
                        InspectionConfiguration.IsBack == 0 &&
                        InspectionConfiguration.IsFront == 0;
            LeftLabel = InspectionConfiguration.FrontLeft == 1 || InspectionConfiguration.BackLeft == 1;
            RightLabel = InspectionConfiguration.FrontRight == 1 || InspectionConfiguration.BackRight == 1;
            var answer = inspectionResultsList?.Find(x => x.QID == InspectionConfiguration.MInspectionConfigID);
            if (answer != null)
            {
                Remarks = answer.Remarks;
                if (answer.FrontLeft == 1)
                {
                    FrontLeftTrue = true;
                    FrontLeftFalse = false;
                }

                if (FrontLeft && answer.FrontLeft == 0)
                {
                    FrontLeftTrue = false;
                    FrontLeftFalse = true;
                }

                if (answer.FrontRight == 1)
                {
                    FrontRightTrue = true;
                    FrontRightFalse = false;
                }

                if (FrontRight && answer.FrontRight == 0)
                {
                    FrontRightTrue = false;
                    FrontRightFalse = true;
                }

                if (answer.BackLeft == 1)
                {
                    RearLeftTrue = true;
                    RearLeftFalse = false;
                }

                if (RearLeft && answer.BackLeft == 0)
                {
                    RearLeftTrue = false;
                    RearLeftFalse = true;
                }

                if (answer.BackRight == 1)
                {
                    RearRightTrue = true;
                    RearRightFalse = false;
                }

                if (RearRight && answer.BackRight == 0)
                {
                    RearRightTrue = false;
                    RearRightFalse = true;
                }

                if (answer.Direct == 1)
                {
                    PlaneTrue = true;
                    PlaneFalse = false;
                }

                if (PlaneOptions && answer.Direct == 0)
                {
                    PlaneTrue = false;
                    PlaneFalse = true;
                }
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



        private string _NextButtonText = "NEXT";
        public string NextButtonText
        {
            get => _NextButtonText;
            set
            {
                _NextButtonText = value;
                RaisePropertyChanged("NextButtonText");
            }
        }

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

        private InspectionConfiguration _InspectionConfiguration;
        public InspectionConfiguration InspectionConfiguration
        {
            get => _InspectionConfiguration;
            set
            {
                _InspectionConfiguration = value;
                RaisePropertyChanged("InspectionConfiguration");
            }
        }

        private bool _RearLeft;
        public bool RearLeft
        {
            get => _RearLeft;
            set
            {
                _RearLeft = value;
                RaisePropertyChanged("RearLeft");
            }
        }

        private bool _RearRight;
        public bool RearRight
        {
            get => _RearRight;
            set
            {
                _RearRight = value;
                RaisePropertyChanged("RearRight");
            }
        }

        private bool _PlaneOptions;
        public bool PlaneOptions
        {
            get => _PlaneOptions;
            set
            {
                _PlaneOptions = value;
                RaisePropertyChanged("PlaneOptions");
            }
        }
        private bool _FrontRight;
        public bool FrontRight
        {
            get => _FrontRight;
            set
            {
                _FrontRight = value;
                RaisePropertyChanged("FrontRight");
            }
        }

        private bool _FrontLeft;
        public bool FrontLeft
        {
            get => _FrontLeft;
            set
            {
                _FrontLeft = value;
                RaisePropertyChanged("FrontLeft");
            }
        }

        private bool _FrontLabel;
        public bool FrontLabel
        {
            get => _FrontLabel;
            set
            {
                _FrontLabel = value;
                RaisePropertyChanged("FrontLabel");
            }
        }

        private bool _RearLabel;
        public bool RearLabel
        {
            get => _RearLabel;
            set
            {
                _RearLabel = value;
                RaisePropertyChanged("RearLabel");
            }
        }

        private bool _LeftLabel;
        public bool LeftLabel
        {
            get => _LeftLabel;
            set
            {
                _LeftLabel = value;
                RaisePropertyChanged("LeftLabel");
            }
        }

        private bool _RightLabel;
        public bool RightLabel
        {
            get => _RightLabel;
            set
            {
                _RightLabel = value;
                RaisePropertyChanged("RightLabel");
            }
        }

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator;
            set
            {
                _loadindicator = value;
                RaisePropertyChanged("loadindicator");
            }
        }

        private string _Remarks;
        public string Remarks
        {
            get => _Remarks;
            set
            {
                _Remarks = value;
                RaisePropertyChanged("Remarks");
            }
        }

        private bool _FrontLeftTrue;
        public bool FrontLeftTrue
        {
            get => _FrontLeftTrue;
            set
            {
                _FrontLeftTrue = value;
                RaisePropertyChanged("FrontLeftTrue");
            }
        }

        private bool _FrontLeftFalse;
        public bool FrontLeftFalse
        {
            get => _FrontLeftFalse;
            set
            {
                _FrontLeftFalse = value;
                RaisePropertyChanged("FrontLeftFalse");
            }
        }

        private bool _FrontRightTrue;
        public bool FrontRightTrue
        {
            get => _FrontRightTrue;
            set
            {
                _FrontRightTrue = value;
                RaisePropertyChanged("FrontRightTrue");
            }
        }

        private bool _FrontRightFalse;
        public bool FrontRightFalse
        {
            get => _FrontRightFalse;
            set
            {
                _FrontRightFalse = value;
                RaisePropertyChanged("FrontRightFalse");
            }
        }

        private bool _RearLeftTrue;
        public bool RearLeftTrue
        {
            get => _RearLeftTrue;
            set
            {
                _RearLeftTrue = value;
                RaisePropertyChanged("RearLeftTrue");
            }
        }

        private bool _RearLeftFalse;
        public bool RearLeftFalse
        {
            get => _RearLeftFalse;
            set
            {
                _RearLeftFalse = value;
                RaisePropertyChanged("RearLeftFalse");
            }
        }

        private bool _RearRightTrue;
        public bool RearRightTrue
        {
            get => _RearRightTrue;
            set
            {
                _RearRightTrue = value;
                RaisePropertyChanged("RearRightTrue");
            }
        }

        private bool _RearRightFalse;
        public bool RearRightFalse
        {
            get => _RearRightFalse;
            set
            {
                _RearRightFalse = value;
                RaisePropertyChanged("RearRightFalse");
            }
        }

        private bool _PlaneTrue;
        public bool PlaneTrue
        {
            get => _PlaneTrue;
            set
            {
                _PlaneTrue = value;
                RaisePropertyChanged("PlaneTrue");
            }
        }

        private bool _PlaneFalse;
        public bool PlaneFalse
        {
            get => _PlaneFalse;
            set
            {
                _PlaneFalse = value;
                RaisePropertyChanged("PlaneFalse");
            }
        }

        #endregion
    }
}