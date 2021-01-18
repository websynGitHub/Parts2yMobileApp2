using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;

namespace YPS.ViewModel
{
    public class CoachMGuidedTViewModel : IBase
    {
        public string[] listOfImages = { "Gridfeatures1.png", "Gridfeatures2.png", "showhidecolumns.png", "CoachuploadPhoto.png", "CoachUploadFile.png" };
        public string[] GuidedTImage1 = { "Gridfeatures1.png", "Gridfeatures2.png" };
        private ObservableCollection<GuidedTourData> listOfItems = new ObservableCollection<GuidedTourData>()
        {
            new GuidedTourData(){ImageTitleName = "Grid Features"},
            new GuidedTourData(){ImageTitleName = "Grid Columns Show & Hide"},
            new GuidedTourData(){ImageTitleName = "Upload Photo"},
            new GuidedTourData(){ImageTitleName = "Upload File"}
        };

        #region ICommands & data members declaration
        public ICommand ICommandSkipBtn { private set; get; }
        public ICommand ICommandBackBtn { private set; get; }
        public ICommand ICommandContinueBtn { private set; get; }
        public ICommand ICommandBackBtnGuidedT { private set; get; }
        public ICommand ICommandContinueBtnGuidedT { private set; get; }
        int imgindex, imgIndexGuidedT;
        int imgCount, imgCountGuidedT;
        YPSService service = new YPSService();// Creating new instance of the YPSService, which is used to call API
        #endregion

        /// <summary>
        /// Parameter less constructor
        /// </summary>
        public CoachMGuidedTViewModel()
        {
            if (Settings.CheckPage == "Guided Tour")
            {
                NavigationBarHideAShow = true;
                MainStackGuidedTour = true;
                GuidedTourList = listOfItems;
            }
            else
            {
                NavigationBarHideAShow = false;
                MainStackCoachMarks = true;
            }

            #region Binding methods to the respective ICommand
            ICommandSkipBtn = new Command(async () => await SkipBtn());
            ICommandBackBtn = new Command(async () => await BackBtn());
            ICommandContinueBtn = new Command(async () => await ContinueBtn());
            ICommandBackBtnGuidedT = new Command(async () => await BackBtnGuidedT());
            ICommandContinueBtnGuidedT = new Command(async () => await ContinueBtnGuidedT());
            #endregion

            imgindex = 0;
            imgCount = listOfImages.Length - 1;
            MyImageSource = listOfImages[imgindex];
        }

        /// <summary>
        /// Gets called when clicked on "Continue".
        /// </summary>
        /// <returns></returns>
        public async Task ContinueBtnGuidedT()
        {
            try
            {
                imgIndexGuidedT = 0;
                imgCountGuidedT = GuidedTImage1.Length - 1;
               
                if (ButtonTextCheckingGT == "Continue")
                {
                    if (imgIndexGuidedT < imgCountGuidedT)
                    {
                        imgIndexGuidedT++;
                        MyImageSource = ImageSource.FromFile(GuidedTImage1[imgIndexGuidedT]);
                        BackBtnGuidedTIsVisible = true;
                    }
                    if (imgIndexGuidedT == imgCountGuidedT)
                    {
                        ButtonTextCheckingGT = "Close";
                    }
                }
                else if (ButtonTextCheckingGT == "Close")
                {
                    MainStackGuidedTour = true;
                    GuidedTourGridView = false;
                    BackBtnGuidedTIsVisible = false;
                    ButtonTextCheckingGT = "Continue";
                }
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ContinueBtnGuidedT method -> in CoachMGuidedTViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on back icon. 
        /// </summary>
        /// <returns></returns>
        private async Task BackBtnGuidedT()
        {
            try
            {
                if (imgIndexGuidedT <= imgCountGuidedT)
                {
                    imgIndexGuidedT--;
                    MyImageSource = ImageSource.FromFile(GuidedTImage1[imgIndexGuidedT]);
                    ButtonTextCheckingGT = "Continue";
                    
                    if (imgIndexGuidedT == 0)
                    {
                        BackBtnGuidedTIsVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "BackBtnGuidedT method -> in CoachMGuidedTViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on skip button.
        /// </summary>
        /// <returns></returns>
        private async Task SkipBtn()
        {
            await App.Current.MainPage.Navigation.PushAsync(new Views.LoginPage());
        }

        private async Task BackBtn()
        {
            try
            {
                if (imgindex <= imgCount)
                {
                    imgindex--;
                    MyImageSource = listOfImages[imgindex];
                    ButtonTextChecking = "Continue";
                    SkipBtnIsVisible = true;
                    if (imgindex == 0)
                    {
                        BackBtnIsVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        public async Task ContinueBtn()
        {
            try
            {
                if (ButtonTextChecking == "Continue")
                {
                    if (imgindex < imgCount)
                    {
                        imgindex++;
                        MyImageSource = null;
                        MyImageSource = listOfImages[imgindex];
                        BackBtnIsVisible = true;
                    }
                    if (imgindex == imgCount)
                    {
                        //imgindex = 0;
                        ButtonTextChecking = "Done";
                        SkipBtnIsVisible = false;
                    }
                }
                else
                {
                    await App.Current.MainPage.Navigation.PushAsync(new Views.LoginPage());
                }
            }
            catch (Exception ex)
            {
            }
        }

        #region Common properties for Coach Marks and Guided Tour.

        private bool _NavigationBarHideAShow = false;
        public bool NavigationBarHideAShow
        {
            get => _NavigationBarHideAShow;
            set
            {
                _NavigationBarHideAShow = value;
                NotifyPropertyChanged();
            }
        }

        private bool _MainStackCoachMarks = false;
        public bool MainStackCoachMarks
        {
            get => _MainStackCoachMarks;
            set
            {
                _MainStackCoachMarks = value;
                NotifyPropertyChanged();
            }
        }

        private bool _MainStackGuidedTour = false;
        public bool MainStackGuidedTour
        {
            get => _MainStackGuidedTour;
            set
            {
                _MainStackGuidedTour = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource _MyImageSource;
        public ImageSource MyImageSource
        {
            get => _MyImageSource;
            set
            {
                _MyImageSource = value;
                NotifyPropertyChanged();
            }
        }
        private bool _SkipBtnIsVisible = true;
        public bool SkipBtnIsVisible
        {
            get => _SkipBtnIsVisible;
            set
            {
                _SkipBtnIsVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _BackBtnIsVisible = false;
        public bool BackBtnIsVisible
        {
            get => _BackBtnIsVisible;
            set
            {
                _BackBtnIsVisible = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ContinueBtnIsVisible = true;
        public bool ContinueBtnIsVisible
        {
            get => _ContinueBtnIsVisible;
            set
            {
                _ContinueBtnIsVisible = value;
                NotifyPropertyChanged();
            }
        }

        private string _ButtonTextChecking = "Continue";
        public string ButtonTextChecking
        {
            get => _ButtonTextChecking;
            set
            {
                _ButtonTextChecking = value;
                NotifyPropertyChanged();
            }
        }

        private string _ButtonTextCheckingGT = "Continue";
        public string ButtonTextCheckingGT
        {
            get => _ButtonTextCheckingGT;
            set
            {
                _ButtonTextCheckingGT = value;
                NotifyPropertyChanged();
            }
        }


        #endregion

        #region Properties for Coach Marks

        #endregion

        #region Properties for Guided Tour

        private bool _GuidedTourGridView = false;
        public bool GuidedTourGridView
        {
            get => _GuidedTourGridView;
            set
            {
                _GuidedTourGridView = value;
                NotifyPropertyChanged();
            }
        }
        
        private bool _BackBtnGuidedTIsVisible = false;
        public bool BackBtnGuidedTIsVisible
        {
            get => _BackBtnGuidedTIsVisible;
            set
            {
                _BackBtnGuidedTIsVisible = value;
                NotifyPropertyChanged();
            }
        }
        
        private bool _ContinueBtnGuidedTIsVisible = true;
        public bool ContinueBtnGuidedTIsVisible
        {
            get => _ContinueBtnGuidedTIsVisible;
            set
            {
                _ContinueBtnGuidedTIsVisible = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<GuidedTourData> _GuidedTourList
            = new ObservableCollection<GuidedTourData>();
        public ObservableCollection<GuidedTourData> GuidedTourList
        {
            get => _GuidedTourList;
            set
            {
                _GuidedTourList = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

    }

    public class GuidedTourData
    {
        public string ImageTitleName { get; set; }
    }
}
