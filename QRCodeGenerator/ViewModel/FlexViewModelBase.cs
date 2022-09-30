using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using QRCodeGenerator.Model;
using QRCodeGenerator.Service;
using QRCodeGenerator.View.Component;
using System;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace QRCodeGenerator.ViewModel
{
    public class FlexViewModelBase : ViewModelBase
    {
        private object _dialogContent;
        private bool _isOpenDialogHost;
        private bool _isActiveSnackbar;
        private string _actionContentSnackbar;
        private object _contentSnackbar;

        public FlexViewModelBase(ITaskService taskService,
                                 IDialogService dialogService)
        {
            _taskService = taskService;

            _dialogService = dialogService;

            customMessageBoxUserControl = new CustomMessageBoxUserControl();

            customMessageBoxViewModel = SimpleIoc.Default.GetInstance<CustomMessageBoxViewModel>();

            model = customMessageBoxViewModel.CustomMessageBoxModel;

            customMessageBoxUserControl.DataContext = customMessageBoxViewModel;

            DialogContent = customMessageBoxUserControl;
        }

        private readonly CustomMessageBoxUserControl customMessageBoxUserControl;
        private readonly CustomMessageBoxViewModel customMessageBoxViewModel;
        private readonly CustomMessageBoxModel model;

        public void OpenCustomMessageBox(Action ok = null,
                                         Action custom = null,
                                         Action cancel = null,
                                         string header = null,
                                         string message = null,
                                         string textButtomOk = "_Ok",
                                         string textButtonCancel = "_Cancel",
                                         string textButtonCustom = null,
                                         ReplaySubject<string> rx = null,
                                         Uri source = null)
        {
            model.Header = header;
            model.Message = message;
            model.TextButtomOk = textButtomOk;
            model.TextButtomCancel = textButtonCancel;
            model.TextButtomCustom = textButtonCustom;
            model.ActionCancel = cancel;
            model.ActionOk = ok;
            model.ActionCustom = custom;
            model.RX = rx;
            model.Source = source;
            IsOpenDialogHost = true;
        }

        public object DialogContent
        {
            get => _dialogContent;
            set => Set(ref _dialogContent, value);
        }

        public bool IsOpenDialogHost
        {
            get => _isOpenDialogHost;
            set
            {
                _ = Set(ref _isOpenDialogHost, value);
                _dialogService.IsOpenDialogHost = value;
            }
        }

        public bool IsActiveSnackbar
        {
            get => _isActiveSnackbar;
            set => Set(ref _isActiveSnackbar, value);
        }

        public string ActionContentSnackbar
        {
            get => _actionContentSnackbar;
            set
            {
                if (Equals(_actionContentSnackbar, value))
                {
                    return;
                }
                _ = Set(ref _actionContentSnackbar, value);
            }
        }

        protected void CancelTask()
        {
            _taskService.CancelAll();
        }

        private bool _isTabEnable = true;

        public bool IsTabEnable
        {
            get => _isTabEnable;
            set => Set(ref _isTabEnable, value);
        }

        protected async Task ShowError(Exception ex)
        {
            if (ex is null)
            {
                return;
            }

            await CloseDialog();

            string message = ex.Message;

            if (ex is OperationCanceledException)
            {
                message = Properties.Resources.Did_not_response;
            }
            if (ex is NullReferenceException)
            {
                message = Properties.Resources.Without_Internet_Connection;
            }

            OpenCustomMessageBox(header: Properties.Resources.Error,
                                 message: message,
                                 ok: async () => await CloseDialog());
        }

        protected async Task CloseDialog(int delay = 0)
        {
            CancelTask();
            await Task.Delay(delay);
            IsOpenDialogHost = false;
        }

        public object ContentSnackbar
        {
            get => _contentSnackbar;
            set => Set(ref _contentSnackbar, value);
        }

        internal Action action;
        internal ITaskService _taskService;
        internal IDialogService _dialogService;

        protected async void ShowSnackbar(int delay = 2000, string contentSnackbar = null)
        {
            IsActiveSnackbar = true;
            ActionContentSnackbar = Properties.Resources._Close;
            ContentSnackbar = contentSnackbar;
            action = async () => await CloseSnackbar(0);
            await CloseSnackbar(delay);
        }

        protected async Task CloseSnackbar(int delay = 0)
        {
            await Task.Delay(delay);
            IsActiveSnackbar = false;
        }
    }
}