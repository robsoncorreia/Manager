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
    public class FXD5011ConfigViewModel : ProjectViewModel
    {
        private int _AllOnAllOff;
        private int _DoubleClickS1;
        private int _FactorySetting = 85;
        private bool _IsBeep;
        private bool _IsDimmerStateSaved;
        private int _LEDBacklitBrightnessLevel;
        private int _MaximumBrightnessLevel;
        private int _MinimumBrightnessLevel;
        private int _OneClickS1;
        private int _OperationReportSwitch1AssociationGroup_2;
        private int _OperationSwitch1AssociationGroup_2_3;
        private int _SceneIDSentAG1WhenHoldS1;
        private int _SceneIDSentAG1WhenOneClicksS1;
        private int _SceneSIDSentAG1WhenDoubleClickS1;
        private int _SingleDimmingStepSize;
        private int _SingleDimmingStepSizeAuto;
        private int _SingleDimmingStepTimeAuto;
        private int _SingleDimmingStepTimeManual;
        private int _TimerFunctionAutoOff;

        public FXD5011ConfigViewModel(IFrameNavigationService navigationService,
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
            GetDimmerStateSavedCommand = new RelayCommand<object>(async (obj) => await GetDimmerStateSaved(obj));
            SetDimmerStateSavedCommand = new RelayCommand<object>(async (obj) => await SetDimmerStateSaved(obj));
            GetBeepCommand = new RelayCommand<object>(async (obj) => await GetBeep(obj));
            SetBeepCommand = new RelayCommand<object>(async (obj) => await SetBeep(obj));
            GetLEDBacklitBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await GetLEDBacklitBrightnessLevel(obj));
            SetLEDBacklitBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await SetLEDBacklitBrightnessLevel(obj));
            GetAllOnAllOffCommand = new RelayCommand<object>(async (obj) => await GetAllOnAllOff(obj));
            SetAllOnAllOffCommand = new RelayCommand<object>(async (obj) => await SetAllOnAllOff(obj));
            GetMinimumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await GetMinimumBrightnessLevel(obj));
            SetMinimumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await SetMinimumBrightnessLevel(obj));
            GetMaximumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await GetMaximumBrightnessLevel(obj));
            SetMaximumBrightnessLevelCommand = new RelayCommand<object>(async (obj) => await SetMaximumBrightnessLevel(obj));
            GetSingleDimmingStepTimeManualCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepTimeManual(obj));
            SetSingleDimmingStepTimeManualCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepTimeManual(obj));
            GetSingleDimmingStepSizeCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepSize(obj));
            SetSingleDimmingStepSizeCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepSize(obj));
            GetSingleDimmingStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepTimeAuto(obj));
            SetSingleDimmingStepTimeAutoCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepTimeAuto(obj));
            GetSingleDimmingStepSizeAutoCommand = new RelayCommand<object>(async (obj) => await GetSingleDimmingStepSizeAuto(obj));
            SetSingleDimmingStepSizeAutoCommand = new RelayCommand<object>(async (obj) => await SetSingleDimmingStepSizeAuto(obj));
            GetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await GetTimerFunctionAutoOff(obj));
            SetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await SetTimerFunctionAutoOff(obj));
            GetOneClickS1Command = new RelayCommand<object>(async (obj) => await GetOneClickS1(obj));
            SetOneClickS1Command = new RelayCommand<object>(async (obj) => await SetOneClickS1(obj));
            GetDoubleClickS1Command = new RelayCommand<object>(async (obj) => await GetDoubleClickS1(obj));
            SetDoubleClickS1Command = new RelayCommand<object>(async (obj) => await SetDoubleClickS1(obj));
            GetOperationSwitch1AssociationGroup_2_3Command = new RelayCommand<object>(async (obj) => await GetOperationSwitch1AssociationGroup_2_3(obj));
            SetOperationSwitch1AssociationGroup_2_3Command = new RelayCommand<object>(async (obj) => await SetOperationSwitch1AssociationGroup_2_3(obj));
            GetOperationReportSwitch1AssociationGroup_2Command = new RelayCommand<object>(async (obj) => await GetOperationReportSwitch1AssociationGroup_2(obj));
            SetOperationReportSwitch1AssociationGroup_2Command = new RelayCommand<object>(async (obj) => await SetOperationReportSwitch1AssociationGroup_2(obj));
            GetSceneIDSentAG1WhenOneClicksS1Command = new RelayCommand<object>(async (obj) => await GetSceneIDSentAG1WhenOneClicksS1(obj));
            SetSceneIDSentAG1WhenOneClicksS1Command = new RelayCommand<object>(async (obj) => await SetSceneIDSentAG1WhenOneClicksS1(obj));
            GetSceneSIDSentAG1WhenDoubleClickS1Command = new RelayCommand<object>(async (obj) => await GetSceneSIDSentAG1WhenDoubleClickS1(obj));
            SetSceneSIDSentAG1WhenDoubleClickS1Command = new RelayCommand<object>(async (obj) => await SetSceneSIDSentAG1WhenDoubleClickS1(obj));
            GetSceneIDSentAG1WhenHoldS1Command = new RelayCommand<object>(async (obj) => await GetSceneIDSentAG1WhenHoldS1(obj));
            GetAllCommand = new RelayCommand<object>(async (obj) => await GetAll(obj));
            SetSceneIDSentAG1WhenHoldS1Command = new RelayCommand<object>(async (obj) => await SetSceneIDSentAG1WhenHoldS1(obj));
            SetFactorySettingCommand = new RelayCommand<object>((obj) => SetFactorySettingAwait(obj));
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

        public bool IsDimmerStateSaved
        {
            get => _IsDimmerStateSaved;
            set
            {
                if (Equals(_IsDimmerStateSaved, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsDimmerStateSaved, value);
            }
        }

        public int LEDBacklitBrightnessLevel
        {
            get => _LEDBacklitBrightnessLevel;
            set
            {
                if (Equals(_LEDBacklitBrightnessLevel, value))
                {
                    return;
                }
                _ = SetProperty(ref _LEDBacklitBrightnessLevel, value);
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

        public int OperationReportSwitch1AssociationGroup_2

        {
            get => _OperationReportSwitch1AssociationGroup_2;
            set
            {
                if (Equals(_OperationReportSwitch1AssociationGroup_2, value))
                {
                    return;
                }
                _ = SetProperty(ref _OperationReportSwitch1AssociationGroup_2, value);
            }
        }

        public int OperationSwitch1AssociationGroup_2_3

        {
            get => _OperationSwitch1AssociationGroup_2_3;
            set
            {
                if (Equals(_OperationSwitch1AssociationGroup_2_3, value))
                {
                    return;
                }
                _ = SetProperty(ref _OperationSwitch1AssociationGroup_2_3, value);
            }
        }

        public int SceneIDSentAG1WhenHoldS1

        {
            get => _SceneIDSentAG1WhenHoldS1;
            set
            {
                if (Equals(_SceneIDSentAG1WhenHoldS1, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneIDSentAG1WhenHoldS1, value);
            }
        }

        public int SceneIDSentAG1WhenOneClicksS1

        {
            get => _SceneIDSentAG1WhenOneClicksS1;
            set
            {
                if (Equals(_SceneIDSentAG1WhenOneClicksS1, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneIDSentAG1WhenOneClicksS1, value);
            }
        }

        public int SceneSIDSentAG1WhenDoubleClickS1
        {
            get => _SceneSIDSentAG1WhenDoubleClickS1;
            set
            {
                if (Equals(_SceneSIDSentAG1WhenDoubleClickS1, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneSIDSentAG1WhenDoubleClickS1, value);
            }
        }

        public int SingleDimmingStepSize
        {
            get => _SingleDimmingStepSize;
            set
            {
                if (Equals(_SingleDimmingStepSize, value))
                {
                    return;
                }
                _ = SetProperty(ref _SingleDimmingStepSize, value);
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
            if (!await GetDimmerStateSaved(obj))
            {
                return;
            }
            if (!await GetBeep(obj))
            {
                return;
            }
            if (!await GetLEDBacklitBrightnessLevel(obj))
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
            if (!await GetSingleDimmingStepTimeManual(obj))
            {
                return;
            }
            if (!await GetSingleDimmingStepSize(obj))
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
            if (!await GetOperationSwitch1AssociationGroup_2_3(obj))
            {
                return;
            }
            if (!await GetOperationReportSwitch1AssociationGroup_2(obj))
            {
                return;
            }
            if (!await GetSceneIDSentAG1WhenOneClicksS1(obj))
            {
                return;
            }
            if (!await GetSceneSIDSentAG1WhenDoubleClickS1(obj))
            {
                return;
            }
            if (!await GetSceneIDSentAG1WhenHoldS1(obj))
            {
                return;
            }
        }

        #region GET

        /// <summary>
        /// Param 21
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
        /// Param 2
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

        private async Task<bool> GetLEDBacklitBrightnessLevel(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                LEDBacklitBrightnessLevel = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetOperationReportSwitch1AssociationGroup_2(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                OperationReportSwitch1AssociationGroup_2 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetOperationSwitch1AssociationGroup_2_3(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                OperationSwitch1AssociationGroup_2_3 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetSceneIDSentAG1WhenHoldS1(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SceneIDSentAG1WhenHoldS1 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 20
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetSceneIDSentAG1WhenOneClicksS1(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SceneIDSentAG1WhenOneClicksS1 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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

        private async Task<bool> GetSceneSIDSentAG1WhenDoubleClickS1(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SceneSIDSentAG1WhenDoubleClickS1 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                      moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                      parameter: 20);
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
        /// Param 15
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 15
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 14
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 13
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 11
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>

        private async Task<bool> GetSingleDimmingStepSize(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SingleDimmingStepSize = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 5
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
        /// Param 10
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 8
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 7
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
        /// Param 85
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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
        /// Param 3
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetLEDBacklitBrightnessLevel(object obj)
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
                                                      LEDBacklitBrightnessLevel);
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
        /// Param 13
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
        /// Param 15
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetOperationReportSwitch1AssociationGroup_2(object obj)
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
                                                      1,
                                                      OperationReportSwitch1AssociationGroup_2);
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
        private async Task<bool> SetOperationSwitch1AssociationGroup_2_3(object obj)
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
                                                      OperationSwitch1AssociationGroup_2_3);
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
        private async Task<bool> SetSceneIDSentAG1WhenHoldS1(object obj)
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
                                                      SceneIDSentAG1WhenHoldS1);
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
        private async Task<bool> SetSceneIDSentAG1WhenOneClicksS1(object obj)
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
                                                      SceneIDSentAG1WhenOneClicksS1);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Param 20
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetSceneSIDSentAG1WhenDoubleClickS1(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
                                                      project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                      20,
                                                      1,
                                                      SceneSIDSentAG1WhenDoubleClickS1);
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
        private async Task<bool> SetSingleDimmingStepSize(object obj)
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
                                                      SingleDimmingStepSize);
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
        /// Param 11
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

        #endregion SET

        #region ICommand

        public ICommand GetAllCommand { get; set; }
        public ICommand GetAllOnAllOffCommand { get; set; }
        public ICommand GetBeepCommand { get; set; }
        public ICommand GetDimmerStateSavedCommand { get; set; }
        public ICommand GetDoubleClickS1Command { get; set; }
        public ICommand GetLEDBacklitBrightnessLevelCommand { get; set; }
        public ICommand GetMaximumBrightnessLevelCommand { get; set; }
        public ICommand GetMinimumBrightnessLevelCommand { get; set; }
        public ICommand GetOneClickS1Command { get; set; }
        public ICommand GetOperationReportSwitch1AssociationGroup_2Command { get; set; }
        public ICommand GetOperationSwitch1AssociationGroup_2_3Command { get; set; }
        public ICommand GetSceneIDSentAG1WhenHoldS1Command { get; set; }
        public ICommand GetSceneIDSentAG1WhenOneClicksS1Command { get; set; }
        public ICommand GetSceneSIDSentAG1WhenDoubleClickS1Command { get; set; }
        public ICommand GetSingleDimmingStepSizeAutoCommand { get; set; }
        public ICommand GetSingleDimmingStepSizeCommand { get; set; }
        public ICommand GetSingleDimmingStepTimeAutoCommand { get; set; }
        public ICommand GetSingleDimmingStepTimeManualCommand { get; set; }
        public ICommand GetTimerFunctionAutoOffCommand { get; set; }
        public ICommand OpenDocumentationCommand { get; set; }
        public ICommand SetAllOnAllOffCommand { get; set; }
        public ICommand SetBeepCommand { get; set; }
        public ICommand SetDimmerStateSavedCommand { get; set; }
        public ICommand SetDoubleClickS1Command { get; set; }
        public ICommand SetFactorySettingCommand { get; set; }
        public ICommand SetLEDBacklitBrightnessLevelCommand { get; set; }
        public ICommand SetMaximumBrightnessLevelCommand { get; set; }
        public ICommand SetMinimumBrightnessLevelCommand { get; set; }
        public ICommand SetOneClickS1Command { get; set; }
        public ICommand SetOperationReportSwitch1AssociationGroup_2Command { get; set; }
        public ICommand SetOperationSwitch1AssociationGroup_2_3Command { get; set; }
        public ICommand SetSceneIDSentAG1WhenHoldS1Command { get; set; }
        public ICommand SetSceneIDSentAG1WhenOneClicksS1Command { get; set; }
        public ICommand SetSceneSIDSentAG1WhenDoubleClickS1Command { get; set; }
        public ICommand SetSingleDimmingStepSizeAutoCommand { get; set; }
        public ICommand SetSingleDimmingStepSizeCommand { get; set; }
        public ICommand SetSingleDimmingStepTimeAutoCommand { get; set; }
        public ICommand SetSingleDimmingStepTimeManualCommand { get; set; }
        public ICommand SetTimerFunctionAutoOffCommand { get; set; }

        #endregion ICommand

        private void OpenDocumentation(object obj)
        {
            _ = Process.Start(new ProcessStartInfo("http://www.mcohome.com/ProductDetail/3894273.html"));
        }
    }
}