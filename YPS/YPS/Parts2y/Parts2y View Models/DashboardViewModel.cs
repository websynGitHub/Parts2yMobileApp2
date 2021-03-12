using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.Helpers;
//using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Model;
using YPS.Service;
using System.Linq;
using Acr.UserDialogs;
using ZXing.Net.Mobile.Forms;

namespace YPS.Parts2y.Parts2y_View_Models
{
    class DashboardViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public Command TaskClickCmd { get; set; }
        public Command ScanClickCmd { get; set; }
        public Command CompareClickCmd { get; set; }
        public Command PhotoClickCmd { get; set; }
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command TaskCmd { get; set; }
        public Command LoadCmd { set; get; }


        YPSService trackService;

        public DashboardViewModel(INavigation _Navigation)
        {
            try
            {
                Navigation = _Navigation;
                trackService = new YPSService();
                BgColor = YPS.CommonClasses.Settings.Bar_Background;
                TaskClickCmd = new Command(async () => await RedirectToPage("task"));
                CompareClickCmd = new Command(async () => await RedirectToPage("compare"));
                ScanClickCmd = new Command(async () => await RedirectToPage("scan"));
                PhotoClickCmd = new Command(async () => await RedirectToPage("photo"));

                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                TaskCmd = new Command(async () => await TabChange("task"));
                LoadCmd = new Command(async () => await TabChange("load"));


                Task.Run(() => RememberUserDetails()).Wait();
                Task.Run(() => GetActionStatus()).Wait();
                Task.Run(() => GetallApplabels()).Wait();
                Task.Run(() => ChangeLabel()).Wait();
                Task.Run(() => GetQuestions()).Wait();
            }
            catch (Exception ex)
            {

            }
        }

        private async Task TabChange(string tabname)
        {
            try
            {
                loadindicator = true;
                await Task.Delay(1);

                if (tabname == "job")
                {
                    await Navigation.PushAsync(new ParentListPage());
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
            }
            loadindicator = false;
        }

        /// <summary>
        /// This method is to remember user details.
        /// </summary>
        /// <returns></returns>
        public async Task RememberUserDetails()
        {
            try
            {
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
                    saveData.BgColor = Settings.Bar_Background.ToArgb();
                    Db.SaveUserPWd(saveData);

                    #region selected profile
                    if (!String.IsNullOrEmpty(Settings.CompanySelected))
                    {
                        Company = Settings.CompanySelected;
                    }

                    if (!String.IsNullOrEmpty(Settings.ProjectSelected) || !String.IsNullOrEmpty(Settings.JobSelected))
                    {
                        if (Settings.SupplierSelected == "ALL")
                        {
                            //ProNjobName = Settings.ProjectSelected + "/" + Settings.JobSelected;
                            ProjectName = Settings.ProjectSelected;
                            JobName = Settings.JobSelected;
                        }
                        else
                        {
                            var pNjobName = Settings.ProjectSelected + "/" + Settings.JobSelected + "/" + Settings.SupplierSelected;
                            string trimpNjobName = pNjobName.TrimEnd('/');
                            ProNjobName = trimpNjobName;
                            ProjectName = Settings.ProjectSelected;
                            JobName = Settings.JobSelected;
                            SupplierName = Settings.SupplierSelected;
                        }
                    }
                    #endregion
                }
                else
                {
                    var DBresponse = await trackService.GetSaveUserDefaultSettings(Settings.userLoginID);

                    if (DBresponse != null)
                    {
                        if (DBresponse.status == 1)
                        {
                            Settings.VersionID = DBresponse.data.VersionID;
                            Company = Settings.CompanySelected = DBresponse.data.CompanyName;

                            if (DBresponse.data.SupplierName == "")
                            {
                                ProNjobName = DBresponse.data.ProjectName + "/" + DBresponse.data.JobNumber;
                                ProjectName = Settings.ProjectSelected = DBresponse.data.ProjectName;
                                JobName = Settings.JobSelected = DBresponse.data.JobNumber;
                                Settings.CompanyID = DBresponse.data.CompanyID;
                                Settings.ProjectID = DBresponse.data.ProjectID;
                                Settings.JobID = DBresponse.data.JobID;
                                Settings.SupplierSelected = DBresponse.data.SupplierName;
                                Settings.SupplierID = DBresponse.data.SupplierID;
                            }
                            else
                            {
                                ProNjobName = DBresponse.data.ProjectName + "/" + DBresponse.data.JobNumber + "/" + DBresponse.data.SupplierName;
                                ProjectName = Settings.ProjectSelected = DBresponse.data.ProjectName;
                                JobName = Settings.JobSelected = DBresponse.data.JobNumber;
                                Settings.CompanyID = DBresponse.data.CompanyID;
                                Settings.ProjectID = DBresponse.data.ProjectID;
                                Settings.JobID = DBresponse.data.JobID;
                                SupplierName = Settings.SupplierSelected = DBresponse.data.SupplierName;
                                Settings.SupplierID = DBresponse.data.SupplierID;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RememberUserDetails method -> in PoDataViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        public async Task RedirectToPage(string page)
        {
            //UserDialogs.Instance.ShowLoading("Loading...");
            try
            {
                loadindicator = true;
                await Task.Delay(2);

                if (page == "task")
                {
                    await Navigation.PushAsync(new ParentListPage());
                }
                else if (page == "compare")
                {
                    await Navigation.PushAsync(new Compare());
                }
                else if (page == "scan")
                {
                    await Navigation.PushAsync(new ScanPage(0, null, false, null));
                }
              
            }
            catch (Exception ex)
            {
                //UserDialogs.Instance.HideLoading(); ;
                loadindicator = false;
            }
            loadindicator = false;
            //UserDialogs.Instance.HideLoading();
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

                    if (result != null)
                    {
                        if (result.status != 0)
                        {
                            if (result.data.Count() > 0)
                            {
                                NotifyCountTxt = result.data[0].listCount.ToString();
                                Settings.notifyCount = result.data[0].listCount;
                            }
                            else
                            {
                                NotifyCountTxt = "0";
                                Settings.notifyCount = 0;
                            }
                        }
                        else if (result.message == "No Data Found")
                        {
                            NotifyCountTxt = "0";
                            Settings.notifyCount = 0;
                        }
                    }
                }
                else
                {
                    // DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetPNCount method -> in PoDataViewModel! " + Settings.userLoginID);
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
                if (Settings.alllabeslvalues == null || Settings.alllabeslvalues.Count == 0)
                {
                    var lblResult = await trackService.GetallApplabelsService();

                    if (lblResult != null && lblResult.data != null)
                    {
                        Settings.alllabeslvalues = lblResult.data.ToList();
                        var datavalues = Settings.alllabeslvalues.Where(x => x.VersionID == Settings.VersionID && x.LanguageID == Settings.LanguageID).ToList();
                        Settings.Companylabel = datavalues.Where(x => x.FieldID == Settings.Companylabel1).Select(m => m.LblText).FirstOrDefault();
                        Settings.projectlabel = datavalues.Where(x => x.FieldID == Settings.projectlabel1).Select(x => x.LblText).FirstOrDefault();
                        Settings.joblabel = datavalues.Where(x => x.FieldID == Settings.joblabel1).Select(x => x.LblText).FirstOrDefault();
                        Settings.supplierlabel = datavalues.Where(x => x.FieldID == Settings.supplierlabel1).Select(x => x.LblText).FirstOrDefault();

                    }
                    else
                    {
                        //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                    }
                }
                else
                {
                    if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                    {
                        var datavalues = Settings.alllabeslvalues.Where(x => x.VersionID == Settings.VersionID && x.LanguageID == Settings.LanguageID).ToList();

                        Settings.Companylabel = datavalues.Where(x => x.FieldID == Settings.Companylabel1).Select(m => m.LblText).FirstOrDefault();
                        Settings.projectlabel = datavalues.Where(x => x.FieldID == Settings.projectlabel1).Select(x => x.LblText).FirstOrDefault();
                        Settings.joblabel = datavalues.Where(x => x.FieldID == Settings.joblabel1).Select(x => x.LblText).FirstOrDefault();
                        Settings.supplierlabel = datavalues.Where(x => x.FieldID == Settings.supplierlabel1).Select(x => x.LblText).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetallApplabels method -> in DashboardViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to get the status of actions present in application.
        /// </summary>
        public async void GetActionStatus()
        {
            try
            {
                //if (Settings.alllabeslvalues == null || Settings.alllabeslvalues.Count == 0)
                //{
                var lblResult = await trackService.GetallActionStatusService();

                if (lblResult != null && lblResult.data != null)
                {
                    Settings.AllActionStatus = lblResult.data.ToList();
                }
                else
                {
                    //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetallApplabels method -> in DashboardViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is for changing the labels dynamically
        /// </summary>
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
                        //Getting Label values & Status based on FieldID
                        var company = labelval.Where(wr => wr.FieldID == labelobj.Company.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var project = labelval.Where(wr => wr.FieldID == labelobj.Project.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var job = labelval.Where(wr => wr.FieldID == labelobj.Job.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var home = labelval.Where(wr => wr.FieldID == labelobj.Home.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var jobs = labelval.Where(wr => wr.FieldID == labelobj.Jobs.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var parts = labelval.Where(wr => wr.FieldID == labelobj.Parts.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var load = labelval.Where(wr => wr.FieldID == labelobj.Load.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();


                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.Company.Name = (company != null ? (!string.IsNullOrEmpty(company.LblText) ? company.LblText : labelobj.Company.Name) : labelobj.Company.Name) + " :";
                        labelobj.Company.Status = company == null ? true : (company.Status == 1 ? true : false);
                        labelobj.Project.Name = (project != null ? (!string.IsNullOrEmpty(project.LblText) ? project.LblText : labelobj.Project.Name) : labelobj.Project.Name) + " :";
                        labelobj.Project.Status = project == null ? true : (project.Status == 1 ? true : false);
                        labelobj.Job.Name = (job != null ? (!string.IsNullOrEmpty(job.LblText) ? job.LblText : labelobj.Job.Name) : labelobj.Job.Name) + " :";
                        labelobj.Job.Status = job == null ? true : (job.Status == 1 ? true : false);

                        labelobj.Home.Name = (home != null ? (!string.IsNullOrEmpty(home.LblText) ? home.LblText : labelobj.Home.Name) : labelobj.Home.Name);
                        labelobj.Home.Status = home == null ? true : (home.Status == 1 ? true : false);
                        //labelobj.Jobs.Name = (jobs != null ? (!string.IsNullOrEmpty(jobs.LblText) ? jobs.LblText : labelobj.Jobs.Name) : labelobj.Jobs.Name);
                        //labelobj.Jobs.Status = jobs == null ? true : (jobs.Status == 1 ? true : false);
                        //labelobj.Parts.Name = (parts != null ? (!string.IsNullOrEmpty(parts.LblText) ? parts.LblText : labelobj.Parts.Name) : labelobj.Parts.Name);
                        labelobj.Parts.Status = parts == null ? true : (parts.Status == 1 ? true : false);
                        //labelobj.Load.Name = (load != null ? (!string.IsNullOrEmpty(load.LblText) ? load.LblText : labelobj.Load.Name) : labelobj.Load.Name);
                        //labelobj.Load.Status = load == null ? true : (load.Status == 1 ? true : false);
                        labelobj.Load.Name = Settings.VersionID == 2 ? "Insp" : "Load";
                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabelAndShowHide method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
            }
        }

        private async void GetQuestions()
        {
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

        #region Properties

        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields Job { get; set; } = new DashboardLabelFields { Status = true, Name = "Job" };
            public DashboardLabelFields Project { get; set; } = new DashboardLabelFields { Status = true, Name = "Project" };
            public DashboardLabelFields Company { get; set; } = new DashboardLabelFields { Status = true, Name = "Company" };
            public DashboardLabelFields Supplier { get; set; } = new DashboardLabelFields { Status = true, Name = "SupplierCompanyName" };
            public DashboardLabelFields Home { get; set; } = new DashboardLabelFields { Status = true, Name = "Home" };
            public DashboardLabelFields Jobs { get; set; } = new DashboardLabelFields { Status = true, Name = "Job" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "Parts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "Load" };

        }
        public class DashboardLabelFields : IBase
        {
            //public bool Status { get; set; }
            //public string Name { get; set; }

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
                OnPropertyChanged("Company");
            }
        }

        private string _ProjectName = Settings.ProjectSelected;
        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                _ProjectName = value;
                OnPropertyChanged("ProjectName");
            }
        }

        private string _JobName = Settings.JobSelected;
        public string JobName
        {
            get { return _JobName; }
            set
            {
                _JobName = value;
                OnPropertyChanged("JobName");
            }
        }


        private string _SupplierName = Settings.SupplierSelected;
        public string SupplierName
        {
            get { return _SupplierName; }
            set
            {
                _SupplierName = value;
                OnPropertyChanged("SupplierName");
            }
        }


        //private string _Id = Settings.CompanyID.ToString();
        //public string Id
        //{
        //    get { return _Id; }
        //    set
        //    {
        //        _UserName = value;
        //        OnPropertyChanged("Id");
        //    }
        //}

        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
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
