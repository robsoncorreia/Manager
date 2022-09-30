using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain.Model;
using FC.Domain.Model.Project;
using FC.Domain.Model.User;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class UserDetailDeviceViewModel : ProjectViewModelBase, IDropTarget
    {
        public UserDetailDeviceViewModel(IFrameNavigationService navigationService,
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
            LoadedCommand = new RelayCommand<object>(Loaded);

            UpdateCommand = new RelayCommand<object>(Update);

            ClearAllBlacklistCommand = new RelayCommand<object>(ClearAllBlacklist);

            RemoveUserCommand = new RelayCommand<object>(RemoveUser);

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        public ICommand ClearAllBlacklistCommand { set; get; }

        private void ClearAllBlacklist(object obj)
        {
            if (!SelectedProjectModel.SelectedGateway.BlacklistUsers.Any())
            {
                return;
            }

            SelectedProjectModel.SelectedGateway.BlacklistUsers.Clear();
        }

        private void GetBlacklistUsers(GatewayModel selectedDevice)
        {
            if (selectedDevice.BlacklistUsers.Any())
            {
                selectedDevice.BlacklistUsers.Clear();
            }

            foreach (string objectId in selectedDevice.Blacklist)
            {
                ParseUserCustom user = SelectedProjectModel.Users.FirstOrDefault(x => x.ParseUser.ObjectId == objectId);
                if (user is not null)
                {
                    selectedDevice.BlacklistUsers.Add(user);
                }
            }
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;
            GetBlacklistUsers(SelectedProjectModel.SelectedGateway);
        }

        private void RemoveUser(object obj)
        {
            if (obj is null)
            {
                return;
            }

            if (obj is not object[])
            {
                return;
            }

            if ((string)((object[])obj)[1] == "LbBlacklistUsers")
            {
                _ = SelectedProjectModel.SelectedGateway.BlacklistUsers.Remove((ParseUserCustom)((object[])obj)[0]);
            }
        }

        private async void Update(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Updating,
                                     message: Domain.Properties.Resources.Updating_blacklist_of_device_users,
                                     cancel: async () => await CloseDialog());

                await _gatewayRepository.UpdateBlacklistUsers(SelectedProjectModel.SelectedGateway);

                await CloseDialog(1000);
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

            if (dropInfo.Data is ParseUserCustom)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }

            if (dropInfo.Data is TabItem && dropInfo.TargetCollection != null)
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

            if (dropInfo.Data is ParseUserCustom custom)
            {
                if (((IList<ParseUserCustom>)dropInfo.TargetCollection).FirstOrDefault(x => x.ParseUser.ObjectId == custom.ParseUser.ObjectId) != null)
                {
                    return;
                }

                ((IList<ParseUserCustom>)dropInfo.TargetCollection).Insert(dropInfo.InsertIndex, custom);
                return;
            }
        }

        #endregion DragDrop
    }
}