using FC.Domain._Base;
using Newtonsoft.Json;

namespace FC.Domain.Model._Serial
{
    public enum ParitySerial
    {
        None = 0,
        Even = 2,
        Odd = 3,
    }

    public enum SerialBaudRate
    {
        Baudrate1200 = 0,
        Baudrate2400 = 1,
        Baudrate4800 = 2,
        Baudrate9600 = 3,
        Baudrate19200 = 4,
        Baudrate38400 = 5,
        Baudrate57600 = 6,
        Baudrate115200 = 7,
    }

    public enum SerialProtocol
    {
        T232 = 1,
        T485 = 2
    }

    public class ComboxItemModel : ModelBase
    {
        #region Private Property

        private string _name;
        private int _value;

        #endregion Private Property

        #region Public Property

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }

        #endregion Public Property
    }

    public class ParitySerialModel : ComboxItemModel
    { }

    public class SerialBaudRateModel : ComboxItemModel
    { }

    public class SerialCombox : ComboxItemModel
    { }

    public class SerialModel : ModelBase
    {
        public const int LENGTH_MAX_DATA = 100;

        #region Private Fields

        private string _command;
        private string _data;
        private int _delay;
        private int _id = -1;
        private int _protocol;
        private int _repetition = 1;
        private SerialProtocol _serialProtocol;

        private int _type;
        private int _uartId = -1;

        #endregion Private Fields

        #region Public Properties

        [JsonProperty("command", NullValueHandling = NullValueHandling.Ignore)]
        public string Command
        {
            get => _command;
            set
            {
                _command = value;
                NotifyPropertyChanged();
            }
        }

        public override string ToString()
        {
            return Data;
        }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public string Data
        {
            get => _data;
            set
            {
                if (value?.Length > LENGTH_MAX_DATA)
                {
                    return;
                }
                _data = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("delay", NullValueHandling = NullValueHandling.Ignore)]
        public int Delay
        {
            get => _delay;
            set
            {
                _delay = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int MemoryId
        {
            get => _id;
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("protocol", NullValueHandling = NullValueHandling.Ignore)]
        public int Protocol
        {
            get => _protocol;
            set
            {
                if (_protocol != value)
                {
                    _protocol = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty("repetition", NullValueHandling = NullValueHandling.Ignore)]
        public int Repetition
        {
            get => _repetition;
            set
            {
                _repetition = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedProtocolIndex
        {
            get => _selectedProtocolIndex;
            set
            {
                if (_selectedProtocolIndex != value)
                {
                    _selectedProtocolIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public SerialProtocol SerialProtocol
        {
            get => _serialProtocol;
            set
            {
                _serialProtocol = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public int Type
        {
            get => _type;
            set
            {
                _type = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("uart_id", NullValueHandling = NullValueHandling.Ignore)]
        public int UartId
        {
            get => _uartId;
            set
            {
                _uartId = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedProtocolIndex;

        #endregion Public Properties
    }
}