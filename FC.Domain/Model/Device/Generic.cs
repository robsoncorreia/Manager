using FC.Domain.Model.Device;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FC.Domain.Model.Zwave
{
    public partial class GenericClass
    {
        [JsonProperty("key", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }

        [JsonProperty("name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("help", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Help { get; set; }

        [JsonProperty("read_only", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public bool? ReadOnly { get; set; }

        [JsonProperty("comment", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Comment { get; set; }

        [JsonProperty("spec_dev", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public IList<Specific> Specifics { get; set; }

        //[JsonProperty("spec_dev", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        //public SpecDevUnion? SpecDev { get; set; }
    }
}