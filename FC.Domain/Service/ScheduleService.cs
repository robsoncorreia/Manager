using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FC.Domain.Service
{
    public interface IScheduleService
    {
        Uri Source { get; set; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public class ScheduleService : IScheduleService
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