using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Zwave.Config.Flex
{
    public class FXR5011ConfigViewModel : ProjectViewModel
    {
        private readonly CountDownTimer count = new();
        private int _allOnAllOff;
        private int _BasicCCIntegrationSetting;
        private int _DimmingDurationsFrom;
        private int _DisableLocalControl;
        private int _FactorySetting = 255;
        private bool _isSwitchStateSaved;
        private int _Key2ActivateSceneId;
        private int _Key2SceneActivateModeSetting;
        private int _Key3activateSceneDuration;
        private int _Key3ActivateSceneId;
        private int _Key3SceneActivateModeSetting;
        private int _Key4ActivateSceneDuration;
        private int _Key4ActivateSceneId;
        private int _Key4SceneActivateModeSetting;
        private int _KeyMode;
        private int _KeyOneActivateSceneDuration;
        private int _KeyOneActivateSceneId;
        private int _KeyOneSceneActivateModeSetting;
        private int _LEDBacklightBrightness;
        private int _OnOffStateDuration;
        private int _SceneRespond;
        private int _SceneRespondId101150;
        private int _SceneRespondId150;
        private int _SceneRespondId151200;
        private int _SceneRespondId201250;
        private int _SceneRespondId51100;

        public FXR5011ConfigViewModel(IFrameNavigationService navigationService,
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
            GetSwitchStateSavedCommand = new RelayCommand<object>((obj) => GetSwitchStateSaved(obj));
            SetSwitchStateSavedCommand = new RelayCommand<object>((obj) => SetSwitchStateSaved(obj));
            GetAllOnAllOffCommand = new RelayCommand<object>((obj) => GetAllOnAllOff(obj));
            SetAllOnAllOffCommand = new RelayCommand<object>((obj) => SetAllOnAllOff(obj));
            GetLEDBacklightBrightnessCommand = new RelayCommand<object>((obj) => GetLEDBacklightBrightness(obj));
            SetLEDBacklightBrightnessCommand = new RelayCommand<object>((obj) => SetLEDBacklightBrightness(obj));
            GetKeyModeCommand = new RelayCommand<object>((obj) => GetKeyMode(obj));
            SetKeyModeCommand = new RelayCommand<object>((obj) => SetKeyMode(obj));
            GetOnOffStateDurationCommand = new RelayCommand<object>((obj) => GetOnOffStateDuration(obj));
            SetOnOffStateDurationCommand = new RelayCommand<object>((obj) => SetOnOffStateDuration(obj));
            GetBasicCCIntegrationSettingCommand = new RelayCommand<object>((obj) => GetBasicCCIntegrationSetting(obj));
            SetBasicCCIntegrationSettingCommand = new RelayCommand<object>((obj) => SetBasicCCIntegrationSetting(obj));
            GetDisableLocalControlCommand = new RelayCommand<object>(async (obj) => await GetDisableLocalControl(obj));
            SetDisableLocalControlCommand = new RelayCommand<object>(async (obj) => await SetDisableLocalControl(obj));
            GetSceneRespondCommand = new RelayCommand<object>((obj) => GetSceneRespond(obj));
            SetSceneRespondCommand = new RelayCommand<object>((obj) => SetSceneRespond(obj));
            GetKeyOneSceneActivateModeSettingCommand = new RelayCommand<object>((obj) => GetKeyOneSceneActivateModeSetting(obj));
            SetKeyOneSceneActivateModeSettingCommand = new RelayCommand<object>((obj) => SetKeyOneSceneActivateModeSetting(obj));
            GetKeyOneActivateSceneIdCommand = new RelayCommand<object>((obj) => GetKeyOneActivateSceneId(obj));
            SetKeyOneActivateSceneIdCommand = new RelayCommand<object>((obj) => SetKeyOneActivateSceneId(obj));
            GetKeyOneActivateSceneDurationCommand = new RelayCommand<object>((obj) => GetKeyOneActivateSceneDuration(obj));
            SetKeyOneActivateSceneDurationCommand = new RelayCommand<object>((obj) => SetKeyOneActivateSceneDuration(obj));
            GetKey2SceneActivateModeSettingCommand = new RelayCommand<object>((obj) => GetKey2SceneActivateModeSetting(obj));
            SetKey2SceneActivateModeSettingCommand = new RelayCommand<object>((obj) => SetKey2SceneActivateModeSetting(obj));
            GetKey2ActivateSceneIdCommand = new RelayCommand<object>((obj) => GetKey2ActivateSceneId(obj));
            SetKey2ActivateSceneIdCommand = new RelayCommand<object>((obj) => SetKey2ActivateSceneId(obj));
            GetDimmingDurationsFromCommand = new RelayCommand<object>((obj) => GetDimmingDurationsFrom(obj));
            SetDimmingDurationsFromCommand = new RelayCommand<object>((obj) => SetDimmingDurationsFrom(obj));
            GetKey3SceneActivateModeSettingCommand = new RelayCommand<object>((obj) => GetKey3SceneActivateModeSetting(obj));
            SetKey3SceneActivateModeSettingCommand = new RelayCommand<object>((obj) => SetKey3SceneActivateModeSetting(obj));
            GetKey3ActivateSceneIdCommand = new RelayCommand<object>((obj) => GetKey3ActivateSceneId(obj));
            SetKey3ActivateSceneIdCommand = new RelayCommand<object>((obj) => SetKey3ActivateSceneId(obj));
            GetKey3activateSceneDurationCommand = new RelayCommand<object>((obj) => GetKey3activateSceneDuration(obj));
            SetKey3activateSceneDurationCommand = new RelayCommand<object>((obj) => SetKey3activateSceneDuration(obj));
            GetKey4SceneActivateModeSettingCommand = new RelayCommand<object>((obj) => GetKey4SceneActivateModeSetting(obj));
            SetKey4SceneActivateModeSettingCommand = new RelayCommand<object>((obj) => SetKey4SceneActivateModeSetting(obj));
            GetKey4ActivateSceneIdCommand = new RelayCommand<object>((obj) => GetKey4ActivateSceneId(obj));
            SetKey4ActivateSceneIdCommand = new RelayCommand<object>((obj) => SetKey4ActivateSceneId(obj));
            GetKey4ActivateSceneDurationCommand = new RelayCommand<object>((obj) => GetKey4ActivateSceneDuration(obj));
            SetKey4ActivateSceneDurationCommand = new RelayCommand<object>((obj) => SetKey4ActivateSceneDuration(obj));
            GetSceneRespondId150Command = new RelayCommand<object>((obj) => GetSceneRespondId150(obj));
            SetSceneRespondId150Command = new RelayCommand<object>((obj) => SetSceneRespondId150(obj));
            GetSceneRespondId51100Command = new RelayCommand<object>((obj) => GetSceneRespondId51100(obj));
            SetSceneRespondId51100Command = new RelayCommand<object>((obj) => SetSceneRespondId51100(obj));
            GetSceneRespondId101150Command = new RelayCommand<object>((obj) => GetSceneRespondId101150(obj));
            SetSceneRespondId101150Command = new RelayCommand<object>((obj) => SetSceneRespondId101150(obj));
            GetSceneRespondId151200Command = new RelayCommand<object>((obj) => GetSceneRespondId151200(obj));
            SetSceneRespondId151200Command = new RelayCommand<object>((obj) => SetSceneRespondId151200(obj));
            GetSceneRespondId201250Command = new RelayCommand<object>((obj) => GetSceneRespondId201250(obj));
            SetSceneRespondId201250Command = new RelayCommand<object>((obj) => SetSceneRespondId201250(obj));
            SetFactorySettingCommand = new RelayCommand<object>((obj) => SetFactorySetting(obj));
        }

        public int AllOnAllOff
        {
            get => _allOnAllOff;
            set
            {
                if (Equals(_allOnAllOff, value))
                {
                    return;
                }
                _ = SetProperty(ref _allOnAllOff, value);
            }
        }

        public int BasicCCIntegrationSetting
        {
            get => _BasicCCIntegrationSetting;
            set
            {
                if (Equals(_BasicCCIntegrationSetting, value))
                {
                    return;
                }
                _ = SetProperty(ref _BasicCCIntegrationSetting, value);
            }
        }

        public int DimmingDurationsFrom
        {
            get => _DimmingDurationsFrom;
            set
            {
                if (Equals(_DimmingDurationsFrom, value))
                {
                    return;
                }
                _ = SetProperty(ref _DimmingDurationsFrom, value);
            }
        }

        public int DisableLocalControl
        {
            get => _DisableLocalControl;
            set
            {
                if (Equals(_DisableLocalControl, value))
                {
                    return;
                }
                _ = SetProperty(ref _DisableLocalControl, value);
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

        public bool IsSwitchStateSaved
        {
            get => _isSwitchStateSaved;
            set
            {
                if (Equals(_isSwitchStateSaved, value))
                {
                    return;
                }
                _ = SetProperty(ref _isSwitchStateSaved, value);
            }
        }

        public int Key2ActivateSceneId
        {
            get => _Key2ActivateSceneId;
            set
            {
                if (Equals(_Key2ActivateSceneId, value))
                {
                    return;
                }
                _ = SetProperty(ref _Key2ActivateSceneId, value);
            }
        }

        public int Key2SceneActivateModeSetting
        {
            get => _Key2SceneActivateModeSetting;
            set
            {
                if (Equals(_Key2SceneActivateModeSetting, value))
                {
                    return;
                }
                _ = SetProperty(ref _Key2SceneActivateModeSetting, value);
            }
        }

        public int Key3activateSceneDuration
        {
            get => _Key3activateSceneDuration;
            set
            {
                if (Equals(_Key3activateSceneDuration, value))
                {
                    return;
                }
                _ = SetProperty(ref _Key3activateSceneDuration, value);
            }
        }

        public int Key3ActivateSceneId
        {
            get => _Key3ActivateSceneId;
            set
            {
                if (Equals(_Key3ActivateSceneId, value))
                {
                    return;
                }
                _ = SetProperty(ref _Key3ActivateSceneId, value);
            }
        }

        public int Key3SceneActivateModeSetting
        {
            get => _Key3SceneActivateModeSetting;
            set
            {
                if (Equals(_Key3SceneActivateModeSetting, value))
                {
                    return;
                }
                _ = SetProperty(ref _Key3SceneActivateModeSetting, value);
            }
        }

        public int Key4ActivateSceneDuration
        {
            get => _Key4ActivateSceneDuration;
            set
            {
                if (Equals(_Key4ActivateSceneDuration, value))
                {
                    return;
                }
                _ = SetProperty(ref _Key4ActivateSceneDuration, value);
            }
        }

        public int Key4ActivateSceneId
        {
            get => _Key4ActivateSceneId;
            set
            {
                if (Equals(_Key4ActivateSceneId, value))
                {
                    return;
                }
                _ = SetProperty(ref _Key4ActivateSceneId, value);
            }
        }

        public int Key4SceneActivateModeSetting
        {
            get => _Key4SceneActivateModeSetting;
            set
            {
                if (Equals(_Key4SceneActivateModeSetting, value))
                {
                    return;
                }
                _ = SetProperty(ref _Key4SceneActivateModeSetting, value);
            }
        }

        public int KeyMode
        {
            get => _KeyMode;
            set
            {
                if (Equals(_KeyMode, value))
                {
                    return;
                }
                _ = SetProperty(ref _KeyMode, value);
            }
        }

        public int KeyOneActivateSceneDuration
        {
            get => _KeyOneActivateSceneDuration;
            set
            {
                if (Equals(_KeyOneActivateSceneDuration, value))
                {
                    return;
                }
                _ = SetProperty(ref _KeyOneActivateSceneDuration, value);
            }
        }

        public int KeyOneActivateSceneId
        {
            get => _KeyOneActivateSceneId;
            set
            {
                if (Equals(_KeyOneActivateSceneId, value))
                {
                    return;
                }
                _ = SetProperty(ref _KeyOneActivateSceneId, value);
            }
        }

        public int KeyOneSceneActivateModeSetting
        {
            get => _KeyOneSceneActivateModeSetting;
            set
            {
                if (Equals(_KeyOneSceneActivateModeSetting, value))
                {
                    return;
                }
                _ = SetProperty(ref _KeyOneSceneActivateModeSetting, value);
            }
        }

        public int LEDBacklightBrightness
        {
            get => _LEDBacklightBrightness;
            set
            {
                if (Equals(_LEDBacklightBrightness, value))
                {
                    return;
                }
                _ = SetProperty(ref _LEDBacklightBrightness, value);
            }
        }

        public int OnOffStateDuration
        {
            get => _OnOffStateDuration;
            set
            {
                if (Equals(_OnOffStateDuration, value))
                {
                    return;
                }
                _ = SetProperty(ref _OnOffStateDuration, value);
            }
        }

        public int SceneRespond
        {
            get => _SceneRespond;
            set
            {
                if (Equals(_SceneRespond, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneRespond, value);
            }
        }

        public int SceneRespondId101150
        {
            get => _SceneRespondId101150;
            set
            {
                if (Equals(_SceneRespondId101150, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneRespondId101150, value);
            }
        }

        public int SceneRespondId150
        {
            get => _SceneRespondId150;
            set
            {
                if (Equals(_SceneRespondId150, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneRespondId150, value);
            }
        }

        public int SceneRespondId151200
        {
            get => _SceneRespondId151200;
            set
            {
                if (Equals(_SceneRespondId151200, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneRespondId151200, value);
            }
        }

        public int SceneRespondId201250
        {
            get => _SceneRespondId201250;
            set
            {
                if (Equals(_SceneRespondId201250, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneRespondId201250, value);
            }
        }

        public int SceneRespondId51100
        {
            get => _SceneRespondId51100;
            set
            {
                if (Equals(_SceneRespondId51100, value))
                {
                    return;
                }
                _ = SetProperty(ref _SceneRespondId51100, value);
            }
        }

        #region ICommand

        public ICommand GetAllOnAllOffCommand { get; set; }
        public ICommand GetBasicCCIntegrationSettingCommand { get; set; }
        public ICommand GetDimmingDurationsFromCommand { get; set; }
        public ICommand GetDisableLocalControlCommand { get; set; }
        public ICommand GetKey2ActivateSceneIdCommand { get; set; }
        public ICommand GetKey2SceneActivateModeSettingCommand { get; set; }
        public ICommand GetKey3activateSceneDurationCommand { get; set; }
        public ICommand GetKey3ActivateSceneIdCommand { get; set; }
        public ICommand GetKey3SceneActivateModeSettingCommand { get; set; }
        public ICommand GetKey4ActivateSceneDurationCommand { get; set; }
        public ICommand GetKey4ActivateSceneIdCommand { get; set; }
        public ICommand GetKey4SceneActivateModeSettingCommand { get; set; }
        public ICommand GetKeyModeCommand { get; set; }
        public ICommand GetKeyOneActivateSceneDurationCommand { get; set; }
        public ICommand GetKeyOneActivateSceneIdCommand { get; set; }
        public ICommand GetKeyOneSceneActivateModeSettingCommand { get; set; }
        public ICommand GetLEDBacklightBrightnessCommand { get; set; }
        public ICommand GetOnOffStateDurationCommand { get; set; }
        public ICommand GetSceneRespondCommand { get; set; }
        public ICommand GetSceneRespondId101150Command { get; set; }
        public ICommand GetSceneRespondId150Command { get; set; }
        public ICommand GetSceneRespondId151200Command { get; set; }
        public ICommand GetSceneRespondId201250Command { get; set; }
        public ICommand GetSceneRespondId51100Command { get; set; }
        public ICommand GetSwitchStateSavedCommand { get; set; }
        public ICommand SetAllOnAllOffCommand { get; set; }
        public ICommand SetBasicCCIntegrationSettingCommand { get; set; }
        public ICommand SetDimmingDurationsFromCommand { get; set; }
        public ICommand SetDisableLocalControlCommand { get; set; }
        public ICommand SetFactorySettingCommand { get; set; }
        public ICommand SetKey2ActivateSceneIdCommand { get; set; }
        public ICommand SetKey2SceneActivateModeSettingCommand { get; set; }
        public ICommand SetKey3activateSceneDurationCommand { get; set; }
        public ICommand SetKey3ActivateSceneIdCommand { get; set; }
        public ICommand SetKey3SceneActivateModeSettingCommand { get; set; }
        public ICommand SetKey4ActivateSceneDurationCommand { get; set; }
        public ICommand SetKey4ActivateSceneIdCommand { get; set; }
        public ICommand SetKey4SceneActivateModeSettingCommand { get; set; }
        public ICommand SetKeyModeCommand { get; set; }
        public ICommand SetKeyOneActivateSceneDurationCommand { get; set; }
        public ICommand SetKeyOneActivateSceneIdCommand { get; set; }
        public ICommand SetKeyOneSceneActivateModeSettingCommand { get; set; }
        public ICommand SetLEDBacklightBrightnessCommand { get; set; }
        public ICommand SetOnOffStateDurationCommand { get; set; }
        public ICommand SetSceneRespondCommand { get; set; }
        public ICommand SetSceneRespondId101150Command { get; set; }
        public ICommand SetSceneRespondId150Command { get; set; }
        public ICommand SetSceneRespondId151200Command { get; set; }
        public ICommand SetSceneRespondId201250Command { get; set; }
        public ICommand SetSceneRespondId51100Command { get; set; }
        public ICommand SetSwitchStateSavedCommand { get; set; }

        #endregion ICommand

        #region Set

        /// <summary>
        /// Param 3
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetAllOnAllOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                string[] values = new string[] { "0", "1", "2", "255" };

                if (!values.Contains(AllOnAllOff + ""))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                        message: string.Format(Domain.Properties.Resources.Valid_values__0_, string.Join(", ", values)),
                                        cancel: async () => await CloseDialog(),
                                        textButtonCancel: Domain.Properties.Resources.Close);
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 3,
                                                          size: 1,
                                                          value: AllOnAllOff))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetAllOnAllOff(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetAllOnAllOff(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 8
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetBasicCCIntegrationSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 8,
                                                          size: 1,
                                                          value: BasicCCIntegrationSetting))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetBasicCCIntegrationSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetBasicCCIntegrationSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 22
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetDimmingDurationsFrom(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 22,
                                                          size: 1,
                                                          value: DimmingDurationsFrom))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetDimmingDurationsFrom(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetDimmingDurationsFrom(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 14
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> SetDisableLocalControl(object obj)
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
                                                      DisableLocalControl);
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
        private async void SetFactorySetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 85,
                                                          size: 1,
                                                          value: FactorySetting))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetFactorySetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetFactorySetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 21
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKey2ActivateSceneId(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 21,
                                                          size: 1,
                                                          value: Key2ActivateSceneId))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKey2ActivateSceneId(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKey2ActivateSceneId(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 20
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKey2SceneActivateModeSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 20,
                                                          size: 1,
                                                          value: Key2SceneActivateModeSetting))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKey2SceneActivateModeSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKey2SceneActivateModeSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 25
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKey3activateSceneDuration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 25,
                                                          size: 1,
                                                          value: Key3activateSceneDuration))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKey3activateSceneDuration(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKey3activateSceneDuration(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 24
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKey3ActivateSceneId(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 24,
                                                          size: 1,
                                                          value: Key3ActivateSceneId))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKey3ActivateSceneId(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKey3ActivateSceneId(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 23
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKey3SceneActivateModeSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 23,
                                                          size: 1,
                                                          value: Key3SceneActivateModeSetting))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKey3SceneActivateModeSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKey3SceneActivateModeSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 28
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKey4ActivateSceneDuration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 28,
                                                          size: 1,
                                                          value: Key4ActivateSceneDuration))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKey4ActivateSceneDuration(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKey4ActivateSceneDuration(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 27
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKey4ActivateSceneId(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 27,
                                                          size: 1,
                                                          value: Key4ActivateSceneId))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKey4ActivateSceneId(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKey4ActivateSceneId(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 26
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKey4SceneActivateModeSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 26,
                                                          size: 1,
                                                          value: Key4SceneActivateModeSetting))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKey4SceneActivateModeSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKey4SceneActivateModeSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 5
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKeyMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 5,
                                                          size: 1,
                                                          value: KeyMode))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKeyMode(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKeyMode(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 19
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKeyOneActivateSceneDuration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 19,
                                                          size: 1,
                                                          value: KeyOneActivateSceneDuration))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKeyOneActivateSceneDuration(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKeyOneActivateSceneDuration(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 18
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKeyOneActivateSceneId(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 18,
                                                          size: 1,
                                                          value: KeyOneActivateSceneId))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKeyOneActivateSceneId(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKeyOneActivateSceneId(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 17
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetKeyOneSceneActivateModeSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 17,
                                                          size: 1,
                                                          value: KeyOneSceneActivateModeSetting))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetKeyOneSceneActivateModeSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetKeyOneSceneActivateModeSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetLEDBacklightBrightness(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 4,
                                                          size: 1,
                                                          value: LEDBacklightBrightness))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetLEDBacklightBrightness(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetLEDBacklightBrightness(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetOnOffStateDuration(object obj)
        {
            //try
            //{
            //    if (!(obj is ProjectModel project))
            //    {
            //        return false;
            //    }

            //    await _zwaveRepository.SetZwaveConfig(project.SelectedGateway,
            //                                          project.SelectedGateway.SelectedZwaveDevice.ModuleId,
            //                                          6,
            //                                          2,
            //                                          OnOffStateDuration);
            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    ShowError(ex);
            //    return false;
            //}
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 6,
                                                          size: 2,
                                                          value: OnOffStateDuration))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetOnOffStateDuration(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetOnOffStateDuration(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 16
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetSceneRespond(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 16,
                                                          size: 1,
                                                          value: SceneRespond))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetSceneRespond(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetSceneRespond(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 34
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetSceneRespondId101150(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 34,
                                                          size: 1,
                                                          value: SceneRespondId101150))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetSceneRespondId101150(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetSceneRespondId101150(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 32
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetSceneRespondId150(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 32,
                                                          size: 1,
                                                          value: SceneRespondId150))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetSceneRespondId150(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetSceneRespondId150(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 35
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetSceneRespondId151200(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 35,
                                                          size: 1,
                                                          value: SceneRespondId151200))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetSceneRespondId151200(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetSceneRespondId151200(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 36
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetSceneRespondId201250(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 36,
                                                          size: 1,
                                                          value: SceneRespondId201250))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetSceneRespondId201250(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetSceneRespondId201250(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 33
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetSceneRespondId51100(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 33,
                                                          size: 1,
                                                          value: SceneRespondId51100))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetSceneRespondId51100(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetSceneRespondId51100(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void SetSwitchStateSaved(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: project,
                                                          parameter: 2,
                                                          size: 1,
                                                          value: IsSwitchStateSaved ? 1 : 0))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetSwitchStateSaved(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetSwitchStateSaved(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        #endregion Set

        #region Get

        /// <summary>
        /// Param 3
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetAllOnAllOff(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 3);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetAllOnAllOff(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                AllOnAllOff = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetAllOnAllOff(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 8
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetBasicCCIntegrationSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 8);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetBasicCCIntegrationSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                BasicCCIntegrationSetting = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetBasicCCIntegrationSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 22
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetDimmingDurationsFrom(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 22);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetDimmingDurationsFrom(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                DimmingDurationsFrom = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetDimmingDurationsFrom(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 14
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async Task<bool> GetDisableLocalControl(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return false;
                }

                DisableLocalControl = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
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
        /// Param 21
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKey2ActivateSceneId(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 21);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKey2ActivateSceneId(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                Key2ActivateSceneId = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKey2ActivateSceneId(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 20
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKey2SceneActivateModeSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 20);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKey2SceneActivateModeSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                Key2SceneActivateModeSetting = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKey2SceneActivateModeSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 25
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKey3activateSceneDuration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 25);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKey3activateSceneDuration(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                Key3activateSceneDuration = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKey3activateSceneDuration(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 24
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKey3ActivateSceneId(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 24);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKey3ActivateSceneId(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                Key3ActivateSceneId = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKey3ActivateSceneId(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 23
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKey3SceneActivateModeSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 23);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKey3SceneActivateModeSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                Key3SceneActivateModeSetting = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKey3SceneActivateModeSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 28
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKey4ActivateSceneDuration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 28);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKey4ActivateSceneDuration(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                Key4ActivateSceneDuration = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKey4ActivateSceneDuration(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 27
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKey4ActivateSceneId(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 27);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKey4ActivateSceneId(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                Key4ActivateSceneId = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKey4ActivateSceneId(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 26
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKey4SceneActivateModeSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 26);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKey4SceneActivateModeSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                Key4SceneActivateModeSetting = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKey4SceneActivateModeSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 5
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKeyMode(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 5);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKeyMode(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                KeyMode = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKeyMode(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 19
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKeyOneActivateSceneDuration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());

                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 19);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKeyOneActivateSceneDuration(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                KeyOneActivateSceneDuration = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKeyOneActivateSceneDuration(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 18
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKeyOneActivateSceneId(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 18);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKeyOneActivateSceneId(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                KeyOneActivateSceneId = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKeyOneActivateSceneId(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 17
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetKeyOneSceneActivateModeSetting(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());

                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 17);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetKeyOneSceneActivateModeSetting(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                KeyOneSceneActivateModeSetting = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetKeyOneSceneActivateModeSetting(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 4
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetLEDBacklightBrightness(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 4);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetLEDBacklightBrightness(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                LEDBacklightBrightness = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetLEDBacklightBrightness(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 6
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetOnOffStateDuration(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 6);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetOnOffStateDuration(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                OnOffStateDuration = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetOnOffStateDuration(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 16
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        ///
        private async void GetSceneRespond(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 16);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetSceneRespond(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                SceneRespond = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetSceneRespond(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 34
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetSceneRespondId101150(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 34);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetSceneRespondId101150(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                SceneRespondId101150 = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetSceneRespondId101150(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 32
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetSceneRespondId150(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 32);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetSceneRespondId150(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                SceneRespondId150 = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetSceneRespondId150(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 35
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetSceneRespondId151200(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 35);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetSceneRespondId151200(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                SceneRespondId151200 = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetSceneRespondId151200(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 36
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetSceneRespondId201250(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 36);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetSceneRespondId201250(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                SceneRespondId201250 = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetSceneRespondId201250(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 33
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetSceneRespondId51100(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());
                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 33);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetSceneRespondId51100(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                SceneRespondId51100 = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetSceneRespondId51100(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        /// <summary>
        /// Param 2
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void GetSwitchStateSaved(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                int @return = await _zwaveRepository.GetZwaveConfig(gateway: project.SelectedGateway,
                                                                    moduleId: project.SelectedGateway.SelectedZwaveDevice.ModuleId,
                                                                    parameter: 2);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, project.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetSwitchStateSaved(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                IsSwitchStateSaved = @return == 1;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetSwitchStateSaved(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        #endregion Get
    }
}