using ConfigurationFlexCloudHubBlaster.Model;
using ConfigurationFlexCloudHubBlaster.Model.Device;
using ConfigurationFlexCloudHubBlaster.Model.FlexCloudClone;
using ConfigurationFlexCloudHubBlaster.Model.IfThen;
using ConfigurationFlexCloudHubBlaster.Repository;
using ConfigurationFlexCloudHubBlaster.Repository.Gateway;
using ConfigurationFlexCloudHubBlaster.Repository.Zwave;
using ConfigurationFlexCloudHubBlaster.Service;
using ConfigurationFlexCloudHubBlaster.ViewModel.Components;
using GalaSoft.MvvmLight.Command;
using GongSolutions.Wpf.DragDrop;
using Newtonsoft.Json;
using Parse;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConfigurationFlexCloudHubBlaster.ViewModel.Project.Device.IfThen
{
    public class CreateIfThenViewModel : ProjectViewModelBase, IDropTarget
    {
        private IIfThenService _ifThenService;

        private IIfThenRepository _ifThenRepository;

        private IfThenModel _selectedifThenModel;

        public IfThenModel SelectedIfThenModel
        {
            get { return _selectedifThenModel; }
            set
            {
                Set(ref _selectedifThenModel, value);
            }
        }

        public ICommand ResetCommand { get; set; }
        public ICommand PlayMacroCommand { get; set; }
        public ICommand StopMacroCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand TestCommand { get; set; }
        public ICommand BackCommand { get; set; }
        public ICommand GetAllCommand { get; set; }
        public ICommand DeleteAsyncCommand { get; set; }
        public ICommand RenameCommand { get; set; }

        public CreateIfThenViewModel(IFrameNavigationService navigationService,
                                     IProjectService projectService,
                                     IUdpRepository udpRepository,
                                     ITCPRepository tcpRepository,
                                     IIRRepository irRepository,
                                     IUserRepository userService,
                                     ILicenseRepository licenseRepository,
                                     IProjectRepository projectRepository,
                                     ILocalDBRepository localDBRepository,
                                     ILogRepository logRepository,
                                     IUserRepository userRepository,
                                     ISerialRepository serialRepository,
                                     ICommandRepository commandRepository,
                                     IIPCommandRepository ipCommandRepository,
                                     IGatewayService gatewayService,
                                     IConfigurationRepository configurationRepository,
                                     ITaskService taskService,
                                     IParseService parseService,
                                     IRFRepository rfRepository,
                                     IZwaveRepository zwaveRepository,
                                     IRelayTestRepository relayTestRepository,
                                     IGatewayRepository gatewayRepository,
                                     IIfThenRepository ifThenRepository,
                                     IIfThenService ifThenService,
                                     IAmbienceRepository ambienceRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, licenseRepository, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, ambienceRepository)
        {
            _ifThenService = ifThenService;

            _ifThenRepository = ifThenRepository;

            CreateCommand = new RelayCommand<object>(Create);

            LoadedCommand = new RelayCommand<object>(Loaded);

            ResetCommand = new RelayCommand<object>(Reset);

            RemoveCommand = new RelayCommand<object>(Remove);

            TestCommand = new RelayCommand<object>(Test);

            BackCommand = new RelayCommand<object>(Back);

            GetAllCommand = new RelayCommand<object>(GetAll);

            PlayMacroCommand = new RelayCommand<object>(PlayMacro);

            StopMacroCommand = new RelayCommand<object>(StopMacro);

            DeleteAsyncCommand = new RelayCommand<object>(DeleteAsync);

            RenameCommand = new RelayCommand<object>(Rename);

            SelectedIfThenModel = new IfThenModel();
        }

        private async void Rename(object obj)
        {
            try
            {
                if (_parseService.IsSendingToCloud)
                {
                    return;
                }

                await _ifThenRepository.Rename(SelectedIfThenModel).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await ShowError(ex).ConfigureAwait(true);
            }
        }

        private async void DeleteAsync(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                OpenCustomMessageBox(header: Properties.Resources.Delete,
                                     message: string.Format(CultureInfo.InvariantCulture, Properties.Resources.You_want_to_delete_that_ifthen, SelectedIfThenModel.Name),
                                     custom: () => Delete(),
                                     textButtonCustom: Properties.Resources.Delete,
                                     cancel: async () => await CloseDialog().ConfigureAwait(true));
            }
            catch (Exception ex)
            {
                await ShowError(ex).ConfigureAwait(true);
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

                await _ifThenRepository.DeleteIfThenFromGataway(SelectedProjectModel, SelectedIfThenModel).ConfigureAwait(true);

                await _ifThenRepository.DeleteIfThenFromCloud(SelectedProjectModel, SelectedIfThenModel).ConfigureAwait(true);

                await CloseDialog().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await ShowError(ex).ConfigureAwait(true);
            }
        }

        private async void GetAll(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                if (!(obj is GatewayModel gateway))
                {
                    return;
                }

                await _irRepository.GetAllFromGateway(gateway).ConfigureAwait(true);

                if (gateway.IRsGateway.Any())
                {
                    gateway.SelectedIndexIR = 0;
                }

                if (gateway.ProductId != 255)
                {
                    return;
                }

                await _rfRepository.GetAll(gateway, TypeRF.Any).ConfigureAwait(true);

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
                await ShowError(ex).ConfigureAwait(true);
            }
        }

        private async void StopMacro(object obj)
        {
            if (!(obj is int id))
            {
                return;
            }

            await _ifThenRepository.StopMacroAsync(SelectedProjectModel, id).ConfigureAwait(true);
        }

        private async void PlayMacro(object obj)
        {
            if (!(obj is int id))
            {
                return;
            }

            await _ifThenRepository.PlayMacroAsync(SelectedProjectModel, id).ConfigureAwait(true);
        }

        private void Back(object obj)
        {
            _ifThenService.Source = new Uri("/view/project/device/ifthen/listifthenpage.xaml", UriKind.Relative);
        }

        private void Remove(object obj)
        {
            if (!(obj is ZwaveDevice device))
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

                SelectedIfThenModel.ZwaveDevicesThen[0].IsHiddenDelay = true;

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

                SelectedIfThenModel.ZwaveDevicesElse[0].IsHiddenDelay = true;
            }
        }

        private ZwaveDevice _selectedZwaveDevice;

        private bool _isTest = true;

        public ZwaveDevice SelectedZwaveDevice
        {
            get { return _selectedZwaveDevice; }
            set => Set(ref _selectedZwaveDevice, value);
        }

        public bool IsTest
        {
            get => _isTest;
            set => Set(ref _isTest, value);
        }

        public string LastParseObjectId { get; private set; }

        private async void Reset(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Properties.Resources.Reset,
                                     message: Properties.Resources.Reseting_Device,
                                     cancel: async () => await CloseDialog().ConfigureAwait(true));

                await _ifThenRepository.ResetDevice(SelectedProjectModel).ConfigureAwait(true);

                await CloseDialog().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await ShowError(ex).ConfigureAwait(true);
            }
        }

        private async void Create(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Properties.Resources.Wait,
                                     message: Properties.Resources.Creating_If_Then);

                await _ifThenRepository.CreateIfThen(SelectedProjectModel, SelectedIfThenModel).ConfigureAwait(true);

                await _ifThenRepository.Update(SelectedProjectModel, SelectedIfThenModel).ConfigureAwait(true);

                await CloseDialog().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await ShowError(ex).ConfigureAwait(true);
            }
        }

        #region DragDrop

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                return;
            }
            if (dropInfo.Data is ScheduleViewModel && dropInfo.TargetCollection != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
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

        public async void Drop(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                throw new ArgumentNullException(nameof(dropInfo));
            }
            if (dropInfo.TargetCollection is null)
            {
                return;
            }

            if (dropInfo.Data is ScheduleViewModel schedule)
            {
                using ZwaveDevice zwaveDevice = new ZwaveDevice
                {
                    IfthenType = IfthenType.Schedule,
                    Name = Properties.Resources.IR,
                    SelectedIndexDateType = schedule.SelectedIndexDateType,
                    SelectedDateType = (DateTypeEnum)schedule.SelectedIndexDateType,
                    SelectedIndexDaysOfWeek = schedule.SelectedIndexDaysOfWeek,
                    SelectedDaysOfWeek = (DaysOfWeek)schedule.SelectedIndexDaysOfWeek,
                    SelectedIndexOperatorsType = schedule.SelectedIndexOperatorsType,
                    SelectedOperatorsTypeSchedule = (OperatorsTypeSchedule)schedule.SelectedIndexOperatorsType,
                    TimePickerValue = schedule.TimePickerValue,
                    ValueSchedule = schedule.ValueSchedule,
                };

                if (!((ObservableCollection<ZwaveDevice>)dropInfo.TargetCollection is ObservableCollection<ZwaveDevice> list))
                {
                    return;
                }

                list.Add(zwaveDevice);

                if (SelectedIfThenModel.ZwaveDevicesIf.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesIf[0].IsHiddenLogicGateIfThen = true;
                    SelectedIfThenModel.ZwaveDevicesIf[0].SelectedIndexLogicGateIfThen = 2;
                }

                if (SelectedIfThenModel.ZwaveDevicesThen.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesThen[0].IsHiddenDelay = true;
                    SelectedIfThenModel.ZwaveDevicesThen[0].DelayIfThen = 0;
                }

                if (SelectedIfThenModel.ZwaveDevicesElse.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesElse[0].IsHiddenDelay = true;
                    SelectedIfThenModel.ZwaveDevicesElse[0].DelayIfThen = 0;
                }

                return;
            }

            if (dropInfo.Data is GatewayModel gateway)
            {
                if (!gateway.IsSuportThenElse && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesThen")
                {
                    return;
                }

                if (!gateway.IsSuportThenElse && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesElse")
                {
                    return;
                }

                if (!((ObservableCollection<ZwaveDevice>)dropInfo.TargetCollection is ObservableCollection<ZwaveDevice> list))
                {
                    return;
                }

                ZwaveDevice zwaveDevice = new ZwaveDevice();

                switch (gateway.SelectedTabIndexIfThen)
                {
                    case 0:

                        if (gateway.SelectedIndexIR < 0)
                        {
                            OpenCustomMessageBox(header: Properties.Resources.Error,
                                                 message: Properties.Resources.Select_a_combobox_item,
                                                 cancel: async () => await CloseDialog().ConfigureAwait(true));

                            return;
                        }

                        zwaveDevice.IfthenType = IfthenType.IR;
                        zwaveDevice.Name = Properties.Resources.IR;
                        zwaveDevice.SelectedIndexIR = gateway.SelectedIndexIR;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionMemoryId = gateway.GatewayFunctionMemoryId;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    case 1:

                        if (gateway.SelectedIndexRadio433 < 0)
                        {
                            OpenCustomMessageBox(header: Properties.Resources.Error,
                                                 message: Properties.Resources.Select_a_combobox_item,
                                                 cancel: async () => await CloseDialog().ConfigureAwait(true));
                            return;
                        }

                        zwaveDevice.IfthenType = IfthenType.Radio433;
                        zwaveDevice.Name = Properties.Resources.Radio_433MHz;
                        zwaveDevice.SelectedIndexRadio433 = gateway.SelectedIndexRadio433;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionMemoryId = gateway.GatewayFunctionMemoryId;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    case 2:

                        if (gateway.SelectedIndexRTS < 0)
                        {
                            OpenCustomMessageBox(header: Properties.Resources.Error,
                                                 message: Properties.Resources.Select_a_combobox_item,
                                                 cancel: async () => await CloseDialog().ConfigureAwait(true));
                            return;
                        }

                        zwaveDevice.IfthenType = IfthenType.RTS;
                        zwaveDevice.Name = Properties.Resources.RTS;
                        zwaveDevice.SelectedIndexRTS = gateway.SelectedIndexRTS;
                        zwaveDevice.SelectedIndexActionRTSSomfy = gateway.SelectedIndexActionRTSSomfy;
                        zwaveDevice.ActionsRTSSomfy = (ActionsRTSSomfy)gateway.SelectedIndexActionRTSSomfy + 1;
                        zwaveDevice.SelectedTabIndexIfThen = gateway.SelectedTabIndexIfThen;
                        zwaveDevice.GatewayFunctionName = gateway.GatewayFunctionName;
                        zwaveDevice.GatewayFunctionMemoryId = gateway.GatewayFunctionMemoryId;
                        zwaveDevice.GatewayFunctionUID = gateway.UID;
                        zwaveDevice.GatewayName = gateway.Name;
                        break;

                    default:
                        return;
                }

                zwaveDevice.IsHiddenLogicGateIfThen = false;
                zwaveDevice.IsHiddenDelay = false;

                list.Add(zwaveDevice);

                if (SelectedIfThenModel.ZwaveDevicesIf.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesIf[0].IsHiddenLogicGateIfThen = true;
                    SelectedIfThenModel.ZwaveDevicesIf[0].SelectedIndexLogicGateIfThen = 2;
                }

                if (SelectedIfThenModel.ZwaveDevicesThen.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesThen[0].IsHiddenDelay = true;
                    SelectedIfThenModel.ZwaveDevicesThen[0].DelayIfThen = 0;
                }

                if (SelectedIfThenModel.ZwaveDevicesElse.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesElse[0].IsHiddenDelay = true;
                    SelectedIfThenModel.ZwaveDevicesElse[0].DelayIfThen = 0;
                }

                return;
            }
            else if (dropInfo.Data is ZwaveDevice device)
            {
                if (!device.IsSuportIf && ((dynamic)dropInfo.VisualTarget).Name == "ZwaveDevicesIf")
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

                if (!((ObservableCollection<ZwaveDevice>)dropInfo.TargetCollection is ObservableCollection<ZwaveDevice> list))
                {
                    return;
                }

                device.IsHiddenLogicGateIfThen = false;
                device.IsHiddenDelay = false;

                list.Add(JsonConvert.DeserializeObject<ZwaveDevice>(JsonConvert.SerializeObject(device)));

                if (SelectedIfThenModel.ZwaveDevicesIf.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesIf[0].IsHiddenLogicGateIfThen = true;
                    SelectedIfThenModel.ZwaveDevicesIf[0].SelectedIndexLogicGateIfThen = 2;
                }

                if (SelectedIfThenModel.ZwaveDevicesThen.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesThen[0].IsHiddenDelay = true;
                    SelectedIfThenModel.ZwaveDevicesThen[0].DelayIfThen = 0;
                }

                if (SelectedIfThenModel.ZwaveDevicesElse.Count >= 1)
                {
                    SelectedIfThenModel.ZwaveDevicesElse[0].IsHiddenDelay = true;
                    SelectedIfThenModel.ZwaveDevicesElse[0].DelayIfThen = 0;
                }
            }
        }

        #endregion DragDrop

        public async void Test(object @object)
        {
            try
            {
                if (@object is null)
                {
                    throw new ArgumentNullException(nameof(@object));
                }

                if (@object is GatewayModel gateway)
                {
                    await _gatewayRepository.Test(gateway).ConfigureAwait(true);
                    return;
                }

                if (!IsTest || _gatewayService.IsSendingToGateway)
                {
                    return;
                }
                if (@object is ZwaveDevice zwaveDevice)
                {
                    await _zwaveRepository.Test(SelectedProjectModel, zwaveDevice).ConfigureAwait(true);
                    return;
                }
            }
            catch (Exception ex)
            {
                await ShowError(ex).ConfigureAwait(true);
            }
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            SelectedGateway = _projectService.SelectedProject.SelectedGateway;

            SelectedIfThenModel = _projectService.SelectedProject.SelectedIfThen;

            IsTest = false;

            if (!(obj is Page view))
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            GetIfThen(SelectedIfThenModel);
        }

        private async void GetIfThen(IfThenModel selectedIfThenModel)
        {
            try
            {
                if (!SelectedProjectModel.SelectedGateway.ZwaveDevices.Any())
                {
                    await _gatewayRepository.GetZwaveDevices(SelectedProjectModel).ConfigureAwait(true);
                }

                if (selectedIfThenModel.ParseObject != null)
                {
                    await _ifThenRepository.GetIfThen(selectedIfThenModel).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                await ShowError(ex).ConfigureAwait(true);
            }
        }
    }
}