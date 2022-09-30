using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain.Model.Device;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Gateway.Zwave
{
    public class ZwaveAssociationViewModel : ProjectViewModelBase, IDropTarget
    {
        private int groupId;

        public ZwaveAssociationViewModel(IFrameNavigationService navigationService,
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
                                         ILoginRepository loginRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            GetAllAssociationCommand = new RelayCommand<object>(GetAllAssociation);

            LoadedCommand = new RelayCommand<object>(Loaded);

            UnloadedCommand = new RelayCommand<object>(Unloaded);

            RemoveAssociationCommmand = new RelayCommand<object>(RemoveAssociation);

            DropCommand = new RelayCommand<object>(Drop);

            ZwaveDevices = new ObservableCollection<ZwaveDevice>();

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        private void Drop(object obj)
        {
            if (obj is null)
            {
                return;
            }

            groupId = (int)obj;
        }

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

                await _zwaveRepository.RemoveAssociation(SelectedProjectModel, device);

                await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public ObservableCollection<ZwaveDevice> ZwaveDevices { get; set; }

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

            GetZwaveDevices();

            GetAllAssociation();
        }

        private async void GetAllAssociation(object obj = null)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Getting_information_from_association_groups,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());

                await _zwaveRepository.GetAssociationNumberGroups(selectedProject: SelectedProjectModel,
                                                                  selectedDevice: SelectedProjectModel.SelectedGateway.SelectedZwaveDevice,
                                                                  isSendingToGateway: true);

                await _zwaveRepository.GetAllAssociation(selectedProject: SelectedProjectModel,
                                                         selectedDevice: SelectedProjectModel.SelectedGateway.SelectedZwaveDevice,
                                                         isSendingToGateway: true);

                _gatewayService.IsSendingToGateway = false;

                if (IsCanceled)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: $"{Domain.Properties.Resources.Task_canceled}",
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                await _zwaveRepository.Update(SelectedProjectModel.SelectedGateway.SelectedZwaveDevice, SelectedProjectModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void GetZwaveDevices()
        {
            ZwaveDevices.Clear();

            foreach (ZwaveDevice zwave in SelectedProjectModel.SelectedGateway.IsPrimary ?
                SelectedProjectModel.SelectedGateway.ZwaveDevices.Where(x => x.ModuleId != SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.ModuleId) :
                SelectedProjectModel.SelectedGateway.SecondaryZwaveDevices.Where(x => x.ModuleId != SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.ModuleId))
            {
                ZwaveDevices.Add(zwave);
            }
        }

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

        public ICommand GetAllAssociationCommand { get; set; }
        public ICommand RemoveAssociationCommmand { get; set; }
        public ICommand DropCommand { get; set; }
    }
}