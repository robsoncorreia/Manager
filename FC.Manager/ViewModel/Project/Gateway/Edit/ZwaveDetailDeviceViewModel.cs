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
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class ZwaveDetailDeviceViewModel : ProjectViewModelBase
    {
        private readonly INetworkService _internetService;
        private string _filterZwaveDevices;
        private bool _isOpenFilter;
        private bool _IsTopDrawerOpen;

        public ZwaveDetailDeviceViewModel(IFrameNavigationService navigationService,
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
            LoadedCommand = new RelayCommand<object>(Loaded);
            ResetZwaveNetworkAsyncCommand = new RelayCommand<object>(ResetZwaveNetworkAsync);
            RestartZwaveChipAsyncCommand = new RelayCommand<object>(RestartZwaveChipAsync);
            IncludeAsyncCommand = new RelayCommand<object>(IncludeAsync);
            RemoveAsyncCommand = new RelayCommand<object>(RemoveAsync);
            OpenDetailCommand = new RelayCommand<object>(OpenDetail);
            RemoveDefectiveZwaveDeviceAsyncCommand = new RelayCommand<object>(RemoveDefectiveZwaveDeviceAsync);
            ReplaceZwaveDeviceAsyncCommand = new RelayCommand<object>(ReplaceZwaveDeviceAsync);
            ReloadCommand = new RelayCommand<object>(GetZwaveDevices);
            OpenDialogFilterCommand = new RelayCommand<object>(OpenDialogFilter);
            OrderByCommand = new RelayCommand<object>(OrderBy);
            FilterByCommand = new RelayCommand<object>(FilterBy);
            AllOffCommand = new RelayCommand<object>(AllOff);
            AllOnCommand = new RelayCommand<object>(AllOn);
            MouseOverCommand = new RelayCommand<object>(MouseOver);
            SizeChangedCommand = new RelayCommand<object>(SizeChanged);

            SelectedProjectModel = _projectService.SelectedProject;
            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        private void SizeChanged(object obj)
        {
        }

        private double _Height;

        public double Height
        {
            get => _Height;
            set => SetProperty(ref _Height, value);
        }

        public override void Unloaded(object obj)
        {
            base.Unloaded(obj);
            IsTopDrawerOpen = false;
        }

        public ICommand SizeChangedCommand { get; set; }
        public ICommand AllOffCommand { get; set; }

        public ICommand AllOnCommand { get; set; }

        public ICommand FilterByCommand { get; set; }

        public string FilterZwaveDevices
        {
            get => _filterZwaveDevices;
            set
            {
                if (Equals(_filterZwaveDevices, value))
                {
                    return;
                }

                _ = SetProperty(ref _filterZwaveDevices, value);

                FilterBy();
            }
        }

        public ICollectionView GatewaysCollectionView => CollectionViewSource.GetDefaultView(SelectedProjectModel.SelectedGateway.ZwaveDevices);

        public ICommand IncludeAsyncCommand { get; set; }

        public bool IsTopDrawerOpen
        {
            get => _IsTopDrawerOpen;
            set => SetProperty(ref _IsTopDrawerOpen, value);
        }

        public bool IsOpenFilter
        {
            get => _isOpenFilter;
            set => SetProperty(ref _isOpenFilter, value);
        }

        public ICommand MouseOverCommand { get; set; }

        public ICommand OpenDetailCommand { get; set; }

        public ICommand OpenDialogFilterCommand { get; set; }

        public ICommand OrderByCommand { get; set; }

        public ICommand RemoveAsyncCommand { get; set; }

        public ICommand RemoveDefectiveZwaveDeviceAsyncCommand { get; set; }

        public ICommand ReplaceZwaveDeviceAsyncCommand { get; set; }

        public ICommand ResetZwaveNetworkAsyncCommand { get; set; }

        public ICommand RestartZwaveChipAsyncCommand { get; set; }

        private async void AllOff(object obj)
        {
            try
            {
                _ = await _zwaveRepository.AllOff(SelectedProjectModel);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void AllOn(object obj)
        {
            try
            {
                _ = await _zwaveRepository.AllOn(SelectedProjectModel);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void FilterBy()
        {
            if (string.IsNullOrEmpty(FilterZwaveDevices))
            {
                return;
            }

            GatewaysCollectionView.Filter = w =>
            {
                return w is ZwaveDevice device && (FilterByZwave)SelectedProjectModel.SelectedGateway.SelectedIndexFilterby switch
                {
                    FilterByZwave.Name => device.Name.IndexOf(FilterZwaveDevices, StringComparison.OrdinalIgnoreCase) != -1,
                    FilterByZwave.NodeId => device.ModuleId.ToString().IndexOf(FilterZwaveDevices, StringComparison.OrdinalIgnoreCase) != -1,
                    _ => false,
                };
            };
        }

        private void FilterBy(object obj)
        {
            if (obj is not ComboBox comboBox)
            {
                return;
            }
            if (!comboBox.IsMouseOver)
            {
                return;
            }
            FilterBy();
        }

        //private async void GetInfoAllDeviceFromCloud()
        //{
        //    try
        //    {
        //        await _zwaveRepository.GetInfoAllDeviceFromCloud();
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowError(ex);
        //    }
        //}

        private async void GetZwaveDevices(object obj = null)
        {
            try
            {
                if (_parseService.IsSendingToCloud)
                {
                    return;
                }

                DateTime timeNow = DateTime.Now.AddSeconds(2);

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading_device_list,
                                     isProgressBar: true,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                await _gatewayRepository.GetZwaveDevices(SelectedProjectModel);

                OrderBy();

                if (DateTime.Now < timeNow)
                {
                    int delay = (int)Math.Round((timeNow - DateTime.Now).TotalSeconds) * 1000;
                    await Task.Delay(delay);
                }

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task Include()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Inclusion_mode,
                                     message: Domain.Properties.Resources.Gateway_in_inclusion_mode);

                using ZwaveDevice device = await _zwaveRepository.IncludeZwaveDevice(SelectedProjectModel);

                if (device is null)
                {
                    await CloseDialog();
                    return;
                }

                device.IsNew = true;

                if (!await _zwaveRepository.GetDeviceManufacturer(selectedProject: SelectedProjectModel,
                                                                  device: device,
                                                                  isSendingToGateway: true))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         ok: async () => await CloseDialog(),
                                         message: Domain.Properties.Resources.No_devices_found);
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Getting_information_from_the_device);

                await _zwaveRepository.GetInfoDeviceFromCloud(device);

                await _zwaveRepository.GetAssociationNumberGroups(selectedProject: SelectedProjectModel,
                                                                  selectedDevice: device,
                                                                  isSendingToGateway: true);

                SelectedProjectModel.SelectedGateway.SelectedZwaveDevice = device;

                _ = await _zwaveRepository.AddGatewayLifeline(selectedProject: SelectedProjectModel,
                                                              isSendingToGateway: true);

                await _zwaveRepository.GetAllAssociation(selectedProject: SelectedProjectModel,
                                                         selectedDevice: device,
                                                         isSendingToGateway: true);

                _gatewayService.IsSendingToGateway = false;

                await _zwaveRepository.InsertZwaveDevice(SelectedProjectModel, device);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void IncludeAsync(object obj)
        {
            if (_gatewayService.IsSendingToGateway)
            {
                return;
            }

            await Include();
        }

        private void Loaded(object obj)
        {
            if (SelectedProjectModel.SelectedGateway.ZwaveDevices.Any())
            {
                return;
            }

            if (obj is not UserControl view)
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            GetZwaveDevices();
        }

        private void MouseOver(object obj)
        {
            //todo get custom id
            SetDeviceIsNew(obj);
        }

        private void OpenDetail(object obj)
        {
            if (obj is not ZwaveDevice zwave)
            {
                return;
            }

            SelectedProjectModel.SelectedGateway.SelectedZwaveDevice = zwave;

            if (SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.IsGateway)
            {
                return;
            }

            IsTopDrawerOpen = true;
        }

        private void OpenDialogFilter(object obj)
        {
            IsOpenFilter = true;
        }

        private void OrderBy(object obj)
        {
            if (obj is not ComboBox comboBox)
            {
                return;
            }

            if (!comboBox.IsMouseOver)
            {
                return;
            }

            OrderBy();
        }

        private void OrderBy()
        {
            ZwaveDevice[] temp = new ZwaveDevice[SelectedProjectModel.SelectedGateway.ZwaveDevices.Count];

            switch ((OrderByZwaveDevice)SelectedProjectModel.SelectedGateway.SelectedIndexOrderBy)
            {
                case OrderByZwaveDevice.NodeId:
                    switch ((OrderByDirection)SelectedProjectModel.SelectedGateway.SelectedIndexOrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            temp = SelectedProjectModel.SelectedGateway.ZwaveDevices.OrderBy(x => x.ModuleId).ToArray();
                            break;

                        case OrderByDirection.Descending:
                            temp = SelectedProjectModel.SelectedGateway.ZwaveDevices.OrderByDescending(x => x.ModuleId).ToArray();
                            break;
                    }
                    SelectedProjectModel.SelectedGateway.ZwaveDevices.Clear();
                    break;

                case OrderByZwaveDevice.Name:
                    switch ((OrderByDirection)SelectedProjectModel.SelectedGateway.SelectedIndexOrderByDirection)
                    {
                        case OrderByDirection.Ascending:
                            temp = SelectedProjectModel.SelectedGateway.ZwaveDevices.OrderBy(x => x.Name).ToArray();
                            break;

                        case OrderByDirection.Descending:
                            temp = SelectedProjectModel.SelectedGateway.ZwaveDevices.OrderByDescending(x => x.Name).ToArray();
                            break;
                    }
                    SelectedProjectModel.SelectedGateway.ZwaveDevices.Clear();
                    break;

                default:
                    break;
            }

            for (int i = 0; i < temp.Length; i++)
            {
                SelectedProjectModel.SelectedGateway.ZwaveDevices.Add(temp[i]);
            }
        }

        private async Task Remove()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         textButtomOk: Domain.Properties.Resources._Close,
                                         ok: async () => await CloseDialog());
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Removal_mode,
                                     message: Domain.Properties.Resources.Gateway_in_removal_mode,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                int moduleId = await _zwaveRepository.Remove(SelectedProjectModel);

                ZwaveDevice module = SelectedProjectModel.SelectedGateway.ZwaveDevices.FirstOrDefault(x => x.ModuleId == moduleId);

                if (module is not null)
                {
                    await module.ParseObject.DeleteAsync();
                    _ = SelectedProjectModel.SelectedGateway.ZwaveDevices.Remove(module);
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Sucess,
                                         message: $"{module.Name ?? module.DefaultName} {Domain.Properties.Resources.successfully_removed}.",
                                         textButtomOk: Domain.Properties.Resources._Close,
                                         ok: async () => await CloseDialog());
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Sucess,
                                     message: Domain.Properties.Resources.Device_removed,
                                     textButtomOk: Domain.Properties.Resources._Close,
                                     ok: async () => await CloseDialog());
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RemoveAsync(object obj)
        {
            if (_gatewayService.IsSendingToGateway)
            {
                return;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Remove,
                                 message: Domain.Properties.Resources.Do_you_want_to_remove_Z_Wave_device_,
                                 custom: async () => await Remove(),
                                 textButtonCustom: Domain.Properties.Resources._Remove,
                                 textButtonCancel: Domain.Properties.Resources.Cancel,
                                 cancel: async () => await CloseDialog());
        }

        private async Task RemoveDefectiveZwaveDevice(object obj)
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         textButtonCancel: Domain.Properties.Resources.Cancel,
                                         ok: async () => await CloseDialog());
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Remove,
                                     isProgressBar: true,
                                     message: Domain.Properties.Resources.Removing_defective_Z_Wave_device);

                await _zwaveRepository.RemoveDefectiveZwaveDevice(SelectedProjectModel, obj as ZwaveDevice);

                if (obj is ZwaveDevice module)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Sucess,
                                         message: $"{module.Name ?? module.DefaultName} {Domain.Properties.Resources.successfully_removed}.",
                                         textButtomOk: Domain.Properties.Resources._Close,
                                         ok: async () => await CloseDialog());
                    return;
                }

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RemoveDefectiveZwaveDeviceAsync(object obj)
        {
            if (_gatewayService.IsSendingToGateway)
            {
                return;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Remove,
                                 message: Domain.Properties.Resources.Do_you_want_to_remove_Z_Wave_device_,
                                 textButtonCustom: Domain.Properties.Resources._Remove,
                                 custom: async () => await RemoveDefectiveZwaveDevice(obj),
                                 textButtonCancel: Domain.Properties.Resources.Cancel,
                                 cancel: async () => await CloseDialog());
        }

        private async Task ReplaceZwaveDevice(object obj)
        {
            try
            {
                if (obj is not ZwaveDevice)
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

                using ReplaySubject<string> replay = new();

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Replacing_Z_wave_device,
                                     rx: replay,
                                     cancel: async () => await CloseDialog());

                await _zwaveRepository.ReplaceZwaveDevice(SelectedProjectModel, obj as ZwaveDevice, replay);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ReplaceZwaveDeviceAsync(object obj)
        {
            if (_gatewayService.IsSendingToGateway)
            {
                return;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Replace,
                                 message: Domain.Properties.Resources.Remove_defective_Z_Wave_device,
                                 textButtonCustom: Domain.Properties.Resources._Replace,
                                 custom: async () => await ReplaceZwaveDevice(obj),
                                 cancel: async () => await CloseDialog());
        }

        private async Task ResetZwaveNetwork()
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Resetting_z_wave_network,
                                     cancel: async () => await CloseDialog());

                await _zwaveRepository.ResetZwaveNetwork(SelectedProjectModel, isPrimary: true);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ResetZwaveNetworkAsync(object obj)
        {
            if (_gatewayService.IsSendingToGateway)
            {
                return;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Reset,
                                 message: Domain.Properties.Resources.Do_you_want_to_restart_the_Z_Wave_network_,
                                 custom: async () => await ResetZwaveNetwork(),
                                 textButtonCustom: Domain.Properties.Resources._Reset,
                                 cancel: async () => await CloseDialog());
        }

        private async Task RestartZwaveChip()
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
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

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Resetting_Z_Wave_chip,
                                     cancel: async () => await CloseDialog());

                await _zwaveRepository.RestartZwaveChip(SelectedProjectModel);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RestartZwaveChipAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Reset,
                                  message: Domain.Properties.Resources.Do_you_want_to_restart_the_Z_Wave_chip_,
                                  custom: async () => await RestartZwaveChip(),
                                  textButtonCustom: Domain.Properties.Resources._Reset,
                                  cancel: async () => await CloseDialog());
        }

        private void SetDeviceIsNew(object obj)
        {
            if (obj is null)
            {
                return;
            }
            if (obj is not ZwaveDevice device)
            {
                return;
            }
            if (!device.IsNew)
            {
                return;
            }
            device.IsNew = !device.IsNew;
        }
    }
}