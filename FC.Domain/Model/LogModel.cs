using FC.Domain._Base;
using Newtonsoft.Json;

namespace FC.Domain.Model
{
    public enum LogEnum
    {
        Z_WaveDeviceNotFoundDatabase
    }

    public class LogModel : ModelBase
    {
        [JsonProperty("errorCode", NullValueHandling = NullValueHandling.Ignore)]
        public LogEnum ErrorCode { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("deviceName", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceName { get; set; }

        [JsonProperty("gateway", NullValueHandling = NullValueHandling.Ignore)]
        public GatewayModel Gateway { get; set; }

        [JsonProperty("manufacturerKey", NullValueHandling = NullValueHandling.Ignore)]
        public int ManufacturerKey { get; internal set; }

        [JsonProperty("productKey", NullValueHandling = NullValueHandling.Ignore)]
        public int ProductKey { get; internal set; }

        [JsonProperty("firmwareVersion", NullValueHandling = NullValueHandling.Ignore)]
        public int FirmwareVersion { get; internal set; }

        [JsonProperty("customId", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomId { get; internal set; }
    }
}