using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using YPS.CommonClasses;
using YPS.CustomToastMsg;
using YPS.Helpers;
using YPS.Service;
using YPS.ViewModels;
using static YPS.Models.ChatMessage;

namespace YPS.ViewModel
{
    public class QnAlistPageViewModel : BaseViewModel
    {
        #region Data Member
        YPSService service;
        #endregion

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="potadID"></param>
        /// <param name="qatype"></param>
        public QnAlistPageViewModel(int poId, int potadID, int qaType)
        {
            YPSLogger.TrackEvent("QnAlistPageViewModel", "page loading " + DateTime.Now + " UserId: " + Settings.userLoginID);
            service = new YPSService();// Creating new instance of the YPSService, which is used to call AIP 

            try
            {
                //GetChatConversations(poId, potadID, qaType);
            }
            catch (Exception ex)
            {
                var trackResult = service.Handleexception(ex);
                YPSLogger.ReportException(ex, "QnAlistPageViewModel constructor -> in QnAlistPageViewModel.cs " + Settings.userLoginID);
            }
        }

        /// <summary>
        /// Gets called when clicked a Q&A from Q&A list, to get the conversation data of that Q&A
        /// </summary>
        /// <param name="poid"></param>
        /// <param name="potagID"></param>
        /// <param name="QAType"></param>
        public async void GetChatConversations(int poid, int potagID, int QAType)
        {
            YPSLogger.TrackEvent("QnAlistPageViewModel.cs ", " in getchatConversations method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            IndicatorVisibility = true;

            try
            {
                bool checkInternet = await App.CheckInterNetConnection();// Check the internet connection

                if (checkInternet)
                {
                    var pnDataList = await service.GetNotifyHistory(Settings.userLoginID);
                    GetQADataList result;

                    if (poid == 0 && potagID == 0)
                    {
                        result = await service.GetArchivedChatConversations(Settings.userLoginID, QAType);
                    }
                    else
                    {
                        result = await service.GetChatConversations(poid, potagID, QAType);
                    }

                    if (result != null && (result.data != null && result.data.Count > 0))
                    {

                        foreach (var item in result.data)
                        {
                            int? showUnreadMsgCount = pnDataList.data.Where(p => p.QAID == item.QAID && p.IsRead == false).Count();

                            item.UnreadMessagesCount = showUnreadMsgCount > 0 ? showUnreadMsgCount : null;

                            if (item.Status == 1)
                            {
                                item.Chatstatus = "Live";
                                item.StatusColor = "#008000";
                            }
                            else if (item.Status == -1)
                            {
                                item.Chatstatus = "Archived";
                                item.StatusColor = "#DD4B39";
                            }
                            else
                            {
                                item.Chatstatus = "Closed";
                                item.StatusColor = "#FF0800";
                            }
                        }

                        UserConversations = new List<ChatData>(result.data);
                    }
                    MessagingCenter.Send<string>("ChatconverPage", "Bindchatconvers");
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }

            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetChatConversations method -> in QnAlistPageViewModel.cs " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
                IndicatorVisibility = false;
            }
            IndicatorVisibility = false;
        }

        #region Properties
        private Color _BgColor = YPS.CommonClasses.Settings.Bar_Background;
        public Color BgColor
        {
            get => _BgColor;
            set
            {
                _BgColor = value;
                OnPropertyChanged("BgColor");
            }
        }

        public ICommand btn_start { get; set; }
        public INavigation Navigation { get; set; }

        private List<ChatData> _UserConversations;
        public List<ChatData> UserConversations
        {
            get { return _UserConversations; }
            set
            {
                _UserConversations = value;
                OnPropertyChanged("UserConversations");
            }
        }
        public bool _IndicatorVisibility;
        public bool IndicatorVisibility
        {
            get { return _IndicatorVisibility; }
            set
            {
                _IndicatorVisibility = value;
                OnPropertyChanged("IndicatorVisibility");
            }
        }

        public bool _chatUserList = true;
        public bool chatUserList
        {
            get { return _chatUserList; }
            set
            {
                _chatUserList = value;
                OnPropertyChanged("chatUserList");
            }
        }

        public bool _lblalert = false;
        public bool lblalert
        {
            get { return _lblalert; }
            set
            {
                _lblalert = value;
                OnPropertyChanged("lblalert");
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
        #endregion
    }
}
