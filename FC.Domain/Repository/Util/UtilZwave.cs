namespace FC.Domain.Repository.Zwave
{
    public static class UtilZwave
    {
        public static string STATE = "state";
        public static string TIMEOUT = "timeout";
        public static string ERROR = "error";
        public static string COMMAND = "command";
        public static string ZWAVEREPLY = "zwaveReply";
        public static string DATA = "data";
        public static string FINISH = "finish";
        public static string MODULEID = "moduleId";
        public static string GROUP = "group";
        public static string PARAMETER = "parameter";
        public static string MANUFACTURER = "manufacturer";
        public static string VERSION = "version";
        public static string MODEL = "model";
        public static string SIZE = "size";
        public static string VALUE = "value";
        public static string STARTED = "started";
        public static string OK = "OK";
        public static string LEVEL = "level";
        public static string BATTERYSTATUS = "batteryStatus";
        public static string NODEFOUND = "node found";
        public static string ADDZWAVEDEVICE = "addZwaveDevice";
        public static int TIMEOUTREMOVEZWAVEDEVICE = 25000;
        public static int TIMEOUTINCLUDEZWAVEDEVICE = 21000;

        #region COMMANDS

        internal static string REMOVEZWAVEDEVICECOM = "{\"command\":\"removeZwaveDevice\",\"type\":0}";

        internal static string ADDZWAVEDEVICECOM = "{\"command\":\"addZwaveDevice\",\"type\":0}";

        internal static string LEARNZWAVENETWORKCOM = "{\"command\":\"learnZwaveNetwork\",\"type\":0}";
        internal static string LEARNZWAVENETWORK = "learnZwaveNetwork";

        internal static string RESTARTZWAVECHIPCOM = "{\"command\":\"restartZwaveChip\",\"type\":0}";

        internal static string RESETZWAVENETWORKCOM = "{\"command\":\"resetZwaveNetwork\",\"type\":0}";
        internal static string RESETZWAVENETWORK = "resetZwaveNetwork";

        internal static string GETDEVICEMANUFACTURER = "getDeviceManufacturer";

        internal static string ASSOCIATIONNUMBEROFGROUPS = "associationNumberofGroups";

        internal static string ASSOCIATIONGET = "associationGet";

        internal static string ASSOCIATIONREMOVE = "associationRemove";

        internal static string ASSOCIATIONSET = "associationSet";

        internal static string GETZWAVECONFIG = "getZwaveConfig";

        internal static string TYPE = "type";

        internal static string THERMOSTATMODESET = "thermostatModeSet";

        internal static string THERMOSTATFANMODESET = "thermostatFanModeSet";

        internal static string SETZWAVECONFIG = "setZwaveConfig";

        internal static string THERMOSTATTEMPERATURESET = "thermostatTemperatureSet";

        internal static string MULTILEVELSENSORMEASURE = "multiLevelSensorMeasure";

        #endregion COMMANDS
    }
}