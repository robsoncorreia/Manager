using CommunityToolkit.Mvvm.DependencyInjection;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Updater.ViewModel.Components.Terminal;
using FC.Updater.ViewModel.Gateway;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Updater.ViewModel
{
    public class ViewModelLocator
    {
        public CustomMessageBoxViewModel CustomMessageBox => Ioc.Default.GetRequiredService<CustomMessageBoxViewModel>();

        public MainViewModel MainView => Ioc.Default.GetRequiredService<MainViewModel>();

        public TerminalAutoScrollViewModel TerminalAutoScroll => Ioc.Default.GetRequiredService<TerminalAutoScrollViewModel>();

        public TerminalAutoCompleteViewModel TerminalAutoComplete => Ioc.Default.GetRequiredService<TerminalAutoCompleteViewModel>();

        public TerminalViewModel Terminal => Ioc.Default.GetRequiredService<TerminalViewModel>();

        public GatewayViewModel Gateway => Ioc.Default.GetRequiredService<GatewayViewModel>();

        public ViewModelLocator()
        {
            Ioc.Default.ConfigureServices
                (new ServiceCollection()
            .AddSingleton<IIfThenRepository, IfThenRepository>()
            .AddSingleton<IIfThenService, IfThenService>()
            .AddSingleton<IVoiceAssistantRepository, VoiceAssistantRepository>()
            .AddSingleton<IParseService, ParseService>()
            .AddSingleton<IFrameNavigationService, FrameNavigationService>()
            .AddSingleton<IGatewayRepository, GatewayRepository>()
            .AddSingleton<IZwaveService, ZwaveService>()
            .AddSingleton<IZwaveRepository, ZwaveRepository>()
            .AddSingleton<IUDPRepository, UDPRepository>()
            .AddSingleton<ITcpRepository, TCPRespository>()
            .AddSingleton<ILocalDBRepository, LocalDBRepository>()
            .AddSingleton<ICommandRepository, CommandRepository>()
            .AddSingleton<ILoginRepository, LoginRepository>()
            .AddSingleton<IIRRepository, IRRepository>()
            .AddSingleton<IRFRepository, RFRepository>()
            .AddSingleton<IIPCommandRepository, IPCommandRepository>()
            .AddSingleton<IRelayRepository, RelayRepository>()
            .AddSingleton<ISerialRepository, SerialRepository>()
            .AddSingleton<IUserRepository, UserRepository>()
            .AddSingleton<ISettingsRepository, SettingsRepository>()
            .AddSingleton<ILogRepository, LogRepository>()
            .AddSingleton<IGatewayService, GatewayService>()
            .AddSingleton<IProjectService, ProjectService>()
            .AddSingleton<IProjectRepository, ProjectRepository>()
            .AddSingleton<ILicenseService, LicenseService>()
            .AddSingleton<ISoftwareAdministratorService, SoftwareAdministratorService>()
            .AddSingleton<ITaskService, TaskService>()
            .AddSingleton<ILanguageRepository, LanguageRepository>()
            .AddSingleton<INetworkService, NetworkService>()
            .AddSingleton<CustomMessageBoxViewModel>()
            .AddSingleton<MainViewModel>()
            .AddSingleton<TerminalAutoScrollViewModel>()
            .AddSingleton<TerminalAutoCompleteViewModel>()
            .AddSingleton<TerminalViewModel>()
            .AddSingleton<GatewayViewModel>()
            .BuildServiceProvider()
        );
        }

        public static void Cleanup()
        {
        }
    }
}