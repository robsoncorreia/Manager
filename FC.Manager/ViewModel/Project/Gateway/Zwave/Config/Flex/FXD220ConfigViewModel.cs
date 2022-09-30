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

namespace FC.Manager.ViewModel.Project.Device.Zwave.Config.Flex
{
    public class FXD220ConfigViewModel : ProjectViewModel
    {
        private int _FactorySetting = 85;
        private bool _DimmerStateSaved;
        private int _DimmingMode;
        private bool _IsAutoDetectionDimminMode;
        private int _AutoOff;
        private int _TimerFunctionAutoOff;
        private int _MinimumBrightnessLevel;
        private int _MaximumBrightnessLevel;
        private int _SingleDimmingStepTimeManual;
        private int _SingleDimmingStepSizeManual;
        private int _SingleDimmingStepSizeAuto;
        private int _SingleDimmingStepTimeAuto;
        private int _DimmingPercentageSingleTouchButton;
        private int _ReportingIntervalDimmingLevel;
        private int _ExternalSwitchType;
        private int _ExternalSwitchInput;
        private int _EnergyMeteringReporting;
        private int _EnergyMeteringReportingInterval;
        private int _AmpereAlarmThreshold;
        private int _VoltageAlarmThreshold;
        private int _InstantaneouscConsumptionAlarmThreshold;
        private int _OverloadTimeBeforeLoadOff;
        private bool _IsAutoLoadOffWhenOverload;
        private int _DetectionAmpereOverload;
        private int _DetectionVoltageOverload;
        private int _DetectionPowerOverload;
        private int _OverloadTime;

        public FXD220ConfigViewModel(IFrameNavigationService navigationService,
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
            GetAutoDetectionDimminModeCommand = new RelayCommand<object>(async (obj) => await GetAutoDetectionDimminMode(obj));
            SetAutoDetectionDimminModeCommand = new RelayCommand<object>(async (obj) => await SetAutoDetectionDimminMode(obj));
            GetDimmerStateSavedCommand = new RelayCommand<object>(async (obj) => await GetDimmerStateSaved(obj));
            SetDimmerStateSavedCommand = new RelayCommand<object>(async (obj) => await SetDimmerStateSaved(obj));
            GetDimmingModeCommand = new RelayCommand<object>(async (obj) => await GetDimmingMode(obj));
            SetDimmingModeCommand = new RelayCommand<object>(async (obj) => await SetDimmingMode(obj));
            GetAutoOffCommand = new RelayCommand<object>(async (obj) => await GetAutoOff(obj));
            SetAutoOffCommand = new RelayCommand<object>(async (obj) => await SetAutoOff(obj));
            GetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await GetTimerFunctionAutoOff(obj));
            SetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await SetTimerFunctionAutoOff(obj));
            GetMinimumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await GetMinimumBrightnessLevel(obj));
            SetMinimumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await SetMinimumBrightnessLevel(obj));
            GetMaximumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await GetMaximumBrightnessLevel(obj));
            SetMaximumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await SetMaximumBrightnessLevel(obj));
            GetSingleDimmingStepTimeManualCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepTimeManual(obj));
            SetSingleDimmingStepTimeManualCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepTimeManual(obj));
            GetSingleDimmingStepSizeManualCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepSizeManual(obj));
            SetSingleDimmingStepSizeManualCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepSizeManual(obj));
            GetSingleDimmingStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepTimeAuto(obj));
            SetSingleDimmingStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepTimeAuto(obj));
            GetSingleDimmingStepSizeAutoCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepSizeAuto(obj));
            SetSingleDimmingStepSizeAutoCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepSizeAuto(obj));
            GetDimmingPercentageSingleTouchButtonCommand = new RelayCommand<object>(async (obj) => await GetDimmingPercentageSingleTouchButton(obj));
            SetDimmingPercentageSingleTouchButtonCommand = new RelayCommand<object>(async (obj) => await SetDimmingPercentageSingleTouchButton(obj));
            GetReportingIntervalDimmingLevelCommand = new RelayCommand<object>(async (obj) => await GetReportingIntervalDimmingLevel(obj));
            SetReportingIntervalDimmingLevelCommand = new RelayCommand<object>(async (obj) => await SetReportingIntervalDimmingLevel(obj));
            GetExternalSwitchTypeCommand = new RelayCommand<object>(async (obj) => await GetExternalSwitchType(obj));
            SetExternalSwitchTypeCommand = new RelayCommand<object>(async (obj) => await SetExternalSwitchType(obj));
            GetExternalSwitchInputCommand = new RelayCommand<object>(async (obj) => await GetExternalSwitchInput(obj));
            SetExternalSwitchInputCommand = new RelayCommand<object>(async (obj) => await SetExternalSwitchInput(obj));
            GetAmpereAlarmThresholdCommand = new RelayCommand<object>(async (obj) => await GetAmpereAlarmThreshold(obj));
            SetAmpereAlarmThresholdCommand = new RelayCommand<object>(async (obj) => await SetAmpereAlarmThreshold(obj));
            GetAutoLoadOffWhenOverloadCommand = new RelayCommand<object>(async (obj) => await GetAutoLoadOffWhenOverload(obj));
            SetAutoLoadOffWhenOverloadCommand = new RelayCommand<object>(async (obj) => await SetAutoLoadOffWhenOverload(obj));
            GetEnergyMeteringReportingCommand = new RelayCommand<object>(async (obj) => await GetEnergyMeteringReporting(obj));
            SetEnergyMeteringReportingCommand = new RelayCommand<object>(async (obj) => await SetEnergyMeteringReporting(obj));
            GetEnergyMeteringReportingIntervalCommand = new RelayCommand<object>(async (obj) => await GetEnergyMeteringReportingInterval(obj));
            SetEnergyMeteringReportingIntervalCommand = new RelayCommand<object>(async (obj) => await SetEnergyMeteringReportingInterval(obj));
            GetVoltageAlarmThresholdCommand = new RelayCommand<object>(async (obj) => await GetVoltageAlarmThreshold(obj));
            SetVoltageAlarmThresholdCommand = new RelayCommand<object>(async (obj) => await SetVoltageAlarmThreshold(obj));
            GetInstantaneouscConsumptionAlarmThresholdCommand = new RelayCommand<object>(async (obj) => await GetInstantaneouscConsumptionAlarmThreshold(obj));
            SetInstantaneouscConsumptionAlarmThresholdCommand = new RelayCommand<object>(async (obj) => await SetInstantaneouscConsumptionAlarmThreshold(obj));
            SetOverloadTimeBeforeLoadOffCommand = new RelayCommand<object>(async (obj) => await SetOverloadTimeBeforeLoadOff(obj));
            GetOverloadTimeBeforeLoadOffCommand = new RelayCommand<object>(async (obj) => await GetOverloadTimeBeforeLoadOff(obj));
            GetDetectionAmpereOverloadCommand = new RelayCommand<object>(async (obj) => await GetDetectionAmpereOverload(obj));
            GetDetectionVoltageOverloadCommand = new RelayCommand<object>(async (obj) => await GetDetectionVoltageOverload(obj));
            GetDetectionPowerOverloadCommand = new RelayCommand<object>(async (obj) => await GetDetectionPowerOverload(obj));
            GetOverloadTimeCommand = new RelayCommand<object>(async (obj) => await GetOverloadTime(obj));
            GetAllCommand = new RelayCommand<object>(async (obj) => await GetAll(obj));
            SetFactorySettingAwaitCommand = new RelayCommand<object>((obj) => SetFactorySettingAwait(obj));

            OpenDocumentationCommand = new RelayCommand<object>(OpenDocumentation);
        }

        private async Task GetAll(object obj)
        {
            if (!await GetAutoDetectionDimminMode(obj))
            {
                return;
            }
            if (!await GetDimmerStateSaved(obj))
            {
                return;
            }
            if (!await GetDimmingMode(obj))
            {
                return;
            }
            if (!await GetAutoOff(obj))
            {
                return;
            }
            if (!await GetTimerFunctionAutoOff(obj))
            {
                return;
            }
            if (!await GetMinimumBrightnessLevel(obj))
            {
                return;
            }
            if (!await SetMinimumBrightnessLevel(obj))
            {
                return;
            }
            if (!await GetMaximumBrightnessLevel(obj))
            {
                return;
            }
            if (!await GetSingleDimmingStepTimeManual(obj))
            {
                return;
            }
            if (!await GetSingleDimmingStepSizeManual(obj))
            {
                return;
            }
            if (!await GetSingleDimmingStepTimeAuto(obj))
            {
                return;
            }
            if (!await GetSingleDimmingStepSizeAuto(obj))
            {
                return;
            }
            if (!await GetDimmingPercentageSingleTouchButton(obj))
            {
                return;
            }
            if (!await GetReportingIntervalDimmingLevel(obj))
            {
                return;
            }
            if (!await GetExternalSwitchType(obj))
            {
                return;
            }
            if (!await GetExternalSwitchInput(obj))
            {
                return;
            }
            if (!await GetAmpereAlarmThreshold(obj))
            {
                return;
            }
            if (!await GetAutoLoadOffWhenOverload(obj))
            {
                return;
            }
            if (!await GetEnergyMeteringReporting(obj))
            {
                return;
            }
            if (!await GetEnergyMeteringReportingInterval(obj))
            {
                return;
            }
            if (!await GetVoltageAlarmThreshold(obj))
            {
                return;
            }
            if (!await GetInstantaneouscConsumptionAlarmThreshold(obj))
            {
                return;
            }
            if (!await GetOverloadTimeBeforeLoadOff(obj))
            {
                return;
            }
            if (!await GetDetectionAmpereOverload(obj))
            {
                return;
            }
            if (!await GetDetectionVoltageOverload(obj))
            {
                return;
            }
            if (!await GetDetectionPowerOverload(obj))
            {
                return;
            }
            if (!await GetOverloadTime(obj))
            {
                return;
            }
        }

        public int OverloadTime
        {
            get => _OverloadTime;
            set
            {
                if (Equals(_OverloadTime, value))
                {
                    return;
                }
                _ = SetProperty(ref _OverloadTime, value);
            }
        }

        public int DetectionAmpereOverload
        {
            get => _DetectionAmpereOverload;
            set
            {
                if (Equals(_DetectionAmpereOverload, value))
                {
                    return;
                }
                _ = SetProperty(ref _DetectionAmpereOverload, value);
            }
        }

        public int OverloadTimeBeforeLoadOff
        {
            get => _OverloadTimeBeforeLoadOff;
            set
            {
                if (Equals(_OverloadTimeBeforeLoadOff, value))
                {
                    return;
                }
                _ = SetProperty(ref _OverloadTimeBeforeLoadOff, value);
            }
        }

        public bool IsAutoLoadOffWhenOverload
        {
            get => _IsAutoLoadOffWhenOverload;
            set
            {
                if (Equals(_IsAutoLoadOffWhenOverload, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsAutoLoadOffWhenOverload, value);
            }
        }

        public int InstantaneouscConsumptionAlarmThreshold
        {
            get => _InstantaneouscConsumptionAlarmThreshold;
            set
            {
                if (Equals(_InstantaneouscConsumptionAlarmThreshold, value))
                {
                    return;
                }
                _ = SetProperty(ref _InstantaneouscConsumptionAlarmThreshold, value);
            }
        }

        public int VoltageAlarmThreshold
        {
            get => _VoltageAlarmThreshold;
            set
            {
                if (Equals(_VoltageAlarmThreshold, value))
                {
                    return;
                }
                _ = SetProperty(ref _VoltageAlarmThreshold, value);
            }
        }

        public int EnergyMeteringReporting
        {
            get => _EnergyMeteringReporting;
            set
            {
                if (Equals(_EnergyMeteringReporting, value))
                {
                    return;
                }
                _ = SetProperty(ref _EnergyMeteringReporting, value);
            }
        }

        public int ExternalSwitchInput
        {
            get => _ExternalSwitchInput;
            set
            {
                if (Equals(_ExternalSwitchInput, value))
                {
                    return;
                }
                _ = SetProperty(ref _ExternalSwitchInput, value);
            }
        }

        public int ReportingIntervalDimmingLevel
        {
            get => _ReportingIntervalDimmingLevel;
            set
            {
                if (Equals(_ReportingIntervalDimmingLevel, value))
                {
                    return;
                }
                _ = SetProperty(ref _ReportingIntervalDimmingLevel, value);
            }
        }

        public int DimmingPercentageSingleTouchButton
        {
            get => _DimmingPercentageSingleTouchButton;
            set
            {
                if (Equals(_DimmingPercentageSingleTouchButton, value))
                {
                    return;
                }
                _ = SetProperty(ref _DimmingPercentageSingleTouchButton, value);
            }
        }

        public int SingleDimmingStepTimeAuto
        {
            get => _SingleDimmingStepTimeAuto;
            set
            {
                if (Equals(_SingleDimmingStepTimeAuto, value))
                {
                    return;
                }
                _ = SetProperty(ref _SingleDimmingStepTimeAuto, value);
            }
        }

        public int SingleDimmingStepSizeAuto
        {
            get => _SingleDimmingStepSizeAuto;
            set
            {
                if (Equals(_SingleDimmingStepSizeAuto, value))
                {
                    return;
                }
                _ = SetProperty(ref _SingleDimmingStepSizeAuto, value);
            }
        }

        public int SingleDimmingStepTimeManual
        {
            get => _SingleDimmingStepTimeManual;
            set
            {
                if (Equals(_SingleDimmingStepTimeManual, value))
                {
                    return;
                }
                _ = SetProperty(ref _SingleDimmingStepTimeManual, value);
            }
        }

        public int MaximumBrightnessLevel
        {
            get => _MaximumBrightnessLevel;
            set
            {
                if (Equals(_MaximumBrightnessLevel, value))
                {
                    return;
                }
                _ = SetProperty(ref _MaximumBrightnessLevel, value);
            }
        }

        public int AutoOff
        {
            get => _AutoOff;
            set
            {
                if (Equals(_AutoOff, value))
                {
                    return;
                }
                _ = SetProperty(ref _AutoOff, value);
            }
        }

        public int DimmingMode
        {
            get => _DimmingMode;
            set
            {
                if (Equals(_DimmingMode, value))
                {
                    return;
                }
                _ = SetProperty(ref _DimmingMode, value);
            }
        }

        public int TimerFunctionAutoOff
        {
            get => _TimerFunctionAutoOff;
            set
            {
                if (Equals(_TimerFunctionAutoOff, value))
                {
                    return;
                }
                _ = SetProperty(ref _TimerFunctionAutoOff, value);
            }
        }

        public int FactorySetting
        {
            get => _FactorySetting;
            set
            {
                if (Equals(_FactorySetting, value))
                {
                    return;
                }
                _ = SetProperty(ref _FactorySetting, value);
            }
        }

        public bool IsDimmerStateSaved
        {
            get => _DimmerStateSaved;
            set
            {
                if (Equals(_DimmerStateSaved, value))
                {
                    return;
                }
                _ = SetProperty(ref _DimmerStateSaved, value);
            }
        }

        public int MinimumBrightnessLevel
        {
            get => _MinimumBrightnessLevel;
            set
            {
                if (Equals(_MinimumBrightnessLevel, value))
                {
                    return;
                }
                _ = SetProperty(ref _MinimumBrightnessLevel, value);
            }
        }

        public bool IsAutoDetectionDimminMode
        {
            get => _IsAutoDetectionDimminMode;
            set
            {
                if (Equals(_IsAutoDetectionDimminMode, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsAutoDetectionDimminMode, value);
            }
        }

        public int ExternalSwitchType
        {
            get => _ExternalSwitchType;
            set
            {
                if (Equals(_ExternalSwitchType, value))
                {
                    return;
                }
                _ = SetProperty(ref _ExternalSwitchType, value);
            }
        }

        public int EnergyMeteringReportingInterval
        {
            get => _EnergyMeteringReportingInterval;
            set
            {
                if (Equals(_EnergyMeteringReportingInterval, value))
                {
                    return;
                }
                _ = SetProperty(ref _EnergyMeteringReportingInterval, value);
            }
        }

        public int SingleDimmingStepSizeManual
        {
            get => _SingleDimmingStepSizeManual;
            set
            {
                if (Equals(_SingleDimmingStepSizeManual, value))
                {
                    return;
                }
                _ = SetProperty(ref _SingleDimmingStepSizeManual, value);
            }
        }

        public int AmpereAlarmThreshold
        {
            get => _AmpereAlarmThreshold;
            set
            {
                if (Equals(_AmpereAlarmThreshold, value))
                {
                    return;
                }
                _ = SetProperty(ref _AmpereAlarmThreshold, value);
            }
        }

        public int DetectionPowerOverload
        {
            get => _DetectionPowerOverload;
            set
            {
                if (Equals(_DetectionPowerOverload, value))
                {
                    return;
                }
                _ = SetProperty(ref _DetectionPowerOverload, value);
            }
        }

        public int DetectionVoltageOverload
        {
            get => _DetectionVoltageOverload;
            set
            {
                if (Equals(_DetectionVoltageOverload, value))
                {
                    return;
                }
                _ = SetProperty(ref _DetectionVoltageOverload, value);
            }
        }

        #region GET

        /// <summary>
        /// Param 67
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetOverloadTime(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                OverloadTime = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                  moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                  parameter: 67);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 66
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetDetectionPowerOverload(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DetectionPowerOverload = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                  moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                  parameter: 66);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 65
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetDetectionVoltageOverload(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DetectionVoltageOverload = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                  moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                  parameter: 65);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 64
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetDetectionAmpereOverload(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DetectionAmpereOverload = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                  moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                  parameter: 64);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 36
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetAutoLoadOffWhenOverload(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsAutoLoadOffWhenOverload = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                  moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                  parameter: 36) != 0;
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 35
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetOverloadTimeBeforeLoadOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                OverloadTimeBeforeLoadOff = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 35);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 16
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetEnergyMeteringReportingInterval(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                EnergyMeteringReportingInterval = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 16);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 15
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetEnergyMeteringReporting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                EnergyMeteringReporting = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 15);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 13
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetExternalSwitchType(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ExternalSwitchType = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 13);
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

        private async Task<bool> GetInstantaneouscConsumptionAlarmThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                InstantaneouscConsumptionAlarmThreshold = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 34);
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

        private async Task<bool> GetDimmingPercentageSingleTouchButton(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DimmingPercentageSingleTouchButton = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 11);
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

        private async Task<bool> GetSingleDimmingStepTimeAuto(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SingleDimmingStepTimeAuto = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 9);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 10
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetSingleDimmingStepSizeAuto(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SingleDimmingStepSizeAuto = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 10);
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

        private async Task<bool> GetReportingIntervalDimmingLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ReportingIntervalDimmingLevel = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 12);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetSingleDimmingStepSizeManual(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SingleDimmingStepSizeManual = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 8);
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

        private async Task<bool> GetSingleDimmingStepTimeManual(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SingleDimmingStepTimeManual = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetMaximumBrightnessLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                MaximumBrightnessLevel = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 5
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetMinimumBrightnessLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                MinimumBrightnessLevel = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 5
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetTimerFunctionAutoOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                TimerFunctionAutoOff = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetAutoOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                AutoOff = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 32
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetAmpereAlarmThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                AmpereAlarmThreshold = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                           moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                           parameter: 32);
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

        private async Task<bool> GetAutoDetectionDimminMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsAutoDetectionDimminMode = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                           moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                           parameter: 3) != 0;
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

        private async Task<bool> GetDimmingMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DimmingMode = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        public string[] DimmingModes { get; } = { Domain.Properties.Resources.Leading_edge, Domain.Properties.Resources.Trailing_edge, Domain.Properties.Resources.On_off_only };

        /// <summary>
        /// Param 33
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetVoltageAlarmThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                VoltageAlarmThreshold = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                              moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                              parameter: 33);
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

        private async Task<bool> GetDimmerStateSaved(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsDimmerStateSaved = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                           moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                           parameter: 1) != 0;
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 14
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetExternalSwitchInput(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ExternalSwitchInput = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                                           moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                                           parameter: 14);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        #endregion GET

        #region SET

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

        private bool SetFactorySettingAwait(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Caution,
                                     textButtonCustom: Domain.Properties.Resources.Reset,
                                     custom: async () => await SetFactorySetting(obj),
                                     message: Domain.Properties.Resources.The_device_will_have_its_settings_restored_to_the_factory_settings_,
                                     cancel: async () => await CloseDialog());

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 36
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetAutoLoadOffWhenOverload(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      36,
                                                      1,
                                                      IsAutoLoadOffWhenOverload ? 1 : 0);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 35
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetOverloadTimeBeforeLoadOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      35,
                                                      1,
                                                      OverloadTimeBeforeLoadOff);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 14
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetExternalSwitchInput(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      14,
                                                      1,
                                                      ExternalSwitchInput);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 13
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetExternalSwitchType(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      13,
                                                      1,
                                                      ExternalSwitchType);
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
        private async Task<bool> SetReportingIntervalDimmingLevel(object obj)
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
                                                      ReportingIntervalDimmingLevel);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 10
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetSingleDimmingStepSizeAuto(object obj)
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
                                                      SingleDimmingStepSizeAuto);
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
        private async Task<bool> SetSingleDimmingStepSizeManual(object obj)
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
                                                      SingleDimmingStepSizeManual);
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
        private async Task<bool> SetSingleDimmingStepTimeManual(object obj)
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
                                                      SingleDimmingStepTimeManual);
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
        private async Task<bool> SetMaximumBrightnessLevel(object obj)
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
                                                      MaximumBrightnessLevel);
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
        private async Task<bool> SetMinimumBrightnessLevel(object obj)
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
                                                      MinimumBrightnessLevel);
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
        private async Task<bool> SetTimerFunctionAutoOff(object obj)
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
                                                      TimerFunctionAutoOff);
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
        private async Task<bool> SetAutoOff(object obj)
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
                                                      AutoOff);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 16
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetEnergyMeteringReportingInterval(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      16,
                                                      2,
                                                      EnergyMeteringReportingInterval);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 33
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetVoltageAlarmThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      33,
                                                      2,
                                                      VoltageAlarmThreshold);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 32
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetAmpereAlarmThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      32,
                                                      2,
                                                      AmpereAlarmThreshold);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 15
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetEnergyMeteringReporting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      15,
                                                      1,
                                                      EnergyMeteringReporting);
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
        private async Task<bool> SetInstantaneouscConsumptionAlarmThreshold(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      34,
                                                      2,
                                                      InstantaneouscConsumptionAlarmThreshold);
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
        private async Task<bool> SetDimmingPercentageSingleTouchButton(object obj)
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
                                                      DimmingPercentageSingleTouchButton);
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
        private async Task<bool> SetAutoDetectionDimminMode(object obj)
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
                                                      IsAutoDetectionDimminMode ? 1 : 0);
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
        private async Task<bool> SetDimmingMode(object obj)
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
                                                      DimmingMode);
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
        private async Task<bool> SetDimmerStateSaved(object obj)
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
                                                      IsDimmerStateSaved ? 1 : 0);
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
        private async Task<bool> SetSingleDimmingStepTimeAuto(object obj)
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
                                                      2,
                                                      SingleDimmingStepTimeAuto);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        #endregion SET

        #region ICommand

        public ICommand GetAutoDetectionDimminModeCommand { get; set; }
        public ICommand SetAutoDetectionDimminModeCommand { get; set; }
        public ICommand GetDimmerStateSavedCommand { get; set; }
        public ICommand SetDimmerStateSavedCommand { get; set; }
        public ICommand GetAutoOffCommand { get; set; }
        public ICommand SetAutoOffCommand { get; set; }
        public ICommand GetTimerFunctionAutoOffCommand { get; set; }
        public ICommand SetTimerFunctionAutoOffCommand { get; set; }
        public ICommand GetMinimumBrightnessLevelCommand { get; set; }
        public ICommand SetMinimumBrightnessLevelCommand { get; set; }
        public ICommand GetDimmingModeCommand { get; set; }
        public ICommand SetDimmingModeCommand { get; set; }
        public ICommand GetSingleDimmingStepTimeManualCommand { get; set; }
        public ICommand SetSingleDimmingStepTimeManualCommand { get; set; }
        public ICommand GetMaximumBrightnessLevelCommand { get; set; }
        public ICommand SetMaximumBrightnessLevelCommand { get; set; }
        public ICommand GetSingleDimmingStepSizeManualCommand { get; set; }
        public ICommand SetSingleDimmingStepSizeManualCommand { get; set; }
        public ICommand GetSingleDimmingStepTimeAutoCommand { get; set; }
        public ICommand SetSingleDimmingStepTimeAutoCommand { get; set; }
        public ICommand GetSingleDimmingStepSizeAutoCommand { get; set; }
        public ICommand SetSingleDimmingStepSizeAutoCommand { get; set; }
        public ICommand GetDimmingPercentageSingleTouchButtonCommand { get; set; }
        public ICommand SetDimmingPercentageSingleTouchButtonCommand { get; set; }
        public ICommand GetReportingIntervalDimmingLevelCommand { get; set; }
        public ICommand SetReportingIntervalDimmingLevelCommand { get; set; }
        public ICommand GetExternalSwitchTypeCommand { get; set; }
        public ICommand SetExternalSwitchTypeCommand { get; set; }
        public ICommand GetExternalSwitchInputCommand { get; set; }
        public ICommand SetExternalSwitchInputCommand { get; set; }
        public ICommand GetEnergyMeteringReportingCommand { get; set; }
        public ICommand SetEnergyMeteringReportingCommand { get; set; }
        public ICommand GetEnergyMeteringReportingIntervalCommand { get; set; }
        public ICommand SetEnergyMeteringReportingIntervalCommand { get; set; }
        public ICommand GetAmpereAlarmThresholdCommand { get; set; }
        public ICommand SetAmpereAlarmThresholdCommand { get; set; }
        public ICommand GetVoltageAlarmThresholdCommand { get; set; }
        public ICommand SetVoltageAlarmThresholdCommand { get; set; }
        public ICommand GetInstantaneouscConsumptionAlarmThresholdCommand { get; set; }
        public ICommand SetInstantaneouscConsumptionAlarmThresholdCommand { get; set; }
        public ICommand GetOverloadTimeBeforeLoadOffCommand { get; set; }
        public ICommand SetOverloadTimeBeforeLoadOffCommand { get; set; }
        public ICommand GetAutoLoadOffWhenOverloadCommand { get; set; }
        public ICommand SetAutoLoadOffWhenOverloadCommand { get; set; }
        public ICommand OpenDocumentationCommand { get; set; }
        public ICommand SetFactorySettingAwaitCommand { get; set; }
        public ICommand GetDetectionAmpereOverloadCommand { get; set; }
        public ICommand GetDetectionVoltageOverloadCommand { get; set; }
        public ICommand GetDetectionPowerOverloadCommand { get; set; }
        public ICommand GetOverloadTimeCommand { get; set; }
        public ICommand GetAllCommand { get; set; }

        #endregion ICommand

        private void OpenDocumentation(object obj)
        {
            _ = Process.Start(new ProcessStartInfo("http://www.mcohome.com/ProductDetail/3897788.html"));
        }
    }
}