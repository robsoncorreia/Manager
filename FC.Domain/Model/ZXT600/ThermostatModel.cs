using Newtonsoft.Json;
using System;

namespace FC.Domain.Model.ZXT600
{
    public class ThermostatModel
    {
        public string Name { get; set; }

        [JsonIgnore]
        public Enum Code { get; set; }
    }
}