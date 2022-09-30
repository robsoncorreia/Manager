using FC.Domain.Model;
using FC.Domain.Model.IR;
using FC.Domain.Model.Project;
using FC.Domain.Repository.Util;
using FC.Domain.Service;
using FC.Domain.Util;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface IIRRepository
    {
        Task Delete(GatewayModel selectedGateway, int memoryId);

        Task DeleteAll(GatewayModel selectedDevice);

        Task DeleteAllFromCloudAsync(ProjectModel selectedProjectModel);

        Task DeleteFromCloudAsync(ProjectModel selectedProjectModel, IRModel irModel);

        Task<bool> GetAll(GatewayModel selectedDevice, bool isSendingGateway = false);

        Task GetAllFromCloud(ProjectModel selectedProjectModel);

        Task Insert(ProjectModel selectedProjectModel, IRModel iRModel);

        Task<bool> Learn(GatewayModel selectedGateway, IRModel iRModel);

        Task PlayCode(GatewayModel selectedDevice, IRModel iRModel);

        Task PlayMemory(GatewayModel selectedDevice, int memoryId);

        Task Save(GatewayModel selectedDevice, IRModel iRModel, bool isSendingToGateway = false);
    }

    public class IRRepository : RepositoryBase, IIRRepository
    {
        private readonly IParseService _parseService;
        private JObject json;

        #region Collections

        private readonly IList<object> commands = new List<object>();

        #endregion Collections

        #region Constructor

        public IRRepository(ITaskService taskService,
                            IGatewayService gatewayService,
                    ITcpRepository tcpRespository,
                    IUDPRepository udpRepository,
                    IParseService parseService) : base(taskService, gatewayService, tcpRespository, udpRepository)
        {
            _gatewayService = gatewayService;

            _taskService = taskService;

            _parseService = parseService;
        }

        #endregion Constructor

        #region Delete

        public async Task Delete(GatewayModel selectedGateway, int memoryId)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            commands.Clear();

            json = null;

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
                if (json.ContainsKey(UtilIR.IRDELETEMEMORYCODE))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"irDeleteMemoryCode\", \"id\":{memoryId},\"type\":0}}";

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task DeleteAll(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            json = null;

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

                if (json.ContainsKey(UtilIR.COMMAND) && json.Value<string>(UtilIR.COMMAND) == UtilIR.IRDELETEALLMEMORYCODES)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = UtilIR.DELETEALLMEMORYCODES;

            await ToCommand(selectedGateway, commands, replay, command);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();
        }

        public async Task DeleteAllFromCloudAsync(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            _parseService.IsSendingToCloud = true;

            await ParseObject.DeleteAllAsync(selectedProjectModel.SelectedGateway.IRsCloud.Select(x => x.ParseObject));

            selectedProjectModel.SelectedGateway.IRsCloud.Clear();

            _parseService.IsSendingToCloud = false;
        }

        public async Task DeleteFromCloudAsync(ProjectModel selectedProjectModel, IRModel irModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            if (irModel is null)
            {
                throw new ArgumentNullException(nameof(irModel));
            }

            _parseService.IsSendingToCloud = true;

            if (irModel.ParseObject is null)
            {
                _ = selectedProjectModel.SelectedGateway.IRsCloud.Remove(irModel);

                _parseService.IsSendingToCloud = false;

                return;
            }

            await irModel.ParseObject.DeleteAsync();

            _parseService.IsSendingToCloud = false;

            _ = selectedProjectModel.SelectedGateway.IRsCloud.Remove(irModel);

            if (selectedProjectModel.SelectedGateway.IRsGateway.FirstOrDefault(x => x.MemoryId == irModel.MemoryId) is IRModel temp)
            {
                temp.ParseObject = null;
            }
        }

        #endregion Delete

        #region Get

        public async Task<bool> GetAll(GatewayModel selectedGateway, bool isSendingGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            commands.Clear();

            using ReplaySubject<string> replay = new();

            json = null;

            int[] ids = null;

            bool @return = false;

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

                if (json.TryGetIntArray(UtilIR.LIST, out ids))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            if (selectedGateway.IRsGateway.Any())
            {
                selectedGateway.IRsGateway.Clear();
            }

            string command = UtilIR.GETALLIRID;

            await ToCommand(selectedGateway, commands, replay, command, isSendingGateway);

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            foreach (int id in ids)
            {
                if (isCanceled)
                {
                    break;
                }
                if (await GetIrFromGateway(selectedGateway, id, isSendingGateway) is IRModel ir)
                {
                    selectedGateway.IRsGateway.Add(ir);
                }
            }

            return @return;
        }

        public async Task GetAllFromCloud(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (selectedProject.SelectedGateway.IRsCloud.Any())
            {
                selectedProject.SelectedGateway.IRsCloud.Clear();
            }

            _parseService.IsSendingToCloud = true;

            IEnumerable<ParseObject> irs = await selectedProject.SelectedGateway.ParseObject
                                                                                .GetRelation<ParseObject>("irs")
                                                                                .Query.FindAsync();

            foreach (ParseObject parseObject in irs)
            {
                if (isCanceled)
                {
                    break;
                }
                selectedProject.SelectedGateway.IRsCloud.Add(ParseObjectToIRModel(parseObject));
            }

            _parseService.IsSendingToCloud = false;
        }

        private async Task<IRModel> GetIrFromGateway(GatewayModel selectedGateway, int id, bool isSendingGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            commands.Clear();

            using ReplaySubject<string> subject = new();

            json = null;

            _ = subject.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }
                if (!response.TryParseJObject(out json))
                {
                    return;
                }
                if (json.ContainsKey(UtilIR.COMMAND) && json.Value<string>(UtilIR.GETIRMEMORYCODE) == UtilIR.GETIRMEMORYCODE)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"irMemoryCodeGet\", \"id\":{id}, \"type\":0}}";

            try
            {
                await ToCommand(selectedGateway, commands, subject, command, isSendingGateway);

                subject.Dispose();
            }
            catch (Exception)
            {
                subject.Dispose();

                return null;
            }

            if (json is null)
            {
                subject.Dispose();

                return null;
            }

            using IRModel irModel = new();

            if (json.ContainsKey(UtilIR.ID))
            {
                irModel.MemoryId = json.Value<int>(UtilIR.ID);
            }

            if (json.ContainsKey(UtilIR.VERSION))
            {
                irModel.Version = json.Value<int>(UtilIR.VERSION);
            }

            if (json.ContainsKey(UtilIR.VERSIONID))
            {
                irModel.VersionId = json.Value<int>(UtilIR.VERSIONID);
            }

            if (json.ContainsKey(UtilIR.CHANNEL))
            {
                irModel.Channel = json.Value<int>(UtilIR.CHANNEL);
            }
            if (json.ContainsKey(UtilIR.REPETITION))
            {
                irModel.Repetition = json.Value<int>(UtilIR.REPETITION);
            }
            if (json.ContainsKey(UtilIR.DELAY))
            {
                irModel.Delay = json.Value<int>(UtilIR.DELAY);
            }

            if (json.ContainsKey(UtilIR.FREQUENCY))
            {
                irModel.Frequency = json.Value<int>(UtilIR.FREQUENCY);
            }
            if (json.ContainsKey(UtilIR.LENGTH))
            {
                irModel.Lenght = json.Value<int>(UtilIR.LENGTH);
            }

            if (json.ContainsKey(UtilIR.XDATA))
            {
                irModel.XData = json.Value<string>(UtilIR.XDATA);
            }

            if (json.ContainsKey(UtilIR.YDATA))
            {
                irModel.YData = json.Value<string>(UtilIR.YDATA);
            }

            if (json.ContainsKey(UtilIR.DATA))
            {
                string data = json.Value<string>(UtilIR.DATA);

                irModel.IsCompress = data.Contains("X") || data.Contains("Y");

                irModel.DataCompress = data;

                irModel.Data = $"{irModel.Frequency:X8}{irModel.Lenght:X8}{data.Replace("X", irModel.XData).Replace("Y", irModel.YData)}";
            }

            using ReplaySubject<string> replay = new();

            JObject jsonDesc = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }
                if (!response.TryParseJObject(out jsonDesc))
                {
                    return;
                }
                if (jsonDesc.ContainsKey(UtilIR.NAME))
                {
                    irModel.Description = jsonDesc.Value<string>(UtilIR.NAME);
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            command = $"{{\"command\":\"irMemoryDescriptionGet\", \"type\":0, \"id\":{id}}}";

            try
            {
                await ToCommand(selectedGateway, commands, replay, command, isSendingGateway);

                replay.Dispose();
            }
            catch (Exception)
            {
                replay.Dispose();

                return null;
            }

            return irModel;
        }

        private async Task GetNextIRId(GatewayModel selectedGateway, IRModel irModel, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (irModel is null)
            {
                throw new ArgumentNullException(nameof(irModel));
            }

            commands.Clear();

            using ReplaySubject<string> replay = new();

            json = null;

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
                if (json.ContainsKey(UtilIR.IRID))
                {
                    irModel.MemoryId = json.Value<int>(UtilIR.IRID);
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = UtilIR.GETNEXTIRID;

            await ToCommand(selectedGateway, commands, replay, command, isSendingToGateway);

            replay.Dispose();
        }

        #endregion Get

        #region Play

        public async Task PlayCode(GatewayModel selectedGateway, IRModel irModel)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (irModel is null)
            {
                throw new ArgumentNullException(nameof(irModel));
            }

            commands.Clear();

            using ReplaySubject<string> replay = new();

            json = null;

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
                if (json.ContainsKey(UtilIR.RESULT))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            irModel.Data = irModel.Data.Replace(" ", "").ToUpper();

            if (int.TryParse(irModel.Data.Substring(4, 4), NumberStyles.HexNumber, null, out int frequency))
            {
                irModel.Frequency = frequency;
            }

            if (irModel.Data.Substring(8, 4) == "0000")
            {
                if (int.TryParse(irModel.Data.Substring(12, 4), NumberStyles.HexNumber, null, out int lenght))
                {
                    irModel.Lenght = lenght;
                }
            }
            else
            {
                if (int.TryParse(irModel.Data.Substring(8, 4), NumberStyles.HexNumber, null, out int lenght))
                {
                    irModel.Lenght = lenght;
                }
            }

            ChannelIR channel = Enum.GetValues(typeof(ChannelIR)).Cast<ChannelIR>().ToList().ElementAt(irModel.SelectedChannelIndex);

            string command = string.Empty;

            CompressIR(irModel);

            command = $"{{\"command\":\"irPlayCode\",\"type\":0,\"version\":{irModel.Version},\"channel\":{(int)channel},\"repetition\":{irModel.Repetition},\"delay\":{irModel.Delay},\"frequency\":{irModel.Frequency},\"length\":{irModel.Lenght},\"xData\":\"{irModel.XData}\",\"yData\":\"{irModel.YData}\",\"data\":\"{irModel.DataCompress}\"}}";

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task PlayMemory(GatewayModel selectedGateway, int memoryId)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            commands.Clear();

            using ReplaySubject<string> replay = new();

            json = null;

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
                if (json.ContainsKey(UtilIR.RESULT))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"irPlayMemoryCode\", \"id\":{memoryId},\"type\":0}}";

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        #endregion Play

        public async Task Insert(ProjectModel selectedProject, IRModel irModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (irModel is null)
            {
                throw new ArgumentNullException(nameof(irModel));
            }

            irModel.ParseObject = await selectedProject.SelectedGateway.ParseObject
                                                                           .GetRelation<ParseObject>("irs")
                                                                           .Query
                                                                           .WhereEqualTo(UtilIR.MEMORYID, irModel.MemoryId)
                                                                           .FirstOrDefaultAsync();

            await IrToParseObject(irModel, selectedProject);

            ParseRelation<ParseObject> relation = selectedProject.SelectedGateway.ParseObject.GetRelation<ParseObject>("irs");

            _parseService.IsSendingToCloud = true;

            relation.Add(irModel.ParseObject);

            await selectedProject.SelectedGateway.ParseObject.SaveAsync();

            _parseService.IsSendingToCloud = false;
        }

        public async Task<bool> Learn(GatewayModel selectedGateway, IRModel irModel)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            json = null;

            using ReplaySubject<string> replay = new();

            bool isLearned = false;

            commands.Clear();

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

                if (json.ContainsKey(UtilIR.COMMAND) && json.ContainsKey(UtilIR.DATA))
                {
                    string data = json.Value<string>(UtilIR.DATA);

                    if (int.TryParse(json.Value<string>(UtilIR.DATA).Substring(0, 8), NumberStyles.HexNumber, null, out int frequency))
                    {
                        irModel.Frequency = frequency;
                    }

                    if (int.TryParse(json.Value<string>(UtilIR.DATA).Substring(8, 8), NumberStyles.HexNumber, null, out int lenght))
                    {
                        irModel.Lenght = lenght;
                    }

                    if (json.ContainsKey(UtilIR.COMPRESS))
                    {
                        irModel.IsCompress = json.Value<int>(UtilIR.COMPRESS) == 1;
                    }

                    if (json.ContainsKey(UtilIR.XDATA))
                    {
                        irModel.XData = json.Value<string>(UtilIR.XDATA);
                    }

                    if (json.ContainsKey(UtilIR.YDATA))
                    {
                        irModel.YData = json.Value<string>(UtilIR.YDATA);
                    }

                    if (irModel.IsCompress)
                    {
                        irModel.DataCompress = data.Substring(16, data.Length - 16);

                        data = data.Replace("X", irModel.XData).Replace("Y", irModel.YData);
                    }

                    irModel.Data = data;

                    _taskService.CancellationTokenSource.Cancel();

                    isLearned = true;

                    return;
                }
            });

            string command = "{\"command\":\"irLearn\",\"compress\":1,\"type\":0}";

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();

            return isLearned;
        }

        public async Task Save(GatewayModel selectedGateway, IRModel irModel, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (irModel is null)
            {
                throw new ArgumentNullException(nameof(irModel));
            }

            commands.Clear();

            bool IsNew = false;

            if (irModel.MemoryId < 0)
            {
                IsNew = true;
                await GetNextIRId(selectedGateway, irModel, isSendingToGateway);
            }

            using ReplaySubject<string> replay = new();

            json = null;

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
                if (json.ContainsKey(UtilIR.RESULT))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            irModel.Channel = (int)Enum.GetValues(typeof(ChannelIR)).Cast<ChannelIR>().ToList().ElementAt(irModel.SelectedChannelIndex);

            string command = string.Empty;

            irModel.Data = irModel.Data.Replace(" ", "").ToUpper();

            if (int.TryParse(irModel.Data.Substring(4, 4), NumberStyles.HexNumber, null, out int frequency))
            {
                irModel.Frequency = frequency;
            }

            if (irModel.Data.Substring(8, 4) == "0000")
            {
                if (int.TryParse(irModel.Data.Substring(12, 4), NumberStyles.HexNumber, null, out int lenght))
                {
                    irModel.Lenght = lenght;
                }
            }
            else
            {
                if (int.TryParse(irModel.Data.Substring(8, 4), NumberStyles.HexNumber, null, out int lenght))
                {
                    irModel.Lenght = lenght;
                }
            }

            CompressIR(irModel);

            command = $"{{\"command\":\"irMemoryCodeSet\",\"type\":0,\"version\":{irModel.Version},\"id\":{irModel.MemoryId},\"channel\":{irModel.Channel}, \"repetition\": {irModel.Repetition},\"delay\": {irModel.Delay},\"frequency\":{irModel.Frequency},\"length\": {irModel.Lenght},\"xData\":\"{irModel.XData}\",\"yData\":\"{irModel.YData}\",\"data\":\"{irModel.DataCompress}\"}}";

            await ToCommand(selectedGateway, commands, replay, command, isSendingToGateway);

            replay.Dispose();

            using ReplaySubject<string> subject = new();

            JObject jsonDesc = null;

            _ = subject.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out jsonDesc))
                {
                    return;
                }

                if (jsonDesc.ContainsKey(UtilIR.SETIRMEMORYDESCRIPTION))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            command = $"{{\"command\":\"irMemoryDescriptionSet\",\"type\":1,\"id\":{irModel.MemoryId},\"name\":\"{irModel.Description}\"}}";

            await ToCommand(selectedGateway, commands, replay, command, isSendingToGateway);

            subject.Dispose();

            if (!IsNew)
            {
                if (selectedGateway.IRsGateway.FirstOrDefault(x => x.MemoryId == irModel.MemoryId) is IRModel temp)
                {
                    int index = selectedGateway.IRsGateway.IndexOf(temp);

                    selectedGateway.IRsGateway.RemoveAt(index);

                    irModel.CopyPropertiesTo(temp);

                    selectedGateway.IRsGateway.Insert(index, temp);

                    selectedGateway.SelectedIndexIR = index;
                }

                return;
            }

            selectedGateway.IRsGateway.Add(irModel);
        }

        private void CompressIR(IRModel irModel)
        {
            irModel.XData = irModel.Data.Substring(24, 8);

            char[] arrayData = irModel.Data.ToArray();
            char[] arrayXData = irModel.XData.ToArray();

            for (int i = 0; i < arrayData.Length; i += 8)
            {
                if (arrayData[i] == arrayXData[0]
                    && arrayData[i + 1] == arrayXData[1]
                    && arrayData[i + 2] == arrayXData[2]
                    && arrayData[i + 3] == arrayXData[3]
                    && arrayData[i + 4] == arrayXData[4]
                    && arrayData[i + 5] == arrayXData[5]
                    && arrayData[i + 6] == arrayXData[6]
                    && arrayData[i + 7] == arrayXData[7]
                    )
                {
                    arrayData[i] = 'X';
                    arrayData[i + 1] = 'X';
                    arrayData[i + 2] = 'X';
                    arrayData[i + 3] = 'X';
                    arrayData[i + 4] = 'X';
                    arrayData[i + 5] = 'X';
                    arrayData[i + 6] = 'X';
                    arrayData[i + 7] = 'X';
                }
            }

            irModel.DataCompress = string.Concat(arrayData).Replace("XXXXXXXX", "X");

            int index = FindIndexY(irModel.DataCompress);

            if (index != -1)
            {
                irModel.YData = irModel.DataCompress.Substring(index, 8);

                arrayData = irModel.DataCompress.ToArray();

                char[] arrayYData = irModel.YData.ToArray();

                for (int i = 0; i < arrayData.Length; i++)
                {
                    if (arrayData[i] == 'X')
                    {
                        continue;
                    }
                    if (arrayData[i] == arrayXData[0]
                        && arrayData[i + 1] == arrayYData[1]
                        && arrayData[i + 2] == arrayYData[2]
                        && arrayData[i + 3] == arrayYData[3]
                        && arrayData[i + 4] == arrayYData[4]
                        && arrayData[i + 5] == arrayYData[5]
                        && arrayData[i + 6] == arrayYData[6]
                        && arrayData[i + 7] == arrayYData[7]
                        )
                    {
                        arrayData[i] = 'Y';
                        arrayData[i + 1] = 'Y';
                        arrayData[i + 2] = 'Y';
                        arrayData[i + 3] = 'Y';
                        arrayData[i + 4] = 'Y';
                        arrayData[i + 5] = 'Y';
                        arrayData[i + 6] = 'Y';
                        arrayData[i + 7] = 'Y';
                    }
                    i += 7;
                }

                irModel.DataCompress = string.Concat(arrayData).Replace("YYYYYYYY", "Y");

                irModel.DataCompress = irModel.DataCompress.Substring(16, irModel.DataCompress.Length - 16);
            }
        }

        private int FindIndexY(string dataCompress)
        {
            int indexOf = dataCompress.IndexOf("X");
            for (int i = indexOf; i < dataCompress.Length; i++)
            {
                if (dataCompress[i] != 'X')
                {
                    return i;
                }
            }
            return -1;
        }

        private async Task IrToParseObject(IRModel irModel, ProjectModel selectedProject)
        {
            if (irModel.ParseObject is null)
            {
                irModel.ParseObject = new ParseObject(UtilIR.CLASSNAME);
            }

            if (irModel is null)
            {
                throw new ArgumentNullException(nameof(irModel));
            }

            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            irModel.ParseObject.ACL = selectedProject.ParseObject.ACL;
            irModel.ParseObject[UtilIR.DELAY] = irModel.Delay;
            irModel.ParseObject[UtilIR.DESCRIPTION] = irModel.Description;
            irModel.ParseObject[UtilIR.NUMBERCHANNEL] = irModel.NumberChannel;
            irModel.ParseObject[UtilIR.MEMORYID] = irModel.MemoryId;
            irModel.ParseObject[UtilIR.REPETITION] = irModel.Repetition;
            irModel.ParseObject[UtilIR.SELECTEDCHANNEL] = irModel.SelectedChannelIndex;
            irModel.ParseObject[UtilIR.FREQUENCY] = irModel.Frequency;
            irModel.ParseObject[UtilIR.LENGHT] = irModel.Lenght;
            irModel.ParseObject[UtilIR.DATA] = irModel.Data;
            irModel.ParseObject[UtilIR.XDATA] = irModel.XData;
            irModel.ParseObject[UtilIR.YDATA] = irModel.YData;
            irModel.ParseObject[UtilIR.COMPRESS] = irModel.DataCompress;

            await irModel.ParseObject.SaveAsync();
        }

        private IRModel ParseObjectToIRModel(ParseObject parseObject)
        {
            using IRModel irModel = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey(UtilIR.FREQUENCY))
            {
                irModel.Frequency = parseObject.Get<int>(UtilIR.FREQUENCY);
            }

            if (parseObject.ContainsKey(UtilIR.LENGHT))
            {
                irModel.Lenght = parseObject.Get<int>(UtilIR.LENGHT);
            }

            if (parseObject.ContainsKey(UtilIR.SELECTEDCHANNEL))
            {
                irModel.SelectedChannelIndex = parseObject.Get<int>(UtilIR.SELECTEDCHANNEL);
            }

            if (parseObject.ContainsKey(UtilIR.REPETITION))
            {
                irModel.Repetition = parseObject.Get<int>(UtilIR.REPETITION);
            }

            if (parseObject.ContainsKey(UtilIR.MEMORYID))
            {
                irModel.MemoryId = parseObject.Get<int>(UtilIR.MEMORYID);
            }

            if (parseObject.ContainsKey(UtilIR.NUMBERCHANNEL))
            {
                irModel.NumberChannel = parseObject.Get<int>(UtilIR.NUMBERCHANNEL);
            }

            if (parseObject.ContainsKey(UtilIR.DESCRIPTION))
            {
                irModel.Description = parseObject.Get<string>(UtilIR.DESCRIPTION);
            }

            if (parseObject.ContainsKey(UtilIR.DELAY))
            {
                irModel.Delay = parseObject.Get<int>(UtilIR.DELAY);
            }

            if (parseObject.ContainsKey(UtilIR.DATA))
            {
                irModel.Data = parseObject.Get<string>(UtilIR.DATA);
            }

            return irModel;
        }
    }
}