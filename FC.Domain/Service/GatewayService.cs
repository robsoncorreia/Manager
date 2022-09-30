using FC.Domain.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace FC.Domain.Service
{
    public interface IGatewayService
    {
        ObservableCollection<GatewayModel> Gateways { get; set; }
        bool IsSendingToGateway { get; set; }
        bool IsPrimary { get; set; }
        string LastCommandSend { get; set; }
        GatewayModel SelectedGateway { get; set; }
        int TabControlSelectedIndex { get; set; }
        int RadioCount { get; set; }
        TabControl TabControl { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public class GatewayService : INotifyPropertyChanged, IGatewayService
    {
        public TabControl TabControl { get; set; }

        private int _radioCount;

        public int RadioCount
        {
            get => _radioCount;
            set
            {
                _radioCount = value;
                NotifyPropertyChanged();
            }
        }

        private string _lastCommandSend;

        public string LastCommandSend
        {
            get => _lastCommandSend;
            set
            {
                _lastCommandSend = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<GatewayModel> Gateways { get; set; }

        public bool IsPrimary
        {
            get => _IsPrimary;
            set
            {
                _IsPrimary = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSendingToGateway
        {
            get => _isSending;
            set
            {
                _isSending = value;
                NotifyPropertyChanged();
            }
        }

        public GatewayModel SelectedGateway
        {
            get => _selectedGateway;
            set
            {
                _selectedGateway = value;
                NotifyPropertyChanged();
            }
        }

        public int TabControlSelectedIndex
        {
            get => _tabControlSelectedIndex;
            set
            {
                _tabControlSelectedIndex = value;
                NotifyPropertyChanged();
            }
        }

        public GatewayService()
        {
            SelectedGateway = new GatewayModel();

            Gateways = new ObservableCollection<GatewayModel>();
        }

        private bool _isSending;
        private GatewayModel _selectedGateway;
        private int _tabControlSelectedIndex;
        private bool _IsPrimary;

        #region INotifyPropertyChanged

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged
    }
}