using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Device;
using FC.Domain.Model.IfThen;
using FC.Domain.Model.IpCommand;
using FC.Domain.Model.Project;
using FC.Domain.Repository.Util;
using FC.Domain.Service;
using FC.Domain.Service.Rede;
using FC.Domain.Util;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface IIfThenRepository
    {
        Task CreateIfThen(ProjectModel selectedProject, IfThenModel ifThenModel);

        Task CreateSchedule(ProjectModel selectedProjectModel, IfThenModel selectedIfThenModel);

        Task DeleteAllConditionals(GatewayModel gateway);

        Task DeleteAllRules(GatewayModel gateway);

        Task DeleteIfThenFromCloud(ProjectModel selectedProjectModel, IfThenModel ifThen);

        Task<bool> DeleteIfThenFromGataway(ProjectModel selectedProjectModel, IfThenModel ifThen);

        Task GatewayReboot(ProjectModel selectedProjectModel);

        Task GetGatewayClockTime(ProjectModel selectedProjectModel);

        Task GetIfThen(ProjectModel selectedProjectModel, IfThenModel selectedIfThenModel);

        Task GetIfThens(ProjectModel selectedProjectModel, ObservableCollection<IfThenModel> ifThenModels, IfthenType ifthenType = IfthenType.Default);

        Task<int> GetNextInstructionId(ProjectModel selectedProject);

        Task<int> GetNextMacroId(ProjectModel selectedProject);

        Task PlayMacroAsync(ProjectModel selectedProject, int macroId);

        Task Rename(IfThenModel selectedIfThenModel);

        Task ResetDevice(ProjectModel selectedProject);

        Task RuleIdEnabled(ProjectModel selectedProjectModel, IfThenModel selectedIfThenModel);

        Task<int> SetInstruction(ProjectModel selectedProject, string instruction, int? delay = 0);

        Task SetMacro(ProjectModel selectedProject, int macroId);

        Task<string> SetRule(ProjectModel selectedProject);

        Task StopMacroAsync(ProjectModel selectedProject, int macroId);

        Task Update(ProjectModel selectedProjectModel, IfThenModel selectedIfThenModel);
    }

    public class IfThenRepository : RepositoryBase, IIfThenRepository
    {
        #region Fields

        private readonly IIPCommandRepository _ipCommandRepository;
        private readonly IParseService _parseService;
        private int indexConditional = 0;

        #endregion Fields

        #region Collections

        private readonly IList<object> commands = new List<object>();

        #endregion Collections

        #region Constructor

        public IfThenRepository(ITaskService taskService,
                        IGatewayService gatewayService,
                        IIPCommandRepository ipCommandRepository,
                        IParseService parseService,
                        IUDPRepository udpRepository,
                        ITcpRepository tcpRespository) : base(taskService, gatewayService, tcpRespository, udpRepository)
        {
            _ipCommandRepository = ipCommandRepository;

            _parseService = parseService;
        }

        #endregion Constructor

        #region Create

        public async Task CreateIf(ProjectModel selectedProject, ZwaveDevice selectedZwaveDevice, int ruleId, int conditionalId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (selectedZwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedZwaveDevice));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
              {
                  if (string.IsNullOrEmpty(response))
                  {
                      return;
                  }

                  if (response.Contains("&ITZSOK#"))
                  {
                      _taskService.CancellationTokenSource.Cancel();
                      return;
                  }
              });

            string command = GetIZTSJson(selectedZwaveDevice, conditionalId);

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            await IfThenAttachConditional(selectedProject, ruleId, conditionalId, (LogicGateIfThen)selectedZwaveDevice.SelectedIndexLogicGateIfThen);
        }

        public async Task CreateIfThen(ProjectModel selectedProject, IfThenModel ifThenModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            if (ifThenModel.ConditionalIds.Any())
            {
                await DeleteConditionals(selectedProject, ifThenModel);
                ifThenModel.ConditionalIds.Clear();
            }

            if (ifThenModel.InstructionIds.Any())
            {
                await DeleteInstructions(selectedProject, ifThenModel);
                ifThenModel.InstructionIds.Clear();
            }

            if (ifThenModel.IpCommandIds.Any())
            {
                await DeleteIpCommand(selectedProject, ifThenModel);
                ifThenModel.IpCommandIds.Clear();
            }

            if (ifThenModel.MacroIdThen != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdThen);
                ifThenModel.MacroIdThen = -1;
            }

            if (ifThenModel.MacroIdElse != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdElse);
                ifThenModel.MacroIdElse = -1;
            }

            if (ifThenModel.RuleIds.Any())
            {
                await DeleteRule(selectedProject, ifThenModel.RuleIds);
                ifThenModel.RuleIds.Clear();
            }

            indexConditional = 0;

            int ruleId = await GetNextRuleId(selectedProject);

            ifThenModel.RuleIds.Add(ruleId);

            ifThenModel.MacroIdThen = await GetNextMacroId(selectedProject);

            ifThenModel.MacroIdElse = -1;

            await SetMacro(selectedProject, ifThenModel.MacroIdThen);

            if (ifThenModel.ZwaveDevicesElse.Any())
            {
                ifThenModel.MacroIdElse = await GetNextMacroId(selectedProject);

                await SetMacro(selectedProject, ifThenModel.MacroIdElse);
            }

            _ = await SetRule(selectedProject, ifThenModel, ruleId);

            foreach (ZwaveDevice zwaveDeviceif in ifThenModel.ZwaveDevicesIf)
            {
                int conditionalId = await GetNextConditionalId(selectedProject);

                ifThenModel.ConditionalIds.Add(conditionalId.ToString());

                await CreateIf(selectedProject, zwaveDeviceif, ruleId, conditionalId);
            }

            foreach (ZwaveDevice zwaveDevice in ifThenModel.ZwaveDevicesThen)
            {
                await CreateThen(selectedProject, zwaveDevice, ifThenModel.MacroIdThen, ifThenModel);
            }

            foreach (ZwaveDevice zwaveDevice in ifThenModel.ZwaveDevicesElse)
            {
                await CreateThen(selectedProject, zwaveDevice, ifThenModel.MacroIdElse, ifThenModel);
            }
        }

        public async Task CreateSchedule(ProjectModel selectedProject, IfThenModel ifThenModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            if (ifThenModel.ConditionalIds.Any())
            {
                await DeleteConditionals(selectedProject, ifThenModel);
                ifThenModel.ConditionalIds.Clear();
            }

            if (ifThenModel.InstructionIds.Any())
            {
                await DeleteInstructions(selectedProject, ifThenModel);
                _ = ifThenModel.InstructionIds.Any();
            }

            if (ifThenModel.IpCommandIds.Any())
            {
                await DeleteIpCommand(selectedProject, ifThenModel);
                ifThenModel.IpCommandIds.Clear();
            }

            if (ifThenModel.MacroIdThen != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdThen);
                ifThenModel.MacroIdThen = -1;
            }

            if (ifThenModel.MacroIdElse != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdElse);
                ifThenModel.MacroIdElse = -1;
            }

            if (ifThenModel.RuleIds.Any())
            {
                await DeleteRule(selectedProject, ifThenModel.RuleIds);
                ifThenModel.RuleIds.Clear();
            }

            ZwaveDevice zwave = ifThenModel.ZwaveDevicesIf.FirstOrDefault();

            commands.Clear();

            using ReplaySubject<string> replay = new();

            _ = replay.Subscribe(response =>
            {
            });

            string command = string.Empty;

            int condId = -1;

            switch ((TabScheduleEnum)zwave.SelectedIndexTabSchedule)
            {
                #region Timer

                case TabScheduleEnum.Timer:

                    indexConditional = 0;

                    ifThenModel.MacroIdThen = await GetNextMacroId(selectedProject);

                    await SetMacro(selectedProject, ifThenModel.MacroIdThen);

                    int ruleId = await GetNextRuleId(selectedProject);

                    _ = await SetRule(selectedProject, ifThenModel, ruleId);

                    condId = await GetNextConditionalId(selectedProject);

                    ifThenModel.ConditionalIds.Add(condId.ToString());

                    command = $"{{\"command\":\"ITDS\",\"type\":0,\"conditionalId\":{condId}," +
                              $"\"dateType\":{zwave.SelectedIndexDateType}," +
                              $"\"parameters\":[{zwave.ValueSchedule},0,0,0]}}";

                    ;

                    await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

                    //await TryCommands(selectedProject.SelectedGateway, commands, replay);

                    await IfThenAttachConditional(selectedProject, ruleId, condId, LogicGateIfThen.Or);

                    foreach (ZwaveDevice zwaveDevice in ifThenModel.ZwaveDevicesThen)
                    {
                        await CreateThen(selectedProject, zwaveDevice, ifThenModel.MacroIdThen, ifThenModel);
                    }

                    ifThenModel.RuleIds.Add(ruleId);
                    break;

                #endregion Timer

                #region Date and Time

                case TabScheduleEnum.DateTime:

                    TimeZone localZone = TimeZone.CurrentTimeZone;

                    DateTime currentDate = DateTime.Now;

                    TimeSpan currentOffset = localZone.GetUtcOffset(currentDate);

                    DateTime timeZone = zwave.TimePickerValue - currentOffset;

                    bool days = timeZone.Day == currentDate.Day;

                    IEnumerable<DaysOfWeekModel> daysOfWeekList = zwave.DaysOfWeekList.Where(x => x.IsSelected == true);

                    ifThenModel.MacroIdThen = await GetNextMacroId(selectedProject);

                    await SetMacro(selectedProject, ifThenModel.MacroIdThen);

                    if (daysOfWeekList.Count() == 7)
                    {
                        indexConditional = 0;

                        ruleId = await GetNextRuleId(selectedProject);

                        _ = await SetRule(selectedProject, ifThenModel, ruleId);

                        condId = await GetNextConditionalId(selectedProject);

                        ifThenModel.ConditionalIds.Add(condId.ToString());

                        command = $"{{\"command\":\"ITDS\",\"type\":0,\"conditionalId\":{condId}," +
                                  $"\"dateType\":{(int)DateTypeEnum.CompareClock}," +
                                  $"\"parameters\":[{(int)OperatorsTypeSchedule.Equals}," +
                                                  $"{timeZone.Hour}," +
                                                  $"{timeZone.Minute}," +
                                                  $"1]}}";

                        ;

                        await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

                        await IfThenAttachConditional(selectedProject, ruleId, condId, LogicGateIfThen.Or);

                        foreach (ZwaveDevice zwaveDevice in ifThenModel.ZwaveDevicesThen)
                        {
                            await CreateThen(selectedProject, zwaveDevice, ifThenModel.MacroIdThen, ifThenModel);
                        }

                        ifThenModel.RuleIds.Add(ruleId);

                        break;
                    }

                    List<int> ruleIds = new();

                    for (int i = 0; i < daysOfWeekList.Count(); i++)
                    {
                        int rule = await GetNextRuleId(selectedProject);

                        if (ruleIds.Any(x => x == rule))
                        {
                            i--;
                            continue;
                        }

                        if (!await SetRule(selectedProject, ifThenModel, rule))
                        {
                            i--;
                            continue;
                        }

                        ruleIds.Add(rule);
                    }

                    List<int> condIds = new();

                    for (int i = 0; i < daysOfWeekList.Count(); i++)
                    {
                        indexConditional = 0;

                        condId = await GetNextConditionalId(selectedProject);

                        while (condIds.Any(x => x == condId) || condId == -1)
                        {
                            condId = await GetNextConditionalId(selectedProject);
                        }

                        condIds.Add(condId);

                        ifThenModel.ConditionalIds.Add(condId.ToString());

                        command = $"{{\"command\":\"ITDS\",\"type\":0,\"conditionalId\":{condId}," +
                                  $"\"dateType\":{(int)DateTypeEnum.CompareDayWeek}," +
                                  $"\"parameters\":[{(int)OperatorsTypeSchedule.Equals}," +
                                                  $"{ParseCompareDayWeek(zwave, daysOfWeekList.ElementAt(i), currentDate, timeZone)}," +
                                                  $"0," +
                                                  $"0]}}";

                        ;

                        await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

                        //await TryCommands(selectedProject.SelectedGateway, commands, replay);

                        await IfThenAttachConditional(selectedProject, ruleIds[i], condId, LogicGateIfThen.Or);

                        condId = await GetNextConditionalId(selectedProject);

                        while (condIds.Any(x => x == condId) || condId == -1)
                        {
                            condId = await GetNextConditionalId(selectedProject);
                        }

                        condIds.Add(condId);

                        ifThenModel.ConditionalIds.Add(condId.ToString());

                        command = $"{{\"command\":\"ITDS\",\"type\":0,\"conditionalId\":{condId}," +
                                  $"\"dateType\":{(int)DateTypeEnum.CompareClock}," +
                                  $"\"parameters\":[{(int)OperatorsTypeSchedule.Equals}," +
                                                  $"{timeZone.Hour}," +
                                                  $"{timeZone.Minute}," +
                                                  $"1]}}";

                        ;

                        await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

                        await IfThenAttachConditional(selectedProject, ruleIds[i], condId, LogicGateIfThen.And);

                        ifThenModel.RuleIds.Add(ruleIds[i]);
                    }

                    replay.Dispose();

                    foreach (ZwaveDevice zwaveDevice in ifThenModel.ZwaveDevicesThen)
                    {
                        await CreateThen(selectedProject, zwaveDevice, ifThenModel.MacroIdThen, ifThenModel);
                    }

                    break;

                    #endregion Date and Time
            }
        }

        public async Task CreateThen(ProjectModel selectedProject, ZwaveDevice zwaveDevice, int macroId, IfThenModel ifThenModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            string instruction = await GetInstructionByZwaveDevice(selectedProject, zwaveDevice, ifThenModel);

            int instructionId = await SetInstruction(selectedProject, instruction, zwaveDevice.DelayIfThen);

            ifThenModel.InstructionIds.Add(instructionId);

            await AssignInstructionMacro(selectedProject, macroId, instructionId);
        }

        #endregion Create

        #region Delete

        public async Task DeleteAllConditionals(GatewayModel gateway)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            string @return = string.Empty;

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) != UtilIfThen.IFTHENCONDDELETEALL)
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.IFTHENCONDDELETEALL))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            string command = UtilIfThen.IFTHENCONDDELETEALLCOMMAND;

            ;

            await ToCommand(gateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task DeleteAllRules(GatewayModel gateway)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            string @return = string.Empty;

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) != UtilIfThen.IFTHENRULEDELETEALL)
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.IFTHENRULEDELETEALL))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            string command = UtilIfThen.IFTHENRULEDELETEALLCOMMAND;

            ;

            await ToCommand(gateway, commands, replay, command);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task DeleteIfThenFromCloud(ProjectModel selectedProjectModel, IfThenModel ifThen)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            if (ifThen is null)
            {
                throw new ArgumentNullException(nameof(ifThen));
            }

            IEnumerable<ParseObject> @if = ifThen.ZwaveDevicesIf.Select(x => x.ParseObject);
            IEnumerable<ParseObject> @the = ifThen.ZwaveDevicesThen.Select(x => x.ParseObject);
            IEnumerable<ParseObject> @else = ifThen.ZwaveDevicesElse.Select(x => x.ParseObject);

            IEnumerable<ParseObject> result = @if.Concat(@the).Concat(@else).Concat(ifThen.DeletedZwaveDevices);

            await ParseObject.DeleteAllAsync(result);

            await ifThen.ParseObject.DeleteAsync();
        }

        public async Task<bool> DeleteIfThenFromGataway(ProjectModel selectedProject, IfThenModel ifThenModel)
        {
            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            if (ifThenModel.ConditionalIds.Any())
            {
                await DeleteConditionals(selectedProject, ifThenModel);
            }

            if (ifThenModel.InstructionIds.Any())
            {
                await DeleteInstructions(selectedProject, ifThenModel);
            }

            if (ifThenModel.IpCommandIds.Any())
            {
                await DeleteIpCommand(selectedProject, ifThenModel);
            }

            if (ifThenModel.MacroIdThen != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdThen);
            }

            if (ifThenModel.MacroIdElse != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdElse);
            }
            if (ifThenModel.RuleIds.Any())
            {
                await DeleteRule(selectedProject, ifThenModel.RuleIds);
                ifThenModel.RuleIds.Clear();
            }

            return true;
        }

        #endregion Delete

        #region Get

        public async Task GatewayReboot(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            commands.Clear();

            using ReplaySubject<string> replay = new();

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }
                if (resp == "&RIPOK#")
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = UtilRede.REBOOT;

            ;

            await ToCommand(selectedProjectModel.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task GetGatewayClockTime(ProjectModel selectedProject)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            string @return = string.Empty;

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) != UtilIfThen.GETCLOCKTIME)
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.DATE))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();

                if (!DateTime.TryParse(@object.Value<string>(UtilIfThen.DATE), new CultureInfo("en-US", false), DateTimeStyles.None, out DateTime dateTime))
                {
                    return;
                }

                TimeZone localZone = TimeZone.CurrentTimeZone;

                DateTime currentDate = DateTime.Now;

                TimeSpan currentOffset = localZone.GetUtcOffset(currentDate);

                selectedProject.SelectedGateway.ClockTime = dateTime + currentOffset;
            });

            string command = "{\"command\":\"get_clock_time\",\"type\":0}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task GetIfThen(ProjectModel selectedProjectModel, IfThenModel selectedIfThen)
        {
            if (selectedIfThen is null)
            {
                throw new ArgumentNullException(nameof(selectedIfThen));
            }

            _parseService.IsSendingToCloud = true;

            selectedIfThen.ZwaveDevicesIf.Clear();

            selectedIfThen.ZwaveDevicesThen.Clear();

            selectedIfThen.ZwaveDevicesElse.Clear();

            selectedIfThen.ConditionalIds.Clear();

            selectedIfThen.InstructionIds.Clear();

            selectedIfThen.IpCommandIds.Clear();

            selectedIfThen.RuleIds.Clear();

            _ = await selectedIfThen.ParseObject.FetchAsync();

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.IFTHENTYPE))
            {
                selectedIfThen.IfthenType = (IfthenType)selectedIfThen.ParseObject.Get<int>(UtilIfThen.IFTHENTYPE);
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.ISENABLED))
            {
                selectedIfThen.IsEnabled = selectedIfThen.ParseObject.Get<bool>(UtilIfThen.ISENABLED);
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.CONDITIONALIDS))
            {
                IList<object> conditionalIds = selectedIfThen.ParseObject.Get<IList<object>>(UtilIfThen.CONDITIONALIDS);

                foreach (object conditionalId in conditionalIds)
                {
                    selectedIfThen.ConditionalIds.Add(conditionalId.ToString());
                }
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.INSTRUCTIONIDS))
            {
                IList<object> instructionIds = selectedIfThen.ParseObject.Get<IList<object>>(UtilIfThen.INSTRUCTIONIDS);

                foreach (object instructionId in instructionIds)
                {
                    if (int.TryParse(instructionId.ToString(), out int id))
                    {
                        selectedIfThen.InstructionIds.Add(id);
                    }
                }
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.IPCOMMANDIDS))
            {
                IList<object> ipCommandIds = selectedIfThen.ParseObject.Get<IList<object>>(UtilIfThen.IPCOMMANDIDS);

                foreach (object ipCommandId in ipCommandIds)
                {
                    if (int.TryParse(ipCommandId.ToString(), out int id))
                    {
                        selectedIfThen.IpCommandIds.Add(id);
                    }
                }
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.RULEIDS))
            {
                IList<object> ruleIds = selectedIfThen.ParseObject.Get<IList<object>>(UtilIfThen.RULEIDS);

                foreach (object ruleId in ruleIds)
                {
                    if (int.TryParse(ruleId.ToString(), out int id))
                    {
                        selectedIfThen.RuleIds.Add(id);
                    }
                }
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.MACROIDTHEN))
            {
                selectedIfThen.MacroIdThen = selectedIfThen.ParseObject.Get<int>(UtilIfThen.MACROIDTHEN);
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.MACROIDELSE))
            {
                selectedIfThen.MacroIdElse = selectedIfThen.ParseObject.Get<int>(UtilIfThen.MACROIDELSE);
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.NAME))
            {
                selectedIfThen.Name = selectedIfThen.ParseObject.Get<string>(UtilIfThen.NAME);
            }

            IEnumerable<ParseObject> @if = await selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.IF).Query.OrderBy(UtilIfThen.INDEX).FindAsync();

            selectedIfThen.ParseObjectsIf = @if;

            foreach (ParseObject parseObject in @if)
            {
                ZwaveDevice parse = parseObject.ParseObjectToIfThenObject();

                parse.IsChangedDevice = IsChangedDevice(selectedProjectModel, parse);

                selectedIfThen.ZwaveDevicesIf.Add(parse);
            }

            List<ZwaveDevice> ifOrderByDescending = selectedIfThen.ZwaveDevicesIf.OrderByDescending(x => x.Index).ToList();

            if (selectedIfThen.ZwaveDevicesIf.Any())
            {
                selectedIfThen.ZwaveDevicesIf.First().IsHiddenLogicGateIfThen = true;
            }

            IEnumerable<ParseObject> @then = await selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.THEN).Query.OrderBy(UtilIfThen.INDEX).FindAsync();

            selectedIfThen.ParseObjectsThen = @then;

            foreach (ParseObject parseObject in @then)
            {
                ZwaveDevice parse = parseObject.ParseObjectToIfThenObject();

                parse.IsChangedDevice = IsChangedDevice(selectedProjectModel, parse);

                selectedIfThen.ZwaveDevicesThen.Add(parse);
            }

            if (selectedIfThen.ZwaveDevicesThen.Any())
            {
                selectedIfThen.ZwaveDevicesThen.Last().IsHiddenDelay = true;
            }

            IEnumerable<ParseObject> @else = await selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.ELSE).Query.OrderBy(UtilIfThen.INDEX).FindAsync();

            selectedIfThen.ParseObjectsElse = @else;

            foreach (ParseObject parseObject in @else)
            {
                ZwaveDevice parse = parseObject.ParseObjectToIfThenObject();

                parse.IsChangedDevice = IsChangedDevice(selectedProjectModel, parse);

                selectedIfThen.ZwaveDevicesElse.Add(parse);
            }

            if (selectedIfThen.ZwaveDevicesElse.Any())
            {
                selectedIfThen.ZwaveDevicesElse.Last().IsHiddenDelay = true;
            }

            _parseService.IsSendingToCloud = false;
        }

        public async Task GetIfThens(ProjectModel selectedProjectModel, ObservableCollection<IfThenModel> ifThenModels, IfthenType ifthenType = IfthenType.Default)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            if (ifThenModels is null)
            {
                throw new ArgumentNullException(nameof(ifThenModels));
            }

            _gatewayService.IsSendingToGateway = true;

            if (ifThenModels.Any())
            {
                ifThenModels.Clear();
            }

            IEnumerable<ParseObject> query = await selectedProjectModel.ParseObject.GetRelation<ParseObject>(UtilIfThen.IFTHENS).Query.WhereContains(UtilIfThen.GATEWAYID, selectedProjectModel.SelectedGateway.UID).FindAsync();

            foreach (ParseObject parseObject in query.Where(x => x.Get<int>(UtilIfThen.IFTHENTYPE) == (int)ifthenType))
            {
                ifThenModels.Add(parseObject.ParseObjectToIfThenModel());
            }

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task<int> GetNextConditionalId(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            int conditionalId = -1;

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            string @return = string.Empty;

            _ = replay.Subscribe(response =>
              {
                  if (string.IsNullOrEmpty(response))
                  {
                      return;
                  }

                  if (!response.TryParseJObject(out @object))
                  {
                      return;
                  }

                  if (!@object.ContainsKey(UtilIfThen.COMMAND))
                  {
                      return;
                  }

                  if (@object.Value<string>(UtilIfThen.COMMAND) != UtilIfThen.IFTHENCONDUSED)
                  {
                      return;
                  }
                  if (@object.TryGetIntArray("list", out int[] list))
                  {
                      _taskService.CancellationTokenSource.Cancel();
                      if (!list.Any())
                      {
                          throw new Exception(Domain.Properties.Resources.Full_memory_delete_some_Schedule_or_If_Then);
                      }
                      Random rnd = new();
                      int index = rnd.Next(list.Length);
                      conditionalId = list[index]; ;
                  }
              });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.IFTHENCONDUSEDCOM);

            if (conditionalId == -1)
            {
                throw new Exception(Properties.Resources.Could_not_get_next_unused_conditional_Id);
            }

            replay.Dispose();

            return conditionalId;
        }

        public async Task<int> GetNextInstructionId(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            int @return = -1;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.NEXTINSTRUCTIONID)
                {
                    _taskService.CancellationTokenSource.Cancel();

                    @return = @object.Value<int>(UtilIfThen.INSTRUCTIONID);

                    return;
                }
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.GETNEXTINSTRUCTIONIDCOM);

            replay.Dispose();

            return @return;
        }

        public async Task<int> GetNextMacroId(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            int @return = -1;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.NEXTMACROID)
                {
                    _taskService.CancellationTokenSource.Cancel();

                    @return = @object.Value<int>(UtilIfThen.MACROID);

                    return;
                }
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.GETNEXTMACROIDCOM);

            replay.Dispose();

            return @return == -1 ? throw new Exception(Properties.Resources.Failed_to_get_the_next_macro_Id) : @return;
        }

        public async Task<int> GetNextRuleId(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            int @return = -1;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) != UtilIfThen.ITRU)
                {
                    return;
                }
                if (@object.TryGetIntArray("list", out int[] list))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    if (!list.Any())
                    {
                        throw new Exception(Domain.Properties.Resources.Full_memory_delete_some_Schedule_or_If_Then);
                    }
                    Random rnd = new();
                    int index = rnd.Next(list.Length);
                    @return = list[index];
                }
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.ITRUCOM);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            return @return == -1 ? throw new Exception(Properties.Resources.Unable_to_get_next_unused_rule_Id) : @return;
        }

        private int GetByte3(ZwaveDevice selectedZwaveDevice)
        {
            return selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass == 16
                ? selectedZwaveDevice.CustomId == ZwaveModelUtil.FXA0404 ||
                    selectedZwaveDevice.CustomId == ZwaveModelUtil.FXA5029
                    ? 37
                    : 32
                : 38;
        }

        private int GetEndpointToValueIfThen(ZwaveDevice selectedZwaveDevice, int min1 = 0, int max1 = 100, int min2 = 27, int max2 = 99)
        {
            return selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass == 16
                           ? selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? 255 : 0
                           : (int)selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel.ConvertRange(min1, max1, min2, max2);
        }

        private async Task<string> GetInstructionByZwaveDevice(ProjectModel selectedProject, ZwaveDevice selectedZwaveDevice, IfThenModel ifThenModel)
        {
            if (selectedZwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedZwaveDevice));
            }

            #region RTS

            if (selectedZwaveDevice.IfthenType == IfthenType.RTS)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    using IpCommandModel ipCommand = new()
                    {
                        Name = GetNameIpCommand(ifThenModel, selectedProject),
                        Command = $"{{\"command\":\"playRts\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId}, \"button\":{(int)selectedZwaveDevice.ActionsRTSSomfy}, \"type\":0}}",
                        TargetIp = gateway.LocalIP,
                        TargetPort = gateway.LocalPortUDP
                    };

                    int memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand);

                    ifThenModel.IpCommandIds.Add(memoryId);

                    return $"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}".Replace("\"", "\\\"");
                }

                throw new Exception(Properties.Resources.Gateways_Found_Network);
            }

            #endregion RTS

            #region Radio433

            if (selectedZwaveDevice.IfthenType == IfthenType.Radio433)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    using IpCommandModel ipCommand = new()
                    {
                        Name = GetNameIpCommand(ifThenModel, selectedProject),
                        Command = $"{{\"command\":\"playRadioMemory\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId}, \"type\":0}}",
                        TargetIp = gateway.LocalIP,
                        TargetPort = gateway.LocalPortUDP
                    };

                    int memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand);

                    ifThenModel.IpCommandIds.Add(memoryId);

                    return $"{{\"command\":\"play_memory_ip_command\",\"type\":0,\"id\":{memoryId}}}".Replace("\"", "\\\"");
                }

                throw new Exception(Properties.Resources.Gateways_Found_Network);
            }

            #endregion Radio433

            #region IR

            if (selectedZwaveDevice.IfthenType == IfthenType.IR)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    if (gateway.UID == selectedProject.SelectedGateway.UID)
                    {
                        return $"{{\"command\":\"irPlayMemoryCode\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId},\"type\":0}}".Replace("\"", "\\\"");
                    }
                    else
                    {
                        using IpCommandModel ipCommand = new()
                        {
                            Name = GetNameIpCommand(ifThenModel, selectedProject),
                            Command = $"{{\"command\":\"irPlayMemoryCode\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId},\"type\":0}}",
                            TargetIp = gateway.LocalIP,
                            TargetPort = gateway.LocalPortUDP
                        };

                        int memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand);

                        ifThenModel.IpCommandIds.Add(memoryId);

                        return $"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}".Replace("\"", "\\\"");
                    }
                }

                throw new Exception(Properties.Resources.Gateway_Did_Not_Respond);
            }

            #endregion IR

            #region IP Command

            if (selectedZwaveDevice.IfthenType == IfthenType.IPCommand)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    if (gateway.UID == selectedProject.SelectedGateway.UID)
                    {
                        return $"{{\"command\":\"play_memory_ip_command\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId},\"type\":0}}".Replace("\"", "\\\"");
                    }
                    else
                    {
                        using IpCommandModel ipCommand = new()
                        {
                            Name = GetNameIpCommand(ifThenModel, selectedProject),
                            Command = $"{{\"command\":\"play_memory_ip_command\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId},\"type\":0}}",
                            TargetIp = gateway.LocalIP,
                            TargetPort = gateway.LocalPortUDP
                        };

                        int memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand);

                        selectedProject.SelectedIfThen.IpCommandIds.Add(memoryId);

                        return $"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}".Replace("\"", "\\\"");
                    }
                }

                throw new Exception(Properties.Resources.Gateway_Did_Not_Respond);
            }

            #endregion IP Command

            #region Serial

            if (selectedZwaveDevice.IfthenType == IfthenType.Serial)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    if (gateway.UID == selectedProject.SelectedGateway.UID)
                    {
                        return $"{{\"command\":\"serialCommandPlayMemory\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId},\"type\":0}}".Replace("\"", "\\\"");
                    }
                    else
                    {
                        using IpCommandModel ipCommand = new()
                        {
                            Name = GetNameIpCommand(ifThenModel, selectedProject),
                            Command = $"{{\"command\":\"serialCommandPlayMemory\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId},\"type\":0}}",
                            TargetIp = gateway.LocalIP,
                            TargetPort = gateway.LocalPortUDP
                        };

                        int memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand);

                        selectedProject.SelectedIfThen.IpCommandIds.Add(memoryId);

                        return $"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}".Replace("\"", "\\\"");
                    }
                }

                throw new Exception(Properties.Resources.Gateway_Did_Not_Respond);
            }

            #endregion Serial

            #region Relay

            if (selectedZwaveDevice.IfthenType == IfthenType.Relay)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    if (gateway.UID == selectedProject.SelectedGateway.UID)
                    {
                        return $"{{\"command\":\"set_relay_output\",\"type\":0,\"value\":{(selectedZwaveDevice.StateRelay ? 1 : 0)},\"mode\": {selectedZwaveDevice.SelectedIndexRelayStateMode},\"time\":{selectedZwaveDevice.RelayPulseTime}}}".Replace("\"", "\\\"");
                    }
                    else
                    {
                        using IpCommandModel ipCommand = new()
                        {
                            Name = GetNameIpCommand(ifThenModel, selectedProject),
                            Command = $"{{\"command\":\"set_relay_output\",\"type\":0,\"value\":{(selectedZwaveDevice.StateRelay ? 1 : 0)},\"mode\": {selectedZwaveDevice.SelectedIndexRelayStateMode},\"time\":{selectedZwaveDevice.RelayPulseTime}}}",
                            TargetIp = gateway.LocalIP,
                            TargetPort = gateway.LocalPortUDP
                        };

                        int memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand);

                        selectedProject.SelectedIfThen.IpCommandIds.Add(memoryId);

                        return $"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}".Replace("\"", "\\\"");
                    }
                }

                throw new Exception(Properties.Resources.Gateway_Did_Not_Respond);
            }

            #endregion Relay

            #region Zwave Device

            if (ZwaveModelUtil.FXC222 == selectedZwaveDevice.CustomId)
            {
                long? genericDeviceClass = selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass;

                if (genericDeviceClass == 17)
                {
                    return $"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\":{selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel}}}".Replace("\"", "\\\"");
                }
                else if (genericDeviceClass == 16)
                {
                    return $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{(selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? 255 : 0)}}}".Replace("\"", "\\\"");
                }
            }
            else if (ZwaveModelUtil.ONOFF == selectedZwaveDevice.CustomId)
            {
                return (EndpointState)selectedZwaveDevice.StateIndex switch
                {
                    EndpointState.On => $"{{\"command\":\"basicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"level\": 255}}".Replace("\"", "\\\""),
                    EndpointState.Off => $"{{\"command\":\"basicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"level\": 0}}".Replace("\"", "\\\""),
                    EndpointState.Toggle => "",//todo falta implementar o comando
                    _ => "",
                };
            }
            else if (ZwaveModelUtil.FXD5011 == selectedZwaveDevice.CustomId ||
                     ZwaveModelUtil.FXD211 == selectedZwaveDevice.CustomId ||
                     ZwaveModelUtil.FXD211B == selectedZwaveDevice.CustomId)
            {
                return $"{{\"command\":\"multilevelSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"level\": {selectedZwaveDevice.MultiLevel.ConvertRange(0, 100, 0, 63)}}}".Replace("\"", "\\\"");
            }
            else if (ZwaveModelUtil.FXD220 == selectedZwaveDevice.CustomId)
            {
                return $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{selectedZwaveDevice.MultiLevel.ConvertRange(0, 100, 0, 63)}}}".Replace("\"", "\\\"");
            }
            else if (ZwaveModelUtil.FXR211 == selectedZwaveDevice.CustomId)
            {
                return $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{(selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? 255 : 0)}}}".Replace("\"", "\\\"");
            }
            else if (selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5011) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5012) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXR5013) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5014) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA3011) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA0404) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA3012) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5016) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5029) ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA0400) ||

                     selectedZwaveDevice.CustomId == ZwaveModelUtil.FXA0404 ||
                     selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXA5018))
            {
                long? genericDeviceClass = selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass;

                if (genericDeviceClass == 17)
                {
                    return $"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\":{selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel.ConvertRange()}}}".Replace("\"", "\\\"");
                }
                else if (genericDeviceClass == 16)
                {
                    return (EndpointState)selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).EndpointStateIndex switch
                    {
                        EndpointState.On => $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\": 255}}".Replace("\"", "\\\""),
                        EndpointState.Off => $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\": 0}}".Replace("\"", "\\\""),
                        EndpointState.Toggle => "",//todo falta implementar o comando
                        _ => "",
                    };
                }
            }
            else if (selectedZwaveDevice.CustomId == ZwaveModelUtil.FXA0600)
            {
                return selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOnOff
                    ? (EndpointState)selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).EndpointStateIndex switch
                    {
                        EndpointState.On => $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\": 255}}".Replace("\"", "\\\""),
                        EndpointState.Off => $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\": 0}}".Replace("\"", "\\\""),
                        EndpointState.Toggle => "",//todo falta implementar o comando
                        _ => "",
                    }
                    : $"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\":{selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel.ConvertRange()}}}".Replace("\"", "\\\"");
            }
            else if (selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXS69A))
            {
                return $"{{\"command\":\"binarySwitchSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"level\":{(selectedZwaveDevice.IsOn ? 255 : 0)}}}".Replace("\"", "\\\"");
            }

            if (selectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.ZXT600))
            {
                return GetCommandZXT600(selectedZwaveDevice).Replace("\"", "\\\"");
            }

            #endregion Zwave Device

            throw new Exception(Properties.Resources.Device_is_not_yet_supported_in_Then_Else);
        }

        private string GetCommandZXT600(ZwaveDevice zwaveDevice)
        {
            string command = string.Empty;

            switch (zwaveDevice.SelectedIndexThermostatFunction)
            {
                case 0:

                    string roomTemperature = zwaveDevice.RoomTemperature.ToString();

                    command = $"{{\"command\":\"thermostatTemperatureSet\", \"type\":0,{(zwaveDevice.IsEncrypted ? "\"crypto\":true," : null)} \"moduleId\":{zwaveDevice.ModuleId},\"temperature\":{roomTemperature.Remove(roomTemperature.Length - 1)}}}";

                    break;

                case 1:

                    int selectedIndexThermostatMode = zwaveDevice.SelectedIndexThermostatMode;

                    ThermostatModeEnum enumMode = (ThermostatModeEnum)zwaveDevice.ThermostatModes.ElementAt(selectedIndexThermostatMode).Code;

                    int thermostatMode = (int)enumMode;

                    command = $"{{\"command\":\"thermostatModeSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},{(zwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"mode\":{thermostatMode}}}";
                    break;

                case 2:

                    int selectedIndexThermostatFan = zwaveDevice.SelectedIndexThermostatFan;

                    ThermostatFanEnum @enumFan = (ThermostatFanEnum)zwaveDevice.ThermostatFans.ElementAt(selectedIndexThermostatFan).Code;

                    int thermostatFan = (int)@enumFan;

                    command = $"{{\"command\":\"thermostatFanModeSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},{(zwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"mode\":{thermostatFan}}}";
                    break;

                default:
                    break;
            }

            return command;
        }

        private string GetIZTSJson(ZwaveDevice selectedZwaveDevice, int conditionalId)
        {
            if (ZwaveModelUtil.DomeMotionSensor == selectedZwaveDevice.CustomId)
            {
                switch ((SensorType)selectedZwaveDevice.SelectedTabIndexSensorType)
                {
                    case SensorType.Motion:
                        return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                         $"\"commandClass\":48," +
                                         $"\"params0\":[7,0,3]," +
                                         $"\"params1\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                      $"1," +
                                                      $"{(selectedZwaveDevice.IsOn ? 255 : 0)}]," +
                                         $"\"params2\":[0,0,0]," +
                                         $"\"params3\":[0,0,0]," +
                                         $"\"params4\":[0,0,0]," +
                                         $"\"params5\":[0,0,0]," +
                                         $"\"params6\":[0,0,0]," +
                                         $"\"params7\":[0,0,0]}}";

                    case SensorType.Light:
                        return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                         $"\"commandClass\":49," +
                                         $"\"params0\":[7,0,5]," +
                                         $"\"params1\":[7,1,3]," +
                                         $"\"params2\":[7,2,10]," +
                                         $"\"params3\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                      $"3," +
                                                      $"{selectedZwaveDevice.MultiLevel}]," +
                                         $"\"params4\":[0,0,0]," +
                                         $"\"params5\":[0,0,0]," +
                                         $"\"params6\":[0,0,0]," +
                                         $"\"params7\":[0,0,0]}}";
                }
            }
            else if (ZwaveModelUtil.DomeDoorWindowSensor == selectedZwaveDevice.CustomId)
            {
                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":113," +
                                                         $"\"params0\":[7,0,5]," +
                                                         $"\"params1\":[7,1,0]," +
                                                         $"\"params2\":[7,2,0]," +
                                                         $"\"params3\":[7,3,0]," +
                                                         $"\"params4\":[7,4,255]," +
                                                         $"\"params5\":[7,5,6]," +
                                                         $"\"params6\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"6," +
                                                                      $"{(selectedZwaveDevice.IsOn ? 23 : 22)}]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXR5011 == selectedZwaveDevice.CustomId ||
                     ZwaveModelUtil.FXR5012 == selectedZwaveDevice.CustomId ||
                     ZwaveModelUtil.FXR5013 == selectedZwaveDevice.CustomId)
            {
                int @value = selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).EndpointStateIndex == 0 ? 0 : 255;

                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":96," +
                                                         $"\"params0\":[7,0,13]," +
                                                         $"\"params1\":[7,1," +
                                                                      $"{selectedZwaveDevice.SelectedIndexEndpoint + 1}]," +
                                                         $"\"params2\":[7,2,0]," +
                                                         $"\"params3\":[0,3,20]," +
                                                         $"\"params4\":[7,4,3]," +
                                                         $"\"params5\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"5," +
                                                                      $"{@value}]," +
                                                         $"\"params6\":[0,0,0]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXA5029 == selectedZwaveDevice.CustomId)
            {
                int @value = GetEndpointToValueIfThen(selectedZwaveDevice);

                int byte3 = GetByte3(selectedZwaveDevice);

                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":96," +
                                                         $"\"params0\":[7,0,13]," +
                                                         $"\"params1\":[7,1," +
                                                                      $"{selectedZwaveDevice.SelectedIndexEndpoint + 1}]," +
                                                         $"\"params2\":[7,2,0]," +
                                                         $"\"params3\":[7,3,{byte3}]," +
                                                         $"\"params4\":[7,4,3]," +
                                                         $"\"params5\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"5," +
                                                                      $"{@value}]," +
                                                         $"\"params6\":[0,0,0]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXD211 == selectedZwaveDevice.CustomId ||
                     ZwaveModelUtil.FXD211B == selectedZwaveDevice.CustomId)
            {
                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":65," +
                                                         $"\"params0\":[7,0,3]," +
                                                         $"\"params1\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"1," +
                                                                      $"{selectedZwaveDevice.MultiLevel.ConvertRange()}]," +
                                                         $"\"params2\":[0,0,0]," +
                                                         $"\"params3\":[0,0,0]," +
                                                         $"\"params4\":[0,0,0]," +
                                                         $"\"params5\":[0,0,0]," +
                                                         $"\"params6\":[0,0,0]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXD5011 == selectedZwaveDevice.CustomId)
            {
                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":32," +
                                                         $"\"params0\":[7,0,3]," +
                                                         $"\"params1\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"1," +
                                                                      $"{selectedZwaveDevice.MultiLevel.ConvertRange()}]," +
                                                         $"\"params2\":[0,0,0]," +
                                                         $"\"params3\":[0,0,0]," +
                                                         $"\"params4\":[0,0,0]," +
                                                         $"\"params5\":[0,0,0]," +
                                                         $"\"params6\":[0,0,0]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXR211 == selectedZwaveDevice.CustomId)
            {
                int @value = GetEndpointToValueIfThen(selectedZwaveDevice);

                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":32," +
                                                         $"\"params0\":[7,0,3]," +
                                                         $"\"params1\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"1," +
                                                                      $"{@value}]," +
                                                         $"\"params2\":[0,0,0]," +
                                                         $"\"params3\":[0,0,0]," +
                                                         $"\"params4\":[0,0,0]," +
                                                         $"\"params5\":[0,0,0]," +
                                                         $"\"params6\":[0,0,0]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXS69A == selectedZwaveDevice.CustomId)
            {
                int @value = GetEndpointToValueIfThen(selectedZwaveDevice);

                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":37," +
                                                         $"\"params0\":[7,0,3]," +
                                                         $"\"params1\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"1," +
                                                                      $"{@value}]," +
                                                         $"\"params2\":[0,0,0]," +
                                                         $"\"params3\":[0,0,0]," +
                                                         $"\"params4\":[0,0,0]," +
                                                         $"\"params5\":[0,0,0]," +
                                                         $"\"params6\":[0,0,0]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXD220 == selectedZwaveDevice.CustomId)
            {
                int @value = GetEndpointToValueIfThen(selectedZwaveDevice);

                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":65," +
                                                         $"\"params0\":[7,0,3]," +
                                                         $"\"params1\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"1," +
                                                                      $"{@value}]," +
                                                         $"\"params2\":[0,0,0]," +
                                                         $"\"params3\":[0,0,0]," +
                                                         $"\"params4\":[0,0,0]," +
                                                         $"\"params5\":[0,0,0]," +
                                                         $"\"params6\":[0,0,0]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXC222 == selectedZwaveDevice.CustomId)
            {
                int @value = GetEndpointToValueIfThen(selectedZwaveDevice);

                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":96," +
                                                         $"\"params0\":[7,0,17]," +
                                                         $"\"params1\":[0,1,0]," +
                                                         $"\"params2\":[7,2,0]," +
                                                         $"\"params3\":[7,3,37]," +
                                                         $"\"params4\":[7,4,3]," +
                                                         $"\"params5\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"5," +
                                                                      $"{@value}]," +
                                                         $"\"params6\":[0,0,0]," +
                                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXA5014 == selectedZwaveDevice.CustomId)
            {
                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                                         $"\"commandClass\":96," +
                                                         $"\"params0\":[0,0,0]," +
                                                         $"\"params1\":[7,1,{selectedZwaveDevice.SelectedIndexEndpoint + 1}]," +
                                                         $"\"params2\":[0,2,0]," +
                                                         $"\"params3\":[0,3,4]," +
                                                         $"\"params4\":[0,4,0]," +
                                                         $"\"params5\":[{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                                      $"5," +
                                                                      $"{(selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? 255 : 0)}]," +
                                                         $"\"params6\":[0,6,0]," +
                                                         $"\"params7\":[0,7,0]}}";
            }
            else if (ZwaveModelUtil.FXA0404 == selectedZwaveDevice.CustomId ||
                     ZwaveModelUtil.FXA0600 == selectedZwaveDevice.CustomId)
            {
                long value = selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOnOff ?
                             selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? 255 : 0 :
                             selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel.ConvertRange();
                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                         $"\"commandClass\":96," +
                                         $"\"params0\":[0,0,0]," +
                                         $"\"params1\":[7,1,{selectedZwaveDevice.SelectedIndexEndpoint + 1}]," +
                                         $"\"params2\":[0,0,0]," +
                                         $"\"params3\":[0,0,0]," +
                                         $"\"params4\":[" +
                                                      $"{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                      $"5," +
                                                      $"{value}" +
                                                      $"]," +
                                         $"\"params5\":[0,0,0]," +
                                         $"\"params6\":[0,0,0]," +
                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.FXA0400 == selectedZwaveDevice.CustomId)
            {
                long value = selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass == 16 ?
                             selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? 255 : 0 :
                             selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel.ConvertRange();
                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                         $"\"commandClass\":96," +
                                         $"\"params0\":[0,0,0]," +
                                         $"\"params1\":[7,1,{selectedZwaveDevice.SelectedIndexEndpoint + 1}]," +
                                         $"\"params2\":[0,0,0]," +
                                         $"\"params3\":[0,0,0]," +
                                         $"\"params4\":[" +
                                                      $"{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                      $"5," +
                                                      $"{value}" +
                                                      $"]," +
                                         $"\"params5\":[0,0,0]," +
                                         $"\"params6\":[0,0,0]," +
                                         $"\"params7\":[0,0,0]}}";
            }
            else if (
                ZwaveModelUtil.FXA3011 == selectedZwaveDevice.CustomId ||
                ZwaveModelUtil.FXA3012 == selectedZwaveDevice.CustomId ||
                ZwaveModelUtil.FXA5018 == selectedZwaveDevice.CustomId ||
                ZwaveModelUtil.FXA5016 == selectedZwaveDevice.CustomId
                )
            {
                long value = /*selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOnOff ?*/
                             selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).EndpointStateIndex == 0 ? 0 : 255;
                /*selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel.ConvertRange();*/
                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                         $"\"commandClass\":96," +
                                         $"\"params0\":[7,0,13]," +
                                         $"\"params1\":[7,1,{selectedZwaveDevice.SelectedIndexEndpoint + 1}]," +
                                         $"\"params2\":[7,2,0]," +
                                         $"\"params3\":[7,3,37]," +
                                         $"\"params4\":[7,4,3]," +
                                         $"\"params5\":[" +
                                                      $"{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                      $"5," +
                                                      $"{value}" +
                                                      $"]," +
                                         $"\"params6\":[0,0,0]," +
                                         $"\"params7\":[0,0,0]}}";
            }
            else if (ZwaveModelUtil.ZXT600 == selectedZwaveDevice.CustomId)
            {
                return $"{{\"command\":\"ITZS\",\"type\":0,\"conditionalId\":{conditionalId}," +
                                         $"\"deviceId\":{selectedZwaveDevice.ModuleId}," +
                                         $"\"commandClass\":49," +
                                         $"\"params0\":[{(int)OperatorTypeITZS.IgnoreIfDifferent},0,5]," +
                                         $"\"params1\":[{(int)OperatorTypeITZS.IgnoreIfDifferent},1,1]," +
                                         $"\"params2\":[{(int)OperatorTypeITZS.IgnoreIfDifferent},2,34]," +
                                         $"\"params3\":[{(int)OperatorTypeITZS.IgnoreIfDifferent},3,0]," +
                                         $"\"params4\":[" +
                                                      $"{selectedZwaveDevice.SelectedIndexOperatorType + 1}," +
                                                      $"4," +
                                                      $"{selectedZwaveDevice.RoomTemperature}" +
                                                      $"]," +
                                         $"\"params5\":[0,0,0]," +
                                         $"\"params6\":[0,0,0]," +
                                         $"\"params7\":[0,0,0]}}";
            }

            return string.Empty;
        }

        private string GetNameIpCommand(IfThenModel ifThenModel, ProjectModel selectedProject)
        {
            int max = 30;

            string name, suffix = "...";

            if (ifThenModel.IfthenType == IfthenType.Default)
            {
                name = $"{Properties.Resources.Used_by_ifthen} {selectedProject.SelectedIfThen.Name}";
                return name.Length > max ? $"{name.Substring(0, max - suffix.Length)}{suffix}" : name;
            }

            name = $"{Properties.Resources.Used_by_schedule} {selectedProject.SelectedIfThen.Name}";
            return name.Length > max ? $"{name.Substring(0, max - suffix.Length)}{suffix}" : name;
        }

        #endregion Get

        #region Set

        public async Task<int> SetInstruction(ProjectModel selectedProject, string instruction, int? delay = 0)
        {
            int instructionId = -1;

            instructionId = await GetNextInstructionId(selectedProject);

            while (instructionId == -1)
            {
                instructionId = await GetNextInstructionId(selectedProject);
            }

            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            _ = replay.Subscribe(response =>
              {
                  if (string.IsNullOrEmpty(response))
                  {
                      return;
                  }

                  if (!response.TryParseJObject(out @object))
                  {
                      return;
                  }

                  if (!@object.ContainsKey(UtilIfThen.COMMAND))
                  {
                      return;
                  }

                  if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.SETINSTRUCTION)
                  {
                      _taskService.CancellationTokenSource.Cancel();
                      return;
                  }
              });

            string command = $"{{\"command\":\"set_instruction\",\"id\":{instructionId},\"delay\":{delay},\"instruction\":\"{instruction}\", \"type\": 0}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return instructionId;
        }

        public async Task SetMacro(ProjectModel selectedProject, int macroId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.SETMACRO)
                {
                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\":\"set_macro\",\"macro_id\":{macroId},\"type\":0}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task<string> SetRule(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            string @return = string.Empty;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (response.Contains("&ITRU"))
                {
                    @return = response.Replace("&ITRU", "").Replace("#", "").StringToNextRuleOrConditionalFree();
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.GETNEXTRULEIDCOM);

            replay.Dispose();

            return @return;
        }

        private async Task<bool> SetRule(ProjectModel selectedProject, IfThenModel ifThenModel, int ruleId)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.IFTHENRULESSET)
                {
                    @return = true;
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"if_then_rules_set\",\"type\":0,\"ruleId\":{ruleId},\"macroId\":{ifThenModel.MacroIdThen},\"useElse\":{ifThenModel.ZwaveDevicesElse.Any().ToString().ToLower()},\"elseMacroId\":{ifThenModel.MacroIdElse}}}";

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return @return;
        }

        #endregion Set

        #region Delete

        private async Task DeleteConditionals(ProjectModel selectedProject, IfThenModel ifThenModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
            });

            ;

            foreach (string conditionalId in ifThenModel.ConditionalIds)
            {
                string command = string.Empty;

                if (int.TryParse(conditionalId, out int id))
                {
                    if (id < 0)
                    {
                        continue;
                    }
                    command = $"{{\"command\":\"if_then_cond_delete\",\"type\":0,\"id\":{id}}}";
                }
                else
                {
                    command = $"@ITCD{conditionalId}#";
                }
                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);
            }

            replay.Dispose();

            ifThenModel.ConditionalIds.Clear();
        }

        private async Task DeleteInstructions(ProjectModel selectedProject, IfThenModel ifThenModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
            });

            string command = string.Empty;

            ;

            foreach (int id in ifThenModel.InstructionIds)
            {
                command = $"{{\"command\":\"erase_instruction\",\"instruction_id\":{id}, \"type\":0}}";

                ;

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);
            }

            replay.Dispose();

            ifThenModel.InstructionIds.Clear();
        }

        private async Task DeleteIpCommand(ProjectModel selectedProject, IfThenModel ifThenModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
            });

            string command = string.Empty;

            foreach (int id in ifThenModel.IpCommandIds)
            {
                command = $"{{\"command\":\"erase_memory_ip_command\",\"type\":0,\"id\":{id}}}";

                ;

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);
            }

            foreach (int id in ifThenModel.IpCommandIds)
            {
                _ = selectedProject.SelectedGateway.IpCommands.Remove(selectedProject.SelectedGateway.IpCommands.FirstOrDefault(x => x.MemoryId == id));
            }

            replay.Dispose();

            ifThenModel.IpCommandIds.Clear();

            _gatewayService.IsSendingToGateway = false;
        }

        private async Task DeleteMacro(ProjectModel selectedProject, int macroId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
            });

            string command = $"{{\"command\":\"erase_macro\",\"macro_id\":{macroId}, \"type\":0}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        private async Task DeleteRule(ProjectModel selectedProject, IList<int> ruleIds)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
            });

            foreach (int ruleId in ruleIds)
            {
                string command = $"{{\"command\":\"if_then_rule_delete\",\"type\":0,\"ruleId\":{ruleId}}}";

                ;

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);
            }

            replay.Dispose();
        }

        #endregion Delete

        public async Task PlayMacroAsync(ProjectModel selectedProject, int macroId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.PLAYMACRO)
                {
                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\":\"play_macro\",\"macro_id\":{macroId},\"type\": 0}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task Rename(IfThenModel selectedIfThenModel)
        {
            if (selectedIfThenModel is null)
            {
                throw new ArgumentNullException(nameof(selectedIfThenModel));
            }
            _parseService.IsSendingToCloud = true;

            selectedIfThenModel.ParseObject[UtilIfThen.NAME] = selectedIfThenModel.Name;

            await selectedIfThenModel.ParseObject.SaveAsync();

            _parseService.IsSendingToCloud = false;
        }

        public async Task ResetDevice(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            commands.Clear();

            using ReplaySubject<string> replay = new();

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (response.Contains("&"))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            const int DELETE_NUMBER = 30;

            ;

            for (int i = 0; i <= DELETE_NUMBER; i++)
            {
                string command = $"{{\"command\":\"erase_instruction\",\"instruction_id\":{i}, \"type\":0}}";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

                command = $"{{\"command\":\"erase_macro\",\"macro_id\":{i}, \"type\":0}}";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

                command = $"@ITCD{i:X2}#";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

                command = $"@ITRD{i:X2}#";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);
            }

            replay.Dispose();
        }

        public async Task RuleIdEnabled(ProjectModel selectedProject, IfThenModel selectedIfThenModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (selectedIfThenModel is null)
            {
                throw new ArgumentNullException(nameof(selectedIfThenModel));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            string @return = string.Empty;

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.IFTHENATTACHCOND)
                {
                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            for (int i = 0; i < selectedIfThenModel.RuleIds.Count; i++)
            {
                string command = $"{{\"command\":\"if_then_rule_enable\",\"type\":0,\"ruleId\":{selectedIfThenModel.RuleIds.ElementAt(i)},\"isEnabled\":{selectedIfThenModel.IsEnabled.ToString().ToLower()}}}";

                ;

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command);
            }

            replay.Dispose();
        }

        public async Task StopMacroAsync(ProjectModel selectedProject, int macroId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.PLAYMACRO)
                {
                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\": \"stop_macro\",\"macro_id\": {macroId}, \"type\": 0}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task Update(ProjectModel selectedProjectModel, IfThenModel selectedIfThen)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            if (selectedIfThen is null)
            {
                throw new ArgumentNullException(nameof(selectedIfThen));
            }

            _parseService.IsSendingToCloud = true;

            if (selectedIfThen.ParseObjectsIf.Any())
            {
                foreach (ParseObject parseObject in selectedIfThen.ParseObjectsIf)
                {
                    selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.IF).Remove(parseObject);
                }
            }

            if (selectedIfThen.ParseObjectsThen.Any())
            {
                foreach (ParseObject parseObject in selectedIfThen.ParseObjectsThen)
                {
                    selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.THEN).Remove(parseObject);
                }
            }

            if (selectedIfThen.ParseObjectsElse.Any())
            {
                foreach (ParseObject parseObject in selectedIfThen.ParseObjectsElse)
                {
                    selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.ELSE).Remove(parseObject);
                }
            }

            if (selectedIfThen.DeletedZwaveDevices.Any())
            {
                await ParseObject.DeleteAllAsync(selectedIfThen.DeletedZwaveDevices);

                selectedIfThen.DeletedZwaveDevices.Clear();
            }

            selectedIfThen.ParseObject ??= new ParseObject(UtilIfThen.CLASSNAME);
            selectedIfThen.ParseObject[UtilIfThen.GATEWAYID] = selectedProjectModel.SelectedGateway.UID;
            selectedIfThen.ParseObject[UtilIfThen.CONDITIONALIDS] = selectedIfThen.ConditionalIds;
            selectedIfThen.ParseObject[UtilIfThen.INSTRUCTIONIDS] = selectedIfThen.InstructionIds;
            selectedIfThen.ParseObject[UtilIfThen.IPCOMMANDIDS] = selectedIfThen.IpCommandIds;
            selectedIfThen.ParseObject[UtilIfThen.RULEIDS] = selectedIfThen.RuleIds;
            selectedIfThen.ParseObject[UtilIfThen.MACROIDTHEN] = selectedIfThen.MacroIdThen;
            selectedIfThen.ParseObject[UtilIfThen.MACROIDELSE] = selectedIfThen.MacroIdElse;
            selectedIfThen.ParseObject[UtilIfThen.NAME] = selectedIfThen.Name;
            selectedIfThen.ParseObject[UtilIfThen.ISENABLED] = selectedIfThen.IsEnabled;
            selectedIfThen.ParseObject[UtilIfThen.IFTHENTYPE] = (int)selectedIfThen.IfthenType;

            for (int i = 0; i < selectedIfThen.ZwaveDevicesIf.Count; i++)
            {
                selectedIfThen.ZwaveDevicesIf.ElementAt(i).Index = i;
                selectedIfThen.ZwaveDevicesIf.ElementAt(i).InstructionType = InstructionType.If;
                selectedIfThen.ZwaveDevicesIf.ElementAt(i).ZwaveDeviceToIfThenParseObject(selectedProjectModel);
                await selectedIfThen.ZwaveDevicesIf.ElementAt(i).ParseObject.SaveAsync();
                selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.IF).Add(selectedIfThen.ZwaveDevicesIf.ElementAt(i).ParseObject);
            }

            for (int i = 0; i < selectedIfThen.ZwaveDevicesThen.Count; i++)
            {
                selectedIfThen.ZwaveDevicesThen.ElementAt(i).Index = i;
                selectedIfThen.ZwaveDevicesThen.ElementAt(i).InstructionType = InstructionType.Then;
                selectedIfThen.ZwaveDevicesThen.ElementAt(i).ZwaveDeviceToIfThenParseObject(selectedProjectModel);
                await selectedIfThen.ZwaveDevicesThen.ElementAt(i).ParseObject.SaveAsync();
                selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.THEN).Add(selectedIfThen.ZwaveDevicesThen.ElementAt(i).ParseObject);
            }

            for (int i = 0; i < selectedIfThen.ZwaveDevicesElse.Count; i++)
            {
                selectedIfThen.ZwaveDevicesElse.ElementAt(i).Index = i;
                selectedIfThen.ZwaveDevicesElse.ElementAt(i).InstructionType = InstructionType.Else;
                selectedIfThen.ZwaveDevicesElse.ElementAt(i).ZwaveDeviceToIfThenParseObject(selectedProjectModel);
                await selectedIfThen.ZwaveDevicesElse.ElementAt(i).ParseObject.SaveAsync();
                selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.ELSE).Add(selectedIfThen.ZwaveDevicesElse.ElementAt(i).ParseObject);
            }

            await selectedIfThen.ParseObject.SaveAsync();

            ParseRelation<ParseObject> relations = selectedProjectModel.ParseObject.GetRelation<ParseObject>(UtilIfThen.IFTHENS);

            relations.Add(selectedIfThen.ParseObject);

            await selectedProjectModel.ParseObject.SaveAsync();

            _parseService.IsSendingToCloud = false;
        }

        private static int ParseCompareDayWeek(ZwaveDevice zwave, DaysOfWeekModel day, DateTime currentDate, DateTime timeZone)
        {
            if (currentDate.Day == timeZone.Day)
            {
                return zwave.DaysOfWeekList.IndexOf(day);
            }
            int difference = (int)Math.Ceiling((timeZone - currentDate).TotalDays);
            int dayOfWeek = zwave.DaysOfWeekList.IndexOf(day) + difference;
            int @return = dayOfWeek > 6 ? dayOfWeek - 7 : dayOfWeek;
            return @return;
        }

        private async Task AssignInstructionMacro(ProjectModel selectedProject, int macroId, int instructionId)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            JObject @object = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out @object))
                {
                    return;
                }

                if (!@object.ContainsKey(UtilIfThen.COMMAND))
                {
                    return;
                }

                if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.ASSIGNINSTRUCTION)
                {
                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\":\"assign_instruction\",\"macro_id\":{macroId},\"instruction_id\":{instructionId},\"type\":0}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        private async Task IfThenAttachConditional(ProjectModel selectedProject, int ruleId, int conditionalId, LogicGateIfThen operatorType)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            string @return = string.Empty;

            JObject @object = null;

            _ = replay.Subscribe(response =>
              {
                  if (string.IsNullOrEmpty(response))
                  {
                      return;
                  }

                  if (!response.TryParseJObject(out @object))
                  {
                      return;
                  }

                  if (!@object.ContainsKey(UtilIfThen.COMMAND))
                  {
                      return;
                  }

                  if (@object.Value<string>(UtilIfThen.COMMAND) == UtilIfThen.IFTHENATTACHCOND)
                  {
                      _taskService.CancellationTokenSource.Cancel();

                      return;
                  }
              });

            string command = $"{{\"command\":\"if_then_attach_cond\",\"type\":0,\"ruleId\":{ruleId},\"conditionalIndex\":{NextIndexConditional()},\"conditionalId\":{conditionalId},\"operatorType\":{(int)operatorType}}}";//$"@ITAC{ruleId}{NextIndexConditional}{conditionalId}{(int)operatorType:X2}#";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        private bool IsChangedDevice(ProjectModel selectedProject, ZwaveDevice device)
        {
            if (selectedProject is null)
            {
                return true;
            }

            if (device is null)
            {
                return true;
            }

            switch (device.IfthenType)
            {
                case IfthenType.Device:

                    ZwaveDevice temp = selectedProject.SelectedGateway.IsPrimary ?
                                       selectedProject.SelectedGateway.ZwaveDevices.FirstOrDefault(x => x.ModuleId == device.ModuleId) :
                                       selectedProject.SelectedGateway.SecondaryZwaveDevices.FirstOrDefault(x => x.ModuleId == device.ModuleId);

                    if (temp is null)
                    {
                        return true;
                    }

                    if (temp.ManufacturerKey != device.ManufacturerKey)
                    {
                        return true;
                    }

                    if (temp.FirmwareVersion != device.FirmwareVersion)
                    {
                        return true;
                    }

                    if (temp.ProductKey != device.ProductKey)
                    {
                        return true;
                    }

                    device.Name = temp.Name;

                    break;

                case IfthenType.IR:
                    break;

                case IfthenType.Radio433:

                    return selectedProject.Devices.FirstOrDefault(x => x.UID == device.GatewayFunctionUID) == null;

                case IfthenType.RTS:
                    break;

                case IfthenType.Schedule:
                    break;

                case IfthenType.IPCommand:
                    break;

                default:
                    break;
            }
            return false;
        }

        private int NextIndexConditional()
        {
            return indexConditional++;
        }
    }
}