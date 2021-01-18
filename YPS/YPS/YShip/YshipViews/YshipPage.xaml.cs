using Syncfusion.SfBusyIndicator.XForms;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.XForms.Buttons;
using System;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.YshipViewModel;
using YPS.Views;
using YPS.Model.Yship;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Syncfusion.SfDataGrid.XForms.DataPager;
using System.Threading.Tasks;
using System.Timers;
using Acr.UserDialogs;

namespace YPS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YshipPage : ContentPage
    {
        #region Data members declaration
        YshipPageViewModel Vm;
        YPSService trackService;
        SfBusyIndicator indicator;
        public List<GridColumn> columns, finalcoumns;
        bool checkInternet;
        bool reloadGrid;
        public static Timer loadertimer;
        int selectedindex = 0;
        int photoupload;
        int Files;
        int PermitFiles;
        int Chat;
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public YshipPage()
        {
            try
            {
                InitializeComponent();
                bool reloadGrid = true;
                BindingContext = Vm = new YshipPageViewModel(Navigation, this);
                YPSLogger.TrackEvent("MainPage", "Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "MainPage";
                Settings.countmenu = 1;
                Settings.QAType = (int)QAType.YS;
                trackService = new YPSService();
                indicator = new SfBusyIndicator();
                dataGrid.AutoExpandGroups = false;
                Vm.IsPNenable = Settings.IsPNEnabled;

                /// hiding the chat based on user role.
                if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.SuperUser || Settings.userRoleID == (int)UserRoles.Viewer)
                {
                    stckChat.Opacity = 0.5;
                    stckChat.GestureRecognizers.Clear();
                    Vm.IsPNenable = false;
                }

                /// Subscribing push notification.
                MessagingCenter.Subscribe<string, string>("PushNotificationCame", "IncreaseCount", (sender, args) =>
                {
                    Vm.NotifyCountTxt = args;
                });

                PageSizeDDLBinding();
                dataPageryShip.PageIndexChanged += pageIndexChanged;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Main Page constructor -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method get called when page index is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void pageIndexChanged(object sender, PageIndexChangedEventArgs args)
        {
            try
            {
                dataPageryShip.PageIndexChanged -= pageIndexChanged;

                if (!Settings.mutipleTimeClick)
                {
                    Settings.mutipleTimeClick = true;

                    if (args.NewPageIndex != args.OldPageIndex && reloadGrid == true)
                    {
                        Settings.startPageyShip = args.NewPageIndex * Settings.pageSizeyShip;
                        Settings.toGoPageIndexyShip = args.NewPageIndex;
                        await GetData();
                    }
                    else
                    {
                        reloadGrid = true;
                        Settings.isSkipyShip = false;
                    }

                    Vm.activityIndicator = false;
                    UserDialogs.Instance.HideLoading();
                    Settings.mutipleTimeClick = false;
                }
                dataPageryShip.PageIndexChanged += pageIndexChanged;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "pageIndexChanged method -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        #region yShip grid - column text dynamic change and Show & hide

        /// <summary>
        /// This method get called when yship grid is loading, for changing label dynamically and Show/hide column.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void yShipGridLoaded(object sender, GridLoadedEventArgs e)
        {
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    dataPageryShip.NumericButtonsGenerateMode = NumericButtonsGenerateMode.Manual;
                }

                if (Device.RuntimePlatform == Device.iOS)
                {
                    dataPageryShip.NumericButtonsGenerateMode = NumericButtonsGenerateMode.Auto;
                }

                await GetData();
                await GridColumnUpdate();
                Selectcolums();

                #region Assigning method that must execute when page is loaded, for binding gestures
                loadertimer = new System.Timers.Timer();
                loadertimer.Interval = 1000;
                loadertimer.Elapsed += CreateGestureWithCommands;
                loadertimer.AutoReset = false;
                loadertimer.Enabled = true;
                #endregion
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "yShipGridLoaded method -> in YShipPage.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
            finally
            {
                Vm.activityIndicator = false;
            }
        }

        /// <summary>
        /// This method is to dynamically update the columns of data grid.
        /// </summary>
        /// <returns></returns>
        public async Task GridColumnUpdate()
        {
            try
            {
                bool isFieldAvaliable = false;
                columns = new List<GridColumn>();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    var yShipColumns = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).OrderBy(wr => wr.OrderID).ToList();

                    if (yShipColumns.Count > 0)
                    {
                        for (int i = 0; i < yShipColumns.Count; i++)
                        {
                            isFieldAvaliable = false;

                            for (int j = 0; j < dataGrid.Columns.Count; j++)
                            {
                                if (dataGrid.Columns[j].MappingName != "emptyCellValue")
                                {
                                    if (yShipColumns[i].FieldID.ToLower().Trim() == dataGrid.Columns[j].MappingName.ToLower().Trim())
                                    {
                                        if (!string.IsNullOrEmpty(yShipColumns[i].LblText))
                                        {
                                            dataGrid.Columns[j].HeaderText = yShipColumns[i].LblText;
                                            columns.Add(dataGrid.Columns[j]);
                                            isFieldAvaliable = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    finalcoumns = columns;

                    for (int j = 0; j < dataGrid.Columns.Count; j++)
                    {
                        isFieldAvaliable = false;

                        for (int i = 0; i < columns.Count; i++)
                        {
                            if (columns[i].MappingName.ToLower().Trim() == dataGrid.Columns[j].MappingName.ToLower().Trim())
                            {
                                isFieldAvaliable = true;
                                break;
                            }
                        }

                        if (isFieldAvaliable == false && dataGrid.Columns[j].MappingName != "emptyCellValue")
                        {
                            finalcoumns.Add(dataGrid.Columns[j]);
                        }
                    }

                    columns = null;
                    int gridColumnsCount = dataGrid.Columns.Count - 1;

                    for (int index = gridColumnsCount; index > 0; index--)
                    {
                        dataGrid.Columns.RemoveAt(index);
                    }

                    foreach (GridColumn grid in finalcoumns)
                    {
                        dataGrid.Columns.Add(grid);
                    }

                    finalcoumns = null;

                    if (dataGrid.Columns.Count == 1)
                    {
                        App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GridColumnUpdate method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
            finally
            {
                Vm.activityIndicator = false;
            }
        }

        /// <summary>
        /// This method is for hide/show columns.
        /// </summary>
        public async void Selectcolums()
        {
            try
            {
                await Vm.yShipTagcolumns();

                ObservableCollection<ColumnInfo> columnsData = new ObservableCollection<ColumnInfo>();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    var columnstatusdata = Settings.alllabeslvalues.Where(wh => wh.VersionID == Settings.VersionID && wh.LanguageID == Settings.LanguageID).ToList();

                    ObservableCollection<ColumnInfo> newcolumns = new ObservableCollection<ColumnInfo>();

                    if (Vm.showColumns != null)
                    {
                        foreach (var itemingrid in dataGrid.Columns)
                        {
                            bool flag1 = false;
                            int status = 0;
                            ColumnInfo matchingcolumns = new ColumnInfo();
                            ColumnInfo perviouscolumns = new ColumnInfo();

                            foreach (var colstatus in columnstatusdata)
                            {
                                perviouscolumns.mappingText = itemingrid.MappingName;
                                perviouscolumns.headerText = itemingrid.HeaderText;
                                perviouscolumns.check = true;
                                status = colstatus.Status;

                                if (colstatus.FieldID.ToLower().Trim() == itemingrid.MappingName.ToLower().Trim())
                                {
                                    flag1 = true;
                                    break;
                                }
                            }

                            if (status == 1 && flag1 == true)
                            {
                                matchingcolumns = Vm.showColumns.Where(x => x.mappingText == itemingrid.MappingName).FirstOrDefault();

                                if (matchingcolumns != null)
                                {
                                    newcolumns.Add(matchingcolumns);
                                }
                                else
                                {
                                    newcolumns.Add(perviouscolumns);
                                }
                            }
                        }

                        Vm.showColumns = newcolumns;
                        newcolumns = null;
                        bool flag = false;

                        try
                        {
                            foreach (var itemingrid in dataGrid.Columns)
                            {
                                flag = false;
                                string columnName = "";

                                foreach (var item in Vm.showColumns)
                                {
                                    columnName = itemingrid.MappingName;

                                    if (item.mappingText == itemingrid.MappingName)
                                    {
                                        if (item.mappingText != "emptyCellValue")
                                        {
                                            if (item.Value)
                                            {
                                                dataGrid.Columns[item.mappingText].IsHidden = false;
                                                columnsData.Add(new ColumnInfo() { mappingText = item.mappingText, headerText = dataGrid.Columns[item.mappingText].HeaderText, check = true });
                                            }
                                            else
                                            {
                                                dataGrid.Columns[item.mappingText].IsHidden = true;
                                                columnsData.Add(new ColumnInfo() { mappingText = item.mappingText, headerText = dataGrid.Columns[item.mappingText].HeaderText, check = false });
                                            }

                                            flag = true;
                                            break;
                                        }
                                    }
                                }

                                if (flag == false)
                                {
                                    if (itemingrid.MappingName == "emptyCellValue")
                                    {
                                        dataGrid.Columns[columnName].IsHidden = false;
                                    }
                                    else
                                    {
                                        dataGrid.Columns[columnName].IsHidden = false;
                                        columnsData.Add(new ColumnInfo() { mappingText = itemingrid.MappingName, headerText = dataGrid.Columns[itemingrid.MappingName].HeaderText, check = true });
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            UserDialogs.Instance.HideLoading();
                        }
                    }
                    else
                    {
                        foreach (var statusitem in columnstatusdata)
                        {
                            foreach (var item in dataGrid.Columns)
                            {
                                if (statusitem.FieldID == item.MappingName)
                                {
                                    if (statusitem.Status == 1)
                                    {
                                        if (item.MappingName != "emptyCellValue")
                                        {
                                            item.IsHidden = false;
                                            columnsData.Add(new ColumnInfo() { mappingText = item.MappingName, headerText = item.HeaderText, check = true });
                                        }
                                    }
                                    else
                                    {
                                        item.IsHidden = true;
                                    }
                                }
                            }
                        }
                    }

                    Vm.showColumns = columnsData;

                    if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                    {
                        this.GetBottomMenVal();
                    }

                    if (Settings.ShowSuccessAlert == true)
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Success");
                    }
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                YPSLogger.ReportException(ex, "Selectcolums method -> in YShipPage.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
                Vm.activityIndicator = false;
            }
        }

        #endregion

        /// <summary>
        /// This method is to disable bottom menu options.
        /// </summary>
        public void BottomMenuHide()
        {
            try
            {
                if (photoupload == 0)
                {
                    stckCamera.Opacity = 0.5;
                    stckCamera.GestureRecognizers.Clear();
                }

                if (Files == 0)
                {
                    stckFileUpload.Opacity = 0.5;
                    stckFileUpload.GestureRecognizers.Clear();
                }

                if (Chat == 0)
                {
                    stckChat.Opacity = 0.5;
                    stckChat.GestureRecognizers.Clear();
                }
                if (PermitFiles == 0)
                {
                    stckPermits.Opacity = 0.5;
                    stckPermits.GestureRecognizers.Clear();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BottomMenuHide method -> in Main Page.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method checks, if bottom menu option(s) is to be disable or not.
        /// </summary>
        public void GetBottomMenVal()
        {
            try
            {
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    var valuesDef = Settings.alllabeslvalues.Where(x => x.LanguageID == Settings.LanguageID && x.VersionID == Settings.VersionID).ToList();
                    photoupload = valuesDef.Where(wr => wr.FieldID == "PhotoCount").Select(g => g.Status == 0 ? 0 : 1).FirstOrDefault();
                    Files = valuesDef.Where(wr => wr.FieldID == "InvoicePLCount").Select(g => g.Status == 0 ? 0 : 1).FirstOrDefault();
                    PermitFiles = valuesDef.Where(wr => wr.FieldID == "PermitCount").Select(g => g.Status == 0 ? 0 : 1).FirstOrDefault();
                    Chat = valuesDef.Where(wr => wr.FieldID == "yShipQACount").Select(g => g.Status == 0 ? 0 : 1).FirstOrDefault();
                    Settings.IsAppLabelCall = ((photoupload == 0) || (PermitFiles == 0) || (Files == 0) || (Chat == 0)) == true ? true : false;

                    if (Settings.IsAppLabelCall == true)
                    {
                        BottomMenuHide();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetBottomMenVal method -> in Main Page.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method is to set the values to pagesize DDL, set start page & select the button when page is loaded.
        /// </summary>
        public void PageSizeDDLBinding()
        {
            try
            {
                PageSize.SelectedIndexChanged += PageSizeChanged;
                List<int> listpagesize = new List<int>(new int[] { 5, 10, 20, 50, 100 });
                PageSize.ItemsSource = listpagesize;
                reloadGrid = false;

                Settings.startPageyShip = 0;
                Settings.toGoPageIndexyShip = 0;
                PageSize.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "PageSizeDDLBinding method -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method get called when page size is selected from pagesize DDL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void PageSizeChanged(object sender, EventArgs args)
        {
            try
            {
                PageSize.SelectedIndexChanged -= PageSizeChanged;

                if (!Settings.mutipleTimeClick)
                {
                    Settings.mutipleTimeClick = true;
                    var senderobj = sender as Picker;
                    Settings.pageSizeyShip = (int)senderobj.SelectedItem;
                    selectedSizeName.Text = Convert.ToString(senderobj.SelectedItem);
                    Settings.selectedIndexyShip = senderobj.SelectedIndex;

                    if (reloadGrid == true)
                    {
                        Settings.startPageyShip = 0;
                        Settings.toGoPageIndexyShip = 0;
                        dataPageryShip.MoveToPage(0);
                        await GetData();
                        reloadGrid = false;
                    }

                    reloadGrid = true;
                    Settings.mutipleTimeClick = false;
                }
                UserDialogs.Instance.HideLoading();
                PageSize.SelectedIndexChanged += PageSizeChanged;
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "PageSizeChanged method -> in Main Page.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method is to bind the data to the data grid.
        /// </summary>
        /// <returns></returns>
        public async Task GetData()
        {
            await Task.Delay(50);
            UserDialogs.Instance.ShowLoading("Loading...");
            Vm.activityIndicator = false;
            try
            {
                YPSLogger.TrackEvent("YshipPageViewModel", "in GetData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    await Vm.Defaultsettingdata();
                    yShipSearch yShipData = new yShipSearch();
                    yShipData.UserID = Settings.userLoginID;
                    yShipData.PageSize = Settings.pageSizeyShip;
                    yShipData.StartPage = Settings.startPageyShip;
                    await Vm.GetSeatchCriteria(yShipData);

                    var result = await trackService.LoadYshipData(yShipData);

                    if (result.status != 0)
                    {
                        Vm.NoRecordsLbl = false;
                        Vm.DataGrid = true;
                        Vm.DataGridShowColumn = true;

                        foreach (var data in result.data)
                        {
                            #region Binding icon

                            #region Chat
                            if (data.yShipQACount == 0)
                            {
                                if (data.yShipQAClosedCount > 0)
                                {
                                    data.chatImage = Icons.chatIcon;
                                    data.chatTickVisible = true;
                                }
                                else
                                {
                                    data.chatImage = Icons.minus;
                                }
                            }
                            else
                            {
                                data.chatImage = Icons.chatIcon;
                                data.countVisible = true;
                            }

                            if (data.PhotoCount > 0)
                            {
                                data.minusVisible = false;
                                data.FIlecountVisible = true;
                            }
                            else
                            {
                                data.FIlecountVisible = false;
                                data.minusVisible = true;
                            }

                            if (data.InvoicePLCount > 0)
                            {
                                data.invoiceminusVisible = false;
                                data.invoiceFIlecountVisible = true;
                            }
                            else
                            {
                                data.invoiceminusVisible = true;
                                data.invoiceFIlecountVisible = false;
                            }

                            if (data.PermitCount > 0)
                            {
                                data.permitminusVisible = false;
                                data.permitFIlecountVisible = true;
                            }
                            else
                            {
                                data.permitminusVisible = true;
                                data.permitFIlecountVisible = false;
                            }
                            #endregion

                            #region Act
                            if (data.IsACT == 5)
                            {
                                data.ActImage = Icons.Flag;
                                data.FlagColor = Color.Green;
                            }
                            else if (data.IsACT == 6)
                            {
                                data.ActImage = Icons.Flag;
                                data.FlagColor = Color.Orange;
                            }
                            else if (data.IsACT == 7)
                            {
                                data.ActImage = Icons.Flag;
                                data.FlagColor = Color.Red;
                            }
                            #endregion

                            #region BKGconfirm
                            if (data.IsBkgConfirmed == 0)
                            {
                                data.BkgConfirmedImage = Icons.CloseIc;
                                data.BkgConfirmedColor = Color.Red;
                            }
                            else if (data.IsBkgConfirmed == 1)
                            {
                                data.BkgConfirmedImage = Icons.tickb;
                                data.BkgConfirmedColor = Color.Green;
                            }
                            #endregion

                            #region Completed
                            if (data.Complete == 0)
                            {
                                data.CompleteConfirmedImage = Icons.CloseIc;
                                data.CompleteConfirmedColor = Color.Red;
                            }
                            else if (data.Complete == 1)
                            {
                                data.CompleteConfirmedImage = Icons.tickb;
                                data.CompleteConfirmedColor = Color.Green;
                            }
                            #endregion

                            #region Canceled
                            if (data.Cancel == 0)
                            {
                                data.CancelConfirmedImage = Icons.CloseIc;
                                data.CancelConfirmedColor = Color.Red;
                            }
                            else if (data.Cancel == 1)
                            {
                                data.CancelConfirmedImage = Icons.tickb;
                                data.CancelConfirmedColor = Color.Green;
                            }
                            #endregion

                            #region OTLBkg
                            if (data.IsOTLBkg == 0)
                            {
                                data.OTLBkgImage = Icons.CloseIc;
                                data.OTLBkgColor = Color.Red;
                            }
                            else if (data.IsOTLBkg == 1)
                            {
                                data.OTLBkgImage = Icons.tickb;
                                data.OTLBkgColor = Color.Green;
                            }
                            #endregion

                            #region Inv_P_L
                            if (data.IsInv_P_L == 0)
                            {
                                data.IsInv_P_LImage = Icons.CloseIc;
                                data.IsInv_P_LColor = Color.Red;
                            }
                            else if (data.IsInv_P_L == 1)
                            {
                                data.IsInv_P_LImage = Icons.tickb;
                                data.IsInv_P_LColor = Color.Green;
                            }
                            #endregion

                            #region ExportBkg
                            if (data.IsExportBkg == 0)
                            {
                                data.ExportBkgImage = Icons.CloseIc;
                                data.ExportBkgColor = Color.Red;
                            }
                            else if (data.IsExportBkg == 1)
                            {
                                data.ExportBkgImage = Icons.tickb;
                                data.ExportBkgColor = Color.Green;
                            }
                            #endregion

                            #region ImportBkg
                            if (data.IsImportBkg == 0)
                            {
                                data.ImportBkgImage = Icons.CloseIc;
                                data.ImportBkgColor = Color.Red;
                            }
                            else if (data.IsImportBkg == 1)
                            {
                                data.ImportBkgImage = Icons.tickb;
                                data.ImportBkgColor = Color.Green;
                            }
                            #endregion

                            #region EstP/UpTime
                            if (data.IsESTPickUpTime == 0)
                            {
                                data.EstP_UpTimeImage = Icons.CloseIc;
                                data.EstP_UpTimeColor = Color.Red;
                            }
                            else if (data.IsESTPickUpTime == 1)
                            {
                                data.EstP_UpTimeImage = Icons.tickb;
                                data.EstP_UpTimeColor = Color.Green;
                            }
                            #endregion

                            #region P/UpTime
                            if (data.IsPickUpInfo == -1)
                            {
                                data.P_UpTimeImage = Icons.CloseIc;
                                data.P_UpTimeColor = Color.Red;
                            }
                            else if (data.IsPickUpInfo == -2)
                            {
                                data.P_UpTimeImage = Icons.Exclamation;
                                data.P_UpTimeColor = Color.Yellow;
                            }
                            else if (data.IsPickUpInfo == -3)
                            {
                                data.P_UpTimeImage = Icons.tickb;
                                data.P_UpTimeColor = Color.Green;
                            }
                            #endregion

                            #region ATD
                            if (data.IsATD == 0)
                            {
                                data.ATDImage = Icons.CloseIc;
                                data.ATDColor = Color.Red;
                            }
                            else if (data.IsATD == 1)
                            {
                                data.ATDImage = Icons.tickb;
                                data.ATDColor = Color.Green;
                            }
                            #endregion

                            #region ETA
                            if (data.IsETA == 0)
                            {
                                data.ETAImage = Icons.CloseIc;
                                data.ETAColor = Color.Red;
                            }
                            else if (data.IsETA == 1)
                            {
                                data.ETAImage = Icons.tickb;
                                data.ETAColor = Color.Green;
                            }
                            #endregion

                            #region DeliveryInfo
                            if (data.IsDeliveryInfo == -1)
                            {
                                data.DeliveryInfoImage = Icons.CloseIc;
                                data.DeliveryInfoColor = Color.Red;
                            }
                            else if (data.IsDeliveryInfo == -2)
                            {
                                data.DeliveryInfoImage = Icons.Exclamation;
                                data.DeliveryInfoColor = Color.Yellow;
                            }
                            else if (data.IsDeliveryInfo == -3)
                            {
                                data.DeliveryInfoImage = Icons.tickb;
                                data.DeliveryInfoColor = Color.Green;
                            }
                            #endregion

                            #region ATA
                            if (data.IsATA == 0)
                            {
                                data.ATAImage = Icons.CloseIc;
                                data.ATAColor = Color.Red;
                            }
                            else if (data.IsATA == 1)
                            {
                                data.ATAImage = Icons.tickb;
                                data.ATAColor = Color.Green;
                            }

                            #endregion

                            #region ETD
                            if (data.IsETD == -4)
                            {
                                data.ETD = data.ETD1;
                                if (data.ETD != "")
                                {
                                    data.ETDBgColor = "#FF0800";
                                    data.ETDTextColor = "#F7F7F7";
                                }
                            }
                            else if (data.IsETD == 4)
                            {
                                data.ETD = data.ETD1;
                                data.ETDBgColor = "#F7F7F7";
                                data.ETDTextColor = "#000000";
                            }
                            #endregion
                            #endregion
                        }
                        Vm.YshipDataCollections = result.data;

                        if (Vm.YshipDataCollections.Count > 0)
                        {
                            dataGrid.ItemsSource = Vm.YshipDataCollections;
                            var roundcount = Math.Ceiling((decimal)result.data.Select(c => c.listCount).FirstOrDefault() / (decimal)Settings.pageSizeyShip);
                            dataPageryShip.PageCount = (Int32)roundcount;
                            dataPageryShip.NumericButtonCount = (Int32)roundcount;

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (Device.RuntimePlatform == Device.Android)
                                {
                                    if (dataPageryShip.NumericButtonCount < 6 && dataPageryShip.NumericButtonCount != 0)
                                    {
                                        dataPageryShip.NumericButtonsGenerateMode = NumericButtonsGenerateMode.Auto;
                                    }
                                    else
                                    {
                                        dataPageryShip.NumericButtonsGenerateMode = NumericButtonsGenerateMode.Manual;
                                    }
                                }
                            });
                        }
                        else
                        {
                            Vm.NoRecordsLbl = true;
                            Vm.DataGrid = false;
                            Vm.DataGridShowColumn = false;
                        }
                    }
                    else
                    {
                        Vm.NoRecordsLbl = true;
                        Vm.DataGrid = false;
                        Vm.DataGridShowColumn = false;
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }

            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetData method -> in YshipPageViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
            finally
            {
                Vm.activityIndicator = false;
                Vm.profileSettingVisible = true;
            }
        }

        /// <summary>
        /// This method gets called when page is appearing.
        /// </summary>
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            YPSLogger.TrackEvent("MainPage", "OnAppearing " + DateTime.Now + " UserId: " + Settings.userLoginID);
            await SecureStorage.SetAsync("mainPageisOn", "1");

            /// Enable and Disble master detail page menu gesture
            (Application.Current.MainPage as YPSMasterPage).IsGestureEnabled = true;

            /// Get PN count
            Vm.GetPNCount();

            Settings.countmenu = 1;

            try
            {
                Settings.mutipleTimeClick = false;

                if (Settings.isyShipRefreshPage)
                {
                    Settings.isyShipRefreshPage = false;
                    await GetData();
                    UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
            finally
            {
                Settings.mutipleTimeClick = false;
            }
        }

        /// <summary>
        /// This method is called when leaving the page.
        /// </summary>
        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            //Vm.GetPNCount();
            await SecureStorage.SetAsync("mainPageisOn", "0");
        }

        /// <summary>
        /// This method gets called when checkbox on data grid are selected/deselected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void selectAll_checkBox(object sender, StateChangedEventArgs e)
        {
            var itemsselected = Vm.selecteditems;
            try
            {
                bool? val = (sender as SfCheckBox).IsChecked;
                int shipid = Convert.ToInt32((sender as SfCheckBox).ClassId);

                if (val == true && shipid > 0)
                {
                    var data = Vm.YshipDataCollections.Where(x => x.yShipId == shipid).FirstOrDefault();
                    var tounchkid = new YshipModel();

                    if (Vm.selecteditems.Count > 0)
                    {
                        foreach (var id in Vm.selecteditems)
                        {
                            tounchkid = Vm.YshipDataCollections.Where(x => x.yShipId == id).FirstOrDefault();

                            if (tounchkid != null)
                            {
                                tounchkid.IsChecked = false;
                            }
                            Vm.selecteditems.Remove(id);
                            break;
                        }
                        Vm.selecteditems.Add(shipid);
                        data.IsChecked = true;
                    }
                    else
                    {
                        Vm.selecteditems.Add(data.yShipId);
                    }
                }
                else if (val == false && shipid > 0)
                {
                    Vm.selecteditems.Clear();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "selectAll_checkBox method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method is called when clicked on notification icon and redirect to notification page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Notification_Tap(object sender, EventArgs e)
        {
            try
            {
                if (Vm.NotifyCountTxt == "0")
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("There is no new notification.");
                }
                else
                {
                    await Navigation.PushAsync(new NotificationListPage());
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Notification_Tap method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This gets caled when clicked on the more (three dots) icon, to show/hide more options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoreItems_Tapped(object sender, EventArgs e)
        {
            Vm.CheckToolBarPopUpHideOrShow();
        }

        /// <summary>
        /// Gets called after 1 sec. of page getting loaded, for adding gestures with command to labels & stacks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void CreateGestureWithCommands(object sender, ElapsedEventArgs args)
        {
            try
            {
                loadertimer.Enabled = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    refreshName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.RefreshClicked)
                    }); ;


                    NameyShipfilter.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await Vm.FilterPageNav()),
                    });

                    chooseColumnName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.ChooseColumns),
                        CommandParameter = dataGrid,
                    });

                    stckCamera.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.tap_InitialCamera),
                        CommandParameter = dataGrid,
                    });

                    if (Settings.userRoleID != (int)UserRoles.SuperAdmin && Settings.userRoleID != (int)UserRoles.SuperUser && Settings.userRoleID != (int)UserRoles.Viewer)
                    {
                        stckChat.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.tap_OnMessage),
                            CommandParameter = dataGrid,
                        });
                    }

                    stckFileUpload.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.tap_InitialFileUpload),
                        CommandParameter = dataGrid,
                    });

                    stckPermits.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.tap_Permits),
                        CommandParameter = dataGrid,
                    });

                    settingsName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.profile_Tap)
                    });

                    pageSizerStackName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command<View>((view) =>
                        {
                            view?.Focus();
                        }),
                        CommandParameter = PageSize,
                    });
                });
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "OnTimedEvent -> in MainPage.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }
    }
}
