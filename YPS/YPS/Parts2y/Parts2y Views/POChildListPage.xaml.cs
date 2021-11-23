using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;
using YPS.Views;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class POChildListPage : ContentPage
    {
        POChildListPageViewModel Vm;
        public static Timer loadertimer;
        YPSService trackService;
        int? taskResourceID;

        public POChildListPage(ObservableCollection<AllPoData> potag, SendPodata sendpodata)
        {
            try
            {
                trackService = new YPSService();
                InitializeComponent();
                Settings.refreshPage = 1;
                taskResourceID = potag[0]?.TaskResourceID;

                BindingContext = Vm = new POChildListPageViewModel(Navigation, potag, sendpodata, this);

                Settings.refreshPage = 1;

                #region Subscribing MessageCenter

                MessagingCenter.Subscribe<string, string>("APhotoCount", "msgA", (sender, args) =>
                {
                    var updateCount = Settings.AllPOData.Where(x => x.PUID == Settings.currentPuId).ToList();
                    updateCount.ForEach(a => { a.TagAPhotoCount = Convert.ToInt32(args); });

                });

                MessagingCenter.Subscribe<string, string>("BPhotoCount", "msgB", (sender, args) =>
                {
                    var updateCount = Settings.AllPOData.Where(x => x.PUID == Settings.currentPuId).ToList();
                    updateCount.ForEach(a => { a.TagBPhotoCount = Convert.ToInt32(args); });
                });

                MessagingCenter.Subscribe<string, string>("PhotoComplted", "showtickMark", (sender, args) =>
                {
                    var updateCount = Settings.AllPOData.Where(x => x.PUID == Convert.ToInt16(args)).ToList();// Settings.currentPuId).ToList();
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
                    var updateCount = Settings.AllPOData
                    .Where(x => x.POTagID == (Settings.currentPoTagId_Inti.Where(k => k.POTagID == x.POTagID).Select(p => p.POTagID).FirstOrDefault()))
                    .Select(c =>
                    {
                        c.cameImage = "Chatcamera.png";
                        c.TagAPhotoCount = Convert.ToInt16(count[0]);
                        c.TagBPhotoCount = Convert.ToInt16(count[1]);
                        c.PUID = Convert.ToInt16(count[2]);
                        c.BPhotoCountVisible = true;
                        c.APhotoCountVisible = true;
                        c.IsPhotosVisible = true;
                        c.TagTaskStatusIcon = c.TagTaskStatus == 0 ? Icons.Progress : Icons.Pending;
                        c.TagTaskStatus = c.TagTaskStatus == 0 ? 1 : c.TagTaskStatus;
                        return c;
                    }).ToList();
                });

                MessagingCenter.Subscribe<string, string>("FilesCountI", "msgFileInitial", (sender, args) =>
                {
                    var count = args.Split(',');
                    var updateCounts = Settings.AllPOData
                     .Where(x => x.POTagID == (Settings.currentPoTagId_Inti_F.Where(k => k.POTagID == x.POTagID).Select(p => p.POTagID).FirstOrDefault()))
                     .Select(c =>
                     {
                         c.fileImage = "attachb.png";
                         c.TagFilesCount = Convert.ToInt16(count[0]);
                         c.FUID = Convert.ToInt16(count[1]);
                         c.filecountVisible = true;
                         c.IsFilesVisible = true;
                         c.TagTaskStatusIcon = c.TagTaskStatus == 0 ? Icons.Progress : Icons.Pending;
                         c.TagTaskStatus = c.TagTaskStatus == 0 ? 1 : c.TagTaskStatus;
                         return c;
                     }).ToList();
                });

                MessagingCenter.Subscribe<string, string>("FilesCounts", "msgF", (sender, args) =>
                {
                    var count = args.Split(',');
                    var updateCount = Settings.AllPOData.Where(x => x.FUID == Convert.ToInt32(count[1]));
                    foreach (var item in updateCount)
                    {
                        item.TagFilesCount = Convert.ToInt32(count[0]);
                    }
                });

                MessagingCenter.Subscribe<string, string>("FileComplted", "showFileAsComplete", (sender, args) =>
                {
                    var updateCount = Settings.AllPOData.Where(x => x.FUID == Convert.ToInt32(args));// Settings.currentFuId);
                    foreach (var item in updateCount)
                    {
                        item.filecountVisible = false;
                        item.fileTickVisible = true;
                    }

                });

                MessagingCenter.Subscribe<string, string>("FilesCount", "deleteFile", (sender, args) =>
                {
                    var count = args.Split(',');
                    var updateCount = Settings.AllPOData.Where(x => x.FUID == Convert.ToInt32(count[1]));// Settings.currentFuId);
                    foreach (var item in updateCount)
                    {
                        item.TagFilesCount = Convert.ToInt32(count[0]);// Convert.ToInt32(args);                                                                                                                                            
                    }
                });

                MessagingCenter.Subscribe<string, string>("QAChatCount", "updatecount", (sender, args) =>
                {
                    var countdata = args.Split(',');
                    var updateCount = Settings.AllPOData.Where(wr => wr.POID == Convert.ToInt32(countdata[0]) &&
                    wr.POTagID == Convert.ToInt32(countdata[1])).FirstOrDefault();
                    updateCount.TagQACount = Convert.ToInt32(countdata[2]);

                });
                #endregion

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    imgCamera.Opacity = CameraLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "PhotoUpload".Trim().ToLower()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    imgFileUpload.Opacity = FileUploadLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "FileUpload".Trim().ToLower()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    imgChat.Opacity = ChatLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "CreateChat".Trim().ToLower()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    imgPrinter.Opacity = PrinterLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "PrintTagReportDownload".Trim().ToLower()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    imgDone.Opacity = DoneLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "TaskComplete".Trim().ToLower()).FirstOrDefault()) != null ? 1.0 : 0.5;

                    if (Settings.VersionID == 1)
                    {
                        loadStack.IsVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "ELoadInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;

                    }
                    else if (Settings.VersionID == 2)
                    {
                        loadStack.IsVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "CCarrierInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;

                    }
                    else if (Settings.VersionID == 3)
                    {
                        loadStack.IsVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "KrLoadInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;

                    }
                    else if (Settings.VersionID == 4)
                    {
                        loadStack.IsVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "KpLoadInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;

                    }
                    else if (Settings.VersionID == 5)
                    {
                        loadStack.IsVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim().ToLower() == "PLoadInspection".Trim().ToLower())
                            .FirstOrDefault()) != null ? true : false;
                    }
                }

                #region Assigning method that must execute when page is loaded, for binding gestures
                loadertimer = new Timer();
                loadertimer.Interval = 1000;
                loadertimer.Elapsed += CreateGestureWithCommands;
                loadertimer.AutoReset = false;
                loadertimer.Enabled = true;
                #endregion
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "POChildListPage constructor -> in POChildListPage.xaml.cs " + Settings.userLoginID);
                Task.Run(() => trackService.Handleexception(ex)).Wait();
            }
        }


        /// <summary>
        /// This method gets called when page is appearing.
        /// </summary>
        protected async override void OnAppearing()
        {
            try
            {
                YPSLogger.TrackEvent("POChildListPage.xaml.cs", " in OnAppearing method" + DateTime.Now + " UserId: " + Settings.userLoginID);

                Vm.loadindicator = true;
                base.OnAppearing();

                if (Settings.IsRefreshPartsPage == true)
                {
                    if (Vm.AllTabVisibility == true)
                    {
                        await Vm.All_Tap();
                    }
                    else if (Vm.CompleteTabVisibility == true)
                    {
                        await Vm.Complete_Tap();
                    }
                    else if (Vm.InProgressTabVisibility == true)
                    {
                        await Vm.InProgress_Tap();
                    }
                    else
                    {
                        await Vm.Pending_Tap();
                    }

                    Settings.IsRefreshPartsPage = false;
                    Vm.SelectedTagCount = 0;
                    Vm.SelectedTagCountVisible = false;
                }

                Vm.loadindicator = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in POChildListPage.xaml.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
                Settings.ShowSuccessAlert = false;
                Vm.loadindicator = false;
            }
            finally
            {
                Settings.ShowSuccessAlert = false;
                Vm.loadindicator = false;
            }
        }

        #region Add gesture after page is loaded
        /// <summary>
        /// Gets called after 1 sec. of page getting loaded, for adding gestures with command to labels & stacks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public async void CreateGestureWithCommands(object sender, ElapsedEventArgs args)
        {
            try
            {
                loadertimer.Enabled = false;

                Device.BeginInvokeOnMainThread(() =>
                {
                    stckCamera.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.tap_InitialCamera),
                        CommandParameter = ChildDataList,
                    });

                    stckFileUpload.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.tap_InitialFileUpload),
                        CommandParameter = ChildDataList,
                    });

                    stckChat.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.tap_OnMessage),
                        CommandParameter = ChildDataList,
                    });

                    stckPrinter.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.tap_Printer),
                        CommandParameter = ChildDataList,
                    });

                    stckDone.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(Vm.MarkAsDone),
                        CommandParameter = ChildDataList,
                    });


                    loadStack.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(async () => await Vm.TabChange("load")),
                    });
                });
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "CreateGestureWithCommands method -> in POChildListPage.xaml.cs " + Settings.userLoginID);
                Vm.loadindicator = false;
            }
            Vm.loadindicator = false;
        }
        #endregion

        private void CloseScanOrInspPopUp(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = false;

                Vm.IsScanOrInspPopUpVisible = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CloseScanOrInspPopUp method -> in POChildListPage.xaml.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
                Vm.loadindicator = false;
            }
        }
    }
}