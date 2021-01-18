using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Model;
using YPS.Service;
using YPS.Views;

namespace YPS.ViewModel
{
    public class UpdateProfileModelView : IBase
    {
        //public ICommand ICommandCloseUploadProfile { set; get; }
        public ICommand ICommandUpdate { get; set; }
        YPSService service;


        public UpdateProfileModelView()
        {
            try
            {
                //ICommandCloseUploadProfile = new Command(async () => await CloseUploadProfile());
                ICommandUpdate = new Command(async () => await UpdateProfile());
                service = new YPSService();
                getProfileData();
                getData();
            }
            catch (Exception ex)
            {
               
            }
        }

        public async void getProfileData()
        {
            try
            {
                IndicatorVisibility = true;
                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    int loginID = Settings.userLoginID;
                    var getProfileData = await service.GetProfile(loginID);

                    if (getProfileData.status != 0)
                    {
                        Email = getProfileData.data.Email;
                        GivenName = getProfileData.data.GivenName;
                        FamilyName = getProfileData.data.FamilyName;
                        TimeZoneTextDisplay = getProfileData.data.UserCulture;
                    }
                    IndicatorVisibility = false;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                }
            }
            catch(Exception ex)
            {
                var trackResult = await service.Handleexception(ex);
            }
        }
        public async void getData()
        {
            try
            {
                IndicatorVisibility = true;
                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    var stimeZone = await service.GetTimeZone();
                    ObservableCollection<string> addTimeZoneName = new ObservableCollection<string>();

                    foreach (var items in stimeZone.data)
                    {
                        addTimeZoneName.Add(items.DisplayText);
                    }
                    TimeZone = addTimeZoneName;
                    IndicatorVisibility = false;
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                }
            }
            catch (Exception ex)
            {

            }
        }

        //private async Task CloseUploadProfile()
        //{
        //    //App.Current.MainPage = new RootPage(typeof(MainPage));
        //    //Settings.refreshPage = 0;
            
        //    await App.Current.MainPage.Navigation.PopModalAsync();
        //}

        private async Task UpdateProfile()
        {
            try
            {
                IndicatorVisibility = true;
                var checkInternet = await App.CheckInterNetConnection();
                if (checkInternet)
                {
                    UpdateProfile uProfile = new UpdateProfile();

                    uProfile.UserID = Settings.userLoginID;
                    uProfile.CreatedBy = Settings.userLoginID;
                    uProfile.GivenName = GivenName;
                    uProfile.FamilyName = FamilyName;
                    uProfile.UserCulture = TimeZoneTextDisplay;

                    var response = await service.UpdateProfile(uProfile);

                    var finalResponse = JsonConvert.DeserializeObject<updateProfileResponse>(response);

                    IndicatorVisibility = false;
                    Settings.refreshPage = 1;
                    if (finalResponse.status != 0)
                    {
                        //DependencyService.Get<IToastMessage>().ShortAlert("Success");
                        await App.Current.MainPage.DisplayAlert("Alert", "Success", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", "Something went wrong, Please try again.", "OK");
                        //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, Please try again");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection");
                }
            }catch(Exception ex)
            {
                var trackResult = await service.Handleexception(ex);
            }
        }

        #region all properties

        private string _Email;
        public string Email
        {
            get => _Email;
            set
            {
                _Email = value;
                NotifyPropertyChanged();
            }
        }

        private string _GivenName;
        public string GivenName
        {
            get => _GivenName;
            set
            {
                _GivenName = value;
                NotifyPropertyChanged();
            }
        }

        private string _FamilyName;
        public string FamilyName
        {
            get => _FamilyName;
            set
            {
                _FamilyName = value;
                NotifyPropertyChanged();
            }
        }

        private string _TimeZoneTextDisplay = "Please Select";
        public string TimeZoneTextDisplay
        {
            get => _TimeZoneTextDisplay;
            set
            {
                _TimeZoneTextDisplay = value;
                NotifyPropertyChanged();
            }
        }


        private ObservableCollection<string> _TimeZone;
        public ObservableCollection<string> TimeZone
        {
            get { return _TimeZone; }
            set
            {
                _TimeZone = value;
                NotifyPropertyChanged();
            }
        }

        private UpdateProfileModel _UpdateProfileModel;
        public UpdateProfileModel UpdateProfileModel
        {
            get => _UpdateProfileModel;
            set
            {
                _UpdateProfileModel = value;
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
        #endregion
    }
}
