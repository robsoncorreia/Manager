using ConfigurationFlexCloudHubBlaster.Repository;
using ConfigurationFlexCloudHubBlaster.Repository.Gateway;
using ConfigurationFlexCloudHubBlaster.Repository.Zwave;
using ConfigurationFlexCloudHubBlaster.Service;
using System;

namespace ConfigurationFlexCloudHubBlaster.ViewModel.Project.Device.IfThen
{
    public class DashboardIfThenViewModel : ProjectViewModelBase
    {
        private Uri _source;
        private IIfThenService _ifThenService;

        public Uri Source
        {
            get { return _source; }
            set => Set(ref _source, value);
        }

        public DashboardIfThenViewModel(IFrameNavigationService navigationService, IProjectService projectService,
                                        IUdpRepository udpRepository, ITCPRepository tcpRepository,
                                        IIRRepository irRepository, IUserRepository userService,
                                        ILicenseRepository licenseRepository, IProjectRepository projectRepository,
                                        ILocalDBRepository localDBRepository, ILogRepository logRepository,
                                        IUserRepository userRepository, ISerialRepository serialRepository,
                                        ICommandRepository commandRepository, IIPCommandRepository ipCommandRepository,
                                        IGatewayService gatewayService, IConfigurationRepository configurationRepository,
                                        ITaskService taskService, IParseService parseService, IRFRepository rfRepository,
                                        IZwaveRepository zwaveRepository, IRelayTestRepository relayTestRepository,
                                        IIfThenService ifThenService,
                                        IGatewayRepository gatewayRepository, IAmbienceRepository ambienceRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, licenseRepository, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, ambienceRepository)
        {
            _ifThenService = ifThenService;

            _ifThenService.PropertyChanged += IfThenServicePropertyChanged; ;

            _ifThenService.Source = new Uri("/view/project/device/ifthen/listifthenpage.xaml", UriKind.Relative);
        }

        private void IfThenServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!(sender is IfThenService service))
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