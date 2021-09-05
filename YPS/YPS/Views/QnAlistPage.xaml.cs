using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using YPS.CommonClasses;
using YPS.Helpers;
using YPS.Service;
using YPS.ViewModel;
using static YPS.Models.ChatMessage;

namespace YPS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QnAlistPage : ContentPage
    {
        #region Data members declaration
        int count, pId, pTagId, qaType;
        YPSService service;
        private QnAlistPageViewModel vm;
        #endregion

        /// <summary>
        /// Parameterized constructor.
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="poTagId"></param>
        /// <param name="qatype"></param>
        public QnAlistPage(int poid, int poTagId, int qatype)
        {
            YPSLogger.TrackEvent("QnAlistPage", "Page with param Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                InitializeComponent();

                //if (Device.RuntimePlatform == Device.iOS)
                //{
                //    var safeAreaInset = On<Xamarin.Forms.PlatformConfiguration.iOS>().SafeAreaInsets();
                //    safeAreaInset.Bottom = 0;
                //    safeAreaInset.Top = 20;
                //    headerpart.Padding = safeAreaInset;
                //}
                Settings.currentPage = "QnAlistPage";// Giving Current page name
                Settings.RedirectPageQA = "QnAlistPage";// Giving Redirect to page name
                service = new YPSService();// careating new instance of the YPSService, which is used to call AIP 
                count = 1;
                pId = poid;
                pTagId = poTagId;
                qaType = qatype;
                BindingContext = vm = new QnAlistPageViewModel(poid, poTagId, qatype);// Creating new instance of view model & binding events through BindingContext
                MessageCenterSubscribe();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QnAlistPage constructor having parameter -> in QnAlistPage.cs " + Settings.userLoginID);
                var trackResult = service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public QnAlistPage()
        {
            YPSLogger.TrackEvent("QnAlistPage", "Page without param Constructor " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                InitializeComponent();

                service = new YPSService();// careating new instance of the YPSService, which is used to call AIP
                Settings.currentPage = "QnAlistPage";
                Settings.RedirectPageQA = "QnAlistPage";
                Title = "Q&A List";
                var navPages = Settings.GetParamVal.Split(',');
                pId = Convert.ToInt32(navPages[1]);
                pTagId = Convert.ToInt32(navPages[2]);
                qaType = Settings.QAType;
                BindingContext = vm = new QnAlistPageViewModel(Convert.ToInt32(navPages[1]), Convert.ToInt32(navPages[2]), Settings.QAType);// Creating new instance of view model & binding events through BindingContext

                MessageCenterSubscribe();// Method from message center subscription
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "QnAlistPage without param constructor -> in QnAlistPage.cs " + Settings.userLoginID);
                var trackResult = service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is to subscribe MessageCenter.
        /// </summary>
        private void MessageCenterSubscribe()
        {
            try
            {
                MessagingCenter.Subscribe<string>("ChatconverPage", "Bindchatconvers", (sender) =>
                {
                    //chatUserList.ItemsSource = vm.UserConversations;

                    //if (chatUserList.ItemsSource == null || vm.UserConversations.Count == 0)
                    if (vm.UserConversations.Count == 0)
                    {
                        vm.chatUserList = false;
                        vm.lblalert = true;
                    }
                    else
                    {
                        vm.chatUserList = true;
                        vm.lblalert = false;
                    }
                });

                chatUserList.ItemTapped += ChatUserListItemTapped;//method binding to the event
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "MessageCenterSubscribe method -> in QnAlistPage.cs " + Settings.userLoginID);
                var trackResult = service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on an chat from the list chats.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ChatUserListItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                Settings.PerviousPage = Settings.RedirectPage.Trim().ToLower() == "yship" ? "Yship" : Settings.PerviousPage;// Setting previous page to Settings
                Settings.RedirectPageQA = "QnAlistPage";// Setting Redirect to page to setting
                chatUserList.SelectedItem = null;
                bool checkInternet = await App.CheckInterNetConnection();// Checking internet connection

                if (checkInternet)
                {
                    ChatData d = (ChatData)e.Item;

                    if (d.QAID != null)
                    {
                        List<Tag> lstdat = new List<Tag>();

                        foreach (var item in d.tags)
                        {
                            Tag tg = new Tag();
                            tg.POTagID = item.POTagID;
                            lstdat.Add(tg);
                        }
                        d.tags = lstdat;
                        Settings.chatgroupname = d.Title;

                        if (Navigation.NavigationStack.Count == 1 ||
                                         Navigation.NavigationStack.Last().GetType() != typeof(ChatPage))//allowing to navigate only for single click
                        {
                            await Navigation.PushAsync(new ChatPage(d.QAID, d.POID, d.TagNumbers, d.Title, d.tags, d.Status));
                        }
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Information", "No internet conncetion.", "OK");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ChatUserListItemTapped method -> in QnAlistPage.cs " + Settings.userLoginID);
                await service.Handleexception(ex);
                vm.IndicatorVisibility = false;
            }
        }

        /// <summary>
        /// Gets called when the page is loading.
        /// </summary>
        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                Settings.qaListPageCount = 0;
                count = 0;
                vm.GetChatConversations(pId, pTagId, qaType);

                MessagingCenter.Subscribe<string, int>("UpdateUnReadCount", "ChatCount", (sender, qaid) =>
                {
                    if (qaid > 0)
                    {
                        var QA = vm.UserConversations.Where(wr => wr.QAID == qaid).FirstOrDefault();
                        QA.UnreadMessagesCount = sender.Trim().ToLower() == "firebaseservice".Trim().ToLower() ? (QA.UnreadMessagesCount == null ? 1 : QA.UnreadMessagesCount + 1) : null;
                    }
                });

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "OnAppearing method -> in QnAlistPage.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();

                MessagingCenter.Unsubscribe<string, int>("UpdateUnReadCount", "ChatCount");
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Gets called when clicked on Home icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void GoToHomeTapped(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Navigation.PopAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        YPSLogger.ReportException(ex, "GoToHomeTapped method -> in QnAlistPage.cs " + Settings.userLoginID);
        //        service.Handleexception(ex);
        //    }

        //}

        /// <summary>
        /// gets called when clicked on Back icon.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackTapped(object sender, EventArgs e)
        {
            try
            {
                Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Navigation.PopModalAsync();
                YPSLogger.ReportException(ex, "BackTapped method -> in QnAlistPage.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }
    }
}