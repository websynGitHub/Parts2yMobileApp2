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
            YPSLogger.TrackEvent("CheckInterNetConnModelVew", " in CheckConnection method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                IndicatorVisibility = true;
                var conn = await App.CheckInterNetConnection();

                if (conn)
                {
                    await App.GetSSLKeysAndUserDetails();
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
