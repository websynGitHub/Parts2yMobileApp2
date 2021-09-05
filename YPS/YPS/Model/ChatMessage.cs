using System;
using System.Collections.Generic;
using YPS.Service;
using YPS.ViewModels;

namespace YPS.Models
{
    public class ChatMessage : BaseViewModel
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string Image { get; set; }
        public bool IsMine { get; set; }
        public string MessageType { get; set; }
        public int QAID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public object MessageBase64 { get; set; }
        public string MessageBody { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageExtension { get; set; }

        private string messageDateTime;
        public string MessagDateTime
        {
            get { return messageDateTime; }
            set { messageDateTime = value; OnPropertyChanged("MessagDateTime"); }
        }

        private DateTime _messageDateTime;
        public DateTime MessagUtcDateTime
        {
            get { return _messageDateTime; }
            set { _messageDateTime = value; OnPropertyChanged("MessagUtcDateTime"); }
        }

        public string GroupName
        {
            get;
            set;
        }

        public class Tag
        {
            public int QAID { get; set; }
            public int POTagID { get; set; }
            public string TagNumber { get; set; }
            public int TaskID { get; set; }
            public int TagTaskStatus { get; set; }
            public int TaskStatus { get; set; }
        }

        public class User
        {
            public int POID { get; set; }
            public int QAID { get; set; }
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public int Status { get; set; }
        }

        public class ChatData : IBase
        {
            public int POID { get; set; }
            public int QAID { get; set; }
            public string Title { get; set; }
            public string TagNumbers { get; set; }
            public int Status { get; set; }
            public List<Tag> tags { get; set; }
            public List<User> users { get; set; }
            public int CreatedBy { get; set; }
            public int UserCount { get; set; }
            public int QAType { get; set; }
            public string UpdatedDate { get; set; }
            public string Chatstatus { get; set; }
            public string StatusColor { get; set; }
            //public Nullable<int> UnreadMessagesCount { get; set; }
            private Nullable<int> _UnreadMessagesCount;
            public Nullable<int> UnreadMessagesCount
            {
                get { return _UnreadMessagesCount; }
                set
                {
                    _UnreadMessagesCount = value;
                    NotifyPropertyChanged("UnreadMessagesCount");
                }
            }
        }

        public class GetChatData
        {
            public string message { get; set; }
            public int? status { get; set; }
            public ChatData data { get; set; }
        }

        public class GetQADataList
        {
            public string message { get; set; }
            public int status { get; set; }
            public List<ChatData> data { get; set; }
        }


        public class CLoseChat
        {
            public string message { get; set; }
            public int status { get; set; }
            public int data { get; set; }
        }

        public class CMesssage
        {
            public int ID { get; set; }
            public int? QAID { get; set; }
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string MessageType { get; set; }
            public string MessageBase64 { get; set; }
            public string MessageBody { get; set; }
            public int CreatedBy { get; set; }
            public string ImageExtension { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool IsMine { get; set; }
            public int UserCount { get; set; }
            public int? POID { get; set; }
            public int QAType { get; set; }
            public string FileName { get; set; }

        }

        public class CMessageResponse
        {
            public string message { get; set; }
            public int status { get; set; }
            public bool data { get; set; }
        }

        public class GetMessages
        {
            public string message { get; set; }
            public int status { get; set; }
            public List<CMesssage> data { get; set; }
            public int UserCount { get; set; }
        }

        public class TitleUpdate
        {
            public int? QAID { get; set; }
            public int QAtype { get; set; }
            public string Title { get; set; }
            public int CreatedBy { get; set; }
        }

        public class UserUpdating
        {
            public int? POID { get; set; }
            public int QAID { get; set; }
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public int Status { get; set; }
            public int UserCount { get; set; }
            public int RoleID { get; set; }
            public int ISCurrentUser { get; set; }
            public int CreatedBy { get; set; }
            public string Title { get; set; }
            public int QAType { get; set; }
        }
    }
}
