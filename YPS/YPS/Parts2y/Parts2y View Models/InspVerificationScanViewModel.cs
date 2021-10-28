using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using static YPS.Model.SearchModel;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class InspVerificationScanViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        YPSService trackService;
        InspVerificationScanPage pagename;
        bool isAllDone;
        public ICommand reScanCmd { set; get; }
        public ICommand MoveToInspCmd { set; get; }
        public AllPoData selectedTagData { get; set; }
        public ICommand ScanResultCommand { set; get; }
        public ICommand FlashCommand { set; get; }

        public InspVerificationScanViewModel(INavigation _Navigation, AllPoData selectedtagdata, bool isalldone, InspVerificationScanPage page)
        {
            try
            {
                trackService = new YPSService();
                Navigation = _Navigation;
                pagename = page;
                isAllDone = isalldone;
                selectedTagData = selectedtagdata;

                #region BInding tab & click event methods to respective ICommand properties
                reScanCmd = new Command(async () => await ReScan());
                MoveToInspCmd = new Command(async () => await MoveForInspection(selectedTagData));
                #endregion
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "InspVerificationScanViewModel constructor -> in InspVerificationScanViewModel.cs " + Settings.userLoginID);
                trackService.Handleexception(ex);
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
                YPSLogger.ReportException(ex, "ReScan method -> in InspVerificationScanViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
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
                                await GetDataAndVerify();
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
                else
                {
                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OpenScanner method -> in InspVerificationScanViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        public async Task GetDataAndVerify()
        {
            try
            {
                IndicatorVisibility = true;

                YPSLogger.TrackEvent("InspVerificationScanViewModel.cs", "in GerDataAndVerify method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {

                    SendPodata sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;

                    if (selectedTagData != null)
                    {
                        var taskresourceid = selectedTagData.TaskResourceID;

                        if (!string.IsNullOrEmpty(selectedTagData.TagNumber))
                        {
                            ScannedCompareData = (selectedTagData.TagNumber == ScannedResult) ? ScannedResult : null;
                        }
                        else
                        {
                            ScannedCompareData = (selectedTagData.IdentCode == ScannedResult) ? ScannedResult : null;
                        }


                        if (!string.IsNullOrEmpty(ScannedCompareData))
                        {

                            ScannedOn = DateTime.Now.ToString(@"MM/dd/yyyy hh:mm:ss tt");
                            StatusText = selectedTagData.TagTaskStatus == 2 ? "Done" : "Verified";
                            StatusTextBgColor = Color.DarkGreen;
                            ScannedValue = ScannedResult;

                            if (taskresourceid == 0 || taskresourceid == null)
                            {
                                var result = await trackService.AssignUnassignedTask(selectedTagData.TaskID);

                                if (result?.status == 1)
                                {
                                    IsInspEnable = true;
                                    InspOpacity = 1.0;
                                }
                            }
                            else
                            {
                                IsInspEnable = true;
                                InspOpacity = 1.0;
                            }

                        }
                        else
                        {
                            ScannedOn = DateTime.Now.ToString(@"MM/dd/yyyy hh:mm:ss tt");
                            StatusText = "Not matched";
                            StatusTextBgColor = Color.Red;
                            ScannedValue = ScannedResult;
                            IsInspEnable = false;
                            InspOpacity = 0.5;
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
                YPSLogger.ReportException(ex, "GetDataAndVerify method -> in InspVerificationScanViewModel.cs " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async Task MoveForInspection(AllPoData podata)
        {
            try
            {
                IndicatorVisibility = true;
                YPSLogger.TrackEvent("InspVerificationScanViewModel.cs", "in MoveForInspection method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    Settings.POID = podata.POID;
                    Settings.TaskID = podata.TaskID;

                    if (Settings.VersionID == 1)
                    {
                        await Navigation.PushAsync(new EPartsInspectionQuestionsPage(podata, isAllDone), false);
                    }
                    else if (Settings.VersionID == 2)
                    {
                        await Navigation.PushAsync(new CVinInspectQuestionsPage(podata, isAllDone), false);
                    }
                    else if (Settings.VersionID == 3)
                    {
                        await Navigation.PushAsync(new KRPartsInspectionQuestionsPage(podata, isAllDone), false);
                    }
                    else if (Settings.VersionID == 4)
                    {
                        await Navigation.PushAsync(new KPPartsInspectionQuestionPage(podata, isAllDone), false);
                    }
                    else if (Settings.VersionID == 5)
                    {
                        await Navigation.PushAsync(new PPartsInspectionQuestionsPage(podata, isAllDone), false);
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "MoveForInspection method -> in InspVerificationScanViewModel.cs" + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }

        }

        #region Properties

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

        public AllPoData _ScannedAllPOData;
        public AllPoData ScannedAllPOData
        {
            get { return _ScannedAllPOData; }
            set
            {
                _ScannedAllPOData = value;
                RaisePropertyChanged("_ScannedAllPOData");
            }
        }

        public string _ScannedCompareData;
        public string ScannedCompareData
        {
            get { return _ScannedCompareData; }
            set
            {
                _ScannedCompareData = value;
                RaisePropertyChanged("_ScannedCompareData");
            }
        }

        public string _ScannedValue;
        public string ScannedValue
        {
            get { return _ScannedValue; }
            set
            {
                _ScannedValue = value;
                RaisePropertyChanged("_ScannedValue");
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

        public double _InspOpacity = 0.5;
        public double InspOpacity
        {
            get { return _InspOpacity; }
            set
            {
                _InspOpacity = value;
                RaisePropertyChanged("InspOpacity");
            }
        }

        public bool _IsInspEnable;
        public bool IsInspEnable
        {
            get { return _IsInspEnable; }
            set
            {
                _IsInspEnable = value;
                RaisePropertyChanged("IsInspEnable");
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