using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;
using ZXing.Net.Mobile.Forms;

namespace YPS.Parts2y.Parts2y_View_Models
{
    class CompareViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand CompareQRCodeACmd { get; set; }
        public ICommand CompareQRCodeBCmd { get; set; }
        public ICommand ScanTabCmd { get; set; }
        public ICommand ScanConfigCmd { get; set; }
        public ICommand SaveClickCmd { get; set; }


        public int historySerialNo = 1;
        List<CompareHistoryList> lstcomparehistory = new List<CompareHistoryList>();
        private Compare comparepage;
        private int? scancountpermit;
        public CompareViewModel(INavigation _Navigation, Compare ComparePage)
        {
            Navigation = _Navigation;
            comparepage = ComparePage;
            BgColor = Settings.Bar_Background;
            CompareQRCodeACmd = new Command(async () => await CompareQRCode("a"));
            CompareQRCodeBCmd = new Command(async () => await CompareQRCode("b"));
            ScanTabCmd = new Command(async () => await TabChange("scan"));
            ScanConfigCmd = new Command(async () => await TabChange("config"));
            SaveClickCmd = new Command(async () => await SaveConfig());



            //isMatch = "UNMATCHED";
            //isMatchImage = "unmatch.png";
            //isScannedA = "cross.png";
            //isScannedB = "cross.png";
            Settings.scanQRValueA = "";
            Settings.scanQRValueB = "";
        }

        private async Task CompareQRCode(string scanfor)
        {
            try
            {
                loadindicator = true;

                var assembly = typeof(App).GetTypeInfo().Assembly;
                Stream sr = assembly.GetManifestResourceStream("YPS." + "beep.mp3");
                ISimpleAudioPlayer playbeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                playbeep.Load(sr);

                //if (scancountpermit!=0)
                //{
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
                    var ScannerPage = new ZXingScannerPage();


                    ScannerPage.OnScanResult += (scanresult) =>
                    {
                        // Parar de escanear
                        ScannerPage.IsScanning = false;

                        // Alert com o código escaneado
                        Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopAsync();
                        CompareHistoryList comparemodel = new CompareHistoryList();

                        if (scanfor == "a")
                        {
                            Settings.scanQRValueA = ScannedValueA = scanresult != null ? scanresult.Text : "scanned";

                            switch (SelectedScanRule)
                            {
                                case "First 3":

                                    if (Settings.scanQRValueA.Length < 3)
                                    {
                                        await App.Current.MainPage.DisplayActionSheet("minimum value length is less than the scan rule, please scan new Bar code/QR code.", null, null, "", "Ok");

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
                                    break;

                                case "First 5":

                                    if (Settings.scanQRValueA.Length < 5)
                                    {
                                        await App.Current.MainPage.DisplayActionSheet("minimum value length is less than the scan rule, please scan new Bar code/QR code.", null, null, "", "Ok");

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
                                    break;

                                case "Last 4":

                                    if (Settings.scanQRValueA.Length < 4)
                                    {
                                        await App.Current.MainPage.DisplayActionSheet("minimum value length is less than the scan rule, please scan new Bar code/QR code.", null, null, "", "Ok");

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
                                    break;

                                case "Last 9":

                                    if (Settings.scanQRValueA.Length < 9)
                                    {
                                        await App.Current.MainPage.DisplayActionSheet("minimum value length is less than the scan rule, please scan new Bar code/QR code.", null, null, "", "Ok");

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
                                    break;

                                case "Last 10":

                                    if (Settings.scanQRValueA.Length < 10)
                                    {
                                        await App.Current.MainPage.DisplayActionSheet("minimum value length is less than the scan rule, please scan new Bar code/QR code.", null, null, "", "Ok");

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
                                    break;

                                case "Last 11":

                                    if (Settings.scanQRValueA.Length < 11)
                                    {
                                        await App.Current.MainPage.DisplayActionSheet("minimum value length is less than the scan rule, please scan new Bar code/QR code.", null, null, "", "Ok");

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
                                    break;

                                default:
                                    if (SelectedScanRule == "Complete Match")
                                    {
                                        isScannedA = "ok.png";
                                        resultA = Settings.scanQRValueA;
                                        isEnableAFrame = false;
                                        opacityA = 0.50;
                                        isEnableBFrame = true;
                                        opacityB = 1;
                                    }
                                    else
                                    {
                                        // isMatch = "UNMATCHED";
                                        //isMatchImage = "unmatch.png";

                                        isScannedA = "cross.png";
                                        resultA = Settings.scanQRValueA;
                                        isEnableAFrame = true;
                                        opacityA = 1;
                                        isEnableBFrame = false;
                                        opacityB = 0.50;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            int Astartindex;
                            int Bstartindex;

                            Settings.scanQRValueB = ScannedValueB = scanresult != null ? scanresult.Text : "scanned";

                            switch (SelectedScanRule)
                            {
                                case "First 3":

                                    if ((Settings.scanQRValueA.Length >= 3 && Settings.scanQRValueB.Length >= 3) && (Settings.scanQRValueA.Substring(0, 3) == Settings.scanQRValueB.Substring(0, 3)))
                                    {
                                        isMatch = "MATCHED";
                                        isMatchImage = "ookblack.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        scancountpermit--;
                                    }
                                    else
                                    {
                                        isMatch = "UNMATCHED";
                                        isMatchImage = "ng.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        playbeep.Play();
                                    }
                                    break;

                                case "First 5":

                                    if ((Settings.scanQRValueA.Length >= 5 && Settings.scanQRValueB.Length >= 5) && (Settings.scanQRValueA.Substring(0, 5) == Settings.scanQRValueB.Substring(0, 5)))
                                    {
                                        isMatch = "MATCHED";
                                        isMatchImage = "ookblack.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        scancountpermit--;
                                    }
                                    else
                                    {
                                        isMatch = "UNMATCHED";
                                        isMatchImage = "ng.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        playbeep.Play();
                                    }
                                    break;

                                case "Last 4":
                                    Astartindex = Settings.scanQRValueA.Length - 4;
                                    Bstartindex = Settings.scanQRValueB.Length - 4;

                                    if ((Settings.scanQRValueA.Length >= 4 && Settings.scanQRValueB.Length >= 4) && (Settings.scanQRValueA.Substring(Astartindex, 4) == Settings.scanQRValueB.Substring(Bstartindex, 4)))
                                    {
                                        isMatch = "MATCHED";
                                        isMatchImage = "ookblack.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        scancountpermit--;
                                    }
                                    else
                                    {
                                        isMatch = "UNMATCHED";
                                        isMatchImage = "ng.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        playbeep.Play();

                                    }
                                    break;

                                case "Last 9":
                                    Astartindex = Settings.scanQRValueA.Length - 9;
                                    Bstartindex = Settings.scanQRValueB.Length - 9;

                                    if ((Settings.scanQRValueA.Length >= 9 && Settings.scanQRValueB.Length >= 9) && (Settings.scanQRValueA.Substring(Astartindex, 9) == Settings.scanQRValueB.Substring(Bstartindex, 9)))
                                    {
                                        isMatch = "MATCHED";
                                        isMatchImage = "ookblack.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        scancountpermit--;
                                    }
                                    else
                                    {
                                        isMatch = "UNMATCHED";
                                        isMatchImage = "ng.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        playbeep.Play();

                                    }
                                    break;

                                case "Last 10":
                                    Astartindex = Settings.scanQRValueA.Length - 10;
                                    Bstartindex = Settings.scanQRValueB.Length - 10;

                                    if ((Settings.scanQRValueA.Length >= 10 && Settings.scanQRValueB.Length >= 10) && (Settings.scanQRValueA.Substring(Astartindex, 10) == Settings.scanQRValueB.Substring(Bstartindex, 10)))
                                    {
                                        isMatch = "MATCHED";
                                        isMatchImage = "ookblack.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        scancountpermit--;
                                    }
                                    else
                                    {
                                        isMatch = "UNMATCHED";
                                        isMatchImage = "ng.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        playbeep.Play();

                                    }
                                    break;

                                case "Last 11":
                                    Astartindex = Settings.scanQRValueA.Length - 11;
                                    Bstartindex = Settings.scanQRValueB.Length - 11;

                                    if ((Settings.scanQRValueA.Length >= 11 && Settings.scanQRValueB.Length >= 11) && (Settings.scanQRValueA.Substring(Astartindex, 11) == Settings.scanQRValueB.Substring(Bstartindex, 11)))
                                    {
                                        isMatch = "MATCHED";
                                        isMatchImage = "ookblack.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        scancountpermit--;
                                    }
                                    else
                                    {
                                        isMatch = "UNMATCHED";
                                        isMatchImage = "ng.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        playbeep.Play();
                                    }
                                    break;

                                case "Complete Match":

                                    if (Settings.scanQRValueA == Settings.scanQRValueB)
                                    {
                                        isMatch = "MATCHED";
                                        isMatchImage = "ookblack.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        scancountpermit--;
                                    }
                                    else
                                    {
                                        isMatch = "UNMATCHED";
                                        isMatchImage = "ng.png";
                                        isScannedB = "ok.png";
                                        resultB = Settings.scanQRValueB;
                                        playbeep.Play();

                                    }
                                    break;

                                default:
                                    isMatch = "UNMATCHED";
                                    isMatchImage = "ng.png";
                                    isScannedB = "ok.png";
                                    resultB = Settings.scanQRValueB;
                                    playbeep.Play();

                                    break;
                            }
                        }

                        //if (scanfor == "a")
                        //{
                        //    App.Current.MainPage.DisplayAlert("Alert", scanresult.Text, "OK");
                        //    Settings.scanQRValueA = ScannedValueA = scanresult != null ? scanresult.Text : "scanned";


                        //    //if (SelectedScanRule == "First 3")
                        //    //{

                        //    //}
                        //    //else if (SelectedScanRule == "First 5")
                        //    //{

                        //    //}
                        //    //else if (SelectedScanRule == "Last 4")
                        //    //{

                        //    //}
                        //    //else if (SelectedScanRule == "Complete")
                        //    //{
                        //    //    isMatch = "MATCHED";
                        //    //    isMatchImage = "match.png";
                        //    //    isScannedA = "ok.png";
                        //    //    resultA = Settings.scanQRValueA;
                        //    //}
                        //    ////if (Settings.scanQRValueA == Settings.scanQRValueB)
                        //    ////{
                        //    ////    isMatch = "MATCHED";
                        //    ////    isMatchImage = "match.png";
                        //    ////    isScannedA = "ok.png";
                        //    ////    resultA = Settings.scanQRValueA;
                        //    ////}
                        //    //else
                        //    //{
                        //    //    isMatch = "UNMATCHED";
                        //    //    isMatchImage = "unmatch.png";
                        //    //    isScannedA = "ok.png";
                        //    //    resultA = Settings.scanQRValueA;
                        //    //}

                        //    isScannedA = "ok.png";
                        //    resultA = Settings.scanQRValueA;
                        //    //comparemodel.AValue = resultA;
                        //    isEnableAFrame = false;
                        //    opacityA = 0.50;
                        //    isEnableBFrame = true;
                        //    opacityB = 1;
                        //}
                        //else
                        //{
                        //    App.Current.MainPage.DisplayAlert("Alert", scanresult.Text, "OK");
                        //    //ScannedValueA = scanresult.Text;
                        //    Settings.scanQRValueB = ScannedValueB = scanresult != null ? scanresult.Text : "scanned";

                        //    if (SelectedScanRule == "First 3")
                        //    {
                        //        if (Settings.scanQRValueA.Substring(0, 3) == Settings.scanQRValueB.Substring(0, 3))
                        //        {
                        //            isMatch = "MATCHED";
                        //            isMatchImage = "match.png";
                        //            isScannedB = "ok.png";
                        //            resultB = Settings.scanQRValueB;
                        //        }
                        //    }
                        //    else if (SelectedScanRule == "First 5")
                        //    {
                        //        if (Settings.scanQRValueA.Substring(0, 5) == Settings.scanQRValueB.Substring(0, 5))
                        //        {
                        //            isMatch = "MATCHED";
                        //            isMatchImage = "match.png";
                        //            isScannedB = "ok.png";
                        //            resultB = Settings.scanQRValueB;
                        //        }
                        //    }
                        //    else if (SelectedScanRule == "Last 4")
                        //    {
                        //        int Astartindex = Settings.scanQRValueA.Length - 4;
                        //        int Bstartindex = Settings.scanQRValueB.Length - 4;

                        //        if (Settings.scanQRValueA.Substring(Astartindex, 4) == Settings.scanQRValueB.Substring(Bstartindex, 4))
                        //        {
                        //            isMatch = "MATCHED";
                        //            isMatchImage = "match.png";
                        //            isScannedB = "ok.png";
                        //            resultB = Settings.scanQRValueB;
                        //        }
                        //    }
                        //    else if (SelectedScanRule == "Complete")
                        //    {
                        //        if (Settings.scanQRValueA == Settings.scanQRValueB)
                        //        {
                        //            isMatch = "MATCHED";
                        //            isMatchImage = "match.png";
                        //            isScannedB = "ok.png";
                        //            resultB = Settings.scanQRValueB;
                        //        }
                        //    }
                        //    //if (Settings.scanQRValueA == Settings.scanQRValueB)
                        //    //{
                        //    //    isMatch = "MATCHED";
                        //    //    isMatchImage = "match.png";
                        //    //    isScannedB = "ok.png";
                        //    //    resultB = Settings.scanQRValueB;
                        //    //}
                        //    else
                        //    {
                        //        isMatch = "UNMATCHED";
                        //        isMatchImage = "unmatch.png";
                        //        isScannedB = "ok.png";
                        //        resultB = Settings.scanQRValueB;
                        //    }
                        //}

                        isShowMatch = isScannedA == "ok.png" && isScannedB == "ok.png" ? true : false;

                        if (!string.IsNullOrEmpty(Settings.scanQRValueA) && !string.IsNullOrEmpty(Settings.scanQRValueB))
                        {
                            comparemodel.HistorySerialNo = historySerialNo++;
                            comparemodel.AValue = Settings.scanQRValueA;
                            comparemodel.BValue = Settings.scanQRValueB;
                            comparemodel.IsMatchedImg = isMatchImage;
                            compareList.Insert(0, comparemodel);

                            compareHistoryList = new List<CompareHistoryList>(compareList);

                            var val = compareList.OrderByDescending(w => w.HistorySerialNo).Take(5);

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
                            }
                        }

                    });
                    };
                    if (Navigation.ModalStack.Count == 0 ||
                                        Navigation.ModalStack.Last().GetType() != typeof(ZXingScannerPage))
                    {
                        await Navigation.PushAsync(ScannerPage);
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                }
                //}
                //else
                //{
                //    playbeep.Play();
                //}
            }
            catch (Exception ex)
            {

            }
            finally
            {
                loadindicator = false;
            }
        }

        private async Task TabChange(string tab)
        {
            try
            {
                if (tab == "scan")
                {
                    IsScanContentVisible = ScanTabVisibility = true;
                    IsConfigContentVisible = ConfigTabVisibility = false;
                    ScanTabTextColor = Color.Green;
                    CompareTabTextColor = Color.Black;
                }
                else
                {
                    IsScanContentVisible = ScanTabVisibility = false;
                    IsConfigContentVisible = ConfigTabVisibility = true;
                    ScanTabTextColor = Color.Black;
                    CompareTabTextColor = Color.Green;
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task SaveConfig()
        {
            try
            {
                if (TotalCount > 0)
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
                    ScanTabTextColor = Color.Green;
                    CompareTabTextColor = Color.Black;
                }
                else
                {
                    TotalErrorTxt = TotalCount == null ? "Enter total value" : "Total must be more than 0";
                    IsTotalValidMsg = true;
                }
            }
            catch (Exception ex)
            {

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
                OnPropertyChanged("ScanTabTextColor");
            }
        }

        private Color _CompareTabTextColor = Color.Green;
        public Color CompareTabTextColor
        {
            get => _CompareTabTextColor;
            set
            {
                _CompareTabTextColor = value;
                OnPropertyChanged("CompareTabTextColor");
            }
        }

        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }

        private string _ScannedValueA;
        public string ScannedValueA
        {
            get => _ScannedValueA;
            set
            {
                _ScannedValueA = value;
                OnPropertyChanged("ScannedValueA");
            }
        }

        private string _ScannedValueB;
        public string ScannedValueB
        {
            get => _ScannedValueB;
            set
            {
                _ScannedValueB = value;
                OnPropertyChanged("ScannedValueB");
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

        private List<string> _ScanRuleLst = new List<string>(new string[] { "First 3", "First 5", "Last 4", "Last 9", "Last 10", "Last 11", "Complete Match" });
        public List<string> ScanRuleLst
        {
            get => _ScanRuleLst;
            set
            {
                _ScanRuleLst = value;
                NotifyPropertyChanged("ScanRuleLst");
            }
        }

        private string _SelectedScanRule = "First 3";
        public string SelectedScanRule
        {
            get => _SelectedScanRule;
            set
            {
                _SelectedScanRule = value;
                NotifyPropertyChanged("SelectedScanRule");
            }
        }

        private string _SelectedScanRuleHeader = "First 3";
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
