using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Behaviours;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YShipFilter : ContentPage
    {
        #region Data member declaration
        YShipFilterViewModel vm;
        YPSService yPSService;
        bool isValid;
        Int64 result;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="navigation"></param>
        /// <param name="yBkgNo"></param>
        /// <param name="yShipBkgNumber"></param>
        /// <param name="yShipId"></param>
        public YShipFilter(INavigation navigation, string yBkgNo, string yShipBkgNumber, int yShipId)
        {
            try
            {
                InitializeComponent();
                BindingContext = vm = new YShipFilterViewModel(navigation, yBkgNo, yShipBkgNumber, yShipId);
                yPSService = new YPSService();
                FunctionAccesBasedOnRole();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "YShipFilter constructor -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Validating EqmtTypeQty fields  & Summing up all the Qty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EqmtTypeQty_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var id = ((Entry)sender).ClassId;

                var argValue = e.NewTextValue == "" ? "0" : e.NewTextValue;

                if (!(string.IsNullOrEmpty(argValue)))
                {
                    isValid = Int64.TryParse(argValue, out result);

                    if (isValid)
                    {
                        if (id.Trim().ToLower() == "etype1")
                        {
                            vm.EntryEqmtType1Qty = e.NewTextValue;
                        }

                        if (id.Trim().ToLower() == "etype2")
                        {
                            vm.EntryEqmtType2Qty = e.NewTextValue;
                        }

                        if (id.Trim().ToLower() == "etype3")
                        {
                            vm.EntryEqmtType3Qty = e.NewTextValue;
                        }
                        vm.AddTotalEqmtQty();
                    }
                    else
                    {
                        ((Entry)sender).Text = e.OldTextValue;
                    }

                    var entry = sender as Entry;

                    if (string.IsNullOrEmpty(entry.Text))
                    {
                        EqmtTypeFeldVal(entry, id);
                    }
                    else
                    {
                        if (id.Trim().ToLower() == "etype1")
                        {
                            framEqmtType1Qty.HasError = false;

                        }

                        if (id.Trim().ToLower() == "etype2")
                        {
                            framEqmtType2Qty.HasError = false;
                        }

                        if (id.Trim().ToLower() == "etype3")
                        {
                            framEqmtType3Qty.HasError = false;
                        }
                    }
                }
                else
                {
                    ((Entry)sender).Text = e.OldTextValue;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EqmtTypeQty_TextChanged method -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Validating the EqmtTypeQty's entered field value
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="id"></param>
        private void EqmtTypeFeldVal(Entry entry, string id)
        {
            try
            {
                if (id.Trim().ToLower() == "etype1")
                {
                    if (vm.EqmtType1DefaultLblValue.Trim().ToLower() != "select")
                    {
                        framEqmtType1Qty.HasError = true;
                    }
                    else
                    {
                        framEqmtType1Qty.HasError = false;
                    }
                }

                if (id.Trim().ToLower() == "etype2")
                {
                    if (vm.EqmtType2DefaultLblValue.Trim().ToLower() != "select")
                    {
                        framEqmtType2Qty.HasError = true;
                    }
                    else
                    {
                        framEqmtType2Qty.HasError = false;
                    }
                }

                if (id.Trim().ToLower() == "etype3")
                {
                    if (vm.EqmtType3DefaultLblValue.Trim().ToLower() != "select")
                    {
                        framEqmtType3Qty.HasError = true;
                    }
                    else
                    {
                        framEqmtType3Qty.HasError = false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EqmtTypeFeldVal method -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when cross icon present near any location's search textbox is clicked 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Tapped(object sender, EventArgs e)
        {
            try
            {
                SearchBox.Text = "";
                vm.SearchContentVForAll = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Close_Tapped method -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when value is entered/cleared in any location's search field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                /// Verifying internet connection.
                bool InternetC = await App.CheckInterNetConnection();

                if (InternetC)
                {
                    int limit = 4;
                    string textSize = e.NewTextValue;

                    if (textSize.Length >= limit)
                    {
                        vm.IndicatorVisibility = true;
                        await Task.Delay(500);
                        /// Calling search location API to get saerch locations list.
                        var getModelResult = await yPSService.SearchLocation(textSize);

                        if (getModelResult != null)
                        {
                            if (getModelResult.status != 0)
                            {
                                vm.NoResultIsVisibleLbl = false;
                                vm.OrgSearchListStack = true;
                                vm.SearchListItems = getModelResult.data;
                            }
                            else
                            {
                                vm.NoResultIsVisibleLbl = true;
                                vm.OrgSearchListStack = false;
                                vm.NoResultsLbl = "No results found";
                            }
                            vm.IndicatorVisibility = false;
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        int countV = 4 - textSize.Length;
                        vm.NoResultsLbl = "Please enter " + countV + " or more characters";

                        if (vm.SearchListItems != null)
                        {
                            vm.SearchListItems.Clear();
                        }
                        vm.NoResultIsVisibleLbl = true;
                        vm.OrgSearchListStack = false;
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Search_TextChanged method -> in YShipFilter.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
            finally
            {
                vm.IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when item is selected from the location's search result list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var itemSelected = e.Item as SearchData;
                CVListView.SelectedItem = null;
                vm.SearchContentVForAll = false;
                SearchBox.Text = "";

                if (vm.EntrySearchType.Trim().ToLower() == "orgsearch")
                {
                    vm.OrgSearchLocationLbl = itemSelected.Name;
                    vm.OrgSearchLocationLblID = itemSelected.ID;
                    vm.EtyOrgCntryCode = itemSelected.NationHash;
                    vm.EtyOrgCntryName = itemSelected.Nation;
                    vm.EtyOrgLocCode = itemSelected.LocationHash;
                    vm.EtyOrgLocName = itemSelected.Location;
                }
                else if (vm.EntrySearchType.Trim().ToLower() == "destsearch")
                {

                    vm.DestSearchLocationLbl = itemSelected.Name;
                    vm.DestSearchLocationLblID = itemSelected.ID;
                    vm.EtyDestCntryCode = itemSelected.NationHash;
                    vm.EtyDestCntryName = itemSelected.Nation;
                    vm.EtyDestLocCode = itemSelected.LocationHash;
                    vm.EtyDestLocName = itemSelected.Location;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ItemTapped method -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when value is entered/cleared from Total Amount field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EtyTotalAmount_TextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                var text = sender as Entry;
                bool result = Regex.IsMatch(text.Text, @"^(\d{0,11}(\.\d{0,4})?)$");

                if (result == false)
                {
                    ((Entry)sender).Text = args.OldTextValue;

                    if (!text.Text.Contains("."))
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please enter only 11 digits before decimal.");
                    }
                }
                else
                {
                    var getValue = text.Text == "" ? "0" : text.Text;
                    vm.EtyTotalAmount = Convert.ToDecimal(getValue);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EtyTotalAmount_TextChanged method -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when value is entered/changed in TotalM3 field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EtyTotalM3_TextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                var text = sender as Entry;
                bool result = Regex.IsMatch(text.Text, @"^(\d{0,11}(\.\d{0,4})?)$");

                if (result == false)
                {
                    ((Entry)sender).Text = args.OldTextValue;

                    if (!text.Text.Contains("."))
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please enter only 11 digits before decimal.");
                    }
                }
                else
                {
                    var getValue = text.Text == "" ? "0" : text.Text;
                    vm.EtyTotalM3 = Convert.ToDecimal(getValue);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EtyTotalM3_TextChanged method -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when value is entered/cleared from Total Gross Weight.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EtyTotalGrossWt_TextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                var text = sender as Entry;
                bool result = Regex.IsMatch(text.Text, @"^(\d{0,11}(\.\d{0,4})?)$");

                if (result == false)
                {
                    ((Entry)sender).Text = args.OldTextValue;

                    if (!text.Text.Contains("."))
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please enter only 11 digits before decimal.");
                    }
                }
                else
                {
                    var getValue = text.Text == "" ? "0" : text.Text;
                    vm.EtyTotalGrossWt = Convert.ToDecimal(getValue);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EtyTotalGrossWt_TextChanged method -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when value is entered/chenged in Total Net Weitht field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EtyTotalNetWt_TextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                var text = sender as Entry;
                bool result = Regex.IsMatch(text.Text, @"^(\d{0,11}(\.\d{0,4})?)$");

                if (result == false)
                {
                    ((Entry)sender).Text = args.OldTextValue;

                    if (!text.Text.Contains("."))
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please enter only 11 digits before decimal.");
                    }
                }
                else
                {
                    var getValue = text.Text == "" ? "0" : text.Text;
                    vm.EtyTotalNetWt = Convert.ToDecimal(getValue);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "EtyTotalNetWt_TextChanged method -> in YShipFilter.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is for providing access to the tab function(s) based on user role.
        /// </summary>
        /// <returns></returns>
        private async Task FunctionAccesBasedOnRole()
        {
            try
            {
                if (Settings.userRoleID == (int)UserRoles.SupplierAdmin || Settings.userRoleID == (int)UserRoles.SupplierUser)
                {
                    await vm.TabLocationDetails();
                    await BasicDetailsDisable();
                    sublitbtn.IsEnabled = true;
                    sublitbtn.Opacity = 1;
                }

                if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser ||
                    Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser)
                {
                    await vm.TabBasicDetails();
                    await BasicDetailsDisable();
                    await LocationDetailsDisable();
                    await ShipmentDetailsDisable();
                    await OtherDetailsDisable();
                    sublitbtn.IsEnabled = false;
                    sublitbtn.Opacity = 0.3;
                }

                if (Settings.userRoleID == (int)UserRoles.LogisticsAdmin || Settings.userRoleID == (int)UserRoles.LogisticsUser ||
                    Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                {
                    await vm.TabOtherDetails();
                    await BasicDetailsDisable();
                    await LocationDetailsDisable();
                    await ShipmentDetailsDisable();
                    sublitbtn.IsEnabled = true;
                    sublitbtn.Opacity = 1;
                }

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FunctionAccesBasedOnRole method -> in YShipFilter.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to disable controls present in Basic tab.
        /// </summary>
        /// <returns></returns>
        private async Task BasicDetailsDisable()
        {
            try
            {
                bkgrefname.IsReadOnly = true;
                bkgrefname.Opacity = 0.3;
                bkgrefnamelayout.Opacity = 0.3;
                shipmenttypestackname.Opacity = 0.3;
                ShmtTypePicker.IsEnabled = false;
                bkgconfirmstackname.Opacity = 0.3;
                BkgConfirmedPicker.IsEnabled = false;
                eqmttype1stackname.Opacity = 0.3;
                EqmtType1Picker.IsEnabled = false;
                framEqmtType1Qty.Opacity = 0.3;
                eqmttype1fieldname.Opacity = 0.3;
                eqmttype1fieldname.IsReadOnly = true;
                eqmttype2stackname.Opacity = 0.3;
                EqmtType2Picker.IsEnabled = false;
                framEqmtType2Qty.Opacity = 0.3;
                eqmttype2fieldname.Opacity = 0.3;
                eqmttype2fieldname.IsReadOnly = true;
                eqmttype3stackname.Opacity = 0.3;
                EqmtType3Picker.IsEnabled = false;
                framEqmtType3Qty.Opacity = 0.3;
                eqmttype3fieldname.Opacity = 0.3;
                eqmttype3fieldname.IsReadOnly = true;
                cancelshipbtn.IsEnabled = false;
                completeshpbtn.IsEnabled = false;
                cancelshipbtn.Opacity = 0.3;
                completeshpbtn.Opacity = 0.3;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BasicDetailsDisable method -> in YShipFilter.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to disable controls present in Location tab
        /// </summary>
        /// <returns></returns>
        private async Task LocationDetailsDisable()
        {
            try
            {
                shipperfieldname.Opacity = 0.3;
                shipperfieldname.IsReadOnly = true;
                shipperfieldnamelayout.Opacity = 0.3;
                orgsearchstackname.Opacity = 0.3;
                OrgSearchPicker.IsEnabled = false;
                reqpickupaddfieldname.Opacity = 0.3;
                reqpickupaddfieldname.IsReadOnly = true;
                reqpickupaddfieldnamelayout.Opacity = 0.3;
                etddatestackname.Opacity = 0.3;
                etddatestackname.IsEnabled = false;
                etddatestacknamelayout.Opacity = 0.3;
                pickuptimestackname.Opacity = 0.3;
                pickuptimestackname.IsEnabled = false;
                pickuptimestacknamelayout.Opacity = 0.3;
                pickuppicfieldname.Opacity = 0.3;
                pickuppicfieldname.IsEnabled = false;
                pickuppicfieldnamelayout.Opacity = 0.3;
                shiptopartyfieldnamelayout.Opacity = 0.3;
                shiptopartyfieldname.Opacity = 0.3;
                shiptopartyfieldname.IsReadOnly = true;
                destsearchstackname.Opacity = 0.3;
                DestSearchPicker.IsEnabled = false;
                deliverylocfieldname.Opacity = 0.3;
                deliverylocfieldname.IsReadOnly = true;
                deliverylocfieldnamelayout.Opacity = 0.3;
                etadatestackname.Opacity = 0.3;
                etadatestackname.IsEnabled = false;
                etadatestacknamelayout.Opacity = 0.3;
                deliverttimestackname.Opacity = 0.3;
                deliverttimestackname.IsEnabled = false;
                deliverttimestacknamelayout.Opacity = 0.3;
                destpicnfieldname.Opacity = 0.3;
                destpicnfieldname.IsReadOnly = true;
                destpicnfieldnamelayout.Opacity = 0.3;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LocationDetailsDisable method -> in YShipFilter.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to disable controls present in Shipment tab.
        /// </summary>
        /// <returns></returns>
        private async Task ShipmentDetailsDisable()
        {
            try
            {
                shipmentloadedbystackname.Opacity = 0.3;
                ShipmentLoadedByPicker.IsEnabled = false;
                shipmentopenedbyfieldname.Opacity = 0.3;
                shipmentopenedbyfieldname.IsReadOnly = true;
                shipmentopenedbyfieldnamelayout.Opacity = 0.3;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShipmentDetailsDisable method -> in YShipFilter.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to disable controls present in Other tab.
        /// </summary>
        /// <returns></returns>
        private async Task OtherDetailsDisable()
        {
            try
            {
                invoicefieldname.Opacity = 0.3;
                invoicefieldname.IsReadOnly = true;
                invoicefieldnamelayout.Opacity = 0.3;
                custrefnofieldname.Opacity = 0.3;
                custrefnofieldname.IsReadOnly = true;
                custrefnofieldnamelayout.Opacity = 0.3;
                shmttermstackname.Opacity = 0.3;
                ShmtTermPicker.IsEnabled = false;
                specialcargohanfieldname.Opacity = 0.3;
                specialcargohanfieldname.IsReadOnly = true;
                specialcargohanfieldnamelayout.Opacity = 0.3;
                generaldescfieldname.Opacity = 0.3;
                generaldescfieldname.IsReadOnly = true;
                generaldescfieldnamelayout.Opacity = 0.3;
                currencyfieldname.Opacity = 0.3;
                currencyfieldname.IsReadOnly = true;
                currencyfieldnamelayout.Opacity = 0.3;
                totalamountfieldname.Opacity = 0.3;
                totalamountfieldname.IsReadOnly = true;
                totalamountfieldnamelayout.Opacity = 0.3;
                totalpkgfieldname.Opacity = 0.3;
                totalpkgfieldname.IsReadOnly = true;
                totalpkgfieldnamelayout.Opacity = 0.3;
                EtyPkgUnit.Opacity = 0.3;
                EtyPkgUnit.IsReadOnly = true;
                EtyPkgUnitlayout.Opacity = 0.3;
                EtyTotalM3.Opacity = 0.3;
                EtyTotalM3.IsReadOnly = true;
                EtyTotalM3layout.Opacity = 0.3;
                EtyTotalGrossWt.Opacity = 0.3;
                EtyTotalGrossWt.IsReadOnly = true;
                EtyTotalGrossWtlayout.Opacity = 0.3;
                EtyTotalNetWt.Opacity = 0.3;
                EtyTotalNetWt.IsReadOnly = true;
                EtyTotalNetWtlayout.Opacity = 0.3;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OtherDetailsDisable method -> in YShipFilter.xaml.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
        }


        /// <summary>
        /// Gets called when clicked on the Complete Shipment/Cancel Shipment button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void StackCancelOrComp_Tapped(object sender, EventArgs e)
        {
            try
            {
                var val = (StackLayout)sender;
                ViewTappedButtonBehavior buttontap = new ViewTappedButtonBehavior();

                if (val.StyleId == "completeshpStack")
                {
                    buttontap.View_Tapped(completeshpbtn, new EventArgs());
                    await vm.TabCompleteBtn();
                }
                else
                {
                    buttontap.View_Tapped(cancelshipbtn, new EventArgs());
                    await vm.TabCancelBtn();
                }
            }
            catch (Exception ex)
            {

                YPSLogger.ReportException(ex, "StackCancel_Tapped method -> in YShipFilter.xaml.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }
    }
}