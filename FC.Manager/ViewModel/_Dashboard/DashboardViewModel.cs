using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model.Configution;
using FC.Domain.Model.DrawerHost;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.View.Terminal;
using FC.Manager.ViewModel.Project;
using MaterialDesignThemes.Wpf;
using Parse;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FC.Manager.ViewModel._Dashboard
{
    public class DashboardViewModel : ProjectViewModelBase
    {
        private readonly INetworkService _networkService;

        private string _appName = Domain.Properties.Resources.App_Name;

        private object _backgroundSnackbar;

        private ConfigurationApp _configurationApp;

        private string _ConnectedSsid;

        private string _currentPageKey;

        private string _currentUserEmail;

        private Uri _currentUserPicture;

        private bool _isCheckedHamburgerDashboard;

        private string _messageSnackbar;

        private ParseUser _parseUser;

        private int _selectedPageIndex;

        private CancellationTokenSource cancellationTokenSource;

        private int lastSelectedPageIndex;

        private string _DefaultGateway;

        public DashboardViewModel(IFrameNavigationService navigationService,
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
                                 INetworkService internetService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            NetworkChange.NetworkAddressChanged += new

            NetworkAddressChangedEventHandler(AddressChangedCallback);

            _networkService = internetService;

            _navigationService.PropertyChanged += NavigationServicePropertyChanged;

            if (configurationRepository != null)
            {
                ConfigurationApp = configurationRepository.ConfigurationApp;
            }

            SelectedPageCommand = new RelayCommand<object>((obj) => SelectedPage(obj));

            LoadedCommand = new RelayCommand<object>(Loaded);

            GoToInfoUserCommand = new RelayCommand(GoToInfoUser);

            HamburgerToggleButtonCommand = new RelayCommand<object>((obj) => HamburgerToggleButton(obj));

            ParseUser = ParseUser.CurrentUser;

            IsCheckedHamburgerDashboard = Properties.Settings.Default.isCheckedHamburgerDashboard;
        }

        private void AddressChangedCallback(object sender, EventArgs e)
        {
            GetSSID();
        }

        public string AppName
        {
            get => _appName;
            set
            {
                if (Equals(_appName, value))
                {
                    return;
                }
                _ = SetProperty(ref _appName, value);
            }
        }

        public object BackgroundSnackbar
        {
            get => _backgroundSnackbar;
            set
            {
                if (Equals(_backgroundSnackbar, value))
                {
                    return;
                }
                _ = SetProperty(ref _backgroundSnackbar, value);
            }
        }

        public ConfigurationApp ConfigurationApp
        {
            get => _configurationApp;
            set => SetProperty(ref _configurationApp, value);
        }

        public string DefaultGateway
        {
            get => _DefaultGateway;
            set => SetProperty(ref _DefaultGateway, value);
        }

        public string ConnectedSsid
        {
            get => _ConnectedSsid;
            set => SetProperty(ref _ConnectedSsid, value);
        }

        public string CurrentPageKey
        {
            get => _currentPageKey;
            set => SetProperty(ref _currentPageKey, value);
        }

        public string CurrentUserEmail
        {
            get => _currentUserEmail;
            set => SetProperty(ref _currentUserEmail, value);
        }

        public Uri CurrentUserPicture
        {
            get => _currentUserPicture;
            set => SetProperty(ref _currentUserPicture, value);
        }

        public Frame Frame { get; private set; }

        public ICommand GoToInfoUserCommand { get; set; }

        public ICommand HamburgerToggleButtonCommand { get; set; }

        public bool IsCheckedHamburgerDashboard
        {
            get => _isCheckedHamburgerDashboard;
            set
            {
                _ = SetProperty(ref _isCheckedHamburgerDashboard, value);
                Properties.Settings.Default.isCheckedHamburgerDashboard = value;
                Properties.Settings.Default.Save();
            }
        }

        public ObservableCollection<DrawerHostItem> Items { get; } = new ObservableCollection<DrawerHostItem> {
            new DrawerHostItem
            {
                Kind = PackIconKind.FileDocumentOutline,
                Name = Domain.Properties.Resources.Projects,
                Source = AppConstants.PROJECTPAGE,
                EnumName = EnumNamePage.Project
            },
            new DrawerHostItem
            {
                Kind = PackIconKind.Terminal,
                Name = Domain.Properties.Resources.Terminal,
                EnumName = EnumNamePage.Terminal
            },
            new DrawerHostItem
            {
                Kind = PackIconKind.Settings,
                Name = Domain.Properties.Resources.Configuration,
                Source = AppConstants.CONFIGURATIONPAGE,
                EnumName = EnumNamePage.Configuration
            },
            new DrawerHostItem
            {
                Kind = PackIconKind.ExitToApp,
                Name = Domain.Properties.Resources.Exit,
                EnumName = EnumNamePage.ExitToApp
            },
        };

        public string MessageSnackbar
        {
            get => _messageSnackbar;
            set
            {
                if (Equals(_messageSnackbar, value))
                {
                    return;
                }
                _ = SetProperty(ref _messageSnackbar, value);
            }
        }

        public ParseUser ParseUser
        {
            get => _parseUser;
            set => SetProperty(ref _parseUser, value);
        }

        public ICommand SelectedPageCommand { get; set; }

        public int SelectedPageIndex
        {
            get => _selectedPageIndex;
            set
            {
                lastSelectedPageIndex = _selectedPageIndex;
                _ = SetProperty(ref _selectedPageIndex, value);
            }
        }

        private static void OpenTerminal()
        {
            foreach (object item in System.Windows.Application.Current.Windows)
            {
                if (item is TerminalWindow terminalWindow)
                {
                    _ = terminalWindow.Activate();
                    return;
                }
            }

            TerminalWindow termWindow = new();

            termWindow.Show();
        }

        private void CloseSnackbar()
        {
            IsActiveSnackbar = false;
        }

        private void GetSSID()
        {
            try
            {
                Process process = new()
                {
                    StartInfo =
                    {
                        FileName = "netsh.exe",
                        Arguments = "wlan show interfaces",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                _ = process.Start();

                string output = process.StandardOutput.ReadToEnd();

                string line = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(l => l.Contains("SSID") && !l.Contains("BSSID"));

                if (line == null)
                {
                    ConnectedSsid = string.Empty;
                    return;
                }

                string ssid = line.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[1].TrimStart();

                ConnectedSsid = ssid;
                DefaultGateway = _networkService.GetDefaultGateway().ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void GetUserInfo()
        {
            CurrentUserEmail = ParseUser.CurrentUser?.Email;

            string picture = "pack://application:,,/FC.Domain;Component/Assets/User/user.png";

            if (ParseUser.CurrentUser.ContainsKey("picture"))
            {
                picture = ParseUser.CurrentUser.Get<string>("picture") ?? "pack://application:,,/FC.Domain;Component/Assets/User/user.png";
            }

            CurrentUserPicture = new Uri(picture);
        }

        private async void GoToInfoUser()
        {
            if (!await _networkService.IsInternet())
            {
                ShowSnackbarMessage(content: Domain.Properties.Resources.Without_Internet_Connection,
                                    background: "Red",
                                    actionContent: Domain.Properties.Resources._Close,
                                    action: () => CloseSnackbar());
                return;
            }

            NavigationToRoot(AppConstants.ACCOUNTSETTING);
        }

        private void HamburgerToggleButton(object obj)
        {
            if (obj is object[])
            {
                object[] objects = obj as object[];
                ToggleButton toggleButton = objects[0] as ToggleButton;
                ColumnDefinition columnDefinition = objects[1] as ColumnDefinition;
                bool isChecked = (bool)toggleButton.IsChecked;
                columnDefinition.Width = isChecked ? new GridLength(38) : new GridLength(240);
                AppName = isChecked ? "FCC" : Domain.Properties.Resources.App_Name;
            }
        }

        private async void IsSoftwareAdministrator()
        {
            try
            {
                DrawerHostItem itemSoftwareAdmin = Items.FirstOrDefault(x => x?.Name == Domain.Properties.Resources.Software_administrator);

                if (itemSoftwareAdmin is null)
                {
                    return;
                }

                itemSoftwareAdmin.IsVisible = false;

                cancellationTokenSource = new CancellationTokenSource();

                itemSoftwareAdmin.IsVisible = await _userRepository.IsSoftwareAdministrator(cancellationTokenSource);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

        private void Loaded(object obj)
        {
            if (obj is not object[])
            {
                return;
            }

            if (App.IsSoftwareAdministrator)
            {
                IsSoftwareAdministrator();
            }

            GetUserInfo();

            GetSSID();

            object[] objects = obj as object[];

            Frame = objects[0] as Frame;

            ColumnDefinition columnDefinition = objects[1] as ColumnDefinition;

            bool isChecked = Properties.Settings.Default.isCheckedHamburgerDashboard;

            columnDefinition.Width = isChecked ? new GridLength(38) : new GridLength(240);

            AppName = isChecked ? "FCC" : Domain.Properties.Resources.App_Name;

            NavigateToProject();
        }

        private readonly CountDownTimer count = new();

        private async Task Logout()
        {
            try
            {
                count.SetTime(2000);

                count.Start();

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Logging_Out);

                await _loginRepository.LogOutAsync();

                await Task.Run(() => { while (count.IsRunnign) {; } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void NavigateTo(string page)
        {
            _navigationService.CustomFrame = Frame;
            _navigationService.NavigateTo(page);
        }

        private void NavigateToProject()
        {
            NavigateTo(Domain.Properties.Resources.Project);

            SelectedPageIndex = 0;
        }

        private void NavigationServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FrameNavigationService.CurrentPageKey) || sender is not null)
            {
                CurrentPageKey = ((FrameNavigationService)sender).CurrentPageKey;
            }
        }

        private void NavigationToRoot(string page)
        {
            _navigationService.CustomFrame = null;
            _navigationService.NavigateTo(page);
        }

        private async void SelectedPage(object obj)
        {
            if (obj is null)
            {
                return;
            }

            using DrawerHostItem item = obj as DrawerHostItem;

            switch (item.EnumName)
            {
                case EnumNamePage.Project:
                    NavigateTo(Domain.Properties.Resources.Project);
                    break;

                case EnumNamePage.Gateway:
                    NavigateTo(AppConstants.GATEWAY);
                    break;

                case EnumNamePage.Configuration:
                    NavigateTo(AppConstants.CONFIGURATION);
                    break;

                case EnumNamePage.License_Manager:
                    NavigateTo(AppConstants.LICENSEMANAGER);
                    break;

                case EnumNamePage.SoftwareAdministrator:
                    NavigateTo(AppConstants.SOFTWAREADMINISTRATOR);
                    break;

                case EnumNamePage.ExitToApp:
                    await Logout();
                    NavigationToRoot(AppConstants.LOGIN);
                    break;

                case EnumNamePage.Terminal:
                    SelectedPageIndex = lastSelectedPageIndex;
                    OpenTerminal();
                    break;
            }
        }

        private void ShowSnackbarMessage(bool IsActive = true, string actionContent = null, object content = null, Action action = null, object background = null)
        {
            IsActiveSnackbar = IsActive;
            ActionContentSnackbar = actionContent;
            ContentSnackbar = content;
            BackgroundSnackbar = background ?? "Black";
            this.action = action;
        }
    }
}