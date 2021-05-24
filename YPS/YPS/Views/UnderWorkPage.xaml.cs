using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UnderWorkPage : ContentPage
    {
        public ImageSource imgurl { get; set; }
        public UnderWorkPage(string Msg1, string Msg2, string bgimag, bool status)
        {
            try
            {
                InitializeComponent();
                if (status == true)
                {
                    pagesetup.Source = bgimag;
                    // imgurl = bgimag;
                    mesg1.Text = Msg1;
                    mesg2.Text = Msg2;
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}