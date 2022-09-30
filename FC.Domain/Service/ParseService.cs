using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FC.Domain.Service
{
    public interface IParseService
    {
        public bool IsSendingToCloud { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public class ParseService : INotifyPropertyChanged, IParseService
    {
        private bool _isSendingToCloud;

        public bool IsSendingToCloud
        {
            get => _isSendingToCloud; set
            {
                if (Equals(_isSendingToCloud, value))
                {
                    return;
                }

                _isSendingToCloud = value;
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