using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_SQLITE;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransportReportDetails : ContentPage
    {
        VMTransportReportDetails Vm;
        public TransportReportDetails()
        {
            InitializeComponent();
            BindingContext = Vm = new VMTransportReportDetails(Navigation);
            InPrgrs_List.ItemTapped += (s, e) => InPrgrs_List.SelectedItem = null;
        }

        protected async override void OnAppearing()
        {
            //need to confirm regarding this functionality
            try
            {
                Vm.loadindicator = true;

                if (Settings.isScanCompleted || Settings.isPDICompleted || Settings.isLoadCompleted)
                {
                    // need to confirm
                    SupTransportDB veDetailsDB = new SupTransportDB("loaddata");

                    if (Settings.isScanCompleted)
                    {
                        Vm.loadData[0].Loaddata.Where(wr => wr.Vin == Settings.VINNo).FirstOrDefault().IsCompleted = 2;
                    }
                    else if (Settings.isPDICompleted || Settings.isLoadCompleted)
                    {
                        Vm.loadData[0].Loaddata.Where(wr => wr.Vin == Settings.VINNo).FirstOrDefault().IsCompleted = 1;
                    }

                    await veDetailsDB.UpdateDisplayLoadDataList(Vm.loadData);
                    Vm.GetLoadnumberdata();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Settings.isScanCompleted = false;
                Settings.isPDICompleted = false;
                Settings.isLoadCompleted = false;
                Vm.loadindicator = false;
            }
        }
    }
}