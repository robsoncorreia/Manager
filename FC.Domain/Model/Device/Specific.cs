using Newtonsoft.Json;

namespace FC.Domain.Model.Device
{
    public class Specific
    {
        [JsonProperty("key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("help", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Help { get; set; }

        [JsonProperty("comment", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }
    }
}