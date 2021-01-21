using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            navbar.BackgroundColor = Settings.Bar_Background;
            versionno.Text = Settings.VersionName;
        }

        private void BackIcon_Tapped(object sender, EventArgs e)
        {
            try
            {
                App.Current.MainPage.Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {

            }
        }

        private void Link_Tapped(object sender, EventArgs e)
        {
            try
            {
                string url = "http://www.yamato-asia.com/en/";
                Device.OpenUri(new Uri(url));
            }
            catch (Exception ex)
            {

            }
        }
    }
}