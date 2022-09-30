using FC.Domain._Base;
using System.Collections.Generic;

namespace FC.Domain.Model.Configution
{
    public class ConfigurationApp : ModelBase
    {
        private bool _autoCompleteTerminal;
        private bool _autoScrollTerminal;
        private string _parseUserEmail;
        private double _udpDelayBetweenCommands = 50;
        private double _udpWaitTimeResponsesValue = 1500;

        public bool AutoCompleteTerminal
        {
            get => _autoCompleteTerminal;
            set
            {
                _autoCompleteTerminal = value;
                NotifyPropertyChanged();
            }
        }

        public bool AutoScrollTerminal
        {
            get => _autoScrollTerminal;
            set
            {
                _autoScrollTerminal = value;
                NotifyPropertyChanged();
            }
        }

        public int Id { get; set; }
        public IEnumerable<string> OrderDetailDeviceTabs { get; set; }
        public IEnumerable<string> OrderProjectDetailTabs { get; set; }

        public IEnumerable<string> OrderZwaveDeviceConfig { get; set; }
        public string ParseUserCurrentUserObjectId { get; set; }

        public string ParseUserEmail
        {
            get => _parseUserEmail;
            set
            {
                _parseUserEmail = value;
                NotifyPropertyChanged();
            }
        }

        public string[] ProjectsOrder { get; set; }
        public int SelectedIndexItemsPerPageHistoryLicenseManager { get; set; }
        public int SelectedIndexItemsPerPageLogLicenseManager { get; set; }
        public int SelectedIndexPageLicenseManager { get; set; }

        public double UDPDelayBetweenCommands
        {
            get => _udpDelayBetweenCommands;
            set
            {
                _udpDelayBetweenCommands = value;
                NotifyPropertyChanged();
            }
        }

        public double UDPWaitTimeResponsesValue
        {
            get => _udpWaitTimeResponsesValue;
            set
            {
                _udpWaitTimeResponsesValue = value;
                NotifyPropertyChanged();
            }
        }

        public void Reset()
        {
            AutoCompleteTerminal = false;
            AutoScrollTerminal = false;
            UDPDelayBetweenCommands = 50;
            UDPWaitTimeResponsesValue = 1500;
        }
    }
}