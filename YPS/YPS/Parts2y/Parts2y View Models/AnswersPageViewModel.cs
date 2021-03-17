using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class AnswersPageViewModel : INotifyPropertyChanged
    {
        #region IComman and data members declaration

        public INavigation Navigation { get; set; }

        public ICommand ViewallClick { get; set; }

        public ICommand NextClick { get; set; }

        public ICommand PhotoClickCommand { get; set; }


        public ICommand Backevnttapped { set; get; }
        public QuestiionsPageHeaderData QuestiionsPageHeaderData { get; set; }
        AnswersPage pagename;
        YPSService trackService;
        int tagId;
        int photoCounts;
        Stream picStream;
        string extension = "", Mediafile, fileName;
        ObservableCollection<InspectionConfiguration> inspectionConfigurationList;
        ObservableCollection<InspectionPhotosResponseListData> finalPhotoListA;
        List<InspectionResultsList> inspectionResultsList;


        #endregion

        public AnswersPageViewModel(INavigation _Navigation, AnswersPage page, InspectionConfiguration inspectionConfiguration, int tagId, ObservableCollection<InspectionConfiguration> inspectionConfigurationList, List<InspectionResultsList> inspectionResultsList, string tagNumber, string indentCode, string bagNumber, QuestiionsPageHeaderData questiionsPageHeaderData)
        {
            Backevnttapped = new Command(async () => await Backevnttapped_click());
            Navigation = _Navigation;
            pagename = page;
            this.tagId = tagId;
            this.QuestiionsPageHeaderData = questiionsPageHeaderData;
            Task.Run(() => ChangeLabel()).Wait();
            TagNumber = tagNumber;
            IndentCode = indentCode;
            TripNumber = bagNumber;
            InspectionConfiguration = inspectionConfiguration;
            this.inspectionConfigurationList = inspectionConfigurationList;
            this.inspectionResultsList = inspectionResultsList;
            trackService = new YPSService();
            ShowConfigurationOptions();
            ViewallClick = new Command(ViewallClickMethod);
            NextClick = new Command(NextClickMethod);
            PhotoClickCommand = new Command(async () => await SelectPic());
            finalPhotoListA = new ObservableCollection<InspectionPhotosResponseListData>();

        }

        public async Task GetInspectionPhotos()
        {
            loadindicator = true;
            var checkInternet = await App.CheckInterNetConnection();
            if (checkInternet)
            {
                var result = await trackService.GeInspectionPhotosService(tagId, InspectionConfiguration?.MInspectionConfigID);
                if (result != null && result.data != null && result.data.listData != null && result.data.listData.Count > 0)
                {
                    ImagesCount = result.data.listData.Count;
                }
            }

            loadindicator = false;
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

                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                //YPSLogger.ReportException(ex, "ChangeLabelAndShowHide method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on the Camera icon for uploading photo.
        /// </summary>
        /// <returns></returns>
        public async Task SelectPic()
        {
            loadindicator = true;
            await Navigation.PushAsync(new InspectionPhotosPage(this.tagId, InspectionConfiguration, QuestiionsPageHeaderData.VINLabelValue));
            loadindicator = false;
        }

        private async void NextClickMethod()
        {
            if (NextButtonText == "COMPLETE")
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
                            AnswersGridVisibility = false;
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

            AnswersGridVisibility = true;
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

        private bool _AnswersGridVisibility = true;
        public bool AnswersGridVisibility
        {
            get => _AnswersGridVisibility;
            set
            {
                _AnswersGridVisibility = value;
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

        private int _ImagesCount;
        public int ImagesCount
        {
            get => _ImagesCount;
            set
            {
                _ImagesCount = value;
                RaisePropertyChanged("ImagesCount");
            }
        }

        private ImageSource _ImageViewForUpload;
        public ImageSource ImageViewForUpload
        {
            get { return _ImageViewForUpload; }
            set
            {
                _ImageViewForUpload = value;
                NotifyPropertyChanged();
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