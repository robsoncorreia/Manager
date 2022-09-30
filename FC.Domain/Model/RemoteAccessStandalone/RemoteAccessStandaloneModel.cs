using FC.Domain._Base;
using FC.Domain.Model.User;
using LiteDB;
using Parse;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace FC.Domain.Model.FCC
{
    public class RemoteAccessStandaloneModel : ModelBase
    {
        private string _gatewayId;
        private string _gatewayName;

        private ICollectionView _commandsCollectionView;
        private ICollectionView _commandsCloudCollectionView;

        public ObservableCollection<VoiceAssistantCommandModel> Commands { get; set; }
        public ObservableCollection<VoiceAssistantCommandModel> CommandsCloud { get; set; }

        public RemoteAccessStandaloneModel()
        {
            Commands = new ObservableCollection<VoiceAssistantCommandModel>();
            CommandsCloud = new ObservableCollection<VoiceAssistantCommandModel>();
            Users = new ObservableCollection<ParseUserCustom>();
        }

        public string GatewayId
        {
            get => _gatewayId;
            set
            {
                _gatewayId = value;
                NotifyPropertyChanged();
            }
        }

        public string GatewayName
        {
            get => _gatewayName;
            set
            {
                _gatewayName = value;
                NotifyPropertyChanged();
            }
        }

        public void GroupBy(object obj)
        {
            if ((bool)obj)
            {
                CommandsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(VoiceAssistantCommandModel.Type)));
            }
            else
            {
                CommandsCollectionView.GroupDescriptions.Clear();
            }
        }

        [BsonIgnore]
        public ICollectionView CommandsCollectionView
        {
            get => CollectionViewSource.GetDefaultView(Commands);
            set => _commandsCollectionView = value;
        }

        [BsonIgnore]
        public ICollectionView CommandsCloudCollectionView
        {
            get => CollectionViewSource.GetDefaultView(CommandsCloud);
            set => _commandsCloudCollectionView = value;
        }

        public ParseObject ParseObject { get; set; }

        public ObservableCollection<ParseUserCustom> Users { get; set; }
        public string UID { get; set; }
    }
}