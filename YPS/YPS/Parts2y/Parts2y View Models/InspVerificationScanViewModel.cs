﻿using Newtonsoft.Json;
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
using ZXing.Net.Mobile.Forms;
using static YPS.Model.SearchModel;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class InspVerificationScanViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        YPSService trackService;
        InspVerificationScanPage pagename;
        public ICommand reScanCmd { set; get; }
        public ICommand MoveToInspCmd { set; get; }

        public InspVerificationScanViewModel(INavigation _Navigation, InspVerificationScanPage page)
        {
            Navigation = _Navigation;
            pagename = page;
            trackService = new YPSService();
            #region BInding tab & click event methods to respective ICommand properties
            reScanCmd = new Command(async () => await ReScan());
            MoveToInspCmd = new Command(async () => await MoveForInspection(ScannedAllPOData));
            #endregion
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

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PopAsync();

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
                                StatusTextBgColor = Color.DarkGreen;
                                ScannedValue = ScannedResult;
                                IsInspEnable = true;
                                InspOpacity = 1.0;
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
                YPSLogger.ReportException(ex, "SearchResultGet method -> in InspVerificationScanViewModel.cs" + Settings.userLoginID);
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
                YPSLogger.ReportException(ex, "SaveAndClearSearch method -> in InspVerificationScanViewModel.cs" + Settings.userLoginID);
                await trackService.Handleexception(ex);
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
                    //if (Settings.userRoleID != (int)UserRoles.SuperAdmin)
                    //{
                        Settings.POID = podata.POID;

                        //if (podata.TagTaskStatus == 2)
                        //{
                        //    ScannedOn = DateTime.Now.ToString(@"dd/MM/yyyy hh:mm:ss tt");
                        //    StatusText = "Done";
                        //    StatusTextBgColor = Color.DarkGreen;
                        //    IsInspEnable = false;
                        //    InspOpacity = 0.5;
                        //}
                        await Navigation.PushAsync(new QuestionsPage(podata.POTagID));
                    //}
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
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