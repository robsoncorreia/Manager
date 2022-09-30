using ConfigurationFlexCloudHubBlaster.Model.IfThen;
using ConfigurationFlexCloudHubBlaster.Repository;
using ConfigurationFlexCloudHubBlaster.Repository.Gateway;
using ConfigurationFlexCloudHubBlaster.Repository.Zwave;
using ConfigurationFlexCloudHubBlaster.Service;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConfigurationFlexCloudHubBlaster.ViewModel.Project.Device.IfThen
{
    public class ListIfThenViewModel : ProjectViewModelBase
    {
        private IIfThenService _ifThenService;
        private IIfThenRepository _ifThenRepository;

        public ICommand SelectionChangedCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand NewCommand { get; set; }

        public ObservableCollection<IfThenModel> IfThenModels { get; set; }

        public ListIfThenViewModel(IFrameNavigationService navigationService,
                                   IProjectService projectService,
                                   IUdpRepository udpRepository,
                                   ITCPRepository tcpRepository,
                                   IIRRepository irRepository,
                                   IUserRepository userService,
                                   ILicenseRepository licenseRepository,
                                   IProjectRepository projectRepository,
                                   ILocalDBRepository localDBRepository,
                                   ILogRepository logRepository,
                                   IUserRepository userRepository,
                                   ISerialRepository serialRepository,
                                   ICommandRepository commandRepository,
                                   IIPCommandRepository ipCommandRepository,
                                   IGatewayService gatewayService,
                                   IConfigurationRepository configurationRepository,
                                   ITaskService taskService,
                                   IParseService parseService,
                                   IRFRepository rfRepository,
                                   IZwaveRepository zwaveRepository,
                                   IRelayTestRepository relayTestRepository,
                                   IGatewayRepository gatewayRepository,
                                   IIfThenRepository ifThenRepository,
                                   IIfThenService ifThenService,
                                   IAmbienceRepository ambienceRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, licenseRepository, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, ambienceRepository)
        {
            _ifThenService = ifThenService;

            _ifThenRepository = ifThenRepository;

            LoadedCommand = new RelayCommand<object>(Loaded);

            DeleteCommand = new RelayCommand<object>(Delete);

            NewCommand = new RelayCommand<object>(New);

            IfThenModels = new ObservableCollection<IfThenModel>();

            SelectionChangedCommand = new RelayCommand<object>(SelectionChanged);
        }

        private void New(object obj)
        {
            _projectService.SelectedProject.SelectedIfThen = new IfThenModel();
            _ifThenService.Source = new Uri("/view/project/device/ifthen/createifthenpage.xaml", UriKind.Relative);
        }

        private async void Delete(object obj)
        {
            if (!(obj is IfThenModel ifThen))
            {
                return;
            }

            await _ifThenRepository.GetIfThen(ifThen).ConfigureAwait(true);

            bool isDeleted = await _ifThenRepository.DeleteIfThenFromGataway(SelectedProjectModel, ifThen).ConfigureAwait(true);

            if (isDeleted)
            {
                await _ifThenRepository.DeleteIfThenFromCloud(SelectedProjectModel, ifThen).ConfigureAwait(true);
            }

            IfThenModels.Remove(ifThen);
        }

        private void SelectionChanged(object obj)
        {
            if (!(obj is IfThenModel selectedIfThen))
            {
                return;
            }

            _projectService.SelectedProject.SelectedIfThen = selectedIfThen;
            _ifThenService.Source = new Uri("/view/project/device/ifthen/createifthenpage.xaml", UriKind.Relative);
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            SelectedGateway = SelectedProjectModel.SelectedGateway;

            if (!(obj is Page view))
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            GetIfThens();
        }

        private async void GetIfThens()
        {
            try
            {
                await _ifThenRepository.GetIfThens(SelectedProjectModel, IfThenModels).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await ShowError(ex).ConfigureAwait(true);
            }
        }
    }
}