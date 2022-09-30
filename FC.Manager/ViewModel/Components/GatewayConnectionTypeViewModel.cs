using FC.Domain.Model;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.ViewModel.Project;

namespace FC.Manager.ViewModel.Components
{
    public class GatewayConnectionTypeViewModel : ProjectViewModelBase
    {
        private int _SelectedIndexGatewayConnectionType = Domain.Properties.Settings.Default.gatewayConnectionType;

        public GatewayConnectionTypeViewModel(IFrameNavigationService navigationService,
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
        }

        private string _ConnectionTypeHint = GetConnectionTypeHint();

        public string ConnectionTypeHint
        {
            get => _ConnectionTypeHint;
            set => _ = SetProperty(ref _ConnectionTypeHint, value);
        }

        private static readonly int GATEWAYCONNECTIONTYPEDEFAUT = int.Parse(Domain.Properties.Settings.Default.Properties["gatewayConnectionType"].DefaultValue.ToString());

        public int SelectedIndexGatewayConnectionType
        {
            get => _SelectedIndexGatewayConnectionType;
            set
            {
                _ = SetProperty(ref _SelectedIndexGatewayConnectionType, value);
                if (Equals(Domain.Properties.Settings.Default.gatewayConnectionType, value))
                {
                    return;
                }
                _configurationRepository.SetGatewayConnectionType((GatewayConnectionType)SelectedIndexGatewayConnectionType);
                ConnectionTypeHint = GetConnectionTypeHint();
            }
        }

        private static string GetConnectionTypeHint()
        {
            return Domain.Properties.Settings.Default.gatewayConnectionType == GATEWAYCONNECTIONTYPEDEFAUT ?
                                 $"{Domain.Properties.Resources.Connection_type}{Domain.Properties.Resources._Default_}" :
                                 $"{Domain.Properties.Resources.Connection_type}";
        }
    }
}