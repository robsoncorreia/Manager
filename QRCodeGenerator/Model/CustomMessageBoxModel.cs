using System;
using System.Reactive.Subjects;

namespace QRCodeGenerator.Model
{
    public class CustomMessageBoxModel : ModelBase
    {
        private Uri _source;

        public Uri Source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyPropertyChanged();
            }
        }

        public Action ActionCancel
        {
            get => _actionCancel;
            set
            {
                _actionCancel = value;
                NotifyPropertyChanged();
            }
        }

        public Action ActionCustom
        {
            get => _actionCustom;
            set
            {
                _actionCustom = value;
                NotifyPropertyChanged();
            }
        }

        public Action ActionOk
        {
            get => _action;
            set
            {
                _action = value;
                NotifyPropertyChanged();
            }
        }

        public string Header
        {
            get => _header;
            set
            {
                _header = value;
                NotifyPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                NotifyPropertyChanged();
            }
        }

        public string TextButtomCancel
        {
            get => _textButtomCancel;
            set
            {
                _textButtomCancel = value;
                NotifyPropertyChanged();
            }
        }

        public ReplaySubject<string> RX
        {
            get => _rx;
            set
            {
                TextRX = null;

                if (value is null)
                {
                    return;
                }

                _ = value.Subscribe((resp) =>
                {
                    TextRX = resp;
                });
            }
        }

        private string _textRX;

        public string TextRX
        {
            get => _textRX;
            set
            {
                _textRX = value;
                NotifyPropertyChanged();
            }
        }

        public string TextButtomCustom
        {
            get => _textButtomCustom;
            set
            {
                _textButtomCustom = value;
                NotifyPropertyChanged();
            }
        }

        public string TextButtomOk
        {
            get => _textButtomOk;
            set
            {
                _textButtomOk = value;
                NotifyPropertyChanged();
            }
        }

        private Action _action;
        private Action _actionCancel;
        private Action _actionCustom;
        private string _header;
        private string _message;
        private string _textButtomCancel;
        private string _textButtomCustom;
        private string _textButtomOk;
#pragma warning disable CS0649 // Campo "CustomMessageBoxModel._rx" nunca é atribuído e sempre terá seu valor padrão null
        private readonly ReplaySubject<string> _rx;
#pragma warning restore CS0649 // Campo "CustomMessageBoxModel._rx" nunca é atribuído e sempre terá seu valor padrão null
    }
}