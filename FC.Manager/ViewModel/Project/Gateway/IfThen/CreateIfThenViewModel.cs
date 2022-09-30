using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Device;
using FC.Domain.Model.FlexCloudClone;
using FC.Domain.Model.IfThen;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using GongSolutions.Wpf.DragDrop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.IfThen
{
    public class CreateIfThenViewModel : ProjectViewModelBase, IDropTarget
    {
        private readonly IIfThenRepository _ifThenRepository;
        private readonly IIfThenService _ifThenService;
        private ZwaveDevice _copiedObject;
        private bool _IsExpandedElse = true;
        private bool _IsExpandedIf = true;
        private bool _IsExpandedThen = true;
        private bool _IsRightDrawerOpen = true;
        private IfThenModel _selectedifThenModel;
        private ZwaveDevice _selectedZwaveDevice;

        public CreateIfThenViewModel(IFrameNavigationService navigationService,
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
                                 IIfThenService ifThenService,
                                 IIfThenRepository ifThenRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _ifThenService = ifThenService;

            _ifThenRepository = ifThenRepository;

            CreateCommand = new RelayCommand<object>(async (obj) => await Create(obj));

            GetGatewayClockTimeCommand = new RelayCommand<object>(GetGatewayClockTime);

            LoadedCommand = new RelayCommand<object>(Loaded);

            UnloadedCommand = new RelayCommand<object>(Unloaded);

            ReloadCommand = new RelayCommand<object>(GetZwavesDevice);

            ResetCommand = new RelayCommand<object>(Reset);

            RemoveCommand = new RelayCommand<object>(Remove);

            TestCommand = new RelayCommand<object>(Test);

            BackCommand = new RelayCommand<object>(Back);

            GetAllCommand = new RelayCommand<object>(GetAll);

            PlayMacroCommand = new RelayCommand<object>(PlayMacro);

            StopMacroCommand = new RelayCommand<object>(StopMacro);

            DeleteAsyncCommand = new RelayCommand<object>(DeleteAsync);

            RenameCommand = new RelayCommand<object>(Rename);

            SelectionChangedCommand = new RelayCommand<object>(SelectionChanged);

            CopyCommand = new RelayCommand<object>(Copy);

            PasteCommand = new RelayCommand<object>(Paste);

            PlayFuncionCommand = new RelayCommand<object>(PlayFuncion);

            OpenDrawerCommand = new RelayCommand<object>(OpenDrawer);

            GetEndpointValueCommand = new RelayCommand<object>(GetEndpointValue);

            EnabledCommand = new RelayCommand<object>(Enabled);

            ChangeTabCommand = new RelayCommand<object>(ChangeTab);

            NewCommand = new RelayCommand<object>(NewAsync);

            GetTemperatureCommand = new RelayCommand<object>(GetTemperature);

            SelectedIfThenModel = new IfThenModel();

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });

            ZwaveDevices = new ObservableCollection<ZwaveDevice>();

            ScheduleModel = new ZwaveDevice
            {
                IfthenType = IfthenType.Schedule,
                IsHiddenLogicGateIfThen = true,
                IsHiddenDelay = true,
                IsHiddenDelete = true,
            };
        }

        private readonly CountDownTimer count = new();

        private async void GetTemperature(object obj)
        {
            try
            {
                if (obj is not ZwaveDevice zwaveDevice)
                {
                    return;
                }
                if (zwaveDevice.CustomId != ZwaveModelUtil.ZXT600)
                {
                    return;
                }

                SelectedProjectModel.SelectedGateway.SelectedZwaveDevice = zwaveDevice;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Checking_ambient_temperature,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog(),
                                     isProgressBar: true);

                if (await _zwaveRepository.GetMultiLevelSensorStatus(SelectedProjectModel,
                                                                       SensorTypeEnum.Temp,
                                                                       TemperatureScaleEnum.Celsius,
                                                                       pingGateway: false,
                                                                       pingZwave: false) is not JObject json)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetTemperature(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                zwaveDevice.RoomTemperature = json.Value<int>(UtilZwave.VALUE);

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

        #region ICommand

        public ICommand GetTemperatureCommand { get; set; }
        public ICommand BackCommand { get; set; }

        public ICommand EnabledCommand { get; set; }

        public ICommand ChangeTabCommand { get; set; }

        public ICommand DeleteAsyncCommand { get; set; }

        public ICommand GetAllCommand { get; set; }

        public ICommand GetEndpointValueCommand { get; set; }

        public ICommand NewCommand { get; set; }

        public ICommand GetGatewayClockTimeCommand { get; set; }

        public ICommand OpenDrawerCommand { get; set; }

        public ICommand PasteCommand { get; set; }

        public ICommand PlayFuncionCommand { get; set; }

        public ICommand PlayMacroCommand { get; set; }

        public ICommand RenameCommand { get; set; }

        public ICommand ResetCommand { get; set; }

        public ICommand SelectionChangedCommand { get; set; }

        public ICommand StopMacroCommand { get; set; }

        public ICommand TestCommand { get; set; }

        #endregion ICommand

        #region Properties

        public ZwaveDevice ScheduleModel { get; set; }

        public ZwaveDevice CopiedObject
        {
            get => _copiedObject;
            set => SetProperty(ref _copiedObject, value);
        }

        public bool IsExpandedElse
        {
            get => _IsExpandedElse;
            set => SetProperty(ref _IsExpandedElse, value);
        }

        public bool IsExpandedIf
        {
            get => _IsExpandedIf;
            set => SetProperty(ref _IsExpandedIf, value);
        }

        public bool IsExpandedThen
        {
            get => _IsExpandedThen;
            set => SetProperty(ref _IsExpandedThen, value);
        }

        public bool IsRightDrawerOpen
        {
            get => _IsRightDrawerOpen;
            set => SetProperty(ref _IsRightDrawerOpen, value);
        }

        public string LastParseObjectId { get; private set; }

        public IfThenModel SelectedIfThenModel
        {
            get => _selectedifThenModel;
            set => SetProperty(ref _selectedifThenModel, value);
        }

        public ZwaveDevice SelectedZwaveDevice
        {
            get => _selectedZwaveDevice;
            set => SetProperty(ref _selectedZwaveDevice, value);
        }

        #endregion Properties

        #region Collections

        public ObservableCollection<ZwaveDevice> ZwaveDevices { get; set; }

        #endregion Collections

        #region Methods

        public override void Unloaded(object obj)
        {
            base.Unloaded(obj);
            Back();
        }

        private void NewAsync(object obj)
        {
            if (SelectedIfThenModel.ParseObject is null)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.New,
                                     message: Domain.Properties.Resources.The_If_Then_has_not_been_saved__Want_to_save_before_creating_a_new_one_,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources._Close,
                                     ok: async () => await Create(),
                                     textButtomOk: Domain.Properties.Resources.Save,
                                     custom: () => New(),
                                     textButtonCustom: Domain.Properties.Resources.New);
                return;
            }
            New();
        }

        private async void New()
        {
            string tempName = SelectedIfThenModel.Name;
            SelectedIfThenModel = new IfThenModel
            {
                Name = tempName
            };
            await CloseDialog();
        }

        private void ChangeTab(object obj)
        {
            List<TabItem> tabs = _gatewayService.TabControl.Items.Cast<TabItem>().ToList();
            TabItem tabItem = tabs.FirstOrDefault(x => x.Header.ToString() == (SelectedProjectModel.SelectedGateway.IsPrimary ? Domain.Properties.Resources.Primary_Z_Wave : Domain.Properties.Resources.Secondary_Z_Wave));
            int index = tabs.IndexOf(tabItem);
            SelectedProjectModel.SelectedGateway.SelectedIndexTabControl = index;
        }

        private async void GetGatewayClockTime(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: $"{Domain.Properties.Resources.Checking_gateway_date_and_time}");

                await _ifThenRepository.GetGatewayClockTime(SelectedProjectModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void Enabled(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: SelectedIfThenModel.IsEnabled ? $"{Domain.Properties.Resources.Enabling}" : $"{Domain.Properties.Resources.Disabling}");

                await _ifThenRepository.RuleIdEnabled(SelectedProjectModel, SelectedIfThenModel);

                await _ifThenRepository.Update(SelectedProjectModel, SelectedIfThenModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public async void Test(object @object)
        {
            try
            {
                if (@object is null)
                {
                    throw new ArgumentNullException(nameof(@object));
                }

                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                if (@object is GatewayModel gateway)
                {
                    await _gatewayRepository.Test(gateway);
                    return;
                }

                if (@object is ZwaveDevice zwaveDevice)
                {
                    _ = await _zwaveRepository.Test(SelectedProjectModel, zwaveDevice);
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Back(object obj = null)
        {
            _ifThenService.Source = new Uri(AppConstants.LISTIFTHENPAGE, UriKind.Relative);
        }

        private void Copy(object obj)
        {
            if (obj is not ZwaveDevice temp)
            {
                return;
            }

            CopiedObject = temp;
        }

        private async Task Create(object obj = null)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Creating_If_Then);

                await _ifThenRepository.CreateIfThen(SelectedProjectModel, SelectedIfThenModel);

                await _ifThenRepository.Update(SelectedProjectModel, SelectedIfThenModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                if (IsCanceled)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: $"{Domain.Properties.Resources.Task_canceled}",
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }
                OpenCustomMessageBox(header: Domain.Properties.Resources.Try_again,
                                     message: ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     custom: async () => await Create(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again);
            }
        }

        private async void Delete()
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                _ = await _ifThenRepository.DeleteIfThenFromGataway(SelectedProjectModel, SelectedIfThenModel);

                await _ifThenRepository.DeleteIfThenFromCloud(SelectedProjectModel, SelectedIfThenModel);

                _ifThenService.Source = new Uri(AppConstants.LISTIFTHENPAGE, UriKind.Relative);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAsync(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                     message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.You_want_to_delete_that_ifthen, SelectedIfThenModel.Name),
                                     custom: () => Delete(),
                                     textButtonCustom: Domain.Properties.Resources.Delete,
                                     cancel: async () => await CloseDialog());
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetAll(object obj)
        {
            if (_gatewayService.IsSendingToGateway)
            {
                return;
            }

            if (obj is not GatewayModel gateway)
            {
                return;
            }

            try
            {
                gateway.IsSync = true;

                if (gateway.IsSupportsIR)
                {
                    _ = await _irRepository.GetAll(gateway);
                }

                if (gateway.IsSupportsRF)
                {
                    await _rfRepository.GetAll(gateway, TypeRF.Any);
                }
                if (gateway.IsSupportsSerial)
                {
                    await _serialRepository.GetAll(gateway);
                }

                await _ipCommandRepository.GetAll(gateway);

                if (gateway.IpCommands.Any())
                {
                    gateway.SelectedIndexIPCommand = 0;
                }

                if (gateway.Serials.Any())
                {
                    gateway.SelectedIndexSerial = 0;
                }

                if (gateway.IRsGateway.Any())
                {
                    gateway.SelectedIndexIR = 0;
                }

                if (gateway.Radios433Gateway.Any())
                {
                    gateway.SelectedIndexRadio433 = 0;
                }

                if (gateway.RadiosRTSGateway.Any())
                {
                    gateway.SelectedIndexRTS = 0;

                    gateway.SelectedIndexActionRTSSomfy = 0;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                gateway.IsSync = false;
            }
        }

        private async void GetEndpointValue(object obj)
        {
            try
            {
                if (obj is not ZwaveDevice zwave)
                {
                    return;
                }
                _ = await _zwaveRepository.GetEndpointValue(SelectedProjectModel, zwave);
                await CloseDialog();
            }
            catch (Exception)
            {
            }
        }

        private async void GetIfThen(IfThenModel selectedIfThenModel)
        {
            try
            {
                if (selectedIfThenModel is null)
                {
                    _ifThenService.Source = new Uri(AppConstants.LISTIFTHENPAGE, UriKind.Relative);
                    return;
                }

                if (!SelectedProjectModel.SelectedGateway.ZwaveDevices.Any())
                {
                    await _gatewayRepository.GetZwaveDevices(SelectedProjectModel);
                }

                if (selectedIfThenModel.ParseObject != null)
                {
                    await _ifThenRepository.GetIfThen(SelectedProjectModel, selectedIfThenModel);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetZwavesDevice(object @object = null)
        {
            try
            {
                ZwaveDevices.Clear();

                if ((SelectedProjectModel.SelectedGateway.IsPrimary && !SelectedProjectModel.SelectedGateway.ZwaveDevices.Any()) ||
                    (!SelectedProjectModel.SelectedGateway.IsPrimary && !SelectedProjectModel.SelectedGateway.SecondaryZwaveDevices.Any()) ||
                    @object is bool)
                {
                    await _gatewayRepository.GetZwaveDevices(SelectedProjectModel);
                }

                foreach (ZwaveDevice zwave in SelectedProjectModel.SelectedGateway.IsPrimary ? SelectedProjectModel.SelectedGateway.ZwaveDevices : SelectedProjectModel.SelectedGateway.SecondaryZwaveDevices)
                {
                    if (zwave.ZWaveComponents == ZWaveComponents.Controller)
                    {
                        continue;
                    }
                    ZwaveDevices.Add(zwave);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            SelectedIfThenModel = _projectService.SelectedProject.SelectedIfThen;

            GetZwavesDevice();

            if (obj is not Page view)
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            GetIfThen(SelectedIfThenModel);
        }

        private void OpenDrawer(object obj)
        {
            IsRightDrawerOpen = true;
            IsExpandedIf = true;
            IsExpandedElse = true;
            IsExpandedThen = true;
        }

        private void Paste(object obj)
        {
            if (obj is not object[] temp)
            {
                return;
            }

            int selectedIndex = (int)temp[1];

            if (temp[0] is ObservableCollection<ZwaveDevice> list)
            {
                list.Insert(selectedIndex, JsonConvert.DeserializeObject<ZwaveDevice>(JsonConvert.SerializeObject(CopiedObject)));

                list[selectedIndex].ImageParseFile = CopiedObject.ImageParseFile;

                foreach (ZwaveDevice item in list)
                {
                    item.IsHiddenDelay = false;
                    item.IsHiddenLogicGateIfThen = false;
                }

                list.Last().IsHiddenDelay = true;
                list.First().IsHiddenLogicGateIfThen = true;
            }
        }

        private async void PlayFuncion(object obj)
        {
            try
            {
                if (obj is not ZwaveDevice temp)
                {
                    return;
                }

                if (temp.IfthenType == IfthenType.Device)
                {
                    _ = await _zwaveRepository.Test(SelectedProjectModel, temp);
                    await CloseDialog();
                    return;
                }

                if (SelectedProjectModel.Devices.FirstOrDefault(x => x.UID == temp.GatewayFunctionUID) is not GatewayModel gateway)
                {
                    await CloseDialog();
                    return;
                }

                switch (temp.IfthenType)
                {
                    case IfthenType.IR:
                        await _irRepository.PlayMemory(gateway, temp.GatewayFunctionMemoryId);
                        break;

                    case IfthenType.Radio433:
                        await _rfRepository.PlayMemory(gateway, temp.GatewayFunctionMemoryId);
                        break;

                    case IfthenType.RTS:
                        await _rfRepository.PlayMemory(gateway, temp.GatewayFunctionMemoryId);
                        break;

                    case IfthenType.Schedule:
                        break;

                    case IfthenType.IPCommand:
                        await _ipCommandRepository.PlayMemory(gateway, temp.GatewayFunctionMemoryId);
                        break;

                    case IfthenType.Relay:
                        await _relayTestRepository.SetRelayState(gateway);
                        break;

                    case IfthenType.Serial:
                        await _serialRepository.PlayByMemoryId(gateway, temp.GatewayFunctionMemoryId);
                        break;
                }
                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void PlayMacro(object obj)
        {
            try
            {
                if (obj is not int id)
                {
                    return;
                }

                await _ifThenRepository.PlayMacroAsync(SelectedProjectModel, id);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Remove(object obj)
        {
            if (obj is not ZwaveDevice device)
            {
                return;
            }

            if (SelectedIfThenModel.ZwaveDevicesIf.Remove(device))
            {
                if (device.ParseObject is ParseObject parse)
                {
                    SelectedIfThenModel.DeletedZwaveDevices.Add(parse);
                }

                if (SelectedIfThenModel.ZwaveDevicesIf.Count < 1)
                {
                    return;
                }

                if (!SelectedIfThenModel.ZwaveDevicesIf.Any())
                {
                    return;
                }

                SelectedIfThenModel.ZwaveDevicesIf[0].IsHiddenLogicGateIfThen = true;
            }
            if (SelectedIfThenModel.ZwaveDevicesThen.Remove(device))
            {
                if (device.ParseObject is ParseObject parse)
                {
                    SelectedIfThenModel.DeletedZwaveDevices.Add(parse);
                }

                if (!SelectedIfThenModel.ZwaveDevicesThen.Any())
                {
                    return;
                }

                foreach (ZwaveDevice item in SelectedIfThenModel.ZwaveDevicesThen)
                {
                    item.IsHiddenDelay = false;
                }

                SelectedIfThenModel.ZwaveDevicesThen.Last().IsHiddenDelay = true;

                return;
            }

            if (SelectedIfThenModel.ZwaveDevicesElse.Remove(device))
            {
                if (device.ParseObject is ParseObject parse)
                {
                    SelectedIfThenModel.DeletedZwaveDevices.Add(parse);
                }

                if (!SelectedIfThenModel.ZwaveDevicesElse.Any())
                {
                    return;
                }

                foreach (ZwaveDevice item in SelectedIfThenModel.ZwaveDevicesElse)
                {
                    item.IsHiddenDelay = false;
                }

                SelectedIfThenModel.ZwaveDevicesElse.Last().IsHiddenDelay = true;
            }
        }

        private async void Rename(object obj)
        {
            try
            {
                if (_parseService.IsSendingToCloud)
                {
                    return;
                }

                await _ifThenRepository.Rename(SelectedIfThenModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void Reset(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Reset,
                                     message: Domain.Properties.Resources.Reseting_Device,
                                     cancel: async () => await CloseDialog());

                await _ifThenRepository.ResetDevice(SelectedProjectModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void SelectionChanged(object obj)
        {
            if (_gatewayService.IsSendingToGateway)
            {
                return;
            }

            if (obj is not GatewayModel gateway)
            {
                return;
            }

            try
            {
                gateway.IsSync = true;

                switch ((GatewayFunctions)gateway.SelectedTabIndexIfThen)
                {
                    case GatewayFunctions.IPCommand:
                        if (gateway.IpCommands.Any())
                        {
                            gateway.SelectedIndexIPCommand = gateway.SelectedIndexIPCommand == -1 ? 0 : gateway.SelectedIndexIPCommand;
                            return;
                        }
                        await _ipCommandRepository.GetAll(gateway);
                        gateway.SelectedIndexIPCommand = gateway.IpCommands.Any() ? 0 : -1;
                        break;

                    case GatewayFunctions.IR:
                        if (gateway.IRsGateway.Any())
                        {
                            gateway.SelectedIndexIR = gateway.SelectedIndexIR == -1 ? 0 : gateway.SelectedIndexIR;
                            return;
                        }
                        _ = await _irRepository.GetAll(gateway);
                        gateway.SelectedIndexIR = gateway.IRsGateway.Any() ? 0 : -1;
                        break;

                    case GatewayFunctions.Radio433:
                        if (gateway.Radios433Gateway.Any())
                        {
                            gateway.SelectedIndexRadio433 = gateway.SelectedIndexRadio433 == -1 ? 0 : gateway.SelectedIndexRadio433;
                            return;
                        }
                        await _rfRepository.GetAll(gateway, TypeRF.R433);
                        gateway.SelectedIndexRadio433 = gateway.Radios433Gateway.Any() ? 0 : -1;
                        break;

                    case GatewayFunctions.RTS:
                        if (gateway.RadiosRTSGateway.Any())
                        {
                            gateway.SelectedIndexRTS = gateway.SelectedIndexRTS == -1 ? 0 : gateway.SelectedIndexRTS;
                            return;
                        }
                        await _rfRepository.GetAll(gateway, TypeRF.RTS);
                        gateway.SelectedIndexRTS = gateway.RadiosRTSGateway.Any() ? 0 : -1;
                        break;

                    case GatewayFunctions.Serial:
                        if (gateway.Serials.Any())
                        {
                            gateway.SelectedIndexSerial = gateway.SelectedIndexSerial == -1 ? 0 : gateway.SelectedIndexSerial;
                            return;
                        }
                        await _serialRepository.GetAll(gateway);
                        gateway.SelectedIndexSerial = gateway.Serials.Any() ? 0 : -1;
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                gateway.IsSync = false;
            }
        }

        private async void StopMacro(object obj)
        {
            if (obj is not int id)
            {
                return;
            }

            await _ifThenRepository.StopMacroAsync(SelectedProjectModel, id);
        }

        #endregion Methods

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
            if (dropInfo.Data is GatewayModel && dropInfo.TargetCollection != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                throw new ArgumentNullException(nameof(dropInfo));
            }
            if (dropInfo.TargetCollection is null)
            {
                return;
            }

            if (dropInfo.Data is GatewayModel gateway)
            {
                if (((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesIf")
                {
                    return;
                }

                if (!gateway.IsSuportThenElse && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesThen")
                {
                    return;
                }

                if (!gateway.IsSuportThenElse && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesElse")
                {
                    return;
                }

                if ((ObservableCollection<ZwaveDevice>)dropInfo.TargetCollection is not ObservableCollection<ZwaveDevice> list)
                {
                    return;
                }

                ZwaveDevice zwaveDevice = new();

                switch ((GatewayFunctions)gateway.SelectedTabIndexIfThen)
                {
                    case GatewayFunctions.IR:

                        if (gateway.SelectedIndexIR < 0)
                        {
                            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                                 message: Domain.Properties.Resources.Select_a_combobox_item,
                                                 cancel: async () => await CloseDialog());

                            return;
                        }

                        zwaveDevice.IfthenType = IfthenType.IR;
                        zwaveDevice.Name = Domain.Properties.Resources.IR;
                        zwaveDevice.SelectedIndexIR = gateway.SelectedIndexIR;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionMemoryId = gateway.GatewayFunctionMemoryId;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    case GatewayFunctions.Radio433:

                        if (gateway.SelectedIndexRadio433 < 0)
                        {
                            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                                 message: Domain.Properties.Resources.Select_a_combobox_item,
                                                 cancel: async () => await CloseDialog());
                            return;
                        }

                        zwaveDevice.IfthenType = IfthenType.Radio433;
                        zwaveDevice.Name = Domain.Properties.Resources.Radio_433MHz;
                        zwaveDevice.SelectedIndexRadio433 = gateway.SelectedIndexRadio433;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionMemoryId = gateway.GatewayFunctionMemoryId;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    case GatewayFunctions.RTS:

                        if (gateway.SelectedIndexRTS < 0)
                        {
                            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                                 message: Domain.Properties.Resources.Select_a_combobox_item,
                                                 cancel: async () => await CloseDialog());
                            return;
                        }

                        zwaveDevice.IfthenType = IfthenType.RTS;
                        zwaveDevice.Name = Domain.Properties.Resources.RTS;
                        zwaveDevice.SelectedIndexRTS = gateway.SelectedIndexRTS;
                        zwaveDevice.SelectedIndexActionRTSSomfy = gateway.SelectedIndexActionRTSSomfy;
                        zwaveDevice.ActionsRTSSomfy = (ActionsRTSSomfy)gateway.SelectedIndexActionRTSSomfy + 1;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionMemoryId = gateway.GatewayFunctionMemoryId;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    case GatewayFunctions.IPCommand:

                        if (gateway.SelectedIndexIPCommand < 0)
                        {
                            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                                 message: Domain.Properties.Resources.Select_a_combobox_item,
                                                 cancel: async () => await CloseDialog());
                            return;
                        }

                        zwaveDevice.IfthenType = IfthenType.IPCommand;
                        zwaveDevice.Name = Domain.Properties.Resources.Ip_Command;
                        zwaveDevice.SelectedIndexRTS = gateway.SelectedIndexIPCommand;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionMemoryId = gateway.GatewayFunctionMemoryId;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    case GatewayFunctions.Relay:

                        zwaveDevice.IfthenType = IfthenType.Relay;
                        zwaveDevice.Name = Domain.Properties.Resources.Relay;
                        zwaveDevice.SelectedIndexRelayStateMode = gateway.SelectedIndexRelayStateMode;
                        zwaveDevice.RelayPulseTime = gateway.RelayPulseTime;
                        zwaveDevice.StateRelay = gateway.StateRelay;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    case GatewayFunctions.Serial:
                        zwaveDevice.IfthenType = IfthenType.Serial;
                        zwaveDevice.Name = Domain.Properties.Resources.Serial;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionMemoryId = gateway.GatewayFunctionMemoryId;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    default:
                        return;
                }

                foreach (ZwaveDevice item in list)
                {
                    item.IsHiddenDelay = false;
                }

                list.Add(JsonConvert.DeserializeObject<ZwaveDevice>(JsonConvert.SerializeObject(zwaveDevice)));

                if (SelectedIfThenModel.ZwaveDevicesIf.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesIf[0].IsHiddenLogicGateIfThen = true;
                    SelectedIfThenModel.ZwaveDevicesIf[0].SelectedIndexLogicGateIfThen = 2;
                }

                if (SelectedIfThenModel.ZwaveDevicesThen.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesThen.Last().IsHiddenDelay = true;
                }

                if (SelectedIfThenModel.ZwaveDevicesElse.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesElse.Last().IsHiddenDelay = true;
                }
            }
            else if (dropInfo.Data is ZwaveDevice device)
            {
                if (!device.IsSuportIf && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesIf" && device.IfthenType == IfthenType.Device)
                {
                    return;
                }

                if (!device.IsSuportThenElse && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesThen" && string.IsNullOrEmpty(device.GatewayFunctionUID))
                {
                    return;
                }

                if (!device.IsSuportThenElse && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesElse" && string.IsNullOrEmpty(device.GatewayFunctionUID))
                {
                    return;
                }

                if ((ObservableCollection<ZwaveDevice>)dropInfo.TargetCollection is not ObservableCollection<ZwaveDevice> list)
                {
                    return;
                }

                if (list.Count >= AppConstants.MAXIMUMNUMBEROBJECTSINTHEIF && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesIf")
                {
                    return;
                }

                foreach (ZwaveDevice item in list)
                {
                    item.IsHiddenDelay = false;
                }

                device.IsHiddenLogicGateIfThen = false;

                if (device.IfthenType == IfthenType.Schedule)
                {
                    ZwaveDevice temp = JsonConvert.DeserializeObject<ZwaveDevice>(JsonConvert.SerializeObject(device));
                    temp.DaysOfWeekList = JsonConvert.DeserializeObject<ObservableCollection<DaysOfWeekModel>>(JsonConvert.SerializeObject(device.DaysOfWeekList));
                    list.Insert(dropInfo.InsertIndex, temp);
                }
                else
                {
                    list.Insert(dropInfo.InsertIndex, JsonConvert.DeserializeObject<ZwaveDevice>(JsonConvert.SerializeObject(device)));
                }

                list[dropInfo.InsertIndex].ImageParseFile = device.ImageParseFile;

                _ = list.Remove(device);

                if (SelectedIfThenModel.ZwaveDevicesIf.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesIf[0].IsHiddenLogicGateIfThen = true;
                    SelectedIfThenModel.ZwaveDevicesIf[0].SelectedIndexLogicGateIfThen = 2;
                }

                if (SelectedIfThenModel.ZwaveDevicesThen.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesThen.Last().IsHiddenDelay = true;
                }

                if (SelectedIfThenModel.ZwaveDevicesElse.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesElse.Last().IsHiddenDelay = true;
                }
            }
        }

        private static ZwaveDevice ScheduleToZwaveDevice(ZwaveDevice schedule)
        {
            return new ZwaveDevice
            {
                IfthenType = IfthenType.Schedule,
                Name = Domain.Properties.Resources.IR,
                DaysOfWeekList = schedule.DaysOfWeekList
                //SelectedIndexDateType = schedule.SelectedIndexDateType,
                //SelectedDateType = (DateTypeEnum)schedule.SelectedIndexDateType,
                //SelectedIndexDaysOfWeek = schedule.SelectedIndexDaysOfWeek,
                //SelectedDaysOfWeek = (DaysOfWeek)schedule.SelectedIndexDaysOfWeek,
                //SelectedIndexOperatorsType = schedule.SelectedIndexOperatorsType,
                //SelectedOperatorsTypeSchedule = (OperatorsTypeSchedule)schedule.SelectedIndexOperatorsType,
                //TimePickerValue = schedule.TimePickerValue,
                //ValueSchedule = schedule.ValueSchedule,
            };
        }

        #endregion DragDrop
    }
}