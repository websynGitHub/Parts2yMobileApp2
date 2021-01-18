using Acr.UserDialogs;
using Syncfusion.SfBusyIndicator.XForms;
using Syncfusion.SfDataGrid.XForms;
using Syncfusion.SfDataGrid.XForms.DataPager;
using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;
using YPS.Views;

namespace YPS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        #region Data members declaration
        PoDataViewModel Vm;
        YPSService trackService = new YPSService();
        SfBusyIndicator indicator;
        ListView listView;
        public List<GridColumn> columns, finalcoumns;
        bool fromConstructor;
        bool reloadGrid;
        bool isloading;
        bool iscalled;
        public static Timer timer;
        public static Timer loadertimer;
        int lastButtonCount;
        public bool isLoaded { get; set; } = false;
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public MainPage()
        {
            try
            {
                InitializeComponent();
                Settings.mutipleTimeClick = false;
                lastButtonCount = 0;
                bool reloadGrid = true;
                isloading = false;
                iscalled = false;
                YPSLogger.TrackEvent("MainPage", "Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "MainPage";
                Settings.PerviousPage = "MainPage1";
                Settings.countmenu = 1;
                Settings.QAType = (int)QAType.PT;
                BindingContext = Vm = new PoDataViewModel(Navigation, this);
                trackService = new YPSService();
                Vm.IsPNenable = Settings.IsPNEnabled;
                
                if (Settings.userRoleID == (int)UserRoles.SuperAdmin)
                {
                    imgCamera.Opacity = imgChat.Opacity = imgFileUpload.Opacity = imgPrinter.Opacity = 0.5;
                    CameraLbl.Opacity = ChatLbl.Opacity = FileUploadLbl.Opacity = PrinterLbl.Opacity = 0.5;
                    stckCamera.GestureRecognizers.Clear();
                    stckFileUpload.GestureRecognizers.Clear();
                    stckChat.GestureRecognizers.Clear();
                    stckPrinter.GestureRecognizers.Clear();
                    Vm.IsPNenable = false;
                }
                else if (Settings.userRoleID == (int)UserRoles.CustomerAdmin || Settings.userRoleID == (int)UserRoles.CustomerUser)
                {
                    picRequiredStk1.IsVisible = picRequiredStk2.IsVisible = true;
                }

                if (Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                {
                    imgPrinter.Opacity = 0.5;
                    PrinterLbl.Opacity = 0.5;
                    stckPrinter.GestureRecognizers.Clear();
                }
                dataGrid.AutoExpandGroups = false;

                if (Settings.userRoleID == (int)UserRoles.CustomerAdmin)
                {
                    Vm.Archivedchat = true;
                }
                else
                {
                    SetHeightOfFrame.HeightRequest = 100;
                }

                #region Subscribing MessageCenter
                MessagingCenter.Subscribe<string, string>("PushNotificationCame", "IncreaseCount", (sender, args) =>
                {
                    Vm.NotifyCountTxt = args;
                });

                MessagingCenter.Subscribe<string, string>("APhotoCount", "msgA", (sender, args) =>
                {
                    var updateCount = Vm.PoDataCollections.Where(x => x.PUID == Settings.currentPuId).ToList();
                    updateCount.ForEach(a => { a.TagAPhotoCount = Convert.ToInt32(args); });

                });

                MessagingCenter.Subscribe<string, string>("BPhotoCount", "msgB", (sender, args) =>
                {
                    var updateCount = Vm.PoDataCollections.Where(x => x.PUID == Settings.currentPuId).ToList();
                    updateCount.ForEach(a => { a.TagBPhotoCount = Convert.ToInt32(args); });
                });

                MessagingCenter.Subscribe<string, string>("PhotoComplted", "showtickMark", (sender, args) =>
                {
                    var updateCount = Vm.PoDataCollections.Where(x => x.PUID == Convert.ToInt16(args)).ToList();// Settings.currentPuId).ToList();
                    updateCount.ForEach(a =>
                    {
                        a.imgCamOpacityA = a.imgtickOpacityA = (a.TagAPhotoCount == 0) ? 0.5 : 1.0;
                        a.imgCamOpacityB = a.imgTickOpacityB = (a.TagBPhotoCount == 0) ? 0.5 : 1.0;


                        a.BPhotoCountVisible = a.APhotoCountVisible = false;
                        a.photoTickVisible = true;
                    });
                });

                MessagingCenter.Subscribe<string, string>("InitialPhotoCount", "AandB", (sender, args) =>
                {
                    var count = args.Split(',');
                    var updateCount = Vm.PoDataCollections
                    .Where(x => x.POTagID == (Settings.currentPoTagId_Inti.Where(k => k.POTagID == x.POTagID).Select(p => p.POTagID).FirstOrDefault()))
                    .Select(c =>
                    {
                        c.cameImage = "Chatcamera.png";
                        c.TagAPhotoCount = Convert.ToInt16(count[0]);
                        c.TagBPhotoCount = Convert.ToInt16(count[1]);
                        c.PUID = Convert.ToInt16(count[2]);
                        c.BPhotoCountVisible = true;
                        c.APhotoCountVisible = true;
                        return c;
                    }).ToList();
                });

                MessagingCenter.Subscribe<string, string>("FilesCountI", "msgFileInitial", (sender, args) =>
                {
                    var count = args.Split(',');
                    var updateCounts = Vm.PoDataCollections
                     .Where(x => x.POTagID == (Settings.currentPoTagId_Inti_F.Where(k => k.POTagID == x.POTagID).Select(p => p.POTagID).FirstOrDefault()))
                     .Select(c =>
                     {
                         c.fileImage = "attachb.png";
                         c.TagFilesCount = Convert.ToInt16(count[0]);
                         c.FUID = Convert.ToInt16(count[1]);
                         c.filecountVisible = true;
                         return c;
                     }).ToList();
                });

                MessagingCenter.Subscribe<string, string>("FilesCounts", "msgF", (sender, args) =>
                {
                    var count = args.Split(',');
                    var updateCount = Vm.PoDataCollections.Where(x => x.FUID == Convert.ToInt32(count[1]));
                    foreach (var item in updateCount)
                    {
                        item.TagFilesCount = Convert.ToInt32(count[0]);
                    }
                });

                MessagingCenter.Subscribe<string, string>("FileComplted", "showFileAsComplete", (sender, args) =>
                {
                    var updateCount = Vm.PoDataCollections.Where(x => x.FUID == Convert.ToInt32(args));// Settings.currentFuId);
                    foreach (var item in updateCount)
                    {
                        item.filecountVisible = false;
                        item.fileTickVisible = true;
                    }

                });

                MessagingCenter.Subscribe<string, string>("FilesCount", "deleteFile", (sender, args) =>
                {
                    var count = args.Split(',');
                    var updateCount = Vm.PoDataCollections.Where(x => x.FUID == Convert.ToInt32(count[1]));// Settings.currentFuId);
                    foreach (var item in updateCount)
                    {
                        item.TagFilesCount = Convert.ToInt32(count[0]);// Convert.ToInt32(args);                                                                                                                                            
                    }
                });

                #endregion

                indicator = new SfBusyIndicator();

                PageSizeDDLBinding();
                dataPager.PageIndexChanged += pageIndexChanged;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Main Page constructor -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method gets called when page index is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void pageIndexChanged(object sender, PageIndexChangedEventArgs args)
        {
            try
            {
                if (!Settings.mutipleTimeClick)
                {
                    dataPager.PageIndexChanged -= pageIndexChanged;
                    Settings.mutipleTimeClick = true;

                    if (args.NewPageIndex != args.OldPageIndex && reloadGrid == true)
                    {
                        Settings.startPageYPS = args.NewPageIndex * Settings.pageSizeYPS;
                        Settings.toGoPageIndex = args.NewPageIndex;
                        await BindGridData(false, false);
                    }
                    else
                    {
                        reloadGrid = true;
                        Settings.isSkip = false;
                    }

                    Vm.activityIndicator = false;
                    Settings.mutipleTimeClick = false;
                    dataPager.PageIndexChanged += pageIndexChanged;
                    UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "pageIndexChanged method -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method is called when the grid is loaded at the page load time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DataGrid_GridLoaded(object sender, Syncfusion.SfDataGrid.XForms.GridLoadedEventArgs e)
        {
            try
            {
                if (Device.RuntimePlatform == Device.Android)
                {
                    dataPager.NumericButtonsGenerateMode = NumericButtonsGenerateMode.Manual;
                }

                if (Device.RuntimePlatform == Device.iOS)
                {
                    dataPager.NumericButtonsGenerateMode = NumericButtonsGenerateMode.Auto;
                }

                await BindGridData(false, false);
                await Gridcolumsupdate();
                Selectcolums();
                Vm.activityIndicator = false;

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
                Vm.activityIndicator = false;
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "DataGrid_GridLoaded -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        #region Add gesture after page is loaded
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
                    if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    {
                        stckCamera.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.tap_InitialCamera),
                            CommandParameter = dataGrid,
                        });

                        stckChat.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.tap_OnMessage),
                            CommandParameter = dataGrid,
                        });

                        stckFileUpload.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.tap_InitialFileUpload),
                            CommandParameter = dataGrid,
                        });

                        if (Settings.userRoleID != (int)UserRoles.MfrAdmin && Settings.userRoleID != (int)UserRoles.MfrUser && Settings.userRoleID != (int)UserRoles.DealerAdmin && Settings.userRoleID != (int)UserRoles.DealerUser)
                        {
                            stckPrinter.GestureRecognizers.Add(new TapGestureRecognizer
                            {
                                Command = new Command(Vm.tap_Printer),
                                CommandParameter = dataGrid,
                            });
                        }
                    }

                    refreshName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await Vm.RefreshPage()),
                    });

                    Namefilter.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.FilterClicked),
                    });

                    chooseColumnName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.ChooseColumns),
                        CommandParameter = dataGrid,
                    });

                    archivedchats.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.ArchivedChats),
                    });

                    settingsName.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.profile_Tap)
                    });

                    pagesizeStackName.GestureRecognizers.Add(new TapGestureRecognizer
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
        #endregion

        /// <summary>
        /// This is to update the columns in data grid.
        /// </summary>
        /// <returns></returns>
        public async Task Gridcolumsupdate()
        {
            try
            {
                await Vm.TagColumnsBinding();
                GridColumn emptycellvalue;
                GridColumn gridcolumn = null;
                bool isFieldAvaliable = false;
                columns = new List<GridColumn>();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    var data = Settings.alllabeslvalues.Where(wh => wh.VersionID == Settings.VersionID && wh.LanguageID == Settings.LanguageID).OrderBy(wr => wr.OrderID).ToList();

                    if (data.Count > 0)
                    {
                        for (int i = 0; i < data.Count; i++)
                        {
                            isFieldAvaliable = false;

                            for (int j = 0; j < dataGrid.Columns.Count; j++)
                            {
                                if (dataGrid.Columns[j].MappingName != "emptyCellValue")
                                {
                                    if (data[i].FieldID.ToLower().Trim() == dataGrid.Columns[j].MappingName.ToLower().Trim())
                                    {
                                        if (!string.IsNullOrEmpty(data[i].LblText))
                                        {
                                            dataGrid.Columns[j].HeaderText = data[i].LblText;
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

                    columns = dataGrid.Columns.ToList();
                    emptycellvalue = dataGrid.Columns.Where(w => w.MappingName == "emptyCellValue").FirstOrDefault();
                    dataGrid.Columns.Clear();
                    dataGrid.GroupColumnDescriptions.Clear();
                    dataGrid.Columns.Add(emptycellvalue);
                    dataGrid.GroupColumnDescriptions.Clear();

                    foreach (GridColumn grid in finalcoumns)
                    {
                        dataGrid.Columns.Add(grid);
                    }

                    GroupColumnDescription colname = new GroupColumnDescription();
                    colname.ColumnName = "POID";
                    dataGrid.GroupColumnDescriptions.Add(colname);

                    if (dataGrid.Columns.Count == 1)
                    {
                        App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                    }
                }
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Gridcolumsupdate -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method is to hide/show the columns in the grid.
        /// </summary>
        public async Task Selectcolums()
        {
            try
            {
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

                    if (Settings.alllabeslvalues != null && Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    {
                        this.GetBottomMenVal();
                    }

                    Vm.SearchDisable = true;
                    Vm.RefreshDisable = true;
                    Vm.DataGridShowColumn = true;
                    Vm.activityIndicator = false;
                    UserDialogs.Instance.HideLoading();

                    if (Settings.ShowSuccessAlert == true)
                    {
                        Settings.ShowSuccessAlert = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                    }
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

                Vm.activityIndicator = false;
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "Selectcolums -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
            UserDialogs.Instance.HideLoading();
        }

        /// <summary>
        /// This method is to disable the bottom menu option.
        /// </summary>
        public void BottomMenuHide()
        {
            try
            {
                if (Settings.APhoto == 0 && Settings.BPhoto == 0)
                {
                    stckCamera.Opacity = 0.5;
                    picRequiredStk1.IsVisible = false;
                    picRequiredStk2.IsVisible = false;
                    stckCamera.GestureRecognizers.Clear();
                }

                if (Settings.Files == 0)
                {
                    stckFileUpload.Opacity = 0.5;
                    stckFileUpload.GestureRecognizers.Clear();
                }

                if (Settings.Chat == 0)
                {
                    stckChat.Opacity = 0.5;
                    stckChat.GestureRecognizers.Clear();
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
                if (Settings.alllabeslvalues != null && Settings.userRoleID != (int)UserRoles.SuperAdmin)
                {
                    var valuesDef = Settings.alllabeslvalues.Where(x => x.LanguageID == Settings.LanguageID && x.VersionID == Settings.VersionID).ToList();
                    Settings.APhoto = valuesDef.Where(wr => wr.FieldID == "TagAPhotoCount").Select(g => g.Status == 0 ? 0 : 1).FirstOrDefault();
                    Settings.BPhoto = valuesDef.Where(wr => wr.FieldID == "TagBPhotoCount").Select(g => g.Status == 0 ? 0 : 1).FirstOrDefault();
                    Settings.Files = valuesDef.Where(wr => wr.FieldID == "TagFilesCount").Select(g => g.Status == 0 ? 0 : 1).FirstOrDefault();
                    Settings.Chat = valuesDef.Where(wr => wr.FieldID == "TagQACount").Select(g => g.Status == 0 ? 0 : 1).FirstOrDefault();
                    Settings.IsAppLabelCall = ((Settings.APhoto == 0 && Settings.BPhoto == 0) || (Settings.Files == 0) || (Settings.Chat == 0)) == true ? true : false;

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

                Settings.startPageYPS = 0;
                Settings.toGoPageIndex = 0;
                PageSize.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "PageSizeDDLBinding -> in Main Page.cs " + Settings.userLoginID);
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
                    Settings.pageSizeYPS = (int)senderobj.SelectedItem;
                    pageSizeNameYPS.Text = Convert.ToString(senderobj.SelectedItem);
                    Settings.selectedIndexYPS = senderobj.SelectedIndex;

                    if (reloadGrid == true)
                    {
                        Settings.startPageYPS = 0;
                        Settings.toGoPageIndex = 0;
                        lastButtonCount = dataPager.NumericButtonCount;

                        dataPager.MoveToPage(0);
                        await BindGridData(false, false);
                    }

                    Vm.activityIndicator = false;
                    UserDialogs.Instance.HideLoading();
                    Settings.mutipleTimeClick = false;
                }
                PageSize.SelectedIndexChanged += PageSizeChanged;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "PageSizeChanged -> in Main Page.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method is for binding the records/date to the data grid.
        /// </summary>
        /// </summary>
        /// <param name="iSPagingNoAuto"></param>
        /// <returns></returns>
        public async Task BindGridData(bool iscall, bool ISRefresh)
        {
            try
            {
                await Task.Delay(50);
                UserDialogs.Instance.ShowLoading("Loading...");

                YPSLogger.TrackEvent("MainPage.xaml.cs", "in BindGridData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                try
                {
                    bool ifNotRootedDevie;

                    if (Device.RuntimePlatform == Device.Android)
                    {
                        ///By using dependency service, checking mobile is root or not.
                        ifNotRootedDevie = await DependencyService.Get<IRootDetection>().CheckIfRooted();
                    }
                    else
                    {
                        ifNotRootedDevie = true;
                    }

                    ifNotRootedDevie = true;/// need to remove this line when run in device 

                    if (ifNotRootedDevie)
                    {

                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            if (!String.IsNullOrEmpty(Settings.access_token))
                            {
                                await Vm.RememberUserDetails();

                                SendPodata sendPodata = new SendPodata();
                                sendPodata.UserID = Settings.userLoginID;
                                sendPodata.PageSize = Settings.pageSizeYPS;
                                sendPodata.StartPage = Settings.startPageYPS;
                                Vm.SearchResultGet(sendPodata);

                                var result = await trackService.LoadPoDataService(sendPodata);

                                if (result != null)
                                {
                                    if (result.status != 0)
                                    {
                                        Vm.NoRecordsLbl = false;
                                        Vm.ClearSearchLbl = false;
                                        Vm.DataGrid = true;
                                        Vm.PageCount = result.data.listCount;

                                        foreach (var data in result.data.allPoData)
                                        {
                                            if (data.TagNumber != null)
                                            {
                                                #region Chat
                                                if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.SuperUser || Settings.userRoleID == (int)UserRoles.Viewer)
                                                {
                                                    data.chatImage = "minus.png";
                                                }
                                                else
                                                {
                                                    if (data.TagQACount == 0)
                                                    {
                                                        if (data.TagQAClosedCount > 0)
                                                        {
                                                            data.chatImage = "chatIcon.png";
                                                            data.chatTickVisible = true;
                                                        }
                                                        else
                                                        {
                                                            data.chatImage = "minus.png";
                                                        }
                                                    }
                                                    else
                                                    {
                                                        data.chatImage = "chatIcon.png";
                                                        data.countVisible = true;
                                                    }
                                                }
                                                #endregion

                                                #region Before Photo & After Photo
                                                if (data.PUID == 0)
                                                {
                                                    if (data.IsPhotoRequired == 0)
                                                        data.cameImage = "cross.png";
                                                    else
                                                        data.cameImage = "minus.png";
                                                }
                                                else
                                                {
                                                    if (data.ISPhotoClosed == 1)
                                                    {
                                                        data.cameImage = "Chatcamera.png";
                                                        data.photoTickVisible = true;

                                                        data.imgCamOpacityB = data.imgTickOpacityB = (data.TagBPhotoCount == 0) ? 0.5 : 1.0;
                                                        data.imgCamOpacityA = data.imgtickOpacityA = (data.TagAPhotoCount == 0) ? 0.5 : 1.0;
                                                    }
                                                    else
                                                    {
                                                        data.cameImage = "Chatcamera.png";
                                                        data.BPhotoCountVisible = true;
                                                        data.APhotoCountVisible = true;
                                                    }
                                                }
                                                #endregion

                                                #region File upload 
                                                if (data.TagFilesCount == 0 && data.FUID == 0)
                                                {
                                                    data.fileImage = "minus.png";
                                                }
                                                else
                                                {
                                                    if (data.ISFileClosed > 0)
                                                    {
                                                        data.fileImage = "attachb.png";
                                                        data.fileTickVisible = true;
                                                    }
                                                    else
                                                    {
                                                        data.fileImage = "attachb.png";
                                                        data.filecountVisible = true;
                                                    }
                                                }
                                                #endregion
                                            }
                                            else if (data.TagNumber == null)
                                            {
                                                data.countVisible = false;
                                                data.filecountVisible = false;
                                                data.fileTickVisible = false;
                                                data.chatTickVisible = false;
                                                data.BPhotoCountVisible = false;
                                                data.APhotoCountVisible = false;
                                                data.photoTickVisible = false;
                                                data.POS = null;
                                                data.SUB = null;
                                                data.IS_POS = null;
                                                data.IS_SUB = null;
                                                data.emptyCellValue = "No records to display";
                                            }
                                        }

                                        Vm.PoDataCollections = new ObservableCollection<AllPoData>(result.data.allPoData);

                                        if (Vm.PoDataCollections.Count > 0)
                                        {
                                            dataGrid.ItemsSource = Vm.PoDataCollections;

                                            var roundcount = Math.Ceiling((decimal)Vm.PageCount / (decimal)Settings.pageSizeYPS);
                                            dataPager.PageCount = (Int32)roundcount;
                                            dataPager.NumericButtonCount = (Int32)roundcount;

                                            Device.BeginInvokeOnMainThread(() =>
                                        {
                                            if (Device.RuntimePlatform == Device.Android)
                                            {
                                                if (dataPager.NumericButtonCount < 6 && dataPager.NumericButtonCount != 0)
                                                {
                                                    dataPager.NumericButtonsGenerateMode = NumericButtonsGenerateMode.Auto;
                                                }
                                                else
                                                {
                                                    dataPager.NumericButtonsGenerateMode = NumericButtonsGenerateMode.Manual;
                                                }
                                            }
                                        });
                                        }
                                        await Vm.CheckingSearchValues();
                                    }
                                    else
                                    {
                                        Vm.NoRecordsLbl = true;
                                        await Vm.CheckingSearchValues();
                                        Vm.DataGridShowColumn = false;
                                        //ClearSearchLbl = true;
                                        Vm.DataGrid = false;
                                        Vm.activityIndicator = false;
                                        iscall = false;
                                    }

                                    Vm.GetallApplabels();
                                    iscalled = iscall;

                                    if (iscalled == true)
                                    {
                                        await Gridcolumsupdate();
                                        await Selectcolums();
                                    }
                                    else if (ISRefresh == true && Settings.EndRefresh2 == false && (result.data.allPoData != null && result.data.allPoData.Count > 0))
                                    {
                                        Settings.EndRefresh2 = true;

                                        if (ISRefresh == true && Settings.EndRefresh2 == true)
                                        {
                                            await Gridcolumsupdate();
                                            await Selectcolums();
                                        }
                                    }
                                    else
                                    {
                                        if (Settings.RedirectPagefirsttime == "NewAdded")
                                        {
                                            isloading = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //  DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong please try again.");
                                }
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Warning!", "Something went wrong please try again.", "Ok");
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
                        YPSLogger.ReportException(ex, "LoginMethod method ->Your phone is rooted , please unroot to use app in LoginPageViewModel ");
                        await App.Current.MainPage.DisplayAlert("Warning", "Your phone is rooted , please unroot to use app", "OK");
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "BindGridData method -> in PoDataViewModel! " + Settings.userLoginID);
                    await trackService.Handleexception(ex);
                    Vm.activityIndicator = false;
                    UserDialogs.Instance.HideLoading();
                }
                Vm.profileSettingVisible = true;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "BindGridData method -> in MainPage.cs " + Settings.userLoginID);
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// This method is called when the record in grid is expanded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DataGrid_GroupExpanding(object sender, GroupChangingEventArgs e)
        {
            try
            {
                indicator.IsBusy = true;
                indicator.IsVisible = true;

                indicator.AnimationType = AnimationTypes.SlicedCircle;
                await Task.Delay(1000);

                grid.Children.Add(indicator);
                indicator.IsBusy = false;
                indicator.IsVisible = false;
            }
            catch (Exception ex)
            {
                trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "DataGrid_GroupExpanding method -> in MainPage.cs " + Settings.userLoginID);
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

            //For Enable and Disble master detail page menu gesture
            (Application.Current.MainPage as YPSMasterPage).IsGestureEnabled = true;

            /// Get PN count
            Vm.GetPNCount();

            Settings.countmenu = 1;
            try
            {
                Settings.mutipleTimeClick = false;

                if (Settings.refreshPage == 1)
                {
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        Settings.refreshPage = 0;

                        if (Settings.IsFilterreset == true)
                        {
                            Settings.IsFilterreset = false;
                            await BindGridData(false, false);
                            UserDialogs.Instance.HideLoading();
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
                YPSLogger.ReportException(ex, "OnAppearing method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
                Settings.ShowSuccessAlert = false;
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
            await SecureStorage.SetAsync("mainPageisOn", "0");
        }

        /// <summary>
        /// This method is called when all the Tags of a particular PO is selected, by selecting the select all checkbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void selectAll_checkBox(object sender, StateChangedEventArgs e)
        {
            try
            {
                var group = (sender as SfCheckBox).BindingContext as Syncfusion.Data.Group;

                if (group != null)
                {
                    var records = group.Records;

                    if (e.IsChecked == true)
                    {
                        foreach (var record in dataGrid.View.TopLevelGroup.Groups)
                        {
                            if (record != group & record.Source.Any())
                                (record.Source[0] as AllPoData).IsClosed = false;
                        }

                        dataGrid.SelectedItems.Clear();

                        foreach (var item in records)
                        {
                            dataGrid.SelectedItems.Add(item.Data);
                        }
                    }
                    else
                    {
                        foreach (var item in records)
                        {
                            if (dataGrid.SelectedItems.Contains(item.Data))
                            {
                                dataGrid.SelectedItems.Remove(item.Data);
                            }
                        }
                    }
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
        /// This method is for applying style to inner/child grid, when parent grid is expanded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DataGrid_QueryCellStyle(object sender, QueryCellStyleEventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItems.Contains(e.RowData))
                {
                    if (e.ColumnIndex == 1)
                    {
                        e.Style.BackgroundColor = Color.FromHex("#0300ef");
                    }
                }

                e.Style.CellStylePreference = StylePreference.StyleAndSelection;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DataGrid_QueryCellStyle method -> in Main Page.cs " + Settings.userLoginID);
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
        /// This method gets called when clicked on upload icon present in main/parent grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void tap_GroupFileUpload(object sender, EventArgs e)
        {
            Vm.activityIndicator = true;
            try
            {
                var imgid = sender as Image;
                var seletedData = Vm.PoDataCollections.Where(x => x.POShippingNumber == imgid.ClassId).FirstOrDefault();

                if (Settings.fileUploadPageCount == 0)
                {
                    StartUploadFileModel poShipNum_obj = new StartUploadFileModel();
                    poShipNum_obj.alreadyExit = seletedData.POShippingNumber;
                    Settings.fileUploadPageCount = 1;
                    Settings.isFinalvol = seletedData.ISFinalVol;
                    await Navigation.PushAsync(new FileUpload(poShipNum_obj, seletedData.POID, seletedData.FUID, "plFile", false));
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_GroupFileUpload method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
            Vm.activityIndicator = false;
        }

        bool mTimeOpenShipMarkCheck = false;

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
        /// This method gets called when clicked on ShippingMark(Star) icon, on grid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void tap_shipmarkPrinter(object sender, EventArgs e)
        {
            Vm.activityIndicator = true;
            await Task.Delay(100);
            try
            {
                if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("You don't have access ship Print");
                }
                else
                {
                    if (!mTimeOpenShipMarkCheck)
                    {
                        mTimeOpenShipMarkCheck = true;
                        var imgid = sender as Image;
                        var tapedItem = Vm.PoDataCollections.Where(x => x.POShippingNumber == imgid.ClassId).FirstOrDefault();

                        YPSService yPSService = new YPSService();
                        var printResult = await yPSService.PrintPDFByUsingPOID(tapedItem.POID);

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
                        mTimeOpenShipMarkCheck = false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "tap_shipmarkPrinter method -> in Main Page.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                UserDialogs.Instance.HideLoading();
            }
            Vm.activityIndicator = false;
        }
    }
}
