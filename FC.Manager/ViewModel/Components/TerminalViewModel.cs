using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Configution;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Domain.Util;
using FC.Manager.ViewModel.Project;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Components
{
    public class TerminalViewModel : ProjectViewModelBase
    {
        private int _SelectedIndexVisualization = Domain.Properties.Settings.Default.visualization;

        public int SelectedIndexVisualization

        {
            get => _SelectedIndexVisualization;
            set
            {
                _ = SetProperty(ref _SelectedIndexVisualization, value);

                Domain.Properties.Settings.Default.visualization = value;
            }
        }

        public IEnumerable<int> Timeouts { get; set; }

        private int _timeout = 3000;

        public int Timeout
        {
            get => _timeout;
            set => SetProperty(ref _timeout, value);
        }

        public ICommand MouseLeftButtonDownDataGridCommand { get; set; }
        public ICommand MouseDoubleClickCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand ClearAllCommand { get; set; }
        public ICommand DeleteRowCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand ExportToExcelCommand { get; set; }
        public ICommand ResendCommand { get; set; }

        public ICommand DownCommand { get; set; }
        public ICommand TopCommand { get; set; }

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
            if (obj is not CommandModel command)
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
                OpenCustomMessageBox(header: Domain.Properties.Resources.Exported_To,
                                     message: Domain.Properties.Resources.Excel_File_Created_List);

                string[] lines = ExtensionMethods.ExportToExcel(CommandsSent.ToList());

                await CloseDialog();

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Excel file (*.csv)|*.csv",
                    FileName = $"{Domain.Properties.Resources.Terminal}-{DateTime.Now:hhmmssddMMyyyy}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllLines(saveFileDialog.FileName, lines);
                }

                count.Reset();

                count.SetTime(4000);

                count.Start();

                count.CountDownFinished = () => { IsActiveSnackbar = false; };

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.File_saved;

                action = () =>
                {
                    OpenFile(saveFileDialog);
                    ActionContentSnackbar = null;
                    IsActiveSnackbar = false;
                };

                ActionContentSnackbar = Domain.Properties.Resources.Open;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Down(object obj)
        {
            if (obj is ListBox lstBox)
            {
                if (lstBox.Items.Count == 0)
                {
                    return;
                }
                lstBox.SelectedIndex = lstBox.Items.Count - 1;
                lstBox.ScrollIntoView(lstBox.SelectedItem);
            }
            if (obj is DataGrid dataGrid)
            {
                if (dataGrid.Items.Count == 0)
                {
                    return;
                }
                dataGrid.SelectedIndex = dataGrid.Items.Count - 1;
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
        }

        private void Top(object obj)
        {
            if (obj is ListBox lstBox)
            {
                if (lstBox.Items.Count == 0)
                {
                    return;
                }
                lstBox.SelectedIndex = 0;
                lstBox.ScrollIntoView(lstBox.SelectedItem);
            }
            if (obj is DataGrid dataGrid)
            {
                if (dataGrid.Items.Count == 0)
                {
                    return;
                }
                dataGrid.SelectedIndex = 0;
                dataGrid.ScrollIntoView(dataGrid.SelectedItem);
            }
        }

        private void SelectionChanged(object obj)
        {
            if (obj is not CommandModel temp)
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
        }

        private void GatewayServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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
            IsActiveSnackbar = false;

            if (obj is not object[] array)
            {
                return;
            }
            if (array[0] is not DataGrid dataGrid)
            {
                return;
            }
            if (array[1] is not ListBox listBox)
            {
                return;
            }

            DataGrid = dataGrid;

            ListBox = listBox;
        }

        private readonly CountDownTimer timer;

        private ProtocolTypeEnum _selectedCommandProtocolType;
        private string _selectedCommandIP;
        private int _selectedCommandPort;
        private string _selectedCommandSend;
        public DataGrid DataGrid { get; set; }
        public ListBox ListBox { get; set; }

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
                                 IGatewayRepository gatewayRepository,
                                 IDialogService dialogService,
                                 ILoginRepository loginRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
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

            DownCommand = new RelayCommand<object>(Down);

            TopCommand = new RelayCommand<object>(Top);

            MouseDoubleClickCommand = new RelayCommand<object>(MouseDoubleClick);

            Timeouts = Enumerable.Range(1, 10000).Where(x => x % 1000 == 0);

            timer = new CountDownTimer();
        }

        private async void CommandsSentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is not ObservableCollection<CommandModel>)
            {
                return;
            }
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }
            if (DataGrid is null)
            {
                return;
            }

            if (ListBox is null)
            {
                return;
            }
            if (!Domain.Properties.Settings.Default.terminalAutoScroll)
            {
                return;
            }

            await Task.Delay(250);

            ListBox.SelectedIndex = e.NewStartingIndex;
            ListBox.ScrollIntoView(ListBox.SelectedItem);

            DataGrid.SelectedIndex = e.NewStartingIndex;
            DataGrid.ScrollIntoView(DataGrid.SelectedItem);
        }

        private readonly CountDownTimer count = new();

        private async void MouseDoubleClick(object obj)
        {
            using ReplaySubject<string> replay = new();

            if (obj is not CommandModel command)
            {
                return;
            }

            try
            {
                SelectedIndexProtocolType = (int)command.ProtocolType;
                SelectedCommandProtocolType = command.ProtocolType;
                SelectedCommandIP = command.IP;
                SelectedCommandPort = command.Port;
                SelectedCommandSend = command.Send;

                _gatewayService.IsSendingToGateway = true;

                _gatewayService.LastCommandSend = command.Send;

                _taskService.CancellationTokenSource = new CancellationTokenSource(Timeout);

                _ = _taskService.TimeLeftMsStrReplay.Subscribe(resp =>
                {
                    ContentSnackbar = $"{Domain.Properties.Resources.Sending} | {Domain.Properties.Resources.Timeout} {resp}";
                });

                IsActiveSnackbar = true;

                _ = command.ProtocolType == ProtocolTypeEnum.UDP
                    ? await _udpRepository.SendCommandRX(ip: command.IP,
                                                       port: command.Port,
                                                       command: command.Send,
                                                       isMultipleResponses: true,
                                                       replay: replay,
                                                       timeout: Timeout,
                                                       connectionType: command.ConnectionType)
                    : await _tcpRepository.SendCommandRX(ip: command.IP,
                                                       port: command.Port,
                                                       command: command.Send,
                                                       isMultipleResponses: true,
                                                       replay: replay,
                                                       timeout: Timeout,
                                                       connectionType: command.ConnectionType);

                IsActiveSnackbar = false;

                ContentSnackbar = string.Format(Domain.Properties.Resources.Send_Sucess, Domain.Properties.Resources.Sent);

                IsActiveSnackbar = true;

                count.Reset();

                count.SetTime(1000);

                count.Start();

                count.CountDownFinished = () =>
                {
                    IsActiveSnackbar = false;
                };

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                replay.Dispose();
            }
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

                using ReplaySubject<string> replay = new();

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
                ShowError(ex);
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

                using ReplaySubject<string> replay = new();

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
                ShowError(ex);
            }
        }
    }
}