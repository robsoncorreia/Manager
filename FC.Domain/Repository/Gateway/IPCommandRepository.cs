using FC.Domain.Model;
using FC.Domain.Model.IpCommand;
using FC.Domain.Service;
using FC.Domain.Util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface IIPCommandRepository
    {
        Task Delete(GatewayModel selectedGateway, IpCommandModel ipCommand);

        Task DeleteAll(GatewayModel selectedGateway);

        Task GetAll(GatewayModel selectedGateway, bool isSendingToGateway = false);

        Task PlayMemory(GatewayModel selectedGateway, IpCommandModel selectedIpCommand, bool isSendingToGateway = false);

        Task PlayMemory(GatewayModel selectedGateway, int memoryId);

        Task<int> SaveAsync(GatewayModel selectedGateway, IpCommandModel ipCommand);
    }

    public class IPCommandRepository : RepositoryBase, IIPCommandRepository
    {
        #region Fields

        private const string BITFIELD = "bitfield";

        private const string COMMAND = "command";

        private const string COMMANDTYPE = "commandType";

        private const string DATA = "data";

        private const string DELAY = "delay";

        private const string ERROR = "error";

        private const string FORMAT = "format";

        private const string ID = "id";

        private const string INVERT = "invert";

        private const string IP_ADDRESS = "ipAddress";

        private const string IP_TYPE = "ipType";

        private const string NAME = "name";

        private const string PORT = "port";

        private const string REPETITION = "repetition";

        private const int TOTAL = 256;

        private const string TYPE = "type";

        private string command;

        #endregion Fields

        #region Collections

        private readonly IList<object> commands = new List<object>();

        #endregion Collections

        #region Constructor

        public IPCommandRepository(ITaskService taskService,
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

        #region Delete

        public async Task Delete(GatewayModel selectedGateway, IpCommandModel ipCommand)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (ipCommand is null)
            {
                throw new ArgumentNullException(nameof(ipCommand));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject json = null;

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
                if (!json.ContainsKey(ID))
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

            command = $"{{\"command\":\"erase_memory_ip_command\", \"type\":0, \"id\":{ipCommand.MemoryId}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (!json.ContainsKey(ID))
            {
                //todo alterar texto
                throw new Exception(Properties.Resources.Something_went_wrong__Code);
            }

            if (selectedGateway.IpCommands.FirstOrDefault(x => x.MemoryId == json.Value<int>(ID)) is IpCommandModel temp)
            {
                _ = selectedGateway.IpCommands.Remove(temp);
            }
        }

        public async Task DeleteAll(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject json = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                json = JObject.Parse(response);

                if (!json.ContainsKey(COMMAND))
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

            command = $"{{\"command\":\"erase_all_ip_command\", \"type\":0}}";

            await ToCommand(selectedGateway, commands, replay, command);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            selectedGateway.IpCommands.Clear();
        }

        #endregion Delete

        #region Get

        public async Task GetAll(GatewayModel selectedGateway, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (selectedGateway.IpCommands.Any())
            {
                selectedGateway.IpCommands.Clear();
            }

            using ReplaySubject<string> replay = new();

            JObject json = null;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                json = JObject.Parse(response);

                if (!json.ContainsKey(COMMAND))
                {
                    return;
                }
                if (!json.ContainsKey(TYPE))
                {
                    return;
                }
                if (!json.ContainsKey(FORMAT))
                {
                    return;
                }
                if (!json.ContainsKey(INVERT))
                {
                    return;
                }
                if (json.ContainsKey(BITFIELD))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            command = $"{{\"command\":\"get_all_ip_id\",\"type\":0,\"format\":0,\"invert\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway);

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }
            if (!json.ContainsKey(BITFIELD))
            {
                throw new Exception(Properties.Resources.Something_went_wrong__Code);
            }

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (!json.ContainsKey(BITFIELD))
            {
                ThrowException(selectedGateway);
            }

            string bitfield = json.Value<string>(BITFIELD);

            byte[] newBytes = Convert.FromBase64String(bitfield);

            string[] hexas = BitConverter.ToString(newBytes).Split('-');

            int id = 0;

            for (int i = 0; i < hexas.Length; i++)
            {
                string binarystring = string.Join(string.Empty,
                                                               hexas[i].Select(
                                                                   c => Convert.ToString(Convert.ToInt32(c.ToString(CultureInfo.CurrentCulture), 16), 2).PadLeft(4, '0')
                                                                 )
                                                               );
                char[] binarys = binarystring.ToCharArray();

                for (int j = binarys.Length - 1; j >= 0; j--)
                {
                    if (binarys[j].Equals('1'))
                    {
                        if (await GetIpCommand(selectedGateway, id, isSendingToGateway) is IpCommandModel temp)
                        {
                            selectedGateway.IpCommands.Add(temp);
                        }
                    }

                    id++;
                }
            }

            _gatewayService.IsSendingToGateway = false;
        }

        private async Task<IpCommandModel> GetIpCommand(GatewayModel selectedGateway, int id, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            commands.Clear();

            JObject json = null;

            using ReplaySubject<string> replay = new();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                json = JObject.Parse(response);

                if (!json.ContainsKey(COMMAND))
                {
                    return;
                }
                if (!json.ContainsKey(TYPE))
                {
                    return;
                }
                if (!json.ContainsKey(FORMAT))
                {
                    return;
                }
                if (!json.ContainsKey(INVERT))
                {
                    return;
                }
                if (json.ContainsKey(BITFIELD))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            command = $"{{\"command\":\"get_ip_command\", \"id\":{id}, \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway);

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (json.ContainsKey(ERROR))
            {
                return null;
            }

            string name = json.Value<string>(NAME);

            JToken[] jArray = json.Value<JArray>(IP_ADDRESS).ToArray();
            int[] items = jArray.Select(jv => (int)jv).ToArray();
            Array.Reverse(items);
            string targetIP = string.Join(".", items);

            int targetPort = json.Value<int>(PORT);
            int ipType = json.Value<int>(IP_TYPE);
            int delay = json.Value<int>(DELAY);
            int repetition = json.Value<int>(REPETITION);
            int commandType = json.Value<int>(COMMANDTYPE);
            string data = json.Value<string>(DATA);

            return new IpCommandModel
            {
                Name = name,
                MemoryId = id,
                TargetIp = targetIP,
                TargetPort = targetPort,
                IpType = (IpType)ipType,
                Delay = delay,
                Repetition = repetition,
                CommandTypeIndex = commandType,
                Command = data,
                IsSavedGateway = true,
            };
        }

        private async Task GetNextEmptyId(GatewayModel selectedGateway, IpCommandModel ipCommandModel)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (ipCommandModel is null)
            {
                throw new ArgumentNullException(nameof(ipCommandModel));
            }

            commands.Clear();

            using ReplaySubject<string> replay = new();

            JObject json = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                json = JObject.Parse(response);

                if (!json.ContainsKey(COMMAND))
                {
                    return;
                }
                if (!json.ContainsKey(TYPE))
                {
                    return;
                }
                if (!json.ContainsKey(FORMAT))
                {
                    return;
                }
                if (!json.ContainsKey(INVERT))
                {
                    return;
                }
                if (json.ContainsKey(BITFIELD))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            command = $"{{\"command\":\"get_all_ip_id\",\"type\":0, \"format\":0, \"invert\":0}}";

            await ToCommand(selectedGateway, commands, replay, command);

            if (json is null)
            {
                ThrowException(selectedGateway);
            }
            if (!json.ContainsKey(BITFIELD))
            {
                throw new Exception(Properties.Resources.Something_went_wrong__Code);
            }

            string bitfield = json.Value<string>(BITFIELD);

            byte[] newBytes = Convert.FromBase64String(bitfield);

            string[] hexas = BitConverter.ToString(newBytes).Split('-');

            int id = 0;

            for (int i = 0; i < hexas.Length; i++)
            {
                string binarystring = string.Join(string.Empty,
                                                               hexas[i].Select(
                                                                   c => Convert.ToString(Convert.ToInt32(c.ToString(CultureInfo.CurrentCulture), 16), 2).PadLeft(4, '0')
                                                                 )
                                                               );
                char[] binarys = binarystring.ToCharArray();

                for (int j = binarys.Length - 1; j >= 0; j--)
                {
                    if (binarys[j].Equals('0'))
                    {
                        ipCommandModel.MemoryId = id;
                        return;
                    }

                    id++;

                    if (id > (TOTAL - 1))
                    {
                        throw new Exception(Properties.Resources.All_Memories_Are_Full);
                    }
                }
            }
        }

        #endregion Get

        #region Play

        public async Task PlayMemory(GatewayModel selectedGateway, int memoryId)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            if (replay is null)
            {
                throw new ArgumentNullException(nameof(replay));
            }

            commands.Clear();

            JObject json = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                json = JObject.Parse(response);

                if (!json.ContainsKey(COMMAND))
                {
                    return;
                }
                if (!json.ContainsKey(ID))
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

            command = $"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task PlayMemory(GatewayModel selectedGateway, IpCommandModel selectedIpCommand, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (selectedIpCommand is null)
            {
                throw new ArgumentNullException(nameof(selectedIpCommand));
            }

            ReplaySubject<string> replay = new();

            commands.Clear();

            JObject json = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out json))
                {
                    return;
                }

                if (!json.ContainsKey(COMMAND))
                {
                    return;
                }
                if (!json.ContainsKey(ID))
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

            command = $"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{selectedIpCommand.MemoryId}}}";

            await ToCommand(selectedGateway, commands, replay, command);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }
        }

        #endregion Play

        #region Save

        public async Task<int> SaveAsync(GatewayModel selectedGateway, IpCommandModel ipCommand)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (ipCommand is null)
            {
                throw new ArgumentNullException(nameof(ipCommand));
            }

            commands.Clear();

            JObject json = null;

            using ReplaySubject<string> replay = new();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }
                if (!response.TryParseJObject(out json))
                {
                    return;
                }
            });

            bool isNew = false;

            if (ipCommand.MemoryId < 0)
            {
                isNew = true;
                await GetNextEmptyId(selectedGateway, ipCommand);
            }

            string targetIp = $"[{ipCommand.TargetIp.Replace('.', ',')}]";

            string data = ipCommand.StringToIPCommandFormat();

            command = $"{{\"command\":\"set_ip_command\", \"type\":0, \"id\":{ipCommand.MemoryId}, \"name\":\"{ipCommand.Name}\", \"ipAddress\":{targetIp}, \"port\":{ipCommand.TargetPort}, \"ipType\":0, \"delay\":{ipCommand.Delay}, \"repetition\":{ipCommand.Repetition},  \"commandType\":{ipCommand.CommandTypeIndex}, \"data\":\"{data}\"}}";

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();

            if (!isNew)
            {
                if (selectedGateway.IpCommands.FirstOrDefault(x => x.MemoryId == ipCommand.MemoryId) is IpCommandModel temp)
                {
                    int index = selectedGateway.IpCommands.IndexOf(temp);

                    selectedGateway.IpCommands.RemoveAt(index);

                    selectedGateway.IpCommands.Insert(index, ipCommand);

                    selectedGateway.SelectedIndexIPCommand = index;
                }
            }
            else
            {
                IpCommandModel temp = selectedGateway.IpCommands.FirstOrDefault(x => x.MemoryId == ipCommand.MemoryId);
                _ = selectedGateway.IpCommands.Remove(temp);
                selectedGateway.IpCommands.Add(ipCommand);
            }

            return ipCommand.MemoryId;
        }

        #endregion Save
    }
}