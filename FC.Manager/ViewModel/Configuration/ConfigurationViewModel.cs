using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Configution;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.ViewModel.Project;
using Microsoft.Win32;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Configuration
{
    public class ConfigurationViewModel : ProjectViewModelBase
    {
        private readonly IIfThenRepository _ifThenRepository;
        private readonly ILanguageRepository _languageRepository;
        private readonly ISettingsService _settingsService;
        private ConfigurationApp _configurationApp;

        private int _SelectedIndexGateway;
        private int _selectedIndexLanguages;

        private bool isTerminalEnable = Domain.Properties.Settings.Default.enableTerminal;
        private readonly INetworkService _networkService;
        private readonly IGoogleApiService _googleApiService;
        private readonly IGoogleDriveService _googleDriveService;

        private int _delay = Domain.Properties.Settings.Default.delayTimeBetweenCommand;
        private bool _IsFindGatewayBroadcast = Domain.Properties.Settings.Default.isFindGatewayBroadcast;

        public int Delay
        {
            get => _delay;
            set
            {
                if (Equals(_delay, value))
                {
                    return;
                }

                if (int.MaxValue == value)
                {
                    return;
                }

                _ = SetProperty(ref _delay, value);

                Domain.Properties.Settings.Default.delayTimeBetweenCommand = value;
            }
        }

        public ConfigurationViewModel(IFrameNavigationService navigationService,
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
                                 ILanguageRepository languageRepository,
                                 ISettingsService settingsService,
                                 IGoogleApiService googleApiService,
                                 IGoogleDriveService googleDriveService,
                                 INetworkService networkService,
                                 IIfThenRepository ifThenRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _networkService = networkService;

            _googleApiService = googleApiService;

            _googleDriveService = googleDriveService;

            _ifThenRepository = ifThenRepository;

            _settingsService = settingsService;

            _languageRepository = languageRepository;

            ConfigurationApp = _configurationRepository.ConfigurationApp;

            ConfigurationApp.ParseUserEmail = ParseUser.CurrentUser.Email;

            UpdateConfigurationAPPCommand = new RelayCommand<object>(UpdateConfigurationAPP);

            HyperlinkCommand = new RelayCommand<object>(Hyperlink);

            SettingsRestoreCommand = new RelayCommand<object>(SettingsRestore);

            ChangeLanguageCommand = new RelayCommand<object>(ChangeLanguage);

            SetTerminalEnableCommand = new RelayCommand<object>(SetTerminalEnable);

            SearchGatewaysCommand = new RelayCommand<object>(SearchGateways);

            DeleteAllCommand = new RelayCommand<object>(DeleteAll);

            OpenChangelogCommand = new RelayCommand<object>(OpenChangelog);

            Gateways = _gatewayService?.Gateways; ;

            Languages = _languageRepository.Languages;

            SelectedIndexLanguages = Domain.Properties.Settings.Default.language;
        }

        public ICommand ChangeLanguageCommand { get; set; }

        public ConfigurationApp ConfigurationApp
        {
            get => _configurationApp;
            set => SetProperty(ref _configurationApp, value);
        }

        public ICommand DeleteAllCommand { get; set; }

        public ObservableCollection<GatewayModel> Gateways { get; set; }

        public ICommand HyperlinkCommand { get; set; }

        public bool IsTerminalEnable
        {
            get => isTerminalEnable;
            set
            {
                _ = SetProperty(ref isTerminalEnable, value);
                _settingsService.IsTerminalEnable = value;
            }
        }
        public bool IsFindGatewayBroadcast
        {
            get => _IsFindGatewayBroadcast;
            set
            {
                _ = SetProperty(ref _IsFindGatewayBroadcast, value);
                Domain.Properties.Settings.Default.isFindGatewayBroadcast = value;
                Domain.Properties.Settings.Default.Save();
            }
        }

        public IList<LanguageModel> Languages { get; set; }

        public ICommand OpenChangelogCommand { get; set; }

        public ICommand SearchGatewaysCommand { get; set; }

        public int SelectedIndexGateway
        {
            get => _SelectedIndexGateway;
            set => SetProperty(ref _SelectedIndexGateway, value);
        }

        public int SelectedIndexLanguages
        {
            get => _selectedIndexLanguages;
            set => SetProperty(ref _selectedIndexLanguages, value);
        }

        public ICommand SetTerminalEnableCommand { get; set; }

        public ICommand SettingsRestoreCommand { get; set; }

        public ICommand UpdateConfigurationAPPCommand { get; set; }

        //private async void AddCurrentUserToRoleLicenseManager()
        //{
        //    ParseRole moderators = await (from role in ParseRole.Query
        //                                  where role.Name == AppConstants.LICENSEMANAGERS
        //                                  select role).FirstAsync();

        //    moderators.Users.Add(ParseUser.CurrentUser);

        //    await moderators.SaveAsync();
        //}

        private void ChangeLanguage(object obj)
        {
            _languageRepository.ChangeLanguage(SelectedIndexLanguages);

            OpenCustomMessageBox(header: FC.Domain.Properties.Resources.Close,
                                 message: FC.Domain.Properties.Resources.To_make_the_changes_effective__it_is_necessary_to_close_and_reopen_the_software__Do_you_want_to_close_the_software_,
                                 custom: () => Close(),
                                 textButtonCustom: FC.Domain.Properties.Resources.Yes,
                                 textButtonCancel: FC.Domain.Properties.Resources.No,
                                 cancel: async () => await CloseDialog());
        }

        private void Close()
        {
            Application.Current.Shutdown();
        }

        private async void DeleteAll(object obj)
        {
            try
            {
                if (Gateways.ElementAt(SelectedIndexGateway) is not GatewayModel selectedGateway)
                {
                    return;
                }
                //todo timeNow
                DateTime timeNow = DateTime.Now.AddSeconds(2);

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Excluding,
                                     isProgressBar: true,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());

                await _ifThenRepository.DeleteAllConditionals(selectedGateway);
                await _ifThenRepository.DeleteAllRules(selectedGateway);

                if (DateTime.Now < timeNow)
                {
                    int delay = (int)Math.Round((timeNow - DateTime.Now).TotalSeconds) * 1000;
                    await Task.Delay(delay);
                }

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Hyperlink(object obj)
        {
            try
            {
                _ = Process.Start("http://www.flexautomation.com.br/");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void OpenChangelog(object obj)
        {
            try
            {
                if (!await _networkService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         textButtomOk: Domain.Properties.Resources._Close,
                                         ok: async () => await CloseDialog());

                    return;
                }

                _parseService.IsSendingToCloud = true;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Downloading_change_history,
                                     isProgressBar: true);

                Google.Apis.Drive.v3.DriveService service = _googleApiService.LoginServiceAccountCredential();

                IList<Google.Apis.Drive.v3.Data.File> filesInDir = await _googleDriveService.GetFiles(service);

                if (!filesInDir.Any())
                {
                    await CloseDialog();
                    return;
                }

                Google.Apis.Drive.v3.Data.File file = filesInDir.FirstOrDefault(x => x.Name.Contains(".pdf"));

                if (file is null)
                {
                    await CloseDialog();
                    return;
                }

                string path = Path.GetTempPath() + file.Name;

                using (FileStream fileStream = new(path, FileMode.Create))
                {
                    Google.Apis.Drive.v3.FilesResource.GetRequest request = service.Files.Get(file.Id);

                    _ = await request.DownloadAsync(fileStream);
                }

                byte[] bytes = System.IO.File.ReadAllBytes(path);

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = $"{Domain.Properties.Resources.PDF_File} (*.pdf)|*.pdf",
                    FileName = $"{Properties.Resources.AppName}{Domain.Properties.Resources.Changelog.ToUpper()}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, bytes);
                }

                action = () => OpenFile(saveFileDialog);

                ActionContentSnackbar = Domain.Properties.Resources.Open;

                ContentSnackbar = Domain.Properties.Resources.File_saved;

                count.Reset();

                count.SetTime(4000);

                count.Start();

                count.CountDownFinished = () =>
                {
                    IsActiveSnackbar = false;
                    ActionContentSnackbar = null;
                };

                IsActiveSnackbar = true;

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private readonly CountDownTimer count = new();

        private async void SearchGateways(object obj = null)
        {
            ReplaySubject<string> replay = new();

            ReplaySubject<int> replayProgressBarValue = new();

            try
            {
                SelectedIndexGateway = -1;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Finding_Gateways_Network,
                                     rx: replay,
                                     textButtonCancel: Domain.Properties.Resources.Stop_search,
                                     cancel: async () => await CloseDialog(),
                                     isRXProgressBarVisibible: true,
                                     rxProgressBarValue: replayProgressBarValue);

                await _gatewayRepository.GetGateways(GatewayModelEnum.ANY,
                                                     address: IPAddress.Broadcast,
                                                     replay: replay,
                                                     rxProgressBarValue: replayProgressBarValue);

                if (!Gateways.Any())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Finding_Gateways_Network,
                                         message: Domain.Properties.Resources.Could_not_find_any_gateway_on_the_network__Search_again_,
                                         textButtonCustom: Domain.Properties.Resources._Try_again,
                                         custom: () => SearchGateways(),
                                         textButtonCancel: Domain.Properties.Resources._Close,
                                         cancel: async () => await CloseDialog());

                    return;
                }
                await CloseDialog();
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(OperationCanceledException))
                {
                    await CloseDialog();
                    return;
                }
                ShowError(ex);
            }
            finally
            {
                replay.Dispose();

                replayProgressBarValue.Dispose();
            }
        }

        private void SetTerminalEnable(object obj)
        {
            _configurationRepository.SetTerminalEnable(IsTerminalEnable);
        }

        private void SettingsRestore(object obj)
        {
            ConfigurationApp.Reset();
            UpdateConfigurationAPP(obj);
        }

        private void UpdateConfigurationAPP(object obj)
        {
            _ = _configurationRepository.Update(ConfigurationApp);
        }

#if DEBUG
        private static readonly string ENV = Properties.Resources.DEV;
#else
        private readonly static string ENV = Properties.Resources.PROD;
#endif
        public string Version { get; } = $"{Properties.Resources.AppVersion}.{ENV}";

    }
}