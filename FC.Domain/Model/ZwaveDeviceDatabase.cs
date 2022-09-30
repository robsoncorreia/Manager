using FC.Domain._Base;
using Newtonsoft.Json;
using Parse;
using System.Collections.Generic;

namespace FC.Domain.Model
{
    public class ZwaveDeviceDatabase : ModelBase
    {
        public string DefaultName { get; set; }
        public ParseObject ParseObject { get; set; }
        public ParseFile ImageParseFile { get; set; }
        public IList<EndpointDataBase> Endpoints { get; set; }
        public int GenericDeviceClass { get; set; }
        public IList<string> Cryptographys { get; set; }
        public int ManufacturerKey { get; set; }
        public int SpecificDeviceClass { get; set; }
        public int ProductKey { get; set; }

        public IList<AssociationGroupDataBase> AssociationGroups { get; set; }
        public IList<CommandClassDataBase> CommandClasses { get; set; }
        public int FirmwareVersion { get; set; }
    }

    public class AssociationGroupDataBase
    {
        [JsonProperty("commands", NullValueHandling = NullValueHandling.Ignore)]
        public IList<ZwaveCommand> Commands { get; set; }
    }

    public partial class EndpointDataBase
    {
        [JsonProperty("genericDeviceClass", NullValueHandling = NullValueHandling.Ignore)]
        public long? GenericDeviceClass { get; set; }

        [JsonProperty("specificDeviceClass", NullValueHandling = NullValueHandling.Ignore)]
        public long? SpecificDeviceClass { get; set; }

        [JsonProperty("commandClasses", NullValueHandling = NullValueHandling.Ignore)]
        public List<CommandClassDataBase> CommandClasses { get; set; }
    }

    public partial class CommandClassDataBase
    {
        [JsonProperty("commandClass", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommandClassCommandClass { get; set; }

        [JsonProperty("version", NullValueHandling = NullValueHandling.Ignore)]
        public long? Version { get; set; }
    }

    public partial class ZwaveCommand
    {
        [JsonProperty("commandClass", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommandClass { get; set; }

        [JsonProperty("command", NullValueHandling = NullValueHandling.Ignore)]
        public long? CommandCommand { get; set; }
    }
}