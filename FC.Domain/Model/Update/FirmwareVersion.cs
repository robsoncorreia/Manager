using FC.Domain._Base;
using Newtonsoft.Json;
using Parse;
using System;
using System.Collections.Generic;

namespace FC.Domain.Model.Update
{
    public class FirmwareVersion : ModelBase
    {
        public string Address
        {
            get => _address;
            set
            {
                if (_address != value)
                {
                    _address = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(ATTRIBUTES, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Attributes { get; set; }

        [JsonProperty(BUILD_NUMBER, NullValueHandling = NullValueHandling.Ignore)]
        public int BuildNumber { get; set; }

        [JsonProperty(CREATED_AT, NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty(GATEWAY_TYPE, NullValueHandling = NullValueHandling.Ignore)]
        public string GatewayType { get; set; }

        [JsonProperty(OBJECT_ID, NullValueHandling = NullValueHandling.Ignore)]
        public string ObjectId { get; set; }

        [JsonProperty(PATH, NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonProperty(UPDATED_AT, NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty(VERSION, NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        public ParseObject ParseObject { get; set; }

        public const string ADDRESS = "18.219.240.204";
        public const string ATTRIBUTES = "attributes";
        public const string BUILD_NUMBER = "buildNumber";
        public const string CLASSNAME = "FirmwareVersions";
        public const string CREATED_AT = "createdAt";
        public const string GATEWAY_TYPE = "gatewayType";
        public const string OBJECT_ID = "objectId";
        public const string PATH = "path";
        public const int PORT = 80;
        public const string UPDATED_AT = "updatedAt";
        public const string VERSION = "version";
        private string _address = ADDRESS;
        private int _port = PORT;
    }
}