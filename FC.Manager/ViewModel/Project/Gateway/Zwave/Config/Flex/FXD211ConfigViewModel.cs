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
    public class FXD211ConfigViewModel : ProjectViewModel
    {
        private int _AllOnAllOff;
        private int _DimmingPercentageSingleTouchButton;
        private int _DoubleClickS1;
        private int _ExternalkeyType;
        private int _ExternalSwitchInput;
        private int _ExternalSwitchType;
        private int _FactorySetting = 85;
        private bool _IsSavedStatus;
        private int _MaximumBrightnessLevel;
        private int _MinimumBrightnessLevel;
        private int _OneClickS1;
        private int _PositionSwitchToggleBistavel;
        private int _S2Functions;
        private int _SceneIdSentAG1ClickS2;
        private int _SceneIdSentAG1HoldingS2;
        private int _SceneIdSentAG1S2DoubleClick;
        private int _SimpleDimmerPitchSizeManual;
        private int _SimpleDimmerStepSizeAuto;
        private int _SimpleDimmerStepTimeAuto;
        private int _SimpleDimmerStepTimeManual;
        private int _SingleDimmingStepTimeAuto;
        private int _TimerFunctionAutoOff;

        public FXD211ConfigViewModel(IFrameNavigationService navigationService,
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
            GetSceneIdSentAG1HoldingS2Command = new RelayCommand<object>(async (obj) => await GetSceneIdSentAG1HoldingS2(obj));
            SetSceneIdSentAG1HoldingS2Command = new RelayCommand<object>(async (obj) => await SetSceneIdSentAG1HoldingS2(obj));
            GetSavedStatusCommand = new RelayCommand<object>(async (obj) => await GetSavedStatus(obj));
            SetSavedStatusCommand = new RelayCommand<object>(async (obj) => await SetSavedStatus(obj));
            GetThresholdCurrentLoadCautionCommand = new RelayCommand<object>(async (obj) => await GetSavedStatus(obj));
            SetThresholdCurrentLoadCautionCommand = new RelayCommand<object>(async (obj) => await SetSavedStatus(obj));
            GetExternalkeyTypeCommand = new RelayCommand<object>(async (obj) => await GetExternalkeyType(obj));
            SetExternalkeyTypeCommand = new RelayCommand<object>(async (obj) => await SetExternalkeyType(obj));
            GetPositionSwitchToggleBistavelCommand = new RelayCommand<object>(async (obj) => await GetPositionSwitchToggleBistavel(obj));
            SetPositionSwitchToggleBistavelCommand = new RelayCommand<object>(async (obj) => await SetPositionSwitchToggleBistavel(obj));
            GetAllOnAllOffCommand = new RelayCommand<object>(async (obj) => await GetAllOnAllOff(obj));
            SetAllOnAllOffCommand = new RelayCommand<object>(async (obj) => await SetAllOnAllOff(obj));
            GetMinimumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await GetMinimumBrightnessLevel(obj));
            SetMinimumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await SetMinimumBrightnessLevel(obj));
            GetMaximumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await GetMaximumBrightnessLevel(obj));
            SetMaximumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await SetMaximumBrightnessLevel(obj));
            GetSimpleDimmerStepTimeManualCommand = new RelayCommand<object>(async (obj) => await GetSimpleDimmerStepTimeManual(obj));
            SetSimpleDimmerStepTimeManualCommand = new RelayCommand<object>(async (obj) => await SetSimpleDimmerStepTimeManual(obj));
            GetSimpleDimmerPitchSizeManualCommand = new RelayCommand<object>(async (obj) => await GetSimpleDimmerPitchSizeManual(obj));
            SetSimpleDimmerPitchSizeManualCommand = new RelayCommand<object>(async (obj) => await SetSimpleDimmerPitchSizeManual(obj));
            GetSimpleDimmerStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await GetSimpleDimmerStepTimeAuto(obj));
            SetSimpleDimmerStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await SetSimpleDimmerStepTimeAuto(obj));
            GetSimpleDimmerStepSizeAutoCommand = new RelayCommand<object>(async (obj) => await GetSimpleDimmerStepSizeAuto(obj));
            SetSimpleDimmerStepSizeAutoCommand = new RelayCommand<object>(async (obj) => await SetSimpleDimmerStepSizeAuto(obj));
            GetSingleDimmingStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepTimeAuto(obj));
            SetSingleDimmingStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepTimeAuto(obj));
            GetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await GetTimerFunctionAutoOff(obj));
            SetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await SetMaxLevelShuttleOpen(obj));
            GetDimmingPercentageSingleTouchButtonCommand = new RelayCommand<object>(async (obj) => await GetDimmingPercentageSingleTouchButton(obj));
            SetDimmingPercentageSingleTouchButtonCommand = new RelayCommand<object>(async (obj) => await SetDimmingPercentageSingleTouchButton(obj));
            GetSceneIdSentAG1ClickS2Command = new RelayCommand<object>(async (obj) => await GetSceneIdSentAG1ClickS2(obj));
            SetSceneIdSentAG1S2DoubleClickCommand = new RelayCommand<object>(async (obj) => await SetSceneIdSentAG1S2DoubleClick(obj));
            GetSceneIdSentAG1S2DoubleClickCommand = new RelayCommand<object>(async (obj) => await GetSceneIdSentAG1S2DoubleClick(obj));
            SetSceneIdSentAG1ClickS2Command = new RelayCommand<object>(async (obj) => await SetSceneIdSentAG1ClickS2(obj));
            GetExternalSwitchTypeCommand = new RelayCommand<object>(async (obj) => await GetExternalSwitchType(obj));
            SetExternalSwitchTypeCommand = new RelayCommand<object>(async (obj) => await SetExternalSwitchType(obj));
            GetExternalSwitchInputCommand = new RelayCommand<object>(async (obj) => await GetExternalSwitchInput(obj));
            SetExternalSwitchInputCommand = new RelayCommand<object>(async (obj) => await SetExternalSwitchInput(obj));
            GetOneClickS1Command = new RelayCommand<object>(async (obj) => await GetOneClickS1(obj));
            SetOneClickS1Command = new RelayCommand<object>(async (obj) => await SetOneClickS1(obj));
            GetS2FunctionsCommand = new RelayCommand<object>(async (obj) => await GetS2Functions(obj));
            SetS2FunctionsCommand = new RelayCommand<object>(async (obj) => await SetS2Functions(obj));
            GetDoubleClickS1Command = new RelayCommand<object>(async (obj) => await GetDoubleClickS1(obj));
            SetDoubleClickS1Command = new RelayCommand<object>(async (obj) => await SetDoubleClickS1(obj));
            SetFactorySettingCommand = new RelayCommand<object>(async (obj) => await SetFactorySetting(obj));
            GetAllCommand = new RelayCommand<object>(async (obj) => await GetAll(obj));
            SetFactorySettingAwaitCommand = new RelayCommand<object>((obj) => SetFactorySettingAwait(obj));

            OpenDocumentationCommand = new RelayCommand<object>(OpenDocumentation);
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

        public int DoubleClickS1
        {
            get => _DoubleClickS1;
            set
            {
                if (Equals(_DoubleClickS1, value))
                {
                    return;
                }
                _ = SetProperty(ref _DoubleClickS1, value);
            }
        }

        public int ExternalkeyType
        {
            get => _ExternalkeyType;
            set
            {
                if (Equals(_ExternalkeyType, value))
                {
                    return;
                }
                _ = SetProperty(ref _ExternalkeyType, value);
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

        public bool IsSavedStatus
        {
            get => _IsSavedStatus;
            set
            {
                if (Equals(_IsSavedStatus, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsSavedStatus, value);
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

        public int OneClickS1
        {
            get => _OneClickS1;
            set
            {
                if (Equals(_OneClickS1, value))
                {
                    return;
                }
                _ = SetProperty(ref _OneClickS1, value);
            }
        }

        public int PositionSwitchToggleBistavel
        {
            get => _PositionSwitchToggleBistavel;
            set
            {
                if (Equals(_PositionSwitchToggleBistavel, value))
                {
                    return;
                }
                _ = SetProperty(ref _PositionSwitchToggleBistavel, value);
            }
        }

        public int S2Functions
        {
            get => _S2Functions;
            set
            {
                if (Equals(_S2Functions, value))
                {
                    return;
                }
                _ = SetProperty(ref _S2Functions, value);
            }
        }

        public int SceneIdSentAG1ClickS2
        {
            get => _SceneIdSentAG1ClickS2;
            set
            {
                if (Equals(_SceneIdSentAG1ClickS2, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneIdSentAG1ClickS2, value);
            }
        }

        public int SceneIdSentAG1HoldingS2
        {
            get => _SceneIdSentAG1HoldingS2;
            set
            {
                if (Equals(_SceneIdSentAG1HoldingS2, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneIdSentAG1HoldingS2, value);
            }
        }

        public int SceneIdSentAG1S2DoubleClick
        {
            get => _SceneIdSentAG1S2DoubleClick;
            set
            {
                if (Equals(_SceneIdSentAG1S2DoubleClick, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneIdSentAG1S2DoubleClick, value);
            }
        }

        public int SimpleDimmerPitchSizeManual
        {
            get => _SimpleDimmerPitchSizeManual;
            set
            {
                if (Equals(_SimpleDimmerPitchSizeManual, value))
                {
                    return;
                }
                _ = SetProperty(ref _SimpleDimmerPitchSizeManual, value);
            }
        }

        public int SimpleDimmerStepSizeAuto
        {
            get => _SimpleDimmerStepSizeAuto;
            set
            {
                if (Equals(_SimpleDimmerStepSizeAuto, value))
                {
                    return;
                }
                _ = SetProperty(ref _SimpleDimmerStepSizeAuto, value);
            }
        }

        public int SimpleDimmerStepTimeAuto
        {
            get => _SimpleDimmerStepTimeAuto;
            set
            {
                if (Equals(_SimpleDimmerStepTimeAuto, value))
                {
                    return;
                }
                _ = SetProperty(ref _SimpleDimmerStepTimeAuto, value);
            }
        }

        public int SimpleDimmerStepTimeManual
        {
            get => _SimpleDimmerStepTimeManual;
            set
            {
                if (Equals(_SimpleDimmerStepTimeManual, value))
                {
                    return;
                }
                _ = SetProperty(ref _SimpleDimmerStepTimeManual, value);
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

        private async Task GetAll(object obj)
        {
            if (!await GetSavedStatus(obj))
            {
                return;
            }
            if (!await GetPositionSwitchToggleBistavel(obj))
            {
                return;
            }
            if (!await GetAllOnAllOff(obj))
            {
                return;
            }
            if (!await GetMinimumBrightnessLevel(obj))
            {
                return;
            }
            if (!await GetMaximumBrightnessLevel(obj))
            {
                return;
            }
            if (!await GetSimpleDimmerStepTimeManual(obj))
            {
                return;
            }
            if (!await GetSimpleDimmerPitchSizeManual(obj))
            {
                return;
            }
            if (!await GetSimpleDimmerStepTimeAuto(obj))
            {
                return;
            }
            if (!await GetSimpleDimmerStepSizeAuto(obj))
            {
                return;
            }
            if (!await GetTimerFunctionAutoOff(obj))
            {
                return;
            }
            if (!await GetOneClickS1(obj))
            {
                return;
            }
            if (!await GetDoubleClickS1(obj))
            {
                return;
            }
            if (!await GetS2Functions(obj))
            {
                return;
            }
            if (!await GetSceneIdSentAG1ClickS2(obj))
            {
                return;
            }
            if (!await GetSceneIdSentAG1S2DoubleClick(obj))
            {
                return;
            }
            if (!await GetSceneIdSentAG1HoldingS2(obj))
            {
                return;
            }
            if (!await GetSceneIdSentAG1HoldingS2(obj))
            {
                return;
            }
        }

        #region GET

        /// <summary>
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetAllOnAllOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                AllOnAllOff = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 14
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetDoubleClickS1(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DoubleClickS1 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetExternalkeyType(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ExternalkeyType = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 13
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetOneClickS1(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                OneClickS1 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetPositionSwitchToggleBistavel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                PositionSwitchToggleBistavel = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 14
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetS2Functions(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                S2Functions = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 17);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetSavedStatus(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsSavedStatus = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetSceneIdSentAG1ClickS2(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SceneIdSentAG1ClickS2 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                             moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                             parameter: 19);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 21
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetSceneIdSentAG1HoldingS2(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SceneIdSentAG1HoldingS2 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 21);
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
        private async Task<bool> GetSceneIdSentAG1S2DoubleClick(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SceneIdSentAG1S2DoubleClick = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 19);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetSimpleDimmerPitchSizeManual(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SimpleDimmerPitchSizeManual = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 19
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 10
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetSimpleDimmerStepSizeAuto(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SimpleDimmerStepSizeAuto = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetSimpleDimmerStepTimeAuto(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SimpleDimmerStepTimeAuto = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetSimpleDimmerStepTimeManual(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SimpleDimmerStepTimeManual = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// <summary>
        /// Param 8
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 5
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 3
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 1
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        #endregion GET

        #region SET

        /// <summary>
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetAllOnAllOff(object obj)
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
                                                      AllOnAllOff);

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
        /// Param 14
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetDoubleClickS1(object obj)
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
                                                      DoubleClickS1);

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
        private async Task<bool> SetExternalkeyType(object obj)
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
                                                      ExternalkeyType);
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
                                                      11,
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
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetOneClickS1(object obj)
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
                                                      OneClickS1);

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
        private async Task<bool> SetPositionSwitchToggleBistavel(object obj)
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
                                                      PositionSwitchToggleBistavel);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 17
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetS2Functions(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      17,
                                                      1,
                                                      S2Functions);

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
        private async Task<bool> SetSavedStatus(object obj)
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
                                                      IsSavedStatus ? 1 : 0);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 19
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetSceneIdSentAG1ClickS2(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      19,
                                                      1,
                                                      SceneIdSentAG1ClickS2);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 21
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetSceneIdSentAG1HoldingS2(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      21,
                                                      1,
                                                      SceneIdSentAG1HoldingS2);

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 19
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetSceneIdSentAG1S2DoubleClick(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      19,
                                                      1,
                                                      SceneIdSentAG1S2DoubleClick);

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
        private async Task<bool> SetSimpleDimmerPitchSizeManual(object obj)
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
                                                      SimpleDimmerPitchSizeManual);
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
        private async Task<bool> SetSimpleDimmerStepSizeAuto(object obj)
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
                                                      SimpleDimmerStepSizeAuto);
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
        private async Task<bool> SetSimpleDimmerStepTimeAuto(object obj)
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
                                                      SimpleDimmerStepTimeAuto);
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
        private async Task<bool> SetSimpleDimmerStepTimeManual(object obj)
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
                                                      SimpleDimmerStepTimeManual);
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

        public ICommand GetAllCommand { get; set; }
        public ICommand GetAllOnAllOffCommand { get; set; }
        public ICommand GetAmpereAlarmThresholdCommand { get; set; }
        public ICommand GetAutoLoadOffWhenOverloadCommand { get; set; }
        public ICommand GetDetectionAmpereOverloadCommand { get; set; }
        public ICommand GetDetectionPowerOverloadCommand { get; set; }
        public ICommand GetDetectionVoltageOverloadCommand { get; set; }
        public ICommand GetDimmingPercentageSingleTouchButtonCommand { get; set; }
        public ICommand GetDoubleClickS1Command { get; set; }
        public ICommand GetEnergyMeteringReportingCommand { get; set; }
        public ICommand GetEnergyMeteringReportingIntervalCommand { get; set; }
        public ICommand GetExternalkeyTypeCommand { get; set; }
        public ICommand GetExternalSwitchInputCommand { get; set; }
        public ICommand GetExternalSwitchTypeCommand { get; set; }
        public ICommand GetInstantaneouscConsumptionAlarmThresholdCommand { get; set; }
        public ICommand GetLevelReportModeCommand { get; set; }
        public ICommand GetMaximumBrightnessLevelCommand { get; set; }
        public ICommand GetMaxLevelShuttleOpenCommand { get; set; }
        public ICommand GetMinimumBrightnessLevelCommand { get; set; }
        public ICommand GetOneClickS1Command { get; set; }
        public ICommand GetOverloadTimeBeforeLoadOffCommand { get; set; }
        public ICommand GetOverloadTimeCommand { get; set; }
        public ICommand GetPositionSwitchToggleBistavelCommand { get; set; }
        public ICommand GetS2FunctionsCommand { get; set; }
        public ICommand GetSavedStatusCommand { get; set; }
        public ICommand GetSceneIdSentAG1ClickS2Command { get; set; }
        public ICommand GetSceneIdSentAG1HoldingS2Command { get; set; }
        public ICommand GetSceneIdSentAG1S2DoubleClickCommand { get; set; }
        public ICommand GetSimpleDimmerPitchSizeManualCommand { get; set; }
        public ICommand GetSimpleDimmerStepSizeAutoCommand { get; set; }
        public ICommand GetSimpleDimmerStepTimeAutoCommand { get; set; }
        public ICommand GetSimpleDimmerStepTimeManualCommand { get; set; }
        public ICommand GetSingleDimmingStepTimeAutoCommand { get; set; }
        public ICommand GetThresholdCurrentLoadCautionCommand { get; set; }
        public ICommand GetTimerFunctionAutoOffCommand { get; set; }
        public ICommand GetVoltageAlarmThresholdCommand { get; set; }
        public ICommand OpenDocumentationCommand { get; set; }
        public ICommand SetAllOnAllOffCommand { get; set; }
        public ICommand SetAmpereAlarmThresholdCommand { get; set; }
        public ICommand SetAutoLoadOffWhenOverloadCommand { get; set; }
        public ICommand SetDimmingPercentageSingleTouchButtonCommand { get; set; }
        public ICommand SetDoubleClickS1Command { get; set; }
        public ICommand SetEnergyMeteringReportingCommand { get; set; }
        public ICommand SetEnergyMeteringReportingIntervalCommand { get; set; }
        public ICommand SetExternalkeyTypeCommand { get; set; }
        public ICommand SetExternalSwitchInputCommand { get; set; }
        public ICommand SetExternalSwitchTypeCommand { get; set; }
        public ICommand SetFactorySettingAwaitCommand { get; set; }
        public ICommand SetFactorySettingCommand { get; set; }
        public ICommand SetInstantaneouscConsumptionAlarmThresholdCommand { get; set; }
        public ICommand SetLevelReportModeCommand { get; set; }
        public ICommand SetMaximumBrightnessLevelCommand { get; set; }
        public ICommand SetMaxLevelShuttleOpenCommand { get; set; }
        public ICommand SetMinimumBrightnessLevelCommand { get; set; }
        public ICommand SetOneClickS1Command { get; set; }
        public ICommand SetOverloadTimeBeforeLoadOffCommand { get; set; }
        public ICommand SetPositionSwitchToggleBistavelCommand { get; set; }
        public ICommand SetS2FunctionsCommand { get; set; }
        public ICommand SetSavedStatusCommand { get; set; }
        public ICommand SetSceneIdSentAG1ClickS2Command { get; set; }
        public ICommand SetSceneIdSentAG1HoldingS2Command { get; set; }
        public ICommand SetSceneIdSentAG1S2DoubleClickCommand { get; set; }
        public ICommand SetSimpleDimmerPitchSizeManualCommand { get; set; }
        public ICommand SetSimpleDimmerStepSizeAutoCommand { get; set; }
        public ICommand SetSimpleDimmerStepTimeAutoCommand { get; set; }
        public ICommand SetSimpleDimmerStepTimeManualCommand { get; set; }
        public ICommand SetSingleDimmingStepTimeAutoCommand { get; set; }
        public ICommand SetThresholdCurrentLoadCautionCommand { get; set; }
        public ICommand SetTimerFunctionAutoOffCommand { get; set; }
        public ICommand SetVoltageAlarmThresholdCommand { get; set; }

        #endregion ICommand

        private void OpenDocumentation(object obj)
        {
            _ = Process.Start(new ProcessStartInfo("http://www.mcohome.com/ProductDetail/3897788.html"));
        }
    }
}