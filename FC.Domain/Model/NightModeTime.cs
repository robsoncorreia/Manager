using FC.Domain._Base;
using System;

namespace FC.Domain.Model
{
    public class NightModeTime : ModelBase
    {
        private DateTime _EndNightModeTime;
        private DateTime _StartNightModeTime;

        public DateTime EndNightModeTime
        {
            get => _EndNightModeTime;
            set
            {
                _EndNightModeTime = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime StartNightModeTime
        {
            get => _StartNightModeTime;
            set
            {
                _StartNightModeTime = value;
                NotifyPropertyChanged();
            }
        }
    }
}