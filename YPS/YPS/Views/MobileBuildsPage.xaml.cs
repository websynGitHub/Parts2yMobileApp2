using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Helpers;
using YPS.Service;
using YPS.ViewModel;
using YPS.CommonClasses;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MobileBuildsPage : ContentPage
    {
        MobileBuildsViewModel Vm;

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public MobileBuildsPage()
        {
            try
            {
                InitializeComponent();
                BindingContext = Vm = new MobileBuildsViewModel();
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "NotificationListPage Constructor -> in NotificationListPage.cs " + Settings.userLoginID);

            }
        }

        /// <summary>
        /// Gets called when click on red cross icon to close decription dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_DescriptionPopup(object sender, EventArgs e)
        {
            try
            {
                Vm.PopUpForFileDes = false;
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "NotificationListPage Constructor -> in NotificationListPage.cs " + Settings.userLoginID);
            }
        }
    }
}