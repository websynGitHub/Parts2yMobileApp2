using Syncfusion.XForms.Buttons;
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
using YPS.Parts2y.Parts2y_Views;
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
        public ICommand CheckedChangedCmd { set; get; }
        #endregion

        #region Declaring Data Members
        INavigation Navigation;
        YPSService service;
        bool checkInternet; public bool adduser = true, removeuser = true, CheckStack;
        #endregion

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="qaid"></param>
        /// <param name="checkstack"></param>
        public ChatUsersViewModel(INavigation _Navigation, int? poId, int? qaId, bool checkStack, string chatTitle)
        {
            YPSLogger.TrackEvent("ChatUsersViewModel", " constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Navigation = _Navigation;

                CheckStack = checkStack;

                #region Binnding the events to ICommand & creating new instance for data members
                Updatetitle = new Command(async () => await UpdateTitleClicked());
                QnACloseICmd = new Command(async () => await QnACloseClick());
                adduserICmd = new Command(async () => await AddUserClick());
                CheckedChangedCmd = new Command(CheckedChanged);
                service = new YPSService();// Creating new instance of the YPSService, which is used to call API
                UserList = new List<User>();
                GroupName = chatTitle;
                #endregion

                DynamicTextChange();
                GetChatUser(poId, qaId, Settings.QAType);// Get the users in the chat

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
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatUsersViewModel constructor -> in ChatUsersViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when checkbox is checked/unchecked, while creating new chat.
        /// </summary>
        /// <param name="sender"></param>
        private async void CheckedChanged(object sender)
        {
            YPSLogger.TrackEvent("ChatUsersViewModel.cs", "in CheckedChanged method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                var userData = sender as Plugin.InputKit.Shared.Controls.CheckBox;
                int a = Convert.ToInt32(userData.ClassId);
                string b = userData.Text;
                var userObj = (NameIfo)userData.BindingContext;

                if (UserList.Count > 0)
                {
                    bool has = UserList.Any(cus => cus.UserID == a);

                    if (has)
                    {
                        userObj.UserChecked = false;
                        UserList.Remove(UserList.Single(x => x.UserID == a));
                    }
                    else
                    {
                        userObj.UserChecked = true;
                        UserList.Add(new User() { Status = 1, UserID = a });
                    }
                }
                else
                {
                    if (a != 0)
                    {
                        if (userData.IsChecked == false)
                        {
                            bool has = UserList.Any(cus => cus.UserID == a);

                            if (has)
                            {
                                userObj.UserChecked = false;
                                UserList.Remove(UserList.Single(x => x.UserID == a));
                            }
                        }
                        else
                        {
                            userObj.UserChecked = true;
                            UserList.Add(new User() { Status = 1, UserID = a });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckedChanged method -> in ChatUsersViewModel.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            finally
            {
                IndicatorVisibility = false;
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
                qasingletitle = true;
                IsGroupAndUserNameVisible = false;
                qasingleheadertitle = "Add/Remove user(s)";
                addchatUserStack = true;
                QnACloseStack = false;
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "AddUserClick method -> in ChatUsersViewModel.cs " + Settings.userLoginID);
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
                qasingletitle = true;
                IsGroupAndUserNameVisible = false;
                qasingleheadertitle = "Close QA";
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "QnACloseClick method -> in ChatUsersViewModel.cs " + Settings.userLoginID);
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
                        if (UpdateBtnOpacity == 1.0)
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
                            DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
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
                YPSLogger.ReportException(ex, "UpdateTitleClicked method -> in ChatUsersViewModel.cs " + Settings.userLoginID);
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
                        if (result.status == 1 && result.data.Count > 0)
                        {
                            foreach (var item in result.data)
                            {
                                Settings.ChatUserCount = item.UserCount;

                                if (Settings.ChatClosedOrNot == "Closed")
                                {
                                    item.AddRemoveIconOpacity = (item.IsIconVisible = item.ISCurrentUser == 1 || item.Status == 1 ? true : false) == true ? 1.0 : 0.5;
                                    item.img = Icons.tickCircle;
                                    item.IconColor = item.ISCurrentUser == 1 ? Color.Blue : Color.BlueViolet;
                                    //item.IconColor = Color.FromHex("#269DC9");
                                }
                                else if (Settings.ChatClosedOrNot == "Archived")
                                {
                                    item.img = Icons.tickCircle;
                                    item.IconColor = Color.FromHex("#269DC9");
                                }
                                else
                                {
                                    if (item.Status == 1)
                                    {
                                        if (item.ISCurrentUser != 1)
                                        {
                                            if (removeuser == true)
                                            {
                                                item.AddRemoveIconOpacity = 1.0;
                                            }
                                            item.img = Icons.minusCircle;
                                            item.IsAddStatus = false;
                                            item.IconColor = Color.Red;
                                        }
                                        else
                                        {
                                            item.AddRemoveIconOpacity = 1.0;
                                            item.img = Icons.tickCircle;
                                            item.IconColor = item.ISCurrentUser == 1 ? Color.Blue : Color.BlueViolet;
                                        }
                                    }
                                    else if (item.Status == 0)
                                    {
                                        if (adduser == true)
                                        {
                                            item.AddRemoveIconOpacity = 1.0;
                                        }
                                        item.img = Icons.plusCircle;
                                        item.IsAddStatus = true;
                                        item.IconColor = Color.Green;

                                    }

                                    item.Iscurentuser = item.ISCurrentUser == 1 ? false : true;
                                    item.UserChecked = item.ISCurrentUser == 1 ? true : false;
                                    item.CheckBoxOpacity = item.ISCurrentUser == 1 ? 0.5 : 1;
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
                            await Navigation.PopToRootAsync(false);
                            //App.Current.MainPage = new MenuPage(typeof(HomePage));
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
                YPSLogger.ReportException(ex, "GetChatUser method -> in ChatUsersViewModel.cs " + Settings.userLoginID);
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

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    UpdateBtnOpacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatQATitleEdit".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    QnACloseIconOpacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatQAClose".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;
                    adduser = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatAddUsers".Trim()).FirstOrDefault()) != null ? true : false;
                    removeuser = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatRemoveUsers".Trim()).FirstOrDefault()) != null ? true : false;

                    if (CheckStack == true)
                    {
                        addchatUserStack = true;
                        QnACloseStack = false;

                        if (Settings.ChatuserCountImgHide == 1)
                        {
                            QnACloseIcon = false;
                            adduserIcon = false;
                        }
                    }
                    else if (CheckStack == false)
                    {
                        addchatUserStack = false;
                        QnACloseStack = true;
                    }
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "DynamicTextChange method -> in ChatUsersViewModel.cs " + Settings.userLoginID);
            }
        }
        #region Properties
        private double _UpdateBtnOpacity;
        public double UpdateBtnOpacity
        {
            get => _UpdateBtnOpacity;
            set
            {
                _UpdateBtnOpacity = value;
                OnPropertyChanged("UpdateBtnOpacity");
            }
        }

        private bool _IsGroupAndUserNameVisible;
        public bool IsGroupAndUserNameVisible
        {
            get => _IsGroupAndUserNameVisible;
            set
            {
                _IsGroupAndUserNameVisible = value;
                OnPropertyChanged("IsGroupAndUserNameVisible");
            }
        }

        private bool _qasingletitle;
        public bool qasingletitle
        {
            get => _qasingletitle;
            set
            {
                _qasingletitle = value;
                OnPropertyChanged("qasingletitle");
            }
        }

        private string _qasingleheadertitle;
        public string qasingleheadertitle
        {
            get => _qasingleheadertitle;
            set
            {
                _qasingleheadertitle = value;
                OnPropertyChanged("qasingleheadertitle");
            }
        }

        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                NotifyPropertyChanged("BgColor");
            }
        }

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
        public double _QnACloseIconOpacity = 1.0;
        public double QnACloseIconOpacity
        {
            get
            {
                return _QnACloseIconOpacity;
            }
            set
            {
                _QnACloseIconOpacity = value;
                OnPropertyChanged("QnACloseIconOpacity");
            }
        }

        public bool _QnACloseIcon = true;
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

        public bool _adduserIcon = true;
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
            public string closeQAStart { get; set; } = "LCMbtnClose";
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
