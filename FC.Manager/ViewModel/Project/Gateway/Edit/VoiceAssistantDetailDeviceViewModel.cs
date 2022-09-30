using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain.Model.FCC;
using FC.Domain.Model.FlexCloudClone;
using FC.Domain.Model.Project;
using FC.Domain.Model.User;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Domain.Util;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class VoiceAssistantDetailDeviceViewModel : ProjectViewModelBase, IDropTarget
    {
        private bool _isGroupBy = true;
        private readonly INetworkService _internetService;
        private readonly IVoiceAssistantRepository _voiceAssistantRepository;

        public VoiceAssistantDetailDeviceViewModel(IFrameNavigationService navigationService,
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
                                 IVoiceAssistantRepository voiceAssistantRepository,
                                 INetworkService internetService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _internetService = internetService;

            _voiceAssistantRepository = voiceAssistantRepository;

            RFSGateway = new ObservableCollection<RadioModel>();

            LoadedCommand = new RelayCommand<object>(Loaded);

            base.UpdateCommand = new RelayCommand<object>(Update);

            RemoveUserCommand = new RelayCommand<object>(RemoveUser);

            TestCommand = new RelayCommand<object>(Test);

            GetAllCommandsGatewayCommand = new RelayCommand<object>(GetAllCommandsGateway);

            RemoveCommand = new RelayCommand<object>(Remove);

            IsAssistantCommand = new RelayCommand<object>(IsAssistant);

            ReloadCommand = new RelayCommand<object>(Reload);

            RemoveUserCommand = new RelayCommand<object>(RemoveUser);

            CopyGatewayCommandsToCloudCommand = new RelayCommand<object>(CopyGatewayCommandsToCloud);

            DeleteAllCommandsFromCloudCommand = new RelayCommand<object>(DeleteAllCommandsFromCloud);

            CopyAllUserToCloudCommand = new RelayCommand<object>(CopyAllUserToCloud);

            RemoveAllUsersCommand = new RelayCommand<object>(RemoveAllUsers);

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        public ICommand CopyAllUserToCloudCommand { get; set; }
        public ICommand CopyGatewayCommandsToCloudCommand { get; set; }
        public ICommand DeleteAllCommandsFromCloudCommand { get; set; }
        public ICommand GetAllCommand { get; set; }
        public ICommand GetAllCommandsGatewayCommand { get; set; }
        public ICommand IsAmazonAssistantCommand { get; set; }
        public ICommand IsAssistantCommand { get; set; }

        public bool IsGroupBy
        {
            get => _isGroupBy;
            set
            {
                if (Equals(_isGroupBy, value))
                {
                    return;
                }
                _ = SetProperty(ref _isGroupBy, value);
                SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.GroupBy(value);
            }
        }

        public ICommand RemoveAllCloudCommandListCommand { get; set; }
        public ICommand RemoveAllUsersCommand { get; set; }
        public ICommand RemoveAllUsersVoiceAssistantListCommand { get; set; }
        public ObservableCollection<RadioModel> RFSGateway { get; set; }
        public ICommand TestCommand { get; set; }

        private async void CopyAllUserToCloud(object obj)
        {
            try
            {
                await _voiceAssistantRepository.CopyAllUserToCloud(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void CopyGatewayCommandsToCloud(object obj)
        {
            if (!SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.Commands.Any())
            {
                return;
            }

            foreach (VoiceAssistantCommandModel command in SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.Commands)
            {
                if (SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.FirstOrDefault(x => x.MemoryId == command.MemoryId &&
                                                                                             x.Name == command.Name &&
                                                                                             x.CommandTypeVoiceAssistant == command.CommandTypeVoiceAssistant &&
                                                                                             x.Type == command.Type) != null)
                {
                    continue;
                }

                using VoiceAssistantCommandModel temp = new()
                {
                    MemoryId = command.MemoryId,
                    Name = command.Name,
                    Type = command.Type,
                    CommandTypeVoiceAssistant = command.CommandTypeVoiceAssistant
                };

                SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.Add(temp);
            }

            if (await UpdateCommand())
            {
                return;
            }
        }

        private async void DeleteAllCommandsFromCloud(object obj)
        {
            try
            {
                await _voiceAssistantRepository.DeleteAllCommandsFromCloud(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void GatewayServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LastCommandSend))
            {
                LastCommandSend = ((GatewayService)sender).LastCommandSend;
            }
            if (e.PropertyName == nameof(IsSendingToGateway))
            {
                IsSendingToGateway = ((GatewayService)sender).IsSendingToGateway;
            }
        }

        private async void GetAll()
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _voiceAssistantRepository.GetRemoteAccessStandaloneCommand(SelectedProjectModel.SelectedGateway); ;

                GetAllCommandsGateway();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetAllCommandsGateway(object obj = null)
        {
            SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.Commands.Clear();

            if (!SelectedProjectModel.SelectedGateway.IRsGateway.Any())
            {
                await GetAllIR();
            }

            if (!SelectedProjectModel.SelectedGateway.Radios433Gateway.Any())
            {
                await GetAllRadio();
            }

            if (!SelectedProjectModel.SelectedGateway.RadiosRTSGateway.Any())
            {
                await GetAllRTS();
            }

            if (!SelectedProjectModel.SelectedGateway.IpCommands.Any())
            {
                await GetAllIPCommand();
            }

            SelectedProjectModel.SelectedGateway.ParseToVoiceCommand();
        }

        private async Task GetAllIPCommand()
        {
            try
            {
                await _ipCommandRepository.GetAll(SelectedProjectModel.SelectedGateway);
            }
            catch (Exception)
            {
            }
        }

        private async Task GetAllIR()
        {
            try
            {
                _ = await _irRepository.GetAll(SelectedProjectModel.SelectedGateway);
            }
            catch (Exception)
            {
            }
        }

        private async Task GetAllRadio()
        {
            try
            {
                await _rfRepository.GetAll(SelectedProjectModel.SelectedGateway, TypeRF.R433);
            }
            catch (Exception)
            {
            }
        }

        private async Task GetAllRTS()
        {
            try
            {
                await _rfRepository.GetAll(SelectedProjectModel.SelectedGateway, TypeRF.RTS);
            }
            catch (Exception)
            {
            }
        }

        private async void IsAssistant(object obj)
        {
            try
            {
                if (obj is not ParseUserCustom user)
                {
                    return;
                }
                await _voiceAssistantRepository.UpdateUser(user); ;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            if (!(bool)obj)
            {
                return;
            }

            if (SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.Commands.Any())
            {
                return;
            }

            GetAll();

            SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.GroupBy(IsGroupBy);
        }

        private void Reload(object obj)
        {
            GetAll();
        }

        private async void Remove(object obj)
        {
            try
            {
                if (obj is not VoiceAssistantCommandModel voiceAssistantCommand)
                {
                    return;
                }

                await _voiceAssistantRepository.Remove(SelectedProjectModel.SelectedGateway, voiceAssistantCommand);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void RemoveAllUsers(object obj)
        {
            try
            {
                await _voiceAssistantRepository.RemoveAllUsers(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void RemoveUser(object obj)
        {
            if (obj is not ParseUserCustom custom)
            {
                return;
            }
            if (await _voiceAssistantRepository.RemoveUser(custom))
            {
                _ = SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.Users.Remove(custom);
            }
        }

        private async void Test(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                if (obj is not VoiceAssistantCommandModel voiceAssistantCommand)
                {
                    return;
                }

                using ReplaySubject<string> replay = new();

                switch (voiceAssistantCommand.CommandTypeVoiceAssistant)
                {
                    case CommandTypeVoiceAssistant.IR:
                        await _irRepository.PlayMemory(SelectedProjectModel.SelectedGateway, voiceAssistantCommand.MemoryId);
                        break;

                    case CommandTypeVoiceAssistant.Radio433:
                        await _rfRepository.PlayMemory(SelectedProjectModel.SelectedGateway, voiceAssistantCommand.MemoryId);
                        break;

                    case CommandTypeVoiceAssistant.IPCommand:
                        await _ipCommandRepository.PlayMemory(SelectedProjectModel.SelectedGateway, voiceAssistantCommand.MemoryId);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Update(object obj)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task<bool> UpdateUsers()
        {
            try
            {
                return await _voiceAssistantRepository.UpdateUsers(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        #region DragDrop

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                return;
            }

            if (dropInfo.Data is ParseUserCustom user && dropInfo.TargetCollection is IList<ParseUserCustom> list)
            {
                if (list.FirstOrDefault(x => x.ParseUser.ObjectId == user.ParseUser.ObjectId) != null)
                {
                    return;
                }

                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

                dropInfo.Effects = DragDropEffects.Copy;

                return;
            }

            if (dropInfo.Data is VoiceAssistantCommandModel model)
            {
                if (SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.FirstOrDefault(x => x.MemoryId == model.MemoryId &&
                                                                                                  x.Name == model.Name &&
                                                                                                  x.Type == model.Type) != null)
                {
                    return;
                }

                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;

                dropInfo.Effects = DragDropEffects.Copy;

                return;
            }
        }

        public async void Drop(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                return;
            }

            if (dropInfo.Data is ParseUserCustom user && dropInfo.TargetCollection is IList<ParseUserCustom> list)
            {
                if (list.FirstOrDefault(x => x.ParseUser.ObjectId == user.ParseUser.ObjectId) != null)
                {
                    return;
                }

                list.Insert(dropInfo.InsertIndex, user);

                if (await UpdateUsers())
                {
                    return;
                }

                _ = list.Remove(user);
                return;
            }

            if (dropInfo.Data is VoiceAssistantCommandModel command)
            {
                if (SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.FirstOrDefault(x => x.MemoryId == command.MemoryId &&
                                                                           x.Name == command.Name &&
                                                                           x.Type == command.Type) != null)
                {
                    return;
                }

                SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.Add(command);

                if (!await _internetService.IsInternet())
                {
                    _ = SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.Remove(command);
                    return;
                }

                if (await UpdateCommand())
                {
                    return;
                }

                _ = SelectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.Remove(command);
            }
        }

        private new async Task<bool> UpdateCommand()
        {
            try
            {
                return await _voiceAssistantRepository.UpdateCommand(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
                return false;
            }
        }

        #endregion DragDrop
    }
}