using FC.Domain._Util;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;

namespace FC.Manager.ViewModel.Project.Device.IfThen
{
    public class DashboardScheduleViewModel : ProjectViewModelBase
    {
        private readonly IScheduleService _scheduleService;
        private Uri _source;

        public DashboardScheduleViewModel(IFrameNavigationService navigationService,
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
                                 IScheduleService scheduleService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _scheduleService = scheduleService;

            _scheduleService.PropertyChanged += IfThenServicePropertyChanged; ;

            _scheduleService.Source = new Uri(AppConstants.LISTSCHEDULEPAGE, UriKind.Relative);
        }

        public Uri Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        private void IfThenServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is not ScheduleService service)
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