﻿using Plugin.Permissions;
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
    public class PolyBoxViewModel : IBase
    {
        YPSService trackService;
        public int historySerialNo = 1;
        List<CompareHistoryList> lstcomparehistory = new List<CompareHistoryList>();
        private PolyBox polyboxPage;
        public int? scancountpermit;
        private ScanerSettings scansetting;
        public INavigation Navigation { get; set; }
        public ICommand StartScanningCmd { get; set; }
        public ICommand ScanTabCmd { get; set; }
        public ICommand ScanConfigCmd { get; set; }
        public ICommand SaveClickCmd { get; set; }
        public PolyBoxViewModel(INavigation _Navigation, PolyBox polyboxpage, bool reset)
        {
            try
            {
                loadindicator = true;
                Navigation = _Navigation;
                trackService = new YPSService();
                polyboxPage = polyboxpage;
                BgColor = YPS.CommonClasses.Settings.Bar_Background;
                StartScanningCmd = new Command(async () => await StartScanning());
                ScanTabCmd = new Command(async () => await TabChange("scan"));
                ScanConfigCmd = new Command(async () => await TabChange("config"));

                Settings.scanQRValueA = "";
                Settings.scanQRValueB = "";

                ChangeLabel();
                scansetting = SettingsArchiver.UnarchiveSettings();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PolyBoxViewModel constructor -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            loadindicator = false;
        }

        private async Task StartScanning()
        {
            try
            {
                loadindicator = true;

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
                        ScanerSettings scanset = new ScanerSettings();
                        SettingsArchiver.ArchiveSettings(scanset);

                        await Navigation.PushModalAsync(new ScannerPage(scanset, this));
                    }
                    catch (Exception ex1)
                    {
                        YPSLogger.ReportException(ex1, "StartScanning method while navigate-> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "StartScanning method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
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

                Stream oksr = assembly.GetManifestResourceStream("YPS." + "okbeep.mp3");
                ISimpleAudioPlayer okplaybeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                okplaybeep.Load(oksr);

                Device.BeginInvokeOnMainThread(async () =>
                {

                    string sp = "\n\n";
                    var scanvalue = scanresult.Split('/');

                    foreach (var val in scanvalue)
                    {
                        if (scanvalue[0] == CargoCategory)
                        {
                            CargoCategory = val;
                            continue;
                        }
                        else if (scanvalue[1] == BagNumber)
                        {
                            BagNumber = val;
                            continue;
                        }
                        else
                        {
                            TQBPkgSizeNoL1 = scanvalue[2];
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Scanditscan method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
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
                    ConfigTabTextColor = Color.Black;
                }
                else
                {
                    IsConfigContentVisible = ConfigTabVisibility = true;
                    ScanTabTextColor = Color.Black;
                    ConfigTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TabChange method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            loadindicator = false;
        }

        /// <summary>
        /// This is for changing the text dynamically
        /// </summary>
        public async void ChangeLabel()
        {
            try
            {
                loadindicator = true;

                labelobj = new DashboardLabelChangeClass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        //Getting Label values & Status based on FieldID
                        var rule = labelval.Where(wr => wr.FieldID == labelobj.Rule.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var total = labelval.Where(wr => wr.FieldID == labelobj.TotalPolibox.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var scannedtoday = labelval.Where(wr => wr.FieldID == labelobj.ScannedToday.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var isr = labelval.Where(wr => wr.FieldID == labelobj.ISR.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var scan = labelval.Where(wr => wr.FieldID == labelobj.Scan.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var configure = labelval.Where(wr => wr.FieldID == labelobj.Configure.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var save = labelval.Where(wr => wr.FieldID == labelobj.Save.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var printtag = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.PrintTag.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var next = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Next.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var back = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Back.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var scandate = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ScanDate.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var scanby = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ScannedBy.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var status = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Status.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var location = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Location.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var remarks = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Remark.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var done = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Done.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var bagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.BagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var cargocategory1 = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.CargoCategory.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var tqbpkgsizenol1 = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TQBPkgSizeNoL1.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.Rule.Name = (rule != null ? (!string.IsNullOrEmpty(rule.LblText) ? rule.LblText : labelobj.Rule.Name) : labelobj.Rule.Name) + " :";
                        labelobj.TotalPolibox.Name = (total != null ? (!string.IsNullOrEmpty(total.LblText) ? total.LblText : labelobj.TotalPolibox.Name) : labelobj.TotalPolibox.Name) + " :";
                        labelobj.RuleForHint.Name = (rule != null ? (!string.IsNullOrEmpty(rule.LblText) ? rule.LblText : labelobj.Rule.Name) : labelobj.Rule.Name);
                        labelobj.TotalPoliboxlForHint.Name = (total != null ? (!string.IsNullOrEmpty(total.LblText) ? total.LblText : labelobj.TotalPoliboxlForHint.Name) : labelobj.TotalPoliboxlForHint.Name);
                        labelobj.ScannedToday.Name = (scannedtoday != null ? (!string.IsNullOrEmpty(scannedtoday.LblText) ? scannedtoday.LblText : labelobj.ScannedToday.Name) : labelobj.ScannedToday.Name) + " :";
                        labelobj.ISR.Name = (isr != null ? (!string.IsNullOrEmpty(isr.LblText) ? isr.LblText : labelobj.ISR.Name) : labelobj.ISR.Name) + " :";

                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                        labelobj.CargoCategory.Name = (cargocategory1 != null ? (!string.IsNullOrEmpty(cargocategory1.LblText) ? cargocategory1.LblText : labelobj.CargoCategory.Name) : labelobj.CargoCategory.Name) + " :";
                        labelobj.TQBPkgSizeNoL1.Name = (tqbpkgsizenol1 != null ? (!string.IsNullOrEmpty(tqbpkgsizenol1.LblText) ? tqbpkgsizenol1.LblText : labelobj.TQBPkgSizeNoL1.Name) : labelobj.TQBPkgSizeNoL1.Name) + " :";

                        labelobj.Scan.Name = scan != null ? (!string.IsNullOrEmpty(scan.LblText) ? scan.LblText : labelobj.Scan.Name) : labelobj.Scan.Name;
                        labelobj.Configure.Name = configure != null ? (!string.IsNullOrEmpty(configure.LblText) ? configure.LblText : labelobj.Configure.Name) : labelobj.Configure.Name;

                        labelobj.Save.Name = save != null ? (!string.IsNullOrEmpty(save.LblText) ? save.LblText : labelobj.Save.Name) : labelobj.Save.Name;
                        labelobj.PrintTag.Name = (printtag != null ? (!string.IsNullOrEmpty(printtag.LblText) ? printtag.LblText : labelobj.PrintTag.Name) : labelobj.Next.Name) + " :";
                        labelobj.Next.Name = (next != null ? (!string.IsNullOrEmpty(next.LblText) ? next.LblText : labelobj.Next.Name) : labelobj.Next.Name) + " " + labelobj.Scan.Name;
                        labelobj.Back.Name = (back != null ? (!string.IsNullOrEmpty(back.LblText) ? back.LblText : labelobj.Back.Name) : labelobj.Back.Name);
                        labelobj.Done.Name = done != null ? (!string.IsNullOrEmpty(done.LblText) ? done.LblText : labelobj.Done.Name) : labelobj.Done.Name;

                        labelobj.ScanDate.Name = scandate != null ? (!string.IsNullOrEmpty(scandate.LblText) ? scandate.LblText : labelobj.ScanDate.Name) : labelobj.ScanDate.Name;
                        labelobj.ScannedBy.Name = scanby != null ? (!string.IsNullOrEmpty(scanby.LblText) ? scanby.LblText : labelobj.ScannedBy.Name) : labelobj.ScannedBy.Name;
                        labelobj.Status.Name = (status != null ? (!string.IsNullOrEmpty(status.LblText) ? status.LblText : labelobj.Status.Name) : labelobj.Status.Name) + " : ";
                        labelobj.Location.Name = location != null ? (!string.IsNullOrEmpty(location.LblText) ? location.LblText : labelobj.Location.Name) : labelobj.Location.Name;
                        labelobj.Remark.Name = remarks != null ? (!string.IsNullOrEmpty(remarks.LblText) ? remarks.LblText : labelobj.Remark.Name) : labelobj.Remark.Name;

                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabel method -> in PolyBoxViewModel.cs " + Settings.userLoginID);
            }
            finally
            {
                loadindicator = false;
            }
        }

        #region Properties

        #region Properties for dynamic label change
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields CargoCategory { get; set; } = new DashboardLabelFields { Status = true, Name = "CargoCategory1" };
            public DashboardLabelFields BagNumber { get; set; } = new DashboardLabelFields { Status = true, Name = "BagNumber" };
            public DashboardLabelFields TQBPkgSizeNoL1 { get; set; } = new DashboardLabelFields { Status = true, Name = "TQB_PkgSizeNo_L1" };

            public DashboardLabelFields Rule { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMRule" };
            public DashboardLabelFields TotalPolibox { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMTotalPolibox" };
            public DashboardLabelFields RuleForHint { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMRule" };
            public DashboardLabelFields TotalPoliboxlForHint { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMTotal" };
            public DashboardLabelFields ScannedToday { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMScannedToday" };
            public DashboardLabelFields ISR { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMISL" };

            public DashboardLabelFields ScanDate { get; set; } = new DashboardLabelFields { Status = true, Name = "EventDT_L1" };
            public DashboardLabelFields ScannedBy { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMTotal" };
            public DashboardLabelFields Status { get; set; } = new DashboardLabelFields { Status = true, Name = "Attributes" };
            public DashboardLabelFields Location { get; set; } = new DashboardLabelFields { Status = true, Name = "FromLoc_L1" };
            public DashboardLabelFields Remark { get; set; } = new DashboardLabelFields { Status = true, Name = "EventRemarks_L1" };

            public DashboardLabelFields Scan { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMScan" };
            public DashboardLabelFields Configure { get; set; } = new DashboardLabelFields { Status = true, Name = "TBMConfigure" };
            public DashboardLabelFields Save { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMbtnSave" };
            public DashboardLabelFields PrintTag { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMPrintTag" };
            public DashboardLabelFields Next { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMbtnNext" };
            public DashboardLabelFields Back { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMbtnBack" };
            public DashboardLabelFields Done { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMbtnDone"
            };
        }
        public class DashboardLabelFields : IBase
        {
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

        private string _CargoCategory;
        public string CargoCategory
        {
            get => _CargoCategory;
            set
            {
                _CargoCategory = value;
                RaisePropertyChanged("CargoCategory");
            }
        }

        private string _BagNumber;
        public string BagNumber
        {
            get => _BagNumber;
            set
            {
                _BagNumber = value;
                RaisePropertyChanged("BagNumber");
            }
        }

        private string _TQBPkgSizeNoL1;
        public string TQBPkgSizeNoL1
        {
            get => _TQBPkgSizeNoL1;
            set
            {
                _TQBPkgSizeNoL1 = value;
                RaisePropertyChanged("TQBPkgSizeNoL1    ");
            }
        }

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

        private Color _ConfigTabTextColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color ConfigTabTextColor
        {
            get => _ConfigTabTextColor;
            set
            {
                _ConfigTabTextColor = value;
                RaisePropertyChanged("ConfigTabTextColor");
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
