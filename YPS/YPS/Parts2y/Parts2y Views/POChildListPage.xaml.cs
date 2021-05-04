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

        public POChildListPage(ObservableCollection<AllPoData> potag, SendPodata sendpodata)
        {
            try
            {
                trackService = new YPSService();
                InitializeComponent();
                Settings.refreshPage = 1;

                BindingContext = Vm = new POChildListPageViewModel(Navigation, potag, sendpodata);

                if (Settings.VersionID == 5 || Settings.VersionID == 1)
                {
                    loadStack.IsVisible = false;
                }

                //if (Device.RuntimePlatform == Device.iOS)
                //{
                //    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                //    safeAreaInset.Bottom = 0;
                //    safeAreaInset.Top = 30;
                //    headerpart.Padding = safeAreaInset;
                //}


                if (Settings.VersionID != 2 && Settings.VersionID != 4 &&
                    Settings.VersionID != 5 && Settings.VersionID != 3 && Settings.VersionID != 1)
                {
                    Vm.IsRightArrowVisible = false;
                    ChildDataList.SelectionMode = SelectionMode.None;
                }

                Settings.refreshPage = 1;

                #region Subscribing MessageCenter
                //MessagingCenter.Subscribe<string, string>("PushNotificationCame", "IncreaseCount", (sender, args) =>
                //{
                //    Vm.NotifyCountTxt = args;
                //});

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
                        //item.TagTaskStatus = item.TagTaskStatus == 0 ? 1 : item.TagTaskStatus;

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

                #endregion

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    imgCamera.Opacity = CameraLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    imgFileUpload.Opacity = FileUploadLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "FileUpload".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    imgChat.Opacity = ChatLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CreateChat".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    imgPrinter.Opacity = PrinterLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PrintTagReportDownload".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    imgDone.Opacity = DoneLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "TaskComplete".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;

                    if (Settings.VersionID == 2)
                    {
                        Vm.LoadTextColor = Color.Black;
                        loadStack.IsVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CarrierInspection".Trim())
                          .FirstOrDefault()) != null ? true : false;
                    }
                    else if (Settings.VersionID == 4 || Settings.VersionID == 3)
                    {
                        Vm.LoadTextColor = Color.Black;
                        loadStack.IsVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "LoadInspection".Trim())
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
                YPSLogger.ReportException(ex, "POChildListPage Constructor -> in POChildListPage.xaml.cs " + Settings.userLoginID);
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
                YPSLogger.TrackEvent("POChildListPage.xaml.cs", "OnAppearing " + DateTime.Now + " UserId: " + Settings.userLoginID);

                Vm.loadindicator = true;
                base.OnAppearing();

                //if (Navigation.NavigationStack.Count == 1)
                //{
                //    Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[0]);
                //    Navigation.InsertPageBefore(new HomePage(), Navigation.NavigationStack[0]);
                //}

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

                if (Settings.VersionID == 4 || Settings.VersionID == 3)
                {
                    //if (POChildListPageViewModel.isalldone == true)
                    //{
                    Vm.LoadTextColor = Color.Black;
                    Vm.MoveToNextPageCmd = new Command(Vm.MoveToNextPage);
                    //}
                    //else
                    //{
                    //    Vm.LoadTextColor = Color.Gray;
                    //    Vm.MoveToNextPageCmd = null;
                    //}
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
                    if (imgCamera.Opacity == 1.0)
                    {
                        stckCamera.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.tap_InitialCamera),
                            CommandParameter = ChildDataList,
                        });

                    }
                    if (imgFileUpload.Opacity == 1.0)
                    {
                        stckFileUpload.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.tap_InitialFileUpload),
                            CommandParameter = ChildDataList,
                        });

                    }
                    if (imgChat.Opacity == 1.0)
                    {
                        stckChat.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.tap_OnMessage),
                            CommandParameter = ChildDataList,
                        });
                    }
                    if (imgPrinter.Opacity == 1.0)
                    {
                        stckPrinter.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.tap_Printer),
                            CommandParameter = ChildDataList,
                        });
                    }
                    if (imgDone.Opacity == 1.0)
                    {
                        stckDone.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(Vm.MarkAsDone),
                            CommandParameter = ChildDataList,
                        });
                    }

                    if ((Settings.VersionID == 4 || Settings.VersionID == 3) || Settings.VersionID == 2)
                    {
                        loadStack.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(async () => await Vm.TabChange("load")),
                        });
                    }


                    //if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                    //{
                    //    imgCamera.Opacity = CameraLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    //    imgFileUpload.Opacity = FileUploadLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "FileUpload".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    //    imgChat.Opacity = ChatLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "CreateChat".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    //    imgPrinter.Opacity = PrinterLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PrintTagReportDownload".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    //    imgDone.Opacity = DoneLbl.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "TaskComplete".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;

                    //    if (imgCamera.Opacity == 0.5)
                    //    {
                    //        stckCamera.GestureRecognizers.Clear();

                    //    }
                    //    if (imgFileUpload.Opacity == 0.5)
                    //    {
                    //        stckFileUpload.GestureRecognizers.Clear();

                    //    }
                    //    if (imgChat.Opacity == 0.5)
                    //    {
                    //        stckChat.GestureRecognizers.Clear();
                    //    }
                    //    if (imgPrinter.Opacity == 0.5)
                    //    {
                    //        stckPrinter.GestureRecognizers.Clear();
                    //    }
                    //    if (imgDone.Opacity == 0.5)
                    //    {
                    //        stckPrinter.GestureRecognizers.Clear();
                    //    }
                    //}

                });
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "CreateGestureWithCommands -> in POChildListPage.cs " + Settings.userLoginID);
                Vm.loadindicator = false;
            }
            Vm.loadindicator = false;
        }
        #endregion

        private async void MoveToPage(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;
                var val = sender as Label;
                var selecteditem = (((TappedEventArgs)e).Parameter as CollectionView).SelectedItems as ObservableCollection<AllPoData>;

                if (selecteditem != null)
                {
                    if (val.StyleId == "file")
                    {
                        if (selecteditem[0].TagFilesCount > 0)
                        {
                            await Navigation.PushAsync(new FileUpload(null, selecteditem[0].POID
                           , selecteditem[0].FUID, "fileUpload", selecteditem[0].fileTickVisible));
                        }
                        else
                        {
                            await Navigation.PushAsync(new FileUpload(null, selecteditem[0].POID
                          , selecteditem[0].FUID, "fileUpload", selecteditem[0].fileTickVisible));
                        }

                    }
                    else if (val.StyleId == "photo")
                    {
                        await Navigation.PushAsync(new PhotoUpload(null, selecteditem[0], "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, selecteditem[0].photoTickVisible));
                    }
                    else
                    {
                        await Navigation.PushAsync(new QnAlistPage(selecteditem[0].POID, selecteditem[0].POTagID, Settings.QAType));
                    }
                }
                else
                {
                    await DisplayAlert("Select tag", "", "Cancel");
                }
            }
            catch (Exception ex)
            {
                Vm.loadindicator = false;
            }
            Vm.loadindicator = false;
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var val = sender as Plugin.InputKit.Shared.Controls.CheckBox;
                var item = (AllPoData)val.BindingContext;

                if (val.IsChecked == true)
                {
                    item.IsChecked = true;
                    item.SelectedTagBorderColor = Settings.Bar_Background;
                }
                else
                {
                    item.IsChecked = false;
                    item.SelectedTagBorderColor = Color.Transparent;
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}