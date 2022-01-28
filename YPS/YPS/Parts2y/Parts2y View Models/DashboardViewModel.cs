using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.Helpers;
using YPS.Parts2y.Parts2y_Views;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Model;
using YPS.Service;
using System.Linq;
using Acr.UserDialogs;
using ZXing.Net.Mobile.Forms;
using System.Collections.ObjectModel;

namespace YPS.Parts2y.Parts2y_View_Models
{
    class DashboardViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        public Command TaskClickCmd { get; set; }
        public Command ScanClickCmd { get; set; }
        public Command CompareClickCmd { get; set; }
        public Command PhotoClickCmd { get; set; }
        public Command CompareContinuousClickCmd { get; set; }
        public Command PolyBoxClickCmd { get; set; }
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command TaskCmd { get; set; }
        public Command LoadCmd { set; get; }


        YPSService trackService;
        HomePage homePage;

        public DashboardViewModel(INavigation _Navigation, HomePage homepage)
        {
            try
            {
                loadindicator = true;
                Navigation = _Navigation;
                homePage = homepage;
                trackService = new YPSService();
                TaskClickCmd = new Command(async () => await RedirectToPage("task"));
                CompareClickCmd = new Command(async () => await RedirectToPage("compare"));
                ScanClickCmd = new Command(async () => await RedirectToPage("scan"));
                PhotoClickCmd = new Command(async () => await RedirectToPage("photo"));
                CompareContinuousClickCmd = new Command(async () => await RedirectToPage("comparecontinuous"));
                PolyBoxClickCmd = new Command(async () => await RedirectToPage("polybox"));

                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                TaskCmd = new Command(async () => await TabChange("task"));
                LoadCmd = new Command(async () => await TabChange("load"));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DashboardViewModel constructor -> in DashboardViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        /// <summary>
        /// This method is for binding the records/date to the data grid.
        /// </summary>
        /// </summary>
        /// <param name="iSPagingNoAuto"></param>
        /// <returns></returns>
        public async void GetTaskData()
        {
            try
            {
                loadindicator = true;

                YPSLogger.TrackEvent("DashboardViewModel.cs", " in GetTaskData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    GetPoData result = new GetPoData();
                    SendPodata sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;

                    result = await trackService.LoadPoDataService(sendPodata);

                    if (result?.status == 1 && result?.data?.allPoDataMobile != null)
                    {
                        Settings.AllPOData = new ObservableCollection<AllPoData>();
                        Settings.AllPOData = result.data.allPoDataMobile;

                        var groubbyval = result.data.allPoDataMobile.GroupBy(gb => new { gb.TaskID });

                        ObservableCollection<AllPoData> groupedlist = new ObservableCollection<AllPoData>();

                        foreach (var val in groubbyval)
                        {
                            AllPoData groupdata = new AllPoData();
                            groupdata.TaskStatus = val.Select(s => s.TaskStatus).FirstOrDefault();
                            groupdata.TaskResourceID = val.Select(s => s.TaskResourceID).FirstOrDefault();
                            //IEnumerable<int> poidlist = val.Select(s => s.POID).Distinct();
                            //groupdata.PONumberForDisplay = poidlist.Count() == 1 ? groupdata.PONumber : "Multiple";
                            //groupdata.ShippingNumberForDisplay = poidlist.Count() == 1 ? groupdata.ShippingNumber : "Multiple";
                            //groupdata.REQNoForDisplay = poidlist.Count() == 1 ? groupdata.REQNo : "Multiple";
                            groupedlist.Add(groupdata);
                        }

                        groupedlist = new ObservableCollection<AllPoData>(groupedlist?.Where(wr => wr.TaskResourceID == Settings.userLoginID && wr.TaskStatus == 0));

                        Settings.notifyJobCount = groupedlist?.Count() > 0 ? groupedlist?.Count() : 0;
                        JobCountText = Settings.notifyJobCount > 0 ? Settings.notifyJobCount.ToString() : null;
                    }
                    else
                    {
                        JobCountText = null;
                        Settings.notifyJobCount = 0;
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }

            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetTaskData method -> in DashboardViewModel.cs " + Settings.userLoginID);
                loadindicator = false;
            }
            finally
            {
                loadindicator = false;
            }
        }

        private async Task TabChange(string tabname)
        {
            try
            {
                loadindicator = true;

                if (tabname == "job")
                {
                    await Navigation.PushAsync(new ParentListPage(), false);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TabChange method -> in DashboardViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
            loadindicator = false;
        }

        /// <summary>
        /// This method is to remember user details.
        /// </summary>
        /// <returns></returns>
        public async void RememberUserDetails()
        {
            try
            {
                loadindicator = true;

                RememberPwdDB Db = new RememberPwdDB();
                var user = Db.GetUserDetails();

                if (user.Count == 0)
                {
                    RememberPwd saveData = new RememberPwd();
                    saveData.encUserId = EncryptManager.Encrypt(Convert.ToString(Settings.userLoginID));
                    saveData.encLoginID = Settings.LoginID;
                    saveData.encUserRollID = EncryptManager.Encrypt(Convert.ToString(Settings.userRoleID));
                    saveData.encSessiontoken = Settings.Sessiontoken;
                    saveData.AndroidVersion = Settings.AndroidVersion;
                    saveData.iOSversion = Settings.iOSversion;
                    saveData.IIJEnable = Settings.IsIIJEnabled;
                    saveData.IsPNEnabled = Settings.IsPNEnabled;
                    saveData.IsEmailEnabled = Settings.IsEmailEnabled;
                    saveData.BgColor = Settings.Bar_Background.ToHex();
                    saveData.RoleColorCode = Settings.RoleColorCode;
                    saveData.ScanditKey = HostingURL.scandItLicencekey;
                    saveData.IsMobileCompareCont = Settings.IsMobileCompareCont;
                    Db.SaveUserPWd(saveData);
                }

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var DBresponse = await trackService.GetSaveUserDefaultSettings(Settings.userLoginID);

                    if (DBresponse != null)
                    {
                        if (DBresponse.status == 1)
                        {
                            Settings.VersionID = DBresponse.data.VersionID;
                            Company = Settings.CompanySelected = DBresponse.data.CompanyName;


                            ProNjobName = DBresponse.data.ProjectName + "/" + DBresponse.data.JobNumber;
                            ProjectName = Settings.ProjectSelected = DBresponse.data.ProjectName;
                            JobName = Settings.JobSelected = DBresponse.data.JobNumber;
                            Settings.CompanyID = DBresponse.data.CompanyID;
                            Settings.ProjectID = DBresponse.data.ProjectID;
                            Settings.JobID = DBresponse.data.JobID;
                        }
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RememberUserDetails method -> in DashboardViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task RedirectToPage(string page)
        {
            try
            {
                loadindicator = true;

                if (page.Trim().ToLower() == "task".Trim().ToLower())
                {
                    await Navigation.PushAsync(new ParentListPage(), false);
                }
                else if (page.Trim().ToLower() == "compare".Trim().ToLower())
                {
                    await Navigation.PushAsync(new Compare(), false);
                }
                else if (page.Trim().ToLower() == "scan".Trim().ToLower())
                {
                    await Navigation.PushAsync(new ScanPage(0, null, false, null), false);
                }
                else if (page.Trim().ToLower() == "photo".Trim().ToLower())
                {
                    await Navigation.PushAsync(new PhotoRepoPage(), false);
                }
                else if(page.Trim().ToLower()== "comparecontinuous".Trim().ToLower())
                {
                    await Navigation.PushAsync(new CompareContinuous(), false);
                }
                else
                {
                    await Navigation.PushAsync(new PolyBox(), false);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RedirectToPage method -> in DashboardViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
            loadindicator = false;
        }

        /// <summary>
        /// This method is for getting PN count.
        /// </summary>
        public async void GetPNCount()
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in GetPNCount method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var result = await trackService.GetNotifyHistory(Settings.userLoginID);

                    if (result?.status == 1 && result?.data?.Count() > 0)
                    {
                        NotificationListCount = result.data.Count();
                        NotifyCountTxt = result.data[0].listCount > 0 ? result.data[0].listCount.ToString() : null;
                        Settings.notifyCount = result.data[0].listCount;
                    }
                    else
                    {
                        NotificationListCount = 0;
                        NotifyCountTxt = null;
                        Settings.notifyCount = 0;
                    }
                    Task.Run(homePage.MoveBell);
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetPNCount method -> in DashboardViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method get all label texts, used in the app.
        /// </summary>
        public async void GetallApplabels()
        {
            try
            {
                loadindicator = true;

                //if (Settings.alllabeslvalues == null || Settings.alllabeslvalues.Count == 0)
                //{
                var lblResult = await trackService.GetallApplabelsService();

                if (lblResult != null && lblResult.data != null)
                {
                    Settings.alllabeslvalues = lblResult.data.ToList();
                    var datavalues = Settings.alllabeslvalues.Where(x => x.VersionID == Settings.VersionID && x.LanguageID == Settings.LanguageID).ToList();
                    Settings.Companylabel = datavalues.Where(x => x.FieldID == Settings.Companylabel1).Select(m => m.LblText).FirstOrDefault();
                    Settings.projectlabel = datavalues.Where(x => x.FieldID == Settings.projectlabel1).Select(x => x.LblText).FirstOrDefault();
                    Settings.joblabel = datavalues.Where(x => x.FieldID == Settings.joblabel1).Select(x => x.LblText).FirstOrDefault();
                }
                //}
                //else
                //{
                //    if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                //    {
                //        var datavalues = Settings.alllabeslvalues.Where(x => x.VersionID == Settings.VersionID && x.LanguageID == Settings.LanguageID).ToList();

                //        Settings.Companylabel = datavalues.Where(x => x.FieldID == Settings.Companylabel1).Select(m => m.LblText).FirstOrDefault();
                //        Settings.projectlabel = datavalues.Where(x => x.FieldID == Settings.projectlabel1).Select(x => x.LblText).FirstOrDefault();
                //        Settings.joblabel = datavalues.Where(x => x.FieldID == Settings.joblabel1).Select(x => x.LblText).FirstOrDefault();
                //    }
                //}
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetallApplabels method -> in DashboardViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        /// <summary>
        /// This method is to get the status of actions present in application.
        /// </summary>
        public async void GetActionStatus()
        {
            try
            {
                loadindicator = true;

                var lblResult = await trackService.GetallActionStatusService(Settings.userLoginID);

                if (lblResult != null && lblResult.data != null)
                {
                    Settings.AllActionStatus = lblResult.data.ToList();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetActionStatus method -> in DashboardViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        /// <summary>
        /// This is for changing the labels dynamically
        /// </summary>
        public async void ChangeLabel()
        {
            try
            {
                loadindicator = true;

                labelobj = new DashboardLabelChangeClass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        //Getting Label values & Status based on FieldID
                        var company = labelval.Where(wr => wr.FieldID == labelobj.Company.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var project = labelval.Where(wr => wr.FieldID == labelobj.Project.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var job = labelval.Where(wr => wr.FieldID == labelobj.Job.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var home = labelval.Where(wr => wr.FieldID == labelobj.Home.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var jobs = labelval.Where(wr => wr.FieldID == labelobj.Jobs.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var parts = labelval.Where(wr => wr.FieldID == labelobj.Parts.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var load = labelval.Where(wr => wr.FieldID == labelobj.Load.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var scan = labelval.Where(wr => wr.FieldID == labelobj.Scan.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var compare = labelval.Where(wr => wr.FieldID == labelobj.Compare.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var comparecont = labelval.Where(wr => wr.FieldID == labelobj.CompareCont.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var photo = labelval.Where(wr => wr.FieldID == labelobj.Photo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();


                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.Company.Name = (company != null ? (!string.IsNullOrEmpty(company.LblText) ? company.LblText : labelobj.Company.Name) : labelobj.Company.Name) + " :";
                        labelobj.Company.Status = company?.Status == 1 || company?.Status == 2 ? true : false;
                        labelobj.Project.Name = (project != null ? (!string.IsNullOrEmpty(project.LblText) ? project.LblText : labelobj.Project.Name) : labelobj.Project.Name) + " :";
                        labelobj.Project.Status = project?.Status == 1 || project?.Status == 2 ? true : false;
                        labelobj.Job.Name = (job != null ? (!string.IsNullOrEmpty(job.LblText) ? job.LblText : labelobj.Job.Name) : labelobj.Job.Name) + " :";
                        labelobj.Job.Status = job?.Status == 1 || job?.Status == 2 ? true : false;

                        labelobj.Home.Name = (home != null ? (!string.IsNullOrEmpty(home.LblText) ? home.LblText : labelobj.Home.Name) : labelobj.Home.Name);
                        //labelobj.Home.Status = home?.Status == 1 || home.Status == 2 ? true : false;
                        //labelobj.Parts.Status = parts?.Status == 1 || parts.Status == 2 ? true : false;
                        //labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                        labelobj.Jobs.Name = jobs != null ? (!string.IsNullOrEmpty(jobs.LblText) ? jobs.LblText : labelobj.Jobs.Name) : labelobj.Jobs.Name;
                        //labelobj.Jobs.Status = jobs?.Status == 1 || jobs?.Status == 2 ? true : false;
                        labelobj.Parts.Name = parts != null ? (!string.IsNullOrEmpty(parts.LblText) ? parts.LblText : labelobj.Parts.Name) : labelobj.Parts.Name;
                        //labelobj.Parts.Status = parts?.Status == 1 || parts.Status == 2 ? true : false;
                        labelobj.Load.Name = load != null ? (!string.IsNullOrEmpty(load.LblText) ? load.LblText : labelobj.Load.Name) : labelobj.Load.Name;
                        //labelobj.Load.Status = load?.Status == 1 || load?.Status == 2 ? true : false;
                        labelobj.Scan.Name = scan != null ? (!string.IsNullOrEmpty(scan.LblText) ? scan.LblText : labelobj.Scan.Name) : labelobj.Scan.Name;
                        //labelobj.Scan.Status = scan?.Status == 1 || scan?.Status == 2 ? true : false;
                        labelobj.Compare.Name = compare != null ? (!string.IsNullOrEmpty(compare.LblText) ? compare.LblText : labelobj.Compare.Name) : labelobj.Compare.Name;
                        //labelobj.Compare.Status = compare?.Status == 1 || compare?.Status == 2 ? true : false;
                        labelobj.Photo.Name = photo != null ? (!string.IsNullOrEmpty(photo.LblText) ? photo.LblText : labelobj.Photo.Name) : labelobj.Photo.Name;
                        //labelobj.Photo.Status = photo?.Status == 1 || photo?.Status == 2 ? true : false;

                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    if (Settings.VersionID == 1)
                    {
                        IsLoadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "ELoadInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;

                    }
                    else if (Settings.VersionID == 2)
                    {
                        IsLoadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "CCarrierInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;

                    }
                    else if (Settings.VersionID == 3)
                    {
                        IsLoadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "KrLoadInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;

                    }
                    else if (Settings.VersionID == 4)
                    {
                        IsLoadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "KpLoadInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;

                    }
                    else if (Settings.VersionID == 5)
                    {
                        IsLoadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "PLoadInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabel method -> in DashboardViewModel.cs " + Settings.userLoginID);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async void GetQuestions()
        {
            try
            {
                loadindicator = true;

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var configurationResult = await trackService.GetAllMInspectionConfigurations();
                    
                    if (configurationResult != null && configurationResult.data != null)
                    {
                        Settings.allInspectionConfigurations = configurationResult.data;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetQuestions method -> in DashboardViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        #region Properties

        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields Job { get; set; } = new DashboardLabelFields { Status = true, Name = "Job" };
            public DashboardLabelFields Project { get; set; } = new DashboardLabelFields { Status = true, Name = "Project" };
            public DashboardLabelFields Company { get; set; } = new DashboardLabelFields { Status = true, Name = "Company" };
            public DashboardLabelFields Supplier { get; set; } = new DashboardLabelFields { Status = true, Name = "SupplierCompanyName" };
            public DashboardLabelFields Home { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMHome" };
            public DashboardLabelFields Jobs { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMTask" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMParts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMLoad" };
            public DashboardLabelFields Scan { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMScan" };
            public DashboardLabelFields Compare { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMCompare" };
            public DashboardLabelFields CompareCont { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMScan" };
            public DashboardLabelFields Photo { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMPhoto" };

        }
        public class DashboardLabelFields : IBase
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

        private string _HomePageTitle { set; get; }
        public string HomePageTitle
        {
            get
            {
                return _HomePageTitle;
            }
            set
            {
                this._HomePageTitle = value;
                NotifyPropertyChanged("HomePageTitle");
            }
        }

        private string _JobCountText { set; get; }
        public string JobCountText
        {
            get
            {
                return _JobCountText;
            }
            set
            {
                this._JobCountText = value;
                NotifyPropertyChanged("JobCountText");
            }
        }

        private bool _IsJobCountVisible { set; get; }
        public bool IsJobCountVisible
        {
            get
            {
                return _IsJobCountVisible;
            }
            set
            {
                this._IsJobCountVisible = value;
                NotifyPropertyChanged("IsJobCountVisible");
            }
        }

        private bool _IsLoadTabVisible { set; get; } = true;
        public bool IsLoadTabVisible
        {
            get
            {
                return _IsLoadTabVisible;
            }
            set
            {
                this._IsLoadTabVisible = value;
                NotifyPropertyChanged("IsLoadTabVisible");
            }
        }

        private bool _IsPNenable = false;
        public bool IsPNenable
        {
            get { return _IsPNenable; }
            set
            {
                _IsPNenable = value;
                NotifyPropertyChanged();
            }
        }

        private int _NotificationListCount;
        public int NotificationListCount
        {
            get { return _NotificationListCount; }
            set
            {
                _NotificationListCount = value;
                NotifyPropertyChanged();
            }
        }

        private string _NotifyCountTxt;
        public string NotifyCountTxt
        {
            get { return _NotifyCountTxt; }
            set
            {
                _NotifyCountTxt = value;
                NotifyPropertyChanged();
            }
        }

        private string _Company = Settings.CompanySelected;
        public string Company
        {
            get { return _Company; }
            set
            {
                _Company = value;
                NotifyPropertyChanged("Company");
            }
        }

        private string _ProjectName = Settings.ProjectSelected;
        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                _ProjectName = value;
                NotifyPropertyChanged("ProjectName");
            }
        }

        private string _JobName = Settings.JobSelected;
        public string JobName
        {
            get { return _JobName; }
            set
            {
                _JobName = value;
                NotifyPropertyChanged("JobName");
            }
        }

        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                NotifyPropertyChanged("BgColor");
            }
        }

        private bool _isCompareVisible = true;
        public bool isCompareVisible
        {
            get => _isCompareVisible;
            set
            {
                _isCompareVisible = value;
                NotifyPropertyChanged("isCompareVisible");
            }
        }

        private bool _IsMobileCompareCont = Settings.IsMobileCompareCont;
        public bool IsMobileCompareCont
        {
            get => _IsMobileCompareCont;
            set
            {
                _IsMobileCompareCont = value;
                NotifyPropertyChanged("IsMobileCompareCont");
            }
        }

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator;
            set
            {
                _loadindicator = value;
                NotifyPropertyChanged("loadindicator");
            }
        }

        private string _CompanyName;
        public string CompanyName
        {
            get { return _CompanyName; }
            set
            {
                _CompanyName = value;
                NotifyPropertyChanged();
            }
        }

        private string _ProNjobName;
        public string ProNjobName
        {
            get { return _ProNjobName; }
            set
            {
                _ProNjobName = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Properties
    }
}
