using FC.Domain._Util;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;

namespace FC.Manager.ViewModel.Project.Device.IfThen
{
    public class DashboardIfThenViewModel : ProjectViewModelBase
    {
        private readonly IIfThenService _ifThenService;
        private Uri _source;

        public DashboardIfThenViewModel(IFrameNavigationService navigationService,
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
                                 IIfThenService ifThenService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _ifThenService = ifThenService;

            _ifThenService.PropertyChanged += IfThenServicePropertyChanged; ;

            _ifThenService.Source = new Uri(AppConstants.LISTIFTHENPAGE, UriKind.Relative);
        }

        public Uri Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        private void IfThenServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not IfThenService service)
            {
                return;
            }
            if (e.PropertyName == nameof(Source))
            {
                Source = service.Source;
            }
        }
    }
}