using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.FlexCloudClone;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Domain.Util;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class RTSDetailDeviceViewModel : ProjectViewModelBase
    {
        #region Fields

        private readonly ISettingsService _settingsService;
        private readonly CountDownTimer count = new();
        private bool _isConnection;
        private bool _isSendingDown;
        private bool _isSendingStop;
        private bool _isSendingUp;
        private bool _isSensor;
        private bool _isTerminalEnable;
        private int _radioCount;
        private RadioModel _selectedRadioModel;

        #endregion Fields

        public RTSDetailDeviceViewModel(IFrameNavigationService navigationService,
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
                                 ILoginRepository loginRepository,
                                 ISettingsService settingsService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            SelectedRadioModel = new RadioModel
            {
                TypeRF = TypeRF.RTS
            };

            AddSomfyCommand = new RelayCommand<object>(AddSomfy);

            SelectedRFCommand = new RelayCommand<object>(SelectedRF);

            SensorCommand = new RelayCommand(async () => await Sensor());

            SendActionCommand = new RelayCommand<object>(SendAction);

            NewCommand = new RelayCommand<object>(New);

            DeleteRFFromGatewayAwaitCommand = new RelayCommand<object>((obj) => DeleteRFFromGatewayAwait(obj));

            UpdatePICCommad = new RelayCommand<object>(UpdatePIC);

            DeleteAllFromGatewayAwaitCommand = new RelayCommand(() => DeleteAllFromGatewayAwait());

            GetFirmwareInfoPICCommand = new RelayCommand(async () => await GetFirmwareInfoPIC());

            LoadedCommand = new RelayCommand<object>(Loaded);

            GetAllGatewayCommand = new RelayCommand<object>(GetAllRFGateway);

            PlayRTSCommand = new RelayCommand<object>(PlayRTS);

            _gatewayService.PropertyChanged += GatewayServicePropertyChanged;

            _parseService.PropertyChanged += ParseServicePropertyChanged;

            _settingsService = settingsService;

            _settingsService.PropertyChanged += SettingsServicePropertyChanged;

            IsTerminalEnable = Domain.Properties.Settings.Default.enableTerminal;

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        #region ICommand

        public ICommand AddSomfyCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand DeleteAllFromGatewayAwaitCommand { get; set; }

        public ICommand DeleteRFFromGatewayAwaitCommand { get; set; }

        public ICommand GetAllGatewayCommand { get; set; }

        public ICommand GetFirmwareInfoPICCommand { get; set; }
        public ICommand NewCommand { get; set; }

        public ICommand PlayRTSCommand { get; set; }

        public ICommand SelectedRFCommand { get; set; }

        public ICommand SendActionCommand { get; set; }

        public ICommand SensorCommand { get; set; }

        public ICommand UpdatePICCommad { get; set; }

        #endregion ICommand

        public bool IsConnection
        {
            get => _isConnection;
            set => SetProperty(ref _isConnection, value);
        }

        public bool IsSendingDown
        {
            get => _isSendingDown;
            set
            => SetProperty(ref _isSendingDown, value);
        }

        public bool IsSendingStop
        {
            get => _isSendingStop;
            set
            => SetProperty(ref _isSendingStop, value);
        }

        public bool IsSendingUp
        {
            get => _isSendingUp;
            set
            => SetProperty(ref _isSendingUp, value);
        }

        public bool IsSensor
        {
            get => _isSensor;
            set
            => SetProperty(ref _isSensor, value);
        }

        public bool IsTerminalEnable
        {
            get => _isTerminalEnable;
            set => SetProperty(ref _isTerminalEnable, value);
        }

        public int RadioCount
        {
            get => _radioCount;
            set => SetProperty(ref _radioCount, value);
        }

        public RadioModel SelectedRadioModel
        {
            get => _selectedRadioModel;
            set
            => SetProperty(ref _selectedRadioModel, value);
        }

        #region Methods

        public async void PlayRTS(object obj)
        {
            try
            {
                if (obj is not ActionsRTSSomfy actionRTS)
                {
                    return;
                }

                SelectedRadioModel.ActionRTSSomfy = actionRTS;

                SelectedRadioModel.IsSending = true;

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.Sending;

                await _rfRepository.PlayRts(SelectedProjectModel.SelectedGateway, SelectedRadioModel);

                IsActiveSnackbar = false;

                ContentSnackbar = string.Format(Domain.Properties.Resources.Send_Sucess, Domain.Properties.Resources.Sent);

                IsActiveSnackbar = true;

                count.Reset();

                count.SetTime(1000);

                count.Start();

                count.CountDownFinished = () =>
                {
                    IsActiveSnackbar = false;
                };

                await CloseDialog();
            }
            catch (Exception ex)
            {
                IsActiveSnackbar = false;
                ShowError(ex);
            }
            finally
            {
                SelectedRadioModel.IsSending = false;
            }
        }

        private async void AddSomfy(object obj)
        {
            try
            {
                using ReplaySubject<string> replay = new();

                await _rfRepository.AddSomfy(SelectedProjectModel.SelectedGateway, SelectedRadioModel, replay);

                OpenCustomMessageBox(header: Domain.Properties.Resources.Learn,
                                     source: new Uri("/FC.Domain;component/Assets/programming_button.png", UriKind.Relative),
                                     message: Domain.Properties.Resources.Press_the_programming_button_on_the_back_of_control__then_press_the_prog_button_below,
                                     textButtonCustom: Domain.Properties.Resources.Prog,
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => PlayRTS(ActionsRTSSomfy.PROG));

                using RadioModel radio = new();

                SelectedRadioModel.CopyPropertiesTo(radio);

                radio.TypeRF = TypeRF.RTS;

                SelectedRadioModel = radio;

                _localDBRepository.Update(SelectedProjectModel.SelectedGateway, SelectedRadioModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task DeleteAllFromGateway()
        {
            try
            {
                await CloseDialog();

                RadioModel[] temp = new RadioModel[SelectedProjectModel.SelectedGateway.RadiosRTSGateway.Count];

                SelectedProjectModel.SelectedGateway.RadiosRTSGateway.CopyTo(temp, 0);

                foreach (RadioModel radio in temp)
                {
                    _ = await _rfRepository.DeleteAsync(SelectedProjectModel.SelectedGateway, radio);
                }

                SelectedRadioModel = new RadioModel();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAllFromGatewayAwait()
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete_All,
                                 message: Domain.Properties.Resources.Delete_All_RTS_From_Gateway,
                                 custom: async () => await DeleteAllFromGateway(),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 cancel: async () => await CloseDialog());
        }

        private async Task DeleteRFFromGateway(RadioModel radio)
        {
            try
            {
                await CloseDialog();

                _ = await _rfRepository.DeleteAsync(SelectedProjectModel.SelectedGateway, radio);

                SelectedRadioModel = new RadioModel();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteRFFromGatewayAwait(object obj)
        {
            if (obj is not RadioModel radio)
            {
                return;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Delete_RF, ((RadioModel)obj).MemoryId),
                                 custom: async () => await DeleteRFFromGateway(radio),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 cancel: async () => await CloseDialog());
        }

        private void GatewayServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(IsSending))
            //{
            //    IsSending = ((GatewayService)sender).IsSendingToGateway;
            //}
            if (e.PropertyName == nameof(LastCommandSend))
            {
                LastCommandSend = ((GatewayService)sender).LastCommandSend;
            }
            if (e.PropertyName == nameof(IsSendingToGateway))
            {
                IsSendingToGateway = ((GatewayService)sender).IsSendingToGateway;
            }
            if (e.PropertyName == nameof(RadioCount))
            {
                RadioCount = ((GatewayService)sender).RadioCount;
            }
        }

        private async void GetAllRFGateway(object obj = null)
        {
            try
            {
                SelectedRadioModel = new RadioModel();

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading,
                                     isProgressBar: true);

                await _rfRepository.GetAll(SelectedProjectModel.SelectedGateway, TypeRF.RTS);

                count.SetTime(1000);

                count.Reset();

                count.Start();

                count.CountDownFinished = async () =>
                {
                    await CloseDialog();
                };
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task GetFirmwareInfoPIC()
        {
            try
            {
                await _rfRepository.GetFirmwareInfoPICAsync(SelectedProjectModel.SelectedGateway);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            if (obj is not UserControl view)
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            if (SelectedProjectModel.SelectedGateway.RadiosRTSGateway.Any())
            {
                return;
            }

            GetAllRFGateway();
        }

        private void New(object obj)
        {
            SelectedRadioModel = new RadioModel
            {
                TypeRF = TypeRF.RTS
            };

            SelectedProjectModel.SelectedGateway.SelectedIndexRTS = -1;
        }

        private void ParseServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSendingToCloud))
            {
                IsSendingToCloud = ((ParseService)sender).IsSendingToCloud;
            }
        }

        private void SelectedRF(object obj)
        {
            if (obj is not RadioModel radio)
            {
                return;
            }

            radio.CopyPropertiesTo(SelectedRadioModel);
        }

        private async void SendAction(object obj)
        {
            try
            {
                if (obj is not ActionsRTSSomfy actionRTS)
                {
                    return;
                }

                SelectedRadioModel.ActionRTSSomfy = actionRTS;

                SelectedRadioModel.IsSending = true;

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.Sending;

                await _rfRepository.PlayRts(SelectedProjectModel.SelectedGateway, SelectedRadioModel);

                IsActiveSnackbar = false;

                ContentSnackbar = string.Format(Domain.Properties.Resources.Send_Sucess, Domain.Properties.Resources.Sent);

                IsActiveSnackbar = true;

                count.Reset();

                count.SetTime(1000);

                count.Start();

                count.CountDownFinished = () =>
                {
                    IsActiveSnackbar = false;
                };

                await CloseDialog();
            }
            catch (Exception ex)
            {
                IsActiveSnackbar = false;
                ShowError(ex);
            }
            finally
            {
                SelectedRadioModel.IsSending = false;
            }
        }

        private async Task Sensor()
        {
            try
            {
                SelectedRadioModel.ActionRTSSomfy = SelectedRadioModel.IsSensorRTS ? ActionsRTSSomfy.ENABLESUNWINDSENSOR : ActionsRTSSomfy.DISABLESUNWINDSENSOR;

                SelectedRadioModel.IsSending = true;

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.Sending;

                await _rfRepository.PlayRts(SelectedProjectModel.SelectedGateway, SelectedRadioModel);

                _localDBRepository.Update(SelectedProjectModel.SelectedGateway, SelectedRadioModel);

                IsActiveSnackbar = false;

                ContentSnackbar = string.Format(Domain.Properties.Resources.Send_Sucess, Domain.Properties.Resources.Sent);

                IsActiveSnackbar = true;

                count.Reset();

                count.SetTime(1000);

                count.Start();

                count.CountDownFinished = () =>
                {
                    IsActiveSnackbar = false;
                };

                await CloseDialog();
            }
            catch (Exception ex)
            {
                IsActiveSnackbar = false;
                ShowError(ex);
            }
            finally
            {
                SelectedRadioModel.IsSending = false;
            }
        }

        private void SettingsServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsTerminalEnable))
            {
                IsTerminalEnable = ((SettingsService)sender).IsTerminalEnable;
            }
        }

        private async void UpdatePIC(object obj)
        {
            try
            {
                await _rfRepository.UpdatePICAsync(SelectedProjectModel.SelectedGateway);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        #endregion Methods
    }
}