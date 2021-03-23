﻿using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using YPS.Views;

namespace YPS.ViewModel
{
    public class PhotoUplodeViewModel : IBase
    {
        #region IComman and data members declaration
        public INavigation Navigation { get; set; }
        public ICommand select_pic { set; get; }
        public ICommand upload_pic { set; get; }
        public ICommand beforeCommand { set; get; }
        public ICommand afterCommand { set; get; }
        public ICommand CloseCommand { set; get; }
        public ICommand tap_OnImge { set; get; }
        public ICommand deleteImage { set; get; }
        public ICommand HomeCommand { get; set; }

        YPSService service;
        PhotoUploadModel Select_Items;
        Stream picStream;
        PhotosList AllPhotosData;
        AllPoData SelectedTagData;
        int puid, poid, selectiontype_index, photoCounts = 0;
        string extension = "", fileName, Mediafile, types = string.Empty;
        bool Access_Photo, multiple_Taps, checkInternet, isuploadcompleted;
        #endregion

        /// <summary>
        /// Parameterized Constructor.
        /// </summary>
        /// <param name="_Navigation"></param>
        /// <param name="page"></param>
        /// <param name="select_items"></param>
        /// <param name="allPo_Data"></param>
        /// <param name="SelectionType"></param>
        /// <param name="uploadtype"></param>
        /// <param name="accessPhoto"></param>
        public PhotoUplodeViewModel(INavigation _Navigation, Page page, PhotoUploadModel select_items, AllPoData allPo_Data, string SelectionType, int uploadtype, bool accessPhoto)
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "Page Load " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                UploadType = uploadtype;
                service = new YPSService();
                Navigation = _Navigation;
                AllPhotosData = new PhotosList();
                Access_Photo = accessPhoto;
                types = SelectionType;
                SelectedTagData = allPo_Data;

                isuploadcompleted = select_items != null ? select_items.isCompleted : allPo_Data.photoTickVisible;

                if (SelectionType.Trim().ToLower() == "initialphoto")
                {
                    Select_Items = select_items;
                    puid = select_items.PUID;
                    poid = select_items.POID;
                    //RowHeightTitle = 40;
                    //RowHeightTagNumber = 0;
                    selectiontype_index = 0;
                    closeLabelText = false;
                    RowHeightcomplete = 0;
                    NoPhotos_Visibility = true;
                    //var value = select_items.photoTags.ForEach.Select(c => c.TagNumber).ToList();
                    Tagnumbers = string.Join(",", select_items.photoTags.Select(c => c.TagNumber));

                    if (string.IsNullOrEmpty(Tagnumbers))
                    {
                        Tagnumbers = string.Join(",", select_items.photoTags.Select(c => c.IdentCode));
                    }

                    if (select_items.photoTags.Count > 1)
                    {
                        select_items.photoTags = select_items.photoTags.Select(c =>
                        {
                            c.TagNumber = null;
                            return c;
                        }).ToList();
                    }

                }
                else
                {
                    selectiontype_index = 1;
                    poid = allPo_Data.POID;
                    puid = allPo_Data.PUID;
                    GetPhotosData(puid);

                    //if (accessPhoto == true)
                    //{
                    //    FirstMainStack = false;
                    //    CompletedVal = true;
                    //    notCompeleteDisable = NotCompletedVal = false;
                    //    CompleteBtnOpacity = 0.5;
                    //}
                    //else
                    //{
                    //    FirstMainStack = true;
                    //}
                }

                //if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.SuperUser || Settings.userRoleID == (int)UserRoles.SuperViewer)
                //{
                //    FirstMainStack = closeLabelText = false;
                //    RowHeightcomplete = 0;
                //}
                //else if (Settings.userRoleID == (int)UserRoles.SupplierAdmin || Settings.userRoleID == (int)UserRoles.SupplierUser ||
                //    Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin ||
                //    Settings.userRoleID == (int)UserRoles.DealerUser || Settings.userRoleID == (int)UserRoles.LogisticsAdmin ||
                //    Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                //{
                //    closeLabelText = false;
                //    RowHeightcomplete = 0;


                //    //if (Settings.userRoleID == (int)UserRoles.SupplierAdmin ||
                //    //       Settings.userRoleID == (int)UserRoles.SupplierUser)
                //    //{
                //    //    if (isuploadcompleted == true)
                //    //    {
                //    //        DeleteIconStack = false;
                //    //        closeLabelText = true;
                //    //        RowHeightcomplete = 0;
                //    //    }
                //    //}
                //}
                //else
                //{
                //    if (accessPhoto == true)
                //    {
                //        FirstMainStack = false;
                //        CompletedVal = true;
                //        DeleteIconStack = false;
                //        notCompeleteDisable = NotCompletedVal = false;
                //        CompleteBtnOpacity = 0.5;
                //    }
                //    else
                //    {
                //        FirstMainStack = true;
                //    }
                //}

                #region Method binding to the ICommands
                select_pic = new Command(async () => await MoveToscan());
                upload_pic = new Command(async () => await Photo_Upload());
                beforeCommand = new Command(async () => await beforePic());
                afterCommand = new Command(async () => await afterPic());
                CloseCommand = new Command(async () => await ClosePic());
                tap_OnImge = new Command(image_tap);
                deleteImage = new Command(delete_image);
                HomeCommand = new Command(HomeCommand_btn);
                #endregion

                DynamicTextChange();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PhotoUplodeViewModel constructor " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked the delete icon on photo.
        /// </summary>
        /// <param name="obj"></param>
        private async void delete_image(object obj)
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in delete_image method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            IndicatorVisibility = true;
            try
            {
                if (!multiple_Taps)
                {
                    multiple_Taps = true;

                    if (Access_Photo == true)
                    {
                        await App.Current.MainPage.DisplayAlert("Message", "You can't able to delete this photo.", "OK");
                    }
                    else
                    {
                        bool conform = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure want to delete?", "OK", "Cancel");

                        if (conform)
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                IndicatorVisibility = true;
                                try
                                {
                                    /// Verifying internet connection.
                                    checkInternet = await App.CheckInterNetConnection();

                                    if (checkInternet)
                                    {
                                        var findData = obj as CustomPhotoModel;
                                        /// Calling photo delete API to delete files based on a photo id.
                                        var uploadresult = await service.DeleteImageService(findData.PhotoID);

                                        if (uploadresult != null)
                                        {
                                            if (uploadresult.status != 0)
                                            {
                                                if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BP)
                                                {
                                                    var item = finalPhotoListB.Where(x => x.PhotoID == findData.PhotoID).FirstOrDefault();
                                                    AllPhotosData.data.BPhotos.Remove(item);

                                                    if (AllPhotosData.data.BPhotos.Count == 0)
                                                    {
                                                        NoPhotos_Visibility = true;
                                                    }
                                                }
                                                else
                                                {
                                                    var item = finalPhotoListA.Where(x => x.PhotoID == findData.PhotoID).FirstOrDefault();
                                                    AllPhotosData.data.Aphotos.Remove(item);

                                                    if (AllPhotosData.data.Aphotos.Count == 0)
                                                    {
                                                        NoPhotos_Visibility = true;
                                                    }
                                                }

                                                UpdateDataBackEnd();

                                                if (AllPhotosData.data.Aphotos.Count == 0 && AllPhotosData.data.BPhotos.Count == 0)
                                                {
                                                    closeLabelText = false;
                                                    RowHeightcomplete = 0;
                                                }
                                                await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "OK");
                                            }
                                            else
                                            {
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                    else
                                    {
                                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    YPSLogger.ReportException(ex, "delete_image method from if(conform) -> in PhotoUplodeViewModel " + Settings.userLoginID);
                                    await service.Handleexception(ex);
                                }

                                IndicatorVisibility = false;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "delete_image method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
            multiple_Taps = false;
        }

        /// <summary>
        /// Gets called when clicked on "Home" icon, to move to home page
        /// </summary>
        /// <param name="obj"></param>
        private async void HomeCommand_btn(object obj)
        {
            try
            {
                //await Navigation.PopAsync(true);
                App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "HomeCommand_btn method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }

        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void image_tap(object obj)
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in image_tap method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                if (!multiple_Taps)
                {
                    multiple_Taps = true;
                    var data = obj as CustomPhotoModel;
                    int photoid = Convert.ToInt32(data.PhotoID);
                    var des = (UploadType == (int)UploadTypeEnums.GoodsPhotos_AP) ? finalPhotoListA.Where(x => x.PhotoID == photoid).FirstOrDefault() : finalPhotoListB.Where(x => x.PhotoID == photoid).FirstOrDefault();
                    var imageLists = UploadType == (int)UploadTypeEnums.GoodsPhotos_AP ? finalPhotoListA : finalPhotoListB;

                    foreach (var items in imageLists)
                    {
                        if (items.PhotoDescription.Length > 150)
                        {
                            items.ShowAndHideDescr = true;
                            items.ShowAndHideBtn = true;
                            items.ShowAndHideBtnEnable = true;
                        }
                        else if (items.PhotoDescription.Length > 0)
                        {
                            items.ShowAndHideDescr = true;
                        }
                    }
                    await Navigation.PushAsync(new ImageView(imageLists, photoid, Tagnumbers));
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "image_tap method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
            multiple_Taps = false;
        }

        /// <summary>
        /// Update the Photo count to MessageCenter.
        /// </summary>
        public void UpdateDataBackEnd()
        {
            try
            {
                int a = AllPhotosData.data.Aphotos.Count();
                int b = AllPhotosData.data.BPhotos.Count();

                if (types.Trim().ToLower() == "initialphoto")
                {
                    MessagingCenter.Send<string, string>("InitialPhotoCount", "AandB", (a + "," + b + "," + puid));
                }
                else
                {
                    if (UploadType == (int)UploadTypeEnums.GoodsPhotos_BP)
                    {
                        MessagingCenter.Send<string, string>("BPhotoCount", "msgB", b.ToString());
                    }
                    else
                    {
                        MessagingCenter.Send<string, string>("APhotoCount", "msgA", a.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "UpdateDataBackEnd method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on After Packing Button.
        /// </summary>
        /// <returns></returns>
        public async Task afterPic()
        {
            try
            {
                UploadType = (int)UploadTypeEnums.GoodsPhotos_AP;
                BeforePackingTextColor = Color.Black;
                AfterPackingTextColor = Settings.Bar_Background;

                if (AllPhotosData.data != null)
                {
                    BStack = false;
                    NoPhotos_Visibility = AllPhotosData.data.Aphotos.Count != 0 ? false : true;
                    AStack = (NoPhotos_Visibility) ? false : true;

                    if (Access_Photo == true)
                    {
                        closeLabelText = CompletedVal = true;
                        notCompeleteDisable = false;
                        RowHeightcomplete = 50;
                    }
                    else
                    {
                        if (AllPhotosData.data.Aphotos.Count == 0 && AllPhotosData.data.BPhotos.Count == 0)
                        {
                            closeLabelText = false;
                            RowHeightcomplete = 0;
                        }
                        else
                        {
                            RowHeightcomplete = 50;
                        }
                    }

                    //if (Settings.userRoleID == (int)UserRoles.SuperAdmin)
                    //{
                    //    closeLabelText = true;
                    //    RowHeightcomplete = 0;
                    //    DeleteIconStack = false;
                    //}
                    //else if (Settings.userRoleID == (int)UserRoles.SupplierAdmin || Settings.userRoleID == (int)UserRoles.SupplierUser || Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                    //                Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser ||
                    //                Settings.userRoleID == (int)UserRoles.LogisticsAdmin || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                    //{
                    //    closeLabelText = true;
                    //    RowHeightcomplete = 0;
                    //}
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "afterPic method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// gets called when clicked on Before Packing Button.
        /// </summary>
        /// <returns></returns>
        public async Task beforePic()
        {
            try
            {
                UploadType = (int)UploadTypeEnums.GoodsPhotos_BP;
                BeforePackingTextColor = Settings.Bar_Background;
                AfterPackingTextColor = Color.Black;
                if (AllPhotosData.data != null)
                {
                    AStack = false;
                    NoPhotos_Visibility = AllPhotosData.data.BPhotos.Count != 0 ? false : true;
                    BStack = (NoPhotos_Visibility) ? false : true;

                    //if (Settings.userRoleID == (int)UserRoles.SuperAdmin)
                    //{
                    //    closeLabelText = true;
                    //    RowHeightcomplete = 0;
                    //    DeleteIconStack = false;
                    //}
                    //else if (Settings.userRoleID == (int)UserRoles.SupplierAdmin || Settings.userRoleID == (int)UserRoles.SupplierUser || Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                    //                Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser ||
                    //                Settings.userRoleID == (int)UserRoles.LogisticsAdmin || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                    //{
                    //    closeLabelText = true;
                    //    RowHeightcomplete = 0;
                    //}
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "beforePic method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on Upload Button.
        /// </summary>
        /// <returns></returns>
        public async Task Photo_Upload()
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in Photo_Upload method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                IndicatorVisibility = true; ;
                try
                {
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        FirstMainStack = true;
                        RowHeightOpenCam = 0;
                        SecondMainStack = firstStack = secondStack = false;
                        listStack = true;

                        if (extension.Trim().ToLower() == ".png" || extension.Trim().ToLower() == ".jpg" || extension.Trim().ToLower() == ".jpeg" || extension.Trim().ToLower() == ".gif" || extension.Trim().ToLower() == ".bmp")
                        {
                            description_txt = Regex.Replace(description_txt, @"\s+", " ");

                            if (selectiontype_index == 0 && puid == 0)
                            {
                                //List<PhotoUploadModel> phUploadlist = new List<PhotoUploadModel>();

                                Photo singlePhoto = new Photo();
                                singlePhoto.PUID = Select_Items.PUID;
                                singlePhoto.PhotoID = 0;
                                singlePhoto.PhotoURL = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                                singlePhoto.PhotoDescription = description_txt;
                                singlePhoto.FileName = fileName;
                                singlePhoto.CreatedBy = Settings.userLoginID;
                                singlePhoto.UploadType = UploadType;// uploadType;
                                singlePhoto.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                                singlePhoto.FullName = Settings.Username;
                                singlePhoto.PicStream = picStream;

                                PhotoUploadModel DataForFileUpload = new PhotoUploadModel();
                                DataForFileUpload = Select_Items;
                                DataForFileUpload.CreatedBy = Settings.userLoginID;
                                //DataForFileUpload.photo.FileName = fileName;
                                DataForFileUpload.photos.Add(singlePhoto);

                                /// Calling the blob API to upload photo. 
                                var initialdata = await BlobUpload.YPSFileUpload(UploadType, (int)BlobContainer.cnttagfiles, DataForFileUpload);

                                var initialresult = initialdata as InitialResponse;
                                if (initialresult != null)
                                {
                                    if (initialresult.status != 0)
                                    {
                                        selectiontype_index = 1;
                                        puid = initialresult.data.PUID;

                                        if (initialresult.data.photoTags.Count != 0)
                                        {
                                            var result = initialresult.data.photoTags.Select(x => x.TagNumber).ToList();
                                            Settings.Tagnumbers = String.Join(" , ", result);
                                        }

                                        if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AP)
                                        {
                                            foreach (var photo in initialresult.data.photos)
                                            {
                                                AllPhotosData.data = new UploadedPhotosList<CustomPhotoModel>() { Aphotos = { new CustomPhotoModel() { PhotoURL = photo.PhotoURL, PhotoDescription = photo.PhotoDescription, PhotoID = photo.PhotoID, UploadType = UploadType/* uploadType*/, FullName = photo.FullName, CreatedDate = photo.CreatedDate } } };
                                            }
                                            finalPhotoListA = AllPhotosData.data.Aphotos;
                                            AStack = true;
                                            BStack = false;
                                            AfterPackingTextColor = Settings.Bar_Background;
                                            BeforePackingTextColor = Color.Black;
                                        }
                                        else
                                        {
                                            foreach (var photo in initialresult.data.photos)
                                            {
                                                AllPhotosData.data = new UploadedPhotosList<CustomPhotoModel> { BPhotos = { new CustomPhotoModel() { PhotoURL = photo.PhotoURL, PhotoDescription = photo.PhotoDescription, PhotoID = photo.PhotoID, UploadType = UploadType/* uploadType*/, FullName = photo.FullName, CreatedDate = photo.CreatedDate } } };
                                            }

                                            finalPhotoListB = AllPhotosData.data.BPhotos;
                                            AStack = false;
                                            BStack = true;
                                            AfterPackingTextColor = Color.Black;
                                            BeforePackingTextColor = Settings.Bar_Background;
                                        }

                                        foreach (var items in Select_Items.photoTags)
                                        {
                                            if (items.TaskID != 0 && items.TagTaskStatus == 0)
                                            {
                                                TagTaskStatus tagtaskstatus = new TagTaskStatus();
                                                tagtaskstatus.TaskID = Helperclass.Encrypt(items.TaskID.ToString());
                                                tagtaskstatus.POTagID = Helperclass.Encrypt(items.POTagID.ToString());
                                                tagtaskstatus.Status = 1;
                                                tagtaskstatus.CreatedBy = Settings.userLoginID;

                                                var result = await service.UpdateTagTaskStatus(tagtaskstatus);

                                                //if (result.status == 1)
                                                //{
                                                if (items.TaskID != 0 && items.TaskStatus == 0)
                                                {
                                                    TagTaskStatus taskstatus = new TagTaskStatus();
                                                    taskstatus.TaskID = Helperclass.Encrypt(items.TaskID.ToString());
                                                    taskstatus.TaskStatus = 1;
                                                    taskstatus.CreatedBy = Settings.userLoginID;

                                                    var taskval = await service.UpdateTaskStatus(taskstatus);
                                                }
                                                //DependencyService.Get<IToastMessage>().ShortAlert("Marked as done.");
                                                //}
                                            }
                                        }

                                        Settings.IsRefreshPartsPage = true;
                                        DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                }
                            }
                            else if (selectiontype_index == 1 && puid > 0)
                            {
                                List<CustomPhotoModel> phUploadlist = new List<CustomPhotoModel>();

                                CustomPhotoModel phUpload = new CustomPhotoModel();
                                phUpload.PUID = puid;
                                phUpload.PhotoID = 0;
                                phUpload.PhotoURL = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                                phUpload.PhotoDescription = description_txt;
                                phUpload.FileName = fileName;
                                phUpload.CreatedBy = Settings.userLoginID;
                                phUpload.UploadType = UploadType;// uploadType;
                                phUpload.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                                phUpload.FullName = Settings.Username;
                                phUpload.PicStream = picStream;

                                phUploadlist.Add(phUpload);

                                /// Calling the blob API to upload photo. 
                                var data = await BlobUpload.YPSFileUpload(UploadType, (int)BlobContainer.cnttagfiles, null, phUploadlist);
                                var result = data as SecondTimeResponse;

                                if (result != null)
                                {
                                    if (result.status != 0)
                                    {
                                        if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AP) //(uploadType == "A")
                                        {
                                            foreach (var val in result.data)
                                            {
                                                AllPhotosData.data.Aphotos.Insert(0, new CustomPhotoModel() { PhotoURL = val.PhotoURL, PhotoDescription = val.PhotoDescription, PhotoID = val.PhotoID, UploadType = UploadType, FullName = val.FullName, CreatedDate = val.CreatedDate });
                                            }
                                            finalPhotoListA = AllPhotosData.data.Aphotos;
                                            NoPhotos_Visibility = finalPhotoListA.Count != 0 ? false : true;
                                            BStack = false;
                                            AStack = NoPhotos_Visibility ? false : true;
                                        }
                                        else
                                        {
                                            foreach (var val in result.data)
                                            {
                                                AllPhotosData.data.BPhotos.Insert(0, new CustomPhotoModel() { PhotoURL = val.PhotoURL, PhotoDescription = val.PhotoDescription, PhotoID = val.PhotoID, UploadType = UploadType, FullName = val.FullName, CreatedDate = val.CreatedDate });
                                            }
                                            finalPhotoListB = AllPhotosData.data.BPhotos;
                                            NoPhotos_Visibility = finalPhotoListB.Count != 0 ? false : true;
                                            AStack = false;
                                            BStack = NoPhotos_Visibility ? false : true;
                                        }

                                        Settings.IsRefreshPartsPage = true;
                                        DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                }
                            }
                            description_txt = string.Empty;

                            UpdateDataBackEnd();

                            if (AllPhotosData.data.Aphotos.Count == 0 && AllPhotosData.data.BPhotos.Count == 0)
                            {
                                closeLabelText = false;
                                RowHeightcomplete = 0;
                            }
                            else
                            {
                                //if (Settings.userRoleID == (int)UserRoles.SupplierAdmin ||
                                //    Settings.userRoleID == (int)UserRoles.SupplierUser)
                                //{
                                //    if (isuploadcompleted == true)
                                //    {
                                //        DeleteIconStack = false;
                                //        closeLabelText = false;
                                //        RowHeightcomplete = 0;
                                //    }
                                //}
                                //else
                                //{
                                //    closeLabelText = true;
                                //    RowHeightcomplete = 50;
                                //}
                            }

                            //if (Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                            //        Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser ||
                            //        Settings.userRoleID == (int)UserRoles.LogisticsAdmin || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                            //{
                            //    closeLabelText = true;
                            //    RowHeightcomplete = 0;
                            //    DeleteIconStack = false;
                            //}

                            if (Device.RuntimePlatform == Device.Android)
                            {

                                if (!string.IsNullOrEmpty(Mediafile))
                                {
                                    try
                                    {
                                        bool b = File.Exists(Mediafile);
                                        File.Delete(Mediafile);
                                    }
                                    catch (Exception ex)
                                    {
                                        await service.Handleexception(ex);
                                        YPSLogger.ReportException(ex, "while deleting the Mediafile in Photo_Upload method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                                    }
                                }
                            }
                            else
                            {
                                string pathToNewFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp");

                                if (Directory.Exists(pathToNewFolder))
                                {
                                    try
                                    {
                                        Directory.Delete(pathToNewFolder, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        await service.Handleexception(ex);
                                        YPSLogger.ReportException(ex, "while deleting the folder in Photo_Upload method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                                    }
                                }
                                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                                var files = Directory.GetFiles(documents);

                                if (!files.Any())
                                {
                                    IndicatorVisibility = false;
                                    return;
                                }

                                foreach (var file in files)
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch (Exception ex)
                                    {
                                        await service.Handleexception(ex);
                                        YPSLogger.ReportException(ex, "while deleting the each files in Parts2y folder -> Photo_Upload method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                                    }
                                }
                            }

                        }
                        else
                        {
                            DependencyService.Get<IToastMessage>().ShortAlert("Please upload files having extensions: .png, .jpg, .jpeg, .gif, .bmp only.");
                        }
                    }
                    else
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "Photo_Upload method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
                finally
                {
                    IndicatorVisibility = false;
                }
            });
        }

        /// <summary>
        /// Get the existing uploaded photo(s).
        /// </summary>
        /// <param name="puid_value"></param>
        /// <returns></returns>
        public async Task GetPhotosData(int puid_value)
        {

            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in GetPhotosData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IndicatorVisibility = true;
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        /// Calling photo API to get photos.
                        var uploadresult = await service.FinalPhotosList(puid_value);

                        if (uploadresult != null)
                        {
                            if (uploadresult.status != 0)
                            {
                                AllPhotosData = uploadresult;

                                if (AllPhotosData.data.photoTags.Count != 0)
                                {
                                    var result = AllPhotosData.data.photoTags.Select(x => x.TagNumber).ToList();
                                    string result1 = String.Join(" , ", result);
                                    Tagnumbers = result1;
                                }

                                finalPhotoListA = AllPhotosData.data.Aphotos;
                                finalPhotoListB = AllPhotosData.data.BPhotos;

                                if (UploadType == (int)UploadTypeEnums.GoodsPhotos_AP)
                                {
                                    AStack = true;
                                    NoPhotos_Visibility = finalPhotoListA.Count != 0 ? false : true;
                                    AfterPackingTextColor = Settings.Bar_Background;
                                    BeforePackingTextColor = Color.Black;
                                }
                                else
                                {
                                    BStack = true;
                                    NoPhotos_Visibility = finalPhotoListB.Count != 0 ? false : true;
                                    AfterPackingTextColor = Color.Black;
                                    BeforePackingTextColor = Settings.Bar_Background;
                                }

                                //if (AllPhotosData.data.Aphotos.Count == 0 && AllPhotosData.data.BPhotos.Count == 0)
                                //{
                                //    closeLabelText = false;
                                //    RowHeightcomplete = 0;
                                //}
                                //else
                                //{
                                //    if (Settings.userRoleID == (int)UserRoles.SupplierAdmin ||
                                //    Settings.userRoleID == (int)UserRoles.SupplierUser)
                                //    {
                                //        if (isuploadcompleted == true)
                                //        {
                                //            DeleteIconStack = false;
                                //            closeLabelText = false;
                                //            RowHeightcomplete = 0;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        closeLabelText = true;
                                //        RowHeightcomplete = 50;
                                //    }

                                //    if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.MfrAdmin ||
                                //    Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser ||
                                //    Settings.userRoleID == (int)UserRoles.LogisticsAdmin || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                                //    {
                                //        RowHeightcomplete = 0;
                                //        DeleteIconStack = false;
                                //    }
                                //}
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "GetPhotosData method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
                IndicatorVisibility = false;
            });
        }

        /// <summary>
        /// Gets called when clicked on Complete RadioButton
        /// </summary>
        /// <returns></returns>
        public async Task ClosePic()
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in ClosePic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                NotCompletedVal = false;
                CompleteBtnOpacity = 0.5;

                bool result = await App.Current.MainPage.DisplayAlert("Complete", "Are you sure?", "Yes", "No");

                if (result)
                {
                    /// Verifying internet connection.
                    checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        /// Calling ClosePhoto API to close upload photo.
                        var closePhotoResult = await service.ClosePhotoData(poid, puid);

                        if (closePhotoResult.status != 0 || closePhotoResult != null)
                        {
                            MessagingCenter.Send<string, string>("PhotoComplted", "showtickMark", puid.ToString());

                            switch (Device.RuntimePlatform)
                            {
                                case Device.iOS:
                                    await App.Current.MainPage.DisplayAlert("Completed", "Success.", "Close");
                                    break;
                                case Device.Android:
                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                    break;
                            }
                            await Navigation.PopAsync(true);
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                else
                {
                    CompletedVal = false;
                    NotCompletedVal = true;
                    CompleteBtnOpacity = 1.0;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ClosePic method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private async Task MoveToscan()
        {
            try
            {
                if ((AllPhotosData != null && AllPhotosData.data != null && AllPhotosData.data.photoTags != null && AllPhotosData.data.photoTags.Count > 1) || (Select_Items != null && Select_Items.photoTags != null && Select_Items.photoTags.Count > 1))
                {
                    await SelectPic();
                }
                else
                {
                    if (Settings.CanOpenScanner == true)
                    {
                        //Settings.CanOpenScanner = false;
                        await SelectPic();
                    }
                    else
                    {
                        //if (types.Trim().ToLower() == "initialphoto")
                        if (selectiontype_index == 0 && puid == 0)
                        {
                            await Navigation.PushAsync(new ScanPage(UploadType, Select_Items, true, null));
                        }
                        else
                        {
                            await Navigation.PushAsync(new ScanPage(UploadType, null, false, SelectedTagData));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Gets called when clicked on the Camera icon for uploading photo.
        /// </summary>
        /// <returns></returns>
        public async Task SelectPic()
        {
            YPSLogger.TrackEvent("PhotoUplodeViewModel", "in SelectPic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                if (Select_Items != null && Select_Items.photoTags != null && Select_Items.photoTags.Count > 0)
                {
                    Select_Items.photoTags = Select_Items.photoTags.Select(c =>
                    {
                        c.TagNumber = null;
                        return c;
                    }).ToList();
                }


                string action = await App.Current.MainPage.DisplayActionSheet("", "Cancel", null, "Camera", "Gallery");

                if (action == "Camera")
                {
                    IndicatorVisibility = true;

                    /// Checking camera is available or not in in mobile.
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
                        btnenable = false;
                        /// Request permission a user to allowed take photos from the camera.
                        var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                        var statusiOS = resultIOS[Permission.Camera];

                        /// Checking permission is allowed or denied by the user to access the photo from mobile.
                        if (statusiOS == PermissionStatus.Denied)
                        {
                            var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the camera to take photos.", null, null, "Maybe Later", "Settings");
                            switch (checkSelect)
                            {
                                case "Maybe Later":
                                    break;
                                case "Settings":
                                    CrossPermissions.Current.OpenAppSettings();
                                    break;
                            }
                        }
                        else if (statusiOS == PermissionStatus.Granted)
                        {
                            var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                            {
                                PhotoSize = PhotoSize.Custom,
                                CustomPhotoSize = Settings.PhotoSize,
                                CompressionQuality = Settings.CompressionQuality
                            });

                            if (file != null)
                            {
                                IndicatorVisibility = false;
                                btnenable = true;

                                if (photoCounts == 0)
                                {
                                    CaptchaImage1 = ImageSource.FromStream(() =>
                                    {
                                        return file.GetStreamWithImageRotatedForExternalStorage();
                                    });

                                    firstStack = true;
                                    closeLabelText = false;
                                    RowHeightcomplete = 0;
                                    listStack = false;
                                    secondStack = false;
                                    NoPhotos_Visibility = false;

                                }
                                else
                                {
                                    CaptchaImage2 = ImageSource.FromStream(() =>
                                    {
                                        return file.GetStreamWithImageRotatedForExternalStorage();
                                    });

                                    firstStack = false;
                                    secondStack = true;
                                    closeLabelText = false;
                                    listStack = false;
                                    RowHeightcomplete = 0;
                                    NoPhotos_Visibility = false;

                                }

                                extension = Path.GetExtension(file.Path);
                                Mediafile = file.Path;
                                picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(file.Path);
                                FirstMainStack = false;
                                RowHeightOpenCam = 100;
                                SecondMainStack = true;
                            }
                        }
                        else
                        {
                            btnenable = true;
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                    }
                    btnenable = true;
                }
                else if (action == "Gallery")
                {
                    /// Request permission a user to allowed take photos from the gallery.
                    var resultIOS = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Photos);
                    var statusiOS = resultIOS[Permission.Photos];

                    /// Checking permission is allowed or denied by the user to access the photo from mobile.
                    if (statusiOS == PermissionStatus.Denied)
                    {
                        var checkSelect = await App.Current.MainPage.DisplayActionSheet("Permission is needs access to the gallery to take photos.", null, null, "Maybe Later", "Settings");
                        switch (checkSelect)
                        {
                            case "Maybe Later":
                                break;
                            case "Settings":
                                CrossPermissions.Current.OpenAppSettings();
                                break;
                        }
                    }
                    else if (statusiOS == PermissionStatus.Granted)
                    {
                        IndicatorVisibility = true;

                        var fileOS = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                        {
                            PhotoSize = PhotoSize.Custom,
                            CustomPhotoSize = Settings.PhotoSize,
                            CompressionQuality = Settings.CompressionQuality
                        });

                        if (fileOS != null)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                if (photoCounts == 0)
                                {
                                    CaptchaImage1 = ImageSource.FromFile(fileOS.Path);
                                    firstStack = true;
                                    listStack = false;
                                    secondStack = false;
                                }
                                else
                                {
                                    CaptchaImage2 = ImageSource.FromFile(fileOS.Path);
                                    firstStack = false;
                                    secondStack = true;
                                }

                                closeLabelText = false;
                                RowHeightcomplete = 0;
                                FirstMainStack = false;
                                RowHeightOpenCam = 100;
                                SecondMainStack = true;
                                NoPhotos_Visibility = false;
                                btnenable = true;
                                extension = Path.GetExtension(fileOS.Path);
                                picStream = fileOS.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(fileOS.Path);
                            });
                        }
                    }
                    else
                    {
                        btnenable = true;
                    }
                }
                IndicatorVisibility = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectPic method -> in PhotoUplodeViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Dynamic text changed
        /// </summary>
        public void DynamicTextChange()
        {
            try
            {
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var uploadBtn = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjUploadBtn.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var desc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjDesc.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var complete = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjComplete.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();
                        var notcomplete = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjNotComplete.Trim().ToLower().Replace(" ", string.Empty)).Select(c => c.LblText).FirstOrDefault();

                        labelobjUploadBtn = uploadBtn != null ? (!string.IsNullOrEmpty(uploadBtn) ? uploadBtn : labelobjUploadBtn) : labelobjUploadBtn;
                        labelobjDesc = desc != null ? (!string.IsNullOrEmpty(desc) ? desc : labelobjDesc) : labelobjDesc;
                        labelobjComplete = (complete != null ? (!string.IsNullOrEmpty(complete) ? complete : "Complete") : "Complete");
                        labelobjNotComplete = (notcomplete != null ? (!string.IsNullOrEmpty(notcomplete) ? "Not " + notcomplete : "Not Complete") : "Not Complete");
                    }
                }

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    DeleteIconStack = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoDelete".Trim()).FirstOrDefault()) != null ? true : false;
                    FirstMainStack = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? true : false;

                    //if (isuploadcompleted == true)
                    //{
                    //    FirstMainStack = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange constructor " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        #region Properties
        private double _CompleteBtnOpacity { set; get; } = 1.0;
        public double CompleteBtnOpacity
        {
            get
            {
                return _CompleteBtnOpacity;
            }
            set
            {
                _CompleteBtnOpacity = value;
                RaisePropertyChanged("CompleteBtnOpacity");
            }
        }

        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                RaisePropertyChanged("BgColor");
            }
        }

        private Color _BeforePackingTextColor = Settings.Bar_Background;
        public Color BeforePackingTextColor
        {
            get { return _BeforePackingTextColor; }
            set
            {
                _BeforePackingTextColor = value;
                RaisePropertyChanged("BeforePackingTextColor");
            }
        }

        private Color _AfterPackingTextColor = Color.Black;
        public Color AfterPackingTextColor
        {
            get { return _AfterPackingTextColor; }
            set
            {
                _AfterPackingTextColor = value;
                RaisePropertyChanged("AfterPackingTextColor");
            }
        }


        private ImageSource _captchaImage1;
        public ImageSource CaptchaImage1
        {
            get { return _captchaImage1; }
            set
            {
                _captchaImage1 = value;
                NotifyPropertyChanged();
            }
        }
        private ImageSource _captchaImage2;
        public ImageSource CaptchaImage2
        {
            get { return _captchaImage2; }
            set
            {
                _captchaImage2 = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<CustomPhotoModel> _finalPhotoList_B;
        public ObservableCollection<CustomPhotoModel> finalPhotoListB
        {
            get { return _finalPhotoList_B; }
            set
            {
                _finalPhotoList_B = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<CustomPhotoModel> _finalPhotoList_A;
        public ObservableCollection<CustomPhotoModel> finalPhotoListA
        {
            get { return _finalPhotoList_A; }
            set
            {
                _finalPhotoList_A = value;
                NotifyPropertyChanged();
            }
        }
        private bool _BStack = false;
        public bool BStack
        {
            get { return _BStack; }
            set
            {
                _BStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _AStack = false;
        public bool AStack
        {
            get { return _AStack; }
            set
            {
                _AStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _firstStack = false;
        public bool firstStack
        {
            get { return _firstStack; }
            set
            {
                _firstStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _secondStack = false;
        public bool secondStack
        {
            get { return _secondStack; }
            set
            {
                _secondStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _listStack = true;
        public bool listStack
        {
            get { return _listStack; }
            set
            {
                _listStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _FirstMainStack = true;
        public bool FirstMainStack
        {
            get { return _FirstMainStack; }
            set
            {
                _FirstMainStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _SecondMainStack = false;
        public bool SecondMainStack
        {
            get { return _SecondMainStack; }
            set
            {
                _SecondMainStack = value;
                NotifyPropertyChanged();
            }
        }
        private bool _btnenable = true;
        public bool btnenable
        {
            get { return _btnenable; }
            set
            {
                _btnenable = value;
                NotifyPropertyChanged();
            }
        }
        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();
            }
        }
        private string _description_txt = "";
        public string description_txt
        {
            get { return _description_txt; }
            set
            {
                _description_txt = value;
                NotifyPropertyChanged();
            }
        }
        public bool _NoPhotos_Visibility = false;
        public bool NoPhotos_Visibility
        {
            get { return _NoPhotos_Visibility; }
            set
            {
                _NoPhotos_Visibility = value;
                NotifyPropertyChanged();
            }
        }
        public string _Tagnumbers = Settings.Tagnumbers;
        public string Tagnumbers
        {
            get { return _Tagnumbers; }
            set
            {
                _Tagnumbers = value;
                NotifyPropertyChanged();
            }
        }
        private bool _closeLabelText = false;
        public bool closeLabelText
        {
            get { return _closeLabelText; }
            set
            {
                _closeLabelText = value;
                NotifyPropertyChanged();
            }
        }
        private int _RowHeightTitle = 30;
        public int RowHeightTitle
        {
            get => _RowHeightTitle;
            set
            {
                _RowHeightTitle = value;
                NotifyPropertyChanged();
            }
        }
        private int _RowHeightTagNumber = 20;
        public int RowHeightTagNumber
        {
            get => _RowHeightTagNumber;
            set
            {
                _RowHeightTagNumber = value;
                NotifyPropertyChanged();
            }
        }
        private int _RowHeightcomplete = 0;
        public int RowHeightcomplete
        {
            get => _RowHeightcomplete;
            set
            {
                _RowHeightcomplete = value;
                NotifyPropertyChanged();
            }
        }
        private int _RowHeightOpenCam = 0;
        public int RowHeightOpenCam
        {
            get => _RowHeightOpenCam;
            set
            {
                _RowHeightOpenCam = value;
                NotifyPropertyChanged();
            }
        }
        private bool _CompletedVal = false;
        public bool CompletedVal
        {
            get { return _CompletedVal; }
            set
            {
                _CompletedVal = value;
                NotifyPropertyChanged();
            }
        }
        private bool _notCompeleteDisable = true;
        public bool notCompeleteDisable
        {
            get { return _notCompeleteDisable; }
            set
            {
                _notCompeleteDisable = value;
                NotifyPropertyChanged();
            }
        }
        private bool _NotCompletedVal = true;
        public bool NotCompletedVal
        {
            get { return _NotCompletedVal; }
            set
            {
                _NotCompletedVal = value;
                NotifyPropertyChanged();
            }
        }
        public int _UploadType;
        public int UploadType
        {
            get { return _UploadType; }
            set
            {
                _UploadType = value;
                NotifyPropertyChanged();
            }
        }
        private bool _DeleteIconStack = true;
        public bool DeleteIconStack
        {
            get => _DeleteIconStack;
            set
            {
                _DeleteIconStack = value;
                NotifyPropertyChanged();
            }
        }

        private string _labelobjUploadBtn = "Upload";
        public string labelobjUploadBtn
        {
            get => _labelobjUploadBtn;
            set
            {
                _labelobjUploadBtn = value;
                NotifyPropertyChanged();
            }
        }

        private string _labelobjDesc = "Description";
        public string labelobjDesc
        {
            get => _labelobjDesc;
            set
            {
                _labelobjDesc = value;
                NotifyPropertyChanged();
            }
        }

        private string _labelobjComplete = "LCUploadComplete";
        public string labelobjComplete
        {
            get => _labelobjComplete;
            set
            {
                _labelobjComplete = value;
                NotifyPropertyChanged();
            }
        }

        private string _labelobjNotComplete = "LCUploadNotComplete";
        public string labelobjNotComplete
        {
            get => _labelobjNotComplete;
            set
            {
                _labelobjNotComplete = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}