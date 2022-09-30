using FC.Domain.Model;
using FC.Domain.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public class RepositoryBase
    {
        internal IUDPRepository _udpRepository;
        internal ITaskService _taskService;
        internal IGatewayService _gatewayService;
        internal ITcpRepository _tcpRepository;
        internal bool isCanceled;

        public RepositoryBase(ITaskService taskService,
                              IGatewayService gatewayService,
                              ITcpRepository tcpRespository,
                              IUDPRepository udpRepository)
        {
            _udpRepository = udpRepository;

            _taskService = taskService;

            _gatewayService = gatewayService;

            _tcpRepository = tcpRespository;

            _ = _taskService.CanceledReplay.Subscribe(response =>
            {
                isCanceled = response;
            });
        }

        public static void ThrowException(GatewayModel selectedGateway)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            throw new Exception(string.Format(CultureInfo.CurrentCulture,
                                              Properties.Resources.Device_did_not_respond,
                                              selectedGateway.Name,
                                              selectedGateway.LocalIP,
                                              (GatewayConnectionType)Properties.Settings.Default.gatewayConnectionType == GatewayConnectionType.UDP ?
                                                selectedGateway.LocalPortUDP : selectedGateway.LocalPortTCP));
        }

        private async Task<bool> SendCommandRX(GatewayModel selectedGateway,
                                               ReplaySubject<string> replay,
                                               string command,
                                               int timeout = 3000,
                                               bool isMultipleResponses = false,
                                               bool isSendingToGateway = false,
                                               bool isForceUDPDefault = false)
        {
            _gatewayService.LastCommandSend = command;

            bool @return = isForceUDPDefault || (GatewayConnectionType)Properties.Settings.Default.gatewayConnectionType == GatewayConnectionType.UDP
                ? await _udpRepository.SendCommandRX(ip: selectedGateway.LocalIP,
                                                                              port: isForceUDPDefault ? 9999 : selectedGateway.LocalPortUDP,
                                                                              command: command,
                                                                              replay: replay,
                                                                              timeout: timeout,
                                                                              isMultipleResponses: isMultipleResponses,
                                                                              connectionType: selectedGateway.ConnectionType)
                : await _tcpRepository.SendCommandRX(ip: selectedGateway.LocalIP,
                                                                               port: selectedGateway.LocalPortTCP,
                                                                               command: command,
                                                                               isMultipleResponses: isMultipleResponses,
                                                                               replay: replay,
                                                                               timeout: timeout,
                                                                               connectionType: selectedGateway.ConnectionType);
            _gatewayService.IsSendingToGateway = isSendingToGateway;

            return @return;
        }

        public async Task ToCommand(GatewayModel selectedGateway,
                                     IList<object> commands,
                                     ReplaySubject<string> replay,
                                     string command,
                                     bool isSendingToGateway = false,
                                     bool isMultipleResponses = false,
                                     byte repetitions = 4,
                                     int timeout = 3000,
                                     bool isForceUDP = false)
        {
            commands.Add(new
            {
                Command = command,
                IsOk = await SendCommandRX(selectedGateway: selectedGateway,
                                           replay: replay,
                                           command: command,
                                           isMultipleResponses: isMultipleResponses,
                                           isSendingToGateway: isSendingToGateway,
                                           timeout: timeout,
                                           isForceUDPDefault: isForceUDP)
            });

            List<dynamic> list = commands.Where(x => ((dynamic)x).IsOk == false).Select(x => (dynamic)x).ToList();

            for (int j = 0; j < (repetitions - 1) && list.Any(); j++)
            {
                if (isCanceled)
                {
                    break;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (isCanceled)
                    {
                        break;
                    }
                    if (await SendCommandRX(selectedGateway: selectedGateway,
                                            replay: replay,
                                            command: list.ElementAt(i).Command,
                                            isSendingToGateway: isSendingToGateway,
                                            isMultipleResponses: isMultipleResponses,
                                            timeout: timeout))
                    {
                        list.RemoveAt(i);
                    }

                    continue;
                }
            }
            commands.Clear();
        }
    }
}