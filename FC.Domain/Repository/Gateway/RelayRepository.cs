using FC.Domain.Model;
using FC.Domain.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FC.Domain.Repository.Gateway
{
    public interface IRelayRepository
    {
        Task GetRelayState(GatewayModel selectedGateway);

        Task SetRelayState(GatewayModel selectedGateway);
    }

    public class RelayRepository : RepositoryBase, IRelayRepository
    {
        #region Fields

        private const string COMMAND = "command";
        private const string MODE = "mode";
        private const string STATE = "state";
        private const string TIME = "time";
        private const string TYPE = "type";
        private const string VALUE = "value";

        #endregion Fields

        #region Constructor

        public RelayRepository(ITaskService taskService,
                       IGatewayService gatewayService,
                       ITcpRepository tcpRespository,
                       IUDPRepository udpRepository) : base(taskService, gatewayService, tcpRespository, udpRepository)
        {
            _gatewayService = gatewayService;

            _udpRepository = udpRepository;

            _taskService = taskService;

            _tcpRepository = tcpRespository;
        }

        #endregion Constructor

        #region Collections

        private readonly IList<object> commands = new List<object>();

        #endregion Collections

        #region Get

        public async Task GetRelayState(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            JObject json = null;

            commands.Clear();

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                json = JObject.Parse(resp);

                if (!json.ContainsKey(COMMAND))
                {
                    return;
                }
                if (!json.ContainsKey(STATE))
                {
                    return;
                }
                if (!json.ContainsKey(TIME))
                {
                    return;
                }
                if (!json.ContainsKey(TYPE))
                {
                    return;
                }
                if (json.Value<int>(TYPE) == 1)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"get_relay_output\", \"type\":0}}";

            _gatewayService.LastCommandSend = command;

            await ToCommand(selectedGateway, commands, replay, command);

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (json.ContainsKey(STATE))
            {
                selectedGateway.StateRelay = !(json.Value<int>(STATE) == 0);
            }

            replay.Dispose();
        }

        #endregion Get

        #region Set

        public async Task SetRelayState(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            JObject json = null;

            commands.Clear();

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                json = JObject.Parse(resp);

                if (!json.ContainsKey(COMMAND))
                {
                    return;
                }
                if (!json.ContainsKey(VALUE))
                {
                    return;
                }
                if (!json.ContainsKey(MODE))
                {
                    return;
                }
                if (!json.ContainsKey(TIME))
                {
                    return;
                }
                if (!json.ContainsKey(TYPE))
                {
                    return;
                }
                if (json.Value<int>(TYPE) == 1)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            ;

            string command = $"{{\"command\":\"set_relay_output\", \"type\":0, \"value\":{(selectedGateway.StateRelay ? 1 : 0)},\"mode\": {selectedGateway.SelectedIndexRelayStateMode}, \"time\":{selectedGateway.RelayPulseTime}}}";

            _gatewayService.IsSendingToGateway = true;

            _gatewayService.LastCommandSend = command;

            await ToCommand(selectedGateway, commands, replay, command);

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            replay.Dispose();
        }

        #endregion Set
    }
}