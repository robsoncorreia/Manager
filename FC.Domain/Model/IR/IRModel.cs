using FC.Domain._Base;
using Newtonsoft.Json;
using Parse;
using System.ComponentModel;

namespace FC.Domain.Model.IR
{
    public enum ChannelIR
    {
        [Description("Channel 1")]
        Channel1 = 1,

        [Description("Channel 2")]
        Channel2 = 2,

        [Description("Channel 3")]
        Channel3 = 3,

        [Description("Blaster")]
        Blaster = 153
    }

    public class IRModel : ModelBase
    {
        public int Delay
        {
            get => _delay;
            set
            {
                if (value is < 0 or > 255)
                {
                    return;
                }
                _delay = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (value is null)
                {
                    return;
                }
                if (value.Length > maxLengthDescription)
                {
                    return;
                }
                _description = value;
                NotifyPropertyChanged();
            }
        }

        public int Frequency { get; set; }

        public int IRModelID { get; set; }

        public int Lenght { get; set; } = -1;

        public int NumberChannel
        {
            get => _numberChannel;
            set
            {
                _numberChannel = value;
                NotifyPropertyChanged();
            }
        }

        [JsonIgnore]
        public ParseObject ParseObject { get; set; }

        public int MemoryId
        {
            get => _memoryId;

            set
            {
                _memoryId = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isSending = false;

        public bool IsSending
        {
            get => _isSending;
            set
            {
                _isSending = value;
                NotifyPropertyChanged();
            }
        }

        public int Repetition
        {
            get => _repetition;
            set
            {
                if (value is < 1 or > 255)
                {
                    return;
                }
                _repetition = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedChannelIndex
        {
            get => _selectedChannelIndex;
            set
            {
                if (Equals(_selectedChannelIndex, value))
                {
                    return;
                }
                _selectedChannelIndex = value;
                NotifyPropertyChanged();
            }
        }

        public int Version { get; set; } = 0;

        public int VersionId { get; set; }

        public int Channel
        {
            get => _channel;
            set
            {
                if (Equals(value, _channel))
                {
                    return;
                }

                SelectedChannelIndex = value switch
                {
                    1 => 0,
                    2 => 1,
                    3 => 2,
                    153 => 3,
                    _ => -1,
                };

                _channel = value;

                NotifyPropertyChanged();
            }
        }

        public string XData { get; set; } = "00000000";

        public string YData { get; set; } = "00000000";

        public string Data
        {
            get => _data;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _data = value;
                    NotifyPropertyChanged();
                    return;
                }
                _data = value.Replace(" ", "").Replace("\n", "").Replace("\t", "");
                NotifyPropertyChanged();
            }
        }

        private bool _isCompress;

        public bool IsCompress
        {
            get => _isCompress;
            set
            {
                if (Equals(_isCompress, value))
                {
                    return;
                }
                _isCompress = value;
                NotifyPropertyChanged();
            }
        }

        public string DataCompress
        {
            get => _dataCompress;
            set
            {
                if (Equals(_dataCompress, value))
                {
                    return;
                }

                _dataCompress = value;
                NotifyPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"{Description}";
        }

        private const int maxLengthDescription = 30;
        private int _delay;
        private string _description;
        private int _numberChannel;
        private int _memoryId = -1;
        private int _repetition = 1;
        private int _selectedChannelIndex = 3;
        private string _data;
        private int _channel;
        private string _dataCompress;
    }
}