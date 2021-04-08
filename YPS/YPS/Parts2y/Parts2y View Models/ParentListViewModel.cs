using Newtonsoft.Json;
using Syncfusion.SfDataGrid.XForms;
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
//using YPS.Views;
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
        public ICommand btn_close { get; set; }
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
                YPSLogger.TrackEvent("ParentListViewModel", "page loading " + DateTime.Now + " UserId: " + Settings.userLoginID);
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

                Task.Run(() => ChangeLabel()).Wait();
                Task.Run(() => BindGridData(false, false, -1)).Wait();

                #region Assigning methods to the respective ICommands
                tab_ClearSearch = new Command(async () => await ClearSearch());
                tap_OnChat = new Command(tap_eachMessage);
                tap_OnCamB = new Command(tap_eachCamB);
                tap_OnCamA = new Command(tap_eachCamA);
                tap_FileUpload = new Command(tap_eachFileUpload);
                btn_close = new Command(closeBtn);
                btn_done = new Command(doneBtn);
                IsRequired_Tap_IC = new Command(IsRequired_Tap);
                IsNotRequired_Tap_IC = new Command(IsNotRequired_Tap);
                PrintShippingMarkReportCmd = new Command(PrintShippingMarkReport);
                GroupeFileUploadCmd = new Command(GroupFileUpload);
                SelectedPOCmd = new Command(SelectedParentDetails);
                InProgressCmd = new Command(async () => await InProgress_Tap());
                CompletedCmd = new Command(async () => await Complete_Tap());
                PendingCmd = new Command(async () => await Pending_Tap());
                AllCmd = new Command(async () => await All_Tap());
                Backevnttapped = new Command(async () => await Backevnttapped_click());

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
        }


        private async Task TabChange(string tabname)
        {
            try
            {
                loadingindicator = true;
                await Task.Delay(1);

                if (tabname == "home")
                {
                    App.Current.MainPage = new MenuPage(typeof(HomePage));
                }
            }
            catch (Exception ex)
            {
                loadingindicator = false;
            }
            loadingindicator = false;
        }

        public async Task Pending_Tap()
        {
            try
            {
                await BindGridData(false, false, 0);

                PendingTabVisibility = true;
                CompleteTabVisibility = InProgressTabVisibility = AllTabVisibility = false;
                PendingTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = CompleteTabTextColor = AllTabTextColor = Color.Black;

            }
            catch (Exception ex)
            {

            }
        }

        public async Task InProgress_Tap()
        {
            try
            {
                await BindGridData(false, false, 1);

                InProgressTabVisibility = true;
                CompleteTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                InProgressTabTextColor = Settings.Bar_Background;
                CompleteTabTextColor = PendingTabTextColor = AllTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {

            }
        }

        public async Task Complete_Tap()
        {
            try
            {
                await BindGridData(false, false, 2);

                CompleteTabVisibility = true;
                InProgressTabVisibility = PendingTabVisibility = AllTabVisibility = false;
                CompleteTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = PendingTabTextColor = AllTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {

            }
        }

        public async Task All_Tap()
        {
            try
            {
                await BindGridData(false, false, -1);

                AllTabVisibility = true;
                InProgressTabVisibility = CompleteTabVisibility = PendingTabVisibility = false;
                AllTabTextColor = Settings.Bar_Background;
                InProgressTabTextColor = CompleteTabTextColor = PendingTabTextColor = Color.Black;
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<GetPoData> GetRefreshedData()
        {
            GetPoData result = new GetPoData();
            try
            {
                sendPodata = new SendPodata();
                sendPodata.UserID = Settings.userLoginID;
                sendPodata.PageSize = Settings.pageSizeYPS;
                sendPodata.StartPage = Settings.startPageYPS;
                SearchResultGet(sendPodata);

                result = await trackService.LoadPoDataService(sendPodata);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// This method is for binding the records/date to the data grid.
        /// </summary>
        /// </summary>
        /// <param name="iSPagingNoAuto"></param>
        /// <returns></returns>
        public async Task BindGridData(bool iscall, bool ISRefresh, int postatus)
        {
            try
            {
                //await Task.Delay(10);
                //UserDialogs.Instance.ShowLoading("Loading...");
                loadingindicator = true;

                YPSLogger.TrackEvent("MainPage.xaml.cs", "in BindGridData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    bool ifNotRootedDevie;

                    ifNotRootedDevie = true;/// need to remove this line when run in device 

                    if (ifNotRootedDevie)
                    {

                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            var result = await GetRefreshedData();

                            if (result != null)
                            {
                                if (result.status != 0 && result.data.allPoData != null)
                                {
                                    Settings.AllPOData = new ObservableCollection<AllPoData>();
                                    Settings.AllPOData = result.data.allPoData;

                                    var groubbyval = result.data.allPoData.GroupBy(gb => new { gb.POID, gb.TaskID });
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
                                        groupdata.StartTime = val.Select(s => s.StartTime).FirstOrDefault();
                                        groupdata.EndTime = val.Select(s => s.EndTime).FirstOrDefault();
                                        groupdata.TaskID = val.Select(s => s.TaskID).FirstOrDefault();
                                        groupdata.TaskStatus = val.Select(s => s.TaskStatus).FirstOrDefault();
                                        groupdata.TaskResourceName = val.Select(s => s.TaskResourceName).FirstOrDefault();
                                        groupdata.POTaskStatusIcon = null;

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

                                        groupdata.IsTaskResourceVisible = val.Select(c => c.TaskResourceID).FirstOrDefault() == Settings.userLoginID ? false : true;

                                        groupedlist.Add(groupdata);
                                    }

                                    groupedlist = new ObservableCollection<AllPoData>(groupedlist.OrderBy(o => o.TaskStatus).
                                        ThenBy(tob => tob.TaskName));

                                    if (postatus == -1)
                                    {
                                        PoDataCollections = new ObservableCollection<AllPoData>(groupedlist);
                                    }
                                    else
                                    {
                                        PoDataCollections = new ObservableCollection<AllPoData>(groupedlist.Where(wr => wr.TaskStatus == postatus));
                                    }

                                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                                    if (labelval.Count > 0)
                                    {
                                        var pending = labelval.Where(wr => wr.FieldID == labelobj.Pending.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                                        var inprogress = labelval.Where(wr => wr.FieldID == labelobj.Inprogress.Name.Trim().Replace(" ", "")).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                                        var complete = labelval.Where(wr => wr.FieldID == labelobj.Completed.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                                        var all = labelval.Where(wr => wr.FieldID == labelobj.All.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                                        labelobj.Pending.Name = (pending != null ? (!string.IsNullOrEmpty(pending.LblText) ? pending.LblText : labelobj.Pending.Name) : labelobj.Pending.Name);
                                        labelobj.Pending.Status = pending == null ? true : (pending.Status == 1 ? true : false);
                                        labelobj.Inprogress.Name = (inprogress != null ? (!string.IsNullOrEmpty(inprogress.LblText) ? inprogress.LblText : labelobj.Inprogress.Name) : labelobj.Inprogress.Name);
                                        labelobj.Inprogress.Status = inprogress == null ? true : (inprogress.Status == 1 ? true : false);
                                        labelobj.Completed.Name = (complete != null ? (!string.IsNullOrEmpty(complete.LblText) ? complete.LblText : labelobj.Completed.Name) : labelobj.Completed.Name);
                                        labelobj.Completed.Status = complete == null ? true : (complete.Status == 1 ? true : false);
                                        labelobj.All.Name = (all != null ? (!string.IsNullOrEmpty(all.LblText) ? all.LblText : labelobj.All.Name) : labelobj.All.Name);
                                        labelobj.All.Status = all == null ? true : (all.Status == 1 ? true : false);
                                    }

                                    if (PoDataCollections.Count > 0)
                                    {
                                        ListCount = result.data.listCount;
                                        var roundcount = Math.Ceiling((decimal)ListCount / (decimal)Settings.pageSizeYPS);
                                        PageCount = (Int32)roundcount;
                                        NumericButtonCount = (Int32)roundcount;

                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            if (Device.RuntimePlatform == Device.Android)
                                            {
                                                if (NumericButtonCount < 6 && NumericButtonCount != 0)
                                                {
                                                    NumericButtonsGenerateMode = NumericButtonsGenerateMode.Auto;
                                                }
                                                else
                                                {
                                                    NumericButtonsGenerateMode = NumericButtonsGenerateMode.Manual;
                                                }
                                            }
                                        });

                                        await CheckingSearchValues();
                                        //loadingindicator = false;
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
                                    iscall = false;
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
                            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }
                    }
                    else
                    {
                        Exception ex = new Exception();
                        YPSLogger.ReportException(ex, "BindGridData method ->Your phone is rooted , please unroot to use app in ParentListViewModel.cs ");
                        await App.Current.MainPage.DisplayAlert("Warning", "Your phone is rooted , please unroot to use app", "OK");
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
                YPSLogger.ReportException(ex, "BindGridData method -> in ParentListViewModel.cs.cs " + Settings.userLoginID);
                loadingindicator = false;
            }
            finally
            {
                loadingindicator = false;
            }
        }

        public async void SelectedParentDetails()
        {
            try
            {
                if (PODetails != null)
                {
                    try
                    {
                        loadingindicator = true;
                        PODetails.SelectedTagBorderColor = Settings.Bar_Background;
                        var PoDataChilds = new ObservableCollection<AllPoData>(
                            Settings.AllPOData.Where(wr => wr.POID == PODetails.POID && wr.TaskID == PODetails.TaskID).ToList()
                            );

                        await Navigation.PushAsync(new POChildListPage(PoDataChilds, sendPodata));
                    }
                    catch (Exception ex)
                    {

                    }
                    finally
                    {
                        loadingindicator = false;
                    }
                }
            }
            catch (Exception ex)
            {
                loadingindicator = false;
            }
        }

        public async void PrintShippingMarkReport(object sender)
        {
            try
            {
                if (!loadingindicator)
                {
                    loadingindicator = true;
                    await Task.Delay(100);

                    //if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                    //{
                    //    DependencyService.Get<IToastMessage>().ShortAlert("You don't have access ship Print");
                    //}
                    //else
                    //{
                    var senderval = sender as AllPoData;
                    //var tapedItem = PoDataCollections.Where(x => x.POShippingNumber == senderval.POShippingNumber).FirstOrDefault();

                    YPSService yPSService = new YPSService();
                    var printResult = await yPSService.PrintPDFByUsingPOID(senderval.POID);

                    PrintPDFModel printPDFModel = new PrintPDFModel();

                    if (printResult.status != 0 && printResult != null)
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
                                await Navigation.PushAsync(new PdfViewPage(printPDFModel));
                                break;
                        }
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_shipmarkPrinter method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                loadingindicator = false;
            }
            loadingindicator = false;
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
                //var seletedData = PoDataCollections.Where(x => x.POShippingNumber == sender.ClassId).FirstOrDefault();

                if (Settings.fileUploadPageCount == 0)
                {
                    StartUploadFileModel poShipNum_obj = new StartUploadFileModel();
                    poShipNum_obj.alreadyExit = senderval.POShippingNumber;
                    Settings.fileUploadPageCount = 1;
                    Settings.isFinalvol = senderval.ISFinalVol;
                    await Navigation.PushAsync(new FileUpload(poShipNum_obj, senderval.POID, senderval.FUID,
                        "plFile", false));
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

        public async Task Backevnttapped_click()
        {
            try
            {
                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// This method is called when clicked on PhotoRequired icon. 
        /// </summary>
        /// <param name="obj"></param>
        public async void IsRequired_Tap(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in IsRequired_Tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {

                var dataGrid = obj as SfDataGrid;
                var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                var uniq = data.GroupBy(x => x.POShippingNumber);

                if (uniq.Count() >= 2)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please select any one group.");
                }
                else if (uniq.Count() == 0)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please select tag.");
                }
                else if (uniq.Count() == 1)
                {
                    var requiredDataCount = from s in data
                                            where s.TagAPhotoCount > 0 || s.TagBPhotoCount > 0 || s.ISPhotoClosed == 1
                                            select s;
                    if (requiredDataCount.Count() > 0)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Photo upload is already created.");
                    }
                    else
                    {
                        List<PhotoTag> lstVal = new List<PhotoTag>();
                        string poTagIDs = "";
                        foreach (var podata in uniq)
                        {

                            foreach (var item in podata)
                            {
                                poTagIDs += item.POTagID + ",";
                                lstVal.Add(new PhotoTag() { POTagID = item.POTagID });
                            }
                        }
                        poTagIDs = poTagIDs.TrimEnd(',');

                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            if (!String.IsNullOrEmpty(poTagIDs))
                            {
                                YPSService yPSService = new YPSService();
                                var result = await yPSService.IsRequiredOrNotReuired(poTagIDs, 1);

                                if (result != null)
                                {
                                    if (result.message == "Required")
                                    {
                                        var updateCounts = PoDataCollections
                                       .Where(x => x.POTagID == (lstVal.Where(k => k.POTagID == x.POTagID).Select(p => p.POTagID).FirstOrDefault()))
                                       .Select(c =>
                                       {
                                           c.cameImage = "minus.png";
                                           return c;
                                       }).ToList();
                                    }
                                }

                            }

                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "IsRequired_Tap method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is called when clicked on PhotoNotRequired icon.
        /// </summary>
        /// <param name="obj"></param>
        public async void IsNotRequired_Tap(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in IsNotRequired_Tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var dataGrid = obj as SfDataGrid;
                    var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                    var uniq = data.GroupBy(x => x.POShippingNumber);

                    if (uniq.Count() >= 2)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select any one group.");
                    }
                    else if (uniq.Count() == 0)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select tag.");
                    }
                    else if (uniq.Count() == 1)
                    {
                        var requiredDataCount = from s in data
                                                where s.TagAPhotoCount > 0 || s.TagBPhotoCount > 0 || s.ISPhotoClosed == 1
                                                select s;
                        if (requiredDataCount.Count() > 0)
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Photo upload is already created.");
                        }
                        else
                        {
                            List<PhotoTag> lstVal = new List<PhotoTag>();
                            string poTagIDs = "";
                            foreach (var podata in uniq)
                            {
                                foreach (var item in podata)
                                {
                                    poTagIDs += item.POTagID + ",";
                                    lstVal.Add(new PhotoTag() { POTagID = item.POTagID });
                                }
                            }
                            poTagIDs = poTagIDs.TrimEnd(',');

                            if (!String.IsNullOrEmpty(poTagIDs))
                            {
                                YPSService yPSService = new YPSService();
                                var result = await yPSService.IsRequiredOrNotReuired(poTagIDs, 0);

                                if (result != null)
                                {
                                    if (result.message == "Not Required")
                                    {

                                        var updateCounts = PoDataCollections
                                         .Where(x => x.POTagID == (lstVal.Where(k => k.POTagID == x.POTagID).Select(p => p.POTagID).FirstOrDefault()))
                                         .Select(c =>
                                         {
                                             c.cameImage = "cross.png";
                                             return c;
                                         }).ToList();
                                    }
                                }
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
                YPSLogger.ReportException(ex, "IsNotRequired_Tap method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is called when clicked on done button from choose column popup.
        /// </summary>
        /// <param name="obj"></param>
        public async void doneBtn(object obj)
        {
            loadingindicator = true;
            popup = false;
            try
            {
                var dataGrid = obj as SfDataGrid;

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    var columnstatusdata = Settings.alllabeslvalues.Where(wh => wh.VersionID == Settings.VersionID && wh.LanguageID == Settings.LanguageID).ToList();

                    foreach (var statusitem in columnstatusdata)
                    {
                        foreach (var item in showColumns)
                        {
                            if (statusitem.FieldID == item.mappingText)
                            {

                                if (item.check == true)
                                {
                                    if (dataGrid.Columns[item.mappingText].IsHidden == true)
                                        dataGrid.Columns[item.mappingText].IsHidden = false;
                                }
                                else
                                {
                                    if (dataGrid.Columns[item.mappingText].IsHidden == false)
                                        dataGrid.Columns[item.mappingText].IsHidden = true;
                                }
                            }
                            else
                            {
                            }
                        }
                    }
                }

                ObservableCollection<ColumnInfoSave> SaveColumns = new ObservableCollection<ColumnInfoSave>();

                foreach (var item in showColumns)
                {
                    SaveColumns.Add(new ColumnInfoSave() { Column = item.mappingText, Value = item.check == false ? false : true });
                }

                SaveUserDefaultSettingsModel defaultData = new SaveUserDefaultSettingsModel();
                defaultData.UserID = Settings.userLoginID;
                defaultData.VersionID = Settings.VersionID;
                defaultData.TagColumns = JsonConvert.SerializeObject(SaveColumns); //showColumns
                var responseData = await trackService.SaveUserPrioritySetting(defaultData);
                SearchDisable = true;
                RefreshDisable = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "doneBtn method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            loadingindicator = false;
        }

        /// <summary>
        /// This method is called when clicked on the settings icon from bottom menu.
        /// </summary>
        /// <param name="obj"></param>
        public async void profile_Tap(object obj)
        {
            if (!loadingindicator)
            {
                try
                {
                    loadingindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        //await Navigation.PushAsync(new ProfileSelectionPage((int)QAType.PT));
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "profile_Tap method -> in PoDataViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    loadingindicator = false;
                }
            }
        }

        /// <summary>
        /// This method is called when clicked on close button from choose column popup.
        /// </summary>
        /// <param name="obj"></param>
        private void closeBtn(object obj)
        {
            popup = false;
            SearchDisable = true;
            RefreshDisable = true;
        }

        /// <summary>
        /// This method is called when clicked on Choose Columns.
        /// </summary>
        public void ChooseColumns()
        {
            if (!loadingindicator)
            {
                try
                {
                    CheckToolBarPopUpHideOrShow();
                    Settings.mutipleTimeClick = true;

                    popup = true;
                    SearchDisable = false;
                    RefreshDisable = false;
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "ChooseColumns method -> in PoDataViewModel! " + Settings.userLoginID);
                    trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                }
            }
        }

        #region for each time 
        /// <summary>
        /// This method is called when clicked on camera icon form data grid, to view After Packing photo(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachCamA(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachCamA method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            if (!loadingindicator)
            {
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var allPo = obj as AllPoData;

                    if (allPo.imgCamOpacityA != 0.5)
                    {
                        loadingindicator = true;
                        try
                        {
                            if (allPo.cameImage == "Chatcamera.png")
                            {
                                Settings.AphotoCount = allPo.TagAPhotoCount;
                                Settings.currentPuId = allPo.PUID;
                                //await Navigation.PushAsync(new PhotoUpload(null, allPo, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, allPo.photoTickVisible));
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamA method -> in PoDataViewModel! " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                        }
                        loadingindicator = false;
                    }
                }
                else
                {
                    loadingindicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                loadingindicator = false;
            }
        }

        /// <summary>
        /// This method is called when clicked on camera icon form data grid, to view Before Packing photo(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachCamB(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachCamB method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            if (!loadingindicator)
            {
                avoidMutiplePageOpen = true;
                var allPo = obj as AllPoData;
                bool checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    if (allPo.imgCamOpacityB != 0.5)
                    {
                        loadingindicator = true;
                        try
                        {
                            if (allPo.cameImage == "Chatcamera.png")
                            {
                                Settings.currentPuId = allPo.PUID;
                                Settings.BphotoCount = allPo.TagBPhotoCount;
                                //await Navigation.PushAsync(new PhotoUpload(null, allPo, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, allPo.photoTickVisible));
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_eachCamB method -> in PoDataViewModel! " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                        }
                    }
                }
                else
                {
                    loadingindicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                loadingindicator = false;
            }
        }

        /// <summary>
        /// This method is called when clicked on chat icon form data grid, to view chat(s).
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachMessage(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachMessage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            if (!loadingindicator)
            {
                loadingindicator = true;

                avoidMutiplePageOpen = true;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    try
                    {
                        var allPo = obj as AllPoData;

                        if (allPo.chatImage != "minus.png")
                        {
                            //await Navigation.PushAsync(new QnAlistPage(allPo.POID, allPo.POTagID, Settings.QAType));
                        }
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "tap_eachMessage method -> in PoDataViewModel! " + Settings.userLoginID);
                        var trackResult = await trackService.Handleexception(ex);
                    }
                }
                else
                {
                    loadingindicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            loadingindicator = false;
        }

        /// <summary>
        /// This method is called when clicked on attachment icon form data grid, to view uploaded file(s).
        /// </summary>
        /// <param name="obj"></param>
        public async void tap_eachFileUpload(object obj)
        {
            YPSLogger.TrackEvent("PoDataViewModel", "in tap_eachFileUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            if (!loadingindicator)
            {
                loadingindicator = true;
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    try
                    {
                        var allPo = obj as AllPoData;

                        if (allPo.fileImage != "minus.png")
                        {
                            Settings.currentFuId = allPo.FUID;
                            Settings.FilesCount = allPo.TagFilesCount;
                            //await Navigation.PushAsync(new FileUpload(null, allPo.POID, allPo.FUID, "fileUpload", allPo.fileTickVisible));
                        }
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "tap_eachFileUpload method -> in PoDataViewModel! " + Settings.userLoginID);
                        var trackResult = await trackService.Handleexception(ex);
                    }
                }
                else
                {
                    loadingindicator = false;
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            loadingindicator = false;
        }
        #endregion

        #region for bottom bar 
        /// <summary>
        /// This method is called when clicked on camera icon from bottom menu, to start uploading photo(s).
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_InitialCamera(object sender)
        {
            if (!loadingindicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in tap_InitialCamera method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    loadingindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        //{
                        var dataGrid = sender as SfDataGrid;
                        var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                        var uniq = data.GroupBy(x => x.POShippingNumber);

                        if (uniq.Count() >= 2)
                        {
                            //Please select any one group.
                            DependencyService.Get<IToastMessage>().ShortAlert("Please select tags under same po.");
                        }
                        else if (uniq.Count() == 0)
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s) to start upload photo(s).");
                        }
                        else if (uniq.Count() == 1)
                        {
                            var restricData = data.Where(r => r.cameImage == "cross.png").ToList();

                            if (restricData.Count > 0)
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Photos not required to upload for the selected tag(s).");
                            }
                            else
                            {
                                PhotoUploadModel selectedTagsData = new PhotoUploadModel();

                                foreach (var podata in uniq)
                                {
                                    var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                    selectedTagsData.POID = d.POID;
                                    List<PhotoTag> lstdat = new List<PhotoTag>();

                                    foreach (var item in podata)
                                    {
                                        if (item.TagAPhotoCount == 0 && item.TagBPhotoCount == 0 && item.PUID == 0)
                                        {
                                            PhotoTag tg = new PhotoTag();

                                            if (item.POTagID != 0)
                                            {
                                                tg.POTagID = item.POTagID;
                                                Settings.Tagnumbers = item.TagNumber;
                                                lstdat.Add(tg);
                                            }
                                        }
                                        else
                                        {
                                            selectedTagsData.alreadyExit = "alreadyExit";
                                            break;
                                        }
                                    }
                                    selectedTagsData.photoTags = lstdat;
                                    Settings.currentPoTagId_Inti = lstdat;
                                }

                                if (!String.IsNullOrEmpty(selectedTagsData.alreadyExit))
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Photo upload is already started for the selected tag(s).");
                                }
                                else
                                {
                                    if (selectedTagsData.photoTags.Count != 0)
                                    {
                                        //await Navigation.PushAsync(new PhotoUpload(selectedTagsData, null, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false));
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
                                    }
                                }
                            }
                        }
                        //}
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_InitialCamera method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    loadingindicator = false;
                }
            }
        }

        /// <summary>
        /// This method is called when clicked on chat icon from bottom menu, to start chat.
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_OnMessage(object sender)
        {
            if (!loadingindicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in tap_OnMessage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    loadingindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        //{
                        var dataGrid = sender as SfDataGrid;
                        var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                        var uniq = data.GroupBy(x => x.POShippingNumber);

                        if (uniq.Count() >= 2)
                        {
                            //Please select any one group.
                            DependencyService.Get<IToastMessage>().ShortAlert("Please select tags from same po.");
                        }
                        else if (uniq.Count() == 0)
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s) to start conversation.");
                        }
                        else if (uniq.Count() == 1)
                        {
                            ChatData selectedTagsData = new ChatData();

                            foreach (var podata in uniq)
                            {
                                var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                selectedTagsData.POID = d.POID;
                                List<Tag> lstdat = new List<Tag>();

                                foreach (var item in podata)
                                {
                                    Tag tg = new Tag();

                                    if (item.POTagID != 0)
                                    {
                                        tg.POTagID = item.POTagID;
                                        lstdat.Add(tg);
                                    }
                                }
                                selectedTagsData.tags = lstdat;
                            }
                            if (selectedTagsData.tags.Count != 0)
                            {
                                Settings.ChatuserCountImgHide = 1;
                                //await Navigation.PushAsync(new ChatUsers(selectedTagsData, true));
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("No tags available");
                            }
                        }
                        //}
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_OnMessage method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    loadingindicator = false;
                }
            }
        }

        /// <summary>
        /// This method is called when clicked on file icon from bottom menu, to start uploading file(s).
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_InitialFileUpload(object sender)
        {
            if (!loadingindicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in tap_InitialFileUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    loadingindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        //{
                        try
                        {
                            var dataGrid = sender as SfDataGrid;
                            var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                            var uniq = data.GroupBy(x => x.POShippingNumber);

                            if (uniq.Count() >= 2)
                            {
                                //Please select any one group.
                                DependencyService.Get<IToastMessage>().ShortAlert("File upload is already marked as completed for the selected tag(s).");
                            }
                            else if (uniq.Count() == 0)
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s) to start upload file(s).");
                            }
                            else if (uniq.Count() == 1)
                            {
                                StartUploadFileModel selectedTagsData = new StartUploadFileModel();
                                foreach (var podata in uniq)
                                {
                                    var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                    selectedTagsData.POID = d.POID;

                                    List<FileTag> lstdat = new List<FileTag>();
                                    foreach (var item in podata)
                                    {
                                        FileTag tg = new FileTag();
                                        if (item.TagFilesCount == 0 && item.FUID == 0)
                                        {
                                            if (item.POTagID != 0)
                                            {
                                                tg.POTagID = item.POTagID;
                                                lstdat.Add(tg);
                                            }
                                        }
                                        else
                                        {
                                            selectedTagsData.alreadyExit = "alreadyExit";
                                            break;
                                        }

                                    }
                                    selectedTagsData.FileTags = Settings.currentPoTagId_Inti_F = lstdat;
                                }

                                if (!String.IsNullOrEmpty(selectedTagsData.alreadyExit))
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("File upload is already started for the selected tag(s).");
                                }
                                else
                                {
                                    if (selectedTagsData.FileTags.Count != 0)
                                    {
                                        //await Navigation.PushAsync(new FileUpload(selectedTagsData, 0, 0, "initialFile", false));
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "tap_InitialFileUpload method -> in PoDataViewModel! " + Settings.userLoginID);
                            var trackResult = await trackService.Handleexception(ex);
                        }
                        //}
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_InitialFileUpload method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    loadingindicator = false;
                }
            }
        }

        /// <summary>
        /// This method is called when clicked on PrintTag icon from bottom menu.
        /// </summary>
        /// <param name="obj"></param>
        public async void tap_Printer(object obj)
        {
            if (!loadingindicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in tap_Printer method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    loadingindicator = true;

                    //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    //{
                    //    if (Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                    //    {
                    //    }
                    //    else
                    //    {
                    var dataGrid = obj as SfDataGrid;
                    var data = dataGrid.SelectedItems.Cast<AllPoData>().ToList();
                    var uniq = data.GroupBy(x => x.POShippingNumber);

                    if (uniq.Count() >= 2)
                    {
                        //Please select any one group.
                        DependencyService.Get<IToastMessage>().ShortAlert("please select tag(s) under same po.");
                    }
                    else if (uniq.Count() == 0)
                    {
                        //Please select tag(s) to download the report
                        DependencyService.Get<IToastMessage>().ShortAlert("Please select tag(s).");
                    }
                    else if (uniq.Count() == 1)
                    {
                        var checkInternet = await App.CheckInterNetConnection();
                        if (checkInternet)
                        {
                            string poTagID = "";
                            foreach (var podata in uniq)
                            {
                                var d = data.Where(y => y.POShippingNumber == podata.Key).FirstOrDefault();
                                foreach (var item in podata)
                                {
                                    poTagID += item.POTagID + ",";
                                }
                            }
                            poTagID = poTagID.TrimEnd(',');

                            if (poTagID != "0")
                            {
                                YPSService pSService = new YPSService();
                                var printResult = await pSService.PrintPDF(poTagID);

                                PrintPDFModel printPDFModel = new PrintPDFModel();

                                if (printResult.status != 0 && printResult != null)
                                {
                                    var bArray = printResult.data;
                                    byte[] bytes = Convert.FromBase64String(bArray);
                                    printPDFModel.bArray = bytes;
                                    printPDFModel.FileName = "PrintTag" + "_" + String.Format("{0:yyyyMMMdd_hh-mm-ss}", DateTime.Now) + ".pdf";
                                    printPDFModel.PDFFileTitle = "Print Tag";

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
                                            //await Navigation.PushAsync(new PdfViewPage(printPDFModel));
                                            break;
                                    }
                                }
                                else
                                {
                                    //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong!");
                                }
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("No tags available.");
                            }
                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }
                    }
                    //}
                    //}
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_Printer method -> in PoDataViewModel! " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    loadingindicator = false;
                }
            }
        }
        #endregion

        /// <summary>
        /// This method is called when clicked on search icon, there on ToolBar.
        /// </summary>
        public async void FilterClicked()
        {
            if (!loadingindicator)
            {
                YPSLogger.TrackEvent("PoDataViewModel", "in FilterClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    loadingindicator = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (Settings.filterPageCount == 0)
                        {
                            Settings.filterPageCount = 1;
                            Settings.refreshPage = 1;
                            await Navigation.PushAsync(new FilterData());
                        }
                    }
                    else
                    {
                        loadingindicator = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "FilterClicked method-> in PoDataViewModel! " + Settings.userLoginID);
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
                    YPSLogger.TrackEvent("PoDataViewModel", "in FilterClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    CheckToolBarPopUpHideOrShow();
                    loadingindicator = true;

                    SearchDisable = false;
                    //App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                    //await BindGridData(false, false, -1);
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
                YPSLogger.ReportException(ex, "RefreshPage -> in PoDataViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                loadingindicator = false;
            }
        }

        /// <summary>
        /// This methid is called when clicked on Archived Chat, from more options.
        /// </summary>
        public async void ArchivedChats()
        {
            try
            {
                CheckToolBarPopUpHideOrShow();
                await Navigation.PushAsync(new QnAlistPage(0, 0, (int)QAType.PT));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ArchivedChats method -> in PoDataViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
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
                YPSLogger.TrackEvent("PoDataViewModel", "in clearData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    loadingindicator = true;

                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        await SaveAndClearSearch(false);

                        if (NoRecordsLbl == true)
                        {
                            BindGridData(false, false, -1);
                        }
                        else if (NoRecordsLbl == false)
                        {
                            BindGridData(true, false, -1);
                        }

                        isloading = true;
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "ClearSearch method -> in PoDataViewModel! " + Settings.userLoginID);
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
        private async Task SaveAndClearSearch(bool val)
        {
            SendPodata SaveUserDS = new SendPodata();
            SearchPassData defaultData = new SearchPassData();
            try
            {
                //Key
                SaveUserDS.PONumber = Settings.PONumber = string.Empty;
                SaveUserDS.REQNo = Settings.REQNo = string.Empty;
                SaveUserDS.ShippingNo = Settings.ShippingNo = string.Empty;
                SaveUserDS.DisciplineID = Settings.DisciplineID = 0;
                SaveUserDS.ELevelID = Settings.ELevelID = 0;
                SaveUserDS.ConditionID = Settings.ConditionID = 0;
                SaveUserDS.ExpeditorID = Settings.ExpeditorID = 0;
                SaveUserDS.PriorityID = Settings.PriorityID = 0;
                SaveUserDS.ResourceID = Settings.ResourceID = 0;
                SaveUserDS.TagNo = Settings.TAGNo = string.Empty;
                SaveUserDS.IdentCode = Settings.IdentCodeNo = string.Empty;
                SaveUserDS.BagNo = Settings.BagNo = string.Empty;
                SaveUserDS.yBkgNumber = Settings.Ybkgnumber = string.Empty;
                SaveUserDS.TaskName = Settings.TaskName = string.Empty;
                defaultData.CompanyID = Settings.CompanyID;
                defaultData.UserID = Settings.userLoginID;
                defaultData.SearchCriteria = JsonConvert.SerializeObject(SaveUserDS);
                var responseData = await trackService.SaveSerchvaluesSetting(defaultData);

                if (val == true)
                {
                    SearchResultGet(SaveUserDS);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveAndClearSearch method -> in PoDataViewModel! " + Settings.userLoginID);
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
                if ((!String.IsNullOrEmpty(Settings.PONumber) || !String.IsNullOrEmpty(Settings.REQNo) || !String.IsNullOrEmpty(Settings.ShippingNo) || !string.IsNullOrEmpty(Settings.TaskName) ||
                   Settings.DisciplineID != 0 || Settings.ELevelID != 0 || Settings.ConditionID != 0 || Settings.ResourceID != 0 || Settings.ExpeditorID != 0 || !string.IsNullOrEmpty(Settings.Ybkgnumber)
                   || Settings.PriorityID != 0 || !String.IsNullOrEmpty(Settings.TAGNo) || !String.IsNullOrEmpty(Settings.IdentCodeNo) || !String.IsNullOrEmpty(Settings.BagNo)) && Settings.SearchWentWrong == false)
                {
                    //ClearSearchLbl = true;
                    SearchIcon = "\uf00e";
                }
                else if ((Settings.LocationPickupID != 0 || Settings.LocationPOLID != 0 || Settings.LocationPODID != 0 || Settings.LocationDeliverPlaceID != 0)
                    && Settings.SearchWentWrong == false)
                {
                    //ClearSearchLbl = true;
                    SearchIcon = "\uf00e";
                }
                else if ((!String.IsNullOrEmpty(Settings.DeliveryFrom) || !String.IsNullOrEmpty(Settings.ETDFrom) || !String.IsNullOrEmpty(Settings.ETAFrom) ||
                    !String.IsNullOrEmpty(Settings.OnsiteFrom) || !String.IsNullOrEmpty(Settings.DeliveryTo) || !String.IsNullOrEmpty(Settings.ETDTo) ||
                    !string.IsNullOrEmpty(Settings.Ybkgnumber) || !string.IsNullOrEmpty(Settings.TaskName) ||
                    !String.IsNullOrEmpty(Settings.ETATo) || !String.IsNullOrEmpty(Settings.OnsiteTo)) && Settings.SearchWentWrong == false)
                {
                    //ClearSearchLbl = true;
                    SearchIcon = "\uf00e";
                }
                else
                {
                    //ClearSearchLbl = false;
                    SearchIcon = "\uf002";
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckingSearchValues method -> in PoDataViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        #region TagColumns Binding
        /// <summary>
        /// This method binds the tags columns to the grid. 
        /// </summary>
        /// <returns></returns>
        public async Task TagColumnsBinding()
        {
            try
            {
                var getpoData = await trackService.GetUserPrioritySettings(Settings.userLoginID);

                if (getpoData != null)
                {
                    if (getpoData.status != 0)
                    {
                        var searchTags = JsonConvert.DeserializeObject<List<ColumnInfo>>(getpoData.data.TagColumns);
                        Settings.VersionID = getpoData.data.VersionID;

                        if (searchTags != null)
                        {
                            ObservableCollection<ColumnInfo> columnsData = new ObservableCollection<ColumnInfo>();

                            foreach (var name in searchTags)
                            {
                                columnsData.Add(new ColumnInfo() { mappingText = name.Column, headerText = name.Column, Value = name.Value });
                                showColumns = columnsData;
                            }
                        }
                        else
                        {
                            showColumns = null;
                        }
                    }
                    else
                    {
                        showColumns = null;

                        //if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0 && Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        //{
                        //    //pagename.GetBottomMenVal();
                        //}
                    }
                }
                else
                {
                    showColumns = null;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TagColumnsBinding method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }
        #endregion

        /// <summary>
        /// This method gets the search result based on search values.
        /// </summary>
        /// <param name="sendPodata"></param>
        public async void SearchResultGet(SendPodata sendPodata)
        {
            try
            {
                var Serchdata = await trackService.GetSearchValuesService(Settings.userLoginID);

                if (Serchdata != null)
                {
                    if (Serchdata.status == 1)
                    {
                        if (!string.IsNullOrEmpty(Serchdata.data.SearchCriteria))
                        {
                            var searchC = JsonConvert.DeserializeObject<SendPodata>(Serchdata.data.SearchCriteria);

                            if (searchC != null)
                            {
                                //Key
                                sendPodata.PONumber = Settings.PONumber = searchC.PONumber;
                                sendPodata.REQNo = Settings.REQNo = searchC.REQNo;
                                sendPodata.ShippingNo = Settings.ShippingNo = searchC.ShippingNo;
                                sendPodata.DisciplineID = Settings.DisciplineID = searchC.DisciplineID;
                                sendPodata.ELevelID = Settings.ELevelID = searchC.ELevelID;
                                sendPodata.ConditionID = Settings.ConditionID = searchC.ConditionID;
                                sendPodata.ExpeditorID = Settings.ExpeditorID = searchC.ExpeditorID;
                                sendPodata.PriorityID = Settings.PriorityID = searchC.PriorityID;
                                sendPodata.ResourceID = Settings.ResourceID = searchC.ResourceID;
                                sendPodata.TagNo = Settings.TAGNo = searchC.TagNo;
                                sendPodata.IdentCode = Settings.IdentCodeNo = searchC.IdentCode;
                                sendPodata.BagNo = Settings.BagNo = searchC.BagNo;
                                sendPodata.yBkgNumber = Settings.Ybkgnumber = searchC.yBkgNumber;
                                sendPodata.TaskName = Settings.TaskName = searchC.TaskName;

                                Settings.SearchWentWrong = false;
                            }

                        }
                        else
                        {
                            await SaveAndClearSearch(true);
                        }
                    }
                    else
                    {
                        Settings.SearchWentWrong = true;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SearchResultGet method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to show/hide the toolbar.
        /// </summary>
        public void CheckToolBarPopUpHideOrShow()
        {
            if (ToolBarItemPopUp == false)
            {
                MoreItemIconColor = Color.Gray;
                ToolBarItemPopUp = true;
            }
            else
            {
                MoreItemIconColor = Color.White;
                ToolBarItemPopUp = false;
            }
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

                        var poid = labelval.Where(wr => wr.FieldID == labelobj.POID.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID == labelobj.ShippingNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqnumber = labelval.Where(wr => wr.FieldID == labelobj.REQNo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID == labelobj.TaskName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var starttime = labelval.Where(wr => wr.FieldID == labelobj.StartTime.Name.Trim().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var endtime = labelval.Where(wr => wr.FieldID == labelobj.EndTime.Name.Trim().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var resource = labelval.Where(wr => wr.FieldID == labelobj.Resource.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var home = labelval.Where(wr => wr.FieldID == labelobj.Home.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var jobs = labelval.Where(wr => wr.FieldID == labelobj.Jobs.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var parts = labelval.Where(wr => wr.FieldID == labelobj.Parts.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var load = labelval.Where(wr => wr.FieldID == labelobj.Load.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        //var pending = labelval.Where(wr => wr.FieldID == labelobj.Pending.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        //var inprogress = labelval.Where(wr => wr.FieldID == labelobj.Inprogress.Name.Trim().Replace(" ", "")).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        //var complete = labelval.Where(wr => wr.FieldID == labelobj.Completed.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        //var all = labelval.Where(wr => wr.FieldID == labelobj.All.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();


                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.Company.Name = (company != null ? (!string.IsNullOrEmpty(company.LblText) ? company.LblText : labelobj.Company.Name) : labelobj.Company.Name) + " :";
                        labelobj.Company.Status = company == null ? true : (company.Status == 1 ? true : false);
                        labelobj.Project.Name = (project != null ? (!string.IsNullOrEmpty(project.LblText) ? project.LblText : labelobj.Project.Name) : labelobj.Project.Name) + " :";
                        labelobj.Project.Status = project == null ? true : (project.Status == 1 ? true : false);
                        labelobj.Job.Name = (job != null ? (!string.IsNullOrEmpty(job.LblText) ? job.LblText : labelobj.Job.Name) : labelobj.Job.Name) + " :";
                        labelobj.Job.Status = job == null ? true : (job.Status == 1 ? true : false);

                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid == null ? true : (poid.Status == 1 ? true : false);
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber == null ? true : (shippingnumber.Status == 1 ? true : false);
                        labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                        labelobj.REQNo.Status = reqnumber == null ? true : (reqnumber.Status == 1 ? true : false);
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme == null ? true : (taskanme.Status == 1 ? true : false);

                        labelobj.StartTime.Name = (starttime != null ? (!string.IsNullOrEmpty(starttime.LblText) ? starttime.LblText : labelobj.StartTime.Name) : labelobj.StartTime.Name) + " :";
                        labelobj.StartTime.Status = starttime == null ? true : (starttime.Status == 1 ? true : false);
                        labelobj.EndTime.Name = (endtime != null ? (!string.IsNullOrEmpty(endtime.LblText) ? endtime.LblText : labelobj.EndTime.Name) : labelobj.EndTime.Name) + " :";
                        labelobj.EndTime.Status = endtime == null ? true : (endtime.Status == 1 ? true : false);

                        labelobj.Resource.Name = (resource != null ? (!string.IsNullOrEmpty(resource.LblText) ? resource.LblText : labelobj.Resource.Name) : labelobj.Resource.Name) + " :";


                        labelobj.Home.Name = (home != null ? (!string.IsNullOrEmpty(home.LblText) ? home.LblText : labelobj.Home.Name) : labelobj.Home.Name);
                        labelobj.Home.Status = home == null ? true : (home.Status == 1 ? true : false);
                        //labelobj.Jobs.Name = (jobs != null ? (!string.IsNullOrEmpty(jobs.LblText) ? jobs.LblText : labelobj.Jobs.Name) : labelobj.Jobs.Name);
                        //labelobj.Jobs.Status = jobs == null ? true : (jobs.Status == 1 ? true : false);
                        //labelobj.Parts.Name = (parts != null ? (!string.IsNullOrEmpty(parts.LblText) ? parts.LblText : labelobj.Parts.Name) : labelobj.Parts.Name);
                        labelobj.Parts.Status = parts == null ? true : (parts.Status == 1 ? true : false);
                        //labelobj.Load.Name = (load != null ? (!string.IsNullOrEmpty(load.LblText) ? load.LblText : labelobj.Load.Name) : labelobj.Load.Name);
                        labelobj.Load.Status = load == null ? true : (load.Status == 1 ? true : false);

                        labelobj.Load.Name = Settings.VersionID == 2 ? "Carrier" : "Load";
                        labelobj.Parts.Name = Settings.VersionID == 2 ? "VIN" : "Parts";

                        //labelobj.Pending.Name = (pending != null ? (!string.IsNullOrEmpty(pending.LblText) ? pending.LblText : labelobj.Pending.Name) : labelobj.Pending.Name) ;
                        //labelobj.Pending.Status = pending == null ? true : (pending.Status == 1 ? true : false);
                        //labelobj.Inprogress.Name = (inprogress != null ? (!string.IsNullOrEmpty(inprogress.LblText) ? inprogress.LblText : labelobj.Inprogress.Name) : labelobj.Inprogress.Name);
                        //labelobj.Inprogress.Status = inprogress == null ? true : (inprogress.Status == 1 ? true : false);
                        //labelobj.Completed.Name = (complete != null ? (!string.IsNullOrEmpty(complete.LblText) ? complete.LblText : labelobj.Completed.Name) : labelobj.Completed.Name);
                        //labelobj.Completed.Status = complete == null ? true : (complete.Status == 1 ? true : false);
                        //labelobj.All.Name = (all != null ? (!string.IsNullOrEmpty(all.LblText) ? all.LblText : labelobj.All.Name) : labelobj.All.Name);
                        //labelobj.All.Status = all == null ? true : (all.Status == 1 ? true : false);

                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    IsShippingMarkVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "ShippingMarkDownload".Trim().ToLower()).FirstOrDefault()) != null ? true : false;

                    if (Settings.VersionID == 2)
                    {
                        IsLoadTabVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CarrierInspection".Trim())
                            .FirstOrDefault()) != null && (Settings.EntityTypeName.Trim() == "Owner" || Settings.EntityTypeName.Trim() == "Dealer"
                    || Settings.EntityTypeName.Trim() == "LLP") ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabelAndShowHide method -> in ParentListViewModel.cs " + Settings.userLoginID);
            }
        }

        #region Properties
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

        public bool _IsShippingMarkVisible = true;
        public bool IsShippingMarkVisible
        {
            get => _IsShippingMarkVisible;
            set
            {
                _IsShippingMarkVisible = value;
                RaisePropertyChanged("IsShippingMarkVisible");
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
            public DashboardLabelFields Supplier { get; set; } = new DashboardLabelFields { Status = true, Name = "SupplierCompanyName" };
            public DashboardLabelFields POID { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "PONumber"
            };
            public DashboardLabelFields REQNo { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "REQNo"
            };
            public DashboardLabelFields ShippingNumber { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "ShippingNumber"
            };
            public DashboardLabelFields TagDescription { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TagDescription"
            };
            public DashboardLabelFields TaskName { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "TaskName"
            };

            public DashboardLabelFields Resource { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "Resource"
            };


            public DashboardLabelFields Pending { get; set; } = new DashboardLabelFields { Status = true, Name = "Pending" };
            public DashboardLabelFields Inprogress { get; set; } = new DashboardLabelFields { Status = true, Name = "Progress" };
            public DashboardLabelFields Completed { get; set; } = new DashboardLabelFields { Status = true, Name = "Done" };
            public DashboardLabelFields All { get; set; } = new DashboardLabelFields { Status = true, Name = "All" };

            public DashboardLabelFields Home { get; set; } = new DashboardLabelFields { Status = true, Name = "Home" };
            public DashboardLabelFields Jobs { get; set; } = new DashboardLabelFields { Status = true, Name = "Job" };
            public DashboardLabelFields Parts { get; set; } = new DashboardLabelFields { Status = true, Name = "Parts" };
            public DashboardLabelFields Load { get; set; } = new DashboardLabelFields { Status = true, Name = "Load" };
            public DashboardLabelFields StartTime { get; set; } = new DashboardLabelFields { Status = true, Name = "Start Time" };
            public DashboardLabelFields EndTime { get; set; } = new DashboardLabelFields { Status = true, Name = "End Time" };
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

        //private bool _activityIndicator;
        //public bool activityIndicator
        //{
        //    get { return _activityIndicator; }
        //    set
        //    {
        //        _activityIndicator = value;
        //        RaisePropertyChanged("activityIndicator");
        //    }
        //}

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

        private NumericButtonsGenerateMode _NumericButtonsGenerateMode;
        public NumericButtonsGenerateMode NumericButtonsGenerateMode
        {
            get { return _NumericButtonsGenerateMode; }
            set
            {
                _NumericButtonsGenerateMode = value;
                RaisePropertyChanged("NumericButtonsGenerateMode");
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

        private bool _ToolBarItemPopUp = false;
        public bool ToolBarItemPopUp
        {
            get { return _ToolBarItemPopUp; }
            set
            {
                _ToolBarItemPopUp = value;
                NotifyPropertyChanged();
            }
        }

        private Color _MoreItemIconColor = Color.White;
        public Color MoreItemIconColor
        {
            get => _MoreItemIconColor;
            set
            {
                _MoreItemIconColor = value;
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


        //private string _SupplierName = Settings.SupplierSelected;
        //public string SupplierName
        //{
        //    get { return _SupplierName; }
        //    set
        //    {
        //        _SupplierName = value;
        //        RaisePropertyChanged("SupplierName");
        //    }
        //}

        //private string _Id = Settings.CompanyID.ToString();
        //public string Id
        //{
        //    get { return _Id; }
        //    set
        //    {
        //        _UserName = value;
        //        RaisePropertyChanged("Id");
        //    }
        //}

        private AllPoData _PODetails;
        public AllPoData PODetails
        {
            get => _PODetails;
            set
            {
                _PODetails = value;
                SelectedParentDetails();
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
        #endregion
    }

    public class ModelForIsPhotoRequired
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }
}
