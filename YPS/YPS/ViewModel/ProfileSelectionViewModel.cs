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
        public ICommand ICommandUpdate { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand ICommandJobSave { set; get; }
        public ICommand ICommandPolySave { set; get; }
        public INavigation Navigation { get; set; }

        YPSService service;
        SaveUserDefaultSettingsModel SaveUDS;
        bool checkInternet;
        public bool PrintApiStatus;
        #endregion

        /// <summary>
        /// Parameter less constructor.
        /// </summary>
        public ProfileSelectionViewModel(INavigation _Navigation, int pagetype)
        {
            YPSLogger.TrackEvent("ProfileSelectionViewModel.cs ", "page loading " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                Navigation = _Navigation;
                BgColor = YPS.CommonClasses.Settings.Bar_Background;

                service = new YPSService();

                SaveUDS = new SaveUserDefaultSettingsModel();

                ICommandSetAsDefault = new Command(async () => await SetAsDefaultClick(pagetype));
                ICommandUpdate = new Command(async () => await UpdateClick());
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                ICommandJobSave = new Command(async () => await JobSaveClick());
                ICommandPolySave = new Command(async () => await PolySaveClick());

                profileBgColor = Color.LightBlue;
                settingsBgColor = Color.FromHex("#269DC9");
                settingsVisibility = true;
                PolyprintFieldTextColor = JobprintFieldTextColor = Color.Black;
                PolyprintfieldVisibility = JobprintfieldVisibility = profileVisibility = false;

                Task.Run(() => GetProfileData()).Wait();
                Task.Run(() => GetDefaultSettingsData()).Wait();
                Task.Run(() => GetSettingsData()).Wait();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ProfileSelectionViewModel constructor -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
            }
        }
        
        private async Task PolySaveClick()
        {
            try
            {
                bool result = await App.Current.MainPage.DisplayAlert("Save Polybox Print Fields", "Are you sure?", "Yes", "No");

                if (result)
                {
                    if (PolyPrintFields?.Where(wr => wr.Status == 1).FirstOrDefault() != null)
                    {
                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            var checkedJobPrintFields = PolyPrintFields?.Where(wr => wr.Status == 1).Select(c => c.Name).ToList();

                            var commafields = string.Join(",", checkedJobPrintFields);

                            var data = await service.UpdatePrintField("PolyboxPrint", commafields);

                            if (data?.status == 1)
                            {
                                PolyPrintFieldBorderColor = Color.Transparent;
                                Navigation.PopAsync(false);
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        }
                    }
                    else
                    {
                        PolyPrintFieldBorderColor = (PolyPrintFields == null
                            || PolyPrintFields?.Where(wr => wr.Status == 1).FirstOrDefault() == null) ?
                            Color.Red : Color.Transparent;
                    }
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "PolySaveClick constructor -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
            }
        }
      
        private async Task JobSaveClick()
        {
            try
            {
                bool result = await App.Current.MainPage.DisplayAlert("Save Print Fields", "Are you sure?", "Yes", "No");

                if (result)
                {
                    if (JobPrintFields?.Where(wr => wr.Status == 1).FirstOrDefault() != null)
                    {
                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            var checkedJobPrintFields = JobPrintFields?.Where(wr => wr.Status == 1).Select(c => c.Name).ToList();

                            var commafields = string.Join(",", checkedJobPrintFields);

                            var data = await service.UpdatePrintField("JobPrint", commafields);

                            if (data?.status == 1)
                            {
                                JobPrintFieldBorderColor = Color.Transparent;
                                Navigation.PopToRootAsync(false);
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        }
                    }
                    else
                    {
                        JobPrintFieldBorderColor = (JobPrintFields == null
                            || JobPrintFields?.Where(wr => wr.Status == 1).FirstOrDefault() == null) ?
                            Color.Red : Color.Transparent;
                    }
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "JobSaveClick constructor -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
            }
        }

        public async void GetPrintFieldData()
        {
            try
            {
                var resultData = await service.GetScanConfig();

                if (resultData?.data != null)
                {
                    JobPrintFields = resultData.data?.PrintFields_JobPart;
                    IsAllSelected = JobPrintFields?.All(a => a.Status == 1) == true ? true : false;
                    PrintApiStatus = resultData.status == 1 ? true : false;
                    if (JobPrintFields?.Count > 0)
                    {
                        JobPrintFields.ForEach(fr => fr.LblText
                        = verfieldsforID?.Where(wr => wr.FieldID == fr.Name).Select(c => c.LblText).FirstOrDefault());
                    }

                    PolyPrintFields = resultData.data?.PrintFields_Polybox;
                    PolyAllSelected = PolyPrintFields?.All(a => a.Status == 1) == true ? true : false;
                    PrintApiStatus = resultData.status == 1 ? true : false;
                    if (PolyPrintFields?.Count > 0)
                    {
                        var polyfields = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.PBVersionID && wr.LanguageID == Settings.LanguageID).ToList();
                        PolyPrintFields.ForEach(fr => fr.LblText
                        = polyfields?.Where(wr => wr.FieldID == fr.Name).Select(c => c.LblText).FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetPrintFieldData method -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                if (Navigation.ModalStack.Count > 0)
                {
                    await Navigation.PopModalAsync(false);
                }
                else
                {
                    await Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "Backevnttapped_click constructor -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("ProfileSelectionViewModel.cs ", "in GDefaultSettingData method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    // Calling API to get update profile data for the login user.
                    PDefaultSettingModel = await service.DefaultSettingProfile(Settings.userLoginID);
                    // Calling API to get default settings data for login user.
                    var DBresponse = await service.GetSaveUserDefaultSettings(Settings.userLoginID);

                    if (DBresponse != null && DBresponse.status == 1)
                    {
                        Settings.VersionID = DBresponse.data.VersionID;
                        Settings.ELevelID = DBresponse.data.EventID;

                        if (PDefaultSettingModel != null && PDefaultSettingModel.status == 1 && PDefaultSettingModel.data != null)
                        {
                            // Getting all Label values based on the language Id and version Id from the settings page. 
                            if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                            {
                                verfieldsforID = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                                if (verfieldsforID != null && verfieldsforID.Count > 0)
                                {
                                    string defaultsettings = verfieldsforID.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == Settings.DefaultSettinglabel.Trim().ToLower().Replace(" ", string.Empty)).Select(s => s.LblText).FirstOrDefault();
                                    string updateprofile = verfieldsforID.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == Settings.UpdateProfilelabel.Trim().ToLower().Replace(" ", string.Empty)).Select(s => s.LblText).FirstOrDefault();
                                    string company = verfieldsforID.Where(wr => wr.FieldID == Settings.Companylabel1).Select(s => s.LblText).FirstOrDefault();
                                    string job = verfieldsforID.Where(wr => wr.FieldID == Settings.joblabel1).Select(s => s.LblText).FirstOrDefault();
                                    string proj = verfieldsforID.Where(wr => wr.FieldID == Settings.projectlabel1).Select(s => s.LblText).FirstOrDefault();
                                    //string supplier = verfieldsforID.Where(wr => wr.FieldID == Settings.supplierlabel1).Select(s => s.LblText).FirstOrDefault();
                                    string setAsDefaultBtn = verfieldsforID.Where(wr => wr.FieldID == Settings.SetAsDefaultBtn1).Select(s => s.LblText).FirstOrDefault();
                                    string email = verfieldsforID.Where(wr => wr.FieldID == Settings.Emaillabel1).Select(s => s.LblText).FirstOrDefault();
                                    string givenName = verfieldsforID.Where(wr => wr.FieldID == Settings.GivenNamelabel1).Select(s => s.LblText).FirstOrDefault();
                                    string familyName = verfieldsforID.Where(wr => wr.FieldID == Settings.FamilyNamelabel1).Select(s => s.LblText).FirstOrDefault();
                                    string timeZone = verfieldsforID.Where(wr => wr.FieldID == Settings.TimeZonelabel1).Select(s => s.LblText).FirstOrDefault();
                                    string language = verfieldsforID.Where(wr => wr.FieldID == Settings.Languagelabel1).Select(s => s.LblText).FirstOrDefault();
                                    string update = verfieldsforID.Where(wr => wr.FieldID == Settings.UpdateBtn1).Select(s => s.LblText).FirstOrDefault();
                                    string save = verfieldsforID.Where(wr => wr.FieldID == Settings.SaveBtn).Select(s => s.LblText).FirstOrDefault();
                                    //var supplierstatus = verfieldsforID.Where(wr => wr.FieldID == Settings.supplierlabel1).FirstOrDefault();
                                    var loginid = verfieldsforID.Where(wr => wr.FieldID == LoginLbl).Select(s => s.LblText).FirstOrDefault();
                                    //suppliestatus = supplierstatus.Status;

                                    DefaultSettinglabel = !string.IsNullOrEmpty(defaultsettings) ? defaultsettings : "Default Setting";
                                    UpdateProfilelabel = !string.IsNullOrEmpty(updateprofile) ? updateprofile : "Update Profile";
                                    Companylabel = !string.IsNullOrEmpty(company) ? company + " *" : "Company" + " *";
                                    joblabel = !string.IsNullOrEmpty(job) ? job + " *" : "Job" + " *";
                                    projectlabel = !string.IsNullOrEmpty(proj) ? proj + " *" : "Project" + " *";
                                    SetAsDefaultBtn = !string.IsNullOrEmpty(setAsDefaultBtn) ? setAsDefaultBtn : "Set As Default";
                                    EmailLbl = !string.IsNullOrEmpty(email) ? email + " *" : "Email" + " *";
                                    GivenNameLbl = !string.IsNullOrEmpty(givenName) ? givenName + " *" : "Given Name" + " *";
                                    FamilyNameLbl = !string.IsNullOrEmpty(familyName) ? familyName : "Family Name";
                                    TimeZoneLbl = !string.IsNullOrEmpty(timeZone) ? timeZone + " *" : "Time Zone" + " *";
                                    LangaugeLbl = !string.IsNullOrEmpty(language) ? language + " *" : "Language" + " *";
                                    UpdateBtn = !string.IsNullOrEmpty(update) ? update : "Update";
                                    SaveBtn = !string.IsNullOrEmpty(save) ? save : "Save";
                                    LoginLbl = !string.IsNullOrEmpty(loginid) ? loginid + " *" : "Login ID" + " *";
                                    if (JobPrintFields?.Count > 0)
                                    {
                                        JobPrintFields.ForEach(fr => fr.LblText
                                        = verfieldsforID.Where(wr => wr.FieldID == fr.Name).Select(c => c.LblText).FirstOrDefault());
                                    }
                                }
                            }

                            if (PDefaultSettingModel.data.Company != null && PDefaultSettingModel.data.Company.Count > 0 &&
                                PDefaultSettingModel.data.Project != null && PDefaultSettingModel.data.Project.Count > 0 &&
                                PDefaultSettingModel.data.Job != null && PDefaultSettingModel.data.Job.Count > 0)
                            {
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
                            }
                        }
                    }
                    else //If the login user default setting data is not available.
                    {
                        if (PDefaultSettingModel?.status == 1)
                        {
                            //Getting a list of company name from company list model.
                            CompanyList = PDefaultSettingModel.data.Company;
                        }

                        DefaultSettinglabel = "Default Setting";
                        UpdateProfilelabel = "Update Profile";
                        Companylabel = "Company" + " *";
                        joblabel = "Job" + " *";
                        projectlabel = "Project" + " *";
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
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GDefaultSettingData method -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("ProfileSelectionViewModel.cs ", "in SetAsDefault method " + DateTime.Now + " UserId: " + Settings.userLoginID);
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
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please select company.");
                    }
                    else if (ProjectName.ToLower().Trim() == "please select project" || string.IsNullOrEmpty(ProjectName))
                    {
                        projectHaserror = true;
                        companyHaserror = false;
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please select project.");
                    }
                    else if (JobName.ToLower().Trim() == "please select job" || string.IsNullOrEmpty(JobName))
                    {
                        jobHaserror = true;
                        companyHaserror = false;
                        projectHaserror = false;
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please select job.");
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
                            // Calling API to save and upadate default settings data.
                            var DBresponse = await service.SaveUserDefaultSettings(SaveUDS);

                            if (DBresponse != null)
                            {
                                if (DBresponse.status == 1) // if user default settings are saved successfully
                                {
                                    var val = await service.GetSaveUserDefaultSettings(Settings.userLoginID);

                                    if (val?.data != null)
                                    {
                                        //Settings.Bar_Background = Color.FromHex(val.data?.VersionColorCode != null ? val.data.VersionColorCode : "#269DC9");
                                        Settings.Bar_Background = !string.IsNullOrEmpty(Settings.RoleColorCode) ? Color.FromHex(Settings.RoleColorCode) : (val.data?.VersionColorCode != null ? Color.FromHex(val.data.VersionColorCode) : Settings.Bar_Background);
                                        Settings.VersionID = val.data.VersionID;
                                        Settings.ELevelID = val.data.EventID;
                                    }
                                    Settings.CanCallForSettings = false;
                                    Navigation.PopToRootAsync(false);
                                }
                            }
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    IndicatorVisibility = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SetAsDefault method -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
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

                    if (getProfileData != null && getProfileData.status == 1 && getProfileData.data != null)
                    {
                        //Setting update profile data to Labels.
                        Email = getProfileData.data.Email;
                        LoginID = getProfileData.data.LoginID;
                        GivenName = getProfileData.data.GivenName;
                        FamilyName = getProfileData.data.FamilyName;
                        TimeZoneTextDisplay = getProfileData.data.UserCulture;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetProfileData method -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
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

                    if (stimeZone != null && stimeZone.status == 1 && (stimeZone.data != null && stimeZone.data.Count > 0))
                    {
                        //Setting a list of TimeZone to TimeZone PopUp
                        TimeZone = new ObservableCollection<string>(stimeZone.data.Select(X => X.DisplayText).ToList());
                    }

                    // Calling Languages API.
                    var language = await service.GetLanguages();

                    if (language != null && language.status == 1 && (language.data != null && language.data.Count > 0))
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
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetDefaultSettingsData method -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
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
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please enter Login ID.");
                    }
                    if (String.IsNullOrEmpty(Email))
                    {
                        emailHaserror = true;
                        loginIDHaserror = false;
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please enter values for all mandatory fields.");
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
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please enter values for all mandatory fields.");
                    }
                    else if (string.IsNullOrEmpty(TimeZoneTextDisplay) || string.IsNullOrEmpty(TimeZoneTextDisplay))
                    {
                        timezoneHaserror = true;
                        givennameHaserror = false;
                        emailHaserror = false;
                        loginIDHaserror = false;
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please select timezone.");
                    }
                    else if (LangaugeTextDisplay == "Please Select" || string.IsNullOrEmpty(LangaugeTextDisplay))
                    {
                        languageHaserror = true;
                        timezoneHaserror = false;
                        givennameHaserror = false;
                        emailHaserror = false;
                        loginIDHaserror = false;
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please select language.");
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

                            if (finalResponse?.status == 1)
                            {
                                Settings.SGivenName = GivenName;
                                UpdateDb.GivenName = GivenName;
                                //Upadating user login id in local database.
                                rememberPwd.UpdatePWd(UpdateDb, Settings.userLoginID);
                                Settings.CanCallForSettings = false;
                                Navigation.PopToRootAsync(false);
                            }

                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "UpdateProfile method -> in ProfileSelectionViewModel.cs " + Settings.userLoginID);
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

        private List<Alllabeslvalues> verfieldsforID { get; set; }

        private List<CompareModel> _JobPrintFields { get; set; }
        public List<CompareModel> JobPrintFields
        {
            get => _JobPrintFields;
            set
            {
                _JobPrintFields = value;
                NotifyPropertyChanged("JobPrintFields");
            }
        }
        
        private List<CompareModel> _PolyPrintFields { get; set; }
        public List<CompareModel> PolyPrintFields
        {
            get => _PolyPrintFields;
            set
            {
                _PolyPrintFields = value;
                NotifyPropertyChanged("PolyPrintFields");
            }
        }



        private Color _JobPrintFieldBorderColor = Color.Transparent;
        public Color JobPrintFieldBorderColor
        {
            get => _JobPrintFieldBorderColor;
            set
            {
                _JobPrintFieldBorderColor = value;
                NotifyPropertyChanged("JobPrintFieldBorderColor");
            }
        }

         private Color _PolyPrintFieldBorderColor = Color.Transparent;
        public Color PolyPrintFieldBorderColor
        {
            get => _PolyPrintFieldBorderColor;
            set
            {
                _PolyPrintFieldBorderColor = value;
                NotifyPropertyChanged("PolyPrintFieldBorderColor");
            }
        }

        private bool _IsAllSelected;
        public bool IsAllSelected
        {
            get => _IsAllSelected;
            set
            {
                _IsAllSelected = value;
                NotifyPropertyChanged("IsAllSelected");
            }
        }
        
        private bool _PolyAllSelected;
        public bool PolyAllSelected
        {
            get => _PolyAllSelected;
            set
            {
                _PolyAllSelected = value;
                NotifyPropertyChanged("PolyAllSelected");
            }
        }

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
        
        private Color _JobprintFieldTextColor = Settings.Bar_Background;
        public Color JobprintFieldTextColor
        {
            get { return _JobprintFieldTextColor; }
            set
            {
                _JobprintFieldTextColor = value;
                RaisePropertyChanged("JobprintFieldTextColor");
            }
        }
        private Color _PolyprintFieldTextColor = Settings.Bar_Background;
        public Color PolyprintFieldTextColor
        {
            get { return _PolyprintFieldTextColor; }
            set
            {
                _PolyprintFieldTextColor = value;
                RaisePropertyChanged("PolyprintFieldTextColor");
            }
        }

        bool _profileVisibility = true;
        public bool profileVisibility
        {
            get { return _profileVisibility; }
            set
            {
                _profileVisibility = value;
                NotifyPropertyChanged();
            }
        }
        
        bool _JobprintfieldVisibility = true;
        public bool JobprintfieldVisibility
        {
            get { return _JobprintfieldVisibility; }
            set
            {
                _JobprintfieldVisibility = value;
                NotifyPropertyChanged();
            }
        }
        
        bool _PolyprintfieldVisibility = true;
        public bool PolyprintfieldVisibility
        {
            get { return _PolyprintfieldVisibility; }
            set
            {
                _PolyprintfieldVisibility = value;
                NotifyPropertyChanged();
            }
        }
        bool _settingsVisibility = true;
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

        string _DefaultSettinglabel;
        public string DefaultSettinglabel
        {
            get { return _DefaultSettinglabel; }
            set
            {
                _DefaultSettinglabel = value;
                NotifyPropertyChanged();
            }
        }

        string _UpdateProfilelabel;
        public string UpdateProfilelabel
        {
            get { return _UpdateProfilelabel; }
            set
            {
                _UpdateProfilelabel = value;
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
        
        private string _SaveBtn = "Save";
        public string SaveBtn
        {
            get { return _SaveBtn; }
            set
            {
                _SaveBtn = value;
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
