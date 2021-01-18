using Syncfusion.XForms.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
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
            YPSLogger.TrackEvent("ChatUsers", " Page constructor with 4 params " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                InitializeComponent();

                tag = tags;
                visableState = checkStack;
                StartUser.IsVisible = false;
                Adduser.IsVisible = true;
                Startstack.IsVisible = false;
                Title_entry.Text = Settings.ChatTitle;
                Groupname.Text = Settings.ChatTitle;
                Usernames.Text = Settings.tagnumbers;
                Settings.currentPage = "chatPage";// Setting the current page as "chatPage" to the Settings property
                service = new YPSService();// Creating new instance of the YPSService, which is used to call API
                BindingContext = vm = new ChatUsersViewModel(poid, qaid, checkStack, Settings.ChatTitle);

                if (Settings.chatstatus == 0)
                {
                    titleupdate.IsVisible = false;
                    chatExitImg.IsVisible = false;
                    Settings.ChatClosedOrNot = chatClosedOrNot = "Closed";
                }
                else if(Settings.chatstatus== -1)
                {
                    titleupdate.IsVisible = false;
                    chatExitImg.IsVisible = false;
                    Settings.ChatClosedOrNot = chatClosedOrNot = "Archived";
                }
                else
                {
                    titleupdate.IsVisible = true;
                    Settings.ChatClosedOrNot = chatClosedOrNot = "Open";
                }

                if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.SuperUser || Settings.userRoleID == (int)UserRoles.SuperViewer ||
                    Settings.userRoleID == (int)UserRoles.SupplierAdmin || Settings.userRoleID == (int)UserRoles.SupplierUser || Settings.userRoleID == (int)UserRoles.MfrUser
                    || Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser
                    || Settings.userRoleID == (int)UserRoles.LogisticsAdmin || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin
                    || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                {
                    chatExitImg.IsVisible = false;
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
                YPSLogger.ReportException(ex, "ChatUsers constructor with 4 params -> in ChatUsers Page.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="checkStack"></param>
        public ChatUsers(ChatData data, bool checkStack)
        {
            YPSLogger.TrackEvent("ChatUsers", " Page constructor with 2 params " + DateTime.Now + " UserId: " + Settings.userLoginID);
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
                BindingContext = vm = new ChatUsersViewModel(data.POID, data.QAID, visableState, null);


                if (Settings.userRoleID == (int)UserRoles.SuperAdmin || Settings.userRoleID == (int)UserRoles.SuperUser || Settings.userRoleID == (int)UserRoles.SuperViewer ||
                    Settings.userRoleID == (int)UserRoles.SupplierAdmin || Settings.userRoleID == (int)UserRoles.SupplierUser || Settings.userRoleID == (int)UserRoles.MfrUser
                    || Settings.userRoleID == (int)UserRoles.MfrAdmin || Settings.userRoleID == (int)UserRoles.DealerAdmin || Settings.userRoleID == (int)UserRoles.DealerUser
                    || Settings.userRoleID == (int)UserRoles.LogisticsAdmin || Settings.userRoleID == (int)UserRoles.LogisticsUser || Settings.userRoleID == (int)UserRoles.TruckingAdmin
                    || Settings.userRoleID == (int)UserRoles.TruckingDriver)
                {
                    chatExitImg.IsVisible = false;
                }

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
                YPSLogger.ReportException(ex, "ChatUsers constructor with 2 params -> in ChatUsers Page.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called while Adding/Removing user from existing conversation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListItemTapped(object sender, ItemTappedEventArgs e)
        {
            YPSLogger.TrackEvent("ChatUsers", " In ListItemTapped method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            NameIfo d = (NameIfo)e.Item;
            Userlist.SelectedItem = null;
           
            try
            {
                if (!string.IsNullOrEmpty(d.img.Trim().ToLower()) || d.img.Trim().ToLower() != "ok.png")
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

                                    if (d.img.Trim().ToLower() == "minusic.png")
                                    {
                                        type = 1;
                                    }
                                    else if (d.img.Trim().ToLower() == "plusic.png")
                                    {
                                        type = 2;
                                    }
                                    if (d.checkType == false)
                                    {
                                        type = 1;
                                    }
                                    else if (d.checkType == true)
                                    {
                                        type = 2;
                                    }

                                    var result = await service.Updateuser(userupdate, type);// Calling the API to update the user(s)

                                    if (result != null && result.status == 1)
                                    {
                                        vm.GetChatUser(Settings.PoId, d.QAID, Settings.QAType);
                                    }
                                }
                                else
                                {
                                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                                }
                            }
                            else
                            {
                                DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        YPSLogger.ReportException(ex, "ListItemTapped method -> in ChatUsers Page.cs " + Settings.userLoginID);
                        await service.Handleexception(ex);
                        vm.IndicatorVisibility = false;
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                    vm.IndicatorVisibility = false;
                }
                else
                {
                    vm.IndicatorVisibility = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ListItemTapped method -> in ChatUsers Page.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                vm.IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called while selecting/deselecting the user while creating new Q&A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CheckBoxCheckChanged(object sender, Syncfusion.XForms.Buttons.StateChangedEventArgs e)
        {
            YPSLogger.TrackEvent("ChatUsers", " In CheckBoxCheckChanged method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                if (!avoidLogicExcution)
                {
                    avoidLogicExcution = true;
                    var userData = (SfCheckBox)sender;
                    int a = Convert.ToInt32(userData.ClassId);
                    string b = userData.Text;

                    if (vm.UserList.Count > 0)
                    {
                        bool has = vm.UserList.Any(cus => cus.UserID == a);

                        if (has)
                        {
                            vm.UserList.Remove(vm.UserList.Single(x => x.UserID == a));
                        }
                        else
                        {
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
                                    vm.UserList.Remove(vm.UserList.Single(x => x.UserID == a));
                                }
                            }
                            else
                            {
                                vm.UserList.Add(new User() { Status = 1, UserID = a });
                            }
                        }
                    }
                    avoidLogicExcution = false;
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CheckBoxCheckChanged method -> in ChatUsers Page.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Start Chat" button, to create a new Q&A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddToChatClicked(object sender, EventArgs e)
        {

            YPSLogger.TrackEvent("ChatUsers", " In AddToChatClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            vm.IndicatorVisibility = true;

            try
            {
                ChatData data = new ChatData();

                if (Settings.userRoleID != (int)UserRoles.CustomerAdmin)
                {
                    bool has = vm.UserList.Any(cus => cus.UserID == Settings.userLoginID);

                    if (!has)
                    {
                        vm.UserList.Add(new User() { Status = 1, UserID = Settings.userLoginID });
                    }
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

                                if (result.status != 0)
                                {
                                    if (result.data.QAID == 0)
                                    {
                                        await Navigation.PopAsync();
                                        await App.Current.MainPage.DisplayAlert("Alert", "Conversation is not started, please try again.", "Ok");
                                    }
                                    else
                                    {
                                        vm.GroupName = result.data.Title;
                                        Settings.tagnumbers = result.data.TagNumbers;
                                        Settings.QaId = result.data.QAID;
                                        Settings.PoId = result.data.POID;
                                        Settings.chatgroupname = result.data.Title;
                                        Settings.ChatuserCountImgHide = 0;
                                        App.Current.MainPage = new YPSMasterPage(typeof(ChatPage));
                                        Settings.mutipleTimeClick = false;
                                    }
                                }
                                vm.chatListCollections = result.data;
                            }
                            else
                            {
                                //   DependencyService.Get<IToastMessage>().ShortAlert(result.message);
                            }
                        }
                        catch (Exception ex)
                        {
                            await service.Handleexception(ex);
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("YPS!", "Please enter chat title.", "Ok");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("YPS!", "Please select atleast one user.", "Ok");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "AddToChatClicked method -> in ChatUsers Page.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("ChatUsers", " In CancelClicked1 method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Settings.mutipleTimeClick = false;
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "CancelClicked1 method -> in ChatUsers Page.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("ChatUsers", " In AddToChatClicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Settings.mutipleTimeClick = false;
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BackTapped method -> in ChatUsers Page.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("ChatUsers", " In QnAClose method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                vm.IndicatorVisibility = true;
                bool answer = await App.Current.MainPage.DisplayAlert("Close", "Are you sure?", "Yes,Close", "No");

                if (answer)//Checking if selected "Yes,Close"
                {
                    checkInternet = await App.CheckInterNetConnection();// Checking if internet connection is there or not

                    if (checkInternet)
                    {
                        var result = await service.ConversationsClose(Settings.PoId, Settings.QaId, Settings.QAType);

                        if (result != null && result.status == 1)
                        {
                            await App.Current.MainPage.DisplayAlert("Completed", "Success.", "Close");// Display message for success
                            App.Current.MainPage = Settings.CheckQnAClose == true ? new YPSMasterPage(typeof(YshipPage)) : App.Current.MainPage = new YPSMasterPage(typeof(MainPage));
                        }
                        else
                        {
                            // DependencyService.Get<IToastMessage>().ShortAlert(result.message);
                        }
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QnAClose method -> in ChatUsers Page.cs " + Settings.userLoginID);
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
                await Navigation.PopToRootAsync();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GoToHomePageTapped method -> in ChatUsers Page.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }
    }
}