using CommunityToolkit.Mvvm.Input;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Zwave.Config.Flex
{
    public class FXS69AConfigViewModel : ProjectViewModel
    {
        private int _frontButtonActuationMode;

        public int FrontButtonActuationMode
        {
            get => _frontButtonActuationMode;
            set
            {
                if (Equals(_frontButtonActuationMode, value))
                {
                    return;
                }
                _ = SetProperty(ref _frontButtonActuationMode, value);
            }
        }

        private int _loadPowerThresholdKWh;

        public int LoadPowerThresholdKWh
        {
            get => _loadPowerThresholdKWh;
            set
            {
                if (Equals(_loadPowerThresholdKWh, value))
                {
                    return;
                }
                _ = SetProperty(ref _loadPowerThresholdKWh, value);
            }
        }

        private int _reportKWhMeterPeriod;

        public int ReportKWhMeterPeriod
        {
            get => _reportKWhMeterPeriod;
            set
            {
                if (Equals(_reportKWhMeterPeriod, value))
                {
                    return;
                }
                _ = SetProperty(ref _reportKWhMeterPeriod, value);
            }
        }

        private bool _isMemoryFunction;

        public bool IsMemoryFunction
        {
            get => _isMemoryFunction;
            set
            {
                if (Equals(_isMemoryFunction, value))
                {
                    return;
                }
                _ = SetProperty(ref _isMemoryFunction, value);
            }
        }

        private int _reportWattMeterPeriod;

        public int ReportWattMeterPeriod
        {
            get => _reportWattMeterPeriod;
            set
            {
                if (Equals(_reportWattMeterPeriod, value))
                {
                    return;
                }
                _ = SetProperty(ref _reportWattMeterPeriod, value);
            }
        }

        private int _LEDIndication;

        private int _loadPowerThreshold;

        public int LoadPowerThreshold
        {
            get => _loadPowerThreshold;
            set
            {
                if (Equals(_loadPowerThreshold, value))
                {
                    return;
                }
                _ = SetProperty(ref _loadPowerThreshold, value);
            }
        }

        public ICommand GetAllCommand { get; set; }
        public ICommand GetFrontButtonActuationModeCommand { get; set; }
        public ICommand GetLoadPowerThresholdKWhCommand { get; set; }
        public ICommand GetReportKWhMeterPeriodCommand { get; set; }
        public ICommand GetReportWattMeterPeriodCommand { get; set; }
        public ICommand GetLoadPowerThresholdCommand { get; set; }
        public ICommand GetLEDIndicationCommand { get; set; }
        public ICommand GetMemoryFunctionCommand { get; set; }
        public ICommand SetLEDIndicationCommand { get; set; }
        public ICommand SetMemoryFunctionCommand { get; set; }
        public ICommand SetReportWattMeterPeriodCommand { get; set; }
        public ICommand SetLoadPowerThresholdCommand { get; set; }
        public ICommand SetReportKWhMeterPeriodCommand { get; set; }
        public ICommand SetLoadPowerThresholdKWhCommand { get; set; }
        public ICommand SetFrontButtonActuationModeCommand { get; set; }

        public int LEDIndication
        {
            get => _LEDIndication;
            set
            {
                if (Equals(_LEDIndication, value))
                {
                    return;
                }
                _ = SetProperty(ref _LEDIndication, value);
            }
        }

        public FXS69AConfigViewModel(IFrameNavigationService navigationService,
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
            GetLEDIndicationCommand = new RelayCommand<object>(async (obj) => await GetLEDIndication(obj));
            SetLEDIndicationCommand = new RelayCommand<object>(async (obj) => await SetLEDIndication(obj));
            SetMemoryFunctionCommand = new RelayCommand<object>(async (obj) => await SetMemoryFunction(obj));
            GetMemoryFunctionCommand = new RelayCommand<object>(async (obj) => await GetMemoryFunction(obj));
            SetReportWattMeterPeriodCommand = new RelayCommand<object>(async (obj) => await SetReportWattMeterPeriod(obj));
            GetReportWattMeterPeriodCommand = new RelayCommand<object>(async (obj) => await GetReportWattMeterPeriod(obj));
            GetLoadPowerThresholdCommand = new RelayCommand<object>(async (obj) => await GetLoadPowerThreshold(obj));
            SetLoadPowerThresholdCommand = new RelayCommand<object>(async (obj) => await SetLoadPowerThreshold(obj));
            GetReportKWhMeterPeriodCommand = new RelayCommand<object>(async (obj) => await GetReportKWhMeterPeriod(obj));
            SetReportKWhMeterPeriodCommand = new RelayCommand<object>(async (obj) => await SetReportKWhMeterPeriod(obj));
            SetLoadPowerThresholdKWhCommand = new RelayCommand<object>(async (obj) => await SetLoadPowerThresholdKWh(obj));
            GetLoadPowerThresholdKWhCommand = new RelayCommand<object>(async (obj) => await GetLoadPowerThresholdKWh(obj));
            SetFrontButtonActuationModeCommand = new RelayCommand<object>(async (obj) => await SetFrontButtonActuationMode(obj));
            GetFrontButtonActuationModeCommand = new RelayCommand<object>(async (obj) => await GetFrontButtonActuationMode(obj));
            GetAllCommand = new RelayCommand<object>(GetAll);
        }

        #region Set

        /// <summary>
        /// Param 7
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetFrontButtonActuationMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      7,
                                                      1,
                                                      FrontButtonActuationMode);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetLoadPowerThresholdKWh(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      6,
                                                      2,
                                                      LoadPowerThresholdKWh);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetReportKWhMeterPeriod(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      4,
                                                      2,
                                                      ReportKWhMeterPeriod);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 5
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetLoadPowerThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      5,
                                                      2,
                                                      LoadPowerThreshold);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 3
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetReportWattMeterPeriod(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      3,
                                                      2,
                                                      ReportWattMeterPeriod);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetLEDIndication(object obj)
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
                                                      1,
                                                      LEDIndication);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetMemoryFunction(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      2,
                                                      1,
                                                      IsMemoryFunction ? 1 : 0);
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

        private async void GetAll(object obj)
        {
            if (!await GetLEDIndication(obj))
            {
                return;
            }
            if (!await GetMemoryFunction(obj))
            {
                return;
            }
            if (!await GetReportWattMeterPeriod(obj))
            {
                return;
            }
            if (!await GetReportKWhMeterPeriod(obj))
            {
                return;
            }
            if (!await GetLoadPowerThreshold(obj))
            {
                return;
            }
            if (!await GetLoadPowerThresholdKWh(obj))
            {
                return;
            }
            if (!await GetFrontButtonActuationMode(obj))
            {
                return;
            }
        }

        /// <summary>
        /// Param 1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetLEDIndication(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                LEDIndication = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                      1);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetMemoryFunction(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsMemoryFunction = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                         moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                         parameter: 2) != 0;
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 3
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetReportWattMeterPeriod(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ReportWattMeterPeriod = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                              moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                              parameter: 3);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetReportKWhMeterPeriod(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ReportKWhMeterPeriod = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                              moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                              parameter: 4);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        ///  Param 5
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetLoadPowerThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                LoadPowerThreshold = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                              moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                              parameter: 5);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetLoadPowerThresholdKWh(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                LoadPowerThresholdKWh = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                              moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                              parameter: 6);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 7
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetFrontButtonActuationMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                FrontButtonActuationMode = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                              moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                              parameter: 7);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        #endregion Get
    }
}