using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class ConfigurationDatailDeviceViewModel : ProjectViewModelBase
    {
        private readonly INetworkService _internetService;
        private bool isUpdated = false;
        private GatewayModel lastSelectedGateway;

        public ConfigurationDatailDeviceViewModel(IFrameNavigationService navigationService,
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
            UpdateNameCommand = new RelayCommand<object>(Rename);

            LoadedCommand = new RelayCommand<object>(Loaded);

            DeleteDeviceAsyncCommand = new RelayCommand<object>(DeleteDeviceAsync);

            UpdateFirmaweCommand = new RelayCommand<object>(UpdateFirmawe);

            SyncGatewayCommand = new RelayCommand(async () => await SyncGateway());

            CheckUpdatesCommand = new RelayCommand(CheckUpdates);

            _internetService = internetService;

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        private async void CheckUpdates()
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                 message: Domain.Properties.Resources.Checking_for_updates,
                                 textButtonCancel: Domain.Properties.Resources.Cancel,
                                 isProgressBar: true,
                                 cancel: async () => await CloseDialog());
            if (!await SyncGateway())
            {
                return;
            }

            await GetFirmwares();
        }

        private async Task<bool> SyncGateway()
        {
            try
            {
                if (!await _gatewayRepository.FindGateway(SelectedProjectModel.SelectedGateway, isSendingToGateway: true))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Gateway_not_found,
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         custom: () => CheckUpdates(),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         cancel: async () => await CloseDialog());
                    return false;
                }

                await _gatewayRepository.GetGatewayInfo(SelectedProjectModel.SelectedGateway);

                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                await _gatewayRepository.Update(SelectedProjectModel);

                await CloseDialog(0);

                return true;
            }
            catch (Exception ex)
            {
                ShowError(ex);

                return false;
            }
        }

        public ICommand DeleteDeviceAsyncCommand { get; set; }

        public ICommand UpdateFirmaweCommand { get; set; }

        public ICommand UpdateNameCommand { get; set; }

        public ICommand SyncGatewayCommand { get; set; }

        public ICommand CheckUpdatesCommand { get; set; }

        private async void DeleteDevice()
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Deleting,
                                     message: Domain.Properties.Resources.Deleting_device,
                                     cancel: async () => await CloseDialog());

                await _gatewayRepository.Delete(SelectedProjectModel.SelectedGateway);

                _navigationService.NavigateTo(Domain.Properties.Resources.Detail_Project);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteDeviceAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Are_you_sure_you_want_to_delete_device_,
                                 custom: () => DeleteDevice(),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 cancel: async () => await CloseDialog());
        }

        private async Task GetFirmwares()
        {
            try
            {
                await _gatewayRepository.GetFirmawes(SelectedProjectModel.SelectedGateway);

                if (SelectedProjectModel.SelectedGateway.Firmwares.FirstOrDefault().Name.Substring(0, 6) == SelectedProjectModel.SelectedGateway.Build.ToString().Substring(0, 6))
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Update,
                                     message: Domain.Properties.Resources.A_new_version_is_available__Do_you_want_to_update_the_gateway_to_the_latest_version_,
                                     custom: () => UpdateFirmawe(),
                                     textButtonCustom: Domain.Properties.Resources.Yes,
                                     textButtonCancel: Domain.Properties.Resources.No,
                                     cancel: async () => await CloseDialog());
            }
            catch (Exception)
            {
            }
        }

        private bool IsFist()
        {
            if (isUpdated)
            {
                return true;
            }

            if (SelectedProjectModel.SelectedGateway.Equals(lastSelectedGateway))
            {
                return false;
            }

            lastSelectedGateway = SelectedProjectModel.SelectedGateway;

            return true;
        }

        private void Loaded(object obj)
        {
            if (obj is not UserControl view)
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            if (!IsFist())
            {
                return;
            }

            CheckUpdates();
        }

        private readonly CountDownTimer count = new();

        private async void Rename(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Update,
                                     message: Domain.Properties.Resources.Updating_device_name);

                count.SetTime(2000);

                count.Start();

                await _gatewayRepository.Rename(SelectedProjectModel.SelectedGateway);

                await CloseDialog(3000);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void UpdateFirmawe(object obj = null)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Update,
                                     message: Domain.Properties.Resources.The_gateway_firmware_will_be_updated_to_the_latest_version__Observe_the_gateway_leds_and_wait_for_the_update_to_finish_,
                                     ok: async () => await CloseDialog());

                await _gatewayRepository.UpdateFirmawe(SelectedProjectModel.SelectedGateway);

                isUpdated = true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}