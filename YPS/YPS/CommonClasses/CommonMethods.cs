using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using YPS.Helpers;
using YPS.CommonClasses;
using YPS.Service;
using YPS.Parts2y.Parts2y_Views;
using YPS.Model;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using YPS.CustomToastMsg;
using System.Linq;

namespace YPS.CommonClasses
{
    public static class CommonMethods
    {
        static YPSService service = new YPSService();
        static SendPodata sendPodata = new SendPodata();
        static int pagecount;

        public static async void BackClickFromPhotoUpload(INavigation Navigation)
        {
            try
            {
                if (Settings.TaskID > 0)
                {
                    if (Navigation?.NavigationStack[2]?.GetType()?.Name.Trim().ToLower() != "LinkPage".Trim().ToLower())
                    {
                        if (Navigation.NavigationStack.Count == 4)
                        {
                            Navigation.RemovePage(Navigation.NavigationStack[1]);
                        }
                        Navigation.RemovePage(Navigation.NavigationStack[1]);

                        Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);

                        Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);
                    }

                    Settings.POID = 0;
                    Settings.TaskID = 0;
                }

                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BackClickFromPhotoUpload method -> in CommonMethods.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        public static async void BackClickFromInspToParts(INavigation Navigation)
        {
            try
            {
                pagecount = Navigation.NavigationStack.Count() - 1;

                if (Navigation.NavigationStack[2].GetType().Name.Trim().ToLower() == "POChildListPage".Trim().ToLower())
                {
                    while (pagecount > 3)
                    {
                        pagecount--;
                        Navigation.RemovePage(Navigation.NavigationStack[pagecount]);
                    }
                }
                else
                {
                    Navigation.InsertPageBefore(new POChildListPage(await GetUpdatedAllPOData(), sendPodata), Navigation.NavigationStack[1]);
                    Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);

                    pagecount = Navigation.NavigationStack.Count() - 1;

                    while (pagecount > 3)
                    {
                        pagecount--;
                        Navigation.RemovePage(Navigation.NavigationStack[pagecount]);
                    }

                }

                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BackClickFromInsp method -> in CommonMethods.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }


        public static async void BackClickFromInspToJobs(INavigation Navigation)
        {
            try
            {
                pagecount = Navigation.NavigationStack.Count() - 1;

                if (Navigation.NavigationStack[1].GetType().Name.Trim().ToLower() == "ParentListPage".Trim().ToLower())
                {
                    while (pagecount > 2)
                    {
                        pagecount--;
                        Navigation.RemovePage(Navigation.NavigationStack[pagecount]);
                    }
                }
                else
                {
                    Navigation.InsertPageBefore(new ParentListPage(), Navigation.NavigationStack[1]);

                    pagecount = Navigation.NavigationStack.Count() - 1;

                    while (pagecount > 2)
                    {
                        pagecount--;
                        Navigation.RemovePage(Navigation.NavigationStack[pagecount]);
                    }

                }
                await Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BackClickFromInspToJobs method -> in CommonMethods.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        private static async Task<ObservableCollection<AllPoData>> GetUpdatedAllPOData()
        {
            ObservableCollection<AllPoData> AllPoDataList = new ObservableCollection<AllPoData>();

            try
            {
                YPSLogger.TrackEvent("CommonMethods.xaml.cs", " in GetUpdatedAllPOData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {

                    sendPodata = new SendPodata();
                    sendPodata.UserID = Settings.userLoginID;
                    sendPodata.PageSize = Settings.pageSizeYPS;
                    sendPodata.StartPage = Settings.startPageYPS;

                    var result = await service.LoadPoDataService(sendPodata);

                    if (result != null && result.data != null)
                    {
                        if (result.status == 1 && result.data.allPoDataMobile != null && result.data.allPoDataMobile.Count > 0)
                        {
                            AllPoDataList = new ObservableCollection<AllPoData>(result.data.allPoDataMobile.Where(wr => wr.TaskID == Settings.TaskID));
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetUpdatedAllPOData method -> in CommonMethods.cs  " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
            }
            return AllPoDataList;
        }
    }
}
