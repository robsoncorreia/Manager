using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FC.Domain.Service
{
    public interface INetworkService
    {
        Task<bool> IsInternet();

        IPAddress GetDefaultGateway();
    }

    public class NetworkService : INetworkService
    {
        private readonly ITaskService _taskService;

        public NetworkService(ITaskService taskService)
        {
            _taskService = taskService;
        }

        private readonly Regex rxIp = regex;
        private static readonly Regex regex = new(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-4])$");

        public IPAddress GetDefaultGateway()
        {
            IEnumerable<IPAddress> addresses = NetworkInterface.GetAllNetworkInterfaces()
                                            .Where(n => n.OperationalStatus == OperationalStatus.Up)
                                            .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                            .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
                                            .Select(g => g?.Address)
                                            .Where(a => a != null);

            foreach (IPAddress address in addresses)
            {
                Match match = rxIp.Match(address.ToString());

                if (match.Success)
                {
                    return address;
                }
            }

            return IPAddress.Broadcast;
        }

        public async Task<bool> IsInternet()
        {
            return await Task.Run<bool>(() =>
            {
                _taskService.CancellationTokenSource = new System.Threading.CancellationTokenSource();

                using WebClient client = new();

                _ = _taskService.CancellationTokenSource.Token.Register(() =>
                {
                    client.CancelAsync();
                });

                try
                {
                    _ = client.OpenRead("http://google.com/generate_204");

                    return true;
                }
                catch
                {
                    return false;
                }
                finally
                {
                    client.Dispose();
                }
            });
        }
    }
}