using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;

namespace FC.Updater.ViewModel
{
    public class MainViewModel : FlexViewModelBase
    {
        public MainViewModel(IFrameNavigationService navigationService,
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
                             IGatewayRepository gatewayRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository)
        {
            _navigationService = navigationService;

            LoadedCommand = new RelayCommand<object>((obj) => Loaded(obj));
        }

        public string Version { get; } = $"{Properties.Resources.AppName} {Properties.Resources.AppVersion}";

        private void Loaded(object obj)
        {
            _navigationService.NavigateTo(AppConstants.GATEWAY);
        }
    }
}