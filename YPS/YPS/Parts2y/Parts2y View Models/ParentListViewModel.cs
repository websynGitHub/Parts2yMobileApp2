using Newtonsoft.Json;
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
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using static YPS.Model.SearchModel;
using static YPS.Models.ChatMessage;
using YPS.Parts2y.Parts2y_Views;
using YPS.Views;
using Acr.UserDialogs;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class ParentListViewModel : INotifyPropertyChanged
    {
        #region IComman and data members declaration
        public INavigation Navigation { get; set; }
        public ICommand tap_InitialCameraCommand { set; get; }
        public ICommand tap_OnMessageCommand { set; get; }
        public ICommand tap_InitialFileUploadCommand { set; get; }
        public ICommand tap_PrinterCommand { get; set; }
        public ICommand tap_OnChat { get; set; }
        public ICommand tap_OnCamB { get; set; }
        public ICommand tap_OnCamA { get; set; }
        public ICommand tap_FileUpload { get; set; }
        public ICommand ChooseColumns_Tap { get; set; }
        public ICommand btn_done { get; set; }
        public ICommand profile_Change_Tap { get; set; }
        public ICommand IsRequired_Tap_IC { get; set; }
        public ICommand IsNotRequired_Tap_IC { get; set; }
        public ICommand tap_OnFilterCommand { get; set; }
        public ICommand tab_RefreshPage { get; set; }
        public ICommand tab_ClearSearch { get; set; }
        public ICommand ShowPageSizePickerItems { get; set; }
        public ICommand PrintShippingMarkReportCmd { get; set; }
        public ICommand GroupeFileUploadCmd { get; set; }
        public ICommand SelectedPOCmd { get; set; }
        public ICommand InProgressCmd { get; set; }
        public ICommand CompletedCmd { get; set; }
        public ICommand PendingCmd { get; set; }
        public ICommand AllCmd { set; get; }
        public Command HomeCmd { get; set; }
        public Command JobCmd { get; set; }
        public Command PartsCmd { get; set; }
        public Command LoadCmd { set; get; }
        public ICommand DeleteFilterSearchCmd { get; set; }
        public ICommand SelectedFilterbyName { get; set; }
        public ICommand SelectedParentDetailsCmd { get; set; }

        YPSService trackService;
        ParentListPage pagename;
        private bool avoidMutiplePageOpen = false;
        SendPodata sendPodata;
        public ICommand Backevnttapped { set; get; }
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="_Navigation"></param>
        /// <param name="page"></param>
        public ParentListViewModel(INavigation _Navigation, ParentListPage page)
        {
            try
            {
                loadingindicator = true;
                YPSLogger.TrackEvent("ParentListViewModel.cs", " in ParentListViewModel constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Navigation = _Navigation;
                pagename = page;
                trackService = new YPSService();
                Settings.QAType = (int)QAType.PT;
                var cID = Settings.CompanyID;
                var pID = Settings.ProjectID;
                var jID = Settings.JobID;
                ISPoDataListVisible = true;
                profileSettingVisible = true;
                mainStack = true;

                //ChangeLabel();
                Task.Run(() => ChangeLabel()).Wait();
                Task.Run(() => BindGridData(-1)).Wait();

                #region Assigning methods to the respective ICommands
                PrintShippingMarkReportCmd = new Command(PrintShippingMarkReport);
                GroupeFileUploadCmd = new Command(GroupFileUpload);
                SelectedPOCmd = new Command(SelectedParentDetails);
                InProgressCmd = new Command(async () => await InProgress_Tap());
                CompletedCmd = new Command(async () => await Complete_Tap());
                PendingCmd = new Command(async () => await Pending_Tap());
                AllCmd = new Command(async () => await All_Tap());
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                DeleteFilterSearchCmd = new Command(DeleteFilterSearch);
                SelectedFilterbyName = new Command(SelectedFilterbyNameMethod);

                HomeCmd = new Command(async () => await TabChange("home"));
                JobCmd = new Command(async () => await TabChange("job"));
                PartsCmd = new Command(async () => await TabChange("parts"));
                LoadCmd = new Command(async () => await TabChange("load"));
                #endregion
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ParentListViewModel constructor -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
            loadingindicator = false;
        }

        /// <summary>
        /// Gets called when an item is selected from Saved Search Filters popup.
        /// </summary>
        /// <param name="sender"></param>
        public async void SelectedFilterbyNameMethod(object sender)
        {
            try
            {
                loadingindicator = true;
                SearchPassData value = sender as SearchPassData;
                SelectedFilterName = value;
            }
            catch (Exception ex)
            {
                loadingindicator = false;
                YPSLogger.ReportException(ex, "SelectedFilterbyNameMethod -> in ParentListViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadingindicator = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on delete icon, of any search from the saved search popup, to delete that respective search.
        /// </summary>
        /// <param name="sender"></param>
        public async void DeleteFilterSearch(object sender)
        {
            try
            {
                YPSLogger.TrackEvent("ParentListViewModel.cs", " in DeleteFilterSearch method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                if (await App.Current.MainPage.DisplayAlert("Delete", "Are you sure you want to delete?", "Ok", "Cancel") == true)
                {
                    var value = sender as SearchPassData;
                    var result = await trackService.DeleteUserSearchFilter(value);

                    if (result != null && result.data != null)
                    {
                        SearchFilterList.Remove(value);

                        if (SearchFilterList == null || SearchFilterList.Count == 0)
                        {
                            if (AllTabVisibility == true)
                            {
                                await All_Tap();
                            }
                            else if (CompleteTabVisibility == true)
                            {
                                await Complete_Tap();
                            }
                            else if (InProgressTabVisibility == true)
                            {
                                await InProgress_Tap();
                            }
                            else
                            {
                                await Pending_Tap();
                            }

                            IsSearchFilterListVisible = false;
                            IsSearchFilterIconVisible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DeleteFilterSearch -> in ParentListViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadingindicator = false;
            }
        }

        /// <summary>
        /// This method is to show/hide filter icon & to show the popup when clicked on filter icon.
        /// </summary>
        /// <param name="popfilterlist"></param>
        /// <returns></returns>
        public async Task ShowHideSearFilterList(bool popfilterlist)
        {
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    YPSLogger.TrackEvent("ParentListViewModel.cs", " in ShowHideSearFilterList method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    var result = await trackService.GetSavedUserSearchSettings();

                    if (result != null && result.data != null)
                    {
                        if (result.data.Count != 0)
                        {
                            SearchFilterList = new ObservableCollection<SearchPassData>(result.data);

                            SearchFilterList.Where(wr => wr.Status == 1 && Settings.IsSearchClicked == false).Select(c =>
                              {
                                  c.SelectedTagBorderColor = Settings.Bar_Background;
                                  return c;
                              }).ToList();

                            IsSearchFilterIconVisible = SearchFilterList?.Count > 0 ? true : false;
                            IsSearchFilterListVisible = popfilterlist == true ? true : false;

                            if (NoRecordsLbl == true && popfilterlist == true)
                            {
                                NoRecordsLbl = false;
                            }
                        }
                        else
                        {
                            IsSearchFilterIconVisible = false;
                        }
                    }
                    else
                    {
                        IsSearchFilterIconVisible = false;
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
                YPSLogger.ReportException(ex, "ShowHideSearFilterList -> in ParentListViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadingindicator = false;
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Saved Search Filters popup.
        /// </summary>
        public async void FilterSelected()
        {
            try
            {
                loadingindicator = true;

                if (SelectedFilterName != null)
                {
                    SearchFilterList?.Where(wr => wr.SelectedTagBorderColor == Settings.Bar_Background)
 .Select(c =>
 {
     c.SelectedTagBorderColor = Color.White;
     return c;
 }).ToList();
                    SelectedFilterName.SelectedTagBorderColor = Settings.Bar_Background;
                    Settings.IsSearchClicked = false;
                    await BindGridData(-1, true);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FilterSelected method -> in ParentListViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                loadingindicator = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on Home tab.
        /// </summary>
        /// <param name="tabname"></param>
        /// <returns></returns>
        private async Task TabChange(string tabname)
        {
            try
            {
                loadingindicator = true;

                if (tabname == "home")
                {
                    Navigation.PopToRootAsync(false);
                }
            }
            catch (Exception ex)
            {
                loadingindicator = false;
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "TabChange method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
            finally
            {
                loadingindicator = false;
            }
        }

        /// <summary>
        /// Gets called whne clicked on pending tab.
        /// </summary>
        /// <returns></returns>
        public async Task Pending_Tap()
        {
            try
            {
                await BindGridData(0);

                PendingTabVisibility = true;
                CompleteTabVisibility = InProgressTabVisibility = AllTabVisibility = false;
                PendingTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = CompleteTabTextColor = AllTabTextColor = Color.Black;

            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Pending_Tap method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called whne clicked on inprogress tab.
        /// </summary>
        /// <returns></returns>
        public async Task InProgress_Tap()
        {
            try
            {
                await BindGridData(1);

                InProgressTabVisibility = true;
                CompleteTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                InProgressTabTextColor = Settings.Bar_Background;
                CompleteTabTextColor = PendingTabTextColor = AllTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "InProgress_Tap method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called whne clicked on done tab.
        /// </summary>
        /// <returns></returns>
        public async Task Complete_Tap()
        {
            try
            {
                await BindGridData(2);

                CompleteTabVisibility = true;
                InProgressTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                CompleteTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = PendingTabTextColor = AllTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Complete_Tap method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called whne clicked on all tab.
        /// </summary>
        /// <returns></returns>
        public async Task All_Tap()
        {
            try
            {
                await BindGridData(-1);

                AllTabVisibility = true;
                InProgressTabVisibility = CompleteTabVisibility = PendingTabVisibility = false;
                AllTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = CompleteTabTextColor = PendingTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "All_Tap method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method is to get the fresh PoData.
        /// </summary>
        /// <param name="isfromsearchfilter"></param>
        /// <returns></returns>
        public async Task<GetPoData> GetRefreshedData(bool isfromsearchfilter)
        {
            GetPoData result = new GetPoData();
            try
            {
                sendPodata = new SendPodata();
                sendPodata.UserID = Settings.userLoginID;
                sendPodata.PageSize = Settings.pageSizeYPS;
                sendPodata.StartPage = Settings.startPageYPS;
                await SearchResultGet(sendPodata, isfromsearchfilter);

                result = await trackService.LoadPoDataService(sendPodata);
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "GetRefreshedData method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
            return result;
        }

        /// <summary>
        /// This method is for binding the records/date to the data grid.
        /// </summary>
        /// </summary>
        /// <param name="iSPagingNoAuto"></param>
        /// <returns></returns>
        public async Task BindGridData(int postatus, bool isfromsearchfilter = false)
        {
            try
            {
                loadingindicator = true;

                YPSLogger.TrackEvent("ParentListViewModel.cs", " in BindGridData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    bool ifNotRootedDevie;

                    ifNotRootedDevie = true;/// need to remove this line when run in device 

                    if (ifNotRootedDevie)
                    {
                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            var result = await GetRefreshedData(isfromsearchfilter);

                            if (result != null)
                            {
                                if (result.status == 1 && result.data.allPoDataMobile != null)
                                {
                                    Settings.AllPOData = new ObservableCollection<AllPoData>();
                                    Settings.AllPOData = result.data.allPoDataMobile;

                                    var groubbyval = result.data.allPoDataMobile.GroupBy(gb => new { gb.TaskID });

                                    ObservableCollection<AllPoData> groupedlist = new ObservableCollection<AllPoData>();

                                    foreach (var val in groubbyval)
                                    {
                                        AllPoData groupdata = new AllPoData();
                                        groupdata.POID = val.Select(s => s.POID).FirstOrDefault();
                                        groupdata.PONumber = val.Select(s => s.PONumber).FirstOrDefault();
                                        groupdata.REQNo = val.Select(s => s.REQNo).FirstOrDefault();
                                        groupdata.ShippingNumber = val.Select(s => s.ShippingNumber).FirstOrDefault();
                                        groupdata.POShippingNumber = val.Select(s => s.POShippingNumber).FirstOrDefault();
                                        groupdata.TaskName = val.Select(s => s.TaskName).FirstOrDefault();
                                        groupdata.StartTime = val.Select(s => !string.IsNullOrEmpty(s.StartTime) ? Convert.ToDateTime(s.StartTime).ToString("HH:mm") : s.StartTime).FirstOrDefault();
                                        groupdata.EndTime = val.Select(s => !string.IsNullOrEmpty(s.EndTime) ? Convert.ToDateTime(s.EndTime).ToString("HH:mm") : s.EndTime).FirstOrDefault();
                                        groupdata.IsTimeGiven = string.IsNullOrEmpty(groupdata.StartTime) && string.IsNullOrEmpty(groupdata.EndTime) ? false : true;
                                        groupdata.TaskID = val.Select(s => s.TaskID).FirstOrDefault();
                                        groupdata.TaskStatus = val.Select(s => s.TaskStatus).FirstOrDefault();
                                        //groupdata.TaskResourceName = val.Select(s => s.TaskResourceName).FirstOrDefault();
                                        groupdata.EventName = val.Select(s => s.EventName).FirstOrDefault();
                                        groupdata.POTaskStatusIcon = null;
                                        groupdata.TaskResourceID = val.Select(s => s.TaskResourceID).FirstOrDefault();
                                        groupdata.EventID = val.Select(s => s.EventID).FirstOrDefault();

                                        IEnumerable<int> poidlist = val.Select(s => s.POID).Distinct();
                                        groupdata.PONumberForDisplay = poidlist.Count() == 1 ? groupdata.PONumber : "Multiple";
                                        groupdata.ShippingNumberForDisplay = poidlist.Count() == 1 ? groupdata.ShippingNumber : "Multiple";
                                        groupdata.REQNoForDisplay = poidlist.Count() == 1 ? groupdata.REQNo : "Multiple";
                                        groupdata.TagNumberForDisplay = poidlist.Count() == 1 ? groupdata.TagNumber : "Multiple";

                                        if (groupdata.TaskStatus == 0)
                                        {
                                            groupdata.POTaskStatusIcon = Icons.Pending;
                                        }
                                        else if (groupdata.TaskStatus == 1)
                                        {
                                            groupdata.POTaskStatusIcon = Icons.Progress;
                                        }
                                        else
                                        {
                                            groupdata.POTaskStatusIcon = Icons.Done;
                                        }

                                        //groupdata.IsTaskResourceVisible = val.Select(c => c.TaskResourceID).FirstOrDefault() == Settings.userLoginID ? false : true;
                                        groupdata.ShippingMarkOpacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "ShippingMarkDownload".Trim().ToLower()).FirstOrDefault()) != null ? 1.0 : 0.5;

                                        if (val.Select(c => c?.TaskResourceID).FirstOrDefault() == 0 || val.Select(c => c?.TaskResourceID).FirstOrDefault() == null)
                                        {
                                            groupdata.JobTileColor = Color.LightGray;
                                            groupdata.IsIconVisible = false;
                                            groupdata.IsShippingMarkVisible = false;
                                        }

                                        groupedlist.Add(groupdata);
                                    }

                                    groupedlist = new ObservableCollection<AllPoData>(groupedlist.OrderBy(o => o.EventID).ThenBy(tob => tob.TaskStatus).
                                        ThenBy(tob => tob.TaskName));

                                    if (postatus == -1)
                                    {
                                        PoDataCollections = new ObservableCollection<AllPoData>(groupedlist);
                                    }
                                    else
                                    {
                                        PoDataCollections = new ObservableCollection<AllPoData>(groupedlist.Where(wr => wr.TaskStatus == postatus));
                                    }

                                    //List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                                    //if (labelval.Count > 0)
                                    //{
                                    //    var pending = labelval.Where(wr => wr.FieldID == labelobj.Pending.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                                    //    var inprogress = labelval.Where(wr => wr.FieldID == labelobj.Inprogress.Name.Trim().Replace(" ", "")).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                                    //    var complete = labelval.Where(wr => wr.FieldID == labelobj.Completed.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                                    //    var all = labelval.Where(wr => wr.FieldID == labelobj.All.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                                    //    labelobj.Pending.Name = (pending != null ? (!string.IsNullOrEmpty(pending.LblText) ? pending.LblText : labelobj.Pending.Name) : labelobj.Pending.Name);
                                    //    labelobj.Pending.Status = pending == null ? true : (pending.Status == 1 ? true : false);
                                    //    labelobj.Inprogress.Name = (inprogress != null ? (!string.IsNullOrEmpty(inprogress.LblText) ? inprogress.LblText : labelobj.Inprogress.Name) : labelobj.Inprogress.Name);
                                    //    labelobj.Inprogress.Status = inprogress == null ? true : (inprogress.Status == 1 ? true : false);
                                    //    labelobj.Completed.Name = (complete != null ? (!string.IsNullOrEmpty(complete.LblText) ? complete.LblText : labelobj.Completed.Name) : labelobj.Completed.Name);
                                    //    labelobj.Completed.Status = complete == null ? true : (complete.Status == 1 ? true : false);
                                    //    labelobj.All.Name = (all != null ? (!string.IsNullOrEmpty(all.LblText) ? all.LblText : labelobj.All.Name) : labelobj.All.Name);
                                    //    labelobj.All.Status = all == null ? true : (all.Status == 1 ? true : false);
                                    //}

                                    if (PoDataCollections.Count > 0)
                                    {
                                        ListCount = result.data.listCount;
                                        var roundcount = Math.Ceiling((decimal)ListCount / (decimal)Settings.pageSizeYPS);
                                        PageCount = (Int32)roundcount;
                                        NumericButtonCount = (Int32)roundcount;

                                        await CheckingSearchValues();
                                        NoRecordsLbl = false;
                                        ISPoDataListVisible = true;

                                        PendingText = labelobj.Pending.Name + "\n" + "(" + groupedlist.Where(wr => wr.TaskStatus == 0).Count() + ")";
                                        InProgress = labelobj.Inprogress.Name + "\n" + "(" + groupedlist.Where(wr => wr.TaskStatus == 1).Count() + ")";
                                        Completed = labelobj.Completed.Name + "\n" + "(" + groupedlist.Where(wr => wr.TaskStatus == 2).Count() + ")";
                                        All = labelobj.All.Name + "\n" + "(" + groubbyval.Count() + ")";
                                    }
                                    else
                                    {
                                        NoRecordsLbl = true;
                                        ISPoDataListVisible = false;

                                        PendingText = labelobj.Pending.Name + "\n" + "(" + groupedlist.Where(wr => wr.TaskStatus == 0).Count() + ")";
                                        InProgress = labelobj.Inprogress.Name + "\n" + "(" + groupedlist.Where(wr => wr.TaskStatus == 1).Count() + ")";
                                        Completed = labelobj.Completed.Name + "\n" + "(" + groupedlist.Where(wr => wr.TaskStatus == 2).Count() + ")";
                                        All = labelobj.All.Name + "\n" + "(" + groubbyval.Count() + ")";
                                    }

                                }
                                else
                                {
                                    NoRecordsLbl = true;
                                    await CheckingSearchValues();
                                    ISPoDataListVisible = false;
                                    loadingindicator = false;
                                    PendingText = labelobj.Pending.Name + "\n" + "(0)";
                                    InProgress = labelobj.Inprogress.Name + "\n" + "(0)";
                                    Completed = labelobj.Completed.Name + "\n" + "(0)";
                                    All = labelobj.All.Name + "\n" + "(0)";
                                }
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                            //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }
                    }
                    else
                    {
                        Exception ex = new Exception();
                        YPSLogger.ReportException(ex, "BindGridData method -> Your phone is rooted , please unroot to use app in ParentListViewModel.cs ");
                        await App.Current.MainPage.DisplayAlert("Warning", "Your phone is rooted , please unroot to use app", "Ok");
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "BindGridData method -> in ParentListViewModel.cs " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                    loadingindicator = false;
                }
                profileSettingVisible = true;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "BindGridData method -> in ParentListViewModel.cs " + Settings.userLoginID);
                loadingindicator = false;
            }
            finally
            {
                loadingindicator = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on any Job.
        /// </summary>
        /// <param name="sender"></param>
        public async void SelectedParentDetails(object sender)
        {
            try
            {
                PODetails = sender as AllPoData;

                if (PODetails != null)
                {
                    try
                    {
                        loadingindicator = true;

                        PoDataCollections?.ToList().ForEach(fe => { fe.SelectedTagBorderColor = Color.Transparent; });
                        PODetails.SelectedTagBorderColor = Settings.Bar_Background;

                        List<Alllabeslvalues> labelval = Settings.alllabeslvalues?.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                        var PoDataChilds = new ObservableCollection<AllPoData>(
                                Settings.AllPOData.Where(wr => wr.TaskID == PODetails.TaskID).ToList()
                                );

                        PoDataChilds.ToList().ForEach(fe =>
                        {
                            fe.PONumberForDisplay = PODetails.PONumberForDisplay;
                            fe.ShippingNumberForDisplay = PODetails.ShippingNumberForDisplay;
                            fe.REQNoForDisplay = PODetails.REQNoForDisplay;
                            fe.TagNumberForDisplay = PODetails.TagNumberForDisplay;
                        });

                        if (PODetails?.TaskResourceID == 0 || PODetails?.TaskResourceID == null)
                        {
                            var tag = labelval?.Where(wr => wr.FieldID.Trim().ToLower() == "TagNumber".Trim().ToLower())?.FirstOrDefault().LblText.ToString();

                            await App.Current.MainPage.DisplayAlert("This job is not yet assigned.",
                                "To assign this job, scan one of the " + tag + " from the Home.\n\n" +
                               tag + "\n" + String.Join("\n \n", PoDataChilds?.Select(c => c.TagNumber).ToList()), "Ok");
                        }
                        else
                        {
                            await Navigation.PushAsync(new POChildListPage(PoDataChilds, sendPodata), false);
                        }

                    }
                    catch (Exception ex)
                    {
                        trackService.Handleexception(ex);
                        YPSLogger.ReportException(ex, "SelectedParentDetails method -> in ParentListViewModel.cs, inner exception" + Settings.userLoginID);
                    }
                    finally
                    {
                        loadingindicator = false;
                    }
                }
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "SelectedParentDetails method -> in ParentListViewModel.cs " + Settings.userLoginID);
                loadingindicator = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on ShippingMark icon, to view the ShippingMark report.
        /// </summary>
        /// <param name="sender"></param>
        public async void PrintShippingMarkReport(object sender)
        {
            try
            {
                if (!loadingindicator)
                {
                    loadingindicator = true;

                    var senderval = sender as AllPoData;

                    YPSService yPSService = new YPSService();

                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (senderval.ShippingMarkOpacity == 1.0)
                        {
                            var printResult = await yPSService.PrintPDFByUsingPOID(senderval.POID);

                            PrintPDFModel printPDFModel = new PrintPDFModel();

                            if (printResult?.status == 1)
                            {
                                var bArrayPOID = printResult.data;
                                byte[] bytesPOID = Convert.FromBase64String(bArrayPOID);

                                printPDFModel.bArray = bytesPOID;
                                printPDFModel.FileName = "ShippingMark" + "_" + String.Format("{0:yyyyMMMdd_hh-mm-ss}", DateTime.Now) + ".pdf";
                                printPDFModel.PDFFileTitle = "Shipping Marks";

                                switch (Device.RuntimePlatform)
                                {
                                    case Device.iOS:

                                        if (await FileManager.ExistsAsync(printPDFModel.FileName) == false)
                                        {
                                            await FileManager.GetByteArrayData(printPDFModel);
                                        }

                                        var url = FileManager.GetFilePathFromRoot(printPDFModel.FileName);

                                        DependencyService.Get<NewOpenPdfI>().passPath(url);

                                        break;
                                    case Device.Android:
                                        Navigation.PushAsync(new PdfViewPage(printPDFModel), false);
                                        break;
                                }
                            }
                        }
                        else
                        {
                             await App.Current.MainPage.DisplayAlert("Action denied", "You don't have permission to do this action.", "Ok");
                            //DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PrintShippingMarkReport method -> in ParentListViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                loadingindicator = false;
            }
            finally
            {
                loadingindicator = false;
            }
        }

        /// <summary>
        /// This method gets called when clicked on upload icon present in main/parent grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GroupFileUpload(object sender)
        {
            loadingindicator = true;
            try
            {
                var senderval = sender as AllPoData;

                if (Settings.fileUploadPageCount == 0)
                {
                    StartUploadFileModel poShipNum_obj = new StartUploadFileModel();
                    poShipNum_obj.alreadyExit = senderval.POShippingNumber;
                    Settings.fileUploadPageCount = 1;
                    Settings.isFinalvol = senderval.ISFinalVol;
                    Navigation.PushAsync(new FileUpload(poShipNum_obj, senderval.POID, senderval.FUID,
                       "plFile", false), false);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GroupFileUpload method -> in ParentListViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
            loadingindicator = false;
        }

        /// <summary>
        /// Gets called when clciked on back icon, to move back to the previous page i.e Home page.
        /// </summary>
        /// <returns></returns>
        public async Task Backevnttapped_click()
        {
            try
            {
                Navigation.PopToRootAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Backevnttapped_click method -> in ParentListViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is called when clicked on search icon, there on ToolBar.
        /// </summary>
        public async void FilterClicked()
        {
            if (!loadingindicator)
            {
                YPSLogger.TrackEvent("ParentListViewModel.cs", " in FilterClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    loadingindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (Settings.filterPageCount == 0)
                        {
                            IsSearchFilterListVisible = false;
                            Settings.filterPageCount = 1;
                            Settings.refreshPage = 1;
                            Navigation.PushAsync(new FilterData(), false);
                        }
                    }
                    else
                    {
                        loadingindicator = false;
                        await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "FilterClicked method-> in ParentListViewModel.cs " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    loadingindicator = false;
                }
            }
        }

        bool iscalled = false;

        /// <summary>
        /// This method is called when clicked on Refresh Page, from more options.
        /// </summary>
        /// <returns></returns>
        public async Task RefreshPage()
        {
            try
            {
                if (!loadingindicator)
                {
                    YPSLogger.TrackEvent("ParentListViewModel.cs", " in FilterClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    loadingindicator = true;
                    SearchDisable = false;

                    if (AllTabVisibility == true)
                    {
                        await All_Tap();
                    }
                    else if (CompleteTabVisibility == true)
                    {
                        await Complete_Tap();
                    }
                    else if (InProgressTabVisibility == true)
                    {
                        await InProgress_Tap();
                    }
                    else
                    {
                        await Pending_Tap();
                    }
                    SearchDisable = true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RefreshPage -> in ParentListViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadingindicator = false;
            }
        }

        bool isloading = false;

        /// <summary>
        /// This method gets called when clicked on "Clear Search" label.
        /// </summary>
        /// <returns></returns>
        public async Task ClearSearch()
        {
            if (!loadingindicator)
            {
                YPSLogger.TrackEvent("ParentListViewModel.cs", " in clearData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    loadingindicator = true;

                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        await SaveAndClearSearch(new SendPodata(), false);

                        if (NoRecordsLbl == true)
                        {
                            BindGridData(-1);
                        }
                        else if (NoRecordsLbl == false)
                        {
                            BindGridData(-1);
                        }

                        isloading = true;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "ClearSearch method -> in ParentListViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    loadingindicator = false;
                }
            }
        }

        /// <summary>
        /// This method is to clear the search criteria values and save to DB.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private async Task SaveAndClearSearch(SendPodata sendPodata, bool val)
        {
            //SendPodata SaveUserDS = new SendPodata();
            SearchPassData defaultData = new SearchPassData();
            try
            {
                //Key
                sendPodata.PONumber = Settings.PONumber = Settings.IsSearchClicked == false ? string.Empty : Settings.PONumber;
                sendPodata.REQNo = Settings.REQNo = Settings.IsSearchClicked == false ? string.Empty : Settings.REQNo;
                sendPodata.ShippingNo = Settings.ShippingNo = Settings.IsSearchClicked == false ? string.Empty : Settings.ShippingNo;
                sendPodata.DisciplineID = Settings.DisciplineID = Settings.IsSearchClicked == false ? 0 : Settings.DisciplineID;
                sendPodata.ELevelID = Settings.ELevelID = Settings.IsSearchClicked == false ? 0 : Settings.ELevelID;
                sendPodata.ConditionID = Settings.ConditionID = Settings.IsSearchClicked == false ? 0 : Settings.ConditionID;
                sendPodata.ExpeditorID = Settings.ExpeditorID = Settings.IsSearchClicked == false ? 0 : Settings.ExpeditorID;
                sendPodata.PriorityID = Settings.PriorityID = Settings.IsSearchClicked == false ? 0 : Settings.PriorityID;
                sendPodata.ResourceID = Settings.ResourceID = Settings.IsSearchClicked == false ? 0 : Settings.ResourceID;
                sendPodata.TagNo = Settings.TAGNo = Settings.IsSearchClicked == false ? string.Empty : Settings.TAGNo;
                sendPodata.IdentCode = Settings.IdentCodeNo = Settings.IsSearchClicked == false ? string.Empty : Settings.IdentCodeNo;
                sendPodata.BagNo = Settings.BagNo = Settings.IsSearchClicked == false ? string.Empty : Settings.BagNo;
                sendPodata.yBkgNumber = Settings.Ybkgnumber = Settings.IsSearchClicked == false ? string.Empty : Settings.Ybkgnumber;
                sendPodata.TaskName = Settings.TaskName = Settings.IsSearchClicked == false ? string.Empty : Settings.TaskName;

                defaultData.CompanyID = Settings.CompanyID;
                defaultData.UserID = Settings.userLoginID;
                defaultData.SearchCriteria = JsonConvert.SerializeObject(sendPodata);

                if (val == true)
                {
                    await SearchResultGet(sendPodata);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveAndClearSearch method -> in ParentListViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is for checking if the search value(s) has any value or is empty. 
        /// </summary>
        /// <returns></returns>
        public async Task CheckingSearchValues()
        {
            try
            {
                if (!String.IsNullOrEmpty(Settings.PONumber) || !String.IsNullOrEmpty(Settings.REQNo) || !String.IsNullOrEmpty(Settings.ShippingNo) ||
                    Settings.DisciplineID != 0 || Settings.ELevelID != 0 || Settings.ConditionID != 0 || !String.IsNullOrEmpty(Settings.TAGNo) ||
                    !String.IsNullOrEmpty(Settings.IdentCodeNo) || !String.IsNullOrEmpty(Settings.BagNo) || !string.IsNullOrEmpty(Settings.Ybkgnumber) ||
                    !string.IsNullOrEmpty(Settings.TaskName) || Settings.ResourceID != 0)
                {
                    SearchIcon = "\uf00e";
                }
                else
                {
                    SearchIcon = "\uf002";
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckingSearchValues method -> in ParentListViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method gets the search result based on search values.
        /// </summary>
        /// <param name="sendPodata"></param>
        public async Task SearchResultGet(SendPodata sendPodata, bool isfromsearchfilter = false)
        {
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    SearchSetting Serchdata = new SearchSetting();

                    if (isfromsearchfilter == true)
                    {
                        Serchdata = await trackService.GetSavedUserSearchSettingsByID(SelectedFilterName);

                        if (Serchdata != null && Serchdata.data != null && !string.IsNullOrEmpty(Serchdata.data.SearchCriteria))
                        {
                            SearchPassData defaultData = new SearchPassData();
                            defaultData.SearchName = !string.IsNullOrEmpty(SelectedFilterName?.Name) ? SelectedFilterName?.Name : null;
                            defaultData.ID = SelectedFilterName != null && SelectedFilterName.ID != 0 ? SelectedFilterName.ID : 0;
                            defaultData.UserID = Settings.userLoginID;
                            defaultData.CompanyID = Settings.CompanyID;
                            defaultData.ProjectID = Settings.ProjectID;
                            defaultData.JobID = Settings.JobID;
                            defaultData.SearchCriteria = Serchdata.data.SearchCriteria;
                            var responseData = await trackService.SaveSerchvaluesSetting(defaultData);
                        }
                    }
                    else
                    {
                        Serchdata = await trackService.GetSearchValuesService(Settings.userLoginID);
                    }

                    if (Serchdata?.status == 1 && !string.IsNullOrEmpty(Serchdata.data.SearchCriteria))
                    {
                        var searchC = JsonConvert.DeserializeObject<SendPodata>(Serchdata.data.SearchCriteria);

                        if (searchC != null)
                        {
                            //Key
                            sendPodata.PONumber = Settings.PONumber = Settings.IsSearchClicked == false ? searchC.PONumber : Settings.PONumber;
                            sendPodata.REQNo = Settings.REQNo = Settings.IsSearchClicked == false ? searchC.REQNo : Settings.REQNo;
                            sendPodata.ShippingNo = Settings.ShippingNo = Settings.IsSearchClicked == false ? searchC.ShippingNo : Settings.ShippingNo;
                            sendPodata.DisciplineID = Settings.DisciplineID = Settings.IsSearchClicked == false ? searchC.DisciplineID : Settings.DisciplineID;
                            sendPodata.ELevelID = Settings.ELevelID = Settings.IsSearchClicked == false ? searchC.ELevelID : Settings.ELevelID;
                            sendPodata.ConditionID = Settings.ConditionID = Settings.IsSearchClicked == false ? searchC.ConditionID : Settings.ConditionID;
                            sendPodata.ExpeditorID = Settings.ExpeditorID = Settings.IsSearchClicked == false ? searchC.ExpeditorID : Settings.ExpeditorID;
                            sendPodata.PriorityID = Settings.PriorityID = Settings.IsSearchClicked == false ? searchC.PriorityID : Settings.PriorityID;
                            sendPodata.ResourceID = Settings.ResourceID = Settings.IsSearchClicked == false ? searchC.ResourceID : Settings.ResourceID;
                            sendPodata.TagNo = Settings.TAGNo = Settings.IsSearchClicked == false ? searchC.TagNo : Settings.TAGNo;
                            sendPodata.IdentCode = Settings.IdentCodeNo = Settings.IsSearchClicked == false ? searchC.IdentCode : Settings.IdentCodeNo;
                            sendPodata.BagNo = Settings.BagNo = Settings.IsSearchClicked == false ? searchC.BagNo : Settings.BagNo;
                            sendPodata.yBkgNumber = Settings.Ybkgnumber = Settings.IsSearchClicked == false ? searchC.yBkgNumber : Settings.Ybkgnumber;
                            sendPodata.TaskName = Settings.TaskName = Settings.IsSearchClicked == false ? searchC.TaskName : Settings.TaskName;
                        }
                    }
                    else
                    {
                        await SaveAndClearSearch(sendPodata, false);
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
                YPSLogger.ReportException(ex, "SearchResultGet method -> in ParentListViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
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
                        var company = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Company.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var project = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Project.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var job = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Job.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var poid = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.PONumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ShippingNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.REQNo.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var starttime = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.StartTime.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var endtime = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.EndTime.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        //var resource = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Resource.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var home = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Home.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var jobs = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Jobs.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var parts = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Parts.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var load = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Load.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var savedsearchfilters = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.SavedSearchFilters.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var pending = labelval.Where(wr => wr.FieldID == labelobj.Pending.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var inprogress = labelval.Where(wr => wr.FieldID == labelobj.Inprogress.Name.Trim().Replace(" ", "")).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var complete = labelval.Where(wr => wr.FieldID == labelobj.Completed.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var all = labelval.Where(wr => wr.FieldID == labelobj.All.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.Company.Name = (company != null ? (!string.IsNullOrEmpty(company.LblText) ? company.LblText : labelobj.Company.Name) : labelobj.Company.Name) + " :";
                        labelobj.Company.Status = company?.Status == 1 || company?.Status == 2 ? true : false;
                        labelobj.Project.Name = (project != null ? (!string.IsNullOrEmpty(project.LblText) ? project.LblText : labelobj.Project.Name) : labelobj.Project.Name) + " :";
                        labelobj.Project.Status = project?.Status == 1 || project?.Status == 2 ? true : false;
                        labelobj.Job.Name = (job != null ? (!string.IsNullOrEmpty(job.LblText) ? job.LblText : labelobj.Job.Name) : labelobj.Job.Name) + " :";
                        labelobj.Job.Status = job?.Status == 1 || job?.Status == 2 ? true : false;

                        labelobj.PONumber.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.PONumber.Name) : labelobj.PONumber.Name) + " :";
                        labelobj.PONumber.Status = poid?.Status == 1 || poid?.Status == 2 ? true : false;
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber?.Status == 1 || shippingnumber?.Status == 2 ? true : false;
                        labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                        labelobj.REQNo.Status = reqnumber?.Status == 1 || reqnumber?.Status == 2 ? true : false;
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme?.Status == 1 || taskanme?.Status == 2 ? true : false;
                        labelobj.StartTime.Name = (starttime != null ? (!string.IsNullOrEmpty(starttime.LblText) ? starttime.LblText : labelobj.StartTime.Name) : labelobj.StartTime.Name);
                        labelobj.StartTime.Status = starttime?.Status == 1 || starttime?.Status == 2 ? true : false;
                        labelobj.EndTime.Name = (endtime != null ? (!string.IsNullOrEmpty(endtime.LblText) ? endtime.LblText : labelobj.EndTime.Name) : labelobj.EndTime.Name) /*+ " :"*/;
                        labelobj.EndTime.Status = endtime?.Status == 1 || endtime?.Status == 2 ? true : false;
                        labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                        labelobj.EventName.Status = eventname?.Status == 1 || eventname?.Status == 2 ? true : false;
                        //labelobj.Resource.Name = (resource != null ? (!string.IsNullOrEmpty(resource.LblText) ? resource.LblText : labelobj.Resource.Name) : labelobj.Resource.Name) + " :";

                        labelobj.Home.Name = (home != null ? (!string.IsNullOrEmpty(home.LblText) ? home.LblText : labelobj.Home.Name) : labelobj.Home.Name);
                        labelobj.Jobs.Name = (jobs != null ? (!string.IsNullOrEmpty(jobs.LblText) ? jobs.LblText : labelobj.Jobs.Name) : labelobj.Jobs.Name);
                        labelobj.Load.Name = (load != null ? (!string.IsNullOrEmpty(load.LblText) ? load.LblText : labelobj.Load.Name) : labelobj.Load.Name);
                        labelobj.Parts.Name = (parts != null ? (!string.IsNullOrEmpty(parts.LblText) ? parts.LblText : labelobj.Parts.Name) : labelobj.Parts.Name);

                        //labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";

                        labelobj.SavedSearchFilters.Name = (savedsearchfilters != null ? (!string.IsNullOrEmpty(savedsearchfilters.LblText) ? savedsearchfilters.LblText : labelobj.SavedSearchFilters.Name) : labelobj.SavedSearchFilters.Name);

                        labelobj.Pending.Name = (pending != null ? (!string.IsNullOrEmpty(pending.LblText) ? pending.LblText : labelobj.Pending.Name) : labelobj.Pending.Name);
                        labelobj.Pending.Status = pending == null ? true : (pending.Status == 1 ? true : false);
                        labelobj.Inprogress.Name = (inprogress != null ? (!string.IsNullOrEmpty(inprogress.LblText) ? inprogress.LblText : labelobj.Inprogress.Name) : labelobj.Inprogress.Name);
                        labelobj.Inprogress.Status = inprogress == null ? true : (inprogress.Status == 1 ? true : false);
                        labelobj.Completed.Name = (complete != null ? (!string.IsNullOrEmpty(complete.LblText) ? complete.LblText : labelobj.Completed.Name) : labelobj.Completed.Name);
                        labelobj.Completed.Status = complete == null ? true : (complete.Status == 1 ? true : false);
                        labelobj.All.Name = (all != null ? (!string.IsNullOrEmpty(all.LblText) ? all.LblText : labelobj.All.Name) : labelobj.All.Name);
                        labelobj.All.Status = all == null ? true : (all.Status == 1 ? true : false);
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
                YPSLogger.ReportException(ex, "ChangeLabel method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        #region Properties
        private bool _IsSearchFilterIconVisible { set; get; }
        public bool IsSearchFilterIconVisible
        {
            get
            {
                return _IsSearchFilterIconVisible;
            }
            set
            {
                this._IsSearchFilterIconVisible = value;
                RaisePropertyChanged("IsSearchFilterIconVisible");
            }
        }

        private ObservableCollection<SearchPassData> _SearchFilterList;
        public ObservableCollection<SearchPassData> SearchFilterList
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
                IsSearchFilterListVisible = false;
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

        private bool _IsSearchFilterListVisible { set; get; } = false;
        public bool IsSearchFilterListVisible
        {
            get
            {
                return _IsSearchFilterListVisible;
            }
            set
            {
                this._IsSearchFilterListVisible = value;
                RaisePropertyChanged("IsSearchFilterListVisible");
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
                RaisePropertyChanged("IsLoadTabVisible");
            }
        }

        public bool _IsPluploadVisible = true;
        public bool IsPluploadVisible
        {
            get => _IsPluploadVisible;
            set
            {
                _IsPluploadVisible = value;
                RaisePropertyChanged("IsPluploadVisible");
            }
        }

        public string _PendingText;
        public string PendingText
        {
            get => _PendingText;
            set
            {
                _PendingText = value;
                RaisePropertyChanged("PendingText");
            }
        }

        public string _InProgress;
        public string InProgress
        {
            get => _InProgress;
            set
            {
                _InProgress = value;
                RaisePropertyChanged("InProgress");
            }
        }

        public string _Completed;
        public string Completed
        {
            get => _Completed;
            set
            {
                _Completed = value;
                RaisePropertyChanged("Completed");
            }
        }

        public string _All;
        public string All
        {
            get => _All;
            set
            {
                _All = value;
                RaisePropertyChanged("All");
            }
        }

        private bool _InProgressTabVisibility = false;
        public bool InProgressTabVisibility
        {
            get => _InProgressTabVisibility;
            set
            {
                _InProgressTabVisibility = value;
                RaisePropertyChanged("InProgressTabVisibility");
            }
        }

        private bool _CompleteTabVisibility = false;
        public bool CompleteTabVisibility
        {
            get => _CompleteTabVisibility;
            set
            {
                _CompleteTabVisibility = value;
                RaisePropertyChanged("CompleteTabVisibility");
            }
        }

        private bool _PendingTabVisibility = false;
        public bool PendingTabVisibility
        {
            get => _PendingTabVisibility;
            set
            {
                _PendingTabVisibility = value;
                RaisePropertyChanged("PendingTabVisibility");
            }
        }

        private bool _AllTabVisibility = true;
        public bool AllTabVisibility
        {
            get => _AllTabVisibility;
            set
            {
                _AllTabVisibility = value;
                RaisePropertyChanged("AllTabVisibility");
            }
        }

        private string _Cmptext = "COMPLETED";
        public string Cmptext
        {
            get => _Cmptext;
            set
            {
                _Cmptext = value;
                RaisePropertyChanged("Cmptext");
            }
        }

        private string _Pendingtext = "PENDING";
        public string Pendingtext
        {
            get => _Pendingtext;
            set
            {
                _Pendingtext = value;
                RaisePropertyChanged("Pendingtext");
            }
        }

        private string _INPtext = "IN PROGRESS";
        public string INPtext
        {
            get => _INPtext;
            set
            {
                _INPtext = value;
                RaisePropertyChanged("INPtext");
            }
        }

        private string _Alltext = "ALL \n";
        public string Alltext
        {
            get => _Alltext;
            set
            {
                _Alltext = value;
                RaisePropertyChanged("Alltext");
            }
        }

        private Color _PendingTabTextColor = Color.Black;
        public Color PendingTabTextColor
        {
            get => _PendingTabTextColor;
            set
            {
                _PendingTabTextColor = value;
                RaisePropertyChanged("PendingTabTextColor");
            }
        }

        private Color _InProgressTabTextColor = Color.Black;
        public Color InProgressTabTextColor
        {
            get => _InProgressTabTextColor;
            set
            {
                _InProgressTabTextColor = value;
                RaisePropertyChanged("InProgressTabTextColor");
            }
        }
        private Color _CompleteTabTextColor = Color.Black;
        public Color CompleteTabTextColor
        {
            get => _CompleteTabTextColor;
            set
            {
                _CompleteTabTextColor = value;
                RaisePropertyChanged("CompleteTabTextColor");
            }
        }
        private Color _AllTabTextColor = Settings.Bar_Background;
        public Color AllTabTextColor
        {
            get => _AllTabTextColor;
            set
            {
                _AllTabTextColor = value;
                RaisePropertyChanged("AllTabTextColor");
            }
        }

        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields Job { get; set; } = new DashboardLabelFields { Status = true, Name = "Job" };
            public DashboardLabelFields Project { get; set; } = new DashboardLabelFields { Status = true, Name = "Project" };
            public DashboardLabelFields Company { get; set; } = new DashboardLabelFields { Status = true, Name = "Company" };
            public DashboardLabelFields Supplier { get; set; } = new DashboardLabelFields { Status = false, Name = "SupplierCompanyName" };
            public DashboardLabelFields PONumber { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "PONumber"
            };
            public DashboardLabelFields REQNo { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "REQNo"
            };
            public DashboardLabelFields ShippingNumber { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "ShippingNumber"
            };
            public DashboardLabelFields TagDescription { get; set; } = new DashboardLabelFields
            {
                Status = false,
                //Name = "TagDescription"
                Name = "IDENT_DEVIATED_TAG_DESC"
            };
            public DashboardLabelFields TaskName { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "TaskName"
            };

            public DashboardLabelFields Resource { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "Resource"
            };

            public DashboardLabelFields EventName { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "Event"
            };
            public DashboardLabelFields StartTime { get; set; } = new DashboardLabelFields { Status = false, Name = "Start Time" };
            public DashboardLabelFields EndTime { get; set; } = new DashboardLabelFields { Status = false, Name = "End Time" };

            public DashboardLabelFields Pending { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMPending" };
            public DashboardLabelFields Inprogress { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMProgress" };
            public DashboardLabelFields Completed { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMDone" };
            public DashboardLabelFields All { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMAll" };

            public DashboardLabelFields Home { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMHome" };
            public DashboardLabelFields Jobs { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMTask" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMParts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMLoad" };
            public DashboardLabelFields SavedSearchFilters { get; set; } = new DashboardLabelFields { Status = true, Name = "Saved Search Filters" };
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

        public string _PageSizeFromDDL;
        public string PageSizeFromDDL
        {
            get => _PageSizeFromDDL;
            set
            {
                _PageSizeFromDDL = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<AllPoData> _PoData;
        public ObservableCollection<AllPoData> PoDataCollections
        {
            get { return _PoData; }
            set
            {
                _PoData = value;
                RaisePropertyChanged("PoDataCollections");
            }
        }

        private ObservableCollection<AllPoData> _PoDataChild;
        public ObservableCollection<AllPoData> PoDataChildCollections
        {
            get { return _PoDataChild; }
            set
            {
                _PoDataChild = value;
                RaisePropertyChanged("PoDataChildCollections");
            }
        }

        private bool _RefreshDisable = true;
        public bool RefreshDisable
        {
            get { return _RefreshDisable; }
            set
            {
                _RefreshDisable = value;
                RaisePropertyChanged("RefreshDisable");
            }
        }

        private bool _Archivedchat = false;
        public bool Archivedchat
        {
            get { return _Archivedchat; }
            set
            {
                _Archivedchat = value;
                RaisePropertyChanged("Archivedchat");
            }
        }

        private bool _SearchDisable = false;
        public bool SearchDisable
        {
            get { return _SearchDisable; }
            set
            {
                _SearchDisable = value;
                RaisePropertyChanged("SearchDisable");
            }
        }

        private bool _profileSettingVisible = false;
        public bool profileSettingVisible
        {
            get { return _profileSettingVisible; }
            set
            {
                _profileSettingVisible = value;
                RaisePropertyChanged("profileSettingVisible");
            }
        }

        private bool _popup = false;
        public bool popup
        {
            get { return _popup; }
            set
            {
                _popup = value;
                NotifyPropertyChanged();
            }
        }

        private bool _columnsStack = false;
        public bool columnsStack
        {
            get { return _columnsStack; }
            set
            {
                _columnsStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _mainStack = true;
        public bool mainStack
        {
            get { return _mainStack; }
            set
            {
                _mainStack = value;
                NotifyPropertyChanged();
            }
        }

        private string _boolImage = "unchecked_ic.png";
        public string boolImage
        {
            get { return _boolImage; }
            set
            {
                _boolImage = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ColumnInfo> _showColumns;
        public ObservableCollection<ColumnInfo> showColumns
        {
            get { return _showColumns; }
            set
            {
                _showColumns = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ColumnInfo> _showColumnsstatus;
        public ObservableCollection<ColumnInfo> showColumnsstatus
        {
            get { return _showColumnsstatus; }
            set
            {
                _showColumnsstatus = value;
                NotifyPropertyChanged();
            }
        }

        private bool _collapseVisible = true;
        public bool collapseVisible
        {
            get { return _collapseVisible; }
            set
            {
                _collapseVisible = value;
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

        private bool _noRecordsLbl = false;
        public bool NoRecordsLbl
        {
            get { return _noRecordsLbl; }
            set
            {
                _noRecordsLbl = value;
                NotifyPropertyChanged();
            }
        }

        private int _ListCount;
        public int ListCount
        {
            get { return _ListCount; }
            set
            {
                _ListCount = value;
                RaisePropertyChanged("ListCount");
            }
        }

        private int _PageCount;
        public int PageCount
        {
            get { return _PageCount; }
            set
            {
                _PageCount = value;
                RaisePropertyChanged("PageCount");
            }
        }

        private int _NumericButtonCount;
        public int NumericButtonCount
        {
            get { return _NumericButtonCount; }
            set
            {
                _NumericButtonCount = value;
                RaisePropertyChanged("NumericButtonCount");
            }
        }

        private bool _ISPoDataListVisible = false;
        public bool ISPoDataListVisible
        {
            get { return _ISPoDataListVisible; }
            set
            {
                _ISPoDataListVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _DataGridShowColumn = true;
        public bool DataGridShowColumn
        {
            get { return _DataGridShowColumn; }
            set
            {
                _DataGridShowColumn = value;
                NotifyPropertyChanged();
            }
        }

        private string _SearchIcon = "\uf002";
        public string SearchIcon
        {
            get { return _SearchIcon; }
            set
            {
                _SearchIcon = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ClearSearchLbl = false;
        public bool ClearSearchLbl
        {
            get { return _ClearSearchLbl; }
            set
            {
                _ClearSearchLbl = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsShipPrinterhide = false;
        public bool IsShipPrinterhide
        {
            get { return _IsShipPrinterhide; }
            set
            {
                _IsShipPrinterhide = value;
                NotifyPropertyChanged();
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

        private string _CompanyName = Settings.CompanySelected;
        public string CompanyName
        {
            get { return _CompanyName; }
            set
            {
                _CompanyName = value;
                RaisePropertyChanged("Company");
            }
        }

        private string _ProjectName = Settings.ProjectSelected;
        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                _ProjectName = value;
                RaisePropertyChanged("ProjectName");
            }
        }

        private string _JobName = Settings.JobSelected;
        public string JobName
        {
            get { return _JobName; }
            set
            {
                _JobName = value;
                RaisePropertyChanged("JobName");
            }
        }

        private AllPoData _PODetails;
        public AllPoData PODetails
        {
            get => _PODetails;
            set
            {
                _PODetails = value;
                //SelectedParentDetails();
            }
        }

        private bool _loadingindicator = false;
        public bool loadingindicator
        {
            get => _loadingindicator; set
            {
                _loadingindicator = value;
                RaisePropertyChanged("loadingindicator");
            }
        }

        private bool _IsParentStackListVisible = true;
        public bool IsParentStackListVisible
        {
            get => _IsParentStackListVisible; set
            {
                _IsParentStackListVisible = value;
                RaisePropertyChanged("IsParentStackListVisible");
            }
        }

        private bool _IsChildStackListVisible = false;
        public bool IsChildStackListVisible
        {
            get => _IsChildStackListVisible; set
            {
                _IsChildStackListVisible = value;
                RaisePropertyChanged("IsChildStackListVisible");
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

        #endregion
    }
}
