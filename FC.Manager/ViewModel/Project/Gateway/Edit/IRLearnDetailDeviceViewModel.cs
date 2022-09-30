using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.Configution;
using FC.Domain.Model.IR;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Domain.Util;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class IRLearnDetailDeviceViewModel : ProjectViewModelBase
    {
        private readonly INetworkService _internetService;
        private readonly ISettingsService _settingsService;
        private ConfigurationApp _configurationApp;
        private IRModel _irmodel;
        private bool _isTerminalEnable;
        private bool _isTerminalIRVisible;
        private int _selectedChannelIndex;
        private int _selectedIRIndex = -1;

        public IRLearnDetailDeviceViewModel(IFrameNavigationService navigationService,
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
                                 ILoginRepository loginRepository,
                                 INetworkService internetService,
                                 ISettingsService settingsService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _internetService = internetService;

            _settingsService = settingsService;

            IRModel = new IRModel();

            IRLearningCommand = new RelayCommand<object>(Learning);

            SaveCommand = new RelayCommand<object>(Save);

            NewCommand = new RelayCommand<object>(New);

            DeleteAllFromGatewayCommand = new RelayCommand<object>(DeleteAllFromGateway);

            LoadedCommand = new RelayCommand<object>(Loaded);

            ExportToExcelCloudListCommand = new RelayCommand<object>(ExportToExcelCloudList);

            ExportToExcelGatewayListCommand = new RelayCommand<object>(ExportToExcelGatewayList);

            PlayMemoryCommand = new RelayCommand<object>(PlayMemoryAsync);

            PlayCodeCommand = new RelayCommand<object>(PlayCode);

            PlayCodeCloudCommand = new RelayCommand<object>(PlayCodeCloud);

            SendToGatewayCommand = new RelayCommand<object>(SendToGateway);

            DeleteAllFromCloudCommand = new RelayCommand<object>(DeleteAllFromCloud);

            GetAllFromGatewayCommand = new RelayCommand<object>(async (obj) => await GetAllFromGateway(obj));

            GetAllFromCloudCommand = new RelayCommand<object>(async (obj) => await GetAllFromCloud(obj));

            DeleteFromGatewayCommand = new RelayCommand<object>(DeleteFromGateway);

            DeleteCloudAwaitCommand = new RelayCommand<object>(DeleteCloudAwait);

            DeleteAllFromCloudAwaitCommand = new RelayCommand<object>(DeleteAllFromCloudAwait);

            DeleteFromGatewayAwaitCommand = new RelayCommand<object>(DeleteFromGatewayAwait);

            DeleteAllFromGatewayAwaitCommand = new RelayCommand<object>(DeleteAllFromGatewayAwait);

            SelectedIRCommand = new RelayCommand<object>(SelectedIR);

            GetAllCommand = new RelayCommand<object>(async (obj) => await GetAllFromGateway(obj));

            UnloadedCommand = new RelayCommand<object>(Unloaded);

            ClearCommand = new RelayCommand<object>(Clear);

            DeleteCloudAsyncCommand = new RelayCommand<object>(DeleteCloudAsync);

            IsTerminalEnable = Domain.Properties.Settings.Default.enableTerminal;

            _settingsService.PropertyChanged += SettingsServicePropertyChanged;

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });

            if (_parseService != null)
            {
                _parseService.PropertyChanged += ParseServicePropertyChanged;
            }
        }

        #region ICommand

        public ICommand ClearCommand { get; set; }
        public ICommand DeleteAllFromCloudAwaitCommand { get; set; }
        public ICommand DeleteAllFromCloudCommand { get; set; }
        public ICommand DeleteAllFromGatewayAwaitCommand { get; set; }
        public ICommand DeleteAllFromGatewayCommand { get; set; }
        public ICommand DeleteCloudAsyncCommand { get; set; }
        public ICommand DeleteCloudAwaitCommand { get; set; }
        public ICommand DeleteFromGatewayAwaitCommand { get; set; }
        public ICommand DeleteFromGatewayCommand { get; set; }
        public ICommand ExportToExcelCloudListCommand { get; set; }
        public ICommand ExportToExcelGatewayListCommand { get; set; }
        public ICommand GetAllCommand { get; set; }
        public ICommand GetAllFromCloudCommand { get; set; }
        public ICommand GetAllFromGatewayCommand { get; set; }
        public ICommand IRLearningCommand { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand PlayCodeCloudCommand { get; set; }
        public ICommand PlayCodeCommand { get; set; }
        public ICommand PlayMemoryCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SelectedIRCommand { get; set; }
        public ICommand SendToGatewayCommand { get; set; }

        #endregion ICommand

        #region Properties

        public ConfigurationApp ConfigurationApp
        {
            get => _configurationApp;
            set => SetProperty(ref _configurationApp, value);
        }

        public IRModel IRModel
        {
            get => _irmodel;
            set => SetProperty(ref _irmodel, value);
        }

        public bool IsTerminalEnable
        {
            get => _isTerminalEnable;
            set => SetProperty(ref _isTerminalEnable, value);
        }

        public bool IsTerminalIRVisible
        {
            get => _isTerminalIRVisible;
            set
            {
                if (Equals(_isTerminalIRVisible, value))
                {
                    return;
                }
                _ = SetProperty(ref _isTerminalIRVisible, value);
            }
        }

        public int SelectedChannelIndex
        {
            get => _selectedChannelIndex;
            set
            {
                if (Equals(_selectedChannelIndex, value))
                {
                    return;
                }
                _ = SetProperty(ref _selectedChannelIndex, value);
            }
        }

        public int SelectedIRIndex
        {
            get => _selectedIRIndex;
            set => SetProperty(ref _selectedIRIndex, value);
        }

        #endregion Properties

        #region Method

        private readonly CountDownTimer count = new();

        private void Clear(object obj)
        {
            IRModel.Data = string.Empty;
        }

        private async void DeleteAllFromCloud(object obj)
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());

                    return;
                }

                await _irRepository.DeleteAllFromCloudAsync(SelectedProjectModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAllFromCloudAwait(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Delete_All_IRs_Cloud,
                                 custom: () => DeleteAllFromCloud(obj),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 cancel: async () => await CloseDialog());
        }

        private async void DeleteAllFromGateway(object obj)
        {
            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                await _irRepository.DeleteAll(SelectedProjectModel.SelectedGateway);

                SelectedProjectModel.SelectedGateway.IRsGateway.Clear();

                IRModel = new IRModel();

                await CloseDialog(1000);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAllFromGatewayAwait(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete_All,
                                 message: Domain.Properties.Resources.Delete_All_IR_From_Gateway,
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 custom: () => DeleteAllFromGateway(obj),
                                 cancel: async () => await CloseDialog());
        }

        private async Task DeleteCloud(object obj)
        {
            using ReplaySubject<string> rx = new();
            using CountDownTimer timer = new();

            try
            {
                if (obj is not IRModel irModel)
                {
                    return;
                }

                if (!await _internetService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         ok: async () => await CloseDialog());

                    return;
                }

                timer.SetTime(1000);
                timer.Start();
                timer.TimeChanged += () => rx.OnNext($"{timer.TimeLeftMsStr}");
                timer.StepMs = 77;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Deleting);

                await _irRepository.DeleteFromCloudAsync(SelectedProjectModel, irModel);

                await Task.Run(() => { while (timer.IsRunnign) { }; });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                rx.Dispose();

                timer.Dispose();
            }
        }

        private void DeleteCloudAsync(object obj)
        {
            if (obj is not IRModel temp)
            {
                return;
            }
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Want_to_delete_IR_from_the_cloud_,
                                 textButtonCancel: Domain.Properties.Resources._Cancel,
                                 cancel: async () => await CloseDialog(),
                                 textButtomOk: Domain.Properties.Resources._Delete,
                                 ok: async () => await DeleteCloud(temp));
        }

        private void DeleteCloudAwait(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Delete_IR_From_Cloud,
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 custom: async () => await DeleteCloud(obj),
                                 cancel: async () => await CloseDialog());
        }

        private async void DeleteFromGateway(object obj)
        {
            try
            {
                if (obj is not IRModel irModel)
                {
                    return;
                }

                await _irRepository.Delete(SelectedProjectModel.SelectedGateway, irModel.MemoryId);

                _ = SelectedProjectModel.SelectedGateway.IRsGateway.Remove(irModel);

                IRModel = new IRModel();

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteFromGatewayAwait(object obj)
        {
            if (obj is not IRModel temp)
            {
                return;
            }

            SelectedProjectModel.SelectedGateway.SelectedIndexIR = SelectedProjectModel.SelectedGateway.IRsGateway.IndexOf(temp);

            IRModel = temp;

            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Delete_IR_From_Gateway,
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 custom: () => DeleteFromGateway(obj),
                                 cancel: async () => await CloseDialog(0));
        }

        private async void ExportToExcelCloudList(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Exported_To,
                                     message: Domain.Properties.Resources.Excel_File_Created_List);

                string[] lines = ExtensionMethods.ExportToExcel(SelectedProjectModel.SelectedGateway.IRsCloud.ToList());

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Excel file (*.csv)|*.csv",
                    FileName = $"Terminal-{DateTime.Now:hhmmssddMMyyyy}"
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

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void ExportToExcelGatewayList(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Exported_To,
                                     message: Domain.Properties.Resources.Excel_File_Created_List);

                string[] lines = ExtensionMethods.ExportToExcel(SelectedProjectModel.SelectedGateway.IRsGateway.ToList());

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Excel file (*.csv)|*.csv",
                    FileName = $"{Domain.Properties.Resources.IR}-{DateTime.Now:hhmmssddMMyyyy}"
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

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task GetAllFromCloud(object obj = null)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog(),
                                     isProgressBar: true);

                await _irRepository.GetAllFromCloud(SelectedProjectModel);

                count.SetTime(2000);

                count.Reset();

                count.Start();

                await Task.Run(() => { while (count.IsRunnign) {; } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: ex.Message,
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: async () => await GetAllFromCloud(obj),
                                     cancel: async () => await CloseDialog());
            }
        }

        private async Task<bool> GetAllFromGateway(object obj = null)
        {
            bool @return = false;

            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog(),
                                     isProgressBar: true);

                @return = await _irRepository.GetAll(SelectedProjectModel.SelectedGateway, true);

                if (!@return)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: GatewayException(SelectedProjectModel.SelectedGateway),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: async () => await GetAllFromGateway(obj),
                                         cancel: async () => await CloseDialog());

                    return @return;
                }

                count.SetTime(2000);

                count.Reset();

                count.Start();

                await Task.Run(() => { while (count.IsRunnign) {; } });

                await CloseDialog();

                IRModel = new IRModel();

                return @return;
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                    message: ex.Message,
                                    textButtonCancel: Domain.Properties.Resources.Close,
                                    textButtonCustom: Domain.Properties.Resources.Try_again,
                                    custom: async () => await GetAllFromGateway(obj),
                                    cancel: async () => await CloseDialog());

                return @return;
            }
        }

        private async void Learning(object obj)
        {
            using ReplaySubject<string> rx = new();
            using CountDownTimer timer = new();

            try
            {
                if (IRModel.MemoryId < 0)
                {
                    IRModel = new IRModel();
                }

                timer.SetTime(10000);
                timer.Start();
                timer.TimeChanged += () => rx.OnNext($"{timer.TimeLeftMsStr}");
                timer.StepMs = 77;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Learning,
                                     rx: rx,
                                     message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Learning_mode_device, SelectedProjectModel.SelectedGateway.Name));

                bool isResponse = await _irRepository.Learn(SelectedProjectModel.SelectedGateway, IRModel);

                if (!isResponse)
                {
                    await Task.Run(() => { while (timer.IsRunnign) { }; });
                }

                rx.Dispose();

                timer.Dispose();

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);

                rx.Dispose();

                timer.Dispose();
            }
            finally
            {
                rx.Dispose();

                timer.Dispose();
            }
        }

        private async void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            if (obj is not UserControl view)
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            IRModel = new IRModel();

            SelectedProjectModel.SelectedGateway.SelectedIndexIR = -1;

            SelectedProjectModel.SelectedGateway.SelectedIndexIRCloud = -1;

            if (!SelectedProjectModel.SelectedGateway.IRsGateway.Any())
            {
                if (!await GetAllFromGateway())
                {
                    return;
                }
            }

            if (!SelectedProjectModel.SelectedGateway.IRsCloud.Any())
            {
                await GetAllFromCloud();
            }
        }

        private void New(object obj)
        {
            IRModel = new IRModel();

            SelectedProjectModel.SelectedGateway.SelectedIndexIR = -1;
            SelectedProjectModel.SelectedGateway.SelectedIndexIRCloud = -1;
        }

        private void ParseServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsSendingToCloud))
            {
                IsSendingToCloud = ((ParseService)sender).IsSendingToCloud;
            }
        }

        private async void PlayCode(object obj)
        {
            try
            {
                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.Sending;

                await _irRepository.PlayCode(SelectedProjectModel.SelectedGateway, IRModel);

                IsActiveSnackbar = false;

                count.Reset();

                count.Start();

                count.SetTime(2000);

                ContentSnackbar = string.Format(Domain.Properties.Resources.Send_Sucess, Domain.Properties.Resources.Sent);

                IsActiveSnackbar = true;

                count.CountDownFinished = () => { IsActiveSnackbar = false; };

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void PlayCodeCloud(object obj)
        {
            if (obj is not IRModel irModel)
            {
                return;
            }

            try
            {
                SelectedProjectModel.SelectedGateway.SelectedIndexIRCloud = SelectedProjectModel.SelectedGateway.IRsCloud.IndexOf(irModel);

                IRModel = JsonConvert.DeserializeObject<IRModel>(JsonConvert.SerializeObject(irModel));

                IRModel.IsSending = true;

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.Sending;

                await _irRepository.PlayCode(SelectedProjectModel.SelectedGateway, IRModel);

                IsActiveSnackbar = false;

                count.Reset();

                count.Start();

                count.SetTime(2000);

                ContentSnackbar = string.Format(Domain.Properties.Resources.Send_Sucess, Domain.Properties.Resources.Sent);

                IsActiveSnackbar = true;

                count.CountDownFinished = () => { IsActiveSnackbar = false; };

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                IRModel.IsSending = false;

                irModel.IsSending = false;
            }
        }

        private async void PlayMemoryAsync(object obj)
        {
            if (obj is not IRModel irModel)
            {
                return;
            }
            try
            {
                irModel.IsSending = true;

                IRModel = JsonConvert.DeserializeObject<IRModel>(JsonConvert.SerializeObject(irModel));

                SelectedProjectModel.SelectedGateway.SelectedIndexIR = SelectedProjectModel.SelectedGateway.IRsGateway.IndexOf(irModel);

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.Sending;

                await _irRepository.PlayMemory(SelectedProjectModel.SelectedGateway, irModel.MemoryId);

                IsActiveSnackbar = false;

                count.Reset();

                count.Start();

                count.SetTime(2000);

                ContentSnackbar = string.Format(Domain.Properties.Resources.Send_Sucess, Domain.Properties.Resources.Sent);

                IsActiveSnackbar = true;

                count.CountDownFinished = () => { IsActiveSnackbar = false; };

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                irModel.IsSending = false;
            }
        }

        private async void Save(object obj)
        {
            try
            {
                await _irRepository.Save(SelectedProjectModel.SelectedGateway, IRModel, true);

                if (await _internetService.IsInternet())
                {
                    await _irRepository.Insert(SelectedProjectModel, IRModel);

                    if (SelectedProjectModel.SelectedGateway.IRsCloud.FirstOrDefault(x => x.MemoryId == IRModel.MemoryId) is IRModel temp)
                    {
                        _ = SelectedProjectModel.SelectedGateway.IRsCloud.Remove(temp);
                    }

                    using IRModel ir = new();

                    IRModel.CopyPropertiesTo(ir);

                    SelectedProjectModel.SelectedGateway.IRsCloud.Add(ir);
                }

                IRModel = new IRModel();

                SelectedProjectModel.SelectedGateway.SelectedIndexIR = -1;

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SelectedIR(object obj)
        {
            try
            {
                if (obj is not IRModel irModel)
                {
                    return;
                }

                IRModel = JsonConvert.DeserializeObject<IRModel>(JsonConvert.SerializeObject(irModel));
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void SendToGateway(object obj)
        {
            try
            {
                if (obj is not IRModel irModel)
                {
                    return;
                }

                using ReplaySubject<string> replay = new();

                //  await _irRepository.SaveAsync(SelectedDevice, irModel, replay);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SettingsServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsTerminalEnable))
            {
                IsTerminalEnable = ((SettingsService)sender).IsTerminalEnable;
            }
        }

        #endregion Method
    }
}