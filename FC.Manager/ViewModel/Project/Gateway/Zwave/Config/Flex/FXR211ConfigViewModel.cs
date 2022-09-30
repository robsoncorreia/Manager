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
    public class FXR211ConfigViewModel : ProjectViewModel
    {
        private int _AllOnAllOff;
        private int _AutoOff;
        private int _ExternalKeyType;
        private int _FactorySetting = 85;
        private bool _IsSwitchStateSavedWhenPowerFailure;
        private int _PositionSwitchToggleBistavel;
        private int _S2Functions;
        private int _SceneIdSentAG1S2Click;
        private int _SceneIdSentAG1S2DoubleClick;
        private int _SceneIdSentAG1WhenHoldingS2;
        private int _TimerFunctionAutoOff;

        public FXR211ConfigViewModel(IFrameNavigationService navigationService,
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
            GetPositionSwitchToggleBistavelCommand = new RelayCommand<object>(async (obj) => await GetPositionSwitchToggleBistavel(obj));
            SetPositionSwitchToggleBistavelCommand = new RelayCommand<object>(async (obj) => await SetPositionSwitchToggleBistavel(obj));
            GetSwitchStateSavedWhenPowerFailureCommand = new RelayCommand<object>(async (obj) => await GetSwitchStateSavedWhenPowerFailure(obj));
            SetSwitchStateSavedWhenPowerFailureCommand = new RelayCommand<object>(async (obj) => await SetSwitchStateSavedWhenPowerFailure(obj));
            GetExternalKeyTypeCommand = new RelayCommand<object>(async (obj) => await GetExternalKeyType(obj));
            SetExternalKeyTypeCommand = new RelayCommand<object>(async (obj) => await SetExternalKeyType(obj));
            GetAutoOffCommand = new RelayCommand<object>(async (obj) => await GetAutoOff(obj));
            SetAutoOffCommand = new RelayCommand<object>(async (obj) => await SetAutoOff(obj));
            GetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await GetTimerFunctionAutoOff(obj));
            SetTimerFunctionAutoOffCommand = new RelayCommand<object>(async (obj) => await SetTimerFunctionAutoOff(obj));
            GetAllOnAllOffCommand = new RelayCommand<object>(async (obj) => await GetAllOnAllOff(obj));
            SetAllOnAllOffCommand = new RelayCommand<object>(async (obj) => await SetAllOnAllOff(obj));
            GetS2FunctionsCommand = new RelayCommand<object>(async (obj) => await GetS2Functions(obj));
            SetS2FunctionsCommand = new RelayCommand<object>(async (obj) => await SetS2Functions(obj));
            GetSceneIdSentAG1S2ClickCommand = new RelayCommand<object>(async (obj) => await GetSceneIdSentAG1S2Click(obj));
            SetSceneIdSentAG1S2ClickCommand = new RelayCommand<object>(async (obj) => await SetSceneIdSentAG1S2Click(obj));
            GetSceneIdSentAG1S2DoubleClickCommand = new RelayCommand<object>(async (obj) => await GetSceneIdSentAG1S2DoubleClick(obj));
            SetSceneIdSentAG1S2DoubleClickCommand = new RelayCommand<object>(async (obj) => await SetSceneIdSentAG1S2DoubleClick(obj));
            GetSceneIdSentAG1WhenHoldingS2Command = new RelayCommand<object>(async (obj) => await GetSceneIdSentAG1WhenHoldingS2(obj));
            SetSceneIdSentAG1WhenHoldingS2Command = new RelayCommand<object>(async (obj) => await SetSceneIdSentAG1WhenHoldingS2(obj));
            SetFactorySettingAwaitCommand = new RelayCommand<object>((obj) => SetFactorySettingAwait(obj));
            GetAllCommand = new RelayCommand<object>(async (obj) => await GetAll(obj));

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

        public int ExternalKeyType
        {
            get => _ExternalKeyType;
            set
            {
                if (Equals(_ExternalKeyType, value))
                {
                    return;
                }
                _ = SetProperty(ref _ExternalKeyType, value);
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

        public bool IsSwitchStateSavedWhenPowerFailure
        {
            get => _IsSwitchStateSavedWhenPowerFailure;
            set
            {
                if (Equals(_IsSwitchStateSavedWhenPowerFailure, value))
                {
                    return;
                }
                _ = SetProperty(ref _IsSwitchStateSavedWhenPowerFailure, value);
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

        public int SceneIdSentAG1S2Click
        {
            get => _SceneIdSentAG1S2Click;
            set
            {
                if (Equals(_SceneIdSentAG1S2Click, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneIdSentAG1S2Click, value);
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

        public int SceneIdSentAG1WhenHoldingS2
        {
            get => _SceneIdSentAG1WhenHoldingS2;
            set
            {
                if (Equals(_SceneIdSentAG1WhenHoldingS2, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneIdSentAG1WhenHoldingS2, value);
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
            if (!await GetPositionSwitchToggleBistavel(obj))
            {
                return;
            }
            if (!await GetSwitchStateSavedWhenPowerFailure(obj))
            {
                return;
            }
            if (!await GetExternalKeyType(obj))
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
            if (!await GetAllOnAllOff(obj))
            {
                return;
            }
            if (!await GetS2Functions(obj))
            {
                return;
            }
            if (!await GetSceneIdSentAG1S2Click(obj))
            {
                return;
            }
            if (!await GetSceneIdSentAG1S2DoubleClick(obj))
            {
                return;
            }
            if (!await GetSceneIdSentAG1WhenHoldingS2(obj))
            {
                return;
            }
        }

        #region GET

        /// <summary>
        /// Param 13
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
                                                                             parameter: 6);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

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

        private async Task<bool> GetExternalKeyType(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                ExternalKeyType = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
                                                                             parameter: 9);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetSceneIdSentAG1S2Click(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SceneIdSentAG1S2Click = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
                                                                             parameter: 12);
                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private async Task<bool> GetSceneIdSentAG1WhenHoldingS2(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                SceneIdSentAG1WhenHoldingS2 = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 12
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 11
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// <summary>
        /// Param 9
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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

        private async Task<bool> GetSwitchStateSavedWhenPowerFailure(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                IsSwitchStateSavedWhenPowerFailure = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 6
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
                                                      6,
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
        /// Param 2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetExternalKeyType(object obj)
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
                                                      ExternalKeyType);
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
        /// Param 3
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
        /// Param 9
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
                                                      9,
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
        /// Param 11
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetSceneIdSentAG1S2Click(object obj)
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
                                                      SceneIdSentAG1S2Click);
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
                                                      12,
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
        /// Param 13
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetSceneIdSentAG1WhenHoldingS2(object obj)
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
                                                      SceneIdSentAG1WhenHoldingS2);
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
        private async Task<bool> SetSwitchStateSavedWhenPowerFailure(object obj)
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
                                                      IsSwitchStateSavedWhenPowerFailure ? 1 : 0);
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

        #endregion SET

        #region ICommand

        public ICommand GetAllCommand { get; set; }
        public ICommand GetAllOnAllOffCommand { get; set; }
        public ICommand GetAutoOffCommand { get; set; }
        public ICommand GetExternalKeyTypeCommand { get; set; }
        public ICommand GetPositionSwitchToggleBistavelCommand { get; set; }
        public ICommand GetS2FunctionsCommand { get; set; }
        public ICommand GetSceneIdSentAG1S2ClickCommand { get; set; }
        public ICommand GetSceneIdSentAG1S2DoubleClickCommand { get; set; }
        public ICommand GetSceneIdSentAG1WhenHoldingS2Command { get; set; }
        public ICommand GetSwitchStateSavedWhenPowerFailureCommand { get; set; }
        public ICommand GetTimerFunctionAutoOffCommand { get; set; }
        public ICommand OpenDocumentationCommand { get; set; }
        public ICommand SetAllOnAllOffCommand { get; set; }
        public ICommand SetAutoOffCommand { get; set; }
        public ICommand SetExternalKeyTypeCommand { get; set; }
        public ICommand SetFactorySettingAwaitCommand { get; set; }
        public ICommand SetPositionSwitchToggleBistavelCommand { get; set; }
        public ICommand SetS2FunctionsCommand { get; set; }
        public ICommand SetSceneIdSentAG1S2ClickCommand { get; set; }
        public ICommand SetSceneIdSentAG1S2DoubleClickCommand { get; set; }
        public ICommand SetSceneIdSentAG1WhenHoldingS2Command { get; set; }
        public ICommand SetSwitchStateSavedWhenPowerFailureCommand { get; set; }
        public ICommand SetTimerFunctionAutoOffCommand { get; set; }

        #endregion ICommand

        private void OpenDocumentation(object obj)
        {
            _ = Process.Start(new ProcessStartInfo("http://www.mcohome.com/ProductDetail/3897788.html"));
        }
    }
}