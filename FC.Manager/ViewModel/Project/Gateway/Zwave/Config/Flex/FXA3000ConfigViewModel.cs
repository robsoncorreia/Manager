using CommunityToolkit.Mvvm.Input;
using FC.Domain.Model;
using FC.Domain.Model.Device;
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
    public class FXA3000ConfigViewModel : ProjectViewModel
    {
        private bool _IsRecordLastState;
        private bool _IsBeep;
        private bool _IsDemoMode;
        private int _FactorySetting = 85;
        private NightModeTime _NightModeTime;
        private int _DimmingMode;
        private int _AutoOff;
        private int _TimerFunctionAutoOff;
        private bool _IsNightMode;
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
        private int _DemoSceneInterval;
        private int _AllOnAllOff;
        private int _TreeWayFunctions;
        private bool _IsOpenEconomicMode;

        public FXA3000ConfigViewModel(IFrameNavigationService navigationService,
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
            OpenEconomicModeCommand = new RelayCommand<object>(OpenEconomicMode);
            CloseEconomicModeCommand = new RelayCommand<object>(CloseEconomicMode);
            GetEconomicModeCommand = new RelayCommand<object>(async (obj) => await GetEconomicMode(obj));
            SetEconomicModeCommand = new RelayCommand<object>(async (obj) => await SetEconomicMode(obj));
            GetTreeWayFunctionsCommand = new RelayCommand<object>(async (obj) => await GetTreeWayFunctions(obj));
            SetTreeWayFunctionsCommand = new RelayCommand<object>(async (obj) => await SetTreeWayFunctions(obj));
            GetAllOnAllOffCommand = new RelayCommand<object>(async (obj) => await GetAllOnAllOff(obj));
            SetAllOnAllOffCommand = new RelayCommand<object>(async (obj) => await SetAllOnAllOff(obj));
            GetRecordLastStateCommand = new RelayCommand<object>(async (obj) => await GetRecordLastState(obj));
            SetRecordLastStateCommand = new RelayCommand<object>(async (obj) => await SetRecordLastState(obj));
            GetDemoSceneIntervalCommand = new RelayCommand<object>(async (obj) => await GetDemoSceneInterval(obj));
            SetDemoSceneIntervalCommand = new RelayCommand<object>(async (obj) => await SetDemoSceneInterval(obj));
            GetDemoModeCommand = new RelayCommand<object>(async (obj) => await GetDemoMode(obj));
            SetDemoModeCommand = new RelayCommand<object>(async (obj) => await SetDemoMode(obj));
            GetBeepCommand = new RelayCommand<object>(async (obj) => await GetBeep(obj));
            SetBeepCommand = new RelayCommand<object>(async (obj) => await SetBeep(obj));
            GetNightModeTimeCommand = new RelayCommand<object>(async (obj) => await GetNightModeTime(obj));
            SetNightModeTimeCommand = new RelayCommand<object>(async (obj) => await SetNightModeTime(obj));
            GetAutoOffCommand = new RelayCommand<object>(async (obj) => await GetAutoOff(obj));
            SetAutoOffCommand = new RelayCommand<object>(async (obj) => await SetAutoOff(obj));
            GetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await GetTimerFunctionAutoOff(obj));
            SetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await SetTimerFunctionAutoOff(obj));
            GetNightModeCommand = new RelayCommand<object>(async (obj) => await GetNightMode(obj));
            SetNightModeCommand = new RelayCommand<object>(async (obj) => await SetNightMode(obj));
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
            NightModeTime = new NightModeTime();
            SelectedProjectModel = _projectService.SelectedProject;
        }

        private void CloseEconomicMode(object obj)
        {
            IsOpenEconomicMode = false;
        }

        private void OpenEconomicMode(object obj)
        {
            IsOpenEconomicMode = true;
        }

        private async Task GetAll(object obj)
        {
            if (!await GetBeep(obj))
            {
                return;
            }
        }

        public int DemoSceneInterval
        {
            get => _DemoSceneInterval;
            set
            {
                if (Equals(_DemoSceneInterval, value))
                {
                    return;
                }
                _ = SetProperty(ref _DemoSceneInterval, value);
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

        public bool IsRecordLastState
        {
            get => _IsRecordLastState;
            set
            {
                if (Equals(_IsRecordLastState, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsRecordLastState, value);
            }
        }

        public int AllOnAllOff
        {
            get => _AllOnAllOff;
            set
            {
                if (Equals(_AllOnAllOff, value))
                {
                    return;
                }
                _ = SetProperty(ref _AllOnAllOff, value);
            }
        }

        public bool IsDemoMode
        {
            get => _IsDemoMode;
            set
            {
                if (Equals(_IsDemoMode, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsDemoMode, value);
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

        public bool IsOpenEconomicMode
        {
            get => _IsOpenEconomicMode;
            set
            {
                if (Equals(_IsOpenEconomicMode, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsOpenEconomicMode, value);
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

        public NightModeTime NightModeTime
        {
            get => _NightModeTime;
            set
            {
                if (Equals(_NightModeTime, value))
                {
                    return;
                }
                _ = SetProperty(ref _NightModeTime, value);
            }
        }

        public bool IsNightMode
        {
            get => _IsNightMode;
            set
            {
                if (Equals(_IsNightMode, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsNightMode, value);
            }
        }

        public bool IsBeep
        {
            get => _IsBeep;
            set
            {
                if (Equals(_IsBeep, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsBeep, value);
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

        public int TreeWayFunctions
        {
            get => _TreeWayFunctions;
            set
            {
                if (Equals(_TreeWayFunctions, value))
                {
                    return;
                }
                _ = SetProperty(ref _TreeWayFunctions, value);
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

        private async Task<bool> GetEconomicMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.GetEconomicMode(selectedGateway: project.SelectedGateway);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetTreeWayFunctions(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                TreeWayFunctions = await _zwaveRepository.GetTreeWayFunctions(selectedGateway: project.SelectedGateway,
                                                                              moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetAllOnAllOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                AllOnAllOff = await _zwaveRepository.GetAllOnAllOff(selectedGateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetRecordLastState(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsRecordLastState = await _zwaveRepository.GetRecordLastState(selectedGateway: project.SelectedGateway,
                                                                              moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetDemoSceneInterval(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DemoSceneInterval = await _zwaveRepository.GetDemoSceneInterval(selectedGateway: project.SelectedGateway,
                                                                                moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetDemoMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsDemoMode = await _zwaveRepository.GetDemoMode(selectedGateway: project.SelectedGateway,
                                                                moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

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

        private async Task<bool> GetNightMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsNightMode = await _zwaveRepository.GetNightMode(gateway: project.SelectedGateway,
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

        private async Task<bool> GetBeep(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsBeep = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetNightModeTime(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.GetNightModeTime(gateway: project.SelectedGateway,
                                                        moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                        nightModeTime: NightModeTime);
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

        //private async Task<bool> GetClock(object obj)
        //{
        //    try
        //    {
        //        if (!(obj is ProjectModel project))
        //        {
        //            return false;
        //        }

        //        await _zwaveRepository.GetClock(gateway: project.SelectedGateway,
        //                                        moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
        //                                        clock: Clock);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError(ex);
        //        return false;
        //    }
        //}

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

        private async Task<bool> SetEconomicMode(object obj)
        {
            try
            {
                if (obj is null)
                {
                    return false;
                }
                if (obj is not object[] values)
                {
                    return false;
                }
                if (values[0] is not ProjectModel project)
                {
                    return false;
                }
                if (values[1] is not Endpoint endpoint)
                {
                    return false;
                }

                await _zwaveRepository.SetEconomicMode(selectedGateway: project.SelectedGateway,
                                                       endpoint: endpoint);

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetTreeWayFunctions(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetTreeWayFunctions(selectedGateway: project.SelectedGateway,
                                                           moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                           index: TreeWayFunctions);

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetAllOnAllOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetAllOnAllOff(selectedGateway: project.SelectedGateway,
                                                   moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                   allOnAllOff: AllOnAllOff);

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetRecordLastState(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetRecordLastState(selectedGateway: project.SelectedGateway,
                                                          moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                          isRecordLastState: IsRecordLastState);

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetDemoSceneInterval(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetDemoSceneInterval(selectedGateway: project.SelectedGateway,
                                                            moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                            demoSceneInterval: DemoSceneInterval);

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> SetDemoMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetDemoMode(selectedGateway: project.SelectedGateway,
                                                   moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                   isDemoMode: IsDemoMode);

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

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
        private async Task<bool> SetNightMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetNightMode(project.SelectedGateway,
                                                    project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                    IsNightMode);
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
        private async Task<bool> SetBeep(object obj)
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
                                                      IsBeep ? 1 : 0);
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
        private async Task<bool> SetNightModeTime(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetNightModeTime(selectedGateway: project.SelectedGateway,
                                                        moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                        nightModeTime: NightModeTime);
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
        //private async Task<bool> SetClock(object obj)
        //{
        //    try
        //    {
        //        if (!(obj is ProjectModel project))
        //        {
        //            return false;
        //        }

        //        await _zwaveRepository.SetClock(project.SelectedGateway,
        //                                        project.SelectedGateway.SelectedZwaveDevice.ModuleId,
        //                                        Clock);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError(ex);
        //        return false;
        //    }
        //}

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

        public ICommand GetEconomicModeCommand { get; set; }
        public ICommand SetEconomicModeCommand { get; set; }
        public ICommand GetTreeWayFunctionsCommand { get; set; }
        public ICommand SetTreeWayFunctionsCommand { get; set; }
        public ICommand GetAllOnAllOffCommand { get; set; }
        public ICommand SetAllOnAllOffCommand { get; set; }
        public ICommand GetRecordLastStateCommand { get; set; }
        public ICommand SetRecordLastStateCommand { get; set; }
        public ICommand GetDemoSceneIntervalCommand { get; set; }
        public ICommand SetDemoSceneIntervalCommand { get; set; }
        public ICommand GetDemoModeCommand { get; set; }
        public ICommand SetDemoModeCommand { get; set; }
        public ICommand GetBeepCommand { get; set; }
        public ICommand SetBeepCommand { get; set; }
        public ICommand GetClockCommand { get; set; }
        public ICommand SetClockCommand { get; set; }
        public ICommand GetAutoOffCommand { get; set; }
        public ICommand SetAutoOffCommand { get; set; }
        public ICommand GetTimerFunctionAutoOffCommand { get; set; }
        public ICommand SetTimerFunctionAutoOffCommand { get; set; }
        public ICommand GetNightModeCommand { get; set; }
        public ICommand SetNightModeCommand { get; set; }
        public ICommand GetNightModeTimeCommand { get; set; }
        public ICommand SetNightModeTimeCommand { get; set; }
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
        public ICommand OpenEconomicModeCommand { get; set; }
        public ICommand CloseEconomicModeCommand { get; set; }

        #endregion ICommand

        private void OpenDocumentation(object obj)
        {
            _ = Process.Start(new ProcessStartInfo("http://www.mcohome.com/ProductDetail/3897788.html"));
        }
    }
}