using FC.Domain._Base;
using FC.Domain._Util;
using FC.Domain.Model.FlexCloudClone;
using FC.Domain.Model.ZXT600;
using FC.Domain.Repository.Util;
using Newtonsoft.Json;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FC.Domain.Model.Device
{
    public enum TabScheduleEnum
    {
        Timer,
        DateTime
    }

    public enum AirflowScaleEnum
    {
        M3_h,
        CFM
    }

    public enum AutoReportCondition
    {
        DisableAutoReport = 0,
        _1_F_0_5_C = 1,
        _2_F_1_C = 2,
        _3_F_1_5_C = 3,
        _4_F_2_C = 4,
        _5_F_2_5_C = 5,
        _6_F_3_C = 6,
        _7_F_3_5_C = 7,
        _8_F_4_C = 8
    }

    public enum AutoReportConditionTimeInterval
    {
        DisableAutoReport = 0,
        _1_H = 1,
        _2_H = 2,
        _3_H = 3,
        _4_H = 4,
        _5_H = 5,
        _6_H = 6,
        _7_H = 7,
        _8_H = 8
    }

    public enum C02LeveScaleEnum
    {
        Ppm
    }

    public enum CommandClassesEnum
    {
        COMMAND_CLASS_ALARM = 0x71,
        COMMAND_CLASS_APPLICATION_STATUS = 0x22,
        COMMAND_CLASS_ASSOCIATION_COMMAND_CONFIGURATION = 0x9B,
        COMMAND_CLASS_ASSOCIATION = 0x85,
        COMMAND_CLASS_AV_CONTENT_DIRECTORY_MD = 0x95,
        COMMAND_CLASS_AV_CONTENT_SEARCH_MD = 0x97,
        COMMAND_CLASS_AV_RENDERER_STATUS = 0x96,
        COMMAND_CLASS_AV_TAGGING_MD = 0x99,
        COMMAND_CLASS_BASIC_TARIFF_INFO = 0x36,
        COMMAND_CLASS_BASIC_WINDOW_COVERING = 0x50,
        COMMAND_CLASS_BASIC = 0x20,
        COMMAND_CLASS_BATTERY = 0x80,
        COMMAND_CLASS_CHIMNEY_FAN = 0x2A,
        COMMAND_CLASS_CLIMATE_CONTROL_SCHEDULE = 0x46,
        COMMAND_CLASS_CLOCK = 0x81,
        COMMAND_CLASS_CONFIGURATION = 0x70,
        COMMAND_CLASS_CONTROLLER_REPLICATION = 0x21,
        COMMAND_CLASS_CRC_16_ENCAP = 0x56,
        COMMAND_CLASS_DCP_CONFIG = 0x3A,
        COMMAND_CLASS_DCP_MONITOR = 0x3B,
        COMMAND_CLASS_DOOR_LOCK_LOGGING = 0x4C,
        COMMAND_CLASS_DOOR_LOCK = 0x62,
        COMMAND_CLASS_ENERGY_PRODUCTION = 0x90,
        COMMAND_CLASS_FIRMWARE_UPDATE_MD = 0x7A,
        COMMAND_CLASS_GEOGRAPHIC_LOCATION = 0x8C,
        COMMAND_CLASS_GROUPING_NAME = 0x7B,
        COMMAND_CLASS_HAIL = 0x82,
        COMMAND_CLASS_HRV_CONTROL = 0x39,
        COMMAND_CLASS_HRV_STATUS = 0x37,
        COMMAND_CLASS_INDICATOR = 0x87,
        COMMAND_CLASS_IP_CONFIGURATION = 0x9A,
        COMMAND_CLASS_LANGUAGE = 0x89,
        COMMAND_CLASS_LOCK = 0x76,
        COMMAND_CLASS_MANUFACTURER_PROPRIETARY = 0x91,
        COMMAND_CLASS_MANUFACTURER_SPECIFIC = 0x72,
        COMMAND_CLASS_MARK = 0xEF,
        COMMAND_CLASS_METER_PULSE = 0x35,
        COMMAND_CLASS_METER_TBL_CONFIG = 0x3C,
        COMMAND_CLASS_METER_TBL_MONITOR = 0x3D,
        COMMAND_CLASS_METER_TBL_PUSH = 0x3E,
        COMMAND_CLASS_METER = 0x32,
        COMMAND_CLASS_MTP_WINDOW_COVERING = 0x51,
        COMMAND_CLASS_MULTI_CHANNEL_ASSOCIATION = 0x8E,
        COMMAND_CLASS_MULTI_CHANNEL = 0x60,
        COMMAND_CLASS_MULTI_CMD = 0x8F,
        COMMAND_CLASS_NETWORK_MANAGEMENT_PROXY = 0x52,
        COMMAND_CLASS_NETWORK_MANAGEMENT_BASIC = 0x4D,
        COMMAND_CLASS_NETWORK_MANAGEMENT_INCLUSION = 0x34,
        COMMAND_CLASS_NO_OPERATION = 0x00,
        COMMAND_CLASS_NODE_NAMING = 0x77,
        COMMAND_CLASS_NON_INTEROPERABLE = 0xF0,
        COMMAND_CLASS_POWERLEVEL = 0x73,
        COMMAND_CLASS_PREPAYMENT_ENCAPSULATION = 0x41,
        COMMAND_CLASS_PREPAYMENT = 0x3F,
        COMMAND_CLASS_PROPRIETARY = 0x88,
        COMMAND_CLASS_PROTECTION = 0x75,
        COMMAND_CLASS_RATE_TBL_CONFIG = 0x48,
        COMMAND_CLASS_RATE_TBL_MONITOR = 0x49,
        COMMAND_CLASS_REMOTE_ASSOCIATION_ACTIVATE = 0x7C,
        COMMAND_CLASS_REMOTE_ASSOCIATION = 0x7D,
        COMMAND_CLASS_SCENE_ACTIVATION = 0x2B,
        COMMAND_CLASS_SCENE_ACTUATOR_CONF = 0x2C,
        COMMAND_CLASS_SCENE_CONTROLLER_CONF = 0x2D,
        COMMAND_CLASS_SCHEDULE_ENTRY_LOCK = 0x4E,
        COMMAND_CLASS_SCREEN_ATTRIBUTES = 0x93,
        COMMAND_CLASS_SCREEN_MD = 0x92,
        COMMAND_CLASS_SECURITY_PANEL_MODE = 0x24,
        COMMAND_CLASS_SECURITY_PANEL_ZONE_SENSOR = 0x2F,
        COMMAND_CLASS_SECURITY_PANEL_ZONE = 0x2E,
        COMMAND_CLASS_SECURITY = 0x98,
        COMMAND_CLASS_SENSOR_ALARM = 0x9C,
        COMMAND_CLASS_SENSOR_BINARY = 0x30,
        COMMAND_CLASS_SENSOR_CONFIGURATION = 0x9E,
        COMMAND_CLASS_SENSOR_MULTILEVEL = 0x31,
        COMMAND_CLASS_SILENCE_ALARM = 0x9D,
        COMMAND_CLASS_SIMPLE_AV_CONTROL = 0x94,
        COMMAND_CLASS_SWITCH_ALL = 0x27,
        COMMAND_CLASS_SWITCH_BINARY = 0x25,
        COMMAND_CLASS_SWITCH_MULTILEVEL = 0x26,
        COMMAND_CLASS_SWITCH_TOGGLE_BINARY = 0x28,
        COMMAND_CLASS_SWITCH_TOGGLE_MULTILEVEL = 0x29,
        COMMAND_CLASS_TARIFF_CONFIG = 0x4A,
        COMMAND_CLASS_TARIFF_TBL_MONITOR = 0x4B,
        COMMAND_CLASS_THERMOSTAT_FAN_MODE = 0x44,
        COMMAND_CLASS_THERMOSTAT_FAN_STATE = 0x45,
        COMMAND_CLASS_THERMOSTAT_HEATING = 0x38,
        COMMAND_CLASS_THERMOSTAT_MODE = 0x40,
        COMMAND_CLASS_THERMOSTAT_OPERATING_STATE = 0x42,
        COMMAND_CLASS_THERMOSTAT_SETBACK = 0x47,
        COMMAND_CLASS_THERMOSTAT_SETPOINT = 0x43,
        COMMAND_CLASS_TIME_PARAMETERS = 0x8B,
        COMMAND_CLASS_TIME = 0x8A,
        COMMAND_CLASS_TRANSPORT_SERVICE = 0x55,
        COMMAND_CLASS_USER_CODE = 0x63,
        COMMAND_CLASS_VERSION = 0x86,
        COMMAND_CLASS_WAKE_UP = 0x84,
        COMMAND_CLASS_ZENSOR_NET = 0x02,
        COMMAND_CLASS_ZIP_6LOWPAN = 0x4F,
        COMMAND_CLASS_ZIP = 0x23,
        ZWAVE_CMD_CLASS = 0x01,
        COMMAND_CLASS_APPLICATION_CAPABILITY = 0x57,
        COMMAND_CLASS_SWITCH_COLOR = 0x33,
        COMMAND_CLASS_SCHEDULE = 0x53,
        COMMAND_CLASS_NETWORK_MANAGEMENT_PRIMARY = 0x54,
        COMMAND_CLASS_ZIP_ND = 0x58,
        COMMAND_CLASS_ASSOCIATION_GRP_INFO = 0x59,
        COMMAND_CLASS_DEVICE_RESET_LOCALLY = 0x5A,
        COMMAND_CLASS_CENTRAL_SCENE = 0x5B,
        COMMAND_CLASS_IP_ASSOCIATION = 0x5C,
        COMMAND_CLASS_ANTITHEFT = 0x5D,
        COMMAND_CLASS_ZWAVEPLUS_INFO = 0x5E,
        COMMAND_CLASS_ZIP_GATEWAY = 0x5F,
        COMMAND_CLASS_ZIP_PORTAL = 0x61,
        COMMAND_CLASS_DMX = 0x65,
        COMMAND_CLASS_BARRIER_OPERATOR = 0x66,
        COMMAND_CLASS_NETWORK_MANAGEMENT_INSTALLATION_MAINTENANCE = 0x67,
        COMMAND_CLASS_ZIP_NAMING = 0x68,
        COMMAND_CLASS_MAILBOX = 0x69,
        COMMAND_CLASS_WINDOW_COVERING = 0x6A,
        COMMAND_CLASS_SECURITY_2 = 0x9F,
        COMMAND_CLASS_IRRIGATION = 0x6B,
        COMMAND_CLASS_SUPERVISION = 0x6C,
        COMMAND_CLASS_HUMIDITY_CONTROL_SETPOINT = 0x64,
        COMMAND_CLASS_HUMIDITY_CONTROL_MODE = 0x6D,
        COMMAND_CLASS_HUMIDITY_CONTROL_OPERATING_STATE = 0x6E,
        COMMAND_CLASS_ENTRY_CONTROL = 0x6F,
        COMMAND_CLASS_INCLUSION_CONTROLLER = 0x74,
        COMMAND_CLASS_NODE_PROVISIONING = 0x78,
        COMMAND_CLASS_SOUND_SWITCH = 0x79
    }

    public enum ConductivityScaleEnum
    {
        SM_1
    }

    public enum CurrentScaleEnum
    {
        A,
        MA,
    }

    public enum DateTypeEnum
    {
        [Description("Every Amount of Seconds")]
        EveryAmountSeconds = 0,

        [Description("Every Amount of Minutes")]
        EveryAmountMinutes = 1,

        [Description("Every Amount of Hours")]
        EveryAmountHours = 2,

        [Description("Every Amount of Days")]
        EveryAmountDays = 3,

        [Description("Every Amount of Months")]
        EveryAmountMonths = 4,

        [Description("Every Amount of Years")]
        EveryAmountYears = 5,

        [Description("Compare Clock")]
        CompareClock = 6,

        [Description("Compare Day of the week")]
        CompareDayWeek = 10,
    }

    public enum DayPeriodEnum
    {
        Sunset,
        Sunrise
    }

    public enum DaysOfWeek
    {
        [Description("Sunday")]
        Sunday = 0,

        [Description("Monday")]
        Monday = 1,

        [Description("Tuesday")]
        Tuesday = 2,

        [Description("Wednesday")]
        Wednesday = 3,

        [Description("Thursday")]
        Thursday = 4,

        [Description("Friday")]
        Friday = 5,

        [Description("Saturday")]
        Saturday = 6,
    }

    public enum DewPointScaleEnum
    {
        Celsius,
        Fahrenheit
    }

    public enum DimensionlessScaleEnum
    {
        PercentageVolume,
        Lux
    }

    public enum DistanceScaleEnum
    {
        Meter_m,
        Centimeter_cm,
        Feet_ft
    }

    public enum ElectricResistanceScaleEnum
    {
        Ohm
    }

    public enum EndpointState
    {
        Off,
        On,
        Toggle
    }

    public enum EndpointType
    {
        OnOff,
        Dimmer,
        NA
    }

    public enum GeneralPurposeScaleEnum
    {
        PercentageVolume,
        Dimensionless
    }

    public enum GroundTemperatureScaleEnum
    {
        Celsius_C,
        Fahrenh_F
    }

    public enum Humidity2ScaleEnum
    {
        PercentageValue,
        WaterVolume,
        Impedance,
        WaterActivity
    }

    public enum HumidityScaleEnum
    {
        PercentageVolume,
        Absolute
    }

    public enum IfthenType
    {
        Device,
        IR,
        Radio433,
        RTS,
        Schedule,
        IPCommand,
        Relay,
        Serial,
        Default
    }

    public enum InstructionType
    {
        If,
        Then,
        Else
    }

    public enum LogicGateIfThen
    {
        Disabled = 0,
        And = 1,
        Or = 2,
        Xor = 3,
        Nand = 4,
        Nor = 5,
        Xnor = 6
    }

    public enum OperatorsTypeSchedule
    {
        NA = 0,
        LessThan = 1,
        LessOrEquals = 2,
        Equals = 3,
        Different = 4,
        GreaterOrEquals = 5,
        GreaterThan = 6
    }

    public enum OperatorTypeIfThen
    {
        [Description("Less than")]
        LessThan = 1,

        [Description("Less than or equal")]
        LessThanOrEqual = 2,

        [Description("Equal")]
        Equal = 3,

        [Description("Not equal")]
        NotEqual = 4,

        [Description("Greater than or equal")]
        GreaterThanOrEqual = 5,

        [Description("Greater than")]
        GreaterThan = 6
    }

    public enum OperatorTypeITZS
    {
        Disabled = 0,
        LessThan = 1,
        LessOrEquals = 2,
        Equals = 3,
        Different = 4,
        GreaterOrEquals = 5,
        GreaterThan = 6,
        IgnoreIfDifferent = 7
    }

    public enum OrderByDirection
    {
        Ascending,
        Descending
    }

    public enum OrderByZwaveDevice
    {
        NodeId,
        Name
    }

    public enum PositionAngleScaleEnum
    {
        PercentageValue,
        DegreeNorth,
        DegreeSouth
    }

    public enum PowerScaleEnum
    {
        Watt,
        BTU_h
    }

    public enum PrecipitationIndexScaleEnum
    {
        MM_H,
        In_H
    }

    public enum PressureATMScaleEnum
    {
        Kpa,
        InchMercury
    }

    public enum RotationScaleEnum
    {
        RPM,
        Hertz
    }

    public enum SeismicIntensityScaleEnum
    {
        Mercalli,
        Macroseismic,
        Liedu,
        Shindo
    }

    public enum SensorType
    {
        Motion,
        Light
    }

    public enum SensorTypeEnum
    {
        Reserved = 0,
        Temp = 1,
        GeneralPurpose = 2,
        Luminescence = 3,
        Power = 4,
        Humidity = 5,
        Speed = 6,
        Direction = 7,
        AtmosphericPressure = 8,
        BarometricPressure = 9,
        SolarRadiation = 10,
        DewPoint = 11,
        PrecipitationIndex = 12,
        TideLevel = 13,
        Weight = 14,
        Voltage = 15,
        Current = 16,
        CO2Level = 17,
        Airflow = 18,
        Volume = 19,
        Distance = 20,
        Position_Angle_ = 21,
        Rotation = 22,
        WaterTemperature = 23,
        GroundTemperature = 24,
        SeismicIntensity = 25,
        UltravioletIntensity = 26,
        ElectricResistance = 27,
        Conductivity = 28,
        Volume2 = 29,
        Humidity2 = 30
    }

    public enum SolarRadiationScaleEnum
    {
        W_M2
    }

    public enum SpeedScaleEnum
    {
        M_s,
        MPH
    }

    public enum TemperatureOffsetValueEnum
    {
        _0_C_Default = 0,
        _1_C = 1,
        _2_C = 2,
        _3_C = 3,
        _4_C = 4,
        _5_C = 5,
        _1_C_Negative = 255,
        _2_C_Negative = 254,
        _3_C_Negative = 253,
        _4_C_Negative = 252,
        _5_C_Negative = 251
    }

    public enum TemperatureScaleEnum
    {
        Celsius,
        Fahrenheit
    }

    public enum ThermostatFanEnum
    {
        LowAuto = 0,
        LowManual = 1,
        HighAuto = 2,
        HighManual = 3,
        MediumAuto = 4,
        MediumManual = 5,
        Circulation = 6,
        Humidification = 7
    }

    public enum ThermostatModeEnum
    {
        TurnOff = 0x00,
        Heat = 0x01,
        Cool = 0x02,
        Auto = 0x03,
        Aux = 0x04,
        Resume = 0x05,
        Fan = 0x06,
        Furnace = 0x07,
        Dehumidifier = 0x08,
        Humidifier = 0x09,

        //todo verificar
        AutoB = 0x0A,

        EnergySaveHeat = 0x0B,
        EnergySaveCool = 0x0C,
        Away = 0x0D
    }

    public enum TideLeveScaleEnum
    {
        Meter_M,
        Feet_F,
    }

    public enum UltravioletIntensityScaleEnum
    {
        UVIndex
    }

    public enum VoltageScaleEnum
    {
        Volt,
        MiliVolt
    }

    public enum VolumeAudioScaleEnum
    {
        DB,
        DBA
    }

    public enum VolumeScaleEnum
    {
        Liter_l,
        CFM,
        Gallon,
    }

    public enum WaterTemperatureScaleEnum
    {
        Celsius_C,
        Fahrenh_F
    }

    public enum WeightScaleEnum
    {
        Kg
    }

    public enum ZWaveComponents
    {
        Slave,
        Controller
    }

    public enum ZWaveDeviceType
    {
        NA,
        Gateway,
        Sensor,
        Panel,
        MicroModule,
        Generic
    }

    public class AssociationGroup : ModelBase
    {
        private int _MaxRegister;

        public AssociationGroup()
        {
            ZwaveDevices = new ObservableCollection<ZwaveDevice>();
        }

        //zwave: [class,command]
        [JsonProperty("zwave", NullValueHandling = NullValueHandling.Ignore)]
        public IList<int> ClassesCommands { get; set; }

        [JsonProperty("commands", NullValueHandling = NullValueHandling.Ignore)]
        public IList<ZwaveCommand> Commands { get; set; }

        //association moduleId
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public IList<int> Data { get; set; }

        //number of group
        [JsonProperty("group", NullValueHandling = NullValueHandling.Ignore)]
        public int Group { get; set; }

        [JsonProperty("groupId", NullValueHandling = NullValueHandling.Ignore)]
        public int GroupId { get; set; }

        [JsonProperty("maxRegister", NullValueHandling = NullValueHandling.Ignore)]
        public int MaxRegister
        {
            get => _MaxRegister;
            set
            {
                _MaxRegister = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("moduleId", NullValueHandling = NullValueHandling.Ignore)]
        public int ModuleId { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = "Name";

        [JsonProperty("random", NullValueHandling = NullValueHandling.Ignore)]
        public string Random { get; set; }

        [JsonProperty("zwaveDevices", NullValueHandling = NullValueHandling.Ignore)]
        public ObservableCollection<ZwaveDevice> ZwaveDevices { get; set; }
    }

    public partial class BasicDeviceClass
    {
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public partial class CommandClass
    {
        [JsonProperty("commandClass", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommandClassId { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public long? Version { get; set; }

        public override string ToString()
        {
            return $"{(CommandClassesEnum)Convert.ToByte(CommandClassId)}\n{Properties.Resources.Version} {Version}";
        }
    }

    public class DaysOfWeekModel : ModelBase
    {
        private bool _IsSelected;

        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                _IsSelected = value;
                NotifyPropertyChanged();
            }
        }

        public string Name { get; set; }
    }

    public partial class Endpoint : ModelBase
    {
        private long _EconomicMode;
        private int _EndpointStateIndex;
        private bool _isOn;
        private bool _IsOnOff;
        private bool _isSelected;
        private long _multiLevel;

        [JsonProperty("channel", NullValueHandling = NullValueHandling.Ignore)]
        public int Channel { get; set; }

        [JsonProperty("commandClasses", NullValueHandling = NullValueHandling.Ignore)]
        public ObservableCollection<CommandClass> CommandClasses { get; set; }

        [JsonProperty("economicMode", NullValueHandling = NullValueHandling.Ignore)]
        public long EconomicMode
        {
            get => _EconomicMode;
            set
            {
                _EconomicMode = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("endpointStateIndex", NullValueHandling = NullValueHandling.Ignore)]
        public int EndpointStateIndex
        {
            get => _EndpointStateIndex;
            set
            {
                _EndpointStateIndex = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("genericDeviceClass", NullValueHandling = NullValueHandling.Ignore)]
        public long? GenericDeviceClass { get; set; }

        [JsonProperty("onOff", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("isOnOff", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsOnOff
        {
            get => _IsOnOff;
            set
            {
                if (Equals(_IsOnOff, value))
                {
                    return;
                }
                _IsOnOff = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("isSelected", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("multiLevel", NullValueHandling = NullValueHandling.Ignore)]
        public long MultiLevel
        {
            get => _multiLevel;
            set
            {
                _multiLevel = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("specificDeviceClass", NullValueHandling = NullValueHandling.Ignore)]
        public long? SpecificDeviceClass { get; set; }
    }

    public partial class GenericDeviceClass
    {
        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("spec_dev", NullValueHandling = NullValueHandling.Ignore)]
        public IList<SpecificDeviceClass> SpecificDeviceClasses { get; set; }
    }

    public partial class Scene : ModelBase
    {
        private bool _isOn;

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (Equals(_isOn, value))
                {
                    return;
                }
                _isOn = value;
                NotifyPropertyChanged();
            }
        }

        public int Number { get; set; }
    }

    public partial class SpecificDeviceClass
    {
        [JsonProperty("comment", NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        [JsonProperty("key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("flagmask", NullValueHandling = NullValueHandling.Ignore)]
        public string Mask { get; set; }

        [JsonProperty("flagname", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
    }

    public partial class ZwaveCommand
    {
        [JsonProperty("command", NullValueHandling = NullValueHandling.Ignore)]
        public long? Command { get; set; }

        [JsonProperty("commandClass", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommandClass { get; set; }
    }

    public class ZwaveDevice : ModelBase, IDataErrorInfo
    {
        private int? _delayIfThen = 0;
        private string _gatewayFunctionName;
        private bool _isChangedDevice;
        private bool _isEnabledAssociation;
        private bool _IsGateway;
        private bool _isHiddenDelay;
        private bool _IsHiddenDelete;
        private bool _isHiddenLogicGateIfThen;
        private bool _IsNew;
        private bool _isOn;
        private int _ModuleId = -1;
        private long _multiLevel;
        private string _name;
        private int _numberOfAssociationGroups;
        private int _parameter;
        private int _SelectedIndexDateType;
        private int _SelectedIndexDayPeriod;
        private int _StateIndex;
        private int _selectedIndexEndpoint;
        private int _selectedIndexLogicGateIfThen = 2;
        private int _selectedIndexOperatorType;
        private int _selectedIndexRadio433;
        private int? _selectedIndexScene;
        private int _selectedIndexTabControl;
        private int _SelectedIndexTabSchedule;
        private int _selectedTabIndexIfThen;
        private int _selectedTabIndexSensorType;
        private string _SuffixText;
        private int _value;
        private long _ValueSchedule;

        private int _selectedIndexThermostatMode;

        public int SelectedIndexThermostatMode
        {
            get => _selectedIndexThermostatMode;
            set
            {
                _selectedIndexThermostatMode = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexThermostatFunction
        {
            get => _selectedIndexThermostatFunction;
            set
            {
                _selectedIndexThermostatFunction = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexThermostatFan
        {
            get => _selectedIndexThermostatFan;
            set
            {
                _selectedIndexThermostatFan = value;
                NotifyPropertyChanged();
            }
        }

        public ZwaveDevice()
        {
            Associations = new ObservableCollection<AssociationGroup>();

            Delays = UtilIfThen.DELAYS;

            DaysOfWeekList = new ObservableCollection<DaysOfWeekModel>(new List<DaysOfWeekModel> {
                new DaysOfWeekModel{
                    Name= Properties.Resources.Sun_,
                },
                new DaysOfWeekModel{
                    Name= Properties.Resources.Mon_,
                },
                new DaysOfWeekModel{
                    Name= Properties.Resources.Tues_,
                },
                new DaysOfWeekModel{
                    Name= Properties.Resources.Wed_,
                },
                new DaysOfWeekModel{
                    Name= Properties.Resources.Thurs_,
                },
                new DaysOfWeekModel{
                    Name= Properties.Resources.Fri_,
                },
                new DaysOfWeekModel{
                    Name= Properties.Resources.Sat_,
                },
            });

            Scenes = new ObservableCollection<Scene>();

            ThermostatModes = new List<ThermostatModel>
            {
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Resume,
                    Code= ThermostatModeEnum.Resume
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Off,
                    Code= ThermostatModeEnum.TurnOff
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Auto,
                    Code= ThermostatModeEnum.Auto
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Heat,
                    Code= ThermostatModeEnum.Heat
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Fan,
                    Code= ThermostatModeEnum.Fan
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Furnace,
                    Code= ThermostatModeEnum.Furnace
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Cool,
                    Code= ThermostatModeEnum.Cool
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Dehumidifier,
                    Code= ThermostatModeEnum.Dehumidifier
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Humidifier,
                    Code= ThermostatModeEnum.Humidifier
                },
            };

            ThermostatFans = new List<ThermostatModel>
            {
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Auto_Low,
                    Code= ThermostatFanEnum.LowAuto,
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Manual_Low,
                    Code= ThermostatFanEnum.LowManual,
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Auto_High,
                    Code= ThermostatFanEnum.HighAuto,
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Manual_High,
                    Code= ThermostatFanEnum.HighManual,
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Auto_Medium,
                    Code= ThermostatFanEnum.HighManual,
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Manual_Medium,
                    Code= ThermostatFanEnum.MediumManual,
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Circulation,
                    Code= ThermostatFanEnum.Circulation,
                },
                new ThermostatModel
                {
                    Name = Domain.Properties.Resources.Humidification,
                    Code= ThermostatFanEnum.Humidification,
                },
            };
        }

        #region Data Error Info

        public string Error => null;

        public List<ThermostatModel> ThermostatModes { get; set; }

        public List<ThermostatModel> ThermostatFans { get; set; }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(RoomTemperature):
                        if (RoomTemperature is < 0 or > 400)
                        {
                            return "The age must be between 10 and 100";
                        }

                        break;
                }

                return string.Empty;
            }
        }

        #endregion Data Error Info

        private int _size;

        private int _RoomTemperature;
        private int _selectedIndexThermostatFan;
        private int _selectedIndexThermostatFunction;

        public ActionsRTSSomfy ActionsRTSSomfy { get; set; }
        public IList<AssociationGroup> AssociationGroups { get; set; }

        [JsonProperty("associationData", NullValueHandling = NullValueHandling.Ignore)]
        public ObservableCollection<AssociationGroup> Associations { get; set; }

        public string AssociationsSerialize { get; set; }

        [JsonProperty("basicDeviceClass", NullValueHandling = NullValueHandling.Ignore)]
        public int BasicDeviceClass { get; set; }

        [JsonProperty("commandClasses", NullValueHandling = NullValueHandling.Ignore)]
        public IList<int> CommandClass { get; set; } = new List<int>();

        public IList<CommandClass> CommandClasses { get; set; } = new List<CommandClass>();
        public IList<string> Cryptographys { get; set; }
        public string CustomId => $"{ProductKey}{ManufacturerKey}{FirmwareVersion}";
        public ObservableCollection<DaysOfWeekModel> DaysOfWeekList { get; set; }
        public string DefaultName { get; set; }

        public int? DelayIfThen
        {
            get => _delayIfThen;
            set
            {
                if (Equals(_delayIfThen, value))
                {
                    return;
                }
                _delayIfThen = value;
                NotifyPropertyChanged();
            }
        }

        [JsonIgnore]
        public IList<int> Delays { get; }

        public IList<Endpoint> Endpoints { get; set; }
        public int FirmwareVersion { get; set; } = int.MaxValue;
        public int GatewayFunctionMemoryId { get; set; }

        public string GatewayFunctionName
        {
            get => _gatewayFunctionName;
            set
            {
                if (Equals(_gatewayFunctionName, value))
                {
                    return;
                }
                _gatewayFunctionName = value;
                NotifyPropertyChanged();
            }
        }

        public string GatewayFunctionUID { get; set; }
        public string GatewayName { get; set; }

        [JsonIgnore]
        public ParseObject GatewayParseObject { get; set; }

        [JsonProperty("genericDeviceClass", NullValueHandling = NullValueHandling.Ignore)]
        public int GenericDeviceClass { get; set; }

        public int GroupId { get; set; }
        public int Id { get; set; }
        public IfthenType IfthenType { get; set; }

        [JsonIgnore]
        public ParseFile ImageParseFile { get; set; }

        public int Index { get; internal set; }
        public InstructionType InstructionType { get; set; }

        public bool IsChangedDevice
        {
            get => _isChangedDevice;
            set
            {
                _isChangedDevice = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnabledAssociation
        {
            get => _isEnabledAssociation;
            set
            {
                _isEnabledAssociation = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEncrypted => Encrypted();

        public bool? IsFriendSettings { get; set; }

        public bool IsGateway
        {
            get
            {
                _IsGateway = CustomId == ZwaveModelUtil.FCZWS100 || CustomId == ZwaveModelUtil.FCZIR100311 || ZWaveComponents == ZWaveComponents.Controller;
                return _IsGateway;
            }
            set => _IsGateway = value;
        }

        public bool IsHiddenDelay
        {
            get => _isHiddenDelay;
            set
            {
                if (Equals(_isHiddenDelay, value))
                {
                    return;
                }
                _isHiddenDelay = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsHiddenDelete
        {
            get => _IsHiddenDelete;
            set
            {
                _IsHiddenDelete = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsHiddenLogicGateIfThen
        {
            get => _isHiddenLogicGateIfThen;
            set
            {
                if (Equals(_isHiddenLogicGateIfThen, value))
                {
                    return;
                }
                _isHiddenLogicGateIfThen = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsNew
        {
            get => _IsNew;
            set
            {
                _IsNew = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (Equals(_isOn, value))
                {
                    return;
                }
                _isOn = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSuportIf => UtilZwaveDevice.DEVICESSUPPORTIF.Contains($"{ProductKey}{ManufacturerKey}{FirmwareVersion}");

        public bool IsSuportThenElse => UtilZwaveDevice.DEVICESSUPPORTTHENELSE.Contains($"{ProductKey}{ManufacturerKey}{FirmwareVersion}");

        public bool IsTestable => IfthenType != IfthenType.Device || UtilZwaveDevice.DEVICESSUPPORTTEST.Contains($"{ProductKey}{ManufacturerKey}{FirmwareVersion}");

        [JsonProperty("length", NullValueHandling = NullValueHandling.Ignore)]
        public int Length { get; set; }

        [JsonProperty("manufacturer", NullValueHandling = NullValueHandling.Ignore)]
        public int Manufacturer { get; set; }

        public int ManufacturerKey { get; set; } = int.MaxValue;

        [JsonProperty("model", NullValueHandling = NullValueHandling.Ignore)]
        public int Model { get; set; }

        [JsonProperty("moduleId", NullValueHandling = NullValueHandling.Ignore)]
        public int ModuleId
        {
            get => _ModuleId;
            set
            {
                _ModuleId = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("stateIndex", NullValueHandling = NullValueHandling.Ignore)]
        public int StateIndex
        {
            get => _StateIndex;
            set
            {
                _StateIndex = value;
                NotifyPropertyChanged();
            }
        }

        public long MultiLevel
        {
            get => _multiLevel;
            set
            {
                if (Equals(_multiLevel, value))
                {
                    return;
                }
                _multiLevel = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name ?? DefaultName;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public int NumberOfAssociationGroups
        {
            get => _numberOfAssociationGroups;
            set
            {
                _numberOfAssociationGroups = value;

                NotifyPropertyChanged();

                if (_numberOfAssociationGroups == 0)
                {
                    return;
                }

                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    if (Associations.Any())
                    {
                        Associations.Clear();
                    }

                    for (int i = 0; i < _numberOfAssociationGroups; i++)
                    {
                        using AssociationGroup associationGroup = new()
                        {
                            Name = $"Group {i + 1}",
                            GroupId = i + 1
                        };

                        Associations.Add(associationGroup);
                    }
                });
            }
        }

        [JsonProperty("parameter", NullValueHandling = NullValueHandling.Ignore)]
        public int Parameter
        {
            get => _parameter;
            set
            {
                if (Equals(_parameter, value))
                {
                    return;
                }
                _parameter = value;
                NotifyPropertyChanged();
            }
        }

        [JsonIgnore]
        public ParseObject ParseObject { get; set; }

        public int ProductKey { get; set; } = int.MaxValue;

        public int RelayPulseTime { get; set; }

        public int? Scene { get; set; }

        public ObservableCollection<Scene> Scenes { get; set; }

        public DateTypeEnum SelectedDateType { get; set; }

        public DaysOfWeek SelectedDaysOfWeek { get; set; }

        public int SelectedIndexActionRTSSomfy { get; set; }

        public int SelectedIndexDateType
        {
            get => _SelectedIndexDateType;
            set
            {
                _SelectedIndexDateType = value;
                GetSuffixText();
                NotifyPropertyChanged();
            }
        }

        private void GetSuffixText()
        {
            SuffixText = _SelectedIndexDateType switch
            {
                0 => _ValueSchedule > 1 ? Properties.Resources.Seconds.ToLower() : Properties.Resources.Second.ToLower(),
                1 => _ValueSchedule > 1 ? Properties.Resources.Minutes.ToLower() : Properties.Resources.Minute.ToLower(),
                2 => _ValueSchedule > 1 ? Properties.Resources.Hours.ToLower() : Properties.Resources.Hour.ToLower(),
                3 => _ValueSchedule > 1 ? Properties.Resources.Days.ToLower() : Properties.Resources.Day.ToLower(),
                4 => _ValueSchedule > 1 ? Properties.Resources.Months.ToLower() : Properties.Resources.Month.ToLower(),
                5 => _ValueSchedule > 1 ? Properties.Resources.Years.ToLower() : Properties.Resources.Year.ToLower(),
                _ => string.Empty,
            };
        }

        public int SelectedIndexDayPeriod
        {
            get => _SelectedIndexDayPeriod;
            set
            {
                _SelectedIndexDayPeriod = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexDaysOfWeek { get; set; }

        public int SelectedIndexEndpoint
        {
            get => _selectedIndexEndpoint;
            set
            {
                if (Equals(_selectedIndexEndpoint, value))
                {
                    return;
                }
                _selectedIndexEndpoint = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexIR { get; set; }

        public int SelectedIndexLogicGateIfThen
        {
            get => _selectedIndexLogicGateIfThen;
            set
            {
                if (Equals(_selectedIndexLogicGateIfThen, value))
                {
                    return;
                }
                _selectedIndexLogicGateIfThen = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexOperatorsType { get; set; }

        public int SelectedIndexOperatorType
        {
            get => _selectedIndexOperatorType;
            set
            {
                if (Equals(_selectedIndexOperatorType, value))
                {
                    return;
                }
                _selectedIndexOperatorType = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexRadio433
        {
            get => _selectedIndexRadio433;
            set
            {
                if (Equals(_selectedIndexRadio433, value))
                {
                    return;
                }
                _selectedIndexRadio433 = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexRelayStateMode { get; set; }

        public int SelectedIndexRTS { get; set; }

        public int? SelectedIndexScene
        {
            get => _selectedIndexScene;
            set
            {
                _selectedIndexScene = value;
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

        public int SelectedIndexTabSchedule
        {
            get => _SelectedIndexTabSchedule;
            set
            {
                _SelectedIndexTabSchedule = value;
                NotifyPropertyChanged();
            }
        }

        public OperatorsTypeSchedule SelectedOperatorsTypeSchedule { get; set; }

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

        public int SelectedTabIndexSensorType
        {
            get => _selectedTabIndexSensorType;
            set
            {
                if (Equals(_selectedTabIndexSensorType, value))
                {
                    return;
                }
                _selectedTabIndexSensorType = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public int Size
        {
            get => _size;
            set
            {
                if (Equals(_size, value))
                {
                    return;
                }
                _size = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("specificDevice", NullValueHandling = NullValueHandling.Ignore)]
        public int SpecificDeviceClass { get; set; }

        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

        public bool StateRelay { get; set; }

        public string SuffixText
        {
            get => _SuffixText;
            set
            {
                _SuffixText = value;
                NotifyPropertyChanged();
            }
        }

        public DateTime TimePickerValue { get; set; } = DateTime.Now;

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public int Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value))
                {
                    return;
                }
                _value = value;
                NotifyPropertyChanged();
            }
        }

        public long ValueSchedule
        {
            get => _ValueSchedule;
            set
            {
                _ValueSchedule = value;
                GetSuffixText();
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public int Version { get; set; }

        public ZWaveComponents ZWaveComponents { get; set; } = ZWaveComponents.Slave;

        public ZWaveDeviceType ZWaveDeviceType { get; set; }

        public int RoomTemperature
        {
            get => _RoomTemperature;
            set
            {
                _RoomTemperature = value;
                if (value < 0)
                {
                    _RoomTemperature = 0;
                }
                if (value > 400)
                {
                    _RoomTemperature = 400;
                }
                NotifyPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"{DefaultName}";
        }

        //todo verificar funcionamento
        private bool Encrypted()
        {
            if (CommandClasses.Any())
            {
                IEnumerable<CommandClass> list = CommandClasses.Where(x => x.CommandClassId is 152 or 159);
                return list.Any();
            }
            if (CommandClass.Any())
            {
                IEnumerable<int> list = CommandClass.Where(x => x is 152 or 159);
                return list.Any();
            }
            return false;
        }
    }
}