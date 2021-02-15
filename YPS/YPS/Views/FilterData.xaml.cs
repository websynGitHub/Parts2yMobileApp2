using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterData : ContentPage
    {
        #region Data member declaration
        FilterDataViewModel vm;
        YPSService yPSService;
        bool checkInternet;
        #endregion

        /// <summary>
        /// Parameterless constructor. 
        /// </summary>
        public FilterData()
        {
            try
            {
                InitializeComponent();
                YPSLogger.TrackEvent("FilterData", "Page Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

                Settings.currentPage = "FilterDataPage";
                BindingContext = vm = new FilterDataViewModel(Navigation, this);
                yPSService = new YPSService();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "FilterData constructor -> in FilterData.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on the back button and redirect to previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in FileUpload.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        ///// <summary>
        ///// Gets called when cross icon present near to location search field  is clicked.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void ClosePop_Tapped(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        DeliverPlaceEntrySearch.Text = "";
        //        vm.SearchContentView = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        YPSLogger.ReportException(ex, "ClosePop_Tapped method -> in FilterData.cs " + Settings.userLoginID);
        //        yPSService.Handleexception(ex);
        //    }

        //}

        ///// <summary>
        ///// Gets called when text is change in location search textbox .
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private async void SearchPlaceEntry_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    try
        //    {
        //        //Verifying internet connection.
        //        checkInternet = await App.CheckInterNetConnection();

        //        if (checkInternet)
        //        {
        //            int limit = 4;
        //            string textSize = DeliverPlaceEntrySearch.Text;

        //            if (textSize.Length >= limit)
        //            {
        //                vm.IndicatorVisibility = true;
        //                await System.Threading.Tasks.Task.Delay(500);

        //                //Getting the location list with the searched text from API
        //                var result = await yPSService.SearchLocation(textSize);

        //                if (result.status != 0)
        //                {
        //                    //Binding the list to the deopdown
        //                    List<SearchData> addItems = new List<SearchData>();
        //                    addItems.Add(new SearchData() { Name = "ALL", ID = 0 });
        //                    addItems.AddRange(result.data);
        //                    vm.ListOfPlaceName = new ObservableCollection<SearchData>(addItems);
        //                    vm.NoResultsIsVisibleLbl = false;
        //                    vm.DeliverPlaceListViewStack = true;
        //                }
        //                else
        //                {
        //                    vm.NoResultsIsVisibleLbl = true;
        //                    vm.DeliverPlaceListViewStack = false;
        //                    vm.NoResultsLbl = "No results found";
        //                }
        //                vm.IndicatorVisibility = false;
        //            }
        //            else
        //            {
        //                int countV = 4 - textSize.Length;
        //                vm.NoResultsLbl = "Please enter " + countV + " or more characters";

        //                if (vm.ListOfPlaceName != null)
        //                {
        //                    vm.ListOfPlaceName.Clear();
        //                }

        //                vm.NoResultsIsVisibleLbl = true;
        //                vm.DeliverPlaceListViewStack = false;
        //            }
        //        }
        //        else
        //        {
        //            DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        YPSLogger.ReportException(ex, "SearchPlaceEntry_TextChanged method -> in FilterData.cs " + Settings.userLoginID);
        //        await yPSService.Handleexception(ex);
        //    }
        //    finally
        //    {
        //        vm.IndicatorVisibility = false;
        //    }
        //}

        ///// <summary>
        ///// Gets called when item is selected from the search result for location any field.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void SearchItemTapped(object sender, ItemTappedEventArgs e)
        //{
        //    try
        //    {
        //        SearchData itemSelected = e.Item as SearchData;
        //        DeliverPlaceListView.SelectedItem = null;
        //        //vm.SearchContentView = false;
        //        DeliverPlaceEntrySearch.Text = "";

        //        //if (vm.EntrySearchType.Trim().ToLower() == "pickupsearchl")
        //        //{
        //        //    Settings.LocationPickupName = vm.PickUpDefaultValue_L = itemSelected.Name;
        //        //    Settings.LocationPickupID = itemSelected.ID;
        //        //}
        //        //else if (vm.EntrySearchType.Trim().ToLower() == "polsearchl")
        //        //{
        //        //    Settings.LocationPOLName = vm.POLDefaultValue_L = itemSelected.Name;
        //        //    Settings.LocationPOLID = itemSelected.ID;
        //        //}
        //        //else if (vm.EntrySearchType.Trim().ToLower() == "podsearchl")
        //        //{
        //        //    Settings.LocationPODName = vm.PODDefaultValue_L = itemSelected.Name;
        //        //    Settings.LocationPODID = itemSelected.ID;
        //        //}
        //        //else if (vm.EntrySearchType.Trim().ToLower() == "deliverysearchl")
        //        //{
        //        //    Settings.LocationDeliverPlaceName = vm.DeliverPlaceDefaultValue_L = itemSelected.Name;
        //        //    Settings.LocationDeliverPlaceID = itemSelected.ID;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        YPSLogger.ReportException(ex, "SearchItemTapped method -> in FilterData.cs " + Settings.userLoginID);
        //        yPSService.Handleexception(ex);
        //    }
        //}

        /// <summary>
        /// Gets called when clicked on the home icon in filter page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GoToHome_Tapped(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GoToHome_Tapped method -> in FilterData.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }
    }
}