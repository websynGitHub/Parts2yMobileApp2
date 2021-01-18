using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.YShip.YshipViewModel;

namespace YPS.YShip.YshipViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YshipFileUpload : ContentPage
    {
        YshipFileUploadViewModel Vm;

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="yshipid"></param>
        /// <param name="yBkgNo"></param>
        /// <param name="uploadType"></param>
        /// <param name="completed"></param>
        /// <param name="canceled"></param>
        public YshipFileUpload(int yshipid, string yBkgNo, string uploadType, int completed, int canceled)
        {
            try
            {
                InitializeComponent();
                BindingContext = Vm = new YshipFileUploadViewModel(yshipid, completed, canceled);

                if ("Invoice/Packing List" == uploadType)
                {
                    yBkgNoLbl.Text = yBkgNo;
                    Groupname.Text = "Invoice/Packing List";
                    PermitsStack.IsVisible = false;
                    InVoicePackingStack.IsVisible = true;
                    Vm.CheckUploadType = (int)UploadTypeEnums.Invoice + "," + (int)UploadTypeEnums.PackingList;
                    Vm.UploadType = (int)UploadTypeEnums.Invoice;
                    LblChange(string.Empty);
                }
                else
                {
                    yBkgNoLbl.Text = yBkgNo;
                    Groupname.Text = "Permits";
                    InVoicePackingStack.IsVisible = false;
                    PermitsStack.IsVisible = true;
                    Vm.CheckUploadType = (int)UploadTypeEnums.ExportPermit + "," + (int)UploadTypeEnums.TransitPermit + "," + (int)UploadTypeEnums.ImportPermit;
                    Vm.UploadType = (int)UploadTypeEnums.ExportPermit;
                    LblChange("permit");
                }
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                YPSLogger.ReportException(ex, "YshipFileUpload constructor -> in FileUpload Page.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on cross icon to clear selected file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ClearSelectFile_Tapped(object sender, EventArgs e)
        {
            try
            {
                if (Vm.SetFileName != "Please Select A File")
                {
                    Vm.SetFileName = "Please Select A File";
                    Vm.AddFolder = "ChooseFile.png";
                    Vm.IsFolderVisible = true;
                    Vm.IsCrossVisible = false;
                }
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                YPSLogger.ReportException(ex, "ClearSelectFile_Tapped -> in FileUpload Page.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on the back button and redirect to previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Tapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        /// <summary>
        /// Gets called when clicked on a cross icon on PopUp dialog, to close the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Popup(object sender, EventArgs e)
        {
            Vm.PopUpForFileDes = false;
        }

        /// <summary>
        /// Gets called when clicked on the home icon and redirect to yShip main page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TapYShipHome(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        /// <summary>
        /// This method is for dynamic text change.
        /// </summary>
        /// <param name="page"></param>
        private void LblChange(string page)
        {
            try
            {
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> lblChangeVal = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    var upload = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower() == "upload").Select(c => c.LblText).FirstOrDefault();
                    var uploadedBy = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower() == "createdby").Select(c => c.LblText).FirstOrDefault();
                    var uploadedDate = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower() == "createddate").Select(c => c.LblText).FirstOrDefault();
                    var uploadFileName = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower() == "attachment").Select(c => c.LblText).FirstOrDefault();

                    nameUploadedBylbl.Text = !string.IsNullOrEmpty(uploadedBy) ? uploadedBy + " : " : nameUploadedBylbl.Text;
                    nameUploadedDatelbl.Text = !string.IsNullOrEmpty(uploadedDate) ? uploadedDate + " : " : nameUploadedDatelbl.Text;
                    nameAttachmentlbl.Text = !string.IsNullOrEmpty(uploadFileName) ? uploadFileName + " : " : nameAttachmentlbl.Text;

                    if (page == "permit")
                    {
                        var permitno = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower() == "permitno").Select(c => c.LblText).FirstOrDefault();
                        var permitdate = lblChangeVal.Where(wr => wr.FieldID.Trim().ToLower() == "permitdate").Select(c => c.LblText).FirstOrDefault();

                        namePermitNo.Placeholder = !string.IsNullOrEmpty(permitno) ? permitno : namePermitNo.Placeholder;
                        Vm.PermitsDate = Vm.PermitsDateDefaultVal = !string.IsNullOrEmpty(permitdate) ? permitdate : Vm.PermitsDate;
                        permitUploadBtnLbl.Text = !string.IsNullOrEmpty(upload) ? upload : permitUploadBtnLbl.Text;
                        namePermitlbl.Text = !string.IsNullOrEmpty(permitno) ? permitno + " : " : namePermitlbl.Text;
                        namePermitOnlbl.Text = !string.IsNullOrEmpty(permitdate) ? permitdate + " : " : namePermitOnlbl.Text;

                    }
                    else
                    {
                        pluploadBtnLbl.Text = !string.IsNullOrEmpty(upload) ? upload : pluploadBtnLbl.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSService service = new YPSService();
                YPSLogger.ReportException(ex, "LblChange method -> in FileUpload Page.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
    }
}