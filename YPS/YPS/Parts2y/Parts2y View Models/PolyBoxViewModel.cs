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
using YPS.Views;
using ZXing.Net.Mobile.Forms;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class PolyBoxViewModel : IBase
    {
        YPSService trackService;
        public int historySerialNo = 1;
        private PolyBox polyboxPage;
        public int? scancountpermit;
        private ScanerSettings scansetting;
        public INavigation Navigation { get; set; }
        public ICommand StartScanningCmd { get; set; }
        public ICommand DoneCmd { get; set; }
        public ICommand NextScanningCmd { get; set; }
        public ICommand ScanTabCmd { get; set; }
        public ICommand ScanConfigCmd { get; set; }
        public ICommand SaveClickCmd { get; set; }
        public ICommand PrintPolyboxCmd { get; set; }
        public ICommand GoToSettingsCmd { get; set; }

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
                DoneCmd = new Command(async () => await DoneClick());
                NextScanningCmd = new Command(async () => await NextScan());
                ScanTabCmd = new Command(async () => await TabChange("scan"));
                ScanConfigCmd = new Command(async () => await TabChange("config"));
                SaveClickCmd = new Command(async () => await SaveConfig());
                PrintPolyboxCmd = new Command(async () => await PrintPolybox());
                GoToSettingsCmd = new Command(async () =>
                {
                    loadindicator = true;
                    await Navigation.PushAsync(new ProfileSelectionPage("PolyboxPrintSettings"));
                    loadindicator = false;
                });

                Task.Run(() => GetSavedDatasFromDB()).Wait();
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

        public async Task GetSavedDatasFromDB()
        {
            try
            {
                loadindicator = true;

                var result = await trackService.GetSaveScanConfig(Settings.CompanyID, Settings.ProjectID, Settings.JobID);

                if (result?.status == 1 && result?.data != null)
                {
                    ConfigSelectedRule.ID = result.data.PolyboxRule;
                    ConfigSelectedFromLoc.Name = result.data.PolyboxLocation;
                    ConfigSelectedEventRemark.ID = result.data.PolyboxRemarks;
                    ConfigSelectedSataus = result.data.PolyboxStatus;
                }

                await GetPolyboxHeaderData();

                var resultData = await trackService.GetScanConfig();

                if (resultData?.data != null)
                {
                    ScanConfigResult.data = resultData.data;

                    ConfigSelectedRule = ConfigSelectedRule.ID == 0 ? ScanConfigResult.data.PolyboxRule[0] :
                        ScanConfigResult.data.PolyboxRule?.Where(wr => wr.ID == ConfigSelectedRule.ID).FirstOrDefault();
                    SelectedScanRuleHeader = result.data.PolyboxRule == 0 ? "" : ConfigSelectedRule.Name;

                    ScanSelectedFromLoc = (string.IsNullOrEmpty(ConfigSelectedFromLoc?.Name) ||
                        ConfigSelectedFromLoc?.Name == "0") ?
                        null : ScanConfigResult.data.PolyboxLocation?.Where(wr => wr.Name
                        == ConfigSelectedFromLoc?.Name).FirstOrDefault();

                    ConfigSelectedFromLoc = (string.IsNullOrEmpty(ConfigSelectedFromLoc?.Name) ||
                        ConfigSelectedFromLoc?.Name == "0") ?
                        ScanConfigResult.data.PolyboxLocation[0] :
                        ScanConfigResult.data.PolyboxLocation?.
                        Where(wr => wr.Name == ConfigSelectedFromLoc?.Name).FirstOrDefault();

                    ScanSelectedEventRemark = ConfigSelectedEventRemark.ID == 0 ? null : ScanConfigResult.data.PolyboxRemarks?.Where(wr => wr.ID == ConfigSelectedEventRemark.ID).FirstOrDefault();
                    ConfigSelectedEventRemark = ConfigSelectedEventRemark.ID == 0 ?
                        ScanConfigResult.data.PolyboxRemarks[0] :
                        ScanConfigResult.data.PolyboxRemarks?.Where(wr => wr.ID == ConfigSelectedEventRemark.ID).FirstOrDefault();

                    if (ConfigSelectedSataus == ScanConfigResult.data.PolyboxStatus[0].ID)
                    {
                        ScanIsEmpty = IsEmpty = true;
                    }
                    else if (ConfigSelectedSataus == ScanConfigResult.data.PolyboxStatus[1].ID)
                    {
                        ScanIsFull = IsFull = true;
                    }
                }

                if (ConfigSelectedRule?.ID != 0 && !string.IsNullOrEmpty(ConfigSelectedFromLoc?.Name) &&
                ConfigSelectedEventRemark?.ID != 0 && ConfigSelectedSataus != 0)
                {
                    IsScanEnable = true;
                    ScanOpacity = 1;
                    TabChange("scan");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetSavedConfigDataFromDB method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        private async Task GetPolyboxHeaderData()
        {
            try
            {
                loadindicator = true;

                var resultHeader = await trackService.GetPolyboxHeaderDetails();

                if (resultHeader?.status == 1 && resultHeader.data != null)
                {
                    TotalPolyboxCountHeader = resultHeader.data.TotalPolyboxCount;
                    TotalScannedToday = resultHeader.data.TotalScannedToday;
                    ISRHeader = resultHeader.data.ISR;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetPolyboxHeaderData method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        private async Task PrintPolybox()
        {
            try
            {
                loadindicator = true;

                YPSLogger.TrackEvent("PolyBoxViewModel.cs", " in PrintPolybox method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    YPSService pSService = new YPSService();
                    var printResult = await pSService.PrintPolyboxPDF(TagNumber);

                    PrintPDFModel printPDFModel = new PrintPDFModel();

                    if (printResult?.status == 1)
                    {
                        var bArray = printResult.data;
                        //var bArray = "JVBERi0xLjMNCjEgMCBvYmoNClsvUERGIC9UZXh0IC9JbWFnZUIgL0ltYWdlQyAvSW1hZ2VJXQ0KZW5kb2JqDQo1IDAgb2JqDQo8PCAvTGVuZ3RoIDMzOSAvRmlsdGVyIC9GbGF0ZURlY29kZSA+PiBzdHJlYW0NClgJrZRLS8NAEIDvQv/DgBcVOt3Z56y3ShXEHlq64Fna1FZaU2MV/PduEildaKSvBHKYDN98O5lM6+IDJKGtH0SKkDU4z6gZigye4b1Vpqj4loVAKTWomKuAHLKxm5y7AJ2HGDXovYcwBVHdxSs4qVFFWuhXGCpBCo2XECZwNZrNV6usuLyG8AbhBu7Dnix2aH3KuoVtynAj7hyjsny6uGNC52xVLOT9fHywtWUkkgkotd6L4iz6eJ5tyhOZ9miWLabt0aChC8aiITpDF4xHZq7KDl6K9eHfzpZTxgnomC4IdEInFOq2RbyooQNKozXmDB1QjMLVQzco8snXeA297PPwcSDyaK1OgI1TTBJdRJ1uTxYp/uVlsV62mH9nxU+s1T3C3qFIcLvcy8Vi4kKRlbsp3eNJzJ86yBiq4XU4yR8vofO41NDLYbjN+29bDX8BNsEHrg0KZW5kc3RyZWFtDQplbmRvYmoNCjIgMCBvYmoNCjw8IC9UeXBlIC9QYWdlIC9QYXJlbnQgNiAwIFIgL01lZGlhQm94IFswIDAgMTE3MS40NCA4NDEuNjhdIC9Db250ZW50cyA1IDAgUiAvUmVzb3VyY2VzIDw8IC9Qcm9jU2V0IDEgMCBSIC9YT2JqZWN0IDw8IC9JbTQgNCAwIFIgPj4gL0ZvbnQgPDwgL0YzIDMgMCBSID4+ID4+ID4+DQplbmRvYmoNCjQgMCBvYmoNCjw8IC9UeXBlIC9YT2JqZWN0IC9TdWJ0eXBlIC9JbWFnZSAvQ29sb3JTcGFjZSAvRGV2aWNlUkdCIC9CaXRzUGVyQ29tcG9uZW50IDggL0ZpbHRlciAvRmxhdGVEZWNvZGUgIC9XaWR0aCAzMDAgL0hlaWdodCAxNTAgL0xlbmd0aCAyOTY4ID4+DQpzdHJlYW0NClgJ5ZAxDgNBDALz/08nUqo0QcBhGij38NzY7/ejvL759/L6CX5hvv7rP/mXat408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZldBPdouLC+O+at408QgLNIaZmlUJ/WS3uLgw7qvmTROPsEBjmKlZlWDkAwVsNykNCmVuZHN0cmVhbQ0KZW5kb2JqDQozIDAgb2JqDQo8PCAvVHlwZSAvRm9udCAvU3VidHlwZSAvVHlwZTEgL0Jhc2VGb250IC9IZWx2ZXRpY2EgL0VuY29kaW5nIC9XaW5BbnNpRW5jb2RpbmcgPj4NCmVuZG9iag0KNiAwIG9iag0KPDwgL1R5cGUgL1BhZ2VzIC9LaWRzIFsgMiAwIFIgXSAvQ291bnQgMSA+Pg0KZW5kb2JqDQo3IDAgb2JqDQo8PCAvVHlwZSAvQ2F0YWxvZyAvUGFnZXMgNiAwIFIgPj4NCmVuZG9iag0KOCAwIG9iag0KPDwgL1RpdGxlIDxmZWZmMDA1MDAwNmYwMDZjMDA3OTAwNjIwMDZmMDA3ODAwNTIwMDY1MDA3MDAwNmYwMDcyMDA3ND4NCi9BdXRob3IgPD4NCi9TdWJqZWN0IDw+DQovQ3JlYXRvciAoTWljcm9zb2Z0IFJlcG9ydGluZyBTZXJ2aWNlcyAxNS4wLjAuMCkNCi9Qcm9kdWNlciAoTWljcm9zb2Z0IFJlcG9ydGluZyBTZXJ2aWNlcyBQREYgUmVuZGVyaW5nIEV4dGVuc2lvbiAxNS4wLjAuMCkNCi9DcmVhdGlvbkRhdGUgKEQ6MjAyMjAzMTAxNzMzMzArMDUnMzAnKQ0KPj4NCmVuZG9iag0KeHJlZg0KMCA5DQowMDAwMDAwMDAwIDY1NTM1IGYNCjAwMDAwMDAwMTAgMDAwMDAgbg0KMDAwMDAwMDQ4MSAwMDAwMCBuDQowMDAwMDAzODAzIDAwMDAwIG4NCjAwMDAwMDA2NTggMDAwMDAgbg0KMDAwMDAwMDA2NSAwMDAwMCBuDQowMDAwMDAzOTAzIDAwMDAwIG4NCjAwMDAwMDM5NjUgMDAwMDAgbg0KMDAwMDAwNDAxNyAwMDAwMCBuDQp0cmFpbGVyIDw8IC9TaXplIDkgL1Jvb3QgNyAwIFIgL0luZm8gOCAwIFIgPj4NCnN0YXJ0eHJlZg0KNDI5OQ0KJSVFT0Y=";
                        byte[] bytes = Convert.FromBase64String(bArray);
                        printPDFModel.bArray = bytes;
                        printPDFModel.FileName = "Polybox" + "_" + String.Format("{0:yyyyMMMdd_hh-mm-ss}", DateTime.Now) + ".pdf";
                        printPDFModel.PDFFileTitle = "Polybox";

                        switch (Device.RuntimePlatform)
                        {
                            case Device.iOS:
                                if (await FileManager.ExistsAsync(printPDFModel.FileName) == false)
                                {
                                    await FileManager.GetByteArrayData(printPDFModel);
                                }

                                var url = FileManager.GetFilePathFromRoot(printPDFModel.FileName);
                                DependencyService.Get<NewOpenPdfI>().passPath(url);
                                break;
                            case Device.Android:
                                await Navigation.PushAsync(new PdfViewPage(printPDFModel), false);
                                break;
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ListForPrint method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        public async Task SaveConfig()
        {
            try
            {
                loadindicator = true;

                bool result = await App.Current.MainPage.DisplayAlert("Save configuration", "Are you sure?", "Yes", "No");

                if (result)
                {
                    if (ConfigSelectedRule?.ID != 0 && !string.IsNullOrEmpty(ConfigSelectedFromLoc?.Name) &&
                ConfigSelectedEventRemark?.ID != 0 && ConfigSelectedSataus != 0)
                    {
                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {

                            var data = await trackService.SaveScanConfig(Settings.CompanyID, Settings.ProjectID, Settings.JobID, 0, 0, ConfigSelectedRule.ID,
                                ConfigSelectedFromLoc.Name, ConfigSelectedEventRemark.ID, ConfigSelectedSataus);

                            if (data?.status == 1)
                            {
                                SelectedScanRuleHeader = ConfigSelectedRule.Name;
                                ScanSelectedFromLoc = ConfigSelectedFromLoc;
                                ScanSelectedEventRemark = ConfigSelectedEventRemark;

                                if (ConfigSelectedSataus == ScanConfigResult.data.PolyboxStatus[0].ID)
                                {
                                    ScanIsEmpty = IsEmpty = true;
                                }
                                else if (ConfigSelectedSataus == ScanConfigResult.data.PolyboxStatus[1].ID)
                                {
                                    ScanIsFull = IsFull = true;
                                }

                                IsScanEnable = true;
                                ScanOpacity = 1;
                                TabChange("scan");
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        }
                    }
                    else
                    {
                        IsRuleError = (ConfigSelectedRule == null ||
                            ConfigSelectedRule?.ID == 0) ? true : false;
                        IsLocError = (ConfigSelectedFromLoc == null ||
                            string.IsNullOrEmpty(ConfigSelectedFromLoc?.Name)) ? true : false;
                        IsRemarkError = (ConfigSelectedEventRemark == null ||
                            ConfigSelectedEventRemark?.ID == 0) ? true : false;
                        IsStatusError = (ConfigSelectedSataus == null
                            || ConfigSelectedSataus == 0) ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveConfig method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        private async Task StartScanning()
        {
            try
            {
                loadindicator = true;
                if ((ScanSelectedFromLoc?.Name == "GPS Location" && IsGPSCorVisible == false))
                {
                    var status = await Xamarin.Essentials.Permissions.RequestAsync
                        <Xamarin.Essentials.Permissions.LocationWhenInUse>();

                    if (status != Xamarin.Essentials.PermissionStatus.Granted)
                    {
                        var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needed to access the location.", null, null, "Maybe later", "Settings");
                        switch (checkSelect)
                        {
                            case "Maybe later":
                                return;
                                break;
                            case "Settings":
                                CrossPermissions.Current.OpenAppSettings();
                                return;
                                break;
                        }
                    }
                    else
                    {
                        ScanSelectedFromLoc = ScanConfigResult.data.PolyboxLocation?.Where(wr => wr.Name == "GPS Location").FirstOrDefault();
                    }
                }

                var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                var requestedPermissionStatus = requestedPermissions[Permission.Camera];
                var pass1 = requestedPermissions[Permission.Camera];

                if (pass1 == PermissionStatus.Denied)
                {
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
                    try
                    {
                        if (Settings.MobileScanProvider.Trim().ToLower() == "scandit".ToLower())
                        {
                            ScanerSettings scanset = new ScanerSettings();
                            SettingsArchiver.ArchiveSettings(scanset);
                            await Navigation.PushModalAsync(new ScannerPage(scanset, this));
                        }
                        else
                        {
                            await ZxingScanner();
                        }
                    }
                    catch (Exception ex1)
                    {
                        YPSLogger.ReportException(ex1, "StartScanning method while navigate-> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                        var trackResult = trackService.Handleexception(ex1);
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable.", "Ok");
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

        async Task ZxingScanner()
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
                        if (!string.IsNullOrEmpty(scanresult.Text))
                        {
                            await Scanditscan(scanresult.Text);
                            await Navigation.PopAsync(false);
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
                YPSLogger.ReportException(ex, "ZxingScanner method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
        }

        private async Task DoneClick()
        {
            try
            {
                loadindicator = true;

                if (IsVerifiedDataVisible == false)
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Scan Barcode/QRcode to validate before saving.", "Ok");
                }
                else
                {
                    await SaveScanData("done");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DoneClick method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        private async Task NextScan()
        {
            try
            {
                loadindicator = true;

                if (IsVerifiedDataVisible == false)
                {
                    await StartScanning();
                }
                else
                {
                    await SaveScanData("nextscan");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NextScan method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                var trackResult = trackService.Handleexception(ex);
            }
            finally
            {
                loadindicator = false;
            }
        }

        private async Task SaveScanData(string savefrom = "")
        {
            try
            {
                loadindicator = true;

                PolyBoxModel savepolyboxscan = new PolyBoxModel();

                savepolyboxscan.CompanyID = Settings.PBCompanyID;
                savepolyboxscan.ProjectID = Settings.PBProjectID;
                savepolyboxscan.JobID = Settings.PBJobID;
                savepolyboxscan.CargoCategory1 = CargoCategory;
                savepolyboxscan.BagNumber = BagNumber;
                savepolyboxscan.TQB_PkgSizeNo_L1 = TQBPkgSizeNoL1;
                savepolyboxscan.EventDT_L1 = ScannedDateTime;
                savepolyboxscan.UserID = Settings.userLoginID;
                savepolyboxscan.Attributes = ScanIsEmpty == true ?
                    ScanConfigResult?.data?.PolyboxStatus[0]?.Name :
                    ScanConfigResult?.data?.PolyboxStatus[1]?.Name;
                savepolyboxscan.FromLoc_L1 = ScanSelectedFromLoc?.Name;
                savepolyboxscan.EventRemarks_L1 = ScanSelectedEventRemark?.Name;
                savepolyboxscan.Remarks_Description = ScanRemarkDesc;
                savepolyboxscan.Location_Details = ScanLocText;
                savepolyboxscan.TotalPolyboxCount = TotalPolyboxCountHeader;
                savepolyboxscan.TotalScannedToday = TotalScannedToday;
                savepolyboxscan.ISR = ISRHeader;
                savepolyboxscan.TagNumber = TagNumber;

                var result = await trackService.SavePolyboxScanData(savepolyboxscan);

                if (result?.status == 1)
                {
                    await GetPolyboxHeaderData();

                    if (savefrom.Trim().ToLower() == "nextscan".Trim().ToLower())
                    {
                        await StartScanning();
                    }
                    else if (savefrom.Trim().ToLower() == "done".Trim().ToLower())
                    {
                        IsPrintEnabled = IsVerifiedDataVisible = IsNoRecordsVisible = false;
                        ScannedBy = ScannedDateTime = "";
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "Scan data did not got saved.", "Ok");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveScanData method -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
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

                Stream ngsr = assembly.GetManifestResourceStream("YPS." + "ngbeep.mp3");
                ISimpleAudioPlayer ngplaybeep = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
                ngplaybeep.Load(ngsr);

                Device.BeginInvokeOnMainThread(async () =>
                {

                    string sp = "\n\n";
                    var scanvalue = scanresult;

                    if (!string.IsNullOrEmpty(scanvalue))
                    {
                        var checkInternet = await App.CheckInterNetConnection();

                        if (checkInternet)
                        {
                            var result = await trackService.PolyboxScanValidation(scanvalue);

                            if (result?.status == 1)
                            {
                                okplaybeep.Play();

                                CargoCategory = result?.data?.CargoCategory1;
                                BagNumber = result?.data?.BagNumber;
                                TQBPkgSizeNoL1 = result?.data?.TQB_PkgSizeNo_L1;
                                TagNumber = scanvalue;

                                ScannedDateTime = String.Format(Settings.DateFormat, DateTime.Now);
                                ScannedBy = Settings.Username;

                                IsNoRecordsVisible = false;
                                IsPrintEnabled = IsVerifiedDataVisible = true;
                            }
                            else
                            {
                                IsNoRecordsVisible = true;
                                IsPrintEnabled = IsVerifiedDataVisible = false;
                                ngplaybeep.Play();
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Scanning", "Got no value from the code.", "Ok");
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
                    IsScanContentVisible = ScanTabVisibility = false;
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
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.PBVersionID && wr.LanguageID == Settings.LanguageID).ToList();

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

                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " : ";
                        labelobj.CargoCategory.Name = (cargocategory1 != null ? (!string.IsNullOrEmpty(cargocategory1.LblText) ? cargocategory1.LblText : labelobj.CargoCategory.Name) : labelobj.CargoCategory.Name) + " : ";
                        labelobj.TQBPkgSizeNoL1.Name = (tqbpkgsizenol1 != null ? (!string.IsNullOrEmpty(tqbpkgsizenol1.LblText) ? tqbpkgsizenol1.LblText : labelobj.TQBPkgSizeNoL1.Name) : labelobj.TQBPkgSizeNoL1.Name) + " : ";

                        labelobj.Scan.Name = scan != null ? (!string.IsNullOrEmpty(scan.LblText) ? scan.LblText : labelobj.Scan.Name) : labelobj.Scan.Name;
                        labelobj.Configure.Name = configure != null ? (!string.IsNullOrEmpty(configure.LblText) ? configure.LblText : labelobj.Configure.Name) : labelobj.Configure.Name;

                        labelobj.Save.Name = save != null ? (!string.IsNullOrEmpty(save.LblText) ? save.LblText : labelobj.Save.Name) : labelobj.Save.Name;
                        labelobj.PrintTag.Name = (printtag != null ? (!string.IsNullOrEmpty(printtag.LblText) ? printtag.LblText : labelobj.PrintTag.Name) : labelobj.Next.Name) + " : ";
                        labelobj.Next.Name = (next != null ? (!string.IsNullOrEmpty(next.LblText) ? next.LblText : labelobj.Next.Name) : labelobj.Next.Name) + " " + labelobj.Scan.Name;
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
            public DashboardLabelFields PrintTag { get; set; } = new DashboardLabelFields { Status = true, Name = "LCMPrint" };
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
                
        private bool _IsPrintEnabled;
        public bool IsPrintEnabled
        {
            get => _IsPrintEnabled;
            set
            {
                _IsPrintEnabled = value;
                RaisePropertyChanged("IsPrintEnabled");
            }
        }

        private double _PrintIconOpacity;
        public double PrintIconOpacity
        {
            get => _PrintIconOpacity;
            set
            {
                _PrintIconOpacity = value;
                NotifyPropertyChanged("PrintIconOpacity");
            }
        }

        private Color _PrintFieldBorderColor = Color.Transparent;
       
        private string _ScanRemarkDesc;
        public string ScanRemarkDesc
        {
            get => _ScanRemarkDesc;
            set
            {
                _ScanRemarkDesc = value;
                NotifyPropertyChanged("ScanRemarkDesc");
            }
        }

        private string _TagNumber;
        public string TagNumber
        {
            get => _TagNumber;
            set
            {
                _TagNumber = value;
                NotifyPropertyChanged("TagNumber");
            }
        }

        private bool _IsNoRecordsVisible;
        public bool IsNoRecordsVisible
        {
            get => _IsNoRecordsVisible;
            set
            {
                _IsNoRecordsVisible = value;
                RaisePropertyChanged("IsNoRecordsVisible");
            }
        }

        private bool _IsVerifiedDataVisible;
        public bool IsVerifiedDataVisible
        {
            get => _IsVerifiedDataVisible;
            set
            {
                _IsVerifiedDataVisible = value;
                RaisePropertyChanged("IsVerifiedDataVisible");
            }
        }

        private ScanConfigResponse _ScanConfigResult = new ScanConfigResponse();
        public ScanConfigResponse ScanConfigResult
        {
            get => _ScanConfigResult;
            set
            {
                _ScanConfigResult = value;
                NotifyPropertyChanged("ScanConfigResult");
            }
        }

        private bool _IsGPSCorVisible;
        public bool IsGPSCorVisible
        {
            get => _IsGPSCorVisible;
            set
            {
                _IsGPSCorVisible = value;
                NotifyPropertyChanged("IsGPSCorVisible");
            }
        }

        private bool _IsPrintError;
        public bool IsPrintError
        {
            get => _IsPrintError;
            set
            {
                _IsPrintError = value;
                NotifyPropertyChanged("IsPrintError");
            }
        }

        private bool _IsRuleError;
        public bool IsRuleError
        {
            get => _IsRuleError;
            set
            {
                _IsRuleError = value;
                NotifyPropertyChanged("IsRuleError");
            }
        }

        private bool _IsLocError;
        public bool IsLocError
        {
            get => _IsLocError;
            set
            {
                _IsLocError = value;
                NotifyPropertyChanged("IsLocError");
            }
        }

        private bool _IsRemarkError;
        public bool IsRemarkError
        {
            get => _IsRemarkError;
            set
            {
                _IsRemarkError = value;
                NotifyPropertyChanged("IsRemarkError");
            }
        }

        private bool _IsStatusError;
        public bool IsStatusError
        {
            get => _IsStatusError;
            set
            {
                _IsStatusError = value;
                NotifyPropertyChanged("IsStatusError");
            }
        }

        private string _ScannedBy;
        public string ScannedBy
        {
            get => _ScannedBy;
            set
            {
                _ScannedBy = value;
                NotifyPropertyChanged("ScannedBy");
            }
        }

        private string _ScannedDateTime;
        public string ScannedDateTime
        {
            get => _ScannedDateTime;
            set
            {
                _ScannedDateTime = value;
                NotifyPropertyChanged("ScannedDateTime");
            }
        }

        private string _ScanLocText;
        public string ScanLocText
        {
            get => _ScanLocText;
            set
            {
                _ScanLocText = value;
                NotifyPropertyChanged("ScanLocText");
            }
        }

        private YPS.Model.CompareModel _ConfigSelectedRule = new Model.CompareModel();
        public YPS.Model.CompareModel ConfigSelectedRule
        {
            get => _ConfigSelectedRule;
            set
            {
                _ConfigSelectedRule = value;
                NotifyPropertyChanged("ConfigSelectedRule");

                if (IsRuleError == true && value.ID > 0)
                {
                    IsRuleError = false;
                }
            }
        }

        private YPS.Model.CompareModel _ConfigSelectedFromLoc = new Model.CompareModel();
        public YPS.Model.CompareModel ConfigSelectedFromLoc
        {
            get => _ConfigSelectedFromLoc;
            set
            {
                _ConfigSelectedFromLoc = value;
                NotifyPropertyChanged("ConfigSelectedFromLoc");

                if (IsLocError == true && !string.IsNullOrEmpty(value.Name))
                {
                    IsLocError = false;
                }
            }
        }

        private YPS.Model.CompareModel _ConfigSelectedEventRemark = new Model.CompareModel();
        public YPS.Model.CompareModel ConfigSelectedEventRemark
        {
            get => _ConfigSelectedEventRemark;
            set
            {
                _ConfigSelectedEventRemark = value;
                NotifyPropertyChanged("ConfigSelectedEventRemark");

                if (IsRemarkError == true && value.ID > 0)
                {
                    IsRemarkError = false;
                }
            }
        }

        private int _ConfigSelectedSataus;
        public int ConfigSelectedSataus
        {
            get => _ConfigSelectedSataus;
            set
            {
                _ConfigSelectedSataus = value;
                NotifyPropertyChanged("ConfigSelectedSataus");

                if (IsStatusError == true && value > 0)
                {
                    IsStatusError = false;
                }
            }
        }

        private YPS.Model.CompareModel _ScanSelectedFromLoc = new Model.CompareModel();
        public YPS.Model.CompareModel ScanSelectedFromLoc
        {
            get => _ScanSelectedFromLoc;
            set
            {
                _ScanSelectedFromLoc = value;
                NotifyPropertyChanged("ScanSelectedFromLoc");

                if (value?.Name == "GPS Location")
                {
                    try
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            var status = await Xamarin.Essentials.Permissions.RequestAsync
                            <Xamarin.Essentials.Permissions.LocationWhenInUse>();

                            if (status == Xamarin.Essentials.PermissionStatus.Granted)
                            {
                                var locval = await Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
                                ScanLocText = locval?.Latitude.ToString() + ", " + locval?.Longitude.ToString();
                                IsGPSCorVisible = true;
                            }
                            else
                            {
                                var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs to access Location.", null, null, "Maybe Later", "Settings");

                                switch (checkSelect)
                                {
                                    case "Maybe Later":
                                        return;
                                        break;
                                    case "Settings":
                                        CrossPermissions.Current.OpenAppSettings();
                                        return;
                                        break;
                                }
                            }

                        });
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "ScanSelectedFromLoc property -> in PolyBoxViewModel " + YPS.CommonClasses.Settings.userLoginID);
                        var trackResult = trackService.Handleexception(ex);
                    }
                }
                else
                {
                    ScanLocText = ScanSelectedFromLoc?.Name;
                    IsGPSCorVisible = false;
                }
            }
        }

        private YPS.Model.CompareModel _ScanSelectedEventRemark = new Model.CompareModel();
        public YPS.Model.CompareModel ScanSelectedEventRemark
        {
            get => _ScanSelectedEventRemark;
            set
            {
                _ScanSelectedEventRemark = value;
                NotifyPropertyChanged("ScanSelectedEventRemark");
            }
        }

        private bool _ScanIsEmpty;
        public bool ScanIsEmpty
        {
            get => _ScanIsEmpty;
            set
            {
                _ScanIsEmpty = value;
                RaisePropertyChanged("ScanIsEmpty");
            }
        }

        private bool _ScanIsFull;
        public bool ScanIsFull
        {
            get => _ScanIsFull;
            set
            {
                _ScanIsFull = value;
                RaisePropertyChanged("ScanIsFull");
            }
        }

        private bool _IsEmpty;
        public bool IsEmpty
        {
            get => _IsEmpty;
            set
            {
                _IsEmpty = value;
                RaisePropertyChanged("IsEmpty");
            }
        }

        private bool _IsFull;
        public bool IsFull
        {
            get => _IsFull;
            set
            {
                _IsFull = value;
                RaisePropertyChanged("IsFull");
            }
        }

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
                RaisePropertyChanged("TQBPkgSizeNoL1");
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

        private string _TotalPolyboxCountHeader;
        public string TotalPolyboxCountHeader
        {
            get => _TotalPolyboxCountHeader;
            set
            {
                _TotalPolyboxCountHeader = value;
                NotifyPropertyChanged("TotalPolyboxCountHeader");
            }
        }

        private string _TotalScannedToday;
        public string TotalScannedToday
        {
            get => _TotalScannedToday;
            set
            {
                _TotalScannedToday = value;
                NotifyPropertyChanged("TotalScannedToday");
            }
        }

        private string _ISRHeader;
        public string ISRHeader
        {
            get => _ISRHeader;
            set
            {
                _ISRHeader = value;
                NotifyPropertyChanged("ISRHeader");
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
