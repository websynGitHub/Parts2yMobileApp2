using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoachMAndGuidedTourP : ContentPage
    {
        CoachMGuidedTViewModel CoachVm;
        public CoachMAndGuidedTourP()
        {
            InitializeComponent();
            BindingContext = CoachVm = new CoachMGuidedTViewModel();
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                ((ListView)sender).SelectedItem = null;

                var items = (GuidedTourData)e.SelectedItem;

                if (items.ImageTitleName == "Grid Features")
                {
                    CoachVm.MainStackGuidedTour = false;
                    CoachVm.GuidedTourGridView = true;
                    CoachVm.MyImageSource = CoachVm.GuidedTImage1[0];
                }
                else if (items.ImageTitleName == "Grid Columns Show & Hide")
                {
                    CoachVm.MainStackGuidedTour = false;
                    CoachVm.GuidedTourGridView = true;
                    CoachVm.ButtonTextCheckingGT = "Close";
                    CoachVm.MyImageSource = CoachVm.listOfImages[2];
                }
                else if (items.ImageTitleName == "Upload Photo")
                {
                    CoachVm.MainStackGuidedTour = false;
                    CoachVm.GuidedTourGridView = true;
                    CoachVm.ButtonTextCheckingGT = "Close";
                    CoachVm.MyImageSource = CoachVm.listOfImages[3];
                }
                else if(items.ImageTitleName == "Upload File")
                {
                    CoachVm.MainStackGuidedTour = false;
                    CoachVm.GuidedTourGridView = true;
                    CoachVm.ButtonTextCheckingGT = "Close";
                    CoachVm.MyImageSource = CoachVm.listOfImages[4];
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}