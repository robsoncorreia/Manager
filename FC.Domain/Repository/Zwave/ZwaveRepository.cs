using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Device;
using FC.Domain.Model.Project;
using FC.Domain.Model.ZXT600;
using FC.Domain.Service;
using FC.Domain.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Domain.Repository.Zwave
{
    public interface IZwaveRepository
    {
        Task<bool> AddGatewayLifeline(ProjectModel selectedProject, bool isSendingToGateway = false);

        Task<bool> AllOff(ProjectModel selectedProjectModel);

        Task<bool> AllOn(ProjectModel selectedProjectModel);

        Task BulkSet(GatewayModel selectedGateway, Endpoint endpoint);

        Task DeleteAsync(ZwaveDevice zwaveDevice);

        Task GetAllAssociation(ProjectModel selectedProject, ZwaveDevice selectedDevice, bool isSendingToGateway = false);

        Task<ZwaveDevice[]> GetAllDeviceByCloud();

        Task<int> GetAllOnAllOff(GatewayModel selectedGateway, int moduleId);

        Task GetAssociationNumberGroups(ProjectModel selectedProject, ZwaveDevice selectedDevice, bool isSendingToGateway = false);

        Task<int?> GetBattery(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false);

        Task<int> GetBatteryStatus(GatewayModel gateway, int moduleId);

        Task<bool> GetBlaster360(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false);

        Task GetClock(GatewayModel gateway, int moduleId, DateTime clock);

        Task<bool> GetDemoMode(GatewayModel selectedGateway, int moduleId);

        Task<int> GetDemoSceneInterval(GatewayModel selectedGateway, int moduleId);

        Task<bool> GetDeviceManufacturer(ProjectModel selectedProject, ZwaveDevice device, bool isSendingToGateway = false);

        Task GetEconomicMode(GatewayModel selectedGateway);

        Task<bool> GetEndpointValue(ProjectModel selectedProjectModel, ZwaveDevice zwaveDevice);

        Task<bool> GetExternalIr(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false);

        Task GetInfoAllDeviceFromCloud();

        Task GetInfoDeviceFromCloud(ZwaveDevice device);

        Task<JObject> GetMultiLevelSensorStatus(ProjectModel selectedProject, Enum sensorType, Enum scale, bool pingGateway = false, bool pingZwave = false);

        Task<bool> GetNightMode(GatewayModel gateway, int moduleId);

        Task GetNightModeTime(GatewayModel gateway, int moduleId, NightModeTime nightModeTime);

        Task<bool> GetRecordLastState(GatewayModel selectedGateway, int moduleId);

        Task<bool> GetStatesEndpoints(ProjectModel selectedProject, bool isGetScene = true);

        Task<bool> GetSwing(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false);

        Task<int> GetTreeWayFunctions(GatewayModel selectedGateway, int moduleId);

        Task<bool> GetValues(ProjectModel selectedProjectModel);

        Task GetZwaveConfig(ProjectModel selectedProjectModel);

        Task<int> GetZwaveConfig(GatewayModel gateway, int moduleId, int parameter);

        Task<int> GetZwaveConfig(ProjectModel selectedProject, int parameter, bool pingGateway = false, bool pingZwave = false);

        Task<int> GetZXTModel(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false);

        Task<ZwaveDevice> IncludeZwaveDevice(ProjectModel selectedProject);

        Task InsertZwaveDevice(ProjectModel selectedProject, ZwaveDevice device, bool secondary = false);

        Task<bool> Learn(ProjectModel selectedProjectModel, ZXTIRLearningMappingModel mappingModel, bool pingGateway = false, bool pingZwave = false);

        Task<bool> LearnZwaveNetwork(ProjectModel selectedProjectModel);

        ZwaveDevice ParseObjectToZwaveDevice(ref ZwaveDevice device, ParseObject parseObject);

        Task<int> Remove(ProjectModel selectedProject);

        Task RemoveAssociation(ProjectModel selectedProject, ZwaveDevice device);

        Task RemoveDefectiveZwaveDevice(ProjectModel selectedProjectModel, ZwaveDevice selectedZwaveDevice);

        Task ReplaceZwaveDevice(ProjectModel selectedProjectModel, ZwaveDevice zwaveDevice, ReplaySubject<string> replay);

        Task ResetZwaveNetwork(ProjectModel selectedProject, bool isPrimary);

        Task RestartZwaveChip(ProjectModel selectedProject);

        Task SetAllOnAllOff(GatewayModel selectedGateway, int moduleId, int allOnAllOff);

        Task<bool> SetAssociation(ZwaveDevice zwaveDevice, ProjectModel selectedProjectModel);

        Task SetBlaster360(ProjectModel selectedProject, bool isBlaster360, bool pingGateway = false, bool pingZwave = false);

        Task SetClock(GatewayModel selectedGateway, int moduleId, DateTime clock);

        Task SetDemoMode(GatewayModel selectedGateway, int moduleId, bool isDemoMode);

        Task SetDemoSceneInterval(GatewayModel selectedGateway, int moduleId, int demoSceneInterval);

        Task SetEconomicMode(GatewayModel selectedGateway, Endpoint endpoint);

        Task SetExternalIr(ProjectModel selectedProject, bool isExternalIr, bool pingGateway = false, bool pingZwave = false);

        Task SetNightMode(GatewayModel selectedGateway, int moduleId, bool isNightMode);

        Task SetNightModeTime(GatewayModel selectedGateway, int moduleId, NightModeTime nightModeTime);

        Task SetRecordLastState(GatewayModel selectedGateway, int moduleId, bool isRecordLastState);

        Task<bool> SetScene(ProjectModel selectedProjectModel, Scene scene, bool isSendingToGateway = false);

        Task<bool> SetStateEndpoint(GatewayModel selectedDeviceModel, Endpoint endpoint);

        Task SetSwing(ProjectModel selectedProject, bool isSwing, bool pingGateway = false, bool pingZwave = false);

        Task SetTreeWayFunctions(GatewayModel selectedGateway, int moduleId, int index);

        Task<bool> SetZwaveConfig(ProjectModel selectedProjectModel);

        Task SetZwaveConfig(GatewayModel gateway, int moduleId, int parameter, int size, int value);

        Task<bool> SetZwaveConfig(ProjectModel selectedProject, int parameter, int size, int value, bool isPingGateway = false, bool isPingZwave = false);

        Task SetZXTModel(ProjectModel selectedProject, ZXTCodeModel code, bool pingGateway = false, bool pingZwave = false);

        Task<bool> Test(ProjectModel selectedProjectModel, ZwaveDevice zwaveDevice);

        Task<bool> ThermostatFanSet(ProjectModel selectedProject, ThermostatFanEnum thermostatFan, bool pingGateway = false, bool pingZwave = false);

        Task<bool> ThermostatModeSet(ProjectModel selectedProject, ThermostatModeEnum thermostatMode, bool pingGateway = false, bool pingZwave = false);

        Task<bool> ThermostatTemperatureSet(ProjectModel selectedProject, int temperature, bool pingGateway = false, bool pingZwave = false);

        Task Update(ZwaveDevice selectedZwaveDevice, ProjectModel selectedProjectModel);
    }

    public class ZwaveRepository : RepositoryBase, IZwaveRepository
    {
        #region Fields

        private readonly ILogRepository _logRepository;
        private readonly IParseService _parseService;
        private JObject json = null;

        #endregion Fields

        #region Collections

        private readonly List<object> commands = new();

        #endregion Collections

        #region Constructor

        public ZwaveRepository(ITaskService taskService,
                          IGatewayService gatewayService,
                          ITcpRepository tcpRespository,
                          IUDPRepository udpRepository,
                          ITcpRepository tcpRepository,
                          IParseService parseService,
                          ILogRepository logRepository) : base(taskService, gatewayService, tcpRespository, udpRepository)
        {
            _gatewayService = gatewayService;

            _udpRepository = udpRepository;

            _taskService = taskService;

            _tcpRepository = tcpRepository;

            _parseService = parseService;

            _logRepository = logRepository;
        }

        #endregion Constructor

        public async Task<bool> AddGatewayLifeline(ProjectModel selectedProject, bool isSendingToGateway = false)
        {
            using ReplaySubject<string> replay = new();

            json = null;

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

                  if (!json.ContainsKey(UtilZwave.COMMAND))
                  {
                      return;
                  }

                  if (json.Value<string>(UtilZwave.COMMAND) != UtilZwave.ASSOCIATIONSET)
                  {
                      return;
                  }

                  if (json.ContainsKey(UtilZwave.ZWAVEREPLY))
                  {
                      _taskService.CancellationTokenSource.Cancel();
                  }
              });

            string command = $"{{\"command\":\"associationSet\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"groupId\":1,\"addId\":1}}";

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway);

            replay.Dispose();

            return json is null ? throw new Exception(Properties.Resources.The_command_sent_did_not_return_a_result)
                                : json.Value<string>(UtilZwave.ZWAVEREPLY) == UtilZwave.OK;
        }

        public async Task<bool> AllOff(ProjectModel selectedProject)
        {
            using ReplaySubject<string> replay = new();

            bool @return = false;

            json = null;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }
                if (json.Value<string>(UtilZwave.ZWAVEREPLY) == UtilZwave.OK)
                {
                    @return = true;
                }
            });

            string command = $"{{\"command\":\"allOff\",\"type\":0}}";

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> AllOn(ProjectModel selectedProject)
        {
            using ReplaySubject<string> replay = new();

            bool @return = false;

            json = null;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }
                if (json.Value<string>(UtilZwave.ZWAVEREPLY) == UtilZwave.OK)
                {
                    @return = true;
                }
            });

            string command = $"{{\"command\":\"allOn\",\"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return @return;
        }

        public async Task BulkSet(GatewayModel selectedGateway, Endpoint endpoint)
        {
            int offset = 31;

            int[] positions = new int[17];

            for (int i = 0; i < positions.Length; i++)
            {
                if (selectedGateway.SelectedZwaveDevice.Endpoints.Count > i)
                {
                    positions[i] = selectedGateway.SelectedZwaveDevice.Endpoints.ElementAt(i).IsOnOff ? 0 : 1;
                }
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out JObject json))
                {
                    return;
                }

                //if (!json.ContainsKey(UtilZwave.DATA))
                //{
                //    return;
                //}

                //JArray x = json.Value<JArray>(UtilZwave.DATA);

                //int[] y = (int[])x.ToObject(typeof(int[]));

                //GetChannelValues(selectedGateway, y);

                //_taskService.CancellationTokenSource.Cancel();
            });

            string command = $"{{\"command\":\"setZwaveConfigLarge\",\"moduleId\":{selectedGateway.SelectedZwaveDevice.ModuleId},\"offset\":{offset},\"zwave\":[{positions[0]},{positions[1]},{positions[2]},{positions[3]},{positions[4]},{positions[5]},{positions[6]},{positions[7]},{positions[8]},{positions[9]},{positions[10]},{positions[11]},{positions[12]},{positions[13]},{positions[14]},{positions[15]},{positions[16]}],\"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();
        }

        public async Task DeleteAsync(ZwaveDevice zwaveDevice)
        {
            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            _parseService.IsSendingToCloud = true;

            await zwaveDevice.ParseObject.DeleteAsync();

            _parseService.IsSendingToCloud = false;
        }

        #region Get

        public async Task GetAllAssociation(ProjectModel selectedProject, ZwaveDevice selectedDevice, bool isSendingToGateway = true)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (selectedDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedDevice));
            }

            using ReplaySubject<string> replay = new();

            json = null;

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

                  if (!json.ContainsKey(UtilZwave.COMMAND))
                  {
                      return;
                  }

                  if (json.Value<string>(UtilZwave.COMMAND) == UtilZwave.ASSOCIATIONGET && json.ContainsKey(UtilZwave.DATA))
                  {
                      IEnumerable<int> ids = json.Value<JArray>(UtilZwave.DATA).ToObject<List<int>>().OrderBy(x => x).Distinct();

                      int groupId = json.Value<int>(UtilZwave.GROUP);

                      AssociationGroup association = selectedDevice.Associations.FirstOrDefault(x => x.GroupId == groupId);

                      association.MaxRegister = json.Value<int>("maxRegister");

                      association.Data = json.Value<JArray>(UtilZwave.DATA).ToObject<List<int>>();

                      System.Windows.Application.Current.Dispatcher.Invoke(delegate
                      {
                          association.ZwaveDevices.Clear();
                      });

                      for (int i = 0; i < ids.Count(); i++)
                      {
                          int id = ids.ElementAt(i);

                          ZwaveDevice device = selectedProject.SelectedGateway.IsPrimary ?
                                              selectedProject.SelectedGateway.ZwaveDevices.FirstOrDefault(x => x.ModuleId == id) :
                                              selectedProject.SelectedGateway.SecondaryZwaveDevices.FirstOrDefault(x => x.ModuleId == id);

                          device ??= new ZwaveDevice
                          {
                              ModuleId = id,
                              GroupId = i + 1,
                              Name = Properties.Resources.Device_not_found
                          };

                          System.Windows.Application.Current.Dispatcher.Invoke(delegate
                          {
                              device.GroupId = i + 1;
                              association.ZwaveDevices.Add(device);
                          });
                      }

                      _taskService.CancellationTokenSource.Cancel();
                  }
              });

            for (int i = 0; i < selectedDevice.NumberOfAssociationGroups; i++)
            {
                if (isCanceled)
                {
                    break;
                }
                string command = $"{{\"command\":\"associationGet\",\"type\":0, \"moduleId\":{selectedDevice.ModuleId},{(selectedDevice.IsEncrypted ? "\"crypto\":true," : null)}\"groupId\":{i + 1}}}";

                await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                                commands: commands,
                                replay: replay,
                                command: command,
                                isSendingToGateway: isSendingToGateway);
            }

            replay.Dispose();
        }

        public async Task<ZwaveDevice[]> GetAllDeviceByCloud()
        {
            _taskService.CancellationTokenSource = new CancellationTokenSource(60000);

            _parseService.IsSendingToCloud = true;

            IEnumerable<ParseObject> devicesParseObject = await ParseObject.GetQuery("SMA_DevicesDatabase")
                                                                           .WhereEqualTo(AppConstants.ISFRIENDLYSETTINGS, true)
                                                                           .WhereNotEqualTo(AppConstants.TYPE, ZWaveDeviceType.Gateway.ToString().ToLower())
                                                                           .OrderBy(AppConstants.DEFAULTNAMEDEVICEDATABASE)
                                                                           .FindAsync(_taskService.CancellationTokenSource.Token);

            ZwaveDevice[] devices = new ZwaveDevice[devicesParseObject.Count()];

            for (int i = 0; i < devicesParseObject.Count(); i++)
            {
                devices[i] = devicesParseObject.ElementAt(i).ParseObjectToZwaveDevice();
            }

            _parseService.IsSendingToCloud = true;

            return devices;
        }

        public async Task<int> GetAllOnAllOff(GatewayModel selectedGateway, int moduleId)
        {
            using ReplaySubject<string> replay = new();

            int @return = 0;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&ALLG{moduleId:X2}{"2703[1234567890ABCDEF]{2,}"}#"))
                {
                    return;
                }

                @return = response.Substring(11, 2) == "FF" ? 3 : int.Parse(response.Substring(11, 2));
            });

            string command = $"@ALLG{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task GetAssociationNumberGroups(ProjectModel selectedProject, ZwaveDevice selectedDevice, bool isSendingToGateway = true)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (selectedDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedDevice));
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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.Value<string>(UtilZwave.COMMAND) == UtilZwave.ASSOCIATIONNUMBEROFGROUPS && json.ContainsKey(UtilZwave.GROUP))
                {
                    selectedDevice.NumberOfAssociationGroups = json.Value<int>(UtilZwave.GROUP);

                    _taskService.CancellationTokenSource.Cancel();
                }
            });

            string command = $"{{\"command\":\"associationNumberofGroups\",\"moduleId\":{selectedDevice.ModuleId},{(selectedDevice.IsEncrypted ? "\"crypto\":true," : null)}\"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway);

            replay.Dispose();
        }

        public async Task<int> GetBatteryStatus(GatewayModel selectedGateway, int moduleId)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            json = null;

            using ReplaySubject<string> replay = new();

            int @return = -1;

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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.Value<string>(UtilZwave.COMMAND) != UtilZwave.BATTERYSTATUS)
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.LEVEL))
                {
                    @return = json.Value<int>(UtilZwave.LEVEL);

                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\":\"batteryStatus\",\"type\":0,\"moduleId\":{moduleId}}}";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            return json is null
                ? throw new Exception(Properties.Resources.Make_sure_the_device_is_on_battery_and_awake)
                : json.ContainsKey(UtilZwave.ERROR) ? throw new Exception(Properties.Resources.Error) : @return;
        }

        public async Task GetClock(GatewayModel selectedGateway, int moduleId, DateTime clock)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&JCKG[1234567890ABCDEF]{8,}#"))
                {
                    return;
                }

                clock = DateTime.Now.Date.Add(new TimeSpan(int.Parse(response.Substring(9, 2), System.Globalization.NumberStyles.HexNumber),
                                                           int.Parse(response.Substring(11, 2), System.Globalization.NumberStyles.HexNumber), 0));
            });

            string command = $"@JCKG02{moduleId:X2}8105#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();
        }

        public async Task<bool> GetDemoMode(GatewayModel selectedGateway, int moduleId)
        {
            using ReplaySubject<string> replay = new();

            bool @return = false;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFGG{moduleId:X2}{"700601[1234567890ABCDEF]{2,}"}0000#"))
                {
                    return;
                }

                @return = response.Substring(13, 2) == "01";
            });

            string command = $"@CFGG0D{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return @return;
        }

        public async Task<int> GetDemoSceneInterval(GatewayModel selectedGateway, int moduleId)
        {
            using ReplaySubject<string> replay = new();

            int index = 0;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFGG{moduleId:X2}700601{"[1234567890ABCDEF]{2,}"}0000#"))
                {
                    return;
                }
                if (int.TryParse(response.Substring(14, 1), out index))
                {
                    index--;
                    return;
                }
            });

            string command = $"@CFGG0E{moduleId:X2}#";

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return index;
        }

        public async Task<bool> GetDeviceManufacturer(ProjectModel selectedProject, ZwaveDevice device, bool isSendingToGateway = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            bool isRespond = false;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.COMMAND) && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.GETDEVICEMANUFACTURER && json.ContainsKey(UtilZwave.MANUFACTURER))
                {
                    device.Manufacturer = json.Value<int>(UtilZwave.MANUFACTURER);
                    device.ManufacturerKey = json.Value<int>(UtilZwave.MANUFACTURER);

                    device.Version = json.Value<int>(UtilZwave.VERSION);
                    device.FirmwareVersion = json.Value<int>(UtilZwave.VERSION);

                    device.Model = json.Value<int>(UtilZwave.MODEL);
                    device.ProductKey = json.Value<int>(UtilZwave.MODEL);

                    _taskService.CancellationTokenSource.Cancel();

                    isRespond = true;

                    return;
                }
            });

            string command = $"{{\"command\":\"getDeviceManufacturer\",\"moduleId\":{device.ModuleId},{(device.IsEncrypted ? "\"crypto\":true," : null)}\"type\":0}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command, isSendingToGateway);

            replay.Dispose();

            return isRespond;
        }

        public async Task GetEconomicMode(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            int index = -1;
            int moduleId = selectedGateway.SelectedZwaveDevice.ModuleId;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFGG{moduleId:X2}700601{"[1234567890ABCDEF]{2,}"}0000#"))
                {
                    return;
                }

                if (selectedGateway.SelectedZwaveDevice.Endpoints.ElementAt(index) is Endpoint endpoint)
                {
                    endpoint.EconomicMode = Convert.ToInt64(response.Substring(13, 2), 16).ConvertRange(0, 99, 0, 100);
                }
            });

            foreach (Endpoint endpoint in selectedGateway.SelectedZwaveDevice.Endpoints)
            {
                index = selectedGateway.SelectedZwaveDevice.Endpoints.IndexOf(endpoint);

                string command = $"@CFGG{index + 5:X2}{selectedGateway.SelectedZwaveDevice.ModuleId:X2}#";

                ;

                await ToCommand(selectedGateway: selectedGateway,
                                commands: commands,
                                replay: replay,
                                command: command);
            }

            replay.Dispose();
        }

        public async Task<bool> GetEndpointValue(ProjectModel selectedProject, ZwaveDevice zwave)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            string command = string.Empty;

            bool @return = false;

            using ReplaySubject<string> replay = new();

            commands.Clear();

            if (zwave.Endpoints?.ElementAt(zwave.SelectedIndexEndpoint) is not Endpoint endpoint)
            {
                return @return;
            }

            if (ZwaveModelUtil.FXA0600 == zwave.CustomId ||
                ZwaveModelUtil.FXA0404 == zwave.CustomId ||
                ZwaveModelUtil.FXA5018 == zwave.CustomId ||
                ZwaveModelUtil.FXA0400 == zwave.CustomId
                )
            {
                _gatewayService.IsSendingToGateway = true;

                ;

                _ = replay.Subscribe(response =>
                {
                    if (string.IsNullOrEmpty(response))
                    {
                        return;
                    }

                    if (!response.TryParseJObject(out JObject json))
                    {
                        return;
                    }

                    if (!json.ContainsKey(UtilZwave.DATA))
                    {
                        return;
                    }

                    @return = true;

                    JArray x = json.Value<JArray>(UtilZwave.DATA);

                    int[] y = (int[])x.ToObject(typeof(int[]));

                    GetChannelValues(zwave, y);

                    _taskService.CancellationTokenSource.Cancel();
                });

                command = $"{{\"command\":\"allStatusRead\",\"type\":0,\"moduleId\":{zwave.ModuleId}}}";

                ;

                await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                                commands: commands,
                                replay: replay,
                                command: command);

                replay.Dispose();

                return @return;
            }

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out JObject json))
                {
                    return;
                }
                switch (endpoint.GenericDeviceClass)
                {
                    case 16:
                        endpoint.IsOn = json.Value<int>(UtilZwave.LEVEL) == 255;
                        endpoint.EndpointStateIndex = endpoint.IsOn ? 1 : 0;
                        break;

                    case 17:
                        endpoint.MultiLevel = json.Value<long>(UtilZwave.LEVEL).ConvertRange(originalStart: 27, originalEnd: 99, newStart: 0, newEnd: 100);
                        break;
                }
                _taskService.CancellationTokenSource.Cancel();

                @return = true;
            });

            command = endpoint.GenericDeviceClass == 16 ? $"{{\"command\":\"mcBinarySwitchGet\",\"type\":0,\"moduleId\":{zwave.ModuleId},\"channel\":{endpoint.Channel}}}"
                                                        : $"{{\"command\":\"mcMultilevelGet\",\"type\":0,\"moduleId\":{zwave.ModuleId},\"channel\":{endpoint.Channel}}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task GetInfoAllDeviceFromCloud()
        {
            _taskService.CancellationTokenSource = new CancellationTokenSource(60000);

            IEnumerable<ParseObject> deviceParseObject = await ParseObject.GetQuery("SMA_DevicesDatabase")
                                                             .FindAsync(_taskService.CancellationTokenSource.Token);

            IList<ZwaveDevice> list = new List<ZwaveDevice>();

            foreach (ParseObject device in deviceParseObject)
            {
                ZwaveDevice zwave = new();
                _ = ParseObjectToZwaveDevice(ref zwave, device);
                list.Add(zwave);
            }
            //todo list
            //list.ExportToExcel();
        }

        public async Task GetInfoDeviceFromCloud(ZwaveDevice device)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource(60000);

            //ParseObject deviceParseObject = await ParseObject.GetQuery("SMA_DevicesDatabase")
            //                                                 .WhereEqualTo(AppConstants.MANUFACTURERKEYDEVICEDATABASE, 351)
            //                                                 .WhereEqualTo(AppConstants.PRODUCTKEYDEVICEDATABASE, 20739)
            //                                                 .WhereEqualTo(AppConstants.FIRMWAREVERSIONDEVICEDATABASE, 20769)
            //                                                 .FirstOrDefaultAsync(_taskService.CancellationTokenSource.Token);

            ParseObject deviceParseObject = await ParseObject.GetQuery("SMA_DevicesDatabase")
                                                             .WhereEqualTo(AppConstants.MANUFACTURERKEYDEVICEDATABASE, device.Manufacturer)
                                                             .WhereEqualTo(AppConstants.PRODUCTKEYDEVICEDATABASE, device.Model)
                                                             .WhereEqualTo(AppConstants.FIRMWAREVERSIONDEVICEDATABASE, device.Version)
                                                             .FirstOrDefaultAsync(_taskService.CancellationTokenSource.Token);

            if (deviceParseObject is null)
            {
                LogModel logModel = new()
                {
                    ErrorCode = LogEnum.Z_WaveDeviceNotFoundDatabase,
                    Description = Properties.Resources.Device_not_found_in_the_database_,
                    ManufacturerKey = device.Manufacturer,
                    ProductKey = device.Model,
                    FirmwareVersion = device.Version,
                    CustomId = device.CustomId,
                };

                await _logRepository.SaveLog(logModel);

                throw new Exception(Properties.Resources.Device_not_found_in_the_database_);
            }

            _ = ParseObjectToZwaveDevice(ref device, deviceParseObject);
        }

        public async Task<JObject> GetMultiLevelSensorStatus(ProjectModel selectedProject, Enum sensorType, Enum scale, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (scale is null)
            {
                throw new ArgumentNullException(nameof(scale));
            }

            JObject @return = null;

            using ReplaySubject<string> replay = new();

            commands.Clear();

            json = null;

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out json))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }
                if (json.Value<string>(UtilZwave.COMMAND) == UtilZwave.MULTILEVELSENSORMEASURE && json.ContainsKey(UtilZwave.VALUE))
                {
                    @return = json;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            string command = $"{{\"command\":\"multiLevelSensorMeasure\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"sensorType\":{(int)(object)sensorType},\"scale\":{(int)(object)scale}}}";

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> GetNightMode(GatewayModel selectedGateway, int moduleId)
        {
            using ReplaySubject<string> replay = new();

            bool @return = false;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGA[1234567890ABCDEF]{14,}#"))
                {
                    return;
                }

                @return = !(response == $"&CFGA{moduleId:X2}000100001A08#");
            });

            string command = $"@CFGA001A08{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return @return;
        }

        public async Task GetNightModeTime(GatewayModel selectedGateway, int moduleId, NightModeTime nightModeTime)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFGA{moduleId:X2}{"[1234567890ABCDEF]{18,}#"}"))
                {
                    return;
                }

                double initHour = int.Parse(response.Substring(11, 2), NumberStyles.HexNumber);
                double initMin = int.Parse(response.Substring(13, 2), NumberStyles.HexNumber);
                double endHour = int.Parse(response.Substring(15, 2), NumberStyles.HexNumber);
                double endtMin = int.Parse(response.Substring(17, 2), NumberStyles.HexNumber);

                double defaultInitHour = 23 - nightModeTime.StartNightModeTime.Hour;
                double defaultInitMin = 60 - nightModeTime.StartNightModeTime.Minute;
                double defaultEndHour = 23 - nightModeTime.EndNightModeTime.Hour;
                double defaultEndMin = 60 - nightModeTime.EndNightModeTime.Minute;

                nightModeTime.StartNightModeTime = nightModeTime.StartNightModeTime.AddHours(defaultInitHour);
                nightModeTime.StartNightModeTime = nightModeTime.StartNightModeTime.AddMinutes(defaultInitMin);
                nightModeTime.StartNightModeTime = nightModeTime.StartNightModeTime.AddHours(initHour);
                nightModeTime.StartNightModeTime = nightModeTime.StartNightModeTime.AddMinutes(initMin);

                nightModeTime.EndNightModeTime = nightModeTime.EndNightModeTime.AddHours(defaultEndHour);
                nightModeTime.EndNightModeTime = nightModeTime.EndNightModeTime.AddMinutes(defaultEndMin);
                nightModeTime.EndNightModeTime = nightModeTime.EndNightModeTime.AddHours(endHour);
                nightModeTime.EndNightModeTime = nightModeTime.EndNightModeTime.AddMinutes(endtMin);
            });

            string command = $"@CFGA001908{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();
        }

        public async Task<bool> GetRecordLastState(GatewayModel selectedGateway, int moduleId)
        {
            using ReplaySubject<string> replay = new();

            bool @return = false;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFGG{moduleId:X2}700601{"[1234567890ABCDEF]{2,}"}0000#"))
                {
                    return;
                }

                @return = response.Substring(14, 1) == "1";
            });

            string command = $"@CFGG03{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> GetStatesEndpoints(ProjectModel selectedProject, bool isGetScene = true)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            GatewayModel selectedGateway = selectedProject.SelectedGateway;

            bool @return = false;

            using ReplaySubject<string> replay = new();

            commands.Clear();

            if (selectedGateway.SelectedZwaveDevice.CustomId == ZwaveModelUtil.FXA0600 ||
                selectedGateway.SelectedZwaveDevice.CustomId == ZwaveModelUtil.FXA0404 ||
                selectedGateway.SelectedZwaveDevice.CustomId == ZwaveModelUtil.FXA0400 ||
                selectedGateway.SelectedZwaveDevice.CustomId == ZwaveModelUtil.FXA5014 ||
                selectedGateway.SelectedZwaveDevice.CustomId == ZwaveModelUtil.FXA5016 ||
                selectedGateway.SelectedZwaveDevice.CustomId == ZwaveModelUtil.FXA3012 ||
                selectedGateway.SelectedZwaveDevice.CustomId == ZwaveModelUtil.FXA3011 ||
                selectedGateway.SelectedZwaveDevice.CustomId == ZwaveModelUtil.FXA5018)

            {
                _ = replay.Subscribe(response =>
                   {
                       if (string.IsNullOrEmpty(response))
                       {
                           return;
                       }

                       if (!response.TryParseJObject(out JObject json))
                       {
                           return;
                       }

                       if (!json.ContainsKey(UtilZwave.DATA))
                       {
                           return;
                       }

                       @return = true;

                       JArray x = json.Value<JArray>(UtilZwave.DATA);

                       int[] y = (int[])x.ToObject(typeof(int[]));

                       GetChannelValues(selectedGateway, y, isGetScene);

                       _taskService.CancellationTokenSource.Cancel();
                   });

                _taskService.CancellationTokenSource = new CancellationTokenSource(10000);

                string command = $"{{\"command\":\"allStatusRead\",\"type\":0,\"moduleId\":{selectedGateway.SelectedZwaveDevice.ModuleId}}}";

                await ToCommand(selectedGateway, commands, replay, command);

                replay.Dispose();

                return @return;
            }

            _gatewayService.IsSendingToGateway = true;

            Endpoint endpoint = null;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!response.TryParseJObject(out JObject json))
                {
                    return;
                }
                switch (endpoint.GenericDeviceClass)
                {
                    case 16:
                        endpoint.IsOn = json.Value<int>(UtilZwave.LEVEL) == 255;
                        break;

                    case 17:
                        endpoint.MultiLevel = json.Value<long>(UtilZwave.LEVEL).ConvertRange(originalStart: 27, originalEnd: 99, newStart: 0, newEnd: 100);
                        break;
                }
                _taskService.CancellationTokenSource.Cancel();

                @return = true;
            });

            for (int i = 0; i < selectedGateway.SelectedZwaveDevice.Endpoints.Count; i++)
            {
                endpoint = selectedGateway.SelectedZwaveDevice.Endpoints.ElementAt(i);

                string command = BuildCommand(selectedGateway, endpoint);

                ;

                await ToCommand(selectedGateway, commands, replay, command);
            }

            replay.Dispose();

            return @return;
        }

        public async Task<int> GetTreeWayFunctions(GatewayModel selectedGateway, int moduleId)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            int @return = 1;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"CFGG{moduleId:X2}{"700601[1234567890ABCDEF]{2,}"}0000#"))
                {
                    return;
                }

                @return = int.Parse(response.Substring(13, 2), NumberStyles.HexNumber);
            });

            string command = $"@CFGG04{moduleId:X2}#";

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> GetValues(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            bool @return = false;

            json = null;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }
                if (json.ContainsKey(UtilZwave.LEVEL))
                {
                    selectedProject.SelectedGateway.SelectedZwaveDevice.MultiLevel = json.Value<int>(UtilZwave.LEVEL).ConvertRange(0, 99, 0, 100);
                    @return = true;
                }
            });

            string command = $"{{\"command\":\"multilevelGet\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId}}}";

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return @return;
        }

        #endregion Get

        public async Task<ZwaveDevice> IncludeZwaveDevice(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            ZwaveDevice device = null;

            json = null;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                IEnumerable<string> multiResponse = Regex.Split(response, @"(?<=[.}?])").Where(x => !string.IsNullOrEmpty(x) && x != "\n").Distinct();

                foreach (string resp in multiResponse)
                {
                    if (!resp.TryParseJObject(out json))
                    {
                        return;
                    }

                    if (!json.ContainsKey(UtilZwave.COMMAND))
                    {
                        return;
                    }

                    if (json.Value<string>(UtilZwave.COMMAND) == UtilZwave.NODEFOUND)
                    {
                        return;
                    }

                    if (json.Value<string>(UtilZwave.COMMAND) != UtilZwave.ADDZWAVEDEVICE)
                    {
                        return;
                    }

                    if (json.Value<string>(UtilZwave.STATE) == UtilZwave.STARTED && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.ADDZWAVEDEVICE)
                    {
                        return;
                    }

                    if (json.Value<string>(UtilZwave.STATE) == UtilZwave.TIMEOUT && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.ADDZWAVEDEVICE)
                    {
                        _taskService.CancellationTokenSource.Cancel();
                        return;
                    }

                    if (json.Value<string>(UtilZwave.STATE) == UtilZwave.FINISH && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.ADDZWAVEDEVICE)
                    {
                        _taskService.CancellationTokenSource.Cancel();
                        device = json.ToObject<ZwaveDevice>();

                        return;
                    }
                }
            });

            string command = UtilZwave.ADDZWAVEDEVICECOM;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands,
                            isMultipleResponses: true,
                            timeout: 30000);

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedProject.SelectedGateway);
            }

            return json.Value<string>(UtilZwave.STATE) == UtilZwave.TIMEOUT
                ? throw new Exception(Properties.Resources.Time_out)
                : json.ContainsKey(UtilZwave.ERROR)
                ? throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__error_code, json.Value<int>(UtilZwave.ERROR)))
                : device;
        }

        public async Task InsertZwaveDevice(ProjectModel selectedProject, ZwaveDevice device, bool secondary = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            _parseService.IsSendingToCloud = true;

            _taskService.CancellationTokenSource = new CancellationTokenSource(10000);

            ParseRelation<ParseObject> relation = selectedProject.SelectedGateway.ParseObject.GetRelation<ParseObject>(secondary ? AppConstants.SECONDARYZWAVEDEVICE : AppConstants.ZWAVEDEVICES);

            ParseObject deviceParseObject = await relation.Query.WhereEqualTo(AppConstants.MODULEIDDEVICEDATABASE, device.ModuleId).FirstOrDefaultAsync(_taskService.CancellationTokenSource.Token);

            device.ParseObject = deviceParseObject is null ? new ParseObject(AppConstants.ZWAVEDEVICECLASS) : deviceParseObject;

            device.ZwaveDeviceToParseObject(selectedProject);

            await device.ParseObject.SaveAsync();

            relation.Add(device.ParseObject);

            await selectedProject.SelectedGateway.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

            if (secondary)
            {
                ZwaveDevice dev = selectedProject.SelectedGateway.SecondaryZwaveDevices.FirstOrDefault(x => x?.ParseObject?.ObjectId == device.ParseObject.ObjectId);

                _ = selectedProject.SelectedGateway.SecondaryZwaveDevices.Remove(dev);

                selectedProject.SelectedGateway.SecondaryZwaveDevices.Add(device);
            }
            else
            {
                ZwaveDevice dev = selectedProject.SelectedGateway.ZwaveDevices.FirstOrDefault(x => x?.ParseObject?.ObjectId == device.ParseObject.ObjectId);

                _ = selectedProject.SelectedGateway.ZwaveDevices.Remove(dev);

                selectedProject.SelectedGateway.ZwaveDevices.Add(device);
            }

            _parseService.IsSendingToCloud = false;
        }

        public async Task<bool> LearnZwaveNetwork(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            json = null;

            bool @return = false;

            commands.Clear();

            _ = replay.Subscribe(response =>
              {
                  if (string.IsNullOrEmpty(response))
                  {
                      return;
                  }

                  IEnumerable<string> multiResponse = Regex.Split(response, @"(?<=[.}?])").Where(x => !string.IsNullOrEmpty(x) && x != "\n").Distinct();

                  foreach (string resp in multiResponse)
                  {
                      if (!resp.TryParseJObject(out json))
                      {
                          return;
                      }

                      if (!json.ContainsKey(UtilZwave.COMMAND))
                      {
                          return;
                      }

                      if (json.Value<string>(UtilZwave.COMMAND) != UtilZwave.LEARNZWAVENETWORK)
                      {
                          return;
                      }

                      if (json.Value<string>(UtilZwave.STATE) == UtilZwave.STARTED && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.LEARNZWAVENETWORK)
                      {
                          return;
                      }

                      if (json.Value<string>(UtilZwave.STATE) == UtilZwave.TIMEOUT && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.LEARNZWAVENETWORK)
                      {
                          _taskService.CancellationTokenSource.Cancel();
                          return;
                      }

                      if (json.Value<string>(UtilZwave.STATE) == UtilZwave.FINISH && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.LEARNZWAVENETWORK)
                      {
                          @return = true;
                          _taskService.CancellationTokenSource.Cancel();
                          return;
                      }
                  }
              });

            string command = UtilZwave.LEARNZWAVENETWORKCOM;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands,
                            isMultipleResponses: true,
                            timeout: 30000);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedProject.SelectedGateway);
            }

            return json.Value<string>(UtilZwave.STATE) == UtilZwave.TIMEOUT
                ? throw new Exception(Properties.Resources.Time_out)
                : json.ContainsKey(UtilZwave.ERROR)
                ? throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__error_code, json.Value<int>(UtilZwave.ERROR)))
                : @return;
        }

        public ZwaveDevice ParseObjectToZwaveDevice(ref ZwaveDevice device, ParseObject parseObject)
        {
            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (parseObject is null)
            {
                throw new ArgumentNullException(nameof(parseObject));
            }

            device.ParseObject = parseObject;

            if (parseObject.ContainsKey(AppConstants.SCENE))
            {
                device.Scene = parseObject.Get<int?>(AppConstants.SCENE);

                if (device.Scene is int lenght)
                {
                    for (int i = 0; i < lenght; i++)
                    {
                        device.Scenes.Add(new Scene { Number = i + 1 });
                    }
                }
            }

            if (parseObject.ContainsKey(AppConstants.NUMBEROFASSOCIATIONGROUPSDEVICEDATABASE))
            {
                device.NumberOfAssociationGroups = parseObject.Get<int>(AppConstants.NUMBEROFASSOCIATIONGROUPSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.DEFAULTNAMEDEVICEDATABASE))
            {
                device.DefaultName = parseObject.Get<string>(AppConstants.DEFAULTNAMEDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEIMAGEDEVICEDATABASE))
            {
                device.ImageParseFile = parseObject.Get<ParseFile>(AppConstants.DEVICEIMAGEDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.BASICDEVICECLASSDEVICEDATABASE))
            {
                device.BasicDeviceClass = parseObject.Get<int>(AppConstants.BASICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.GENERICDEVICECLASSDEVICEDATABASE))
            {
                device.GenericDeviceClass = parseObject.Get<int>(AppConstants.GENERICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.SPECIFICDEVICECLASSDEVICEDATABASE))
            {
                device.SpecificDeviceClass = parseObject.Get<int>(AppConstants.SPECIFICDEVICECLASSDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.MANUFACTURERKEYDEVICEDATABASE))
            {
                device.ManufacturerKey = parseObject.Get<int>(AppConstants.MANUFACTURERKEYDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.PRODUCTKEYDEVICEDATABASE))
            {
                device.ProductKey = parseObject.Get<int>(AppConstants.PRODUCTKEYDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.FIRMWAREVERSIONDEVICEDATABASE))
            {
                device.FirmwareVersion = parseObject.Get<int>(AppConstants.FIRMWAREVERSIONDEVICEDATABASE);
            }

            if (parseObject.ContainsKey(AppConstants.CRYPTOGRAPHYDEVICEDATABASE))
            {
                device.Cryptographys = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(parseObject.Get<IList<object>>(AppConstants.CRYPTOGRAPHYDEVICEDATABASE)));
            }

            if (parseObject.ContainsKey(AppConstants.COMMANDCLASSESDEVICEDATABASE))
            {
                device.CommandClasses = JsonConvert.DeserializeObject<IList<CommandClass>>(JsonConvert.SerializeObject(parseObject.Get<IList<object>>(AppConstants.COMMANDCLASSESDEVICEDATABASE)));
            }

            if (parseObject.ContainsKey(AppConstants.DEVICEENDPOINTSDEVICEDATABASE))
            {
                device.Endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(JsonConvert.SerializeObject(parseObject.Get<IList<object>>(AppConstants.DEVICEENDPOINTSDEVICEDATABASE)));
                for (int i = 0; i < device.Endpoints.Count; i++)
                {
                    device.Endpoints.ElementAt(i).Channel = i + 1;
                }
            }

            if (parseObject.ContainsKey(AppConstants.ASSOCIATIONGROUPSDEVICEDATABASE))
            {
                device.AssociationGroups = JsonConvert.DeserializeObject<IList<AssociationGroup>>(JsonConvert.SerializeObject(parseObject.Get<IList<object>>(AppConstants.ASSOCIATIONGROUPSDEVICEDATABASE)));
            }

            if (parseObject.ContainsKey(AppConstants.NAMEDEVICEDATABASE))
            {
                device.Name = parseObject.Get<string>(AppConstants.NAMEDEVICEDATABASE);
            }

            return device;
        }

        #region Remove

        public async Task<int> Remove(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            int moduleId = -1;

            json = null;

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

                  if (json.ContainsKey(UtilZwave.COMMAND) && json.ContainsKey(UtilZwave.ERROR))
                  {
                      _taskService.CancellationTokenSource.Cancel();
                      return;
                  }

                  if (!json.ContainsKey(UtilZwave.STATE))
                  {
                      return;
                  }

                  if (json.Value<string>(UtilZwave.STATE) == UtilZwave.FINISH)
                  {
                      moduleId = json.Value<int>(UtilZwave.MODULEID);
                      _taskService.CancellationTokenSource.Cancel();
                      return;
                  }

                  if (json.Value<string>(UtilZwave.STATE) == UtilZwave.TIMEOUT)
                  {
                      _taskService.CancellationTokenSource.Cancel();
                      return;
                  }
              });

            string command = UtilZwave.REMOVEZWAVEDEVICECOM;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands,
                            isMultipleResponses: true,
                            timeout: 30000);
            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            if (json is null)
            {
                ThrowException(selectedProject.SelectedGateway);
            }

            return json.Value<string>(UtilZwave.STATE) == UtilZwave.TIMEOUT
                ? throw new Exception(Properties.Resources.Time_out)
                : json.ContainsKey(UtilZwave.ERROR)
                ? throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__error_code, json.Value<int>(UtilZwave.ERROR)))
                : moduleId;
        }

        public async Task RemoveAssociation(ProjectModel selectedProject, ZwaveDevice device)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            using ReplaySubject<string> replay = new();

            json = null;

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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (!json.ContainsKey(UtilZwave.ZWAVEREPLY))
                {
                    return;
                }

                if (json.Value<string>(UtilZwave.COMMAND) == UtilZwave.ASSOCIATIONREMOVE && json.Value<string>(UtilZwave.ZWAVEREPLY) == UtilZwave.OK)
                {
                    if (selectedProject.SelectedGateway.SelectedZwaveDevice.Associations.FirstOrDefault(x => x.GroupId == device.GroupId) is AssociationGroup association)
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(delegate
                        {
                            _ = association.ZwaveDevices.Remove(device);
                        });
                    };

                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"associationRemove\",\"type\":0, \"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},\"groupId\":{device.GroupId},{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"removeId\":{device.ModuleId}}}";

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            if (json is null)
            {
                throw new Exception(Properties.Resources.The_command_sent_did_not_return_a_result);
            }
        }

        public async Task RemoveDefectiveZwaveDevice(ProjectModel selectedProject, ZwaveDevice selectedZwaveDevice)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (selectedZwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedZwaveDevice));
            }

            if (await DoesTheDeviceRespond(selectedProject, selectedZwaveDevice))
            {
                throw new Exception(Properties.Resources.Device_is_responding);
            }

            using ReplaySubject<string> replay = new();

            json = null;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                IList<string> multiResponse = Regex.Split(response, @"(?<=[.}?])").Where(x => !string.IsNullOrEmpty(x)).Distinct().ToList();

                _ = multiResponse.Remove("\n");

                foreach (string resp in multiResponse)
                {
                    if (string.IsNullOrEmpty(response))
                    {
                        return;
                    }

                    if (!response.TryParseJObject(out json))
                    {
                        continue;
                    }

                    if (json.ContainsKey(UtilZwave.ERROR))
                    {
                        _taskService.CancellationTokenSource.Cancel();
                        continue;
                    }

                    if (!json.ContainsKey(UtilZwave.STATE))
                    {
                        continue;
                    }

                    if (json.Value<string>(UtilZwave.STATE) == UtilZwave.STARTED)
                    {
                        continue;
                    }

                    if (json.Value<string>(UtilZwave.STATE) == UtilZwave.FINISH)
                    {
                        _taskService.CancellationTokenSource.Cancel();
                        continue;
                    }
                }
            });

            ;

            string command = $"{{\"command\":\"removeDefectiveZwaveDevice\",\"type\":0,\"moduleId\":{selectedZwaveDevice.ModuleId}}}";

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands,
                            isMultipleResponses: true);
            if (json is null)
            {
                replay.Dispose();
                ThrowException(selectedProject.SelectedGateway);
            }

            if (json.ContainsKey(UtilZwave.ERROR))
            {
                await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                replay: replay,
                command: command,
                commands: commands,
                isMultipleResponses: true);
            }

            if (json.ContainsKey(UtilZwave.ERROR))
            {
                throw new Exception(Properties.Resources.Error);
            }

            if (json.Value<string>(UtilZwave.STATE) == UtilZwave.NODEFOUND)
            {
                throw new Exception(Properties.Resources.Device_responded_normally);
            }

            if (json.Value<string>(UtilZwave.STATE) == UtilZwave.FINISH)
            {
                _taskService.CancellationTokenSource.Cancel();
                await selectedZwaveDevice.ParseObject.DeleteAsync();
                _ = selectedProject.SelectedGateway.ZwaveDevices.Remove(selectedZwaveDevice);
            }

            replay.Dispose();
        }

        #endregion Remove

        public async Task ReplaceZwaveDevice(ProjectModel selectedProject, ZwaveDevice zwaveDevice, ReplaySubject<string> replay)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            if (replay is null)
            {
                throw new ArgumentNullException(nameof(replay));
            }

            string command = $"{{\"command\":\"replaceZwaveDevice\",\"moduleId\": {zwaveDevice.ModuleId},\"type\":0}}";

            json = null;

            commands.Clear();

            _ = replay.Subscribe(resp =>
            {
                json = JObject.Parse(resp);

                if (!json.ContainsKey(UtilZwave.STATE))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (json.Value<string>(UtilZwave.STATE) == UtilZwave.TIMEOUT)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (json.Value<string>(UtilZwave.STATE) != UtilZwave.FINISH)
                {
                    return;
                }

                if (json.Value<int>(UtilZwave.MODULEID) < 1)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                //await RemoveDeviceFromGateway(selectedProject, json.Value<int>(UtilZwave.MODULEID));

                _taskService.CancellationTokenSource.Cancel();
            });

            _taskService.CancellationTokenSource = new CancellationTokenSource(5000);

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            if (json is null)
            {
                throw new Exception(Properties.Resources.Gateway_Did_Not_Respond);
            }

            if (json.Value<string>(UtilZwave.STATE) == UtilZwave.TIMEOUT)
            {
                throw new Exception(Properties.Resources.Time_out);
            }

            if (json.ContainsKey(UtilZwave.ERROR))
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__error_code, json.Value<int>(UtilZwave.ERROR)));
            }
        }

        #region Reset

        public async Task ResetZwaveNetwork(ProjectModel selectedProject, bool isPrimary)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            json = null;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.Value<string>(UtilZwave.COMMAND) == UtilZwave.RESETZWAVENETWORK &&
                    json.Value<string>(UtilZwave.ZWAVEREPLY) == UtilZwave.OK
                )
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = UtilZwave.RESETZWAVENETWORKCOM;

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedProject.SelectedGateway);
            }

            if (json.Value<string>(UtilZwave.COMMAND) == UtilZwave.RESETZWAVENETWORK &&
                json.Value<string>(UtilZwave.ZWAVEREPLY) == UtilZwave.OK)
            {
                if (isPrimary)
                {
                    foreach (ParseObject parseObject in selectedProject.SelectedGateway.ZwaveDevices.Select(x => x.ParseObject))
                    {
                        if (parseObject is null)
                        {
                            continue;
                        }
                        await parseObject.DeleteAsync();
                    }

                    List<ZwaveDevice> list = selectedProject.SelectedGateway.ZwaveDevices.Where(x => x.ModuleId != 1).ToList();

                    foreach (ZwaveDevice item in list)
                    {
                        _ = selectedProject.SelectedGateway.ZwaveDevices.Remove(item);
                    }
                }
                else
                {
                    foreach (ParseObject parseObject in selectedProject.SelectedGateway.SecondaryZwaveDevices.Select(x => x.ParseObject))
                    {
                        if (parseObject is null)
                        {
                            continue;
                        }
                        await parseObject.DeleteAsync();
                    }

                    selectedProject.SelectedGateway.SecondaryZwaveDevices.Clear();
                }
            }
        }

        public async Task RestartZwaveChip(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            json = null;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = UtilZwave.RESTARTZWAVECHIPCOM;

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            if (json is null)
            {
                ThrowException(selectedProject.SelectedGateway);
            }

            if (json.ContainsKey(UtilZwave.ERROR))
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__error_code, json.Value<int>(UtilZwave.ERROR)));
            }
        }

        #endregion Reset

        #region Set

        public async Task SetAllOnAllOff(GatewayModel selectedGateway, int moduleId, int allOnAllOff)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }
                if (!Regex.IsMatch(response, $"&ALLSOK#"))
                {
                    return;
                }
            });

            string command = $"@ALLS{(allOnAllOff == 3 ? "FF" : $"{allOnAllOff:X2}")}{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();
        }

        public async Task<bool> SetAssociation(ZwaveDevice zwaveDevice, ProjectModel selectedProject)
        {
            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            json = null;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.Value<string>(UtilZwave.COMMAND) != UtilZwave.ASSOCIATIONSET)
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.ZWAVEREPLY))
                {
                    _taskService.CancellationTokenSource.Cancel();
                }
            });

            string command = $"{{\"command\":\"associationSet\",\"type\":0, \"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},\"groupId\":{zwaveDevice.GroupId}, {(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"addId\":{zwaveDevice.ModuleId}}}";

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();

            return json is null
                ? throw new Exception(Properties.Resources.The_command_sent_did_not_return_a_result)
                : json.Value<string>(UtilZwave.ZWAVEREPLY) == UtilZwave.OK;
        }

        public async Task SetClock(GatewayModel selectedGateway, int moduleId, DateTime clock)
        {
            using ReplaySubject<string> replay = new();

            string command = $"@JCKS04{moduleId:X2}8104{clock.Hour:X2}{clock.Minute:X2}#";

            commands.Clear();

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);
        }

        public async Task SetDemoMode(GatewayModel selectedGateway, int moduleId, bool isDemoMode)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
              {
                  if (string.IsNullOrEmpty(response))
                  {
                      return;
                  }

                  if (!Regex.IsMatch(response, $"&CFGSOK"))
                  {
                      return;
                  }
              });

            string command = $"@CFGS0D01{(isDemoMode ? "01" : "00")}0000{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();
        }

        public async Task SetDemoSceneInterval(GatewayModel selectedGateway, int moduleId, int demoSceneInterval)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFGSOK#"))
                {
                    return;
                }
            });

            string command = $"@CFGS0E010{demoSceneInterval + 1}0000{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            replay.Dispose();
        }

        public async Task SetEconomicMode(GatewayModel selectedGateway, Endpoint endpoint)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (endpoint is null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            int index = selectedGateway.SelectedZwaveDevice.Endpoints.IndexOf(endpoint);

            int moduleId = selectedGateway.SelectedZwaveDevice.ModuleId;

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGSOK#"))
                {
                    return;
                }
            });

            string command = $"@CFGS{index + 5:X2}01{endpoint.EconomicMode.ConvertRange(0, 100, 0, 99):X2}0000{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);
        }

        public async Task SetNightMode(GatewayModel selectedGateway, int moduleId, bool isNightMode)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFSAOK"))
                {
                    return;
                }
            });

            string command = $"@CFSA001A0800{(isNightMode ? "01" : "00")}0000000000000000000000000000{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task SetNightModeTime(GatewayModel selectedGateway, int moduleId, NightModeTime nightModeTime)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFSAOK#"))
                {
                    return;
                }
            });

            string command = $"@CFSA00190800{nightModeTime.StartNightModeTime.Hour:X2}{nightModeTime.StartNightModeTime.Minute:X2}{nightModeTime.EndNightModeTime.Hour:X2}{nightModeTime.EndNightModeTime.Minute:X2}0000000000000000000000{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task SetRecordLastState(GatewayModel selectedGateway, int moduleId, bool isRecordLastState)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFGSOK#"))
                {
                    return;
                }
            });

            string command = $"@CFGS0301{(isRecordLastState ? "01" : "00")}0000{moduleId:X2}#";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task<bool> SetScene(ProjectModel selectedProjectModel, Scene scene, bool isSendingToGateway = false)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            if (scene is null)
            {
                throw new ArgumentNullException(nameof(scene));
            }

            commands.Clear();

            foreach (Scene item in selectedProjectModel.SelectedGateway.SelectedZwaveDevice.Scenes)
            {
                if (scene.Equals(item))
                {
                    item.IsOn = true;
                    continue;
                }
                item.IsOn = false;
            }

            using ReplaySubject<string> replay = new();

            bool @return = false;

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

                  if (!json.ContainsKey(UtilZwave.ZWAVEREPLY))
                  {
                      return;
                  }

                  if (!json.Value<string>(UtilZwave.ZWAVEREPLY).Equals(UtilZwave.OK))
                  {
                      return;
                  }

                  _taskService.CancellationTokenSource.Cancel();

                  @return = true;
              });

            string command = $"{{\"command\":\"sceneActivation\",\"type\":0,\"moduleId\":{selectedProjectModel.SelectedGateway.SelectedZwaveDevice.ModuleId},\"scene\":{scene.Number}}}";

            await ToCommand(selectedProjectModel.SelectedGateway, commands, replay, command, isSendingToGateway: isSendingToGateway);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> SetStateEndpoint(GatewayModel selectedGateway, Endpoint endpoint)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (endpoint is null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            bool @return = false;

            commands.Clear();

            using ReplaySubject<string> replay = new();

            string command = string.Empty;

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

                  @return = true;

                  _taskService.CancellationTokenSource.Cancel();
              });

            command = ZwaveModelUtil.FXA0404 == selectedGateway.SelectedZwaveDevice.CustomId ||
                ZwaveModelUtil.FXA0600 == selectedGateway.SelectedZwaveDevice.CustomId
                ? endpoint.IsOnOff ? $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedGateway.SelectedZwaveDevice.ModuleId},\"channel\":{endpoint.Channel},\"level\":{(endpoint.IsOn ? 255 : 0)}}}"
                                           : $"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{selectedGateway.SelectedZwaveDevice.ModuleId},\"channel\":{endpoint.Channel},\"level\":{endpoint.MultiLevel.ConvertRange()}}}"
                : endpoint.GenericDeviceClass == 16 ? $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{selectedGateway.SelectedZwaveDevice.ModuleId},\"channel\":{endpoint.Channel},\"level\":{(endpoint.IsOn ? 255 : 0)}}}"
                                                            : $"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{selectedGateway.SelectedZwaveDevice.ModuleId},\"channel\":{endpoint.Channel},\"level\":{endpoint.MultiLevel.ConvertRange()}}}";

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task SetTreeWayFunctions(GatewayModel selectedGateway, int moduleId, int index)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, $"&CFGSOK#"))
                {
                    return;
                }
            });

            string command = $"@CFGS0401{index:X2}0000{moduleId:X2}#";

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        #endregion Set

        public async Task<bool> Test(ProjectModel selectedProject, ZwaveDevice zwaveDevice)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            bool @return = false;

            commands.Clear();

            using ReplaySubject<string> replay = new();

            string command = string.Empty;

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

                  @return = true;

                  _taskService.CancellationTokenSource.Cancel();
              });

            if (ZwaveModelUtil.FXD220.Equals(zwaveDevice.CustomId))
            {
                command = $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"channel\": {zwaveDevice.SelectedIndexEndpoint + 1},\"level\":{zwaveDevice.MultiLevel.ConvertRange()}}}";
            }
            else if (
                    ZwaveModelUtil.FXD211.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXD211B.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXD5011.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXC221.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXM5012.Equals(zwaveDevice.CustomId)
                    )
            {
                command = $"{{\"command\":\"multilevelSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"level\":{zwaveDevice.MultiLevel.ConvertRange()}}}";
            }
            else if (
                    ZwaveModelUtil.FXA0600.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA0404.Equals(zwaveDevice.CustomId)
                    )
            {
                command = zwaveDevice.Endpoints.ElementAt(zwaveDevice.SelectedIndexEndpoint).IsOnOff
                    ? $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"channel\": {zwaveDevice.SelectedIndexEndpoint + 1},\"level\":{((EndpointState)zwaveDevice.Endpoints.ElementAt(zwaveDevice.SelectedIndexEndpoint).EndpointStateIndex == EndpointState.On ? 255 : 0)}}}"
                    : $"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"channel\":{zwaveDevice.SelectedIndexEndpoint + 1},\"level\":{zwaveDevice.Endpoints.ElementAt(zwaveDevice.SelectedIndexEndpoint).MultiLevel.ConvertRange()}}}";
            }
            else if (
                    ZwaveModelUtil.FXA0400.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA3011.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA3012.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA5016.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA5018.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA5018.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA5018.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXC222.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA5029.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXA5014.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXR5011.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXR5012.Equals(zwaveDevice.CustomId) ||
                    ZwaveModelUtil.FXR5013.Equals(zwaveDevice.CustomId)
                    )
            {
                long? genericDeviceClass = zwaveDevice.Endpoints.ElementAt(zwaveDevice.SelectedIndexEndpoint).GenericDeviceClass;

                if (genericDeviceClass == 17)
                {
                    command = $"{{\"command\":\"mcMultilevelSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"channel\":{zwaveDevice.SelectedIndexEndpoint + 1},\"level\":{zwaveDevice.Endpoints.ElementAt(zwaveDevice.SelectedIndexEndpoint).MultiLevel.ConvertRange()}}}";
                }
                else if (genericDeviceClass == 16)
                {
                    switch (zwaveDevice.Endpoints.ElementAt(zwaveDevice.SelectedIndexEndpoint).EndpointStateIndex)
                    {
                        case 0:
                            command = $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"channel\": {zwaveDevice.SelectedIndexEndpoint + 1},\"level\":0}}";
                            break;

                        case 1:
                            command = $"{{\"command\":\"mcBasicSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"channel\": {zwaveDevice.SelectedIndexEndpoint + 1},\"level\":255}}";
                            break;

                        case 2:
                            break;
                    }
                }
            }
            else if (ZwaveModelUtil.ZXT600.Equals(zwaveDevice.CustomId))
            {
                command = GetCommandZXT600(zwaveDevice);
            }
            else if (ZwaveModelUtil.FXS69A.Equals(zwaveDevice.CustomId) ||
                     ZwaveModelUtil.DOORLOCK.Equals(zwaveDevice.CustomId))
            {
                command = $"{{\"command\":\"binarySwitchSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"level\":{(zwaveDevice.IsOn ? 255 : 0)}}}";
            }
            else if (ZwaveModelUtil.ONOFF.Equals(zwaveDevice.CustomId))
            {
                command = $"{{\"command\":\"basicSet\",\"type\":0,\"moduleId\":{zwaveDevice.ModuleId},\"level\":{((EndpointState)zwaveDevice.StateIndex == EndpointState.On ? 255 : 0)}}}";
            }

            if (string.IsNullOrEmpty(command))
            {
                return false;
            }

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        private string GetCommandZXT600(ZwaveDevice zwaveDevice)
        {
            string command = string.Empty;

            switch (zwaveDevice.SelectedIndexThermostatFunction)
            {
                case 0:

                    string roomTemperature = zwaveDevice.RoomTemperature.ToString();

                    command = $"{{\"command\":\"thermostatTemperatureSet\", \"type\":0,{(zwaveDevice.IsEncrypted ? "\"crypto\":true," : null)} \"moduleId\":{zwaveDevice.ModuleId},\"temperature\":{(roomTemperature.Length == 1 ? roomTemperature : roomTemperature.Remove(roomTemperature.Length - 1))}}}";

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

        public async Task Update(ZwaveDevice zwaveDevice, ProjectModel selectedProjectModel)
        {
            _parseService.IsSendingToCloud = true;

            if (zwaveDevice is null)
            {
                throw new ArgumentNullException(nameof(zwaveDevice));
            }

            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }
            if (zwaveDevice.ParseObject is null)
            {
                _parseService.IsSendingToCloud = false;
                return;
            }
            zwaveDevice.ZwaveDeviceToParseObject(selectedProjectModel);

            await zwaveDevice.ParseObject.SaveAsync();

            _parseService.IsSendingToCloud = false;
        }

        private static string BuildCommand(GatewayModel selectedDeviceModel, Endpoint endpoint)
        {
            if (selectedDeviceModel is null)
            {
                throw new ArgumentNullException(nameof(selectedDeviceModel));
            }

            if (endpoint is null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }
            //todo provavel erro
            return selectedDeviceModel.SelectedZwaveDevice.CustomId.Equals(ZwaveModelUtil.FXD220)
                ? $"{{\"command\":\"mcBasicGet\",\"type\":0,\"moduleId\":{selectedDeviceModel.SelectedZwaveDevice.ModuleId},\"channel\":{endpoint.Channel}}}"
                : endpoint.GenericDeviceClass switch
                {
                    16 => $"{{\"command\":\"mcBinarySwitchGet\",\"type\":0,\"moduleId\":{selectedDeviceModel.SelectedZwaveDevice.ModuleId},\"channel\":{endpoint.Channel}}}",
                    17 => $"{{\"command\":\"mcMultilevelGet\",\"type\":0,\"moduleId\":{selectedDeviceModel.SelectedZwaveDevice.ModuleId},\"channel\":{endpoint.Channel}}}",
                    _ => $"{{\"command\":\"mcBinarySwitchGet\",\"type\":0,\"moduleId\":{selectedDeviceModel.SelectedZwaveDevice.ModuleId},\"channel\":{endpoint.Channel}}}",
                };
        }

        private async Task<bool> DoesTheDeviceRespond(ProjectModel selectedProject, ZwaveDevice selectedZwaveDevice, int timeout = 2000)
        {
            bool isRespond = false;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.COMMAND) && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.GETDEVICEMANUFACTURER && json.ContainsKey(UtilZwave.MANUFACTURER))
                {
                    _taskService.CancellationTokenSource.Cancel();

                    isRespond = true;

                    return;
                }
            });

            string command = $"{{\"command\":\"getDeviceManufacturer\",\"moduleId\":{selectedZwaveDevice.ModuleId},\"type\":0}}";

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            timeout: timeout,
                            command: command);

            replay.Dispose();

            return isRespond;
        }

        #region GetChannelValues

        private void GetChannelValues(ZwaveDevice zwave, int[] comando)
        {
            var response = new
            {
                COMMAND_CLASS_PROPRIETARY = comando[0],
                PROPRIETARY_REPORT = comando[1],
                Model1 = comando[2],
                Model2 = comando[3],
                Dimers = comando[4],
                Relays = comando[5],
                Scenes = comando[6],
                Channels = new int[]
                {
                    comando[7],
                    comando[8],
                    comando[9],
                    comando[10],
                    comando[11],
                    comando[12],
                    comando[13],
                    comando[14],
                },
                Values = new int[]
                {
                    comando[15],
                    comando[16],
                    comando[17],
                    comando[18],
                    comando[19],
                    comando[20],
                    comando[21],
                    comando[22]
                },
                All = comando[23],
                Scene = comando[24],
                Hour = comando[25],
                Minute = comando[26]
            };
            for (int i = 0; i < zwave.Endpoints.Count; i++)
            {
                zwave.Endpoints.ElementAt(i).IsOnOff = response.Channels[i] == (int)EndpointType.OnOff;
            }

            for (int i = 0; i < zwave.Scenes?.Count; i++)
            {
                if (response.Scene - 1 == i)
                {
                    zwave.Scenes.ElementAt(i).IsOn = true;
                    continue;
                }
                zwave.Scenes.ElementAt(i).IsOn = false;
            }

            zwave.SelectedIndexScene = response.Scene - 1;

            for (int i = 0; i < zwave.Endpoints?.Count; i++)
            {
                if (zwave.Endpoints.ElementAt(i).IsOnOff)
                {
                    zwave.Endpoints.ElementAt(i).IsOn = response.Values[i] == 255;
                    zwave.Endpoints.ElementAt(i).EndpointStateIndex = response.Values[i] == 255 ? 1 : 0;
                    continue;
                }
                zwave.Endpoints.ElementAt(i).MultiLevel = response.Values[i].ConvertRange(27, 99, 0, 100);
            }
        }

        private void GetChannelValues(GatewayModel gateway, int[] comando, bool isGetScene)
        {
            var response = new
            {
                COMMAND_CLASS_PROPRIETARY = comando[0],
                PROPRIETARY_REPORT = comando[1],
                Model1 = comando[2],
                Model2 = comando[3],
                Dimers = comando[4],
                Relays = comando[5],
                Scenes = comando[6],
                Channels = new int[]
                {
                    comando[7],
                    comando[8],
                    comando[9],
                    comando[10],
                    comando[11],
                    comando[12],
                    comando[13],
                    comando[14],
                },
                Values = new int[]
                {
                    comando[15],
                    comando[16],
                    comando[17],
                    comando[18],
                    comando[19],
                    comando[20],
                    comando[21],
                    comando[22]
                },
                All = comando[23],
                Scene = comando[24],
                Hour = comando[25],
                Minute = comando[26]
            };
            for (int i = 0; i < gateway.SelectedZwaveDevice.Endpoints.Count; i++)
            {
                gateway.SelectedZwaveDevice.Endpoints.ElementAt(i).IsOnOff = response.Channels[i] == (int)EndpointType.OnOff;
            }

            if (isGetScene)
            {
                for (int i = 0; i < gateway.SelectedZwaveDevice?.Scenes?.Count; i++)
                {
                    if (response.Scene - 1 == i)
                    {
                        gateway.SelectedZwaveDevice.Scenes.ElementAt(i).IsOn = true;
                        continue;
                    }
                    gateway.SelectedZwaveDevice.Scenes.ElementAt(i).IsOn = false;
                }
                gateway.SelectedZwaveDevice.SelectedIndexScene = response.Scene - 1;
            }

            for (int i = 0; i < gateway.SelectedZwaveDevice.Endpoints.Count; i++)
            {
                if (gateway.SelectedZwaveDevice.Endpoints.ElementAt(i).IsOnOff)
                {
                    gateway.SelectedZwaveDevice.Endpoints.ElementAt(i).IsOn = response.Values[i] == 255;
                    continue;
                }
                gateway.SelectedZwaveDevice.Endpoints.ElementAt(i).MultiLevel = response.Values[i].ConvertRange(27, 99, 0, 100);
            }
        }

        #endregion GetChannelValues

        #region Ping

        private async Task PingGateway(ProjectModel selectedProject, int @try = 2, string com = "@GCR#", string resp = "&GCR[1234567890ABCDEF]{76,}#")
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (string.IsNullOrEmpty(com))
            {
                throw new ArgumentException($"'{nameof(com)}' não pode ser nulo nem vazio.", nameof(com));
            }

            if (string.IsNullOrEmpty(resp))
            {
                throw new ArgumentException($"'{nameof(resp)}' não pode ser nulo nem vazio.", nameof(resp));
            }

            using ReplaySubject<string> replay = new();

            int tryCount = 0;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    ;

                    tryCount++;

                    if (tryCount >= @try)
                    {
                        throw new ArgumentException(Properties.Resources.Gateway_did_not_respond_to_commands__check_if_the_computer_is_on_the_same_network_as_the_Gateway_and_the_Gateway_is_turned_on_);
                    }
                    return;
                }

                if (!Regex.IsMatch(response, resp))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, com);

            replay.Dispose();
        }

        private async Task PingZwave(ProjectModel selectedProject, int @try = 4)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            int tryCount = 0;

            string command = $"{{\"command\":\"getDeviceManufacturer\",\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"type\":0}}";

            commands.Clear();

            _ = replay.Subscribe(response =>
           {
               if (string.IsNullOrEmpty(response))
               {
                   tryCount++;

                   if (tryCount >= @try)
                   {
                       throw new Exception(Properties.Resources.Make_sure_the_device_is_powered_on_and_is_still_part_of_the_Z_Wave_network_);
                   }
                   return;
               }
           });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        #endregion Ping

        #region ZXT

        public async Task<int?> GetBattery(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            //string command = $"@SBTG{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";
            string command = $"{{\"command\":\"batteryStatus\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId}}}";

            int? @return = null;

            json = null;

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.LEVEL))
                {
                    @return = json.Value<int>(UtilZwave.LEVEL);
                    _taskService.CancellationTokenSource.Cancel();
                }
            });

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return isCanceled ? throw new Exception(Domain.Properties.Resources.Task_canceled) : @return;
        }

        public async Task<bool> GetBlaster360(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }
            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"@CFGG20{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            bool @return = false;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGG[1234567890ABCDEF]{2,}700601[1234567890ABCDEF]{2,}0000#"))
                {
                    return;
                }

                @return = response.Substring(13, 2).Equals("FF");

                _taskService.CancellationTokenSource.Cancel();
            });

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> GetExternalIr(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"@CFGG1C{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            bool @return = false;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGG[1234567890ABCDEF]{2,}700601[1234567890ABCDEF]{2,}0000#"))
                {
                    return;
                }

                @return = response.Substring(13, 2).Equals("FF");

                _taskService.CancellationTokenSource.Cancel();
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> GetSwing(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)

            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"@CFGG21{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            bool @return = false;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGG[1234567890ABCDEF]{2,}700601[1234567890ABCDEF]{2,}0000#"))
                {
                    return;
                }

                @return = response.Substring(13, 2).Equals("01");

                _taskService.CancellationTokenSource.Cancel();
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task<int> GetZXTModel(ProjectModel selectedProject, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"@CFGG1B{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            int @return = -1;

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGG[1234567890ABCDEF]{2,}700602[1234567890ABCDEF]{4,}00#"))
                {
                    return;
                }

                int lenght = response.Length;

                string codeHex = response.Substring(lenght - 7, 4);

                if (!int.TryParse(codeHex, System.Globalization.NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int code))
                {
                    throw new System.Exception(Properties.Resources.Device_Not_Responding);
                }

                @return = code;

                _taskService.CancellationTokenSource.Cancel();
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> Learn(ProjectModel selectedProject, ZXTIRLearningMappingModel mappingModel, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (mappingModel is null)
            {
                throw new ArgumentNullException(nameof(mappingModel));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            bool @return = false;

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGSOK#"))
                {
                    return;
                }

                @return = true;

                _taskService.CancellationTokenSource.Cancel();
            });

            string command = $"@CFGS190200{mappingModel.StorageLocation:X2}00{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return isCanceled ? throw new Exception(Domain.Properties.Resources.Task_canceled) : @return;
        }

        public async Task SetBlaster360(ProjectModel selectedProject, bool isBlaster360, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"@CFGS210200{(isBlaster360 ? "FF" : "00")}00{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGSOK#"))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task SetExternalIr(ProjectModel selectedProject, bool isExternalIr, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }
            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"@CFGS210200{(isExternalIr ? "FF" : "00")}00{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGSOK#"))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task SetSwing(ProjectModel selectedProject, bool isSwing, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }
            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"@CFGS210200{(isSwing ? "01" : "00")}00{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGSOK#"))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task SetZXTModel(ProjectModel selectedProject, ZXTCodeModel code, bool pingGateway = false, bool pingZwave = false)
        {
            if (code is null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"@CFGS1B02{code.Codes[code.SelectedIndexCode]:X4}00{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId:X2}K#";

            commands.Clear();

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (!Regex.IsMatch(response, "&CFGSOK#"))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
            });

            ;

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task<bool> ThermostatFanSet(ProjectModel selectedProject, ThermostatFanEnum thermostatFan, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"{{\"command\":\"thermostatFanModeSet\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"mode\":{(int)thermostatFan}}}";

            json = null;

            commands.Clear();

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.COMMAND) && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.THERMOSTATFANMODESET)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> ThermostatModeSet(ProjectModel selectedProject,
                                            ThermostatModeEnum thermostatMode,
                                            bool pingGateway = false,
                                            bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            string command = $"{{\"command\":\"thermostatModeSet\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"mode\":{(int)thermostatMode}}}";

            json = null;

            commands.Clear();

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

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.COMMAND) && json.Value<string>(UtilZwave.COMMAND) == UtilZwave.THERMOSTATMODESET)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        public async Task<bool> ThermostatTemperatureSet(ProjectModel selectedProject, int temperature, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            json = null;

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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!json.ContainsKey(UtilZwave.ZWAVEREPLY))
                {
                    return;
                }
                if (json.Value<string>(UtilZwave.ZWAVEREPLY) == UtilZwave.OK)
                {
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"thermostatTemperatureSet\", \"type\":0,{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)} \"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},\"temperature\":{temperature}}}";

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return @return;
        }

        #endregion ZXT

        #region SetZwaveConfig

        public async Task<bool> SetZwaveConfig(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            using ReplaySubject<string> replay = new();

            json = null;

            commands.Clear();

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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (json.ContainsKey(UtilZwave.PARAMETER))
                {
                    selectedProject.SelectedGateway.SelectedZwaveDevice.Parameter = json.Value<int>(UtilZwave.PARAMETER);

                    selectedProject.SelectedGateway.SelectedZwaveDevice.Size = json.Value<int>(UtilZwave.SIZE);

                    selectedProject.SelectedGateway.SelectedZwaveDevice.Value = json.Value<int>(UtilZwave.VALUE);

                    _taskService.CancellationTokenSource.Cancel();

                    @return = true;

                    return;
                }
            });

            string command = $"{{\"command\":\"setZwaveConfig\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},\"parameter\":{selectedProject.SelectedGateway.SelectedZwaveDevice.Parameter},\"size\":{selectedProject.SelectedGateway.SelectedZwaveDevice.Size},\"value\":{selectedProject.SelectedGateway.SelectedZwaveDevice.Value}}}";

            await ToCommand(selectedGateway: selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return isCanceled
                ? throw new Exception(Properties.Resources.Task_canceled)
                : json is null
                ? throw new Exception(Properties.Resources.Gateway_Did_Not_Respond)
                : json.ContainsKey(UtilZwave.ERROR)
                ? throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__error_code, json.Value<int>(UtilZwave.ERROR)))
                : @return;
        }

        public async Task<bool> SetZwaveConfig(ProjectModel selectedProject, int parameter, int size, int value, bool isPingGateway = false, bool isPingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (isPingGateway)
            {
                await PingGateway(selectedProject);
            }

            if (isPingZwave)
            {
                await PingZwave(selectedProject);
            }

            using ReplaySubject<string> replay = new();

            json = null;

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(response =>
            {
                if (string.IsNullOrEmpty(response))
                {
                    return;
                }

                if (response.TryParseJObject(out json))
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (json.ContainsKey(UtilZwave.SETZWAVECONFIG))
                {
                    _taskService.CancellationTokenSource.Cancel();

                    @return = true;

                    return;
                }
            });

            string command = $"{{\"command\":\"setZwaveConfig\",\"type\":0,{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},\"parameter\":{parameter},\"size\":{size},\"value\":{value}}}";

            await ToCommand(selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            return isCanceled ? throw new Exception(Domain.Properties.Resources.Task_canceled) : @return;
        }

        public async Task SetZwaveConfig(GatewayModel selectedGateway, int moduleId, int parameter, int size, int value)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            json = null;

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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (json.ContainsKey(UtilZwave.PARAMETER))
                {
                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\":\"setZwaveConfig\",\"type\":0,\"moduleId\":{moduleId},\"parameter\":{parameter},\"size\":{size},\"value\":{value}}}";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();

            if (json is null)
            {
                throw new Exception(Properties.Resources.Make_sure_the_device_is_on_battery_and_awake);
            }

            if (json.ContainsKey(UtilZwave.ERROR))
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Something_went_wrong__error_code, json.Value<int>(UtilZwave.ERROR)));
            }
        }

        #endregion SetZwaveConfig

        #region GetZwaveConfig

        public async Task<int> GetZwaveConfig(GatewayModel selectedGateway, int moduleId, int parameter)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            json = null;

            using ReplaySubject<string> replay = new();

            commands.Clear();

            int @return = -1;

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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.Value<string>(UtilZwave.COMMAND) != UtilZwave.GETZWAVECONFIG)
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.PARAMETER))
                {
                    @return = json.Value<int>(UtilZwave.VALUE);

                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\":\"getZwaveConfig\",\"type\":0,\"moduleId\":{moduleId},{(selectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"parameter\":{parameter}}}";

            ;

            await ToCommand(selectedGateway: selectedGateway, commands, replay, command);

            replay.Dispose();

            _gatewayService.IsSendingToGateway = false;

            return json is null
                ? throw new Exception(Properties.Resources.Make_sure_the_device_is_on_battery_and_awake)
                : json.ContainsKey(UtilZwave.ERROR) ? throw new Exception(Properties.Resources.Error) : @return;
        }

        public async Task GetZwaveConfig(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            json = null;

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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.Value<string>(UtilZwave.COMMAND) != UtilZwave.GETZWAVECONFIG)
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.PARAMETER))
                {
                    selectedProject.SelectedGateway.SelectedZwaveDevice.Parameter = json.Value<int>(UtilZwave.PARAMETER);

                    selectedProject.SelectedGateway.SelectedZwaveDevice.Size = json.Value<int>(UtilZwave.SIZE);

                    selectedProject.SelectedGateway.SelectedZwaveDevice.Value = json.Value<int>(UtilZwave.VALUE);

                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\":\"getZwaveConfig\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"parameter\":{selectedProject.SelectedGateway.SelectedZwaveDevice.Parameter}}}";

            ;

            await ToCommand(selectedGateway: selectedProject.SelectedGateway, commands, replay, command);

            replay.Dispose();

            if (json is null)
            {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Properties.Resources.Gateway_Did_Not_Respond, selectedProject.SelectedGateway.LocalIP, 9999));
            }

            if (json.ContainsKey(UtilZwave.ERROR))
            {
                throw new Exception(Properties.Resources.Error);
            }
        }

        public async Task<int> GetZwaveConfig(ProjectModel selectedProject, int parameter, bool pingGateway = false, bool pingZwave = false)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (pingGateway)
            {
                await PingGateway(selectedProject);
            }
            if (pingZwave)
            {
                await PingZwave(selectedProject);
            }

            json = null;

            using ReplaySubject<string> replay = new();

            int @return = -1;

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

                if (json.ContainsKey(UtilZwave.ERROR))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }

                if (!json.ContainsKey(UtilZwave.COMMAND))
                {
                    return;
                }

                if (json.Value<string>(UtilZwave.COMMAND) != UtilZwave.GETZWAVECONFIG)
                {
                    return;
                }

                if (json.ContainsKey(UtilZwave.VALUE))
                {
                    @return = json.Value<int>(UtilZwave.VALUE);

                    _taskService.CancellationTokenSource.Cancel();

                    return;
                }
            });

            string command = $"{{\"command\":\"getZwaveConfig\",\"type\":0,\"moduleId\":{selectedProject.SelectedGateway.SelectedZwaveDevice.ModuleId},{(selectedProject.SelectedGateway.SelectedZwaveDevice.IsEncrypted ? "\"crypto\":true," : null)}\"parameter\":{parameter}}}";

            await ToCommand(selectedGateway: selectedProject.SelectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command);

            return isCanceled ? throw new Exception(Domain.Properties.Resources.Task_canceled) : @return;
        }

        #endregion GetZwaveConfig
    }
}