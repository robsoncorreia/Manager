using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Device;
using FC.Domain.Model.Project;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Util;
using FC.Domain.Service;
using FC.Domain.Service.Rede;
using FC.Domain.Util;
using Newtonsoft.Json.Linq;
using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface IGatewayRepository
    {
        Task CustomUpdate(GatewayModel selectedGateway, bool isSendingToGateway = false);

        Task Delete(GatewayModel selectedDevice);

        Task<bool> FindGateway(GatewayModel gateway, ReplaySubject<string> replay = null, int timeout = 10000, bool isSendingToGateway = false);

        Task GetAll(ProjectModel selectedProject);

        Task GetBuild(GatewayModel selectedGateway);

        Task GetFirmawes(GatewayModel selectedGateway);

        Task GetGatewayInfo(GatewayModel selectedGateway, bool isSendingToGateway = true);

        Task GetGateways(GatewayModelEnum type, IPAddress address, ReplaySubject<string> replay = null, ReplaySubject<int> rxProgressBarValue = null, int timeout = 8000);

        Task GetUID(GatewayModel selectedGateway);

        Task GetZwaveDevices(ProjectModel selectedProject);

        Task Insert(ProjectModel selectedProjectModel, GatewayModel device);

        Task Reboot(GatewayModel selectedGateway);

        Task Rename(GatewayModel selectedDevice);

        Task SetAPDHCP(GatewayModel selectedGateway);

        Task SetEthernetNetworkConfiguration(GatewayModel selectedGateway);

        Task SetEthernetPort(GatewayModel selectedGateway);

        Task SetSSIDPasswordAP(GatewayModel selectedGateway);

        Task<bool> SetWIFIAP(GatewayModel selectedGateway);

        Task SetWIFIAPSSID(GatewayModel selectedGateway);

        Task SetWifiNetworkConfiguration(GatewayModel selectedGateway);

        Task SetWIFIPass(GatewayModel selectedGateway);

        Task SetWifiPort(GatewayModel selectedGateway);

        Task SetWIFISSID(GatewayModel selectedGateway);

        Task SetWifiStation(GatewayModel selectedGateway);

        Task Test(GatewayModel gateway);

        Task Update(ProjectModel selectedProjectModel);

        Task Update(GatewayModel selectedGateway);

        Task UpdateBlacklistUsers(GatewayModel selectedDevice);

        Task UpdateFirmawe(GatewayModel selectedGateway);
    }

    public class GatewayRepository : RepositoryBase, IGatewayRepository
    {
        #region Fields

        public const string BLASTERBIN = "{\"command\":\"UpdateEsp32\",\"address\":\"18.219.240.204\",\"port\":\"80\",\"filename\":\"/FCB/latest_release.bin\",\"type\":0}";
        public const string CLONERBIN = "{\"command\":\"UpdateEsp32\",\"address\":\"18.219.240.204\",\"port\":\"80\",\"filename\":\"/FRG/latest_release.bin\",\"type\":0}";
        public const string HUBBIN = "{\"command\":\"UpdateEsp32\",\"address\":\"18.219.240.204\",\"port\":\"80\",\"filename\":\"/FCH/latest_release.bin\",\"type\":0}";
        public const int NUMBERTRY = 20;
        private const int MILLISECONDSDELAYAWAITREBOOT = 15000;
        private readonly IIPCommandRepository _ipCommandRepository;
        private readonly IIRRepository _irRepository;
        private readonly IParseService _parseService;
        private readonly IRelayRepository _relayRepository;
        private readonly IRFRepository _rFRepository;
        private readonly ISerialRepository _serialRepository;

        #endregion Fields

        #region Collections

        private readonly IList<object> commands = new List<object>();

        public GatewayRepository(ITaskService taskService, IRelayRepository relayRepository, IGatewayService gatewayService, ITcpRepository tcpRepository, IUDPRepository udpRepository, IRFRepository rFRepository, IIRRepository irRepository, IParseService parseService, ISerialRepository serialRepository, IIPCommandRepository ipCommandRepository) : base(taskService, gatewayService, tcpRepository, udpRepository)
        {
            _relayRepository = relayRepository;

            _udpRepository = udpRepository;

            _rFRepository = rFRepository;

            _tcpRepository = tcpRepository;

            _taskService = taskService;

            _gatewayService = gatewayService;

            _irRepository = irRepository;

            _parseService = parseService;

            _serialRepository = serialRepository;

            _ipCommandRepository = ipCommandRepository;
        }

        #endregion Collections

        public async Task CustomUpdate(GatewayModel selectedGateway, bool isSendingToGateway = false)
        {
            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
                return;
            });

            string model = string.Empty;

            GetModel(selectedGateway, ref model);

            string build = selectedGateway.Builds.ElementAt(selectedGateway.SelectedIndexBuild).Version; ;

            string firmware = selectedGateway.Firmwares.ElementAt(selectedGateway.SelectedIndexFirmware).Version;

            string command = $"{{\"command\":\"UpdateEsp32\",\"address\":\"18.219.240.204\",\"port\":\"80\",\"filename\":\"/{model}/fw/{firmware}{build}\",\"type\":0}}";

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: command,
                            isSendingToGateway: isSendingToGateway,
                            isMultipleResponses: false,
                            repetitions: 4,
                            timeout: 30000);

            replay.Dispose();
        }

        public async Task Delete(GatewayModel selectedDevice)
        {
            if (selectedDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedDevice));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource();
            await selectedDevice.ParseObject.DeleteAsync(_taskService.CancellationTokenSource.Token);
        }

        public async Task<bool> FindGateway(GatewayModel gateway, ReplaySubject<string> replay = null, int timeout = 10000, bool isSendingToGateway = false)
        {
            return gateway is null
                ? throw new ArgumentNullException(nameof(gateway))
                : await _udpRepository.FindGateway(GatewayModelEnum.ANY,
                                                    replay: replay,
                                                    timeout: timeout,
                                                    gateway: gateway,
                                                    isSendingToGateway: isSendingToGateway);
        }

        #region Get

        private static void GetModel(GatewayModel selectedGateway, ref string model)
        {
            switch (selectedGateway.GatewayModelEnum)
            {
                case GatewayModelEnum.FCIRF100311:
                    model = UtilGateway.IRF311;
                    break;

                case GatewayModelEnum.FCIRF100211:
                    model = UtilGateway.IRF211;
                    break;

                case GatewayModelEnum.FCZWS100V1:
                    model = UtilGateway.ZWS100;
                    break;

                case GatewayModelEnum.FCZWS100V2:
                    model = UtilGateway.ZWS100;
                    break;

                case GatewayModelEnum.FCZIR100311:
                    model = UtilGateway.ZIR311;
                    break;

                case GatewayModelEnum.FCZIR100211:
                    model = UtilGateway.ZIR211;
                    break;

                case GatewayModelEnum.FCGIR100211:
                    model = UtilGateway.GIR211;
                    break;

                case GatewayModelEnum.FCGIR100311:
                    model = UtilGateway.GIR311;
                    break;

                case GatewayModelEnum.ANY:
                    return;

                default:
                    return;
            }
        }

        private string GetBuildName(string innerText)
        {
            return innerText is null
                ? throw new ArgumentNullException(nameof(innerText))
                : innerText.Equals("esp_r_i.bin")
                ? UtilGateway.RELEASEINTEGRATION
                : innerText.Equals("esp_r.bin")
                ? UtilGateway.RELEASE
                : innerText.Equals("esp_d_i.bin")
                ? UtilGateway.DEBUGINTEGRATION
                : innerText.Equals("esp_d.bin") ? UtilGateway.DEBUG : string.Empty;
        }

        public async Task GetAll(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            IEnumerable<ParseObject> devices = await selectedProject.ParseObject.GetRelation<ParseObject>(AppConstants.GATEWAYS)
                                                                                .Query
                                                                                .Include(AppConstants.GATEWAYREMOTEACCESSSTANDALONE)
                                                                                .FindAsync();

            if (selectedProject.Devices.Any())
            {
                selectedProject.Devices.Clear();
            }

            foreach (ParseObject parseObject in devices)
            {
                if (parseObject.ParseObjectToGateway() is GatewayModel deviceModel)
                {
                    selectedProject.Devices.Add(deviceModel);
                }
            }
        }

        public async Task GetBuild(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }
            selectedGateway.Builds.Clear();

            using HttpClient client = new();

            string model = string.Empty;

            switch (selectedGateway.GatewayModelEnum)
            {
                case GatewayModelEnum.FCIRF100311:
                    model = UtilGateway.IRF311;
                    break;

                case GatewayModelEnum.FCIRF100211:
                    model = UtilGateway.IRF211;
                    break;

                case GatewayModelEnum.FCZWS100V1:
                    model = UtilGateway.ZWS100;
                    break;

                case GatewayModelEnum.FCZWS100V2:
                    model = UtilGateway.ZWS100;
                    break;

                case GatewayModelEnum.FCZIR100311:
                    model = UtilGateway.ZIR311;
                    break;

                case GatewayModelEnum.FCZIR100211:
                    model = UtilGateway.ZIR211;
                    break;

                case GatewayModelEnum.FCGIR100211:
                    model = UtilGateway.GIR211;
                    break;

                case GatewayModelEnum.FCGIR100311:
                    model = UtilGateway.GIR311;
                    break;

                case GatewayModelEnum.ANY:
                    return;

                default:
                    return;
            }

            _gatewayService.IsSendingToGateway = true;

            FirmwareModel xy = selectedGateway.Firmwares[selectedGateway.SelectedIndexFirmware];

            string url = $"http://18.219.240.204/{model}/fw/{xy.Version}";

            using HttpResponseMessage response = await client.GetAsync(url);

            StreamReader streamRetorno = new(await response.Content.ReadAsStreamAsync());

            string html = streamRetorno.ReadToEnd();

            HtmlAgilityPack.HtmlDocument doc = new();

            doc.LoadHtml(html);

            HtmlAgilityPack.HtmlNodeCollection allElementsWithClassG = doc.DocumentNode.SelectNodes("//a");

            Regex rx = new(@"^.*\.(bin)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            if (selectedGateway.Builds.Any())
            {
                selectedGateway.Builds.Clear();
            }

            foreach (HtmlAgilityPack.HtmlNode x in allElementsWithClassG.OrderByDescending(x => x.InnerText))
            {
                if (rx.IsMatch(x.InnerText))
                {
                    using BuildModel build = new()
                    {
                        Name = GetBuildName(x.InnerText),
                        Version = x.InnerText
                    };

                    selectedGateway.Builds.Add(build);
                }
            }

            selectedGateway.SelectedIndexBuild = 0;

            _gatewayService.IsSendingToGateway = false;
        }

        public async Task GetFirmawes(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (selectedGateway.Firmwares.Any())
            {
                selectedGateway.Firmwares.Clear();
            }

            string model = string.Empty;

            GetModel(selectedGateway, ref model);

            using HttpClient client = new();

            string url = $"http://18.219.240.204/{model}/fw/";

            using HttpResponseMessage response = await client.GetAsync(url);

            StreamReader streamRetorno = new(await response.Content.ReadAsStreamAsync());

            Regex rxVersions = new(@"^[0-9]{3}_[0-9]{3}_[0-9]{3}\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex rxVersionsV2 = new(@"^[0-9]{6}\/", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Regex rxLastmodifieds = new(@"^[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}\:[0-9]{2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            string html = streamRetorno.ReadToEnd();

            if (html.Contains("404 Not Found"))
            {
                throw new Exception(string.Format(Properties.Resources.Page_not_found_page, url));
            }

            HtmlAgilityPack.HtmlDocument doc = new();

            doc.LoadHtml(html);

            List<HtmlAgilityPack.HtmlNode> allElementsWithA = doc.DocumentNode.SelectNodes("//a").Where(x => rxVersions.IsMatch(x.InnerText) || rxVersionsV2.IsMatch(x.InnerText)).ToList();

            List<HtmlAgilityPack.HtmlNode> allElementsWithTd = doc.DocumentNode.SelectNodes("//td").Where(x => rxLastmodifieds.IsMatch(x.InnerText)).ToList();

            if (selectedGateway.Firmwares.Any())
            {
                selectedGateway.Firmwares.Clear();
            }

            List<string> lastmodifieds = new();

            List<string> versions = new();

            List<DateTime> dateTimes = new();

            for (int i = 0; i < allElementsWithTd.Count; i++)
            {
                lastmodifieds.Add(allElementsWithTd[i].InnerText.Replace("-", "").Replace(":", "").Replace(" ", "").Substring(2, 8));
                string date = allElementsWithTd[i].InnerText.Replace("-", "").Replace(":", "").Replace(" ", "");
                DateTime dateTime = new(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2)), int.Parse(date.Substring(8, 2)), int.Parse(date.Substring(10, 2)), 0);
                dateTimes.Add(dateTime);
            }

            for (int i = 0; i < allElementsWithA.Count; i++)
            {
                versions.Add(allElementsWithA[i].InnerText);
            }

            dateTimes.Reverse();

            versions.Reverse();

            lastmodifieds.Reverse();

            FirmwareModel[] firmwares = new FirmwareModel[versions.Count];

            for (int i = 0; i < firmwares.Length; i++)
            {
                using FirmwareModel firmware = new()
                {
                    Name = lastmodifieds[i],
                    Version = versions[i],
                    LastModified = dateTimes[i]
                };
                firmwares[i] = firmware;
            }

            firmwares = firmwares.OrderByDescending(x => x.LastModified).ToArray();

            foreach (FirmwareModel firmware in firmwares)
            {
                selectedGateway.Firmwares.Add(firmware);
            }

            selectedGateway.SelectedIndexFirmware = 0;
        }

        public async Task GetGatewayInfo(GatewayModel selectedGateway, bool isSendingToGateway = false)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            commands.Clear();

            using ReplaySubject<string> replay = new();

            string command = string.Empty;

            _ = replay.Subscribe(resp =>
              {
                  if (string.IsNullOrEmpty(resp))
                  {
                      return;
                  }

                  if (resp.TryParseJObject() is not JObject json)
                  {
                      return;
                  }

                  if (!json.ContainsKey(UtilGateway.COMMAND))
                  {
                      return;
                  }

                  if (!json.ContainsKey(UtilGateway.TYPE))
                  {
                      return;
                  }

                  if (json.Value<int>(UtilGateway.TYPE) != 1)
                  {
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFIAPSSID && json.ContainsKey(UtilGateway.SSID))
                  {
                      selectedGateway.APSSID = json.Value<string>(UtilGateway.SSID);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFIAPPASS && json.ContainsKey(UtilGateway.PASSWORD))
                  {
                      selectedGateway.APPassword = json.Value<string>(UtilGateway.PASSWORD);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFIAPENABLE && json.ContainsKey(UtilGateway.ISENABLED))
                  {
                      selectedGateway.SelectedIndexAPStatus = json.Value<bool>(UtilGateway.ISENABLED) ? 1 : 0;
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFIAPDHCP && json.ContainsKey(UtilGateway.ISENABLED))
                  {
                      selectedGateway.SelectedIndexAPDHCP = json.Value<bool>(UtilGateway.ISENABLED) ? 1 : 0;
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFINETWORKCONFIGURATION && json.ContainsKey(UtilGateway.DHCP))
                  {
                      selectedGateway.SelectedIndexIPTypeWiFi = json.Value<bool>(UtilGateway.DHCP) ? 0 : 1;

                      selectedGateway.CurrentIpWiFi = json.JArrayToIP(UtilGateway.CURRENTIP);

                      selectedGateway.StaticIPWiFi = json.JArrayToIP(UtilGateway.STATICIP);

                      selectedGateway.RouterGatewayWiFi = json.JArrayToIP(UtilGateway.GATEWAYIP);

                      selectedGateway.MaskWiFi = json.JArrayToIP(UtilGateway.NETWORKMASK);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFIPORTNUMBER && json.ContainsKey(UtilGateway.TCPPORT))
                  {
                      selectedGateway.TcpPortWiFi = json.Value<int>(UtilGateway.TCPPORT);

                      selectedGateway.UdpPortWifi = json.Value<int>(UtilGateway.UDPPORT);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFISTATIONSSID && json.ContainsKey(UtilGateway.SSID))
                  {
                      selectedGateway.SSID = json.Value<string>(UtilGateway.SSID);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFISTATIONPASS && json.ContainsKey(UtilGateway.PASSWORD))
                  {
                      selectedGateway.Password = json.Value<string>(UtilGateway.PASSWORD);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFISTATIONENABLE && json.ContainsKey(UtilGateway.ISENABLED))
                  {
                      selectedGateway.IsEnableWifi = json.Value<bool>(UtilGateway.ISENABLED);
                      selectedGateway.SelectedIndexWiFiStatus = selectedGateway.IsEnableWifi ? 1 : 0;
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETWIFIMACADDRESS && json.ContainsKey(UtilGateway.MACADDRESS))
                  {
                      selectedGateway.MacAddressWiFi = json.JArrayToMAC(UtilGateway.MACADDRESS);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETETHNETWORKCONFIG && json.ContainsKey(UtilGateway.NETWORKMASK))
                  {
                      selectedGateway.SelectedIndexIPTypeEthernet = json.Value<bool>(UtilGateway.DHCP) ? 0 : 1;
                      selectedGateway.CurrentIpEthernet = json.JArrayToIP(UtilGateway.CURRENTIP);
                      selectedGateway.StaticIPEthernet = json.JArrayToIP(UtilGateway.STATICIP);
                      selectedGateway.RouterGatewayEthernet = json.JArrayToIP(UtilGateway.GATEWAYIP);
                      selectedGateway.MaskEthernet = json.JArrayToIP(UtilGateway.NETWORKMASK);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETETHPORTNUMBER && json.ContainsKey(UtilGateway.TCPPORT))
                  {
                      selectedGateway.TcpPortEthernet = json.Value<int>(UtilGateway.TCPPORT);
                      selectedGateway.UdpPortEthernet = json.Value<int>(UtilGateway.UDPPORT);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETETHERNETMAC && json.ContainsKey(UtilGateway.MACADDRESS))
                  {
                      selectedGateway.MacAddressEthernet = json.JArrayToMAC(UtilGateway.MACADDRESS);
                      selectedGateway.Pin = selectedGateway.MacAddressEthernet.Substring(12, selectedGateway.MacAddressEthernet.Length - 12).Replace(":", "");
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETZWAVEFREQUENCY && json.ContainsKey(UtilGateway.FREQUENCY))
                  {
                      selectedGateway.ZwaveFrequency = (ZwaveFrequencyEnum)json.Value<int>(UtilGateway.FREQUENCY);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.CHECKPRIMARY && json.ContainsKey(UtilGateway.ISPRIMARY))
                  {
                      selectedGateway.IsPrimary = json.Value<bool>(UtilGateway.ISPRIMARY);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETZWAVEHOMEID && json.ContainsKey(UtilGateway.HOMEID))
                  {
                      selectedGateway.HomeId = json.Value<string>(UtilGateway.HOMEID);
                      selectedGateway.ModuleId = GetModuleIdGateway(selectedGateway.HomeId);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETUNIQUEID && json.ContainsKey(UtilGateway.UID))
                  {
                      selectedGateway.UID ??= json.Value<string>(UtilGateway.UID);
                      selectedGateway.Build = json.Value<int>(UtilGateway.BUILD);
                      return;
                  }

                  if (json.Value<string>(UtilGateway.COMMAND) == UtilGateway.GETCONFIGIR && json.ContainsKey(UtilGateway.CHANNELCOUNT))
                  {
                      selectedGateway.IsIRBlasterAvailable = json.Value<bool>(UtilGateway.BLASTERAVAILABLE);
                      selectedGateway.IRCommandsLimit = json.Value<int>(UtilGateway.COMMANDSLIMIT);
                      selectedGateway.IRSizeLimit = json.Value<int>(UtilGateway.IRSIZELIMIT);
                      return;
                  }
              });

            #region Port

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.WIFIPORTNUMBERGET,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            await ToCommand(selectedGateway: selectedGateway,
                           commands: commands,
                           replay: replay,
                           command: UtilRede.ETHPORTNUMBERGET,
                           isSendingToGateway: isSendingToGateway,
                           isForceUDP: true);

            #endregion Port

            #region IP

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.WIFINETWORKCONFIGURATIONGET,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.ETHNETWORKCONFIGGET,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            #endregion IP

            #region WI-FI

            await ToCommand(selectedGateway: selectedGateway,
                  commands: commands,
                  replay: replay,
                  command: UtilRede.GETWIFISTATIONSSID,
                  isSendingToGateway: isSendingToGateway,
                  isForceUDP: true);

            await ToCommand(selectedGateway: selectedGateway,
                  commands: commands,
                  replay: replay,
                  command: UtilRede.GETWIFISTATIONPASS,
                  isSendingToGateway: isSendingToGateway,
                  isForceUDP: true);

            await ToCommand(selectedGateway: selectedGateway,
                              commands: commands,
                              replay: replay,
                              command: UtilRede.GETWIFISTATIONENABLE,
                              isSendingToGateway: isSendingToGateway,
                              isForceUDP: true);

            await ToCommand(selectedGateway: selectedGateway,
                              commands: commands,
                              replay: replay,
                              command: UtilRede.WIFIMACADDRESSGET,
                              isSendingToGateway: isSendingToGateway,
                              isForceUDP: true);

            #endregion WI-FI

            #region Ethernet

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.GETETHERNETMAC,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            #endregion Ethernet

            #region AP

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.GETWIFIAPSSID,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.GETWIFIAPPASS,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.GETWIFIAPENABLE,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.GETWIFIAPDHCP,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            #endregion AP

            #region IR

            if (selectedGateway?.GatewayModelEnum != GatewayModelEnum.FCZWS100V2)
            {
                await ToCommand(selectedGateway: selectedGateway,
                                commands: commands,
                                replay: replay,
                                command: UtilGateway.GETCONFIGIRCOMMAND,
                                isSendingToGateway: isSendingToGateway,
                                isForceUDP: true);
            }

            #endregion IR

            #region Zwave

            GatewayModelEnum[] compatibleDevices = new GatewayModelEnum[] { GatewayModelEnum.FCZIR100311, GatewayModelEnum.FCZWS100V2 };

            if (compatibleDevices.Contains(selectedGateway.GatewayModelEnum))
            {
                await ToCommand(selectedGateway: selectedGateway,
                                commands: commands,
                                replay: replay,
                                command: UtilRede.CHECKPRIMARY,
                                isSendingToGateway: isSendingToGateway,
                                isForceUDP: true);

                await ToCommand(selectedGateway: selectedGateway,
                               commands: commands,
                               replay: replay,
                               command: UtilRede.GETZWAVEFREQUENCY,
                               isSendingToGateway: isSendingToGateway,
                               isForceUDP: true);

                await ToCommand(selectedGateway: selectedGateway,
                               commands: commands,
                               replay: replay,
                               command: UtilRede.GETZWAVEHOMEID,
                               isSendingToGateway: isSendingToGateway,
                               isForceUDP: true);
            }

            #endregion Zwave

            await ToCommand(selectedGateway: selectedGateway,
                            commands: commands,
                            replay: replay,
                            command: UtilRede.GETUNIQUEID,
                            isSendingToGateway: isSendingToGateway,
                            isForceUDP: true);

            selectedGateway.GetConnectionType();

            replay.Dispose();

            _gatewayService.IsSendingToGateway = isSendingToGateway;
        }

        public async Task GetGateways(GatewayModelEnum type,
                                      IPAddress address,
                                      ReplaySubject<string> replay = null,
                                      ReplaySubject<int> rxProgressBarValue = null,
                                      int timeout = 8000)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (_gatewayService.IsSendingToGateway)
            {
                return;
            }

            replay ??= new ReplaySubject<string>();

            ;

            _gatewayService.IsSendingToGateway = true;

            await _udpRepository.GetGateways(GatewayModelEnum.ANY,
                                             replay: replay,
                                             rxProgressBarValue: rxProgressBarValue,
                                             command: "@GCR#");

            _gatewayService.IsSendingToGateway = false;

            replay.Dispose();
        }

        private static int GetModuleIdGateway(string homeId)
        {
            return homeId is null
                ? -1
                : !Regex.IsMatch(homeId, "^[1234567890ABCDEF]{10}$")
                ? -1
                : int.Parse(homeId.Substring(8, 2), System.Globalization.NumberStyles.HexNumber);
        }

        public async Task GetUID(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (json.ContainsKey(UtilGateway.UID) &&
                    json.ContainsKey(UtilGateway.BUILD)
                )
                {
                    selectedGateway.UID = json.Value<string>(UtilGateway.UID);
                    selectedGateway.Build = json.Value<int>(UtilGateway.BUILD);
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            ;

            await ToCommand(selectedGateway, commands, replay, UtilRede.GETUNIQUEID);

            replay.Dispose();
        }

        public async Task GetZwaveDevices(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            _parseService.IsSendingToCloud = true;

            IEnumerable<ParseObject> zwaveDevices = await selectedProject.SelectedGateway.ParseObject
                                                                                          .GetRelation<ParseObject>(selectedProject.SelectedGateway.IsPrimary ? AppConstants.ZWAVEDEVICES : AppConstants.SECONDARYZWAVEDEVICE)
                                                                                          .Query.OrderBy(AppConstants.MODULEIDDEVICEDATABASE).FindAsync();

            if (selectedProject.SelectedGateway.IsPrimary)
            {
                selectedProject.SelectedGateway.ZwaveDevices.Clear();

                selectedProject.SelectedGateway.ZwaveDevices.Add(
                    new ZwaveDevice
                    {
                        DefaultName = selectedProject.SelectedGateway.Name ?? selectedProject.SelectedGateway.DefaultName,
                        ModuleId = selectedProject.SelectedGateway.ModuleId,
                        ZWaveComponents = ZWaveComponents.Controller
                    });
            }
            else
            {
                selectedProject.SelectedGateway.SecondaryZwaveDevices.Clear();

                selectedProject.SelectedGateway.SecondaryZwaveDevices.Add(
                    new ZwaveDevice
                    {
                        DefaultName = Properties.Resources.Primary_Gateway,
                        ModuleId = 1,
                        ZWaveComponents = ZWaveComponents.Controller
                    });

                selectedProject.SelectedGateway.SecondaryZwaveDevices.Add(
                    new ZwaveDevice
                    {
                        DefaultName = selectedProject.SelectedGateway.Name ?? selectedProject.SelectedGateway.DefaultName,
                        ModuleId = selectedProject.SelectedGateway.ModuleId,
                        ZWaveComponents = ZWaveComponents.Controller
                    });
            }

            foreach (ParseObject parseObject in zwaveDevices)
            {
                if (parseObject.ParseObjectToZwaveDevice() is ZwaveDevice zwaveDevice)
                {
                    if (selectedProject.SelectedGateway.IsPrimary)
                    {
                        if (zwaveDevice.CustomId == ZwaveModelUtil.FCZWS100)
                        {
                            zwaveDevice.ZWaveComponents = ZWaveComponents.Controller;
                        }
                        selectedProject.SelectedGateway.ZwaveDevices.Add(zwaveDevice);
                    }
                    else
                    {
                        selectedProject.SelectedGateway.SecondaryZwaveDevices.Add(zwaveDevice);
                    }
                }
            }

            _parseService.IsSendingToCloud = false;
        }

        #endregion Get

        public async Task Insert(ProjectModel selectedProjectModel, GatewayModel device)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            if (device is null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            ;

            ParseACL parseACLDevice = new();

            parseACLDevice.SetRoleReadAccess(selectedProjectModel.RoleUsers, true);

            parseACLDevice.SetRoleWriteAccess(selectedProjectModel.RoleUsers, true);

            device.GatewayToParseObject();

            device.ParseObject.ACL = parseACLDevice;

            await device.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

            ParseRelation<ParseObject> relation = selectedProjectModel.ParseObject.GetRelation<ParseObject>(UtilGateway.GATEWAYS);

            relation.Add(device.ParseObject);

            await selectedProjectModel.ParseObject.SaveAsync();
        }

        public async Task Reboot(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

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

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            await Task.Delay(UtilRede.MILLISECONDSDELAYAWAITREBOOT);

            replay.Dispose();
        }

        public async Task Rename(GatewayModel selectedDevice)
        {
            if (selectedDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedDevice));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource();

            selectedDevice.ParseObject[AppConstants.DEVICENAME] = selectedDevice.Name;

            await selectedDevice.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);
        }

        #region Set

        private async Task SetWIFIAPPassword(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (!json.ContainsKey(UtilGateway.COMMAND))
                {
                    return;
                }

                if (!json.ContainsKey("pass"))
                {
                    return;
                }

                if (!json.ContainsKey("type"))
                {
                    return;
                }

                if (json.Value<int>("type") == 1)
                {
                    selectedGateway.APPassword = json.Value<string>("pass");
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"set_WIFI_ap_pass\", \"type\":0,\"pass\":\"{selectedGateway.APPassword}\"}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            commands: commands,
                            command: command);

            replay.Dispose();
        }

        public async Task SetAPDHCP(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (!json.ContainsKey(UtilGateway.COMMAND))
                {
                    return;
                }

                if (!json.ContainsKey("option"))
                {
                    return;
                }

                if (!json.ContainsKey("type"))
                {
                    return;
                }

                if (json.Value<int>("type") == 1)
                {
                    selectedGateway.SelectedIndexAPDHCP = json.Value<int>("option");
                    _taskService.CancellationTokenSource.Cancel();
                    return;
                }
            });

            string command = $"{{\"command\":\"set_WIFI_ap_dhcp\", \"type\":0,\"option\":{selectedGateway.SelectedIndexAPDHCP}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            replay.Dispose();
        }

        public async Task SetEthernetNetworkConfiguration(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (json.Value<string>(UtilGateway.COMMAND) == "ethNetworkConfigSet" && json.ContainsKey(UtilGateway.DHCP))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"ethNetworkConfigSet\", \"type\":0, \"dhcp\":{((selectedGateway.SelectedIndexIPTypeEthernet == 0) + "").ToLower()}, \"ipAddress\":{selectedGateway.StaticIPEthernet.IPToArray()},\"gatewayIp\":{selectedGateway.RouterGatewayEthernet.IPToArray()}, \"networkMask\":{selectedGateway.MaskEthernet.IPToArray()}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            replay.Dispose();

            if (!@return)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task SetEthernetPort(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (json.Value<string>(UtilGateway.COMMAND) == "ethPortNumberSet" && json.ContainsKey(UtilGateway.TCPPORT))
                {
                    selectedGateway.TcpPortEthernet = json.Value<int>(UtilGateway.TCPPORT);
                    selectedGateway.UdpPortEthernet = json.Value<int>(UtilGateway.UDPPORT);
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"ethPortNumberSet\",\"type\":0,\"tcpPort\":{selectedGateway.TcpPortEthernet},\"udpPort\":{selectedGateway.UdpPortEthernet}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            replay.Dispose();

            if (!@return)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task SetSSIDPasswordAP(GatewayModel selectedGateway)
        {
            await SetWIFIAPSSID(selectedGateway);
            await SetWIFIAPPassword(selectedGateway);
            _gatewayService.IsSendingToGateway = false;
        }

        public async Task<bool> SetWIFIAP(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (!json.ContainsKey(UtilGateway.COMMAND))
                {
                    return;
                }

                if (!json.Value<string>(UtilGateway.COMMAND).Contains("set_WIFI_ap_enable"))
                {
                    return;
                }

                if (!json.ContainsKey("option"))
                {
                    return;
                }

                if (!json.ContainsKey("type"))
                {
                    return;
                }

                selectedGateway.SelectedIndexAPStatus = json.Value<int>("option");
                _taskService.CancellationTokenSource.Cancel();
                @return = true;
                return;
            });

            string command = $"{{\"command\":\"set_WIFI_ap_enable\", \"type\":0,\"option\": {selectedGateway.SelectedIndexAPStatus}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            replay.Dispose();

            return @return;
        }

        public async Task SetWIFIAPSSID(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (!json.ContainsKey(UtilGateway.COMMAND))
                {
                    return;
                }

                if (!json.ContainsKey(UtilGateway.SSID))
                {
                    return;
                }

                if (!json.ContainsKey("type"))
                {
                    return;
                }

                if (json.Value<int>("type") == 1)
                {
                    selectedGateway.APSSID = json.Value<string>(UtilGateway.SSID);
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"set_WIFI_ap_ssid\", \"type\":0,\"ssid\":\"{selectedGateway.APSSID}\"}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            replay.Dispose();

            if (!@return)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task SetWifiNetworkConfiguration(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (json.Value<string>(UtilGateway.COMMAND) == "WifiNetworkConfigurationSet" && json.ContainsKey(UtilGateway.DHCP))
                {
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"WifiNetworkConfigurationSet\",\"type\":0,\"dhcp\":{((selectedGateway.SelectedIndexIPTypeWiFi == 0) + "").ToLower()},\"localIp\":{selectedGateway.StaticIPWiFi.IPToArray()}, \"gatewayIp\":{selectedGateway.RouterGatewayWiFi.IPToArray()}, \"networkMask\":{selectedGateway.MaskWiFi.IPToArray()},\"tcpPort\":{selectedGateway.TcpPortWiFi}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            replay.Dispose();

            if (!@return)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task SetWIFIPass(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (json.Value<string>(UtilGateway.COMMAND) == "set_WIFI_station_pass" && json.ContainsKey("pass"))
                {
                    selectedGateway.Password = json.Value<string>("pass");
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"set_WIFI_station_pass\", \"type\":0,\"pass\":\"{selectedGateway.Password}\"}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            replay.Dispose();

            if (!@return)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task SetWifiPort(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (json.Value<string>(UtilGateway.COMMAND) == "WifiPortNumberSet" && json.ContainsKey(UtilGateway.TCPPORT))
                {
                    selectedGateway.TcpPortWiFi = json.Value<int>(UtilGateway.TCPPORT);
                    selectedGateway.UdpPortWifi = json.Value<int>(UtilGateway.UDPPORT);
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"WifiPortNumberSet\",\"type\":0,\"tcpPort\":{selectedGateway.TcpPortWiFi},\"udpPort\":{selectedGateway.UdpPortWifi}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            if (!@return)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task SetWIFISSID(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (json.Value<string>(UtilGateway.COMMAND) == "set_WIFI_station_ssid" && json.ContainsKey(UtilGateway.SSID))
                {
                    selectedGateway.SSID = json.Value<string>(UtilGateway.SSID);
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"set_WIFI_station_ssid\", \"type\":0,\"ssid\":\"{selectedGateway.SSID}\"}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            if (!@return)
            {
                ThrowException(selectedGateway);
            }
        }

        public async Task SetWifiStation(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            bool @return = false;

            _ = replay.Subscribe(resp =>
            {
                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                if (resp.TryParseJObject() is not JObject json)
                {
                    return;
                }

                if (json.Value<string>(UtilGateway.COMMAND) == "set_WIFI_station_enable" && json.ContainsKey("option"))
                {
                    selectedGateway.SelectedIndexWiFiStatus = json.Value<int>("option");
                    _taskService.CancellationTokenSource.Cancel();
                    @return = true;
                    return;
                }
            });

            string command = $"{{\"command\":\"set_WIFI_station_enable\", \"type\":0,\"option\":{selectedGateway.SelectedIndexWiFiStatus}}}";

            await ToCommand(selectedGateway: selectedGateway,
                            replay: replay,
                            command: command,
                            commands: commands);

            if (!@return)
            {
                ThrowException(selectedGateway);
            }
        }

        #endregion Set

        public async Task Test(GatewayModel gateway)
        {
            if (gateway is null)
            {
                throw new ArgumentNullException(nameof(gateway));
            }

            switch ((GatewayFunctions)gateway.SelectedTabIndexIfThen)
            {
                case GatewayFunctions.IR:
                    if (!gateway.IRsGateway.Any())
                    {
                        return;
                    }
                    if (gateway.SelectedIndexIR < 0)
                    {
                        return;
                    }
                    await _irRepository.PlayMemory(gateway, gateway.IRsGateway.ElementAt(gateway.SelectedIndexIR).MemoryId);
                    break;

                case GatewayFunctions.Radio433:
                    if (!gateway.Radios433Gateway.Any())
                    {
                        return;
                    }
                    if (gateway.SelectedIndexRadio433 < 0)
                    {
                        return;
                    }
                    await _rFRepository.PlayMemory(gateway, gateway.Radios433Gateway.ElementAt(gateway.SelectedIndexRadio433).MemoryId);
                    break;

                case GatewayFunctions.RTS:
                    if (!gateway.RadiosRTSGateway.Any())
                    {
                        return;
                    }
                    if (gateway.SelectedIndexRTS < 0)
                    {
                        return;
                    }
                    await _rFRepository.PlayRts(gateway, gateway.RadiosRTSGateway.ElementAt(gateway.SelectedIndexRTS));
                    break;

                case GatewayFunctions.Serial:
                    if (!gateway.Serials.Any())
                    {
                        return;
                    }
                    if (gateway.SelectedIndexSerial < 0)
                    {
                        return;
                    }
                    await _serialRepository.PlayByMemoryId(gateway, gateway.Serials.ElementAt(gateway.SelectedIndexSerial).MemoryId);
                    break;

                case GatewayFunctions.IPCommand:
                    if (!gateway.IpCommands.Any())
                    {
                        return;
                    }
                    if (gateway.SelectedIndexIPCommand < 0)
                    {
                        return;
                    }
                    await _ipCommandRepository.PlayMemory(gateway, gateway.IpCommands.ElementAt(gateway.SelectedIndexIPCommand).MemoryId);
                    break;

                case GatewayFunctions.Relay:
                    await _relayRepository.SetRelayState(gateway);
                    break;

                default:
                    break;
            }
        }

        #region Update

        public async Task Update(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            _parseService.IsSendingToCloud = true;

            selectedProjectModel.SelectedGateway.GatewayToParseObject();

            await selectedProjectModel.SelectedGateway.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

            _parseService.IsSendingToCloud = false;
        }

        public async Task UpdateBlacklistUsers(GatewayModel selectedDevice)
        {
            if (selectedDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedDevice));
            }

            selectedDevice.ParseObject[AppConstants.DEVICEBLACKLISTUSERS] = selectedDevice.BlacklistUsers.Select(x => x.ParseUser.ObjectId).ToList();

            selectedDevice.ParseObject[AppConstants.DEVICEISENABLEBLACKLIST] = selectedDevice.IsEnableBlackList;

            _taskService.CancellationTokenSource = new CancellationTokenSource();

            await selectedDevice.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);
        }

        public async Task UpdateFirmawe(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            using ReplaySubject<string> replay = new();

            commands.Clear();

            string response = string.Empty;

            replay.Subscribe(resp =>
            {
                response = resp;

                if (string.IsNullOrEmpty(resp))
                {
                    return;
                }

                _taskService.CancellationTokenSource.Cancel();
                return;
            });

            string model = string.Empty;

            GetModel(selectedGateway, ref model);

            string version = string.Empty;

#if DEBUG
            version = "latest_debug";
#else
            version = "latest_release";
#endif

            string command = $"{{\"command\":\"UpdateEsp32\",\"address\":\"18.219.240.204\",\"port\":\"80\",\"filename\":\"/{model}/{version}.bin\",\"type\":0}}";

            ;

            await ToCommand(selectedGateway, commands, replay, command);

            replay.Dispose();
        }

        public async Task Update(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            _parseService.IsSendingToCloud = true;

            selectedGateway.GatewayToParseObject();

            await selectedGateway.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

            _parseService.IsSendingToCloud = false;
        }

        #endregion Update
    }
}