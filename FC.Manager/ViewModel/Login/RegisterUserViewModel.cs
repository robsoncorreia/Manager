using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Manager.View.Login;
using FC.Manager.ViewModel.Project;
using Google.Apis.Drive.v3;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Login
{
    public class RegisterUserViewModel : ProjectViewModelBase
    {
        private string _password;
        private string _userName;

        public RegisterUserViewModel(IFrameNavigationService navigationService,
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
                                 INetworkService networkService,
                                 IGoogleApiService googleApiService,
                                 IGoogleDriveService googleDriveService,
                                 ILoginRepository loginRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _networkService = networkService;

            _googleDriveService = googleDriveService;

            _googleApiService = googleApiService;

            CancelRegisterCommand = new RelayCommand<object>(CancelRegister);

            RegisterUserCommand = new RelayCommand<object>(RegisterUser);

            TermsConditionsHiperLinkCommand = new RelayCommand<object>(TermsConditionsHiperLink);
        }

        private readonly CountDownTimer count = new();

        private async void TermsConditionsHiperLink(object obj)
        {
            try
            {
                _parseService.IsSendingToCloud = true;

                if (!await _networkService.IsInternet())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Without_Internet_Connection,
                                         textButtomOk: Domain.Properties.Resources._Close,
                                         ok: async () => await CloseDialog());

                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Downloading,
                                     isProgressBar: true);

                DriveService service = _googleApiService.LoginServiceAccountCredential();

                byte[] bytes = await _googleDriveService.GetFiles(driveService: service, path: "TermSoftware", fileName: "Term.pdf");

                if (bytes is null)
                {
                    await CloseDialog();
                    return;
                }

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = $"{Domain.Properties.Resources.PDF_File} (*.pdf)|*.pdf",
                    FileName = $"{Domain.Properties.Resources.Terms_of_use}"
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

        private readonly INetworkService _networkService;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly IGoogleApiService _googleApiService;

        public ICommand CancelRegisterCommand { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public ICommand RegisterUserCommand { get; set; }
        public ICommand TermsConditionsHiperLinkCommand { get; set; }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private void CancelRegister(object obj)
        {
            RegisterUserPage registerUserPage = (RegisterUserPage)obj;
            registerUserPage.NavigationService.GoBack();
        }

        private async void RegisterUser(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Signing_up,
                                     cancel: async () => await CloseDialog());

                await _loginRepository.RegisterUser(UserName, Password, Email);

                IsOpenDialogHost = false;

                _navigationService.NavigateTo(AppConstants.DASHBOARD);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}