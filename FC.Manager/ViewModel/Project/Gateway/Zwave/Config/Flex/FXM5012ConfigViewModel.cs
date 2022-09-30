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
    public class FXM5012ConfigViewModel : ProjectViewModel
    {
        private int _AutoCalibration = 1;
        private int _DemoTrip = 1;
        private int _DemoTripAndCalibration = 1;
        private int _FactorySetting = 85;
        private int _KWHMeterReportPeriod;
        private int _LEDBacklightBrightnessLevel;
        private int _LevelReportMode;
        private int _MaxLevelShuttleOpen;
        private int _MinLevelShuttleClose;
        private int _WattMeterReportPeriod;

        public FXM5012ConfigViewModel(IFrameNavigationService navigationService,
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
            SetWattMeterReportPeriodCommand = new RelayCommand<object>(execute: async (obj) => await SetWattMeterReportPeriod(obj));
            GetWattMeterReportPeriodCommand = new RelayCommand<object>(execute: async (obj) => await GetWattMeterReportPeriod(obj));
            SetKWHMeterReportPeriodCommand = new RelayCommand<object>(execute: async (obj) => await SetKWHMeterReportPeriod(obj));
            GetKWHMeterReportPeriodCommand = new RelayCommand<object>(execute: async (obj) => await GetKWHMeterReportPeriod(obj));
            SetLevelReportModeCommand = new RelayCommand<object>(execute: async (obj) => await SetLevelReportMode(obj));
            GetLevelReportModeCommand = new RelayCommand<object>(execute: async (obj) => await GetLevelReportMode(obj));
            SetDemoTripCommand = new RelayCommand<object>(execute: async (obj) => await SetDemoTrip(obj));
            GetDemoTripCommand = new RelayCommand<object>(execute: async (obj) => await GetDemoTrip(obj));
            SetLEDBacklightBrightnessLevelCommand = new RelayCommand<object>(execute: async (obj) => await SetLEDBacklightBrightnessLevel(obj));
            GetLEDBacklightBrightnessLevelCommand = new RelayCommand<object>(execute: async (obj) => await GetLEDBacklightBrightnessLevel(obj));
            SetDemoTripAndCalibrationCommand = new RelayCommand<object>(execute: async (obj) => await SetDemoTripAndCalibration(obj));
            GetDemoTripAndCalibrationCommand = new RelayCommand<object>(execute: async (obj) => await GetDemoTripAndCalibration(obj));
            SetAutoCalibrationCommand = new RelayCommand<object>(execute: async (obj) => await SetAutoCalibration(obj));
            GetAutoCalibrationCommand = new RelayCommand<object>(execute: async (obj) => await GetAutoCalibration(obj));
            SetMinLevelShuttleCloseCommand = new RelayCommand<object>(execute: async (obj) => await SetMinLevelShuttleClose(obj));
            GetMinLevelShuttleCloseCommand = new RelayCommand<object>(execute: async (obj) => await GetMinLevelShuttleClose(obj));
            SetMaxLevelShuttleOpenCommand = new RelayCommand<object>(execute: async (obj) => await SetMaxLevelShuttleOpen(obj));
            GetMaxLevelShuttleOpenCommand = new RelayCommand<object>(execute: async (obj) => await GetMaxLevelShuttleOpen(obj));
            SetFactorySettingCommand = new RelayCommand<object>(execute: async (obj) => await SetFactorySetting(obj));
            GetAllCommand = new RelayCommand<object>(execute: async (obj) => await GetAll(obj));
        }

        public int AutoCalibration
        {
            get => _AutoCalibration;
            set
            {
                if (Equals(value, _AutoCalibration))
                {
                    return;
                }
                _ = SetProperty(ref _AutoCalibration, value);
            }
        }

        public int DemoTrip
        {
            get => _DemoTrip;
            set
            {
                if (Equals(value, _DemoTrip))
                {
                    return;
                }
                _ = SetProperty(ref _DemoTrip, value);
            }
        }

        public int DemoTripAndCalibration
        {
            get => _DemoTripAndCalibration;
            set
            {
                if (Equals(value, _DemoTripAndCalibration))
                {
                    return;
                }
                _ = SetProperty(ref _DemoTripAndCalibration, value);
            }
        }

        public int FactorySetting
        {
            get => _FactorySetting;
            set
            {
                if (Equals(value, _FactorySetting))
                {
                    return;
                }
                _ = SetProperty(ref _FactorySetting, value);
            }
        }

        public int KWHMeterReportPeriod
        {
            get => _KWHMeterReportPeriod;
            set
            {
                if (Equals(value, _KWHMeterReportPeriod))
                {
                    return;
                }
                _ = SetProperty(ref _KWHMeterReportPeriod, value);
            }
        }

        public int LEDBacklightBrightnessLevel
        {
            get => _LEDBacklightBrightnessLevel;
            set
            {
                if (Equals(value, _LEDBacklightBrightnessLevel))
                {
                    return;
                }
                _ = SetProperty(ref _LEDBacklightBrightnessLevel, value);
            }
        }

        public int LevelReportMode
        {
            get => _LevelReportMode;
            set
            {
                if (Equals(value, _LevelReportMode))
                {
                    return;
                }
                _ = SetProperty(ref _LevelReportMode, value);
            }
        }

        public int MaxLevelShuttleOpen
        {
            get => _MaxLevelShuttleOpen;
            set
            {
                if (Equals(value, _MaxLevelShuttleOpen))
                {
                    return;
                }
                _ = SetProperty(ref _MaxLevelShuttleOpen, value);
            }
        }

        public int MinLevelShuttleClose
        {
            get => _MinLevelShuttleClose;
            set
            {
                if (Equals(value, _MinLevelShuttleClose))
                {
                    return;
                }
                _ = SetProperty(ref _MinLevelShuttleClose, value);
            }
        }

        public int WattMeterReportPeriod
        {
            get => _WattMeterReportPeriod;
            set
            {
                if (Equals(value, _WattMeterReportPeriod))
                {
                    return;
                }
                _ = SetProperty(ref _WattMeterReportPeriod, value);
            }
        }

        private async Task GetAll(object obj)
        {
            if (!await GetWattMeterReportPeriod(obj))
            {
                return;
            }
            if (!await GetKWHMeterReportPeriod(obj))
            {
                return;
            }
            if (!await GetLevelReportMode(obj))
            {
                return;
            }
            if (!await GetDemoTrip(obj))
            {
                return;
            }
            if (!await GetLEDBacklightBrightnessLevel(obj))
            {
                return;
            }
            if (!await GetDemoTripAndCalibration(obj))
            {
                return;
            }
            if (!await GetAutoCalibration(obj))
            {
                return;
            }
            if (!await GetMinLevelShuttleClose(obj))
            {
                return;
            }
            if (!await GetMaxLevelShuttleOpen(obj))
            {
                return;
            }
        }

        #region Set

        /// <summary>
        /// Param 10
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetAutoCalibration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      10,
                                                      1,
                                                      AutoCalibration);
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
        private async Task<bool> SetDemoTrip(object obj)
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
                                                      DemoTrip);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 9
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetDemoTripAndCalibration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      9,
                                                      1,
                                                      DemoTripAndCalibration);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 255
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetFactorySetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      255,
                                                      1,
                                                      FactorySetting);
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
        private async Task<bool> SetKWHMeterReportPeriod(object obj)
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
                                                      2,
                                                      KWHMeterReportPeriod);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 8
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetLEDBacklightBrightnessLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      8,
                                                      1,
                                                      LEDBacklightBrightnessLevel);
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
        private async Task<bool> SetLevelReportMode(object obj)
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
                                                      1,
                                                      LevelReportMode);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 12
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetMaxLevelShuttleOpen(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      12,
                                                      1,
                                                      MaxLevelShuttleOpen);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 11
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetMinLevelShuttleClose(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      11,
                                                      1,
                                                      MinLevelShuttleClose);
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
        private async Task<bool> SetWattMeterReportPeriod(object obj)
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
                                                      WattMeterReportPeriod);
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

        /// <summary>
        /// Param 10
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetAutoCalibration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                AutoCalibration = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                        project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                        10);
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
        private async Task<bool> GetDemoTrip(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DemoTrip = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                 project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                 7);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 9
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetDemoTripAndCalibration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DemoTripAndCalibration = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                               project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                               9);
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
        private async Task<bool> GetKWHMeterReportPeriod(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                KWHMeterReportPeriod = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                              project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                              2);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 8
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetLEDBacklightBrightnessLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                LEDBacklightBrightnessLevel = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                 project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                 8);
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
        private async Task<bool> GetLevelReportMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                LevelReportMode = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                        project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                        5);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 12
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetMaxLevelShuttleOpen(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                MaxLevelShuttleOpen = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                        project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                        12);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 11
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetMinLevelShuttleClose(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                MinLevelShuttleClose = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                             project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             11);
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
        private async Task<bool> GetWattMeterReportPeriod(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                WattMeterReportPeriod = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
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

        #endregion Get

        #region ICommand

        public ICommand GetAllCommand { get; set; }
        public ICommand GetAutoCalibrationCommand { get; set; }
        public ICommand GetDemoTripAndCalibrationCommand { get; set; }
        public ICommand GetDemoTripCommand { get; set; }
        public ICommand GetKWHMeterReportPeriodCommand { get; set; }
        public ICommand GetLEDBacklightBrightnessLevelCommand { get; set; }
        public ICommand GetLevelReportModeCommand { get; set; }
        public ICommand GetMaxLevelShuttleOpenCommand { get; set; }
        public ICommand GetMinLevelShuttleCloseCommand { get; set; }
        public ICommand GetWattMeterReportPeriodCommand { get; set; }
        public ICommand SetAutoCalibrationCommand { get; set; }
        public ICommand SetDemoTripAndCalibrationCommand { get; set; }
        public ICommand SetDemoTripCommand { get; set; }
        public ICommand SetFactorySettingCommand { get; set; }
        public ICommand SetKWHMeterReportPeriodCommand { get; set; }
        public ICommand SetLEDBacklightBrightnessLevelCommand { get; set; }
        public ICommand SetLevelReportModeCommand { get; set; }
        public ICommand SetMaxLevelShuttleOpenCommand { get; set; }
        public ICommand SetMinLevelShuttleCloseCommand { get; set; }
        public ICommand SetWattMeterReportPeriodCommand { get; set; }

        #endregion ICommand
    }
}