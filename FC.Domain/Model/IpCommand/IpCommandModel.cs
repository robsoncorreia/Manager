using FC.Domain._Base;
using Newtonsoft.Json;
using System;

namespace FC.Domain.Model.IpCommand
{
    public enum CommandTypeEnum
    {
        Hexa = 0,
        Base64 = 1,
        Text = 2
    }

    public enum IpType
    {
        TCP = 0,
        UDP = 1
    }

    public class IpCommandModel : ModelBase
    {
        [JsonProperty(COMMAND, NullValueHandling = NullValueHandling.Ignore)]
        public string Command
        {
            get => _command;
            set
            {
                _command = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(COMMAND_TYPE_INDEX, NullValueHandling = NullValueHandling.Ignore)]
        public int CommandTypeIndex
        {
            get => _commandTypeIndex;
            set
            {
                if (_commandTypeIndex != value)
                {
                    _commandTypeIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _isSending;

        [JsonIgnore]
        public bool IsSending
        {
            get => _isSending;
            set
            {
                _isSending = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(CREATEDAT, NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(DELAY, NullValueHandling = NullValueHandling.Ignore)]
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

        public IpType IpType
        {
            get => _ipType;
            set
            {
                if (_ipType != value)
                {
                    _ipType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(ISSAVEDCLOUD, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSavedCloud
        {
            get => _isSavedCloud;
            set
            {
                _isSavedCloud = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(ISSAVEDGATEWAY, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSavedGateway
        {
            get => _isSavedGateway;
            set
            {
                _isSavedGateway = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(ISSAVEDLOCALDATABASE, NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSavedLocalDatabase
        {
            get => _isSavedLocalDatabase;
            set
            {
                _isSavedLocalDatabase = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(MEMORYID, NullValueHandling = NullValueHandling.Ignore)]
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

        [JsonProperty(NAME, NullValueHandling = NullValueHandling.Ignore)]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(OBJECTID, NullValueHandling = NullValueHandling.Ignore)]
        public string ObjectId { get; set; }

        [JsonProperty(REPETITION, NullValueHandling = NullValueHandling.Ignore)]
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

        [JsonProperty(TARGET_IP, NullValueHandling = NullValueHandling.Ignore)]
        public string TargetIp
        {
            get => _targetIp;
            set
            {
                _targetIp = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(TARGET_PORT, NullValueHandling = NullValueHandling.Ignore)]
        public int TargetPort
        {
            get => _targetPort;
            set
            {
                _targetPort = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(UPDATEDAT, NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set
            {
                _updatedAt = value;
                NotifyPropertyChanged();
            }
        }

        public IpCommandModel()
        {
        }

        public override string ToString()
        {
            return $"{Name}\n{TargetIp} | {TargetPort}";
        }

        public const string COMMAND = "command";
        public const string COMMAND_TYPE_INDEX = "commandTypeIndex";
        public const string CREATEDAT = "createdAt";
        public const string CURRENTUSEROBJECTID = "currentUserObjectId";
        public const string DELAY = "Delay";
        public const string GATEWAY_IP = "gatewayIp";
        public const string GATEWAY_PORT = "gatewayPort";
        public const string GATEWAYMAC = "gatewayMac";
        public const string IPCOMMANDMODELLID = "ipCommandModel";
        public const string ISSAVEDCLOUD = "isSavedCloud";
        public const string ISSAVEDGATEWAY = "isSavedGateway";
        public const string ISSAVEDLOCALDATABASE = "isSavedLocalDatabase";
        public const string MEMORYID = "memoryId";
        public const string NAME = "name";
        public const string OBJECTID = "objectId";
        public const string REPETITION = "repetition";
        public const string TARGET_IP = "targetIp";
        public const string TARGET_PORT = "targetPort";
        public const string UPDATEDAT = "UpdatedAt";
        private string _command;

        // private CommandTypeEnum _commandType;
        private int _commandTypeIndex = 2;

        private DateTime? _createdAt;
        private int _delay;
        private IpType _ipType = IpType.UDP;
        private bool _isSavedCloud;
        private bool _isSavedGateway;
        private bool _isSavedLocalDatabase;
        private int _memoryId = -1;
        private string _name;
        private int _repetition;
        private string _targetIp;
        private int _targetPort;
        private DateTime? _updatedAt;
    }
}