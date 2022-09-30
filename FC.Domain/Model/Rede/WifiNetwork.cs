using FC.Domain._Base;
using NativeWifi;

namespace FC.Domain.Model.Rede
{
    public class WifiNetwork : ModelBase
    {
        public string Dot11DefaultAuthAlgorithm
        {
            get => _dot11DefaultAuthAlgorithm;
            set
            {
                _dot11DefaultAuthAlgorithm = value;
                NotifyPropertyChanged();
            }
        }

        public string Dot11DefaultCipherAlgorithm
        {
            get => _dot11DefaultCipherAlgorithm;
            set
            {
                _dot11DefaultCipherAlgorithm = value;
                NotifyPropertyChanged();
            }
        }

        public string NetworkConnectable
        {
            get => _networkConnectable;
            set
            {
                _networkConnectable = value;
                NotifyPropertyChanged();
            }
        }

        public string Ssid
        {
            get => _ssid;
            set
            {
                _ssid = value;
                NotifyPropertyChanged();
            }
        }

        public string WlanNotConnectableReason
        {
            get => _wlanNotConnectableReason;
            set
            {
                _wlanNotConnectableReason = value;
                NotifyPropertyChanged();
            }
        }

        public int WlanSignalQuality
        {
            get => _wlanSignalQuality;
            set
            {
                _wlanSignalQuality = value;
                NotifyPropertyChanged();
            }
        }

        public WlanClient.WlanInterface WlanInterface { get; set; } = null;
        private string _dot11DefaultAuthAlgorithm;
        private string _dot11DefaultCipherAlgorithm;
        private string _networkConnectable;
        private string _ssid;
        private string _wlanNotConnectableReason;
        private int _wlanSignalQuality;
    }
}