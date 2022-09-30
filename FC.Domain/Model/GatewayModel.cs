using FC.Domain._Base;
using FC.Domain.Model._Serial;
using FC.Domain.Model.Device;
using FC.Domain.Model.FCC;
using FC.Domain.Model.FlexCloudClone;
using FC.Domain.Model.IpCommand;
using FC.Domain.Model.IR;
using FC.Domain.Model.User;
using FC.Domain.Repository.Util;
using LiteDB;
using Newtonsoft.Json;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace FC.Domain.Model
{
    public enum GatewayFunctions
    {
        IPCommand,
        IR,
        Radio433,
        RTS,
        Serial,
        Relay
    }

    public enum GatewayConnectionType
    {
        TCP,
        UDP
    }

    public enum GatewayModelEnum
    {
        [Description(UtilGateway.FCIRF100311)]
        FCIRF100311 = 255,

        [Description(UtilGateway.FCIRF100211)]
        FCIRF100211 = 256,

        [Description(UtilGateway.FCZWS100)]
        FCZWS100V1 = 1000,

        [Description(UtilGateway.FCZWS100)]
        FCZWS100V2 = 1001,

        [Description(UtilGateway.FCZIR100211)]
        FCZIR100311 = 1002,

        [Description(UtilGateway.FCZIR100311)]
        FCZIR100211 = 1003,

        [Description(UtilGateway.FCGIR100311)]
        FCGIR100311 = 1005,

        [Description(UtilGateway.FCGIR100211)]
        FCGIR100211 = 1004,

        ANY = -1
    }

    public enum RelayStateMode
    {
        Permanent,
        Pulse
    }

    public enum ConnectionType
    {
        Default = 0,
        WiFi = 1,
        Ethernet = 2,
        EthernetWiFi = 3
    }

    public enum FilterByZwave
    {
        Name,
        NodeId
    }

    public enum TypeIPEnum
    {
        DHCP = 0,
        Static = 1
    }

    public enum WiFiStatusEnum
    {
        Disabled = 0,
        Enable = 1
    }

    public enum APStatusEnum
    {
        Disabled = 0,
        Enable = 1
    }

    public enum APDHCP
    {
        Disabled = 0,
        Enable = 1
    }

    public enum ZwaveFrequencyEnum
    {
        None = -1,
        US = 0,
        ANZBR = 1,
        EU = 2,
        IN = 3,
        IL = 4,
        HK = 5,
        JP = 6
    }

    public class GatewayModel : ModelBase
    {
        private bool _isSync;

        public bool IsSync
        {
            get => _isSync;
            set
            {
                _isSync = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexOrderByDirection;

        public int SelectedIndexOrderByDirection
        {
            get => _selectedIndexOrderByDirection;
            set
            {
                _selectedIndexOrderByDirection = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSupportdIfthen => new GatewayModelEnum[] { GatewayModelEnum.FCZIR100211, GatewayModelEnum.FCZIR100311, GatewayModelEnum.FCZWS100V1, GatewayModelEnum.FCZWS100V2 }.Contains(GatewayModelEnum);

        public bool IsSupportVoiceAssistant => new GatewayModelEnum[] { GatewayModelEnum.FCIRF100311, GatewayModelEnum.FCZIR100211, GatewayModelEnum.FCIRF100211 }.Contains(GatewayModelEnum);

        public bool IsSupportSerial485 => new GatewayModelEnum[] { GatewayModelEnum.FCZWS100V1, GatewayModelEnum.FCZWS100V2 }.Contains(GatewayModelEnum);

        public bool IsSupportsRF => new GatewayModelEnum[] { GatewayModelEnum.FCIRF100211, GatewayModelEnum.FCIRF100311 }.Contains(GatewayModelEnum);

        public bool IsSupportsRelay => !new GatewayModelEnum[] { GatewayModelEnum.FCZWS100V1, GatewayModelEnum.FCZWS100V2 }.Contains(GatewayModelEnum);

        public bool IsSupportsIR => new GatewayModelEnum[] { GatewayModelEnum.FCGIR100211, GatewayModelEnum.FCGIR100311, GatewayModelEnum.FCIRF100211, GatewayModelEnum.FCIRF100311, GatewayModelEnum.FCZIR100211, GatewayModelEnum.FCZIR100311 }.Contains(GatewayModelEnum);

        public bool IsSupportsSerial => new GatewayModelEnum[] { GatewayModelEnum.FCZWS100V1, GatewayModelEnum.FCZWS100V2, GatewayModelEnum.FCGIR100211, GatewayModelEnum.FCGIR100311, GatewayModelEnum.FCZIR100211 }.Contains(GatewayModelEnum);

        public IList<SerialBaudRateModel> BaudRates232 { get; private set; } = new List<SerialBaudRateModel>
        {
            new SerialBaudRateModel { Name = "1200", Value = 0 },
            new SerialBaudRateModel { Name = "2400", Value = 1 },
            new SerialBaudRateModel { Name = "4800", Value = 2 },
            new SerialBaudRateModel { Name = "9600", Value = 3 },
            new SerialBaudRateModel { Name = "19200", Value = 4 },
            new SerialBaudRateModel { Name = "38400", Value = 5 },
            new SerialBaudRateModel { Name = "57600", Value = 6 },
            new SerialBaudRateModel { Name = "115200", Value = 7 }
        };

        public IList<SerialBaudRateModel> BaudRates485 { get; private set; } = new List<SerialBaudRateModel>
        {
            new SerialBaudRateModel { Name = "1200", Value = 0 },
            new SerialBaudRateModel { Name = "2400", Value = 1 },
            new SerialBaudRateModel { Name = "4800", Value = 2 },
            new SerialBaudRateModel { Name = "9600", Value = 3 },
            new SerialBaudRateModel { Name = "19200", Value = 4 },
            new SerialBaudRateModel { Name = "38400", Value = 5 },
            new SerialBaudRateModel { Name = "57600", Value = 6 },
            new SerialBaudRateModel { Name = "115200", Value = 7 }
        };

        public IList<ParitySerialModel> Paritys232 { get; private set; } = new List<ParitySerialModel>
        {
            new ParitySerialModel { Name = "None", Value = 0},
            new ParitySerialModel { Name = "Even", Value = 2},
            new ParitySerialModel { Name = "Odd", Value = 3},
        };

        public IList<ParitySerialModel> Paritys485 { get; private set; } = new List<ParitySerialModel>
        {
            new ParitySerialModel { Name = "None", Value = 0},
            new ParitySerialModel { Name = "Even", Value = 2},
            new ParitySerialModel { Name = "Odd", Value = 3},
        };

        public SerialModel SelectedSerialModel
        {
            get => _selectedSerialModel;
            set
            {
                if (Equals(_selectedSerialModel, value))
                {
                    return;
                }
                _selectedSerialModel = value;
                NotifyPropertyChanged();
            }
        }

        private int _serialsCommandLimit = 255;

        public int SerialsCommandLimit
        {
            get => _serialsCommandLimit;
            set
            {
                if (Equals(_serialsCommandLimit, value))
                {
                    return;
                }
                _serialsCommandLimit = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedBaudRates232Index
        {
            get => _selectedBaudRates232Index;
            set
            {
                if (Equals(_selectedBaudRates232Index, value))
                {
                    return;
                }
                _selectedBaudRates232Index = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedBaudRates485Index
        {
            get => _selectedBaudRates485Index;
            set
            {
                if (Equals(_selectedBaudRates485Index, value))
                {
                    return;
                }
                _selectedBaudRates485Index = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedParity232Index
        {
            get => _selectedParity232Index;
            set
            {
                if (Equals(_selectedParity232Index, value))
                {
                    return;
                }
                _selectedParity232Index = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedParity485Index
        {
            get => _selectedParity485Index;
            set
            {
                if (Equals(_selectedParity485Index, value))
                {
                    return;
                }
                _selectedParity485Index = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexSerial = -1;

        public int SelectedIndexSerial
        {
            get => _selectedIndexSerial;
            set
            {
                _selectedIndexSerial = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<FirmwareModel> Firmwares { get; set; }
        public ObservableCollection<BuildModel> Builds { get; set; }

        public ObservableCollection<SerialModel> Serials { get; set; }

        private int _selectedIndexBuild;

        public int SelectedIndexBuild
        {
            get => _selectedIndexBuild;
            set
            {
                if (Equals(_selectedIndexBuild, value))
                {
                    return;
                }
                _selectedIndexBuild = value;
                NotifyPropertyChanged();
            }
        }

        public ObjectId Id { get; set; }

        private int _selectedIndexRadio433 = -1;

        private bool _isSuportThenElse = true;

        public bool IsSuportThenElse
        {
            get => _isSuportThenElse;
            set
            {
                _isSuportThenElse = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexRadio433
        {
            get => _selectedIndexRadio433;
            set
            {
                _selectedIndexRadio433 = value;
                NotifyPropertyChanged();
            }
        }

        public RemoteAccessStandaloneModel RemoteAccessStandaloneModel { get; set; }

        private int _selectedIndexAPDHCP;

        public int SelectedIndexAPDHCP
        {
            get => _selectedIndexAPDHCP;
            set
            {
                if (Equals(_selectedIndexAPDHCP, value))
                {
                    return;
                }
                _selectedIndexAPDHCP = value;

                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexAPStatus;

        public int SelectedIndexAPStatus
        {
            get => _selectedIndexAPStatus;
            set
            {
                _selectedIndexAPStatus = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexFilterby;

        public int SelectedIndexFilterby
        {
            get => _selectedIndexFilterby;
            set
            {
                if (Equals(_selectedIndexFilterby, value))
                {
                    return;
                }
                _selectedIndexFilterby = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexIR = -1;

        public int SelectedIndexIR
        {
            get => _selectedIndexIR;
            set
            {
                _selectedIndexIR = value;
                NotifyPropertyChanged();
            }
        }

        private int _stateRelayRead;

        public int StateRelayRead
        {
            get => _stateRelayRead;
            set
            {
                if (Equals(_stateRelayRead, value))
                {
                    return;
                }
                _stateRelayRead = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexRelayStateMode;

        public int SelectedIndexRelayStateMode
        {
            get => _selectedIndexRelayStateMode;
            set
            {
                if (Equals(_selectedIndexRelayStateMode, value))
                {
                    return;
                }
                _selectedIndexRelayStateMode = value;
                NotifyPropertyChanged();
            }
        }

        public bool StateRelay
        {
            get => _stateRelay;
            set
            {
                _stateRelay = value;
                NotifyPropertyChanged();
            }
        }

        private int _relayPulseTime;

        public int RelayPulseTime
        {
            get => _relayPulseTime;
            set
            {
                if (Equals(_relayPulseTime, value))
                {
                    return;
                }
                _relayPulseTime = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexOrderBy;

        public int SelectedIndexOrderBy
        {
            get => _selectedIndexOrderBy;
            set
            {
                if (Equals(_selectedIndexOrderBy, value))
                {
                    return;
                }
                _selectedIndexOrderBy = value;

                NotifyPropertyChanged();
            }
        }

        private int _selectedRadio433Index = -1;

        public int SelectedIndexRadio433Gateway
        {
            get => _selectedRadio433Index;
            set
            {
                _selectedRadio433Index = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexZwaveDevice;

        public int SelectedIndexZwaveDevice
        {
            get => _selectedIndexZwaveDevice;
            set
            {
                _selectedIndexZwaveDevice = value;
                NotifyPropertyChanged();
            }
        }

        private ZwaveFrequencyEnum _frequency = ZwaveFrequencyEnum.None;

        public ZwaveFrequencyEnum ZwaveFrequency
        {
            get => _frequency;
            set
            {
                _frequency = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexFilterRadio433 { get; set; }

        private int _selectedIndexActionRTSSomfy;

        public int SelectedIndexActionRTSSomfy
        {
            get => _selectedIndexActionRTSSomfy;
            set
            {
                if (Equals(_selectedIndexActionRTSSomfy, value))
                {
                    return;
                }
                _selectedIndexActionRTSSomfy = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexRadio433Cloud;

        public int SelectedIndexRadio433Cloud
        {
            get => _selectedIndexRadio433Cloud;
            set
            {
                if (Equals(_selectedIndexRadio433Cloud, value))
                {
                    return;
                }
                _selectedIndexRadio433Cloud = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexFilterRadio433Cloud;

        public int SelectedIndexFilterRadio433Cloud
        {
            get => _selectedIndexFilterRadio433Cloud;
            set
            {
                if (Equals(SelectedIndexFilterRadio433Cloud, value))
                {
                    return;
                }
                _selectedIndexFilterRadio433Cloud = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexFilterIPCommand;

        public int SelectedIndexFilterIPCommand
        {
            get => _selectedIndexFilterIPCommand;
            set
            {
                _selectedIndexFilterIPCommand = value;
                NotifyPropertyChanged();
            }
        }

        public string ActualIP
        {
            get => _actualip;
            set
            {
                _actualip = value;
                NotifyPropertyChanged();
            }
        }

        public string ActualMask
        {
            get => _actualMask;
            set
            {
                _actualMask = value;
                NotifyPropertyChanged();
            }
        }

        public string ActualRouterGateway
        {
            get => _actualRouterGateway;
            set
            {
                _actualRouterGateway = value;
                NotifyPropertyChanged();
            }
        }

        public int? ActualTCPPort

        {
            get => _ActualTCPPort;
            set
            {
                _ActualTCPPort = value;
                NotifyPropertyChanged();
            }
        }

        public int? ActualUDPPort
        {
            get => _actualUDPPort;
            set
            {
                _actualUDPPort = value;
                NotifyPropertyChanged();
            }
        }

        public IList<string> Blacklist { get; set; }

        [JsonIgnore]
        public IList<int> TimesRelay { get; }

        [JsonIgnore]
        public IList<int> Delays { get; }

        public ObservableCollection<ParseUserCustom> BlacklistUsers { get; set; }

        public ObservableCollection<IpCommandModel> IpCommands { get; set; }

        public ObservableCollection<RadioModel> Radios433Gateway { get; set; }

        public ObservableCollection<RadioModel> Radios433Cloud { get; set; }

        public ObservableCollection<RadioModel> RadiosRTSGateway { get; set; }

        public int Build
        {
            get => _build;
            set
            {
                if (Equals(_build, value))
                {
                    return;
                }
                _build = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentIpEthernet
        {
            get => _currentIpEthernet;
            set
            {
                _currentIpEthernet = value;
                NotifyPropertyChanged();
            }
        }

        internal void GetConnectionType()
        {
            if (LocalIP == CurrentIpEthernet)
            {
                LocalPortUDP = UdpPortEthernet;
                LocalPortTCP = TcpPortEthernet;
                ConnectionType = ConnectionType.Ethernet;

                return;
            }

            if (LocalIP == CurrentIpWiFi)
            {
                LocalPortUDP = UdpPortWifi;
                LocalPortTCP = TcpPortWiFi;
                ConnectionType = ConnectionType.WiFi;
                return;
            }
        }

        public string CurrentIpWiFi
        {
            get => _currentIpWiFi;
            set
            {
                _currentIpWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentMacAddressEthernet
        {
            get => _currentMacAddressEthernet;
            set
            {
                _currentMacAddressEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentMacAddressWiFi
        {
            get => _currentMacAddressWiFi;
            set
            {
                _currentMacAddressWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentMaskEthernet
        {
            get => _currentMaskEthernet;
            set
            {
                _currentMaskEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentMaskWiFi
        {
            get => _currentMaskWiFi;
            set
            {
                _currentMaskWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentRouterGatewayEthernet
        {
            get => _currentRouterGatewayEthernet;
            set
            {
                _currentRouterGatewayEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public string CurrentRouterGatewayWiFi
        {
            get => _currentRouterGatewayWiFi;
            set
            {
                _currentRouterGatewayWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public int? CurrentTcpPortEthernet
        {
            get => _currentTcpPortEthernet;
            set
            {
                _currentTcpPortEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public int? CurrentTcpPortWiFi
        {
            get => _currentTcpPortWiFi;
            set
            {
                _currentTcpPortWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public int? CurrentUdpPortEthernet
        {
            get => _currentUdpPortEthernet;
            set
            {
                _currentUdpPortEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public int? CurrentUdpPortWifi
        {
            get => _currentUdpPortWifi;
            set
            {
                _currentUdpPortWifi = value;
                NotifyPropertyChanged();
            }
        }

        public string DefaultName { get; set; }

        public string Firmware
        {
            get => _firmware;
            set
            {
                _firmware = value;
                NotifyPropertyChanged();
            }
        }

        public string HomeId
        {
            get => _homeId;
            set
            {
                _homeId = value;
                NotifyPropertyChanged();
            }
        }

        public string IPAP
        {
            get => _ipAP;
            set
            {
                _ipAP = value;
                NotifyPropertyChanged();
            }
        }

        public string StaticIPEthernet
        {
            get => _ipEthernet;
            set
            {
                _ipEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public string StaticIPWiFi
        {
            get => _ipWiFi;
            set
            {
                _ipWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsCloudEnabled
        {
            get => _isCloudEnabled;
            set
            {
                if (_isCloudEnabled != value)
                {
                    _isCloudEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsCurrentUserOnBlacklist
        {
            get => _isCurrentUserOnBlacklist;
            set
            {
                _isCurrentUserOnBlacklist = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnableBlackList
        {
            get => _isEnableBlackList;
            set
            {
                _isEnableBlackList = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnableWifi
        {
            get => _isEnableWifi;
            set
            {
                _isEnableWifi = value;
                NotifyPropertyChanged();
            }
        }

        public string LocalIP
        {
            get => _localIP;
            set
            {
                _localIP = value;
                NotifyPropertyChanged();
            }
        }

        public int LocalPortUDP
        {
            get => _localPort;
            set
            {
                _localPort = value;
                NotifyPropertyChanged();
            }
        }

        public string LocalMacAddress
        {
            get => _macAddress;
            set
            {
                _macAddress = value;
                NotifyPropertyChanged();
            }
        }

        public string MacAddressEthernet
        {
            get => _macAddressEthernet;
            set
            {
                _macAddressEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public string MacAddressWiFi
        {
            get => _macAddressWiFi;
            set
            {
                _macAddressWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public string ManufacturerCode
        {
            get => _manufacturerCode;
            set
            {
                _manufacturerCode = value;
                NotifyPropertyChanged();
            }
        }

        public string Mask
        {
            get => _mask;
            set
            {
                if (Equals(_mask, value))
                {
                    return;
                }
                _mask = value;
                NotifyPropertyChanged();
            }
        }

        public string MaskEthernet
        {
            get => _maskEthernet;
            set
            {
                if (Equals(_maskEthernet, value))
                {
                    return;
                }
                _maskEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public string MaskWiFi
        {
            get => _maskWiFi;
            set
            {
                _maskWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public ParseObject ParseObject { get; internal set; }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                NotifyPropertyChanged();
            }
        }

        public string Pin
        {
            get => _pin;
            set
            {
                _pin = value;
                NotifyPropertyChanged();
            }
        }

        public int ProductId
        {
            get => _productId;
            set
            {
                if (Equals(_productId, value))
                {
                    return;
                }

                _productId = value;

                GatewayModelEnum = (GatewayModelEnum)value;

                NotifyPropertyChanged();
            }
        }

        public string RadioBuildFirmware
        {
            get => _radioBuildFirmware;
            set
            {
                _radioBuildFirmware = value;
                NotifyPropertyChanged();
            }
        }

        public string RadioVersionFirmware
        {
            get => _radioVersionFirmware;
            set
            {
                _radioVersionFirmware = value;
                NotifyPropertyChanged();
            }
        }

        public string RouterGateway
        {
            get => _routerGateway;
            set
            {
                _routerGateway = value;
                NotifyPropertyChanged();
            }
        }

        public string RouterGatewayEthernet
        {
            get => _routerGatewayEthernet;
            set
            {
                _routerGatewayEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public string RouterGatewayWiFi
        {
            get => _routerGatewayWiFi;
            set
            {
                _routerGatewayWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexIPTypeEthernet
        {
            get => _selectedIndexIPTypeEthernet;
            set
            {
                _selectedIndexIPTypeEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexIPTypeWiFi
        {
            get => _selectedIndexIPTypeWiFi;
            set
            {
                _selectedIndexIPTypeWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexTabControl
        {
            get => _selectedIndexTabControl;
            set
            {
                if (Equals(_selectedIndexTabControl, value))
                {
                    return;
                }
                _selectedIndexTabControl = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedTabIndexIfThen;

        public int SelectedTabIndexIfThen
        {
            get => _selectedTabIndexIfThen;
            set
            {
                if (Equals(_selectedTabIndexIfThen, value))
                {
                    return;
                }
                _selectedTabIndexIfThen = value;
                NotifyPropertyChanged();
            }
        }

        public string GatewayFunctionName => (GatewayFunctions)SelectedTabIndexIfThen switch
        {
            GatewayFunctions.IPCommand => IpCommands.ElementAt(SelectedIndexIPCommand).Name,
            GatewayFunctions.IR => IRsGateway.ElementAt(SelectedIndexIR).Description,
            GatewayFunctions.Radio433 => Radios433Gateway.ElementAt(SelectedIndexRadio433).ToString(),
            GatewayFunctions.RTS => RadiosRTSGateway.ElementAt(SelectedIndexRTS).ToString(),
            GatewayFunctions.Serial => Serials.ElementAt(SelectedIndexSerial).Data,
            GatewayFunctions.Relay => (RelayStateMode)SelectedIndexRelayStateMode switch
            {
                RelayStateMode.Permanent => StateRelay ? Properties.Resources.Hold_on : Properties.Resources.Hold_off,
                RelayStateMode.Pulse => StateRelay ? string.Format(CultureInfo.CurrentCulture, Properties.Resources.Keeps_on_for_milliseconds, RelayPulseTime) : string.Format(CultureInfo.CurrentCulture, Properties.Resources.Keeps_off_for_milliseconds, RelayPulseTime),
                _ => string.Empty,
            },
            _ => string.Empty,
        };

        public int GatewayFunctionMemoryId => (GatewayFunctions)SelectedTabIndexIfThen switch
        {
            GatewayFunctions.IPCommand => IpCommands.ElementAt(SelectedIndexIPCommand).MemoryId,
            GatewayFunctions.IR => IRsGateway.ElementAt(SelectedIndexIR).MemoryId,
            GatewayFunctions.Radio433 => Radios433Gateway.ElementAt(SelectedIndexRadio433).MemoryId,
            GatewayFunctions.RTS => RadiosRTSGateway.ElementAt(SelectedIndexRTS).MemoryId,
            GatewayFunctions.Serial => Serials.ElementAt(SelectedIndexSerial).MemoryId,
            _ => -1,
        };

        public int SelectedIndexWiFiStatus
        {
            get => _selectedIndexWiFiStatus;
            set
            {
                _selectedIndexWiFiStatus = value;
                NotifyPropertyChanged();
            }
        }

        public TypeIPEnum SelectedTypeIPEthernet
        {
            get => _selectedTypeIPEthernet;
            set
            {
                _selectedTypeIPEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public TypeIPEnum SelectedTypeIPWiFi
        {
            get => _selectedTypeIPWiFi;
            set
            {
                _selectedTypeIPWiFi = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (Equals(_isSelected, value))
                {
                    return;
                }
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string SSID
        {
            get => _ssid;
            set
            {
                _ssid = value;
                NotifyPropertyChanged();
            }
        }

        public int TCPPort
        {
            get => _TCPPort;
            set
            {
                _TCPPort = value;
                NotifyPropertyChanged();
            }
        }

        public int TcpPortEthernet
        {
            get => _tcpPortEthernet;
            set
            {
                _tcpPortEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public int TcpPortWiFi
        {
            get => _tcpPortWiFi;
            set
            {
                _tcpPortWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public int UDPPort
        {
            get => _UDPPort;
            set
            {
                _UDPPort = value;
                NotifyPropertyChanged();
            }
        }

        public int UdpPortEthernet
        {
            get => _udpPortEthernet;
            set
            {
                _udpPortEthernet = value;
                NotifyPropertyChanged();
            }
        }

        public int UdpPortWifi
        {
            get => _udpPortWifi;
            set
            {
                _udpPortWifi = value;
                NotifyPropertyChanged();
            }
        }

        public string UID
        {
            get => _uid;
            set
            {
                if (Equals(_uid, value))
                {
                    return;
                }

                _uid = value;

                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<IRModel> IRsGateway { get; set; }
        public ObservableCollection<IRModel> IRsCloud { get; set; }
        public ObservableCollection<ZwaveDevice> ZwaveDevices { get; set; }
        public ObservableCollection<ZwaveDevice> SecondaryZwaveDevices { get; set; }

        public int TotalRadioMemory
        {
            get => _totalRadioMemory;
            set
            {
                _totalRadioMemory = value;
                NotifyPropertyChanged();
            }
        }

        private ZwaveDevice _selectedZwaveDevice;

        public ZwaveDevice SelectedZwaveDevice
        {
            get => _selectedZwaveDevice;
            set
            {
                _selectedZwaveDevice = value;
                NotifyPropertyChanged();
            }
        }

        public int TotalRadioMemoryInUse
        {
            get => _totalRadioMemoryInUse;
            set
            {
                _totalRadioMemoryInUse = value;
                NotifyPropertyChanged();
            }
        }

        private int _maximumNumberRFMemoryPositions;

        public int MaximumNumberRFMemoryPositions
        {
            get => _maximumNumberRFMemoryPositions;
            set
            {
                if (Equals(_maximumNumberRFMemoryPositions, value))
                {
                    return;
                }
                _maximumNumberRFMemoryPositions = value;
                NotifyPropertyChanged();
            }
        }

        public string APIP
        {
            get => _apIP;
            set
            {
                if (Equals(_apIP, value))
                {
                    return;
                }

                _apIP = value;
                NotifyPropertyChanged();
            }
        }

        public string LocalIPWiFi
        {
            get => _localIPWiFi;
            set
            {
                if (Equals(_localIPWiFi, value))
                {
                    return;
                }
                _localIPWiFi = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (Equals(_isEnabled, value))
                {
                    return;
                }
                _isEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public string APSSID
        {
            get => _apSSID;
            set
            {
                if (Equals(_apSSID, value))
                {
                    return;
                }

                _apSSID = value;
                NotifyPropertyChanged();
            }
        }

        public string APPassword
        {
            get => _apPassword;
            set
            {
                if (Equals(_apPassword, value))
                {
                    return;
                }
                _apPassword = value;
                NotifyPropertyChanged();
            }
        }

        public ConnectionType ConnectionType
        {
            get => _connectionType;
            set
            {
                if (Equals(_connectionType, value))
                {
                    return;
                }
                _connectionType = value;
                NotifyPropertyChanged();
            }
        }

        public int LocalPortTCP
        {
            get => _localPorTCP;
            set
            {
                if (Equals(_localPorTCP, value))
                {
                    return;
                }
                _localPorTCP = value;
                NotifyPropertyChanged();
            }
        }

        public string Background { get; set; }
        public int ChannelCount { get; internal set; }
        public bool IsIRBlasterAvailable { get; internal set; }
        public int IRCommandsLimit { get; internal set; }
        public int IRSizeLimit { get; internal set; }

        public int SelectedIndexIPCommand
        {
            get => _selectedIndexIPCommand;
            set
            {
                _selectedIndexIPCommand = value;
                NotifyPropertyChanged();
            }
        }

        private GatewayModelEnum _gatewayModelEnum;

        public GatewayModelEnum GatewayModelEnum
        {
            get => _gatewayModelEnum;
            set
            {
                switch (value)
                {
                    case GatewayModelEnum.FCIRF100311:
                        Name = Properties.Resources.FC_IRF100_311;
                        Background = "#30475E";
                        break;

                    case GatewayModelEnum.FCIRF100211:
                        Name = Properties.Resources.FC_IRF100_211;
                        Background = "#FBC687";
                        break;

                    case GatewayModelEnum.FCZWS100V1:
                        Name = Properties.Resources.FC_ZWS100;
                        Background = "#0D7377";
                        break;

                    case GatewayModelEnum.FCZWS100V2:
                        Name = Properties.Resources.FC_ZWS100;
                        Background = "#0D7377";
                        break;

                    case GatewayModelEnum.FCZIR100311:
                        Name = Properties.Resources.FC_ZIR100_311;
                        Background = "#726A95";
                        break;

                    case GatewayModelEnum.FCZIR100211:
                        Name = Properties.Resources.FC_ZIR100_211;
                        Background = "#679B9B";
                        break;

                    case GatewayModelEnum.FCGIR100211:
                        Name = Properties.Resources.FC_GIR100_211;
                        Background = "#5D54A4";
                        break;

                    case GatewayModelEnum.FCGIR100311:
                        Name = Properties.Resources.FC_GIR100_311;
                        Background = "#D789D7";
                        break;

                    case GatewayModelEnum.ANY:
                        break;

                    default:
                        break;
                }

                _gatewayModelEnum = value;
                NotifyPropertyChanged();
            }
        }

        private int _selectedIndexFirmware;

        public int SelectedIndexFirmware
        {
            get => _selectedIndexFirmware;
            set
            {
                _selectedIndexFirmware = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexRTS
        {
            get => _selectedIndexRTS;
            set
            {
                _selectedIndexRTS = value;
                NotifyPropertyChanged();
            }
        }

        public int ModuleId
        {
            get => _ModuleId;
            set
            {
                if (Equals(_selectedIndexIRCloud, value))
                {
                    return;
                }
                _ModuleId = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexIRCloud
        {
            get => _selectedIndexIRCloud;
            set
            {
                if (Equals(_selectedIndexIRCloud, value))
                {
                    return;
                }
                _selectedIndexIRCloud = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsPrimary
        {
            get => _IsPrimary;
            set
            {
                if (Equals(_IsPrimary, value))
                {
                    return;
                }
                _IsPrimary = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime? ClockTime
        {
            get => _ClockTime;
            set
            {
                _ClockTime = value;
                NotifyPropertyChanged();
            }
        }

        public GatewayModel()
        {
            IRsGateway = new ObservableCollection<IRModel>();
            IRsCloud = new ObservableCollection<IRModel>();
            BlacklistUsers = new ObservableCollection<ParseUserCustom>();
            IpCommands = new ObservableCollection<IpCommandModel>();
            Radios433Gateway = new ObservableCollection<RadioModel>();
            Radios433Cloud = new ObservableCollection<RadioModel>();
            RadiosRTSGateway = new ObservableCollection<RadioModel>();
            ZwaveDevices = new ObservableCollection<ZwaveDevice>();
            SecondaryZwaveDevices = new ObservableCollection<ZwaveDevice>();
            Blacklist = new List<string>();
            RemoteAccessStandaloneModel = new RemoteAccessStandaloneModel();
            Firmwares = new ObservableCollection<FirmwareModel>();
            Builds = new ObservableCollection<BuildModel>();
            Serials = new ObservableCollection<SerialModel>();
            SelectedSerialModel = new SerialModel();
            TimesRelay = UtilIfThen.DELAYS;
            Delays = UtilIfThen.DELAYS;
        }

        public override string ToString()
        {
            return $"{Name}\n{Properties.Resources.Pin} {Pin}";
        }

        public const string _UID = "UID";
        public const string BUILD = "build";
        public const string IS_CLOUDENABLED = "isCloudEnabled";
        public const string PRODUCT_ID = "productId";

        private string _actualip;
        private string _actualMask;
        private string _actualRouterGateway;
        private int? _ActualTCPPort;
        private int? _actualUDPPort;
        private int _build = -1;
        private ConnectionType _connectionType;
        private string _currentIpEthernet;
        private string _currentIpWiFi;
        private string _currentMacAddressEthernet;
        private string _currentMacAddressWiFi;
        private string _currentMaskEthernet;
        private string _currentMaskWiFi;
        private string _currentRouterGatewayEthernet;
        private string _currentRouterGatewayWiFi;
        private int? _currentTcpPortEthernet;
        private int? _currentTcpPortWiFi;
        private int? _currentUdpPortEthernet;
        private int? _currentUdpPortWifi;
        private string _firmware;
        private string _homeId;
        private string _ipAP;
        private string _ipEthernet;
        private string _ipWiFi;
        private bool _isCloudEnabled;
        private bool _isCurrentUserOnBlacklist;
        private bool _isEnableBlackList;
        private bool _isEnableWifi;
        private string _localIP;
        private int _localPort;
        private string _macAddress;
        private string _macAddressEthernet;
        private string _macAddressWiFi;
        private string _manufacturerCode;
        private string _mask;
        private string _maskEthernet;
        private string _maskWiFi;
        private string _name;
        private string _password;
        private string _pin;
        private string _radioBuildFirmware;
        private string _radioVersionFirmware;
        private string _routerGateway;
        private string _routerGatewayEthernet;
        private string _routerGatewayWiFi;
        private int _selectedIndexIPTypeEthernet;
        private int _selectedIndexIPTypeWiFi;
        private int _selectedIndexTabControl = 0;
        private int _selectedIndexWiFiStatus;
        private TypeIPEnum _selectedTypeIPEthernet;
        private TypeIPEnum _selectedTypeIPWiFi;
        private string _ssid;
        private int _TCPPort;
        private int _tcpPortEthernet;
        private int _tcpPortWiFi;
        private int _UDPPort;
        private int _udpPortEthernet;
        private int _udpPortWifi;
        private string _uid;
        private int _totalRadioMemory;
        private int _totalRadioMemoryInUse;
        private bool _stateRelay;
        private string _apIP;
        private string _localIPWiFi;
        private bool _isEnabled = true;
        private string _apSSID;
        private string _apPassword;

        //private bool _isEnabledAP;
        private int _localPorTCP;

        private int _productId;
        private int _selectedIndexIPCommand = -1;
        private SerialModel _selectedSerialModel;
        private int _selectedBaudRates232Index;
        private int _selectedBaudRates485Index;
        private int _selectedParity232Index;
        private int _selectedParity485Index;
        private int _selectedIndexRTS = -1;
        private int _selectedIndexIRCloud;
        private bool _IsPrimary;
        private int _ModuleId;
        private DateTime? _ClockTime;
    }
}