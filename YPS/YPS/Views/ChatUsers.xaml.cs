using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using YPS.ViewModel;
using static YPS.Models.ChatMessage;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatUsers : ContentPage
    {
        #region Data Members
        bool avoidLogicExcution = false;
        private ChatUsersViewModel vm;
        List<Tag> tag = new List<Tag>();
        bool visableState;
        YPSService service;
        bool checkInternet;
        int type;
        string chatClosedOrNot = "Open";
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="qaid"></param>
        /// <param name="tags"></param>
        /// <param name="checkStack"></param>
        public ChatUsers(int? poid, int? qaid, List<Tag> tags, bool checkStack)
        {
            YPSLogger.TrackEvent("ChatUsers.xaml.cs ", " Page constructor with 4 params " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                InitializeComponent();
                BindingContext = vm = new ChatUsersViewModel(Navigation, poid, qaid, checkStack, Settings.ChatTitle);

                tag = tags;
                visableState = checkStack;
                StartUser.IsVisible = false;
                Adduser.IsVisible = true;
                Startstack.IsVisible = false;
                if (Settings.HeaderTitle == "Close QA" || Settings.HeaderTitle == "Add/Remove user(s)")
                {
                    vm.IsGroupAndUserNameVisible = false;
                    vm.qasingletitle = true;
                    vm.qasingleheadertitle = Settings.HeaderTitle;
                    singleheader.Text = vm.qasingleheadertitle;
                    singleheader.IsVisible = true;
                    Settings.HeaderTitle = string.Empty;
                }
                else
                {
                    vm.IsGroupAndUserNameVisible = true;
                    vm.qasingletitle = false;
                    Title_entry.Text = Settings.ChatTitle;
                    Groupname.Text = Settings.ChatTitle;
                    Usernames.Text = Settings.tagnumbers;
                }

                Settings.currentPage = "chatPage";// Setting the current page as "chatPage" to the Settings property
                service = new YPSService();// Creating new instance of the YPSService, which is used to call API


                if (Settings.chatstatus == 0)
                {
                    titleupdate.IsVisible = false;
                    chatExitImg.IsVisible = false;
                    Settings.ChatClosedOrNot = chatClosedOrNot = "Closed";
                }
                else if (Settings.chatstatus == -1)
                {
                    titleupdate.IsVisible = false;
                    chatExitImg.IsVisible = false;
                    Settings.ChatClosedOrNot = chatClosedOrNot = "Archived";
                }
                else
                {
                    Settings.ChatClosedOrNot = chatClosedOrNot = "Open";
                }

                MessagingCenter.Subscribe<string>("ChatUsers", "BindUsers", (sender) =>
                {
                    Userlist.ItemsSource = vm.UserListCollections;
                });

                Userlist.ItemTapped += ListItemTapped; // Binding the ListItemTapped method to the event
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatUsers constructor with 4 params -> in ChatUsers.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="checkStack"></param>
        public ChatUsers(ChatData data, bool checkStack)
        {
            YPSLogger.TrackEvent("ChatUsers.xaml.cs ", " Page constructor with 2 params " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                InitializeComponent();

                Settings.PoId = data.POID;
                Settings.QaId = data.QAID;
                visableState = checkStack;
                tag = data.tags;
                Adduser.IsVisible = false;
                StartUser.IsVisible = true;
                Startstack.IsVisible = true;
                titleupdate.IsVisible = false;
                Settings.chatPageCount = 0;
                Settings.currentPage = "chatPage";// Setting the current page as "chatPage" to the Settings property
                service = new YPSService();// Creating new instance of the YPSService, which is used to call API
                BindingContext = vm = new ChatUsersViewModel(Navigation, data.POID, data.QAID, visableState, null);

                MessagingCenter.Subscribe<string>("ChatUsers", "BindUsers", (sender) =>
                {
                    ChatUserlist.ItemsSource = vm.UserListCollections;
                    MessagingCenter.Unsubscribe<string>("ChatUsers", "BindUsers");
                });

                Userlist.ItemTapped += ListItemTapped;
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatUsers constructor with 2 params -> in ChatUsers.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called while Adding/Removing user from existing conversation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListItemTapped(object sender, ItemTappedEventArgs e)
        {
            YPSLogger.TrackEvent("ChatUsers.xaml.cs ", " In ListItemTapped method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            NameIfo d = (NameIfo)e.Item;
            Userlist.SelectedItem = null;

            try
            {
                if (d?.IconColor == Color.Green || d?.IconColor == Color.Red)
                {
                    if (d?.AddRemoveIconOpacity == 1.0)
                    {
                        vm.IndicatorVisibility = true;
                        Userlist.SelectedItem = null;
                        BackgroundColor = Color.Transparent;

                        try
                        {
                            if (chatClosedOrNot == "Open")
                            {
                                vm.IndicatorVisibility = true;
                                BackgroundColor = Color.Transparent;
                                checkInternet = await App.CheckInterNetConnection();// Checking the internet connection

                                if (checkInternet)
                                {
                                    if (d.QAID != 0)
                                    {
                                        UserUpdating userupdate = new UserUpdating();
                                        Settings.QaId = d.QAID;
                                        userupdate.POID = Settings.PoId;
                                        userupdate.QAID = d.QAID;
                                        userupdate.RoleID = d.RoleID;
                                        userupdate.Status = d.Status;
                                        userupdate.UserCount = d.UserCount;
                                        userupdate.UserID = d.UserID;
                                        userupdate.UserName = d.UserName;
                                        userupdate.ISCurrentUser = d.ISCurrentUser;
                                        userupdate.CreatedBy = Settings.userLoginID;
                                        userupdate.FullName = d.FullName;
                                        userupdate.Title = Settings.ChatTitle;
                                        userupdate.QAType = Settings.QAType;

                                        if (d.IsAddStatus == false)
                                        {
                                            type = 1;
                                        }
                                        else if (d.IsAddStatus == true)
                                        {
                                            type = 2;
                                        }

                                        var result = await service.Updateuser(userupdate, type);// Calling the API to update the user(s)

                                        if (result?.status == 1)
                                        {
                                            vm.GetChatUser(Settings.PoId, d.QAID, Settings.QAType);
                                        }
                                    }
                                    else
                                    {
                                        await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                                        //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                                    }
                                }
                                else
                                {
                                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            YPSLogger.ReportException(ex, "ListItemTapped method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
                            await service.Handleexception(ex);
                            await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                            //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Action denied", "You don't have permission to do this action.", "Ok");
                        //DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ListItemTapped method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                vm.IndicatorVisibility = false;
            }
            vm.IndicatorVisibility = false;
        }

        /// <summary>
        /// Gets called while selecting/deselecting the user while creating new Q&A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBoxCheckChanged(object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            YPSLogger.TrackEvent("ChatUsers.xaml.cs ", " In CheckBoxCheckChanged method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                if (!avoidLogicExcution)
                {
                    avoidLogicExcution = true;
                    var userData = (SfCheckBox)sender;
                    int a = Convert.ToInt32(userData.ClassId);
                    string b = userData.Text;
                    var userObj = (NameIfo)userData.BindingContext;

                    if (vm.UserList.Count > 0)
                    {
                        bool has = vm.UserList.Any(cus => cus.UserID == a);

                        if (has)
                        {
                            userObj.UserChecked = false;
                            vm.UserList.Remove(vm.UserList.Single(x => x.UserID == a));
                        }
                        else
                        {
                            userObj.UserChecked = true;
                            vm.UserList.Add(new User() { Status = 1, UserID = a });
                        }
                    }
                    else
                    {
                        if (a != 0)
                        {
                            if (userData.IsChecked == false)
                            {
                                bool has = vm.UserList.Any(cus => cus.UserID == a);

                                if (has)
                                {
                                    userObj.UserChecked = false;
                                    vm.UserList.Remove(vm.UserList.Single(x => x.UserID == a));
                                }
                            }
                            else
                            {
                                userObj.UserChecked = true;
                                vm.UserList.Add(new User() { Status = 1, UserID = a });
                            }
                        }
                    }
                    avoidLogicExcution = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckBoxCheckChanged method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Start" button, to create a new Q&A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddToChatClicked(object sender, EventArgs e)
        {

            YPSLogger.TrackEvent("ChatUsers.xaml.cs ", " In AddToChatClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            vm.IndicatorVisibility = true;

            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    ChatData data = new ChatData();

                    bool has = vm.UserList.Any(cus => cus.UserID == Settings.userLoginID);

                    if (!has)
                    {
                        vm.UserList.Add(new User() { Status = 1, UserID = Settings.userLoginID });
                    }

                    if (vm.UserList.Count >= 1)
                    {
                        if (!string.IsNullOrEmpty(Title_entry.Text))
                        {
                            int bgCount = vm.UserList.Count;

                            try
                            {
                                data.Title = vm.GroupName;
                                data.users = vm.UserList;
                                data.tags = tag;
                                data.POID = Settings.PoId;
                                data.CreatedBy = Settings.userLoginID;
                                data.QAType = Settings.QAType;
                                var result = await service.ChatStart(data);

                                if (result != null)
                                {
                                    Settings.refreshPage = 1;

                                    if (result?.status == 1)
                                    {
                                        if (result.data.QAID == 0)
                                        {
                                            await Navigation.PopAsync(false);
                                            await App.Current.MainPage.DisplayAlert("Chat", "Conversation is not started, please try again.", "Ok");
                                        }
                                        else
                                        {
                                            vm.GroupName = result.data.Title;
                                            Settings.tagnumbers = result.data.TagNumbers;
                                            Settings.QaId = result.data.QAID;
                                            Settings.PoId = result.data.POID;
                                            Settings.chatgroupname = result.data.Title;
                                            Settings.ChatuserCountImgHide = 0;

                                            foreach (var items in data.tags)
                                            {
                                                if (items.TaskID != 0 && items.TagTaskStatus == 0)
                                                {
                                                    TagTaskStatus tagtaskstatus = new TagTaskStatus();
                                                    tagtaskstatus.TaskID = Helperclass.Encrypt(items.TaskID.ToString());
                                                    tagtaskstatus.POTagID = Helperclass.Encrypt(items.POTagID.ToString());
                                                    tagtaskstatus.Status = 1;
                                                    tagtaskstatus.CreatedBy = Settings.userLoginID;

                                                    var val = await service.UpdateTagTaskStatus(tagtaskstatus);

                                                    if (val?.status == 1)
                                                    {
                                                        if (items.TaskID != 0 && items.TaskStatus == 0)
                                                        {
                                                            TagTaskStatus taskstatus = new TagTaskStatus();
                                                            taskstatus.TaskID = Helperclass.Encrypt(items.TaskID.ToString());
                                                            taskstatus.TaskStatus = 1;
                                                            taskstatus.CreatedBy = Settings.userLoginID;

                                                            var taskval = await service.UpdateTaskStatus(taskstatus);
                                                        }
                                                    }
                                                }
                                            }

                                            await Navigation.PushAsync(new ChatPage(), false);
                                            Settings.mutipleTimeClick = false;
                                        }
                                    }
                                    vm.chatListCollections = result.data;
                                }
                            }
                            catch (Exception ex)
                            {
                                await service.Handleexception(ex);
                            }
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Input", "Please enter chat title.", "Ok");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Select", "Please select atleast one user.", "Ok");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "AddToChatClicked method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
            vm.IndicatorVisibility = false;
        }

        /// <summary>
        /// Getd called when clicked on cancelled button, to cancal the creation of new Q&A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelClicked1(object sender, EventArgs e)
        {
            YPSLogger.TrackEvent("ChatUsers.xaml.cs ", " In CancelClicked1 method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Settings.mutipleTimeClick = false;
                Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CancelClicked1 method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on back icon, to redirect to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackTapped(object sender, EventArgs e)
        {
            YPSLogger.TrackEvent("ChatUsers.xaml.cs ", " In AddToChatClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Settings.mutipleTimeClick = false;
                Navigation.PopAsync(false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BackTapped method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on Q&A Closed button, to close the particulat Q&A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void QnAClose(object sender, EventArgs e)
        {
            YPSLogger.TrackEvent("ChatUsers.xaml.cs ", " In QnAClose method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                vm.IndicatorVisibility = true;
                bool answer = await App.Current.MainPage.DisplayAlert("Close", "Are you sure?", "Yes", "No");

                if (answer)//Checking if selected "Yes,Close"
                {
                    checkInternet = await App.CheckInterNetConnection();// Checking if internet connection is there or not

                    if (checkInternet)
                    {
                        var result = await service.ConversationsClose(Settings.PoId, Settings.QaId, Settings.QAType);

                        if (result?.status == 1)
                        {
                            await App.Current.MainPage.DisplayAlert("Close", "QA closed Successfully.", "Ok");// Display message for success
                            await Navigation.PopToRootAsync(false);
                            //App.Current.MainPage = App.Current.MainPage = new MenuPage(typeof(HomePage));
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                        //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QnAClose method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                vm.IndicatorVisibility = false;
            }
            vm.IndicatorVisibility = false;
        }

        /// <summary>
        /// Gets called when clicked on the Home icon, to redirect to home(YPS/yShip) page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoToHomePageTapped(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PopToRootAsync(false);
                //App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GoToHomePageTapped method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        private void AddOrRemoveUsers_Tapped(object sender, EventArgs e)
        {
            try
            {
                YPSLogger.TrackEvent("ChatUsers.xaml.cs", " AddOrRemoveUsers_Tapped " + DateTime.Now + " UserId: " + Settings.userLoginID); YPSLogger.TrackEvent("ChatUsersViewModel", " QnACloseClick " + DateTime.Now + " UserId: " + Settings.userLoginID);

                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    vm.adduser = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatAddUsers".Trim()).FirstOrDefault()) != null ? true : false;
                    vm.removeuser = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatRemoveUsers".Trim()).FirstOrDefault()) != null ? true : false;
                }

                singleheader.Text = "Add/Remove user(s)";
                singleheader.IsVisible = true;
                vm.addchatUserStack = true;
                vm.QnACloseStack = false;
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "AddOrRemoveUsers_Tapped method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
            }
        }

        private async void CloseQA_Tapped(object sender, EventArgs e)
        {
            try
            {
                YPSLogger.TrackEvent("ChatUsers.xaml.cs", " CloseQA_Tapped " + DateTime.Now + " UserId: " + Settings.userLoginID);

                if (vm.QnACloseIconOpacity == 1.0)
                {
                    if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                    {
                        vm.adduser = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatAddUsers".Trim()).FirstOrDefault()) != null ? true : false;
                        vm.removeuser = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatRemoveUsers".Trim()).FirstOrDefault()) != null ? true : false;
                    }

                    vm.addchatUserStack = false;
                    vm.QnACloseStack = true;
                    singleheader.Text = "Close QA";
                    singleheader.IsVisible = true;
                    Settings.HeaderTitle = string.Empty;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Action denied", "You don't have permission to do this action.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "CloseQA_Tapped method -> in ChatUsers.xaml.cs " + Settings.userLoginID);
            }
        }
    }
}