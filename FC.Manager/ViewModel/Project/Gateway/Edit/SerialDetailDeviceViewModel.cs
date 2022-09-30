using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model._Serial;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class SerialDetailDeviceViewModel : ProjectViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private bool _isTerminalEnable;

        public SerialDetailDeviceViewModel(IFrameNavigationService navigationService,
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
            GetAllFromGatewayCommand = new RelayCommand(async () => await GetAll());

            SelectionChangedSerialCommand = new RelayCommand<object>(SelectionChangedSerial);

            NewCommand = new RelayCommand<object>(New);

            SaveCommand = new RelayCommand<object>(Save);

            SetBaudRateCommand = new RelayCommand<object>(SetBaudRate);

            GetBaudRateCommand = new RelayCommand<object>(GetBaudRate);

            PlayCommand = new RelayCommand<object>(Play);

            SetParityCommand = new RelayCommand<object>(async (obj) => await SetParity(obj));

            GetParityCommand = new RelayCommand<object>(GetParity);

            DeleteAllFromGatewayAwaitCommand = new RelayCommand<object>(DeleteAllFromGatewayAwait);

            DeleteFromGatewayAwaitCommand = new RelayCommand<object>(DeleteFromGatewayAwait);

            SaveProtocolCommand = new RelayCommand<object>(SaveProtocol);

            LoadedCommand = new RelayCommand<object>(Loaded);

            _settingsService = settingsService;

            _settingsService.PropertyChanged += SettingsServicePropertyChanged;

            IsTerminalEnable = Domain.Properties.Settings.Default.enableTerminal;

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        #region Properties

        public bool IsTerminalEnable
        {
            get => _isTerminalEnable;
            set => SetProperty(ref _isTerminalEnable, value);
        }

        #endregion Properties

        #region Collections

        public IList<SerialCombox> Protocols { get; private set; } = new List<SerialCombox> { new SerialCombox { Name = "485", Value = 2 }, new SerialCombox { Name = "232", Value = 1 } };

        #endregion Collections

        #region ICommand

        public ICommand SaveCommand { get; set; }

        public ICommand SaveProtocolCommand { get; set; }

        public ICommand SelectionChangedSerialCommand { get; set; }

        public ICommand SetBaudRateCommand { get; set; }

        public ICommand SetParityCommand { get; set; }

        public ICommand DeleteAllFromGatewayAwaitCommand { get; set; }

        public ICommand DeleteFromGatewayAwaitCommand { get; set; }

        public ICommand GetAllFromGatewayCommand { get; set; }

        public ICommand GetBaudRateCommand { get; set; }

        public ICommand GetParityCommand { get; set; }

        public ICommand NewCommand { get; set; }

        public ICommand PlayCommand { get; set; }

        #endregion ICommand

        #region Method

        private void SettingsServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsTerminalEnable))
            {
                IsTerminalEnable = ((SettingsService)sender).IsTerminalEnable;
            }
        }

        private async Task DeleteAllFromGateway()
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _serialRepository.DeleteAll(SelectedProjectModel.SelectedGateway);

                SelectedProjectModel.SelectedGateway.SelectedSerialModel = new SerialModel();

                await CloseDialog(1000);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAllFromGatewayAwait(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Delete_All_Serial_Commands, SelectedProjectModel.SelectedGateway.Name),
                                 custom: async () => await DeleteAllFromGateway(),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 cancel: async () => await CloseDialog());
        }

        private async Task DeleteFromGateway(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }
                if (obj is not SerialModel serial)
                {
                    return;
                }

                await _serialRepository.DeleteByMemoryId(SelectedProjectModel.SelectedGateway, serial);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteFromGatewayAwait(object obj)
        {
            if (obj is not SerialModel serial)
            {
                return;
            }

            SelectedProjectModel.SelectedGateway.SelectedSerialModel = serial;

            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Delete_Serial_Command, SelectedProjectModel.SelectedGateway.Name, serial.MemoryId),
                                 custom: async () => await DeleteFromGateway(obj),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 cancel: async () => await CloseDialog());
        }

        private async Task GetAll()
        {
            using CountDownTimer timer = new();

            SelectedProjectModel.SelectedGateway.SelectedSerialModel = new SerialModel();

            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                timer.SetTime(2000);
                timer.Start();

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Seeking_Out_All_Serial_Commands_Gateway);

                await _serialRepository.GetAll(SelectedProjectModel.SelectedGateway);

                await Task.Run(() => { while (timer.IsRunnign) { }; });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                timer.Dispose();
            }
        }

        private async void GetBaudRate(object obj)
        {
            using CountDownTimer timer = new();

            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                if (obj is not SerialProtocol temp)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: temp == SerialProtocol.T232 ? Domain.Properties.Resources.Obtaining_baud_rate_settings_232 : Domain.Properties.Resources.Obtaining_baud_rate_settings_485);

                timer.SetTime(2000);
                timer.Start();

                await _serialRepository.GetBaudRate(selectedProject: SelectedProjectModel, temp);

                await Task.Run(() => { while (timer.IsRunnign) { }; });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                timer.Dispose();
            }
        }

        private async void GetParity(object obj)
        {
            using CountDownTimer timer = new();

            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                if (obj is not SerialProtocol temp)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: temp == SerialProtocol.T232 ? Domain.Properties.Resources.Getting_parity_settings_232 : Domain.Properties.Resources.Getting_parity_settings_485);

                timer.SetTime(2000);
                timer.Start();

                await _serialRepository.GetParity(SelectedProjectModel.SelectedGateway, temp);

                await Task.Run(() => { while (timer.IsRunnign) { }; });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                timer.Dispose();
            }
        }

        private readonly CountDownTimer count = new();

        private async void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            if (obj is not bool isVisible)
            {
                return;
            }

            if (!isVisible)
            {
                return;
            }

            if (SelectedProjectModel.SelectedGateway.Serials.Any())
            {
                return;
            }

            SelectedProjectModel.SelectedGateway.SelectedSerialModel = new SerialModel();

            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }
                string message = Domain.Properties.Resources.Seeking_Out_All_Serial_Commands_Gateway;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: message);

                await _serialRepository.GetAll(selectedGateway: SelectedProjectModel.SelectedGateway, isSendingToGateway: true);

                message += '\n' + Domain.Properties.Resources.Obtaining_baud_rate_settings_485;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: message);

                await _serialRepository.GetBaudRate(selectedProject: SelectedProjectModel,
                                                    serialProtocol: SerialProtocol.T485,
                                                    isSendingToGateway: true);

                message += '\n' + Domain.Properties.Resources.Obtaining_baud_rate_settings_232;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: message);

                await _serialRepository.GetBaudRate(selectedProject: SelectedProjectModel,
                                                    serialProtocol: SerialProtocol.T232,
                                                    isSendingToGateway: true);

                message += '\n' + Domain.Properties.Resources.Getting_parity_settings_485;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                    message: message);

                await _serialRepository.GetParity(SelectedProjectModel.SelectedGateway, SerialProtocol.T485,
                                                  isSendingToGateway: true);

                message += '\n' + Domain.Properties.Resources.Getting_parity_settings_232;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: message);

                await _serialRepository.GetParity(SelectedProjectModel.SelectedGateway, SerialProtocol.T232,
                                                  isSendingToGateway: true);

                count.SetTime(2000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign) {; } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                count.Dispose();
            }
        }

        private void New(object obj)
        {
            SelectedProjectModel.SelectedGateway.SelectedSerialModel = new SerialModel();
            SelectedProjectModel.SelectedGateway.SelectedIndexSerial = -1;
        }

        private async void Play(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                if (obj is not SerialModel serial)
                {
                    return;
                }

                SelectedProjectModel.SelectedGateway.SelectedSerialModel = serial;

                await _serialRepository.PlayByMemoryId(SelectedProjectModel.SelectedGateway,
                                                       serial.MemoryId);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void Save(object obj)
        {
            using CountDownTimer timer = new();

            try
            {
                if (_gatewayService.IsSendingToGateway || string.IsNullOrEmpty(SelectedProjectModel.SelectedGateway.SelectedSerialModel.Data))
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Saving,
                                     message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Saving_Serial_Command, SelectedProjectModel.SelectedGateway.Name));

                timer.SetTime(2000);
                timer.Start();

                await _serialRepository.Save(SelectedProjectModel.SelectedGateway);

                await Task.Run(() => { while (timer.IsRunnign) { }; });

                SelectedProjectModel.SelectedGateway.SelectedSerialModel = new SerialModel();

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                timer.Dispose();
            }
        }

        private void SaveProtocol(object obj)
        {
            if (SelectedProjectModel.SelectedGateway.SelectedSerialModel.MemoryId < 0)
            {
                return;
            }

            Save(obj);
        }

        private void SelectionChangedSerial(object obj)
        {
            if (obj is SerialModel serial)
            {
                SelectedProjectModel.SelectedGateway.SelectedSerialModel = serial;
                SelectedProjectModel.SelectedGateway.SelectedSerialModel.SelectedProtocolIndex = serial.Protocol == 1 ? 1 : 0;
            }
        }

        private async void SetBaudRate(object obj)
        {
            using CountDownTimer timer = new();

            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                if (obj is not SerialProtocol)
                {
                    return;
                }

                SerialProtocol protocol = (SerialProtocol)obj;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Changing_Baud_Rate);

                timer.SetTime(2000);
                timer.Start();

                await _serialRepository.SetBaudRate(SelectedProjectModel.SelectedGateway, protocol);

                await Task.Run(() => { while (timer.IsRunnign) { }; });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                timer.Dispose();
            }
        }

        private async Task SetParity(object obj)
        {
            using CountDownTimer timer = new();

            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                if (obj is not SerialProtocol)
                {
                    return;
                }

                SerialProtocol protocol = (SerialProtocol)obj;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Changing_parity);

                timer.SetTime(2000);
                timer.Start();

                await _serialRepository.SetParity(SelectedProjectModel.SelectedGateway, protocol);

                await Task.Run(() => { while (timer.IsRunnign) { }; });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                timer?.Dispose();
            }
        }

        #endregion Method
    }
}