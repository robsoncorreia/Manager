using FC.Domain.Model;
using FC.Domain.Model.FlexCloudClone;
using FC.Domain.Repository.Util;
using FC.Domain.Service;
using FC.Domain.Util;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface IRFRepository
    {
        Task AddSomfy(GatewayModel selectedGateway, RadioModel selectedRadioModel, ReplaySubject<string> replay);

        Task DeleteAllCloud(GatewayModel selectedGateway, TypeRF r433);

        Task<bool> DeleteAsync(GatewayModel selectedGateway, RadioModel radioModel);

        Task DeleteFromCloud(GatewayModel selectedGateway, RadioModel radioModel);

        Task GetAll(GatewayModel selectedGateway, TypeRF typeClone);

        Task GetAllFromCloud(GatewayModel selectedGateway, TypeRF typeRF);

        Task GetFirmwareInfoPICAsync(GatewayModel selectedGateway, ProtocolTypeEnum protocol = ProtocolTypeEnum.TCP);

        Task Learn(GatewayModel selectedGateway, RadioModel selectedRadioModel);

        Task PlayMemory(GatewayModel selectedGateway, int memoryId);

        Task PlayRadioMemory(GatewayModel selectedGateway, RadioModel radioModel);

        Task PlayRts(GatewayModel selectedGateway, RadioModel selectedRadioModel);

        Task ResetPICAsync(GatewayModel selectedGateway);

        Task Update(GatewayModel selectedGateway, RadioModel radioModel);

        Task UpdatePICAsync(GatewayModel selectedGateway, ProtocolTypeEnum protocol = ProtocolTypeEnum.TCP);
    }

    public class RFRepository : RepositoryBase, IRFRepository
    {
        public async Task GetAll(GatewayModel selectedGateway, TypeRF typeClone)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
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

                if (!response.TryParseJObject(out json))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.ERROR))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.TOTAL))
                {
                    return;
                }

                if (json.ContainsKey(UtilRF.RADIO))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = UtilRF.GETALLMEMORYTYPECOMMAND;

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            commands: commands,
                            command: command,
                            isSendingToGateway: true,
                            timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (json.ContainsKey(UtilRF.ERROR))
            {
                if (json.Value<int>(UtilRF.ERROR) == (int)ErrorCodeRF.TimeOut)
                {
                    throw new Exception(Properties.Resources.Time_out);
                }

                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__Code, json.Value<int>(UtilRF.ERROR)));
            }

            JToken[] jArrayRTS = json.Value<JArray>(UtilRF.RTS).ToArray();

            int[] rfRTSIds = jArrayRTS.Select(jv => (int)jv).ToArray();

            JToken[] jArrayRadio = json.Value<JArray>(UtilRF.RADIO).ToArray();

            int[] rf433Ids = jArrayRadio.Select(jv => (int)jv).ToArray();

            _gatewayService.RadioCount = jArrayRadio.Length + jArrayRTS.Length;

            selectedGateway.MaximumNumberRFMemoryPositions = json.Value<int>(UtilRF.TOTAL);

            if (json.ContainsKey(UtilRF.RADIO) && (typeClone == TypeRF.R433 || typeClone == TypeRF.Any))
            {
                if (selectedGateway.Radios433Gateway.Any())
                {
                    selectedGateway.Radios433Gateway.Clear();
                }

                foreach (int id in rf433Ids)
                {
                    if (await GetRadioDescription(selectedGateway, id) is RadioModel radio)
                    {
                        selectedGateway.Radios433Gateway.Add(radio);
                    }
                }
            }
            if (json.ContainsKey(UtilRF.RADIO) && (typeClone == TypeRF.RTS || typeClone == TypeRF.Any))
            {
                if (selectedGateway.RadiosRTSGateway.Any())
                {
                    selectedGateway.RadiosRTSGateway.Clear();
                }

                foreach (int id in rfRTSIds)
                {
                    if (await GetRts(selectedGateway, id) is RadioModel radio)
                    {
                        selectedGateway.RadiosRTSGateway.Add(radio);
                    }
                }
            }

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;
        }

        private async Task<RadioModel> GetRts(GatewayModel selectedGateway, int id)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            JObject json = null;

            RadioModel radio = null;

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

                if (json.ContainsKey(UtilRF.REMOTEID))
                {
                    int remoteId = json.Value<int>(UtilRF.REMOTEID);
                    int rollingCode = json.Value<int>(UtilRF.ROLLINGCODE);
                    int repetition = json.Value<int>(UtilRF.REPETITION);
                    int power = json.Value<int>(UtilRF.POWER);
                    radio = new RadioModel
                    {
                        MemoryId = id,
                        Ip = selectedGateway.LocalIP,
                        Port = selectedGateway.LocalPortUDP,
                        MacAddress = selectedGateway.LocalMacAddress,
                        IsSavedGateway = true,
                        RemoteId = remoteId,
                        RollingCode = rollingCode,
                        Repetition = repetition,
                        Power = power,
                        TypeRF = TypeRF.RTS
                    };

                    if (_localDBRepository.FindOne(selectedGateway, radio) is RadioModel temp)
                    {
                        radio.Description = temp.Description;
                        radio.IsSensorRTS = temp.IsSensorRTS;
                    }
                }
            });

            string command = $"{{\"command\":\"getRts\", \"id\":{id}, \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            commands: commands,
                            command: command,
                            isSendingToGateway: true,
                            timeout: 5000);

            replay.Dispose();

            return radio;
        }

        private async Task<RadioModel> GetRadioDescription(GatewayModel selectedGateway, int id)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            JObject json = null;

            RadioModel radio = null;

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

                if (json.ContainsKey(UtilRF.DESCRIPTION))
                {
                    radio = new RadioModel
                    {
                        MemoryId = id,
                        TypeRF = TypeRF.R433,
                        Description = json.Value<string>(UtilRF.DESCRIPTION)
                    };

                    return;
                }
            });

            string command = $"{{\"command\":\"getRadioDescription\", \"id\":{id}, \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            commands: commands,
                            command: command,
                            isSendingToGateway: true,
                            timeout: 5000);

            replay.Dispose();

            return radio;
        }

        public async Task PlayMemory(GatewayModel selectedGateway, int memoryId)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            JObject json = null;

            using ReplaySubject<string> replay = new();

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

                _taskService.CancellationTokenSource.Cancel();
                return;
            });

            string command = $"{{\"command\":\"playRadioMemory\",\"id\":{memoryId}, \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway, replay: replay, commands: commands, command: command, timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();
        }

        private async Task<int> GetNextEmptyId(GatewayModel selectedGateway, RadioModel selectedRadioModel)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (selectedRadioModel is null)
            {
                throw new ArgumentNullException(nameof(selectedRadioModel));
            }

            JObject json = null;

            using ReplaySubject<string> replay = new();

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

                _taskService.CancellationTokenSource.Cancel();
                return;
            });

            ;

            string command = UtilRF.GETCHECKALLRADIOMEMORYCOMMAND;

            ;

            await ToCommand(selectedGateway: selectedGateway, replay: replay, commands: commands, command: command, timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (json.ContainsKey(UtilRF.COMMAND) && json.ContainsKey(UtilRF.BITFIELD))
            {
                string bitfield = json.Value<string>(UtilRF.BITFIELD);

                int total = json.Value<int>(UtilRF.TOTAL);

                byte[] newBytes = Convert.FromBase64String(bitfield);

                string[] hexas = BitConverter.ToString(newBytes).Split('-');

                int id = 0;

                for (int i = 0; i < hexas.Length; i++)
                {
                    string binarystring = string.Join(string.Empty,
                                                                   hexas[i].Select(
                                                                       c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                                                                     )
                                                                   );
                    char[] binarys = binarystring.ToCharArray();

                    for (int j = binarys.Length - 1; j >= 0; j--)
                    {
                        if (binarys[j].Equals('0'))
                        {
                            Random rnd = new();
                            selectedRadioModel.MemoryId = id;
                            selectedRadioModel.RollingCode = id + 1;
                            selectedRadioModel.RemoteId = rnd.Next(RadioModel.MIN_REMOTE_ID, RadioModel.MAX_REMOTE_ID);
                            return id;
                        }

                        id++;

                        if (id > (total - 1))
                        {
                            throw new Exception(Properties.Resources.All_Memories_Are_Full);
                        }
                    }
                }
                return -1;
            }
            return -1;
        }

        public async Task<bool> DeleteAsync(GatewayModel selectedGateway, RadioModel radioModel)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (radioModel is null)
            {
                throw new ArgumentNullException(nameof(radioModel));
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

                if (!response.TryParseJObject(out json))
                {
                    return;
                }

                if (json.ContainsKey(UtilRF.COMMAND) && json.Value<string>(UtilRF.COMMAND) == UtilRF.ERASERADIOMEMORY)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"eraseRadioMemory\", \"id\":{radioModel.MemoryId}, \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway, replay: replay, commands: commands, command: command, timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (json.ContainsKey(UtilRF.COMMAND) &&
                json.ContainsKey(UtilRF.ID) &&
                json.ContainsKey(UtilRF.TYPE) &&
                json.Value<int>(UtilRF.TYPE) == 1
                )
            {
                if (radioModel.TypeRF == TypeRF.R433)
                {
                    _ = selectedGateway.Radios433Gateway.Remove(selectedGateway.Radios433Gateway.FirstOrDefault(x => x.MemoryId == radioModel.MemoryId));
                    _gatewayService.RadioCount = selectedGateway.RadiosRTSGateway.Count + selectedGateway.Radios433Gateway.Count;
                }

                if (radioModel.TypeRF == TypeRF.RTS)
                {
                    _ = selectedGateway.RadiosRTSGateway.Remove(selectedGateway.RadiosRTSGateway.FirstOrDefault(x => x.MemoryId == radioModel.MemoryId));
                    _gatewayService.RadioCount = selectedGateway.RadiosRTSGateway.Count + selectedGateway.Radios433Gateway.Count;
                }
                return true;
            }

            return false;
        }

        public async Task GetFirmwareInfoPICAsync(GatewayModel selectedGateway, ProtocolTypeEnum protocol = ProtocolTypeEnum.TCP)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
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
                if (!response.TryParseJObject(out json))
                {
                    return;
                }
                if (!json.ContainsKey(UtilRF.COMMAND))
                {
                    return;
                }

                if (json.ContainsKey(UtilRF.COMMAND) && json.Value<string>(UtilRF.COMMAND) == UtilRF.GETRADIOFIRMWAREINFO)
                {
                    selectedGateway.RadioVersionFirmware = json.Value<string>(UtilRF.VERSION);

                    selectedGateway.RadioBuildFirmware = json.Value<string>(UtilRF.BUILD);

                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = UtilRF.GETRADIOFIRMWAREINFOCOMMAND;

            ;

            await ToCommand(selectedGateway: selectedGateway, replay: replay, commands: commands, command: command, timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task Learn(GatewayModel selectedGateway, RadioModel selectedRadio)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (selectedRadio is null)
            {
                throw new ArgumentNullException(nameof(selectedRadio));
            }

            using ReplaySubject<string> replay = new();

            if (selectedRadio.MemoryId == -1)
            {
                _ = await GetNextEmptyId(selectedGateway, selectedRadio);
            }
            else
            {
                bool removed = await DeleteAsync(selectedGateway, selectedRadio);
            }

            JObject json = null;

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

                if (json.ContainsKey(UtilRF.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!json.ContainsKey(UtilRF.COMMAND))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.ID))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.RADIOTYPE))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.FREQUENCY))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.POWER))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.REPETITION))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.DESCRIPTION))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.TYPE))
                {
                    return;
                }

                if (json.Value<int>(UtilRF.TYPE) == 1)
                {
                    _taskService.CancellationTokenSource.Cancel();
                }
            });

            string command = $"{{\"command\":\"recordToRadioMemory\", \"id\":{selectedRadio.MemoryId}, \"radioType\":1, \"frequency\":0, \"power\":7, \"repetition\":{selectedRadio.Repetition},\"description\":\"{selectedRadio.Description}\", \"type\":0}}";

            _gatewayService.LastCommandSend = command;

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            commands: commands,
                            command: command,
                            timeout: 20000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                return;
            }

            if (json.ContainsKey(UtilRF.ERROR))
            {
                if (json.Value<int>(UtilRF.ERROR) == (int)ErrorCodeRF.TimeOut)
                {
                    throw new Exception(Properties.Resources.Time_out);
                }

                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__Code, json.Value<int>(UtilRF.ERROR)));
            }

            if (json.ContainsKey(UtilRF.COMMAND) &&
                 json.ContainsKey(UtilRF.ID) &&
                 json.ContainsKey(UtilRF.RADIOTYPE) &&
                 json.ContainsKey(UtilRF.FREQUENCY) &&
                 json.ContainsKey(UtilRF.POWER) &&
                 json.ContainsKey(UtilRF.REPETITION) &&
                 json.ContainsKey(UtilRF.DESCRIPTION) &&
                 json.ContainsKey(UtilRF.TYPE) &&
                 json.Value<int>(UtilRF.TYPE) == 1
                )
            {
                using RadioModel radio = new()
                {
                    Description = json.Value<string>(UtilRF.DESCRIPTION),
                    MemoryId = json.Value<int>(UtilRF.ID),
                    TypeRF = TypeRF.R433
                };

                selectedGateway.Radios433Gateway.Add(radio);

                _gatewayService.RadioCount = selectedGateway.RadiosRTSGateway.Count + selectedGateway.Radios433Gateway.Count;
            }
        }

        public async Task Update(GatewayModel selectedGateway, RadioModel radioModel)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (radioModel is null)
            {
                throw new ArgumentNullException(nameof(radioModel));
            }

            RadioModelToParseObject(selectedGateway, radioModel);

            _parseService.IsSendingToCloud = true;

            await radioModel.ParseObject.SaveAsync();

            ParseRelation<ParseObject> relation = selectedGateway.ParseObject.GetRelation<ParseObject>(RadioModel.RELATIONNAME);

            relation.Add(radioModel.ParseObject);

            await selectedGateway.ParseObject.SaveAsync();

            using RadioModel radio = new();

            radioModel.CopyPropertiesTo(radio);

            selectedGateway.Radios433Cloud.Add(radio);

            _parseService.IsSendingToCloud = false;
        }

        public async Task GetAllFromCloud(GatewayModel selectedGateway, TypeRF typeRF)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (selectedGateway.Radios433Cloud.Any())
            {
                selectedGateway.Radios433Cloud.Clear();
            }

            _parseService.IsSendingToCloud = true;

            IEnumerable<ParseObject> relation = await selectedGateway.ParseObject.GetRelation<ParseObject>(RadioModel.RELATIONNAME)
                                                           .Query.WhereEqualTo(RadioModel.TYPERF, (int)typeRF).FindAsync();

            _parseService.IsSendingToCloud = false;

            foreach (ParseObject parseObject in relation)
            {
                selectedGateway.Radios433Cloud.Add(ParseObjectToRadioModel(parseObject));
            }
        }

        private RadioModel ParseObjectToRadioModel(ParseObject parseObject)
        {
            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            using RadioModel radioModel = new()
            {
                ParseObject = parseObject
            };

            if (parseObject.ContainsKey(RadioModel.MEMORY_ID))
            {
                radioModel.MemoryId = parseObject.Get<int>(RadioModel.MEMORY_ID);
            }
            if (parseObject.ContainsKey(RadioModel.DESCRIPTION))
            {
                radioModel.Description = parseObject.Get<string>(RadioModel.DESCRIPTION);
            }

            if (parseObject.ContainsKey(RadioModel.TYPERF))
            {
                radioModel.TypeRF = (TypeRF)parseObject.Get<int>(RadioModel.TYPERF);
            }

            return radioModel;
        }

        private static void RadioModelToParseObject(GatewayModel selectedGateway, RadioModel radioModel)
        {
            radioModel.ParseObject ??= new ParseObject(RadioModel.CLASSNAME);
            radioModel.ParseObject[RadioModel.MEMORY_ID] = radioModel.MemoryId;
            radioModel.ParseObject[RadioModel.DESCRIPTION] = radioModel.Description;
            radioModel.ParseObject[RadioModel.TYPERF] = (int)radioModel.TypeRF;
            radioModel.ParseObject.ACL = selectedGateway.ParseObject.ACL;
        }

        public async Task PlayRadioMemory(GatewayModel selectedGateway, RadioModel radioModel)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (radioModel is null)
            {
                throw new ArgumentNullException(nameof(radioModel));
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

                if (!response.TryParseJObject(out json))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
                return;
            });

            string command = $"{{\"command\":\"playRadioMemory\",\"id\":{radioModel.MemoryId}, \"type\":0}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            commands: commands,
                            command: command);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (json.ContainsKey(UtilRF.ERROR))
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__Code, json.Value<string>(UtilRF.ERROR)));
            }
        }

        public async Task ResetPICAsync(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
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

                if (!response.TryParseJObject(out json))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            string command = $"{{\"command\":\"resetRadio\", \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway, replay: replay, commands: commands, command: command, timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                throw new Exception($"{Properties.Resources.Flex_Cloud_Cloner}\n{selectedGateway.LocalIP}\n{selectedGateway.LocalPortUDP} {Properties.Resources.Did_not_response}");
            }

            if (json.ContainsKey(UtilRF.ERROR))
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__Code, json.Value<int>(UtilRF.ERROR)));
            }
        }

        public async Task DeleteFromCloud(GatewayModel selectedGateway, RadioModel radioModel)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (radioModel is null)
            {
                throw new ArgumentNullException(nameof(radioModel));
            }

            if (radioModel.ParseObject is null)
            {
                _ = selectedGateway.Radios433Cloud.Remove(radioModel);
                return;
            }

            _parseService.IsSendingToCloud = true;

            await radioModel.ParseObject.DeleteAsync();

            _parseService.IsSendingToCloud = false;

            _ = selectedGateway.Radios433Cloud.Remove(radioModel);
        }

        public async Task DeleteAllCloud(GatewayModel selectedGateway, TypeRF r433)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            switch (r433)
            {
                case TypeRF.R433:

                    _parseService.IsSendingToCloud = true;

                    await ParseObject.DeleteAllAsync(selectedGateway.Radios433Cloud.Select(x => x.ParseObject));

                    _parseService.IsSendingToCloud = false;

                    selectedGateway.Radios433Cloud.Clear();
                    break;

                case TypeRF.RTS:
                    break;

                case TypeRF.Any:
                    break;

                default:
                    break;
            }
        }

        public async Task PlayRts(GatewayModel selectedGateway, RadioModel radio)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (radio is null)
            {
                throw new ArgumentNullException(nameof(radio));
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

                if (!response.TryParseJObject(out json))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            string command = $"{{\"command\":\"playRts\",\"id\":{radio.MemoryId}, \"button\":{(int)radio.ActionRTSSomfy}, \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway, replay: replay, commands: commands, command: command, timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                throw new Exception($"{Properties.Resources.Flex_Cloud_Cloner}\n{selectedGateway.LocalIP}\n{selectedGateway.LocalPortUDP} {Properties.Resources.Did_not_response}");
            }

            if (json.ContainsKey(UtilRF.ERROR))
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__Code, json.Value<int>(UtilRF.ERROR)));
            }

            if (selectedGateway.RadiosRTSGateway.FirstOrDefault(x => x.MemoryId == radio.MemoryId) is RadioModel temp)
            {
                radio.CopyPropertiesTo(temp);

                temp.TypeRF = TypeRF.RTS;
            }
        }

        public async Task AddSomfy(GatewayModel selectedGateway, RadioModel radio, ReplaySubject<string> replay)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (radio is null)
            {
                throw new ArgumentNullException(nameof(radio));
            }

            if (replay is null)
            {
                throw new ArgumentNullException(nameof(replay));
            }

            bool isNew = false;

            JObject json = null;

            commands.Clear();

            _ = replay.Subscribe((response) =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
                if (!response.TryParseJObject(out json))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.ERROR))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.TYPE))
                {
                    return;
                }

                if (json.Value<int>(UtilRF.TYPE) == 1)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            if (radio.MemoryId == -1)
            {
                isNew = true;
                _ = await GetNextEmptyId(selectedGateway, radio);
            }

            string command = $"{{\"command\": \"setupRts\",\"id\": {radio.MemoryId}, \"remoteId\":{radio.RemoteId}, \"rollingCode\": {radio.RollingCode}, \"repetition\": {radio.Repetition}, \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway, replay: replay, commands: commands, command: command, timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedGateway);
            }

            if (json.ContainsKey(UtilRF.ERROR))
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__Code, json.Value<int>(UtilRF.ERROR)));
            }

            if (isNew)
            {
                using RadioModel radioModel = new();

                radio.CopyPropertiesTo(radioModel);

                radioModel.TypeRF = TypeRF.RTS;

                selectedGateway.RadiosRTSGateway.Add(radioModel);
            }
            else
            {
                if (selectedGateway.RadiosRTSGateway.FirstOrDefault(x => x.MemoryId == radio.MemoryId) is RadioModel temp)
                {
                    radio.CopyPropertiesTo(temp);
                }
            }
            _gatewayService.RadioCount = selectedGateway.RadiosRTSGateway.Count + selectedGateway.Radios433Gateway.Count;
        }

        private readonly IList<object> commands = new List<object>();

        public async Task UpdatePICAsync(GatewayModel selectedGateway, ProtocolTypeEnum protocol = ProtocolTypeEnum.TCP)
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

                if (!json.ContainsKey(UtilRF.COMMAND))
                {
                    return;
                }
                if (!json.ContainsKey(UtilRF.ADDRESS))
                {
                    return;
                }
                if (!json.ContainsKey(UtilRF.PORT))
                {
                    return;
                }

                if (!json.ContainsKey(UtilRF.FILENAME))
                {
                    return;
                }
                if (!json.ContainsKey(UtilRF.TYPE))
                {
                    return;
                }

                if (json.Value<int>(UtilRF.TYPE) == 1)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"updateRadio433\", \"address\":\"{UtilRF.IPUPDATERADIO433}\", \"port\":\"80\", \"filename\":\"/IRF_311/r_fw/release.hex\", \"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway, replay: replay, commands: commands, command: command, timeout: 5000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();
        }

        private readonly ILocalDBRepository _localDBRepository;
        private readonly IParseService _parseService;

        public RFRepository(ITaskService taskService,
                            IGatewayService gatewayService,
                            ITcpRepository tcpRespository,
                            IUDPRepository udpRepository,
                            ILocalDBRepository localDBRepository,
                            IParseService parseService) : base(taskService, gatewayService, tcpRespository, udpRepository)
        {
            _localDBRepository = localDBRepository;

            _parseService = parseService;

            _taskService = taskService;

            _gatewayService = gatewayService;
        }
    }
}