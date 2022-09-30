using CommunityToolkit.Mvvm.Messaging;
using FC.Domain.Model;
using System.Collections.ObjectModel;

namespace FC.Domain.Repository
{
    public interface ICommandRepository
    {
        ObservableCollection<CommandModel> Commands { get; set; }

        ObservableCollection<CommandModel> GetAll();
    }

    public class CommandRepository : ICommandRepository
    {
        public ObservableCollection<CommandModel> Commands { get; set; }

        public CommandRepository(ILocalDBRepository liteDBService)
        {
            _liteDBService = liteDBService;

            Commands = GetAll();

            WeakReferenceMessenger.Default.Register<CommandModel>(this, (r, command) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    Commands.Add(command);
                    _ = _liteDBService.Update(command);
                });
            });
        }

        public ObservableCollection<CommandModel> GetAll()
        {
            return _liteDBService.GetAllCommands();
        }

        private readonly ILocalDBRepository _liteDBService;
    }
}