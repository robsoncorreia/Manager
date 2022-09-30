using FC.Domain._Base;
using System;
using System.Reactive.Subjects;

namespace FC.Domain._Util
{
    public class CustomMessageBoxModel : ModelBase
    {
        private int _ProgressBarValue;

        public int ProgressBarValue
        {
            get => _ProgressBarValue;
            set
            {
                _ProgressBarValue = value;
                NotifyPropertyChanged();
            }
        }

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

        public ReplaySubject<int> RXProgressBarValue
        {
            get => _rxProgressBar;
            set
            {
                ProgressBarValue = 0;

                if (value is null)
                {
                    return;
                }
                _rxProgressBar = value;
                _ = value.Subscribe((resp) =>
                {
                    ProgressBarValue = resp;
                });
            }
        }

        private bool _IsRXProgressBarVisibible = false;

        public bool IsRXProgressBarVisibible
        {
            get => _IsRXProgressBarVisibible;
            set
            {
                if (Equals(_IsRXProgressBarVisibible, value))
                {
                    return;
                }

                _IsRXProgressBarVisibible = value;
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

        public bool IsInput
        {
            get => _IsInput;
            set
            {
                _IsInput = value;
                NotifyPropertyChanged();
            }
        }

        public string Input
        {
            get => _Input;
            set
            {
                _Input = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsProgressBar
        {
            get => _IsProgressBar;
            set
            {
                _IsProgressBar = value;
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
        private bool _IsInput;
        private string _Input;
        private bool _IsProgressBar;
        private ReplaySubject<int> _rxProgressBar;
#pragma warning disable CS0649 // Campo "CustomMessageBoxModel._rx" nunca é atribuído e sempre terá seu valor padrão null
        private readonly ReplaySubject<string> _rx;
#pragma warning restore CS0649 // Campo "CustomMessageBoxModel._rx" nunca é atribuído e sempre terá seu valor padrão null
    }
}