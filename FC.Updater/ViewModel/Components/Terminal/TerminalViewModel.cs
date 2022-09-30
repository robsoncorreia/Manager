using CommunityToolkit.Mvvm.Input;
using FC.Domain.Model;
using FC.Domain.Model.Configution;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Domain.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Updater.ViewModel.Components.Terminal
{
    public class TerminalViewModel : FlexViewModelBase
    {
        public IEnumerable<int> Timeouts { get; set; }

        private int _timeout = 3000;

        public int Timeout
        {
            get => _timeout;
            set => SetProperty(ref _timeout, value);
        }

        public ICommand MouseLeftButtonDownDataGridCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand ClearAllCommand { get; set; }
        public ICommand DeleteRowCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand ExportToExcelCommand { get; set; }
        public ICommand ResendCommand { get; set; }

        private int _selectedIndexProtocolType;

        public int SelectedIndexProtocolType
        {
            get => _selectedIndexProtocolType;
            set => SetProperty(ref _selectedIndexProtocolType, value);
        }

        public ProtocolTypeEnum SelectedCommandProtocolType
        {
            get => _selectedCommandProtocolType;
            set => SetProperty(ref _selectedCommandProtocolType, value);
        }

        public string SelectedCommandIP
        {
            get => _selectedCommandIP;
            set => SetProperty(ref _selectedCommandIP, value);
        }

        public int SelectedCommandPort
        {
            get => _selectedCommandPort;
            set => SetProperty(ref _selectedCommandPort, value);
        }

        public string SelectedCommandSend
        {
            get => _selectedCommandSend;
            set => SetProperty(ref _selectedCommandSend, value);
        }

        private int _selectedIndexCommand;

        public int SelectedIndexCommand
        {
            get => _selectedIndexCommand;
            set => SetProperty(ref _selectedIndexCommand, value);
        }

        private ConfigurationApp _configurationApp;

        public ObservableCollection<CommandModel> CommandsSent { get; set; }

        public ConfigurationApp ConfigurationApp
        {
            get => _configurationApp;
            set => SetProperty(ref _configurationApp, value);
        }

        private void MouseLeftButtonDownDataGrid(object obj)
        {
            if (!(obj is CommandModel command))
            {
                return;
            }

            if (!Domain.Properties.Settings.Default.terminalAutoComplete)
            {
                return;
            }
            SelectedIndexProtocolType = (int)command.ProtocolType;
            SelectedCommandProtocolType = command.ProtocolType;
            SelectedCommandIP = command.IP;
            SelectedCommandPort = command.Port;
            SelectedCommandSend = command.Send;
        }

        private async void ExportToExcel(object obj)
        {
            try
            {
                if (!Domain.Properties.Settings.Default.isCurrentProcessAdmin)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Run_the_program_with_administrator_to_export,
                                         ok: async () => await CloseDialog(),
                                         textButtomOk: Domain.Properties.Resources.Ok);
                    return;
                }
                _gatewayService.IsSendingToGateway = true;

                _ = ExtensionMethods.ExportToExcel(CommandsSent.ToList());

                _gatewayService.IsSendingToGateway = false;
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void SelectionChanged(object obj)
        {
            if (!(obj is CommandModel temp))
            {
                return;
            }

            if (!Domain.Properties.Settings.Default.terminalAutoComplete)
            {
                return;
            }

            SelectedIndexProtocolType = (int)temp.ProtocolType;
            SelectedCommandProtocolType = temp.ProtocolType;
            SelectedCommandIP = temp.IP;
            SelectedCommandPort = temp.Port;
            SelectedCommandSend = temp.Send;
        }

        private void Copy(object obj)
        {
            if (SelectedIndexCommand < 0)
            {
                return;
            }
            if (!CommandsSent.Any())
            {
                return;
            }

            Clipboard.SetText(CommandsSent[SelectedIndexCommand] + "");
        }

        private void DeleteRow(object obj)
        {
            if (SelectedIndexCommand < 0)
            {
                return;
            }
            if (!CommandsSent.Any())
            {
                return;
            }

            _ = _localDBRepository.Delete(CommandsSent[SelectedIndexCommand]);

            CommandsSent.RemoveAt(SelectedIndexCommand);
        }

        private void ConfigurationAppPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _ = _configurationRepository.Update(ConfigurationApp);
            System.Diagnostics.Debug.WriteLine("Update config");
        }

        private void GatewayServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(IsSending))
            //{
            //    IsSending = ((GatewayService)sender).IsSendingToGateway;
            //}
            if (e.PropertyName == nameof(LastCommandSend))
            {
                LastCommandSend = ((GatewayService)sender).LastCommandSend;
            }
            if (e.PropertyName == nameof(IsSendingToGateway))
            {
                IsSendingToGateway = ((GatewayService)sender).IsSendingToGateway;
            }
        }

        private void Loaded(object obj)
        {
            if (!(obj is DataGrid dataG))
            {
                return;
            }

            dataGrid = dataG;
        }

        private void CommandsSentCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (CommandsSent.Count < 1)
            {
                return;
            }

            if (!Domain.Properties.Settings.Default.terminalAutoScroll)
            {
                return;
            }

            if (!(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add))
            {
                return;
            }
            if (dataGrid is null)
            {
                return;
            }
            cancellationTokenSource.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            newStartingIndex = e.NewStartingIndex;
            ScrollIntoView();
        }

        private int newStartingIndex;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private async void ScrollIntoView()
        {
            await Task.Delay(120);
            _ = Task.Run(() =>
              {
                  Application.Current.Dispatcher.Invoke(delegate
                  {
                      dataGrid.ScrollIntoView(dataGrid.Items.GetItemAt(newStartingIndex));
                  });
              }, cancellationTokenSource.Token);
        }

        private DataGrid dataGrid;
        private ProtocolTypeEnum _selectedCommandProtocolType;
        private string _selectedCommandIP;
        private int _selectedCommandPort;
        private string _selectedCommandSend;

        public TerminalViewModel(IFrameNavigationService navigationService,
                                 IProjectService projectService,
                                 IUDPRepository udpRepository,
                                 ITcpRepository tcpRepository,
                                 IIRRepository irRepository,
                                 IUserRepository userService,
                                 IProjectRepository projectRepository,
                                 ILocalDBRepository localDBRepository,
                                 ILogRepository logRepository,
                                 IUserRepository userRepository,
                                 ISerialRepository serialRepository,
                                 ICommandRepository commandRepository,
                                 IIPCommandRepository ipCommandRepository,
                                 IGatewayService gatewayService,
                                 ISettingsRepository configurationRepository,
                                 ITaskService taskService,
                                 IParseService parseService,
                                 IRFRepository rfRepository,
                                 IZwaveRepository zwaveRepository,
                                 IRelayRepository relayTestRepository,
                                 IGatewayRepository gatewayRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository)
        {
            _tcpRepository = tcpRepository;

            CommandsSent = new ObservableCollection<CommandModel>();

            CommandsSent = _commandRepository?.Commands;

            ConfigurationApp = _configurationRepository?.ConfigurationApp;

            _gatewayService.PropertyChanged += GatewayServicePropertyChanged;

            ConfigurationApp.PropertyChanged += ConfigurationAppPropertyChanged;

            CommandsSent.CollectionChanged += CommandsSentCollectionChanged;

            MouseLeftButtonDownDataGridCommand = new RelayCommand<object>(MouseLeftButtonDownDataGrid);

            SendCommand = new RelayCommand<object>(Send);

            ClearAllCommand = new RelayCommand<object>(ClearAll);

            DeleteRowCommand = new RelayCommand<object>(DeleteRow);

            SelectionChangedCommand = new RelayCommand<object>(SelectionChanged);

            ExportToExcelCommand = new RelayCommand<object>(ExportToExcel);

            ClearCommand = new RelayCommand<object>(Clear);

            LoadedCommand = new RelayCommand<object>(Loaded);

            CopyCommand = new RelayCommand<object>(Copy);

            ResendCommand = new RelayCommand<object>(Resend);

            Timeouts = Enumerable.Range(1, 10000).Where(x => x % 1000 == 0);
        }

        private async void Resend(object obj)
        {
            try
            {
                if (SelectedIndexCommand < 0)
                {
                    return;
                }
                if (!CommandsSent.Any())
                {
                    return;
                }

                using ReplaySubject<string> replay = new ReplaySubject<string>();

                _taskService.CancellationTokenSource = new CancellationTokenSource(Timeout);

                _gatewayService.IsSendingToGateway = true;

                _gatewayService.LastCommandSend = CommandsSent[SelectedIndexCommand].Send;

                if (CommandsSent[SelectedIndexCommand].ProtocolType == ProtocolTypeEnum.UDP)
                {
                    _ = await _udpRepository.SendCommandRX(ip: CommandsSent[SelectedIndexCommand].IP,
                                                       port: CommandsSent[SelectedIndexCommand].Port,
                                                       command: CommandsSent[SelectedIndexCommand].Send,
                                                       isMultipleResponses: true,
                                                       replay: replay,
                                                       timeout: Timeout,
                                                       connectionType: ConnectionType.Default);

                    _gatewayService.IsSendingToGateway = false;

                    return;
                }

                _ = await _tcpRepository.SendCommandRX(ip: CommandsSent[SelectedIndexCommand].IP,
                                                   port: CommandsSent[SelectedIndexCommand].Port,
                                                   command: CommandsSent[SelectedIndexCommand].Send,
                                                   isMultipleResponses: true,
                                                   replay: replay,
                                                   timeout: Timeout,
                                                   connectionType: ConnectionType.Default);

                _gatewayService.IsSendingToGateway = false;

                replay.Dispose();
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async Task SendCommandRX(GatewayModel selectedGateway,
                                         ReplaySubject<string> replay,
                                         string command,
                                         int timeout = 6000)
        {
            _taskService.CancellationTokenSource = new CancellationTokenSource(timeout);

            _gatewayService.IsSendingToGateway = true;

            _gatewayService.LastCommandSend = command;

            switch ((GatewayConnectionType)Domain.Properties.Settings.Default.gatewayConnectionType)
            {
                case GatewayConnectionType.TCP:
                    _ = await _tcpRepository.SendCommandRX(ip: selectedGateway.LocalIP,
                                                       port: selectedGateway.LocalPortTCP,
                                                       command: command,
                                                       isMultipleResponses: false,
                                                       replay: replay,
                                                       timeout: timeout,
                                                       connectionType: selectedGateway.ConnectionType);

                    break;

                case GatewayConnectionType.UDP:
                    _ = await _udpRepository.SendCommandRX(ip: selectedGateway.LocalIP,
                                                       port: selectedGateway.LocalPortUDP,
                                                       command: command,
                                                       isMultipleResponses: false,
                                                       replay: replay,
                                                       connectionType: selectedGateway.ConnectionType,
                                                       timeout: timeout);

                    break;

                default:
                    break;
            }
        }

        private void Clear(object obj)
        {
            SelectedCommandSend = string.Empty;
        }

        private void ClearAll(object obj)
        {
            CommandsSent.Clear();
            _ = _localDBRepository.DeleteAll(nameof(CommandModel));
        }

        private async void Send(object obj)
        {
            try
            {
                if (obj is CommandModel temp)
                {
                    SelectedIndexProtocolType = (int)temp.ProtocolType;
                    SelectedCommandProtocolType = temp.ProtocolType;
                    SelectedCommandIP = temp.IP;
                    SelectedCommandPort = temp.Port;
                    SelectedCommandSend = temp.Send;
                }

                using ReplaySubject<string> replay = new ReplaySubject<string>();

                _taskService.CancellationTokenSource = new CancellationTokenSource(Timeout);

                _gatewayService.IsSendingToGateway = true;

                _gatewayService.LastCommandSend = SelectedCommandSend;

                if (SelectedIndexProtocolType == 0)
                {
                    _ = await _udpRepository.SendCommandRX(ip: SelectedCommandIP,
                                                       port: SelectedCommandPort,
                                                       command: SelectedCommandSend,
                                                       isMultipleResponses: true,
                                                       replay: replay,
                                                       timeout: Timeout,
                                                       connectionType: ConnectionType.Default);

                    _gatewayService.IsSendingToGateway = false;

                    return;
                }

                _ = await _tcpRepository.SendCommandRX(ip: SelectedCommandIP,
                                                   port: SelectedCommandPort,
                                                   command: SelectedCommandSend,
                                                   isMultipleResponses: true,
                                                   replay: replay,
                                                   timeout: Timeout,
                                                   connectionType: ConnectionType.Default);

                _gatewayService.IsSendingToGateway = false;
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }
    }
}