using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.ViewModel.Project;
using Parse;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Login
{
    public class LoginViewModel : ProjectViewModelBase
    {
        private string _password;

        public LoginViewModel(IFrameNavigationService navigationService,
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
                                 INetworkService networkService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            LoginCommand = new RelayCommand(Login);

            ResetPasswordCommand = new RelayCommand(ResetPassword);

            PasswordChangedCommand = new RelayCommand<object>((obj) => PasswordChanged(obj));

            LoadedCommand = new RelayCommand(Loaded);

            UnloadedCommand = new RelayCommand<object>((obj) => Unloaded(obj));

            _networkService = networkService;
        }

        public ICommand LoginCommand { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand PasswordChangedCommand { get; set; }
        public ICommand ResetPasswordCommand { get; set; }

        private void Loaded()
        {
        }

        private async void Login()
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Email_field_cannot_be_empty,
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());

                    return;
                }

                if (string.IsNullOrEmpty(Password))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Password_field_cannot_be_empty,
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());

                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     isProgressBar: true);

                if (!await _networkService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         cancel: async () => await CloseDialog(),
                                         textButtonCancel: Domain.Properties.Resources.Close);
                    return;
                }

                count.SetTime(4000);

                count.Start();

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Wait__connecting_to_the_server_,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog(),
                                     isProgressBar: true);

                ParseUser parseUser = await _loginRepository.Login(Email, Password);

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Wait__connecting_to_the_server_);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                IsOpenDialogHost = false;

                _navigationService.NavigateTo(AppConstants.DASHBOARD);
            }
            catch (HttpRequestException)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: Domain.Properties.Resources.No_Network,
                                     ok: async () => await CloseDialog());
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("Invalid username/password."))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Invalid_email_or_password,
                                         ok: async () => await CloseDialog());
                    return;
                }
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: ex.Message,
                                     ok: async () => await CloseDialog());
            }
        }

        private void PasswordChanged(object obj)
        {
            PasswordBox passwordBox = (PasswordBox)obj;
            Password = passwordBox.Password;
        }

        private readonly CountDownTimer count = new();
        private readonly INetworkService _networkService;

        private async void ResetPassword()
        {
            try
            {
                if (_parseService.IsSendingToCloud)
                {
                    return;
                }

                if (string.IsNullOrEmpty(Email))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Email,
                                         message: Domain.Properties.Resources.Email_field_cannot_be_empty,
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());

                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     isProgressBar: true);

                if (!await _networkService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         cancel: async () => await CloseDialog(),
                                         textButtonCancel: Domain.Properties.Resources.Close);
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Reset,
                                     message: $"{Domain.Properties.Resources.Email_Sent_To} {Email}",
                                     cancel: async () => await CloseDialog());

                count.SetTime(5000);

                count.Start();

                await _loginRepository.ResetPassword(Email);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                count.Restart();

                count.Start();

                OpenCustomMessageBox(header: Domain.Properties.Resources.Reset,
                                     message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Instructions_Sent_To_Email, Email));

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}