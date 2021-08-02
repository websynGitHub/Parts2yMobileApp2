using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.SimpleAudioPlayer;
using Scandit.BarcodePicker.Unified;
using Scandit.BarcodePicker.Unified.Abstractions;
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
//using YPS.Parts2y.Parts2y_Common_Classes;
//using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using ZXing.Net.Mobile.Forms;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class CompareViewModel : IBase
    {
        YPSService trackService;
        IBarcodePicker _picker;
        string scanFor;
        public int historySerialNo = 1;
        List<CompareHistoryList> lstcomparehistory = new List<CompareHistoryList>();
        private Compare comparepage;
        private int? scancountpermit;
        private ScanerSettings scansetting;

        //WS
        //public static string appKey = "AeUg02GPQW+LQNsakx9oKmAH8IrQG9AmH1BKUM5tVhLuWMM/FFj+/pMlwybUfcHkzkChSuwZ8jDcLGs2ISqD3bxZIoL2XeE2jw1A7Ut8ZzzdTTcBKmA0pfxprY/ZOunN1C2kPI44zwOoEAiVqgiwSkC4HggoWmuGwSUegKWJcmguvtisIqXeJiv1h9wlALpydmJPLc8Wq2j+u16ugMaQJforuRWjPOknSyk5oRExHQDT2MBc33lR7Hmzql2p15EEb1fLFVbhEStQhwZwxQgGbk7sl8kxmleqmFUeKvJOGf/GG8nJ8blFlTRH7akAQUSu77YCccEBPO19eo4WQTATViFlih0GwvmFIJrWL6/L3vWerQ/8OD/YC1G+ngq2pGVAukbWaSdRhhc5bNRq0w0CwACTmSuqxCPlKJexkpoVECd+Z2TcKsS3rZS4MtUkfCVniWdbQJwsJJXWeppxB8mOk8aUfEYqFYg8QlW7kTIM5XUQyF6+FweSc616B9UYSzBp1WzXuMa5q4nCClUKKUlJfyzqKQzu5Ckg/3EzB+ch6qy6QrrbuU6xjtLJNzz6AXG9ix+tAiQPg4bBdt5YHo8TN6oMuNhncGkjQpMukzJiwdFJLeL8pNu/xiY4rU0wdmuZok7QUZZtMcsX/qi59UFmibOB/sGvf1MReyWdcx+EQmDd2/1JdplFqKEKMjigKAAb1I+OGI9g3Al0WlmmifJxjyvdHW8aFeM/EnSerghQ5CBK3EwiuruueBcdPkNs7ajXkLsRIF+5rqMHOmQQW9w5YCqwEl2s9jPUsWZ0z+s1Utle7OtDhP8bcYuqfEA=";
        //UAT
        //public static string appKey = "AfHgJGWPRZcJD5nCrSuWn3I14pFUOfSbJ14y67JNCkXAeC0d8XF7BqotirkNTX/pQVC82/RLUARkds5Va0Y725AhXzxHU4CB2T5FFGE10G1IfA/gJW69uvUoNe8+M85xs0ATJkIfMj3Out0akHyB4sioSLxmsspl514MA4neokCFZp+XOU+70frw4/J3+hGrx78vGjhf/xns3Js8vLhGO9HKh/k8OQnode7yK+163bdSB2eB2kFQuv1PO/unuBm5Hju1+ZrwUYgOcz6OnYJiec22VAnd4K3y7rHTylU9sZ1ypZyf39iJz78LYDACDTgx4N1FXb/rwGMtYaQnMyi9SaOkJXQGTiNFhjUQ7j72pR3xlaWPDzXGjujcKmD2nZcAjJLSYehdhu7eUqHtbB5E2PrHPT0mVZ5yvWkbaEAr1+QxDyZtRsd/5WWA7ipyB3QPlFPGrA3jC01FNv+Ka3+L44fCwSSZKiT/SDHsgWqSGorvjN3b+ksXzq0tTBaYTCyairR7FouUQ3MbZVhGTZ83AKzRg4uvulczHJMJCjVMfezWpGBYad3rvznTgMuLKGAyan0Cb5TB8LRZrYHGjNXANNJiRvB7dFP62xy9u0CxY2U8hLrMbWA5kR12yxHej/oIDi9s2VywbHsegQ+yAQNcWvchc/A5IMuvtA3ztpT2NVRVi2PLViyOcOIwDFqjRznp3mOnN0NJH64xhXgXwcgOZJTlKDyYrvXDN0FwAuMXTOyH2eIKw689hVCKfTsyfAD7HzjrXyOXYmjaGelfiDeqFVI6LTUlvgyW0ebhbY8qygCiyNbbwFT8";
        public INavigation Navigation { get; set; }
        public ICommand CompareQRCodeACmd { get; set; }
        public ICommand CompareQRCodeBCmd { get; set; }
        public ICommand ScanTabCmd { get; set; }
        public ICommand ScanConfigCmd { get; set; }
        public ICommand SaveClickCmd { get; set; }

        public CompareViewModel(INavigation _Navigation, Compare ComparePage, bool reset)
        {
            try
            {
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
                ScanditService.ScanditLicense.AppKey = HostingURL.scandItLicencekey;
                scansetting = SettingsArchiver.UnarchiveSettings();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CompareViewModel constructor -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task GetSavedConfigDataFromDB()
        {
            try
            {
                var data = await trackService.GetSaveScanConfig();
                var result = data as YPS.Model.GetSavedConfigResponse;

                if (result != null)
                {
                    SelectedRule.ID = result.data.ScanConfigID;
                    TotalCount = result.data.ScanCount;
                }

                var daatddl = await trackService.GetScanConfig();
                var resultddl = daatddl as YPS.Model.ScanConfigResponse;

                if (resultddl != null && SelectedRule != null)
                {
                    ScanRuleLst = resultddl.data;
                    SelectedScanRule = SelectedRule.ID == 0 ? ScanRuleLst[0].Name :
                        ScanRuleLst.Where(wr => wr.ID == SelectedRule.ID).FirstOrDefault().Name;

                    SelectedRule.Length = SelectedRule.ID == 0 ? ScanRuleLst[0].Length :
                       ScanRuleLst.Where(wr => wr.ID == SelectedRule.ID).FirstOrDefault().Length;


                    SelectedRule.Position = SelectedRule.ID == 0 ? ScanRuleLst[0].Position :
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
                YPSLogger.ReportException(ex, "GetUsersScanConfig method -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "RememberUserDetails method -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
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

                        //await PrepareScannerSettings();
                        //await _picker.StartScanningAsync(false);

                        ScanerSettings scanset = new ScanerSettings();
                        SettingsArchiver.ArchiveSettings(scanset);

                        Navigation.PushModalAsync(new ScannerPage(scanset, this));

                    }
                    catch (Exception ex1)
                    {
                        YPSLogger.ReportException(ex1, "CompareQRCode method while navigate-> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
                        var trackResult = trackService.Handleexception(ex1);
                    }
                    #region Zxing related commented code
                    //var overlay = new ZXingDefaultOverlay
                    //{
                    //    ShowFlashButton = true,
                    //    TopText = string.Empty,
                    //    BottomText = string.Empty,
                    //};

                    //overlay.BindingContext = overlay;

                    //var ScannerPage = new ZXingScannerPage(null, overlay);
                    //ScannerPage = new ZXingScannerPage(null, overlay);

                    //ScannerPage.OnScanResult += (scanresult) =>
                    //{
                    //    // Parar de escanear
                    //    //ScannerPage.IsScanning = false;

                    //    // Alert com o código escaneado
                    //    Device.BeginInvokeOnMainThread(async () =>
                    //    {
                    //        //await Navigation.PopAsync();
                    //        CompareHistoryList comparemodel = new CompareHistoryList();

                    //        if (scanfor == "a")
                    //        {
                    //            Settings.scanQRValueA = ScannedValueA = scanresult != null ? scanresult.NewlyRecognizedCodes.FirstOrDefault().Data : "scanned";

                    //            if (Settings.scanQRValueA.Length < SelectedRule.Length)
                    //            {
                    //                await App.Current.MainPage.DisplayActionSheet("minimum value length is less than the scan rule, please scan new Bar code/QR code.", null, null, "", "Ok");

                    //                isScannedA = "ok.png";
                    //                resultA = Settings.scanQRValueA;
                    //                isEnableAFrame = true;
                    //                opacityA = 1;
                    //                isEnableBFrame = false;
                    //                opacityB = 0.50;
                    //            }
                    //            else
                    //            {
                    //                isScannedA = "cross.png";
                    //                resultA = Settings.scanQRValueA;
                    //                isEnableAFrame = false;
                    //                opacityA = 0.50;
                    //                isEnableBFrame = true;
                    //                opacityB = 1;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            int Astartindex;
                    //            int Bstartindex;

                    //            Settings.scanQRValueB = ScannedValueB = scanresult != null ? scanresult.NewlyRecognizedCodes.FirstOrDefault().Data : "scanned";

                    //            switch (SelectedRule.Position)
                    //            {
                    //                case "First":

                    //                    if ((Settings.scanQRValueA.Length >= SelectedRule.Length && Settings.scanQRValueB.Length >= SelectedRule.Length) && (Settings.scanQRValueA.Substring(0, SelectedRule.Length) == Settings.scanQRValueB.Substring(0, SelectedRule.Length)))
                    //                    {
                    //                        isMatch = "MATCHED";
                    //                        isMatchImage = "ook.png";
                    //                        isScannedB = "ok.png";
                    //                        resultB = Settings.scanQRValueB;
                    //                        scancountpermit--;
                    //                        okplaybeep.Play();
                    //                    }
                    //                    else
                    //                    {
                    //                        isMatch = "UNMATCHED";
                    //                        isMatchImage = "ng.png";
                    //                        isScannedB = "ok.png";
                    //                        resultB = Settings.scanQRValueB;
                    //                        ngplaybeep.Play();
                    //                    }
                    //                    break;

                    //                case "Last":

                    //                    Astartindex = Settings.scanQRValueA.Length - SelectedRule.Length;
                    //                    Bstartindex = Settings.scanQRValueB.Length - SelectedRule.Length;

                    //                    if ((Settings.scanQRValueA.Length >= SelectedRule.Length && Settings.scanQRValueB.Length >= SelectedRule.Length) && (Settings.scanQRValueA.Substring(Astartindex, SelectedRule.Length) == Settings.scanQRValueB.Substring(Bstartindex, SelectedRule.Length)))
                    //                    {
                    //                        isMatch = "MATCHED";
                    //                        isMatchImage = "ook.png";
                    //                        isScannedB = "ok.png";
                    //                        resultB = Settings.scanQRValueB;
                    //                        scancountpermit--;
                    //                        okplaybeep.Play();

                    //                    }
                    //                    else
                    //                    {
                    //                        isMatch = "UNMATCHED";
                    //                        isMatchImage = "ng.png";
                    //                        isScannedB = "ok.png";
                    //                        resultB = Settings.scanQRValueB;
                    //                        ngplaybeep.Play();

                    //                    }
                    //                    break;

                    //                case "Full":

                    //                    if (Settings.scanQRValueA == Settings.scanQRValueB)
                    //                    {
                    //                        isMatch = "MATCHED";
                    //                        isMatchImage = "ook.png";
                    //                        isScannedB = "ok.png";
                    //                        resultB = Settings.scanQRValueB;
                    //                        scancountpermit--;
                    //                        okplaybeep.Play();

                    //                    }
                    //                    else
                    //                    {
                    //                        isMatch = "UNMATCHED";
                    //                        isMatchImage = "ng.png";
                    //                        isScannedB = "ok.png";
                    //                        resultB = Settings.scanQRValueB;
                    //                        ngplaybeep.Play();

                    //                    }
                    //                    break;

                    //                default:
                    //                    isMatch = "UNMATCHED";
                    //                    isMatchImage = "ng.png";
                    //                    isScannedB = "ok.png";
                    //                    resultB = Settings.scanQRValueB;
                    //                    ngplaybeep.Play();

                    //                    break;
                    //            }
                    //        }

                    //        isShowMatch = isScannedA == "ok.png" && isScannedB == "ok.png" ? true : false;

                    //        if (!string.IsNullOrEmpty(Settings.scanQRValueA) && !string.IsNullOrEmpty(Settings.scanQRValueB))
                    //        {
                    //            comparemodel.HistorySerialNo = historySerialNo++;
                    //            comparemodel.AValue = Settings.scanQRValueA;
                    //            comparemodel.BValue = Settings.scanQRValueB;
                    //            comparemodel.IsMatchedImg = isMatchImage == "ook.png" ? "ookblack.png" : isMatchImage;
                    //            compareList.Insert(0, comparemodel);

                    //            compareHistoryList = new List<CompareHistoryList>(compareList);

                    //            var val = compareList.OrderByDescending(w => w.HistorySerialNo).Take(5);

                    //            latestCompareHistoryList = new List<CompareHistoryList>(val);

                    //            showLatestViewFrame = true;
                    //            showCurrentStatus = true;


                    //            OKCount = "0";
                    //            NGCount = "0";
                    //            OKCount = compareHistoryList.Where(w => w.IsMatchedImg == "ookblack.png").Count().ToString() + "/" + TotalCount;
                    //            NGCount = compareHistoryList.Where(w => w.IsMatchedImg == "ng.png").Count().ToString();

                    //            if (scancountpermit == 0)
                    //            {
                    //                playbeep.Play();
                    //                isEnableBFrame = false;
                    //                opacityB = 0.50;
                    //            }
                    //        }

                    //    });

                    //};

                    //if (Navigation.ModalStack.Count == 0 ||
                    //                   Navigation.ModalStack.Last().GetType() != typeof(ScanditScannerPage))
                    //{
                    //    //ScannerPage.AutoFocus();

                    //    //overlay.FlashButtonClicked += (s, ed) =>
                    //    //{
                    //    //    ScannerPage.ToggleTorch();
                    //    //};
                    //}
                    #endregion Zxing related commented code
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CompareQRCode method -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public void Scanditscan(string scanresult)
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
                    foreach (var result in scanvalue)
                    {

                        if (!string.IsNullOrEmpty(result))
                        {
                            CompareHistoryList comparemodel = new CompareHistoryList();
                            if (scanFor == "a")
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
                                }
                                else
                                {
                                    isScannedA = "cross.png";
                                    resultA = Settings.scanQRValueA;
                                    isEnableAFrame = false;
                                    opacityA = 0.50;
                                    isEnableBFrame = true;
                                    opacityB = 1;
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

                                if (scancountpermit == 0)
                                {
                                    playbeep.Play();
                                    isEnableBFrame = false;
                                    opacityB = 0.50;
                                    break;
                                }
                            }
                        }
                    }
                    //_picker.DidScan -= ValueScanned;
                });

                // scanresult.StopScanning();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnDidScan method -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task PrepareScannerSettings()
        {
            try
            {
                var settings = ScanditService.BarcodePicker.GetDefaultScanSettings();
                settings.EnableSymbology(Symbology.Ean13, true);
                settings.EnableSymbology(Symbology.Upca, true);
                settings.EnableSymbology(Symbology.Qr, true);
                settings.EnableSymbology(Symbology.Code11, true);
                settings.EnableSymbology(Symbology.Code128, true);
                settings.EnableSymbology(Symbology.Code25, true);
                settings.EnableSymbology(Symbology.Code32, true);
                settings.EnableSymbology(Symbology.Code39, true);
                settings.EnableSymbology(Symbology.Code93, true);
                settings.EnableSymbology(Symbology.DotCode, true);
                settings.EnableSymbology(Symbology.MaxiCode, true);
                settings.EnableSymbology(Symbology.Upce, true);
                settings.EnableSymbology(Symbology.Codabar, true);
                settings.EnableSymbology(Symbology.Aztec, true);
                settings.EnableSymbology(Symbology.DataMatrix, true);
                settings.EnableSymbology(Symbology.Ean8, true);
                settings.EnableSymbology(Symbology.FiveDigitAddOn, true);
                settings.EnableSymbology(Symbology.Gs1Databar, true);
                settings.EnableSymbology(Symbology.Gs1DatabarExpanded, true);
                settings.EnableSymbology(Symbology.Gs1DatabarLimited, true);
                settings.EnableSymbology(Symbology.Interleaved2Of5, true);
                settings.EnableSymbology(Symbology.Kix, true);
                settings.EnableSymbology(Symbology.LAPA4SC, true);
                settings.EnableSymbology(Symbology.MicroPdf417, true);
                settings.EnableSymbology(Symbology.MicroQr, true);
                settings.EnableSymbology(Symbology.MsiPlessey, true);
                settings.EnableSymbology(Symbology.Pdf417, true);
                settings.EnableSymbology(Symbology.Rm4scc, true);
                settings.EnableSymbology(Symbology.TwoDigitAddOn, true);

                _picker = ScanditService.BarcodePicker;
                await _picker.ApplySettingsAsync(settings);

                _picker.DidScan += ValueScanned;

                _picker.AutoFocusOnTapEnabled = true;
                _picker.ScanOverlay.BeepEnabled = false;
                _picker.ScanOverlay.TorchButtonVisible = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CompareQRCode method -> in PrepareScannerSettings " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public void ValueScanned(ScanSession scanresult)
        {
            //try
            //{
            //    var assembly = typeof(App).GetTypeInfo().Assembly;
            //    Stream sr = assembly.GetManifestResourceStream("YPS." + "beep.mp3");
            //    ISimpleAudioPlayer playbeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            //    playbeep.Load(sr);

            //    Stream oksr = assembly.GetManifestResourceStream("YPS." + "okbeep.mp3");
            //    ISimpleAudioPlayer okplaybeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            //    okplaybeep.Load(oksr);

            //    Stream ngsr = assembly.GetManifestResourceStream("YPS." + "ngbeep.mp3");
            //    ISimpleAudioPlayer ngplaybeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            //    ngplaybeep.Load(ngsr);

            //    Device.BeginInvokeOnMainThread(async () =>
            //    {
            //        CompareHistoryList comparemodel = new CompareHistoryList();

            //        if (scanFor == "a")
            //        {
            //            Settings.scanQRValueA = ScannedValueA = scanresult != null ? scanresult.NewlyRecognizedCodes.FirstOrDefault().Data : "scanned";

            //            if (Settings.scanQRValueA.Length < SelectedRule.Length)
            //            {
            //                await App.Current.MainPage.DisplayAlert("Alert", "Length is less than the scan rule, please scan new Barcode/QR code.", "Ok");

            //                isScannedA = "ok.png";
            //                resultA = Settings.scanQRValueA;
            //                isEnableAFrame = true;
            //                opacityA = 1;
            //                isEnableBFrame = false;
            //                opacityB = 0.50;
            //            }
            //            else
            //            {
            //                isScannedA = "cross.png";
            //                resultA = Settings.scanQRValueA;
            //                isEnableAFrame = false;
            //                opacityA = 0.50;
            //                isEnableBFrame = true;
            //                opacityB = 1;
            //            }
            //        }
            //        else
            //        {
            //            int Astartindex;
            //            int Bstartindex;

            //            Settings.scanQRValueB = ScannedValueB = scanresult != null ? scanresult.NewlyRecognizedCodes.FirstOrDefault().Data : "scanned";

            //            switch (SelectedRule.Position)
            //            {
            //                case "First":

            //                    if ((Settings.scanQRValueA.Length >= SelectedRule.Length && Settings.scanQRValueB.Length >= SelectedRule.Length) && (Settings.scanQRValueA.Substring(0, SelectedRule.Length) == Settings.scanQRValueB.Substring(0, SelectedRule.Length)))
            //                    {
            //                        isMatch = "MATCHED";
            //                        isMatchImage = "ook.png";
            //                        isScannedB = "ok.png";
            //                        resultB = Settings.scanQRValueB;
            //                        scancountpermit--;
            //                        okplaybeep.Play();
            //                    }
            //                    else
            //                    {
            //                        isMatch = "UNMATCHED";
            //                        isMatchImage = "ng.png";
            //                        isScannedB = "ok.png";
            //                        resultB = Settings.scanQRValueB;
            //                        ngplaybeep.Play();
            //                    }
            //                    break;

            //                case "Last":

            //                    Astartindex = Settings.scanQRValueA.Length - SelectedRule.Length;
            //                    Bstartindex = Settings.scanQRValueB.Length - SelectedRule.Length;

            //                    if ((Settings.scanQRValueA.Length >= SelectedRule.Length && Settings.scanQRValueB.Length >= SelectedRule.Length) && (Settings.scanQRValueA.Substring(Astartindex, SelectedRule.Length) == Settings.scanQRValueB.Substring(Bstartindex, SelectedRule.Length)))
            //                    {
            //                        isMatch = "MATCHED";
            //                        isMatchImage = "ook.png";
            //                        isScannedB = "ok.png";
            //                        resultB = Settings.scanQRValueB;
            //                        scancountpermit--;
            //                        okplaybeep.Play();

            //                    }
            //                    else
            //                    {
            //                        isMatch = "UNMATCHED";
            //                        isMatchImage = "ng.png";
            //                        isScannedB = "ok.png";
            //                        resultB = Settings.scanQRValueB;
            //                        ngplaybeep.Play();

            //                    }
            //                    break;

            //                case "Full":

            //                    if (Settings.scanQRValueA == Settings.scanQRValueB)
            //                    {
            //                        isMatch = "MATCHED";
            //                        isMatchImage = "ook.png";
            //                        isScannedB = "ok.png";
            //                        resultB = Settings.scanQRValueB;
            //                        scancountpermit--;
            //                        okplaybeep.Play();

            //                    }
            //                    else
            //                    {
            //                        isMatch = "UNMATCHED";
            //                        isMatchImage = "ng.png";
            //                        isScannedB = "ok.png";
            //                        resultB = Settings.scanQRValueB;
            //                        ngplaybeep.Play();

            //                    }
            //                    break;

            //                default:
            //                    isMatch = "UNMATCHED";
            //                    isMatchImage = "ng.png";
            //                    isScannedB = "ok.png";
            //                    resultB = Settings.scanQRValueB;
            //                    ngplaybeep.Play();

            //                    break;
            //            }
            //        }

            //        isShowMatch = isScannedA == "ok.png" && isScannedB == "ok.png" ? true : false;

            //        if (!string.IsNullOrEmpty(Settings.scanQRValueA) && !string.IsNullOrEmpty(Settings.scanQRValueB))
            //        {
            //            comparemodel.HistorySerialNo = historySerialNo++;
            //            comparemodel.AValue = Settings.scanQRValueA;
            //            comparemodel.BValue = Settings.scanQRValueB;
            //            comparemodel.IsMatchedImg = isMatchImage == "ook.png" ? "ookblack.png" : isMatchImage;
            //            compareList.Insert(0, comparemodel);

            //            compareHistoryList = new List<CompareHistoryList>(compareList);

            //            var val = compareList.OrderByDescending(w => w.HistorySerialNo).Take(5);

            //            latestCompareHistoryList = new List<CompareHistoryList>(val);

            //            showLatestViewFrame = true;
            //            showCurrentStatus = true;


            //            OKCount = "0";
            //            NGCount = "0";
            //            OKCount = compareHistoryList.Where(w => w.IsMatchedImg == "ookblack.png").Count().ToString() + "/" + TotalCount;
            //            NGCount = compareHistoryList.Where(w => w.IsMatchedImg == "ng.png").Count().ToString();

            //            if (scancountpermit == 0)
            //            {
            //                playbeep.Play();
            //                isEnableBFrame = false;
            //                opacityB = 0.50;
            //            }
            //        }

            //        _picker.DidScan -= ValueScanned;
            //    });

            //    scanresult.StopScanning();
            //}
            //catch (Exception ex)
            //{
            //    YPSLogger.ReportException(ex, "OnDidScan method -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
            //    var trackResult = trackService.Handleexception(ex);
            //}
        }

        public async Task TabChange(string tab)
        {
            try
            {
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
                YPSLogger.ReportException(ex, "TabChange method -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async Task<bool> SaveConfig()
        {
            try
            {
                if (TotalCount > 0 && TotalCount <= 300)
                {
                    var data = trackService.SaveScanConfig(SelectedRule, TotalCount).Result as YPS.Model.SaveScanConfigResponse;

                    if (data.status == 1)
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
                    TotalErrorTxt = "Total should be in between 1 and 300";
                    IsTotalValidMsg = true;

                    return false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveConfig method -> in CompareViewModel " + YPS.CommonClasses.Settings.userLoginID);
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
