using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace FC.Manager.ViewModel._Util
{
    public class CustomMessageBoxViewModel : ObservableRecipient
    {
        public static ReplaySubject<string> MessageSubject { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand CustomCommand { get; set; }

        public CustomMessageBoxModel CustomMessageBoxModel
        {
            get => _customMessageBoxModel;
            set => SetProperty(ref _customMessageBoxModel, value);
        }

        public ICommand OkCommand { get; set; }

        public CustomMessageBoxViewModel()
        {
            CustomMessageBoxModel = new CustomMessageBoxModel();

            OkCommand = new RelayCommand(Ok);

            CancelCommand = new RelayCommand(Cancel);

            CustomCommand = new RelayCommand(Custom);

            MessageSubject = new ReplaySubject<string>();
        }

        private void Cancel()
        {
            CustomMessageBoxModel?.ActionCancel?.Invoke();
        }

        private void Custom()
        {
            CustomMessageBoxModel?.ActionCustom?.Invoke();
        }

        private void Ok()
        {
            CustomMessageBoxModel?.ActionOk?.Invoke();
        }

        private CustomMessageBoxModel _customMessageBoxModel;
    }
}