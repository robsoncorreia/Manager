using FC.Domain.Model;
using FC.Domain.Model.Configution;

namespace FC.Domain.Repository
{
    public interface ISettingsRepository
    {
        ConfigurationApp ConfigurationApp { get; set; }

        ConfigurationApp FindOneConfigurationApp();

        object Update();

        object Update(ConfigurationApp configurationApp);

        void SetTerminalEnable(bool isTerminalEnable);

        void SetGatewayConnectionType(GatewayConnectionType type);
    }

    public class SettingsRepository : ISettingsRepository
    {
        public ConfigurationApp ConfigurationApp { get; set; }

        public SettingsRepository(ILocalDBRepository liteDBService)
        {
            _liteDBService = liteDBService;
            ConfigurationApp = FindOneConfigurationApp();
        }

        public ConfigurationApp FindOneConfigurationApp()
        {
            return _liteDBService.FindOneConfigurationApp();
        }

        public object Update(ConfigurationApp configurationApp)
        {
            return _liteDBService.Update(configurationApp);
        }

        public object Update()
        {
            return _liteDBService.Update(ConfigurationApp);
        }

        public void SetTerminalEnable(bool isTerminalEnable)
        {
            Domain.Properties.Settings.Default.enableTerminal = isTerminalEnable;
            Domain.Properties.Settings.Default.Save();
        }

        public void SetGatewayConnectionType(GatewayConnectionType type)
        {
            Domain.Properties.Settings.Default.gatewayConnectionType = (int)type;
            Domain.Properties.Settings.Default.Save();
        }

        private readonly ILocalDBRepository _liteDBService;
    }
}