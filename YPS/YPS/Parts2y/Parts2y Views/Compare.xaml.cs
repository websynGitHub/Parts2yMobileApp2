using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.Parts2y.Parts2y_Common_Classes;
using YPS.Parts2y.Parts2y_Models;
using YPS.Parts2y.Parts2y_View_Models;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Compare : ContentPage
    {
        CompareViewModel Vm;
        public Compare()
        {
            try
            {
                InitializeComponent();
                BindingContext = Vm = new CompareViewModel(Navigation, this);
                //int count = 1;
                //Vm.compareHistoryList = new List<CompareHistoryList>()
                //{
                //new CompareHistoryList() { HistorySerialNo=count++, AValue ="scan", BValue= "scan",IsMatchedImg="match.png" } ,
                //    new CompareHistoryList() {HistorySerialNo=count++, AValue ="scan", BValue= "c",IsMatchedImg="unmatch.png" },
                //    new CompareHistoryList() { HistorySerialNo=count++,AValue ="c", BValue= "scan",IsMatchedImg="unmatch.png" },
                //    new CompareHistoryList() {HistorySerialNo=count++,AValue ="scan", BValue= "scan" ,IsMatchedImg="match.png"},
                //    new CompareHistoryList() { HistorySerialNo=count++,AValue ="scan", BValue= "scan",IsMatchedImg="match.png" }
                //}
                //;
                //Vm.compareHistoryList.Add(new CompareHistoryList() { AValue = "scan", BValue = "c", IsMatchedImg = "unmatch.png" });
                //Vm.compareHistoryList.Add(new CompareHistoryList() { AValue = "scan", BValue = "c", IsMatchedImg = "unmatch.png" });

                //Vm.compareHistoryList.Add(new CompareHistoryList() { AValue = "scan", BValue = "c", IsMatchedImg = "unmatch.png" });

                //Vm.compareHistoryList.Add(new CompareHistoryList() { AValue = "scan", BValue = "c", IsMatchedImg = "unmatch.png" });

                //Vm.latestCompareHistoryList = new List<CompareHistoryList>() {
                //  new CompareHistoryList() { HistorySerialNo=1, AValue ="scan", BValue= "scan",IsMatchedImg="ookblack.png" } ,
                //    new CompareHistoryList() { HistorySerialNo=12, AValue ="scan", BValue= "lajckjadcbadchbdcbdhkcbwjcbwjcbhkbchbckqbcbcabcabcabcbacbacc",IsMatchedImg="ng.png" },
                //    new CompareHistoryList() { HistorySerialNo=3199, AValue ="amcnajnckdncwncwjncjncwkncwkncnckncncwncwncjnc", BValue= "scan",IsMatchedImg="ng.png" },
                //    new CompareHistoryList() { HistorySerialNo=4, AValue ="scan", BValue= "scan" ,IsMatchedImg="ookblack.png"},
                //    new CompareHistoryList() { HistorySerialNo=5, AValue ="scan", BValue= "scan",IsMatchedImg="ookblack.png" },

                //};
            }
            catch (Exception ex)
            {

            }
        }

        private void ClearHistory(object sender, EventArgs e)
        {
            try
            {
                BindingContext = Vm = new CompareViewModel(Navigation, this);
                //Vm.historySerialNo = 1;
                //Vm.compareList.Clear();
                //Vm.compareHistoryList.Clear();
                //Vm.latestCompareHistoryList.Clear();
                //Vm.showLatestViewFrame = false;
                //Vm.showCurrentStatus = false;
                //Vm.ScannedValueA = "";
                //Vm.ScannedValueB = "";
                //Vm.isMatch = "UNMATCHED";
                //Vm.isMatchImage = "unmatch.png";
                //Vm.isScannedA = "cross.png";
                //Vm.isScannedB = "cross.png";
                //Vm.resultA = "";
                //Vm.resultB = "";
                //Vm.isShowMatch = false;
                //Vm.opacityB = 0.50;
                //Vm.opacityA = 1;
                //Vm.isEnableAFrame = true;
                //Vm.isEnableBFrame = false;
                //Vm.showScanHistory = false;
                //Vm.showLatestViewFrame = false;
                //Vm.showCurrentStatus = false;

            }
            catch (Exception ex)
            {

            }
        }

        private void ViewHistory(object sender, EventArgs e)
        {
            try
            {
                //Vm.compareHistoryList = new List<CompareHistoryList>() {
                //new CompareHistoryList() { HistorySerialNo=1, AValue ="scan", BValue= "scan",IsMatchedImg="match.png" } ,
                //    new CompareHistoryList() { HistorySerialNo=12, AValue ="scan", BValue= "lajckjadcbadchbdcbdhkcbwjcbwjcbhkbchbckqbcbcabcabcabcbacbacc",IsMatchedImg="unmatch.png" },
                //    new CompareHistoryList() { HistorySerialNo=3199, AValue ="amcnajnckdncwncwjncjncwkncwkncnckncncwncwncjnc", BValue= "scan",IsMatchedImg="unmatch.png" },
                //    new CompareHistoryList() { HistorySerialNo=4, AValue ="scan", BValue= "scan" ,IsMatchedImg="match.png"},
                //    new CompareHistoryList() { HistorySerialNo=5, AValue ="scan", BValue= "scan",IsMatchedImg="match.png" },

                //};
                Vm.showScanHistory = true;
            }
            catch (Exception ex)
            {

            }
        }

        private void HideHistory(object sender, EventArgs e)
        {
            try
            {
                Vm.showScanHistory = false;
            }
            catch (Exception ex)
            {

            }
        }

        private void ShowPicker(object sender, EventArgs e)
        {
            try
            {
                scanrulepicker.Focus();
            }
            catch (Exception ex)
            {

            }
        }

        private void TotalEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Vm.TotalCount = !string.IsNullOrEmpty(e.NewTextValue) ? (int?)Convert.ToInt32(e.NewTextValue) : null;

                //if(Vm.TotalCount == null)
                //{
                //    Vm.IsTotalValidMsg = true;
                //    Vm.TotalErrorTxt = "Enter total value";
                //}
                //else
                //{
                //    Vm.IsTotalValidMsg = false;
                //    Vm.TotalErrorTxt = "";
                //}
            }
            catch (Exception ex)
            {

            }
        }

        private async void SaveClick(object sender, EventArgs e)
        {
            try
            {
                int? total = Vm.TotalCount;
                string selectedrule = Vm.SelectedScanRule;
                BindingContext = Vm = new CompareViewModel(Navigation, this);
                Vm.TotalCount = total;
                Vm.SelectedScanRule = selectedrule;
                await Vm.SaveConfig();
            }
            catch (Exception ex)
            {

            }
        }
    }
}