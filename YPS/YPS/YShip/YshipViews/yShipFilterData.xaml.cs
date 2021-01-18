using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Helpers;
using YPS.Service;
using YPS.CommonClasses;
using YPS.Model;
using YPS.CustomToastMsg;
using System.Collections.ObjectModel;
using YPS.YShip.YshipViewModel;

namespace YPS.YShip.YshipViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class yShipFilterData : ContentPage
    {
        yShipFilterDataViewModel vm;
        YPSService yPSService;
        bool checkInternet;

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public yShipFilterData()
        {
            try
            {
                InitializeComponent();
                Settings.isyShipRefreshPage = true;
                Settings.currentPage = "yShipFilterDataPage";
                BindingContext = vm = new yShipFilterDataViewModel();
                yPSService = new YPSService();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "yShipFilterData Constructor -> in FilterData.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when cross icon present near to location search field  is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseLocationSearch(object sender, EventArgs e)
        {
            try
            {
                LocationSearchName.Text = "";
                vm.SearchContentView = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClosePop_Tapped method -> in FilterData.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when text is change in location search textbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchPlaceEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //Verifying internet connection.
                checkInternet = await App.CheckInterNetConnection();

                int min = 4;

                if (checkInternet)
                {

                    string textSize = LocationSearchName.Text;

                    if (textSize.Length >= min)
                    {
                        vm.IndicatorVisibility = true;
                        await System.Threading.Tasks.Task.Delay(500);

                        //Getting the location list with the searched text from API
                        var result = await yPSService.SearchLocation(textSize);

                        if (result != null && result.status != 0)
                        {
                            //Binding the list to the deopdown
                            List<SearchData> addItems = new List<SearchData>();
                            addItems.Add(new SearchData() { Name = "ALL", ID = 0 });
                            addItems.AddRange(result.data);
                            vm.SearchLocationResults = new ObservableCollection<SearchData>(addItems);
                            vm.NoResultsIsVisibleLbl = false;
                            vm.isLocationListViewVisible = true;
                        }
                        else
                        {
                            vm.NoResultsIsVisibleLbl = true;
                            vm.isLocationListViewVisible = false;
                            vm.NoResultsLbl = "No results found";
                        }
                        vm.IndicatorVisibility = false;
                    }
                    else
                    {

                        vm.NoResultsLbl = "Please enter " + Convert.ToString(min - textSize.Length) + " or more characters";

                        if (vm.SearchLocationResults != null)
                        {
                            vm.SearchLocationResults.Clear();
                        }

                        vm.NoResultsIsVisibleLbl = true;
                        vm.isLocationListViewVisible = false;
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SearchPlaceEntry_TextChanged method -> in FilterData.cs " + Settings.userLoginID);
                await yPSService.Handleexception(ex);
            }
            finally
            {
                vm.IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when item is selected from the search result for location any field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                SearchData val = e.Item as SearchData;
                LocationListViewName.SelectedItem = null;
                vm.SearchContentView = false;
                LocationSearchName.Text = "";

                if (vm.EntrySearchType.Trim().ToLower() == "orisearch")
                {
                    Settings.oriSearchNameyShip = vm.oriSearch = val.Name;
                    Settings.oriSearchIdyShip = val.ID;
                }
                else if (vm.EntrySearchType.Trim().ToLower() == "destsearch")
                {
                    Settings.destSearchNameyShip = vm.destSearch = val.Name;
                    Settings.destSearchIdyShip = val.ID;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SearchItemTapped method -> in FilterData.cs " + Settings.userLoginID);
                yPSService.Handleexception(ex);
            }
        }

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