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

                #region Cam Not Always On
                //if (Settings.ContinuousAfterScan == false)
                //{
                //    StartBtn.IsVisible = false;
                //    PickerView.Delegate = new ScannerDelegate(this);
                //    PickerView.Settings = Settings;
                //}
                //else
                //{
                //    StartBtn.IsVisible = true;
                //}
                #endregion Cam Not Always On
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

        //public void Handle_Clicked(object sender, EventArgs e)
        //{
        //    HideResult();
        //    res.IsVisible = false;
        //    PickerView.StartScanning();
        //}

        //public void HideResult()
        //{
        //    ResultView.IsVisible = false;
        //}

        //ObservableCollection<ResultListdata> scanresult = new ObservableCollection<ResultListdata>();
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
        //public async Task valueadding(string[] splitvalue)
        //{
        //    try
        //    {
        //        if (LocalSettings.scanresult != null)
        //            i = LocalSettings.scanresult.Count;
        //        foreach (var item in splitvalue)
        //        {
        //            if (!string.IsNullOrEmpty(item))
        //            {
        //                ResultListdata data = new ResultListdata();
        //                data.message = item;
        //                data.ID = scanresult.Count + 1;
        //                scanresult.Add(data);
        //            }
        //        }
        //        LocalSettings.scanresult = scanresult;
        //        list.ItemsSource = LocalSettings.scanresult;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

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
                #region Cam Not Always On
                //PickerView.Delegate = new ScannerDelegate(this);
                //PickerView.Settings = Settings;
                //PickerView.StartScanning();
                #endregion Cam Not Always On
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
                //Settings.scanredirectpage = "ScanditScan";

                //int numExpectedCodes = 5;

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

                        #region Cam Not Always On
                        //if (scannerPage.comparecontinuousVM.scancountpermit > 0)
                        //{
                        //    scannerPage.PickerView.StopScanning();
                        //    await scannerPage.comparecontinuousVM.Scanditscan(Settings.scanQRValuecode);
                        //}
                        //else
                        //{
                        //    if (App.Current.MainPage.Navigation.ModalStack.Count > 0)
                        //    {
                        //        await App.Current.MainPage.Navigation.PopModalAsync();
                        //    }
                        //}
                        #endregion Cam Not Always On
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