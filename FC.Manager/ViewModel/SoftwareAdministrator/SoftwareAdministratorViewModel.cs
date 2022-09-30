using CommunityToolkit.Mvvm.Input;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.View.SoftwareAdministrator.Ambience;
using FC.Manager.ViewModel.Project;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FC.Manager.ViewModel.SoftwareAdministrator
{
    public class SoftwareAdministratorViewModel : ProjectViewModelBase
    {
        private readonly ISoftwareAdministratorService _softwareAdministratorService;

        public SoftwareAdministratorViewModel(IFrameNavigationService navigationService,
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
                                              ISoftwareAdministratorService softwareAdministratorService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _softwareAdministratorService = softwareAdministratorService;
            AmbienceImages = new ObservableCollection<AmbienceImageModel>();
            OpenAddImageAmbienceCommmand = new RelayCommand<object>(OpenAddImageAmbience);
            EditImageAmbienceCommmand = new RelayCommand<object>(EditImageAmbience);
            LoadedCommand = new RelayCommand<object>(Loaded);
        }

        public ObservableCollection<AmbienceImageModel> AmbienceImages { get; set; }
        public ICommand EditImageAmbienceCommmand { get; set; }
        public ICommand OpenAddImageAmbienceCommmand { get; set; }

        private void EditImageAmbience(object obj)
        {
            _softwareAdministratorService.SelectedAmbienceModel = obj as AmbienceImageModel;
            AddAmbienceImageWindow window = new();
            _ = window.ShowDialog();
        }

        private void Loaded(object obj)
        {
        }

        private void OpenAddImageAmbience(object obj)
        {
            _softwareAdministratorService.SelectedAmbienceModel = null;
            AddAmbienceImageWindow window = new();
            _ = window.ShowDialog();
        }
    }
}