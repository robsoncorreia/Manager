using FC.Domain._Base;
using System.Windows;

namespace FC.Domain.Model.Control
{
    public class CustomTab : ModelBase
    {
        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                if (Equals(_isEnable, value))
                {
                    return;
                }
                _isEnable = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (Equals(_name, value))
                {
                    return;
                }
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public Visibility Visibility
        {
            get => _visibility;
            set
            {
                if (Equals(_visibility, value))
                {
                    return;
                }
                _visibility = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isEnable;
        private string _name;
        private Visibility _visibility = Visibility.Visible;
    }
}