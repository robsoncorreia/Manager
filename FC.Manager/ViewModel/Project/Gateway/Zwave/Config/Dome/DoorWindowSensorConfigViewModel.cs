using CommunityToolkit.Mvvm.Input;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Zwave.Config.Dome
{
    public class DoorWindowSensorConfigViewModel : ProjectViewModel
    {
        private int _BASICSETLevel;

        private int _BASICSETOffDelay;

        private int _batteryLevel;

        public DoorWindowSensorConfigViewModel(IFrameNavigationService navigationService,
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
                                               ILoginRepository loginRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            OpenDocumentationCommand = new RelayCommand<object>(OpenDocumentation);
            GetBASICSETOffDelayCommand = new RelayCommand<object>(async (obj) => await GetBASICSETOffDelay(obj));
            SetBASICSETOffDelayCommand = new RelayCommand<object>(async (obj) => await SetBASICSETOffDelay(obj));
            GetBASICSETLevelCommand = new RelayCommand<object>(async (obj) => await GetBASICSETLevel(obj));
            SetBASICSETLevelCommand = new RelayCommand<object>(async (obj) => await SetBASICSETLevel(obj));
            GetBatteryStatusCommand = new RelayCommand<object>(async (obj) => await GetBatteryStatus(obj));
            GetAllCommand = new RelayCommand<object>(GetAll);
        }

        public int BASICSETLevel
        {
            get => _BASICSETLevel;
            set => SetProperty(ref _BASICSETLevel, value);
        }

        public int BASICSETOffDelay
        {
            get => _BASICSETOffDelay;
            set => SetProperty(ref _BASICSETOffDelay, value);
        }

        public int BatteryLevel
        {
            get => _batteryLevel;
            set => SetProperty(ref _batteryLevel, value);
        }

        public ICommand GetAllCommand { get; set; }
        public ICommand GetBASICSETLevelCommand { get; set; }
        public ICommand GetBASICSETOffDelayCommand { get; set; }
        public ICommand GetBatteryStatusCommand { get; set; }
        public ICommand OpenDocumentationCommand { get; set; }

        public ICommand SetBASICSETLevelCommand { get; set; }

        public ICommand SetBASICSETOffDelayCommand { get; set; }

        private async void GetAll(object obj)
        {
            if (!await GetBASICSETOffDelay(obj))
            {
                return;
            }
            if (!await GetBASICSETLevel(obj))
            {
                return;
            }
            if (!await GetBatteryStatus(obj))
            {
                return;
            }
        }

        #region Set

        private async Task<bool> SetBASICSETLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(gateway: project.SelectedGateway,
                                                      moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      parameter: 2,
                                                      size: 1,
                                                      value: BASICSETLevel);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetBASICSETOffDelay(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      1,
                                                      2,
                                                      BASICSETOffDelay);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        #endregion Set

        #region Get

        private async Task<bool> GetBASICSETLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                BASICSETLevel = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                      moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                      parameter: 2);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetBASICSETOffDelay(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                BASICSETOffDelay = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                         moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                         parameter: 1);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetBatteryStatus(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                BatteryLevel = await _zwaveRepository.GetBatteryStatus(gateway: project.SelectedGateway,
                                                                      moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        #endregion Get

        private void OpenDocumentation(object obj)
        {
            _ = Process.Start(new ProcessStartInfo("https://docs.domeha.com/#configuration-command-class-parameters30"));
        }
    }
}