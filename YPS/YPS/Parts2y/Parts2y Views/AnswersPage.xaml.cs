using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Model;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnswersPage : ContentPage
    {
        AnswersPageViewModel Vm;
        public AnswersPage(InspectionConfiguration inspectionConfiguration, int tagId, ObservableCollection<InspectionConfiguration> inspectionConfigurationList, List<InspectionResultsList> inspectionResultsLists, string tagNumber, string indentCode, string bagNumber, QuestiionsPageHeaderData questiionsPageHeaderData)
        {
            InitializeComponent();
            BindingContext = Vm = new AnswersPageViewModel(Navigation, this, inspectionConfiguration, tagId, inspectionConfigurationList, inspectionResultsLists, tagNumber, indentCode, bagNumber, questiionsPageHeaderData);

            if (Device.RuntimePlatform == Device.iOS)
            {
                var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                safeAreaInset.Bottom = 0;
                safeAreaInset.Top = 30;
                headerpart.Padding = safeAreaInset;
            }
        }
    }
}