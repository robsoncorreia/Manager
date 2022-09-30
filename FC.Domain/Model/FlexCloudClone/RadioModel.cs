using FC.Domain._Base;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Parse;
using System;
using System.ComponentModel;

namespace FC.Domain.Model.FlexCloudClone
{
    [Flags]
    public enum ActionsRTSSomfy
    {
        [Description("My+Stop")]
        MY_STOP = 1,

        [Description("Up")]
        UP = 2,

        [Description("My+Up")]
        MY_UP = 3,

        [Description("Down")]
        DOWN = 4,

        [Description("My+Down")]
        MY_DOWN = 5,

        [Description("Up+Down")]
        UP_DOWN = 6,

        [Description("Prog")]
        PROG = 8,

        [Description("Enable sun wind sensor")]
        ENABLESUNWINDSENSOR = 9,

        [Description("Disable sun wind sensor")]
        DISABLESUNWINDSENSOR = 10
    }

    public enum RTSError
    {
        NOT_ACKNOWLEDGE = -1,
        HARDWARE_COMMUNICATION = 0,
        MEMORY_EMPTY = 1,
        MEMORY_UNAVAILABLE = 2,
        INVALID_MEMORY_TYPE = 3,
        TIMEOUT = 4
    }

    public enum TypeRF
    {
        R433,
        RTS,
        Any
    }

    public enum ErrorCodeRF
    {
        TimeOut = 4
    }

    public class RadioModel : ModelBase
    {
        private bool _isSensorRTS;

        public bool IsSensorRTS
        {
            get => _isSensorRTS;
            set
            {
                if (Equals(_isSensorRTS, value))
                {
                    return;
                }
                _isSensorRTS = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isSending;

        public bool IsSending
        {
            get => _isSending;
            set
            {
                _isSending = value;
                NotifyPropertyChanged();
            }
        }

        public ActionsRTSSomfy ActionRTSSomfy
        {
            get => _actionRTSSomfy;
            set
            {
                _actionRTSSomfy = value;
                if (_actionRTSSomfy != value)
                {
                    _actionRTSSomfy = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(CODE, NullValueHandling = NullValueHandling.Ignore)]
        public string Code
        {
            get => _code;
            set
            {
                if (_code != value)
                {
                    _code = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(DESCRIPTION, NullValueHandling = NullValueHandling.Ignore)]
        public string Description
        {
            get => _description;
            set
            {
                if (Equals(_description, value))
                {
                    return;
                }
                if (value is null)
                {
                    _description = value;

                    NotifyPropertyChanged();

                    return;
                }
                if (value.Length > 30)
                {
                    return;
                }

                _description = value;

                NotifyPropertyChanged();
            }
        }

        [JsonProperty(FREQUENCY_INDEX, NullValueHandling = NullValueHandling.Ignore)]
        public int FrequencyIndex
        {
            get => _frequencyIndex;
            set
            {
                if (_frequencyIndex != value)
                {
                    _frequencyIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(ID, NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty(IP, NullValueHandling = NullValueHandling.Ignore)]
        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsAdd
        {
            get => _isAdd;
            set
            {
                _isAdd = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(IS_PUBLIC, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsPublic { get; set; }

        [JsonProperty(IS_SAVED_CLOUD, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSavedCloud
        {
            get => _isSavedCloud;
            set
            {
                if (_isSavedCloud != value)
                {
                    _isSavedCloud = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(IS_SAVED_GATEWAY, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSavedGateway
        {
            get => _isSavedGateway;
            set
            {
                if (_isSavedGateway != value)
                {
                    _isSavedGateway = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonIgnore]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(MAC_ADDRESS, NullValueHandling = NullValueHandling.Ignore)]
        public string MacAddress { get; set; }

        [JsonProperty(MEMORY_ID, NullValueHandling = NullValueHandling.Ignore)]
        public int MemoryId
        {
            get => _memoryId;
            set
            {
                if (_memoryId != value)
                {
                    _memoryId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public PackIconKind PackIconKind
        {
            get => _packIconKind;
            set
            {
                _packIconKind = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(PARSE_USER_CURRENT_USER_OBJECTID, NullValueHandling = NullValueHandling.Ignore)]
        public string ParseUserCurrentUserObjectId { get; set; }

        [JsonProperty(PORT, NullValueHandling = NullValueHandling.Ignore)]
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(POWER, NullValueHandling = NullValueHandling.Ignore)]
        public int Power
        {
            get => _power;
            set
            {
                if (_power != value)
                {
                    _power = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(RADIO_FREQUENCYS, NullValueHandling = NullValueHandling.Ignore)]
        public RadioFrequency[] RadioFrequencys { get; set; } = new RadioFrequency[]
        {
            new RadioFrequency
            {
                Name = "433.92MHz",
                Frequency = Frequency.F_433_92
            },
            new RadioFrequency
            {
                Name = "433.42MHz",
                Frequency = Frequency.F_433_42
            },
            new RadioFrequency
            {
                Name = "433.00MHz",
                Frequency = Frequency.F_433
            },
            new RadioFrequency
            {
                Name = "433.96MHz",
                Frequency = Frequency.F_433_96
            }
        };

        [JsonProperty(REGISTER, NullValueHandling = NullValueHandling.Ignore)]
        public int Register
        {
            get => _register;
            set
            {
                _register = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(REMOTE_ID, NullValueHandling = NullValueHandling.Ignore)]
        public int RemoteId
        {
            get => _remoteId;
            set
            {
                if (_remoteId != value && value <= MAX_REMOTE_ID)
                {
                    _remoteId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(REPETITION, NullValueHandling = NullValueHandling.Ignore)]
        public int Repetition
        {
            get => _repetition;
            set
            {
                if (_repetition != value && value > 0 && value <= MAX_REPETITION)
                {
                    _repetition = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(ROLLING_CODE, NullValueHandling = NullValueHandling.Ignore)]
        public int RollingCode
        {
            get => _rollingCode;
            set
            {
                if (_rollingCode != value && value <= MAX_ROLLING_CODE)
                {
                    _rollingCode = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(TIME_TO_DOWN, NullValueHandling = NullValueHandling.Ignore)]
        public int TimeToDown
        {
            get => _timeToDown;
            set
            {
                if (_timeToDown != value)
                {
                    _timeToDown = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(TIME_TO_UP, NullValueHandling = NullValueHandling.Ignore)]
        public int TimeToUp
        {
            get => _timeToUp;
            set
            {
                if (_timeToUp != value)
                {
                    _timeToUp = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public const string TYPERF = "typeRF";

        public TypeRF TypeRF { get; set; }

        [JsonProperty(VERSION, NullValueHandling = NullValueHandling.Ignore)]
        public string Version
        {
            get => _version;
            set
            {
                if (_version != value)
                {
                    _version = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ParseObject ParseObject { get; set; }

        public const string RELATIONNAME = "rfs";

        public const string CLASSNAME = "RF";

        public override string ToString()
        {
            return $"{Description} {Properties.Resources.Memory_Id} {MemoryId}";
        }

        public const string CLASS_NAME = "RadioFrequency";
        public const string CODE = "code";
        public const string CONTROLLER_ID = "controllerId";
        public const string DESCRIPTION = "description";
        public const string FREQUENCY_INDEX = "frequencyIndex";
        public const string ID = "id";
        public const string IP = "ip";
        public const string IS_PUBLIC = "isPublic";
        public const string IS_SAVED_CLOUD = "isSavedCloud";
        public const string IS_SAVED_GATEWAY = "isSavedGateway";
        public const string MAC_ADDRESS = "macAddress";
        public const int MAX_REMOTE_ID = 16777215;
        public const int MIN_REMOTE_ID = 1;
        public const int MAX_REPETITION = 255;
        public const int MAX_ROLLING_CODE = 65535;
        public const string MEMORY_ID = "memoryId";
        public const string OBJECTID = "objectId";
        public const string PARSE_USER_CURRENT_USER_OBJECTID = "userObjectId";
        public const string PORT = "port";
        public const string POWER = "power";
        public const string RADIO_FREQUENCYS = "radioFrequencys";
        public const string REGISTER = "register";
        public const string REMOTE_ID = "remoteId";
        public const string REPETITION = "repetition";
        public const string ROLLING_CODE = "rollingCode";
        public const string TIME_TO_DOWN = "timeToDown";
        public const string TIME_TO_UP = "timeToUp";
        public const string VERSION = "version";

        private ActionsRTSSomfy _actionRTSSomfy;
        private string _code;
        private string _description;
        private int _frequencyIndex;
        private string _ip;
        private bool _isAdd;
        private bool _isSavedCloud;
        private bool _isSavedGateway;
        private bool _isSelected;
        private int _memoryId = -1;
        private PackIconKind _packIconKind = PackIconKind.CloudAlert;
        private int _port = 9999;
        private int _power;
        private int _register;
        private int _remoteId = 1;
        private int _repetition = 1;
        private int _rollingCode = 1;
        private int _timeToDown;
        private int _timeToUp;
        private string _version = "1.0";
    }
}