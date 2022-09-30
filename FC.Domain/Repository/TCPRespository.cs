using CommunityToolkit.Mvvm.Messaging;
using FC.Domain.Model;
using FC.Domain.Repository;
using Parse;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Domain.Service
{
    public interface ITcpRepository
    {
        Task<bool> SendCommandRX(string ip,
                                 int port,
                                 string command,
                                 ReplaySubject<string> replay,
                                 ConnectionType connectionType,
                                 bool isMultipleResponses = false,
                                 int timeout = 2000);
    }

    public class TCPRespository : ITcpRepository
    {
        private readonly ICommandRepository _commandRepository;

        private readonly IGatewayService _gatewayService;

        private readonly ITaskService _taskService;
        private bool isCanceled;

        public TCPRespository(ICommandRepository commandRepository,
                              ITaskService taskService,
                              IGatewayService gatewayService)
        {
            _commandRepository = commandRepository;
            _taskService = taskService;
            _gatewayService = gatewayService;
            _ = _taskService.CanceledReplay.Subscribe(response => { isCanceled = response; });
        }

        public async Task<bool> SendCommandRX(string ip,
                                              int port,
                                              string command,
                                              ReplaySubject<string> replay,
                                              ConnectionType connectionType,
                                              bool isMultipleResponses = false,
                                              int timeout = 2000)
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

            command = command.Replace('\n', '\0');

            _gatewayService.IsSendingToGateway = true;

            timeout += Properties.Settings.Default.delayTimeBetweenCommand;

            _taskService.CancellationTokenSource = new CancellationTokenSource(timeout);

            _taskService.SetTimeout(timeout);

            if (isCanceled)
            {
                return false;
            }

            bool @return = false;

            await Task.Run(async () =>
            {
                using CommandModel comm = new(command, ip, port, connectionType, ProtocolTypeEnum.TCP, ParseUser.CurrentUser?.ObjectId);

                comm.IPTarget = ip;

                comm.Timeout = timeout;

                string response = string.Empty;

                string previousAnswer = string.Empty;

                using TcpClient tcpClient = new()
                {
                    ReceiveTimeout = timeout
                };

                _ = _taskService.CancellationTokenSource.Token.Register(() =>
                  {
                      tcpClient.Close();
                  });

                try
                {
                    await tcpClient.ConnectAsync(ip, port);
                }
                catch (NullReferenceException)
                {
                    return;
                }

                using NetworkStream netStream = tcpClient.GetStream();

                try
                {
                    if (netStream.CanWrite)
                    {
                        byte[] sendBytes = Encoding.UTF8.GetBytes(command);

                        netStream.Write(sendBytes, 0, sendBytes.Length);
                    }
                    else
                    {
                        response = Properties.Resources.No_reply;

                        comm.Receive = response;

                        comm.GetResponseTime();

                        tcpClient.Close();

                        netStream.Close();
                    }

                    if (netStream.CanRead)
                    {
                        do
                        {
                            if (_taskService.CancellationTokenSource.IsCancellationRequested)
                            {
                                break;
                            }

                            if (!netStream.DataAvailable)
                            {
                                continue;
                            }

                            byte[] bytes = new byte[tcpClient.ReceiveBufferSize];

                            int lenght = netStream.Read(bytes, 0, tcpClient.ReceiveBufferSize);

                            byte[] temp = new byte[lenght];

                            Array.Copy(bytes, temp, lenght);

                            response = Encoding.UTF8.GetString(temp);

                            response = response.Replace('\n', '\0');

                            if (response.Equals(previousAnswer))
                            {
                                continue;
                            }

                            previousAnswer = response;

                            @return = true;

                            replay.OnNext(response);

                            comm.Receive = response;

                            comm.GetResponseTime();

                            Add(comm);

                            if (!isMultipleResponses)
                            {
                                break;
                            }
                        } while (true);
                    }
                    else
                    {
                        tcpClient.Close();

                        netStream.Close();

                        comm.Receive = Properties.Resources.No_reply;

                        comm.GetResponseTime();

                        Add(comm);
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.ErrorCode == 10061)
                    {
                        tcpClient.Close();

                        comm.Receive = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Exception_No_connection_could_be_made_because_the_target_machine_actively_refused_them, $"{ip}:{port}");

                        comm.GetResponseTime();

                        Add(comm);
                    }
                    else if (ex.ErrorCode == 10065)
                    {
                        tcpClient.Close();

                        comm.Receive = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Exception_No_connection_could_be_made_because_the_target_machine_actively_refused_them, $"{ip}:{port}");

                        comm.GetResponseTime();

                        Add(comm);
                    }
                    else
                    {
                        tcpClient.Close();

                        comm.Receive = ex.Message;

                        comm.GetResponseTime();

                        Add(comm);
                    }
                }
                catch (NullReferenceException)
                {
                    comm.Receive = Properties.Resources.Not_Respond;

                    comm.GetResponseTime();

                    Add(comm);
                }
                catch (Exception ex)
                {
                    comm.Receive = ex.Message;

                    comm.GetResponseTime();

                    Add(comm);
                }
                finally
                {
                    DebugWriteLine(command, response);

                    tcpClient.Close();

                    netStream.Close();

                    if (comm.Send == Properties.Resources.Not_Respond)
                    {
                        Add(comm);
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
            return @return;
        }

        private void Add(CommandModel command)
        {
            _ = WeakReferenceMessenger.Default.Send(command);
        }

        private static void DebugWriteLine(string command, string response)
        {
            int length = command.Length > response.Length ? command.Length : response.Length;

            Debug.WriteLine($"\n{string.Concat(Enumerable.Repeat("x", length + 5))}");

            Debug.WriteLine($"TCP▲:{command}");

            Debug.WriteLine($"TCP▼:{response}");

            Debug.WriteLine($"\n{string.Concat(Enumerable.Repeat("x", length + 5))}\n");
        }
    }
}