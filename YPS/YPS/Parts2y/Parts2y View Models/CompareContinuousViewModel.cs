using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using ZXing.Net.Mobile.Forms;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class CompareContinuousViewModel : IBase
    {
        YPSService trackService;
        public string scanFor;
        public int historySerialNo = 1;
        List<CompareHistoryList> lstcomparehistory = new List<CompareHistoryList>();
        private CompareContinuous comparepage;
        public int? scancountpermit;
        private ScanerSettings scansetting;
        public ScanerSettings scanset = new ScanerSettings();
        public INavigation Navigation { get; set; }
        public ICommand CompareQRCodeACmd { get; set; }
        public ICommand CompareQRCodeBCmd { get; set; }
        public ICommand ScanTabCmd { get; set; }
        public ICommand ScanConfigCmd { get; set; }
        public ICommand SaveClickCmd { get; set; }

        public CompareContinuousViewModel(INavigation _Navigation, CompareContinuous ComparePage, bool reset)
        {
            try
            {
                loadindicator = true;
                Navigation = _Navigation;
                trackService = new YPSService();
                comparepage = ComparePage;
                BgColor = YPS.CommonClasses.Settings.Bar_Background;
                CompareQRCodeACmd = new Command(async () => await CompareQRCode("a"));
                CompareQRCodeBCmd = new Command(async () => await CompareQRCode("b"));
                ScanTabCmd = new Command(async () => await TabChange("scan"));
                ScanConfigCmd = new Command(async () => await TabChange("config"));
                SaveClickCmd = new Command(async () => await SaveConfig());

                Settings.scanQRValueA = "";
                Settings.scanQRValueB = "";

                Task.Run(() => GetSavedConfigDataFromDB()).Wait();
                scansetting = SettingsArchiver.UnarchiveSettings();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CompareContinuousViewModel constructor -> in CompareContinuousViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            loadindicator = false;
        }

        public async Task GetSavedConfigDataFromDB()
        {
            try
            {
                loadindicator = true;

                var result = await trackService.GetSaveScanConfig();

                if (result?.status == 1 && result?.data != null)
                {
                    SelectedRule.ID = result.data.ScanConfigID;
                    TotalCount = result.data.ScanCount;
                }

                var resultddl = await trackService.GetScanConfig();

                if (resultddl?.status == 1 && resultddl?.data.Count > 0 && SelectedRule != null)
                {
                    ScanRuleLst = resultddl.data;
                    SelectedScanRule = SelectedRule.ID == 0 ? ScanRuleLst[0]?.Name :
                        ScanRuleLst.Where(wr => wr.ID == SelectedRule.ID).FirstOrDefault().Name;

                    SelectedRule.Length = SelectedRule.ID == 0 ? ScanRuleLst[0].Length :
                       ScanRuleLst.Where(wr => wr.ID == SelectedRule.ID).FirstOrDefault().Length;


                    SelectedRule.Position = SelectedRule.ID == 0 ? ScanRuleLst[0]?.Position :
                       ScanRuleLst.Where(wr => wr.ID == SelectedRule.ID).FirstOrDefault().Position;

                    if (!string.IsNullOrEmpty(SelectedScanRule) && TotalCount > 0)
                    {
                        await SaveConfig();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetSavedConfigDataFromDB method -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            loadindicator = false;
        }

        public async Task GetUsersScanConfig()
        {
            try
            {
                var result = trackService.GetScanConfig().Result as ScanConfigResponse;

                if (result != null && SelectedRule != null)
                {
                    ScanRuleLst = result.data;
                    SelectedScanRule = SelectedRule.ID == 0 ? ScanRuleLst[0].Name :
                        ScanRuleLst.Where(wr => wr.ID == SelectedRule.ID).FirstOrDefault().Name;

                    if (!string.IsNullOrEmpty(SelectedScanRule) && TotalCount > 0)
                    {
                        await SaveConfig();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetUsersScanConfig method -> in CompareContinuousViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task RememberUserDetails()
        {
            try
            {
                RememberPwdDB Db = new RememberPwdDB();
                var user = Db.GetUserDetails();

                if (user != null && user.Count > 0)
                {
                    SelectedScanRule = user[0].SelectedScanRule != null ? user[0].SelectedScanRule : SelectedScanRule;
                    TotalCount = user[0].EnteredScanTotal != null ? user[0].EnteredScanTotal : TotalCount;

                    if (!string.IsNullOrEmpty(SelectedScanRule) && TotalCount > 0)
                    {
                        await SaveConfig();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RememberUserDetails method -> in CompareContinuousViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        private async Task CompareQRCode(string scanfor)
        {
            try
            {
                loadindicator = true;
                scanFor = scanfor;

                var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                var requestedPermissionStatus = requestedPermissions[Permission.Camera];
                var pass1 = requestedPermissions[Permission.Camera];

                if (pass1 == PermissionStatus.Denied)
                {
                    var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the camera to scan.", null, null, "Maybe Later", "Settings");
                    switch (checkSelect)
                    {
                        case "Maybe Later":
                            break;
                        case "Settings":
                            CrossPermissions.Current.OpenAppSettings();
                            break;
                    }
                }
                else if (pass1 == PermissionStatus.Granted)
                {
                    try
                    {
                        scanset.ContinuousAfterScan = scanFor.Trim().ToLower() == "b".Trim().ToLower() ? true : false;

                        SettingsArchiver.ArchiveSettings(scanset);

                        await Navigation.PushModalAsync(new ScannerPage(scanset, this));

                    }
                    catch (Exception ex1)
                    {
                        YPSLogger.ReportException(ex1, "CompareQRCode method while navigate-> in CompareContinuousViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                        var trackResult = trackService.Handleexception(ex1);
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CompareQRCode method -> in CompareContinuousViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task Scanditscan(string scanresult)
        {
            try
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                Stream sr = assembly.GetManifestResourceStream("YPS." + "beep.mp3");
                ISimpleAudioPlayer playbeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                playbeep.Load(sr);

                Stream oksr = assembly.GetManifestResourceStream("YPS." + "okbeep.mp3");
                ISimpleAudioPlayer okplaybeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                okplaybeep.Load(oksr);

                Stream ngsr = assembly.GetManifestResourceStream("YPS." + "ngbeep.mp3");
                ISimpleAudioPlayer ngplaybeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                ngplaybeep.Load(ngsr);

                Device.BeginInvokeOnMainThread(async () =>
                {

                    string sp = "\n\n";
                    var scanvalue = scanresult.Split(sp.ToCharArray());

                    if (scancountpermit > 0)
                    {
                        foreach (var result in scanvalue)
                        {

                            if (!string.IsNullOrEmpty(result))
                            {
                                CompareHistoryList comparemodel = new CompareHistoryList();

                                if (scanFor.Trim().ToLower() == "a".Trim().ToLower())
                                {
                                    Settings.scanQRValueA = ScannedValueA = scanvalue[0];

                                    if (Settings.scanQRValueA.Length < SelectedRule.Length)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Alert", "Length is less than the scan rule, please scan new Barcode/QR code.", "Ok");

                                        isScannedA = "ok.png";
                                        resultA = Settings.scanQRValueA;
                                        isEnableAFrame = true;
                                        opacityA = 1;
                                        isEnableBFrame = false;
                                        opacityB = 0.50;
                                        ngplaybeep.Play();
                                    }
                                    else
                                    {
                                        isScannedA = "cross.png";
                                        resultA = Settings.scanQRValueA;
                                        isEnableAFrame = false;
                                        opacityA = 0.50;
                                        isEnableBFrame = true;
                                        opacityB = 1;
                                        okplaybeep.Play();
                                    }
                                }
                                else
                                {
                                    int Astartindex;
                                    int Bstartindex;

                                    Settings.scanQRValueB = ScannedValueB = result;

                                    switch (SelectedRule.Position)
                                    {
                                        case "First":

                                            if ((Settings.scanQRValueA.Length >= SelectedRule.Length && Settings.scanQRValueB.Length >= SelectedRule.Length) && (Settings.scanQRValueA.Substring(0, SelectedRule.Length) == Settings.scanQRValueB.Substring(0, SelectedRule.Length)))
                                            {
                                                isMatch = "MATCHED";
                                                isMatchImage = "ook.png";
                                                isScannedB = "ok.png";
                                                resultB = Settings.scanQRValueB;
                                                scancountpermit--;
                                                okplaybeep.Play();
                                            }
                                            else
                                            {
                                                isMatch = "UNMATCHED";
                                                isMatchImage = "ng.png";
                                                isScannedB = "ok.png";
                                                resultB = Settings.scanQRValueB;
                                                ngplaybeep.Play();
                                            }
                                            break;

                                        case "Last":

                                            Astartindex = Settings.scanQRValueA.Length - SelectedRule.Length;
                                            Bstartindex = Settings.scanQRValueB.Length - SelectedRule.Length;

                                            if ((Settings.scanQRValueA.Length >= SelectedRule.Length && Settings.scanQRValueB.Length >= SelectedRule.Length) && (Settings.scanQRValueA.Substring(Astartindex, SelectedRule.Length) == Settings.scanQRValueB.Substring(Bstartindex, SelectedRule.Length)))
                                            {
                                                isMatch = "MATCHED";
                                                isMatchImage = "ook.png";
                                                isScannedB = "ok.png";
                                                resultB = Settings.scanQRValueB;
                                                scancountpermit--;
                                                okplaybeep.Play();

                                            }
                                            else
                                            {
                                                isMatch = "UNMATCHED";
                                                isMatchImage = "ng.png";
                                                isScannedB = "ok.png";
                                                resultB = Settings.scanQRValueB;
                                                ngplaybeep.Play();

                                            }
                                            break;

                                        case "Full":

                                            if (Settings.scanQRValueA == Settings.scanQRValueB)
                                            {
                                                isMatch = "MATCHED";
                                                isMatchImage = "ook.png";
                                                isScannedB = "ok.png";
                                                resultB = Settings.scanQRValueB;
                                                scancountpermit--;
                                                okplaybeep.Play();

                                            }
                                            else
                                            {
                                                isMatch = "UNMATCHED";
                                                isMatchImage = "ng.png";
                                                isScannedB = "ok.png";
                                                resultB = Settings.scanQRValueB;
                                                ngplaybeep.Play();

                                            }
                                            break;

                                        default:
                                            isMatch = "UNMATCHED";
                                            isMatchImage = "ng.png";
                                            isScannedB = "ok.png";
                                            resultB = Settings.scanQRValueB;
                                            ngplaybeep.Play();

                                            break;
                                    }

                                }

                                isShowMatch = isScannedA == "ok.png" && isScannedB == "ok.png" ? true : false;

                                if (!string.IsNullOrEmpty(Settings.scanQRValueA) && !string.IsNullOrEmpty(Settings.scanQRValueB))
                                {
                                    comparemodel.HistorySerialNo = historySerialNo++;
                                    comparemodel.AValue = Settings.scanQRValueA;
                                    comparemodel.BValue = Settings.scanQRValueB;
                                    comparemodel.IsMatchedImg = isMatchImage == "ook.png" ? "ookblack.png" : isMatchImage;
                                    compareList.Insert(0, comparemodel);

                                    compareHistoryList = new List<CompareHistoryList>(compareList);

                                    var val = compareHistoryList.OrderByDescending(w => w.HistorySerialNo).Take(5);

                                    latestCompareHistoryList = new List<CompareHistoryList>(val);

                                    showLatestViewFrame = true;
                                    showCurrentStatus = true;

                                    OKCount = "0";
                                    NGCount = "0";
                                    OKCount = compareHistoryList.Where(w => w.IsMatchedImg == "ookblack.png").Count().ToString() + "/" + TotalCount;
                                    NGCount = compareHistoryList.Where(w => w.IsMatchedImg == "ng.png").Count().ToString();

                                    if (scanFor.Trim().ToLower() == "b".Trim().ToLower() && scancountpermit == 0)
                                    {
                                        playbeep.Play();
                                        isEnableBFrame = false;
                                        opacityB = 0.50;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnDidScan method -> in CompareContinuousViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task TabChange(string tab)
        {
            try
            {
                loadindicator = true;

                if (tab == "scan")
                {
                    IsScanContentVisible = ScanTabVisibility = true;
                    IsConfigContentVisible = ConfigTabVisibility = false;
                    ScanTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
                    CompareTabTextColor = Color.Black;

                    showLatestViewFrame = latestCompareHistoryList != null && latestCompareHistoryList.Count > 0 ? true : false;
                }
                else
                {
                    showLatestViewFrame = IsScanContentVisible = ScanTabVisibility = false;
                    IsConfigContentVisible = ConfigTabVisibility = true;
                    ScanTabTextColor = Color.Black;
                    CompareTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TabChange method -> in CompareContinuousViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            loadindicator = false;
        }

        public async Task<bool> SaveConfig()
        {
            try
            {
                if (TotalCount > 0 && TotalCount <= 300)
                {
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        var data = await trackService.SaveScanConfig(SelectedRule, TotalCount);

                        if (data?.status == 1)
                        {
                            OKCount = "0";
                            NGCount = "0";
                            compareHistoryList = new List<CompareHistoryList>();
                            latestCompareHistoryList = new List<CompareHistoryList>();
                            scancountpermit = TotalCountHeader = TotalCount;
                            SelectedScanRuleHeader = SelectedScanRule;
                            OKCount = OKCount + "/" + TotalCount;
                            NGCount = NGCount;
                            IsScanEnable = IsScanContentVisible = ScanTabVisibility = true;
                            IsConfigContentVisible = ConfigTabVisibility = false;
                            ScanOpacity = 1;
                            IsTotalValidMsg = false;
                            ScanTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
                            CompareTabTextColor = Color.Black;

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        return false;
                    }
                }
                else
                {
                    TotalErrorTxt = "Total should be in between 1 and 300";
                    IsTotalValidMsg = true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveConfig method -> in CompareContinuousViewModel.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
                return false;
            }
        }

        #region Properties
        private Color _ScanTabTextColor = Color.Black;
        public Color ScanTabTextColor
        {
            get => _ScanTabTextColor;
            set
            {
                _ScanTabTextColor = value;
                RaisePropertyChanged("ScanTabTextColor");
            }
        }

        private Color _CompareTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color CompareTabTextColor
        {
            get => _CompareTabTextColor;
            set
            {
                _CompareTabTextColor = value;
                RaisePropertyChanged("CompareTabTextColor");
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

        private string _ScannedValueA;
        public string ScannedValueA
        {
            get => _ScannedValueA;
            set
            {
                _ScannedValueA = value;
                RaisePropertyChanged("ScannedValueA");
            }
        }

        private string _ScannedValueB;
        public string ScannedValueB
        {
            get => _ScannedValueB;
            set
            {
                _ScannedValueB = value;
                RaisePropertyChanged("ScannedValueB");
            }
        }

        private string _ismatch = "UNMATCHED";
        public string isMatch
        {
            get => _ismatch;
            set
            {
                _ismatch = value;
                NotifyPropertyChanged("isMatch");
            }
        }

        private string ismatchimage = "ng.png";
        public string isMatchImage
        {
            get => ismatchimage;
            set
            {
                ismatchimage = value;
                NotifyPropertyChanged("isMatchImage");
            }
        }

        private string _isScannedA = "cross.png";
        public string isScannedA
        {
            get => _isScannedA;
            set
            {
                _isScannedA = value;
                NotifyPropertyChanged("isScannedA");
            }
        }

        private string _isScannedB = "cross.png";
        public string isScannedB
        {
            get => _isScannedB;
            set
            {
                _isScannedB = value;
                NotifyPropertyChanged("isScannedB");
            }
        }

        private string _resultA = "";
        public string resultA
        {
            get => _resultA;
            set
            {
                _resultA = value;
                NotifyPropertyChanged("resultA");
            }
        }

        private string _resultB = "";
        public string resultB
        {
            get => _resultB;
            set
            {
                _resultB = value;
                NotifyPropertyChanged("resultB");
            }
        }

        private bool _isShowMatch;
        public bool isShowMatch
        {
            get => _isShowMatch;
            set
            {
                _isShowMatch = value;
                NotifyPropertyChanged("isShowMatch");
            }
        }

        private double _opacityB = 0.50;
        public double opacityB
        {
            get => _opacityB;
            set
            {
                _opacityB = value;
                NotifyPropertyChanged("opacityB");
            }
        }

        private double _opacityA = 1;
        public double opacityA
        {
            get => _opacityA;
            set
            {
                _opacityA = value;
                NotifyPropertyChanged("opacityA");
            }
        }

        private bool _isEnableAFrame = true;
        public bool isEnableAFrame
        {
            get => _isEnableAFrame;
            set
            {
                _isEnableAFrame = value;
                NotifyPropertyChanged("isEnableAFrame");
            }
        }

        private bool _isEnableBFrame;
        public bool isEnableBFrame
        {
            get => _isEnableBFrame;
            set
            {
                _isEnableBFrame = value;
                NotifyPropertyChanged("isEnableBFrame");
            }
        }


        private List<CompareHistoryList> _compareHistoryList;
        public List<CompareHistoryList> compareHistoryList
        {
            get => _compareHistoryList;
            set
            {
                _compareHistoryList = value;
                NotifyPropertyChanged("compareHistoryList");
            }
        }

        private List<CompareHistoryList> _compareList = new List<CompareHistoryList>();
        public List<CompareHistoryList> compareList
        {
            get => _compareList;
            set
            {
                _compareList = value;
                NotifyPropertyChanged("compareList");
            }
        }

        private List<CompareHistoryList> _latestCompareHistoryList;
        public List<CompareHistoryList> latestCompareHistoryList
        {
            get => _latestCompareHistoryList;
            set
            {
                _latestCompareHistoryList = value;
                NotifyPropertyChanged("latestCompareHistoryList");
            }
        }

        //private List<string> _ScanRuleLst = new List<string>(new string[] { "First 3", "First 5", "Last 4", "Last 9", "Last 10", "Last 11", "Complete Match" });
        //public List<string> ScanRuleLst
        //{
        //    get => _ScanRuleLst;
        //    set
        //    {
        //        _ScanRuleLst = value;
        //        NotifyPropertyChanged("ScanRuleLst");
        //    }
        //}

        private List<YPS.Model.CompareModel> _ScanRuleLst;
        public List<YPS.Model.CompareModel> ScanRuleLst
        {
            get => _ScanRuleLst;
            set
            {
                _ScanRuleLst = value;
                NotifyPropertyChanged("ScanRuleLst");
            }
        }

        //private string _SelectedScanRule = "First 3";
        //public string SelectedScanRule
        //{
        //    get => _SelectedScanRule;
        //    set
        //    {
        //        _SelectedScanRule = value;
        //        NotifyPropertyChanged("SelectedScanRule");
        //    }
        //}

        //private string _SelectedScanRuleHeader = "First 3";
        //public string SelectedScanRuleHeader
        //{
        //    get => _SelectedScanRuleHeader;
        //    set
        //    {
        //        _SelectedScanRuleHeader = value;
        //        NotifyPropertyChanged("SelectedScanRuleHeader");
        //    }
        //}


        private string _SelectedScanRule;
        public string SelectedScanRule
        {
            get => _SelectedScanRule;
            set
            {
                _SelectedScanRule = value;
                NotifyPropertyChanged("SelectedScanRule");
            }
        }

        private YPS.Model.CompareModel _SelectedRule = new Model.CompareModel();
        public YPS.Model.CompareModel SelectedRule
        {
            get => _SelectedRule;
            set
            {
                _SelectedRule = value;
                NotifyPropertyChanged("SelectedRule");
                SelectedScanRule = !string.IsNullOrEmpty(value.Name) ? value.Name : SelectedScanRule;
            }
        }

        private string _SelectedScanRuleHeader;
        public string SelectedScanRuleHeader
        {
            get => _SelectedScanRuleHeader;
            set
            {
                _SelectedScanRuleHeader = value;
                NotifyPropertyChanged("SelectedScanRuleHeader");
            }
        }

        private string _OKCount = "0";
        public string OKCount
        {
            get => _OKCount;
            set
            {
                _OKCount = value;
                NotifyPropertyChanged("OKCount");
            }
        }

        private string _NGCount = "0";
        public string NGCount
        {
            get => _NGCount;
            set
            {
                _NGCount = value;
                NotifyPropertyChanged("NGCount");
            }
        }

        //private List<CompareHistoryList> _latestCompareList=new List<CompareHistoryList>();
        //public List<CompareHistoryList> latestCompareList
        //{
        //    get => _latestCompareList;
        //    set
        //    {
        //        _latestCompareList = value;
        //        NotifyPropertyChanged("latestCompareList");
        //    }
        //}

        private bool _showScanHistory;
        public bool showScanHistory
        {
            get => _showScanHistory;
            set
            {
                _showScanHistory = value;
                NotifyPropertyChanged("showScanHistory");
            }
        }

        private bool _showLatestViewFrame;
        public bool showLatestViewFrame
        {
            get => _showLatestViewFrame;
            set
            {
                _showLatestViewFrame = value;
                NotifyPropertyChanged("showLatestViewFrame");
            }
        }

        private bool _showCurrentStatus;
        public bool showCurrentStatus
        {
            get => _showCurrentStatus;
            set
            {
                _showCurrentStatus = value;
                NotifyPropertyChanged("showCurrentStatus");
            }
        }

        private double _ScanOpacity = 0.3;
        public double ScanOpacity
        {
            get => _ScanOpacity;
            set
            {
                _ScanOpacity = value;
                NotifyPropertyChanged("ScanOpacity");
            }
        }

        private bool _IsScanEnable;
        public bool IsScanEnable
        {
            get => _IsScanEnable;
            set
            {
                _IsScanEnable = value;
                NotifyPropertyChanged("IsScanEnable");
            }
        }

        private bool _ScanTabVisibility;
        public bool ScanTabVisibility
        {
            get => _ScanTabVisibility;
            set
            {
                _ScanTabVisibility = value;
                NotifyPropertyChanged("ScanTabVisibility");
            }
        }

        private bool _ConfigTabVisibility = true;
        public bool ConfigTabVisibility
        {
            get => _ConfigTabVisibility;
            set
            {
                _ConfigTabVisibility = value;
                NotifyPropertyChanged("ConfigTabVisibility");
            }
        }

        private bool _IsScanContentVisible;
        public bool IsScanContentVisible
        {
            get => _IsScanContentVisible;
            set
            {
                _IsScanContentVisible = value;
                NotifyPropertyChanged("IsScanContentVisible");
            }
        }

        private bool _IsConfigContentVisible = true;
        public bool IsConfigContentVisible
        {
            get => _IsConfigContentVisible;
            set
            {
                _IsConfigContentVisible = value;
                NotifyPropertyChanged("IsConfigContentVisible");
            }
        }

        private int? _TotalCount = 0;
        public int? TotalCount
        {
            get => _TotalCount;
            set
            {
                _TotalCount = value;
                NotifyPropertyChanged("TotalCount");
            }
        }

        private int? _TotalCountHeader = 0;
        public int? TotalCountHeader
        {
            get => _TotalCountHeader;
            set
            {
                _TotalCountHeader = value;
                NotifyPropertyChanged("TotalCountHeader");
            }
        }

        private string _TotalErrorTxt = "";
        public string TotalErrorTxt
        {
            get => _TotalErrorTxt;
            set
            {
                _TotalErrorTxt = value;
                NotifyPropertyChanged("TotalErrorTxt");
            }
        }

        private bool _IsTotalValidMsg;
        public bool IsTotalValidMsg
        {
            get => _IsTotalValidMsg;
            set
            {
                _IsTotalValidMsg = value;
                NotifyPropertyChanged("IsTotalValidMsg");
            }
        }

        private bool _loadindicator;
        public bool loadindicator
        {
            get => _loadindicator;
            set
            {
                _loadindicator = value;
                NotifyPropertyChanged("loadindicator");
            }
        }
        #endregion Properties
    }
}
