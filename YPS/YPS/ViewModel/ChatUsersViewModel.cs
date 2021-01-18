using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Service;
using YPS.ViewModels;
using YPS.Views;
using static YPS.Models.ChatMessage;

namespace YPS.ViewModel
{
    public class ChatUsersViewModel : BaseViewModel
    {
        #region ICommand Declaration 
        public ICommand Updatetitle { get; set; }
        public ICommand QnACloseICmd { get; set; }
        public ICommand adduserICmd { get; set; }
        #endregion

        #region Declaring Data Members
        YPSService service;
        bool checkInternet;
        #endregion

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="qaid"></param>
        /// <param name="checkstack"></param>
        public ChatUsersViewModel(int? poId, int? qaId, bool checkStack, string chatTitle)
        {
            YPSLogger.TrackEvent("ChatUsersViewModel", " constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                #region Binnding the events to ICommand & creating new instance for data members
                Updatetitle = new Command(async () => await UpdateTitleClicked());
                QnACloseICmd = new Command(async () => await QnACloseClick());
                adduserICmd = new Command(async () => await AddUserClick());
                
                service = new YPSService();// Creating new instance of the YPSService, which is used to call API
                UserList = new List<User>();
                GetChatUser(poId, qaId, Settings.QAType);// Get the users in the chat
                GroupName = chatTitle;
                #endregion

                if (checkStack == true)
                {
                    addchatUserStack = true;
                    QnACloseStack = false;

                    if (Settings.ChatuserCountImgHide == 1)
                    {
                        QnACloseIcon = false;
                        adduserIcon = false;
                    }
                }
                else if (checkStack == false)
                {
                    addchatUserStack = false;
                    QnACloseStack = true;
                }

                DynamicTextChange();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatUsersViewModel constructor -> in ChatUsersViewModel " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on user icon with count, to redirect to update user and Q&A title page
        /// </summary>
        /// <returns></returns>
        private async Task AddUserClick()
        {
            YPSLogger.TrackEvent("ChatUsersViewModel", " AddUserClick " + DateTime.Now + " UserId: " + Settings.userLoginID); YPSLogger.TrackEvent("ChatUsersViewModel", " QnACloseClick " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                addchatUserStack = true;
                QnACloseStack = false;
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "AddUserClick method -> in ChatUsersViewModel " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on the box with tick icon in Q&A Close page,  that in to go to Q&A Close page 
        /// </summary>
        /// <returns></returns>
        private async Task QnACloseClick()
        {
            YPSLogger.TrackEvent("ChatUsersViewModel", " QnACloseClick " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                addchatUserStack = false;
                QnACloseStack = true;
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "QnACloseClick method -> in ChatUsersViewModel " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on Update button in QA Title textbox, for updating the Q&A title
        /// </summary>
        /// <returns></returns>
        private async Task UpdateTitleClicked()
        {
            YPSLogger.TrackEvent("ChatUsersViewModel", " UpdateTitleClicked " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    if (!string.IsNullOrEmpty(GroupName))
                    {
                        var result = await service.UpdateChatTitle(GroupName, Settings.QAType);

                        if (result != null && result.status == 0)
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "Update fail.", "Ok");
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "Success.", "Ok");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("YPS!", "Please enter chat title.", "Ok");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "UpdateTitleClicked method -> in ChatUsersViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// Gets called to get all the user(while creating new Q&A/updating users in existing Q&A)
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="qaid"></param>
        /// <param name="qatype"></param>
        public async void GetChatUser(int? poid, int? qaid, int qatype)
        {
            YPSLogger.TrackEvent("ChatUsersViewModel", "GetChatUser" + DateTime.Now + " UserId: " + Settings.userLoginID);
            await Task.Delay(20);
            IndicatorVisibility = true;

            try
            {
                checkInternet = await App.CheckInterNetConnection();// Check for the internet connection

                if (checkInternet)
                {
                    ChatData cdata = new ChatData();
                    var result = await service.GetChatusers(poid, qaid, qatype);

                    if (result != null)
                    {
                        if (result.status == 1)
                        {
                            foreach (var item in result.data)
                            {
                                Settings.ChatUserCount = item.UserCount;

                                //Checking for the type of users to display the Plus(add), Minus(remove), Ok icons and button
                                if (item.Status == 1 || item.ISCurrentUser == (int)UserRoles.SuperAdmin)
                                {
                                    if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.SuperUser || Settings.userRoleID == (int)UserRoles.SuperViewer
                                        || Settings.userRoleID == (int)UserRoles.SupplierAdmin
                                        || Settings.userRoleID == (int)UserRoles.SupplierUser || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.MfrAdmin
                                        || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser || Settings.userRoleID == (int)UserRoles.LogisticsAdmin
                                        || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                                    {
                                        item.img = "";
                                    }

                                    else if (Settings.userRoleID == (int)UserRoles.OwnerAdmin || Settings.userRoleID == (int)UserRoles.OwnerUser)
                                    {
                                        if (Settings.ChatClosedOrNot == "Closed")
                                        {
                                            item.img = Icons.tickCircle;
                                            item.IconColor = Color.FromHex("#269DC9");
                                        }
                                        else if (Settings.ChatClosedOrNot == "Archived")
                                        {
                                            item.img = Icons.tickCircle;
                                            item.IconColor = Color.FromHex("#269DC9");
                                        }
                                        else
                                        {
                                            item.img = Icons.minusCircle;
                                            item.checkType = false;
                                            item.IconColor = Color.Red;
                                        }
                                    }

                                    if (item.ISCurrentUser == (int)UserRoles.SuperAdmin)
                                    {
                                        item.Iscurentuser = false;
                                        item.check = true;
                                        item.img = Icons.tickCircle;
                                        item.IconColor = Color.BlueViolet;
                                    }
                                }
                                else
                                {
                                    if (Settings.userRoleID == (int)UserRoles.OwnerAdmin || Settings.userRoleID == (int)UserRoles.OwnerUser || Settings.userRoleID == (int)UserRoles.SupplierAdmin
                                        || Settings.userRoleID == (int)UserRoles.SupplierUser || Settings.userRoleID == (int)UserRoles.MfrUser || Settings.userRoleID == (int)UserRoles.MfrAdmin
                                        || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser || Settings.userRoleID == (int)UserRoles.LogisticsAdmin
                                        || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                                    {
                                        if (Settings.ChatClosedOrNot == "Closed")
                                        {
                                            item.img = "";
                                        }
                                        else if (Settings.ChatClosedOrNot == "Archived")
                                        {
                                            item.img = "";
                                        }
                                        else
                                        {
                                            item.img = Icons.plusCircle;
                                            item.checkType = true;
                                            item.IconColor = Color.Green;
                                        }
                                    }
                                }
                            }

                            UserListCollections = result.data;//Assigning the list of all users
                            UserList.Clear();

                            foreach (var items in UserListCollections)
                            {
                                if (items.Status == 1)// Assigning the users whose status is 1
                                {
                                    UserList.Add(new User() { Status = 1, UserID = items.UserID });
                                    GroupName = items.Title;
                                }
                            }

                            bgcount = UserList.Count.ToString();
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Chat", "No users avaliable to start chat.", "OK");
                            App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                        }
                    }
                    else
                    {
                    }
                    MessagingCenter.Send<string>("ChatUsers", "BindUsers");
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetChatUser method -> in ChatUsersViewModel " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            IndicatorVisibility = false;
        }

        /// <summary>
        /// This method is for dynamic text change.
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
                        labelobjdefval = new chatuserlabelchange();

                        var cancelQABtn = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjdefval.closeQAStart.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var startQABtn = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjdefval.startQA.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var updateUsersBtn = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjdefval.updateUsers.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var CloseQABtn = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjdefval.closeQA.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();
                        var qatitle = labelval.Where(wr => wr.FieldID.Trim().ToLower().Replace(" ", string.Empty) == labelobjdefval.qatitle.Trim().ToLower()).Select(c => c.LblText).FirstOrDefault();

                        labelobjdefval.closeQAStart = cancelQABtn != null ? (!string.IsNullOrEmpty(cancelQABtn) ? cancelQABtn : labelobjdefval.closeQAStart) : labelobjdefval.closeQAStart;
                        labelobjdefval.startQA = startQABtn != null ? (!string.IsNullOrEmpty(startQABtn) ? startQABtn : labelobjdefval.startQA) : labelobjdefval.startQA;
                        labelobjdefval.updateUsers = updateUsersBtn != null ? (!string.IsNullOrEmpty(updateUsersBtn) ? updateUsersBtn : labelobjdefval.updateUsers) : labelobjdefval.updateUsers;
                        labelobjdefval.closeQA = CloseQABtn != null ? (!string.IsNullOrEmpty(CloseQABtn) ? CloseQABtn : labelobjdefval.closeQA) : labelobjdefval.closeQA;
                        labelobjdefval.qatitle = qatitle != null ? (!string.IsNullOrEmpty(qatitle) ? qatitle : labelobjdefval.qatitle) : labelobjdefval.qatitle;
                    }
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DynamicTextChange method -> in ChatUsersViewModel " + Settings.userLoginID);
            }
        }

        #region Properties
        public string _GroupName;
        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                _GroupName = value;
                NotifyPropertyChanged();
            }
        }
        private ChatData _chatlist;
        public ChatData chatListCollections
        {
            get { return _chatlist; }
            set
            {
                _chatlist = value;
                NotifyPropertyChanged();
            }
        }
        public bool _IndicatorVisibility;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                NotifyPropertyChanged();
                OnPropertyChanged("IndicatorVisibility");
            }
        }
        private List<User> _UserList;
        public List<User> UserList
        {
            get { return _UserList; }
            set
            {
                _UserList = value;
                OnPropertyChanged("UserList");
            }
        }
        private ObservableCollection<NameIfo> _Userlist;
        public ObservableCollection<NameIfo> UserListCollections
        {
            get { return _Userlist; }
            set
            {
                _Userlist = value;
                NotifyPropertyChanged("UserListCollections");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string _bgcount;
        public string bgcount
        {
            get { return _bgcount; }
            set
            {
                _bgcount = value;
                OnPropertyChanged("bgcount");
            }
        }
        private bool _addchatUserStack = false;
        public bool addchatUserStack
        {
            get
            {
                return _addchatUserStack;
            }
            set
            {
                _addchatUserStack = value;
                //NotifyPropertyChanged("addchatUserStack");
                OnPropertyChanged("addchatUserStack");
            }
        }
        private bool _QnACloseStack = false;
        public bool QnACloseStack
        {
            get
            {
                return _QnACloseStack;
            }
            set
            {
                _QnACloseStack = value;
                OnPropertyChanged("QnACloseStack");
            }
        }
        private bool _QnACloseIcon = true;
        public bool QnACloseIcon
        {
            get
            {
                return _QnACloseIcon;
            }
            set
            {
                _QnACloseIcon = value;
                OnPropertyChanged("QnACloseIcon");
            }
        }
        private bool _adduserIcon = true;
        public bool adduserIcon
        {
            get
            {
                return _adduserIcon;
            }
            set
            {
                _adduserIcon = value;
                OnPropertyChanged("adduserIcon");
            }
        }

        #region Assigning values to dynamic text change properties for comparing with FieldID
        public class chatuserlabelchange
        {
            public string closeQA { get; set; } = "QAClose";
            public string updateUsers { get; set; } = "Update";
            public string startQA { get; set; } = "Start";
            public string closeQAStart { get; set; } = "Close";
            public string qatitle { get; set; } = "QATitle";
        }

        private chatuserlabelchange _labelobjdefval = new chatuserlabelchange();
        public chatuserlabelchange labelobjdefval
        {
            get => _labelobjdefval;
            set
            {
                _labelobjdefval = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #endregion
    }
}
