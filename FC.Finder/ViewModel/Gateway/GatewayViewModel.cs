using CommunityToolkit.Mvvm.Input;
using FC.Domain.Model;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Finder.ViewModel.Project;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using System.Windows.Data;
using System.Windows.Input;

namespace FC.Finder.ViewModel.Gateway
{
    public class GatewayViewModel : FlexViewModelBase
    {
        private bool _isGroupByName = true;

        public bool IsGroupByName
        {
            get => _isGroupByName;
            set
            {
                if (Equals(_isGroupByName, value))
                {
                    return;
                }

                _isGroupByName = value;

                _ = SetProperty(ref _isGroupByName, value);

                GroupByName(value);
            }
        }

        private void GroupByName(bool value)
        {
            GatewaysCollectionView.GroupDescriptions.Clear();

            if (value)
            {
                GatewaysCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Name"));
            }
        }

        private int _selectedIndexTransitioner;

        public int SelectedIndexTransitioner
        {
            get => _selectedIndexTransitioner;
            set => SetProperty(ref _selectedIndexTransitioner, value);
        }

        public ICollectionView GatewaysCollectionView => CollectionViewSource.GetDefaultView(Gateways);

        public string FilterGateways
        {
            get => _filterGateways;
            set
            {
                if (Equals(_filterGateways, value))
                {
                    return;
                }

                _ = SetProperty(ref _filterGateways, value);

                GatewaysCollectionView.Filter = w =>
                {
                    using GatewayModel model = w as GatewayModel;

                    return model.Name.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
                };
            }
        }

        public ObservableCollection<GatewayModel> Gateways { get; set; }

        public ICommand GetGatewaysCommand { get; set; }

        public ICommand SelectedChangedGatewayCommand { get; set; }

        private async void Reload(object obj = null)
        {
            ReplaySubject<string> replay = new ReplaySubject<string>();

            ReplaySubject<int> replayProgressBarValue = new ReplaySubject<int>();

            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Finding_Gateways_Network,
                                     rx: replay,
                                     textButtonCancel: Domain.Properties.Resources.Stop_search,
                                     cancel: async () => await CloseDialog(),
                                     isRXProgressBarVisibible: true,
                                     rxProgressBarValue: replayProgressBarValue);

                await _gatewayRepository.GetGateways(GatewayModelEnum.ANY,
                                                     address: IPAddress.Broadcast,
                                                     replay: replay,
                                                     rxProgressBarValue: replayProgressBarValue);

                if (!Gateways.Any())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Finding_Gateways_Network,
                                         message: Domain.Properties.Resources.Could_not_find_any_gateway_on_the_network__Search_again_,
                                         textButtonCustom: Domain.Properties.Resources._Try_again,
                                         custom: () => Reload(),
                                         textButtonCancel: Domain.Properties.Resources._Close,
                                         cancel: async () => await CloseDialog());

                    return;
                }
                await CloseDialog();
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
            finally
            {
                replay.Dispose();

                replayProgressBarValue.Dispose();
            }
        }

        private async void SelectedChangedGateway(object obj)
        {
            try
            {
                if (!(obj is GatewayModel temp))
                {
                    return;
                }

                _gatewayService.SelectedGateway = temp;

                _ = System.Diagnostics.Process.Start($"http:{temp.LocalIP}");
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private string _filterGateways;

        public GatewayViewModel(IFrameNavigationService navigationService,
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
                                IGatewayRepository gatewayRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository)
        {
            LoadedCommand = new RelayCommand<object>(Loaded);

            ReloadCommand = new RelayCommand<object>(Reload);

            SelectedChangedGatewayCommand = new RelayCommand<object>(SelectedChangedGateway);

            Gateways = _gatewayService.Gateways;
        }

        private void Loaded(object obj)
        {
            SelectedIndexTransitioner = 0;

            ChangeTransitioner();

            if (Gateways.Any())
            {
                Gateways.Clear();
            }

            GroupByName(IsGroupByName);
        }

        private async void ChangeTransitioner()
        {
            await System.Threading.Tasks.Task.Delay(1000);

            SelectedIndexTransitioner = 1;

            await System.Threading.Tasks.Task.Delay(1000);

            Reload();
        }
    }
}