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
    public partial class CheckInterNetConn : ContentPage
    {
        CheckInterNetConnModelVew vm;
        public CheckInterNetConn()
        {
            InitializeComponent();
            YPSLogger.TrackEvent("CheckInterNetConn", "Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                BindingContext = vm = new CheckInterNetConnModelVew();

            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatUsers constructor with 2 params -> in ChatUsers Page.cs " + Settings.userLoginID);
            }
        }
    }
}