using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace YPS.Model
{
    class NotificationModel
    {
    }

    public class LoginModel
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string CompanyKey { get; set; }
        public string DeviceModel { get; set; }
        public string DevicePlatform { get; set; }
        public string DeviceUUID { get; set; }
        public string DeviceVersion { get; set; }
        public string FireBasedToken { get; set; }
    }

    public class RegisterationModel
    {
        public bool Status { get; set; }
        public string message { get; set; }
        public string RegistrationId { get; set; }
    }
    public class DeviceRegistration
    {

        public int UserId { get; set; }
        public string Platform { get; set; }
        public string HubRegistrationId { get; set; }
        public string[] Tags { get; set; }
        public string IsDataFromDb { get; set; }
        public string ModelID { get; set; }
        public string FireBasedToken { get; set; }
    }

    public class NotificationSettings
    {
        public string GeneratedAppId { get; set; }
        public string ModelName { get; set; }
        public string ModelID { get; set; }
        public string HubRegistrationId { get; set; }
        public string Platform { get; set; }
        public string Version { get; set; }
        public bool IsNotificationRequired { get; set; }
        public string FireBasedToken { get; set; }
        public int UserID { get; set; }
        public int LoginKey { get; set; }
        public string Appversion { get; set; }
        public DateTime CreatedUTCDateTime { get; set; }
        public DateTime ModifiedUTCDateTime { get; set; }
        public bool is_login_active { get; set; }
    }

    public class NotifyDataModel
    {
        public string message { get; set; }
        public int status { get; set; }
        public ObservableCollection<NotifyHistory> data { get; set; }

        public NotifyDataModel()
        {
            data = new ObservableCollection<NotifyHistory>();
        }
    }

    public class NotifyHistory
    {
        public int UserId { get; set; }
        public string Platform { get; set; }
        public string Message { get; set; }
        public string Parameter { get; set; }
        public List<string> Tags { get; set; }
        public List<string> userss { get; set; }
        public List<string> apnsuserss { get; set; }
        public int NotificationId { get; set; }
        public bool IsRead { get; set; }
        public int NotificationCount { get; set; }
        public string NotifiedOn { get; set; }
        public int listCount { get; set; }
        public int RowNumber { get; set; }
        public string UserName { get; set; }
        public string QATitle { get; set; }
        public int QAID { get; set; }
        public int POID { set; get; }
        public int Status { get; set; }
        public string UpdatedDate { get; set; }
        public string TagNumber { get; set; }
        public int RoleId { get; set; }
        public int NotificationType { get; set; }

        private bool _IsTaskVisible = false;
        public bool IsTaskVisible
        {
            get { return _IsTaskVisible; }
            set
            {
                _IsTaskVisible = value;
                OnPropertyChanged("IsTaskVisible");
            }
        }

        private bool _IsTagnumberVisible = false;
        public bool IsTagnumberVisible
        {
            get { return _IsTagnumberVisible; }
            set
            {
                _IsTagnumberVisible = value;
                OnPropertyChanged("IsTagnumberVisible");
            }
        }

        private string _ReadStatusIcon = YPS.CommonClasses.Icons.WhiteEnvClose;
        public string ReadStatusIcon
        {
            get => _ReadStatusIcon;
            set
            {
                _ReadStatusIcon = value;
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public string FullName { get; set; }
        public int QAType { get; set; }
        public string MessageType { get; set; }

        public string RoleName { get; set; }
        public string TaskName { get; set; }

    }

    public class Readnotification
    {
        public string message { get; set; }
        public int status { get; set; }
        public int data { get; set; }
    }

    public class PNData
    {

    }
    public class PushNotifyModel
    {
        public string message { get; set; }
        public int status { get; set; }
        public PNData data { get; set; }
    }

    public class NotificationcountResponse
    {
        public string message { get; set; }
        public int status { get; set; }
        public Notificationcount data { get; set; }
    }

    public class Notificationcount
    {
        public int QAPendingCount { get; set; }
        public int TaskPendingCount { get; set; }
    }
}
