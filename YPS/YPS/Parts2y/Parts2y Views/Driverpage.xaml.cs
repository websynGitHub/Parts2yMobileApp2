using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Driverpage : ContentPage
    {
        DriverPageViewModel Vm;
        public Driverpage()
        {
            try
            {
                InitializeComponent();
                BindingContext = Vm = new DriverPageViewModel(Navigation);
                //InPrgrs_List.ItemSelected += InPrgrs_List_ItemSelected;
                InPrgrs_List.ItemTapped += (s, e) => InPrgrs_List.SelectedItem = null;
            }
            catch (Exception ex)
            {
            }
        }

        //private async void InPrgrs_List_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        //{
        //    try
        //    {

        //    InPrgrs_List.SelectedItem = null;
        //    Vinlist d = (Vinlist)e.SelectedItem;
        //    if (d.Vin != null)
        //    {
        //            var requestedPermissions = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
        //            var requestedPermissionStatus = requestedPermissions[Permission.Location];
        //            var pass1 = requestedPermissions[Permission.Location];
        //            if (pass1 == PermissionStatus.Denied)
        //            {
        //                var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the location .", null, null, "Maybe Later", "Settings");
        //                switch (checkSelect)
        //                {
        //                    case "Maybe Later":
        //                        break;
        //                    case "Settings":
        //                        CrossPermissions.Current.OpenAppSettings();
        //                        break;
        //                }
        //            }
        //            else if (pass1 == PermissionStatus.Granted)
        //            {

        //                Settings.HRLtext = d.Handover_Retrieval_Loading;
        //                Settings.VINNo = d.Vin;
        //                Settings.IscompltedRecord = d.IsCompleted;

        //                if (Navigation.ModalStack.Count == 0 ||
        //                              Navigation.ModalStack.Last().GetType() != typeof(DriverVehicleDetails))
        //                {
        //                    await Navigation.PushModalAsync(new DriverVehicleDetails());
        //                }
        //            }
        //            else
        //            {
        //                await App.Current.MainPage.DisplayAlert("Oops", "Unable to load location!", "OK");
        //                await App.Current.MainPage.DisplayAlert("Alert", "You don't have permission to access location, Please allow the location permission in App Permission settings", "Ok");
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
    }
}