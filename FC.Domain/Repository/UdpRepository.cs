using CommunityToolkit.Mvvm.Messaging;
using FC.Domain.Model;
using FC.Domain.Model.Device;
using FC.Domain.Repository.Util;
using FC.Domain.Service;
using FC.Domain.Service.Rede;
using FC.Domain.Util;
using Parse;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FC.Domain.Repository
{
    public interface IUDPRepository
    {
        Task<bool> FindGateway(GatewayModelEnum gatewayType,
                                       GatewayModel gateway,
                                       string command = UtilRede.GETUNIQUEID,
                                       int port = 9999,
                                       ReplaySubject<string> replay = null,
                                       int timeout = 10000,
                                       bool isSendingToGateway = false);

        Task GetGateways(GatewayModelEnum gatewayType,
                         string command = "{\"command\":\"eth_network_status\",\"type\":0}",
                         int port = 9999,
                         int timeout = 30000,
                         ReplaySubject<string> replay = null,
                         ReplaySubject<int> rxProgressBarValue = null);

        Task<bool> SendCommandRX(string ip,
                                 int port,
                                 string command,
                                 ReplaySubject<string> replay,
                                 ConnectionType connectionType,
                                 int timeout,
                                 bool isMultipleResponses = false);
    }

    public class UDPRepository : IUDPRepository
    {
        private const int MILLISECONDSDELAY = 50;

        private static readonly Regex regexIP = new(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-4])$");

        private readonly ILocalDBRepository _localDBRepository;

        private readonly IGatewayService _gatewayService;

        private readonly ITaskService _taskService;
        private bool isCanceled;

        public UDPRepository(ICommandRepository commandRepository,
                             IGatewayService gatewayService,
                             ILocalDBRepository localDBRepository,
                             ITaskService taskService)
        {
            _taskService = taskService;

            _gatewayService = gatewayService;

            _localDBRepository = localDBRepository;

            _ = _taskService.CanceledReplay.Subscribe(response => { isCanceled = response; });
        }

        private readonly List<int> addresses = new();

        public async Task<bool> FindGateway(GatewayModelEnum gatewayType,
                                            GatewayModel gateway,
                                            string command = UtilRede.GETUNIQUEID,
                                            int port = 9999,
                                            ReplaySubject<string> replay = null,
                                            int timeout = 10000,
                                            bool isSendingToGateway = false)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException("message", nameof(command));
            }

            bool find = false;

            timeout += Properties.Settings.Default.delayTimeBetweenCommand;

            _taskService.CancellationTokenSource = new(timeout);

            _taskService.SetTimeout(timeout);

            using UdpClient udpClient = new(port);

            commands.Clear();

            await Task.Run(async () =>
            {
                try
                {
                    if (Properties.Settings.Default.isFindGatewayBroadcast)
                    {

                        byte[] sendBytes = Encoding.ASCII.GetBytes(command);

                        byte[] addressDefaultGateway = GetDefaultGateway().GetAddressBytes();

                        addressDefaultGateway[3] = 255;

                        IPAddress addressSend = new(addressDefaultGateway);

                        IPEndPoint ipEndPoint = new(addressSend, port);

                        _ = await udpClient.SendAsync(sendBytes, sendBytes.Length, ipEndPoint);

                        while (true)
                        {
                            await Task.Delay(MILLISECONDSDELAY);

                            if (_taskService.CancellationTokenSource.Token.IsCancellationRequested)
                            {
                                break;
                            }

                            using CommandModel comma = new(command, ipEndPoint.ToString(), port, ProtocolTypeEnum.UDP, ParseUser.CurrentUser?.ObjectId);

                            comma.Timeout = timeout;

                            comma.IPTarget = ipEndPoint.ToString();

                            comma.IP = ipEndPoint.ToString();

                            //Add(comma);

                            commands.Add(comma);

                            if (udpClient.Available == 0)
                            {
                                continue;
                            }

                            UdpReceiveResult receiveResult = await udpClient.ReceiveAsync();

                            string ipReceive = receiveResult.RemoteEndPoint.ToString().Split(':')[0];

                            int portReceive = int.Parse(receiveResult.RemoteEndPoint.ToString().Split(':')[1]);

                            string response = Encoding.ASCII.GetString(receiveResult.Buffer, 0, receiveResult.Buffer.Length);

                            comma.IPTarget = ipReceive;

                            comma.IP = ipReceive;

                            comma.Receive = response;

                            comma.GetResponseTime();

                            DebugWriteLine(comma.Send, comma.Receive);

                            if (response == command)
                            {
                                continue;
                            }

                            if (response.TryParseJObject() is not Newtonsoft.Json.Linq.JObject json)
                            {
                                return;
                            }

                            if (!json.ContainsKey("command") || !json.ContainsKey("type"))
                            {
                                return;
                            }

                            if (json.Value<string>("command") != "get_unique_id")
                            {
                                return;
                            }
                            if (json.Value<int>("type") != 1)
                            {
                                return;
                            }

                            if (!json.ContainsKey(UtilGateway.UID))
                            {
                                return;
                            }

                            if (gateway.UID == json.Value<string>(UtilGateway.UID))
                            {
                                udpClient.Close();
                                udpClient.Dispose();
                                gateway.LocalPortUDP = portReceive;
                                gateway.LocalIP = ipReceive;
                                find = true;
                                replay?.OnNext($"{Properties.Resources.I_found_this_gateway_on_the_IP}\n{ipReceive}");

                                break;
                            }
                        }
                    }
                    else
                    {
                        IPAddress addressDefaultGateway = GetDefaultGateway();

                        List<CommonIP> CommonIPs = _localDBRepository.GetAllMostRepeatedIP();

                        List<CommonIP> ips = CommonIPs.Where(x => x.Gateway == addressDefaultGateway.ToString()).ToList();

                        var group = ips.GroupBy(n => n.Send)
                                                             .Select(n => new
                                                             {
                                                                 IP = n.Key,
                                                                 Count = n.Count()
                                                             })
                                                             .OrderByDescending(n => n.Count);

                        addresses.Clear();

                        foreach (var ip in group)
                        {
                            if (IPAddress.TryParse(ip.IP, out IPAddress address))
                            {
                                addresses.Add(address.GetAddressBytes()[3]);
                            }
                        }

                        byte[] sendBytes = Encoding.ASCII.GetBytes(command);

                        _gatewayService.IsSendingToGateway = true;

                        byte[] addressBytes = addressDefaultGateway.GetAddressBytes();

                        int[] numbers = Enumerable.Range(1, 254).ToArray();

                        List<int> shuffled = numbers.OrderBy(n => Guid.NewGuid()).ToList();

                        IPAddress ipAddress = IPAddress.Parse(gateway.LocalIP);

                        _ = shuffled.Remove(ipAddress.GetAddressBytes()[3]);

                        _ = addresses.Remove(ipAddress.GetAddressBytes()[3]);

                        foreach (int address in addresses)
                        {
                            _ = shuffled.Remove(address);
                            shuffled.Insert(0, address);
                        }

                        shuffled.Insert(0, ipAddress.GetAddressBytes()[3]);

                        for (int i = 0; i < shuffled.Count; i++)
                        {
                            if (_taskService.CancellationTokenSource.Token.IsCancellationRequested)
                            {
                                break;
                            }

                            addressBytes[3] = (byte)shuffled[i];

                            addressDefaultGateway = new(addressBytes);

                            IPEndPoint ipEndPoint = new(addressDefaultGateway, port);

                            _ = await udpClient.SendAsync(sendBytes, sendBytes.Length, ipEndPoint);

                            using CommandModel comma = new(command, addressDefaultGateway.ToString(), port, ProtocolTypeEnum.UDP, ParseUser.CurrentUser?.ObjectId);

                            comma.Timeout = timeout;

                            comma.IPTarget = addressDefaultGateway.ToString();

                            comma.IP = addressDefaultGateway.ToString();

                            //Add(comma);

                            commands.Add(comma);

                            replay?.OnNext(string.Format(Properties.Resources.Looking_for_gateway_on_IP, ipEndPoint.Address));

                            await Task.Delay(MILLISECONDSDELAY);

                            if (udpClient.Available == 0)
                            {
                                continue;
                            }

                            UdpReceiveResult receiveResult = await udpClient.ReceiveAsync();

                            string ipReceive = receiveResult.RemoteEndPoint.ToString().Split(':')[0];

                            int portReceive = int.Parse(receiveResult.RemoteEndPoint.ToString().Split(':')[1]);

                            string response = Encoding.ASCII.GetString(receiveResult.Buffer, 0, receiveResult.Buffer.Length);

                            comma.IPTarget = ipReceive;

                            comma.IP = ipReceive;

                            comma.Receive = response;

                            comma.GetResponseTime();

                            DebugWriteLine(comma.Send, comma.Receive);

                            if (response == command)
                            {
                                continue;
                            }

                            if (response.TryParseJObject() is not Newtonsoft.Json.Linq.JObject json)
                            {
                                return;
                            }

                            if (!json.ContainsKey("command") || !json.ContainsKey("type"))
                            {
                                return;
                            }

                            if (json.Value<string>("command") != "get_unique_id")
                            {
                                return;
                            }
                            if (json.Value<int>("type") != 1)
                            {
                                return;
                            }

                            if (!json.ContainsKey(UtilGateway.UID))
                            {
                                return;
                            }

                            if (gateway.UID == json.Value<string>(UtilGateway.UID))
                            {
                                udpClient.Close();
                                udpClient.Dispose();
                                gateway.LocalPortUDP = portReceive;
                                gateway.LocalIP = ipReceive;
                                find = true;
                                replay?.OnNext($"{Properties.Resources.I_found_this_gateway_on_the_IP}\n{ipReceive}");

                                break;
                            }
                        }
                    }
                }
                finally
                {
                    _gatewayService.IsSendingToGateway = isSendingToGateway;

                    udpClient.Close();

                    udpClient.Dispose();

                    for (int i = 0; i < commands.Count; i++)
                    {
                        Add(commands.ElementAt(i));
                    }

                    CancellationTokenSource cts = new(Properties.Settings.Default.delayTimeBetweenCommand);

                    await Task.Run(() =>
                    {
                        while (!cts.IsCancellationRequested && !isCanceled)
                        {
                        }
                    });

                    cts.Dispose();
                }
            });

            return find;
        }

        private static void Add(CommandModel command)
        {
            _ = WeakReferenceMessenger.Default.Send(command);
        }

        public static IPAddress GetDefaultGateway()
        {
            IEnumerable<IPAddress> addresses = NetworkInterface.GetAllNetworkInterfaces()
                                            .Where(n => n.OperationalStatus == OperationalStatus.Up)
                                            .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                            .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                                            .Select(g => g?.Address)
                                            .Where(a => a != null);

            foreach (IPAddress address in addresses)
            {
                Match match = regexIP.Match(address.ToString());

                if (match.Success)
                {
                    byte[] ip = address.GetAddressBytes();

                    ip[3] = 255;

                    return new IPAddress(ip);
                }
            }

            return IPAddress.Broadcast;
        }

        private readonly IList<CommandModel> commands = new List<CommandModel>();

        public async Task GetGateways(GatewayModelEnum gatewayType,
                                      string command = "{\"command\":\"pedroGet\",\"type\":0}",
                                      int port = 9999,
                                      int timeout = 30000,
                                      ReplaySubject<string> replay = null,
                                      ReplaySubject<int> rxProgressBarValue = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException("message", nameof(command));
            }

            commands.Clear();

            timeout += Properties.Settings.Default.delayTimeBetweenCommand;

            _taskService.CancellationTokenSource = new(timeout);

            _taskService.SetTimeout(timeout);

            using UdpClient udpClient = new(port);

            _gatewayService.Gateways.Clear();

            await Task.Run(async () =>
            {
                try
                {
                    if (Properties.Settings.Default.isFindGatewayBroadcast)
                    {

                        byte[] sendBytes = Encoding.ASCII.GetBytes(command);

                        byte[] addressDefaultGateway = GetDefaultGateway().GetAddressBytes();

                        addressDefaultGateway[3] = 255;

                        IPAddress addressSend = new(addressDefaultGateway);

                        IPEndPoint ipEndPoint = new(addressSend, port);


                        while (true)
                        {

                            await Task.Delay(MILLISECONDSDELAY);

                            _ = await udpClient.SendAsync(sendBytes, sendBytes.Length, ipEndPoint);

                            if (_taskService.CancellationTokenSource.Token.IsCancellationRequested)
                            {
                                break;
                            }

                            using CommandModel comma = new(command, addressSend.ToString(), port, ProtocolTypeEnum.UDP, ParseUser.CurrentUser?.ObjectId);

                            //Add(comma);

                            commands.Add(comma);

                            comma.Timeout = timeout;

                            comma.IPTarget = addressSend.ToString();

                            comma.IP = addressSend.ToString();

                            comma.Port = port;

                            if (udpClient.Available == 0)
                            {
                                continue;
                            }

                            UdpReceiveResult receiveResult = await udpClient.ReceiveAsync();

                            string ipReceive = receiveResult.RemoteEndPoint.ToString().Split(':')[0];

                            int portReceive = int.Parse(receiveResult.RemoteEndPoint.ToString().Split(':')[1]);

                            string response = Encoding.ASCII.GetString(receiveResult.Buffer, 0, receiveResult.Buffer.Length);

                            comma.IP = ipReceive;

                            comma.IPTarget = ipReceive;

                            comma.Receive = response;

                            comma.GetResponseTime();

                            DebugWriteLine(comma.Send, comma.Receive);

                            if (response == command)
                            {
                                continue;
                            }

                            if (command.Contains("eth_network_status"))
                            {
                                if (!response.Contains("eth_network_status"))
                                {
                                    continue;
                                }

                                if (response.StringEthernetNetworkStatusToGateway(ipReceive, portReceive) is not GatewayModel gateway)
                                {
                                    continue;
                                }

                                if (_gatewayService.Gateways.FirstOrDefault(x => x.LocalIP == gateway.LocalIP) is not null)
                                {
                                    continue;
                                }

                                if (gatewayType == GatewayModelEnum.ANY)
                                {
                                    _ = Task.Run(() => { System.Windows.Application.Current.Dispatcher.Invoke(delegate { _gatewayService.Gateways.Add(gateway); }); });
                                }
                                else if (gatewayType == gateway.GatewayModelEnum)
                                {
                                    _ = Task.Run(() => { System.Windows.Application.Current.Dispatcher.Invoke(delegate { _gatewayService.Gateways.Add(gateway); }); });
                                }
                            }
                            else if (command.Contains("@GCR"))
                            {
                                if (!response.Contains("GCR"))
                                {
                                    continue;
                                }

                                if (response.GCRToGateway(ipReceive, portReceive) is not GatewayModel gateway)
                                {
                                    continue;
                                }

                                if (_gatewayService.Gateways.FirstOrDefault(x => x.LocalIP == gateway.LocalIP) is not null)
                                {
                                    continue;
                                }

                                if (gatewayType == GatewayModelEnum.ANY)
                                {
                                    _ = Task.Run(() =>
                                    {
                                        Application.Current.Dispatcher.Invoke(delegate
                                        {
                                            _gatewayService.Gateways.Add(gateway);
                                        });
                                    });
                                }
                                else if (gatewayType == gateway.GatewayModelEnum)
                                {
                                    _ = Task.Run(() =>
                                    {
                                        Application.Current.Dispatcher.Invoke(delegate
                                        {
                                            _gatewayService.Gateways.Add(gateway);
                                        });
                                    });
                                }
                            }
                        }

                    }
                    else
                    {


                        IPAddress addressDefaultGateway = GetDefaultGateway();

                        List<CommonIP> CommonIPs = _localDBRepository.GetAllMostRepeatedIP();

                        List<CommonIP> ips = CommonIPs.Where(x => x.Gateway == addressDefaultGateway.ToString()).ToList();

                        var group = ips.GroupBy(n => n.Send)
                                                             .Select(n => new
                                                             {
                                                                 IP = n.Key,
                                                                 Count = n.Count()
                                                             })
                                                             .OrderByDescending(n => n.Count);

                        List<int> addresses = new();

                        foreach (var ip in group)
                        {
                            if (IPAddress.TryParse(ip.IP, out IPAddress address))
                            {
                                addresses.Add(address.GetAddressBytes()[3]);
                            }
                        }

                        byte[] sendBytes = Encoding.ASCII.GetBytes(command);

                        byte[] addressBytes = addressDefaultGateway.GetAddressBytes();

                        int newEnd = 100;
                        int newStart = 0;
                        int originalEnd = 232;
                        int originalStart = 1;
                        double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);

                        int[] numbers = Enumerable.Range(1, 254).ToArray();

                        List<int> shuffled = numbers.OrderBy(n => Guid.NewGuid()).ToList();

                        foreach (int address in addresses)
                        {
                            _ = shuffled.Remove(address);
                            shuffled.Insert(0, address);
                        }

                        for (int i = 0; i < shuffled.Count; i++)
                        {
                            if (rxProgressBarValue is not null)
                            {
                                rxProgressBarValue.OnNext((int)(newStart + ((i - originalStart) * scale)));
                            }

                            if (_taskService.CancellationTokenSource.Token.IsCancellationRequested)
                            {
                                break;
                            }

                            addressBytes[3] = (byte)shuffled[i];

                            IPAddress addressSend = new(addressBytes);

                            using CommandModel comma = new(command, addressSend.ToString(), port, ProtocolTypeEnum.UDP, ParseUser.CurrentUser?.ObjectId);

                            //Add(comma);

                            commands.Add(comma);

                            comma.Timeout = timeout;

                            comma.IPTarget = addressSend.ToString();

                            comma.IP = addressSend.ToString();

                            comma.Port = port;

                            IPEndPoint ipEndPoint = new(addressSend, port);

                            _ = await udpClient.SendAsync(sendBytes, sendBytes.Length, ipEndPoint);

                            replay?.OnNext(string.Format(Properties.Resources.Looking_for_gateway_on_IP, ipEndPoint.Address));

                            await Task.Delay(MILLISECONDSDELAY);

                            if (udpClient.Available == 0)
                            {
                                continue;
                            }

                            UdpReceiveResult receiveResult = await udpClient.ReceiveAsync();

                            await Task.Delay(MILLISECONDSDELAY);

                            string ipReceive = receiveResult.RemoteEndPoint.ToString().Split(':')[0];

                            int portReceive = int.Parse(receiveResult.RemoteEndPoint.ToString().Split(':')[1]);

                            string response = Encoding.ASCII.GetString(receiveResult.Buffer, 0, receiveResult.Buffer.Length);

                            comma.IP = ipReceive;

                            comma.IPTarget = ipReceive;

                            comma.Receive = response;

                            comma.GetResponseTime();

                            DebugWriteLine(comma.Send, comma.Receive);

                            if (response == command)
                            {
                                continue;
                            }

                            if (command.Contains("eth_network_status"))
                            {
                                if (!response.Contains("eth_network_status"))
                                {
                                    continue;
                                }

                                if (response.StringEthernetNetworkStatusToGateway(ipReceive, portReceive) is not GatewayModel gateway)
                                {
                                    continue;
                                }

                                if (_gatewayService.Gateways.FirstOrDefault(x => x.LocalIP == gateway.LocalIP) is not null)
                                {
                                    continue;
                                }

                                if (gatewayType == GatewayModelEnum.ANY)
                                {
                                    _ = Task.Run(() => { System.Windows.Application.Current.Dispatcher.Invoke(delegate { _gatewayService.Gateways.Add(gateway); }); });
                                }
                                else if (gatewayType == gateway.GatewayModelEnum)
                                {
                                    _ = Task.Run(() => { System.Windows.Application.Current.Dispatcher.Invoke(delegate { _gatewayService.Gateways.Add(gateway); }); });
                                }
                            }
                            else if (command.Contains("@GCR"))
                            {
                                if (!response.Contains("GCR"))
                                {
                                    continue;
                                }

                                if (response.GCRToGateway(ipReceive, portReceive) is not GatewayModel gateway)
                                {
                                    continue;
                                }

                                if (_gatewayService.Gateways.FirstOrDefault(x => x.LocalIP == gateway.LocalIP) is not null)
                                {
                                    continue;
                                }

                                if (gatewayType == GatewayModelEnum.ANY)
                                {
                                    _ = Task.Run(() =>
                                    {
                                        Application.Current.Dispatcher.Invoke(delegate
                                        {
                                            _gatewayService.Gateways.Add(gateway);
                                            _localDBRepository.SaveMostRepeatedIP(new CommonIP
                                            {
                                                Gateway = addressDefaultGateway.ToString(),
                                                Send = gateway.LocalIP
                                            });
                                        });
                                    });
                                }
                                else if (gatewayType == gateway.GatewayModelEnum)
                                {
                                    _ = Task.Run(() => { System.Windows.Application.Current.Dispatcher.Invoke(delegate { _gatewayService.Gateways.Add(gateway); }); });
                                }
                            }
                        }
                    }
                }
                finally
                {
                    udpClient.Close();

                    CancellationTokenSource cts = new(Properties.Settings.Default.delayTimeBetweenCommand);

                    for (int i = 0; i < commands.Count; i++)
                    {
                        Add(commands.ElementAt(i));
                    }

                    await Task.Run(() =>
                    {
                        while (!cts.IsCancellationRequested && !isCanceled)
                        {
                        }
                    });

                    cts.Dispose();
                }
            }, _taskService.CancellationTokenSource.Token);
        }

        public async Task<bool> SendCommandRX(string ip,
                                              int port,
                                              string command,
                                              ReplaySubject<string> replay,
                                              ConnectionType connectionType,
                                              int timeout,
                                              bool isMultipleResponses = false)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new ArgumentException("message", nameof(ip));
            }

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException("message", nameof(command));
            }

            if (replay is null)
            {
                throw new ArgumentNullException(nameof(replay));
            }

            using UdpClient udpClient = new(port);

            _gatewayService.IsSendingToGateway = true;

            timeout += Properties.Settings.Default.delayTimeBetweenCommand;

            _taskService.CancellationTokenSource = new(timeout);

            _taskService.SetTimeout(timeout);

            commands.Clear();

            if (isCanceled)
            {
                return false;
            }

            _ = _taskService.CancellationTokenSource.Token.Register(() =>
            {
                udpClient.Close();
            });

            string response = null;

            string previousAnswer = null;

            byte[] sendBytes = Encoding.ASCII.GetBytes(command);

            IPEndPoint ipEndPoint = new(IPAddress.Parse(ip), port);

            using CommandModel com = new(send: command,
                                         ip: ip,
                                         port: port,
                                         connectionType: connectionType,
                                         protocolTypeEnum: ProtocolTypeEnum.UDP,
                                         objectId: ParseUser.CurrentUser?.ObjectId);

            com.Timeout = timeout;

            await Task.Run(async () =>
            {
                try
                {
                    _ = udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);

                    do
                    {
                        if (_taskService.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            break;
                        }

                        if (udpClient.Available == 0)
                        {
                            continue;
                        }

                        await Task.Delay(MILLISECONDSDELAY);

                        UdpReceiveResult received = await udpClient.ReceiveAsync();

                        response = Encoding.ASCII.GetString(received.Buffer);

                        if (response.Equals(previousAnswer))
                        {
                            continue;
                        }

                        previousAnswer = response;

                        com.GetResponseTime();

                        com.Receive = response;

                        com.IPTarget = ip;

                        DebugWriteLine(com.Send, com.Receive);

                        //Add(com);

                        commands.Add(com);

                        replay.OnNext(response);

                        if (!isMultipleResponses)
                        {
                            break;
                        }
                    } while (true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    udpClient.Close();

                    udpClient.Dispose();

                    if (com.Receive.Equals(Properties.Resources.No_reply))
                    {
                        com.GetResponseTime();

                        DebugWriteLine(com.Send, com.Receive);

                        Add(com);
                    }

                    for (int i = 0; i < commands.Count; i++)
                    {
                        Add(commands.ElementAt(i));
                    }

                    CancellationTokenSource cts = new(Properties.Settings.Default.delayTimeBetweenCommand);

                    await Task.Run(() =>
                    {
                        while (!cts.IsCancellationRequested && !isCanceled)
                        {
                        }
                    });

                    cts.Dispose();
                }
            });
            return !string.IsNullOrEmpty(response);
        }

        private static void DebugWriteLine(string command, string response)
        {
            command = command.Replace('\n', '\0');

            response = response.Replace('\n', '\0');

            int length = command.Length > response.Length ? command.Length : response.Length;

            Debug.WriteLine($"\n{string.Concat(Enumerable.Repeat("x", length + 5))}");

            Debug.WriteLine($"UDP?:{command}");

            Debug.WriteLine($"UDP?:{response}");

            Debug.WriteLine($"\n{string.Concat(Enumerable.Repeat("x", length + 5))}\n");
        }
    }
}