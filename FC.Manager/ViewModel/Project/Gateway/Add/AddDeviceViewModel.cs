using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain.Model;
using FC.Domain.Model.Device;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Gateway.Add
{
    public class AddDeviceViewModel : ProjectViewModelBase
    {
        private bool _isValid;
        private int _selectedIndexBrand;
        private int _selectedIndexGateway = 0;
        private readonly INetworkService _internetService;

        public AddDeviceViewModel(IFrameNavigationService navigationService,
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

            NavigateBeforeCommand = new RelayCommand<object>(NavigateBefore);

            LoadedCommand = new RelayCommand<object>(Loaded);

            SearchCommand = new RelayCommand<object>(Search);

            GetGatewayInfoCommand = new RelayCommand<object>(GetGatewayInfo);

            PingGatewayCommand = new RelayCommand<object>(PingGateway);

            Brands = new ObservableCollection<BrandModel>();

            Gateways = _gatewayService?.Gateways;

            BrandModel FlexAutomation = new()
            {
                Name = Domain.Properties.Resources.Flex_Automation,
                Image = new Uri("/assets/brand/flex-automation-logo.png", UriKind.Relative),
                Manufacturer = ManufacturerEnum.FlexAutomation
            };

            Brands.Add(FlexAutomation);

            Gateways.CollectionChanged += GatewaysCollectionChanged;

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        private void GatewaysCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems[0] is GatewayModel gateway)
            {
                if (SelectedProjectModel.Devices.FirstOrDefault(x => x.MacAddressEthernet == gateway.MacAddressEthernet) is GatewayModel temp)
                {
                    gateway.IsEnabled = false;
                }
            }
        }

        private async void PingGateway(object obj)
        {
            try
            {
                if (obj is not GatewayModel selectedGateway)
                {
                    return;
                }

                await _gatewayRepository.Reboot(selectedGateway);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public ObservableCollection<BrandModel> Brands { get; set; }

        public ObservableCollection<GatewayModel> Gateways { get; set; }

        public ICommand GetGatewayInfoCommand { get; set; }

        public ICommand PingGatewayCommand { get; set; }

        public bool IsValid
        {
            get => _isValid;
            set => SetProperty(ref _isValid, value);
        }

        public char ShortcutToDoNewSearch { get; } = Domain.Properties.Resources._Refresh[1];
        public char ShortcutToStopSearch { get; } = Domain.Properties.Resources._Stop[1];

        public ICommand SearchCommand { get; set; }

        public int SelectedIndexBrand
        {
            get => _selectedIndexBrand;
            set => SetProperty(ref _selectedIndexBrand, value);
        }

        public int SelectedIndexGateway
        {
            get => _selectedIndexGateway;
            set
            {
                _ = SetProperty(ref _selectedIndexGateway, value);

                if (value < 0)
                {
                    return;
                }

                GetGatewayInfo();
            }
        }

        public string this[string name]
        {
            get
            {
                string result = string.Empty;

                switch (name)
                {
                    case nameof(SelectedIndexGateway):

                        if (SelectedIndexGateway < 0 && SelectedIndexBrand == 0)
                        {
                            result = Domain.Properties.Resources.You_must_select_a_gateway;
                        }
                        break;
                }

                return result;
            }
        }

        private async void GetGatewayInfo(object obj = null)
        {
            try
            {
                if (IsSendingToGateway)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Getting_information_from_the_gateway);

                await _gatewayRepository.GetGatewayInfo(Gateways[SelectedIndexGateway]);

                if (string.IsNullOrEmpty(Gateways[SelectedIndexGateway].UID))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Try_again,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await Insert();
            }
            catch (OperationCanceledException)
            {
                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetGateways()
        {
            ReplaySubject<string> replay = new();

            ReplaySubject<int> replayProgressBarValue = new();

            try
            {
                SelectedIndexGateway = -1;

                bool isBroadcast = Domain.Properties.Settings.Default.isFindGatewayBroadcast;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Finding_Gateways_Network,
                                     rx: replay,
                                     textButtonCancel: Domain.Properties.Resources.Stop_search,
                                     cancel: async () => await CloseDialog(),
                                     isRXProgressBarVisibible: !isBroadcast,
                                     isProgressBar: isBroadcast,
                                     rxProgressBarValue: !isBroadcast ? replayProgressBarValue : null);

                await _gatewayRepository.GetGateways(GatewayModelEnum.ANY,
                                                     address: IPAddress.Broadcast,
                                                     replay: replay,
                                                     rxProgressBarValue: replayProgressBarValue);

                if (!Gateways.Any())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Finding_Gateways_Network,
                                         message: Domain.Properties.Resources.Could_not_find_any_gateway_on_the_network__Search_again_,
                                         textButtonCustom: Domain.Properties.Resources._Try_again,
                                         custom: () => GetGateways(),
                                         textButtonCancel: Domain.Properties.Resources._Close,
                                         cancel: async () => await CloseDialog());

                    return;
                }
                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                replay.Dispose();

                replayProgressBarValue.Dispose();
            }
        }

        private async Task Insert()
        {
            try
            {
                switch (Brands[SelectedIndexBrand].Manufacturer)
                {
                    case ManufacturerEnum.FlexAutomation:

                        if (SelectedIndexGateway < 0)
                        {
                            return;
                        }

                        if (Gateways[SelectedIndexGateway].IsEnabled == false)
                        {
                            return;
                        }

                        if (!await _internetService.IsInternet())
                        {
                            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                                 message: Domain.Properties.Resources.Without_Internet_Connection,
                                                 textButtomOk: Domain.Properties.Resources._Close,
                                                 ok: async () => await CloseDialog());

                            return;
                        }

                        OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                             message: Domain.Properties.Resources.Adding_device_to_the_project);

                        await _gatewayRepository.Insert(SelectedProjectModel, Gateways[SelectedIndexGateway]);

                        OpenCustomMessageBox(header: Domain.Properties.Resources.Well_Done,
                                             message: Domain.Properties.Resources.Device_added_to_the_project);

                        SelectedProjectModel.SelectedGateway = Gateways[SelectedIndexGateway];

                        _navigationService.NavigateTo(Domain.Properties.Resources.Detail_Device);

                        await CloseDialog();

                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            if (obj is not Page page)
            {
                return;
            }
            if (!page.IsVisible)
            {
                return;
            }

            SelectedProjectModel = _projectService.SelectedProject;

            if (Gateways.Any())
            {
                Gateways.Clear();
            }

            Search();
        }

        private void NavigateBefore(object obj)
        {
            _navigationService.NavigateTo(Domain.Properties.Resources.Detail_Project);
        }

        private void Search(object obj = null)
        {
            try
            {
                if (SelectedIndexBrand < 0)
                {
                    return;
                }
                switch (Brands[SelectedIndexBrand].Manufacturer)
                {
                    case ManufacturerEnum.FlexAutomation:
                        if (_gatewayService.IsSendingToGateway)
                        {
                            return;
                        }
                        GetGateways();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}