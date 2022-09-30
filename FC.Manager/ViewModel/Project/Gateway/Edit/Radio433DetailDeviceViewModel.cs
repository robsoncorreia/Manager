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
    public class Radio433DetailDeviceViewModel : ProjectViewModelBase
    {
        private int _frequencySelectedIndex;
        private bool _isConnection;
        private int _radioCount;
        private RadioModel _selectedRadioModel;
        private bool isTerminalEnable;

        private void SettingsServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsTerminalEnable))
            {
                IsTerminalEnable = ((SettingsService)sender).IsTerminalEnable;
            }
        }

        public Radio433DetailDeviceViewModel(IFrameNavigationService navigationService,
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
                                 INetworkService internetService,
                                 ISettingsService settingsService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _internetService = internetService;

            _settingsService = settingsService;

            _settingsService.PropertyChanged += SettingsServicePropertyChanged;

            IsTerminalEnable = Domain.Properties.Settings.Default.enableTerminal;

            SelectedRadioModel = new RadioModel();

            DeleteAllFromGatewayAwaitCommand = new RelayCommand(() => DeleteAllFromGatewayAwait());

            SelectionChangedCommand = new RelayCommand<object>(SelectionChanged);

            LearnCommand = new RelayCommand<object>(Learn);

            DeleteRFFromGatewayAwaitCommand = new RelayCommand<object>(DeleteRFFromGatewayAwait);

            GetAllRFGatewayCommand = new RelayCommand<object>(GetAllRFGateway);

            NewCloneCommand = new RelayCommand<object>(NewClone);

            UpdatePICCommad = new RelayCommand<object>(UpdatePIC);

            ResetPICCommad = new RelayCommand<object>(ResetPIC);

            GetFirmwareInfoPICCommand = new RelayCommand<object>(GetFirmwareInfoPIC);

            LoadedCommand = new RelayCommand<object>(Loaded);

            PlayMemoryCommad = new RelayCommand<object>(PlayMemory);

            DeleteAllCloudAwaitCommand = new RelayCommand<object>(DeleteAllCloudAwait);

            DeleteFromCloudCommand = new RelayCommand<object>(async (obj) => await DeleteFromCloud(obj));

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });

            _gatewayService.PropertyChanged += GatewayServicePropertyChanged;

            _parseService.PropertyChanged += ParseServicePropertyChanged;
        }

        public bool IsTerminalEnable
        {
            get => isTerminalEnable;
            set => SetProperty(ref isTerminalEnable, value);
        }

        public ICommand CloseExpanderCommand { get; set; }

        public ICommand DeleteAllCloudAwaitCommand { get; set; }
        public ICommand DeleteAllFromGatewayAwaitCommand { get; set; }
        public ICommand DeleteFromCloudCommand { get; set; }
        public ICommand DeleteRFFromGatewayAwaitCommand { get; set; }

        public int FrequencySelectedIndex
        {
            get => _frequencySelectedIndex;
            set
            => SetProperty(ref _frequencySelectedIndex, value);
        }

        public ICommand GetAllRFGatewayCommand { get; set; }

        public ICommand GetFirmwareInfoPICCommand { get; set; }

        public bool IsConnection
        {
            get => _isConnection;
            set
            => SetProperty(ref _isConnection, value);
        }

        public ICommand LearnCommand { get; set; }

        public ICommand NewCloneCommand { get; set; }

        public ICommand PlayMemoryCommad { get; set; }

        public int RadioCount
        {
            get => _radioCount;
            set => SetProperty(ref _radioCount, value);
        }

        public RadioFrequency[] RadioFrequencys { get; set; } = new RadioFrequency[]
                        {
            new RadioFrequency
            {
                Name = "433.92MHz",
                Frequency = Frequency.F_433_92
},
            new RadioFrequency
            {
                Name = "433.42MHz",
                Frequency = Frequency.F_433_42
            },
            new RadioFrequency
            {
                Name = "433.00MHz",
                Frequency = Frequency.F_433
            },
            new RadioFrequency
            {
                Name = "433.96MHz",
                Frequency = Frequency.F_433_96
            }
        };

        public ICommand ResetPICCommad { get; set; }

        private readonly INetworkService _internetService;
        private readonly ISettingsService _settingsService;

        public RadioModel SelectedRadioModel
        {
            get => _selectedRadioModel;
            set => SetProperty(ref _selectedRadioModel, value);
        }

        public ICommand SelectionChangedCommand { get; set; }

        public ICommand UpdatePICCommad { get; set; }

        private async Task DeleteAllCloud()
        {
            try
            {
                await CloseDialog();

                await _rfRepository.DeleteAllCloud(SelectedProjectModel.SelectedGateway, TypeRF.R433);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAllCloudAwait(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                     message: Domain.Properties.Resources.Delete_All_Radio,
                     custom: async () => await DeleteAllCloud(),
                     textButtonCustom: Domain.Properties.Resources.Delete,
                     cancel: async () => await CloseDialog());
        }

        private async Task DeleteAllFromGateway()
        {
            try
            {
                await CloseDialog();

                RadioModel[] temp = new RadioModel[SelectedProjectModel.SelectedGateway.Radios433Gateway.Count];

                SelectedProjectModel.SelectedGateway.Radios433Gateway.CopyTo(temp, 0);

                foreach (RadioModel radio in temp)
                {
                    if (!await _rfRepository.DeleteAsync(SelectedProjectModel.SelectedGateway, radio))
                    {
                        continue;
                    }

                    _ = SelectedProjectModel.SelectedGateway.Radios433Gateway.Remove(radio);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAllFromGatewayAwait()
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Delete_All_Radio,
                                 custom: async () => await DeleteAllFromGateway(),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 cancel: async () => await CloseDialog());
        }

        private async Task DeleteFromCloud(object obj)
        {
            try
            {
                if (obj is not RadioModel radioModel)
                {
                    return;
                }

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());

                    return;
                }

                await _rfRepository.DeleteFromCloud(SelectedProjectModel.SelectedGateway, radioModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task DeleteRFFromGateway(object obj)
        {
            try
            {
                if (obj is not RadioModel radio)
                {
                    return;
                }

                SelectedProjectModel.SelectedGateway.SelectedIndexRadio433Gateway = SelectedProjectModel.SelectedGateway.Radios433Gateway.IndexOf(radio);

                _ = await _rfRepository.DeleteAsync(SelectedProjectModel.SelectedGateway, radio);

                radio.CopyPropertiesTo(SelectedRadioModel);

                if (SelectedProjectModel.SelectedGateway.Radios433Cloud.FirstOrDefault(x => x.MemoryId == SelectedRadioModel.MemoryId) is RadioModel temp)
                {
                    await DeleteFromCloud(temp);
                }

                SelectedRadioModel = new RadioModel();

                await CloseDialog(100);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteRFFromGatewayAwait(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Delete_Radio_ID_From_Memory, ((RadioModel)obj).Description, ((RadioModel)obj).MemoryId),
                                 custom: async () => await DeleteRFFromGateway(obj),
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
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                SelectedRadioModel = new RadioModel();

                await _rfRepository.GetAll(SelectedProjectModel.SelectedGateway, TypeRF.R433);

                count.Reset();

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign) {; } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetFirmwareInfoPIC(object obj)
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

        private async void Learn(object obj)
        {
            using ReplaySubject<string> rx = new();

            using CountDownTimer count = new();

            try
            {
                count.Reset();
                count.SetTime(15000);
                count.Start();
                count.TimeChanged += () => rx.OnNext($"{Domain.Properties.Resources.Time_out}: {count.TimeLeftMsStr}");
                count.CountDownFinished += () => rx.OnNext(Domain.Properties.Resources.Time_out);
                count.StepMs = 77;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Learning,
                                     rx: rx,
                                     message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Learning_mode_device, SelectedProjectModel.SelectedGateway.Name));

                await _rfRepository.Learn(SelectedProjectModel.SelectedGateway, SelectedRadioModel);

                if (SelectedProjectModel.SelectedGateway.Radios433Cloud.FirstOrDefault(x => x.MemoryId == SelectedRadioModel.MemoryId) is RadioModel temp)
                {
                    SelectedRadioModel.CopyPropertiesTo(temp);
                }

                SelectedRadioModel = new RadioModel();

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                rx.Dispose();

                count.Dispose();
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

            if (!SelectedProjectModel.SelectedGateway.Radios433Gateway.Any())
            {
                GetAllRFGateway();
            }
        }

        private void NewClone(object obj)
        {
            SelectedRadioModel = new RadioModel();

            SelectedProjectModel.SelectedGateway.SelectedIndexRadio433Gateway = -1;
        }

        private void ParseServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSendingToCloud))
            {
                IsSendingToCloud = ((ParseService)sender).IsSendingToCloud;
            }
        }

        private readonly CountDownTimer count = new();

        private async void PlayMemory(object obj)
        {
            if (obj is not RadioModel radio)
            {
                return;
            }
            radio.IsSending = true;
            try
            {
                SelectedProjectModel.SelectedGateway.SelectedIndexRadio433Gateway = SelectedProjectModel.SelectedGateway.Radios433Gateway.IndexOf(radio);

                radio.CopyPropertiesTo(SelectedRadioModel);

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.Sending;

                await _rfRepository.PlayRadioMemory(SelectedProjectModel.SelectedGateway, SelectedRadioModel);

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
                ShowError(ex);

                IsActiveSnackbar = false;
            }
            finally
            {
                radio.IsSending = false;
            }
        }

        private async void ResetPIC(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Resetting_pic,
                                     cancel: async () => await CloseDialog());

                await _rfRepository.ResetPICAsync(SelectedProjectModel.SelectedGateway);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SelectionChanged(object obj)
        {
            if (obj is null)
            {
                return;
            }

            if (obj is not RadioModel radio)
            {
                return;
            }

            radio.CopyPropertiesTo(SelectedRadioModel);
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
    }
}