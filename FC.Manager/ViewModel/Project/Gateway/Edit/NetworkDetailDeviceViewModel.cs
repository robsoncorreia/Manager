using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.Project;
using FC.Domain.Model.Rede;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using NativeWifi;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class NetworkDetailDeviceViewModel : ProjectViewModelBase
    {
        private bool _isTextChangedEthernetIp;
        private bool _isTextChangedEthernetPort;
        private bool _isTextChangedWifi;
        private bool _isTextChangedWifiIP;
        private bool _isTextChangedWifiPort;
        private int _selectedWifiNetworksIndex;

        public NetworkDetailDeviceViewModel(IFrameNavigationService navigationService,
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
                                 INetworkService internetService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _internetService = internetService;

            WifiNetworks = new ObservableCollection<WifiNetwork>();

            ApplyEthernetCommand = new RelayCommand<object>(ApplyEthernet);

            ApplyWifiCommand = new RelayCommand<object>(ApplyWifi);

            ApplyEthernetPortCommand = new RelayCommand<object>(ApplyEthernetPort);

            ApplyWifiPortCommand = new RelayCommand<object>(ApplyWifiPort);

            GetWifisCommand = new RelayCommand<object>(GetWifis);

            SaveConfigurationCommand = new RelayCommand<object>(SaveConfiguration);

            SetWifiStationCommand = new RelayCommand<object>(SetWifiStation);

            LoadedCommand = new RelayCommand<object>(Loaded);

            ReloadGatewayCommand = new RelayCommand<object>(ReloadGateway);

            ClearGatewayCommand = new RelayCommand<object>(ClearGateway);

            SelectedNetworkCommand = new RelayCommand<object>(SelectedNetwork);

            SetWIFIAPCommand = new RelayCommand<object>(SetWIFIAP);

            SetSSIDPasswordAPCommand = new RelayCommand<object>(SetSSIDPasswordAP);

            SetAPDHCPCommand = new RelayCommand<object>(SetAPDHCP);

            RebootCommand = new RelayCommand<object>(Reboot);

            SyncGatewayCommand = new RelayCommand<object>(SyncGateway);

            ActionSnackbarCommand = new RelayCommand<object>(Reboot);

            TextChangedCommand = new RelayCommand<object>(TextChanged);

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        public ICommand ApplyEthernetCommand { get; set; }
        public ICommand ApplyEthernetPortCommand { get; set; }
        public ICommand ApplyWifiCommand { get; set; }
        public ICommand ApplyWifiPortCommand { get; set; }
        public ICommand ClearGatewayCommand { get; set; }
        public ICommand GetWifisCommand { get; set; }

        public bool IsTextChangedEthernetIp
        {
            get => _isTextChangedEthernetIp;
            set => SetProperty(ref _isTextChangedEthernetIp, value);
        }

        public bool IsTextChangedEthernetPort
        {
            get => _isTextChangedEthernetPort;
            set => SetProperty(ref _isTextChangedEthernetPort, value);
        }

        public bool IsTextChangedWifi
        {
            get => _isTextChangedWifi;
            set => SetProperty(ref _isTextChangedWifi, value);
        }

        public bool IsTextChangedWifiIP
        {
            get => _isTextChangedWifiIP;
            set => SetProperty(ref _isTextChangedWifiIP, value);
        }

        public bool IsTextChangedWifiPort
        {
            get => _isTextChangedWifiPort;
            set => SetProperty(ref _isTextChangedWifiPort, value);
        }

        public ICommand RebootCommand { get; set; }
        public ICommand ReloadGatewayCommand { get; set; }
        public ICommand SaveConfigurationCommand { get; set; }
        public ICommand SelectedNetworkCommand { get; set; }

        public int SelectedWifiNetworksIndex
        {
            get => _selectedWifiNetworksIndex;
            set
            {
                if (Equals(_selectedWifiNetworksIndex, value))
                {
                    return;
                }

                _ = SetProperty(ref _selectedWifiNetworksIndex, value);
            }
        }

        public ICommand SetAPDHCPCommand { get; set; }
        public ICommand SetSSIDPasswordAPCommand { get; set; }
        public ICommand SetWIFIAPCommand { get; set; }
        public ICommand SetWifiStationCommand { get; set; }
        public ICommand SyncGatewayCommand { get; set; }
        public ICommand TextChangedCommand { get; set; }

        private readonly INetworkService _internetService;

        public ObservableCollection<WifiNetwork> WifiNetworks { get; set; }

        private static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private static bool SSIDIsNullOrEmpty(Wlan.Dot11Ssid ssid)
        {
            return string.IsNullOrEmpty(Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength));
        }

        private async void ApplyEthernet(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _gatewayRepository.SetEthernetNetworkConfiguration(SelectedProjectModel.SelectedGateway);

                IsTextChangedEthernetIp = false;

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void ApplyEthernetPort(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _gatewayRepository.SetEthernetPort(SelectedProjectModel.SelectedGateway);

                IsTextChangedEthernetPort = false;

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());

                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void ApplyWifi(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _gatewayRepository.SetWifiNetworkConfiguration(SelectedProjectModel.SelectedGateway);

                IsTextChangedWifiIP = false;

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());

                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void ApplyWifiPort(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _gatewayRepository.SetWifiPort(SelectedProjectModel.SelectedGateway);

                IsTextChangedWifiPort = false;

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ClearGateway(object obj)
        {
            //_gatewayService.SelectedProjectModel.SelectedGateway = new GatewayModel();
        }

        private async void GetGatewayInfo()
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }
                await _gatewayRepository.GetGatewayInfo(SelectedProjectModel.SelectedGateway);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void GetWifis(object obj = null)
        {
            try
            {
                WlanClient client = new();

                WifiNetworks.Clear();

                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);

                    foreach (Wlan.WlanAvailableNetwork network in networks)
                    {
                        if (SSIDIsNullOrEmpty(network.dot11Ssid))
                        {
                            continue;
                        }

                        if (WifiNetworks.FirstOrDefault(x => x.Ssid == GetStringForSSID(network.dot11Ssid)) != null)
                        {
                            continue;
                        }

                        WifiNetworks.Add(new WifiNetwork
                        {
                            Ssid = GetStringForSSID(network.dot11Ssid),
                            WlanSignalQuality = (int)network.wlanSignalQuality
                        });
                    }

                    IOrderedEnumerable<WifiNetwork> list = WifiNetworks.OrderBy(x => x.Ssid);

                    foreach (WifiNetwork item in list.GroupBy(x => x.Ssid))
                    {
                        WifiNetworks.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
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

            IsTextChangedEthernetIp = false;
            IsTextChangedEthernetPort = false;
            IsTextChangedWifiIP = false;
            IsTextChangedWifiPort = false;
            IsTextChangedWifi = false;

            GetWifis();
        }

        private async void Reboot(object obj)
        {
            using ReplaySubject<string> rx = new();
            using CountDownTimer timer = new();

            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                timer.SetTime(15000);
                timer.Start();
                timer.TimeChanged += () => rx.OnNext($"{timer.TimeLeftMsStr}");
                timer.CountDownFinished += () => IsOpenDialogHost = false;
                timer.StepMs = 77;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Reboot_,
                                     rx: rx);

                await _gatewayRepository.Reboot(SelectedProjectModel.SelectedGateway);

                rx.Dispose();

                timer.Dispose();

                IsActiveSnackbar = false;

                SyncGateway();
            }
            catch (Exception ex)
            {
                ShowError(ex);

                rx.Dispose();

                timer.Dispose();
            }
            finally
            {
                rx.Dispose();

                timer.Dispose();
            }
        }

        private void ReloadGateway(object obj)
        {
            if (string.IsNullOrEmpty(SelectedProjectModel.SelectedGateway.LocalIP))
            {
                return;
            }
            GetGatewayInfo();
        }

        private async void SaveConfiguration(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _gatewayRepository.SetWIFISSID(SelectedProjectModel.SelectedGateway);

                await _gatewayRepository.SetWIFIPass(SelectedProjectModel.SelectedGateway);

                IsTextChangedWifi = false;

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SelectedNetwork(object obj)
        {
            if (obj is not WifiNetwork)
            {
                return;
            }

            SelectedProjectModel.SelectedGateway.SSID = ((WifiNetwork)obj).Ssid;
        }

        private async void SetAPDHCP(object obj)
        {
            try
            {
                if (obj is not ComboBox comboBox)
                {
                    return;
                }

                if (!comboBox.IsMouseOver)
                {
                    return;
                }

                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _gatewayRepository.SetAPDHCP(SelectedProjectModel.SelectedGateway);

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void SetSSIDPasswordAP(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _gatewayRepository.SetSSIDPasswordAP(SelectedProjectModel.SelectedGateway);

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void SetWIFIAP(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }
                if (obj is not ComboBox comboBox)
                {
                    return;
                }

                if (!comboBox.IsMouseOver)
                {
                    return;
                }

                if (!await _gatewayRepository.SetWIFIAP(SelectedProjectModel.SelectedGateway))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Did_not_response,
                                         ok: async () => await CloseDialog());
                    return;
                }

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void SetWifiStation(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }
                if (obj is not ComboBox comboBox)
                {
                    return;
                }

                if (!comboBox.IsMouseOver)
                {
                    return;
                }

                await _gatewayRepository.SetWifiStation(SelectedProjectModel.SelectedGateway);

                ShowSnackbar();

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.Update(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ShowSnackbar()
        {
            IsActiveSnackbar = true;

            ActionContentSnackbar = Domain.Properties.Resources.Reboot;

            ContentSnackbar = Domain.Properties.Resources.Reboot_is_required;
        }

        private async void SyncGateway(object obj = null)
        {
            ReplaySubject<string> replay = new();

            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Finding,
                                     isProgressBar: true,
                                     rx: replay,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                if (!await _gatewayRepository.FindGateway(gateway: SelectedProjectModel.SelectedGateway,
                                                          replay: replay,
                                                          isSendingToGateway: true))
                {
                    if (IsCanceled)
                    {
                        OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                        message: $"{Domain.Properties.Resources.Task_canceled}",
                        textButtonCancel: Domain.Properties.Resources._Close,
                        cancel: async () => await CloseDialog());
                        return;
                    }

                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Gateway_not_found,
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SyncGateway(obj),
                                         textButtonCancel: Domain.Properties.Resources._Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.GetGatewayInfo(selectedGateway: SelectedProjectModel.SelectedGateway,
                                                        isSendingToGateway: true);

                _gatewayService.IsPrimary = SelectedProjectModel.SelectedGateway.IsPrimary;

                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                await _gatewayRepository.Update(SelectedProjectModel);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                replay.Dispose();
            }
        }

        private void TextChanged(object obj)
        {
            if (obj is not TextBox box)
            {
                return;
            }

            if (!box.IsFocused)
            {
                return;
            }

            if (box.Tag + "" == "EthernetIP")
            {
                IsTextChangedEthernetIp = true;
                return;
            }

            if (box.Tag + "" == "EthernetPort")
            {
                IsTextChangedEthernetPort = true;
                return;
            }

            if (box.Tag + "" == "WifiIP")
            {
                IsTextChangedWifiIP = true;

                return;
            }

            if (box.Tag + "" == "WifiPort")
            {
                IsTextChangedWifiPort = true;
                return;
            }

            if (box.Tag + "" == "Wifi")
            {
                IsTextChangedWifi = true;
                return;
            }
        }
    }
}