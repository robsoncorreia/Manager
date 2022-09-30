using FC.Domain._Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FC.Domain.Model.ZXT600
{
    public class ZXTCodeModel : ModelBase
    {
        [JsonProperty(CODES, NullValueHandling = NullValueHandling.Ignore)]
        public List<int> Codes { get; set; } = new List<int>();

        [JsonProperty("createdAt", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        [JsonProperty(MODEL, NullValueHandling = NullValueHandling.Ignore)]
        public string Model
        {
            get => _model;
            set
            {
                _model = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("objectId", NullValueHandling = NullValueHandling.Ignore)]
        public string ObjectId { get; set; }

        [JsonIgnore]
        public int SelectedIndexCode
        {
            get => _SelectedIndexCode;
            set
            {
                _SelectedIndexCode = value;
                NotifyPropertyChanged();
            }
        }

        [JsonProperty("updatedAt", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? UpdatedAt { get; set; }

        public const string CLASS_NAME = "ZXTCode";
        public const string CODES = "codes";
        public const string MODEL = "model";
        private string _model;
        private int _SelectedIndexCode;

        public override string ToString()
        {
            return Model;
        }
    }
}