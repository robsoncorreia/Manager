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
using FC.Finder.View.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FC.Finder.ViewModel.Project
{
    public abstract class FlexViewModelBase : ObservableRecipient
    {
        private string _lastCommandSend;

        public string LastCommandSend
        {
            get => _lastCommandSend;
            set => SetProperty(ref _lastCommandSend, value);
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

        public object ContentSnackbar
        {
            get => _contentSnackbar;
            set => SetProperty(ref _contentSnackbar, value);
        }

        public IList<SolidColorBrush> DefautColors { get; set; }

        public object DialogContent
        {
            get => _dialogContent;
            set => SetProperty(ref _dialogContent, value);
        }

        public bool IsActiveSnackbar
        {
            get => _isActiveSnackbar;
            set => SetProperty(ref _isActiveSnackbar, value);
        }

        public bool IsOpenDialogHost
        {
            get => _isOpenDialogHost;
            set => SetProperty(ref _isOpenDialogHost, value);
        }

        public bool IsSending
        {
            get => _isSending;
            set => SetProperty(ref _isSending, value);
        }

        public ICommand LoadedCommand { get; set; }
        public ICommand ReloadCommand { get; set; }

        public AmbienceModel SelectedAmbience
        {
            get => _ambienceModel;
            set => SetProperty(ref _ambienceModel, value);
        }

        internal readonly ICommandRepository _commandRepository;
        internal readonly ISerialRepository _serialRepository;
        internal readonly IRelayRepository _relayTestRepository;
        internal readonly IRFRepository _rfRepository;
        internal readonly IIRRepository _irRepository;
        internal readonly ILocalDBRepository _localDBRepository;
        internal readonly IIPCommandRepository _ipCommandRepository;
        internal readonly IUDPRepository _udpRepository;
        internal readonly ITcpRepository _tcpRepository;
        internal readonly IParseService _parseService;

        public GatewayModel SelectedGateway
        {
            get => _selectedGateway;
            set => SetProperty(ref _selectedGateway, value);
        }

        public ICommand UpdateCommand { get; set; }

        public bool IsSendingToGateway
        {
            get => _isSendingToGateway;
            set => SetProperty(ref _isSendingToGateway, value);
        }

        public bool IsSendingToCloud
        {
            get => _isSendingToCloud;
            set => SetProperty(ref _isSendingToCloud, value);
        }

        public ICommand CancelTaskCommand { get; set; }

        public ICommand CopyCommand { get; set; }

        public ICommand UnloadedCommand { get; set; }

        protected FlexViewModelBase(IFrameNavigationService navigationService,
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
                                       IGatewayRepository gatewayRepository)
        {
            _navigationService = navigationService;

            customMessageBoxModel = new CustomMessageBoxModel();

            DefautColors = new List<SolidColorBrush> {
                new SolidColorBrush(Colors.White) ,
                new SolidColorBrush(Colors.Black) ,
                new SolidColorBrush(Colors.Blue) ,
                new SolidColorBrush(Colors.BlueViolet) ,
                new SolidColorBrush(Colors.AliceBlue) ,
                new SolidColorBrush(Colors.CadetBlue) ,
                new SolidColorBrush(Colors.Red) ,
                new SolidColorBrush(Colors.DarkRed) ,
                new SolidColorBrush(Colors.IndianRed) ,
                new SolidColorBrush(Colors.PaleVioletRed) ,
                new SolidColorBrush(Colors.Green) ,
                new SolidColorBrush(Colors.LawnGreen) ,
                new SolidColorBrush(Colors.DarkGreen) ,
                new SolidColorBrush(Colors.DarkOliveGreen) ,
                new SolidColorBrush(Colors.Yellow) ,
                new SolidColorBrush(Colors.YellowGreen) ,
                new SolidColorBrush(Colors.LightGoldenrodYellow) ,
                new SolidColorBrush(Colors.LightYellow) ,
                new SolidColorBrush(Colors.GreenYellow) ,
            };

            SelectedAmbience = new AmbienceModel();

            _commandRepository = commandRepository;

            _serialRepository = serialRepository;

            _relayTestRepository = relayTestRepository;

            _rfRepository = rfRepository;

            _irRepository = irRepository;

            _localDBRepository = localDBRepository;

            _ipCommandRepository = ipCommandRepository;

            _udpRepository = udpRepository;

            _tcpRepository = tcpRepository;

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

            customMessageBoxUserControl = new CustomMessageBoxUserControl();

            customMessageBoxViewModel = Ioc.Default.GetRequiredService<CustomMessageBoxViewModel>();

            customMessageBoxModel = customMessageBoxViewModel.CustomMessageBoxModel;

            customMessageBoxUserControl.DataContext = customMessageBoxViewModel;

            DialogContent = customMessageBoxUserControl;

            CancelTaskCommand = new RelayCommand<object>(CancelTask);

            UnloadedCommand = new RelayCommand<object>(Unloaded);

            CopyCommand = new RelayCommand<object>(Copy);

            if (_gatewayService is GatewayService)
            {
                _gatewayService.PropertyChanged += GatewayServicePropertyChanged;
            }
            if (_parseService is ParseService)
            {
                _parseService.PropertyChanged += ParseServicePropertyChanged;
            }
        }

        private void ParseServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSendingToCloud))
            {
                IsSendingToCloud = ((ParseService)sender).IsSendingToCloud;
            }
        }

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

        internal readonly CustomMessageBoxModel customMessageBoxModel;

        protected void CancelTask()
        {
            _taskService.CancelAll();
        }

        protected async Task CloseDialog(int delay = 0)
        {
            CancelTask();
            await Task.Delay(delay);
            IsOpenDialogHost = false;
            _gatewayService.IsSendingToGateway = false;
            _gatewayService.IsSendingToGateway = false;
            _parseService.IsSendingToCloud = false;
        }

        protected async Task CloseSnackbar(int delay = 0)
        {
            await Task.Delay(delay);
            IsActiveSnackbar = false;
        }

        protected async Task ShowError(Exception ex)
        {
            if (ex is null)
            {
                return;
            }

            await CloseDialog();

            _gatewayService.IsSendingToGateway = false;

            _gatewayService.IsSendingToGateway = false;

            _parseService.IsSendingToCloud = false;

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
                                 ok: async () => await CloseDialog());

            _ = await _logRepository.SaveError(ex, Properties.Resources.AppName, Properties.Resources.AppVersion);
        }

        public virtual void Unloaded(object obj)
        {
            CancelTask();
        }

        private void CancelTask(object obj)
        {
            CancelTask();
            IsOpenDialogHost = false;
            _gatewayService.IsSendingToGateway = false;
            _gatewayService.IsSendingToGateway = false;
            _parseService.IsSendingToCloud = false;
        }

        private async void Copy(object obj)
        {
            try
            {
                if (!(obj is string text))
                {
                    return;
                }

                Clipboard.SetText(text);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void GatewayServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSending))
            {
                IsSending = ((GatewayService)sender).IsSendingToGateway;
            }
            if (e.PropertyName == nameof(LastCommandSend))
            {
                LastCommandSend = ((GatewayService)sender).LastCommandSend;
            }
            if (e.PropertyName == nameof(IsSendingToGateway))
            {
                IsSendingToGateway = ((GatewayService)sender).IsSendingToGateway;
            }
        }

        #region Fields

        internal IGatewayRepository _gatewayRepository;
        internal ISettingsRepository _configurationRepository;
        private readonly CustomMessageBoxUserControl customMessageBoxUserControl;
        private readonly CustomMessageBoxViewModel customMessageBoxViewModel;
        internal IGatewayService _gatewayService;
        internal IZwaveRepository _zwaveRepository;
        internal ILogRepository _logRepository;
        internal IFrameNavigationService _navigationService;
        internal IProjectRepository _projectRepository;
        internal IProjectService _projectService;
        internal IUserRepository _userRepository;
        internal IUserRepository _userService;
        internal ITaskService _taskService;
        internal bool isFirstLoad = true;
        internal bool isVisible = false;
        private string _actionContentSnackbar;
        private AmbienceModel _ambienceModel;
        private object _contentSnackbar;
        private object _dialogContent;
        private bool _isActiveSnackbar;
        private bool _isOpenDialogHost;
        private bool _isSending;
        private GatewayModel _selectedGateway;
        private bool _isSendingToGateway;
        private bool _isSendingToCloud;

        #endregion Fields
    }
}