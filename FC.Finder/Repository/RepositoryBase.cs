using ConfigurationFlexCloudHubBlaster.Model;
using ConfigurationFlexCloudHubBlaster.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigurationFlexCloudHubBlaster.Repository
{
    public interface IRepositoryBase
    {
        Task SendAwaitMultipleResponses(GatewayModel selectedGateway, ReplaySubject<string> replay, string command, int timeout = 5000);

        Task<bool> SendCommandRX(GatewayModel selectedGateway, ReplaySubject<string> replay, string command, int timeout = 5000);

        Task ToCommand(GatewayModel selectedGateway, IList<object> commands, ReplaySubject<string> replay, string command);

        Task TryCommands(GatewayModel selectedGateway, IList<object> commands, ReplaySubject<string> replay);
    }

    public class RepositoryBase : IRepositoryBase
    {
        internal ITaskService _taskService;
        internal IGatewayService _gatewayService;
        internal ITCPRepository _tcpRepository;

        public RepositoryBase(ITaskService taskService, IGatewayService gatewayService, ITCPRepository tcpRespository)
        {
            _taskService = taskService;

            _gatewayService = gatewayService;

            _tcpRepository = tcpRespository;
        }

        public static void ThrowException(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            throw new Exception(string.Format(CultureInfo.InvariantCulture, Properties.Resources.Device_did_not_respond, selectedGateway.Name, selectedGateway.LocalIP, selectedGateway.LocalPortTCP));
        }

        public async Task<bool> SendCommandRX(GatewayModel selectedGateway, ReplaySubject<string> replay, string command, int timeout = 5000)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (replay is null)
            {
                throw new ArgumentNullException(nameof(replay));
            }

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException($"'{nameof(command)}' não pode ser nulo ou vazio", nameof(command));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource(timeout);

            _gatewayService.IsSendingToGateway = true;

            _gatewayService.LastCommandSend = command;

            //await _udpRepository.SendCommandRX(ip: selectedGateway.LocalIP,
            //                                   port: selectedGateway.LocalPortUDP,
            //                                   command: command,
            //                                   isWait: false,
            //                                   replay,
            //                                   cts: _taskService.CancellationTokenSource).ConfigureAwait(true);

            return await _tcpRepository.SendCommandRX(ip: selectedGateway.LocalIP,
                                                 port: selectedGateway.LocalPortTCP,
                                                 command: command,
                                                 isWait: false,
                                                 replay: replay,
                                                 timeout: timeout,
                                                 selectedGateway.ConnectionType).ConfigureAwait(true);
        }

        public async Task SendAwaitMultipleResponses(GatewayModel selectedGateway, ReplaySubject<string> replay, string command, int timeout = 5000)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (replay is null)
            {
                throw new ArgumentNullException(nameof(replay));
            }

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException($"'{nameof(command)}' não pode ser nulo ou vazio", nameof(command));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource(timeout);

            _gatewayService.IsSendingToGateway = true;

            _gatewayService.LastCommandSend = command;

            //await _udpRepository.SendCommandRX(ip: selectedGateway.LocalIP,
            //                                   port: selectedGateway.LocalPortUDP,
            //                                   command: command,
            //                                   isWait: false,
            //                                   replay,
            //                                   cts: _taskService.CancellationTokenSource).ConfigureAwait(true);

            await _tcpRepository.SendAwaitMultipleResponses(ip: selectedGateway.LocalIP,
                                               port: selectedGateway.LocalPortTCP,
                                               command: command,
                                               isWait: false,
                                               replay: replay,
                                               timeout: timeout,
                                               selectedGateway.ConnectionType).ConfigureAwait(true);
        }

        public async Task ToCommand(GatewayModel selectedGateway, IList<object> commands, ReplaySubject<string> replay, string command)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (commands is null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            if (replay is null)
            {
                throw new ArgumentNullException(nameof(replay));
            }

            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException($"'{nameof(command)}' não pode ser nulo ou vazio", nameof(command));
            }

            commands.Add(new
            {
                Command = command,
                IsOk = await SendCommandRX(selectedGateway, replay, command).ConfigureAwait(true)
            });
        }

        public async Task TryCommands(GatewayModel selectedGateway, IList<object> commands, ReplaySubject<string> replay)
        {
            _taskService.CancellationTokenSourceAll = new CancellationTokenSource();

            var list = commands.Where(x => ((dynamic)x).IsOk == false).Select(x => (dynamic)x).ToList();

            for (int j = 0; j < 4 && list.Any() && !_taskService.CancellationTokenSourceAll.IsCancellationRequested; j++)
            {
                for (int i = 0; i < list.Count && !_taskService.CancellationTokenSourceAll.IsCancellationRequested; i++)
                {
                    if (await SendCommandRX(selectedGateway, replay, list.ElementAt(i).Command).ConfigureAwait(true))
                        list.RemoveAt(i);
                    continue;
                }
            }
        }
    }
}