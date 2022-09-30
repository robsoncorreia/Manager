using CommunityToolkit.Mvvm.Input;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.ViewModel.Project;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Components
{
    public class InfoIfThenViewModel : ProjectViewModelBase
    {
        private bool _IsOpenDialogHostIInfo;

        public InfoIfThenViewModel(IFrameNavigationService navigationService,
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
            SelectedProjectModel = _projectService.SelectedProject;

            OpenDialogHostInfoCommand = new RelayCommand(OpenDialogHostIInfo);
        }

        #region ICommand

        public ICommand OpenDialogHostInfoCommand { get; set; }

        #endregion ICommand

        #region Method

        private void OpenDialogHostIInfo()
        {
            IsOpenDialogHostIInfo = !IsOpenDialogHostIInfo;
        }

        #endregion Method

        #region Properties

        public bool IsOpenDialogHostIInfo
        {
            get => _IsOpenDialogHostIInfo;
            set => SetProperty(ref _IsOpenDialogHostIInfo, value);
        }

        #endregion Properties
    }
}