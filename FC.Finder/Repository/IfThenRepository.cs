using ConfigurationFlexCloudHubBlaster._Util;
using ConfigurationFlexCloudHubBlaster.Model;
using ConfigurationFlexCloudHubBlaster.Model.Device;
using ConfigurationFlexCloudHubBlaster.Model.IfThen;
using ConfigurationFlexCloudHubBlaster.Model.IpCommand;
using ConfigurationFlexCloudHubBlaster.Model.Project;
using ConfigurationFlexCloudHubBlaster.Repository.Util;
using ConfigurationFlexCloudHubBlaster.Service;
using ConfigurationFlexCloudHubBlaster.Util;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace ConfigurationFlexCloudHubBlaster.Repository
{
    public interface IIfThenRepository
    {
        Task CreateIf(ProjectModel selectedProject, ZwaveDevice selectedZwaveDevice, string ruleId, string conditionalId);

        Task CreateIfThen(ProjectModel selectedProject, IfThenModel ifThenModel);

        Task CreateThen(ProjectModel selectedProject, ZwaveDevice zwaveDevice, string ruleId, int macroId, IfThenModel ifThenModel);

        Task<string> GetNextConditionalId(ProjectModel selectedProject);

        Task<int> GetNextInstructionId(ProjectModel selectedProject);

        Task<int> GetNextMacroId(ProjectModel selectedProject);

        Task<string> GetNextRuleId(ProjectModel selectedProject);

        Task PlayMacroAsync(ProjectModel selectedProject, int macroId);

        Task ResetDevice(ProjectModel selectedProject);

        Task<int> SetInstruction(ProjectModel selectedProject, string instruction, int? delay = 0);

        Task SetMacro(ProjectModel selectedProject, int macroId);

        Task<string> SetRule(ProjectModel selectedProject);

        Task StopMacroAsync(ProjectModel selectedProject, int macroId);

        Task Update(ProjectModel selectedProjectModel, IfThenModel selectedIfThenModel);

        Task GetIfThens(ProjectModel selectedProjectModel, ObservableCollection<IfThenModel> ifThenModels);

        Task GetIfThen(IfThenModel selectedIfThenModel);

        Task<bool> DeleteIfThenFromGataway(ProjectModel selectedProjectModel, IfThenModel ifThen);

        Task DeleteIfThenFromCloud(ProjectModel selectedProjectModel, IfThenModel ifThen);

        Task Rename(IfThenModel selectedIfThenModel);
    }

    public class IfThenRepository : RepositoryBase, IIfThenRepository
    {
        private int conditionalId = 0;
        private IIPCommandRepository _ipCommandRepository;
        private IParseService _parseService;

        public IfThenRepository(ITaskService taskService,
                                IGatewayService gatewayService,
                                IIPCommandRepository ipCommandRepository,
                                IParseService parseService,
                                ITCPRepository tcpRespository) : base(taskService, gatewayService, tcpRespository)
        {
            _ipCommandRepository = ipCommandRepository;

            _parseService = parseService;
        }

        public async Task<int> GetNextMacroId(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            JObject @object = null;

            int @return = -1;

            replay.Subscribe(response =>
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

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.GETNEXTMACROIDCOM).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            return @return;
        }

        public async Task SetMacro(ProjectModel selectedProject, int macroId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            JObject @object = null;

            replay.Subscribe(response =>
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

            var command = $"{{\"command\":\"set_macro\",\"macro_id\":{macroId},\"type\":0}}";

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task<string> GetNextRuleId(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            string @return = string.Empty;

            replay.Subscribe(response =>
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

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.GETNEXTRULEIDCOM).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            return @return;
        }

        public async Task<string> GetNextConditionalId(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            string @return = string.Empty;

            replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (response.Contains("&ITCU"))
                {
                    @return = response.Replace("&ITCU", "").Replace("#", "").StringToNextRuleOrConditionalFree();
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.GETNEXTCONDITIONALIDCOM).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            return @return;
        }

        public async Task<string> SetRule(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            string @return = string.Empty;

            replay.Subscribe(response =>
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

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.GETNEXTRULEIDCOM).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            return @return;
        }

        private string NextIndexConditional { get => $"{++conditionalId:X2}"; }

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
                await DeleteConditionals(selectedProject, ifThenModel).ConfigureAwait(true);
            }

            if (ifThenModel.InstructionIds.Any())
            {
                await DeleteInstructions(selectedProject, ifThenModel).ConfigureAwait(true);
            }

            if (ifThenModel.IpCommandIds.Any())
            {
                await DeleteIpCommand(selectedProject, ifThenModel).ConfigureAwait(true);
            }

            if (ifThenModel.MacroIdThen != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdThen).ConfigureAwait(true);
            }

            if (ifThenModel.MacroIdElse != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdElse).ConfigureAwait(true);
            }

            if (ifThenModel.RuleId != UtilIfThen.NEW)
            {
                await DeleteRule(selectedProject, ifThenModel.RuleId).ConfigureAwait(true);
            }

            conditionalId = 0;

            ifThenModel.RuleId = await GetNextRuleId(selectedProject).ConfigureAwait(true);

            ifThenModel.MacroIdThen = await GetNextMacroId(selectedProject).ConfigureAwait(true);

            ifThenModel.MacroIdElse = -1;

            await SetMacro(selectedProject, ifThenModel.MacroIdThen).ConfigureAwait(true);

            if (ifThenModel.ZwaveDevicesElse.Any())
            {
                ifThenModel.MacroIdElse = await GetNextMacroId(selectedProject).ConfigureAwait(true);

                await SetMacro(selectedProject, ifThenModel.MacroIdElse).ConfigureAwait(true);
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            string @return = string.Empty;

            replay.Subscribe(response =>
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

            string command = $"@ITRS{ifThenModel.RuleId}{ifThenModel.MacroIdThen:X2}{(ifThenModel.ZwaveDevicesElse.Any() ? "01" : "00")}{ifThenModel.MacroIdElse:X2}#";

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            foreach (var zwaveDeviceif in ifThenModel.ZwaveDevicesIf)
            {
                string conditionalId = await GetNextConditionalId(selectedProject).ConfigureAwait(true);

                ifThenModel.ConditionalIds.Add(conditionalId);

                await CreateIf(selectedProject, zwaveDeviceif, ifThenModel.RuleId, conditionalId).ConfigureAwait(true);
            }

            foreach (var zwaveDevice in ifThenModel.ZwaveDevicesThen)
            {
                await CreateThen(selectedProject, zwaveDevice, ifThenModel.RuleId, ifThenModel.MacroIdThen, ifThenModel).ConfigureAwait(true);
            }

            foreach (var zwaveDevice in ifThenModel.ZwaveDevicesElse)
            {
                await CreateThen(selectedProject, zwaveDevice, ifThenModel.RuleId, ifThenModel.MacroIdElse, ifThenModel).ConfigureAwait(true);
            }
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

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            replay.Subscribe(response =>
            {
            });

            string command = string.Empty;

            foreach (var id in ifThenModel.IpCommandIds)
            {
                command = $"{{\"command\":\"erase_memory_ip_command\",\"type\":0,\"id\":{id}}}";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);
            }

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            foreach (var id in ifThenModel.IpCommandIds)
            {
                selectedProject.SelectedGateway.IpCommands.Remove(selectedProject.SelectedGateway.IpCommands.FirstOrDefault(x => x.MemoryId == id));
            }

            replay.Dispose();

            ifThenModel.IpCommandIds.Clear();

            _gatewayService.IsSendingToGateway = false;
        }

        private async Task DeleteRule(ProjectModel selectedProject, string ruleId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            replay.Subscribe(response =>
            {
            });

            string command = $"@ITRD{ruleId}#"; ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        private async Task DeleteMacro(ProjectModel selectedProject, int macroId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            replay.Subscribe(response =>
            {
            });

            string command = $"{{\"command\":\"erase_macro\",\"macro_id\":{macroId}, \"type\":0}}";

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
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

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            replay.Subscribe(response =>
            {
            });

            string command = string.Empty;

            foreach (var id in ifThenModel.InstructionIds)
            {
                command = $"{{\"command\":\"erase_instruction\",\"instruction_id\":{id}, \"type\":0}}"; ;

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);
            }

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            ifThenModel.InstructionIds.Clear();

            _gatewayService.IsSendingToGateway = false;
        }

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

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            replay.Subscribe(response =>
            {
            });

            foreach (var conditionalId in ifThenModel.ConditionalIds)
            {
                string command = command = $"@ITCD{conditionalId}#";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);
            }

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            ifThenModel.ConditionalIds.Clear();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task CreateIf(ProjectModel selectedProject, ZwaveDevice selectedZwaveDevice, string ruleId, string conditionalId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (selectedZwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedZwaveDevice));
            }

            if (string.IsNullOrEmpty(ruleId))
            {
                throw new ArgumentException($"'{nameof(ruleId)}' não pode ser nulo ou vazio", nameof(ruleId));
            }

            if (conditionalId is null)
            {
                throw new ArgumentNullException(nameof(conditionalId));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            replay.Subscribe(response =>
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

            if (selectedZwaveDevice.IfthenType == IfthenType.Schedule)
            {
                using IfThenDateSetModel ifThenDateSet = new IfThenDateSetModel();

                switch (selectedZwaveDevice.SelectedDateType)
                {
                    case DateTypeEnum.CompareClock:
                        ifThenDateSet.ConditionalID = conditionalId;
                        ifThenDateSet.DataType = $"{(int)selectedZwaveDevice.SelectedDateType:X2}";
                        ifThenDateSet.ValueCC = $"{(int)selectedZwaveDevice.SelectedOperatorsTypeSchedule:X2}";
                        ifThenDateSet.ValueDD = $"{selectedZwaveDevice.TimePickerValue.Hour:X2}";
                        ifThenDateSet.ValueEE = $"{selectedZwaveDevice.TimePickerValue.Minute:X2}";
                        ifThenDateSet.ValueFF = $"{selectedZwaveDevice.TimePickerValue.Second:X2}";
                        break;

                    case DateTypeEnum.CompareDayWeek:
                        ifThenDateSet.ConditionalID = conditionalId;
                        ifThenDateSet.DataType = $"{(int)selectedZwaveDevice.SelectedDateType:X2}";
                        ifThenDateSet.ValueCC = $"{(int)selectedZwaveDevice.SelectedOperatorsTypeSchedule:X2}";
                        ifThenDateSet.ValueDD = $"{(int)selectedZwaveDevice.SelectedDaysOfWeek:X2}";
                        break;

                    default:
                        ifThenDateSet.ConditionalID = conditionalId;
                        ifThenDateSet.DataType = $"{(int)selectedZwaveDevice.SelectedDateType:X2}";
                        ifThenDateSet.ValueCC = $"{selectedZwaveDevice.ValueSchedule:X2}";
                        break;
                }

                string command = ifThenDateSet.ToString();

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

                await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

                replay.Dispose();

                _gatewayService.IsSendingToGateway = false;

                //case "06":
                //    ifThenDateSet = new IfThenDateSet
                //    {
                //        ConditionalID = conditionalID,
                //        DataType = selectedValueDateType.Value,
                //        Value_CC = selectedValueOperatorType.ValueIfThen,
                //        Value_DD = string.Format("{0:X2}", schedule.SelectedTimePicker.Hour),
                //        Value_EE = string.Format("{0:X2}", schedule.SelectedTimePicker.Minute),
                //        Value_FF = string.Format("{0:X2}", schedule.SelectedTimePicker.Second)
                //    };
                //    break;

                //case "0A":
                //    ifThenDateSet = new IfThenDateSet
                //    {
                //        ConditionalID = conditionalID,
                //        DataType = selectedValueDateType.Value,
                //        Value_CC = selectedValueOperatorType.ValueIfThen,
                //        Value_DD = selectedDayOfWeek.Value
                //    };
                //    break;

                //default:
                //    ifThenDateSet = new IfThenDateSet
                //    {
                //        ConditionalID = conditionalID,
                //        DataType = selectedValueDateType.Value,
                //        Value_CC = schedule.ValueToHexa
                //    };
                //    break;
            }
            else
            {
                using IfThenZwaveSetModel ifThenZwaveSet = GetIZTS(selectedZwaveDevice, conditionalId);

                string command = ifThenZwaveSet.ToString();

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

                await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

                replay.Dispose();

                _gatewayService.IsSendingToGateway = false;
            }

            await IfThenAttachConditional(selectedProject, ruleId, conditionalId, (LogicGateIfThen)selectedZwaveDevice.SelectedIndexLogicGateIfThen).ConfigureAwait(true);
        }

        private IfThenZwaveSetModel GetIZTS(ZwaveDevice selectedZwaveDevice, string conditionalId)
        {
            var classes = new HashSet<long?>(selectedZwaveDevice.CommandClasses.Select(c => c.CommandClassId));

            var FXR5011 = new HashSet<long?>(UtilIfThen.FXR5011COMMANDCLASS);
            var FXA0600 = new HashSet<long?>(UtilIfThen.FXA0600COMMANDCLASS);
            var FXS69A = new HashSet<long?>(UtilIfThen.FXS69ACOMMANDCLASS);

            IfThenZwaveSetModel ifThenZwaveSet = new IfThenZwaveSetModel();

            if (classes.SetEquals(FXS69A))
            {
                ifThenZwaveSet.ConditionalID = conditionalId;
                ifThenZwaveSet.DeviceID = $"{selectedZwaveDevice.ModuleId:X2}";
                ifThenZwaveSet.CommandClass = "25";

                ifThenZwaveSet.OperatorType_D1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_D1 = "00";
                ifThenZwaveSet.Value_D1 = "03";

                ifThenZwaveSet.OperatorType_E1 = $"{selectedZwaveDevice.SelectedIndexOperatorType + 1:X2}";
                ifThenZwaveSet.ByteIndex_E1 = "01";
                ifThenZwaveSet.Value_E1 = $"{(selectedZwaveDevice.IsOn ? "FF" : "00")}";

                ifThenZwaveSet.OperatorType_F1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_F1 = "02";
                ifThenZwaveSet.Value_F1 = "00";

                return ifThenZwaveSet;
            }
            else if (classes.SetEquals(FXA0600))
            {
                string @val = GetEndpointToValueIfThen(selectedZwaveDevice);

                string byte3 = GetByte3(selectedZwaveDevice);

                ifThenZwaveSet.ConditionalID = conditionalId;
                ifThenZwaveSet.DeviceID = $"{selectedZwaveDevice.ModuleId:X2}";
                ifThenZwaveSet.CommandClass = "60";

                ifThenZwaveSet.OperatorType_D1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_D1 = "00";
                ifThenZwaveSet.Value_D1 = "0D";

                ifThenZwaveSet.OperatorType_E1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_E1 = "01";
                ifThenZwaveSet.Value_E1 = $"{selectedZwaveDevice.SelectedIndexEndpoint + 1:X2}";

                ifThenZwaveSet.OperatorType_F1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_F1 = "02";
                ifThenZwaveSet.Value_F1 = "00";

                ifThenZwaveSet.OperatorType_G1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_G1 = "03";
                ifThenZwaveSet.Value_G1 = byte3;

                ifThenZwaveSet.OperatorType_H1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_H1 = "04";
                ifThenZwaveSet.Value_H1 = "03";

                ifThenZwaveSet.OperatorType_I1 = $"{selectedZwaveDevice.SelectedIndexOperatorType + 1:X2}";
                ifThenZwaveSet.ByteIndex_I1 = "05";
                ifThenZwaveSet.Value_I1 = @val;

                ifThenZwaveSet.OperatorType_J1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_J1 = "06";
                ifThenZwaveSet.Value_J1 = "00";

                return ifThenZwaveSet;
            }
            else if (classes.SetEquals(FXR5011))
            {
                string @val = GetEndpointToValueIfThen(selectedZwaveDevice);

                string byte3 = GetByte3(selectedZwaveDevice);

                ifThenZwaveSet.ConditionalID = conditionalId;
                ifThenZwaveSet.DeviceID = $"{selectedZwaveDevice.ModuleId:X2}";
                ifThenZwaveSet.CommandClass = "60";

                ifThenZwaveSet.OperatorType_D1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_D1 = "00";
                ifThenZwaveSet.Value_D1 = "0D";

                ifThenZwaveSet.OperatorType_E1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_E1 = "01";
                ifThenZwaveSet.Value_E1 = $"{selectedZwaveDevice.SelectedIndexEndpoint + 1:X2}";

                ifThenZwaveSet.OperatorType_F1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_F1 = "02";
                ifThenZwaveSet.Value_F1 = "00";

                ifThenZwaveSet.OperatorType_G1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_G1 = "03";
                ifThenZwaveSet.Value_G1 = byte3;

                ifThenZwaveSet.OperatorType_H1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_H1 = "04";
                ifThenZwaveSet.Value_H1 = "03";

                ifThenZwaveSet.OperatorType_I1 = $"{selectedZwaveDevice.SelectedIndexOperatorType + 1:X2}";
                ifThenZwaveSet.ByteIndex_I1 = "05";
                ifThenZwaveSet.Value_I1 = @val;

                ifThenZwaveSet.OperatorType_J1 = UtilIfThen.IGNORE;
                ifThenZwaveSet.ByteIndex_J1 = "06";
                ifThenZwaveSet.Value_J1 = "00";

                return ifThenZwaveSet;
            }

            throw new Exception(Properties.Resources.Device_is_not_yet_supported_in_If);
        }

        private string GetByte3(ZwaveDevice selectedZwaveDevice)
        {
            if (selectedZwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedZwaveDevice));
            }
            if (selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass == 16)
            {
                return "20";
            }
            else if (selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass == 17)
            {
                return "26";
            }
            throw new Exception("Classe do endpoint não implementada.");
        }

        private string GetEndpointToValueIfThen(ZwaveDevice selectedZwaveDevice)
        {
            if (selectedZwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedZwaveDevice));
            }
            if (selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass == 16)
            {
                return selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? "FF" : "00";
            }
            else if (selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass == 17)
            {
                return $"{selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel:X2}";
            }
            throw new Exception("Classe do endpoint não implementada.");
        }

        public async Task CreateThen(ProjectModel selectedProject, ZwaveDevice zwaveDevice, string ruleId, int macroId, IfThenModel ifThenModel)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            if (string.IsNullOrEmpty(ruleId))
            {
                throw new ArgumentException($"'{nameof(ruleId)}' não pode ser nulo ou vazio", nameof(ruleId));
            }

            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            string instruction = await GetInstructionByZwaveDevice(selectedProject, zwaveDevice).ConfigureAwait(true);

            int instructionId = await SetInstruction(selectedProject, instruction, zwaveDevice.DelayIfThen).ConfigureAwait(true);

            ifThenModel.InstructionIds.Add(instructionId);

            await AssignInstructionMacro(selectedProject, macroId, instructionId).ConfigureAwait(true);
        }

        private async Task AssignInstructionMacro(ProjectModel selectedProject, int macroId, int instructionId)
        {
            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            JObject @object = null;

            replay.Subscribe(response =>
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

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task<int> SetInstruction(ProjectModel selectedProject, string instruction, int? delay = 0)
        {
            double milliseconds = TimeSpan.FromSeconds((double)delay).TotalMilliseconds;

            int instructionId = -1;

            instructionId = await GetNextInstructionId(selectedProject).ConfigureAwait(true);

            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            JObject @object = null;

            replay.Subscribe(response =>
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

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            return instructionId;
        }

        public async Task<int> GetNextInstructionId(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            JObject @object = null;

            int @return = -1;

            replay.Subscribe(response =>
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

            await ToCommand(selectedProject.SelectedGateway, commands, replay, UtilIfThen.GETNEXTINSTRUCTIONIDCOM).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            return @return;
        }

        private async Task<string> GetInstructionByZwaveDevice(ProjectModel selectedProject, ZwaveDevice selectedZwaveDevice)
        {
            if (selectedZwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedZwaveDevice));
            }

            if (selectedZwaveDevice.IfthenType == IfthenType.RTS)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    using IpCommandModel ipCommand = new IpCommandModel
                    {
                        Name = string.Format(CultureInfo.InvariantCulture, Properties.Resources.Used_by_ifthen, selectedProject.SelectedIfThen.Name),
                        Command = $"{{\"command\":\"playRts\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId}, \"button\":{(int)selectedZwaveDevice.ActionsRTSSomfy}, \"type\":0}}",
                        TargetIp = gateway.LocalIP,
                        TargetPort = gateway.LocalPortUDP
                    };

                    var memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand).ConfigureAwait(true);

                    selectedProject.SelectedIfThen.IpCommandIds.Add(memoryId);

                    return ($"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}").Replace("\"", "\\\"");
                }

                throw new Exception(Properties.Resources.Gateways_Found_Network);
            }

            if (selectedZwaveDevice.IfthenType == IfthenType.Radio433)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    using IpCommandModel ipCommand = new IpCommandModel
                    {
                        Name = string.Format(CultureInfo.InvariantCulture, Properties.Resources.Used_by_ifthen, selectedProject.SelectedIfThen.Name),
                        Command = ($"{{\"command\":\"playRadioMemory\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId}, \"type\":0}}"),
                        TargetIp = gateway.LocalIP,
                        TargetPort = gateway.LocalPortUDP
                    };

                    var memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand).ConfigureAwait(true);

                    selectedProject.SelectedIfThen.IpCommandIds.Add(memoryId);

                    return ($"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}").Replace("\"", "\\\"");
                }

                throw new Exception(Properties.Resources.Gateways_Found_Network);
            }

            if (selectedZwaveDevice.IfthenType == IfthenType.IR)
            {
                if (selectedProject.Devices.FirstOrDefault(x => x.UID == selectedZwaveDevice.GatewayFunctionUID) is GatewayModel gateway)
                {
                    if (gateway.UID == selectedProject.SelectedGateway.UID)
                    {
                        return ($"{{\"command\":\"irPlayMemoryCode\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId},\"type\":0}}").Replace("\"", "\\\"");
                    }
                    else
                    {
                        using IpCommandModel ipCommand = new IpCommandModel
                        {
                            Name = string.Format(CultureInfo.InvariantCulture, Properties.Resources.Used_by_ifthen, selectedProject.SelectedIfThen.Name),
                            Command = ($"{{\"command\":\"irPlayMemoryCode\",\"id\":{selectedZwaveDevice.GatewayFunctionMemoryId},\"type\":0}}"),
                            TargetIp = gateway.LocalIP,
                            TargetPort = gateway.LocalPortUDP
                        };

                        var memoryId = await _ipCommandRepository.SaveAsync(selectedProject.SelectedGateway, ipCommand).ConfigureAwait(true);

                        selectedProject.SelectedIfThen.IpCommandIds.Add(memoryId);

                        return ($"{{\"command\":\"play_memory_ip_command\", \"type\":0, \"id\":{memoryId}}}").Replace("\"", "\\\"");
                    }
                }

                throw new Exception(Properties.Resources.Gateway_Did_Not_Respond);
            }

            var classes = new HashSet<long?>(selectedZwaveDevice.CommandClasses.Select(c => c.CommandClassId));

            var FXR5011 = new HashSet<long?>(UtilIfThen.FXR5011COMMANDCLASS);
            var FXA0600 = new HashSet<long?>(UtilIfThen.FXA0600COMMANDCLASS);
            var FXS69A = new HashSet<long?>(UtilIfThen.FXS69ACOMMANDCLASS);

            if (classes.SetEquals(FXA0600))
            {
                var genericDeviceClass = selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass;

                if (genericDeviceClass == 17)
                {
                    return ($"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\":{selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel}}}").Replace("\"", "\\\"");
                }
                else if (genericDeviceClass == 16)
                {
                    return ($"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{(selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? 255 : 0)}}}").Replace("\"", "\\\"");
                }
            }
            else if (classes.SetEquals(FXR5011))
            {
                var genericDeviceClass = selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).GenericDeviceClass;

                if (genericDeviceClass == 17)
                {
                    return ($"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\":{selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).MultiLevel}}}").Replace("\"", "\\\"");
                }
                else if (genericDeviceClass == 16)
                {
                    return ($"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"channel\": {selectedZwaveDevice.SelectedIndexEndpoint + 1},\"level\":{(selectedZwaveDevice.Endpoints.ElementAt(selectedZwaveDevice.SelectedIndexEndpoint).IsOn ? 255 : 0)}}}").Replace("\"", "\\\"");
                }
            }
            else if (classes.SetEquals(FXS69A))
            {
                return ($"{{\"command\":\"binarySwitchSet\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId},\"level\":{(selectedZwaveDevice.IsOn ? 255 : 0)}}}").Replace("\"", "\\\"");
            }

            throw new Exception(Properties.Resources.Device_is_not_yet_supported_in_Then_Else);
        }

        private async Task IfThenAttachConditional(ProjectModel selectedProject, string ruleId, string conditionalId, LogicGateIfThen operatorType)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (string.IsNullOrEmpty(ruleId))
            {
                throw new ArgumentException($"'{nameof(ruleId)}' não pode ser nulo ou vazio", nameof(ruleId));
            }

            if (string.IsNullOrEmpty(conditionalId))
            {
                throw new ArgumentException($"'{nameof(conditionalId)}' não pode ser nulo ou vazio", nameof(conditionalId));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            string @return = string.Empty;

            replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (response.Contains("&ITACOK#"))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"@ITAC{ruleId}{NextIndexConditional}{conditionalId}{(int)operatorType:X2}#";

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task PlayMacroAsync(ProjectModel selectedProject, int macroId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            JObject @object = null;

            replay.Subscribe(response =>
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

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task StopMacroAsync(ProjectModel selectedProject, int macroId)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            IList<object> commands = new List<object>();

            JObject @object = null;

            replay.Subscribe(response =>
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

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task ResetDevice(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            IList<object> commands = new List<object>();

            using ReplaySubject<string> replay = new ReplaySubject<string>();

            JObject @object = null;

            replay.Subscribe(response =>
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

            for (int i = 0; i <= DELETE_NUMBER; i++)
            {
                string command = $"{{\"command\":\"erase_instruction\",\"instruction_id\":{i}, \"type\":0}}";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

                command = $"{{\"command\":\"erase_macro\",\"macro_id\":{i}, \"type\":0}}";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

                command = $"@ITCD{i:X2}#";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);

                command = $"@ITRD{i:X2}#";

                await ToCommand(selectedProject.SelectedGateway, commands, replay, command).ConfigureAwait(true);
            }

            await TryCommands(selectedProject.SelectedGateway, commands, replay).ConfigureAwait(true);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
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
                foreach (var parseObject in selectedIfThen.ParseObjectsIf)
                {
                    selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.IF).Remove(parseObject);
                }
            }

            if (selectedIfThen.ParseObjectsThen.Any())
            {
                foreach (var parseObject in selectedIfThen.ParseObjectsThen)
                {
                    selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.THEN).Remove(parseObject);
                }
            }

            if (selectedIfThen.ParseObjectsElse.Any())
            {
                foreach (var parseObject in selectedIfThen.ParseObjectsElse)
                {
                    selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.ELSE).Remove(parseObject);
                }
            }

            if (selectedIfThen.DeletedZwaveDevices.Any())
            {
                await ParseObject.DeleteAllAsync(selectedIfThen.DeletedZwaveDevices).ConfigureAwait(true);

                selectedIfThen.DeletedZwaveDevices.Clear();
            }

            selectedIfThen.ParseObject ??= new ParseObject(UtilIfThen.CLASSNAME);
            selectedIfThen.ParseObject[UtilIfThen.GATEWAYID] = selectedProjectModel.SelectedGateway.UID;
            selectedIfThen.ParseObject[UtilIfThen.CONDITIONALIDS] = selectedIfThen.ConditionalIds;
            selectedIfThen.ParseObject[UtilIfThen.INSTRUCTIONIDS] = selectedIfThen.InstructionIds;
            selectedIfThen.ParseObject[UtilIfThen.IPCOMMANDIDS] = selectedIfThen.IpCommandIds;
            selectedIfThen.ParseObject[UtilIfThen.RULEID] = selectedIfThen.RuleId;
            selectedIfThen.ParseObject[UtilIfThen.MACROIDTHEN] = selectedIfThen.MacroIdThen;
            selectedIfThen.ParseObject[UtilIfThen.MACROIDELSE] = selectedIfThen.MacroIdElse;
            selectedIfThen.ParseObject[UtilIfThen.NAME] = selectedIfThen.Name;

            for (int i = 0; i < selectedIfThen.ZwaveDevicesIf.Count; i++)
            {
                selectedIfThen.ZwaveDevicesIf.ElementAt(i).Index = i;
                selectedIfThen.ZwaveDevicesIf.ElementAt(i).InstructionType = InstructionType.If;
                selectedIfThen.ZwaveDevicesIf.ElementAt(i).ZwaveDeviceToIfThenParseObject(selectedProjectModel);
                await selectedIfThen.ZwaveDevicesIf.ElementAt(i).ParseObject.SaveAsync().ConfigureAwait(true);
                selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.IF).Add(selectedIfThen.ZwaveDevicesIf.ElementAt(i).ParseObject);
            }

            for (int i = 0; i < selectedIfThen.ZwaveDevicesThen.Count; i++)
            {
                selectedIfThen.ZwaveDevicesThen.ElementAt(i).Index = i;
                selectedIfThen.ZwaveDevicesThen.ElementAt(i).InstructionType = InstructionType.Then;
                selectedIfThen.ZwaveDevicesThen.ElementAt(i).ZwaveDeviceToIfThenParseObject(selectedProjectModel);
                await selectedIfThen.ZwaveDevicesThen.ElementAt(i).ParseObject.SaveAsync().ConfigureAwait(true);
                selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.THEN).Add(selectedIfThen.ZwaveDevicesThen.ElementAt(i).ParseObject);
            }

            for (int i = 0; i < selectedIfThen.ZwaveDevicesElse.Count; i++)
            {
                selectedIfThen.ZwaveDevicesElse.ElementAt(i).Index = i;
                selectedIfThen.ZwaveDevicesElse.ElementAt(i).InstructionType = InstructionType.Else;
                selectedIfThen.ZwaveDevicesElse.ElementAt(i).ZwaveDeviceToIfThenParseObject(selectedProjectModel);
                await selectedIfThen.ZwaveDevicesElse.ElementAt(i).ParseObject.SaveAsync().ConfigureAwait(true);
                selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.ELSE).Add(selectedIfThen.ZwaveDevicesElse.ElementAt(i).ParseObject);
            }

            await selectedIfThen.ParseObject.SaveAsync().ConfigureAwait(true);

            var relations = selectedProjectModel.ParseObject.GetRelation<ParseObject>(UtilIfThen.IFTHENS);

            relations.Add(selectedIfThen.ParseObject);

            await selectedProjectModel.ParseObject.SaveAsync().ConfigureAwait(true);

            _parseService.IsSendingToCloud = false;
        }

        public async Task GetIfThens(ProjectModel selectedProjectModel, ObservableCollection<IfThenModel> ifThenModels)
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

            var query = await selectedProjectModel.ParseObject.GetRelation<ParseObject>(UtilIfThen.IFTHENS).Query.WhereContains(UtilIfThen.GATEWAYID, selectedProjectModel.SelectedGateway.UID).FindAsync().ConfigureAwait(true);

            foreach (var parseObject in query)
            {
                ifThenModels.Add(parseObject.ParseObjectToIfThenModel());
            }

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task GetIfThen(IfThenModel selectedIfThen)
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

            await selectedIfThen.ParseObject.FetchAsync().ConfigureAwait(true);

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.CONDITIONALIDS))
            {
                var conditionalIds = selectedIfThen.ParseObject.Get<IList<object>>(UtilIfThen.CONDITIONALIDS);

                foreach (var conditionalId in conditionalIds)
                {
                    selectedIfThen.ConditionalIds.Add(conditionalId.ToString());
                }
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.INSTRUCTIONIDS))
            {
                var instructionIds = selectedIfThen.ParseObject.Get<IList<object>>(UtilIfThen.INSTRUCTIONIDS);

                foreach (var instructionId in instructionIds)
                {
                    if (int.TryParse(instructionId.ToString(), out int id))
                    {
                        selectedIfThen.InstructionIds.Add(id);
                    }
                }
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.IPCOMMANDIDS))
            {
                var ipCommandIds = selectedIfThen.ParseObject.Get<IList<object>>(UtilIfThen.IPCOMMANDIDS);

                foreach (var ipCommandId in ipCommandIds)
                {
                    if (int.TryParse(ipCommandId.ToString(), out int id))
                    {
                        selectedIfThen.IpCommandIds.Add(id);
                    }
                }
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.RULEID))
            {
                selectedIfThen.RuleId = selectedIfThen.ParseObject.Get<string>(UtilIfThen.RULEID);
            }

            if (selectedIfThen.ParseObject.ContainsKey(UtilIfThen.RULEID))
            {
                selectedIfThen.RuleId = selectedIfThen.ParseObject.Get<string>(UtilIfThen.RULEID);
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

            var @if = await selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.IF).Query.OrderBy(UtilIfThen.INDEX).FindAsync().ConfigureAwait(true);

            selectedIfThen.ParseObjectsIf = @if;

            foreach (var parseObject in @if)
            {
                selectedIfThen.ZwaveDevicesIf.Add(parseObject.ParseObjectToIfThenObject());
            }

            var ifOrderByDescending = selectedIfThen.ZwaveDevicesIf.OrderByDescending(x => x.Index).ToList();

            if (selectedIfThen.ZwaveDevicesIf.Any())
            {
                selectedIfThen.ZwaveDevicesIf.First().IsHiddenLogicGateIfThen = true;
            }

            var @then = await selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.THEN).Query.OrderBy(UtilIfThen.INDEX).FindAsync().ConfigureAwait(true);

            selectedIfThen.ParseObjectsThen = @then;

            foreach (var parseObject in @then)
            {
                selectedIfThen.ZwaveDevicesThen.Add(parseObject.ParseObjectToIfThenObject());
            }

            if (selectedIfThen.ZwaveDevicesThen.Any())
            {
                selectedIfThen.ZwaveDevicesThen.First().IsHiddenDelay = true;
            }

            var @else = await selectedIfThen.ParseObject.GetRelation<ParseObject>(UtilIfThen.ELSE).Query.OrderBy(UtilIfThen.INDEX).FindAsync().ConfigureAwait(true);

            selectedIfThen.ParseObjectsElse = @else;

            foreach (var parseObject in @else)
            {
                selectedIfThen.ZwaveDevicesElse.Add(parseObject.ParseObjectToIfThenObject());
            }

            if (selectedIfThen.ZwaveDevicesElse.Any())
            {
                selectedIfThen.ZwaveDevicesElse.First().IsHiddenDelay = true;
            }

            _parseService.IsSendingToCloud = false;
        }

        public async Task<bool> DeleteIfThenFromGataway(ProjectModel selectedProject, IfThenModel ifThenModel)
        {
            if (ifThenModel is null)
            {
                throw new ArgumentNullException(nameof(ifThenModel));
            }

            if (ifThenModel.ConditionalIds.Any())
            {
                await DeleteConditionals(selectedProject, ifThenModel).ConfigureAwait(true);
            }

            if (ifThenModel.InstructionIds.Any())
            {
                await DeleteInstructions(selectedProject, ifThenModel).ConfigureAwait(true);
            }

            if (ifThenModel.IpCommandIds.Any())
            {
                await DeleteIpCommand(selectedProject, ifThenModel).ConfigureAwait(true);
            }

            if (ifThenModel.MacroIdThen != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdThen).ConfigureAwait(true);
            }

            if (ifThenModel.MacroIdElse != -1)
            {
                await DeleteMacro(selectedProject, ifThenModel.MacroIdElse).ConfigureAwait(true);
            }

            if (ifThenModel.RuleId != UtilIfThen.NEW)
            {
                await DeleteRule(selectedProject, ifThenModel.RuleId).ConfigureAwait(true);
            }

            return true;
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

            var @if = ifThen.ZwaveDevicesIf.Select(x => x.ParseObject);
            var @the = ifThen.ZwaveDevicesThen.Select(x => x.ParseObject);
            var @else = ifThen.ZwaveDevicesElse.Select(x => x.ParseObject);

            var result = @if.Concat(@the).Concat(@else).Concat(ifThen.DeletedZwaveDevices);

            await ParseObject.DeleteAllAsync(result).ConfigureAwait(true);

            await ifThen.ParseObject.DeleteAsync().ConfigureAwait(true);
        }

        public async Task Rename(IfThenModel selectedIfThenModel)
        {
            if (selectedIfThenModel is null)
            {
                throw new ArgumentNullException(nameof(selectedIfThenModel));
            }
            _parseService.IsSendingToCloud = true;

            selectedIfThenModel.ParseObject[UtilIfThen.NAME] = selectedIfThenModel.Name;

            await selectedIfThenModel.ParseObject.SaveAsync().ConfigureAwait(true);

            _parseService.IsSendingToCloud = false;
        }
    }
}