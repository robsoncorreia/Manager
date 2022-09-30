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
    public class FXC221ConfigViewModel : ProjectViewModel
    {
        private int _FactorySetting = 85;
        private int _WattMeterReportPeriod;
        private int _KWHMeterReportPeriod;
        private int _ThresholdCurrentLoadCaution;
        private int _ThresholdKWHLoadCaution;
        private int _LevelReportMode;
        private int _DemoTrip = 1;
        private int _DemoTripCalibration = 1;
        private int _AutoCalibration = 1;
        private int _MinLevelShuttleClose;
        private int _MaxLevelShuttleOpen;
        private int _SingleDimmingStepTimeAuto;
        private int _DimmingPercentageSingleTouchButton;
        private int _ReportingIntervalDimmingLevel;
        private int _ExternalSwitchType;
        private int _ExternalSwitchInput;

        public FXC221ConfigViewModel(IFrameNavigationService navigationService,
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
            GetThresholdCurrentLoadCautionCommand = new RelayCommand<object>(async (obj) => await GetThresholdCurrentLoadCaution(obj));
            SetThresholdCurrentLoadCautionCommand = new RelayCommand<object>(async (obj) => await SetThresholdCurrentLoadCaution(obj));
            GetWattMeterReportPeriodCommand = new RelayCommand<object>(async (obj) => await GetWattMeterReportPeriod(obj));
            SetWattMeterReportPeriodCommand = new RelayCommand<object>(async (obj) => await SetWattMeterReportPeriod(obj));
            GetKWHMeterReportPeriodCommand = new RelayCommand<object>(async (obj) => await GetKWHMeterReportPeriod(obj));
            SetKWHMeterReportPeriodCommand = new RelayCommand<object>(async (obj) => await SetKWHMeterReportPeriod(obj));
            GetThresholdKWHLoadCautionCommand = new RelayCommand<object>(async (obj) => await GetThresholdKWHLoadCaution(obj));
            SetThresholdKWHLoadCautionCommand = new RelayCommand<object>(async (obj) => await SetThresholdKWHLoadCaution(obj));
            GetLevelReportModeCommand = new RelayCommand<object>(async (obj) => await GetLevelReportMode(obj));
            SetLevelReportModeCommand = new RelayCommand<object>(async (obj) => await SetLevelReportMode(obj));
            GetDemoTripCommand = new RelayCommand<object>(async (obj) => await GetDemoTrip(obj));
            SetDemoTripCommand = new RelayCommand<object>(async (obj) => await SetDemoTrip(obj));
            GetDemoTripCalibrationCommand = new RelayCommand<object>(async (obj) => await GetDemoTripCalibration(obj));
            SetDemoTripCalibrationCommand = new RelayCommand<object>(async (obj) => await SetDemoTripCalibration(obj));
            GetAutoCalibrationCommand = new RelayCommand<object>(async (obj) => await GetAutoCalibration(obj));
            SetAutoCalibrationCommand = new RelayCommand<object>(async (obj) => await SetAutoCalibration(obj));
            GetMinLevelShuttleCloseCommand = new RelayCommand<object>(async (obj) => await GetMinLevelShuttleClose(obj));
            SetMinLevelShuttleCloseCommand = new RelayCommand<object>(async (obj) => await SetMinLevelShuttleClose(obj));
            GetSingleDimmingStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepTimeAuto(obj));
            SetSingleDimmingStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepTimeAuto(obj));
            GetMaxLevelShuttleOpenCommand = new RelayCommand<object>(async (obj) => await GetMaxLevelShuttleOpen(obj));
            SetMaxLevelShuttleOpenCommand = new RelayCommand<object>(async (obj) => await SetMaxLevelShuttleOpen(obj));
            GetDimmingPercentageSingleTouchButtonCommand = new RelayCommand<object>(async (obj) => await GetDimmingPercentageSingleTouchButton(obj));
            SetDimmingPercentageSingleTouchButtonCommand = new RelayCommand<object>(async (obj) => await SetDimmingPercentageSingleTouchButton(obj));
            GetReportingIntervalDimmingLevelCommand = new RelayCommand<object>(async (obj) => await GetReportingIntervalDimmingLevel(obj));
            SetReportingIntervalDimmingLevelCommand = new RelayCommand<object>(async (obj) => await SetReportingIntervalDimmingLevel(obj));
            GetExternalSwitchTypeCommand = new RelayCommand<object>(async (obj) => await GetExternalSwitchType(obj));
            SetExternalSwitchTypeCommand = new RelayCommand<object>(async (obj) => await SetExternalSwitchType(obj));
            GetExternalSwitchInputCommand = new RelayCommand<object>(async (obj) => await GetExternalSwitchInput(obj));
            SetExternalSwitchInputCommand = new RelayCommand<object>(async (obj) => await SetExternalSwitchInput(obj));
            SetFactorySettingCommand = new RelayCommand<object>(async (obj) => await SetFactorySetting(obj));
            GetAllCommand = new RelayCommand<object>(async (obj) => await GetAll(obj));
            SetFactorySettingAwaitCommand = new RelayCommand<object>((obj) => SetFactorySettingAwait(obj));

            OpenDocumentationCommand = new RelayCommand<object>(OpenDocumentation);
        }

        private async Task GetAll(object obj)
        {
            if (!await GetThresholdCurrentLoadCaution(obj))
            {
                return;
            }
            if (!await GetWattMeterReportPeriod(obj))
            {
                return;
            }
            if (!await GetKWHMeterReportPeriod(obj))
            {
                return;
            }
            if (!await GetThresholdKWHLoadCaution(obj))
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
            if (!await SetDemoTrip(obj))
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

        public int DemoTripCalibration
        {
            get => _DemoTripCalibration;
            set
            {
                if (Equals(_DemoTripCalibration, value))
                {
                    return;
                }
                _ = SetProperty(ref _DemoTripCalibration, value);
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

        public int MaxLevelShuttleOpen
        {
            get => _MaxLevelShuttleOpen;
            set
            {
                if (Equals(_MaxLevelShuttleOpen, value))
                {
                    return;
                }
                _ = SetProperty(ref _MaxLevelShuttleOpen, value);
            }
        }

        public int AutoCalibration
        {
            get => _AutoCalibration;
            set
            {
                if (Equals(_AutoCalibration, value))
                {
                    return;
                }
                _ = SetProperty(ref _AutoCalibration, value);
            }
        }

        public int ThresholdKWHLoadCaution
        {
            get => _ThresholdKWHLoadCaution;
            set
            {
                if (Equals(_ThresholdKWHLoadCaution, value))
                {
                    return;
                }
                _ = SetProperty(ref _ThresholdKWHLoadCaution, value);
            }
        }

        public int KWHMeterReportPeriod
        {
            get => _KWHMeterReportPeriod;
            set
            {
                if (Equals(_KWHMeterReportPeriod, value))
                {
                    return;
                }
                _ = SetProperty(ref _KWHMeterReportPeriod, value);
            }
        }

        public int LevelReportMode
        {
            get => _LevelReportMode;
            set
            {
                if (Equals(_LevelReportMode, value))
                {
                    return;
                }
                _ = SetProperty(ref _LevelReportMode, value);
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

        public int WattMeterReportPeriod
        {
            get => _WattMeterReportPeriod;
            set
            {
                if (Equals(_WattMeterReportPeriod, value))
                {
                    return;
                }
                _ = SetProperty(ref _WattMeterReportPeriod, value);
            }
        }

        public int DemoTrip
        {
            get => _DemoTrip;
            set
            {
                if (Equals(_DemoTrip, value))
                {
                    return;
                }
                _ = SetProperty(ref _DemoTrip, value);
            }
        }

        public int ThresholdCurrentLoadCaution
        {
            get => _ThresholdCurrentLoadCaution;
            set
            {
                if (Equals(_ThresholdCurrentLoadCaution, value))
                {
                    return;
                }
                _ = SetProperty(ref _ThresholdCurrentLoadCaution, value);
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

        public int MinLevelShuttleClose
        {
            get => _MinLevelShuttleClose;
            set
            {
                if (Equals(_MinLevelShuttleClose, value))
                {
                    return;
                }
                _ = SetProperty(ref _MinLevelShuttleClose, value);
            }
        }

        #region GET

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

                MaxLevelShuttleOpen = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

                MinLevelShuttleClose = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 7
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

                AutoCalibration = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetDemoTripCalibration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DemoTripCalibration = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 5
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

                DemoTrip = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 5
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

                LevelReportMode = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetThresholdKWHLoadCaution(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ThresholdKWHLoadCaution = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 3
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetThresholdCurrentLoadCaution(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ThresholdCurrentLoadCaution = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

                KWHMeterReportPeriod = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

                WattMeterReportPeriod = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        #endregion GET

        #region SET

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
        /// Param 7
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
        /// Param 9
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetDemoTripCalibration(object obj)
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
                                                      DemoTripCalibration);
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
        /// Param 5
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
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetThresholdKWHLoadCaution(object obj)
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
                                                      ThresholdKWHLoadCaution);
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
        private async Task<bool> SetThresholdCurrentLoadCaution(object obj)
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
                                                      ThresholdCurrentLoadCaution);
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

        public ICommand GetThresholdCurrentLoadCautionCommand { get; set; }
        public ICommand SetThresholdCurrentLoadCautionCommand { get; set; }
        public ICommand GetWattMeterReportPeriodCommand { get; set; }
        public ICommand SetWattMeterReportPeriodCommand { get; set; }
        public ICommand GetThresholdKWHLoadCautionCommand { get; set; }
        public ICommand SetThresholdKWHLoadCautionCommand { get; set; }
        public ICommand GetLevelReportModeCommand { get; set; }
        public ICommand SetLevelReportModeCommand { get; set; }
        public ICommand GetDemoTripCommand { get; set; }
        public ICommand SetDemoTripCommand { get; set; }
        public ICommand GetKWHMeterReportPeriodCommand { get; set; }
        public ICommand SetKWHMeterReportPeriodCommand { get; set; }
        public ICommand GetAutoCalibrationCommand { get; set; }
        public ICommand SetAutoCalibrationCommand { get; set; }
        public ICommand GetMaximumBrightnessLevelCommand { get; set; }
        public ICommand SetMaximumBrightnessLevelCommand { get; set; }
        public ICommand GetMinLevelShuttleCloseCommand { get; set; }
        public ICommand SetMinLevelShuttleCloseCommand { get; set; }
        public ICommand GetSingleDimmingStepTimeAutoCommand { get; set; }
        public ICommand SetSingleDimmingStepTimeAutoCommand { get; set; }
        public ICommand GetMaxLevelShuttleOpenCommand { get; set; }
        public ICommand SetMaxLevelShuttleOpenCommand { get; set; }
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
        public ICommand GetDemoTripCalibrationCommand { get; set; }
        public ICommand SetDemoTripCalibrationCommand { get; set; }
        public ICommand SetFactorySettingCommand { get; set; }

        #endregion ICommand

        private void OpenDocumentation(object obj)
        {
            _ = Process.Start(new ProcessStartInfo("http://www.mcohome.com/ProductDetail/3897788.html"));
        }
    }
}