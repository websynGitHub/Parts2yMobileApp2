using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
    public class DealerVehicleDetailsViewModel : BaseViewModel
    {
        public INavigation Navigation { get; set; }
        public ICommand ScanCommand { get; set; }
        public ICommand InspectionCommand { get; set; }
        public ICommand PODCommand { get; set; }
        public ICommand QrCodeCommand { get; set; }
        public ICommand VerifyCommand { get; set; }
        public ICommand SelecpicCmd { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand ViewAllCommand { get; set; }
        public ICommand NextCommand { get; set; }
        public ICommand AuditorSignatureCommand { get; set; }
        public ICommand DealerSignatureCommand { get; set; }
        public ICommand CrossTapCommand { get; set; }
        public ICommand DoneCommand { get; set; }
        public Command<object> QuestionNo_Tapped { get; set; }
        public ICommand SelectPicCommand { get; set; }
        public Command<object> Question_Tapped { get; set; }
        public ICommand ResendOTPCommand { get; set; }
        public bool isAllAnswered { get; set; } = false;

        public int i = 0;
        public List<DealerVinDetails> result { get; set; }


        RestClient service = new RestClient();
        public ObservableCollection<CPQuestionsdata> cPQuestionsdata = new ObservableCollection<CPQuestionsdata>();


        public DealerVehicleDetailsViewModel(INavigation _Navigation)
        {
            Navigation = _Navigation;
            BgColor = Settings.Bar_Background;
            NextBtnBgColor = Settings.Bar_Background;
            ScanCommand = new Command(async () => await Scan_Tap());
            InspectionCommand = new Command(async () => await Inspection_Tap());
            PODCommand = new Command(async () => await POD_Tap());
            QrCodeCommand = new Command(async () => await QRCode_Tap());
            Backevnttapped = new Command(async () => await Backevnttapped_click());
            AuditorSignatureCommand = new Command(async () => await AuditorSignature_Tapped());
            DealerSignatureCommand = new Command(async () => await DealerSignature_Tapped());
            CrossTapCommand = new Command(async () => await CrossSignaturePadTapped());
            DoneCommand = new Command(async () => await Done_Tap());
            QuestionNo_Tapped = new Command<object>(QuestionNo_Tap);
            Question_Tapped = new Command<object>(Question_Tap);
            SelectPicCommand = new Command(async () => await Select_Pic());
            ViewAllCommand = new Command(async () => await Viewall_Tap());
            NextCommand = new Command(async () => await Next_Tap());
            ResendOTPCommand = new Command(async () => await ResendOTP_Tapped());
            GetDealerVinData();
        }

        private async Task ResendOTP_Tapped()
        {
            try
            {
                await App.Current.MainPage.DisplayAlert("Success", "OTP has been re-sent successfully.", "OK");
            }
            catch (Exception ex)
            {

            }
        }

        public async Task GetDealerVinData()
        {
            try
            {
                DealerDB Db = new DealerDB("dealervehicledetails");
                var current = Connectivity.NetworkAccess;
                result = new List<DealerVinDetails>();

                loadindicator = true;

                if (current == NetworkAccess.Internet)
                {
                    var Dealervindata = await service.GetDealervindetails(Settings.VINNo);
                    var val = JsonConvert.DeserializeObject<DealerVinDetails>(Dealervindata.ToString());
                    result.Add(val);
                    Db.SaveAndUpdatDealerVinData(result);
                }
                else
                {
                    result = Db.GetAllVeData();
                }


                if (result.Count > 0)
                {
                    if (result[0].status != false)
                    {
                        Model = result[0].Dealerdata.Modelname;
                        VIN_NO = Settings.VINNo = result[0].Dealerdata.Vin;
                        Colour = result[0].Dealerdata.Color;
                        CarrierNo = result[0].Dealerdata.Carrier;
                        Settings.PDICompleted = result[0].Dealerdata.Inspection;
                        Settings.ScanCompleted = result[0].Dealerdata.ScanCompleted;
                        Settings.LoadCompleted = result[0].Dealerdata.Pod;
                        StatusRslt = result[0].Dealerdata.Scanstatus;

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
                        if (result[0].Dealerdata.ScanCompleted == "1" && result[0].Dealerdata.Scanstatus == "Verified")
                        {

                            scannedOn = result[0].Dealerdata.ScanOn;
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

                        if (!string.IsNullOrEmpty(result[0].Dealerdata.Inspection))
                        {
                            InspectionStatusSource = Icons.CheckCircle;
                            InspectionTickVisibility = true;
                            InspectionBgColor = Color.FromHex("#E9E9E9");
                            if (result[0].signatureDealerBase64 != null)
                            {
                                DealerImageSign = ImageSource.FromStream(() => new MemoryStream(result[0].signatureDealerBase64));
                            }
                            else
                            {
                                DealerImageSign = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/a/a1/Signature_of_Andrew_Scheer.png"));
                            }
                        }
                        if (!string.IsNullOrEmpty(result[0].Dealerdata.Pod))
                        {
                            PODStatusSource = Icons.CheckCircle;
                            PODTickVisibility = true;
                            PODBgColor = Color.FromHex("#E9E9E9");
                            if (result[0].signatureDealerBase64 != null)
                            {
                                DealerImageSign = ImageSource.FromStream(() => new MemoryStream(result[0].signatureDealerBase64));
                            }
                            else
                            {
                                DealerImageSign = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/a/a1/Signature_of_Andrew_Scheer.png"));
                            }
                            //AuditorImageSign = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/a/ac/Chris_Hemsworth_Signature.png"));
                            //DealerImageSign = ImageSource.FromUri(new Uri("https://upload.wikimedia.org/wikipedia/commons/a/a1/Signature_of_Andrew_Scheer.png"));
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
                        foreach (var items in result[0].VinCheckList)
                        {
                            items.RightSwipeText = items.listOptions[0].Observation;
                            items.RightSwipeSelectedIndex = items.listOptions[0].ObsID;
                            var itemlist = items.listOptions.Where(x => x.Isanswered).ToList();
                            if (itemlist.Count != 0)
                            {
                                items.NotAnsweredBgColor = Color.FromHex("#005800"); ;
                                items.AnsweredQstnBgColor = Color.FromHex("#024182");
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

                        cPQuestionsdata = result[0].VinCheckList;
                        QuestionInfo = cPQuestionsdata;

                        loadindicator = false;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "No data available", "OK");
                        await Navigation.PopModalAsync();
                        loadindicator = false;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Alert", "No data available", "OK");
                    await Navigation.PopModalAsync();
                    loadindicator = false;
                }
            }
            catch (Exception ex)
            {
                loadindicator = false;
            }
        }

        private async Task Next_Tap()
        {
            try
            {

                i = Settings.QNo_Sequence;
                Settings.QNo_Sequence = Settings.QNo_Sequence != QuestionInfo.Count ? QuestionInfo[i].Seqno : Settings.QNo_Sequence;
                QuestionInfo[i - 1].AnsweredRemarks = RemarksText;
                result[0].VinCheckList[i - 1].isAnswered = (QuestionInfo[i - 1].listOptions.Where(x => x.Isanswered == true).FirstOrDefault()) != null ? true : false;
                EPODPhotoUplodeViewModel vmEPODPhoto = new EPODPhotoUplodeViewModel();
                photoCount = Convert.ToString(vmEPODPhoto.photolist.Count);
                if (Next_Text == "SAVE")
                {
                    if (string.IsNullOrEmpty(Settings.PDICompleted))
                    {
                        var notansweredquestions = result[0].VinCheckList.Where(x => x.isAnswered == false).ToList();
                        if (notansweredquestions.Count == 0)
                        {
                            //isAllAnswered = true;
                            Settings.PDICompleted = DateTime.Now.ToString("h:mm");
                            InspectionTickVisibility = true;
                            GetQuestionarieDetils();
                            //GetDealerVinData();
                            //DealerDB vehicleDetailDB = new DealerDB("dealervehicledetails");
                            //vehicleDetailDB.UpdateAllVeData(result);

                            POD_Tap();

                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "Need to answer all questions", "OK");
                        }


                        //DealerDB vehicleDetailDB = new DealerDB("dealervehicledetails");
                        //result[0].Dealerdata.Inspection = DateTime.Now.ToString("h:mm");
                        // vehicleDetailDB.UpdateAllVeData(result);
                        //GetDealerVinData();
                        //Settings.PDICompleted = result[0].Dealerdata.Inspection;
                    }
                }
                else
                {
                    if (i < QuestionInfo.Count)
                    {
                        //i = Settings.QNo_Sequence;
                        //Settings.QNo_Sequence = QuestionInfo[i].Seqno;
                        NextBtnBgColor = ((!string.IsNullOrEmpty(Settings.PDICompleted)) && Settings.QNo_Sequence == QuestionInfo.Count) ? Color.LightGray : Settings.Bar_Background;
                        Next_Text = (Settings.QNo_Sequence == QuestionInfo.Count) ? "SAVE" : "NEXT";
                        Question_No = QuestionInfo[i].Seqno;
                        Qestion_Title = QuestionInfo[i].PDI_QestionTitle;
                        OptionsList = QuestionInfo[i].listOptions;
                        if (!string.IsNullOrEmpty(QuestionInfo[i].AnsweredRemarks))
                        {
                            RemarksText = QuestionInfo[i].AnsweredRemarks;
                        }
                        else
                        {
                            RemarksText = "";
                            if (!string.IsNullOrEmpty(QuestionInfo[i].Remarks))
                            {
                                PlaceholderText = QuestionInfo[i].Remarks;
                            }
                            else
                            {
                                PlaceholderText = "REMARKS";
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void GetQuestionarieDetils()
        {
            try
            {
                if (result[0].VinCheckList.Count > 0)
                {
                    foreach (var items in result[0].VinCheckList)
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
                    QuestionInfo = result[0].VinCheckList;
                }

            }
            catch (Exception ex)
            {

            }
        }


        private async Task Viewall_Tap()
        {
            try
            {
                loadindicator = true;
                i = 0;
                ExpandVisibility = false;
                QuestionListVisibility = true;
            }
            catch (Exception ex)
            {


            }
            finally
            {
                loadindicator = false;
            }
        }

        private void Question_Tap(object obj)
        {
            try
            {
                loadindicator = true;

                var data = obj as CPQuestionsdata;//Vm.ExpandVisibility = true;

                if (data != null)
                {
                    //Vm.PDIFuncVisibility = true;
                    //ScanFuncVisibility = LoadFuncVisibility = false;
                    //expandStack.IsVisible = true;
                    //Vm.QuestionListVisibility = false;
                    QuestionListVisibility = false;
                    ExpandVisibility = true;
                    EPODPhotoUplodeViewModel vmEPODPhoto = new EPODPhotoUplodeViewModel();
                    photoCount = Convert.ToString(vmEPODPhoto.photolist.Count);
                    Settings.QNo_Sequence = data.Seqno;
                    NextBtnBgColor = ((!string.IsNullOrEmpty(Settings.PDICompleted)) && Settings.QNo_Sequence == QuestionInfo.Count) ? Color.LightGray : Settings.Bar_Background;
                    Next_Text = (Settings.QNo_Sequence == QuestionInfo.Count) ? "SAVE" : "NEXT";
                    Question_No = data.Seqno;
                    Qestion_Title = data.PDI_QestionTitle;

                    if (!string.IsNullOrEmpty(data.AnsweredRemarks))
                    {
                        RemarksText = data.AnsweredRemarks;
                    }
                    else
                    {
                        RemarksText = "";
                        if (!string.IsNullOrEmpty(data.Remarks))
                        {
                            PlaceholderText = data.Remarks;
                        }
                        else
                        {
                            PlaceholderText = "REMARKS";

                        }
                    }
                    OptionsList = data.listOptions;
                }
                loadindicator = false;
            }
            catch (Exception ex)
            {
                loadindicator = false;
            }
        }

        private async Task Select_Pic()
        {
            try
            {
                if (Navigation.ModalStack.Count == 0 ||
                                      Navigation.ModalStack.Last().GetType() != typeof(PhotoUploadePOD))
                {
                    Navigation.PushModalAsync(new PhotoUploadePOD());
                }

            }
            catch (Exception ex)
            {

            }
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                await Navigation.PopModalAsync();

            }
            catch (Exception ex)
            {
            }
        }
        public void TappedGestureCommandMethod(object obj)
        {
            try
            {
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
                    EPODPhotoUplodeViewModel vmEPODPhoto = new EPODPhotoUplodeViewModel();
                    photoCount = Convert.ToString(vmEPODPhoto.photolist.Count);
                    OptionsList = tappedItemData.listOptions;
                }
            }
            catch (Exception ex)
            {

            }
        }
        public async Task POD_Tap()
        {
            try
            {
                //if (!string.IsNullOrEmpty(Settings.PDICompleted))
                //{
                if (Settings.ScanCompleted == "1" && !string.IsNullOrEmpty(Settings.PDICompleted))
                {
                    PODFuncVisibility = true;
                    InspectionFuncVisibility = ScanFuncVisibility = false;
                    //LoadTextColor = Color.Black;
                    //PDITextColor = ScanTextColor = Color.Gray;
                    PODTabVisibility = false;
                    InspectionTabVisibility = ScanTabVisibility = true;
                    //LoadBorderColor = Color.FromHex("#000000");
                    //PDIBorderColor = ScanBorderColor = Color.FromHex("#E9E9E9");
                    PODBorderBg = Color.FromHex("#000000");
                    ScanBorderBg = InspectionBorderBg = Color.Transparent;
                    //PODBorderVisibility = true;
                    //InspectionBorderVisibility = ScanBorderVisibility = false;
                    PODBgColor = Color.FromHex("#E9E9E9");
                    //ScanBGColor = PDIBgColor = Color.LightGray;
                }
                //}
            }
            catch (Exception ex)
            {


            }
        }
        public async Task Inspection_Tap()
        {
            try
            {
                if (Settings.ScanCompleted == "1")
                {
                    InspectionTabVisibility = false;
                    PODTabVisibility = ScanTabVisibility = true;
                    InspectionFuncVisibility = true;
                    ScanFuncVisibility = PODFuncVisibility = false;
                    // PDIBorderColor = Color.FromHex("#000000");
                    //LoadBorderColor = ScanBorderColor = Color.FromHex("#E9E9E9");
                    InspectionBorderBg = Color.FromHex("#000000");
                    ScanBorderBg = PODBorderBg = Color.Transparent;
                    //InspectionBorderVisibility = true;
                    //ScanBorderVisibility = PODBorderVisibility = false;
                    InspectionBgColor = Color.FromHex("#E9E9E9");
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


                    ScannerPage.OnScanResult += (result) =>
                    {
                        // Parar de escanear
                        ScannerPage.IsScanning = false;

                        // Alert com o código escaneado
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Navigation.PopModalAsync();
                            if (result != null)
                            {
                                App.Current.MainPage.DisplayAlert("Alert", result.Text, "OK");
                                ScannedValue = result.Text;
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

                                Scanned_Format = result.BarcodeFormat;
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
        }
        private async Task Scan_Tap()
        {
            try
            {
                //if (StatusRslt != "Verified")
                //{
                if (StatusRslt == "Verified" && Settings.ScanCompleted == "1" && !string.IsNullOrEmpty(Settings.PDICompleted))
                {
                    InspectionFuncVisibility = PODFuncVisibility = false;
                    ScanFuncVisibility = true;
                    //ScanTextColor = Color.Black;
                    //PDITextColor = LoadTextColor = Color.Gray;
                    ScanTabVisibility = false;
                    InspectionTabVisibility = PODTabVisibility = true;
                    ScanBorderBg = Color.FromHex("#000000");
                    InspectionBorderBg = PODBorderBg = Color.Transparent;
                    //ScanBorderVisibility = true;
                    //PODBorderVisibility = InspectionBorderVisibility = false;
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
        public async Task DealerSignature_Tapped()
        {
            try
            {
                if (string.IsNullOrEmpty(Settings.LoadCompleted))
                {

                    Signature_Person = "Dealer's Signature";
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
        public async Task Done_Tap()
        {
            try
            {
                DealerDB veDetailsDB = new DealerDB("dealervehicledetails");
                veDetailsDB.UpdateAllVeData(result);
                InspectionTickVisibility = true;
                Settings.PDICompleted = DateTime.Now.ToString("h:mm");
                Settings.LoadCompleted = DateTime.Now.ToString("h:mm");
                Settings.isPDICompleted = Settings.isLoadCompleted = PODTickVisibility = true;
                await Navigation.PopModalAsync();


            }
            catch (Exception ex)
            {
            }
        }
        private void QuestionNo_Tap(object obj)
        {
            try
            {
                var tappedItemData = obj as CPQuestionsdata;
                if (tappedItemData != null)
                {
                    Inspection_Tap();
                    //LoadFuncVisibility = false;
                    //PDIFuncVisibility = true;
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
        }

        #region Properties
        private bool _loadindicator = false;
        public bool loadindicator
        {
            get => _loadindicator;
            set
            {
                _loadindicator = value;
                NotifyPropertyChanged("loadindicator");
            }
        }

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

        private bool _InspectionTabVisibility = true;
        public bool InspectionTabVisibility
        {
            get => _InspectionTabVisibility;
            set
            {
                _InspectionTabVisibility = value;
                OnPropertyChanged("InspectionTabVisibility");
            }
        }
        private bool _PODTabVisibility = true;
        public bool PODTabVisibility
        {
            get => _PODTabVisibility;
            set
            {
                _PODTabVisibility = value;
                OnPropertyChanged("PODTabVisibility");
            }
        }

        private Color _ScanBorderBg = Color.FromHex("#000000");
        public Color ScanBorderBg
        {
            get => _ScanBorderBg;
            set
            {
                _ScanBorderBg = value;
                OnPropertyChanged("ScanBorderBg");
            }
        }

        private Color _InspectionBorderBg = Color.Transparent;
        public Color InspectionBorderBg
        {
            get => _InspectionBorderBg;
            set
            {
                _InspectionBorderBg = value;
                OnPropertyChanged("InspectionBorderBg");
            }
        }
        private Color _PODBorderBg = Color.Transparent;
        public Color PODBorderBg
        {
            get => _PODBorderBg;
            set
            {
                _PODBorderBg = value;
                OnPropertyChanged("PODBorderBg");
            }
        }


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
        //private bool _InspectionBorderVisibility = false;
        //public bool InspectionBorderVisibility
        //{
        //    get => _InspectionBorderVisibility;
        //    set
        //    {
        //        _InspectionBorderVisibility = value;
        //        OnPropertyChanged("InspectionBorderVisibility");
        //    }
        //}
        //private bool _PODBorderVisibility = false;
        //public bool PODBorderVisibility
        //{
        //    get => _PODBorderVisibility;
        //    set
        //    {
        //        _PODBorderVisibility = value;
        //        OnPropertyChanged("PODBorderVisibility");
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
        private Color _InspectionBgColor = Color.LightGray;
        public Color InspectionBgColor
        {
            get => _InspectionBgColor;
            set
            {
                _InspectionBgColor = value;
                OnPropertyChanged("InspectionBgColor");
            }
        }
        private Color _PODBgColor = Color.LightGray;
        public Color PODBgColor
        {
            get => _PODBgColor;
            set
            {
                _PODBgColor = value;
                OnPropertyChanged("PODBgColor");
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

        private string _InspectionStatusSource;
        public string InspectionStatusSource
        {
            get => _InspectionStatusSource;
            set
            {
                _InspectionStatusSource = value;
                OnPropertyChanged("InspectionStatusSource");
            }
        }

        private string _PODStatusSource;
        public string PODStatusSource
        {
            get => _PODStatusSource;
            set
            {
                _PODStatusSource = value;
                OnPropertyChanged("PODStatusSource");
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
        private bool _InspectionTickVisibility = false;
        public bool InspectionTickVisibility
        {
            get => _InspectionTickVisibility;
            set
            {
                _InspectionTickVisibility = value;
                OnPropertyChanged("InspectionTickVisibility");
            }
        }
        private bool _PODTickVisibility = false;
        public bool PODTickVisibility
        {
            get => _PODTickVisibility;
            set
            {
                _PODTickVisibility = value;
                OnPropertyChanged("PODTickVisibility");
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

        private bool _InspectionFuncVisibility = false;
        public bool InspectionFuncVisibility
        {
            get => _InspectionFuncVisibility;
            set
            {
                _InspectionFuncVisibility = value;
                OnPropertyChanged("InspectionFuncVisibility");
            }
        }
        private bool _PODFuncVisibility = false;
        public bool PODFuncVisibility
        {
            get => _PODFuncVisibility;
            set
            {
                _PODFuncVisibility = value;
                OnPropertyChanged("PODFuncVisibility");
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
        private ImageSource _DealerImageSign;
        public ImageSource DealerImageSign
        {
            get => _DealerImageSign;
            set
            {
                _DealerImageSign = value;
                OnPropertyChanged("DealerImageSign");
            }
        }
        private Color _DoneLoadColor = Color.LightGray;
        public Color DoneLoadColor
        {
            get => _DoneLoadColor;
            set
            {
                _DoneLoadColor = value;
                OnPropertyChanged("DoneLoadColor");
            }
        }
        private bool _DoneBtnEnable = false;
        public bool DoneBtnEnable
        {
            get { return _DoneBtnEnable; }
            set
            {
                _DoneBtnEnable = value;
                OnPropertyChanged("DoneBtnEnable");
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

        #endregion
    }
}
