using FC.Domain.Model.License;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FC.Domain.Service
{
    public interface ILicenseService
    {
        bool IsManager { get; set; }
        LicenseModel SelectedLicense { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public class LicenseService : INotifyPropertyChanged, ILicenseService
    {
        public bool IsManager
        {
            get => _isManager;
            set
            {
                _isManager = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isManager;
        private LicenseModel _selectedLicense;

        public LicenseModel SelectedLicense
        {
            get => _selectedLicense;
            set
            {
                if (Equals(_selectedLicense, value))
                {
                    return;
                }
                _selectedLicense = value;
                NotifyPropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged
    }
}