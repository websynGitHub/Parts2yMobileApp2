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
                //StartBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage Constructor -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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

                //StartBtn.IsVisible = Settings.ContinuousAfterScan == false ? false : true;
                //StartBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;

                PickerView.Delegate = new ScannerDelegate(this);
                PickerView.Settings = Settings;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage Constructor with two parameters -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }

        }

        public ScannerPage(ScanerSettings settings)
        {
            try
            {
                Settings = new ScanerSettings();
                MyNavigationPage = new NavigationPage();
                Settings = settings;
                YPS.CommonClasses.Settings.scanredirectpage = "ScanditScan";
                InitializeComponent();

                //StartBtn.IsVisible = false;
                BackBtn.BackgroundColor = CommonClasses.Settings.Bar_Background;

                PickerView.Delegate = new ScannerDelegate(this);
                PickerView.Settings = Settings;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScannerPage Constructor with one parameter -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Close_Scanner Method -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "Start_Scanning Method -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }
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
                            await App.Current.MainPage.Navigation.PopModalAsync();
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
                                await App.Current.MainPage.Navigation.PopModalAsync();
                            }
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DidScan Method -> in ScannerPage.xaml.cs " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = scannerPage.trackService.Handleexception(ex);
            }
        }
    }
}