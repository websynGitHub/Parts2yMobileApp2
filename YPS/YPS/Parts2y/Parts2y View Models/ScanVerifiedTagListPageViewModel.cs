using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

namespace YPS.Parts2y.Parts2y_View_Models
{
    public class ScanVerifiedTagListPageViewModel : IBase
    {
        #region IComman and data members declaration
        public INavigation Navigation { get; set; }
        public ICommand MoveOrAssignAndMoveCmd { set; get; }

        YPSService service;
        ScanVerifiedTagListPage pagename;
        SendPodata sendPodata;
        public static int uploadType;
        ObservableCollection<AllPoData> AllPoDataList;
        #endregion

        public ScanVerifiedTagListPageViewModel(INavigation _Navigation, ScanVerifiedTagListPage page,
            ObservableCollection<AllPoData> matchedtaglist, int uploadtype)
        {
            try
            {
                YPSLogger.TrackEvent("ScanVerifiedTagListPageViewModel constructor", " ScanVerifiedTagListPageViewModel.cs " + DateTime.Now + " UserId: " + Settings.userLoginID);
                Navigation = _Navigation;
                pagename = page;
                uploadType = uploadtype;
                service = new YPSService();

                MoveOrAssignAndMoveCmd = new Command(MoveOrAssignAndMove);

                Task.Run(() => ShowContentsToLink(matchedtaglist));
                Task.Run(() => DynamicTextChange());
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ScanVerifiedTagListPageViewModel constructor -> in ScanVerifiedTagListPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }


        public async Task ShowContentsToLink(ObservableCollection<AllPoData> matchedtaglist)
        {
            YPSLogger.TrackEvent("ScanVerifiedTagListPageViewModel.cs", " in ShowContentsToLink method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                if (matchedtaglist != null && matchedtaglist.Count > 0)
                {
                    TagNumber = matchedtaglist[0]?.TagNumber;
                    IdentCode = matchedtaglist[0]?.IdentCode;

                    SelectedTagCountVisible = false;
                    IsPhotoUploadIconVisible = false;

                    var inspphoto = (Settings.VersionID == 2 && uploadType == 0) == true ? "Insp" : "Photo";

                    foreach (var values in matchedtaglist)
                    {
                        #region Status icon
                        if (values.TagTaskStatus == 0)
                        {
                            values.TagTaskStatusIcon = Icons.Pending;
                        }
                        else if (values.TagTaskStatus == 1)
                        {
                            values.TagTaskStatusIcon = Icons.Progress;
                        }
                        else
                        {
                            values.TagTaskStatusIcon = Icons.Done;
                        }
                        #endregion Status icon

                        if (values.TaskResourceID == 0 || values.TaskResourceID == null)
                        {
                            values.JobTileColor = Color.LightGray;
                        }

                        //values.IsTaskResourceVisible = values.TaskResourceID == Settings.userLoginID ? false : true;
                        values.StartTime = !string.IsNullOrEmpty(values.StartTime) ? Convert.ToDateTime(values.StartTime).ToString("HH:mm") : values.StartTime;
                        values.EndTime = !string.IsNullOrEmpty(values.EndTime) ? Convert.ToDateTime(values.EndTime).ToString("HH:mm") : values.EndTime;
                        values.IsTimeGiven = string.IsNullOrEmpty(values.StartTime) && string.IsNullOrEmpty(values.EndTime) ? false : true;
                        //values.IsTaskResourceVisible = values.TaskResourceID == 0 ? false : true;
                        values.IsTagDescLabelVisible = string.IsNullOrEmpty(values.IDENT_DEVIATED_TAG_DESC) ? false : true;
                        values.IsConditionNameLabelVisible = string.IsNullOrEmpty(values.ConditionName) ? false : true;
                        values.PhotoInspText = values.TaskResourceID == 0 ? "Assign & " + inspphoto : inspphoto;
                        values.PhotoInspLabelOpacity = values.IsPhotoRequired == 0 ? 0.4 : 1.0;
                    }

                    AllPoTagCollections = new ObservableCollection<AllPoData>(matchedtaglist);

                    NoRecordsLbl = (IsLinkButtonsVisible = AllPoTagCollections != null && AllPoTagCollections.Count > 0 ? true : false) == true ? false : true;
                }
                else
                {
                    NoRecordsLbl = true;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ShowContentsToLink method -> in ScanVerifiedTagListPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async void MoveOrAssignAndMove(object sender)
        {
            YPSLogger.TrackEvent("ScanVerifiedTagListPageViewModel.cs", " in MoveOrAssignAndMove method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                bool checkInternet = await App.CheckInterNetConnection();

                if (checkInternet == true)
                {
                    var podata = sender as AllPoData;
                    AllPoTagCollections?.ToList().ForEach(fe => { fe.SelectedTagBorderColor = Color.Transparent; });
                    podata.SelectedTagBorderColor = Settings.Bar_Background;
                    bool isalldone = AllPoTagCollections?.Where(wr => wr.TaskID == podata?.TaskID && wr.TagTaskStatus != 2).FirstOrDefault() == null ? true : false;

                    bool move = true;

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (podata.TaskResourceID == 0 || podata.TaskResourceID == null)
                        {

                            move = await App.Current.MainPage.DisplayAlert("Assign job", "Are you sure you want to take this job and proceed?", "Yes", "No");

                            if (move == true)
                            {
                                await AssignUnAssignedTask(podata);
                            }
                        }

                        if (move == true)
                        {
                            if (Settings.VersionID == 2 && uploadType == 0)
                            {
                                Settings.POID = podata.POID;
                                Settings.TaskID = podata.TaskID;

                                Device.BeginInvokeOnMainThread(async () =>
                                {
                                    IndicatorVisibility = true;
                                    await Navigation.PushAsync(new CVinInspectQuestionsPage(podata, isalldone), false);
                                });
                            }
                            else
                            {
                                #region PhotoUpload
                                if (podata.IsPhotoRequired == 0)
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Photos not required to upload for the selected " + VinsOrParts + ".");
                                }
                                else
                                {
                                    PhotoUploadModel selectedTagsData = new PhotoUploadModel();
                                    Settings.POID = podata.POID;
                                    Settings.TaskID = podata.TaskID;
                                    selectedTagsData.POID = podata.POID;
                                    selectedTagsData.isCompleted = podata.photoTickVisible;

                                    List<PhotoTag> lstdat = new List<PhotoTag>();

                                    if (podata.TagAPhotoCount == 0 && podata.TagBPhotoCount == 0 && podata.PUID == 0)
                                    {
                                        PhotoTag tg = new PhotoTag();

                                        if (podata.POTagID != 0)
                                        {
                                            tg.POTagID = podata.POTagID;
                                            tg.TagNumber = podata.TagNumber;
                                            tg.TaskID = podata.TaskID;
                                            tg.TaskStatus = podata.TaskStatus;
                                            tg.TagTaskStatus = podata.TagTaskStatus;
                                            Settings.Tagnumbers = podata.TagNumber;
                                            lstdat.Add(tg);

                                            selectedTagsData.photoTags = lstdat;
                                            Settings.currentPoTagId_Inti = lstdat;


                                            if (selectedTagsData.photoTags.Count != 0)
                                            {
                                                Device.BeginInvokeOnMainThread(async () =>
                                                {
                                                    IndicatorVisibility = true;
                                                    await Navigation.PushAsync(new PhotoUpload(selectedTagsData, podata, "initialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, false, isalldone, false), false);
                                                });
                                            }
                                        }
                                    }
                                    else
                                    {
                                        selectedTagsData.alreadyExit = "alreadyExit";

                                        if (podata.imgCamOpacityB != 0.5)
                                        {
                                            try
                                            {
                                                Settings.POID = podata.POID;
                                                Settings.TaskID = podata.TaskID;
                                                Settings.currentPuId = podata.PUID;
                                                Settings.BphotoCount = podata.TagBPhotoCount;

                                                Device.BeginInvokeOnMainThread(async () =>
                                                {
                                                    IndicatorVisibility = true;
                                                    await Navigation.PushAsync(new PhotoUpload(null, podata, "NotInitialPhoto", (int)UploadTypeEnums.GoodsPhotos_BP, podata.photoTickVisible, isalldone, false), false);
                                                });
                                            }
                                            catch (Exception ex)
                                            {
                                                YPSLogger.ReportException(ex, "tap_eachCamB method -> in POChildListPageViewModel " + Settings.userLoginID);
                                                await service.Handleexception(ex);
                                            }
                                        }
                                    }
                                }
                                #endregion PhotoUpload
                            }
                        }

                    });
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "MoveOrAssignAndMove method -> in ScanVerifiedTagListPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            finally
            {
                IndicatorVisibility = false;
            }
        }

        public async Task AssignUnAssignedTask(AllPoData podata)
        {
            try
            {
                IndicatorVisibility = true;

                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    YPSLogger.TrackEvent("ScanVerifiedTagListPageViewModel.cs", " in AssignUnAssignedTask method " + DateTime.Now + " UserId: " + Settings.userLoginID);

                    var val = await service.AssignUnassignedTask(podata.TaskID);

                    if (val?.status == 1)
                    {
                        podata.PhotoInspText = (Settings.VersionID == 2 && uploadType == 0) == true ? "Insp" : "Photo";
                        podata.JobTileColor = Color.White;
                        podata.TaskName = val.data;
                        podata.TaskResourceID = Settings.userLoginID;
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "AssignUnAssignedTask method -> in ScanVerifiedTagListPageViewModel.cs " + Settings.userLoginID);
                var trackResult = service.Handleexception(ex);
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
                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {
                        //Getting Label values & Status based on FieldID
                        var poid = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.POID.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var tagdesc = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.TagDesc.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        //var resource = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Resource.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var tagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var identcode = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.IdentCode.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var invoicenumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.InvoiceNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var conditionname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.ConditionName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var eventname = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.EventName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var starttime = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.StartTime.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var endtime = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.EndTime.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var shippingnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobj.ShippingNumber.Name.Trim().ToLower().Replace(" ", string.Empty)).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var barcode1 = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.Barcode1.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var bagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.BagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        //Assigning the Labels & Show/Hide the controls based on the data
                        labelobj.POID.Name = (poid != null ? (!string.IsNullOrEmpty(poid.LblText) ? poid.LblText : labelobj.POID.Name) : labelobj.POID.Name) + " :";
                        labelobj.POID.Status = poid?.Status == 1 || poid?.Status == 2 ? true : false;
                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme?.Status == 1 || taskanme?.Status == 2 ? true : false;
                        labelobj.TagDesc.Name = (tagdesc != null ? (!string.IsNullOrEmpty(tagdesc.LblText) ? tagdesc.LblText : labelobj.TagDesc.Name) : labelobj.TagDesc.Name) + " :";
                        labelobj.TagDesc.Status = tagdesc?.Status == 1 || tagdesc?.Status == 2 ? true : false;
                        labelobj.EventName.Name = (eventname != null ? (!string.IsNullOrEmpty(eventname.LblText) ? eventname.LblText : labelobj.EventName.Name) : labelobj.EventName.Name) + " :";
                        labelobj.EventName.Status = eventname?.Status == 1 || eventname?.Status == 2 ? true : false;
                        //labelobj.Resource.Name = (resource != null ? (!string.IsNullOrEmpty(resource.LblText) ? resource.LblText : labelobj.Resource.Name) : labelobj.Resource.Name) + " :";
                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber?.Status == 1 || tagnumber?.Status == 2 ? true : false;
                        labelobj.IdentCode.Name = (identcode != null ? (!string.IsNullOrEmpty(identcode.LblText) ? identcode.LblText : labelobj.IdentCode.Name) : labelobj.IdentCode.Name) + " :";
                        labelobj.IdentCode.Status = identcode?.Status == 1 || identcode?.Status == 2 ? true : false;
                        labelobj.ConditionName.Name = (conditionname != null ? (!string.IsNullOrEmpty(conditionname.LblText) ? conditionname.LblText : labelobj.ConditionName.Name) : labelobj.ConditionName.Name) + " :";
                        labelobj.ConditionName.Status = conditionname?.Status == 1 || conditionname?.Status == 2 ? true : false;
                        labelobj.InvoiceNumber.Name = (invoicenumber != null ? (!string.IsNullOrEmpty(invoicenumber.LblText) ? invoicenumber.LblText : labelobj.InvoiceNumber.Name) : labelobj.InvoiceNumber.Name) + " :";
                        labelobj.InvoiceNumber.Status = invoicenumber?.Status == 1 || invoicenumber?.Status == 2 ? true : false;
                        labelobj.StartTime.Name = (starttime != null ? (!string.IsNullOrEmpty(starttime.LblText) ? starttime.LblText : labelobj.StartTime.Name) : labelobj.StartTime.Name);
                        labelobj.StartTime.Status = starttime?.Status == 1 || starttime?.Status == 2 ? true : false;
                        labelobj.EndTime.Name = (endtime != null ? (!string.IsNullOrEmpty(endtime.LblText) ? endtime.LblText : labelobj.EndTime.Name) : labelobj.EndTime.Name);
                        labelobj.EndTime.Status = endtime?.Status == 1 || endtime?.Status == 2 ? true : false;
                        labelobj.ShippingNumber.Name = (shippingnumber != null ? (!string.IsNullOrEmpty(shippingnumber.LblText) ? shippingnumber.LblText : labelobj.ShippingNumber.Name) : labelobj.ShippingNumber.Name) + " :";
                        labelobj.ShippingNumber.Status = shippingnumber?.Status == 1 || shippingnumber?.Status == 2 ? true : false;
                        labelobj.Barcode1.Name = (barcode1 != null ? (!string.IsNullOrEmpty(barcode1.LblText) ? barcode1.LblText : labelobj.Barcode1.Name) : labelobj.Barcode1.Name) + " :";
                        labelobj.Barcode1.Status = barcode1?.Status == 1 || barcode1?.Status == 2 ? true : false;
                        labelobj.BagNumber.Name = (bagnumber != null ? (!string.IsNullOrEmpty(bagnumber.LblText) ? bagnumber.LblText : labelobj.BagNumber.Name) : labelobj.BagNumber.Name) + " :";
                        labelobj.BagNumber.Status = bagnumber?.Status == 1 || bagnumber?.Status == 2 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "DynamicTextChange method -> in ScanVerifiedTagListPageViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        public async Task<GetPoData> GetAllPOData()
        {
            GetPoData result = new GetPoData();
            try
            {
                sendPodata = new SendPodata();
                sendPodata.UserID = Settings.userLoginID;
                sendPodata.PageSize = Settings.pageSizeYPS;
                sendPodata.StartPage = Settings.startPageYPS;

                result = await service.LoadPoDataService(sendPodata);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetAllPOData method -> in ScanVerifiedTagListPageViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
            return result;
        }


        #region Properties
        #region Properties for dynamic label change
        public class LabelAndActionsChangeClass
        {
            public LabelAndActionFields POID { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "PONumber"
            };
            public LabelAndActionFields TagNumber { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "TagNumber"
            };
            public LabelAndActionFields TaskName { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "TaskName"
            };
            public LabelAndActionFields Resource { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "Resource"
            };
            public LabelAndActionFields EventName { get; set; } = new LabelAndActionFields
            {
                Status = false,
                Name = "Event"
            };

            public LabelAndActionFields TagDesc { get; set; } = new LabelAndActionFields
            {
                Status = false,  //Name = "TagDescription"
                Name = "IDENT_DEVIATED_TAG_DESC"
            };
            public LabelAndActionFields IdentCode { get; set; } = new LabelAndActionFields { Status = false, Name = "IdentCode" };
            public LabelAndActionFields ConditionName { get; set; } = new LabelAndActionFields { Status = false, Name = "ConditionName" };
            //public LabelAndActionFields InvoiceNumber { get; set; } = new LabelAndActionFields { Status = true, Name = "InvoiceNumber" };
            public LabelAndActionFields InvoiceNumber { get; set; } = new LabelAndActionFields { Status = false, Name = "Invoice1No" };
            public LabelAndActionFields StartTime { get; set; } = new LabelAndActionFields { Status = false, Name = "Start Time" };
            public LabelAndActionFields EndTime { get; set; } = new LabelAndActionFields { Status = false, Name = "End Time" };
            public LabelAndActionFields Barcode1 { get; set; } = new LabelAndActionFields { Status = false, Name = "Barcode1" };
            public LabelAndActionFields BagNumber { get; set; } = new LabelAndActionFields { Status = false, Name = "BagNumber" };

            public LabelAndActionFields ShippingNumber { get; set; } = new LabelAndActionFields { Status = false, Name = "Shipping Number" };
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

        public string _VinsOrParts = Settings.VersionID == 2 ? "VIN(s)" : "part(s)";
        public string VinsOrParts
        {
            get => _VinsOrParts;
            set
            {
                _VinsOrParts = value;
                RaisePropertyChanged("VinsOrParts");
            }
        }

        //private string _PhotoInspText { set; get; }
        //public string PhotoInspText
        //{
        //    get
        //    {
        //        return _PhotoInspText;
        //    }
        //    set
        //    {
        //        this._PhotoInspText = value;
        //        RaisePropertyChanged("PhotoInspText");
        //    }
        //}
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

        private AllPoData _SelectedTag;
        public AllPoData SelectedTag
        {
            get { return _SelectedTag; }
            set
            {
                _SelectedTag = value;
                RaisePropertyChanged("SelectedTag");

                //if (value != null)
                //{
                //    Task.Run(() => MoveOrAssignAndMove(value)).Wait();
                //}
            }
        }

        private string _TagNumber;
        public string TagNumber
        {
            get { return _TagNumber; }
            set
            {
                _TagNumber = value;
                RaisePropertyChanged("TagNumber");
            }
        }

        private string _IdentCode;
        public string IdentCode
        {
            get { return _IdentCode; }
            set
            {
                _IdentCode = value;
                RaisePropertyChanged("IdentCode");
            }
        }

        private bool _SelectedTagCountVisible { set; get; }
        public bool SelectedTagCountVisible
        {
            get
            {
                return _SelectedTagCountVisible;
            }
            set
            {
                this._SelectedTagCountVisible = value;
                RaisePropertyChanged("SelectedTagCountVisible");
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

        private bool _IsLinkButtonsVisible { set; get; }
        public bool IsLinkButtonsVisible
        {
            get
            {
                return _IsLinkButtonsVisible;
            }
            set
            {
                this._IsLinkButtonsVisible = value;
                RaisePropertyChanged("IsLinkButtonsVisible");
            }
        }

        private bool _NoRecordsLbl { set; get; }
        public bool NoRecordsLbl
        {
            get
            {
                return _NoRecordsLbl;
            }
            set
            {
                this._NoRecordsLbl = value;
                RaisePropertyChanged("NoRecordsLbl");
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

        private ObservableCollection<AllPoData> _AllPoTagCollections;
        public ObservableCollection<AllPoData> AllPoTagCollections
        {
            get { return _AllPoTagCollections; }
            set
            {
                _AllPoTagCollections = value;
                RaisePropertyChanged("AllPoTagCollections");
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
        #endregion Properties
    }
}
