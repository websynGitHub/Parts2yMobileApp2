using System;

namespace YPS.ViewModels
{
    public class ChatMessageViewModel : BaseViewModel
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");

            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private string _image;

        public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged("Image");
            }
        }

        private string _document;

        public string Document
        {
            get => _document;
            set
            {
                _document = value;
                OnPropertyChanged("Document");
            }
        }


        private string _messagetype;

        public string MessageType
        {
            get { return _messagetype; }
            set
            {
                _messagetype = value;
                OnPropertyChanged("MessageType");
            }
        }

        private DateTime _messageDateTime;
        public DateTime MessagUtcDateTime
        {
            get { return _messageDateTime; }
            set { _messageDateTime = value; OnPropertyChanged("MessagUtcDateTime"); }
        }

        private bool _isMine;

        public bool IsMine
        {
            get { return _isMine; }
            set
            {
                _isMine = value;
                OnPropertyChanged("IsMine");
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

        private string messageDateTime;
        public string MessagDateTime
        {
            get { return messageDateTime; }
            set { messageDateTime = value; OnPropertyChanged("MessagDateTime"); }
        }
        public bool HasAttachement => !string.IsNullOrEmpty(attachementUrl);

        private string attachementUrl;

        public string AttachementUrl
        {
            get { return attachementUrl; }
            set { attachementUrl = value; OnPropertyChanged("AttachementUrl"); }
        }
    }
}
