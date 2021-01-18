using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace YPS.ViewModel
{
    public class NotificationViewModel : BaseViewModel
    {
        YPSService service;
        public ICommand clearall { get; set; }
        ObservableCollection<NotifyHistory> NotifyHistoryData = new ObservableCollection<NotifyHistory>();

        /// <summary>
        /// Parameter less constructor.
        /// </summary>
        public NotificationViewModel()
        {
            YPSLogger.TrackEvent("NotificationViewModel", "Page Load method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                service = new YPSService();
                clearall = new Command(clearall_clicked);
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NotificationViewModel constructor -> in NotificationViewModel " + Settings.userLoginID);
                service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Clear All".
        /// </summary>
        private async void clearall_clicked()
        {
            YPSLogger.TrackEvent("NotificationViewModel", "in clearall_clicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var result = await service.clearNotifyHistory();

                    if (result.status != 0 || result != null)
                    {

                        PLHideListAndShow = false;
                        HideLabelAndShow = true;
                        await App.Current.MainPage.DisplayAlert("Notification", "Clear all notifications.", "OK");
                    }
                    else
                    {
                        DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "clearall_clicked method -> in NotificationViewModel " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// This method is for getting notification history.
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<NotifyHistory>> GetNotificationHistory()
        {
            YPSLogger.TrackEvent("NotificationViewModel", "in GetNotificationHistory method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            
            try
            {
                Uri uriResult;
                var checkInternet = await App.CheckInterNetConnection();
               
                if (checkInternet)
                {
                    var result = await service.GetNotifyHistory(Settings.userLoginID);
                    NotifyHistoryData.Clear();
                    
                    if (result.data.Count != 0)
                    {
                        if (result.status != 0)
                        {
                            foreach (var item in result.data)
                            {
                                var obj = new NotifyHistory()
                                {
                                    isImageVisible = Uri.TryCreate(item.Message, UriKind.Absolute, out uriResult),
                                    isTextVisible = !Uri.TryCreate(item.Message, UriKind.Absolute, out uriResult),
                                    Message = item.Message,
                                    NotifiedOn = item.NotifiedOn,
                                    UserName = item.UserName,
                                    QATitle = item.QATitle,
                                    POID = item.POID,
                                    QAID = item.QAID,
                                    Tags = item.Tags,
                                    Status = item.Status,
                                    UserId = item.UserId,
                                    RoleId = item.RoleId,
                                    TagNumber = item.TagNumber,
                                    QAType = item.QAType,
                                    MessageType = item.MessageType,
                                    NotificationType = item.NotificationType
                                };
                                NotifyHistoryData.Add(obj);
                            }
                        }
                    }
                    else
                    {
                        PLHideListAndShow = false;
                        HideLabelAndShow = true;
                    }
                }
                else
                {
                    DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                return NotifyHistoryData;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetNotificationHistory method -> in NotificationViewModel " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
                return null;
            }
        }

        /// <summary>
        /// This method is for reading all the notification history.
        /// </summary>
        /// <param name="qaid"></param>
        public async void ReadNotificationHistory(int qaid)
        {
            YPSLogger.TrackEvent("NotificationViewModel", "in ReadNotificationHistory method " + DateTime.Now + " UserId: " + Settings.userLoginID);
            
            try
            {
                var checkInternet = await App.CheckInterNetConnection();
                
                if (checkInternet)
                {
                    var result = await service.ReadNotifyHistory(qaid, Settings.userLoginID);

                    if (result.status != 0)
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
                YPSLogger.ReportException(ex, "ReadNotificationHistory method -> in NotificationViewModel " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
        }

        #region INotifyPropertyChanged Implimentation
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region Properties        

        private bool _isTextVisible = false;

        public bool isTextVisible
        {
            get { return _isTextVisible; }
            set
            {
                _isTextVisible = value;
                OnPropertyChanged("isTextVisible");
            }
        }

        private bool _isImageVisible = false;

        public bool isImageVisible
        {
            get { return _isImageVisible; }
            set
            {
                _isImageVisible = value;
                OnPropertyChanged("isImageVisible");
            }
        }

        private bool _PLHideListAndShow = true;

        public bool PLHideListAndShow
        {
            get { return _PLHideListAndShow; }
            set
            {
                _PLHideListAndShow = value;
                OnPropertyChanged("PLHideListAndShow");
            }
        }

        private bool _HideLabelAndShow = false;

        public bool HideLabelAndShow
        {
            get { return _HideLabelAndShow; }
            set
            {
                _HideLabelAndShow = value;
                OnPropertyChanged("HideLabelAndShow");
            }
        }
        #endregion
    }
}
