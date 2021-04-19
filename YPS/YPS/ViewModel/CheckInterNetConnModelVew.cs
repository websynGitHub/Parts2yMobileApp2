using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Service;
using YPS.Views;
using System.Linq;
using YPS.Helpers;
using YPS.Parts2y.Parts2y_Views;

namespace YPS.ViewModel
{
    public class CheckInterNetConnModelVew : IBase
    {
        /// Declaring ICommand.
        public ICommand CheckConnectionBtn { get; set; }

        /// <summary>
        /// Parameter less constructor
        /// </summary>
        public CheckInterNetConnModelVew()
        {
            try
            {
                CheckConnectionBtn = new Command(async () => await CheckConnection());
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "CheckInterNetConnModelVew constructor -> in CheckInterNetConnModelVew.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// This method is for checking if device has internet connection 
        /// </summary>
        /// <returns></returns>
        private async Task CheckConnection()
        {
            YPSLogger.TrackEvent("CheckInterNetConnModelVew", "in CheckConnection method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                IndicatorVisibility = true;
                var conn = await App.CheckInterNetConnection();

                if (conn)
                {
                    //RememberPwdDB Db = new RememberPwdDB();
                    //var user = Db.GetUserDetails();

                    //if (user.Count == 1)
                    //{
                    //    var userData = user.FirstOrDefault();
                    //    Settings.userLoginID = userData.UserId;
                    //    Settings.userRoleID = userData.UserRollID;
                    //    Settings.UserMail = userData.Email;
                    //    Settings.Username = userData.UserName;
                    //    Settings.SGivenName = userData.GivenName;
                    //    Settings.EntityName = userData.EntityName;
                    //    Settings.RoleName = userData.RoleName;
                    //    Settings.BlobStorageConnectionString = userData.BlobConnection;
                    //    Settings.PhotoSize = userData.PhotoSize;
                    //    Settings.CompressionQuality = userData.CompressionQuality;
                    //    App.Current.MainPage = new MenuPage(typeof(HomePage));
                    //}
                    //else
                    //{
                    //    await Application.Current.MainPage.Navigation.PushAsync(new YPS.Views.LoginPage(), true);
                    //}
                    await App.GetSSLKeysAndUserDetails();
                    //App.Current.MainPage = new MenuPage(typeof(HomePage));
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "CheckConnection method-> in CheckInterNetConnModelVew " + Settings.userLoginID);
            }
            IndicatorVisibility = false;
        }

        #region Properties
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
