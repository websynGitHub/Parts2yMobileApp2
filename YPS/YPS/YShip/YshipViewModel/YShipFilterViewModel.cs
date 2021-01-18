using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomRenders;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.YShip.YshipModel;

namespace YPS.ViewModel
{
    public class YShipFilterViewModel : IBase
    {
        #region ICommand and data member declaration
        #region Tab ICommand
        public ICommand ITabBasicDetails { get; set; }
        public ICommand ITabLocationDetails { get; set; }
        public ICommand ITabShipmentDetails { get; set; }
        public ICommand ITabOtherDetails { get; set; }
        #endregion

        #region All Picker ICommand 
        public ICommand ICompanyPicker { get; set; }
        public ICommand ICancelBtn { get; set; }
        public ICommand ICompleteBtn { get; set; }
        public ICommand IShmtTypePicker { get; set; }
        public ICommand IEqmtType1Picker { get; set; }
        public ICommand IEqmtType2Picker { get; set; }
        public ICommand IEqmtType3Picker { get; set; }
        public ICommand IShmtTermPicker { get; set; }
        public ICommand IBkgConfirmedPicker { get; set; }
        private INavigation MyNavigation { get; set; }
        public ICommand OrgSearchCommand { set; get; }
        public ICommand DestSearchCommand { set; get; }
        public ICommand IShipmentLoadedByPicker { get; set; }
        public ICommand ISubmitBtn { get; set; }
        #endregion

        #region Calendar Picker ICommand
        public ICommand ETDCommandDate { set; get; }
        public ICommand ETACommandDate { set; get; }
        public ICommand OriginalRPkupCommandDate { set; get; }
        public ICommand OriginalRDlyDateCmd { set; get; }
        public ICommand OriginalRDeliveryTimeCmd { set; get; }
        #endregion

        #region Time Picker ICommand
        public ICommand PickUpTimeCommand { set; get; }
        public ICommand DeliveryTimeCommand { set; get; }
        public ICommand OriginalRPkupTimeCommand { set; get; }
        #endregion

        #region Data Member
        int yShipID;
        NullableDatePicker myNullableDate;
        YPSService YSFService;
        YShipPickerDataModel pickerAllData;
        List<ShipmentType> shipmentTypes;
        List<LoadType> eqmtTypes;
        List<ShipmentTerm> shipmentTerms;
        List<ShipmentLoadedType> shipmentLoadedTypes;
        bool checkInternet;
        #endregion
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="_Navigation"></param>
        /// <param name="yBkgNo"></param>
        /// <param name="yShipBkgNumber"></param>
        /// <param name="yShipId"></param>
        public YShipFilterViewModel(INavigation _Navigation, string yBkgNo, string yShipBkgNumber, int yShipId)
        {
            try
            {
                #region Tab Change Command
                this.MyNavigation = _Navigation;
                ITabBasicDetails = new Command(async () => await TabBasicDetails());//When clicked on Basic tab
                ITabLocationDetails = new Command(async () => await TabLocationDetails());//When clicked on Location tab
                ITabShipmentDetails = new Command(async () => await TabShipmentDetails());//When clicked on Shipment tab
                ITabOtherDetails = new Command(async () => await TabOtherDetails());//Whan clicked on Other tab
                ICancelBtn = new Command(async () => await TabCancelBtn());//When clicked on Cancel Shipment button
                ICompleteBtn = new Command(async () => await TabCompleteBtn());///When clicked on Complete Shipment Button
                #endregion

                #region show picker in Basic tab
                //When clicked on company dropdown, shows company picker
                ICompanyPicker = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                //When clicked on ShmtType dropdown, shows ShmtType picker
                IShmtTypePicker = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                //When clicked on EqmtType1 dropdown, shows EqmtType1 picker
                IEqmtType1Picker = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                //When clicked on EqmtType2 dropdown, shows EqmtType2 picker
                IEqmtType2Picker = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                //When clicked on EqmtType3 dropdown, shows EqmtType3 picker
                IEqmtType3Picker = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                //When clicked on Bkg Confirm dropdown, shows Bkg Confirm picker
                IBkgConfirmedPicker = new Command<View>((view) =>
                {
                    view?.Focus();
                });
                #endregion

                #region show pickers in Other tab
                //When clicked on ShipmentTerm  dropdown, shows ShipmentTerm picker
                IShmtTermPicker = new Command<View>((view) =>
                {
                    view?.Focus();
                });
                #endregion

                #region show pickers in Location tab
                OrgSearchCommand = new Command<View>((view) =>
                {
                    EntrySearchType = "OrgSearch";
                    if (SearchListItems != null)
                    {
                        SearchListItems.Clear();
                    }
                    SearchContentVForAll = true;
                    NoResultIsVisibleLbl = true;
                    OrgSearchListStack = false;
                });

                DestSearchCommand = new Command<View>((view) =>
                {
                    EntrySearchType = "DestSearch";
                    if (SearchListItems != null)
                    {
                        SearchListItems.Clear();
                    }
                    SearchContentVForAll = true;
                    NoResultIsVisibleLbl = true;
                    OrgSearchListStack = false;
                });
                #endregion

                #region show picker in Shipment tab
                IShipmentLoadedByPicker = new Command<View>((view) =>
                {
                    view?.Focus();
                });
                #endregion

                #region Show Calendar in Location tab
                ETDCommandDate = new Command(tab_ETDDate);
                ETACommandDate = new Command(tab_ETADate);
                OriginalRPkupCommandDate = new Command(tab_OriginalRPkupDate);
                OriginalRDlyDateCmd = new Command(tab_OriginalRDlyDate);
                #endregion

                #region Show Time in Location tab
                PickUpTimeCommand = new Command(tab_PickUpTime);
                DeliveryTimeCommand = new Command(tab_DeliveryTime);
                OriginalRPkupTimeCommand = new Command(tab_OriginalRPkupTime);
                OriginalRDeliveryTimeCmd = new Command(tab_OriginalRDeliveryTime);
                #endregion

                if (yBkgNo != "")
                {
                    yBkg = yBkgNo;
                }

                yShipBkg = yShipBkgNumber;
                this.yShipID = yShipId;

                YSFService = new YPSService();
                shipmentTypes = new List<ShipmentType>();
                eqmtTypes = new List<LoadType>();
                shipmentLoadedTypes = new List<ShipmentLoadedType>();
                shipmentTerms = new List<ShipmentTerm>();

                ISubmitBtn = new Command(async () => await Submitbtn());//This method will be called whenever clicked on Submit button

                GetyShipDetails(yShipId, yBkgNo);

                ChangeLabels();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YShipFilterViewModel method-> in YShipFilterViewModel" + Settings.userLoginID);
                YSFService.Handleexception(ex);
            }
        }

        #region Methods for diffrent events
        /// <summary>
        /// When clicked on Basic tab
        /// </summary>
        /// <returns></returns>
        public async Task TabBasicDetails()
        {
            BasicDetailsStack = true;
            TabBasicVisibility = true;
            TabOpacityBasic = 1;
            LocatinDetailsStack = ShipmentDetailsStack = OtherDetailsStack = false;
            TabLocationVisibility = TabShipmentVisibility = TabOtherVisibility = false;
            TabOpacityLocation = TabOpacityShipment = TabOpacityOther = 0.3;
        }

        /// <summary>
        /// When Clicked on Loacation tab
        /// </summary>
        /// <returns></returns>
        public async Task TabLocationDetails()
        {
            LocatinDetailsStack = true;
            TabLocationVisibility = true;
            TabOpacityLocation = 1;
            BasicDetailsStack = ShipmentDetailsStack = OtherDetailsStack = false;
            TabBasicVisibility = TabShipmentVisibility = TabOtherVisibility = false;
            TabOpacityBasic = TabOpacityShipment = TabOpacityOther = 0.3;
        }

        /// <summary>
        /// When clicked on Shipment tab
        /// </summary>
        /// <returns></returns>
        private async Task TabShipmentDetails()
        {
            ShipmentDetailsStack = true;
            TabShipmentVisibility = true;
            TabOpacityShipment = 1;
            BasicDetailsStack = LocatinDetailsStack = OtherDetailsStack = false;
            TabBasicVisibility = TabLocationVisibility = TabOtherVisibility = false;
            TabOpacityBasic = TabOpacityLocation = TabOpacityOther = 0.3;
        }

        /// <summary>
        /// When clicked on Other tab
        /// </summary>
        /// <returns></returns>
        public async Task TabOtherDetails()
        {
            OtherDetailsStack = true;
            TabOtherVisibility = true;
            TabOpacityOther = 1;
            BasicDetailsStack = LocatinDetailsStack = ShipmentDetailsStack = false;
            TabBasicVisibility = TabLocationVisibility = TabShipmentVisibility = false;
            TabOpacityBasic = TabOpacityLocation = TabOpacityShipment = 0.3;
        }

        /// <summary>
        /// This method will be called when the company drop-down is chosen in the basic
        /// tab, this drop-down is disabled by default so that user can't change values.
        /// </summary>
        private void SelectedCompany_TapEvent()
        {
            if (SelectedCompanyValue != null)
            {
                CompanyDefaultLblValue = SelectedCompanyValue;
                var items = CompanylList.Where(X => X == CompanyDefaultLblValue).FirstOrDefault();
            }
        }

        /// <summary>
        /// This method will be called when shipment type dropdown is choosen in the basic tab
        /// </summary>
        private void SelectedShmtType_TapEvent()
        {
            if (SelectedShmtTypeValue != null)
            {
                ShmtTypeDefaultLblValue = SelectedShmtTypeValue;
                var items = shipmentTypes.Where(X => X.DisplayText == ShmtTypeDefaultLblValue).FirstOrDefault();
                ShmtTypeID = items.ID;
            }
        }

        /// <summary>
        /// This method will be called when EqmtType1 type dropdown is choosen in the basic tab 
        /// </summary>
        private void SelectedEqmtType1_TapEvent()
        {
            if (SelectedEqmtType1Value != null)
            {
                EqmtType1DefaultLblValue = SelectedEqmtType1Value;

                if (EqmtType1DefaultLblValue.ToLower().Trim() == "select")
                {
                    FBgColorType1Qty = Color.Gray;
                }
                FBgColorEqmtType1 = Color.Gray;
                var items = eqmtTypes.Where(X => X.DisplayText == EqmtType1DefaultLblValue).FirstOrDefault();
                EqmtType1ID = items.ID;
            }
        }

        /// <summary>
        /// This method will be called when EqmtType2 type dropdown is choosen in the basic tab 
        /// </summary>
        private void SelectedEqmtType2_TapEvent()
        {
            if (SelectedEqmtType2Value != null)
            {
                EqmtType2DefaultLblValue = SelectedEqmtType2Value;

                if (EqmtType2DefaultLblValue.ToLower().Trim() == "select")
                {
                    FBgColorType2Qty = Color.Gray;
                }

                FBgColorEqmtType2 = Color.Gray;
                var items = eqmtTypes.Where(X => X.DisplayText == EqmtType2DefaultLblValue).FirstOrDefault();
                EqmtType2ID = items.ID;
            }
        }

        /// <summary>
        /// This method will be called when EqmtType3 type dropdown is choosen in the basic tab
        /// </summary>
        private void SelectedEqmtType3_TapEvent()
        {
            if (SelectedEqmtType3Value != null)
            {
                EqmtType3DefaultLblValue = SelectedEqmtType3Value;

                if (EqmtType3DefaultLblValue.ToLower().Trim() == "select")
                {
                    FBgColorType3Qty = Color.Gray;
                }

                FBgColorEqmtType3 = Color.Gray;
                var items = eqmtTypes.Where(X => X.DisplayText == EqmtType3DefaultLblValue).FirstOrDefault();
                EqmtType3ID = items.ID;
            }
        }

        /// <summary>
        /// Summing up all Eqmt Qty
        /// </summary>
        public void AddTotalEqmtQty()
        {
            try
            {
                int Type1Qty = Convert.ToInt32(string.IsNullOrEmpty(EntryEqmtType1Qty) ? "0" : EntryEqmtType1Qty);
                int Type2Qty = Convert.ToInt32(string.IsNullOrEmpty(EntryEqmtType2Qty) ? "0" : EntryEqmtType2Qty);
                int Type3Qty = Convert.ToInt32(string.IsNullOrEmpty(EntryEqmtType3Qty) ? "0" : EntryEqmtType3Qty);

                var total = Type1Qty + Type2Qty + Type3Qty;
                TotalEqmtQty = total.ToString();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "AddTotalEqmtQty method -> in YShipFilterViewModel.cs " + Settings.userLoginID);
                YSFService.Handleexception(ex);
            }

        }

        /// <summary>
        /// This method will be called when Bkg Confirm type dropdown is choosen in the basic tab 
        /// </summary>
        private void SelectedBkgCnfrmedValue_TapEvent()
        {
            if (SelectedBkgCnfrmedValue != null)
            {
                BkgCnfrmedDfltLblVal = SelectedBkgCnfrmedValue;
                var items = pickerAllData.data.BkgConfirmed.Where(X => X.DisplayText == SelectedBkgCnfrmedValue).FirstOrDefault();
                BkgConfirmedId = items.ID;
            }
        }

        /// <summary>
        /// When clicked on ETD date picker
        /// </summary>
        /// <param name="obj"></param>
        private void tab_ETDDate(object obj)
        {
            myNullableDate = obj as NullableDatePicker;
            myNullableDate.Focus();
            Settings.dateBaseOnPickerVal = "ETDDate";
            myNullableDate.PropertyChanged += Data_PropertyChanged;
        }

        /// <summary>
        /// When clicked on ETA date picker
        /// </summary>
        /// <param name="obj"></param>
        private void tab_ETADate(object obj)
        {
            myNullableDate = obj as NullableDatePicker;
            myNullableDate.Focus();
            Settings.dateBaseOnPickerVal = "ETADate";
            myNullableDate.PropertyChanged += Data_PropertyChanged;
        }

        /// <summary>
        /// When clicked on original Pickup Date
        /// </summary>
        /// <param name="obj"></param>
        private void tab_OriginalRPkupDate(object obj)
        {
            myNullableDate = obj as NullableDatePicker;
            myNullableDate.Focus();
            Settings.dateBaseOnPickerVal = "OriginalRPkupDate";
            myNullableDate.PropertyChanged += Data_PropertyChanged;
        }

        /// <summary>
        /// When Clicked on Original Delivery Date
        /// </summary>
        /// <param name="obj"></param>
        private void tab_OriginalRDlyDate(object obj)
        {
            myNullableDate = obj as NullableDatePicker;
            myNullableDate.Focus();
            Settings.dateBaseOnPickerVal = "OriginalRDlyDate";
            myNullableDate.PropertyChanged += Data_PropertyChanged;
        }

        /// <summary>
        /// When date is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Data_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var date = sender as NullableDatePicker;
                string selectedDate = date.Date.ToString(Settings.DateFormatformAPI);

                if (!string.IsNullOrEmpty(selectedDate))
                {
                    if (Settings.dateBaseOnPickerVal.ToLower().Trim() == "etddate")
                    {
                        ETDDatePickerLbl = OriginalRPkupDateLbl = selectedDate;
                        myNullableDate.CleanDate();

                    }
                    else if (Settings.dateBaseOnPickerVal.ToLower().Trim() == "etadate")
                    {
                        ETADatePickerLbl = OriginalRDlyDateLbl = selectedDate;
                        myNullableDate.CleanDate();
                    }

                    else if (Settings.dateBaseOnPickerVal.ToLower().Trim() == "etavialocdate")
                    {
                        ETAViaLocDateLbl = selectedDate;
                        myNullableDate.CleanDate();
                    }
                    else if (Settings.dateBaseOnPickerVal.ToLower().Trim() == "atavialocdate")
                    {
                        ATAViaLocDateLbl = selectedDate;
                        myNullableDate.CleanDate();
                    }
                    else if (Settings.dateBaseOnPickerVal.ToLower().Trim() == "etadestdate")
                    {
                        ETADestDateLbl = selectedDate;
                        myNullableDate.CleanDate();
                    }
                    else if (Settings.dateBaseOnPickerVal.ToLower().Trim() == "atadestdate")
                    {
                        ATADestDateLbl = selectedDate;
                        myNullableDate.CleanDate();
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YShipFilterViewModel method-> in datechanged event " + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on PickUp time
        /// </summary>
        /// <param name="obj"></param>
        private void tab_PickUpTime(object obj)
        {
            var time = obj as TimePicker;
            time.Focus();
            Settings.dateBaseOnPickerVal = "PickUpTime";
            time.PropertyChanged += Time_PropertyChanged;
        }

        /// <summary>
        /// Gets called when clicked on Delivery time field
        /// </summary>
        /// <param name="obj"></param>
        private void tab_DeliveryTime(object obj)
        {
            var time = obj as TimePicker;
            time.Focus();
            Settings.dateBaseOnPickerVal = "DeliveryTime";
            time.PropertyChanged += Time_PropertyChanged;
        }

        /// <summary>
        /// Gets called when clicked Origin Pickup time field
        /// </summary>
        /// <param name="obj"></param>
        private void tab_OriginalRPkupTime(object obj)
        {
            var time = obj as TimePicker;
            time.Focus();
            Settings.dateBaseOnPickerVal = "OriginalRPkupTime";
            time.PropertyChanged += Time_PropertyChanged;
        }

        /// <summary>
        /// Gets called when clicked on Origin Delivery Time
        /// </summary>
        /// <param name="obj"></param>
        private void tab_OriginalRDeliveryTime(object obj)
        {
            var time = obj as TimePicker;
            time.Focus();
            Settings.dateBaseOnPickerVal = "OriginalRDeliveryTime";
            time.PropertyChanged += Time_PropertyChanged;
        }

        /// <summary>
        /// Gets called when time is selected from the time picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Time_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                TimePicker time = sender as TimePicker;
                string setTimeToLBl;

                if (Settings.dateBaseOnPickerVal.ToLower().Trim() == "pickuptime")
                {
                    setTimeToLBl = TimeFormating(time);
                    PickUpTimeLbl = OriginalRPkupTimeLbl = setTimeToLBl;
                }
                else if (Settings.dateBaseOnPickerVal.ToLower().Trim() == "deliverytime")
                {
                    setTimeToLBl = TimeFormating(time);
                    DeliveryTimeLbl = OriginalRDeliveryTimeLbl = setTimeToLBl;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Time_PropertyChanged constructor -> in FilterDataViewModel " + Settings.userLoginID);
                YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Used to set the time format
        /// </summary>
        /// <param name="timep"></param>
        /// <returns></returns>
        private string TimeFormating(TimePicker timep)
        {
            try
            {
                TimeSpan timeS = timep.Time;
                int hour = timeS.Hours;
                int minutes = timeS.Minutes;
                String timeSet = "", min = "";

                if (hour > 12)
                {
                    hour -= 12;
                    timeSet = "PM";
                }
                else if (hour == 0)
                {
                    hour += 12;
                    timeSet = "AM";
                }
                else if (hour == 12)
                {
                    timeSet = "PM";
                }
                else
                {
                    timeSet = "AM";
                }

                if (minutes < 10)
                    min = "0" + minutes;
                else
                    min = minutes.ToString();

                string aTime = new StringBuilder().Append(hour).Append(':').Append(min).Append(" ").Append(timeSet).ToString();

                return aTime;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TimeFormating constructor -> in FilterDataViewModel " + Settings.userLoginID);
                YSFService.Handleexception(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets called when selected Shipment Loaded by.
        /// </summary>
        private void SelectedShipmentLoadedBy_TapEvent()
        {
            if (SelectedShipmentLoadedBy != null)
            {
                ShipmentLoadedByDefaultLbl = SelectedShipmentLoadedBy;
                var items = shipmentLoadedTypes.Where(X => X.DisplayText == ShipmentLoadedByDefaultLbl).FirstOrDefault();
                ShipmentLoadedByID = items.ID;
            }
        }

        /// <summary>
        /// Gets caalled when selected Shipment Term
        /// </summary>
        private void SelectedhmtTermItem_Tab()
        {
            if (SelectedhmtTermItem != null)
            {
                ShmtTermDefaultValue = SelectedhmtTermItem;
                var items = shipmentTerms.Where(X => X.DisplayText == ShmtTermDefaultValue).FirstOrDefault();
                ShmtTermID = items.ID;
            }
        }
        #endregion

        /// <summary>
        /// This method is for the dynamic text change.
        /// </summary>
        public void ChangeLabels()
        {
            try
            {
                labelobj = new bookingDetailslabelclass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> bookingDetailLabels = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (bookingDetailLabels.Count > 0)
                    {
                        var Owner = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.CompanyName.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var yBkgNumber = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.yBkgNumber.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var yShipBkgNo = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.yShipBkgNo.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var BkgRef = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.BkgRefNo.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ShipmntType = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ShipmntType.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var EqmtQtyTotal = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EqmtQtyTotal.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var BkgConfirmed = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.BkgConfirmed.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var EqmtType1 = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EqmtType1.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var Type1Qty = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Type1Qty.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var EqmtType2 = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EqmtType2.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var Type2Qty = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Type2Qty.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var EqmtType3 = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EqmtType3.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var Type3Qty = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Type3Qty.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ShipmentLoadedBy = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ShipmentLoadedBy.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ReqPickupDate = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ReqPickupDate.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ShipmentOpenedby = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ShipmentOpenedby.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var Shipper = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Shipper.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var OriSearch = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.OrgSearch.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var OriCntryCd = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.OrgCntryCode.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var OriCntry = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.OrgCntryName.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var OriLocCd = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.OrgLocCode.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var OriLoc = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.OrgLocName.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ReqPickupAddress = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.PickupLocation.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ReqPickupDt = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ReqPickupDt.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var PickUpTime = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.PickUpTime.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var PickupPIC = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.PickupPIC.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ShipToParty = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ShipToParty.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var DestSearch = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.DestSearch.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var DestCntryCd = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.DestCntryCode.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var DestCntry = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.DestCntryName.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var DestLocCd = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.DestLocCode.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var DestLoc = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.DestLocName.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ReqDeliveryAddress = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.DeliveryLocation.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ReqDeliveryDt = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ReqDeliveryDate.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var DeliveryTime = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.DeliveryTime.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var DeliveryPIC = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.DestinationPIC.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var CustRefNo = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.CustRefNo.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var InvoiceNo = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.InvoiceNo.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var ShmtTerm = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ShmtTerm.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var GeneralDesc = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.GeneralDesc.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var SpecialCargoHandlingInstr = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.SpecialCargoHandlingInstr.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var Currency = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Currency.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var TotalAmount = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TotalAmount.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var TotalPkg = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TotalPkg.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var PkgUnit = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.PkgUnit.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var TotalM3 = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TotalM3.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var TotalGrossWt = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TotalGrossWt.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var TotalNetWt = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TotalNetWt.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var submit = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Submit).Select(c => c.LblText).FirstOrDefault();
                        var completeShipment = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.CompleteShipment).Select(c => c.LblText).FirstOrDefault();
                        var cancelShipment = bookingDetailLabels.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.CancelShipment).Select(c => c.LblText).FirstOrDefault();

                        labelobj.CompanyName = Owner != null ? Owner + " *" : "Owner" + " *";
                        labelobj.yBkgNumber = yBkgNumber != null ? yBkgNumber : "yBkg Number";
                        labelobj.yShipBkgNo = yShipBkgNo != null ? yShipBkgNo : "yShip Bkg#";
                        labelobj.BkgRefNo = BkgRef != null ? BkgRef : "Bkg Ref#";
                        labelobj.ShipmntType = ShipmntType != null ? ShipmntType : "Shipmnt Type";
                        labelobj.EqmtQtyTotal = EqmtQtyTotal != null ? EqmtQtyTotal : "Eqmt Qty Total";
                        labelobj.BkgConfirmed = BkgConfirmed != null ? BkgConfirmed : "Bkg Confirmed";
                        labelobj.EqmtType1 = EqmtType1 != null ? EqmtType1 + " 1 *" : "Eqmt Type 1 *";
                        labelobj.Type1Qty = Type1Qty != null ? Type1Qty + " *" : "Type 1 Qty *";
                        labelobj.EqmtType2 = EqmtType2 != null ? EqmtType2 + " 2" : "Eqmt Type 2";
                        labelobj.Type2Qty = Type2Qty != null ? Type2Qty : "Type 2 Qty";
                        labelobj.EqmtType3 = EqmtType3 != null ? EqmtType3 + " 3" : "Eqmt Type 3";
                        labelobj.Type3Qty = Type3Qty != null ? Type3Qty : "Type 3 Qty";
                        labelobj.ShipmentLoadedBy = ShipmentLoadedBy != null ? ShipmentLoadedBy : "Shipment Loaded By";
                        labelobj.ReqPickupDate = ReqPickupDate != null ? ReqPickupDate : "Req Pickup Dt";
                        labelobj.ShipmentOpenedby = ShipmentOpenedby != null ? ShipmentOpenedby : "Shipment Opened by";
                        labelobj.Shipper = Shipper != null ? Shipper : "Shipper";
                        labelobj.OrgSearch = OriSearch != null ? OriSearch : "Ori Search";
                        labelobj.OrgCntryCode = OriCntryCd != null ? OriCntryCd : "Ori Cntry Cd";
                        labelobj.OrgCntryName = OriCntry != null ? OriCntry : "Ori Cntry";
                        labelobj.OrgLocCode = OriLocCd != null ? OriLocCd : "Ori Loc Cd";
                        labelobj.OrgLocName = OriLoc != null ? OriLoc : "Org Loc";
                        labelobj.PickupLocation = ReqPickupAddress != null ? ReqPickupAddress : "Req Pickup Address";
                        labelobj.ReqPickupDt = ReqPickupDt != null ? ReqPickupDt : "Req Pickup Dt";
                        ETDDatePickerLbl = OriginalRPkupDateLbl = labelobj.ReqPickupDt;
                        labelobj.PickUpTime = PickUpTime != null ? PickUpTime : "PickUp Time";
                        PickUpTimeLbl = OriginalRPkupTimeLbl = labelobj.PickUpTime;
                        labelobj.PickupPIC = PickupPIC != null ? PickupPIC : "Pickup PIC";
                        labelobj.ShipToParty = ShipToParty != null ? ShipToParty : "Ship To Party";
                        labelobj.DestSearch = DestSearch != null ? DestSearch : "Dest Search";
                        labelobj.DestCntryCode = DestCntryCd != null ? DestCntryCd : "Dest Cntry Cd";
                        labelobj.DestCntryName = DestCntry != null ? DestCntry : "Dest Cntry";
                        labelobj.DestLocCode = DestLocCd != null ? DestLocCd : "Dest Loc Cd";
                        labelobj.DestLocName = DestLoc != null ? DestLoc : "Dest Loc";
                        labelobj.DeliveryLocation = ReqDeliveryAddress != null ? ReqDeliveryAddress : "Req Delivery Address";
                        labelobj.ReqDeliveryDate = ReqDeliveryDt != null ? ReqDeliveryDt : "Req Delivery Dt";
                        ETADatePickerLbl = OriginalRDlyDateLbl = labelobj.ReqDeliveryDate;
                        labelobj.DeliveryTime = DeliveryTime != null ? DeliveryTime : "Delivery Time";
                        DeliveryTimeLbl = OriginalRDeliveryTimeLbl = labelobj.DeliveryTime;
                        labelobj.DestinationPIC = DeliveryPIC != null ? DeliveryPIC : "Delivery PIC";
                        labelobj.InvoiceNo = InvoiceNo != null ? InvoiceNo : "Invoice#";
                        labelobj.CustRefNo = CustRefNo != null ? CustRefNo : "Cust RefNo";
                        labelobj.ShmtTerm = ShmtTerm != null ? ShmtTerm : "Shmt Term";
                        labelobj.SpecialCargoHandlingInstr = SpecialCargoHandlingInstr != null ? SpecialCargoHandlingInstr : "Special Cargo Handling Instr";
                        labelobj.GeneralDesc = GeneralDesc != null ? GeneralDesc : "General Desc";
                        labelobj.Currency = Currency != null ? Currency : "Currency";
                        labelobj.TotalAmount = TotalAmount != null ? TotalAmount : "Total Amount";
                        labelobj.TotalPkg = TotalPkg != null ? TotalPkg : "TotalPkg";
                        labelobj.PkgUnit = PkgUnit != null ? PkgUnit : "Pkg Unit";
                        labelobj.TotalM3 = TotalM3 != null ? TotalM3 : "Total M3";
                        labelobj.TotalGrossWt = TotalGrossWt != null ? TotalGrossWt : "Total Gross Wt";
                        labelobj.TotalNetWt = TotalNetWt != null ? TotalNetWt : "Total Net Wt";
                        labelobj.Submit = submit != null ? submit : "Submit";
                        labelobj.CompleteShipment = completeShipment != null ? completeShipment : "Complete Shipment";
                        labelobj.CancelShipment = cancelShipment != null ? cancelShipment : "Cancel Shipment";
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ChangeLabels method-> in YShipFilterViewModel" + Settings.userLoginID);
                YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called whenever clicked on Cancel Shipment button which is in basic tab
        /// </summary>
        /// <returns></returns>
        public async Task TabCancelBtn()
        {
            try
            {
                bool answer = await App.Current.MainPage.DisplayAlert("Cancel", "Are you sure?", "Yes,Cancel", "No");

                if (answer)
                {
                    IndicatorVisibility = true;
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        /// Calling CloseyShip API to cancel shipment.
                        var result = await YSFService.CloseYShipSevice(yShipID, 0, 1);

                        if (result.status != 0 || result != null)
                        {
                            ShowHideClCmpltStack = false;
                            SubmitStackVisible = false;
                            ShowHideClCmpltLbl = true;
                            MessageForClCmpltbl = "This Shipment was Cancelled.";
                            CancelColorLbl = Color.Red;
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TabCancelBtn method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called whenever clicked on Complete Shipment button which is in basic tab
        /// </summary>
        /// <returns></returns>
        public async Task TabCompleteBtn()
        {
            try
            {
                bool answer = await App.Current.MainPage.DisplayAlert("Complete", "Are you sure?", "Yes,Complete", "No");

                if (answer)
                {
                    IndicatorVisibility = true;
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        /// Calling CloseyShip API to completed shipment.
                        var result = await YSFService.CloseYShipSevice(yShipID, 1, 0);

                        if (result.status != 0 || result != null)
                        {
                            ShowHideClCmpltStack = false;
                            SubmitStackVisible = false;
                            ShowHideClCmpltLbl = true;
                            MessageForClCmpltbl = "This Shippment was Completed.";
                            CancelColorLbl = Color.Green;
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TabCompleteBtn method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Binding data for all the tabs in booking details page.
        /// </summary>
        /// <param name="yShipId"></param>
        /// <param name="yBkgNo"></param>
        public async void GetyShipDetails(int yShipId, string yBkgNo)
        {
            try
            {
                YPSLogger.TrackEvent("YShipFilterViewModel", "in GetyShipDetails method " + DateTime.Now + " UserId: " + Settings.userLoginID);
                IndicatorVisibility = true;

                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    /// Calling API to get all yship masters data.
                    pickerAllData = await YSFService.GetAllYShipPickerData(Settings.userLoginID);

                    if (pickerAllData != null && pickerAllData.status != 0)
                    {
                        /// This method will bind all master's data to all masters pickers.
                        await BindAllPickers();
                    }
                    else
                    {
                    }

                    /// Calling API to get yship details data.
                    var rsltData = await YSFService.GetyShipDetailService(yShipId);

                    if (rsltData != null)
                    {
                        if (rsltData.status != 0)
                        {
                            /// By using these four methods binding data to all booking detail page tabs.
                            await BindBasicDetailsTab(rsltData);
                            await BindLocationTab(rsltData);
                            await BindShipmentTab(rsltData);
                            await BindOtherTab(rsltData);

                            if (rsltData.data.Cancel == 1 || rsltData.data.Complete == 1)
                            {
                                ShowHideClCmpltStack = false;
                                SubmitStackVisible = false;

                                if (rsltData.data.Cancel == 1)
                                {
                                    MessageForClCmpltbl = "This Shipment was Cancelled.";
                                    CancelColorLbl = Color.Red;
                                    ShowHideClCmpltLbl = true;
                                }
                                if (rsltData.data.Complete == 1)
                                {
                                    ShowHideClCmpltLbl = true;
                                    MessageForClCmpltbl = "This Shippment was Completed.";
                                    CancelColorLbl = Color.Green;
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YShipFilterViewModel method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method is used to bind company, equipment types, shipment type, shipment load type, shipment 
        /// terms & Bkg confirm dropdowns.
        /// </summary>
        /// <returns></returns>
        private async Task BindAllPickers()
        {
            try
            {
                //Binding company dropdown
                List<Company> cmpyLists = new List<Company>();
                cmpyLists.Add(new Company() { DisplayText = "Select", ID = 0 });
                cmpyLists.AddRange(pickerAllData.data.Company);
                CompanylList = new ObservableCollection<string>(cmpyLists.Select(X => X.DisplayText).ToList());

                //Binding equipment type 1,2 & 3 dropdowns
                eqmtTypes.Add(new LoadType() { DisplayText = "Select", ID = 0 });
                eqmtTypes.AddRange(pickerAllData.data.LoadType);
                EqmtType1List = EqmtType2List = EqmtType3List = new ObservableCollection<string>(eqmtTypes.Select(X => X.DisplayText));

                //Binding shipment type dropdown
                shipmentTypes.Add(new ShipmentType() { DisplayText = "Select", ID = 0 });
                shipmentTypes.AddRange(pickerAllData.data.ShipmentType);
                ShmtTypeList = new ObservableCollection<string>(shipmentTypes.Select(X => X.DisplayText).ToList());

                //Binding shipment load type dropdown
                shipmentLoadedTypes.Add(new ShipmentLoadedType() { DisplayText = "Select", ID = 0 });
                shipmentLoadedTypes.AddRange(pickerAllData.data.ShipmentLoadedType);
                ShipmentLoadedByList = new ObservableCollection<string>(shipmentLoadedTypes.Select(X => X.DisplayText).ToList());

                //Binding shipment terms dropdown
                shipmentTerms.Add(new ShipmentTerm() { DisplayText = "Select", ID = 0 });
                shipmentTerms.AddRange(pickerAllData.data.ShipmentTerm);
                ShmtTermSelectList = new ObservableCollection<string>(shipmentTerms.Select(X => X.DisplayText).ToList());

                //Binding Bkg confirm dropdown
                BkgCnfrmedList = new ObservableCollection<string>(pickerAllData.data.BkgConfirmed.Select(X => X.DisplayText));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindAllPickers method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Binding data for basic details tab
        /// </summary>
        /// <param name="rsltData"></param>
        /// <returns></returns>
        private async Task BindBasicDetailsTab(GetyShipDetailsResponse rsltData)
        {
            try
            {
                string _EqmtType1, _EqmtType2, _EqmtType3;

                ShmtTypeID = rsltData.data.ShmtType;
                EqmtType1ID = rsltData.data.EqmtType1;
                EqmtType2ID = rsltData.data.EqmtType2;
                EqmtType3ID = rsltData.data.EqmtType3;
                BkgConfirmedId = rsltData.data.BkgConfirmed;

                _EqmtType1 = eqmtTypes.Where(X => X.ID == rsltData.data.EqmtType1).FirstOrDefault().DisplayText;
                _EqmtType2 = eqmtTypes.Where(X => X.ID == rsltData.data.EqmtType2).FirstOrDefault().DisplayText;
                _EqmtType3 = eqmtTypes.Where(X => X.ID == rsltData.data.EqmtType3).FirstOrDefault().DisplayText;

                CompanyDefaultLblValue = rsltData.data.CompanyID != 0 ? pickerAllData.data.Company.Where(X => X.ID == rsltData.data.CompanyID).FirstOrDefault().Name : CompanyDefaultLblValue;
                BkgRef = !string.IsNullOrEmpty(rsltData.data.BkgRefNo) ? rsltData.data.BkgRefNo : BkgRef;
                ShmtTypeDefaultLblValue = rsltData.data.ShmtType != 0 ? shipmentTypes.Where(X => X.ID == rsltData.data.ShmtType).FirstOrDefault().DisplayText : ShmtTypeDefaultLblValue;
                EqmtType1DefaultLblValue = _EqmtType1 != null ? _EqmtType1 : EqmtType1DefaultLblValue;
                EntryEqmtType1Qty = rsltData.data.EqmtType1Qty != 0 ? rsltData.data.EqmtType1Qty.ToString() : EntryEqmtType1Qty;
                EqmtType2DefaultLblValue = _EqmtType2 != null ? _EqmtType2 : EqmtType2DefaultLblValue;
                EntryEqmtType2Qty = rsltData.data.EqmtType2Qty != 0 ? rsltData.data.EqmtType2Qty.ToString() : EntryEqmtType2Qty;
                EqmtType3DefaultLblValue = _EqmtType3 != null ? _EqmtType3 : EqmtType3DefaultLblValue;
                EntryEqmtType3Qty = rsltData.data.EqmtType3Qty != 0 ? rsltData.data.EqmtType3Qty.ToString() : EntryEqmtType3Qty;
                TotalEqmtQty = rsltData.data.TotalEqmtQty != 0 ? rsltData.data.TotalEqmtQty.ToString() : TotalEqmtQty;
                BkgCnfrmedDfltLblVal = rsltData.data.BkgConfirmed != null ? pickerAllData.data.BkgConfirmed.Where(X => X.ID == rsltData.data.BkgConfirmed).FirstOrDefault().DisplayText : BkgCnfrmedDfltLblVal;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindBasicDetailsTab method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Binding data to the location tab
        /// </summary>
        /// <param name="rsltData"></param>
        /// <returns></returns>
        private async Task BindLocationTab(GetyShipDetailsResponse rsltData)
        {
            try
            {
                EtyPickupPIC = !string.IsNullOrEmpty(rsltData.data.PickupPIC) ? rsltData.data.PickupPIC : "";
                EtyDestinationPIC = !string.IsNullOrEmpty(rsltData.data.DestinationPIC) ? rsltData.data.DestinationPIC : EtyDestinationPIC;
                EtyShipper = !string.IsNullOrEmpty(rsltData.data.Shipper) ? rsltData.data.Shipper : EtyShipper;
                EtyShipToParty = rsltData.data.ShipToParty != "" ? rsltData.data.ShipToParty : EtyShipToParty;
                OrgSearchLocationLbl = rsltData.data.OrgSearch != "" ? rsltData.data.OrgSearch : OrgSearchLocationLbl;
                OrgSearchLocationLblID = rsltData.data.OrgLocationID != 0 ? rsltData.data.OrgLocationID : OrgSearchLocationLblID;
                EtyOrgCntryCode = rsltData.data.OrgCntryCode != "" ? rsltData.data.OrgCntryCode : EtyOrgCntryCode;
                EtyOrgCntryName = rsltData.data.OrgCntryName != "" ? rsltData.data.OrgCntryName : EtyOrgCntryName;
                EtyOrgLocCode = rsltData.data.OrgLocCode != "" ? rsltData.data.OrgLocCode : EtyOrgLocCode;
                EtyOrgLocName = rsltData.data.OrgLocName != "" ? rsltData.data.OrgLocName : EtyOrgLocName;
                EtyPickupLocation = rsltData.data.PickupLocation != "" ? rsltData.data.PickupLocation : EtyPickupLocation;
                DestSearchLocationLbl = rsltData.data.DestSearch != "" ? rsltData.data.DestSearch : DestSearchLocationLbl;
                DestSearchLocationLblID = rsltData.data.DestLocationID != 0 ? rsltData.data.DestLocationID : DestSearchLocationLblID;
                EtyDestCntryCode = rsltData.data.DestCntryCode != "" ? rsltData.data.DestCntryCode : EtyDestCntryCode;
                EtyDestCntryName = rsltData.data.DestCntryName != "" ? rsltData.data.DestCntryName : EtyDestCntryName;
                EtyDestLocCode = rsltData.data.DestLocCode != "" ? rsltData.data.DestLocCode : EtyDestLocCode;
                EtyDestLocName = rsltData.data.DestLocName != "" ? rsltData.data.DestLocName : EtyDestLocName;
                EtyDeliveryLocation = rsltData.data.DeliveryLocation != "" ? rsltData.data.DeliveryLocation : EtyDeliveryLocation;

                if (rsltData.data.ETD != "")
                {
                    ETDDatePickerLbl = OriginalRPkupDateLbl = rsltData.data.ETD;
                }

                if (rsltData.data.PickUpTime != "")
                {
                    PickUpTimeLbl = OriginalRPkupTimeLbl = rsltData.data.PickUpTime;
                }

                if (rsltData.data.ETA != "")
                {
                    ETADatePickerLbl = OriginalRDlyDateLbl = rsltData.data.ETA;
                }

                if (rsltData.data.DeliveryTime != "")
                {
                    DeliveryTimeLbl = OriginalRDeliveryTimeLbl = rsltData.data.DeliveryTime;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindLocationTab method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Binding data to the shipment tab
        /// </summary>
        /// <param name="rsltData"></param>
        /// <returns></returns>
        private async Task BindShipmentTab(GetyShipDetailsResponse rsltData)
        {
            try
            {
                ShipmentLoadedByID = rsltData.data.ShipmentLoadedBy;

                ShipmentLoadedByDefaultLbl = rsltData.data.ShipmentLoadedBy != 0 ? shipmentLoadedTypes.Where(X => X.ID == rsltData.data.ShipmentLoadedBy).FirstOrDefault().DisplayText : ShipmentLoadedByDefaultLbl;
                EtyShipmentOpenedby = rsltData.data.ShipmentOpenedby != "" ? rsltData.data.ShipmentOpenedby : EtyShipmentOpenedby;

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindShipmentTab method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Binding data to the other tab
        /// </summary>
        /// <param name="rsltData"></param>
        /// <returns></returns>
        private async Task BindOtherTab(GetyShipDetailsResponse rsltData)
        {
            try
            {
                ShmtTermID = rsltData.data.ShmtTerm;
                EtyInvNo = rsltData.data.InvNo != "" ? rsltData.data.InvNo : EtyInvNo;
                EtyCustRefNo = rsltData.data.CustRefNo != "" ? rsltData.data.CustRefNo : EtyCustRefNo;
                ShmtTermDefaultValue = rsltData.data.ShmtTerm != 0 ? shipmentTerms.Where(X => X.ID == rsltData.data.ShmtTerm).FirstOrDefault().DisplayText : ShmtTermDefaultValue;
                EtySpecialCargoHandlingInstr = rsltData.data.SpecialCargoHandlingInstructions != "" ? rsltData.data.SpecialCargoHandlingInstructions : EtySpecialCargoHandlingInstr;
                EtyGeneralDescription = rsltData.data.GeneralDescription != "" ? rsltData.data.GeneralDescription : EtyGeneralDescription;
                EtyCurrency = rsltData.data.Currency.ToString() != "0" ? rsltData.data.Currency.ToString() : EtyCurrency;
                EtyTotalAmount = rsltData.data.TotalAmount != 0 ? rsltData.data.TotalAmount : EtyTotalAmount;
                EtyTotalPkg = rsltData.data.TtlPkg != 0 ? rsltData.data.TtlPkg : EtyTotalPkg;
                EtyPkgUnit = rsltData.data.PkgUnit != "" ? rsltData.data.PkgUnit.ToString() : EtyPkgUnit;
                EtyTotalM3 = rsltData.data.TtlM3 != 0 ? rsltData.data.TtlM3 : EtyTotalM3;
                EtyTotalGrossWt = rsltData.data.TtlGrossWt != 0 ? rsltData.data.TtlGrossWt : EtyTotalGrossWt;
                EtyTotalNetWt = rsltData.data.TtlNetWt != 0 ? rsltData.data.TtlNetWt : EtyTotalNetWt;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindOtherTab method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// When clicked on Submit button present in Booking Details page
        /// </summary>
        /// <returns></returns>
        private async Task Submitbtn()
        {
            try
            {
                if (EqmtType1DefaultLblValue.ToLower().Trim() != "select")
                {
                    if (!String.IsNullOrEmpty(EntryEqmtType1Qty))
                    {
                        UpdateBookingDetails();
                    }
                    else
                    {
                        FBgColorType1Qty = Color.Red;
                    }
                }
                else
                {
                    FBgColorEqmtType1 = Color.Red;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Submitbtn method-> in GetyShipDetails()" + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is used to update the booking details into the database through API.
        /// </summary>
        private async void UpdateBookingDetails()
        {
            try
            {
                IndicatorVisibility = true;
                /// Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    UpdateyShipDetails UyShipDetails = new UpdateyShipDetails();

                    /// Capturing the booking details from Basic, Location, Shipment & Other tabs into UyShipDetails model.
                    UyShipDetails.UpdatedBy = Settings.userLoginID;
                    UyShipDetails.yShipId = yShipID;
                    UyShipDetails.yBkgNo = yBkg;
                    UyShipDetails.yShipBkgNumber = yShipBkg;
                    UyShipDetails.BkgRefNo = BkgRef;
                    UyShipDetails.ShmtType = ShmtTypeID;
                    UyShipDetails.EqmtType1 = EqmtType1ID;
                    UyShipDetails.EqmtType1Qty = Convert.ToInt32(EntryEqmtType1Qty);
                    UyShipDetails.EqmtType2 = EqmtType2ID;
                    var Qty2 = EntryEqmtType2Qty == "" ? 0 : Convert.ToInt32(EntryEqmtType2Qty);
                    UyShipDetails.EqmtType2Qty = Qty2;
                    UyShipDetails.EqmtType3 = EqmtType3ID;
                    var Qty3 = EntryEqmtType3Qty == "" ? 0 : Convert.ToInt32(EntryEqmtType3Qty);
                    UyShipDetails.EqmtType3Qty = Qty3;
                    UyShipDetails.TotalEqmtQty = Convert.ToInt32(TotalEqmtQty);
                    UyShipDetails.BkgConfirmed = BkgConfirmedId;
                    UyShipDetails.PickupPIC = EtyPickupPIC;
                    UyShipDetails.DestinationPIC = EtyDestinationPIC;
                    UyShipDetails.Shipper = EtyShipper;
                    UyShipDetails.ShipToParty = EtyShipToParty;
                    UyShipDetails.OrgSearch = OrgSearchLocationLbl;
                    UyShipDetails.OrgLocationID = OrgSearchLocationLblID;
                    UyShipDetails.OrgCntryCode = EtyOrgCntryCode;
                    UyShipDetails.OrgCntryName = EtyOrgCntryName;
                    UyShipDetails.OrgLocCode = EtyOrgLocCode;
                    UyShipDetails.OrgLocName = EtyOrgLocName;
                    UyShipDetails.PickupLocation = EtyPickupLocation;
                    UyShipDetails.DestSearch = DestSearchLocationLbl;
                    UyShipDetails.DestLocationID = DestSearchLocationLblID;
                    UyShipDetails.DestCntryCode = EtyDestCntryCode;
                    UyShipDetails.DestCntryName = EtyDestCntryName;
                    UyShipDetails.DestLocCode = EtyDestLocCode;
                    UyShipDetails.DestLocName = EtyDestLocName;
                    UyShipDetails.DeliveryLocation = EtyDeliveryLocation;
                    UyShipDetails.ShipmentOpenedby = EtyShipmentOpenedby;
                    UyShipDetails.ShipmentLoadedBy = ShipmentLoadedByID;
                    UyShipDetails.InvNo = EtyInvNo;
                    UyShipDetails.CustRefNo = EtyCustRefNo;
                    UyShipDetails.ShmtTerm = ShmtTermID;
                    UyShipDetails.SpecialCargoHandlingInstructions = EtySpecialCargoHandlingInstr;
                    UyShipDetails.GeneralDescription = EtyGeneralDescription;
                    UyShipDetails.Currency = EtyCurrency == "" ? "" : EtyCurrency;
                    UyShipDetails.TotalAmount = EtyTotalAmount;
                    UyShipDetails.TtlPkg = EtyTotalPkg;
                    UyShipDetails.PkgUnit = EtyPkgUnit;
                    UyShipDetails.TtlM3 = EtyTotalM3;
                    UyShipDetails.TtlGrossWt = EtyTotalGrossWt;
                    UyShipDetails.TtlNetWt = EtyTotalNetWt;

                    if (ETDDatePickerLbl == labelobj.ReqPickupDt)
                    {
                        UyShipDetails.ETD = null;
                    }
                    else
                    {
                        UyShipDetails.ETD = ETDDatePickerLbl;
                    }

                    if (PickUpTimeLbl == labelobj.PickUpTime)
                    {
                        UyShipDetails.PickUpTime = null;
                    }
                    else
                    {
                        UyShipDetails.PickUpTime = PickUpTimeLbl;
                    }

                    if (ETADatePickerLbl == labelobj.ReqDeliveryDate)
                    {
                        UyShipDetails.ETA = null;
                    }
                    else
                    {
                        UyShipDetails.ETA = ETADatePickerLbl;
                    }

                    if (DeliveryTimeLbl == labelobj.DeliveryTime)
                    {
                        UyShipDetails.DeliveryTime = null;
                    }
                    else
                    {
                        UyShipDetails.DeliveryTime = DeliveryTimeLbl;
                    }
                    /// End of capturing Booking Details

                    /// Sending the Booking Details to the API
                    var UpdateResult = await YSFService.UpdateyShipDetailsService(UyShipDetails);

                    if (UpdateResult.status != 0 || UpdateResult != null) ///if Booking Details saved successfully.
                    {
                        switch (Device.RuntimePlatform)
                        {
                            case Device.iOS:
                                await App.Current.MainPage.DisplayAlert("Submit", "Success.", "Close");
                                break;
                            case Device.Android:
                                DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                break;
                        }
                    }
                    else
                    {
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YShipFilterViewModel method-> in UpdateBasicData " + Settings.userLoginID);
                await YSFService.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        #region Properties

        #region Dynamic text change properties
        public class bookingDetailslabelclass
        {
            public string CompanyName { get; set; } = "Company";
            public string yBkgNumber { get; set; } = "yBkgNumber";
            public string yShipBkgNo { get; set; } = "yShipBkgNo";
            public string BkgRefNo { get; set; } = "BkgRefNo";
            public string ShipmntType { get; set; } = "ShipmentType";
            public string EqmtQtyTotal { get; set; } = "EqmtQtyTotal";
            public string BkgConfirmed { get; set; } = "IsBkgConfirmed";
            public string EqmtType1 { get; set; } = "EqmtType";
            public string Type1Qty { get; set; } = "Type1Qty";
            public string EqmtType2 { get; set; } = "EqmtType";
            public string Type2Qty { get; set; } = "Type2Qty";
            public string EqmtType3 { get; set; } = "EqmtType";
            public string Type3Qty { get; set; } = "Type3Qty";
            public string Shipper { get; set; } = "Shipper";
            public string OrgSearch { get; set; } = "OrgSearch";
            public string OrgCntryCode { get; set; } = "OrgCntryCode";
            public string OrgCntryName { get; set; } = "OrgCntryName";
            public string OrgLocCode { get; set; } = "OrgLocCode";
            public string OrgLocName { get; set; } = "OrgLocName";
            public string PickupLocation { get; set; } = "PickupLocation"; /// Used in 2 tabs
            public string ReqPickupDt { get; set; } = "ReqPickupDate";
            public string PickUpTime { get; set; } = "PickUpTime";
            public string PickupPIC { get; set; } = "PickupPIC";
            public string ShipToParty { get; set; } = "ShipToParty";
            public string DestSearch { get; set; } = "DestSearch";
            public string DestCntryCode { get; set; } = "DestCntryCode";
            public string DestCntryName { get; set; } = "DestCntryName";
            public string DestLocCode { get; set; } = "DestLocCode";
            public string DestLocName { get; set; } = "DestLocName";
            public string DeliveryLocation { get; set; } = "DeliveryLocation"; /// Used in 2 tabs
            public string ReqDeliveryDate { get; set; } = "ReqDeliveryDate";
            public string DeliveryTime { get; set; } = "DeliveryTime";
            public string DestinationPIC { get; set; } = "DestinationPIC";
            public string ShipmentLoadedBy { get; set; } = "ShipmentLoadedBy";
            public string ReqPickupDate { get; set; } = "ReqPickupDate";
            public string ShipmentOpenedby { get; set; } = "ShipmentOpenedby";
            public string InvoiceNo { get; set; } = "InvoiceNo";
            public string CustRefNo { get; set; } = "CustRefNo";
            public string ShmtTerm { get; set; } = "ShmtTerm";
            public string SpecialCargoHandlingInstr { get; set; } = "SpecialCargoHandlingInstr";
            public string GeneralDesc { get; set; } = "GeneralDesc";
            public string Currency { get; set; } = "Currency";
            public string TotalAmount { get; set; } = "TotalAmount";
            public string TotalPkg { get; set; } = "TotalPkg";
            public string PkgUnit { get; set; } = "PkgUnit";
            public string TotalM3 { get; set; } = "TotalM3";
            public string TotalGrossWt { get; set; } = "TotalGrossWt";
            public string TotalNetWt { get; set; } = "TotalNetWt";
            public string Submit { get; set; } = "submit";
            public string CompleteShipment { get; set; } = "complete";
            public string CancelShipment { get; set; } = "cancel";
        }

        public bookingDetailslabelclass _labelobj;
        public bookingDetailslabelclass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        private bool _BasicDetailsLblVisibility;
        public bool BasicDetailsLblVisibility
        {
            get => _BasicDetailsLblVisibility;
            set
            {
                _BasicDetailsLblVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private bool _LocationDetailsLblVisibility;
        public bool LocationDetailsLblVisibility
        {
            get => _LocationDetailsLblVisibility;
            set
            {
                _LocationDetailsLblVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ShipmentDetailsLblVisibility;
        public bool ShipmentDetailsLblVisibility
        {
            get => _ShipmentDetailsLblVisibility;
            set
            {
                _ShipmentDetailsLblVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private bool _OtherDetailsLblVisibility;
        public bool OtherDetailsLblVisibility
        {
            get => _OtherDetailsLblVisibility;
            set
            {
                _OtherDetailsLblVisibility = value;
                NotifyPropertyChanged();
            }
        }

        private bool _BasicDetaisStack = true;
        public bool BasicDetailsStack
        {
            get => _BasicDetaisStack;
            set
            {
                _BasicDetaisStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _LocationDetailsStack = false;
        public bool LocatinDetailsStack
        {
            get => _LocationDetailsStack;
            set
            {
                _LocationDetailsStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _ShipmentDetailsStack = false;
        public bool ShipmentDetailsStack
        {
            get => _ShipmentDetailsStack;
            set
            {
                _ShipmentDetailsStack = value;
                NotifyPropertyChanged();
            }
        }

        private bool _OtherDetailsStack = false;
        public bool OtherDetailsStack
        {
            get => _OtherDetailsStack;
            set
            {
                _OtherDetailsStack = value;
                NotifyPropertyChanged();
            }
        }

        #region Field Properties
        private string _yBkg;
        public string yBkg
        {
            get => _yBkg;
            set
            {
                _yBkg = value;
                NotifyPropertyChanged();
            }
        }

        private string _yShipBkg;
        public string yShipBkg
        {
            get => _yShipBkg;
            set
            {
                _yShipBkg = value;
                NotifyPropertyChanged();
            }
        }

        private string _BkgRef;
        public string BkgRef
        {
            get => _BkgRef;
            set
            {
                _BkgRef = value;
                NotifyPropertyChanged();
            }
        }

        private string _CompanyDefaultLblValue = "Select";
        public string CompanyDefaultLblValue
        {
            get => _CompanyDefaultLblValue;
            set
            {
                _CompanyDefaultLblValue = value;
                NotifyPropertyChanged();
            }
        }


        private string _SelectedCompanyValue;
        public string SelectedCompanyValue
        {
            get => _SelectedCompanyValue;
            set
            {
                _SelectedCompanyValue = value;
                SelectedCompany_TapEvent();
            }
        }

        private ObservableCollection<string> _CompanylList;
        public ObservableCollection<string> CompanylList
        {
            get => _CompanylList;
            set
            {
                _CompanylList = value;
                NotifyPropertyChanged();
            }
        }

        private int _ShmtTypeID;
        public int ShmtTypeID
        {
            get => _ShmtTypeID;
            set
            {
                _ShmtTypeID = value;
                NotifyPropertyChanged();
            }
        }

        private int _EqmtType1ID;
        public int EqmtType1ID
        {
            get => _EqmtType1ID;
            set
            {
                _EqmtType1ID = value;
                NotifyPropertyChanged();
            }
        }

        private int _EqmtType2ID;
        public int EqmtType2ID
        {
            get => _EqmtType2ID;
            set
            {
                _EqmtType2ID = value;
                NotifyPropertyChanged();
            }
        }

        private int _EqmtType3ID;
        public int EqmtType3ID
        {
            get => _EqmtType3ID;
            set
            {
                _EqmtType3ID = value;
                NotifyPropertyChanged();
            }
        }

        private int _ShipmentLoadedByID;
        public int ShipmentLoadedByID
        {
            get => _ShipmentLoadedByID;
            set
            {
                _ShipmentLoadedByID = value;
                NotifyPropertyChanged();
            }
        }

        private int _ShmtTermID;
        public int ShmtTermID
        {
            get => _ShmtTermID;
            set
            {
                _ShmtTermID = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<string> _CompleteList;
        public ObservableCollection<string> CompleteList
        {
            get => _CompleteList;
            set
            {
                _CompleteList = value;
                NotifyPropertyChanged();
            }
        }

        private string _ShmtTypeDefaultLblValue = "Select";
        public string ShmtTypeDefaultLblValue
        {
            get => _ShmtTypeDefaultLblValue;
            set
            {
                _ShmtTypeDefaultLblValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _SelectedShmtTypeValue;
        public string SelectedShmtTypeValue
        {
            get => _SelectedShmtTypeValue;
            set
            {
                _SelectedShmtTypeValue = value;
                SelectedShmtType_TapEvent();
            }
        }

        private ObservableCollection<string> _ShmtTypeList;
        public ObservableCollection<string> ShmtTypeList
        {
            get => _ShmtTypeList;
            set
            {
                _ShmtTypeList = value;
                NotifyPropertyChanged();
            }
        }

        private string _EqmtType1DefaultLblValue = "Select";
        public string EqmtType1DefaultLblValue
        {
            get => _EqmtType1DefaultLblValue;
            set
            {
                _EqmtType1DefaultLblValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _SelectedEqmtType1Value;
        public string SelectedEqmtType1Value
        {
            get => _SelectedEqmtType1Value;
            set
            {
                _SelectedEqmtType1Value = value;
                SelectedEqmtType1_TapEvent();
            }
        }

        private ObservableCollection<string> _EqmtType1List;
        public ObservableCollection<string> EqmtType1List
        {
            get => _EqmtType1List;
            set
            {
                _EqmtType1List = value;
                NotifyPropertyChanged();
            }
        }

        private string _EntryEqmtType1Qty = "0";
        public string EntryEqmtType1Qty
        {
            get => _EntryEqmtType1Qty;
            set
            {
                _EntryEqmtType1Qty = value;
                NotifyPropertyChanged();
            }
        }

        private string _EqmtType2DefaultLblValue = "Select";
        public string EqmtType2DefaultLblValue
        {
            get => _EqmtType2DefaultLblValue;
            set
            {
                _EqmtType2DefaultLblValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _SelectedEqmtType2Value;
        public string SelectedEqmtType2Value
        {
            get => _SelectedEqmtType2Value;
            set
            {
                _SelectedEqmtType2Value = value;
                SelectedEqmtType2_TapEvent();
            }
        }

        private ObservableCollection<string> _EqmtType2List;
        public ObservableCollection<string> EqmtType2List
        {
            get => _EqmtType2List;
            set
            {
                _EqmtType2List = value;
                NotifyPropertyChanged();
            }
        }

        private string _EntryEqmtType2Qty = "0";
        public string EntryEqmtType2Qty
        {
            get => _EntryEqmtType2Qty;
            set
            {
                _EntryEqmtType2Qty = value;
                NotifyPropertyChanged();
            }
        }

        private string _EqmtType3DefaultLblValue = "Select";
        public string EqmtType3DefaultLblValue
        {
            get => _EqmtType3DefaultLblValue;
            set
            {
                _EqmtType3DefaultLblValue = value;
                NotifyPropertyChanged();
            }
        }

        private string _SelectedEqmtType3Value;
        public string SelectedEqmtType3Value
        {
            get => _SelectedEqmtType3Value;
            set
            {
                _SelectedEqmtType3Value = value;
                SelectedEqmtType3_TapEvent();
            }
        }

        private ObservableCollection<string> _EqmtType3List;
        public ObservableCollection<string> EqmtType3List
        {
            get => _EqmtType3List;
            set
            {
                _EqmtType3List = value;
                NotifyPropertyChanged();
            }
        }

        private string _EntryEqmtType3Qty = "0";
        public string EntryEqmtType3Qty
        {
            get => _EntryEqmtType3Qty;
            set
            {
                _EntryEqmtType3Qty = value;
                NotifyPropertyChanged();
            }
        }

        private string _TotalEqmtQty;
        public string TotalEqmtQty
        {

            get => _TotalEqmtQty;
            set
            {
                _TotalEqmtQty = value;
                NotifyPropertyChanged();
            }
        }

        private string _BkgCnfrmedDfltLblVal;
        public string BkgCnfrmedDfltLblVal
        {

            get => _BkgCnfrmedDfltLblVal;
            set
            {
                _BkgCnfrmedDfltLblVal = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<string> _BkgCnfrmedList;
        public ObservableCollection<string> BkgCnfrmedList
        {
            get => _BkgCnfrmedList;
            set
            {
                _BkgCnfrmedList = value;
                NotifyPropertyChanged();
            }
        }

        private string _SelectedBkgCnfrmedValue;
        public string SelectedBkgCnfrmedValue
        {
            get => _SelectedBkgCnfrmedValue;
            set
            {
                _SelectedBkgCnfrmedValue = value;
                SelectedBkgCnfrmedValue_TapEvent();
            }
        }

        private int? _BkgConfirmedId;
        public int? BkgConfirmedId
        {
            get => _BkgConfirmedId;
            set
            {
                _BkgConfirmedId = value;
                NotifyPropertyChanged();
            }
        }

        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();

            }
        }

        public Color _TabBgColor = Color.FromHex("#269DC9");
        public Color TabBgColor
        {
            get => _TabBgColor;
            set
            {
                _TabBgColor = value;
                NotifyPropertyChanged();
            }
        }
        public bool _TabBasicVisibility = true;
        public bool TabBasicVisibility
        {
            get => _TabBasicVisibility;
            set
            {
                _TabBasicVisibility = value;
                NotifyPropertyChanged();
            }
        }
        public bool _TabLocationVisibility = false;
        public bool TabLocationVisibility
        {
            get => _TabLocationVisibility;
            set
            {
                _TabLocationVisibility = value;
                NotifyPropertyChanged();
            }
        }
        public bool _TabShipmentVisibility = false;
        public bool TabShipmentVisibility
        {
            get => _TabShipmentVisibility;
            set
            {
                _TabShipmentVisibility = value;
                NotifyPropertyChanged();
            }
        }
        public bool _TabOtherVisibility = false;
        public bool TabOtherVisibility
        {
            get => _TabOtherVisibility;
            set
            {
                _TabOtherVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityBasic = 1;
        public double TabOpacityBasic
        {
            get => _TabOpacityBasic;
            set
            {
                _TabOpacityBasic = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityLocation = 0.3;
        public double TabOpacityLocation
        {
            get => _TabOpacityLocation;
            set
            {
                _TabOpacityLocation = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityShipment = 0.3;
        public double TabOpacityShipment
        {
            get => _TabOpacityShipment;
            set
            {
                _TabOpacityShipment = value;
                NotifyPropertyChanged();
            }
        }
        private double _TabOpacityOther = 0.3;
        public double TabOpacityOther
        {
            get => _TabOpacityOther;
            set
            {
                _TabOpacityOther = value;
                NotifyPropertyChanged();
            }
        }
        private Color _FBgColorEqmtType1 = Color.Gray;
        public Color FBgColorEqmtType1
        {
            get => _FBgColorEqmtType1;
            set
            {
                _FBgColorEqmtType1 = value;
                NotifyPropertyChanged();
            }
        }
        private Color _FBgColorType1Qty = Color.Gray;
        public Color FBgColorType1Qty
        {
            get => _FBgColorType1Qty;
            set
            {
                _FBgColorType1Qty = value;
                NotifyPropertyChanged();
            }
        }

        private Color _FBgColorEqmtType2 = Color.Gray;
        public Color FBgColorEqmtType2
        {
            get => _FBgColorEqmtType2;
            set
            {
                _FBgColorEqmtType2 = value;
                NotifyPropertyChanged();
            }
        }

        private Color _FBgColorType2Qty = Color.Gray;
        public Color FBgColorType2Qty
        {
            get => _FBgColorType2Qty;
            set
            {
                _FBgColorType2Qty = value;
                NotifyPropertyChanged();
            }
        }

        private Color _FBgColorEqmtType3 = Color.Gray;
        public Color FBgColorEqmtType3
        {
            get => _FBgColorEqmtType3;
            set
            {
                _FBgColorEqmtType3 = value;
                NotifyPropertyChanged();
            }
        }

        private Color _FBgColorType3Qty = Color.Gray;
        public Color FBgColorType3Qty
        {
            get => _FBgColorType3Qty;
            set
            {
                _FBgColorType3Qty = value;
                NotifyPropertyChanged();
            }
        }

        private string _EtyPickupPIC;
        public string EtyPickupPIC
        {
            get => _EtyPickupPIC;
            set
            {
                _EtyPickupPIC = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyDestinationPIC;
        public string EtyDestinationPIC
        {
            get => _EtyDestinationPIC;
            set
            {
                _EtyDestinationPIC = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyShipper;
        public string EtyShipper
        {
            get => _EtyShipper;
            set
            {
                _EtyShipper = value;
                NotifyPropertyChanged();
            }
        }

        private string _ETDDatePickerLbl;
        public string ETDDatePickerLbl
        {
            get => _ETDDatePickerLbl;
            set
            {
                _ETDDatePickerLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _ETADatePickerLbl;
        public string ETADatePickerLbl
        {
            get => _ETADatePickerLbl;
            set
            {
                _ETADatePickerLbl = value;
                NotifyPropertyChanged();
            }
        }

        private string _PickUpTimeLbl;
        public string PickUpTimeLbl
        {
            get => _PickUpTimeLbl;
            set
            {
                _PickUpTimeLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyShipToParty;
        public string EtyShipToParty
        {
            get => _EtyShipToParty;
            set
            {
                _EtyShipToParty = value;
                NotifyPropertyChanged();
            }
        }
        private string _NoResultsLbl = "Please enter 4 or more characters";
        public string NoResultsLbl
        {
            get { return _NoResultsLbl; }
            set
            {
                _NoResultsLbl = value;
                NotifyPropertyChanged();
            }
        }

        private bool _NoResultIsVisibleLbl = false;
        public bool NoResultIsVisibleLbl
        {
            get => _NoResultIsVisibleLbl;
            set
            {
                _NoResultIsVisibleLbl = value;
                NotifyPropertyChanged();
            }
        }
        private bool _OrgSearchListStack = false;
        public bool OrgSearchListStack
        {
            get => _OrgSearchListStack;
            set
            {
                _OrgSearchListStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _SearchContentVForAll = false;
        public bool SearchContentVForAll
        {
            get => _SearchContentVForAll;
            set
            {
                _SearchContentVForAll = value;
                NotifyPropertyChanged();
            }
        }
        private string _OrgSearchLocationLbl = "Select";
        public string OrgSearchLocationLbl
        {
            get => _OrgSearchLocationLbl;
            set
            {
                _OrgSearchLocationLbl = value;
                NotifyPropertyChanged();
            }
        }
        private int _OrgSearchLocationLblID = 0;
        public int OrgSearchLocationLblID
        {
            get => _OrgSearchLocationLblID;
            set
            {
                _OrgSearchLocationLblID = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyOrgCntryCode;
        public string EtyOrgCntryCode
        {
            get => _EtyOrgCntryCode;
            set
            {
                _EtyOrgCntryCode = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyOrgCntryName;
        public string EtyOrgCntryName
        {
            get => _EtyOrgCntryName;
            set
            {
                _EtyOrgCntryName = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyOrgLocCode;
        public string EtyOrgLocCode
        {
            get => _EtyOrgLocCode;
            set
            {
                _EtyOrgLocCode = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyOrgLocName;
        public string EtyOrgLocName
        {
            get => _EtyOrgLocName;
            set
            {
                _EtyOrgLocName = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyPickupLocation;
        public string EtyPickupLocation
        {
            get => _EtyPickupLocation;
            set
            {
                _EtyPickupLocation = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<SearchData> _SearchListItems;
        public ObservableCollection<SearchData> SearchListItems
        {
            get => _SearchListItems;
            set
            {
                _SearchListItems = value;
                NotifyPropertyChanged();
            }
        }
        private string _EntrySearchType;
        public string EntrySearchType
        {
            get => _EntrySearchType;
            set
            {
                _EntrySearchType = value;
                NotifyPropertyChanged();
            }
        }
        private string _DestSearchLocationLbl = "Select";
        public string DestSearchLocationLbl
        {
            get => _DestSearchLocationLbl;
            set
            {
                _DestSearchLocationLbl = value;
                NotifyPropertyChanged();
            }
        }

        private int _DestSearchLocationLblID = 0;
        public int DestSearchLocationLblID
        {
            get => _DestSearchLocationLblID;
            set
            {
                _DestSearchLocationLblID = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyDestCntryCode;
        public string EtyDestCntryCode
        {
            get => _EtyDestCntryCode;
            set
            {
                _EtyDestCntryCode = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyDestCntryName;
        public string EtyDestCntryName
        {
            get => _EtyDestCntryName;
            set
            {
                _EtyDestCntryName = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyDestLocCode;
        public string EtyDestLocCode
        {
            get => _EtyDestLocCode;
            set
            {
                _EtyDestLocCode = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyDestLocName;
        public string EtyDestLocName
        {
            get => _EtyDestLocName;
            set
            {
                _EtyDestLocName = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyDeliveryLocation;
        public string EtyDeliveryLocation
        {
            get => _EtyDeliveryLocation;
            set
            {
                _EtyDeliveryLocation = value;
                NotifyPropertyChanged();
            }
        }
        private string _DeliveryTimeLbl;
        public string DeliveryTimeLbl
        {
            get => _DeliveryTimeLbl;
            set
            {
                _DeliveryTimeLbl = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<string> _ShipmentLoadedByList;
        public ObservableCollection<string> ShipmentLoadedByList
        {
            get => _ShipmentLoadedByList;
            set
            {
                _ShipmentLoadedByList = value;
                NotifyPropertyChanged();
            }
        }
        private string _SelectedShipmentLoadedBy;
        public string SelectedShipmentLoadedBy
        {
            get => _SelectedShipmentLoadedBy;
            set
            {
                _SelectedShipmentLoadedBy = value;
                SelectedShipmentLoadedBy_TapEvent();
            }
        }

        private string _ShipmentLoadedByDefaultLbl = "Select";
        public string ShipmentLoadedByDefaultLbl
        {
            get => _ShipmentLoadedByDefaultLbl;
            set
            {
                _ShipmentLoadedByDefaultLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _OriginalRPkupDateLbl;
        public string OriginalRPkupDateLbl
        {
            get => _OriginalRPkupDateLbl;
            set
            {
                _OriginalRPkupDateLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _OriginalRPkupTimeLbl;
        public string OriginalRPkupTimeLbl
        {
            get => _OriginalRPkupTimeLbl;
            set
            {
                _OriginalRPkupTimeLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyViaHubLoc;
        public string EtyViaHubLoc
        {
            get => _EtyViaHubLoc;
            set
            {
                _EtyViaHubLoc = value;
                NotifyPropertyChanged();
            }
        }
        private string _ETAViaLocDateLbl;
        public string ETAViaLocDateLbl
        {
            get => _ETAViaLocDateLbl;
            set
            {
                _ETAViaLocDateLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _ATAViaLocDateLbl;
        public string ATAViaLocDateLbl
        {
            get => _ATAViaLocDateLbl;
            set
            {
                _ATAViaLocDateLbl = value;
                NotifyPropertyChanged();
            }
        }

        private string _ETADestDateLbl;
        public string ETADestDateLbl
        {
            get => _ETADestDateLbl;
            set
            {
                _ETADestDateLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _ATADestDateLbl;
        public string ATADestDateLbl
        {
            get => _ATADestDateLbl;
            set
            {
                _ATADestDateLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _OriginalRDlyDateLbl;
        public string OriginalRDlyDateLbl
        {
            get => _OriginalRDlyDateLbl;
            set
            {
                _OriginalRDlyDateLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _OriginalRDeliveryTimeLbl;
        public string OriginalRDeliveryTimeLbl
        {
            get => _OriginalRDeliveryTimeLbl;
            set
            {
                _OriginalRDeliveryTimeLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyShipmentOpenedby;
        public string EtyShipmentOpenedby
        {
            get => _EtyShipmentOpenedby;
            set
            {
                _EtyShipmentOpenedby = value;
                NotifyPropertyChanged();
            }
        }

        private string _EtyInvNo;
        public string EtyInvNo
        {
            get => _EtyInvNo;
            set
            {
                _EtyInvNo = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyCustRefNo;
        public string EtyCustRefNo
        {
            get => _EtyCustRefNo;
            set
            {
                _EtyCustRefNo = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<string> _ShmtTermSelectList;
        public ObservableCollection<string> ShmtTermSelectList
        {
            get => _ShmtTermSelectList;
            set
            {
                _ShmtTermSelectList = value;
                NotifyPropertyChanged();
            }
        }
        private string _ShmtTermDefaultValue = "Select";
        public string ShmtTermDefaultValue
        {
            get => _ShmtTermDefaultValue;
            set
            {
                _ShmtTermDefaultValue = value;
                NotifyPropertyChanged();
            }
        }
        private string _SelectedhmtTermItem;
        public string SelectedhmtTermItem
        {
            get => _SelectedhmtTermItem;
            set
            {
                _SelectedhmtTermItem = value;
                SelectedhmtTermItem_Tab();
            }
        }

        private string _EtySpecialCargoHandlingInstr;
        public string EtySpecialCargoHandlingInstr
        {
            get => _EtySpecialCargoHandlingInstr;
            set
            {
                _EtySpecialCargoHandlingInstr = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyGeneralDescription;
        public string EtyGeneralDescription
        {
            get => _EtyGeneralDescription;
            set
            {
                _EtyGeneralDescription = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyCurrency;
        public string EtyCurrency
        {
            get => _EtyCurrency;
            set
            {
                _EtyCurrency = value;
                NotifyPropertyChanged();

            }
        }

        private decimal _EtyTotalAmount;
        public decimal EtyTotalAmount
        {
            get => _EtyTotalAmount;
            set
            {
                _EtyTotalAmount = value;
                NotifyPropertyChanged();
            }
        }
        private int _EtyTotalPkg;
        public int EtyTotalPkg
        {
            get => _EtyTotalPkg;
            set
            {
                _EtyTotalPkg = value;
                NotifyPropertyChanged();
            }
        }
        private string _EtyPkgUnit;
        public string EtyPkgUnit
        {
            get => _EtyPkgUnit;
            set
            {
                _EtyPkgUnit = value;
                NotifyPropertyChanged();
            }
        }
        private decimal _EtyTotalM3;
        public decimal EtyTotalM3
        {
            get => _EtyTotalM3;
            set
            {
                _EtyTotalM3 = value;
                NotifyPropertyChanged();
            }
        }
        private decimal _EtyTotalGrossWt;
        public decimal EtyTotalGrossWt
        {
            get => _EtyTotalGrossWt;
            set
            {
                _EtyTotalGrossWt = value;
                NotifyPropertyChanged();
            }
        }
        private decimal _EtyTotalNetWt;
        public decimal EtyTotalNetWt
        {
            get => _EtyTotalNetWt;
            set
            {
                _EtyTotalNetWt = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ShowHideClCmpltStack = true;
        public bool ShowHideClCmpltStack
        {
            get => _ShowHideClCmpltStack;
            set
            {
                _ShowHideClCmpltStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _ShowHideClCmpltLbl;
        public bool ShowHideClCmpltLbl
        {
            get => _ShowHideClCmpltLbl;
            set
            {
                _ShowHideClCmpltLbl = value;
                NotifyPropertyChanged();
            }
        }
        private string _MessageForClCmpltbl;
        public string MessageForClCmpltbl
        {
            get => _MessageForClCmpltbl;
            set
            {
                _MessageForClCmpltbl = value;
                NotifyPropertyChanged();
            }
        }
        private Color _CancelColorLbl;
        public Color CancelColorLbl
        {
            get => _CancelColorLbl;
            set
            {
                _CancelColorLbl = value;
                NotifyPropertyChanged();
            }
        }
        private bool _SubmitStackVisible = true;
        public bool SubmitStackVisible
        {
            get => _SubmitStackVisible;
            set
            {
                _SubmitStackVisible = value;
                NotifyPropertyChanged();
            }
        }

        #endregion
        #endregion
    }
}