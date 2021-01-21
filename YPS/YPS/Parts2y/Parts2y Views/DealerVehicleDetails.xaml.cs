using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    public partial class DealerVehicleDetails : ContentPage
    {
        DealerVehicleDetailsViewModel Vm;
        public DealerVehicleDetails()
        {
            InitializeComponent();
            BindingContext = Vm = new DealerVehicleDetailsViewModel(Navigation);
            radioOptionList.ItemTapped += (s, e) => radioOptionList.SelectedItem = null;
        }
        public void RightSwipe_Tapped(object sender, EventArgs e)
        {
            try
            {
                int val = Settings.QNo_Sequence;
                ObservableCollection<ListOption> list = new ObservableCollection<ListOption>();

                foreach (var item in Vm.cPQuestionsdata[val].listOptions)
                {
                    if (item == Vm.cPQuestionsdata[val].listOptions.First())
                    {
                       
                    }
                    else
                    {
                       
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void CheckBox_CheckChanged(object sender, EventArgs e)
        {
            try
            {
                var box = (Plugin.InputKit.Shared.Controls.CheckBox)sender;
                int val = Settings.QNo_Sequence - 1;
                int seqId = Convert.ToInt32(box.ClassId);
                ListOption optList = Vm.QuestionInfo[val].listOptions.Where(x => x.ObsID == seqId).FirstOrDefault();

                if (box.IsChecked)
                {
                    optList.Isanswered = true;
                    optList.unSelected = false;
                }
                else
                {
                    optList.Isanswered = false;
                    optList.unSelected = true;
                }
                var checkBox = Vm.QuestionInfo[val].listOptions.Where(x => x.Isanswered == true).ToList();
                if (checkBox != null)
                {

                }

            }
            catch (Exception ex)
            {

            }
        }

        private async void ConfirmSignatureTapped(object sender, EventArgs e)
        {

            try
            {
                byte[] result;
                var strokes = PadView.Strokes;
                Stream image = await PadView.GetImageStreamAsync(SignatureImageFormat.Png);
                if (image != null)
                {

                    //var val = DependencyService.Get<ISaveImage>().OnSaveSignature(image,"Signature_Downloads");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.CopyTo(ms);
                        result = ms.ToArray();
                        string base64 = Convert.ToBase64String(result);
                        if (result != null)
                        {
                            //if (Vm.Signature_Person == "Auditor's Signature")
                            //{
                            //    byte[] Base64Stream = Convert.FromBase64String(base64);
                            //    Vm.AuditorImageSign = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                            //    Vm.SignaturePadPopup = false;
                            //    PadView.Clear();
                            //    //Vm.AuditorImageSign = "https://i.ytimg.com/vi/PCwL3-hkKrg/maxresdefault.jpg";
                            //}
                            //else
                            //{
                            if (!string.IsNullOrEmpty(otpEntry.Text))
                            {
                                    byte[] Base64Stream = Convert.FromBase64String(base64);
                                    Vm.result[0].signatureDealerBase64 = Base64Stream;
                                    Vm.DealerImageSign = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                                Vm.result[0].Dealerdata.Inspection = DateTime.Now.ToString("h:mm");
                                Vm.result[0].Dealerdata.Pod = DateTime.Now.ToString("h:mm");
                                if (Vm.DealerImageSign != null)
                                    {
                                        Vm.DoneLoadColor = Settings.Bar_Background;
                                        Vm.DoneBtnEnable = true;
                                    }
                                    Vm.SignaturePadPopup = false;
                                    PadView.Clear();
                                }
                            else
                            {
                                PadView.Clear();
                                App.Current.MainPage.DisplayAlert("Alert", "Please Enter Dealer's OTP", "Ok");
                            }
                            //}

                        }
                        else
                        {

                        }
                    }
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Alert", "Please sign before saving..", "Ok");

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ListView_SwipeStarted(object sender, Syncfusion.ListView.XForms.SwipeStartedEventArgs e)
        {
            if (e.ItemIndex != null)
            {
                int index = e.ItemIndex;
                var items = Vm.QuestionInfo[index].listOptions.Where(x => x.Isanswered).ToList();
                if (items.Count != 0)
                {
                    e.Cancel = true;

                }
                else
                {
                    e.Cancel = false;
                }

            }
        }
        protected override void OnAppearing()
        {
            try
            {
                Vm.loadindicator = true;
                EPODPhotoUplodeViewModel vmEPODPhoto = new EPODPhotoUplodeViewModel();
                Vm.photoCount = Convert.ToString(vmEPODPhoto.photolist.Count);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Vm.loadindicator = false;
            }
        }
    }
}