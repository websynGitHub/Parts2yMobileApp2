using System;
using System.Collections.Generic;
using System.Text;
using YPS.Views;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;
using Xamarin.Forms;
using YPS.Service;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using ZXing.Net.Mobile.Forms;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using System.Collections.ObjectModel;
using YPS.CommonClasses;
using YPS.Model;
using YPS.CustomToastMsg;
using YPS.Helpers;
using Newtonsoft.Json;
using static YPS.Model.SearchModel;
using ZXing.Mobile;
using System.Reflection;
using System.IO;
using Plugin.SimpleAudioPlayer;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class ScanPageViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        YPSService trackService;
        ScanPage pagename;
        public ICommand reScanCmd { set; get; }
        public ICommand MoveNextCmd { set; get; }
        public ICommand ScanResultCommand { set; get; }
        public ICommand FlashCommand { set; get; }
        public PhotoUploadModel selectedTagData { get; set; }
        public AllPoData selectedPOTagData { get; set; }
        public static int uploadType;
        public bool isInitialPhoto, isbuttonenable;

        public ScanPageViewModel(INavigation _Navigation, int uploadtype, PhotoUploadModel selectedtagdata, bool isinitialphoto, AllPoData selectedpoagdata, ScanPage page)
        {
            try
            {
                Navigation = _Navigation;
                pagename = page;
                uploadType = uploadtype;
                isInitialPhoto = isinitialphoto;
                selectedPOTagData = selectedpoagdata;
                selectedTagData = selectedtagdata;
                trackService = new YPSService();
                ChangeLabel();
                #region BInding tab & click event methods to respective ICommand properties
                reScanCmd = new Command(async () => await ReScan());
                MoveNextCmd = new Command(async () => await MoveNext(ScannedAllPOData));
                #endregion

                if (Settings.VersionID == 2 && uploadtype == 0)
                {
                    PageNextButton = "Insp";
                }
                else
                {
                    PageNextButton = "Photo";
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    isbuttonenable = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? true : false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScanPageViewModel constructor -> in ScanPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task ReScan()
        {
            try
            {
                IsScanPage = false;
                await OpenScanner();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ReScan method -> in ScanPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task OpenScanner()
        {
            try
            {
                var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                var requestedPermissionStatus = requestedPermissions[Permission.Camera];
                var pass1 = requestedPermissions[Permission.Camera];

                if (pass1 == PermissionStatus.Denied)
                {
                    IsScanPage = false;
                    var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needed to access the camera.", null, null, "Maybe later", "Settings");
                    switch (checkSelect)
                    {
                        case "Maybe later":
                            break;
                        case "Settings":
                            CrossPermissions.Current.OpenAppSettings();
                            break;
                    }
                }
                else if (pass1 == PermissionStatus.Granted)
                {
                    if (Settings.MobileScanProvider.Trim().ToLower() == "scandit".ToLower())
                    {
                        ScanerSettings scanset = new ScanerSettings();
                        SettingsArchiver.ArchiveSettings(scanset);
                        await Navigation.PushAsync(new ScannerPage(scanset, this));
                    }
                    else
                    {
                        await ZxingScanner();
                    }

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable.", "Ok");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OpenScanner method -> in ScanPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }


        public async Task ZxingScanner()
        {
            try
            {
                var overlay = new ZXingDefaultOverlay
                {
                    ShowFlashButton = true,
                    TopText = string.Empty,
                    BottomText = string.Empty,
                };

                overlay.BindingContext = overlay;

                var ScannerPage = new ZXingScannerPage(null, overlay);
                ScannerPage = new ZXingScannerPage(null, overlay);

                ScannerPage.OnScanResult += (scanresult) =>
                {
                    ScannerPage.IsScanning = false;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync(false);

                        ScannedResult = scanresult.Text;

                        if (!string.IsNullOrEmpty(ScannedResult))
                        {
                            IsPageVisible = true;

                            if (uploadType == 0 && selectedTagData == null)
                            {
                                await GetDataAndVerify();
                            }
                            else
                            {
                                IsPhotoBtnVisible = true;
                                await SingleTagDataVerification();
                            }
                        }
                    });
                };

                if (Navigation.ModalStack.Count == 0 ||
                                            Navigation.ModalStack.Last().GetType() != typeof(ZXingScannerPage))
                {
                    ScannerPage.AutoFocus();

                    await Navigation.PushAsync(ScannerPage, false);

                    overlay.FlashButtonClicked += (s, ed) =>
                    {
                        ScannerPage.ToggleTorch();
                    };
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ZxingScanner method -> in ScanPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task Scanditscan(string scanresult)
        {
            try
            {
                ScannedResult = scanresult;

                if (!string.IsNullOrEmpty(ScannedResult))
                {
                    IsPageVisible = true;

                    if (uploadType == 0 && selectedTagData == null)
                    {
                        await GetDataAndVerify();
                    }
                    else
                    {
                        IsPhotoBtnVisible = true;
                        await SingleTagDataVerification();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Scanditscan method -> in ScanPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task SingleTagDataVerification()
        {
            try
            {
                IndicatorVisibility = true;

                var assembly = typeof(App).GetTypeInfo().Assembly;

                Stream matchedsr = assembly.GetManifestResourceStream("YPS." + "okbeep.mp3");
                ISimpleAudioPlayer matchedbeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                matchedbeep.Load(matchedsr);

                Stream notmatchedsr = assembly.GetManifestResourceStream("YPS." + "ngbeep.mp3");
                ISimpleAudioPlayer notmatchedbeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                notmatchedbeep.Load(notmatchedsr);

                YPSLogger.TrackEvent("ScanPageViewModel.cs", " in GerDataAndVerify method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {

                    SendPodata sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;
                    sendPodata.TagNo = ScannedResult;
                    sendPodata.EventID = -1;

                    var result = await trackService.LoadPoDataService(sendPodata);

                    if (selectedPOTagData == null && selectedTagData != null && selectedTagData.photoTags != null && result != null && result.data != null && result.data.allPoDataMobile != null)
                    {
                        IsNoJobMsgVisible = false;
                        selectedPOTagData = result.data.allPoDataMobile.Where(wr => wr.POTagID == selectedTagData.photoTags[0].POTagID).FirstOrDefault();
                    }
                    else
                    {
                        IsNoJobMsgVisible = true;
                    }

                    if (selectedTagData != null)
                    {
                        if (!string.IsNullOrEmpty(selectedTagData.photoTags[0].TagNumber))
                        {
                            ScannedCompareData = (selectedTagData.photoTags[0].TagNumber == ScannedResult) ? ScannedResult : null;
                        }
                        else
                        {
                            ScannedCompareData = (selectedTagData.photoTags[0].IdentCode == ScannedResult) ? ScannedResult : null;
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(selectedPOTagData.TagNumber))
                        {
                            ScannedCompareData = (selectedPOTagData.TagNumber == ScannedResult) ? ScannedResult : null;
                        }
                        else
                        {
                            ScannedCompareData = (selectedPOTagData.IdentCode == ScannedResult) ? ScannedResult : null;
                        }
                    }

                    if (!string.IsNullOrEmpty(ScannedCompareData))
                    {
                        ScannedOn = String.Format(Settings.DateFormat, DateTime.Now);
                        IsNoJobMsgVisible = false;

                        if (selectedTagData != null)
                        {
                            StatusText = selectedTagData.photoTags[0].TagTaskStatus == 2 ? "Done" : "Verified";
                        }
                        else
                        {
                            StatusText = selectedPOTagData.TagTaskStatus == 2 ? "Done" : "Verified";
                        }

                        matchedbeep.Play();
                        StatusTextBgColor = Color.DarkGreen;
                        ScannedValue = ScannedResult;

                        if (isbuttonenable == true)
                        {
                            IsPhotoEnable = true;
                            PhotoOpacity = 1.0;
                        }
                    }
                    else
                    {
                        ScannedOn = String.Format(Settings.DateFormat, DateTime.Now);
                        notmatchedbeep.Play();
                        StatusText = "Not matched";
                        StatusTextBgColor = Color.Red;
                        ScannedValue = ScannedResult;
                        IsPhotoEnable = false;
                        PhotoOpacity = 0.5;
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
                YPSLogger.ReportException(ex, "SingleTagDataVerification method -> in ScanPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async Task GetDataAndVerify()
        {
            try
            {
                IndicatorVisibility = true;

                var assembly = typeof(App).GetTypeInfo().Assembly;

                Stream matchedsr = assembly.GetManifestResourceStream("YPS." + "okbeep.mp3");
                ISimpleAudioPlayer matchedbeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                matchedbeep.Load(matchedsr);

                Stream notmatchedsr = assembly.GetManifestResourceStream("YPS." + "ngbeep.mp3");
                ISimpleAudioPlayer notmatchedbeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                notmatchedbeep.Load(notmatchedsr);

                YPSLogger.TrackEvent("ScanPageViewModel.cs", " in GerDataAndVerify method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {

                    SendPodata sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;
                    sendPodata.TagNo = ScannedResult;
                    sendPodata.EventID = -1;

                    var result = await trackService.LoadPoDataService(sendPodata);

                    //if (result != null && result.data != null)
                    //{
                    if (result?.status == 1 && result?.data?.allPoDataMobile != null && result?.data?.allPoDataMobile?.Count > 0)
                    {
                        IsNoJobMsgVisible = false;

                        var groubbyval = result.data.allPoDataMobile.GroupBy(gb => gb.POShippingNumber);
                        ObservableCollection<AllPoData> PoDataCollections = new ObservableCollection<AllPoData>();
                        PoDataCollections = new ObservableCollection<AllPoData>(result.data.allPoDataMobile?
                            .OrderBy(o => o.EventID).ThenBy(tob => tob.TaskStatus).
                            ThenBy(tob => tob.TaskName));

                        if (PoDataCollections?.Count == 0)
                        {
                            PoDataCollections = new ObservableCollection<AllPoData>(result.data.allPoDataMobile
                               .Where(wr => wr.IdentCode == ScannedResult)?
                               .OrderBy(o => o.EventID).ThenBy(tob => tob.TaskStatus).
                               ThenBy(tob => tob.TaskName));
                        }

                        if (PoDataCollections?.Count > 0)
                        {
                            ScannedOn = String.Format(Settings.DateFormat, DateTime.Now);
                            matchedbeep.Play();
                            StatusText = "Verified";
                            StatusTextBgColor = Color.DarkGreen;
                            ScannedValue = ScannedResult;

                            await Navigation.PushAsync(new ScanVerifiedTagListPage(PoDataCollections, result.data.allPoDataMobile, uploadType), false);
                            if (Settings.scanredirectpage.Trim().ToLower() == "ScanPage".Trim().ToLower())
                            {
                                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count-3]);
                            }
                            Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count-2]);
                        }
                        else
                        {
                            ScannedOn = String.Format(Settings.DateFormat, DateTime.Now);
                            notmatchedbeep.Play();
                            StatusText = "Not matched";
                            StatusTextBgColor = Color.Red;
                            ScannedValue = ScannedResult;
                            IsPhotoEnable = false;
                            PhotoOpacity = 0.5;
                        }
                    }
                    else
                    {
                        IsNoJobMsgVisible = true;
                        notmatchedbeep.Play();
                    }
                    //}
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetDataAndVerify method -> in ScanPageViewModel.cs " + Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async Task MoveNext(AllPoData podata)
        {
            try
            {
                if (IsPhotoEnable == true)
                {
                    IndicatorVisibility = true;
                    YPSLogger.TrackEvent("ScanPageViewModel.cs", " in PhotoUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        if (Settings.VersionID == 2 && uploadType == 0)
                        {
                            Settings.POID = podata.POID;
                            Settings.TaskID = podata.TaskID;
                            await Navigation.PushAsync(new CVinInspectQuestionsPage(podata, false), false);
                        }
                        else if (uploadType != 0)
                        {
                            if (isbuttonenable == true)
                            {
                                if (isInitialPhoto == true && selectedTagData != null)
                                {
                                    Settings.POID = selectedTagData.POID;
                                    Settings.TaskID = selectedTagData.photoTags[0].TaskID;
                                    podata = new AllPoData();
                                    podata.TagNumber = selectedTagData.photoTags[0].TagNumber;

                                    Settings.CanUploadPhotos = true;
                                    await Navigation.PopAsync(false);
                                }
                                else
                                {
                                    if (selectedPOTagData != null)
                                    {
                                        Settings.POID = selectedPOTagData.POID;
                                        Settings.TaskID = selectedPOTagData.TaskID;
                                    }

                                    Settings.CanUploadPhotos = true;
                                    await Navigation.PopAsync(false);
                                }
                            }
                        }
                        else
                        {
                            if (isbuttonenable == true)
                            {
                                #region PhotoUpload
                                if (podata.cameImage == "cross.png")
                                {
                                    await App.Current.MainPage.DisplayAlert("Upload", "Photo(s) not required to upload for the selected " + VinsOrParts + ".", "Ok");
                                    //DependencyService.Get<IToastMessage>().ShortAlert("Photos not required to upload for the selected " + VinsOrParts + ".");
                                }
                                else
                                {
                                    PhotoUploadModel selectedTagsData = new PhotoUploadModel();
                                    Settings.POID = podata.POID;
                                    Settings.TaskID = podata.TaskID;
                                    selectedTagsData.POID = podata.POID;
                                    selectedTagsData.isCompleted = podata.photoTickVisible;

                                    List<PhotoTag> lstdat = new List<PhotoTag>();

                                    if (podata.TagAPhotoCount == 0 && podata.TagBPhotoCount == 0 && podata.PUID == 0)
                                    {
                                        PhotoTag tg = new PhotoTag();

                                        if (podata.POTagID != 0)
                                        {
                                            tg.POTagID = podata.POTagID;
                                            tg.TagNumber = podata.TagNumber;
                                            Settings.Tagnumbers = podata.TagNumber;
                                            lstdat.Add(tg);

                                            selectedTagsData.photoTags = lstdat;
                                            Settings.currentPoTagId_Inti = lstdat;


                                            if (selectedTagsData.photoTags.Count != 0)
                                            {
                                                await Navigation.PushAsync(new PhotoUpload(selectedTagsData, podata, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false, false, false), false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        selectedTagsData.alreadyExit = "alreadyExit";

                                        if (podata.imgCamOpacityB != 0.5)
                                        {
                                            try
                                            {
                                                Settings.POID = podata.POID;
                                                Settings.TaskID = podata.TaskID;
                                                Settings.currentPuId = podata.PUID;
                                                Settings.BphotoCount = podata.TagBPhotoCount;
                                                await Navigation.PushAsync(new PhotoUpload(null, podata, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, podata.photoTickVisible, false, false), false);
                                            }
                                            catch (Exception ex)
                                            {
                                                YPSLogger.ReportException(ex, "tap_eachCamB method -> in POChildListPageViewModel " + Settings.userLoginID);
                                                var trackResult = await trackService.Handleexception(ex);
                                            }
                                        }
                                    }
                                }
                                #endregion PhotoUpload
                            }
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
                IndicatorVisibility = false;
                YPSLogger.ReportException(ex, "MoveNext method -> in ScanPageViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }

        }

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
                        var scan = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ScanLabel.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var status = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.StatusLabel.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var value = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ValueLabel.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var rescan = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ReScanLabel.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var photo = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.PhotoLabel.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        labelobj.ScanLabel.Name = (scan != null ? (!string.IsNullOrEmpty(scan.LblText) ? scan.LblText : labelobj.ScanLabel.Name) : labelobj.ScanLabel.Name) + " :";
                        labelobj.StatusLabel.Name = (status != null ? (!string.IsNullOrEmpty(status.LblText) ? status.LblText : labelobj.StatusLabel.Name) : labelobj.StatusLabel.Name) + " :";
                        labelobj.ValueLabel.Name = (value != null ? (!string.IsNullOrEmpty(value.LblText) ? value.LblText : labelobj.ValueLabel.Name) : labelobj.ValueLabel.Name) + " :";
                        labelobj.ReScanLabel.Name = (rescan != null ? (!string.IsNullOrEmpty(rescan.LblText) ? rescan.LblText : labelobj.ReScanLabel.Name) : labelobj.ReScanLabel.Name);
                        labelobj.PhotoLabel.Name = (photo != null ? (!string.IsNullOrEmpty(photo.LblText) ? photo.LblText : labelobj.PhotoLabel.Name) : labelobj.PhotoLabel.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabel method -> in ScanPageViewModel.cs " + Settings.userLoginID);
            }
        }

        #region Properties

        #region Labels
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields ScanLabel { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMScannedOn" };
            public DashboardLabelFields StatusLabel { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMStatus" };
            public DashboardLabelFields ValueLabel { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMValue" };
            public DashboardLabelFields ReScanLabel { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMbtnReScan" };
            public DashboardLabelFields PhotoLabel { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMbtnPhoto" };
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

        public bool _IsNoJobMsgVisible;
        public bool IsNoJobMsgVisible
        {
            get { return _IsNoJobMsgVisible; }
            set
            {
                _IsNoJobMsgVisible = value;
                RaisePropertyChanged("IsNoJobMsgVisible");
            }
        }

        public string _VinsOrParts = Settings.VersionID == 2 ? "VIN(s)" : "part(s)";
        public string VinsOrParts
        {
            get => _VinsOrParts;
            set
            {
                _VinsOrParts = value;
                RaisePropertyChanged("VinsOrParts");
            }
        }

        //public string _NoJobMessage =
        //    "The scanned " + VinsOrParts + " does not exist in any of your entity jobs.";
        public string NoJobMessage
        {
            get => "The scanned " + VinsOrParts + " does not exist in any of your entity jobs.";
            //set
            //{
            //    _NoJobMessage = value;
            //    RaisePropertyChanged("NoJobMessage");
            //}
        }

        public bool _IsAssignMsgVisible;
        public bool IsAssignMsgVisible
        {
            get { return _IsAssignMsgVisible; }
            set
            {
                _IsAssignMsgVisible = value;
                RaisePropertyChanged("IsAssignMsgVisible");
            }
        }

        public string _AssignInstructionMsg;
        public string AssignInstructionMsg
        {
            get { return _AssignInstructionMsg; }
            set
            {
                _AssignInstructionMsg = value;
                RaisePropertyChanged("AssignInstructionMsg");
            }
        }

        public bool _IsScanDataVisible;
        public bool IsScanDataVisible
        {
            get { return _IsScanDataVisible; }
            set
            {
                _IsScanDataVisible = value;
                RaisePropertyChanged("IsScanDataVisible");
            }
        }

        public MobileBarcodeScanningOptions _ScanningOptions;
        public MobileBarcodeScanningOptions ScanningOptions
        {
            get { return _ScanningOptions; }
            set
            {
                _ScanningOptions = value;
                RaisePropertyChanged("ScanningOptions");
            }
        }

        public bool _IsScannerPage;
        public bool IsScannerPage
        {
            get { return _IsScannerPage; }
            set
            {
                _IsScannerPage = value;
                RaisePropertyChanged("IsScannerPage");
            }
        }

        public string _FlashIcon = Icons.flashOffIC;
        public string FlashIcon
        {
            get { return _FlashIcon; }
            set
            {
                _FlashIcon = value;
                RaisePropertyChanged("FlashIcon");
            }
        }


        public bool _TorchOn = false;
        public bool TorchOn
        {
            get { return _TorchOn; }
            set
            {
                _TorchOn = value;
                RaisePropertyChanged("TorchOn");
            }
        }

        public string _ScannedCompareData;
        public string ScannedCompareData
        {
            get { return _ScannedCompareData; }
            set
            {
                _ScannedCompareData = value;
                RaisePropertyChanged("ScannedCompareData");
            }
        }

        public string _PageNextButton = (Settings.VersionID == 2 && uploadType == 0) == true ? "Insp" : "Photo";
        public string PageNextButton
        {
            get { return _PageNextButton; }
            set
            {
                _PageNextButton = value;
                RaisePropertyChanged("PageNextButton");
            }
        }

        public AllPoData _ScannedAllPOData;
        public AllPoData ScannedAllPOData
        {
            get { return _ScannedAllPOData; }
            set
            {
                _ScannedAllPOData = value;
                RaisePropertyChanged("ScannedAllPOData");
            }
        }

        public string _ScannedValue;
        public string ScannedValue
        {
            get { return _ScannedValue; }
            set
            {
                _ScannedValue = value;
                RaisePropertyChanged("ScannedValue");
            }
        }

        public string _ScannedOn;
        public string ScannedOn
        {
            get { return _ScannedOn; }
            set
            {
                _ScannedOn = value;
                RaisePropertyChanged("ScannedOn");
            }
        }

        public double _PhotoOpacity = 0.5;
        public double PhotoOpacity
        {
            get { return _PhotoOpacity; }
            set
            {
                _PhotoOpacity = value;
                RaisePropertyChanged("PhotoOpacity");
            }
        }

        public bool _IsPhotoBtnVisible;
        public bool IsPhotoBtnVisible
        {
            get { return _IsPhotoBtnVisible; }
            set
            {
                _IsPhotoBtnVisible = value;
                RaisePropertyChanged("IsPhotoBtnVisible");
            }
        }

        public bool _IsPhotoEnable;
        public bool IsPhotoEnable
        {
            get { return _IsPhotoEnable; }
            set
            {
                _IsPhotoEnable = value;
                RaisePropertyChanged("IsPhotoEnable");
            }
        }

        public string _StatusText;
        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                _StatusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

        private Color _StatusTextBgColor = Color.Gray;
        public Color StatusTextBgColor
        {
            get => _StatusTextBgColor;
            set
            {
                _StatusTextBgColor = value;
                RaisePropertyChanged("StatusTextBgColor");
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

        public string _ScannedResult;
        public string ScannedResult
        {
            get { return _ScannedResult; }
            set
            {
                _ScannedResult = value;
                RaisePropertyChanged("ScannedResult");
            }
        }

        public bool _IsScanPage;
        public bool IsScanPage
        {
            get { return _IsScanPage; }
            set
            {
                _IsScanPage = value;
                RaisePropertyChanged("IsScanPage");
            }
        }

        public bool _CanOpenScan = true;
        public bool CanOpenScan
        {
            get { return _CanOpenScan; }
            set
            {
                _CanOpenScan = value;
                RaisePropertyChanged("CanOpenScan");
            }
        }


        public bool _IsPageVisible;
        public bool IsPageVisible
        {
            get { return _IsPageVisible; }
            set
            {
                _IsPageVisible = value;
                RaisePropertyChanged("IsPageVisible");
            }
        }

        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                RaisePropertyChanged("IndicatorVisibility");
            }
        }
        #endregion Preopeties
    }
}
