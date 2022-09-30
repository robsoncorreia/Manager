using CommunityToolkit.Mvvm.Input;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.ViewModel.Project;
using System;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Components
{
    public class DeleteSmartSearchViewModel : ProjectViewModelBase
    {
        public ICommand DeleteCommand { get; set; }

        public DeleteSmartSearchViewModel(IFrameNavigationService navigationService,
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
            DeleteCommand = new RelayCommand<object>(Delete);
            _localDBRepository = localDBRepository;
        }

        private void Delete(object obj)
        {
            try
            {
                _localDBRepository.DeleteSmartSearch();
                OpenCustomMessageBox(header: Domain.Properties.Resources.Deleted,
                                     textButtomOk: Domain.Properties.Resources._Close,
                                     ok: async () => await CloseDialog());
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}