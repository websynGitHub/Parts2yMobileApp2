using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public LoginPage ()
		{
            try
            {
                InitializeComponent();
                YPSLogger.TrackEvent("LoginPage", "Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Settings.currentPage = "LoginPage";
                BindingContext = new LoginPageViewModel();
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                YPSLogger.ReportException(ex, "LoginPage Constructor -> in LoginPage.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
		}
    }
}