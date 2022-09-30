using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain.Model.Project;
using FC.Domain.Model.RelayTest;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class RelayTestDetailDeviceViewModel : ProjectViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private bool _isConnection;
        private bool _isTerminalEnable;
        private RelayTestModel _relayTestModel = new();
        private int _selectedIndexFilter;
        private int _selectedModeRelayIndex;

        public RelayTestDetailDeviceViewModel(IFrameNavigationService navigationService,
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
                                 ILoginRepository loginRepository,
                                 ISettingsService settingsService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _settingsService = settingsService;

            IsTerminalEnable = Domain.Properties.Settings.Default.enableTerminal;

            RelayTestModel = new RelayTestModel();

            LoadedCommand = new RelayCommand<object>(Loaded);

            GetRelayStateCommand = new RelayCommand(async () => await GetRelayState());

            SetRelayStateCommand = new RelayCommand<object>(SetRelayState);

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });

            _settingsService.PropertyChanged += SettingsServicePropertyChanged;
        }

        public ICommand GetRelayStateCommand { get; set; }

        public bool IsConnection
        {
            get => _isConnection;
            set => SetProperty(ref _isConnection, value);
        }

        public bool IsTerminalEnable
        {
            get => _isTerminalEnable;
            set => SetProperty(ref _isTerminalEnable, value);
        }

        public RelayTestModel RelayTestModel
        {
            get => _relayTestModel;
            set => SetProperty(ref _relayTestModel, value);
        }

        public int SelectedIndexFilter
        {
            get => _selectedIndexFilter;
            set
            {
                if (Equals(value, _selectedIndexFilter))
                {
                    return;
                }

                _ = SetProperty(ref _selectedIndexFilter, value);
            }
        }

        public int SelectedModeRelayIndex
        {
            get => _selectedModeRelayIndex;
            set
            {
                if (Equals(_selectedModeRelayIndex, value))
                {
                    return;
                }
                _ = SetProperty(ref _selectedModeRelayIndex, value);
            }
        }

        public ICommand SetRelayStateCommand { get; set; }

        private async Task GetRelayState()
        {
            try
            {
                await _relayTestRepository.GetRelayState(SelectedProjectModel.SelectedGateway);

                await CloseDialog();
            }
            catch (SocketException)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: Domain.Properties.Resources.UDP_Connection_Being_Used,
                                     textButtomOk: Domain.Properties.Resources.Close,
                                     ok: async () => await CloseDialog());
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;
        }

        private async void SetRelayState(object obj)
        {
            try
            {
                await _relayTestRepository.SetRelayState(SelectedProjectModel.SelectedGateway);
                await CloseDialog();
            }
            catch (SocketException)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: Domain.Properties.Resources.UDP_Connection_Being_Used,
                                     textButtomOk: Domain.Properties.Resources.Close,
                                     ok: async () => await CloseDialog());
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SettingsServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsTerminalEnable))
            {
                IsTerminalEnable = ((SettingsService)sender).IsTerminalEnable;
            }
        }
    }
}