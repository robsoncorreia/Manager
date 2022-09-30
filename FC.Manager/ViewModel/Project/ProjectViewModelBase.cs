using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.View.Components;
using FC.Manager.View.Terminal;
using FC.Manager.ViewModel._Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FC.Manager.ViewModel.Project
{
    public abstract class ProjectViewModelBase : ObservableRecipient
    {
        #region Fields

        internal IGatewayRepository _gatewayRepository;
        internal IGatewayService _gatewayService;
        internal readonly CustomMessageBoxModel customMessageBoxModel;
        internal ICommandRepository _commandRepository;
        internal ISettingsRepository _configurationRepository;
        internal IDialogService _dialogService;
        internal IIPCommandRepository _ipCommandRepository;
        internal IIRRepository _irRepository;
        internal ILocalDBRepository _localDBRepository;
        internal ILoginRepository _loginRepository;
        internal ILogRepository _logRepository;
        internal IFrameNavigationService _navigationService;
        internal IParseService _parseService;
        internal IProjectRepository _projectRepository;
        internal IProjectService _projectService;
        internal IRelayRepository _relayTestRepository;
        internal IRFRepository _rfRepository;
        internal ISerialRepository _serialRepository;
        internal ITaskService _taskService;
        internal IUDPRepository _udpRepository;
        internal IUserRepository _userRepository;
        internal IUserRepository _userService;
        internal IZwaveRepository _zwaveRepository;
        internal Action action;
        internal bool isFirstLoad = true;
        internal bool isVisible = false;
        internal ITcpRepository _tcpRepository;
        private readonly CustomMessageBoxUserControl customMessageBoxUserControl;
        private readonly CustomMessageBoxViewModel customMessageBoxViewModel;
        private string _actionContentSnackbar;
        private AmbienceModel _ambienceModel;
        private object _contentSnackbar;
        private object _dialogContent;
        private string _email;
        private bool _isActiveSnackbar;
        private bool _isOpenDialogHost;
        private bool _isSendingToCloud;
        private bool _isSendingToGateway;
        private bool _isTabEnable = true;
        private string _lastCommandSend;
        private int _selectedIndexTabControl;
        private ProjectModel _selectedProjectModel;

        #endregion Fields

        internal string GatewayException(GatewayModel selectedGateway)
        {
            return selectedGateway is null
                ? throw new ArgumentNullException(nameof(selectedGateway))
                : string.Format(CultureInfo.CurrentCulture,
                                 Domain.Properties.Resources.Device_did_not_respond,
                                 selectedGateway.Name,
                                 selectedGateway.LocalIP,
                                 (GatewayConnectionType)Properties.Settings.Default.gatewayConnectionType == GatewayConnectionType.UDP ?
                                    selectedGateway.LocalPortUDP : selectedGateway.LocalPortTCP);
        }

        protected ProjectViewModelBase(IFrameNavigationService navigationService,
                                       IProjectService projectService,
                                       IUDPRepository udpRepository,
                                       ITcpRepository tcpRepository,
                                       IIRRepository irRepository,
                                       IUserRepository userService,
                                       IProjectRepository projectRepository,
                                       ILocalDBRepository localDBRepository,
                                       ILogRepository logRepository,
                                       IUserRepository userRepository,
                                       ISerialRepository serialRepository,
                                       ICommandRepository commandRepository,
                                       IIPCommandRepository ipCommandRepository,
                                       IGatewayService gatewayService,
                                       ISettingsRepository configurationRepository,
                                       ITaskService taskService,
                                       IParseService parseService,
                                       IRFRepository rfRepository,
                                       IZwaveRepository zwaveRepository,
                                       IRelayRepository relayTestRepository,
                                       IGatewayRepository gatewayRepository,
                                       IDialogService dialogService,
                                       ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;

            _dialogService = dialogService;

            _navigationService = navigationService;

            _dialogService.PropertyChanged += DialogServicePropertyChanged;

            _commandRepository = commandRepository;

            _serialRepository = serialRepository;

            _relayTestRepository = relayTestRepository;

            _rfRepository = rfRepository;

            _irRepository = irRepository;

            _localDBRepository = localDBRepository;

            _ipCommandRepository = ipCommandRepository;

            _udpRepository = udpRepository;

            _parseService = parseService;

            _taskService = taskService;

            _gatewayService = gatewayService;

            _zwaveRepository = zwaveRepository;

            _userRepository = userRepository;

            _projectRepository = projectRepository;

            _logRepository = logRepository;

            _userService = userService;

            _projectService = projectService;

            _gatewayRepository = gatewayRepository;

            _configurationRepository = configurationRepository;

            _tcpRepository = tcpRepository;

            DefautColors = new List<SolidColorBrush> {
                (SolidColorBrush)new BrushConverter().ConvertFrom("#213e3b") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#41aea9") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#a6f6f1") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#e8ffff") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#f05454") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#af2d2d") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#ce6262") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#321f28") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#734046") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#a05344") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#e79e4f") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#39311d") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#7e7474") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#c4b6b6") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#ffdd93") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#060930") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#333456") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#333456") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#595b83") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#595b83") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#f4abc4") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#709fb0") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#a0c1b8") ,
                (SolidColorBrush)new BrushConverter().ConvertFrom("#f4ebc1")
            };

            SelectedAmbience = new AmbienceModel();

            CancelTaskCommand = new RelayCommand<object>(CancelTask);

            UnloadedCommand = new RelayCommand<object>(Unloaded);

            CopyCommand = new RelayCommand<object>(Copy);

            ActionSnackbarCommand = new RelayCommand(ActionSnackbar);

            if (_gatewayService is GatewayService)
            {
                _gatewayService.PropertyChanged += GatewayServicePropertyChanged;
            }
            if (_parseService is ParseService)
            {
                _parseService.PropertyChanged += ParseServicePropertyChanged;
            }

            customMessageBoxUserControl = new CustomMessageBoxUserControl();

            customMessageBoxViewModel = Ioc.Default.GetRequiredService<CustomMessageBoxViewModel>();

            customMessageBoxModel = customMessageBoxViewModel.CustomMessageBoxModel;

            customMessageBoxUserControl.DataContext = customMessageBoxViewModel;

            OpenTerminalCommand = new RelayCommand<object>(OpenTerminal);

            DialogContent = customMessageBoxUserControl;

            _ = _taskService.CanceledReplay.Subscribe(resp =>
              {
                  IsCanceled = resp;
              });
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
                _ = SetProperty(ref _actionContentSnackbar, value);
            }
        }

        public ICommand ActionSnackbarCommand { get; set; }

        public ICommand CancelTaskCommand { get; set; }

        public object ContentSnackbar
        {
            get => _contentSnackbar;
            set => SetProperty(ref _contentSnackbar, value);
        }

        public ICommand CopyCommand { get; set; }

        public ICommand CreateCommand { get; set; }

        public IList<SolidColorBrush> DefautColors { get; set; }

        public object DialogContent
        {
            get => _dialogContent;
            set => SetProperty(ref _dialogContent, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public bool IsActiveSnackbar
        {
            get => _isActiveSnackbar;
            set => SetProperty(ref _isActiveSnackbar, value);
        }

        public bool IsOpenDialogHost
        {
            get => _isOpenDialogHost;
            set
            {
                _ = SetProperty(ref _isOpenDialogHost, value);
                _dialogService.IsOpenDialogHost = value;
            }
        }

        public bool IsSendingToCloud
        {
            get => _isSendingToCloud;
            set => SetProperty(ref _isSendingToCloud, value);
        }

        public bool IsSendingToGateway
        {
            get => _isSendingToGateway;
            set => SetProperty(ref _isSendingToGateway, value);
        }

        public bool IsTabEnable
        {
            get => _isTabEnable;
            set => SetProperty(ref _isTabEnable, value);
        }

        public string LastCommandSend
        {
            get => _lastCommandSend;
            set => SetProperty(ref _lastCommandSend, value);
        }

        public ICommand LoadedCommand { get; set; }
        public ICommand NavigateBeforeCommand { get; set; }
        public ICommand ReloadCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand RemoveUserCommand { get; set; }
        public ICommand SelectColorCommand { get; set; }

        public AmbienceModel SelectedAmbience
        {
            get => _ambienceModel;
            set => SetProperty(ref _ambienceModel, value);
        }

        //public GatewayModel SelectedGateway
        //{
        //    get => _selectedGateway;
        //    set => SetProperty(ref _selectedGateway, value);
        //}

        public int SelectedIndexTabControl
        {
            get => _selectedIndexTabControl;
            set => SetProperty(ref _selectedIndexTabControl, value);
        }

        public ProjectModel SelectedProjectModel
        {
            get => _selectedProjectModel;
            set => SetProperty(ref _selectedProjectModel, value);
        }

        public ICommand UnloadedCommand { get; set; }
        public ICommand UpdateCommand { get; set; }

        public bool IsCanceled
        {
            get => isCanceled;
            set => SetProperty(ref isCanceled, value);
        }

        public ICommand OpenTerminalCommand { get; set; }

        private bool isCanceled;

        public void OpenCustomMessageBox(Action ok = null,
                                         Action custom = null,
                                         Action cancel = null,
                                         string header = null,
                                         string message = null,
                                         string textButtomOk = "_Ok",
                                         string textButtonCancel = "_Cancel",
                                         string textButtonCustom = null,
                                         ReplaySubject<string> rx = null,
                                         ReplaySubject<int> rxProgressBarValue = null,
                                         Uri source = null,
                                         bool isInput = false,
                                         bool isProgressBar = false,
                                         int progressBarValue = 0,
                                         bool isRXProgressBarVisibible = false)
        {
            customMessageBoxModel.Header = header;
            customMessageBoxModel.Message = message;
            customMessageBoxModel.TextButtomOk = textButtomOk;
            customMessageBoxModel.TextButtomCancel = textButtonCancel;
            customMessageBoxModel.TextButtomCustom = textButtonCustom;
            customMessageBoxModel.ActionCancel = cancel;
            customMessageBoxModel.ActionOk = ok;
            customMessageBoxModel.ActionCustom = custom;
            customMessageBoxModel.RX = rx;
            customMessageBoxModel.RXProgressBarValue = rxProgressBarValue;
            customMessageBoxModel.Source = source;
            customMessageBoxModel.IsInput = isInput;
            customMessageBoxModel.IsProgressBar = isProgressBar;
            customMessageBoxModel.ProgressBarValue = progressBarValue;
            customMessageBoxModel.IsRXProgressBarVisibible = isRXProgressBarVisibible;
            IsOpenDialogHost = true;
        }

        public virtual void Unloaded(object obj)
        {
            IsOpenDialogHost = false;
            _gatewayService.IsSendingToGateway = false;
            _parseService.IsSendingToCloud = false;
        }

        protected void ActionSnackbar()
        {
            if (action is not null)
            {
                action.Invoke();
            }
        }

        protected void CancelTask()
        {
            _taskService.CancelAll();
        }

        protected async Task CloseDialog(int delay = 0)
        {
            IsOpenDialogHost = false;
            _gatewayService.IsSendingToGateway = false;
            _parseService.IsSendingToCloud = false;
            CancelTask();
            await Task.Delay(delay);
        }

        protected async Task CloseSnackbar(int delay = 0)
        {
            await Task.Delay(delay);
            IsActiveSnackbar = false;
        }

        protected async void ShowError(Exception ex)
        {
            _gatewayService.IsSendingToGateway = false;

            _gatewayService.IsSendingToGateway = false;

            _parseService.IsSendingToCloud = false;

            if (IsCanceled)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: $"{Domain.Properties.Resources.Task_canceled}",
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                return;
            }

            if (ex is null)
            {
                return;
            }

            string message = ex.Message;

            if (ex is OperationCanceledException)
            {
                message = Domain.Properties.Resources.Did_not_response;
            }
            if (ex is NullReferenceException)
            {
                message = Domain.Properties.Resources.Without_Internet_Connection;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                 message: message,
                                 textButtomOk: Domain.Properties.Resources._Close,
                                 ok: async () => await CloseDialog());

            _ = await _logRepository.SaveError(ex, Properties.Resources.AppName, Properties.Resources.AppVersion);
        }

        protected async void ShowSnackbar(int delay = 2000, string contentSnackbar = null)
        {
            IsActiveSnackbar = true;
            ActionContentSnackbar = Domain.Properties.Resources._Close;
            ContentSnackbar = contentSnackbar;
            action = async () => await CloseSnackbar(0);
            await CloseSnackbar(delay);
        }

        private void ParseServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSendingToCloud))
            {
                IsSendingToCloud = ((ParseService)sender).IsSendingToCloud;
            }
        }

        private void CancelTask(object obj)
        {
            CancelTask();
            IsOpenDialogHost = false;
            _gatewayService.IsSendingToGateway = false;
            _parseService.IsSendingToCloud = false;
        }

        private void Copy(object obj)
        {
            try
            {
                if (obj is not string text)
                {
                    return;
                }

                Clipboard.SetText(text);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DialogServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_dialogService.IsOpenDialogHost) && IsTabEnable != !((DialogService)sender).IsOpenDialogHost)
            {
                IsTabEnable = !((DialogService)sender).IsOpenDialogHost;
            }
        }

        private void GatewayServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LastCommandSend))
            {
                LastCommandSend = ((GatewayService)sender).LastCommandSend;
            }
            if (e.PropertyName == nameof(IsSendingToGateway))
            {
                IsSendingToGateway = ((GatewayService)sender).IsSendingToGateway;
            }
        }

        internal void OpenFile(SaveFileDialog saveFileDialog)
        {
            if (saveFileDialog is null)
            {
                return;
            }
            _ = Task.Run(() =>
            {
                Process process = new();

                process.StartInfo.FileName = saveFileDialog.FileName;

                _ = process.Start();

                process.WaitForExit();
            });
        }

        private void OpenTerminal(object obj)
        {
            foreach (object item in System.Windows.Application.Current.Windows)
            {
                if (item is TerminalWindow terminalWindow)
                {
                    terminalWindow.Close();
                }
            }

            TerminalWindow termWindow = new();

            termWindow.Show();
        }
    }
}