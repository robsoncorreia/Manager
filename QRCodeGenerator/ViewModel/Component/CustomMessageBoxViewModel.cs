using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using QRCodeGenerator.Model;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace QRCodeGenerator.ViewModel
{
    public class CustomMessageBoxViewModel : ViewModelBase
    {
        public static ReplaySubject<string> MessageSubject { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand CustomCommand { get; set; }

        public CustomMessageBoxModel CustomMessageBoxModel
        {
            get => _customMessageBoxModel;
            set
            {
                _customMessageBoxModel = value;
                RaisePropertyChanged(() => CustomMessageBoxModel);
            }
        }

        public ICommand OkCommand { get; set; }

        public CustomMessageBoxViewModel()
        {
            CustomMessageBoxModel = new CustomMessageBoxModel();

            OkCommand = new RelayCommand<object>((obj) => Ok(obj));

            CancelCommand = new RelayCommand<object>((obj) => Cancel(obj));

            CustomCommand = new RelayCommand<object>((obj) => Custom(obj));

            MessageSubject = new ReplaySubject<string>();
        }

        private void Cancel(object obj)
        {
            CustomMessageBoxModel?.ActionCancel?.Invoke();
        }

        private void Custom(object obj)
        {
            CustomMessageBoxModel?.ActionCustom?.Invoke();
        }

        private void Ok(object obj)
        {
            CustomMessageBoxModel?.ActionOk?.Invoke();
        }

        private CustomMessageBoxModel _customMessageBoxModel;
    }
}