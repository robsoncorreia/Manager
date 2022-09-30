using FC.Domain._Base;
using Newtonsoft.Json;
using Parse;
using System.Collections.Generic;

namespace FC.Domain.Model.User
{
    public class ParseUserCustom : ModelBase
    {
        public bool IsAdd
        {
            get => _isAdd;
            set
            {
                _isAdd = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsAdministratorAmbience { get; set; }
        public bool IsAdministratorProject { get; set; }

        public bool IsAmazonAssistant
        {
            get => _isAmazonAssistant;
            set
            {
                _isAmazonAssistant = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsGoogleAssistant
        {
            get => _isGoogleAssistant;
            set
            {
                _isGoogleAssistant = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public ParseUser ParseUser { get; set; }

        public override string ToString()
        {
            return ParseUser?.Username;
        }

        #region Voice Assistent

        [JsonProperty("assistants")]
        public IList<string> Assistants { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        public ParseObject ParseObject { get; set; }

        #endregion Voice Assistent

        private bool _isAdd;
        private bool _isAmazonAssistant;
        private bool _isEnable;
        private bool _isGoogleAssistant;
        private bool _isSelected;
    }
}