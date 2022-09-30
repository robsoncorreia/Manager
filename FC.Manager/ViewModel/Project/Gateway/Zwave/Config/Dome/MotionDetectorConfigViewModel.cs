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
    public class MotionDetectorConfigViewModel : ProjectViewModel
    {
        private int _ambientLightSensitivityLevel;
        private int _BASICSETLevel;
        private int _batteryLevel;
        private int _group2AmbientLightThreshold;
        private bool _isGroup2AmbientLightThreshold;
        private bool _isLEDIndicator;
        private bool _isMotionDetector;
        private int _lightSensingInterval;
        private int _motionClearedTimeDelay;
        private int _retriggerInterval;
        private int _sensitivityLevel;

        public MotionDetectorConfigViewModel(IFrameNavigationService navigationService,
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
            _zwaveRepository = zwaveRepository;
            SetGroup2AmbientLightThresholdCommand = new RelayCommand<object>(async (obj) => await SetGroup2AmbientLightThreshold(obj));
            SetSensitivityLevelCommand = new RelayCommand<object>(async (obj) => await SetSensitivityLevelAsync(obj));
            SetBASICSETLevelCommand = new RelayCommand<object>(async (obj) => await SetBASICSETLevel(obj));
            GetGroup2AmbientLightThresholdCommand = new RelayCommand<object>(async (obj) => await GetGroup2AmbientLightThreshold(obj));
            GetSensitivityLevelCommand = new RelayCommand<object>(async (obj) => await GetSensitivityLevel(obj));
            GetBASICSETLevelCommand = new RelayCommand<object>(async (obj) => await GetBASICSETLevel(obj));
            SetMotionClearedTimeDelayCommand = new RelayCommand<object>(async (obj) => await SetMotionClearedTimeDelay(obj));
            GetMotionClearedTimeDelayCommand = new RelayCommand<object>(async (obj) => await GetMotionClearedTimeDelay(obj));
            GetMotionDetectorCommand = new RelayCommand<object>(async (obj) => await GetMotionDetector(obj));
            SetMotionDetectorCommand = new RelayCommand<object>(async (obj) => await SetMotionDetector(obj));
            GetRetriggerIntervalCommand = new RelayCommand<object>(async (obj) => await GetRetriggerInterval(obj));
            SetRetriggerIntervalCommand = new RelayCommand<object>(async (obj) => await SetRetriggerInterval(obj));
            SetLightSensingIntervalCommand = new RelayCommand<object>(async (obj) => await SetLightSensingInterval(obj));
            GetLightSensingIntervalCommand = new RelayCommand<object>(async (obj) => await GetLightSensingInterval(obj));
            GetIsGroup2AmbientLightThresholdCommand = new RelayCommand<object>(async (obj) => await GetIsGroup2AmbientLightThreshold(obj));
            SetIsGroup2AmbientLightThresholdCommand = new RelayCommand<object>(async (obj) => await SetIsGroup2AmbientLightThreshold(obj));
            GetAmbientLightSensitivityLevelCommand = new RelayCommand<object>(async (obj) => await GetAmbientLightSensitivityLevel(obj));
            SetAmbientLightSensitivityLevelCommand = new RelayCommand<object>(async (obj) => await SetAmbientLightSensitivityLevel(obj));
            GetLEDIndicatorCommand = new RelayCommand<object>(async (obj) => await GetLEDIndicator(obj));
            SetLEDIndicatorCommand = new RelayCommand<object>(async (obj) => await SetLEDIndicator(obj));
            GetBatteryStatusCommand = new RelayCommand<object>(async (obj) => await GetBatteryStatus(obj));
            GetAllCommand = new RelayCommand<object>(GetAll);
            OpenDocumentationCommand = new RelayCommand<object>(OpenDocumentation);
        }

        public int AmbientLightSensitivityLevel
        {
            get => _ambientLightSensitivityLevel;
            set => SetProperty(ref _ambientLightSensitivityLevel, value);
        }

        public int BASICSETLevel
        {
            get => _BASICSETLevel;
            set => SetProperty(ref _BASICSETLevel, value);
        }

        public int BatteryLevel
        {
            get => _batteryLevel;
            set => SetProperty(ref _batteryLevel, value);
        }

        public ICommand GetAllCommand { get; set; }

        public ICommand GetAmbientLightSensitivityLevelCommand { get; set; }

        public ICommand GetBASICSETLevelCommand { get; set; }

        public ICommand GetGroup2AmbientLightThresholdCommand { get; set; }

        public ICommand GetIsGroup2AmbientLightThresholdCommand { get; set; }

        public ICommand GetLEDIndicatorCommand { get; set; }

        public ICommand GetLightSensingIntervalCommand { get; set; }

        public ICommand GetMotionClearedTimeDelayCommand { get; set; }

        public ICommand GetMotionDetectorCommand { get; set; }

        public ICommand GetRetriggerIntervalCommand { get; set; }

        public ICommand GetSensitivityLevelCommand { get; set; }

        public int Group2AmbientLightThreshold
        {
            get => _group2AmbientLightThreshold;
            set => SetProperty(ref _group2AmbientLightThreshold, value);
        }

        public bool IsGroup2AmbientLightThreshold
        {
            get => _isGroup2AmbientLightThreshold;
            set => SetProperty(ref _isGroup2AmbientLightThreshold, value);
        }

        public bool IsLEDIndicator
        {
            get => _isLEDIndicator;
            set => SetProperty(ref _isLEDIndicator, value);
        }

        public bool IsMotionDetector
        {
            get => _isMotionDetector;
            set => SetProperty(ref _isMotionDetector, value);
        }

        public int LightSensingInterval
        {
            get => _lightSensingInterval;
            set => SetProperty(ref _lightSensingInterval, value);
        }

        public int MotionClearedTimeDelay
        {
            get => _motionClearedTimeDelay;
            set => SetProperty(ref _motionClearedTimeDelay, value);
        }

        public ICommand OpenDocumentationCommand { get; set; }

        public int RetriggerInterval
        {
            get => _retriggerInterval;
            set => SetProperty(ref _retriggerInterval, value);
        }

        public int SensitivityLevel
        {
            get => _sensitivityLevel;
            set => SetProperty(ref _sensitivityLevel, value);
        }

        public ICommand SetAmbientLightSensitivityLevelCommand { get; set; }

        public ICommand SetBASICSETLevelCommand { get; set; }
        public ICommand GetBatteryStatusCommand { get; set; }

        public ICommand SetGroup2AmbientLightThresholdCommand { get; set; }

        public ICommand SetIsGroup2AmbientLightThresholdCommand { get; set; }

        public ICommand SetLEDIndicatorCommand { get; set; }

        public ICommand SetLightSensingIntervalCommand { get; set; }

        public ICommand SetMotionClearedTimeDelayCommand { get; set; }

        public ICommand SetMotionDetectorCommand { get; set; }

        public ICommand SetRetriggerIntervalCommand { get; set; }

        public ICommand SetSensitivityLevelCommand { get; set; }

        #region Get

        private async void GetAll(object obj)
        {
            if (!await GetSensitivityLevel(obj))
            {
                return;
            }
            if (!await GetMotionClearedTimeDelay(obj))
            {
                return;
            }
            if (!await GetBASICSETLevel(obj))
            {
                return;
            }
            if (!await GetMotionDetector(obj))
            {
                return;
            }
            if (!await GetGroup2AmbientLightThreshold(obj))
            {
                return;
            }
            if (!await GetRetriggerInterval(obj))
            {
                return;
            }
            if (!await GetLightSensingInterval(obj))
            {
                return;
            }
            if (!await GetIsGroup2AmbientLightThreshold(obj))
            {
                return;
            }
            if (!await GetAmbientLightSensitivityLevel(obj))
            {
                return;
            }
            if (!await GetLEDIndicator(obj))
            {
                return;
            }
            if (!await GetBatteryStatus(obj))
            {
                return;
            }
        }

        /// <summary>
        /// Parameter 9
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetAmbientLightSensitivityLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                AmbientLightSensitivityLevel = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
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
        /// Parameter 3
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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
                                                                      parameter: 3);
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

        /// <summary>
        /// Parameter 5
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetGroup2AmbientLightThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                Group2AmbientLightThreshold = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
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
        /// Parameter 8
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetIsGroup2AmbientLightThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsGroup2AmbientLightThreshold = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                      moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                      parameter: 8) != 0;
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Parameter 10
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetLEDIndicator(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsLEDIndicator = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
                                                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                      10) != 0;
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Parameter 7
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetLightSensingInterval(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                LightSensingInterval = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        /// <summary>
        /// Parameter 2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetMotionClearedTimeDelay(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                MotionClearedTimeDelay = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
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
        /// Parameter 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetMotionDetector(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsMotionDetector = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                         moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                         parameter: 4) != 0;
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Parameter 6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetRetriggerInterval(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                RetriggerInterval = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Parameter 1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetSensitivityLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SensitivityLevel = await _zwaveRepository.GetZwaveConfig(project.SelectedGateway,
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

        #region Set

        private async Task<bool> SetAmbientLightSensitivityLevel(object obj)
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
                                                      AmbientLightSensitivityLevel);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetBASICSETLevel(object obj)
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
                                                      1,
                                                      BASICSETLevel);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetGroup2AmbientLightThreshold(object obj)
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
                                                      Group2AmbientLightThreshold);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetIsGroup2AmbientLightThreshold(object obj)
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
                                                      IsGroup2AmbientLightThreshold ? 1 : 0);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetLEDIndicator(object obj)
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
                                                      IsLEDIndicator ? 1 : 0);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetLightSensingInterval(object obj)
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
                                                      2,
                                                      LightSensingInterval);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetMotionClearedTimeDelay(object obj)
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
                                                      MotionClearedTimeDelay);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetMotionDetector(object obj)
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
                                                      1,
                                                      IsMotionDetector ? 255 : 0);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetRetriggerInterval(object obj)
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
                                                      1,
                                                      RetriggerInterval);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetSensitivityLevelAsync(object obj)
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
                                                      SensitivityLevel);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        #endregion Set

        private void OpenDocumentation(object obj)
        {
            _ = Process.Start(new ProcessStartInfo("https://docs.domeha.com/#configuration-command-class-parameters30"));
        }
    }
}