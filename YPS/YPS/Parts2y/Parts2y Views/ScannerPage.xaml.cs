using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Service;
using YPS.Helpers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YPS.Parts2y.Parts2y_Views
{
    public interface IScannerDelegate
    {
        Task DidScan(string symbology, string code);
    }

    public partial class ScannerPage : ContentPage
    {
        public static NavigationPage MyNavigationPage;
        public ScanerSettings Settings { get; set; }
        public CompareContinuousViewModel comparecontinuousVM;
        public CompareViewModel compareVM;
        public PolyBoxViewModel polyboxVM;
        public ScanPageViewModel scanPageVM;
        public InspVerificationScanViewModel inspVerifVM;
        public YPSService trackService;
        public Color BgColor { get; } = CommonClasses.Settings.Bar_Background;

        public ScannerPage()
        {
            try
            {
                trackService = new YPSService();
                Settings = new ScanerSettings();
                InitializeComponent();
                MyNavigationPage = new NavigationPage();
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;
                ChangeLabel();
                BindingContext = this;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage constructor -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public ScannerPage(ScanerSettings settings, CompareContinuousViewModel compareContinuousVM)
        {
            try
            {
                Settings = new ScanerSettings();
                MyNavigationPage = new NavigationPage();
                Settings = settings;
                comparecontinuousVM = compareContinuousVM;
                InitializeComponent();
                ChangeLabel();
                BindingContext = this;

                //StartBtn.IsVisible = Settings.ContinuousAfterScan == false ? false : true;
                //StartBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;

                PickerView.Delegate = new ScannerDelegate(this);
                PickerView.Settings = Settings;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage constructor with two parameters -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }

        }

        public ScannerPage(ScanerSettings settings, CompareViewModel compareViewModel)
        {
            try
            {
                InitializeComponent();

                Settings = new ScanerSettings();
                MyNavigationPage = new NavigationPage();
                Settings = settings;
                compareVM = compareViewModel;
                YPS.CommonClasses.Settings.scanredirectpage = "ScanditScan";
                ChangeLabel();
                BindingContext = this;
                //StartBtn.IsVisible = false;
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;

                PickerView.Delegate = new ScannerDelegate(this);
                PickerView.Settings = Settings;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage constructor with one parameter -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public ScannerPage(ScanerSettings settings, ScanPageViewModel scanPageViewModel)
        {
            try
            {
                InitializeComponent();

                Settings = new ScanerSettings();
                MyNavigationPage = new NavigationPage();
                Settings = settings;
                scanPageVM = scanPageViewModel;
                YPS.CommonClasses.Settings.scanredirectpage = "ScanPage";
                ChangeLabel();
                BindingContext = this;
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;

                PickerView.Delegate = new ScannerDelegate(this);
                PickerView.Settings = Settings;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage constructor with ScanPageViewModel parameter -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public ScannerPage(ScanerSettings settings, InspVerificationScanViewModel inspVerificationScanViewModel)
        {
            try
            {
                InitializeComponent();

                Settings = new ScanerSettings();
                MyNavigationPage = new NavigationPage();
                Settings = settings;
                inspVerifVM = inspVerificationScanViewModel;
                YPS.CommonClasses.Settings.scanredirectpage = "InspVerificationScan";
                ChangeLabel();
                BindingContext = this;
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;

                PickerView.Delegate = new ScannerDelegate(this);
                PickerView.Settings = Settings;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage constructor with InspVerificationScan parameter -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }
        public ScannerPage(ScanerSettings settings, PolyBoxViewModel polyboxViewModel)
        {
            try
            {
                InitializeComponent();

                Settings = new ScanerSettings();
                MyNavigationPage = new NavigationPage();
                Settings = settings;
                polyboxVM = polyboxViewModel;
                YPS.CommonClasses.Settings.scanredirectpage = "Polybox";
                ChangeLabel();
                BindingContext = this;
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;

                PickerView.Delegate = new ScannerDelegate(this);
                PickerView.Settings = Settings;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage constructor with polyboxViewModel parameter -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }


        int i;
        public void ShowResult(String symbology, String code)
        {
            //ResultView.IsVisible = true;
            //res.IsVisible = true;
            //SymbologyLabel.Text = " Scan Id= ";// symbology;
            //CodeLabel.Text = code;
            //string sp = "\n\n";
            //var scanvalue = code.Split(sp.ToCharArray());
            //valueadding(scanvalue);

        }

        public void ResumeScanning()
        {
            PickerView.StartScanning();
        }

        public void PauseScanning()
        {
            PickerView.PauseScanning();
        }

        public void StopScanning()
        {
            PickerView.StopScanning();
        }

        private void Close_Scanner(object sender, EventArgs e)
        {
            try
            {
                if (YPS.CommonClasses.Settings.scanredirectpage.Trim().ToLower() == "ScanPage".Trim().ToLower() || YPS.CommonClasses.Settings.scanredirectpage.Trim().ToLower() == "InspVerificationScan".Trim().ToLower())
                {
                    Navigation.PopAsync(false);
                    StopScanning();
                }
                else
                {
                    Navigation.PopModalAsync(false);
                }
                StopScanning();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Close_Scanner method -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        private void Start_Scanning(object sender, EventArgs e)
        {
            try
            {
                Settings.CanScan = true;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Start_Scanning method -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        public async void ChangeLabel()
        {
            try
            {
                labelobj = new DashboardLabelChangeClass();

                if (CommonClasses.Settings.alllabeslvalues != null && CommonClasses.Settings.alllabeslvalues.Count > 0)
                {
                    List<Model.Alllabeslvalues> labelval = CommonClasses.Settings.alllabeslvalues.Where(wr => wr.VersionID == CommonClasses.Settings.VersionID && wr.LanguageID == CommonClasses.Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var back = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Back.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        labelobj.Back.Name = (back != null ? (!string.IsNullOrEmpty(back.LblText) ? back.LblText : labelobj.Back.Name) : labelobj.Back.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                await trackService.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabel method -> in Back_Tapped.xaml.cs " + CommonClasses.Settings.userLoginID);
            }
        }

        #region Labels
        #region INotifyPropertyChanged Implimentation
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields Back { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMbtnBack" };
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
    }

    public class ScannerDelegate : IScannerDelegate
    {
        private ScannerPage scannerPage;

        public ScannerDelegate(ScannerPage scannerPage)
        {
            this.scannerPage = scannerPage;
        }

        public async Task DidScan(string symbology, string code)
        {
            try
            {
                Settings.scanQRValuecode = code;

                if (Settings.scanredirectpage.Trim().ToLower() == "CompareContinuous".Trim().ToLower())
                {
                    if (scannerPage.Settings.ContinuousAfterScan == false)
                    {
                        await scannerPage.comparecontinuousVM.Scanditscan(Settings.scanQRValuecode);

                        if (App.Current.MainPage.Navigation.ModalStack.Count > 0)
                        {
                            await App.Current.MainPage.Navigation.PopModalAsync(false);
                        }
                    }
                    else
                    {
                        if (scannerPage.comparecontinuousVM.scancountpermit > 0)
                        {
                            //if (scannerPage.Settings.CanScan == true)
                            //{
                            //    scannerPage.Settings.CanScan = false;
                            await scannerPage.comparecontinuousVM.Scanditscan(Settings.scanQRValuecode);
                            //}
                        }
                        else
                        {
                            if (App.Current.MainPage.Navigation.ModalStack.Count > 0)
                            {
                                await App.Current.MainPage.Navigation.PopModalAsync(false);
                            }
                        }
                    }
                }
                else if (Settings.scanredirectpage.Trim().ToLower() == "Polybox".Trim().ToLower())
                {
                    await scannerPage.polyboxVM.Scanditscan(Settings.scanQRValuecode);

                    if (App.Current.MainPage.Navigation.ModalStack.Count > 0)
                    {
                        await App.Current.MainPage.Navigation.PopModalAsync(false);
                    }
                }
                else if (Settings.scanredirectpage.Trim().ToLower() == "ScanPage".Trim().ToLower())
                {
                    await scannerPage.scanPageVM.Scanditscan(Settings.scanQRValuecode);

                    if (scannerPage.scanPageVM.Navigation.NavigationStack.Count > 0)
                    {
                        await scannerPage.scanPageVM.Navigation.PopAsync(false);
                    }
                }
                else if (Settings.scanredirectpage.Trim().ToLower() == "InspVerificationScan".Trim().ToLower())
                {
                    await scannerPage.inspVerifVM.Scanditscan(Settings.scanQRValuecode);

                    if (scannerPage.inspVerifVM.Navigation.NavigationStack.Count > 0)
                    {
                        await scannerPage.inspVerifVM.Navigation.PopAsync(false);
                    }
                }
                else
                {
                    await scannerPage.compareVM.Scanditscan(Settings.scanQRValuecode);
                    Settings.scanredirectpage = string.Empty;

                    if (App.Current.MainPage.Navigation.ModalStack.Count > 0)
                    {
                        await App.Current.MainPage.Navigation.PopModalAsync(false);
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DidScan method -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = scannerPage.trackService.Handleexception(ex);
            }
        }

    }
}