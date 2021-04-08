using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
using static YPS.Model.SearchModel;

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class PhotoRepoPageViewModel : IBase
    {
        public INavigation Navigation { get; set; }
        YPSService service;
        PhotoRepoPage pagename;
        SendPodata sendPodata;
        Stream picStream;
        int puid, poid, selectiontype_index, photoCounts = 0;
        string extension = "", Mediafile, fileName, fullFilename;
        public ICommand DeleteAllCmd { set; get; }
        public ICommand MoveLinkCmd { set; get; }
        //public ICommand LinkPhotoCmd { set; get; }
        public ICommand ViewPhotoDetailsCmd { set; get; }
        public ICommand DeleteImageCmd { set; get; }
        public ICommand CheckedChangedCmd { set; get; }
        public Command select_pic { set; get; }
        public Command upload_pic { set; get; }
        public int taskID { get; set; }

        public PhotoRepoPageViewModel(INavigation navigation, PhotoRepoPage page)
        {
            try
            {
                YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in PhotoRepoPageViewModel constructor" + DateTime.Now + " UserId: " + Settings.userLoginID);

                service = new YPSService();
                Navigation = navigation;
                pagename = page;
                DeleteImageCmd = new Command(DeleteImage);
                DeleteAllCmd = new Command(DeleteImage);
                CheckedChangedCmd = new Command(CheckedChanged);
                select_pic = new Command(async () => await SelectPic());
                upload_pic = new Command(async () => await UploadPhoto());
                MoveLinkCmd = new Command(async () => await ShowContentsToLink());
                ViewPhotoDetailsCmd = new Command(ViewPhotoDetails);
                //LinkPhotoCmd = new Command(LinkPhotoToTag);

                Task.Run(() => DynamicTextChange().Wait());
                Task.Run(() => GetPhotosData().Wait());
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "PhotoRepoPageViewModel Constructor  -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void CheckedChanged(object sender)
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in CheckedChanged method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var val = sender as Plugin.InputKit.Shared.Controls.CheckBox;
                var item = (PhotoRepoDBModel)val.BindingContext;

                item.IsSelected = val.IsChecked == true ? true : false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckedChanged method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void LinkPhotoToTag(object sender)
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in LinkPhotoToTag method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var value = sender as AllPoData;

                if (value != null)
                {
                    var firstphotovalue = RepoPhotosList.FirstOrDefault();

                    PhotoUploadModel selectedTagsData = new PhotoUploadModel();

                    taskID = value.TaskID;
                    Settings.POID = value.POID;
                    Settings.TaskID = value.TaskID;
                    selectedTagsData.POID = value.POID;
                    selectedTagsData.isCompleted = value.photoTickVisible;

                    List<PhotoTag> lstdat = new List<PhotoTag>();

                    if (value.TagAPhotoCount == 0 && value.TagBPhotoCount == 0 && value.PUID == 0)
                    {
                        PhotoTag tg = new PhotoTag();

                        if (value.POTagID != 0)
                        {
                            tg.POTagID = value.POTagID;
                            Settings.Tagnumbers = value.TagNumber;
                            lstdat.Add(tg);

                            selectedTagsData.photoTags = lstdat;
                            Settings.currentPoTagId_Inti = lstdat;


                            if (selectedTagsData.photoTags.Count != 0 && value.IsPhotoRequired != 0)
                            {
                                //List<PhotoUploadModel> DataForFileUploadList = new List<PhotoUploadModel>();

                                PhotoUploadModel DataForFileUpload = new PhotoUploadModel();
                                DataForFileUpload = selectedTagsData;
                                DataForFileUpload.CreatedBy = Settings.userLoginID;
                                Photo phUpload = new Photo();
                                phUpload.PUID = value.PUID;
                                phUpload.PhotoID = 0;
                                phUpload.PhotoURL = firstphotovalue.FileUrl;
                                phUpload.PhotoDescription = firstphotovalue.FileDescription;
                                phUpload.FileName = firstphotovalue.FileName;
                                phUpload.CreatedBy = Settings.userLoginID;
                                phUpload.UploadType = (int)UploadTypeEnums.GoodsPhotos_BP;// uploadType;
                                phUpload.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                                phUpload.GivenName = Settings.Username;
                                DataForFileUpload.photos.Add(phUpload);

                                //DataForFileUploadList.Add(DataForFileUpload);

                                var data = await service.InitialUpload(DataForFileUpload);

                                var result = data as InitialResponse;

                                if (result != null && result.status == 1)
                                {
                                    if (value.TagTaskStatus == 0)
                                    {
                                        TagTaskStatus tagtaskstatus = new TagTaskStatus();
                                        tagtaskstatus.TaskID = Helperclass.Encrypt(value.TaskID.ToString());
                                        tagtaskstatus.POTagID = Helperclass.Encrypt(value.POTagID.ToString());
                                        tagtaskstatus.Status = 1;
                                        tagtaskstatus.CreatedBy = Settings.userLoginID;

                                        var val = await service.UpdateTagTaskStatus(tagtaskstatus);

                                        if (result.status == 1)
                                        {
                                            if (value.TaskID == 0)
                                            {
                                                TagTaskStatus taskstatus = new TagTaskStatus();
                                                taskstatus.TaskID = Helperclass.Encrypt(value.TaskID.ToString());
                                                taskstatus.TaskStatus = 1;
                                                taskstatus.CreatedBy = Settings.userLoginID;

                                                var taskval = await service.UpdateTaskStatus(taskstatus);
                                            }
                                            DependencyService.Get<IToastMessage>().ShortAlert("Photo(s) linked successfully.");
                                        }
                                    }
                                    //DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                }
                            }
                        }
                    }
                    else
                    {
                        selectedTagsData.alreadyExit = "alreadyExit";

                        if (value.imgCamOpacityB != 0.5 && value.IsPhotoRequired != 0)
                        {
                            if (value.PUID != 0)
                            {
                                List<CustomPhotoModel> phUploadList = new List<CustomPhotoModel>();


                                CustomPhotoModel phUpload = new CustomPhotoModel();
                                phUpload.PUID = value.PUID;
                                phUpload.PhotoID = 0;
                                phUpload.PhotoURL = firstphotovalue.FileUrl;
                                phUpload.PhotoDescription = firstphotovalue.FileDescription;
                                phUpload.FileName = firstphotovalue.FileName;
                                phUpload.CreatedBy = Settings.userLoginID;
                                phUpload.UploadType = (int)UploadTypeEnums.GoodsPhotos_BP;// uploadType;
                                phUpload.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                                phUpload.FullName = Settings.Username;
                                phUploadList.Add(phUpload);

                                var data = await service.PhotosUpload(phUploadList);

                                var initialresult = data as SecondTimeResponse;

                                if (initialresult != null && initialresult.status == 1)
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Success.");
                                }

                            }
                            //try
                            //{
                            //    Settings.currentPuId = value.PUID;
                            //    Settings.BphotoCount = value.TagBPhotoCount;
                            //    await Navigation.PushAsync(new PhotoUpload(null, value, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, value.photoTickVisible));
                            //}
                            //catch (Exception ex)
                            //{
                            //    YPSLogger.ReportException(ex, "LinkPhotoToTag method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                            //    var trackResult = await service.Handleexception(ex);
                            //}
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "LinkPhotoToTag method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async Task ShowContentsToLink()
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in ShowContentsToLink method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {

                if (RepoPhotosList != null)
                {
                    if ((RepoPhotosList.Where(wr => wr.IsSelected == true).FirstOrDefault()) != null)
                    {
                        bool move = await App.Current.MainPage.DisplayAlert("Move photo(s) for linking", "Are you sure to move the selected photo(s) for linking?", "Yes", "No");

                        if (move)
                        {
                            await Navigation.PushAsync(new LinkPage(new ObservableCollection<PhotoRepoDBModel>(RepoPhotosList.Where(wr => wr.IsSelected == true).ToList())));
                        }
                    }
                    else
                    {
                        bool move = await App.Current.MainPage.DisplayAlert("Move photo(s) for linking", "Are you sure to move all photo(s) for linking?", "Yes", "No");

                        if (move)
                        {
                            await Navigation.PushAsync(new LinkPage(RepoPhotosList));
                        }
                    }

                }


                //var result = await GetAllPOData();

                //if (result != null && result.data != null && result.data.allPoData != null)
                //{
                //    IsRepoaPage = false;
                //    UploadViewContentVisible = false;
                //    IsPhotoUploadIconVisible = false;
                //    POTagLinkContentVisible = true;
                //    Title = Settings.VersionID == 2 ? "VIN" : "Parts";
                //    var potagcolections = result.data.allPoData.Where(wr => wr.TaskID > 0 && wr.IsPhotoRequired != 0).OrderBy(o => o.PONumber).ToList();
                //    AllPoTagCollections = new ObservableCollection<AllPoData>(potagcolections);
                //}
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShowContentsToLink method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async Task<GetPoData> GetAllPOData()
        {
            GetPoData result = new GetPoData();
            try
            {
                YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in ShowContentsToLink method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                sendPodata = new SendPodata();
                sendPodata.UserID = Settings.userLoginID;
                sendPodata.PageSize = Settings.pageSizeYPS;
                sendPodata.StartPage = Settings.startPageYPS;
                //SearchResultGet(sendPodata);

                result = await service.LoadPoDataService(sendPodata);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllPOData method -> in PhotoRepoPageViewModel.cs" + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
            return result;
        }


        /// <summary>
        /// This method gets the search result based on search values.
        /// </summary>
        /// <param name="sendPodata"></param>
        public async void SearchResultGet(SendPodata sendPodata)
        {
            try
            {
                YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in SearchResultGet method " + DateTime.Now + " UserId: " + Settings.userLoginID);
                IndicatorVisibility = true;

                var Serchdata = await service.GetSearchValuesService(Settings.userLoginID);

                if (Serchdata != null)
                {
                    if (Serchdata.status == 1)
                    {
                        if (!string.IsNullOrEmpty(Serchdata.data.SearchCriteria))
                        {
                            var searchC = JsonConvert.DeserializeObject<SendPodata>(Serchdata.data.SearchCriteria);

                            if (searchC != null)
                            {
                                //Key
                                sendPodata.PONumber = Settings.PONumber = searchC.PONumber;
                                sendPodata.REQNo = Settings.REQNo = searchC.REQNo;
                                sendPodata.ShippingNo = Settings.ShippingNo = searchC.ShippingNo;
                                sendPodata.DisciplineID = Settings.DisciplineID = searchC.DisciplineID;
                                sendPodata.ELevelID = Settings.ELevelID = searchC.ELevelID;
                                sendPodata.ConditionID = Settings.ConditionID = searchC.ConditionID;
                                sendPodata.ExpeditorID = Settings.ExpeditorID = searchC.ExpeditorID;
                                sendPodata.PriorityID = Settings.PriorityID = searchC.PriorityID;
                                sendPodata.TagNo = Settings.TAGNo = searchC.TagNo;
                                sendPodata.IdentCode = Settings.IdentCodeNo = searchC.IdentCode;
                                sendPodata.BagNo = Settings.BagNo = searchC.BagNo;
                                sendPodata.yBkgNumber = Settings.Ybkgnumber = searchC.yBkgNumber;
                                sendPodata.TaskName = Settings.TaskName = searchC.TaskName;

                                Settings.SearchWentWrong = false;
                            }

                        }
                        else
                        {
                            await SaveAndClearSearch(true);
                        }
                    }
                    else
                    {
                        Settings.SearchWentWrong = true;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SearchResultGet method -> in PhotoRepoPageViewModel.cs" + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to clear the search criteria values and save to DB.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private async Task SaveAndClearSearch(bool val)
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in SaveAndClearSearch method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;
            try
            {
                SendPodata SaveUserDS = new SendPodata();
                SearchPassData defaultData = new SearchPassData();

                //Key
                SaveUserDS.PONumber = Settings.PONumber = string.Empty;
                SaveUserDS.REQNo = Settings.REQNo = string.Empty;
                SaveUserDS.ShippingNo = Settings.ShippingNo = string.Empty;
                SaveUserDS.DisciplineID = Settings.DisciplineID = 0;
                SaveUserDS.ELevelID = Settings.ELevelID = 0;
                SaveUserDS.ConditionID = Settings.ConditionID = 0;
                SaveUserDS.ExpeditorID = Settings.ExpeditorID = 0;
                SaveUserDS.PriorityID = Settings.PriorityID = 0;
                SaveUserDS.TagNo = Settings.TAGNo = string.Empty;
                SaveUserDS.IdentCode = Settings.IdentCodeNo = string.Empty;
                SaveUserDS.BagNo = Settings.BagNo = string.Empty;
                SaveUserDS.yBkgNumber = Settings.Ybkgnumber = string.Empty;
                SaveUserDS.TaskName = Settings.TaskName = string.Empty;
                defaultData.CompanyID = Settings.CompanyID;
                defaultData.UserID = Settings.userLoginID;
                defaultData.SearchCriteria = JsonConvert.SerializeObject(SaveUserDS);
                var responseData = await service.SaveSerchvaluesSetting(defaultData);

                if (val == true)
                {
                    SearchResultGet(SaveUserDS);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SaveAndClearSearch method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on any, already uploaded image to view it
        /// </summary>
        /// <param name="obj"></param>
        private async void ViewPhotoDetails(object obj)
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in ViewPhotoDetails method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var data = obj as PhotoRepoDBModel;
                int photoid = data.FileID;
                var des = RepoPhotosList.Where(x => x.FileID == photoid).FirstOrDefault();
                var imageLists = RepoPhotosList;


                foreach (var items in imageLists)
                {
                    if (items.FileDescription.Length > 150)
                    {
                        items.ShowAndHideDescr = true;

                    }
                    else if (items.FileDescription.Length > 0)
                    {
                        items.ShowAndHideDescr = true;
                    }
                }

                await Navigation.PushAsync(new ImageView(imageLists, photoid));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ViewPhotoDetails method -> in PhotoRepoPageViewModel.cs" + Settings.userLoginID);
                await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when clicked the delete icon on photo.
        /// </summary>
        /// <param name="obj"></param>
        private async void DeleteImage(object obj)
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in DeleteImage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            IndicatorVisibility = true;
            try
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
                            var checkInternet = await App.CheckInterNetConnection();

                            if (checkInternet)
                            {
                                var data = obj as PhotoRepoDBModel;
                                bool update = false;


                                if (data != null)
                                {
                                    var response = await service.DeleteSingleRepoPhoto(data.FileID);

                                    if (response.status == 1)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "OK");
                                        update = true;
                                    }
                                }
                                else
                                {
                                    var allresponse = await service.DeleteAllRepoPhoto();

                                    if (allresponse.status == 1)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "OK");
                                        update = true;
                                    }
                                }

                                NoRecHeight = (IsNoPhotoTxt = RepoPhotosList.Count == 0 ? true : false) == true ? 30 : 0;


                                if (update)
                                {
                                    await GetPhotosData();
                                }
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "delete_image method from if(conform) -> in LoadPageViewModel " + Settings.userLoginID);
                            await service.Handleexception(ex);
                        }

                        IndicatorVisibility = false;
                    });
                }
                //}
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DeleteImage method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }
        //private async void DeleteImage(object obj)
        //{
        //    YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in DeleteImage method " + DateTime.Now + " UserId: " + Settings.userLoginID);

        //    IndicatorVisibility = true;
        //    try
        //    {
        //        bool conform = await Application.Current.MainPage.DisplayAlert("Delete", "Are you sure want to delete?", "OK", "Cancel");

        //        if (conform)
        //        {
        //            Device.BeginInvokeOnMainThread(async () =>
        //            {
        //                IndicatorVisibility = true;
        //                try
        //                {
        //                    /// Verifying internet connection.
        //                    var checkInternet = await App.CheckInterNetConnection();

        //                    if (checkInternet)
        //                    {
        //                        var data = obj as PhotoRepoModel;

        //                        PhotoRepoSQlLite phoroRepoDB = new PhotoRepoSQlLite();

        //                        if (data != null)
        //                        {
        //                            phoroRepoDB.DeleteOfUser(data.PhotoID);

        //                            var item = RepoPhotosList.Where(x => x.FileID == data.PhotoID).FirstOrDefault();
        //                            RepoPhotosList.Remove(item);
        //                        }
        //                        else
        //                        {
        //                            phoroRepoDB.DeleteOfUser(0);
        //                            RepoPhotosList = new ObservableCollection<PhotoRepoDBModel>();
        //                        }


        //                        NoRecHeight = (IsNoPhotoTxt = RepoPhotosList.Count == 0 ? true : false) == true ? 30 : 0;

        //                        await GetPhotosData();

        //                        await App.Current.MainPage.DisplayAlert("Success", "Photo deleted successfully.", "OK");

        //                    }
        //                    else
        //                    {
        //                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    YPSLogger.ReportException(ex, "delete_image method from if(conform) -> in LoadPageViewModel " + Settings.userLoginID);
        //                    await service.Handleexception(ex);
        //                }

        //                IndicatorVisibility = false;
        //            });
        //        }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        YPSLogger.ReportException(ex, "DeleteImage method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
        //        await service.Handleexception(ex);
        //        IndicatorVisibility = false;
        //    }
        //    finally
        //    {
        //        IndicatorVisibility = false;
        //    }
        //}

        /// <summary>
        /// Get the existing uploaded photo(s).
        /// </summary>
        /// <param name="potagid"></param>
        /// <returns></returns>
        public async Task GetPhotosData()
        {

            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in GetPhotosData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    IndicatorVisibility = true;
                    /// Verifying internet connection.
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        var data = await service.GetRepoPhotos();
                        //var data = result as GetRepoPhotoResponse;

                        if (data != null && data.status == 1 && data.data.Count > 0)
                        {
                            IsPhotosListVisible = true;
                            IsPhotosListStackVisible = true;
                            IsNoPhotoTxt = false;
                            NoRecHeight = 0;
                            IsBottomButtonsVisible = true;
                            IsLinkEnable = true;
                            LinkOpacity = 1.0;
                            IsPhotoUploadIconVisible = true;
                            IsDeleteAllEnable = data.data.Count > 2 ? true : false;
                            DeleteAllOpacity = data.data.Count > 2 ? 1.0 : 0.5;

                            foreach (var val in data.data)
                            {
                                val.FullFileUrl = HostingURL.blob + "tag-files/" + val.FileUrl;
                            }

                            RepoPhotosList = new ObservableCollection<PhotoRepoDBModel>(data.data);
                        }
                        else
                        {
                            IsPhotosListVisible = false;
                            IsPhotosListStackVisible = false;
                            IsNoPhotoTxt = true;
                            NoRecHeight = 30;
                            IsBottomButtonsVisible = false;
                            IsDeleteAllEnable = IsLinkEnable = false;
                            DeleteAllOpacity = LinkOpacity = 0.5;
                            IsPhotoUploadIconVisible = true;
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "GetPhotosData method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                    await service.Handleexception(ex);
                    IndicatorVisibility = false;
                }
                IndicatorVisibility = false;
            });
        }

        ///// <summary>
        ///// Get the existing uploaded photo(s).
        ///// </summary>
        ///// <param name="potagid"></param>
        ///// <returns></returns>
        //public async Task GetPhotosDataLocal()
        //{

        //    YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in GetPhotosData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

        //    Device.BeginInvokeOnMainThread(async () =>
        //    {
        //        try
        //        {
        //            IndicatorVisibility = true;
        //            /// Verifying internet connection.
        //            var checkInternet = await App.CheckInterNetConnection();

        //            if (checkInternet)
        //            {
        //                PhotoRepoSQlLite photorepoDB = new PhotoRepoSQlLite();
        //                var photos = photorepoDB.GetPhotosOfUser();

        //                if (photos != null && photos.Count > 0)
        //                {
        //                    IsPhotosListVisible = true;
        //                    IsPhotosListStackVisible = true;
        //                    IsNoPhotoTxt = false;
        //                    NoRecHeight = 0;
        //                    IsBottomButtonsVisible = true;
        //                    IsLinkEnable = true;
        //                    LinkOpacity = 1.0;
        //                    IsPhotoUploadIconVisible = true;
        //                    IsDeleteAllEnable = photos.Count > 2 ? true : false;
        //                    DeleteAllOpacity = photos.Count > 2 ? 1.0 : 0.5;

        //                    RepoPhotosList = new ObservableCollection<PhotoRepoModel>(photos);
        //                }
        //                else
        //                {
        //                    IsPhotosListVisible = false;
        //                    IsPhotosListStackVisible = false;
        //                    IsNoPhotoTxt = true;
        //                    NoRecHeight = 30;
        //                    IsBottomButtonsVisible = false;
        //                    IsDeleteAllEnable = IsLinkEnable = false;
        //                    DeleteAllOpacity = LinkOpacity = 0.5;
        //                    IsPhotoUploadIconVisible = true;
        //                }
        //            }
        //            else
        //            {
        //                DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            YPSLogger.ReportException(ex, "GetPhotosData method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
        //            await service.Handleexception(ex);
        //            IndicatorVisibility = false;
        //        }
        //        IndicatorVisibility = false;
        //    });
        //}

        /// <summary>
        /// Gets called when clicked on Upload Button.
        /// </summary>
        /// <returns></returns>
        public async Task UploadPhoto()
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in UploadPhoto method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                IndicatorVisibility = true; ;
                try
                {
                    /// Verifying internet connection.
                    var checkInternet = await App.CheckInterNetConnection();

                    if (checkInternet)
                    {
                        RowHeightOpenCam = 50;
                        IsUploadStackVisible = IsImageViewForUploadVisible = false;
                        IsPhotosListStackVisible = true;

                        if (extension.Trim().ToLower() == ".png" || extension.Trim().ToLower() == ".jpg" || extension.Trim().ToLower() == ".jpeg" || extension.Trim().ToLower() == ".gif" || extension.Trim().ToLower() == ".bmp")
                        {
                            DescriptionText = Regex.Replace(DescriptionText, @"\s+", " ");

                            string FullFilename = "ImFi_Mob" + '_' + Settings.userLoginID + "_" + DateTime.Now.ToString("yyyy-MMM-dd-HHmmss") + "_" + Guid.NewGuid() + extension;
                            BlobUpload.UploadFile(CloudFolderKeyVal.GetBlobFolderName((int)BlobContainer.cnttagfiles), FullFilename, picStream);
                            selectiontype_index = 1;

                            //PhotoRepoModel photoRepoModel = new PhotoRepoModel();
                            ////https://ypsuploadsdev.blob.core.windows.net/tag-photos/
                            ////photoRepoModel.PhotoURL = "https://azrbsa026dv00a.blob.core.windows.net/" + "tag-photos/" + FullFilename;
                            //photoRepoModel.PhotoURL = HostingURL.blob + "tag-files/" + FullFilename;
                            //photoRepoModel.FullFileName = FullFilename;
                            //photoRepoModel.FileName = fileName;
                            //photoRepoModel.Description = DescriptionText;
                            //photoRepoModel.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                            //photoRepoModel.UserID = Settings.userLoginID;

                            //PhotoRepoSQlLite photorepoDB = new PhotoRepoSQlLite();
                            //photorepoDB.SavePhoto(photoRepoModel);
                            PhotoRepoDBModel photoobj = new PhotoRepoDBModel();
                            List<PhotoRepoDBModel> photolistobj = new List<PhotoRepoDBModel>();
                            //photoRepoModel.PhotoURL = "https://azrbsa026dv00a.blob.core.windows.net/" + "tag-photos/" + FullFilename;
                            photoobj.FullName = FullFilename;
                            photoobj.FileName = fileName;
                            photoobj.FileUrl = FullFilename;
                            photoobj.FileDescription = DescriptionText;
                            photoobj.CreatedDate = String.Format("{0:dd MMM yyyy hh:mm tt}", DateTime.Now);
                            photoobj.CreatedBy = Settings.userLoginID;
                            photolistobj.Add(photoobj);

                            var response = await service.UploadRepoPhotos(photolistobj);
                            var data = response as GetRepoPhotoResponse;

                            DescriptionText = string.Empty;

                            await GetPhotosData();

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
                                        YPSLogger.ReportException(ex, "UploadPhoto method -> in PhotoRepoPageViewModel " + Settings.userLoginID);
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
                                        YPSLogger.ReportException(ex, "while deleting the folder in Photo_Upload method -> in PhotoRepoPageViewModel " + Settings.userLoginID);
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
                                        YPSLogger.ReportException(ex, "while deleting the each files in Parts2y folder -> Photo_Upload method -> in PhotoRepoPageViewModel " + Settings.userLoginID);
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
                    YPSLogger.ReportException(ex, "UploadPhoto method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                    await service.Handleexception(ex);
                    IndicatorVisibility = false;
                }
                finally
                {
                    IndicatorVisibility = false;
                }
            });
        }

        /// <summary>
        /// Gets called when clicked on the Camera icon for uploading photo.
        /// </summary>
        /// <returns></returns>
        public async Task SelectPic()
        {
            YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in SelectPic method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                string action = await App.Current.MainPage.DisplayActionSheet("", "Cancel", null, "Camera", "Gallery");

                if (action == "Camera")
                {
                    IndicatorVisibility = true;

                    /// Checking camera is available or not in in mobile.
                    if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                    {
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
                                //btnenable = true;

                                if (photoCounts == 0)
                                {
                                    ImageViewForUpload = ImageSource.FromStream(() =>
                                    {
                                        return file.GetStreamWithImageRotatedForExternalStorage();
                                    });

                                    DeleteAllPhotos = false;
                                    IsBottomButtonsVisible = false;
                                    IsUploadStackVisible = true;
                                }
                                else
                                {
                                    DeleteAllPhotos = false;
                                    IsBottomButtonsVisible = false;
                                }

                                extension = Path.GetExtension(file.Path);
                                Mediafile = file.Path;
                                picStream = file.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(file.Path);
                                IsPhotoUploadIconVisible = false;
                                IsPhotosListVisible = false;
                                IsPhotosListStackVisible = false;
                                IsNoPhotoTxt = false;
                                NoRecHeight = 0;
                                RowHeightOpenCam = 100;
                                IsImageViewForUploadVisible = true;
                            }
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Oops", "Camera unavailable!", "OK");
                    }
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
                                    ImageViewForUpload = ImageSource.FromFile(fileOS.Path);
                                    IsUploadStackVisible = true;
                                }

                                DeleteAllPhotos = false;
                                IsNoPhotoTxt = false;
                                NoRecHeight = 0;
                                IsBottomButtonsVisible = false;
                                IsPhotoUploadIconVisible = false;
                                RowHeightOpenCam = 100;
                                IsImageViewForUploadVisible = true;
                                IsPhotosListVisible = false;
                                IsPhotosListStackVisible = false;
                                extension = Path.GetExtension(fileOS.Path);
                                picStream = fileOS.GetStreamWithImageRotatedForExternalStorage();
                                fileName = Path.GetFileNameWithoutExtension(fileOS.Path);
                            });
                        }
                    }

                }
                IndicatorVisibility = false;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "SelectPic method -> in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }


        /// <summary>
        /// Dynamic text changed
        /// </summary>
        public async Task DynamicTextChange()
        {
            try
            {
                YPSLogger.TrackEvent("PhotoRepoPageViewModel.cs", "in DynamicTextChange method " + DateTime.Now + " UserId: " + Settings.userLoginID);


                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        var done = labelval.Where(wr => wr.FieldID == labelobj.Done.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var upload = labelval.Where(wr => wr.FieldID == labelobj.Upload.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var desc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == DescriptipnPlaceholder.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var afterpacking = labelval.Where(wr => wr.FieldID == labelobj.AfterPacking.Name.Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var beforepacking = labelval.Where(wr => wr.FieldID == labelobj.BeforePacking.Name.Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Getting Label values & Status based on FieldID
                        var poid = labelval.Where(wr => wr.FieldID == labelobj.POID.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID == labelobj.ShippingNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var reqnumber = labelval.Where(wr => wr.FieldID == labelobj.REQNo.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID == labelobj.TaskName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var tagdesc = labelval.Where(wr => wr.FieldID == labelobj.TagDesc.Name.Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        var tagnumber = labelval.Where(wr => wr.FieldID == labelobj.TagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID == labelobj.IdentCode.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bagnumber = labelval.Where(wr => wr.FieldID == labelobj.BagNumber.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID == labelobj.ConditionName.Name).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid == null ? true : (poid.Status == 1 ? true : false);
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber == null ? true : (shippingnumber.Status == 1 ? true : false);
                        labelobj.REQNo.Name = (reqnumber != null ? (!string.IsNullOrEmpty(reqnumber.LblText) ? reqnumber.LblText : labelobj.REQNo.Name) : labelobj.REQNo.Name) + " :";
                        labelobj.REQNo.Status = reqnumber == null ? true : (reqnumber.Status == 1 ? true : false);
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme == null ? true : (taskanme.Status == 1 ? true : false);
                        labelobj.TagDesc.Name = (tagdesc != null ? (!string.IsNullOrEmpty(tagdesc.LblText) ? tagdesc.LblText : labelobj.TagDesc.Name) : labelobj.TagDesc.Name) + " :";
                        labelobj.TagDesc.Status = tagdesc == null ? true : (tagdesc.Status == 1 ? true : false);

                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber == null ? true : (tagnumber.Status == 1 ? true : false);
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode == null ? true : (identcode.Status == 1 ? true : false);
                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                        labelobj.BagNumber.Status = bagnumber == null ? true : (bagnumber.Status == 1 ? true : false);
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname == null ? true : (conditionname.Status == 1 ? true : false);


                        labelobj.AfterPacking.Name = "Link to " + (afterpacking != null ? (!string.IsNullOrEmpty(afterpacking.LblText) ? afterpacking.LblText : labelobj.AfterPacking.Name) : labelobj.AfterPacking.Name);
                        labelobj.AfterPacking.Status = afterpacking == null ? true : (afterpacking.Status == 1 ? true : false);
                        labelobj.BeforePacking.Name = "Link to " + (beforepacking != null ? (!string.IsNullOrEmpty(beforepacking.LblText) ? beforepacking.LblText : labelobj.BeforePacking.Name) : labelobj.BeforePacking.Name);
                        labelobj.BeforePacking.Status = beforepacking == null ? true : (beforepacking.Status == 1 ? true : false);

                        labelobj.Done.Name = (done != null ? (!string.IsNullOrEmpty(done.LblText) ? done.LblText : labelobj.Done.Name) : labelobj.Done.Name);
                        labelobj.Done.Status = done == null ? true : (done.Status == 1 ? true : false);
                        labelobj.Upload.Name = (upload != null ? (!string.IsNullOrEmpty(upload.LblText) ? upload.LblText : labelobj.Upload.Name) : labelobj.Upload.Name);
                        labelobj.Upload.Status = upload == null ? true : (upload.Status == 1 ? true : false);
                        DescriptipnPlaceholder = desc != null ? (!string.IsNullOrEmpty(desc) ? desc : DescriptipnPlaceholder) : DescriptipnPlaceholder;
                    }

                    if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                    {
                        DeleteIconStack = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoDelete".Trim()).FirstOrDefault()) != null ? true : false;

                        //if (isAllTasksDone == true)
                        //{
                        //    IsPhotoUploadIconVisible = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "PhotoUpload".Trim()).FirstOrDefault()) != null ? true : false;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method in PhotoRepoPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        #region Properties
        #region Properties for dynamic label change
        public class LabelAndActionsChangeClass
        {
            public LabelAndActionFields Done { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Done"
            };

            public LabelAndActionFields Upload { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Upload"
            };

            public LabelAndActionFields Load { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Load"
            };

            public LabelAndActionFields Parts { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "Parts"
            };

            public LabelAndActionFields POID { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "PONumber"
            };
            public LabelAndActionFields REQNo { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "REQNo"
            };
            public LabelAndActionFields ShippingNumber { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "ShippingNumber"
            };
            public LabelAndActionFields TagNumber { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "TagNumber"
            };
            public LabelAndActionFields TaskName { get; set; } = new LabelAndActionFields
            {
                Status = true,
                Name = "TaskName"
            };
            public LabelAndActionFields TagDesc { get; set; } = new LabelAndActionFields { Status = true, Name = "Tag Description" };


            public LabelAndActionFields IdentCode { get; set; } = new LabelAndActionFields { Status = true, Name = "IdentCode" };
            public LabelAndActionFields BagNumber { get; set; } = new LabelAndActionFields { Status = true, Name = "BagNumber" };
            public LabelAndActionFields ConditionName { get; set; } = new LabelAndActionFields { Status = true, Name = "ConditionName" };
            public LabelAndActionFields BeforePacking { get; set; } = new LabelAndActionFields { Status = true, Name = "Before Packing" };
            public LabelAndActionFields AfterPacking { get; set; } = new LabelAndActionFields { Status = true, Name = "After Packing" };

        }
        public class LabelAndActionFields : IBase
        {
            public bool _Status;
            public bool Status
            {
                get => _Status;
                set
                {
                    _Status = value;
                    NotifyPropertyChanged();
                }
            }

            public string _Name;
            public string Name
            {
                get => _Name;
                set
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public LabelAndActionsChangeClass _labelobj = new LabelAndActionsChangeClass();
        public LabelAndActionsChangeClass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        private int _RowHeightOpenCam = 50;
        public int RowHeightOpenCam
        {
            get => _RowHeightOpenCam;
            set
            {
                _RowHeightOpenCam = value;
                NotifyPropertyChanged();
            }
        }

        private int _NoRecHeight = 20;
        public int NoRecHeight
        {
            get => _NoRecHeight;
            set
            {
                _NoRecHeight = value;
                NotifyPropertyChanged();
            }
        }

        private string _Title = "Photo Repo";
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                RaisePropertyChanged("Title");
            }
        }


        private bool _IsRepoaPage = true;
        public bool IsRepoaPage
        {
            get { return _IsRepoaPage; }
            set
            {
                _IsRepoaPage = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<AllPoData> _AllPoTagCollections;
        public ObservableCollection<AllPoData> AllPoTagCollections
        {
            get { return _AllPoTagCollections; }
            set
            {
                _AllPoTagCollections = value;
                //if (value != null && value.Count > 0)
                //{
                //    //PONumber = value[0].PONumber;
                //    //ShippingNumber = value[0].ShippingNumber;
                //    //REQNo = value[0].REQNo;
                //    //TaskName = value[0].TaskName;
                //}
                RaisePropertyChanged("AllPoTagCollections");
            }
        }

        private string _PONumber;
        public string PONumber
        {
            get { return _PONumber; }
            set
            {
                _PONumber = value;
                RaisePropertyChanged("PONumber");
            }
        }

        private string _ShippingNumber;
        public string ShippingNumber
        {
            get { return _ShippingNumber; }
            set
            {
                _ShippingNumber = value;
                RaisePropertyChanged("ShippingNumber");
            }
        }

        private string _REQNo;
        public string REQNo
        {
            get { return _REQNo; }
            set
            {
                _REQNo = value;
                RaisePropertyChanged("REQNo");
            }
        }

        private string _TaskName;
        public string TaskName
        {
            get { return _TaskName; }
            set
            {
                _TaskName = value;
                RaisePropertyChanged("TaskName");
            }
        }

        private bool _POTagLinkContentVisible = false;
        public bool POTagLinkContentVisible
        {
            get { return _POTagLinkContentVisible; }
            set
            {
                _POTagLinkContentVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _UploadViewContentVisible = true;
        public bool UploadViewContentVisible
        {
            get { return _UploadViewContentVisible; }
            set
            {
                _UploadViewContentVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsBottomButtonsVisible = false;
        public bool IsBottomButtonsVisible
        {
            get { return _IsBottomButtonsVisible; }
            set
            {
                _IsBottomButtonsVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsImageViewForUploadVisible = false;
        public bool IsImageViewForUploadVisible
        {
            get { return _IsImageViewForUploadVisible; }
            set
            {
                _IsImageViewForUploadVisible = value;
                NotifyPropertyChanged();
            }
        }

        private ObservableCollection<PhotoRepoDBModel> _RepoPhotosList;
        public ObservableCollection<PhotoRepoDBModel> RepoPhotosList
        {
            get { return _RepoPhotosList; }
            set
            {
                _RepoPhotosList = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsNoPhotoTxt = true;
        public bool IsNoPhotoTxt
        {
            get { return _IsNoPhotoTxt; }
            set
            {
                _IsNoPhotoTxt = value;
                NotifyPropertyChanged();
            }
        }

        private string _DescriptionText = "";
        public string DescriptionText
        {
            get { return _DescriptionText; }
            set
            {
                _DescriptionText = value;
                NotifyPropertyChanged();
            }
        }

        private bool _DeleteAllPhotos = false;
        public bool DeleteAllPhotos
        {
            get { return _DeleteAllPhotos; }
            set
            {
                _DeleteAllPhotos = value;
                NotifyPropertyChanged();
            }
        }

        private string _DescriptipnPlaceholder = "Description";
        public string DescriptipnPlaceholder
        {
            get => _DescriptipnPlaceholder;
            set
            {
                _DescriptipnPlaceholder = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsUploadStackVisible = false;
        public bool IsUploadStackVisible
        {
            get { return _IsUploadStackVisible; }
            set
            {
                _IsUploadStackVisible = value;
                NotifyPropertyChanged();
            }
        }

        private ImageSource _ImageViewForUpload;
        public ImageSource ImageViewForUpload
        {
            get { return _ImageViewForUpload; }
            set
            {
                _ImageViewForUpload = value;
                NotifyPropertyChanged();
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

        private bool _IsPhotoUploadIconVisible = true;
        public bool IsPhotoUploadIconVisible
        {
            get { return _IsPhotoUploadIconVisible; }
            set
            {
                _IsPhotoUploadIconVisible = value;
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

        private bool _IsPhotosListStackVisible = true;
        public bool IsPhotosListStackVisible
        {
            get { return _IsPhotosListStackVisible; }
            set
            {
                _IsPhotosListStackVisible = value;
                NotifyPropertyChanged();
            }
        }

        private bool _IsPhotosListVisible = true;
        public bool IsPhotosListVisible
        {
            get { return _IsPhotosListVisible; }
            set
            {
                _IsPhotosListVisible = value;
                NotifyPropertyChanged();
            }
        }

        public string _ScannedCompareData;
        public string ScannedCompareData
        {
            get { return _ScannedCompareData; }
            set
            {
                _ScannedCompareData = value;
                RaisePropertyChanged("ScannedCompareData");
            }
        }

        public string _PageNextButton = "Link";
        public string PageNextButton
        {
            get { return _PageNextButton; }
            set
            {
                _PageNextButton = value;
                RaisePropertyChanged("PageNextButton");
            }
        }

        public AllPoData _ScannedAllPOData;
        public AllPoData ScannedAllPOData
        {
            get { return _ScannedAllPOData; }
            set
            {
                _ScannedAllPOData = value;
                RaisePropertyChanged("ScannedAllPOData");
            }
        }

        public string _ScannedValue;
        public string ScannedValue
        {
            get { return _ScannedValue; }
            set
            {
                _ScannedValue = value;
                RaisePropertyChanged("ScannedValue");
            }
        }

        public string _ScannedOn;
        public string ScannedOn
        {
            get { return _ScannedOn; }
            set
            {
                _ScannedOn = value;
                RaisePropertyChanged("ScannedOn");
            }
        }

        public double _LinkOpacity = 0.5;
        public double LinkOpacity
        {
            get { return _LinkOpacity; }
            set
            {
                _LinkOpacity = value;
                RaisePropertyChanged("LinkOpacity");
            }
        }

        public bool _IsLinkEnable;
        public bool IsLinkEnable
        {
            get { return _IsLinkEnable; }
            set
            {
                _IsLinkEnable = value;
                RaisePropertyChanged("IsLinkEnable");
            }
        }


        public double _DeleteAllOpacity = 0.5;
        public double DeleteAllOpacity
        {
            get { return _DeleteAllOpacity; }
            set
            {
                _DeleteAllOpacity = value;
                RaisePropertyChanged("DeleteAllOpacity");
            }
        }

        public bool _IsDeleteAllEnable;
        public bool IsDeleteAllEnable
        {
            get { return _IsDeleteAllEnable; }
            set
            {
                _IsDeleteAllEnable = value;
                RaisePropertyChanged("IsDeleteAllEnable");
            }
        }

        public string _StatusText;
        public string StatusText
        {
            get { return _StatusText; }
            set
            {
                _StatusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

        private Color _StatusTextBgColor = Color.Gray;
        public Color StatusTextBgColor
        {
            get => _StatusTextBgColor;
            set
            {
                _StatusTextBgColor = value;
                RaisePropertyChanged("StatusTextBgColor");
            }
        }

        public string _ScannedResult;
        public string ScannedResult
        {
            get { return _ScannedResult; }
            set
            {
                _ScannedResult = value;
                RaisePropertyChanged("ScannedResult");
            }
        }

        public bool _IsScanPage;
        public bool IsScanPage
        {
            get { return _IsScanPage; }
            set
            {
                _IsScanPage = value;
                RaisePropertyChanged("IsScanPage");
            }
        }

        public bool _CanOpenScan = true;
        public bool CanOpenScan
        {
            get { return _CanOpenScan; }
            set
            {
                _CanOpenScan = value;
                RaisePropertyChanged("CanOpenScan");
            }
        }



        public bool _IndicatorVisibility = false;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                RaisePropertyChanged("IndicatorVisibility");
            }
        }
        #endregion Preopeties
    }
}
