using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.CustomRenders;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Model;
using YPS.Parts2y.Parts2y_Views;
using YPS.Service;
using YPS.ViewModels;
using static YPS.Models.ChatMessage;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : MyContentPage
    {
        #region Data Members
        public string chatTitle, chatTags;
        int poId, qaId;
        private ChatViewModel vm;
        YPSService service = new YPSService();// Creating new instance of the YPSService, which is used to call AIP
        ChatData poData = new ChatData();// Creating new instance of the ChatData
        List<NameIfo> chatData = new List<NameIfo>();// Creating new instance of the NameIfo list
        List<User> userList = new List<User>();// Creating new instance of the User list
        List<Tag> tgs = new List<Tag>();// Creating new instance of the Tag list
        ObservableCollection<ChatMessageViewModel> chatDataOC = new ObservableCollection<ChatMessageViewModel>();// Creating new instance of the ChatMessageViewModel Observable
        #endregion

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public ChatPage()
        {
            YPSLogger.TrackEvent("ChatPage", " Page constructor without params " + DateTime.Now + " UserId: " + Settings.userLoginID);
            try
            {
                InitializeComponent();
                Settings.currentPage = "chatPage";// Setting the current page as "chatPage"
                Backchathide.IsVisible = Settings.IsChatBackButtonVisible;

                if (!string.IsNullOrEmpty(Settings.GetParamVal))
                {
                    var navPages = Settings.GetParamVal.Split(';');
                    string chatstatus = navPages[5]; poId = Convert.ToInt32(navPages[1]);
                    qaId = Convert.ToInt32(navPages[4]);
                    chatTitle = navPages[7]; chatTags = navPages[2];
                    Settings.PoId = Convert.ToInt32(navPages[1]); Settings.QaId = Settings.currentChatPage = Convert.ToInt32(navPages[4]);
                    Settings.tagnumbers = navPages[2]; Settings.ChatTitle = navPages[7];
                    Settings.QAType = Convert.ToInt32(navPages[8]);
                    Settings.chatstatus = Convert.ToInt32(chatstatus);

                    if (chatstatus == "0" || chatstatus == "-1")
                    {
                        MessageEntry.IsVisible = false;
                        btnchatexit.IsVisible = false;
                    }

                    if (Settings.QaId == 0 || Settings.QaId == -1)
                    {
                        btnchatexit.IsVisible = false;
                        addremoveusercol.Width = 0;
                    }

                    Settings.GetParamVal = string.Empty;
                }
                else
                {
                    poId = Settings.PoId;
                    qaId = Settings.QaId;
                    chatTitle = Settings.chatgroupname;
                    chatTags = Settings.tagnumbers;
                    Settings.ChatTitle = Settings.chatgroupname;
                    Settings.chatstatus = 1;
                }

                ChatStack.IsVisible = true;
                Groupname.Text = chatTitle;
                Usernames.Text = chatTags;
                BindingContext = vm = new ChatViewModel(qaId, poId, this);
                Thread threadConnectiongroupchat = new Thread(vm.Connect);
                threadConnectiongroupchat.Start();
                GetChatData();
                bg.BadgeText = (Settings.ChatUserCount = vm.bgcount).ToString();
                vm.IsEmailenable = Settings.IsEmailEnabled;
                //DependencyService.Get<ISQLite>().deleteReadCountNmsg(qaId);
                CheckUserAndOS();

                ShowHideActions();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatPage without constructor -> in ChatPage.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="qaid"></param>
        /// <param name="poid"></param>
        /// <param name="tagnames"></param>
        /// <param name="title"></param>
        /// <param name="tags"></param>
        /// <param name="chatstatus"></param>
        public ChatPage(int qaid, int poid, string tagnames, string title, List<Tag> tags, int chatstatus)
        {
            YPSLogger.TrackEvent("ChatPage.xaml.cs ", " Page constructor with params " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                InitializeComponent();

                Settings.currentPage = "chatPage";
                BindingContext = vm = new ChatViewModel(qaid, poid, this);
                Thread threadConnectiongroupchat = new Thread(vm.Connect);
                threadConnectiongroupchat.Start();
                GetChatData();// Getting all the chat related data 
                ChatStack.IsVisible = true;
                Groupname.Text = title;
                Usernames.Text = tagnames;
                poId = poid;
                tgs = tags;
                qaId = qaid;
                chatTitle = title;
                chatTags = tagnames;
                Settings.PoId = poid;
                Settings.QaId = Settings.currentChatPage = qaid;
                Settings.ChatTitle = title;
                Settings.tagnumbers = tagnames;
                Settings.chatstatus = chatstatus;
                DependencyService.Get<ISQLite>().deleteReadCountNmsg(qaid);
                CheckUserAndOS();// Checking the user and OS of device
                vm.IsEmailenable = Settings.IsEmailEnabled;

                if (chatstatus == 0 || chatstatus == -1)
                {
                    MessageEntry.IsVisible = false;
                    btnchatexit.IsVisible = false;
                }

                if (Settings.QaId == 0 || Settings.QaId == -1)
                {
                    btnchatexit.IsVisible = false;
                    addremoveusercol.Width = 0;
                }

                MessagingCenter.Send<string, int>("ChatPage", "ChatCount", qaid);

                Task.Run(() => ShowHideActions()).Wait();
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChatPage with constructor -> in ChatPage.xaml.cs " + Settings.userLoginID);
            }
        }

        private async Task ShowHideActions()
        {
            try
            {
                if (Settings.AllActionStatus != null && Settings.AllActionStatus.Count > 0)
                {
                    btnchatexit.Opacity = (Settings.AllActionStatus.Where(wr => wr.ActionCode.Trim() == "ChatQAClose".Trim()).FirstOrDefault()) != null ? 1.0 : 0.5;

                    if (Settings.chatstatus == 0 || Settings.chatstatus == -1)
                    {
                        MessageEntry.IsVisible = false;
                        btnchatexit.IsVisible = false;
                    }

                    if (Settings.QaId == 0 || Settings.QaId == -1)
                    {
                        btnchatexit.IsVisible = false;
                        addremoveusercol.Width = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ShowHideActions with method -> in ChatPage.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// To check the user using this application & the device on which it is used.
        /// </summary>
        private void CheckUserAndOS()
        {
            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        vm.BaseOnPlatformEditorShowAndroid = true;
                        break;
                    case Device.iOS:
                        vm.BaseOnPlatformEditorShowiOS = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                service.Handleexception(ex);
                YPSLogger.ReportException(ex, "CheckUserAndOS method -> in ChatPage.xaml.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked on the any Q&A from Q&A list, to go its conversation page
        /// </summary>
        public async void GetChatData()
        {
            YPSLogger.TrackEvent("ChatPage.xaml.cs ", " In GetChatData method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                var data = await vm.GetChatHistory(Settings.QaId);// Get the chat history

                if (data != null)
                {
                    messageList.ItemsSource = data;

                    if (data.Count > 0)
                    {
                        var lastgroup = data[data.Count - 1];

                        if (lastgroup != null)
                        {
                            Device.StartTimer(TimeSpan.FromMilliseconds(200), () =>
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    messageList.ScrollTo(lastgroup.LastOrDefault(), lastgroup, ScrollToPosition.MakeVisible, false);
                                });
                                return false;
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetChatData method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                vm.IndicatorVisibility = false;
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when a new message is about to show in conversation, for scrolling the window and show new message
        /// </summary>
        public async void Scroll()
        {
            YPSLogger.TrackEvent("ChatPage", " In Scroll method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    messageList.ItemsSource = vm.Chat1;

                    if (vm.Chat1.Count > 0)
                    {
                        int count = vm.Chat1.Count;

                        if (vm.Chat1[count - 1] != null)
                        {
                            messageList.ScrollTo(vm.Chat1[count - 1].LastOrDefault(), vm.Chat1[count - 1], ScrollToPosition.End, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    YPSLogger.ReportException(ex, "Scroll method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                    await service.Handleexception(ex);
                }
            });
        }

        /// <summary>
        /// Gets called when the page is loading
        /// </summary>
        protected async override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                Settings.chatPageCount = 0;
                bg.IsVisible = Settings.QaId > 0 ? true : false;

                if (Settings.ChatUserCount == 0)
                {
                    bg.BadgeText = vm != null ? vm.bgcount.ToString() : "0";
                }
                else
                {
                    bg.BadgeText = Settings.ChatUserCount.ToString();
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when any message is tapped, genearlly used to view the image in the conversation 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MessageListItemSelected(object sender, ItemTappedEventArgs e)
        {
            vm.IndicatorVisibility = true;
            try
            {

                messageList.SelectedItem = null;
                var items = e.Item as ChatMessageViewModel;

                if (items != null)
                {
                    if (items.MessageType.Trim().ToLower() == "p")
                    {
                        await vm.GetPhotoData(Settings.QaId, Settings.PhotoOption);

                        if (Navigation.NavigationStack.Count == 1 ||
                                     Navigation.NavigationStack.Last().GetType() != typeof(FileView))
                        {
                            await Navigation.PushAsync(new FileView(items, vm.photoList, chatTitle, chatTags), false);
                        }
                    }
                    else if (items.MessageType.Trim().ToLower() == "pdf")
                    {
                        if (Navigation.NavigationStack.Count == 1 ||
                                    Navigation.NavigationStack.Last().GetType() != typeof(FileView))
                        {
                            await Navigation.PushAsync(new FileView(items.Image), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "MessageListItemSelected method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                vm.IndicatorVisibility = false;
            }
            vm.IndicatorVisibility = false;
        }

        /// <summary>
        /// Gets called when the user is about to exit the page.
        /// </summary>
        protected async override void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();
                Settings.currentChatPage = 0;

                var result = await service.ReadNotifyHistory(qaId, Settings.userLoginID);
                DependencyService.Get<ISQLite>().deleteReadCountNmsg(qaId);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnDisappearing method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on User icon with count, to Add/Remove user(s).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddUserClicked(object sender, EventArgs e)
        {
            vm.IndicatorVisibility = true;

            try
            {
                Settings.HeaderTitle = "Add/Remove user(s)";
                await Navigation.PushAsync(new ChatUsers(Settings.PoId, Settings.QaId, poData.tags, true), false);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "AddUserClicked method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                vm.IndicatorVisibility = false;
            }
            vm.IndicatorVisibility = false;
        }

        /// <summary>
        /// Gets called when clicked on the icon that is for QAClose, to close the Q&A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void QnAClose(object sender, EventArgs e)
        {
            try
            {
                if (btnchatexit.Opacity == 1.0)
                {
                    Settings.HeaderTitle = "Close QA";
                    await Navigation.PushAsync(new ChatUsers(Settings.PoId, Settings.QaId, poData.tags, false), false);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Action denied", "You don't have permission to do this action.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("You don't have permission to do this action.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QnAClose method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when ckicked on Home icon, to redirect to Home.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GoToHomeTapped(object sender, EventArgs e)
        {
            try
            {
                if (App.Current.MainPage.GetType().Name.Trim().ToLower() != "HomePage".Trim().ToLower())
                {
                    App.Current.MainPage = new MenuPage(typeof(HomePage));
                }
                else
                {
                    await Navigation.PopToRootAsync(false);
                }
                //App.Current.MainPage = new MenuPage(typeof(HomePage));
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GoToHomeTapped method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called on when clicked on the refresh icon, to update the conversation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTap(object sender, EventArgs e)
        {
            try
            {
                vm.IndicatorVisibility = true;

                if (App.CheckSignalRConnection())
                {
                    GetChatData();
                }
                else
                {
                    DisplayAlert("Information", "No internet connection.", "ok");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "RefreshTap method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
                vm.IndicatorVisibility = false;
            }
            vm.IndicatorVisibility = false;
        }

        /// <summary>
        /// Gets called when clicked on the back icon, to go back to the previous page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackTapped(object sender, EventArgs e)
        {
            try
            {
                if (Settings.RedirectPageQA.Trim().ToLower() == "qnalistpage")
                {
                    Navigation.PopAsync(false);
                    Settings.RedirectPageQA = "";
                }
                else
                {
                    Settings.RedirectPagefirsttime = "NewAdded";
                    Navigation.PopAsync(false);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "BackTapped method -> in ChatPage.xaml.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
    }
}