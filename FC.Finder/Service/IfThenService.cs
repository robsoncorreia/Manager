using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ConfigurationFlexCloudHubBlaster.Service
{
    public interface IIfThenService
    {
        Uri Source { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public class IfThenService : IIfThenService
    {
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Uri _source;

        public Uri Source
        {
            get => _source;
            set
            {
                _source = value;
                NotifyPropertyChanged();
            }
        }
    }
}