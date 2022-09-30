namespace FC.Domain.Repository.Util
{
    public static class UtilRF
    {
        public const string GETALLMEMORYTYPECOMMAND = "{\"command\":\"radioGetAllMemoryType\",\"type\":0}";
        public const string GETRADIOFIRMWAREINFOCOMMAND = "{\"command\":\"getRadioFirmwareInfo\", \"type\":0}";
        public const string GETCHECKALLRADIOMEMORYCOMMAND = "{\"command\":\"checkAllRadioMemory\",\"type\":0}";
        public const string ERASERADIOMEMORY = "eraseRadioMemory";
        public const string GETRADIOFIRMWAREINFO = "getRadioFirmwareInfo";
        public const string COMMAND = "command";
        public const string BITFIELD = "bitfield";
        public const string BUILD = "build";
        public const string DESCRIPTION = "description";
        public const string ERROR = "error";
        public const string ID = "id";
        public const string IPUPDATERADIO433 = "18.219.240.204";
        public const string POWER = "power";
        public const string RADIO = "radio";
        public const string REMOTEID = "remoteId";
        public const string REPETITION = "repetition";
        public const string ROLLINGCODE = "rollingCode";
        public const string RTS = "rts";
        public const string RTSERRORNOTUSED = "not used";
        public const string TOTAL = "total";
        public const string TYPE = "type";
        public const string VERSION = "version";
        public const string RADIOTYPE = "radioType";
        public const string FREQUENCY = "frequency";
        public const string ADDRESS = "address";
        public const string PORT = "port";
        public const string FILENAME = "filename";
        public const int LEARNTIMEOUT = 32000;
    }
}