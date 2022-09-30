using FC.Domain._Base;
using Newtonsoft.Json;

namespace FC.Domain.Model.RelayTest
{
    public class RelayTestModel : ModelBase
    {
        [JsonProperty(UtilRelayTest.IP, NullValueHandling = NullValueHandling.Ignore)]
        public string Ip
        {
            get => _ip;
            set
            {
                _ip = value;
                NotifyPropertyChanged();
            }
        }

        public string MV => "mV";
        public string MV2 => "mV2";

        [JsonProperty(UtilRelayTest.PORT, NullValueHandling = NullValueHandling.Ignore)]
        public int Port
        {
            get => _port;
            set
            {
                _port = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(UtilRelayTest.FIELD_VALUE_STATE_RELAY, NullValueHandling = NullValueHandling.Ignore)]
        public bool StateRelay
        {
            get => _stateRelay;
            set
            {
                _stateRelay = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(UtilRelayTest.FIELD_VALUE_STATE_RELAY, NullValueHandling = NullValueHandling.Ignore)]
        public int StateRelayRead
        {
            get => _stateRelayRead;
            set
            {
                _stateRelayRead = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(UtilRelayTest.FIELD_VALUE, NullValueHandling = NullValueHandling.Ignore)]
        public bool StateRelayValue
        {
            get => _stateRelayValue;
            set
            {
                _stateRelayValue = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(UtilRelayTest.FIELD_VALUE_TYPE, NullValueHandling = NullValueHandling.Ignore)]
        public int Time
        {
            get => _time;
            set
            {
                _time = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(UtilRelayTest.FIELD_VALUE_ANALOG_MILLIVOLTS, NullValueHandling = NullValueHandling.Ignore)]
        public string ValueAnalogMillivolts
        {
            get => _valueAnalogMillivolts;
            set
            {
                _valueAnalogMillivolts = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty(UtilRelayTest.FIELD_VALUE_RAW, NullValueHandling = NullValueHandling.Ignore)]
        public string ValueRaw
        {
            get => _valueRaw;
            set
            {
                _valueRaw = value;
                NotifyPropertyChanged();
            }
        }

        public RelayTestModel()
        {
        }

        private string _ip = "0.0.0.0";
        private int _port = 9999;
        private bool _stateRelay = false;

        // init false
        private int _stateRelayRead;

        // init false
        private bool _stateRelayValue = false;

        private int _time;
        private string _valueAnalogMillivolts;
        private string _valueRaw;
        // init false
    }
}