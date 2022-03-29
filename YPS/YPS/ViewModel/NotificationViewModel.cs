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

namespace YPS.ViewModel
{
    public class NotificationViewModel : BaseViewModel
    {
        YPSService service;
        NotificationListPage pageName;
        public INavigation Navigation { get; set; }
        public ICommand Backevnttapped { set; get; }
        public ICommand clearall { get; set; }
        public ObservableCollection<NotifyHistory> NotifyHistoryData = new ObservableCollection<NotifyHistory>();

        /// <summary>
        /// Parameter less constructor.
        /// </summary>
        public NotificationViewModel(INavigation _Navigation, NotificationListPage pagename)
        {
            YPSLogger.TrackEvent("NotificationViewModel", "Page Load method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                loadingindicator = true;
                service = new YPSService();
                pageName = pagename;
                Navigation = _Navigation;
                Backevnttapped = new Command(async () => await Backevnttapped_click());
                clearall = new Command(clearall_clicked);
                ChangeLabel();
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "NotificationViewModel constructor -> in NotificationViewModel.cs " + Settings.userLoginID);
                service.Handleexception(ex);
            }
            loadingindicator = false;
        }

        public async Task Backevnttapped_click()
        {
            try
            {
                if (Navigation.NavigationStack.Count == 1)
                {
                    App.Current.MainPage = new YPS.Parts2y.Parts2y_Views.MenuPage(typeof(YPS.Parts2y.Parts2y_Views.HomePage));
                }
                else
                {
                    Navigation.PopToRootAsync(false);
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "Backevnttapped_click method -> in NotificationViewModel.cs" + Settings.userLoginID);
                await service.Handleexception(ex);
            }
        }

        /// <summary>
        /// Gets called when clicked on "Clear All".
        /// </summary>
        private async void clearall_clicked()
        {
            YPSLogger.TrackEvent("NotificationViewModel.cs ", "in clearall_clicked method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                loadingindicator = true;
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var canclear = await App.Current.MainPage.DisplayAlert("Notification", "Mark all notifications as read.", "Ok", "Cancel");

                    if (canclear == true)
                    {
                        var result = await service.clearNotifyHistory();

                        if (result?.status == 1)
                        {
                            await pageName.getPNdata();
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "Something went wrong, please try again.", "Ok");
                            //DependencyService.Get<IToastMessage>().ShortAlert("Something went wrong, please try again.");
                        }
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
                YPSLogger.ReportException(ex, "clearall_clicked method -> in NotificationViewModel.cs " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
            loadingindicator = false;
        }

        /// <summary>
        /// This method is for getting notification history.
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<NotifyHistory>> GetNotificationHistory()
        {
            YPSLogger.TrackEvent("NotificationViewModel.cs ", "in GetNotificationHistory method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                Uri uriResult;
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var result = await service.GetNotifyHistory(Settings.userLoginID);
                    NotifyHistoryData.Clear();

                    if (result?.status == 1 && result?.data?.Count > 0)
                    {
                        foreach (var item in result.data)
                        {
                            //var obj = new NotifyHistory()
                            //{
                            item.isImageVisible = Uri.TryCreate(item.Message, UriKind.Absolute, out uriResult);
                            item.isTextVisible = !Uri.TryCreate(item.Message, UriKind.Absolute, out uriResult);
                            item.IsTagnumberVisible = item.QAID == 0 ? false : labelobj.TagNumber.Status == true ? true : false;
                            item.IsTaskVisible = item.QAID == 0 ? false : labelobj.TaskName.Status == true ? true : false;
                            //Message = item.Message,
                            //NotifiedOn = item.NotifiedOn,
                            //UserName = item.UserName,
                            //QATitle = item.QATitle,
                            //POID = item.POID,
                            //QAID = item.QAID,
                            //Tags = item.Tags,
                            //Status = item.Status,
                            //UserId = item.UserId,
                            //RoleId = item.RoleId,
                            //TagNumber = item.TagNumber,
                            //QAType = item.QAType,
                            //MessageType = item.MessageType,
                            //NotificationType = item.NotificationType,
                            //FullName = item.FullName,
                            //RoleName = item.RoleName,
                            //IsRead = item.IsRead,
                            item.TaskName = "";
                            item.ReadStatusIcon = item.IsRead == true ? Icons.WhiteEnvOpen : Icons.WhiteEnvClose;
                            //listCount = item.listCount,
                            //};
                            //NotifyHistoryData.Add(obj);
                        }
                        NotifyHistoryData = result.data;
                    }
                    else
                    {
                        PLHideListAndShow = false;
                        HideLabelAndShow = true;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
                return NotifyHistoryData;
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "GetNotificationHistory method -> in NotificationViewModel.cs " + Settings.userLoginID);
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
            YPSLogger.TrackEvent("NotificationViewModel.cs ", "in ReadNotificationHistory method " + DateTime.Now + " UserId: " + Settings.userLoginID);

            try
            {
                var checkInternet = await App.CheckInterNetConnection();

                if (checkInternet)
                {
                    var result = await service.ReadNotifyHistory(qaid, Settings.userLoginID);
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Internet", "Please check your internet connection.", "Ok");
                    //DependencyService.Get<IToastMessage>().ShortAlert("Please check your internet connection.");
                }
            }
            catch (Exception ex)
            {
                YPSLogger.ReportException(ex, "ReadNotificationHistory method -> in NotificationViewModel.cs " + Settings.userLoginID);
                var trackResult = await service.Handleexception(ex);
            }
        }


        /// <summary>
        /// This is for changing the labels dynamically
        /// </summary>
        public async Task ChangeLabel()
        {
            try
            {
                labelobj = new DashboardLabelChangeClass();

                if (Settings.alllabeslvalues != null && Settings.alllabeslvalues.Count > 0)
                {
                    List<Alllabeslvalues> labelval = Settings.alllabeslvalues.Where(wr => wr.VersionID == Settings.VersionID && wr.LanguageID == Settings.LanguageID).ToList();

                    if (labelval.Count > 0)
                    {

                        var tagnumber = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TagNumber.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var taskanme = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.TaskName.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();
                        var markallread = labelval.Where(wr => wr.FieldID.Trim().ToLower() == labelobj.MarkAllRead.Name.Trim().ToLower()).Select(c => new { c.LblText, c.Status }).FirstOrDefault();

                        labelobj.TaskName.Name = (taskanme != null ? (!string.IsNullOrEmpty(taskanme.LblText) ? taskanme.LblText : labelobj.TaskName.Name) : labelobj.TaskName.Name) + " :";
                        labelobj.TaskName.Status = taskanme?.Status == 1 || taskanme?.Status == 2 ? true : false;
                        labelobj.TagNumber.Name = (tagnumber != null ? (!string.IsNullOrEmpty(tagnumber.LblText) ? tagnumber.LblText : labelobj.TagNumber.Name) : labelobj.TagNumber.Name) + " :";
                        labelobj.TagNumber.Status = tagnumber?.Status == 1 || tagnumber?.Status == 2 ? true : false;
                        labelobj.MarkAllRead.Name = (markallread != null ? (!string.IsNullOrEmpty(markallread.LblText) ? markallread.LblText : labelobj.MarkAllRead.Name) : labelobj.MarkAllRead.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                await service.Handleexception(ex);
                YPSLogger.ReportException(ex, "ChangeLabel method -> in NotificationViewModel.cs " + Settings.userLoginID);
            }
        }
        public class DashboardLabelChangeClass
        {
            public DashboardLabelFields TagNumber { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "TagNumber"
            };
            public DashboardLabelFields TaskName { get; set; } = new DashboardLabelFields
            {
                Status = false,
                Name = "TaskName"
            };
            public DashboardLabelFields MarkAllRead { get; set; } = new DashboardLabelFields
            {
                Status = true,
                Name = "LCMClearAll"
            };
        }
        public class DashboardLabelFields : IBase
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
        public DashboardLabelChangeClass _labelobj = new DashboardLabelChangeClass();
        public DashboardLabelChangeClass labelobj
        {
            get => _labelobj;
            set
            {
                _labelobj = value;
                NotifyPropertyChanged();
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

        private bool _IsClearAllVisible = false;
        public bool IsClearAllVisible
        {
            get => _IsClearAllVisible; set
            {
                _IsClearAllVisible = value;
                RaisePropertyChanged("IsClearAllVisible");
            }
        }

        private bool _loadingindicator = false;
        public bool loadingindicator
        {
            get => _loadingindicator;
            set
            {
                _loadingindicator = value;
                NotifyPropertyChanged("loadingindicator");
            }
        }

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
