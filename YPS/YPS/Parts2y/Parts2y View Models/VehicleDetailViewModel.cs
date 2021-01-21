using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using SignaturePad.Forms;
using Syncfusion.ListView.XForms;
using Syncfusion.ListView.XForms.Control.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_Services;
using YPS.Parts2y.Parts2y_SQLITE;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class VehicleDetailViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand ScanCommand { get; set; }
        public ICommand PDICommand { get; set; }
        public ICommand LoadCommand { get; set; }
        public ICommand QrCodeCommand { get; set; }
        public ICommand VerifyCommand { get; set; }
        public ICommand HomeCommand { get; set; }
        public ICommand SelecpicCmd { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand tap_OnImge { set; get; }
        public ICommand UploadCmd { get; set; }

        public ICommand ViewAllCommand { get; set; }

        public ICommand NextCommand { get; set; }
        public ICommand AuditorSignatureCommand { get; set; }
        public ICommand SupervisorSignatureCommand { get; set; }
        public ICommand CrossTapCommand { get; set; }
        public ICommand OkaytogoCommand { get; set; }
        public Command<object> QuestionNo_Tapped { get; set; }

        public List<VehicleDetailsModel> result { get; set; }

        private string _photoCount = "0";
        public string photoCount
        {
            get => _photoCount;
            set
            {
                _photoCount = value;
                OnPropertyChanged("photoCount");
            }
        }

        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator; set
            {
                _loadindicator = value;
                OnPropertyChanged("loadindicator");
            }
        }
        public bool isallQuestionsAttended { get; set; } = false;
        //public Command<object> TapGestureCommand { get; set; }

        public ObservableCollection<CPQuestionsdata> cPQuestionsdata = new ObservableCollection<CPQuestionsdata>();
        RestClient service = new RestClient();
        //public List<QuestionDetailsModel> QuestionsListitems { get; set; }
        //private CPQuestionsdata TappedItem;
        //internal SfListView listView;
        //public string StatusCheck = string.Empty;
        public VehicleDetailViewModel(INavigation _Navigation)
        {
            try
            {

                Navigation = _Navigation;
                BgColor = Settings.Bar_Background;
                NextBtnBgColor = Settings.Bar_Background;
                //cPQuestionsdata = new ObservableCollection<CPQuestionsdata>();
                ScanCommand = new Command(async () => await Scan_Tap());
                PDICommand = new Command(async () => await PDI_Tap());
                LoadCommand = new Command(async () => await Load_Tap());
                QrCodeCommand = new Command(async () => await QRCode_Tap());
                //VerifyCommand = new Command(async () => await Verify_Tap());
                HomeCommand = new Command(async () => await Home_Tap());
                //SelecpicCmd = new Command(async () => await Select_Pic());
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                UploadCmd = new Command(async () => await Upload_Pic());
                AuditorSignatureCommand = new Command(async () => await AuditorSignature_Tapped());
                SupervisorSignatureCommand = new Command(async () => await SupervisorSignature_Tapped());
                CrossTapCommand = new Command(async () => await CrossSignaturePadTapped());
                OkaytogoCommand = new Command(async () => await OkayToGo_Tap());
                //ViewAllCommand= new Command(async () => await Viewall_Tap());
                //NextCommand=new Command(async () => await Next_Tap());
                tap_OnImge = new Command(image_tap);

                QuestionNo_Tapped = new Command<object>(QuestionNo_Tap);

                //GetQuestionarieDetils();
                //     TapGestureCommand = new Command<object>(TappedGestureCommandMethod);

                Getvindetails();

            }
            catch (Exception ex)
            {

            }

        }

        private void QuestionNo_Tap(object obj)
        {
            try
            {
                loadindicator = true;

                var tappedItemData = obj as CPQuestionsdata;

                if (tappedItemData != null)
                {
                    PDI_Tap();
                    //LoadFuncVisibility = false;
                    //PDIFuncVisibility = true;
                    Settings.QNo_Sequence = tappedItemData.Seqno;
                    EPODPhotoUplodeViewModel vmEPODPhoto = new EPODPhotoUplodeViewModel();
                    photoCount = Convert.ToString(vmEPODPhoto.photolist.Count);

                    ExpandVisibility = true;
                    Settings.QNo_Sequence = tappedItemData.Seqno;
                    NextBtnBgColor = ((!string.IsNullOrEmpty(Settings.PDICompleted)) && Settings.QNo_Sequence == QuestionInfo.Count) ? Color.LightGray : Settings.Bar_Background;
                    Next_Text = (Settings.QNo_Sequence == QuestionInfo.Count) ? "SAVE" : "NEXT";
                    Question_No = tappedItemData.Seqno;
                    Qestion_Title = tappedItemData.PDI_QestionTitle;
                    OptionsList = tappedItemData.listOptions;
                    if (!string.IsNullOrEmpty(tappedItemData.AnsweredRemarks))
                    {
                        RemarksText = tappedItemData.AnsweredRemarks;
                    }
                    else
                    {
                        RemarksText = "";
                        if (!string.IsNullOrEmpty(tappedItemData.Remarks))
                        {
                            PlaceholderText = tappedItemData.Remarks;
                        }
                        else
                        {
                            PlaceholderText = "REMARKS";

                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                loadindicator = false;
            }
        }




        //private async Task Next_Tap()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception ex)
        //    {


        //    }
        //}

        //private async Task Viewall_Tap()
        //{
        //    try
        //    {

        //        GetQuestionarieDetils();
        //        ExpandVisibility = false;
        //        QuestionListVisibility = true;
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public async Task Backevnttapped_click()
        {
            try
            {
                Navigation.PopModalAsync();

            }
            catch (Exception ex)
            {
            }
        }

        public async Task Upload_Pic()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }



        private async void image_tap(object obj)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }
        public async void GetQuestionarieDetils()
        {
            try
            {
                var poData = await service.GetQuestionarieeDetails();
                var result = JsonConvert.DeserializeObject<QuestionarieeList>(poData.ToString());
                if (result != null)
                {
                    if (result.status != 0)
                    {
                        foreach (var items in result.CPQuestionsslist.CPQuestionsdata)
                        {
                            items.RightSwipeText = items.listOptions[0].Observation;
                            items.RightSwipeSelectedIndex = items.listOptions[0].ObsID;
                        }
                        cPQuestionsdata = result.CPQuestionsslist.CPQuestionsdata;
                        QuestionInfo = cPQuestionsdata;
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        public async void Getvindetails()
        {
            try
            {
                loadindicator = true;

                SupTransportDB vehicleDetailDB = new SupTransportDB("vedetails");
                var poData = string.Empty;
                var current = Connectivity.NetworkAccess;
                result = new List<VehicleDetailsModel>();

                if (current == NetworkAccess.Internet)
                {
                    poData = await service.GetSupvindetails(Settings.VINNo);
                    var val = JsonConvert.DeserializeObject<VehicleDetailsModel>(poData.ToString());
                    result.Add(val);
                    vehicleDetailDB.SaveAndUpdateAllVeData(result);
                }
                else
                {
                    result = vehicleDetailDB.GetAllVeData();
                }

                if (result.Count > 0)
                {
                    if (result[0].status != false)
                    {
                        Model = result[0].Vindata.Modelname;
                        VIN_NO = Settings.VINNo = result[0].Vindata.Vin;
                        Colour = result[0].Vindata.Colour;
                        CarrierNo = result[0].Vindata.Carrier;
                        Settings.PDICompleted = result[0].Vindata.PDICompleted;
                        Settings.ScanCompleted = result[0].Vindata.ScanCompleted;
                        Settings.LoadCompleted = result[0].Vindata.Load;
                        StatusRslt = result[0].Vindata.ScanStatus;

                        if (Settings.VINNo == "MA3NFG81SJC-191208")
                        {
                            scannedOn = DateTime.Now.ToString();
                            StatusBg = Color.FromHex("#DB0000");
                            ScanTickBgColor = Color.FromHex("#DB0000");
                            ScanStatusSource = Icons.CircleWrong;
                            ScanTickVisibility = true;
                            QrCodeCompleteVisibility = true;
                            QrCodeButtonVisibility = false;
                            QrCodeImageVisibility = false;
                            QrCodeImageWithDetailsVisibility = true;
                            ScanBGColor = Color.FromHex("#E9E9E9");
                        }

                        if (result[0].Vindata.ScanCompleted == "1" && result[0].Vindata.ScanStatus == "Verified")
                        {

                            scannedOn = result[0].Vindata.ScanOn;
                            StatusBg = Color.FromHex("#005800");
                            ScanTickBgColor = Color.FromHex("#005800");
                            ScanStatusSource = Icons.CheckCircle;
                            ScanTickVisibility = true;
                            QrCodeCompleteVisibility = false;
                            QrCodeButtonVisibility = false;
                            QrCodeImageVisibility = false;
                            QrCodeImageWithDetailsVisibility = true;
                            ScanBGColor = Color.FromHex("#E9E9E9");
                        }

                        if (!string.IsNullOrEmpty(result[0].Vindata.PDICompleted))
                        {
                            PDIStatusSource = Icons.CheckCircle;
                            PDITickVisibility = true;
                            PDIBgColor = Color.FromHex("#E9E9E9");

                            //#region need to confirm this functionality
                            //OkToLoadColor = Settings.Bar_Background;
                            //OkayToGoEnable = true;
                            //#endregion

                            if (result[0].signatureAuditBase64 != null && result[0].signatureSupervisorBase64 != null)
                            {
                                AuditorImageSign = ImageSource.FromStream(() => new MemoryStream(result[0].signatureAuditBase64));
                                SupervisorImageSign = ImageSource.FromStream(() => new MemoryStream(result[0].signatureSupervisorBase64));
                            }
                            else
                            {
                                AuditorImageSign = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/a/ac/Chris_Hemsworth_Signature.png"));
                                SupervisorImageSign = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/a/a1/Signature_of_Andrew_Scheer.png"));
                            }
                        }

                        if (!string.IsNullOrEmpty(result[0].Vindata.Load))
                        {
                            LoadStatusSource = Icons.CheckCircle;
                            LoadTickVisibility = true;
                            LoadBgColor = Color.FromHex("#E9E9E9");

                            #region need to confirm
                            OkToLoadColor = Color.LightGray;
                            OkayToGoEnable = false;
                            isallQuestionsAttended = true;
                            #endregion 

                            //AuditorImageSign = ImageSource.FromStream(() => new MemoryStream(result[0].signatureAuditBase64));
                            //SupervisorImageSign = ImageSource.FromStream(() => new MemoryStream(result[0].signatureSupervisorBase64));

                            if (result[0].signatureAuditBase64 != null && result[0].signatureSupervisorBase64 != null)
                            {
                                AuditorImageSign = ImageSource.FromStream(() => new MemoryStream(result[0].signatureAuditBase64));
                                SupervisorImageSign = ImageSource.FromStream(() => new MemoryStream(result[0].signatureSupervisorBase64));
                            }
                            else
                            {
                                AuditorImageSign = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/a/ac/Chris_Hemsworth_Signature.png"));
                                SupervisorImageSign = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/a/a1/Signature_of_Andrew_Scheer.png"));
                            }
                        }

                        //foreach (var item in result.Vindata)
                        //{
                        //    VIN_NO = item.Vin;
                        //    Colour = item.Colour;
                        //    CarrierNo = item.Carrier;
                        //}
                        //foreach(var items in result.ModelData)
                        //{
                        //    Model = items.ModelName;
                        //}
                        foreach (var items in result[0].cpquestions)
                        {
                            items.RightSwipeText = items.listOptions[0].Observation;
                            items.RightSwipeSelectedIndex = items.listOptions[0].ObsID;
                            var itemlist = items.listOptions.Where(x => x.Isanswered).ToList();

                            if (itemlist.Count != 0)
                            {
                                items.NotAnsweredBgColor = Color.FromHex("#005800"); ;
                                items.AnsweredQstnBgColor = Settings.Bar_Background;
                                items.isAnswered = true;
                                AllowSwipingVisible = false;

                            }
                            else
                            {
                                items.NotAnsweredBgColor = Color.Black; ;
                                items.AnsweredQstnBgColor = Color.White;
                                items.isAnswered = false;
                                AllowSwipingVisible = true;
                            }
                        }

                        cPQuestionsdata = result[0].cpquestions;
                        QuestionInfo = cPQuestionsdata;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "No data available", "OK");
                        await Navigation.PopModalAsync();
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "No data available", "OK");
                    await Navigation.PopModalAsync();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                loadindicator = false;
            }
        }
        public void TappedGestureCommandMethod(object obj)
        {
            try
            {
                loadindicator = true;

                var tappedItemData = obj as CPQuestionsdata;

                if (tappedItemData != null)
                {
                    // PDIFuncVisibility = true;
                    // ScanFuncVisibility = LoadFuncVisibility = false;
                    QuestionListVisibility = false;
                    ExpandVisibility = true;
                    Settings.QNo_Sequence = tappedItemData.Seqno;
                    Question_No = tappedItemData.Seqno;
                    Qestion_Title = tappedItemData.PDI_QestionTitle;
                    OptionsList = tappedItemData.listOptions;
                    if (!string.IsNullOrEmpty(tappedItemData.AnsweredRemarks))
                    {
                        RemarksText = tappedItemData.AnsweredRemarks;
                    }
                    else
                    {
                        RemarksText = "";
                        if (!string.IsNullOrEmpty(tappedItemData.Remarks))
                        {
                            PlaceholderText = tappedItemData.Remarks;
                        }
                        else
                        {
                            PlaceholderText = "REMARKS";

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                loadindicator = false;
            }
        }
        public async Task Load_Tap()
        {
            try
            {
                //if (Settings.ScanCompleted == "1" )
                if (Settings.ScanCompleted == "1" && isallQuestionsAttended)
                {
                    LoadFuncVisibility = true;
                    PDIFuncVisibility = ScanFuncVisibility = false;
                    //LoadTextColor = Color.Black;
                    //PDITextColor = ScanTextColor = Color.Gray;
                    LoadTabVisibility = false;
                    PDITabVisibility = ScanTabVisibility = true;
                    LoadBorderColor = Color.FromHex("#000000");
                    PDIBorderColor = ScanBorderColor = Color.Transparent;
                    //LoadBorderVisibility = true;
                    //PDIBorderVisibility = ScanBorderVisibility = false;
                    LoadBgColor = Color.FromHex("#E9E9E9");
                    //ScanBGColor = PDIBgColor = Color.LightGray;
                }
            }
            catch (Exception ex)
            {


            }
        }

        public async Task PDI_Tap()
        {
            try
            {
                if (Settings.ScanCompleted == "1")
                {
                    PDITabVisibility = false;
                    LoadTabVisibility = ScanTabVisibility = true;
                    PDIFuncVisibility = true;
                    ScanFuncVisibility = LoadFuncVisibility = false;
                    PDIBorderColor = Color.FromHex("#000000");
                    LoadBorderColor = ScanBorderColor = Color.Transparent;
                    //PDIBorderVisibility = true;
                    //ScanBorderVisibility = LoadBorderVisibility = false;
                    PDIBgColor = Color.FromHex("#E9E9E9");
                    //ScanBGColor = LoadBgColor = Color.LightGray;
                    //QuestionInfo = QuestionsListitems;
                }

            }
            catch (Exception ex)
            {

            }
        }
        private async Task QRCode_Tap()
        {
            try
            {
                loadindicator = true;

                var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                var requestedPermissionStatus = requestedPermissions[Permission.Camera];
                var pass1 = requestedPermissions[Permission.Camera];

                if (pass1 == Plugin.Permissions.Abstractions.PermissionStatus.Denied)
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
                else if (pass1 == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var ScannerPage = new ZXingScannerPage();


                    ScannerPage.OnScanResult += (scanresult) =>
                    {
                        // Parar de escanear
                        ScannerPage.IsScanning = false;

                        // Alert com o código escaneado
                        Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PopModalAsync();
                                if (result != null)
                                {
                                    App.Current.MainPage.DisplayAlert("Alert", scanresult.Text, "OK");
                                    ScannedValue = scanresult.Text;

                                    if (Settings.VINNo == "MA3NFG81SJC-191208")
                                    {
                                        Settings.ScanCompleted = "0";
                                        StatusRslt = "Not Matched";
                                    }
                                    else
                                    {
                                        Settings.ScanCompleted = "1";
                                        StatusRslt = "Verified";
                                    }

                                    #region added by Rameez to save the scan status in local DB
                                    result[0].Vindata.ScanCompleted = Settings.ScanCompleted;
                                    result[0].Vindata.ScanStatus = StatusRslt;
                                    SupTransportDB superDB = new SupTransportDB("vedetails");
                                    superDB.UpdateAllVeData(result);
                                    #endregion

                                    Scanned_Format = scanresult.BarcodeFormat;
                                    QrCodeButtonVisibility = ScanTickVisibility = false;
                                    QrCodeImageVisibility = QrCodeImageWithDetailsVisibility = true;
                                    verifying();
                                    //PDI_Tap();
                                }
                                else
                                {
                                    QrCodeButtonVisibility = true;
                                    QrCodeImageVisibility = QrCodeImageWithDetailsVisibility = ScanTickVisibility = false;
                                }
                            });
                    };
                    if (Navigation.ModalStack.Count == 0 ||
                                        Navigation.ModalStack.Last().GetType() != typeof(ZXingScannerPage))
                    {
                        await Navigation.PushModalAsync(ScannerPage);
                    }
                }
                else
                {
                    //await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to scan, Please allow the camera permission in App Permission settings", "Ok");
                    await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");

                }
                //}
                //else
                //{
                //    await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to scan, Please allow the camera permission in App Permission settings", "Ok");
                //}
                //}
                //else
                //{
                //    await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to scan, Please allow the permission in App Permission settings", "Ok");
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

        private async Task Scan_Tap()
        {
            try
            {
                //if (StatusRslt != "Verified")
                //{
                if (StatusRslt == "Verified" && Settings.ScanCompleted == "1" && !string.IsNullOrEmpty(Settings.PDICompleted))
                {
                    PDIFuncVisibility = LoadFuncVisibility = false;
                    ScanFuncVisibility = true;
                    //ScanTextColor = Color.Black;
                    //PDITextColor = LoadTextColor = Color.Gray;
                    ScanTabVisibility = false;
                    PDITabVisibility = LoadTabVisibility = true;
                    ScanBorderColor = Color.FromHex("#000000");
                    LoadBorderColor = PDIBorderColor = Color.Transparent;
                    //ScanBorderVisibility = true;
                    //LoadBorderVisibility = PDIBorderVisibility = false;
                    ScanBGColor = Color.FromHex("#E9E9E9");
                }
                else
                {
                    ScanBGColor = Color.LightGray;
                }




            }
            catch (Exception ex)
            {

            }
        }

        private async Task verifying()
        {
            try
            {

                scannedOn = DateTime.Now.ToString();
                StatusBg = (StatusRslt == "Verified") ? Color.FromHex("#005800") : Color.FromHex("#DB0000");
                ScanStatusSource = (StatusRslt == "Verified") ? Icons.CheckCircle : Icons.CircleWrong;
                ScanTickBgColor = (StatusRslt == "Verified") ? Color.FromHex("#005800") : Color.FromHex("#DB0000");
                Settings.isScanCompleted = ScanTickVisibility = true;
                QrCodeCompleteVisibility = true;
                QrCodeButtonVisibility = false;
                QrCodeImageVisibility = false;
                QrCodeImageWithDetailsVisibility = true;
            }
            catch (Exception ex)
            {

            }
        }

        private async Task Home_Tap()
        {
            try
            {
                App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {

            }
        }

        public async Task AuditorSignature_Tapped()
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.LoadCompleted))
                {

                    Signature_Person = "Auditor's Signature";
                    SignaturePadPopup = true;
                }

            }
            catch (Exception ex)
            {
            }
        }

        public async Task SupervisorSignature_Tapped()
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.LoadCompleted))
                {

                    Signature_Person = "Supervisor's Signature";
                    SignaturePadPopup = true;
                }
            }
            catch (Exception ex)
            {
            }
        }
        public async Task CrossSignaturePadTapped()
        {
            try
            {
                SignaturePadPopup = false;


            }
            catch (Exception ex)
            {
            }
        }
        public async Task OkayToGo_Tap()
        {
            try
            {
                loadindicator = true;

                #region need to confirm
                SupTransportDB veDetailsDB = new SupTransportDB("vedetails");
                veDetailsDB.UpdateAllVeData(result);
                PDITickVisibility = true;
                Settings.PDICompleted = DateTime.Now.ToString("h:mm");
                Settings.LoadCompleted = DateTime.Now.ToString("h:mm");
                Settings.isPDICompleted = Settings.isLoadCompleted = LoadTickVisibility = true; //need to confirm regarding Settings.isPDICompleted 
                #endregion

                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                loadindicator = false;
            }
        }

        #region properties
        private bool _ScanTabVisibility = false;
        public bool ScanTabVisibility
        {
            get => _ScanTabVisibility;
            set
            {
                _ScanTabVisibility = value;
                OnPropertyChanged("ScanTabVisibility");
            }
        }

        private bool _PDITabVisibility = true;
        public bool PDITabVisibility
        {
            get => _PDITabVisibility;
            set
            {
                _PDITabVisibility = value;
                OnPropertyChanged("PDITabVisibility");
            }
        }
        private bool _LoadTabVisibility = true;
        public bool LoadTabVisibility
        {
            get => _LoadTabVisibility;
            set
            {
                _LoadTabVisibility = value;
                OnPropertyChanged("LoadTabVisibility");
            }
        }
        private Color _ScanBorderColor = Color.FromHex("#000000");
        public Color ScanBorderColor
        {
            get => _ScanBorderColor;
            set
            {
                _ScanBorderColor = value;
                OnPropertyChanged("ScanBorderColor");
            }
        }

        private Color _PDIBorderColor = Color.Transparent;
        public Color PDIBorderColor
        {
            get => _PDIBorderColor;
            set
            {
                _PDIBorderColor = value;
                OnPropertyChanged("PDIBorderColor");
            }
        }
        private Color _LoadBorderColor = Color.Transparent;
        public Color LoadBorderColor
        {
            get => _LoadBorderColor;
            set
            {
                _LoadBorderColor = value;
                OnPropertyChanged("LoadBorderColor");
            }
        }



        //private bool _ScanBorderVisibility = true;
        //public bool ScanBorderVisibility
        //{
        //    get => _ScanBorderVisibility;
        //    set
        //    {
        //        _ScanBorderVisibility = value;
        //        OnPropertyChanged("ScanBorderVisibility");
        //    }
        //}
        //private bool _PDIBorderVisibility = false;
        //public bool PDIBorderVisibility
        //{
        //    get => _PDIBorderVisibility;
        //    set
        //    {
        //        _PDIBorderVisibility = value;
        //        OnPropertyChanged("PDIBorderVisibility");
        //    }
        //}
        //private bool _LoadBorderVisibility = false;
        //public bool LoadBorderVisibility
        //{
        //    get => _LoadBorderVisibility;
        //    set
        //    {
        //        _LoadBorderVisibility = value;
        //        OnPropertyChanged("LoadBorderVisibility");
        //    }
        //}
        private Color _ScanBGColor = Color.FromHex("#E9E9E9");
        public Color ScanBGColor
        {
            get => _ScanBGColor;
            set
            {
                _ScanBGColor = value;
                OnPropertyChanged("ScanBGColor");
            }
        }
        private Color _PDIBgColor = Color.LightGray;
        public Color PDIBgColor
        {
            get => _PDIBgColor;
            set
            {
                _PDIBgColor = value;
                OnPropertyChanged("PDIBgColor");
            }
        }
        private Color _LoadBgColor = Color.LightGray;
        public Color LoadBgColor
        {
            get => _LoadBgColor;
            set
            {
                _LoadBgColor = value;
                OnPropertyChanged("LoadBgColor");
            }
        }



        private bool _QrCodeImageVisibility = false;
        public bool QrCodeImageVisibility
        {
            get => _QrCodeImageVisibility;
            set
            {
                _QrCodeImageVisibility = value;
                OnPropertyChanged("QrCodeImageVisibility");
            }
        }

        private bool _QrCodeButtonVisibility = true;
        public bool QrCodeButtonVisibility
        {
            get => _QrCodeButtonVisibility;
            set
            {
                _QrCodeButtonVisibility = value;
                OnPropertyChanged("QrCodeButtonVisibility");
            }
        }
        private bool _QrCodeImageWithDetailsVisibility = false;
        public bool QrCodeImageWithDetailsVisibility
        {
            get => _QrCodeImageWithDetailsVisibility;
            set
            {
                _QrCodeImageWithDetailsVisibility = value;
                OnPropertyChanged("QrCodeImageWithDetailsVisibility");
            }
        }

        private ObservableCollection<CPQuestionsdata> _QuestionInfo = null;
        public ObservableCollection<CPQuestionsdata> QuestionInfo
        {
            get => _QuestionInfo;
            set
            {
                _QuestionInfo = value;
                OnPropertyChanged("QuestionInfo");
            }
        }

        private string _ScannedValue;
        public string ScannedValue
        {
            get => _ScannedValue;
            set
            {
                _ScannedValue = value;
                OnPropertyChanged("ScannedValue");
            }
        }

        private BarcodeFormat _Scanned_Format;
        public BarcodeFormat Scanned_Format
        {
            get => _Scanned_Format;
            set
            {
                _Scanned_Format = value;
                OnPropertyChanged("Scanned_Format");
            }
        }
        private string _StatusRslt;
        public string StatusRslt
        {
            get => _StatusRslt;
            set
            {
                _StatusRslt = value;
                OnPropertyChanged("StatusRslt");
            }
        }
        private Color _StatusBg;
        public Color StatusBg
        {
            get => _StatusBg;
            set
            {
                _StatusBg = value;
                OnPropertyChanged("StatusBg");
            }
        }
        private string _ScanStatusSource;
        public string ScanStatusSource
        {
            get => _ScanStatusSource;
            set
            {
                _ScanStatusSource = value;
                OnPropertyChanged("ScanStatusSource");
            }
        }

        private string _PDIStatusSource;
        public string PDIStatusSource
        {
            get => _PDIStatusSource;
            set
            {
                _PDIStatusSource = value;
                OnPropertyChanged("PDIStatusSource");
            }
        }

        private string _LoadStatusSource;
        public string LoadStatusSource
        {
            get => _LoadStatusSource;
            set
            {
                _LoadStatusSource = value;
                OnPropertyChanged("LoadStatusSource");
            }
        }


        private bool _ScanTickVisibility = false;
        public bool ScanTickVisibility
        {
            get => _ScanTickVisibility;
            set
            {
                _ScanTickVisibility = value;
                OnPropertyChanged("ScanTickVisibility");
            }
        }
        private bool _PDITickVisibility = false;
        public bool PDITickVisibility
        {
            get => _PDITickVisibility;
            set
            {
                _PDITickVisibility = value;
                OnPropertyChanged("PDITickVisibility");
            }
        }
        private bool _LoadTickVisibility = false;
        public bool LoadTickVisibility
        {
            get => _LoadTickVisibility;
            set
            {
                _LoadTickVisibility = value;
                OnPropertyChanged("LoadTickVisibility");
            }
        }



        private string _VIN_NO;
        public string VIN_NO
        {
            get => _VIN_NO;
            set
            {
                _VIN_NO = value;
                OnPropertyChanged("VIN_NO");
            }
        }
        private string _scannedOn;
        public string scannedOn
        {
            get => _scannedOn;
            set
            {
                _scannedOn = value;
                OnPropertyChanged("scannedOn");
            }
        }

        private Color _BgColor;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }

        private bool _ScanFuncVisibility = true;
        public bool ScanFuncVisibility
        {
            get => _ScanFuncVisibility;
            set
            {
                _ScanFuncVisibility = value;
                OnPropertyChanged("ScanFuncVisibility");
            }
        }

        private bool _PDIFuncVisibility = false;
        public bool PDIFuncVisibility
        {
            get => _PDIFuncVisibility;
            set
            {
                _PDIFuncVisibility = value;
                OnPropertyChanged("PDIFuncVisibility");
            }
        }
        private bool _LoadFuncVisibility = false;
        public bool LoadFuncVisibility
        {
            get => _LoadFuncVisibility;
            set
            {
                _LoadFuncVisibility = value;
                OnPropertyChanged("LoadFuncVisibility");
            }
        }
        private string _CarrierNo;
        public string CarrierNo
        {
            get => _CarrierNo;
            set
            {
                _CarrierNo = value;
                OnPropertyChanged("CarrierNo");
            }
        }

        private string _Colour;
        public string Colour
        {
            get => _Colour;
            set
            {
                _Colour = value;
                OnPropertyChanged("Colour");
            }
        }

        private string _Model;
        public string Model
        {
            get => _Model;
            set
            {
                _Model = value;
                OnPropertyChanged("Model");
            }
        }
        private bool _Radio_Selected = false;
        public bool Radio_Selected
        {
            get => _Radio_Selected;
            set
            {
                _Radio_Selected = value;
                OnPropertyChanged("Radio_Selected");
            }
        }

        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                OnPropertyChanged("IndicatorVisibility");
            }
        }

        //private bool _btnenable = true;
        //public bool btnenable
        //{
        //    get { return _btnenable; }
        //    set
        //    {
        //        _btnenable = value;
        //        OnPropertyChanged("btnenable");
        //    }
        //}

        private int _Question_No;
        public int Question_No
        {
            get => _Question_No;
            set
            {
                _Question_No = value;
                OnPropertyChanged("Question_No");
            }
        }

        private string _Qestion_Title;
        public string Qestion_Title
        {
            get => _Qestion_Title;
            set
            {
                _Qestion_Title = value;
                OnPropertyChanged("Qestion_Title");
            }
        }

        private ObservableCollection<ListOption> _OptionsList;
        public ObservableCollection<ListOption> OptionsList
        {
            get => _OptionsList;
            set
            {
                _OptionsList = value;
                OnPropertyChanged("OptionsList");
            }
        }
        private bool _ExpandVisibility = false;
        public bool ExpandVisibility
        {
            get { return _ExpandVisibility; }
            set
            {
                _ExpandVisibility = value;
                OnPropertyChanged("ExpandVisibility");
            }
        }

        private bool _QuestionListVisibility = true;
        public bool QuestionListVisibility
        {
            get { return _QuestionListVisibility; }
            set
            {
                _QuestionListVisibility = value;
                OnPropertyChanged("QuestionListVisibility");
            }
        }
        private string _Next_Text = "NEXT";
        public string Next_Text
        {
            get => _Next_Text;
            set
            {
                _Next_Text = value;
                OnPropertyChanged("Next_Text");
            }
        }
        private bool _QrCodeCompleteVisibility = false;
        public bool QrCodeCompleteVisibility
        {
            get { return _QrCodeCompleteVisibility; }
            set
            {
                _QrCodeCompleteVisibility = value;
                OnPropertyChanged("QrCodeCompleteVisibility");
            }
        }
        private string _AnsweredInfoLabel;
        public string AnsweredInfoLabel
        {
            get => _AnsweredInfoLabel;
            set
            {
                _AnsweredInfoLabel = value;
                OnPropertyChanged("AnsweredInfoLabel");
            }
        }
        private string _Signature_Person;
        public string Signature_Person
        {
            get => _Signature_Person;
            set
            {
                _Signature_Person = value;
                OnPropertyChanged("Signature_Person");
            }
        }

        private bool _SignaturePadPopup = false;
        public bool SignaturePadPopup
        {
            get { return _SignaturePadPopup; }
            set
            {
                _SignaturePadPopup = value;
                OnPropertyChanged("SignaturePadPopup");
            }
        }
        private ImageSource _AuditorImageSign;
        public ImageSource AuditorImageSign
        {
            get => _AuditorImageSign;
            set
            {
                _AuditorImageSign = value;
                OnPropertyChanged("AuditorImageSign");
            }
        }
        private ImageSource _SupervisorImageSign;
        public ImageSource SupervisorImageSign
        {
            get => _SupervisorImageSign;
            set
            {
                _SupervisorImageSign = value;
                OnPropertyChanged("SupervisorImageSign");
            }
        }
        private Color _OkToLoadColor = Color.LightGray;
        public Color OkToLoadColor
        {
            get => _OkToLoadColor;
            set
            {
                _OkToLoadColor = value;
                OnPropertyChanged("OkToLoadColor");
            }
        }
        private bool _OkayToGoEnable = false;
        public bool OkayToGoEnable
        {
            get { return _OkayToGoEnable; }
            set
            {
                _OkayToGoEnable = value;
                OnPropertyChanged("OkayToGoEnable");
            }
        }
        private Color _NextBtnBgColor;
        public Color NextBtnBgColor
        {
            get => _NextBtnBgColor;
            set
            {
                _NextBtnBgColor = value;
                OnPropertyChanged("NextBtnBgColor");
            }
        }
        private bool _AllowSwipingVisible;
        public bool AllowSwipingVisible
        {
            get { return _AllowSwipingVisible; }
            set
            {
                _AllowSwipingVisible = value;
                OnPropertyChanged("AllowSwipingVisible");
            }
        }
        private Color _ScanTickBgColor;
        public Color ScanTickBgColor
        {
            get => _ScanTickBgColor;
            set
            {
                _ScanTickBgColor = value;
                OnPropertyChanged("ScanTickBgColor");
            }
        }
        private string _RemarksText;
        public string RemarksText
        {
            get => _RemarksText;
            set
            {
                _RemarksText = value;
                OnPropertyChanged("RemarksText");
            }
        }
        private string _PlaceholderText;
        public string PlaceholderText
        {
            get => _PlaceholderText;
            set
            {
                _PlaceholderText = value;
                OnPropertyChanged("PlaceholderText");
            }
        }



        //private Color _NotAnsweredBgColor;
        //public Color NotAnsweredBgColor
        //{
        //    get => _NotAnsweredBgColor;
        //    set
        //    {
        //        _NotAnsweredBgColor = value;
        //        OnPropertyChanged("NotAnsweredBgColor");
        //    }
        //}


        //private CPQuestionsdata _SelectedQuestionNo;
        //public CPQuestionsdata SelectedQuestionNo
        //{
        //    get => _SelectedQuestionNo;
        //    set
        //    {
        //        _SelectedQuestionNo = value;
        //        QuestionNoTappped();
        //    }
        //}
        #endregion

    }
}
