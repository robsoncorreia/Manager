using System.Collections.ObjectModel;

namespace QRCodeGenerator.Model
{
    public class Email : ModelBase
    {
        public int EmailId { get; set; }
        private string _address;
        private string _subject;
        private string _body;
        private string _password;

        public string Address
        {
            get => _address;
            set
            {
                if (Equals(_address, value))
                {
                    return;
                }
                _address = value;
                NotifyPropertyChanged();
            }
        }

        public string Subject
        {
            get => _subject;
            set
            {
                if (Equals(_subject, value))
                {
                    return;
                }
                _subject = value;
                NotifyPropertyChanged();
            }
        }

        public string Body
        {
            get => _body;
            set
            {
                if (Equals(_body, value))
                {
                    return;
                }
                _body = value;
                NotifyPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (Equals(_password, value))
                {
                    return;
                }
                _password = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexSMTPMailServer;

        public int SelectedIndexSMTPMailServer
        {
            get => _selectedIndexSMTPMailServer;
            set
            {
                _selectedIndexSMTPMailServer = value;
                NotifyPropertyChanged();
            }
        }

        private SMTPMailServer _smtpMailServer;

        public SMTPMailServer SMTPMailServer
        {
            get => _smtpMailServer;
            set
            {
                if (Equals(_smtpMailServer, value))
                {
                    return;
                }
                _smtpMailServer = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isBodyHtml;

        public bool IsBodyHtml
        {
            get => _isBodyHtml;
            set
            {
                _isBodyHtml = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> To { get; set; }

        public Email()
        {
            To = new ObservableCollection<string>();
        }
    }

    public enum BodyTypeEnum
    {
        Text,
        HTML
    }
}