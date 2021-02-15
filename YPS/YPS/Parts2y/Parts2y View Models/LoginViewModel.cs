
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_Services;
using YPS.Parts2y.Parts2y_SQLITE;
using YPS.Parts2y.Parts2y_View_Models;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class LoginViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        public ICommand LoginCommand { get; set; }

        RestClient service = new RestClient();

        public LoginViewModel(INavigation _Navigation)
        {
            LoginCommand = new Command(async () => await Login_Tap());
            Navigation = _Navigation;
        }
        public async Task Login_Tap()
        {
            try
            {
                IndicatorVisibility = true;
                await Task.Delay(1);
                var loginData = await service.GetLoginDetails(userName);
                var result = JsonConvert.DeserializeObject<LoginModel>(loginData.ToString());

                if (result != null)
                {
                    if (result.status != false)
                    {
                        Settings.UserName = result.UserDetails.username;
                        Settings.UserMail = result.UserDetails.user_email;
                        Settings.Entity_Name = result.UserDetails.EntityName;
                        Settings.roleid = result.UserDetails.roleid;
                        Settings.ID = result.UserDetails.ID;

                        if (!string.IsNullOrEmpty(result.UserDetails.Colorcode))
                        {
                            Settings.Bar_Background = Color.FromHex(result.UserDetails.Colorcode);
                        }

                        #region added by sindhu for remember me
                        await RememberUserDetails();
                        #endregion
                        // await Navigation.PushModalAsync(new HomePage());

                        //if (Settings.roleid == 1)
                        //{
                            App.Current.MainPage = new MenuPage(typeof(HomePage));
                            //IndicatorVisibility = false;

                        //}
                        //else if (Settings.roleid == 2)
                        //{
                        //    App.Current.MainPage = new MenuPage(typeof(HomePage));
                        //    IndicatorVisibility = false;

                        //}
                        //else if (Settings.roleid == 3)
                        //{
                        //    App.Current.MainPage = new MenuPage(typeof(DealerPage));
                        //    IndicatorVisibility = false;

                        //}
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Login", "Please enter valid Login ID", "ok");
                        IndicatorVisibility = false;

                    }
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
        #region added by sindhu for remember me

        public async Task RememberUserDetails()
        {
            try
            {
                IndicatorVisibility = true;

                RememberPwdDB Db = new RememberPwdDB();
                var user = Db.GetUserDetails();

                if (user.Count == 0)
                {
                    Parts2y.Parts2y_Models.UserDetails saveData = new Parts2y.Parts2y_Models.UserDetails();
                    saveData.user_email = Settings.UserMail;
                    saveData.username = Settings.UserName;
                    saveData.roleid = Settings.roleid;
                    saveData.Colorcode = Settings.Bar_Background.ToHex();
                    saveData.EntityName = Settings.Entity_Name;
                    saveData.ID = Settings.ID;
                    Db.SaveUserPWd(saveData);
                }
            }
            catch (Exception ex)
            {
            }
            //finally
            //{
            //    IndicatorVisibility = false;
            //}
        }
        #endregion

        #region Properties
        public string _userName;
        public string userName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged("userName");
            }
        }

        bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                //NotifyPropertyChanged();
                NotifyPropertyChanged("IndicatorVisibility");
            }
        }

        #endregion
    }
}
