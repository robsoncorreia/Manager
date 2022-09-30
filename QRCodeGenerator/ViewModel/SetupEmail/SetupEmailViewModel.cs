using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using QRCodeGenerator.Model;
using QRCodeGenerator.Repository;
using QRCodeGenerator.Service;
using QRCodeGenerator.Util;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace QRCodeGenerator.ViewModel.SetupEmail
{
    public class SetupEmailViewModel : FlexViewModelBase
    {
        private readonly IEmailService _emailService;
        private readonly ILocalDBRepository _localDBRepository;
        private readonly IMailRepository _smtpRepository;
        private BaseTheme _baseTheme = (BaseTheme)Properties.Settings.Default.baseTheme;
        private readonly IColorService _colorService;
        private string _foreground = Properties.Settings.Default.foreground;
        private bool _isOpenHelpDialogHost;
        private Email _selectedEmail;
        private int _selectedIndexBodyType;

        public SetupEmailViewModel(ITaskService taskService,
                                   IDialogService dialogService,
                                   IEmailService emailService,
                                   IMailRepository smtpRepository,
                                   ILocalDBRepository localDBRepository,
                                   IColorService colorService) : base(taskService, dialogService)
        {
            SaveCommand = new RelayCommand<object>(Save);

            LoadedCommand = new RelayCommand<object>(Loaded);

            GoToHelpPageCommand = new RelayCommand<object>(GoToHelpPage);

            CloseHelpDialogHostCommand = new RelayCommand<object>(CloseHelpDialogHost);

            OpenHelpDialogHostCommand = new RelayCommand<object>(OpenHelpDialogHost);

            _emailService = emailService;

            _smtpRepository = smtpRepository;

            _localDBRepository = localDBRepository;

            SMTPMailServers = new ObservableCollection<SMTPMailServer>(_smtpRepository.GetAll());

            SelectedEmail = new Email();

            _colorService = colorService;

            _colorService.PropertyChanged += ColorServicePropertyChanged;
        }

        public BaseTheme BaseTheme
        {
            get => _baseTheme;
            set => Set(ref _baseTheme, value);
        }

        public ICommand CloseHelpDialogHostCommand { get; set; }

        public string Foreground
        {
            get => _foreground;
            set => Set(ref _foreground, value);
        }

        public ICommand GoToHelpPageCommand { get; set; }

        public bool IsOpenHelpDialogHost
        {
            get => _isOpenHelpDialogHost;
            set => Set(ref _isOpenHelpDialogHost, value);
        }

        public ICommand LoadedCommand { get; set; }

        public ICommand OpenHelpDialogHostCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public Email SelectedEmail
        {
            get => _selectedEmail;
            set => Set(ref _selectedEmail, value);
        }

        public int SelectedIndexBodyType
        {
            get => _selectedIndexBodyType;
            set => Set(ref _selectedIndexBodyType, value);
        }

        public ObservableCollection<SMTPMailServer> SMTPMailServers { get; set; }

        public string TextHTMLExample { get; set; } = "<span style='font-size: 12pt; color: blue ;'>Robson Correia</span>";

        private void CloseHelpDialogHost(object obj)
        {
            IsOpenHelpDialogHost = false;
        }

        private void ColorServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_colorService.Foreground))
            {
                Foreground = ((ColorService)sender).Foreground;
            }
            if (e.PropertyName == nameof(_colorService.BaseTheme))
            {
                BaseTheme = ((ColorService)sender).BaseTheme;
            }
        }

        private void GoToHelpPage(object obj)
        {
            if (SelectedEmail.SelectedIndexSMTPMailServer < 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(SMTPMailServers[SelectedEmail.SelectedIndexSMTPMailServer].HelpPage))
            {
                return;
            }

            _ = System.Diagnostics.Process.Start(SMTPMailServers[SelectedEmail.SelectedIndexSMTPMailServer].HelpPage);
        }

        private void Loaded(object obj)
        {
            if (!(_emailService.SelectedEmail is Email))
            {
                return;
            }

            SelectedEmail = _emailService.SelectedEmail;

            SelectedIndexBodyType = SelectedEmail.IsBodyHtml ? 1 : 0;

            if (SelectedEmail.SMTPMailServer.Provider == AppConstants.Custom)
            {
                _ = SMTPMailServers.Remove(SMTPMailServers.FirstOrDefault(x => x.Provider == SelectedEmail.SMTPMailServer.Provider));
                SMTPMailServers.Add(SelectedEmail.SMTPMailServer);
            }

            SelectedEmail.SelectedIndexSMTPMailServer = SMTPMailServers.IndexOf(SMTPMailServers.FirstOrDefault(x => x.Provider == SelectedEmail.SMTPMailServer.Provider));
        }

        private void OpenHelpDialogHost(object obj)
        {
            IsOpenHelpDialogHost = true;
        }

        private async void Save(object obj)
        {
            try
            {
                if (SelectedEmail.SelectedIndexSMTPMailServer < 0)
                {
                    return;
                }

                SelectedEmail.SMTPMailServer = SMTPMailServers[SelectedEmail.SelectedIndexSMTPMailServer];

                switch ((BodyTypeEnum)SelectedIndexBodyType)
                {
                    case BodyTypeEnum.Text:
                        SelectedEmail.IsBodyHtml = false;
                        break;

                    case BodyTypeEnum.HTML:
                        SelectedEmail.IsBodyHtml = true;
                        break;
                }

                _ = _localDBRepository.Upsert(SelectedEmail);

                OpenCustomMessageBox(header: Properties.Resources.Well_Done,
                                     message: Properties.Resources.Email_settings_saved_successfully,
                                     ok: async () => await CloseDialog());

                await CloseDialog(2000);
            }
            catch (System.Exception ex)
            {
                await ShowError(ex);
            }
        }
    }
}