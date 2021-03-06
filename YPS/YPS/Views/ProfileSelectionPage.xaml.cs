using Syncfusion.XForms.ComboBox;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;
using System.Threading.Tasks;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileSelectionPage : ContentPage
    {
        #region Data member declaration
        ProfileSelectionViewModel Vm;
        YPSService service;
        bool DefaultSettingSupplier;
        string TabName = "";
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ProfileSelectionPage()
        {
            try
            {
                InitializeComponent();

                YPSLogger.TrackEvent("ProfileSelectionPage.xaml.cs", " ProfileSelectionPage constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ProfileSelectionPage";
                Settings.PerviousPage = "ProfileSelectionPage";
                BindingContext = Vm = new ProfileSelectionViewModel(Navigation, 3);
                UpdatePBtn.IsEnabled = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ProfileSelectionPage constructor -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="pagetype"></param>
        public ProfileSelectionPage(int pagetype)
        {
            try
            {
                InitializeComponent();

                YPSLogger.TrackEvent("ProfileSelectionPage.xaml.cs", " ProfileSelectionPage constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ProfileSelectionPage";
                Settings.PerviousPage = "ProfileSelectionPage";
                BindingContext = Vm = new ProfileSelectionViewModel(Navigation, pagetype);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ProfileSelectionPage constructor -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
            }
        }
        
        public ProfileSelectionPage(string tabName)
        {
            try
            {
                InitializeComponent();

                YPSLogger.TrackEvent("ProfileSelectionPage.xaml.cs", " ProfileSelectionPage constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ProfileSelectionPage";
                Settings.PerviousPage = "ProfileSelectionPage";
                BindingContext = Vm = new ProfileSelectionViewModel(Navigation, 3);
                TabName = tabName;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ProfileSelectionPage constructor -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        ///  Parameterized constructor.
        /// </summary>
        /// <param name="hideUpdatePBtn"></param>
        public ProfileSelectionPage(bool hideUpdatePBtn)
        {
            try
            {
                InitializeComponent();

                Settings.currentPage = "ProfileSelectionPage";
                Settings.PerviousPage = "ProfileSelectionPage";
                BindingContext = Vm = new ProfileSelectionViewModel(Navigation, 3);
                UpdatePBtn.IsEnabled = hideUpdatePBtn;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ProfileSelectionPage hideUpdatePBtn -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method is to show project company picker's data, which allows to choose any one company.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PkrCompany_Tapped(object sender, EventArgs e)
        {
            pkrCompany.Focus();
        }

        /// <summary>
        /// This method is to show project project picker's data, which allows to choose any one project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PkrProject_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (Vm.CompanyName.ToLower().Trim() == "please select company")
                {
                    await App.Current.MainPage.DisplayAlert("Select", "Please select project.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please select company.");
                }
                else if (Vm.ProjectList.Count == 0)
                {
                    await App.Current.MainPage.DisplayAlert("Select", "Not found any project, please select another company.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Not found any project, please select another company.");
                }
                else
                {
                    pkrProject.Focus();
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "PkrProject_Tapped method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// This method is to show project picker's data, which allows to choose any one project.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void PkrJob_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (Vm.ProjectName.ToLower().Trim() == "please select project")
                {
                    await App.Current.MainPage.DisplayAlert("Select", "Please select project.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please select project.");
                }
                else if (Vm.JobList.Count == 0)
                {
                    await App.Current.MainPage.DisplayAlert("Select", "Not found any job, please select another project.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Not found any job, please select another project.");
                }
                else
                {
                    pkrJob.Focus();
                }

            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "PkrJob_Tapped method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// Gets called whenever the user has selected any one company from company picker. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ComapanyList_IndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (pkrCompany.SelectedIndex != -1)
                {
                    var picker = sender as SfComboBox;
                    var selectedItem = picker.SelectedItem as DDLmaster;

                    if (selectedItem != null)
                    {
                        Vm.CompanyName = Settings.CompanySelected = selectedItem.Name;

                        Settings.CompanyID = selectedItem.ID;
                        Vm.ProjectList = Vm.PDefaultSettingModel.data.Project.Where(x => x.ParentID == selectedItem.ID).ToList();

                        Vm.ProjectName = "Please select project";
                        Vm.JobName = "Please select job";
                    }
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "ComapanyList_IndexChanged method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// This method is used to get and set Labels dynamically for "Default Settings" and "Update Profile" Tabs.
        /// </summary>
        /// <param name="parentid"></param>
        public async void GetAllLabelData(int parentid)
        {
            try
            {
                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    DefaultSettingSupplier = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "DefaultSettingSupplier".Trim()).FirstOrDefault()) != null ? true : false;
                }

                // Getting all Label values based on the language Id and version Id from the settings page. 
                var data = Settings.alllabeslvalues.Where(x => x.VersionID == parentid && x.LanguageID == Settings.LanguageID).ToList();

                //Setting all the Labels dynamically for "Default Settings" and "Update Profile" Tabs.
                Vm.DefaultSettinglabel = data.Where(x => x.FieldID.Trim().ToLower().Replace(" ", string.Empty) == Settings.DefaultSettinglabel.Trim().ToLower().Replace(" ", string.Empty)).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Company" + " *").FirstOrDefault();
                Vm.UpdateProfilelabel = data.Where(x => x.FieldID.Trim().ToLower().Replace(" ", string.Empty) == Settings.UpdateProfilelabel.Trim().ToLower().Replace(" ", string.Empty)).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Company" + " *").FirstOrDefault();
                Vm.Companylabel = data.Where(x => x.FieldID.Trim().ToLower() == Settings.Companylabel1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Company" + " *").FirstOrDefault();
                Vm.projectlabel = data.Where(x => x.FieldID.Trim().ToLower() == Settings.projectlabel1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Project" + " *").FirstOrDefault();
                Vm.joblabel = data.Where(x => x.FieldID.Trim().ToLower() == Settings.joblabel1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Job" + " *").FirstOrDefault();
                Vm.SetAsDefaultBtn = data.Where(x => x.FieldID.Trim().ToLower() == Settings.SetAsDefaultBtn1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText : "Set As Default").FirstOrDefault();
                Vm.EmailLbl = data.Where(X => X.FieldID.Trim().ToLower() == Settings.Emaillabel1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Email" + " *").FirstOrDefault();
                Vm.GivenNameLbl = data.Where(X => X.FieldID.Trim().ToLower() == Settings.GivenNamelabel1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Given Name" + " *").FirstOrDefault();
                Vm.FamilyNameLbl = data.Where(X => X.FieldID.Trim().ToLower() == Settings.FamilyNamelabel1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText : "Family Name").FirstOrDefault();
                Vm.TimeZoneLbl = data.Where(X => X.FieldID.Trim().ToLower() == Settings.TimeZonelabel1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Time Zone" + " *").FirstOrDefault();
                Vm.LangaugeLbl = data.Where(X => X.FieldID.Trim().ToLower() == Settings.Languagelabel1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Language" + " *").FirstOrDefault();
                Vm.UpdateBtn = data.Where(X => X.FieldID.Trim().ToLower() == Settings.UpdateBtn1.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText : "Update").FirstOrDefault();
                Vm.LoginLbl = data.Where(x => x.FieldID.Trim().ToLower() == Vm.LoginLbl.Trim().ToLower()).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Login ID *").FirstOrDefault();
                //var supplierstatus = data.Where(wr => wr.FieldID.Trim().ToLower() == Settings.supplierlabel1.Trim().ToLower()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "GetAllLabelData method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// Gets called whenever the user has selected any one project from project picker. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ProjectList_IndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (pkrProject.SelectedIndex != -1)
                {
                    var picker = sender as SfComboBox;
                    var selectedItem = (DDLmaster)picker.SelectedItem;

                    if (selectedItem != null)
                    {
                        Vm.ProjectName = Settings.ProjectSelected = selectedItem.Name;
                        Settings.ProjectID = selectedItem.ID;
                        Vm.JobList = Vm.PDefaultSettingModel.data.Job.Where(x => x.ParentID == selectedItem.ID).ToList();
                        Vm.JobName = "Please select job";
                    }
                }
            }
            catch (Exception ex)

            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "ProjectList_IndexChanged method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// Gets called whenever the user has selected any one job from job picker. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void JobList_IndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (pkrJob.SelectedIndex != -1)
                {
                    var picker = sender as SfComboBox;
                    var selectedItem = (DDLmaster)picker.SelectedItem;

                    if (selectedItem != null)
                    {
                        Vm.JobName = Settings.JobSelected = selectedItem.Name;
                        Settings.JobID = selectedItem.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "JobList_IndexChanged method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// TimeZone_Tapped, This method is used to show Popup with List of Time Zone.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeZone_Tapped(object sender, EventArgs e)
        {
            try
            {
                SearchLoc.Text = "";
                TimeZoneCntntView.IsVisible = true;
                ListViewTimeZ.ItemsSource = Vm.TimeZone;
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "TimeZone_Tapped  method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called while entering text into TimeZone search field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.NewTextValue))
                {
                    ListViewTimeZ.ItemsSource = Vm.TimeZone;
                }
                else
                {
                    ListViewTimeZ.ItemsSource = Vm.TimeZone.Where(item => item.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "SearchBar_TextChanged  method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// ListView_ItemTapped, Select the time zone from the list view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            TimeZoneCntntView.IsVisible = false;
            try
            {
                ListViewTimeZ.SelectedItem = null;
                Vm.TimeZoneTextDisplay = e.Item.ToString();
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "ListView_ItemTapped  method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// Shows language picker.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapOnLanguagePicker(object sender, EventArgs e)
        {
            try
            {
                LanguagePicker.Focus();
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "TapOnLanguagePicker  method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called whenever the user has selected any one language from language picker. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LanguagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (LanguagePicker.SelectedIndex != -1)
                {
                    Vm.LangaugeTextDisplay = LanguagePicker.DataSource.ToArray()[LanguagePicker.SelectedIndex].ToString();

                    if (Vm.LangaugeTextDisplay.ToLower().Trim() == "please select")
                    {
                        Vm.LangaugeIDLocal = 0; // 0 means english language.
                    }
                    else
                    {
                        var items = Vm.ListOfLanguage.Where(X => X.Name == Vm.LangaugeTextDisplay).FirstOrDefault();

                        Vm.LangaugeIDLocal = items.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "LanguagePicker_SelectedIndexChanged  method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// Gets called whenever user has clicked on close button in Time zone Popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeZoneClosePopUp(object sender, EventArgs e)
        {
            try
            {
                TimeZoneCntntView.IsVisible = false;
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "TimeZoneClosePopUp method -> in ProfileSelectionPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        private void JobPrintFieldsCheckChanged(object sender, EventArgs e)
        {
            try
            {
                var checkbox = sender as Plugin.InputKit.Shared.Controls.CheckBox;

                if (Vm.JobPrintFieldBorderColor == Color.Red && checkbox.IsChecked)

                {
                    Vm.JobPrintFieldBorderColor = Color.Transparent;
                }

                if (checkbox.StyleId == "selectall")
                {
                    if (Vm?.JobPrintFields?.All(al => al.Status == 1) == true)
                    {
                        Vm?.JobPrintFields.ToList().ForEach(fe => { fe.Status = 0; });
                    }
                    else
                    {
                        Vm?.JobPrintFields.ToList().ForEach(fe => { fe.Status = 1; });
                    }
                }
                else if (checkbox.StyleId == "singleselect")
                {
                    if (Vm?.JobPrintFields?.All(al => al.Status == 1) == true)
                    {
                        Vm.IsAllSelected = true;
                    }
                    else
                    {
                        Vm.IsAllSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "JobPrintFieldsCheckChanged method -> in PlayBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        
        private async void SettingsTapped(object sender, EventArgs e)
        {
            try
            {
                TabName = "";
                ResetToDefault();
                Vm.settingsTextColor = Settings.Bar_Background;
                Vm.settingsVisibility = true;
                await scrollview.ScrollToAsync(settingStack, ScrollToPosition.Start, false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SettingsTapped method -> in PlayBox.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private async void ProfileTapped(object sender, EventArgs e)
        {
            try
            {
                TabName = "";
                ResetToDefault();
                Vm.profileTextColor = Settings.Bar_Background;
                Vm.profileVisibility = true;
                await scrollview.ScrollToAsync(profileStack, ScrollToPosition.Center, false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ProfileTapped method -> in PlayBox.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

       
        private async void JobPrintFieldTapped(object sender, EventArgs e)
        {
            try
            {
                TabName = "";
                if(Vm.PrintApiStatus == false)
                {
                    Vm.IndicatorVisibility = true;
                    Task.Run(() => Vm.GetPrintFieldData()).Wait();
                }
                await scrollview.ScrollToAsync(jobprintfieldStack, ScrollToPosition.Center, false);
                ResetToDefault();
                Vm.JobprintFieldTextColor = Settings.Bar_Background;
                Vm.JobprintfieldVisibility  = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "JobPrintFieldTapped method -> in PlayBox.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                Vm.IndicatorVisibility = false;
            }
        }
        
        public async void PolyPrintFieldTapped(object sender, EventArgs e)
        {
            try
            {
                TabName = "PolyboxPrintSettings";
                if (Vm.PrintApiStatus == false)
                {
                    Vm.IndicatorVisibility = true;
                    Task.Run(() => Vm.GetPrintFieldData()).Wait();
                }
                await scrollview.ScrollToAsync(polyprintfieldStack, ScrollToPosition.End, false);
                ResetToDefault();
                Vm.PolyprintFieldTextColor = Settings.Bar_Background;
                Vm.PolyprintfieldVisibility = true;
                
            }
            catch(Exception ex)
            {
                YPSLogger.ReportException(ex, "PolyPrintFieldTapped method -> in PlayBox.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                Vm.IndicatorVisibility = false;
            }
        }

        void ResetToDefault()
        {
            Vm.PolyprintFieldTextColor = Vm.JobprintFieldTextColor = Vm.settingsTextColor = Vm.profileTextColor = Color.Black;
            Vm.PolyprintfieldVisibility = Vm.JobprintfieldVisibility = Vm.settingsVisibility = Vm.profileVisibility = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (TabName == "PolyboxPrintSettings")
            {
                PolyPrintFieldTapped(null, null);
            }
        }
        private void PolyPrintFieldsCheckChanged(object sender, EventArgs e)
        {
            try
            {
                var checkbox = sender as Plugin.InputKit.Shared.Controls.CheckBox;

                if (Vm.PolyPrintFieldBorderColor == Color.Red && checkbox.IsChecked)

                {
                    Vm.PolyPrintFieldBorderColor = Color.Transparent;
                }

                if (checkbox.StyleId == "polyselectall")
                {
                    if (Vm?.PolyPrintFields?.All(al => al.Status == 1) == true)
                    {
                        Vm?.PolyPrintFields.ToList().ForEach(fe => { fe.Status = 0; });
                    }
                    else
                    {
                        Vm?.PolyPrintFields.ToList().ForEach(fe => { fe.Status = 1; });
                    }
                }
                else if (checkbox.StyleId == "polysingleselect")
                {
                    if (Vm?.PolyPrintFields?.All(al => al.Status == 1) == true)
                    {
                        Vm.PolyAllSelected = true;
                    }
                    else
                    {
                        Vm.PolyAllSelected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PolyPrintFieldsCheckChanged method -> in PlayBox.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
    }
}