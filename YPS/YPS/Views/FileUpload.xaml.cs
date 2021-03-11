using Syncfusion.XForms.Buttons;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModel;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileUpload : ContentPage
    {
        #region Data members declaration
        FileUploadViewModel Vm;
        YPSService service;
        bool fileaccess;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="selectedTagsData"></param>
        /// <param name="poid"></param>
        /// <param name="fuid"></param>
        /// <param name="compareFileValue"></param>
        /// <param name="fileAccess"></param>
        public FileUpload(StartUploadFileModel selectedTagsData, int poid, int fuid, string compareFileValue, bool fileAccess)
        {
            try
            {
                InitializeComponent();
                YPSLogger.TrackEvent("FileUpload", " Page constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);
                fileaccess = fileAccess;
                service = new YPSService();
                Settings.currentPage = "FileUploadPage";
                BindingContext = Vm = new FileUploadViewModel(Navigation, selectedTagsData, poid, fuid, compareFileValue, fileAccess); ;

                if (compareFileValue.ToLower().Trim() == "plfile")
                    Groupname.Text = "PL Upload";
                else
                    Groupname.Text = "File Upload";

                listView.ItemTapped += (s, e) => listView.SelectedItem = null;

            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "FileUpload constructor -> in FileUpload Page.cs " + Settings.userLoginID);
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
                if (!String.IsNullOrEmpty(Vm.SetFileName))
                {
                    Vm.SetFileName = "Please choose a file";
                    Vm.AddFolder = "ChooseFile.png";
                    Vm.IsFolderVisible = true;
                    Vm.IsCrossVisible = false;
                    Vm.FileDescription = "";
                    Vm.picStream = null;

                    if (Vm.SetFileName == "Please choose a file")
                    {
                        if (fileaccess == false && Vm.ListOfFile.Count != 0)
                        {
                            Vm.closeLabelText = true;
                            Vm.RowHeightcomplete = 50;
                        }
                        else
                        {
                            Vm.closeLabelText = false;
                            Vm.RowHeightcomplete = 0;
                        }

                        //if (Settings.userRoleID == (int)UserRoles.SupplierUser || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin ||
                        //    Settings.userRoleID == (int)UserRoles.DealerUser || Settings.userRoleID == (int)UserRoles.LogisticsAdmin ||
                        //    Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                        //{
                        //    Vm.closeLabelText = false;
                        //    Vm.RowHeightcomplete = 0;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClearSelectFile_Tapped -> in FileUpload Page.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when page is appearing.
        /// </summary>
        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                Settings.fileUploadPageCount = 0;
            }
            catch(Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in FileUpload.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on the back button and redirect to previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Tapped(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Back_Tapped method -> in FileUpload.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        private async void SfButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string text = (sender as SfButton).Text;

                if (text == Vm.labelobjFile.complete && fileaccess == false)
                {
                    await Vm.close_btn();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SfButton_Clicked method -> in FileUpload.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on either completed or not completed radio button. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Complete_StateChanged(object sender, StateChangedEventArgs e)
        {
            try
            {
                string text = (sender as SfRadioButton).Text;
               
                if (e.IsChecked == true && text == Vm.labelobjFile.complete && fileaccess == false)
                {
                    await Vm.close_btn();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Complete_StateChanged method -> in FileUpload.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on a cross icon on PopUp dialog, to close the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_DescriptionPopup(object sender, EventArgs e)
        {
            try
            {
                Vm.PopUpForFileDes = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Close_DescriptionPopup method -> in FileUpload.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
    }
}