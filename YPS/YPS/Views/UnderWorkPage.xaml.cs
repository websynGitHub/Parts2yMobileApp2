using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderWorkPage : ContentPage
    {
        public ImageSource imgurl { get; set; }
        YPSService service;

        public UnderWorkPage(string Msg1, string Msg2, string bgimag, bool status)
        {
            try
            {
                YPSLogger.TrackEvent("UnderWorkPage.xaml.cs", " in UnderWorkPage constructor" + DateTime.Now + " UserId: " + Settings.userLoginID);
                service = new YPSService();

                InitializeComponent();
                if (status == true)
                {
                    pagesetup.Source = bgimag;
                    mesg1.Text = Msg1;
                    mesg2.Text = Msg2;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "UnderWorkPage constructor -> in UnderWorkPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
    }
}