using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.Device;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using GongSolutions.Wpf.DragDrop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class SecondaryZwaveViewModel : ProjectViewModelBase, IDropTarget
    {
        private readonly INetworkService _internetService;

        private bool _IsRightDrawerOpen;

        private string _Search;

        private bool _IsTopDrawerOpen;

        public SecondaryZwaveViewModel(IFrameNavigationService navigationService,
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
            LearnZwaveNetworkCommand = new RelayCommand<object>(LearnZwaveNetwork);
            ReloadCommand = new RelayCommand(async () => await GetZwaveDevices());
            RemoveCommand = new RelayCommand<object>(Remove);
            LoadedCommand = new RelayCommand<object>(Loaded);
            RightDrawerOpenCommand = new RelayCommand<object>(RightDrawerOpen);
            OpenDetailCommand = new RelayCommand<object>(OpenDetail);
            ZwaveDevices = new ObservableCollection<ZwaveDevice>();
            ResetZwaveNetworkAsyncCommand = new RelayCommand<object>(ResetZwaveNetworkAsync);
            RestartZwaveChipAsyncCommand = new RelayCommand<object>(RestartZwaveChipAsync);
            ZwaveDevicesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("ZWaveDeviceType"));
            _parseService.PropertyChanged += ParseServicePropertyChanged;
            MouseOverCommand = new RelayCommand<object>(MouseOver);
            AllOffCommand = new RelayCommand<object>(AllOff);
            AllOnCommand = new RelayCommand<object>(AllOn);

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        public override void Unloaded(object obj)
        {
            base.Unloaded(obj);
            IsTopDrawerOpen = false;
            SelectedProjectModel.SelectedGateway.SelectedIndexZwaveDevice = -1;
            if (SelectedProjectModel.SelectedGateway.SelectedZwaveDevice is null)
            {
                return;
            }
            SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.SelectedIndexTabControl = 0;
            SelectedProjectModel.SelectedGateway.SelectedZwaveDevice = null;
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

        public bool IsRightDrawerOpen
        {
            get => _IsRightDrawerOpen;
            set => SetProperty(ref _IsRightDrawerOpen, value);
        }

        public bool IsTopDrawerOpen
        {
            get => _IsTopDrawerOpen;
            set => SetProperty(ref _IsTopDrawerOpen, value);
        }

        private void MouseOver(object obj)
        {
            SetDeviceIsNew(obj);
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

        public ICommand LearnZwaveNetworkCommand { get; set; }

        public ICommand OpenDetailCommand { get; set; }

        public ICommand ResetZwaveNetworkAsyncCommand { get; set; }

        public ICommand RestartZwaveChipAsyncCommand { get; set; }

        public ICommand RightDrawerOpenCommand { get; set; }
        public ICommand MouseOverCommand { get; set; }

        public string Search
        {
            get => _Search;
            set
            {
                _ = SetProperty(ref _Search, value);

                ZwaveDevicesCollectionView.Filter = w =>
                {
                    using ZwaveDevice model = w as ZwaveDevice;

                    return model?.DefaultName?.IndexOf(value ?? string.Empty, StringComparison.OrdinalIgnoreCase) != -1;
                };
            }
        }

        public ICollectionView SecondaryZwaveDevicesCollectionView => CollectionViewSource.GetDefaultView(SelectedProjectModel.SelectedGateway.SecondaryZwaveDevices);

        public ObservableCollection<ZwaveDevice> ZwaveDevices { get; set; }

        public ICollectionView ZwaveDevicesCollectionView => CollectionViewSource.GetDefaultView(ZwaveDevices);

        private async Task GetAllDeviceByCloud()
        {
            try
            {
                if (!ZwaveDevices.Any())
                {
                    ZwaveDevice[] devices = await _zwaveRepository.GetAllDeviceByCloud();

                    IEnumerable<ZwaveDevice> devicesGroupBy = devices.GroupBy(x => x.DefaultName).Select(d => d.First());

                    foreach (ZwaveDevice zwaveDevice in devicesGroupBy)
                    {
                        zwaveDevice.ParseObject = null;
                        ZwaveDevices.Add(zwaveDevice);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task GetGatewayInfo()
        {
            if (!await _gatewayRepository.FindGateway(SelectedProjectModel.SelectedGateway))
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: Domain.Properties.Resources.Gateway_not_found,
                                     cancel: async () => await CloseDialog());
                return;
            }

            await _gatewayRepository.GetGatewayInfo(SelectedProjectModel.SelectedGateway);

            _gatewayService.IsPrimary = SelectedProjectModel.SelectedGateway.IsPrimary;

            if (!await _internetService.IsInternet())
            {
                throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
            }

            await _gatewayRepository.Update(SelectedProjectModel);

            await CloseDialog(0);
        }

        private void GetNextModuleId(ZwaveDevice deviceClone)
        {
            if (!SelectedProjectModel.SelectedGateway.SecondaryZwaveDevices.Any())
            {
                deviceClone.ModuleId = 2;
                return;
            }

            deviceClone.ModuleId = SelectedProjectModel.SelectedGateway.SecondaryZwaveDevices
                                                                       .OrderByDescending(x => x.ModuleId)
                                                                       .FirstOrDefault().ModuleId;

            ++deviceClone.ModuleId;

            if (deviceClone.ModuleId == SelectedProjectModel.SelectedGateway.ModuleId)
            {
                ++deviceClone.ModuleId;
            }
        }

        private readonly CountDownTimer count = new();

        private async Task GetZwaveDevices()
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading_device_list,
                                     isProgressBar: true,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await _gatewayRepository.GetZwaveDevices(SelectedProjectModel);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void LearnZwaveNetwork(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                    message: Domain.Properties.Resources.In_Z_Wave_Network_Learning_Mode,
                                    isProgressBar: true,
                                    cancel: async () => await CloseDialog());

                if (await _zwaveRepository.LearnZwaveNetwork(SelectedProjectModel))
                {
                    await GetGatewayInfo();
                    await GetZwaveDevices();
                }

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public ICommand AllOffCommand { get; set; }
        public ICommand AllOnCommand { get; set; }
        private const int MINID = 1;
        private const int MAXID = 100;

        private async void Loaded(object obj)
        {
            if (SelectedProjectModel.SelectedGateway.IsPrimary)
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

            await GetZwaveDevices();
        }

        private void OpenDetail(object obj)
        {
            SelectedProjectModel.SelectedGateway.SelectedZwaveDevice = SelectedProjectModel.SelectedGateway.SecondaryZwaveDevices[SelectedProjectModel.SelectedGateway.SelectedIndexZwaveDevice];

            IsTopDrawerOpen = true;
        }

        private void ParseServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IsSendingToCloud)))
            {
                IsSendingToCloud = ((ParseService)sender).IsSendingToCloud;
            }
        }

        private async void Remove(object obj)
        {
            try
            {
                if (obj is not ZwaveDevice zwaveDevice)
                {
                    return;
                }
                if (zwaveDevice.ParseObject is null)
                {
                    return;
                }

                if (SelectedProjectModel.SelectedGateway.SecondaryZwaveDevices.Remove(zwaveDevice))
                {
                    await _zwaveRepository.DeleteAsync(zwaveDevice);
                }
            }
            catch (Exception)
            {
                await CloseDialog(0);
            }
        }

        private async Task ResetZwaveNetwork()
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Resetting_z_wave_network,
                                     cancel: async () => await CloseDialog());

                await _zwaveRepository.ResetZwaveNetwork(SelectedProjectModel, isPrimary: false);

                await GetGatewayInfo();

                await CloseDialog();
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

                await CloseDialog();
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

        private async void RightDrawerOpen(object obj)
        {
            try
            {
                IsRightDrawerOpen = true;

                if (ZwaveDevices.Any())
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading_device_list,
                                     isProgressBar: true,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await GetAllDeviceByCloud();

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        #region DragDrop

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                return;
            }

            if (dropInfo.Data is ZwaveDevice && dropInfo.TargetCollection != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            try
            {
                if (dropInfo is null)
                {
                    throw new ArgumentNullException(nameof(dropInfo));
                }

                if (dropInfo.TargetCollection is null)
                {
                    return;
                }

                if (dropInfo.Data is ZwaveDevice device)
                {
                    if ((ObservableCollection<ZwaveDevice>)dropInfo.TargetCollection is not ObservableCollection<ZwaveDevice> list)
                    {
                        return;
                    }

                    ZwaveDevice deviceClone = JsonConvert.DeserializeObject<ZwaveDevice>(JsonConvert.SerializeObject(device));

                    GetNextModuleId(deviceClone);

                    customMessageBoxModel.Input = deviceClone.ModuleId + "";

                    deviceClone.IsNew = true;

                    OpenCustomMessageBox(header: Domain.Properties.Resources.Module_Id,
                                         custom: () => AddDevice(deviceClone),
                                         textButtonCustom: Domain.Properties.Resources.Add,
                                         isInput: true);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void AddDevice(ZwaveDevice deviceClone)
        {
            if (!int.TryParse(customMessageBoxModel.Input, out int moduleId))
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Module_Id,
                                     message: Domain.Properties.Resources.Only_numbers,
                                     custom: () => AddDevice(deviceClone),
                                     textButtonCustom: Domain.Properties.Resources.Add,
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: () => { IsOpenDialogHost = false; },
                                     isInput: true);
                return;
            }
            if (moduleId is < 2 or > 99)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Module_Id,
                                     message: string.Format(CultureInfo.CurrentCulture, Domain.Properties.Resources.The_ID_must_be_greater_than_n_and_less_than_n, MINID, MAXID),
                                     custom: () => AddDevice(deviceClone),
                                     textButtonCustom: Domain.Properties.Resources.Add,
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: () => { IsOpenDialogHost = false; },
                                     isInput: true);
                return;
            }

            deviceClone.ModuleId = moduleId;

            IsOpenDialogHost = false;

            await _zwaveRepository.InsertZwaveDevice(SelectedProjectModel, deviceClone, secondary: true);
        }

        #endregion DragDrop
    }
}