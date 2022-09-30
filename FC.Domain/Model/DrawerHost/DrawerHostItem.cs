using FC.Domain._Base;
using MaterialDesignThemes.Wpf;

namespace FC.Domain.Model.DrawerHost
{
    public class DrawerHostItem : ModelBase
    {
        public EnumNamePage EnumName { get; set; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                NotifyPropertyChanged();
            }
        }

        public PackIconKind Kind { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        private bool _isVisible = true;
    }
}

public enum EnumNamePage
{
    Project,
    LoginFlux,
    Network,
    IfThen,
    DeviceRegistration,
    Terminal,
    Configuration,
    IRLearning,
    ExitToApp,
    FlexCloudClone,
    RelayTest,
    IPCommand,
    Gateway,
    License_Manager,
    SoftwareAdministrator
}