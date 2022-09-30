using CommunityToolkit.Mvvm.DependencyInjection;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.ViewModel._Dashboard;
using FC.Manager.ViewModel._Util;
using FC.Manager.ViewModel.AccountSetting;
using FC.Manager.ViewModel.Components;
using FC.Manager.ViewModel.Configuration;
using FC.Manager.ViewModel.Login;
using FC.Manager.ViewModel.Project;
using FC.Manager.ViewModel.Project.Archived;
using FC.Manager.ViewModel.Project.Create;
using FC.Manager.ViewModel.Project.Dashbord;
using FC.Manager.ViewModel.Project.Detail;
using FC.Manager.ViewModel.Project.Device.Detail;
using FC.Manager.ViewModel.Project.Device.Edit;
using FC.Manager.ViewModel.Project.Device.IfThen;
using FC.Manager.ViewModel.Project.Device.Zwave.Config.Dome;
using FC.Manager.ViewModel.Project.Device.Zwave.Config.Flex;
using FC.Manager.ViewModel.Project.Gateway.Add;
using FC.Manager.ViewModel.Project.Gateway.Zwave;
using FC.Manager.ViewModel.Project.Gateway.Zwave.Test;
using FC.Manager.ViewModel.Project.RecicleBin;
using FC.Manager.ViewModel.SoftwareAdministrator;
using Microsoft.Extensions.DependencyInjection;

namespace FC.Manager.ViewModel
{
    public class ViewModelLocator
    {
        //todo #2

        public ViewModelLocator()
        {
            Ioc.Default.ConfigureServices
                (new ServiceCollection()
            .AddSingleton<IGoogleDriveService, GoogleDriveService>()
            .AddSingleton<IGoogleDriveService, GoogleDriveService>()
            .AddSingleton<IGoogleApiService, GoogleApiService>()
            .AddSingleton<IDialogService, DialogService>()
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
            .AddSingleton<ISettingsService, SettingsService>()
            .AddSingleton<IScheduleService, ScheduleService>()
            .AddSingleton<SecondaryZwaveViewModel>()
            .AddSingleton<ZwaveDeviceConfigViewModel>()
            .AddSingleton<ZwaveDetailDeviceViewModel>()
            .AddSingleton<ZwaveAssociationViewModel>()
            .AddSingleton<VoiceAssistantDetailDeviceViewModel>()
            .AddSingleton<UserDetailDeviceViewModel>()
            .AddSingleton<TerminalViewModel>()
            .AddSingleton<SoftwareAdministratorViewModel>()
            .AddSingleton<SerialDetailDeviceViewModel>()
            .AddSingleton<RTSDetailDeviceViewModel>()
            .AddSingleton<RelayTestDetailDeviceViewModel>()
            .AddSingleton<RegisterUserViewModel>()
            .AddSingleton<RecicleBinProjectViewModel>()
            .AddSingleton<Radio433DetailDeviceViewModel>()
            .AddSingleton<ProjectViewModel>()
            .AddSingleton<NetworkDetailDeviceViewModel>()
            .AddSingleton<MotionDetectorConfigViewModel>()
            .AddSingleton<MainViewModel>()
            .AddSingleton<LoginViewModel>()
            .AddSingleton<ListIfThenViewModel>()
            .AddSingleton<IRLearnDetailDeviceViewModel>()
            .AddSingleton<IPCommandDetailDeviceViewModel>()
            .AddSingleton<FXS69AConfigViewModel>()
            .AddSingleton<FXR5011ConfigViewModel>()
            .AddSingleton<FXR211ConfigViewModel>()
            .AddSingleton<FXM5012ConfigViewModel>()
            .AddSingleton<FXD5011ConfigViewModel>()
            .AddSingleton<FXD220ConfigViewModel>()
            .AddSingleton<FXD211ConfigViewModel>()
            .AddSingleton<FXC222ConfigViewModel>()
            .AddSingleton<FXC221ConfigViewModel>()
            .AddSingleton<FXA0600ConfigViewModel>()
            .AddSingleton<DoorWindowSensorConfigViewModel>()
            .AddSingleton<DetailProjectViewModel>()
            .AddSingleton<DetailDeviceViewModel>()
            .AddSingleton<DashboardProjectViewModel>()
            .AddSingleton<DashboardIfThenViewModel>()
            .AddSingleton<DashboardViewModel>()
            .AddSingleton<CustomMessageBoxViewModel>()
            .AddSingleton<CreateProjectViewModel>()
            .AddSingleton<CreateIfThenViewModel>()
            .AddSingleton<ConfigurationDatailDeviceViewModel>()
            .AddSingleton<ConfigurationViewModel>()
            .AddSingleton<ArchivedProjectViewModel>()
            .AddSingleton<AddDeviceViewModel>()
            .AddSingleton<AccountSettingViewModel>()
            .AddSingleton<ListScheduleViewModel>()
            .AddSingleton<DashboardScheduleViewModel>()
            .AddSingleton<FXA3000ConfigViewModel>()
            .AddSingleton<CreateScheduleViewModel>()
            .AddSingleton<ZXT600ConfigViewModel>()
            .AddSingleton<GatewayConnectionTypeViewModel>()
            .AddSingleton<ZXT600TestViewModel>()
            .AddSingleton<TerminalAutoCompleteViewModel>()
            .AddSingleton<TerminalAutoScrollViewModel>()
            .AddSingleton<DeleteSmartSearchViewModel>()
            .AddSingleton<InfoIfThenViewModel>()
            .BuildServiceProvider());
        }

        public InfoIfThenViewModel InfoIfThen => Ioc.Default.GetRequiredService<InfoIfThenViewModel>();

        public DeleteSmartSearchViewModel DeleteSmartSearch => Ioc.Default.GetRequiredService<DeleteSmartSearchViewModel>();

        public TerminalAutoScrollViewModel TerminalAutoScroll => Ioc.Default.GetRequiredService<TerminalAutoScrollViewModel>();

        public TerminalAutoCompleteViewModel TerminalAutoComplete => Ioc.Default.GetRequiredService<TerminalAutoCompleteViewModel>();

        public ZXT600TestViewModel ZXT600Test => Ioc.Default.GetRequiredService<ZXT600TestViewModel>();

        public GatewayConnectionTypeViewModel GatewayConnectionType => Ioc.Default.GetRequiredService<GatewayConnectionTypeViewModel>();

        public ZXT600ConfigViewModel ZXT600Config => Ioc.Default.GetRequiredService<ZXT600ConfigViewModel>();

        public CreateScheduleViewModel CreateSchedule => Ioc.Default.GetRequiredService<CreateScheduleViewModel>();

        public FXA3000ConfigViewModel FXA3000Config => Ioc.Default.GetRequiredService<FXA3000ConfigViewModel>();

        public DashboardScheduleViewModel DashboardSchedule => Ioc.Default.GetRequiredService<DashboardScheduleViewModel>();

        public ListScheduleViewModel ListSchedule => Ioc.Default.GetRequiredService<ListScheduleViewModel>();

        public AccountSettingViewModel AccountSetting => Ioc.Default.GetRequiredService<AccountSettingViewModel>();

        public AddDeviceViewModel AddDevice => Ioc.Default.GetRequiredService<AddDeviceViewModel>();

        public ArchivedProjectViewModel ArchivedProject => Ioc.Default.GetRequiredService<ArchivedProjectViewModel>();

        public ConfigurationViewModel Configuration => Ioc.Default.GetRequiredService<ConfigurationViewModel>();

        public ConfigurationDatailDeviceViewModel ConfigurationDatailDevice => Ioc.Default.GetRequiredService<ConfigurationDatailDeviceViewModel>();

        public CreateIfThenViewModel CreateIfThen => Ioc.Default.GetRequiredService<CreateIfThenViewModel>();

        public CreateProjectViewModel CreateProject => Ioc.Default.GetRequiredService<CreateProjectViewModel>();

        public CustomMessageBoxViewModel CustomMessageBox => Ioc.Default.GetRequiredService<CustomMessageBoxViewModel>();

        public DashboardViewModel Dashboard => Ioc.Default.GetRequiredService<DashboardViewModel>();

        public DashboardIfThenViewModel DashboardIfThen => Ioc.Default.GetRequiredService<DashboardIfThenViewModel>();

        public DashboardProjectViewModel DashboardProject => Ioc.Default.GetRequiredService<DashboardProjectViewModel>();

        public DetailDeviceViewModel DetailDevice => Ioc.Default.GetRequiredService<DetailDeviceViewModel>();

        public DetailProjectViewModel DetailProject => Ioc.Default.GetRequiredService<DetailProjectViewModel>();

        public DoorWindowSensorConfigViewModel DoorWindowSensorConfig => Ioc.Default.GetRequiredService<DoorWindowSensorConfigViewModel>();

        public FXA0600ConfigViewModel FXA0600Config => Ioc.Default.GetRequiredService<FXA0600ConfigViewModel>();

        public FXC221ConfigViewModel FXC221Config => Ioc.Default.GetRequiredService<FXC221ConfigViewModel>();

        public FXC222ConfigViewModel FXC222Config => Ioc.Default.GetRequiredService<FXC222ConfigViewModel>();

        public FXD211ConfigViewModel FXD211Config => Ioc.Default.GetRequiredService<FXD211ConfigViewModel>();

        public FXD220ConfigViewModel FXD220Config => Ioc.Default.GetRequiredService<FXD220ConfigViewModel>();

        public FXD5011ConfigViewModel FXD5011Config => Ioc.Default.GetRequiredService<FXD5011ConfigViewModel>();

        public FXM5012ConfigViewModel FXM5012Config => Ioc.Default.GetRequiredService<FXM5012ConfigViewModel>();

        public FXR211ConfigViewModel FXR211Config => Ioc.Default.GetRequiredService<FXR211ConfigViewModel>();

        public FXR5011ConfigViewModel FXR5011Config => Ioc.Default.GetRequiredService<FXR5011ConfigViewModel>();

        public FXS69AConfigViewModel FXS69AConfig => Ioc.Default.GetRequiredService<FXS69AConfigViewModel>();

        public IPCommandDetailDeviceViewModel IPCommandDetailDevice => Ioc.Default.GetRequiredService<IPCommandDetailDeviceViewModel>();

        public IRLearnDetailDeviceViewModel IRLearnDetailDevice => Ioc.Default.GetRequiredService<IRLearnDetailDeviceViewModel>();

        public ListIfThenViewModel ListIfThen => Ioc.Default.GetRequiredService<ListIfThenViewModel>();

        public LoginViewModel Login => Ioc.Default.GetRequiredService<LoginViewModel>();

        public MainViewModel Main => Ioc.Default.GetRequiredService<MainViewModel>();

        public MotionDetectorConfigViewModel MotionDetectorConfig => Ioc.Default.GetRequiredService<MotionDetectorConfigViewModel>();

        public NetworkDetailDeviceViewModel NetworkDetailDevice => Ioc.Default.GetRequiredService<NetworkDetailDeviceViewModel>();

        public ProjectViewModel Project => Ioc.Default.GetRequiredService<ProjectViewModel>();

        public Radio433DetailDeviceViewModel Radio433DetailDevice => Ioc.Default.GetRequiredService<Radio433DetailDeviceViewModel>();

        public RecicleBinProjectViewModel RecicleBinProject => Ioc.Default.GetRequiredService<RecicleBinProjectViewModel>();

        public RegisterUserViewModel RegisterUser => Ioc.Default.GetRequiredService<RegisterUserViewModel>();

        public RelayTestDetailDeviceViewModel RelayTestDetailDevice => Ioc.Default.GetRequiredService<RelayTestDetailDeviceViewModel>();

        public RTSDetailDeviceViewModel RTSDetailDevice => Ioc.Default.GetRequiredService<RTSDetailDeviceViewModel>();

        public SerialDetailDeviceViewModel SerialDetailDevice => Ioc.Default.GetRequiredService<SerialDetailDeviceViewModel>();

        public SoftwareAdministratorViewModel SoftwareAdministrator => Ioc.Default.GetRequiredService<SoftwareAdministratorViewModel>();

        public TerminalViewModel Terminal => Ioc.Default.GetRequiredService<TerminalViewModel>();

        public UserDetailDeviceViewModel UserDetailDevice => Ioc.Default.GetRequiredService<UserDetailDeviceViewModel>();

        public VoiceAssistantDetailDeviceViewModel VoiceAssistantDetailDevice => Ioc.Default.GetRequiredService<VoiceAssistantDetailDeviceViewModel>();

        public VoiceAssistantDetailDeviceViewModel VoiceAssistantSetup => Ioc.Default.GetRequiredService<VoiceAssistantDetailDeviceViewModel>();

        public ZwaveAssociationViewModel ZwaveAssociation => Ioc.Default.GetRequiredService<ZwaveAssociationViewModel>();

        public ZwaveDetailDeviceViewModel ZwaveDetailDevice => Ioc.Default.GetRequiredService<ZwaveDetailDeviceViewModel>();

        public ZwaveDeviceConfigViewModel ZwaveDeviceConfig => Ioc.Default.GetRequiredService<ZwaveDeviceConfigViewModel>();

        public SecondaryZwaveViewModel ZwaveQuickSettings => Ioc.Default.GetRequiredService<SecondaryZwaveViewModel>();

        public static void Cleanup()
        {
        }
    }
}