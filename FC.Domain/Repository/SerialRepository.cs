using FC.Domain.Model;
using FC.Domain.Model._Serial;
using FC.Domain.Model.Project;
using FC.Domain.Repository.Util;
using FC.Domain.Service;
using FC.Domain.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface ISerialRepository
    {
        Task DeleteAll(GatewayModel selectedGateway);

        Task GetAll(GatewayModel selectedGateway, bool isSendingToGateway = false);

        Task Save(GatewayModel selectedGateway);

        Task GetBaudRate(ProjectModel selectedProject, SerialProtocol serialProtocol, bool isSendingToGateway = false);

        Task GetParity(GatewayModel selectedGateway, SerialProtocol t485, bool isSendingToGateway = false);

        Task SetParity(GatewayModel selectedGateway, SerialProtocol protocol);

        Task SetBaudRate(GatewayModel selectedGateway, SerialProtocol protocol);

        Task PlayByMemoryId(GatewayModel selectedGateway, int memoryId);

        Task DeleteByMemoryId(GatewayModel selectedGateway, SerialModel serial);
    }

    public class SerialRepository : RepositoryBase, ISerialRepository
    {
        private async Task<SerialModel> GetSerialFromGatewayById(GatewayModel selectedGateway, int id, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            IList<object> commands = new List<object>();

            JObject json = null;

            SerialModel serialModel = null;

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

                serialModel = JsonConvert.DeserializeObject<SerialModel>(response);

                _taskService.CancellationTokenSource.Cancel();

                return;
            });

            string command = $"{{\"command\":\"serialCommandGet\",\"type\":0,\"id\":{id}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            return serialModel;
        }

        public async Task DeleteAll(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            foreach (SerialModel serial in selectedGateway.Serials.ToArray())
            {
                await DeleteByMemoryId(selectedGateway, serial);
            }
        }

        public async Task GetAll(GatewayModel selectedGateway, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            JObject json = null;

            IList<object> commands = new List<object>();

            using ReplaySubject<string> replay = new();

            string response = string.Empty;

            _ = replay.Subscribe(resp =>
            {
                response = resp;

                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (!resp.TryParseJObject(out json))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
                return;
            });

            string command = UtilSerial.GETALLOCCUPIEDSERIALMEMORYIDS;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway);

            if (string.IsNullOrEmpty(response))
            {
                replay.Dispose();

                ThrowException(selectedGateway);
            }

            if (!JObject.Parse(response).ContainsKey(LIST))
            {
                replay.Dispose();

                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Does_Not_Contain_Property, LIST));
            }

            List<int> ids = JObject.Parse(response)[LIST].Select(x => (int)x).ToList();

            if (selectedGateway.Serials.Any())
            {
                selectedGateway.Serials.Clear();
            }

            foreach (int id in ids)
            {
                if (await GetSerialFromGatewayById(selectedGateway, id) is SerialModel temp)
                {
                    selectedGateway.Serials.Add(temp);
                }
            };
        }

        public async Task Save(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (selectedGateway.SelectedSerialModel.MemoryId == -1)
            {
                SerialModel model = await GetNextAvailable(selectedGateway);

                selectedGateway.SelectedSerialModel.MemoryId = model.UartId;
            }

            JObject json = null;

            IList<object> commands = new List<object>();

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

                _taskService.CancellationTokenSource.Cancel();
                return;
            });

            selectedGateway.SelectedSerialModel.Protocol = selectedGateway.SelectedSerialModel.SelectedProtocolIndex == 0 ? (int)SerialProtocol.T485 : (int)SerialProtocol.T232;

            string command = $"{{\"command\":\"serialCommandSet\",\"type\":0,\"id\":{selectedGateway.SelectedSerialModel.MemoryId},\"protocol\":{selectedGateway.SelectedSerialModel.Protocol}, \"repetition\":{selectedGateway.SelectedSerialModel.Repetition}, \"delay\":{selectedGateway.SelectedSerialModel.Delay}, \"data\":\"{selectedGateway.SelectedSerialModel.Data}\"}}";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            SerialModel objetRemove = selectedGateway.Serials.FirstOrDefault(x => x.MemoryId == selectedGateway.SelectedSerialModel.MemoryId);

            _ = selectedGateway.Serials.Remove(objetRemove);

            selectedGateway.Serials.Add(selectedGateway.SelectedSerialModel);

            List<SerialModel> listOrderById = selectedGateway.Serials.OrderBy(x => x.MemoryId).ToList();

            selectedGateway.Serials.Clear();

            listOrderById.ForEach((item) => selectedGateway.Serials.Add(item));
        }

        private async Task<SerialModel> GetNextAvailable(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            JObject json = null;

            IList<object> commands = new List<object>();

            using ReplaySubject<string> replay = new();

            string response = string.Empty;

            _ = replay.Subscribe(resp =>
            {
                response = resp;

                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (!resp.TryParseJObject(out json))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();

                return;
            });

            string command = UtilSerial.GETNEXTMEMORYID;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return JsonConvert.DeserializeObject<SerialModel>(response);
        }

        public async Task GetBaudRate(ProjectModel selectedProject, SerialProtocol serialProtocol, bool isSendingToGateway = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (!selectedProject.SelectedGateway.IsSupportSerial485 && serialProtocol == SerialProtocol.T485)
            {
                return;
            }

            JObject json = null;

            IList<object> commands = new List<object>();

            using ReplaySubject<string> replay = new();

            string response = string.Empty;

            _ = replay.Subscribe(resp =>
            {
                response = resp;

                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (!resp.TryParseJObject(out json))
                {
                    return;
                }

                if (int.TryParse(JObject.Parse(response)["baudrate"].ToString(), out int index))
                {
                    if (serialProtocol == SerialProtocol.T485)
                    {
                        selectedProject.SelectedGateway.SelectedBaudRates485Index = index;
                    }
                    else
                    {
                        selectedProject.SelectedGateway.SelectedBaudRates232Index = index;
                    }
                }

                _taskService.CancellationTokenSource.Cancel();

                return;
            });

            string commandParameter = serialProtocol == SerialProtocol.T485 ? "serialBaudRate485Get" : "serialBaudRate232Get";

            string command = $"{{\"command\":\"{commandParameter}\", \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway);

            if (string.IsNullOrEmpty(response))
            {
                replay.Dispose();

                ThrowException(selectedProject.SelectedGateway);
            }

            replay.Dispose();
        }

        public async Task GetParity(GatewayModel selectedGateway, SerialProtocol serialProtocol, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            JObject json = null;

            IList<object> commands = new List<object>();

            using ReplaySubject<string> replay = new();

            string response = string.Empty;

            _ = replay.Subscribe(resp =>
            {
                response = resp;

                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (!resp.TryParseJObject(out json))
                {
                    return;
                }

                if (int.TryParse(JObject.Parse(resp)["parity"].ToString(), out int parity))
                {
                    switch (parity)
                    {
                        case 0:
                            parity = 0;
                            break;

                        case 2:
                            parity = 1;
                            break;

                        case 3:
                            parity = 2;
                            break;
                    }

                    if (serialProtocol == SerialProtocol.T485)
                    {
                        selectedGateway.SelectedParity485Index = parity;
                    }
                    else
                    {
                        selectedGateway.SelectedParity232Index = parity;
                    }
                }

                _taskService.CancellationTokenSource.Cancel();

                return;
            });

            string commandParameter = serialProtocol == SerialProtocol.T485 ? "serialBaudRate485Get" : "serialBaudRate232Get";

            string command = $"{{\"command\":\"{(serialProtocol == SerialProtocol.T485 ? "serialParityGet485" : "serialParityGet232")}\", \"type\":0}}";

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway);

            if (string.IsNullOrEmpty(response))
            {
                replay.Dispose();

                ThrowException(selectedGateway);
            }

            replay.Dispose();
        }

        public async Task SetParity(GatewayModel selectedGateway, SerialProtocol protocol)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            JObject json = null;

            IList<object> commands = new List<object>();

            using ReplaySubject<string> replay = new();

            string response = string.Empty;

            _ = replay.Subscribe(resp =>
            {
                response = resp;

                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (!resp.TryParseJObject(out json))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();

                return;
            });

            using ParitySerialModel parity = protocol == SerialProtocol.T232 ? selectedGateway.Paritys232[selectedGateway.SelectedParity232Index] : selectedGateway.Paritys485[selectedGateway.SelectedParity485Index];

            string command = $"{{\"command\":\"{(protocol == SerialProtocol.T485 ? "serialParitySet485" : "serialParitySet232")}\",\"type\":0,\"parity\":{parity.Value}}}";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();
        }

        public async Task SetBaudRate(GatewayModel selectedGateway, SerialProtocol protocol)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            JObject json = null;

            IList<object> commands = new List<object>();

            using ReplaySubject<string> replay = new();

            string response = string.Empty;

            _ = replay.Subscribe(resp =>
            {
                response = resp;

                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (!resp.TryParseJObject(out json))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();

                return;
            });

            SerialBaudRateModel baudRate = protocol == SerialProtocol.T232 ? selectedGateway.BaudRates232[selectedGateway.SelectedBaudRates232Index] : selectedGateway.BaudRates485[selectedGateway.SelectedBaudRates485Index];

            string command = $"{{\"command\":\"{(protocol == SerialProtocol.T485 ? "serialBaudRate485Set" : "serialBaudRate232Set")}\",\"type\":0,\"baudrate\":{baudRate.Value}}}"; ;

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task PlayByMemoryId(GatewayModel selectedGateway, int memoryId)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            JObject json = null;

            IList<object> commands = new List<object>();

            using ReplaySubject<string> replay = new();

            string response = string.Empty;

            _ = replay.Subscribe(resp =>
            {
                response = resp;

                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (!resp.TryParseJObject(out json))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();

                return;
            });

            string command = $"{{\"command\":\"serialCommandPlayMemory\",\"type\":0,\"id\":{memoryId}}}";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task DeleteByMemoryId(GatewayModel selectedGateway, SerialModel serial)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (serial is null)
            {
                throw new ArgumentNullException(nameof(serial));
            }

            JObject json = null;

            IList<object> commands = new List<object>();

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

                _taskService.CancellationTokenSource.Cancel();
                return;
            });

            string command = $"{{\"command\":\"serialCommandDelete\",\"type\":0,\"id\":{serial.MemoryId}}}";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            _ = selectedGateway.Serials.Remove(serial);

            replay.Dispose();
        }

        public const string LIST = "list";

        public SerialRepository(ITaskService taskService,
                                IGatewayService gatewayService,
                                ITcpRepository tcpRespository,
                                IUDPRepository udpRepository) : base(taskService, gatewayService, tcpRespository, udpRepository)
        {
        }
    }
}