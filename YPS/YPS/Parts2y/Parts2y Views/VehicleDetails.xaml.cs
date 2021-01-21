using Plugin.InputKit.Shared.Controls;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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
using static YPS.Parts2y.Parts2y_Models.PhotoModel;

namespace YPS.Parts2y.Parts2y_Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VehicleDetails : ContentPage
    {
        VehicleDetailViewModel Vm;
        public int i = 0;
        public VehicleDetails()
        {
            try
            {
                InitializeComponent();
                BindingContext = Vm = new VehicleDetailViewModel(Navigation);
                radioOptionList.ItemTapped += (s, e) => radioOptionList.SelectedItem = null;
            }
            catch (Exception ex)

            {

            }
        }

        private void RadioButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                int val = Settings.QNo_Sequence;
                var grid = (Plugin.InputKit.Shared.Controls.RadioButton)sender;
                int seqId = Convert.ToInt32(grid.ClassId);
                if (grid.IsChecked)
                {
                    grid.IsChecked = true;
                    Vm.Radio_Selected = false;
                }
                else
                {
                    grid.IsChecked = false;
                    Vm.Radio_Selected = true;
                }
            }
            catch (Exception ex)
            {

            }
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
                        //list.Add(new ListOption { Radio_Selected = true });
                        // item.Radio_Selected = true;
                        // Vm.Radio_Selected = true;
                    }
                    else
                    {
                        //item.Radio_Selected = false;
                        //list.Add(new ListOption { Radio_Selected = false }) ;
                        //Vm.Radio_Selected = false;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async void Select_Pic(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;

                if (App.Current.MainPage.Navigation.ModalStack.Count == 0 ||
                                       App.Current.MainPage.Navigation.ModalStack.Last().GetType() != typeof(PhotoUploadePOD))
                {
                    await App.Current.MainPage.Navigation.PushModalAsync(new PhotoUploadePOD());
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                Vm.loadindicator = false;
            }

        }

        private void TapGestureCommand(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;

                // var tappedItemData = (Grid)sender;
                var args = (TappedEventArgs)e;
                var tappedItemData = args.Parameter;

                var data = tappedItemData as CPQuestionsdata;//Vm.ExpandVisibility = true;

                if (tappedItemData != null)
                {
                    //Vm.PDIFuncVisibility = true;
                    //ScanFuncVisibility = LoadFuncVisibility = false;
                    //expandStack.IsVisible = true;
                    //Vm.QuestionListVisibility = false;
                    Vm.QuestionListVisibility = false;
                    Vm.ExpandVisibility = true;
                    Settings.QNo_Sequence = data.Seqno;
                    Vm.NextBtnBgColor = ((!string.IsNullOrEmpty(Settings.PDICompleted)) && Settings.QNo_Sequence == Vm.QuestionInfo.Count) ? Color.LightGray : Settings.Bar_Background;
                    Vm.Next_Text = (Settings.QNo_Sequence == Vm.QuestionInfo.Count) ? "SAVE" : "NEXT";
                    Vm.Question_No = data.Seqno;
                    Vm.Qestion_Title = data.PDI_QestionTitle;

                    if (!string.IsNullOrEmpty(data.AnsweredRemarks))
                    {
                        Vm.RemarksText = data.AnsweredRemarks;
                    }
                    else
                    {
                        Vm.RemarksText = "";
                        if (!string.IsNullOrEmpty(data.Remarks))
                        {
                            Vm.PlaceholderText = data.Remarks;
                        }
                        else
                        {
                            Vm.PlaceholderText = "REMARKS";

                        }
                    }

                    EPODPhotoUplodeViewModel vmEPODPhoto = new EPODPhotoUplodeViewModel();
                    Vm.photoCount = Convert.ToString(vmEPODPhoto.photolist.Count);
                    Vm.OptionsList = data.listOptions;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Vm.loadindicator = false;
            }
        }

        private async void ViewAll_Tap(object sender, EventArgs e)
        {
            Vm.loadindicator = true;

            try
            {
                i = 0;
                Vm.ExpandVisibility = false;
                await GetQuestionarieDetils();
                Vm.QuestionListVisibility = true;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Vm.loadindicator = false;
            }
        }

        #region added by Rameez (need to confirm )
        private async Task GetQuestionarieDetils()
        {
            try
            {
                if (Vm.result[0].cpquestions.Count > 0)
                {
                    foreach (var items in Vm.result[0].cpquestions)
                    {
                        items.RightSwipeText = items.listOptions[0].Observation;
                        items.RightSwipeSelectedIndex = items.listOptions[0].ObsID;
                        var itemlist = items.listOptions.Where(x => x.Isanswered).ToList();

                        if (itemlist.Count != 0)
                        {
                            items.NotAnsweredBgColor = Color.FromHex("#005800"); ;
                            items.AnsweredQstnBgColor = Settings.Bar_Background;
                            items.isAnswered = true;
                            Vm.AllowSwipingVisible = false;

                        }
                        else
                        {
                            items.NotAnsweredBgColor = Color.Black; ;
                            items.AnsweredQstnBgColor = Color.White;
                            items.isAnswered = false;
                            Vm.AllowSwipingVisible = true;
                        }
                    }
                    Vm.QuestionInfo = Vm.result[0].cpquestions;
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
        private void Next_Tap(object sender, EventArgs e)
        {
            try
            {
                Vm.loadindicator = true;

                i = Settings.QNo_Sequence;
                Settings.QNo_Sequence = Settings.QNo_Sequence != Vm.QuestionInfo.Count ? Vm.QuestionInfo[i].Seqno : Settings.QNo_Sequence;
                Vm.QuestionInfo[i - 1].AnsweredRemarks = Vm.RemarksText;
                Vm.result[0].cpquestions[i - 1].isAnswered = (Vm.QuestionInfo[i - 1].listOptions.Where(wr => wr.Isanswered == true).FirstOrDefault()) != null ? true : false;//mark as answered or not

                EPODPhotoUplodeViewModel vmEPODPhoto = new EPODPhotoUplodeViewModel();
                Vm.photoCount = Convert.ToString(vmEPODPhoto.photolist.Count);

                if (Vm.Next_Text == "SAVE")
                {
                    var isAllAttended = Vm.result[0].cpquestions.Where(wr => wr.isAnswered == false).FirstOrDefault();

                    if (isAllAttended == null)
                    {
                        Vm.isallQuestionsAttended = true;
                        GetQuestionarieDetils();
                        Vm.Load_Tap();
                    }
                }
                else
                {
                    if (i < Vm.QuestionInfo.Count)
                    {
                        //i = Settings.QNo_Sequence;
                        //Settings.QNo_Sequence = Vm.QuestionInfo[i].Seqno;
                        Vm.NextBtnBgColor = ((!string.IsNullOrEmpty(Settings.PDICompleted)) && Settings.QNo_Sequence == Vm.QuestionInfo.Count) ? Color.LightGray : Settings.Bar_Background;
                        Vm.Next_Text = (Settings.QNo_Sequence == Vm.QuestionInfo.Count) ? "SAVE" : "NEXT";
                        Vm.Question_No = Vm.QuestionInfo[i].Seqno;
                        Vm.Qestion_Title = Vm.QuestionInfo[i].PDI_QestionTitle;
                        Vm.OptionsList = Vm.QuestionInfo[i].listOptions;

                        if (!string.IsNullOrEmpty(Vm.QuestionInfo[i].AnsweredRemarks))
                        {
                            Vm.RemarksText = Vm.QuestionInfo[i].AnsweredRemarks;
                        }
                        else
                        {
                            Vm.RemarksText = "";
                            if (!string.IsNullOrEmpty(Vm.QuestionInfo[i].Remarks))
                            {
                                Vm.PlaceholderText = Vm.QuestionInfo[i].Remarks;
                            }
                            else
                            {
                                Vm.PlaceholderText = "REMARKS";
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Vm.loadindicator = false;
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
                    //List<SignatureImagesModel> signature = new List<SignatureImagesModel>();

                    //var val = DependencyService.Get<ISaveImage>().OnSaveSignature(image,"Signature_Downloads");
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.CopyTo(ms);
                        result = ms.ToArray();
                        string base64 = Convert.ToBase64String(result);
                        if (result != null)
                        {
                            if (Vm.Signature_Person == "Auditor's Signature")
                            {
                                byte[] Base64Stream = Convert.FromBase64String(base64);
                                Vm.result[0].signatureAuditBase64 = Base64Stream;
                                Vm.AuditorImageSign = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                                Vm.SignaturePadPopup = false;
                                PadView.Clear();
                                //Vm.AuditorImageSign = "https://i.ytimg.com/vi/PCwL3-hkKrg/maxresdefault.jpg";
                            }
                            else
                            {
                                if (Vm.AuditorImageSign != null)
                                {
                                    byte[] Base64Stream = Convert.FromBase64String(base64);
                                    Vm.result[0].signatureSupervisorBase64 = Base64Stream;
                                    Vm.result[0].Vindata.PDICompleted = DateTime.Now.ToString("h:mm");
                                    Vm.result[0].Vindata.Load = DateTime.Now.ToString("h:mm");
                                    Vm.SupervisorImageSign = ImageSource.FromStream(() => new MemoryStream(Base64Stream));

                                    if (Vm.SupervisorImageSign != null)
                                    {
                                        Vm.OkToLoadColor = Settings.Bar_Background;
                                        Vm.OkayToGoEnable = true;
                                    }
                                    Vm.SignaturePadPopup = false;
                                    PadView.Clear();
                                }
                                else
                                {
                                    PadView.Clear();
                                    App.Current.MainPage.DisplayAlert("Alert", "Please complete Auditor's signature", "Ok");
                                }
                            }

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

        //private void QuestionNo_Tapped(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        var args = (TappedEventArgs)e;
        //        var tappedItemData1 = args.Parameter;

        //        var data = tappedItemData1 as CPQuestionsdata;
        //        if(data!=null)
        //        {
        //            //Vm.QuestionListVisibility = false;
        //            Vm.LoadFuncVisibility = false;
        //            Vm.PDIFuncVisibility = true;
        //            Vm.ExpandVisibility = true;
        //            Settings.QNo_Sequence = data.Seqno;
        //            Vm.NextBtnBgColor = ((!string.IsNullOrEmpty(Settings.PDICompleted)) && Settings.QNo_Sequence == Vm.QuestionInfo.Count) ? Color.LightGray : Settings.Bar_Background;
        //            Vm.Next_Text = (Settings.QNo_Sequence == Vm.QuestionInfo.Count) ? "SAVE" : "NEXT";
        //            Vm.Question_No = data.Seqno;
        //            Vm.Qestion_Title = data.PDI_QestionTitle;
        //            if (!string.IsNullOrEmpty(data.AnsweredRemarks))
        //            {
        //                Vm.RemarksText = data.AnsweredRemarks;
        //            }
        //            else
        //            {
        //                Vm.RemarksText = "";
        //                if (!string.IsNullOrEmpty(data.Remarks))
        //                {
        //                    Vm.PlaceholderText = data.Remarks;
        //                }
        //                else
        //                {
        //                    Vm.PlaceholderText = "REMARKS";

        //                }
        //            }
        //            Vm.OptionsList = data.listOptions;
        //        }

        //    }
        //    catch(Exception ex)
        //    {

        //    }

        //}

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