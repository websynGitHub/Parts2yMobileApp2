using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.Views;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.ViewModel
{
    public class ProfileSelectionViewModel : IBase
    {
        #region IComman and data members declaration
        public ICommand ICommandSetAsDefault { get; set; }
        public ICommand ICommandProfile { set; get; }
        public ICommand ICommandSettings { set; get; }
        public ICommand ICommandUpdate { get; set; }
        public ICommand Backevnttapped { set; get; }
        public INavigation Navigation { get; set; }

        YPSService service;
        SaveUserDefaultSettingsModel SaveUDS;
        bool checkInternet, DefaultSettingSupplier;
        #endregion

        /// <summary>
        /// Parameter less constructor.
        /// </summary>
        public ProfileSelectionViewModel(INavigation _Navigation, int pagetype)
        {
            YPSLogger.TrackEvent("ProfileSelectionViewModel", "page loading " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                Navigation = _Navigation;
                BgColor = YPS.CommonClasses.Settings.Bar_Background;

                service = new YPSService();

                SaveUDS = new SaveUserDefaultSettingsModel();

                ICommandSetAsDefault = new Command(async () => await SetAsDefaultClick(pagetype));
                ICommandProfile = new Command(async () => await UpdateProfileClick());
                ICommandSettings = new Command(async () => await DefaultSettingsClick());
                Backevnttapped = new Command(async () => await Backevnttapped_click());

                profileBgColor = Color.LightBlue;
                //settingsTextColor = Color.White;
                settingsBgColor = Color.FromHex("#269DC9");
                //profileTextColor = Color.White;
                settingsVisibility = settingbox = true;
                profileVisibility = profilebox = false;

                ICommandUpdate = new Command(async () => await UpdateClick());
                GetProfileData();
                GetDefaultSettingsData();

                Task.Run(() => GetSettingsData());
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ProfileSelectionViewModel constructor -> in ProfileSelectionViewModel " + Settings.userLoginID);
            }
        }


        public async Task Backevnttapped_click()
        {
            try
            {
                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
                await Navigation.PopModalAsync();
            }
        }

        #region Methods related to Default Settings
        /// <summary>
        /// This method will get default settings and update profile data from the API.
        /// </summary>
        /// <returns></returns>
        private async Task GetSettingsData()
        {
            int suppliestatus = 0;
            IndicatorVisibility = true;
            YPSLogger.TrackEvent("ProfileSelectionViewModel", "in GDefaultSettingData method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                    {
                        DefaultSettingSupplier = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "DefaultSettingSupplier".Trim()).FirstOrDefault()) != null ? true : false;
                    }

                    // Calling API to get update profile data for the login user.
                    PDefaultSettingModel = await service.DefaultSettingProfile(Settings.userLoginID);
                    // Calling API to get default settings data for login user.
                    var DBresponse = await service.GetSaveUserDefaultSettings(Settings.userLoginID);

                    if (DBresponse.status != 0)
                    {
                        Settings.VersionID = DBresponse.data.VersionID;

                        if (PDefaultSettingModel != null && PDefaultSettingModel.status != 0)
                        {
                            // Getting all Label values based on the language Id and version Id from the settings page. 
                            if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                            {
                                var verfieldsforID = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                                if (verfieldsforID != null && verfieldsforID.Count > 0)
                                {
                                    string company = verfieldsforID.Where(wr => wr.FieldID == Settings.Companylabel1).Select(s => s.LblText).FirstOrDefault();
                                    string job = verfieldsforID.Where(wr => wr.FieldID == Settings.joblabel1).Select(s => s.LblText).FirstOrDefault();
                                    string proj = verfieldsforID.Where(wr => wr.FieldID == Settings.projectlabel1).Select(s => s.LblText).FirstOrDefault();
                                    string supplier = verfieldsforID.Where(wr => wr.FieldID == Settings.supplierlabel1).Select(s => s.LblText).FirstOrDefault();
                                    string setAsDefaultBtn = verfieldsforID.Where(wr => wr.FieldID == Settings.SetAsDefaultBtn1).Select(s => s.LblText).FirstOrDefault();
                                    string email = verfieldsforID.Where(wr => wr.FieldID == Settings.Emaillabel1).Select(s => s.LblText).FirstOrDefault();
                                    string givenName = verfieldsforID.Where(wr => wr.FieldID == Settings.GivenNamelabel1).Select(s => s.LblText).FirstOrDefault();
                                    string familyName = verfieldsforID.Where(wr => wr.FieldID == Settings.FamilyNamelabel1).Select(s => s.LblText).FirstOrDefault();
                                    string timeZone = verfieldsforID.Where(wr => wr.FieldID == Settings.TimeZonelabel1).Select(s => s.LblText).FirstOrDefault();
                                    string language = verfieldsforID.Where(wr => wr.FieldID == Settings.Languagelabel1).Select(s => s.LblText).FirstOrDefault();
                                    string update = verfieldsforID.Where(wr => wr.FieldID == Settings.UpdateBtn1).Select(s => s.LblText).FirstOrDefault();
                                    var supplierstatus = verfieldsforID.Where(wr => wr.FieldID == Settings.supplierlabel1).FirstOrDefault();
                                    var loginid = verfieldsforID.Where(wr => wr.FieldID == LoginLbl).Select(s => s.LblText).FirstOrDefault();
                                    suppliestatus = supplierstatus.Status;

                                    Companylabel = !string.IsNullOrEmpty(company) ? company + " *" : "Company" + " *";
                                    joblabel = !string.IsNullOrEmpty(job) ? job + " *" : "Job" + " *";
                                    projectlabel = !string.IsNullOrEmpty(proj) ? proj + " *" : "Project" + " *";
                                    supplierlabel = !string.IsNullOrEmpty(supplier) ? supplier : "Supplier";
                                    SetAsDefaultBtn = !string.IsNullOrEmpty(setAsDefaultBtn) ? setAsDefaultBtn : "Set As Default";
                                    EmailLbl = !string.IsNullOrEmpty(email) ? email + " *" : "Email" + " *";
                                    GivenNameLbl = !string.IsNullOrEmpty(givenName) ? givenName + " *" : "Given Name" + " *";
                                    FamilyNameLbl = !string.IsNullOrEmpty(familyName) ? familyName : "Family Name";
                                    TimeZoneLbl = !string.IsNullOrEmpty(timeZone) ? timeZone + " *" : "Time Zone" + " *";
                                    LangaugeLbl = !string.IsNullOrEmpty(language) ? language + " *" : "Language" + " *";
                                    UpdateBtn = !string.IsNullOrEmpty(update) ? update : "Update";
                                    LoginLbl = !string.IsNullOrEmpty(loginid) ? loginid + " *" : "Login ID" + " *";
                                }
                            }

                            // Getting company data based on company id from a list of the company.
                            var companyData = PDefaultSettingModel.data.Company.Where(s => s.ID == DBresponse.data.CompanyID).FirstOrDefault();
                            CompanyName = companyData.Name;
                            Settings.CompanyID = companyData.ID;

                            // List of company
                            CompanyList = PDefaultSettingModel.data.Company;

                            // Getting project data based on project id from a list of the project.
                            var projectData = PDefaultSettingModel.data.Project.Where(s => s.ID == DBresponse.data.ProjectID).FirstOrDefault();
                            ProjectName = projectData.Name;
                            Settings.ProjectID = projectData.ID;

                            // List of project based on company id.
                            ProjectList = PDefaultSettingModel.data.Project.Where(s => s.ParentID == companyData.ID).ToList();

                            // Getting job data based on job id from a list of the job.
                            var jobData = PDefaultSettingModel.data.Job.Where(s => s.ID == DBresponse.data.JobID).FirstOrDefault();
                            JobName = jobData.Name;
                            Settings.JobID = jobData.ID;

                            // List of job based on project id.
                            JobList = PDefaultSettingModel.data.Job.Where(s => s.ParentID == projectData.ID).ToList();

                            if (DefaultSettingSupplier == true)
                            {
                                //Checking supplier status based on RoleID, Base on status supplier picker Hide/Show.
                                if (DefaultSettingSupplier == true)
                                {
                                    SupplierLabelAndFrame = true;
                                }
                                else
                                {
                                    SupplierLabelAndFrame = false;
                                    SupplierGridRowHeight = 0;
                                }
                                var supplierData = PDefaultSettingModel.data.Supplier.Where(s => s.ID == DBresponse.data.SupplierID).FirstOrDefault();
                                if (supplierData != null)
                                {
                                    SupplierName = supplierData.Name;
                                    Settings.SupplierID = supplierData.ID;
                                }
                                else
                                {
                                    SupplierName = "ALL";
                                    Settings.SupplierID = 0;
                                }
                                List<string> allValues = new List<string>();

                                //Getting a list of supplier names from a list supplier model.
                                var list = PDefaultSettingModel.data.Supplier.Select(s => s.Name).ToList();
                                allValues.Add("ALL");
                                allValues.AddRange(list);
                                SupplierList = new ObservableCollection<string>(allValues);
                            }
                            else
                            {
                                SupplierGridRowHeight = 0;
                            }
                        }
                        else
                        {
                            //  DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                        }
                    }
                    else //If the login user default setting data is not available.
                    {
                        if (PDefaultSettingModel.status != 0)
                        {
                            //Getting a list of company name from company list model.
                            CompanyList = PDefaultSettingModel.data.Company;

                            if (DefaultSettingSupplier == true)
                            {
                                //Supplier layout showing with a list of suppliers base on supplier id.
                                SupplierLabelAndFrame = true;
                                var supplierData = PDefaultSettingModel.data.Supplier.Where(s => s.ID == DBresponse.data.SupplierID).FirstOrDefault();

                                if (supplierData != null)
                                {
                                    SupplierName = supplierData.Name;
                                    Settings.SupplierID = supplierData.ID;
                                }
                                else
                                {
                                    SupplierName = "ALL";
                                    Settings.SupplierID = 0;
                                }
                                List<string> allValues = new List<string>();

                                //Getting a list of supplier name from a list supplier model.
                                var list = PDefaultSettingModel.data.Supplier.Select(s => s.Name).ToList();
                                allValues.Add("ALL");
                                allValues.AddRange(list);
                                SupplierList = new ObservableCollection<string>(allValues);
                            }
                        }

                        Companylabel = "Company" + " *";
                        joblabel = "Job" + " *";
                        projectlabel = "Project" + " *";
                        supplierlabel = "Supplier";
                        SetAsDefaultBtn = "Set As Default";
                        EmailLbl = "Email" + " *";
                        GivenNameLbl = "Given Name" + " *";
                        FamilyNameLbl = "Family Name" + " *";
                        TimeZoneLbl = "Time Zone";
                        LangaugeLbl = "Language";
                        UpdateBtn = "Update";
                        LoginLbl = "Login ID" + " *";
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GDefaultSettingData method -> in ProfileSelectionViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method is used to save and update user default settings data when clicked on Set as Default button.
        /// </summary>
        /// <returns></returns>
        private async Task SetAsDefaultClick(int pagetypeNav)
        {
            YPSLogger.TrackEvent("ProfileSelectionViewModel", "in SetAsDefault method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;
            try
            {
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (CompanyName.ToLower().Trim() == "please select company" || string.IsNullOrEmpty(CompanyName))
                    {
                        companyHaserror = true;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select company.");
                    }
                    else if (ProjectName.ToLower().Trim() == "please select project" || string.IsNullOrEmpty(ProjectName))
                    {
                        projectHaserror = true;
                        companyHaserror = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select project.");
                    }
                    else if (JobName.ToLower().Trim() == "please select job" || string.IsNullOrEmpty(JobName))
                    {
                        jobHaserror = true;
                        companyHaserror = false;
                        projectHaserror = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select job.");
                    }
                    else
                    {
                        bool result = await App.Current.MainPage.DisplayAlert("Set as default", "Are you sure?", "Yes", "No");

                        if (result)
                        {
                            //Getting company, project and job data base on select company,project and job from company, project and job pickers. 
                            var allCompanyValues = PDefaultSettingModel.data.Company.Where(X => X.ID == Settings.CompanyID).FirstOrDefault();
                            var allProjectValues = PDefaultSettingModel.data.Project.Where(X => X.ID == Settings.ProjectID).FirstOrDefault();
                            var allJobValues = PDefaultSettingModel.data.Job.Where(X => X.ID == Settings.JobID).FirstOrDefault();

                            SaveUDS.CompanyID = Settings.CompanyID = allCompanyValues.ID;
                            SaveUDS.ProjectID = Settings.ProjectID = allProjectValues.ID;
                            SaveUDS.JobID = Settings.JobID = allJobValues.ID;
                            SaveUDS.UserID = Settings.userLoginID;
                            SaveUDS.SupplierID = Settings.SupplierID;
                            // Calling API to save and upadate default settings data.
                            var DBresponse = await service.SaveUserDefaultSettings(SaveUDS);

                            if (DBresponse != null)
                            {
                                if (DBresponse.status != 0) // if user default settings are saved successfully
                                {
                                    //if (pagetypeNav == (int)QAType.YS)
                                    //{
                                    //    App.Current.MainPage = new YPSMasterPage(typeof(YshipPage));
                                    //}
                                    //else
                                    //{
                                    //    App.Current.MainPage = new YPSMasterPage(typeof(MainPage));

                                    //}
                                    var val = await service.GetSaveUserDefaultSettings(Settings.userLoginID);

                                    if (val != null && val.data != null)
                                    {
                                        Settings.Bar_Background = Color.FromHex(val.data.VersionColorCode);
                                    }

                                    App.Current.MainPage = new MenuPage(typeof(HomePage));
                                }
                                else
                                {
                                    //  DependencyService.Get<IToastMessage>().ShortAlert(DBresponse.message);
                                }
                            }
                            else
                            {
                                //   DependencyService.Get<IToastMessage>().ShortAlert("Something went wroung please try again.");
                            }
                        }
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    IndicatorVisibility = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SetAsDefault method -> in ProfileSelectionViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
                Settings.ShowSuccessAlert = false;
            }
            finally
            {
                IndicatorVisibility = false;
                Settings.ShowSuccessAlert = true;
            }
        }
        #endregion

        /// <summary>
        /// Gets called when clicking on "Default Setting" button.
        /// </summary>
        /// <returns></returns>
        private async Task DefaultSettingsClick()
        {
            try
            {
                //settingsBgColor = Color.FromHex("#269DC9");
                settingsTextColor = Settings.Bar_Background;
                //profileBgColor = Color.LightBlue;
                profileTextColor = Color.Black;
                settingsVisibility = settingbox = true;
                profileVisibility = profilebox = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DefaultSettingsClick method -> in ProfileSelectionViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Update Profile" button.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateProfileClick()
        {
            try
            {
                //settingsBgColor = Color.Black;
                settingsTextColor = Color.Black;
                //profileBgColor = Color.FromHex("#269DC9");
                profileTextColor = Settings.Bar_Background;
                settingsVisibility = settingbox = false;
                profileVisibility = profilebox = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "UpdateProfileClick method -> in ProfileSelectionViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        #region Methods related to Update Profile
        /// <summary>
        /// This method will get update profile data from the API.
        /// </summary>
        public async void GetProfileData()
        {
            IndicatorVisibility = true;
            try
            {
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    int loginID = Settings.userLoginID;
                    // Calling GetProfile API.
                    var getProfileData = await service.GetProfile(loginID);

                    if (getProfileData != null && getProfileData.status != 0 && getProfileData.data != null)
                    {
                        //Setting update profile data to Labels.
                        Email = getProfileData.data.Email;
                        LoginID = getProfileData.data.LoginID;
                        GivenName = getProfileData.data.GivenName;
                        FamilyName = getProfileData.data.FamilyName;
                        TimeZoneTextDisplay = getProfileData.data.UserCulture;
                    }
                    else
                    {
                        //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "getProfileData method -> in ProfileSelectionViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This methed will get time zones and languages data from the API's.
        /// </summary>
        public async void GetDefaultSettingsData()
        {
            IndicatorVisibility = true;

            try
            {
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();
                //If internet connection is available.
                if (checkInternet)
                {
                    // Calling TimeZones API.
                    var stimeZone = await service.GetTimeZone();

                    if (stimeZone != null && stimeZone.status != 0 && (stimeZone.data != null && stimeZone.data.Count > 0))
                    {
                        //Setting a list of TimeZone to TimeZone PopUp
                        TimeZone = new ObservableCollection<string>(stimeZone.data.Select(X => X.DisplayText).ToList());
                    }
                    else
                    {
                        // DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                    }

                    // Calling Languages API.
                    var language = await service.GetLanguages();

                    if (language != null && language.status != 0 && (language.data != null && language.data.Count > 0))
                    {
                        ListOfLanguage = language.data;
                        var addLangName = new List<string>();

                        ////Setting a list of languages to language PopUp.
                        addLangName.AddRange(language.data.Select(X => X.Name).ToList());
                        ListOfLanguageName = new ObservableCollection<string>(addLangName);

                        if (Settings.LanguageID != 0)
                        {
                            var items = language.data.Where(X => X.ID == Settings.LanguageID).FirstOrDefault();
                            LangaugeTextDisplay = items.Name;
                        }
                    }
                    else
                    {
                        //   DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "getData method -> in ProfileSelectionViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method will Update profile data when clicked on the Update button.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateClick()
        {
            try
            {
                IndicatorVisibility = true;
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();
                //If internet connection is available.

                if (checkInternet)
                {
                    //RememberPwdDB is a SQLite database class, this is use for save user login detail in mobile.
                    RememberPwdDB rememberPwd = new RememberPwdDB();
                    RememberPwd UpdateDb = new RememberPwd();

                    if (string.IsNullOrEmpty(LoginID))
                    {
                        loginIDHaserror = true;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please enter Login ID.");
                    }
                    if (String.IsNullOrEmpty(Email))
                    {
                        emailHaserror = true;
                        loginIDHaserror = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please enter values for all mandatory fields.");
                    }
                    else if ((!Regex.IsMatch(Email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$")))
                    {
                        emailHaserror = true;
                        loginIDHaserror = false;
                        await App.Current.MainPage.DisplayAlert("Login", "Invalid email", "Ok");
                    }
                    else if (String.IsNullOrEmpty(GivenName))
                    {
                        givennameHaserror = true;
                        emailHaserror = false;
                        loginIDHaserror = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please enter values for all mandatory fields.");
                    }
                    else if (string.IsNullOrEmpty(TimeZoneTextDisplay) || string.IsNullOrEmpty(TimeZoneTextDisplay))
                    {
                        timezoneHaserror = true;
                        givennameHaserror = false;
                        emailHaserror = false;
                        loginIDHaserror = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select Timezone.");
                    }
                    else if (LangaugeTextDisplay == "Please Select" || string.IsNullOrEmpty(LangaugeTextDisplay))
                    {
                        languageHaserror = true;
                        timezoneHaserror = false;
                        givennameHaserror = false;
                        emailHaserror = false;
                        loginIDHaserror = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select language.");
                    }
                    else
                    {
                        bool result = await App.Current.MainPage.DisplayAlert("Update profile", "Are you sure?", "Yes", "No");

                        if (result)
                        {
                            UpdateProfileData uProfile = new UpdateProfileData();
                            uProfile.UserID = Settings.userLoginID;
                            uProfile.CreatedBy = Settings.userLoginID;
                            uProfile.GivenName = GivenName;
                            uProfile.LoginID = LoginID;
                            uProfile.FamilyName = FamilyName;
                            uProfile.UserCulture = TimeZoneTextDisplay;

                            if (LangaugeIDLocal == 0)
                            {
                                uProfile.LanguageID = Settings.LanguageID;
                            }
                            else
                            {
                                uProfile.LanguageID = LangaugeIDLocal;
                                Settings.LanguageID = LangaugeIDLocal;
                            }

                            uProfile.Email = Email;
                            // Calling UpdateProfile API to update profile data.
                            var finalResponse = await service.UpdateProfile(uProfile);

                            if (finalResponse.status != 0 || finalResponse != null)
                            {
                                Settings.SGivenName = GivenName;
                                UpdateDb.GivenName = GivenName;
                                //Upadating user login id in local database.
                                rememberPwd.UpdatePWd(UpdateDb, Settings.userLoginID);

                                //if (Settings.RedirectPage == "Yship")
                                //{
                                //    App.Current.MainPage = new YPSMasterPage(typeof(YshipPage));
                                //}
                                //else
                                //{
                                //    App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                                //}

                                App.Current.MainPage = new MenuPage(typeof(HomePage));
                            }
                            else
                            {
                                //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                            }
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
                YPSLogger.ReportException(ex, "UpdateProfile method -> in ProfileSelectionViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
                Settings.ShowSuccessAlert = false;
            }
            finally
            {
                IndicatorVisibility = false;
                Settings.ShowSuccessAlert = true;
            }
        }
        #endregion

        #region Properties

        private Color _SetAsDefaultTextColor = Color.LightBlue;
        public Color SetAsDefaultTextColor
        {
            get => _SetAsDefaultTextColor;
            set
            {
                _SetAsDefaultTextColor = value;
                RaisePropertyChanged("SetAsDefaultTextColor");
            }
        }

        private Color _ProfileSelectionTextColor = Color.Black;
        public Color ProfileSelectionTextColor
        {
            get => _ProfileSelectionTextColor;
            set
            {
                _ProfileSelectionTextColor = value;
                RaisePropertyChanged("ProfileSelectionTextColor");
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

        ObservableCollection<DDLmaster> _CompanyList;
        public ObservableCollection<DDLmaster> CompanyList
        {
            get { return _CompanyList; }
            set
            {
                _CompanyList = value;
                NotifyPropertyChanged();
            }
        }
        List<DDLmaster> _ProjectList;
        public List<DDLmaster> ProjectList
        {
            get { return _ProjectList; }
            set
            {
                _ProjectList = value;
                NotifyPropertyChanged();
            }
        }
        List<DDLmaster> _JobList;
        public List<DDLmaster> JobList
        {
            get { return _JobList; }
            set
            {
                _JobList = value;
                NotifyPropertyChanged();
            }
        }
        ObservableCollection<string> _SupplierList;
        public ObservableCollection<string> SupplierList
        {
            get { return _SupplierList; }
            set
            {
                _SupplierList = value;
                NotifyPropertyChanged();
            }
        }
        private string _companyName = "Please select company";
        public string CompanyName
        {
            get => _companyName;
            set
            {
                _companyName = value;
                NotifyPropertyChanged();
            }
        }
        private string _projectName = "Please select project";
        public string ProjectName
        {
            get => _projectName;
            set
            {
                _projectName = value;
                NotifyPropertyChanged();
            }
        }
        private string _jobName = "Please select job";
        public string JobName
        {
            get => _jobName;
            set
            {
                _jobName = value;
                NotifyPropertyChanged();
            }
        }
        private string _Supplier = "ALL";
        public string SupplierName
        {
            get => _Supplier;
            set
            {
                _Supplier = value;
                NotifyPropertyChanged();
            }
        }
        private DefaultSettingModel _PDefaultSettingModel;
        public DefaultSettingModel PDefaultSettingModel
        {
            get => _PDefaultSettingModel;
            set
            {
                _PDefaultSettingModel = value;
                NotifyPropertyChanged();
            }
        }
        private bool _SupplierFrame = false;
        public bool SupplierLabelAndFrame
        {
            get => _SupplierFrame;
            set
            {
                _SupplierFrame = value;
                NotifyPropertyChanged();
            }
        }

        private int _SupplierGridRowHeight = 55;
        public int SupplierGridRowHeight
        {
            get => _SupplierGridRowHeight;
            set
            {
                _SupplierGridRowHeight = value;
                NotifyPropertyChanged();
            }
        }

        private Color _profileBgColor = Color.LightBlue;
        public Color profileBgColor
        {
            get { return _profileBgColor; }
            set
            {
                _profileBgColor = value;
                NotifyPropertyChanged();
            }
        }
        private Color _profileTextColor = Color.Black;
        public Color profileTextColor
        {
            get { return _profileTextColor; }
            set
            {
                _profileTextColor = value;
                RaisePropertyChanged("profileTextColor");
            }
        }
        private Color _settingsBgColor = Color.FromHex("#269DC9");
        public Color settingsBgColor
        {
            get { return _settingsBgColor; }
            set
            {
                _settingsBgColor = value;
                NotifyPropertyChanged();
            }
        }
        private Color _settingsTextColor = Settings.Bar_Background;
        public Color settingsTextColor
        {
            get { return _settingsTextColor; }
            set
            {
                _settingsTextColor = value;
                RaisePropertyChanged("settingsTextColor");
            }
        }

        bool _profileVisibility = false;
        public bool profileVisibility
        {
            get { return _profileVisibility; }
            set
            {
                _profileVisibility = value;
                NotifyPropertyChanged();
            }
        }
        bool _settingsVisibility = false;
        public bool settingsVisibility
        {
            get { return _settingsVisibility; }
            set
            {
                _settingsVisibility = value;
                NotifyPropertyChanged();
            }
        }

        bool _UpdateprofiletitleVisibility = false;
        public bool UpdateprofiletitleVisibility
        {
            get { return _UpdateprofiletitleVisibility; }
            set
            {
                _UpdateprofiletitleVisibility = value;
                NotifyPropertyChanged();
            }
        }
        int _UpdateprofiletitleRowHt = 0;
        public int UpdateprofiletitleRowHt
        {
            get { return _UpdateprofiletitleRowHt; }
            set
            {
                _UpdateprofiletitleRowHt = value;
                NotifyPropertyChanged();
            }
        }
        string _Companylabel;
        public string Companylabel
        {
            get { return _Companylabel; }
            set
            {
                _Companylabel = value;
                NotifyPropertyChanged();
            }
        }
        string _projectlabel;
        public string projectlabel
        {
            get { return _projectlabel; }
            set
            {
                _projectlabel = value;
                NotifyPropertyChanged();
            }
        }
        string _joblabel;
        public string joblabel
        {
            get { return _joblabel; }
            set
            {
                _joblabel = value;
                NotifyPropertyChanged();
            }
        }
        string _supplierlabel;
        public string supplierlabel
        {
            get { return _supplierlabel; }
            set
            {
                _supplierlabel = value;
                NotifyPropertyChanged();
            }
        }
        private string _SetAsDefaultBtn;
        public string SetAsDefaultBtn
        {
            get { return _SetAsDefaultBtn; }
            set
            {
                _SetAsDefaultBtn = value;
                NotifyPropertyChanged();
            }
        }

        private string _EmailLbl;
        public string EmailLbl
        {
            get => _EmailLbl;
            set
            {
                _EmailLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _Email;
        public string Email
        {
            get => _Email;
            set
            {
                _Email = value;
                NotifyPropertyChanged();
            }
        }
        private string _LoginID;
        public string LoginID
        {
            get => _LoginID;
            set
            {
                _LoginID = value;
                NotifyPropertyChanged();
            }
        }
        private string _GivenNameLbl;
        public string GivenNameLbl
        {
            get => _GivenNameLbl;
            set
            {
                _GivenNameLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _GivenName;
        public string GivenName
        {
            get => _GivenName;
            set
            {
                _GivenName = value;
                NotifyPropertyChanged();
            }
        }
        private int _LangaugeIDLocal;
        public int LangaugeIDLocal
        {
            get => _LangaugeIDLocal;
            set
            {
                _LangaugeIDLocal = value;
                NotifyPropertyChanged();
            }
        }
        private string _FamilyNameLbl;
        public string FamilyNameLbl
        {
            get => _FamilyNameLbl;
            set
            {
                _FamilyNameLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _FamilyName;
        public string FamilyName
        {
            get => _FamilyName;
            set
            {
                _FamilyName = value;
                NotifyPropertyChanged();
            }
        }
        private string _TimeZoneLbl;
        public string TimeZoneLbl
        {
            get => _TimeZoneLbl;
            set
            {
                _TimeZoneLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _TimeZoneTextDisplay = "Please Select";
        public string TimeZoneTextDisplay
        {
            get => _TimeZoneTextDisplay;
            set
            {
                _TimeZoneTextDisplay = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<string> _TimeZone;
        public ObservableCollection<string> TimeZone
        {
            get { return _TimeZone; }
            set
            {
                _TimeZone = value;
                NotifyPropertyChanged();
            }
        }
        private string _LangaugeLbl;
        public string LangaugeLbl
        {
            get => _LangaugeLbl;
            set
            {
                _LangaugeLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _LoginLbl = "LoginID";
        public string LoginLbl
        {
            get => _LoginLbl;
            set
            {
                _LoginLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _LangaugeTextDisplay = "Please Select";
        public string LangaugeTextDisplay
        {
            get => _LangaugeTextDisplay;
            set
            {
                _LangaugeTextDisplay = value;
                NotifyPropertyChanged();
            }
        }
        private string _UpdateBtn = "Update";
        public string UpdateBtn
        {
            get { return _UpdateBtn; }
            set
            {
                _UpdateBtn = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<string> _ListOfLanguageName;
        public ObservableCollection<string> ListOfLanguageName
        {
            get { return _ListOfLanguageName; }
            set
            {
                _ListOfLanguageName = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<LanguageModel> _ListOfLanguage;
        public ObservableCollection<LanguageModel> ListOfLanguage
        {
            get { return _ListOfLanguage; }
            set
            {
                _ListOfLanguage = value;
                NotifyPropertyChanged();
            }
        }
        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private bool _settingbox = true;
        public bool settingbox
        {
            get { return _settingbox; }
            set
            {
                _settingbox = value;
                NotifyPropertyChanged();
            }
        }
        private bool _profilebox = false;
        public bool profilebox
        {
            get { return _profilebox; }
            set
            {
                _profilebox = value;
                NotifyPropertyChanged();
            }
        }

        private bool _loginIDHaserror = false;
        public bool loginIDHaserror
        {
            get { return _loginIDHaserror; }
            set
            {
                _loginIDHaserror = value;
                NotifyPropertyChanged();
            }
        }

        private bool _givennameHaserror = false;
        public bool givennameHaserror
        {
            get { return _givennameHaserror; }
            set
            {
                _givennameHaserror = value;
                NotifyPropertyChanged();
            }
        }

        private bool _emailHaserror = false;
        public bool emailHaserror
        {
            get { return _emailHaserror; }
            set
            {
                _emailHaserror = value;
                NotifyPropertyChanged();
            }
        }

        private bool _timezoneHaserror = false;
        public bool timezoneHaserror
        {
            get { return _timezoneHaserror; }
            set
            {
                _timezoneHaserror = value;
                NotifyPropertyChanged();
            }
        }

        private bool _languageHaserror = false;
        public bool languageHaserror
        {
            get { return _languageHaserror; }
            set
            {
                _languageHaserror = value;
                NotifyPropertyChanged();
            }
        }

        private bool _companyHaserror = false;
        public bool companyHaserror
        {
            get { return _companyHaserror; }
            set
            {
                _companyHaserror = value;
                NotifyPropertyChanged();
            }
        }

        private bool _projectHaserror = false;
        public bool projectHaserror
        {
            get { return _projectHaserror; }
            set
            {
                _projectHaserror = value;
                NotifyPropertyChanged();
            }
        }

        private bool _jobHaserror = false;
        public bool jobHaserror
        {
            get { return _jobHaserror; }
            set
            {
                _jobHaserror = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
