using FC.Domain._Base;
using Parse;

namespace FC.Domain.Model.FCC
{
    public enum CommandTypeVoiceAssistant
    {
        IR,
        Radio433,
        IPCommand,
        RTS
    }

    public class VoiceAssistantCommandModel : ModelBase
    {
        public ParseObject ParseObject { get; set; }
        public CommandTypeVoiceAssistant CommandTypeVoiceAssistant { get; set; }

        public int MemoryId
        {
            get => _memoryID;
            set
            {
                if (Equals(_memoryID, value))
                {
                    return;
                }
                _memoryID = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (Equals(_name, value))
                {
                    return;
                }
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                if (Equals(_type, value))
                {
                    return;
                }
                _type = value;
                NotifyPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"{Name}\n{Properties.Resources.Memory_Id} {MemoryId} | {Properties.Resources.Type} {Type}";
        }

        private int _memoryID;
        private string _name;
        private string _type;
    }
}