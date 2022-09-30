using ConfigurationFlexCloudHubBlaster._Base;
using ConfigurationFlexCloudHubBlaster.Model.Device;
using ConfigurationFlexCloudHubBlaster.Repository.Util;
using Parse;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ConfigurationFlexCloudHubBlaster.Model.IfThen
{
    public class IfThenModel : ModelBase
    {
        public ParseObject ParseObject { get; set; }

        public ObservableCollection<ZwaveDevice> ZwaveDevicesIf { get; set; }
        public ObservableCollection<ZwaveDevice> ZwaveDevicesThen { get; set; }
        public ObservableCollection<ZwaveDevice> ZwaveDevicesElse { get; set; }
        public string RuleId { get; internal set; } = UtilIfThen.NEW;

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

        public IList<string> ConditionalIds { get; set; }
        public IList<int> InstructionIds { get; set; }
        public IList<int> IpCommandIds { get; set; }
        private string _name;
        private int _macroIdThen = -1;
        private int _macroIdElse = -1;

        public string Name
        {
            get { return _name; }
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

        public IEnumerable<ParseObject> ParseObjectsIf { get; internal set; }
        public IEnumerable<ParseObject> ParseObjectsThen { get; internal set; }
        public IEnumerable<ParseObject> ParseObjectsElse { get; internal set; }
        public IList<ParseObject> DeletedZwaveDevices { get; internal set; }

        public IfThenModel()
        {
            ZwaveDevicesIf = new ObservableCollection<ZwaveDevice>();

            ZwaveDevicesThen = new ObservableCollection<ZwaveDevice>();

            ZwaveDevicesElse = new ObservableCollection<ZwaveDevice>();

            ParseObjectsIf = new List<ParseObject>();

            ParseObjectsThen = new List<ParseObject>();

            ParseObjectsElse = new List<ParseObject>();

            DeletedZwaveDevices = new List<ParseObject>();

            ConditionalIds = new List<string>();

            InstructionIds = new List<int>();

            IpCommandIds = new List<int>();
        }
    }
}