using Acr.UserDialogs;
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
using YPS.Model.Yship;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using YPS.Views;
using YPS.YShip.YshipViews;
using static YPS.Model.SearchModel;
using static YPS.Models.ChatMessage;

namespace YPS.YshipViewModel
{
    public class YshipPageViewModel : INotifyPropertyChanged
    {
        #region ICommand & data member declaration
        public INavigation Navigation { get; set; }
        public ICommand tap_InitialCameraCommand { set; get; }
        public ICommand tap_OnMessageCommand { set; get; }
        public ICommand tap_InitialFileUploadCommand { set; get; }
        public ICommand tap_PermitsCommand { get; set; }
        public ICommand tap_OnChat { get; set; }
        public ICommand tap_yBkgno { get; set; }
        public ICommand ChooseColumns_Tap { get; set; }
        public ICommand btn_close { get; set; }
        public ICommand btn_done { get; set; }
        public ICommand profile_Change_Tap { get; set; }
        public ICommand ShowPageSizePickerItems { get; set; }
        public ICommand filtePageNavigate { get; set; }
        public ICommand clearSearchCmd { get; set; }
        public ICommand refreshClickCmd { get; set; }
        YPSService trackService;
        bool checkInternet;
        YshipPage pageNameyShip;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="_Navigation"></param>
        public YshipPageViewModel(INavigation _Navigation, YshipPage yshippagename)
        {
            Settings.mutipleTimeClick = false;
            YPSLogger.TrackEvent("YshipPageViewModel", "page loading " + DateTime.Now + " UserId: " + Settings.userLoginID);
            Navigation = _Navigation;
            pageNameyShip = yshippagename;
            trackService = new YPSService();
            Settings.QAType = (int)QAType.YS;
            var cID = Settings.CompanyID;
            var pID = Settings.ProjectID;
            var jID = Settings.JobID;
            DataGrid = true;

            #region Assigning methods to respective ICommands
            tap_OnChat = new Command(tap_eachMessage);
            tap_yBkgno = new Command(tap_yBkgnoClick);
            btn_close = new Command(closeBtn);
            btn_done = new Command(doneBtn);
            clearSearchCmd = new Command(async () => await ClearSearch(new yShipSearch(), true));
            #endregion
        }

        /// <summary>
        /// Called when search icon is clicked, to navidate to filter page.
        /// </summary>
        /// <returns></returns>
        public async Task FilterPageNav()
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in FilterPageNav method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    activityIndicator = true;
                    Settings.mutipleTimeClick = true;

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        await Navigation.PushAsync(new yShipFilterData());
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    activityIndicator = false;
                    Settings.mutipleTimeClick = false;
                    YPSLogger.ReportException(ex, "FilterPageNav method-> in YshipPageViewModel.cs " + Settings.userLoginID);
                    var trackResult = await trackService.Handleexception(ex);
                }
                finally
                {
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method will call when clicked on the done button inside popup (Dialog).
        /// </summary>
        /// <param name="obj"></param>
        public async void doneBtn(object obj)
        {
            try
            {
                activityIndicator = true;
                popup = false;

                var dataGrid = obj as SfDataGrid;

                foreach (var item in showColumns)
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
                ObservableCollection<ColumnInfoSave> SaveColumns = new ObservableCollection<ColumnInfoSave>();

                foreach (var item in showColumns)
                {
                    SaveColumns.Add(new ColumnInfoSave() { Column = item.mappingText, Value = item.check == false ? false : true });
                }

                SaveUserDefaultSettingsModel defaultData = new SaveUserDefaultSettingsModel();
                defaultData.UserID = Settings.userLoginID;
                defaultData.VersionID = Settings.VersionID;
                defaultData.yShipColumns = JsonConvert.SerializeObject(SaveColumns);
                var responseData = await trackService.SaveUserPrioritySetting(defaultData);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "doneBtn method -> in YshipPageViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                activityIndicator = false;
            }
        }

        /// <summary>
        /// This method will call when clicked on the settings icon and redirect to the profile select page.
        /// </summary>
        /// <param name="obj"></param>
        public async void profile_Tap(object obj)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in profile_Tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        Settings.RedirectPage = "Yship";
                        await Navigation.PushAsync(new ProfileSelectionPage((int)QAType.YS));
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "profile_Tap method -> in YshipPageViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method will call when clicked on the close button inside pop up (Dialog), for closing pop up.
        /// </summary>
        /// <param name="obj"></param>
        private void closeBtn(object obj)
        {
            popup = false;
        }

        /// <summary>
        /// This method will call when clicked on the checkbox icon that is on the toolbar and shows pop up (Dialog).
        /// </summary>
        /// <param name="obj"></param>
        public async void ChooseColumns()
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in ChooseColumns method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    CheckToolBarPopUpHideOrShow();
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;
                    popup = true;
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "ChooseColumns method -> in YshipPageViewModel! " + Settings.userLoginID);
                    trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }

        }

        #region for each time
        /// <summary>
        /// This method gets called when clicked on the chat icon that icon is inside the data grid and redirects to QnAlistPage.
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_eachMessage(object obj)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in tap_eachMessage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        Settings.RedirectPage = "Yship";
                        var allyshipdata = obj as YshipModel;

                        if (allyshipdata.chatImage != "minus.png")
                        {
                            await Navigation.PushAsync(new QnAlistPage(allyshipdata.yShipId, 0, (int)QAType.YS));
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_eachMessage method -> in YshipPageViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method gets called when clicked on Bkgno text and redirect to the booking details page.
        /// </summary>
        /// <param name="obj"></param>
        private async void tap_yBkgnoClick(object obj)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in tap_yBkgnoClick method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        var allyshipdata = obj as YshipModel;

                        if (allyshipdata.yBkgNumber != null)
                        {
                            await Navigation.PushAsync(new YShipFilter(Navigation, allyshipdata.yBkgNumber, allyshipdata.yShipBkgNumber, allyshipdata.yShipId));
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_yBkgnoClick method -> in YshipPageViewModel.cs " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }
        #endregion

        #region for bottom bar 
        /// <summary>
        /// This method gets called when clicked on the chat icon that icon is available on the bottom 
        /// navigation bar and then redirect to create a chat page (ChatUsers).
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_OnMessage(object sender)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in tap_OnMessage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    {
                        Settings.mutipleTimeClick = true;
                        activityIndicator = true;

                        /// Verifying internet connection.
                        checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            Settings.PerviousPage = "Yship";

                            if (selecteditems.Count > 1)
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select only one recored.");
                            }
                            else if (selecteditems.Count == 0)
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please select one recored to start QA.");
                            }
                            else if (selecteditems.Count == 1)
                            {
                                ChatData selectedTagsData = new ChatData();
                                var data = YshipDataCollections.Where(y => y.yShipId == selecteditems.FirstOrDefault());

                                foreach (var podata in data)
                                {
                                    var d = data.Where(y => y.yShipId == podata.yShipId).FirstOrDefault();
                                    selectedTagsData.POID = d.yShipId;
                                    List<Tag> lstdat = new List<Tag>();
                                    Tag tg = new Tag();
                                    selectedTagsData.tags = lstdat;
                                }
                                if (Settings.chatPageCount == 0)
                                {
                                    Settings.chatPageCount = 1;
                                    Settings.ChatuserCountImgHide = 1;
                                    await Navigation.PushAsync(new ChatUsers(selectedTagsData, true));
                                }
                            }
                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_OnMessage method -> in YshipPageViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method gets called when clicked on the camera icon that icon is available on the bottom 
        /// navigation bar and then redirect to photo upload page (YshipPhotoUpload).
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_InitialCamera(object sender)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in tap_InitialCamera method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        var dataGrid = sender as SfDataGrid;
                        await pagenavigation("Photo");
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_InitialCamera method -> in YshipPageViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        /// <summary>
        /// This method gets called when clicked on the Invoice/Packing list icon that icon is available on the bottom 
        /// navigation bar and then redirect to file upload page (YshipFileUpload).
        /// </summary>
        /// <param name="sender"></param>
        public async void tap_InitialFileUpload(object sender)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in tap_InitialFileUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    Settings.mutipleTimeClick = true;
                    activityIndicator = true;

                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        var dataGrid = sender as SfDataGrid;
                        await pagenavigation("Invoice/PL");
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_InitialFileUpload method -> in YshipPageViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    activityIndicator = false;
                }
            }
        }

        bool mTimeOpenPrinterCheck = false;
        /// <summary>
        /// This method gets called when clicked on the Permits icon that icon is available on the bottom 
        /// navigation bar and then redirect to file upload page (YshipFileUpload).
        /// </summary>
        /// <param name="obj"></param>
        public async void tap_Permits(object obj)
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in tap_Printer method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    activityIndicator = true;
                    Settings.mutipleTimeClick = true;

                    if (!mTimeOpenPrinterCheck)
                    {
                        mTimeOpenPrinterCheck = true;
                        var dataGrid = obj as SfDataGrid;
                        Settings.mutipleTimeClick = true;
                        await pagenavigation("Permits");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "tap_Printer method -> in YshipPageViewModel.cs " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                }
                finally
                {
                    Settings.mutipleTimeClick = false;
                    mTimeOpenPrinterCheck = false;
                    activityIndicator = false;
                }
            }
        }
        #endregion

        /// <summary>
        /// This method will fetch Tag columns based on the user id.
        /// </summary>
        /// <returns></returns>
        public async Task yShipTagcolumns()
        {
            activityIndicator = true;

            try
            {
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var yshiptagcolumns = await trackService.GetUserPrioritySettings(Settings.userLoginID);

                    if (yshiptagcolumns != null)
                    {
                        if (yshiptagcolumns.status != 0)
                        {
                            var searchTags = JsonConvert.DeserializeObject<List<ColumnInfo>>(yshiptagcolumns.data.yShipColumns);
                            Settings.VersionID = yshiptagcolumns.data.VersionID;
                            
                            if (searchTags != null)
                            {
                                var columnsData = new ObservableCollection<ColumnInfo>();
                               
                                foreach (var name in searchTags)
                                {
                                    columnsData.Add(new ColumnInfo() { mappingText = name.Column, headerText = name.Column, Value = name.Value });
                                }
                                showColumns = columnsData;
                            }
                            else
                            {
                                showColumns = null;
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
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "yShipTagcolumns method -> in YshipPageViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                activityIndicator = false;
            }
        }

        /// <summary>
        /// This method is for getting PN count.
        /// </summary>
        public async void GetPNCount()
        {
            YPSLogger.TrackEvent("YshipPageViewModel", "in GetPNCount method " + DateTime.Now + " UserId: " + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "GetPNCount method -> in YshipPageViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
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
        /// This method is for navigating to different functional page (Photo,Invoice/PL,Permits) that bottom menu has.
        /// </summary>
        /// <param name="navigationtype"></param>
        /// <returns></returns>
        public async Task pagenavigation(string navigationtype)
        {
            try
            {
                activityIndicator = true;

                if (selecteditems.Count > 1)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please select only one recored.");
                }
                else if (selecteditems.Count == 0)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please select one recored to upload/view " + navigationtype);
                }
                else if (selecteditems.Count == 1)
                {
                    YshipModel ymodel = new YshipModel();
                    var data = YshipDataCollections.Where(y => y.yShipId == selecteditems.FirstOrDefault());

                    foreach (var podata in data)
                    {
                        var d = data.Where(y => y.yShipId == podata.yShipId).FirstOrDefault();
                        ymodel.yShipId = d.yShipId;
                        ymodel.Complete = d.Complete;
                        ymodel.Cancel = d.Cancel;
                        ymodel.yBkgNumber = d.yShipBkgNumber;
                    }

                    if (navigationtype == "Photo")
                    {
                        await Navigation.PushAsync(new YshipPhotoUpload(ymodel.yShipId, ymodel.yBkgNumber, ymodel.Complete, ymodel.Cancel));
                    }
                    else if (navigationtype == "Invoice/PL")
                    {
                        await Navigation.PushAsync(new YshipFileUpload(ymodel.yShipId, ymodel.yBkgNumber, "Invoice/Packing List", ymodel.Complete, ymodel.Cancel));
                    }
                    else if (navigationtype == "Permits")
                    {
                        await Navigation.PushAsync(new YshipFileUpload(ymodel.yShipId, ymodel.yBkgNumber, "Permits", ymodel.Complete, ymodel.Cancel));
                    }
                    else
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "pagenavigation method -> in YshipPageViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                activityIndicator = false;
            }
        }
     
        /// <summary>
        /// This method gets the default setting values.
        /// </summary>
        /// <returns></returns>
        public async Task Defaultsettingdata()
        {
            try
            {
                var DBresponse = await trackService.GetSaveUserDefaultSettings(Settings.userLoginID);
                
                if (DBresponse != null)
                {
                    if (DBresponse.status == 1)
                    {
                        Settings.VersionID = DBresponse.data.VersionID;
                        CompanyName = Settings.CompanySelected = DBresponse.data.CompanyName;
                        
                        if (DBresponse.data.SupplierName == "")
                        {
                            ProNjobName = DBresponse.data.ProjectName + "/" + DBresponse.data.JobNumber;
                            Settings.ProjectSelected = DBresponse.data.ProjectName;
                            Settings.JobSelected = DBresponse.data.JobNumber;
                            Settings.CompanyID = DBresponse.data.CompanyID;
                            Settings.ProjectID = DBresponse.data.ProjectID;
                            Settings.JobID = DBresponse.data.JobID;
                            Settings.SupplierSelected = DBresponse.data.SupplierName;
                            Settings.SupplierID = DBresponse.data.SupplierID;
                        }
                        else
                        {
                            ProNjobName = DBresponse.data.ProjectName + "/" + DBresponse.data.JobNumber + "/" + DBresponse.data.SupplierName;
                            Settings.ProjectSelected = DBresponse.data.ProjectName;
                            Settings.JobSelected = DBresponse.data.JobNumber;
                            Settings.CompanyID = DBresponse.data.CompanyID;
                            Settings.ProjectID = DBresponse.data.ProjectID;
                            Settings.JobID = DBresponse.data.JobID;
                            Settings.SupplierSelected = DBresponse.data.SupplierName;
                            Settings.SupplierID = DBresponse.data.SupplierID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RememberUserDetails method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is for getting the saved yShipsearch criteria values from the API.
        /// </summary>
        /// <param name="yShipData"></param>
        public async Task GetSeatchCriteria(yShipSearch yShipData)
        {
            try
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in GetSeatchCriteria method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var Serchdata = await trackService.GetSearchValuesService(Settings.userLoginID);

                if (Serchdata != null)
                {
                    if (Serchdata.status == 1)
                    {
                        if (!string.IsNullOrEmpty(Serchdata.data.yShipSearchCriteria))
                        {
                            var searchC = JsonConvert.DeserializeObject<yShipSearch>(Serchdata.data.yShipSearchCriteria);

                            if (searchC != null)
                            {
                                //Key
                                yShipData.yBkgNumber = Settings.ybkgNumberyShip = searchC.yBkgNumber;
                                isClearSearchLbl = string.IsNullOrEmpty(yShipData.yBkgNumber) ? false : true;

                                yShipData.yShipBkgNumber = Settings.yshipBkgNumberyShip = searchC.yShipBkgNumber;
                                isClearSearchLbl = string.IsNullOrEmpty(yShipData.yShipBkgNumber) && !isClearSearchLbl ? false : true;

                                yShipData.ShippingNumber = Settings.shippingNumberyShip = searchC.ShippingNumber;
                                isClearSearchLbl = string.IsNullOrEmpty(yShipData.ShippingNumber) && !isClearSearchLbl ? false : true;

                                yShipData.BkgRefNo = Settings.bkgRefNumberyShip = searchC.BkgRefNo;
                                isClearSearchLbl = string.IsNullOrEmpty(yShipData.BkgRefNo) && !isClearSearchLbl ? false : true;

                                yShipData.BkgConfirmed = Settings.bkgConfirmIdyShip = searchC.BkgConfirmed;
                                isClearSearchLbl = yShipData.BkgConfirmed == -1 && !isClearSearchLbl ? false : true;

                                yShipData.Cancel = Settings.cancelIdyShip = searchC.Cancel;
                                isClearSearchLbl = yShipData.Cancel == -1 && !isClearSearchLbl ? false : true;

                                yShipData.Complete = Settings.completedIdyShip = searchC.Complete;
                                isClearSearchLbl = yShipData.Complete == -1 && !isClearSearchLbl ? false : true;

                                yShipData.EqmtTypeID = Settings.eqmtTypeIdyShip = searchC.EqmtTypeID;
                                isClearSearchLbl = yShipData.EqmtTypeID == 0 && !isClearSearchLbl ? false : true;

                                //Location
                                yShipData.OrgLocationID = Settings.oriSearchIdyShip = searchC.OrgLocationID;
                                Settings.oriSearchNameyShip = searchC.OrgLocation;
                                isClearSearchLbl = yShipData.OrgLocationID == 0 && !isClearSearchLbl ? false : true;

                                yShipData.DestLocationID = Settings.destSearchIdyShip = searchC.DestLocationID;
                                Settings.destSearchNameyShip = searchC.DestLocation;
                                isClearSearchLbl = yShipData.DestLocationID == 0 && !isClearSearchLbl ? false : true;

                                //Date
                                yShipData.ETA = Settings.reqPickUpDateyShip = searchC.ETA;
                                isClearSearchLbl = string.IsNullOrEmpty(yShipData.ETA) && !isClearSearchLbl ? false : true;

                                yShipData.PickUpTime = Settings.pickUpTimeyShip = searchC.PickUpTime;
                                isClearSearchLbl = string.IsNullOrEmpty(yShipData.PickUpTime) && !isClearSearchLbl ? false : true;

                                yShipData.ETD = Settings.reqDeliveryDateyShip = searchC.ETD;
                                isClearSearchLbl = string.IsNullOrEmpty(yShipData.ETD) && !isClearSearchLbl ? false : true;

                                yShipData.DeliveryTime = Settings.deliveryTimeyShip = searchC.DeliveryTime;
                                isClearSearchLbl = string.IsNullOrEmpty(yShipData.DeliveryTime) && !isClearSearchLbl ? false : true;
                            }
                        }
                        else
                        {
                            await ClearSearch(yShipData);
                        }
                    }
                    else
                    {
                        await ClearSearch(yShipData);
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SearchResultGet method -> in YshipPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to save the yShip search criteria in the db with default values.
        /// </summary>
        /// <returns></returns>
        public async Task ClearSearch(yShipSearch saveDefaultVal, bool callGetData = false)
        {
            YPSLogger.TrackEvent("YshipPageViewModel", "in ClearSearch method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");

                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    SearchPassData defaultData = new SearchPassData();
                    defaultData.CompanyID = Settings.CompanyID;
                    defaultData.UserID = Settings.userLoginID;

                    // Setting the default values for fields in Key tab
                    saveDefaultVal.yBkgNumber = Settings.ybkgNumberyShip = string.Empty;
                    saveDefaultVal.yShipBkgNumber = Settings.yshipBkgNumberyShip = string.Empty;
                    saveDefaultVal.ShippingNumber = Settings.shippingNumberyShip = string.Empty;
                    saveDefaultVal.BkgRefNo = Settings.bkgRefNumberyShip = string.Empty;
                    saveDefaultVal.BkgConfirmed = Settings.bkgConfirmIdyShip = -1;
                    saveDefaultVal.Cancel = Settings.cancelIdyShip = -1;
                    saveDefaultVal.Complete = Settings.completedIdyShip = -1;
                    saveDefaultVal.EqmtTypeID = Settings.eqmtTypeIdyShip = 0;

                    // Setting the default values for fields in Location tab
                    saveDefaultVal.OrgLocationID = Settings.oriSearchIdyShip = 0;
                    saveDefaultVal.DestLocationID = Settings.destSearchIdyShip = 0;

                    // Setting the default values for fields in Date tabe
                    saveDefaultVal.ETA = Settings.reqPickUpDateyShip = null;
                    saveDefaultVal.ETD = Settings.reqDeliveryDateyShip = null;
                    saveDefaultVal.PickUpTime = Settings.pickUpTimeyShip = null;
                    saveDefaultVal.DeliveryTime = Settings.deliveryTimeyShip = null;

                    //Saving all the filter fields with default value into the DB
                    defaultData.yShipSearchCriteria = JsonConvert.SerializeObject(saveDefaultVal);
                    var responseData = await trackService.SaveSerchvaluesSetting(defaultData);
                    var responseDataDeSer = responseData != null ? JsonConvert.DeserializeObject<SearchSetting>(responseData.ToString()) : null;


                    if (responseDataDeSer != null && responseDataDeSer.status == 1 && callGetData == true)
                    {
                        await pageNameyShip.GetData();// called to get the yShip grid data
                    }
                    else
                    {
                        await GetSeatchCriteria(saveDefaultVal);
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                YPSLogger.ReportException(ex, "ClearSearch -> in YshipPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
                activityIndicator = false;
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
        /// Gets called when clicked on refresh iocn, to refresh the page.
        /// </summary>
        public void RefreshClicked()
        {
            if (!Settings.mutipleTimeClick && !activityIndicator)
            {
                try
                {
                    CheckToolBarPopUpHideOrShow();
                    Settings.mutipleTimeClick = true;

                    NoRecordsLbl = false;

                    #region Clear header tab values 
                    Settings.PONumber = string.Empty;
                    Settings.REQNo = string.Empty;
                    Settings.ShippingNo = string.Empty;
                    Settings.DisciplineID = 0;
                    Settings.ELevelID = 0;
                    Settings.Condition = string.Empty;
                    Settings.Expeditor = string.Empty;
                    Settings.PriorityID = 0;
                    Settings.TAGNo = string.Empty;
                    Settings.IdentCodeNo = string.Empty;
                    #endregion

                    #region Clear date tab values
                    Settings.DeliveryFrom = string.Empty;
                    Settings.ETDFrom = string.Empty;
                    Settings.ETAFrom = string.Empty;
                    Settings.OnsiteFrom = string.Empty;
                    Settings.DeliveryTo = string.Empty;
                    Settings.ETDTo = string.Empty;
                    Settings.ETATo = string.Empty;
                    Settings.OnsiteTo = string.Empty;

                    App.Current.MainPage = new MenuPage(typeof(YshipPage));

                    #endregion

                    selecteditems.Clear();
                }
                catch (Exception ex)
                {
                    Settings.mutipleTimeClick = false;
                    YPSLogger.ReportException(ex, "RefreshClicked method -> in YshipPageViewModel.cs " + Settings.userLoginID);
                    trackService.Handleexception(ex);
                }
                finally
                {
                    activityIndicator = false;
                    Settings.mutipleTimeClick = false;
                }
            }
        }

        #region Properties
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

        private ObservableCollection<YshipModel> _PoData;
        public ObservableCollection<YshipModel> YshipDataCollections
        {
            get { return _PoData; }
            set
            {
                _PoData = value;
                RaisePropertyChanged("YshipDataCollections");
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

        private bool _activityIndicator;
        public bool activityIndicator
        {
            get { return _activityIndicator; }
            set
            {
                _activityIndicator = value;
                RaisePropertyChanged("activityIndicator");
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

        private List<int> _selecteditems = new List<int>();
        public List<int> selecteditems
        {
            get { return _selecteditems; }
            set
            {
                _selecteditems = value;
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

        private bool _isClearSearchLbl = false;
        public bool isClearSearchLbl
        {
            get { return _isClearSearchLbl; }
            set
            {
                _isClearSearchLbl = value;
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

        private bool _DataGrid = false;
        public bool DataGrid
        {
            get { return _DataGrid; }
            set
            {
                _DataGrid = value;
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
        #endregion
    }
}
