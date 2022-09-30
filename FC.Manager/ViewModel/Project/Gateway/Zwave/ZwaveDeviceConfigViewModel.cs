using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model.Device;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using GongSolutions.Wpf.DragDrop;
using Google.Apis.Drive.v3;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Gateway.Zwave
{
    public class ZwaveDeviceConfigViewModel : ProjectViewModelBase, IDropTarget
    {
        private int groupId;
        private readonly INetworkService _networkService;
        private readonly IGoogleApiService _googleApiService;
        private readonly IGoogleDriveService _googleDriveService;

        public ZwaveDeviceConfigViewModel(IFrameNavigationService navigationService,
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
                                 IGoogleApiService googleApiService,
                                 IGoogleDriveService googleDriveService,
                                 INetworkService networkService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _networkService = networkService;

            _googleApiService = googleApiService;

            _googleDriveService = googleDriveService;

            LoadedCommand = new RelayCommand<object>(Loaded);

            UnloadedCommand = new RelayCommand<object>(Unloaded);

            TestCommand = new RelayCommand<object>(Test);

            UpdateCommand = new RelayCommand<object>(Update);

            GetStatesEndpointsCommand = new RelayCommand(async () => await GetStatesEndpoints());

            GetZwaveConfigCommand = new RelayCommand<object>(GetZwaveConfig);

            SetZwaveConfigCommand = new RelayCommand<object>(SetZwaveConfig);

            GetAllAssociationCommand = new RelayCommand<object>(GetAllAssociation);

            DropCommand = new RelayCommand<object>(Drop);

            //CloseWindowCommand = new RelayCommand<IClosable>(CloseWindow);

            RemoveAssociationCommmand = new RelayCommand<object>(RemoveAssociation);

            BulkSetCommand = new RelayCommand<object>(BulkSet);

            GetValuesCommand = new RelayCommand<object>(GetValues);

            GetDocumentationCommand = new RelayCommand<object>(GetDocumentation);

            if (_gatewayService is null)
            {
                return;
            }

            _gatewayService.PropertyChanged += GatewayService_PropertyChanged;

            if (_parseService is null)
            {
                return;
            }

            _parseService.PropertyChanged += ParseServicePropertyChanged;

            _taskService = taskService;
        }

        public override void Unloaded(object obj)
        {
            base.Unloaded(obj);

            SelectedProjectModel.SelectedGateway.SelectedIndexZwaveDevice = -1;

            if (SelectedProjectModel.SelectedGateway.SelectedZwaveDevice is null)
            {
                return;
            }

            SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.SelectedIndexTabControl = 0;
            SelectedProjectModel.SelectedGateway.SelectedZwaveDevice = null;
        }

        private async void GetDocumentation(object obj)
        {
            try
            {
                _parseService.IsSendingToCloud = true;

                if (!await _networkService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         textButtomOk: Domain.Properties.Resources._Close,
                                         ok: async () => await CloseDialog());

                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Downloading,
                                     isProgressBar: true);

                DriveService service = _googleApiService.LoginServiceAccountCredential();

                byte[] bytes = await _googleDriveService.GetFiles(driveService: service, path: "Documents", fileName: SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.DefaultName);

                if (bytes is null)
                {
                    await CloseDialog();
                }

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = $"{Domain.Properties.Resources.PDF_File} (*.pdf)|*.pdf",
                    FileName = $"{SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.DefaultName}-{Domain.Properties.Resources.Documentation}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, bytes);
                }

                if ($"{SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.DefaultName}-{Domain.Properties.Resources.Documentation}" == saveFileDialog.FileName)
                {
                    await CloseDialog();
                    return;
                }

                action = () => OpenFile(saveFileDialog);

                ActionContentSnackbar = Domain.Properties.Resources.Open;

                ContentSnackbar = Domain.Properties.Resources.File_saved;

                count.Reset();

                count.SetTime(4000);

                count.Start();

                count.CountDownFinished = () =>
                {
                    IsActiveSnackbar = false;
                    ActionContentSnackbar = null;
                };

                IsActiveSnackbar = true;

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public ObservableCollection<string> Commands { get; set; }

        public ICommand BulkSetCommand { get; set; }

        public ICommand CloseWindowCommand { get; set; }

        public ICommand DropCommand { get; set; }

        public ICommand GetAllAssociationCommand { get; set; }

        public ICommand GetStatesEndpointsCommand { get; set; }

        public ICommand GetValuesCommand { get; set; }

        public ICommand GetZwaveConfigCommand { get; set; }

        public ICommand RemoveAssociationCommmand { get; set; }

        public ICommand SetZwaveConfigCommand { get; set; }

        public ICommand TestCommand { get; set; }

        public ICommand GetDocumentationCommand { get; set; }

        private void ParseServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IsSendingToCloud)))
            {
                IsSendingToCloud = ((ParseService)sender).IsSendingToCloud;
            }
        }

        //private void AddGatewayZwaveDevices()
        //{
        //    ZwaveDevice gateway = SelectedProjectModel.SelectedGateway.ZwaveDevices.FirstOrDefault(x => x.ZWaveComponents == ZWaveComponents.Controller);

        //    SelectedProjectModel.SelectedGateway.ZwaveDevices.Remove(gateway);

        //    gateway ??= new ZwaveDevice
        //    {
        //        ZWaveComponents = ZWaveComponents.Controller,
        //        Name = SelectedProjectModel.SelectedGateway.Name,
        //        ModuleId = 1
        //    };

        //    SelectedProjectModel.SelectedGateway.ZwaveDevices.Add(gateway);

        //    foreach (ZwaveDevice zwaveDevice in SelectedProjectModel.SelectedGateway.ZwaveDevices)
        //    {
        //        if (zwaveDevice.ModuleId == SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.ModuleId)
        //        {
        //            zwaveDevice.IsEnabledAssociation = false;
        //            continue;
        //        }
        //        zwaveDevice.IsEnabledAssociation = true;
        //    }
        //}

        private async void BulkSet(object @object)
        {
            try
            {
                if (@object is not Endpoint endpoint)
                {
                    return;
                }
                await _zwaveRepository.BulkSet(SelectedProjectModel.SelectedGateway, endpoint);
                await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        //private void CloseWindow(IClosable window)
        //{
        //    if (window != null)
        //    {
        //        window.Close();
        //    }
        //}

        private void Drop(object obj)
        {
            if (obj is null)
            {
                return;
            }

            groupId = (int)obj;
        }

        private void GatewayService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(IsSendingToGateway)))
            {
                IsSendingToGateway = ((GatewayService)sender).IsSendingToGateway;
            }
        }

        private async void GetAllAssociation(object obj)
        {
            try
            {
                await _zwaveRepository.GetAllAssociation(selectedProject: SelectedProjectModel,
                                                         selectedDevice: SelectedProjectModel.SelectedGateway.SelectedZwaveDevice);

                await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task GetStatesEndpoints()
        {
            try
            {
                if (await _zwaveRepository.GetStatesEndpoints(SelectedProjectModel))
                {
                    await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);
                    await CloseDialog();
                    return;
                }
                if (IsCanceled)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: $"{Domain.Properties.Resources.Task_canceled}",
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: Domain.Properties.Resources.Make_sure_the_device_is_turned_on,
                                     textButtomOk: Domain.Properties.Resources.Ok,
                                     ok: async () => await CloseDialog());
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetValues(object obj)
        {
            try
            {
                bool test = await _zwaveRepository.GetValues(SelectedProjectModel);

                await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetZwaveConfig(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Getting,
                                     message: Domain.Properties.Resources.Getting_information_from_the_gateway,
                                     cancel: async () => await CloseDialog());

                await _zwaveRepository.GetZwaveConfig(SelectedProjectModel);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            SortTabs(obj);

            GetTabs(obj);

            //AddGatewayZwaveDevices();

            //ParseAssociations();
        }

        //private void ParseAssociations()
        //{
        //    if (string.IsNullOrEmpty(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.AssociationsSerialize))
        //    {
        //        return;
        //    }

        //    if (SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Associations.Any())
        //    {
        //        SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Associations.Clear();
        //    }

        //    foreach (AssociationGroup association in JsonConvert.DeserializeObject<IList<AssociationGroup>>(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.AssociationsSerialize))
        //    {
        //        for (int i = 0; i < association.Data.Count; i++)
        //        {
        //            ZwaveDevice zwaveDevice = SelectedProjectModel.SelectedGateway.ZwaveDevices.FirstOrDefault(x => x.ModuleId == association.Data[i]);

        //            zwaveDevice ??= new ZwaveDevice
        //            {
        //                GroupId = association.Data[i],
        //                Name = Domain.Properties.Resources.Device_not_found
        //            };

        //            association.ZwaveDevices.Add(zwaveDevice);
        //        }

        //        SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Associations.Add(association);
        //    }
        //}

        private async void RemoveAssociation(object obj)
        {
            try
            {
                if (obj is null)
                {
                    return;
                }
                if (groupId < 1)
                {
                    return;
                }
                if (obj is not ZwaveDevice device)
                {
                    return;
                }

                device.GroupId = groupId;

                using ReplaySubject<string> replay = new();

                await _zwaveRepository.RemoveAssociation(SelectedProjectModel, device);

                await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void SetZwaveConfig(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Set,
                                     message: Domain.Properties.Resources.Changing_zwave_device_configuration,
                                     cancel: async () => await CloseDialog());

                _ = await _zwaveRepository.SetZwaveConfig(SelectedProjectModel);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private readonly CountDownTimer count = new();

        private async void Test(object @object)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());

                if (@object is Endpoint endpoint)
                {
                    if (!await _zwaveRepository.SetStateEndpoint(SelectedProjectModel.SelectedGateway, endpoint))
                    {
                        if (IsCanceled)
                        {
                            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                                 message: $"{Domain.Properties.Resources.Task_canceled}",
                                                 textButtonCancel: Domain.Properties.Resources.Close,
                                                 cancel: async () => await CloseDialog());
                            return;
                        }
                        OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                             message: string.Format(Domain.Properties.Resources.Device_did_not_respond,
                                                                     SelectedProjectModel.SelectedGateway.Name,
                                                                     SelectedProjectModel.SelectedGateway.LocalIP,
                                                                     SelectedProjectModel.SelectedGateway.LocalPortUDP),
                                             custom:  ()=>  Test(@object),
                                             textButtonCustom: Domain.Properties.Resources.Try_again,
                                             cancel: async () => await CloseDialog(),
                                             textButtonCancel: Domain.Properties.Resources.Close) ;
                        return;
                    }
                    await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);
                }
                if (@object is ZwaveDevice zwaveDevice)
                {
                    if (!await _zwaveRepository.Test(SelectedProjectModel, zwaveDevice))
                    {
                        if (IsCanceled)
                        {
                            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                                 message: $"{Domain.Properties.Resources.Task_canceled}",
                                                 textButtonCancel: Domain.Properties.Resources.Close,
                                                 cancel: async () => await CloseDialog());
                            return;
                        }
                        OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                             message: Domain.Properties.Resources.Device_did_not_respond,
                                             ok: async () => await CloseDialog());
                    }
                    await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);
                }
                if (@object is Scene scene)
                {
                    if (!await _zwaveRepository.SetScene(SelectedProjectModel, scene, isSendingToGateway: true))
                    {
                        if (IsCanceled)
                        {
                            OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                                 message: $"{Domain.Properties.Resources.Task_canceled}",
                                                 textButtonCancel: Domain.Properties.Resources.Close,
                                                 cancel: async () => await CloseDialog());
                            return;
                        }
                        OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                             message: Domain.Properties.Resources.Device_did_not_respond,
                                             ok: async () => await CloseDialog());

                        return;
                    }
                    count.Stop();

                    count.SetTime(1000);

                    count.Start();

                    await Task.Run(() => { while (count.IsRunnign) { } });

                    _ = await _zwaveRepository.GetStatesEndpoints(selectedProject: SelectedProjectModel,
                                                              isGetScene: false);

                    await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);
                }

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void Update(object obj = null)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Update,
                                     message: Domain.Properties.Resources.Updating_Z_Wave_device_information);

                await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);

                await CloseDialog(0);
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

            if (dropInfo.Data is TabItem && dropInfo.TargetCollection != null)
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

            if (dropInfo.Data is TabItem item)
            {
                ((ItemCollection)dropInfo.TargetCollection).RemoveAt(dropInfo.DragInfo.SourceIndex);

                if (dropInfo.InsertIndex == 0)
                {
                    ((ItemCollection)dropInfo.TargetCollection).Insert(0, item);
                    SelectedIndexTabControl = 0;
                    SaveTabOrder((ItemCollection)dropInfo.TargetCollection);
                    return;
                }

                ((ItemCollection)dropInfo.TargetCollection).Insert(dropInfo.InsertIndex - 1, item);
                SelectedIndexTabControl = dropInfo.InsertIndex - 1;
                SaveTabOrder((ItemCollection)dropInfo.TargetCollection);
                return;
            }

            if (dropInfo.Data is ZwaveDevice device)
            {
                if (device.ModuleId == 0)
                {
                    return;
                }

                if ((ObservableCollection<ZwaveDevice>)dropInfo.TargetCollection is not ObservableCollection<ZwaveDevice> list)
                {
                    return;
                }

                if (list.Contains(device))
                {
                    return;
                }

                if (SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Associations.FirstOrDefault(x => x.GroupId == groupId) is not AssociationGroup associationGroup)
                {
                    return;
                }

                if (list.Count >= associationGroup.MaxRegister)
                {
                    return;
                }

                device.GroupId = groupId;

                if (!await SetAssociation(device))
                {
                    return;
                }

                list.Add(device);

                await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);
            }
        }

        private void GetTabs(object obj)
        {
            if (obj is null)
            {
                return;
            }

            if (SelectedProjectModel.SelectedGateway.SelectedZwaveDevice is null)
            {
                return;
            }

            List<TabItem> tabs = ((TabControl)obj).Items.Cast<TabItem>().Select(x => { x.Visibility = Visibility.Visible; return x; }).ToList();

            if (!SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.IsTestable)
            {
                TabItem tabTest = tabs.FirstOrDefault(x => x.Header.Equals(Domain.Properties.Resources.Test));

                if (tabTest is not null)
                {
                    tabTest.Visibility = Visibility.Collapsed;
                }
            }

            SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.SelectedIndexTabControl = tabs.IndexOf(tabs.FirstOrDefault(x => x.Visibility == Visibility.Visible));
        }

        private void SaveTabOrder(ItemCollection targetCollection)
        {
            _configurationRepository.ConfigurationApp.OrderZwaveDeviceConfig = targetCollection.Cast<TabItem>().Select(x => x.Name);
            _ = _configurationRepository.Update();
        }

        private async Task<bool> SetAssociation(ZwaveDevice zwaveDevice)
        {
            try
            {
                return zwaveDevice is not null && await _zwaveRepository.SetAssociation(zwaveDevice, SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        private void SortTabs(object obj)
        {
            if (obj is null)
            {
                return;
            }

            List<TabItem> tabs = ((TabControl)obj).Items.Cast<TabItem>().ToList();

            ((TabControl)obj).Items.Clear();

            if (_configurationRepository.ConfigurationApp.OrderZwaveDeviceConfig is null)
            {
                foreach (TabItem item in tabs)
                {
                    _ = ((TabControl)obj).Items.Add(item);
                }

                SaveTabOrder(((TabControl)obj).Items);

                return;
            }

            if (_configurationRepository.ConfigurationApp.OrderZwaveDeviceConfig.Count() != tabs.Count)
            {
                foreach (TabItem item in tabs)
                {
                    _ = ((TabControl)obj).Items.Add(item);
                }

                SaveTabOrder(((TabControl)obj).Items);

                return;
            }

            foreach (string name in _configurationRepository.ConfigurationApp.OrderZwaveDeviceConfig)
            {
                TabItem tab = tabs.FirstOrDefault(x => x.Name == name);

                if (tab != null)
                {
                    _ = ((TabControl)obj).Items.Add(tab);
                }
                else
                {
                    foreach (TabItem item in tabs)
                    {
                        _ = ((TabControl)obj).Items.Add(item);
                    }
                    SaveTabOrder(((TabControl)obj).Items);
                    break;
                }
            }
        }

        #endregion DragDrop
    }
}