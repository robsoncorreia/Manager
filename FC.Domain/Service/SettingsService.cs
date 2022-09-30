using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FC.Domain.Service
{
    public interface ISettingsService
    {
        bool IsTerminalEnable { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public class SettingsService : INotifyPropertyChanged, ISettingsService
    {
        private bool isTerminalEnable = Properties.Settings.Default.enableTerminal;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsTerminalEnable
        {
            get => isTerminalEnable;
            set
            {
                isTerminalEnable = value;
                NotifyPropertyChanged();
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}