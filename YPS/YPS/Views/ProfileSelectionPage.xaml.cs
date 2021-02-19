using Syncfusion.XForms.ComboBox;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfileSelectionPage : ContentPage
    {
        #region Data member declaration
        ProfileSelectionViewModel Vm;
        YPSService service;
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ProfileSelectionPage()
        {
            try
            {
                InitializeComponent();
                YPSLogger.TrackEvent("ProfileSelectionPage", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ProfileSelectionPage";
                Settings.PerviousPage = "ProfileSelectionPage";
                BindingContext = Vm = new ProfileSelectionViewModel(Navigation, 3);
                UpdatePBtn.IsEnabled = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ProfileSelectionPage -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.TrackEvent("ProfileSelectionPage", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "ProfileSelectionPage";
                Settings.PerviousPage = "ProfileSelectionPage";
                BindingContext = Vm = new ProfileSelectionViewModel(Navigation, pagetype);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ProfileSelectionPage pagetype -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "ProfileSelectionPage hideUpdatePBtn -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                    DependencyService.Get<IToastMessage>().ShortAlert("Please select company.");

                else if (Vm.ProjectList.Count == 0)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Not found any project, please select another company.");
                }
                else
                {
                    pkrProject.Focus();
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "PkrProject_Tapped method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                    DependencyService.Get<IToastMessage>().ShortAlert("Please select project.");
                }
                else if (Vm.JobList.Count == 0)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Not found any job, please select another project.");
                }
                else
                {
                    pkrJob.Focus();
                }

            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "PkrJob_Tapped method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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

                        Settings.VersionID = selectedItem.ParentID;

                        if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                        {
                            GetAllLabelData(selectedItem.ParentID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "ComapanyList_IndexChanged method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                // Getting all Label values based on the language Id and version Id from the settings page. 
                var data = Settings.alllabeslvalues.Where(x => x.VersionID == parentid && x.LanguageID == Settings.LanguageID).ToList();

                //Setting all the Labels dynamically for "Default Settings" and "Update Profile" Tabs.
                Vm.Companylabel = data.Where(x => x.FieldID == Settings.Companylabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Company" + " *").FirstOrDefault();
                Vm.projectlabel = data.Where(x => x.FieldID == Settings.projectlabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Project" + " *").FirstOrDefault();
                Vm.joblabel = data.Where(x => x.FieldID == Settings.joblabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Job" + " *").FirstOrDefault();
                Vm.supplierlabel = data.Where(x => x.FieldID == Settings.supplierlabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText : "Supplier").FirstOrDefault();
                Vm.SetAsDefaultBtn = data.Where(x => x.FieldID == Settings.SetAsDefaultBtn1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText : "Set As Default").FirstOrDefault();
                Vm.EmailLbl = data.Where(X => X.FieldID == Settings.Emaillabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Email" + " *").FirstOrDefault();
                Vm.GivenNameLbl = data.Where(X => X.FieldID == Settings.GivenNamelabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Given Name" + " *").FirstOrDefault();
                Vm.FamilyNameLbl = data.Where(X => X.FieldID == Settings.FamilyNamelabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText : "Family Name").FirstOrDefault();
                Vm.TimeZoneLbl = data.Where(X => X.FieldID == Settings.TimeZonelabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Time Zone" + " *").FirstOrDefault();
                Vm.LangaugeLbl = data.Where(X => X.FieldID == Settings.Languagelabel1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Language" + " *").FirstOrDefault();
                Vm.UpdateBtn = data.Where(X => X.FieldID == Settings.UpdateBtn1).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText : "Update").FirstOrDefault();
                Vm.LoginLbl = data.Where(x => x.FieldID == Vm.LoginLbl).Select(m => !string.IsNullOrEmpty(m.LblText) ? m.LblText + " *" : "Login ID *").FirstOrDefault();
                var supplierstatus = data.Where(wr => wr.FieldID == Settings.supplierlabel1).FirstOrDefault();

                if (Settings.userRoleID == (int)UserRoles.SupplierAdmin || Settings.userRoleID == (int)UserRoles.SupplierUser)
                {
                    Vm.SupplierLabelAndFrame = false;
                    Settings.SupplierID = 0;
                    Vm.SupplierGridRowHeight = 0;
                }
                else if (supplierstatus.Status == 0) // if supplier status is in-active
                {
                    Vm.SupplierLabelAndFrame = false;
                    Settings.SupplierID = 0;
                    Vm.SupplierGridRowHeight = 0;
                }
                else
                {
                    Vm.SupplierLabelAndFrame = true;
                }

            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "GetAllLabelData method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "ProjectList_IndexChanged method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "JobList_IndexChanged method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "SearchBar_TextChanged  method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "SearchBar_TextChanged  method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "ListView_ItemTapped  method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "TapOnLanguagePicker  method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "LanguagePicker_SelectedIndexChanged  method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "TimeZoneClosePopUp method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is used to show a list of suppliers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Supplier_Tapped(object sender, EventArgs e)
        {
            try
            {
                SupplierSrch.Text = "";
                SupplierCntntView.IsVisible = true;
                ListViewSupplier.ItemsSource = Vm.SupplierList;
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "Supplier_Tapped method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }

        /// <summary>
        /// This method is used to search for the suppliers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SupplierSrch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(e.NewTextValue))
                {
                    ListViewSupplier.ItemsSource = Vm.SupplierList;
                }
                else
                {
                    ListViewSupplier.ItemsSource = Vm.SupplierList.Where(item => item.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "SupplierSrch_TextChanged  method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called whenever user has clicked on close button in supplier Popup.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SupplierClosePopUp(object sender, EventArgs e)
        {
            try
            {
                SupplierCntntView.IsVisible = false;
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "SupplierClosePopUp  method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called whenever user has select supplier from the search supplier list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListViewSupplier_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            SupplierCntntView.IsVisible = false;
            try
            {
                ListViewSupplier.SelectedItem = null;
                Settings.SupplierSelected = Vm.SupplierName = e.Item.ToString();

                if (Vm.SupplierName == "ALL")
                {
                    Settings.SupplierID = 0; // 0 means all suppliers
                }
                else
                {
                    var filterJobtData = Vm.PDefaultSettingModel.data.Supplier.Where(x => x.Name == Vm.SupplierName).FirstOrDefault();

                    if (filterJobtData != null)
                    {
                        Settings.SupplierID = filterJobtData.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                service = new YPSService();
                YPSLogger.ReportException(ex, "ListViewSupplier_ItemTapped  method -> in ProfileSelectionPage.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                service = null;
            }
        }
    }
}