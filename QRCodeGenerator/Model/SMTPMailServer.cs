using System.Net.Mail;

namespace QRCodeGenerator.Model
{
    public class SMTPMailServer : ModelBase
    {
        private string _provider;
        private string _url;
        private string _smtpSettings;
        private int _port;
        private bool _enableSsl;

        public string HelpPage
        {
            get => _helpPage;
            set
            {
                if (Equals(_helpPage, value))
                {
                    return;
                }
                _helpPage = value;
                NotifyPropertyChanged();
            }
        }

        private SmtpDeliveryMethod _smtpDeliveryMethod;
        private string _helpPage;

        public SmtpDeliveryMethod SmtpDeliveryMethod
        {
            get => _smtpDeliveryMethod;
            set
            {
                if (Equals(_smtpDeliveryMethod, value))
                {
                    return;
                }
                _smtpDeliveryMethod = value;
                NotifyPropertyChanged();
            }
        }

        public int SMTPMailServerId { get; set; }

        public int Port
        {
            get => _port;
            set
            {
                if (Equals(_port, value))
                {
                    return;
                }
                _port = value;
                NotifyPropertyChanged();
            }
        }

        public bool EnableSsl
        {
            get => _enableSsl;
            set
            {
                if (Equals(_enableSsl, value))
                {
                    return;
                }
                _enableSsl = value;
                NotifyPropertyChanged();
            }
        }

        public string Provider
        {
            get => _provider;
            set
            {
                if (Equals(_provider, value))
                {
                    return;
                }
                _provider = value;
                NotifyPropertyChanged();
            }
        }

        public string URL
        {
            get => _url;
            set
            {
                if (Equals(_url, value))
                {
                    return;
                }
                _url = value;
                NotifyPropertyChanged();
            }
        }

        public string SMTPSettings
        {
            get => _smtpSettings;
            set
            {
                if (Equals(_smtpSettings, value))
                {
                    return;
                }
                _smtpSettings = value;
                NotifyPropertyChanged();
            }
        }
    }
}