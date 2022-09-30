using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QRCodeGenerator.Service
{
    public interface IDialogService
    {
        bool IsOpenDialogHost { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public class DialogService : INotifyPropertyChanged, IDialogService
    {
        private bool _isOpenDialogHost;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsOpenDialogHost
        {
            get => _isOpenDialogHost;
            set
            {
                _isOpenDialogHost = value;
                NotifyPropertyChanged();
            }
        }
    }
}