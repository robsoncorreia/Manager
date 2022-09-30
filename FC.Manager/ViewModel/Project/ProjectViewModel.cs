using CommunityToolkit.Mvvm.Input;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Windows.Controls;

namespace FC.Manager.ViewModel.Project
{
    public class ProjectViewModel : ProjectViewModelBase
    {
        public Uri SourceFrame
        {
            get => _sourceFrame;
            set => SetProperty(ref _sourceFrame, value);
        }

        private void Loaded(object obj = null)
        {
            if (obj is not Frame frame)
            {
                return;
            }

            _navigationService.CustomFrame = frame;
            _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
        }

        private Uri _sourceFrame;

        public ProjectViewModel(IFrameNavigationService navigationService,
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
        }
    }
}