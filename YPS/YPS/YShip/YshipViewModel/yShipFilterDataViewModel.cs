using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using YPS.CustomRenders;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;
using YPS.Model;
using static YPS.Model.SearchModel;
using Newtonsoft.Json;
using System.Windows.Input;
using YPS.Views;
using YPS.CustomToastMsg;
using System.Linq;
using System.Collections.ObjectModel;
using YPS.Model.Yship;
using Syncfusion.XForms.Buttons;
using Acr.UserDialogs;

namespace YPS.YShip.YshipViewModel
{
    public class yShipFilterDataViewModel : IBase
    {
        #region
        public YPSService service;
        private NullableDatePicker nullableDatePicker;
        bool checkInternet;
        string pickerOf = "";
        #endregion

        #region All ICommands
        public ICommand OriSearchPicker { set; get; }
        public ICommand DestSearchPicker { set; get; }
        public ICommand ReqPickUpDateTapCmd { set; get; }
        public ICommand pickerTimeClickCmd { set; get; }
        public ICommand reqDelivertyDateCmd { set; get; }
        public ICommand bkgConfirmPickerCmd { set; get; }
        public ICommand cancelPickerCmd { set; get; }
        public ICommand completedPickerCmd { set; get; }
        public ICommand eqmtTypePickerCmd { set; get; }
        public ICommand searchData { set; get; }
        public ICommand tabClickCmd { set; get; }

        #endregion

        /// <summary>
        /// Parameter less constructor.
        /// </summary>
        public yShipFilterDataViewModel()
        {
            YPSLogger.TrackEvent("yShipFilterDataViewModel", "Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                IndicatorVisibility = true;
                service = new YPSService();

                #region Command binding for showing pickers
                bkgConfirmPickerCmd = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                cancelPickerCmd = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                completedPickerCmd = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                eqmtTypePickerCmd = new Command<View>((view) =>
                {
                    view?.Focus();
                });

                OriSearchPicker = new Command<View>((view) =>
                {
                    EntrySearchType = "orisearch";

                    if (SearchLocationResults != null)
                    {
                        SearchLocationResults.Clear();
                    }
                    SearchContentView = true;
                    NoResultsIsVisibleLbl = true;
                });

                DestSearchPicker = new Command<View>((view) =>
                {
                    EntrySearchType = "destsearch";

                    if (SearchLocationResults != null)
                    {
                        SearchLocationResults.Clear();
                    }
                    SearchContentView = true;
                    NoResultsIsVisibleLbl = true;
                });
                #endregion

                #region Tab & Click events binding to ICommand properties
                searchData = new Command(SearchOrResetData);
                ReqPickUpDateTapCmd = new Command(ReqPickUpDateTap);
                pickerTimeClickCmd = new Command(TimePickerClick);
                reqDelivertyDateCmd = new Command(ReqDeliveryDateTap);
                tabClickCmd = new Command(HeaderTabClicked);
                #endregion

                ChangeLabelAndShowHide();
                HeaderFilterData();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "yShipFilterDataViewModel constructor -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This method if for dynamic text change & show/hide fields.
        /// </summary>
        public void ChangeLabelAndShowHide()
        {
            try
            {
                labelobj = new filtterlabelclass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        //Getting Label values & Status based on FieldID
                        var bkgConfirm = labelval.Where(wr => wr.FieldID == labelobj.BkgConfirm.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var yShipBkgNumber = labelval.Where(wr => wr.FieldID == labelobj.yShipBkgNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingno = labelval.Where(wr => wr.FieldID == labelobj.ShippingNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bkgrefno = labelval.Where(wr => wr.FieldID == labelobj.BkgRefNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bkgnumber = labelval.Where(wr => wr.FieldID == labelobj.yBkgNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var cancel = labelval.Where(wr => wr.FieldID == labelobj.Cancel.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var complete = labelval.Where(wr => wr.FieldID == labelobj.Completed.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var eqmttype = labelval.Where(wr => wr.FieldID == labelobj.EqmtType.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var orisearch = labelval.Where(wr => wr.FieldID == labelobj.OriSearch.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var destsearch = labelval.Where(wr => wr.FieldID == labelobj.DestSearch.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqpickupdate = labelval.Where(wr => wr.FieldID == labelobj.ReqPickupDate.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var pickuptime = labelval.Where(wr => wr.FieldID == labelobj.PickupTime.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqdeliverydate = labelval.Where(wr => wr.FieldID == labelobj.ReqDeliveryDate.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var deliverytime = labelval.Where(wr => wr.FieldID == labelobj.DeliveryTime.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var resetbtn = labelval.Where(wr => wr.FieldID == labelobj.ResetBtn.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var searchbtn = labelval.Where(wr => wr.FieldID == labelobj.SearchBtn.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.BkgConfirm.Name = bkgConfirm != null ? (!string.IsNullOrEmpty(bkgConfirm.LblText) ? bkgConfirm.LblText : "Bkg Confirm") : "Bkg Confirm";
                        labelobj.BkgConfirm.Status = bkgConfirm == null ? true : (bkgConfirm.Status == 1 ? true : false);
                        labelobj.yShipBkgNumber.Name = yShipBkgNumber != null ? (!string.IsNullOrEmpty(yShipBkgNumber.LblText) ? yShipBkgNumber.LblText : "yShip Bkg#") : "yShip Bkg#";
                        labelobj.yShipBkgNumber.Status = yShipBkgNumber == null ? true : (yShipBkgNumber.Status == 1 ? true : false);
                        labelobj.ShippingNumber.Name = shippingno != null ? (!string.IsNullOrEmpty(shippingno.LblText) ? shippingno.LblText : "Shipping#") : "Shipping#";
                        labelobj.ShippingNumber.Status = shippingno == null ? true : (shippingno.Status == 1 ? true : false);
                        labelobj.BkgRefNumber.Name = bkgrefno != null ? (!string.IsNullOrEmpty(bkgrefno.LblText) ? bkgrefno.LblText : "Bkg Ref#") : "Bkg Ref#";
                        labelobj.BkgRefNumber.Status = bkgrefno == null ? true : (bkgrefno.Status == 1 ? true : false);
                        labelobj.yBkgNumber.Name = bkgnumber != null ? (!string.IsNullOrEmpty(bkgnumber.LblText) ? bkgnumber.LblText : "yBkg Number") : "yBkg Number";
                        labelobj.yBkgNumber.Status = bkgnumber == null ? true : (bkgnumber.Status == 1 ? true : false);
                        labelobj.Cancel.Name = cancel != null ? (!string.IsNullOrEmpty(cancel.LblText) ? cancel.LblText : "Cancel Shipment") : "Cancel Shipment";
                        labelobj.Cancel.Status = cancel == null ? true : (cancel.Status == 1 ? true : false);
                        labelobj.Completed.Name = complete != null ? (!string.IsNullOrEmpty(complete.LblText) ? complete.LblText : "Complete Shipment") : "Complete Shipment";
                        labelobj.Completed.Status = complete == null ? true : (complete.Status == 1 ? true : false);
                        labelobj.EqmtType.Name = eqmttype != null ? (!string.IsNullOrEmpty(eqmttype.LblText) ? eqmttype.LblText : "Eqmt Type") : "Eqmt Type";
                        labelobj.EqmtType.Status = eqmttype == null ? true : (eqmttype.Status == 1 ? true : false);
                        labelobj.OriSearch.Name = orisearch != null ? (!string.IsNullOrEmpty(orisearch.LblText) ? orisearch.LblText : "Ori Search") : "Ori Search";
                        labelobj.OriSearch.Status = orisearch == null ? true : (orisearch.Status == 1 ? true : false);
                        labelobj.DestSearch.Name = destsearch != null ? (!string.IsNullOrEmpty(destsearch.LblText) ? destsearch.LblText : "Dest Search") : "Dest Search";
                        labelobj.DestSearch.Status = destsearch == null ? true : (destsearch.Status == 1 ? true : false);
                        labelobj.ReqPickupDate.Name = reqpickupdate != null ? (!string.IsNullOrEmpty(reqpickupdate.LblText) ? reqpickupdate.LblText : "Req Pickup Date") : "Req Pickup Date";
                        labelobj.ReqPickupDate.Status = reqpickupdate == null ? true : (reqpickupdate.Status == 1 ? true : false);
                        labelobj.PickupTime.Name = pickuptime != null ? (!string.IsNullOrEmpty(pickuptime.LblText) ? pickuptime.LblText : "Pickup Time") : "Pickup Time";
                        labelobj.PickupTime.Status = pickuptime == null ? true : (pickuptime.Status == 1 ? true : false);
                        labelobj.ReqDeliveryDate.Name = reqdeliverydate != null ? (!string.IsNullOrEmpty(reqdeliverydate.LblText) ? reqdeliverydate.LblText : "Req Delivery Dt") : "Req Delivery Dt";
                        labelobj.ReqDeliveryDate.Status = reqdeliverydate == null ? true : (reqdeliverydate.Status == 1 ? true : false);
                        labelobj.DeliveryTime.Name = deliverytime != null ? (!string.IsNullOrEmpty(deliverytime.LblText) ? deliverytime.LblText : "Delivery Time") : "Delivery Time";
                        labelobj.DeliveryTime.Status = deliverytime == null ? true : (deliverytime.Status == 1 ? true : false);
                        labelobj.ResetBtn.Name = resetbtn != null ? (!string.IsNullOrEmpty(resetbtn.LblText) ? resetbtn.LblText : "Reset") : "Reset";
                        labelobj.SearchBtn.Name = searchbtn != null ? (!string.IsNullOrEmpty(searchbtn.LblText) ? searchbtn.LblText : "Search") : "Search";

                        //Date & Time pickers place holder label change
                        ReqPickUpDateSelected = !string.IsNullOrEmpty(labelobj.ReqPickupDate.Name) ? labelobj.ReqPickupDate.Name : ReqPickUpDateSelected;
                        PickUpTimeSelected = !string.IsNullOrEmpty(labelobj.PickupTime.Name) ? labelobj.PickupTime.Name : PickUpTimeSelected;
                        ReqDeliveryDateSelected = !string.IsNullOrEmpty(labelobj.ReqDeliveryDate.Name) ? labelobj.ReqDeliveryDate.Name : ReqDeliveryDateSelected;
                        DeliveryTimeSelected = !string.IsNullOrEmpty(labelobj.DeliveryTime.Name) ? labelobj.DeliveryTime.Name : DeliveryTimeSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabelAndShowHide method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on Apply/Reset button.
        /// </summary>
        /// <returns></returns>
        public async void SearchOrResetData(object sender)
        {
            try
            {
                YPSLogger.TrackEvent("yShipFilterDataViewModel", "in SearchData method " + DateTime.Now + " UserId: " + Settings.userLoginID);
                //IndicatorVisibility = true;
                await Task.Delay(50);
                UserDialogs.Instance.ShowLoading("Loading...");

                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var objsender = sender as SfButton;
                    var button = objsender.ClassId;

                    yShipSearch yShipSearchCriteria = new yShipSearch();
                    Settings.UserID = Settings.userLoginID;

                    //Save the filter field values to DB
                    SearchPassData defaultVal = new SearchPassData();
                    defaultVal.UserID = Settings.userLoginID;
                    defaultVal.CompanyID = Settings.CompanyID;

                    //Setting the entered/default values for field from Key tab base on search/reset click
                    yShipSearchCriteria.yBkgNumber = Settings.ybkgNumberyShip = ybkgNumber = button == "search" ? ybkgNumber : string.Empty;
                    yShipSearchCriteria.yShipBkgNumber = Settings.yshipBkgNumberyShip = yshipBkgNumber = button == "search" ? yshipBkgNumber : string.Empty;
                    yShipSearchCriteria.ShippingNumber = Settings.shippingNumberyShip = shipNumber = button == "search" ? shipNumber : string.Empty;
                    yShipSearchCriteria.BkgRefNo = Settings.bkgRefNumberyShip = bkgRefnumber = button == "search" ? bkgRefnumber : string.Empty;
                    yShipSearchCriteria.BkgConfirmed = Settings.bkgConfirmIdyShip = button == "search" ? Settings.bkgConfirmIdyShip : -1;
                    yShipSearchCriteria.Cancel = Settings.cancelIdyShip = button == "search" ? Settings.cancelIdyShip : -1;
                    yShipSearchCriteria.Complete = Settings.completedIdyShip = button == "search" ? Settings.completedIdyShip : -1;
                    yShipSearchCriteria.EqmtTypeID = Settings.eqmtTypeIdyShip = button == "search" ? Settings.eqmtTypeIdyShip : 0;

                    //Setting the entered/default values for field from Location tab base on search/reset
                    yShipSearchCriteria.OrgLocation = Settings.oriSearchNameyShip = button == "search" ? Settings.oriSearchNameyShip : null;
                    yShipSearchCriteria.OrgLocationID = Settings.oriSearchIdyShip = button == "search" ? Settings.oriSearchIdyShip : 0;
                    yShipSearchCriteria.DestLocation = Settings.destSearchNameyShip = button == "search" ? Settings.destSearchNameyShip : null;
                    yShipSearchCriteria.DestLocationID = Settings.destSearchIdyShip = button == "search" ? Settings.destSearchIdyShip : 0;

                    //Setting the entered/default values for field  from Date tab base on search/reset
                    yShipSearchCriteria.ETA = Settings.reqPickUpDateyShip = button == "search" ? string.IsNullOrEmpty(Settings.reqPickUpDateyShip) ? null : ReqPickUpDateSelected : null;
                    yShipSearchCriteria.ETD = Settings.reqDeliveryDateyShip = button == "search" ? string.IsNullOrEmpty(Settings.reqDeliveryDateyShip) ? null : ReqDeliveryDateSelected : null;
                    yShipSearchCriteria.PickUpTime = Settings.pickUpTimeyShip = button == "search" ? string.IsNullOrEmpty(Settings.pickUpTimeyShip) ? null : PickUpTimeSelected : null;
                    yShipSearchCriteria.DeliveryTime = Settings.deliveryTimeyShip = button == "search" ? string.IsNullOrEmpty(Settings.deliveryTimeyShip) ? null : DeliveryTimeSelected : null;

                    if (button == "reset")
                    {
                        //Set default value for DDL in Key tab
                        bkgConfirmDDLDisplayVal = "ALL";
                        cancelDDLDisplayVal = "ALL";
                        completeDDLDisplayVal = "ALL";
                        eqmtTypeDDLDisplayVal = "ALL";

                        //Set the default value for pickers in Location tab
                        oriSearch = "ALL";
                        destSearch = "ALL";

                        //Set default value for the pickers in Date tab
                        ReqPickUpDateSelected = !string.IsNullOrEmpty(labelobj.ReqPickupDate.Name) ? labelobj.ReqPickupDate.Name : "Req PickUp Date";
                        PickUpTimeSelected = !string.IsNullOrEmpty(labelobj.PickupTime.Name) ? labelobj.PickupTime.Name : "PickUp Time";
                        ReqDeliveryDateSelected = !string.IsNullOrEmpty(labelobj.ReqDeliveryDate.Name) ? labelobj.ReqDeliveryDate.Name : "Req Delivery Date";
                        DeliveryTimeSelected = !string.IsNullOrEmpty(labelobj.DeliveryTime.Name) ? labelobj.DeliveryTime.Name : "Delivery Time";
                        Settings.isyShipRefreshPage = true;

                        //Hide the hint from Date & Time pickers
                        showdeldatehint = false;
                        showdeltimehint = false;
                        showpickupdatehint = false;
                        showpickuptimehint = false;
                    }

                    defaultVal.yShipSearchCriteria = JsonConvert.SerializeObject(yShipSearchCriteria);
                    var responseData = await service.SaveSerchvaluesSetting(defaultVal);

                    if (button == "search")
                    {
                        Settings.ShowSuccessAlert = true;
                        App.Current.MainPage = new YPSMasterPage(typeof(YshipPage));
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SearchOrResetData method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                Settings.ShowSuccessAlert = false;
            }
            finally
            {
                //IndicatorVisibility = false;
                UserDialogs.Instance.HideLoading();
            }
        }

        /// <summary>
        /// Gets called when clicked any tab.
        /// </summary>
        /// <returns></returns>
        private async void HeaderTabClicked(object sender)
        {
            try
            {
                var senderobj = sender as Label;

                if (senderobj.ClassId == "key")
                {
                    headerVisibility = true;
                    locationVisibility = datesVisibility = false;
                    headerTextColor = locationTextColor = datesTextColor = Color.White;
                    headerbox = true; locationbox = datebox = false;
                    headerBgColor = Color.FromHex("#269DC9");
                    locationBgColor = datesBgColor = Color.LightBlue;
                }
                else if (senderobj.ClassId == "location")
                {
                    locationVisibility = true;
                    headerVisibility = datesVisibility = false;
                    locationTextColor = headerTextColor = datesTextColor = Color.White;
                    locationbox = true; headerbox = datebox = false;
                    locationBgColor = Color.FromHex("#269DC9");
                    headerBgColor = datesBgColor = Color.LightBlue;
                }
                else
                {
                    datesVisibility = true;
                    headerVisibility = locationVisibility = false;
                    datesTextColor = headerTextColor = locationTextColor = Color.White;
                    datebox = true; headerbox = locationbox = false;
                    datesBgColor = Color.FromHex("#269DC9");
                    headerBgColor = locationBgColor = Color.LightBlue;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HeaderTabClicked method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets when clicked on ReqPickUp Date field.
        /// </summary>
        /// <param name="sender"></param>
        private async void ReqPickUpDateTap(object sender)
        {
            try
            {
                nullableDatePicker = sender as NullableDatePicker;
                nullableDatePicker.Focus();
                pickerOf = "pickupdate";
                nullableDatePicker.PropertyChanged += TimeDatePropertyChanged;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ReqPickUpDateTap method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when any date picker is clicked.
        /// </summary>
        /// <param name="sender"></param>
        private async void TimePickerClick(object sender)
        {
            try
            {
                var timePicker = sender as TimePicker;
                timePicker.Focus();
                pickerOf = timePicker.ClassId;
                timePicker.PropertyChanged += TimeDatePropertyChanged;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TimePickerClick method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Get called when clicked on Delivery Date field.
        /// </summary>
        /// <param name="sender"></param>
        private async void ReqDeliveryDateTap(object sender)
        {
            try
            {
                nullableDatePicker = sender as NullableDatePicker;
                nullableDatePicker.Focus();
                pickerOf = "deliverydate";
                nullableDatePicker.PropertyChanged += TimeDatePropertyChanged;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ReqDeliveryDateTap method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when Time/Date is choosen from TimePicker/DatePicker respectively.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TimeDatePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                NullableDatePicker date = sender as NullableDatePicker;
                TimePicker time = sender as TimePicker;

                if (date != null)
                {
                    string selectedDate = date.Date.ToString(Settings.DateFormatformAPI);

                    if (!string.IsNullOrEmpty(selectedDate))
                    {
                        if (pickerOf == "pickupdate")
                        {
                            showpickupdatehint = true;
                            ReqPickUpDateSelected = selectedDate;
                            Settings.reqPickUpDateyShip = selectedDate;
                            nullableDatePicker.CleanDate();
                        }

                        else if (pickerOf == "deliverydate")
                        {
                            showdeldatehint = true;
                            ReqDeliveryDateSelected = selectedDate;
                            Settings.reqDeliveryDateyShip = selectedDate;
                            nullableDatePicker.CleanDate();
                        }
                    }
                }
                else if (time != null)
                {
                    if (pickerOf == "pickuptime")
                    {
                        showpickuptimehint = true;
                        PickUpTimeSelected = TimeFormating(time);
                        Settings.pickUpTimeyShip = PickUpTimeSelected;
                    }

                    else if (pickerOf == "deliverytime")
                    {
                        showdeltimehint = true;
                        DeliveryTimeSelected = TimeFormating(time);
                        Settings.deliveryTimeyShip = DeliveryTimeSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "TimeDatePropertyChanged method-> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Used to set the time format.
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
                YPSLogger.ReportException(ex, "TimeFormating constructor -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                return null;
            }
        }

        /// <summary>
        /// This is used to get the DDL values from DB.
        /// </summary>
        public async void Searchdatapicker()
        {
            try
            {
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var filterData = await service.GetyShipHeaderFilterDataService();

                    if (filterData != null)
                    {
                        if (filterData.status != 0)
                        {
                            Settings.yShipDDLValues = filterData.data;
                            HeaderFilterData();
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Searchdatapicker method-> in yShipFilterDataViewModel.cs" + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is for binding the existing entered/selected filed values to their respective fields.
        /// called during the page loding.
        /// </summary>
        private async void HeaderFilterData()
        {
            try
            {
                IndicatorVisibility = true;

                if (Settings.yShipDDLValues != null)
                {
                    var bkgval = Settings.yShipDDLValues.BkgConfirmed.ToList();
                    bkgConfirmItems = new List<DDLmaster>();
                    bkgConfirmItems.Add(new DDLmaster() { Name = "ALL", ID = -1 });
                    bkgConfirmItems.AddRange(bkgval);
                    bkgConfirmAllItems = bkgConfirmItems.Select(x => x.Name).ToList();

                    cancelDDLItems = new List<DDLmaster>();
                    cancelDDLItems.Add(new DDLmaster() { Name = "ALL", ID = -1 });
                    cancelDDLItems.Add(new DDLmaster() { Name = "No", ID = 0 });
                    cancelDDLItems.Add(new DDLmaster() { Name = "Yes", ID = 1 });
                    cancelAllItems = cancelDDLItems.Select(x => x.Name).ToList();

                    completedDDLItems = new List<DDLmaster>();
                    completedDDLItems.Add(new DDLmaster() { Name = "ALL", ID = -1 });
                    completedDDLItems.Add(new DDLmaster() { Name = "No", ID = 0 });
                    completedDDLItems.Add(new DDLmaster() { Name = "Yes", ID = 1 });
                    completedAllItems = completedDDLItems.Select(x => x.Name).ToList();


                    var eqmtval = Settings.yShipDDLValues.LoadType.ToList();
                    eqmtTypeDDLItems = new List<DDLmaster>();
                    eqmtTypeDDLItems.Add(new DDLmaster() { Name = "ALL", ID = 0 });
                    eqmtTypeDDLItems.AddRange(eqmtval);
                    eqmtTypeAllItems = eqmtTypeDDLItems.Select(x => x.Name).ToList();

                    BindKeyTabValues();
                    BindLocationTabValues();
                    BindDateTabValues();
                }
                else
                {
                    try
                    {
                        Searchdatapicker();
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "Inner catch block in HeaderFilterData method-> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                        await service.Handleexception(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HeaderFilterData method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// This is for binding the existing Key tab field values.
        /// </summary>
        private async void BindKeyTabValues()
        {
            try
            {
                ybkgNumber = !(String.IsNullOrEmpty(Settings.ybkgNumberyShip)) ? Settings.ybkgNumberyShip : ybkgNumber;
                yshipBkgNumber = !(String.IsNullOrEmpty(Settings.yshipBkgNumberyShip)) ? Settings.yshipBkgNumberyShip : yshipBkgNumber;
                shipNumber = !(String.IsNullOrEmpty(Settings.shippingNumberyShip)) ? Settings.shippingNumberyShip : shipNumber;
                bkgRefnumber = !(String.IsNullOrEmpty(Settings.bkgRefNumberyShip)) ? Settings.bkgRefNumberyShip : bkgRefnumber;
                bkgConfirmDDLDisplayVal = Settings.bkgConfirmIdyShip != -1 ? bkgConfirmItems.Where(x => x.ID == Settings.bkgConfirmIdyShip).Select(c => c.Name).FirstOrDefault() : bkgConfirmAllItems.FirstOrDefault();
                cancelDDLDisplayVal = Settings.cancelIdyShip != -1 ? cancelDDLItems.Where(x => x.ID == Settings.cancelIdyShip).Select(c => c.Name).FirstOrDefault() : cancelAllItems.FirstOrDefault();
                completeDDLDisplayVal = Settings.completedIdyShip != -1 ? completedDDLItems.Where(x => x.ID == Settings.completedIdyShip).Select(c => c.Name).FirstOrDefault() : completedAllItems.FirstOrDefault();
                eqmtTypeDDLDisplayVal = Settings.eqmtTypeIdyShip != -1 ? eqmtTypeDDLItems.Where(x => x.ID == Settings.eqmtTypeIdyShip).Select(c => c.Name).FirstOrDefault() : eqmtTypeAllItems.FirstOrDefault();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindingKeyValues method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is for binding the existing Location tab field values.
        /// </summary>
        private async void BindLocationTabValues()
        {
            try
            {
                oriSearch = Settings.oriSearchIdyShip != 0 ? Settings.oriSearchNameyShip : oriSearch;
                destSearch = Settings.destSearchIdyShip != 0 ? Settings.destSearchNameyShip : destSearch;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindLocationTabValues method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This is for binding the existing Date tab field values.
        /// </summary>
        private async void BindDateTabValues()
        {
            try
            {
                ReqPickUpDateSelected = !(String.IsNullOrEmpty(Settings.reqPickUpDateyShip)) ? Settings.reqPickUpDateyShip : ReqPickUpDateSelected;
                ReqDeliveryDateSelected = !(String.IsNullOrEmpty(Settings.reqDeliveryDateyShip)) ? Settings.reqDeliveryDateyShip : ReqDeliveryDateSelected;
                PickUpTimeSelected = !(String.IsNullOrEmpty(Settings.pickUpTimeyShip)) ? Settings.pickUpTimeyShip : PickUpTimeSelected;
                DeliveryTimeSelected = !(String.IsNullOrEmpty(Settings.deliveryTimeyShip)) ? Settings.deliveryTimeyShip : DeliveryTimeSelected;

                //Show the hint if found value
                showdeldatehint = !string.IsNullOrEmpty(Settings.reqDeliveryDateyShip) ? true : false;
                showdeltimehint = !string.IsNullOrEmpty(Settings.deliveryTimeyShip) ? true : false;
                showpickupdatehint = !string.IsNullOrEmpty(Settings.reqPickUpDateyShip) ? true : false;
                showpickuptimehint = !string.IsNullOrEmpty(Settings.pickUpTimeyShip) ? true : false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BindDateTabValues method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Bkg Confirm DropDownList.
        /// </summary>
        public async void BkgConfirmDDLValSelected()
        {
            try
            {
                if (bkgConfirmDDLDisplayVal != null)
                {
                    var val = bkgConfirmItems.Where(X => X.Name == bkgConfirmDDLDisplayVal).FirstOrDefault();
                    Settings.bkgConfirmIdyShip = val.ID;
                    Settings.bkgConfirmNameyShip = val.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BkgConfirmDDLValSelected method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Cancel DropDownList.
        /// </summary>
        public async void CancelDDLValueSelected()
        {
            try
            {
                if (selectedCancelValue != null)
                {
                    cancelDDLDisplayVal = selectedCancelValue;
                    var val = cancelDDLItems.Where(X => X.Name == cancelDDLDisplayVal).FirstOrDefault();
                    Settings.cancelIdyShip = val.ID;
                    Settings.cancelNameyShip = val.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CancelDDLValueSelected method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Completed DropDownList.
        /// </summary>
        public async void CompletedDDLValueSelected()
        {
            try
            {
                if (selectedCompletedValue != null)
                {
                    completeDDLDisplayVal = selectedCompletedValue;
                    var val = completedDDLItems.Where(X => X.Name == completeDDLDisplayVal).FirstOrDefault();
                    Settings.completedIdyShip = val.ID;
                    Settings.completedNameyShip = val.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CompletedDDLValueSelected method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when an item is selected from Eqmt Type DropDownList.
        /// </summary>
        public async void EqmtTypeDDLValueSelected()
        {
            try
            {
                if (selectedEqmtTypeValue != null)
                {
                    eqmtTypeDDLDisplayVal = selectedEqmtTypeValue;
                    var val = eqmtTypeDDLItems.Where(X => X.Name == eqmtTypeDDLDisplayVal).FirstOrDefault();
                    Settings.eqmtTypeIdyShip = val.ID;
                    Settings.eqmtTypeNameyShip = val.Name;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EqmtTypeDDLValueSelected method -> in yShipFilterDataViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        #region Properties
        private bool _SearchContentView = false;
        public bool SearchContentView
        {
            get { return _SearchContentView; }
            set
            {
                _SearchContentView = value;
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

        private bool _NoResultsIsVisibleLbl = true;
        public bool NoResultsIsVisibleLbl
        {
            get { return _NoResultsIsVisibleLbl; }
            set
            {
                _NoResultsIsVisibleLbl = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isLocationListViewVisible = false;
        public bool isLocationListViewVisible
        {
            get { return _isLocationListViewVisible; }
            set
            {
                _isLocationListViewVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _headerbox = true;
        public bool headerbox
        {
            get { return _headerbox; }
            set
            {
                _headerbox = value;
                NotifyPropertyChanged();
            }
        }
        private bool _locationbox = false;
        public bool locationbox
        {
            get { return _locationbox; }
            set
            {
                _locationbox = value;
                NotifyPropertyChanged();
            }
        }
        private bool _datebox = false;
        public bool datebox
        {
            get { return _datebox; }
            set
            {
                _datebox = value;
                NotifyPropertyChanged();
            }
        }

        #region Date Properties

        private string _ReqPickUpDateSelected = "Req PickUp Date";
        public string ReqPickUpDateSelected
        {
            get { return _ReqPickUpDateSelected; }
            set
            {
                _ReqPickUpDateSelected = value;
                RaisePropertyChanged("ReqPickUpDateSelected");
            }
        }

        private string _PickUpTimeSelected = "PickUp Time";
        public string PickUpTimeSelected
        {
            get { return _PickUpTimeSelected; }
            set
            {
                _PickUpTimeSelected = value;
                RaisePropertyChanged("PickUpTimeSelected");
            }
        }
        private string _ReqDeliveryDateSelected = "Req Delivery Date";
        public string ReqDeliveryDateSelected
        {
            get { return _ReqDeliveryDateSelected; }
            set
            {
                _ReqDeliveryDateSelected = value;
                RaisePropertyChanged("ReqDeliveryDateSelected");
            }
        }

        private string _DeliveryTimeSelected = "Delivery Time";
        public string DeliveryTimeSelected
        {
            get { return _DeliveryTimeSelected; }
            set
            {
                _DeliveryTimeSelected = value;
                RaisePropertyChanged("DeliveryTimeSelected");
            }
        }
        #endregion

        #region Key tab related field properties
        private List<DDLmaster> _bkgConfirmItems;
        public List<DDLmaster> bkgConfirmItems
        {
            get { return _bkgConfirmItems; }
            set
            {
                _bkgConfirmItems = value;
                NotifyPropertyChanged();
            }
        }

        private List<DDLmaster> _cancelDDLItems;
        public List<DDLmaster> cancelDDLItems
        {
            get { return _cancelDDLItems; }
            set
            {
                _cancelDDLItems = value;
                NotifyPropertyChanged();
            }
        }


        private List<DDLmaster> _completedDDLItems;
        public List<DDLmaster> completedDDLItems
        {
            get { return _completedDDLItems; }
            set
            {
                _completedDDLItems = value;
                NotifyPropertyChanged();
            }
        }


        private List<DDLmaster> _eqmtTypeDDLItems;
        public List<DDLmaster> eqmtTypeDDLItems
        {
            get { return _eqmtTypeDDLItems; }
            set
            {
                _eqmtTypeDDLItems = value;
                NotifyPropertyChanged();
            }
        }


        private string _ybkgNumber = string.Empty;
        public string ybkgNumber
        {
            get { return _ybkgNumber; }
            set
            {
                _ybkgNumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _yshipBkgNumber = string.Empty;
        public string yshipBkgNumber
        {
            get { return _yshipBkgNumber; }
            set
            {
                _yshipBkgNumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _shipNumber = string.Empty;
        public string shipNumber
        {
            get { return _shipNumber; }
            set
            {
                _shipNumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _bkgRefnumber = string.Empty;
        public string bkgRefnumber
        {
            get { return _bkgRefnumber; }
            set
            {
                _bkgRefnumber = value;
                NotifyPropertyChanged();
            }
        }

        private string _selectedBkgConfirm;
        public string selectedBkgConfirm
        {
            get { return _selectedBkgConfirm; }
            set
            {
                _selectedBkgConfirm = value;
                BkgConfirmDDLValSelected();
            }
        }

        private string _selectedCancelValue;
        public string selectedCancelValue
        {
            get { return _selectedCancelValue; }
            set
            {
                _selectedCancelValue = value;
                CancelDDLValueSelected();
            }
        }

        private string _selectedCompletedValue;
        public string selectedCompletedValue
        {
            get { return _selectedCompletedValue; }
            set
            {
                _selectedCompletedValue = value;
                CompletedDDLValueSelected();
            }
        }

        private string _selectedEqmtTypeValue;
        public string selectedEqmtTypeValue
        {
            get { return _selectedEqmtTypeValue; }
            set
            {
                _selectedEqmtTypeValue = value;
                EqmtTypeDDLValueSelected();
            }
        }
        //.....
        #region Set default values for Picker
        private string _bkgConfirmDDLDisplayVal;
        public string bkgConfirmDDLDisplayVal
        {
            get { return _bkgConfirmDDLDisplayVal; }
            set
            {
                _bkgConfirmDDLDisplayVal = value;
                NotifyPropertyChanged();
            }
        }
        private string _cancelDDLDisplayVal;
        public string cancelDDLDisplayVal
        {
            get { return _cancelDDLDisplayVal; }
            set
            {
                _cancelDDLDisplayVal = value;
                NotifyPropertyChanged();
            }
        }

        private string _completeDDLDisplayVal;
        public string completeDDLDisplayVal
        {
            get { return _completeDDLDisplayVal; }
            set
            {
                _completeDDLDisplayVal = value;
                NotifyPropertyChanged();
            }
        }

        private string _eqmtTypeDDLDisplayVal;
        public string eqmtTypeDDLDisplayVal
        {
            get { return _eqmtTypeDDLDisplayVal; }
            set
            {
                _eqmtTypeDDLDisplayVal = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region Set default value for Picker Labels

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

        private string _oriSearch = "ALL";
        public string oriSearch
        {
            get { return _oriSearch; }
            set
            {
                _oriSearch = value;
                NotifyPropertyChanged();
            }
        }

        private string _destSearch = "ALL";
        public string destSearch
        {
            get { return _destSearch; }
            set
            {
                _destSearch = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

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

        #region Picker ItemSources
        private List<string> _bkgConfirmAllItems;
        public List<string> bkgConfirmAllItems
        {
            get { return _bkgConfirmAllItems; }
            set
            {
                _bkgConfirmAllItems = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _cancelAllItems;
        public List<string> cancelAllItems
        {
            get { return _cancelAllItems; }
            set
            {
                _cancelAllItems = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _completedAllItems;
        public List<string> completedAllItems
        {
            get { return _completedAllItems; }
            set
            {
                _completedAllItems = value;
                NotifyPropertyChanged();
            }
        }

        private List<string> _eqmtTypeAllItems;
        public List<string> eqmtTypeAllItems
        {
            get { return _eqmtTypeAllItems; }
            set
            {
                _eqmtTypeAllItems = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<SearchData> _SearchLocationResults;
        public ObservableCollection<SearchData> SearchLocationResults
        {
            get { return _SearchLocationResults; }
            set
            {
                _SearchLocationResults = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region filter tabs visibility
        public bool _headerVisibility = true;
        public bool headerVisibility
        {
            get { return _headerVisibility; }
            set
            {
                _headerVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public bool _locationVisibility = false;
        public bool locationVisibility
        {
            get { return _locationVisibility; }
            set
            {
                _locationVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public bool _datesVisibility = false;
        public bool datesVisibility
        {
            get { return _datesVisibility; }
            set
            {
                _datesVisibility = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region  tabs text color

        public Color _headerTextColor = Color.White;
        public Color headerTextColor
        {
            get { return _headerTextColor; }
            set
            {
                _headerTextColor = value;
                NotifyPropertyChanged();
            }
        }

        public Color _locationTextColor = Color.White;
        public Color locationTextColor
        {
            get { return _locationTextColor; }
            set
            {
                _locationTextColor = value;
                NotifyPropertyChanged();
            }
        }

        public Color _datesTextColor = Color.White;
        public Color datesTextColor
        {
            get { return _datesTextColor; }
            set
            {
                _datesTextColor = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region  tabs background color

        public Color _headerBgColor = Color.FromHex("#269DC9");
        public Color headerBgColor
        {
            get { return _headerBgColor; }
            set
            {
                _headerBgColor = value;
                NotifyPropertyChanged();
            }
        }

        public Color _locationBgColor = Color.LightBlue;
        public Color locationBgColor
        {
            get { return _locationBgColor; }
            set
            {
                _locationBgColor = value;
                NotifyPropertyChanged();
            }
        }

        public Color _datesBgColor = Color.LightBlue;
        public Color datesBgColor
        {
            get { return _datesBgColor; }
            set
            {
                _datesBgColor = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Properties for dynamic label change
        public class filtterlabelclass
        {
            public filtterlabelFields yBkgNumber { get; set; } = new filtterlabelFields { Status = true, Name = "yBkgNumber" };
            public filtterlabelFields yShipBkgNumber { get; set; } = new filtterlabelFields { Status = true, Name = "yShipBkgNumber" };
            public filtterlabelFields ShippingNumber { get; set; } = new filtterlabelFields { Status = true, Name = "ShippingNumber" };
            public filtterlabelFields BkgRefNumber { get; set; } = new filtterlabelFields { Status = true, Name = "BkgRefNo" };
            public filtterlabelFields BkgConfirm { get; set; } = new filtterlabelFields { Status = true, Name = "IsBkgConfirmed" };
            public filtterlabelFields Cancel { get; set; } = new filtterlabelFields { Status = true, Name = "Cancel" };
            public filtterlabelFields Completed { get; set; } = new filtterlabelFields { Status = true, Name = "Complete" };
            public filtterlabelFields EqmtType { get; set; } = new filtterlabelFields { Status = true, Name = "EqmtType" };
            public filtterlabelFields OriSearch { get; set; } = new filtterlabelFields { Status = true, Name = "OrgSearch" };
            public filtterlabelFields DestSearch { get; set; } = new filtterlabelFields { Status = true, Name = "DestSearch" };
            public filtterlabelFields PickUp { get; set; } = new filtterlabelFields { Status = true, Name = "PickUp" };
            public filtterlabelFields Delivery { get; set; } = new filtterlabelFields { Status = true, Name = "Delivery" };
            public filtterlabelFields ReqPickupDate { get; set; } = new filtterlabelFields { Status = true, Name = "ReqPickupDate" };
            public filtterlabelFields PickupTime { get; set; } = new filtterlabelFields { Status = true, Name = "PickUpTime" };
            public filtterlabelFields ReqDeliveryDate { get; set; } = new filtterlabelFields { Status = true, Name = "ReqDeliveryDate" };
            public filtterlabelFields DeliveryTime { get; set; } = new filtterlabelFields { Status = true, Name = "DeliveryTime" };
            public filtterlabelFields ResetBtn { get; set; } = new filtterlabelFields { Status = true, Name = "Reset" };
            public filtterlabelFields SearchBtn { get; set; } = new filtterlabelFields { Status = true, Name = "Search" };

        }
        public class filtterlabelFields : IBase
        {
            public bool Status { get; set; }
            public string Name { get; set; }
        }

        public filtterlabelclass _labelobj = new filtterlabelclass();
        public filtterlabelclass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
            }
        }
        #endregion


        private bool _showpickupdatehint;
        public bool showpickupdatehint
        {
            get { return _showpickupdatehint; }
            set
            {
                _showpickupdatehint = value;
                NotifyPropertyChanged();
            }
        }

        private bool _showpickuptimehint;
        public bool showpickuptimehint
        {
            get { return _showpickuptimehint; }
            set
            {
                _showpickuptimehint = value;
                NotifyPropertyChanged();
            }
        }

        private bool _showdeldatehint;
        public bool showdeldatehint
        {
            get { return _showdeldatehint; }
            set
            {
                _showdeldatehint = value;
                NotifyPropertyChanged();
            }
        }

        private bool _showdeltimehint;
        public bool showdeltimehint
        {
            get { return _showdeltimehint; }
            set
            {
                _showdeltimehint = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
