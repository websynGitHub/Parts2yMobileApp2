using Acr.UserDialogs;
using Newtonsoft.Json;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomRenders;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.Views;
using static YPS.Model.SearchModel;

namespace YPS.ViewModel
{
    public class FilterDataViewModel : IBase
    {


        #region ICommands and variables declaration.
        public INavigation Navigation { get; set; }
        public YPSService service;
        private NullableDatePicker myNullableDate;
        bool checkInternet;

        public ICommand disc_PickerCommand { set; get; }
        public ICommand eLevel_PickerCommand { set; get; }
        public ICommand Condition_PickerCommand { set; get; }
        public ICommand Expeditor_PickerCommand { set; get; }
        public ICommand Priority_PickerCommand { set; get; }
        public ICommand resetCommand { set; get; }
        public ICommand applyCommand { set; get; }
        public ICommand SaveAndSearchCmd { set; get; }
        public ICommand keyTabCommand { set; get; }
        #endregion

        #region Properties for dynamic label change
        public class filtterlabelclass
        {
            public filtterlabelFields PO { get; set; } = new filtterlabelFields();
            public filtterlabelFields REQNo { get; set; } = new filtterlabelFields();
            public filtterlabelFields ShippingNumber { get; set; } = new filtterlabelFields();
            public filtterlabelFields DisciplineName { get; set; } = new filtterlabelFields();
            public filtterlabelFields ELevelName { get; set; } = new filtterlabelFields();
            public filtterlabelFields Condition { get; set; } = new filtterlabelFields();
            public filtterlabelFields Expeditor { get; set; } = new filtterlabelFields();
            public filtterlabelFields PriorityName { get; set; } = new filtterlabelFields();
            public filtterlabelFields TagNumber { get; set; } = new filtterlabelFields();
            public filtterlabelFields IdentCode { get; set; } = new filtterlabelFields();
            public filtterlabelFields BagNumber { get; set; } = new filtterlabelFields();
            public filtterlabelFields yBkgNumber { get; set; } = new filtterlabelFields();
            public filtterlabelFields JobName { get; set; } = new filtterlabelFields();
            public filtterlabelFields ResourceName { get; set; } = new filtterlabelFields() { Name = "Resource", Status = true };
            public filtterlabelFields ResetBtn { get; set; } = new filtterlabelFields();
            public filtterlabelFields SearchBtn { get; set; } = new filtterlabelFields();
            public filtterlabelFields SaveSearchBtn { get; set; } = new filtterlabelFields() { Name = "Save & Search", Status = true };

        }
        public class filtterlabelFields : IBase
        {
            public bool Status { get; set; }
            public string Name { get; set; }
        }

        public filtterlabelclass _labelobj;
        public filtterlabelclass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
            }
        }


        #endregion
        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="_Navigation"></param>
        /// <param name="page"></param>
        public FilterDataViewModel(INavigation _Navigation, Page page)
        {
            YPSLogger.TrackEvent("FilterDataViewModel", "Page FilterDataViewModel method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Navigation = _Navigation;
                IndicatorVisibility = true;
                service = new YPSService();

                page.Appearing += Page_Appearing;

                #region Command binding for showing pickers
                disc_PickerCommand = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                eLevel_PickerCommand = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                Condition_PickerCommand = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                Expeditor_PickerCommand = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                Priority_PickerCommand = new Command<View>((view) =>
                {
                    view?.Focus();
                });
                #endregion

                #region BInding tab & click event methods to respective ICommand properties
                applyCommand = new Command(ApplyFilter);
                SaveAndSearchCmd = new Command(ApplyFilter);
                resetCommand = new Command(async () => await ResetFilter());
                keyTabCommand = new Command(async () => await KeyTabClick());
                #endregion

                ChangeLabel();
                HeaderFilterData();
                Task.Run(() => GetSavedUserSearchSettings());
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "FilterDataViewModel constructor -> in FilterDataViewModel " + Settings.userLoginID);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async Task GetSavedUserSearchSettings()
        {
            try
            {
                SearchFilterList = new List<SearchPassData>();
                SearchFilterList.Add(new SearchPassData() { Name = "New", ID = 0 });

                var result = await service.GetSavedUserSearchSettings();

                if (result != null && result.data != null)
                {

                    SearchFilterList.AddRange(result.data.Where(wr => !string.IsNullOrEmpty(wr.Name)));

                    SelectedFilterName = SearchFilterList?.Where(wr => wr.Status == 1).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetSavedUserSearchSettings method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
        /// <summary>
        /// Gets called when an item is selected from Filter DropDownList
        /// </summary>
        public async void FilterSelected()
        {
            try
            {
                if (SelectedFilterName != null)
                {
                    FilterName = (IsNewSaveSearchEntryVisible = SelectedFilterName.Name.Trim().ToLower() == "new" ? true : false) == false ? SelectedFilterName.Name : string.Empty;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FilterSelected method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when page is loaded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Page_Appearing(object sender, EventArgs e)
        {
            try
            {
                Settings.filterPageCount = 0;
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "Page_Appearing method -> in FilterDataViewModel " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Search" button.
        /// </summary>
        /// <returns></returns>
        private async void ApplyFilter(object sender)
        {
            try
            {
                YPSLogger.TrackEvent("FilterDataViewModel", "in applyClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);
                await Task.Delay(50);
                UserDialogs.Instance.ShowLoading("Loading...");

                var val = sender as SfButton;
                Settings.IsSearchClicked = val.StyleId.Trim().ToLower() == "search".Trim().ToLower() ? true : false;

                if (val.StyleId.Trim().ToLower() != "searchfilter".Trim().ToLower() ||
                    (val.StyleId.Trim().ToLower() == "searchfilter".Trim().ToLower() && SelectedFilterName != null &&
                    !string.IsNullOrEmpty(FilterName)))
                {
                    //Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {

                        SendPodata SaveUserDS = new SendPodata();
                        Settings.UserID = Settings.userLoginID;

                        //Getting the entered feild values from Key tab
                        SaveUserDS.PONumber = Settings.PONumber = poNumber;
                        SaveUserDS.REQNo = Settings.REQNo = reqNumber;
                        SaveUserDS.ShippingNo = Settings.ShippingNo = shipNumber;
                        SaveUserDS.DisciplineID = Settings.DisciplineID;
                        SaveUserDS.ELevelID = Settings.ELevelID;
                        SaveUserDS.ConditionID = Settings.ConditionID;
                        SaveUserDS.ExpeditorID = Settings.ExpeditorID;
                        SaveUserDS.PriorityID = Settings.PriorityID;
                        SaveUserDS.ResourceID = Settings.ResourceID;
                        SaveUserDS.TagNo = Settings.TAGNo = tagNumber;
                        SaveUserDS.IdentCode = Settings.IdentCodeNo = Identcode;
                        SaveUserDS.BagNo = Settings.BagNo = BagNumber;
                        SaveUserDS.yBkgNumber = Settings.Ybkgnumber = YbkgNumber;
                        SaveUserDS.TaskName = Settings.TaskName = JobName;

                        //Save the filter field values to DB
                        SearchPassData defaultData = new SearchPassData();
                        defaultData.UserID = Settings.userLoginID;
                        defaultData.CompanyID = Settings.CompanyID;
                        defaultData.ProjectID = Settings.ProjectID;
                        defaultData.JobID = Settings.JobID;
                        defaultData.SearchName = val.StyleId.Trim().ToLower() != "search".Trim().ToLower() ? FilterName : null;
                        //defaultData.IsCurrentSearch = true;
                        defaultData.ID = defaultData.SearchName?.Trim().ToLower() == "new" || val.StyleId.Trim().ToLower() == "search".Trim().ToLower() ? 0 : SelectedFilterName.ID;
                        defaultData.SearchCriteria = JsonConvert.SerializeObject(SaveUserDS);

                        if (val.StyleId.Trim().ToLower() != "search".Trim().ToLower())
                        {
                            var responseData = await service.SaveSerchvaluesSetting(defaultData);
                        }

                        Settings.IsFilterreset = true;
                        FilterName = string.Empty;

                        //App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                else
                {
                    SearchFilterDDlHasError = SelectedFilterName == null ? true : false;
                    SearchFilterEntryHasError = string.IsNullOrEmpty(FilterName) ? true : false;
                }

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ApplyFilter method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
                Settings.ShowSuccessAlert = true;
            }
        }

        /// <summary>
        /// Gets called when clicked on "Reset" button.
        /// </summary>
        private async Task ResetFilter()
        {
            try
            {
                YPSLogger.TrackEvent("FilterDataViewModel", "in applyClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);
                await Task.Delay(50);
                UserDialogs.Instance.ShowLoading("Loading...");

                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                Settings.IsFilterreset = true;

                if (checkInternet)
                {
                    SendPodata SaveUserDS = new SendPodata();
                    SearchPassData defaultData = new SearchPassData();
                    defaultData.CompanyID = Settings.CompanyID;
                    defaultData.UserID = Settings.userLoginID;
                    defaultData.CompanyID = Settings.CompanyID;
                    defaultData.ProjectID = Settings.ProjectID;
                    defaultData.JobID = Settings.JobID;
                    //defaultData.SearchName = FilterName;
                    ////defaultData.IsCurrentSearch = true;
                    //defaultData.ID = defaultData.SearchName.Trim().ToLower() == "new" ? 0 : SelectedFilterName.ID;
                    // Setting the default values for fields in Key tab
                    SaveUserDS.PONumber = Settings.PONumber = poNumber = string.Empty;
                    SaveUserDS.REQNo = Settings.REQNo = reqNumber = string.Empty;
                    SaveUserDS.ShippingNo = Settings.ShippingNo = shipNumber = string.Empty;
                    SaveUserDS.DisciplineID = Settings.DisciplineID = 0;
                    DisciplineDefaultValue = "ALL";
                    SaveUserDS.ResourceID = Settings.ResourceID = 0;
                    ResourceDefaultValue = "ALL";
                    SaveUserDS.ELevelID = Settings.ELevelID = 0;
                    ELevelDefaultValue = "ALL";
                    SaveUserDS.ConditionID = Settings.ConditionID = 0;
                    ConditionDefaultValue = "ALL";
                    SaveUserDS.ExpeditorID = Settings.ExpeditorID = 0;
                    ExpeditorDefaultValue = "ALL";
                    SaveUserDS.PriorityID = Settings.PriorityID = 0;
                    PriorityDefaultValue = "ALL";
                    SaveUserDS.TagNo = Settings.TAGNo = tagNumber = string.Empty;
                    SaveUserDS.IdentCode = Settings.IdentCodeNo = Identcode = string.Empty;
                    SaveUserDS.BagNo = Settings.BagNo = BagNumber = string.Empty;
                    SaveUserDS.yBkgNumber = Settings.Ybkgnumber = YbkgNumber = string.Empty;
                    SaveUserDS.yBkgNumber = Settings.Ybkgnumber = YbkgNumber = string.Empty;
                    SaveUserDS.TaskName = Settings.TaskName = JobName = string.Empty;

                    //Saving all the filter fields with default value into the DB
                    defaultData.SearchCriteria = JsonConvert.SerializeObject(SaveUserDS);
                    var responseData = await service.SaveSerchvaluesSetting(defaultData);

                    Settings.refreshPage = 1;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ResetFilter method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// Gets called when clicked on "Key" tab.
        /// </summary>
        /// <returns></returns>
        private async Task KeyTabClick()
        {
            try
            {
                headerVisibility = true;
                countryVisibility = false;
                headerTextColor = countryTextColor = Color.White;
                headerbox = true;
                headerBgColor = Color.FromHex("#269DC9");
                countryBgColor = Color.LightBlue;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "KeyTabClick method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is used to get the entered/selected field values from DB
        /// </summary>
        public async void Searchdatapicker()
        {
            try
            {
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var filterData = await service.GetHeaderFilterDataService();

                    if (filterData != null)
                    {
                        if (filterData.status != 0)
                        {
                            Settings.alldropdownvalues = filterData.data;
                            HeaderFilterData();
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Searchdatapicker method-> in datechanged event " + Settings.userLoginID);
                await service.Handleexception(ex);
            }

        }

        /// <summary>
        /// This is for binding the existing entered/selected filed values to their respective fields
        /// Fires during the page loding
        /// </summary>
        private async void HeaderFilterData()
        {
            try
            {
                IndicatorVisibility = true;

                if (Settings.alldropdownvalues != null)
                {
                    var list = Settings.alldropdownvalues.Discipline.Where(x => x.ParentID == Settings.CompanyID).ToList();
                    DisciplineList = new List<DDLmaster>();
                    DisciplineList.Add(new DDLmaster() { Name = "ALL", ID = 0 });
                    DisciplineList.AddRange(list);
                    DisciplineNames = DisciplineList.Select(x => x.Name).ToList();

                    var elevllist = Settings.alldropdownvalues.ELevel.Where(x => x.ParentID == Settings.CompanyID).ToList();
                    ELevelList = new List<DDLmaster>();
                    ELevelList.Add(new DDLmaster() { Name = "ALL", ID = 0 });
                    ELevelList.AddRange(elevllist);
                    ELevelNames = ELevelList.Select(x => x.Name).ToList();

                    var prioritylist = Settings.alldropdownvalues.Priority.Where(x => x.ParentID == Settings.CompanyID).ToList();
                    PriorityList = new List<DDLmaster>();
                    PriorityList.Add(new DDLmaster() { Name = "ALL", ID = 0 });
                    PriorityList.AddRange(prioritylist);
                    PriorityNames = PriorityList.Select(x => x.Name).ToList();

                    var expeditorlist = Settings.alldropdownvalues.Expeditor.Where(x => x.ParentID == Settings.CompanyID).ToList();
                    Expeditorlist = new List<DDLmaster>();
                    Expeditorlist.Add(new DDLmaster() { Name = "ALL", ID = 0 });
                    Expeditorlist.AddRange(expeditorlist);
                    ExpeditorNames = Expeditorlist.Select(x => x.Name).ToList();

                    var conditionlist = Settings.alldropdownvalues.Condition.Where(x => x.ParentID == Settings.CompanyID).ToList();
                    Conditionlist = new List<DDLmaster>();
                    Conditionlist.Add(new DDLmaster() { Name = "ALL", ID = 0 });
                    Conditionlist.AddRange(conditionlist);
                    ConditionNames = Conditionlist.Select(x => x.Name).ToList();

                    //var resourcelist = Settings.alldropdownvalues.Condition.Where(x => x.ParentID == Settings.CompanyID).ToList();
                    Resourcelist = new List<DDLmaster>();
                    Resourcelist.Add(new DDLmaster() { Name = "ALL", ID = 0 });
                    Resourcelist.AddRange(Settings.alldropdownvalues.Resource.Where(x => x.ParentID == Settings.CompanyID).ToList());
                    ResourceNames = Resourcelist.Select(x => x.Name).ToList();

                    BindKeyTabValues();
                }
                else
                {
                    try
                    {
                        Searchdatapicker();
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "Inner catch block in HeaderFilterData method-> in datechanged event " + Settings.userLoginID);
                        await service.Handleexception(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HeaderFilterData method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This is for binding the existing Key tab field values
        /// </summary>
        private async void BindKeyTabValues()
        {
            try
            {
                poNumber = !(String.IsNullOrEmpty(Settings.PONumber)) ? Settings.PONumber : poNumber;
                reqNumber = !(String.IsNullOrEmpty(Settings.REQNo)) ? Settings.REQNo : reqNumber;
                shipNumber = !(String.IsNullOrEmpty(Settings.ShippingNo)) ? Settings.ShippingNo : shipNumber;
                tagNumber = !(String.IsNullOrEmpty(Settings.TAGNo)) ? Settings.TAGNo : tagNumber;
                Identcode = !(String.IsNullOrEmpty(Settings.IdentCodeNo)) ? Settings.IdentCodeNo : Identcode;
                BagNumber = !(String.IsNullOrEmpty(Settings.BagNo)) ? Settings.BagNo : BagNumber;
                YbkgNumber = !(String.IsNullOrEmpty(Settings.Ybkgnumber)) ? Settings.Ybkgnumber : YbkgNumber;
                JobName = !(String.IsNullOrEmpty(Settings.TaskName)) ? Settings.TaskName : JobName;

                if (Settings.DisciplineID != 0)
                {
                    var name = DisciplineList.Where(x => x.ID == Settings.DisciplineID).FirstOrDefault();
                    DisciplineDefaultValue = Settings.DisciplineName = name?.Name;
                }
                else
                {
                    DisciplineDefaultValue = DisciplineNames.FirstOrDefault();
                }

                if (Settings.ResourceID != 0)
                {
                    var name = Resourcelist.Where(x => x.ID == Settings.ResourceID).FirstOrDefault();
                    ResourceDefaultValue = Settings.ResourceName = name?.Name;
                }
                else
                {
                    ResourceDefaultValue = ResourceNames.FirstOrDefault();
                }

                if (Settings.ELevelID != 0)
                {
                    var name = ELevelList.Where(x => x.ID == Settings.ELevelID).FirstOrDefault();
                    ELevelDefaultValue = Settings.ELevelName = name?.Name;
                }
                else
                {
                    ELevelDefaultValue = ELevelNames.FirstOrDefault();
                }

                if (Settings.ConditionID != 0)
                {
                    var name = Conditionlist.Where(x => x.ID == Settings.ConditionID).FirstOrDefault();
                    ConditionDefaultValue = Settings.ConditionName = name?.Name;
                }
                else
                {
                    ConditionDefaultValue = ConditionNames.FirstOrDefault();
                }

                if (Settings.ExpeditorID != 0)
                {
                    var name = Expeditorlist.Where(x => x.ID == Settings.ExpeditorID).FirstOrDefault();
                    ExpeditorDefaultValue = Settings.ExpeditorName = name?.Name;
                }
                else
                {
                    ExpeditorDefaultValue = ExpeditorNames.FirstOrDefault();
                }

                if (Settings.PriorityID != 0)
                {
                    var name = PriorityList.Where(x => x.ID == Settings.PriorityID).FirstOrDefault();
                    PriorityDefaultValue = Settings.PriorityName = name?.Name;
                }
                else
                {
                    PriorityDefaultValue = PriorityNames.FirstOrDefault();
                }

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindingKeyValues method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Discipline DropDownList
        /// </summary>
        public async void SelectedDiscipline_TapEvent()
        {
            try
            {
                if (SelectedDisciplineValue != null)
                {
                    DisciplineDefaultValue = SelectedDisciplineValue;
                    var DisciplineValue = DisciplineList.Where(X => X.Name == SelectedDisciplineValue).FirstOrDefault();
                    Settings.DisciplineID = DisciplineValue.ID;
                    Settings.DisciplineName = DisciplineValue.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectedDiscipline_TapEvent method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Resource DropDownList
        /// </summary>
        public async void SelectedResource_TapEvent()
        {
            try
            {
                if (SelectedResourceValue != null)
                {
                    ResourceDefaultValue = SelectedResourceValue;
                    var ResourceValue = Resourcelist.Where(X => X.Name == ResourceDefaultValue).FirstOrDefault();
                    Settings.ResourceID = ResourceValue.ID;
                    Settings.ResourceName = ResourceValue.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectedResource_TapEvent method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from ELevel DropDownList
        /// </summary>
        public async void SelectedELevel_TapEvent()
        {
            try
            {
                if (SelectedELevelValue != null)
                {
                    ELevelDefaultValue = SelectedELevelValue;
                    var ELevelListvalue = ELevelList.Where(X => X.Name == ELevelDefaultValue).FirstOrDefault();
                    Settings.ELevelID = ELevelListvalue.ID;
                    Settings.ELevelName = ELevelListvalue.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectedELevel_TapEvent method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Condition DropDownList
        /// </summary>
        public async void SelectedCondition_TapEvent()
        {
            try
            {
                if (SelectedConditionValue != null)
                {
                    ConditionDefaultValue = SelectedConditionValue;
                    var ConditionListvalue = Conditionlist.Where(X => X.Name == ConditionDefaultValue).FirstOrDefault();
                    Settings.ConditionID = ConditionListvalue.ID;
                    Settings.ConditionName = ConditionListvalue.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectedCondition_TapEvent method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Expeditor DropDownList
        /// </summary>
        public async void SelectedExpeditor_TapEvent()
        {
            try
            {
                if (SelectedExpeditorValue != null)
                {
                    ExpeditorDefaultValue = SelectedExpeditorValue;
                    var ExpeditorListvalue = Expeditorlist.Where(X => X.Name == ExpeditorDefaultValue).FirstOrDefault();
                    Settings.ExpeditorID = ExpeditorListvalue.ID;
                    Settings.ExpeditorName = ExpeditorListvalue.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectedExpeditor_TapEvent method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Fires when an item is selected from Priority DropDownList
        /// </summary>
        public async void SelectedPriority_TapEvent()
        {
            try
            {
                if (SelectedPrioritytValue != null)
                {
                    PriorityDefaultValue = SelectedPrioritytValue;
                    var PriorityListvalue = PriorityList.Where(X => X.Name == PriorityDefaultValue).FirstOrDefault();
                    Settings.PriorityID = PriorityListvalue.ID;
                    Settings.PriorityName = PriorityListvalue.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectedPriority_TapEvent method -> in FilterDataViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is for changing the labels dynamically & field Show/Hide
        /// </summary>
        public async void ChangeLabel()
        {
            try
            {
                labelobj = new filtterlabelclass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> filteredlabel = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (filteredlabel.Count > 0)
                    {
                        //Assigning default names to the label properties for comparinf with FieldID 
                        await AssignValueForComparision();

                        // Selecting Label Text and Status 
                        var poval = filteredlabel.Where(wr => wr.FieldID == labelobj.PO.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var REQNo = filteredlabel.Where(wr => wr.FieldID == labelobj.REQNo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var ShippingNumber = filteredlabel.Where(wr => wr.FieldID == labelobj.ShippingNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var DisciplineName = filteredlabel.Where(wr => wr.FieldID == labelobj.DisciplineName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var ELevelName = filteredlabel.Where(wr => wr.FieldID == labelobj.ELevelName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var Condition = filteredlabel.Where(wr => wr.FieldID == labelobj.Condition.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var Expeditor = filteredlabel.Where(wr => wr.FieldID == labelobj.Expeditor.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var PriorityName = filteredlabel.Where(wr => wr.FieldID == labelobj.PriorityName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var TagNumber = filteredlabel.Where(wr => wr.FieldID == labelobj.TagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var IdentCode = filteredlabel.Where(wr => wr.FieldID == labelobj.IdentCode.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var BagNumber = filteredlabel.Where(wr => wr.FieldID == labelobj.BagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var ybkgnumber = filteredlabel.Where(wr => wr.FieldID == labelobj.yBkgNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var jobname = filteredlabel.Where(wr => wr.FieldID == labelobj.JobName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var resourcename = filteredlabel.Where(wr => wr.FieldID == labelobj.ResourceName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var ResetBtn = filteredlabel.Where(wr => wr.FieldID == labelobj.ResetBtn.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var SearchBtn = filteredlabel.Where(wr => wr.FieldID == labelobj.SearchBtn.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var SaveSearchBtn = filteredlabel.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.SaveSearchBtn.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Changing label & Show/Hide fields
                        labelobj.PO.Name = poval != null ? poval.LblText : "Purchase#";
                        labelobj.PO.Status = (poval == null ? true : false) || (poval != null && poval.Status == 1) ? true : false;
                        labelobj.REQNo.Name = REQNo != null ? REQNo.LblText : "REQ#";
                        labelobj.REQNo.Status = (REQNo == null ? true : false) || (REQNo != null && REQNo.Status == 1) ? true : false;
                        labelobj.ShippingNumber.Name = ShippingNumber != null ? ShippingNumber.LblText : "Shipping#";
                        labelobj.ShippingNumber.Status = (ShippingNumber == null ? true : false) || (ShippingNumber != null && ShippingNumber.Status == 1) ? true : false;
                        labelobj.DisciplineName.Name = DisciplineName != null ? DisciplineName.LblText : "Discipline";
                        labelobj.DisciplineName.Status = (DisciplineName == null ? true : false) || ((DisciplineName != null && DisciplineName.Status == 1) ? true : false);
                        labelobj.ELevelName.Name = ELevelName != null ? ELevelName.LblText : "ELeavel";
                        labelobj.ELevelName.Status = (ELevelName == null ? true : false) || (ELevelName != null && ELevelName.Status == 1) ? true : false;
                        labelobj.Condition.Name = Condition != null ? Condition.LblText : "Condition";
                        labelobj.Condition.Status = (Condition == null ? true : false) || (Condition != null && Condition.Status == 1) ? true : false;
                        labelobj.Expeditor.Name = Expeditor != null ? Expeditor.LblText : "Expeditor";
                        labelobj.Expeditor.Status = (Expeditor == null ? true : false) || (Expeditor != null && Expeditor.Status == 1) ? true : false;
                        labelobj.PriorityName.Name = PriorityName != null ? PriorityName.LblText : "Priority";
                        labelobj.PriorityName.Status = (PriorityName == null ? true : false) || (PriorityName != null && PriorityName.Status == 1) ? true : false;
                        labelobj.TagNumber.Name = TagNumber != null ? TagNumber.LblText : "Tag#";
                        labelobj.TagNumber.Status = (TagNumber == null ? true : false) || (TagNumber != null && TagNumber.Status == 1) ? true : false;
                        labelobj.IdentCode.Name = IdentCode != null ? IdentCode.LblText : "Ident Code";
                        labelobj.IdentCode.Status = (IdentCode == null ? true : false) || (IdentCode != null && IdentCode.Status == 1) ? true : false;
                        labelobj.BagNumber.Name = BagNumber != null ? BagNumber.LblText : "Outer Package";
                        labelobj.BagNumber.Status = (BagNumber == null ? true : false) || (BagNumber != null && BagNumber.Status == 1) ? true : false;
                        labelobj.yBkgNumber.Name = ybkgnumber != null ? ybkgnumber.LblText : "yBkg Number";
                        labelobj.yBkgNumber.Status = (ybkgnumber == null ? true : false) || (ybkgnumber != null && ybkgnumber.Status == 1) ? true : false;
                        labelobj.JobName.Name = jobname != null ? jobname.LblText : "Job Name";
                        labelobj.JobName.Status = (jobname == null ? true : false) || (jobname != null && jobname.Status == 1) ? true : false;
                        labelobj.ResourceName.Name = resourcename != null ? resourcename.LblText : "Resource";
                        //labelobj.ResourceName.Status = (resourcename == null ? true : false) || (resourcename != null && resourcename.Status == 1) ? true : false;

                        labelobj.ResetBtn.Name = ResetBtn != null ? ResetBtn.LblText : "Reset";
                        labelobj.SearchBtn.Name = SearchBtn != null ? SearchBtn.LblText : "Search";
                        labelobj.SaveSearchBtn.Name = SaveSearchBtn != null ? SearchBtn.LblText : "Save & Search";
                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    labelobj.ResourceName.Status = Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CreateTask").FirstOrDefault() != null ? true : false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ChangeLabel method -> FilterDataViewModel" + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Assigning values to dynamic text change properties for comparing with FieldID
        /// </summary>
        /// <returns></returns>
        private async Task AssignValueForComparision()
        {
            try
            {
                labelobj.PO.Name = "PONumber"; labelobj.REQNo.Name = "REQNo"; labelobj.ShippingNumber.Name = "ShippingNumber"; labelobj.DisciplineName.Name = "DisciplineName";
                labelobj.ELevelName.Name = "ELevelName"; labelobj.Condition.Name = "ConditionName"; labelobj.Expeditor.Name = "ExpeditorName"; labelobj.PriorityName.Name = "PriorityName";
                labelobj.TagNumber.Name = "TagNumber"; labelobj.IdentCode.Name = "IdentCode"; labelobj.BagNumber.Name = "BagNumber";
                labelobj.yBkgNumber.Name = "yBkgNumber"; labelobj.JobName.Name = "JobName";
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "AssignValueForComparision method -> FilterDataViewModel" + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        #region Properties
        private Color _FiltercontainerColor = Color.FromHex("#772F4F4F");
        public Color FiltercontainerColor
        {
            get { return _FiltercontainerColor; }
            set
            {
                _FiltercontainerColor = value;
                NotifyPropertyChanged();
            }
        }

        private bool _SearchFilterDDlHasError;
        public bool SearchFilterDDlHasError
        {
            get { return _SearchFilterDDlHasError; }
            set
            {
                _SearchFilterDDlHasError = value;
                NotifyPropertyChanged();
            }
        }

        private bool _SearchFilterEntryHasError;
        public bool SearchFilterEntryHasError
        {
            get { return _SearchFilterEntryHasError; }
            set
            {
                _SearchFilterEntryHasError = value;
                NotifyPropertyChanged();
            }
        }

        private List<SearchPassData> _SearchFilterList;
        public List<SearchPassData> SearchFilterList
        {
            get { return _SearchFilterList; }
            set
            {
                _SearchFilterList = value;
                NotifyPropertyChanged();
            }
        }

        private SearchPassData _SelectedFilterName;
        public SearchPassData SelectedFilterName
        {
            get { return _SelectedFilterName; }
            set
            {
                _SelectedFilterName = value;
                NotifyPropertyChanged();
                FilterSelected();
            }
        }

        private string _FilterName;
        public string FilterName
        {
            get { return _FilterName; }
            set
            {
                _FilterName = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsNewSaveSearchEntryVisible;
        public bool IsNewSaveSearchEntryVisible
        {
            get { return _IsNewSaveSearchEntryVisible; }
            set
            {
                _IsNewSaveSearchEntryVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsSaveSearchContentVisible;
        public bool IsSaveSearchContentVisible
        {
            get { return _IsSaveSearchContentVisible; }
            set
            {
                _IsSaveSearchContentVisible = value;
                NotifyPropertyChanged();
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

        private bool _POLNoResultsFoundLbl = true;
        public bool POLNoResultsFoundLbl
        {
            get { return _POLNoResultsFoundLbl; }
            set
            {
                _POLNoResultsFoundLbl = value;
                NotifyPropertyChanged();
            }
        }

        private bool _POLListViewStack = false;
        public bool POLListViewStack
        {
            get { return _POLListViewStack; }
            set
            {
                _POLListViewStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _PODNoResultsFoundLbl = true;
        public bool PODNoResultsFoundLbl
        {
            get { return _PODNoResultsFoundLbl; }
            set
            {
                _PODNoResultsFoundLbl = value;
                NotifyPropertyChanged();
            }
        }

        private bool _PODListViewStack = false;
        public bool PODListViewStack
        {
            get { return _PODListViewStack; }
            set
            {
                _PODListViewStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _OnsiteNoResultsFoundLbl = true;
        public bool OnsiteNoResultsFoundLbl
        {
            get { return _OnsiteNoResultsFoundLbl; }
            set
            {
                _OnsiteNoResultsFoundLbl = value;
                NotifyPropertyChanged();
            }
        }

        private bool _OnsiteListViewStack = false;
        public bool OnsiteListViewStack
        {
            get { return _OnsiteListViewStack; }
            set
            {
                _OnsiteListViewStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _headerbox = true;
        public bool headerbox
        {
            get { return _headerbox; }
            set
            {
                _headerbox = value;
                NotifyPropertyChanged();
            }
        }


        private string _tagNumber = string.Empty;
        public string tagNumber
        {
            get { return _tagNumber; }
            set
            {
                _tagNumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _Identcode = string.Empty;
        public string Identcode
        {
            get { return _Identcode; }
            set
            {
                _Identcode = value;
                NotifyPropertyChanged();
            }
        }

        private string _BagNumber = string.Empty;
        public string BagNumber
        {
            get { return _BagNumber; }
            set
            {
                _BagNumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _YbkgNumber = string.Empty;
        public string YbkgNumber
        {
            get { return _YbkgNumber; }
            set
            {
                _YbkgNumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _JobName = string.Empty;
        public string JobName
        {
            get { return _JobName; }
            set
            {
                _JobName = value;
                NotifyPropertyChanged();
            }
        }

        #region Key tab related field properties

        private string _poNumber = string.Empty;
        public string poNumber
        {
            get { return _poNumber; }
            set
            {
                _poNumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _reqNumber = string.Empty;
        public string reqNumber
        {
            get { return _reqNumber; }
            set
            {
                _reqNumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _shipNumber = string.Empty;
        public string shipNumber
        {
            get { return _shipNumber; }
            set
            {
                _shipNumber = value;
                NotifyPropertyChanged();
            }
        }

        private List<DDLmaster> _DisciplineList;
        public List<DDLmaster> DisciplineList
        {
            get { return _DisciplineList; }
            set
            {
                _DisciplineList = value;
                NotifyPropertyChanged();
            }
        }

        private List<DDLmaster> _ELevelList;
        public List<DDLmaster> ELevelList
        {
            get { return _ELevelList; }
            set
            {
                _ELevelList = value;
                NotifyPropertyChanged();
            }
        }

        private List<DDLmaster> _PriorityList;
        public List<DDLmaster> PriorityList
        {
            get { return _PriorityList; }
            set
            {
                _PriorityList = value;
                NotifyPropertyChanged();
            }
        }

        private List<DDLmaster> _Expeditorlist;
        public List<DDLmaster> Expeditorlist
        {
            get { return _Expeditorlist; }
            set
            {
                _Expeditorlist = value;
                NotifyPropertyChanged();
            }
        }

        private List<DDLmaster> _Conditionlist;
        public List<DDLmaster> Conditionlist
        {
            get { return _Conditionlist; }
            set
            {
                _Conditionlist = value;
                NotifyPropertyChanged();
            }
        }

        private List<DDLmaster> _Resourcelist;
        public List<DDLmaster> Resourcelist
        {
            get { return _Resourcelist; }
            set
            {
                _Resourcelist = value;
                NotifyPropertyChanged();
            }
        }

        private string _SelectedDisciplineValue;
        public string SelectedDisciplineValue
        {
            get { return _SelectedDisciplineValue; }
            set
            {
                _SelectedDisciplineValue = value;
                SelectedDiscipline_TapEvent();
            }
        }

        private string _SelectedResourceValue;
        public string SelectedResourceValue
        {
            get { return _SelectedResourceValue; }
            set
            {
                _SelectedResourceValue = value;
                SelectedResource_TapEvent();
            }
        }

        private string _SelectedELevelValue;
        public string SelectedELevelValue
        {
            get { return _SelectedELevelValue; }
            set
            {
                _SelectedELevelValue = value;
                SelectedELevel_TapEvent();
            }
        }

        private string _SelectedConditionValue;
        public string SelectedConditionValue
        {
            get { return _SelectedConditionValue; }
            set
            {
                _SelectedConditionValue = value;
                SelectedCondition_TapEvent();
            }
        }

        private string _SelectedExpeditorValue;
        public string SelectedExpeditorValue
        {
            get { return _SelectedExpeditorValue; }
            set
            {
                _SelectedExpeditorValue = value;
                SelectedExpeditor_TapEvent();
            }
        }

        private string _SelectedPrioritytValue;
        public string SelectedPrioritytValue
        {
            get { return _SelectedPrioritytValue; }
            set
            {
                _SelectedPrioritytValue = value;
                SelectedPriority_TapEvent();
            }
        }

        #region Set default values for Picker
        private string _DisciplineDefaultValue;
        public string DisciplineDefaultValue
        {
            get { return _DisciplineDefaultValue; }
            set
            {
                _DisciplineDefaultValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _ResourceDefaultValue;
        public string ResourceDefaultValue
        {
            get { return _ResourceDefaultValue; }
            set
            {
                _ResourceDefaultValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _ELevelDefaultValue;
        public string ELevelDefaultValue
        {
            get { return _ELevelDefaultValue; }
            set
            {
                _ELevelDefaultValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _ConditionDefaultValue;
        public string ConditionDefaultValue
        {
            get { return _ConditionDefaultValue; }
            set
            {
                _ConditionDefaultValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _ExpeditorDefaultValue;
        public string ExpeditorDefaultValue
        {
            get { return _ExpeditorDefaultValue; }
            set
            {
                _ExpeditorDefaultValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _PriorityDefaultValue;
        public string PriorityDefaultValue
        {
            get { return _PriorityDefaultValue; }
            set
            {
                _PriorityDefaultValue = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Set default value for Picker Labels
        private string _EntrySearchType;
        public string EntrySearchType
        {
            get => _EntrySearchType;
            set
            {
                _EntrySearchType = value;
                NotifyPropertyChanged();
            }
        }

        private string _PickUpDefaultValue_C;
        public string PickUpDefaultValue_C
        {
            get { return _PickUpDefaultValue_C; }
            set
            {
                _PickUpDefaultValue_C = value;
                NotifyPropertyChanged();
            }
        }

        private string _POLDefaultValue_C;
        public string POLDefaultValue_C
        {
            get { return _POLDefaultValue_C; }
            set
            {
                _POLDefaultValue_C = value;
                NotifyPropertyChanged();
            }
        }

        private string _PODDefaultValue_C;
        public string PODDefaultValue_C
        {
            get { return _PODDefaultValue_C; }
            set
            {
                _PODDefaultValue_C = value;
                NotifyPropertyChanged();
            }
        }

        private string _OnsiteDefaultValue_C;
        public string OnsiteDefaultValue_C
        {
            get { return _OnsiteDefaultValue_C; }
            set
            {
                _OnsiteDefaultValue_C = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        private List<DDLmaster> _CountryList;
        public List<DDLmaster> CountryList
        {
            get { return _CountryList; }
            set
            {
                _CountryList = value;
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
        #region Picker ItemSources

        private List<string> _DisciplineNames;
        public List<string> DisciplineNames
        {
            get { return _DisciplineNames; }
            set
            {
                _DisciplineNames = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _ELevelNames;
        public List<string> ELevelNames
        {
            get { return _ELevelNames; }
            set
            {
                _ELevelNames = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _PriorityNames;
        public List<string> PriorityNames
        {
            get { return _PriorityNames; }
            set
            {
                _PriorityNames = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _ConditionNames;
        public List<string> ConditionNames
        {
            get { return _ConditionNames; }
            set
            {
                _ConditionNames = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _ResourceNames;
        public List<string> ResourceNames
        {
            get { return _ResourceNames; }
            set
            {
                _ResourceNames = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _ExpeditorNames;
        public List<string> ExpeditorNames
        {
            get { return _ExpeditorNames; }
            set
            {
                _ExpeditorNames = value;
                NotifyPropertyChanged();
            }
        }


        private ObservableCollection<SearchData> _POLNames_L;
        public ObservableCollection<SearchData> POLNames_L
        {
            get { return _POLNames_L; }
            set
            {
                _POLNames_L = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SearchData> _PODNames_L;
        public ObservableCollection<SearchData> PODNames_L
        {
            get { return _PODNames_L; }
            set
            {
                _PODNames_L = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SearchData> _OnsiteNames_L;
        public ObservableCollection<SearchData> OnsiteNames_L
        {
            get { return _OnsiteNames_L; }
            set
            {
                _OnsiteNames_L = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _PickUpNames_C;
        public List<string> PickUpNames_C
        {
            get { return _PickUpNames_C; }
            set
            {
                _PickUpNames_C = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _POLNames_C;
        public List<string> POLNames_C
        {
            get { return _POLNames_C; }
            set
            {
                _POLNames_C = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _PODNames_C;
        public List<string> PODNames_C
        {
            get { return _PODNames_C; }
            set
            {
                _PODNames_C = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _OnsiteNames_C;
        public List<string> OnsiteNames_C
        {
            get { return _OnsiteNames_C; }
            set
            {
                _OnsiteNames_C = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region filter tabs visibility
        public bool _headerVisibility = true;
        public bool headerVisibility
        {
            get { return _headerVisibility; }
            set
            {
                _headerVisibility = value;
                NotifyPropertyChanged();
            }
        }


        public bool _countryVisibility = false;
        public bool countryVisibility
        {
            get { return _countryVisibility; }
            set
            {
                _countryVisibility = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region  tabs text color
        public Color _headerTextColor = Color.White;
        public Color headerTextColor
        {
            get { return _headerTextColor; }
            set
            {
                _headerTextColor = value;
                NotifyPropertyChanged();
            }
        }

        public Color _countryTextColor = Color.White;
        public Color countryTextColor
        {
            get { return _countryTextColor; }
            set
            {
                _countryTextColor = value;
                NotifyPropertyChanged();
            }
        }


        #endregion

        #region  tabs background color
        public Color _headerBgColor = Color.FromHex("#269DC9");
        public Color headerBgColor
        {
            get { return _headerBgColor; }
            set
            {
                _headerBgColor = value;
                NotifyPropertyChanged();
            }
        }

        public Color _countryBgColor = Color.LightBlue;
        public Color countryBgColor
        {
            get { return _countryBgColor; }
            set
            {
                _countryBgColor = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #endregion
    }
}