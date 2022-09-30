using FC.Domain._Base;
using FC.Domain.Model.Device;
using Parse;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FC.Domain.Model.IfThen
{
    public class IfThenModel : ModelBase
    {
        public ParseObject ParseObject
        {
            get => _ParseObject;
            set
            {
                _ParseObject = value;
                NotifyPropertyChanged();
            }
        }

        public IfthenType IfthenType { get; set; } = IfthenType.Default;

        public ObservableCollection<ZwaveDevice> ZwaveDevicesIf { get; set; }
        public ObservableCollection<ZwaveDevice> ZwaveDevicesThen { get; set; }
        public ObservableCollection<ZwaveDevice> ZwaveDevicesElse { get; set; }

        public int MacroIdThen
        {
            get => _macroIdThen;
            set
            {
                if (Equals(_macroIdThen, value))
                {
                    return;
                }
                _macroIdThen = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _IsEnabled;
            set
            {
                _IsEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public int MacroIdElse
        {
            get => _macroIdElse;
            set
            {
                if (Equals(_macroIdElse, value))
                {
                    return;
                }
                _macroIdElse = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<string> ConditionalIds { get; set; }
        public ObservableCollection<int> RuleIds { get; set; }
        public ObservableCollection<int> InstructionIds { get; set; }
        public ObservableCollection<int> IpCommandIds { get; set; }
        private string _name;
        private int _macroIdThen = -1;
        private int _macroIdElse = -1;
        private ParseObject _ParseObject;
        private bool _IsEnabled = true;

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

        public IEnumerable<ParseObject> ParseObjectsIf { get; set; }
        public IEnumerable<ParseObject> ParseObjectsThen { get; set; }
        public IEnumerable<ParseObject> ParseObjectsElse { get; set; }
        public IList<ParseObject> DeletedZwaveDevices { get; set; }

        public IfThenModel()
        {
            ZwaveDevicesIf = new ObservableCollection<ZwaveDevice>();

            ZwaveDevicesThen = new ObservableCollection<ZwaveDevice>();

            ZwaveDevicesElse = new ObservableCollection<ZwaveDevice>();

            ParseObjectsIf = new List<ParseObject>();

            ParseObjectsThen = new List<ParseObject>();

            ParseObjectsElse = new List<ParseObject>();

            DeletedZwaveDevices = new List<ParseObject>();

            ConditionalIds = new ObservableCollection<string>();

            InstructionIds = new ObservableCollection<int>();

            IpCommandIds = new ObservableCollection<int>();

            RuleIds = new ObservableCollection<int>();
        }
    }
}