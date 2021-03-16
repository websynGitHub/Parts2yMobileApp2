﻿using System;
using System.Collections.Generic;
using System.Text;
using YPS.Views;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;

using Xamarin.Forms;
using YPS.Service;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using ZXing.Net.Mobile.Forms;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using System.Collections.ObjectModel;
using YPS.CommonClasses;
using YPS.Model;
using YPS.CustomToastMsg;
using YPS.Helpers;
using Newtonsoft.Json;
using static YPS.Model.SearchModel;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class ScanPageViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        YPSService trackService;
        ScanPage pagename;
        public ICommand reScanCmd { set; get; }
        public ICommand MoveNextCmd { set; get; }
        public PhotoUploadModel selectedTagData { get; set; }
        public AllPoData selectedPOTagData { get; set; }
        public static int uploadType;
        public bool isInitialPhoto, isbuttonenable;

        public ScanPageViewModel(INavigation _Navigation, int uploadtype, PhotoUploadModel selectedtagdata, bool isinitialphoto, AllPoData selectedpoagdata, ScanPage page)
        {
            try
            {
                Navigation = _Navigation;
                pagename = page;
                uploadType = uploadtype;
                isInitialPhoto = isinitialphoto;
                selectedPOTagData = selectedpoagdata;
                selectedTagData = selectedtagdata;
                trackService = new YPSService();
                #region BInding tab & click event methods to respective ICommand properties
                reScanCmd = new Command(async () => await ReScan());
                MoveNextCmd = new Command(async () => await MoveNext(ScannedAllPOData));
                #endregion

                if (Settings.VersionID == 2 && uploadtype == 0)
                {
                    PageNextButton = "Insp";
                }
                else
                {
                    PageNextButton = "Photo";
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    isbuttonenable = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? true : false;
                }
                //Task.Run(() => OpenScanner()).Wait();
            }
            catch (Exception ex)
            {

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
                        ScannerPage.IsScanning = false;
                        //CanOpenScan = false;

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PopAsync();

                            ScannedResult = scanresult.Text;

                            if (!string.IsNullOrEmpty(ScannedResult))
                            {
                                IsPageVisible = true;

                                if (uploadType == 0 && selectedTagData == null)
                                {
                                    await GetDataAndVerify();
                                }
                                else
                                {
                                    await SingleTagDataVerification();
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
                //CanOpenScan = true;
            }
            catch (Exception ex)
            {

            }
        }

        public async Task SingleTagDataVerification()
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

                    //var result = await trackService.LoadPoDataService(sendPodata);

                    if (selectedTagData != null)
                    {
                        if (!string.IsNullOrEmpty(selectedTagData.photoTags[0].TagNumber))
                        {
                            ScannedCompareData = (selectedTagData.photoTags[0].TagNumber == ScannedResult) ? ScannedResult : null;
                        }
                        else
                        {
                            ScannedCompareData = (selectedTagData.photoTags[0].IdentCode == ScannedResult) ? ScannedResult : null;
                        }

                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(selectedPOTagData.TagNumber))
                        {
                            ScannedCompareData = (selectedPOTagData.TagNumber == ScannedResult) ? ScannedResult : null;
                        }
                        else
                        {
                            ScannedCompareData = (selectedPOTagData.IdentCode == ScannedResult) ? ScannedResult : null;
                        }
                    }

                    if (!string.IsNullOrEmpty(ScannedCompareData))
                    {
                        ScannedOn = DateTime.Now.ToString(@"MM/dd/yyyy hh:mm:ss tt");

                        if (selectedTagData != null)
                        {
                            StatusText = selectedTagData.photoTags[0].TagTaskStatus == 2 ? "Done" : "Verified";
                        }
                        else
                        {
                            StatusText = selectedPOTagData.TagTaskStatus == 2 ? "Done" : "Verified";
                        }

                        StatusTextBgColor = Color.DarkGreen;
                        ScannedValue = ScannedResult;

                        if (isbuttonenable == true)
                        {
                            IsPhotoEnable = true;
                            PhotoOpacity = 1.0;
                        }
                    }
                    else
                    {
                        ScannedOn = DateTime.Now.ToString(@"MM/dd/yyyy hh:mm:ss tt");
                        StatusText = "Not matched";
                        StatusTextBgColor = Color.Red;
                        ScannedValue = ScannedResult;
                        IsPhotoEnable = false;
                        PhotoOpacity = 0.5;
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async Task GetDataAndVerify()
        {
            try
            {
                IndicatorVisibility = true;

                YPSLogger.TrackEvent("ScanPageViewModel.cs", "in GerDataAndVerify method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {

                    SendPodata sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;

                    var result = await trackService.LoadPoDataService(sendPodata);

                    if (result != null && result.data != null)
                    {
                        if (result.status != 0 && result.data.allPoData != null && result.data.allPoData.Count > 0)
                        {

                            var groubbyval = result.data.allPoData.GroupBy(gb => gb.POShippingNumber);
                            ObservableCollection<AllPoData> PoDataCollections = new ObservableCollection<AllPoData>();
                            PoDataCollections = new ObservableCollection<AllPoData>(result.data.allPoData);

                            ScannedAllPOData = PoDataCollections.Where(wr => wr.TagNumber == ScannedResult).FirstOrDefault();

                            if (ScannedAllPOData == null)
                            {
                                ScannedAllPOData = PoDataCollections.Where(wr => wr.IdentCode == ScannedResult).FirstOrDefault();
                            }

                            if (ScannedAllPOData != null)
                            {
                                ScannedOn = DateTime.Now.ToString(@"MM/dd/yyyy hh:mm:ss tt");
                                StatusText = ScannedAllPOData.TagTaskStatus == 2 ? "Done" : "Verified";
                                StatusTextBgColor = Settings.Bar_Background;
                                ScannedValue = ScannedResult;

                                if (Settings.VersionID == 2)
                                {
                                    //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                                    //{
                                    IsPhotoEnable = true;
                                    PhotoOpacity = 1.0;
                                    //}
                                }
                                else
                                {
                                    if (isbuttonenable == true)
                                    {
                                        IsPhotoEnable = true;
                                        PhotoOpacity = 1.0;
                                    }
                                }
                            }
                            else
                            {
                                ScannedOn = DateTime.Now.ToString(@"MM/dd/yyyy hh:mm:ss tt");
                                StatusText = "Not matched";
                                StatusTextBgColor = Color.Red;
                                ScannedValue = ScannedResult;
                                IsPhotoEnable = false;
                                PhotoOpacity = 0.5;
                            }
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
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async void SearchResultGet(SendPodata sendPodata)
        {
            try
            {
                var Serchdata = await trackService.GetSearchValuesService(Settings.userLoginID);

                if (Serchdata != null)
                {
                    if (Serchdata.status == 1)
                    {
                        if (!string.IsNullOrEmpty(Serchdata.data.SearchCriteria))
                        {
                            var searchC = JsonConvert.DeserializeObject<SendPodata>(Serchdata.data.SearchCriteria);

                            if (searchC != null)
                            {
                                //Key
                                sendPodata.PONumber = Settings.PONumber = searchC.PONumber;
                                sendPodata.REQNo = Settings.REQNo = searchC.REQNo;
                                sendPodata.ShippingNo = Settings.ShippingNo = searchC.ShippingNo;
                                sendPodata.DisciplineID = Settings.DisciplineID = searchC.DisciplineID;
                                sendPodata.ELevelID = Settings.ELevelID = searchC.ELevelID;
                                sendPodata.ConditionID = Settings.ConditionID = searchC.ConditionID;
                                sendPodata.ExpeditorID = Settings.ExpeditorID = searchC.ExpeditorID;
                                sendPodata.PriorityID = Settings.PriorityID = searchC.PriorityID;
                                sendPodata.TagNo = Settings.TAGNo = searchC.TagNo;
                                sendPodata.IdentCode = Settings.IdentCodeNo = searchC.IdentCode;
                                sendPodata.BagNo = Settings.BagNo = searchC.BagNo;
                                sendPodata.yBkgNumber = Settings.Ybkgnumber = searchC.yBkgNumber;
                                sendPodata.TaskName = Settings.TaskName = searchC.TaskName;

                                Settings.SearchWentWrong = false;
                            }

                        }
                        else
                        {
                            await SaveAndClearSearch(true);
                        }
                    }
                    else
                    {
                        Settings.SearchWentWrong = true;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SearchResultGet method -> in PoDataViewModel! " + Settings.userLoginID);
                var trackResult = await trackService.Handleexception(ex);
            }
        }


        private async Task SaveAndClearSearch(bool val)
        {
            SendPodata SaveUserDS = new SendPodata();
            SearchPassData defaultData = new SearchPassData();
            try
            {
                //Key
                SaveUserDS.PONumber = Settings.PONumber = string.Empty;
                SaveUserDS.REQNo = Settings.REQNo = string.Empty;
                SaveUserDS.ShippingNo = Settings.ShippingNo = string.Empty;
                SaveUserDS.DisciplineID = Settings.DisciplineID = 0;
                SaveUserDS.ELevelID = Settings.ELevelID = 0;
                SaveUserDS.ConditionID = Settings.ConditionID = 0;
                SaveUserDS.ExpeditorID = Settings.ExpeditorID = 0;
                SaveUserDS.PriorityID = Settings.PriorityID = 0;
                SaveUserDS.TagNo = Settings.TAGNo = string.Empty;
                SaveUserDS.IdentCode = Settings.IdentCodeNo = string.Empty;
                SaveUserDS.BagNo = Settings.BagNo = string.Empty;
                SaveUserDS.yBkgNumber = Settings.Ybkgnumber = string.Empty;
                SaveUserDS.TaskName = Settings.TaskName = string.Empty;
                defaultData.CompanyID = Settings.CompanyID;
                defaultData.UserID = Settings.userLoginID;
                defaultData.SearchCriteria = JsonConvert.SerializeObject(SaveUserDS);
                var responseData = await trackService.SaveSerchvaluesSetting(defaultData);

                if (val == true)
                {
                    SearchResultGet(SaveUserDS);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveAndClearSearch method -> in PoDataViewModel! " + Settings.userLoginID);
                await trackService.Handleexception(ex);
            }
        }

        public async Task MoveNext(AllPoData podata)
        {
            try
            {
                if (IsPhotoEnable == true)
                {
                    IndicatorVisibility = true;
                    YPSLogger.TrackEvent("ScanPageViewModel", "in PhotoUpload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    bool checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                        //{
                        if (Settings.VersionID == 2 && uploadType == 0)
                        {
                            //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                            //{
                            Settings.POID = podata.POID;
                            await Navigation.PushAsync(new QuestionsPage(podata.POTagID, podata.TagNumber, podata.IdentCode, podata.BagNumber, null));
                            //}
                        }
                        else if (uploadType != 0)
                        {
                            if (isbuttonenable == true)
                            {
                                if (isInitialPhoto == true && selectedTagData != null)
                                {
                                    Settings.POID = selectedTagData.POID;
                                    podata = new AllPoData();
                                    podata.TagNumber = selectedTagData.photoTags[0].TagNumber;
                                    await Navigation.PushAsync(new PhotoUpload(selectedTagData, podata, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false));
                                }
                                else
                                {
                                    if (selectedPOTagData != null)
                                    {
                                        Settings.POID = selectedPOTagData.POID;
                                    }

                                    Settings.CanUploadPhotos = true;
                                    await Navigation.PopAsync();

                                    //Settings.POID = selectedPOTagData.POID;
                                    //if (uploadType == (int)UploadTypeEnums.GoodsPhotos_BP)
                                    //{

                                    //    await Navigation.PushAsync(new PhotoUpload(null, selectedPOTagData, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, selectedPOTagData.photoTickVisible));
                                    //}
                                    //else
                                    //{
                                    //    await Navigation.PushAsync(new PhotoUpload(null, selectedPOTagData, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_AP, selectedPOTagData.photoTickVisible));
                                    //}
                                }
                            }
                        }
                        else
                        {
                            if (isbuttonenable == true)
                            {
                                #region PhotoUpload
                                if (podata.cameImage == "cross.png")
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Photos not required to upload for the selected tag(s).");
                                }
                                else
                                {
                                    PhotoUploadModel selectedTagsData = new PhotoUploadModel();

                                    Settings.POID = podata.POID;
                                    selectedTagsData.POID = podata.POID;
                                    selectedTagsData.isCompleted = podata.photoTickVisible;

                                    List<PhotoTag> lstdat = new List<PhotoTag>();

                                    //if (podata.TagTaskStatus == 2)
                                    //{
                                    //    ScannedOn = DateTime.Now.ToString(@"dd/MM/yyyy hh:mm:ss tt");
                                    //    StatusText = "Done";
                                    //    StatusTextBgColor = Settings.Bar_Background;
                                    //    IsPhotoEnable = false;
                                    //    PhotoOpacity = 0.5;
                                    //}
                                    //else 
                                    if (podata.TagAPhotoCount == 0 && podata.TagBPhotoCount == 0 && podata.PUID == 0)
                                    {
                                        PhotoTag tg = new PhotoTag();

                                        if (podata.POTagID != 0)
                                        {
                                            tg.POTagID = podata.POTagID;
                                            Settings.Tagnumbers = podata.TagNumber;
                                            lstdat.Add(tg);

                                            selectedTagsData.photoTags = lstdat;
                                            Settings.currentPoTagId_Inti = lstdat;


                                            if (selectedTagsData.photoTags.Count != 0)
                                            {
                                                await Navigation.PushAsync(new PhotoUpload(selectedTagsData, podata, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        selectedTagsData.alreadyExit = "alreadyExit";

                                        if (podata.imgCamOpacityB != 0.5)
                                        {
                                            try
                                            {
                                                Settings.currentPuId = podata.PUID;
                                                Settings.BphotoCount = podata.TagBPhotoCount;
                                                await Navigation.PushAsync(new PhotoUpload(null, podata, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, podata.photoTickVisible));
                                            }
                                            catch (Exception ex)
                                            {
                                                YPSLogger.ReportException(ex, "tap_eachCamB method -> in POChildListPageViewModel " + Settings.userLoginID);
                                                var trackResult = await trackService.Handleexception(ex);
                                            }
                                        }
                                    }
                                }
                                #endregion PhotoUpload
                            }
                        }
                        //}
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }

        }

        #region Properties

        public string _ScannedCompareData;
        public string ScannedCompareData
        {
            get { return _ScannedCompareData; }
            set
            {
                _ScannedCompareData = value;
                RaisePropertyChanged("ScannedCompareData");
            }
        }

        public string _PageNextButton = (Settings.VersionID == 2 && uploadType == 0) == true ? "Insp" : "Photo";
        public string PageNextButton
        {
            get { return _PageNextButton; }
            set
            {
                _PageNextButton = value;
                RaisePropertyChanged("PageNextButton");
            }
        }

        public AllPoData _ScannedAllPOData;
        public AllPoData ScannedAllPOData
        {
            get { return _ScannedAllPOData; }
            set
            {
                _ScannedAllPOData = value;
                RaisePropertyChanged("ScannedAllPOData");
            }
        }

        public string _ScannedValue;
        public string ScannedValue
        {
            get { return _ScannedValue; }
            set
            {
                _ScannedValue = value;
                RaisePropertyChanged("ScannedValue");
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

        public double _PhotoOpacity = 0.5;
        public double PhotoOpacity
        {
            get { return _PhotoOpacity; }
            set
            {
                _PhotoOpacity = value;
                RaisePropertyChanged("PhotoOpacity");
            }
        }

        public bool _IsPhotoEnable;
        public bool IsPhotoEnable
        {
            get { return _IsPhotoEnable; }
            set
            {
                _IsPhotoEnable = value;
                RaisePropertyChanged("IsPhotoEnable");
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
