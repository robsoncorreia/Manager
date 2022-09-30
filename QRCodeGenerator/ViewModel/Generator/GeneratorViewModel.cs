using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using QRCodeGenerator.Model;
using QRCodeGenerator.Repository;
using QRCodeGenerator.Service;
using QRCodeGenerator.Util;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QRCodeGenerator.ViewModel.Generator
{
    public class GeneratorViewModel : FlexViewModelBase
    {
        private readonly IColorService _colorService;
        private readonly IEmailService _emailService;
        private readonly ILanguageRepository _languageRepository;
        private readonly ILocalDBRepository _localDBRepository;
        private readonly IQRCodeRepository _qRCodeRepository;
        private BaseTheme _baseTheme = (BaseTheme)Properties.Settings.Default.baseTheme;
        private string _body;
        private string _emails;
        private string _foreground = Properties.Settings.Default.foreground;
        private string _from;
        private ImageSource _imageSource;
        private bool _is24Hours;
        private bool _isConfiguredEmail;
        private bool _isOpenEmailDialogHost;
        private Email _selectedEmail;
        private DateTime _selectedEndDate;
        private DateTime _selectedEndTime;
        private int _selectedIndexTransitioner;
        private DateTime _selectedStartDate;
        private DateTime _selectedStartTime;
        private string _subject;
        private string _text;
        private string _comment;

        public GeneratorViewModel(ITaskService taskService,
                                  IDialogService dialogService,
                                  ILanguageRepository languageRepository,
                                  IEmailService emailService,
                                  IQRCodeRepository qRCodeRepository,
                                  ILocalDBRepository localDBRepository,
                                  IColorService colorService) : base(taskService, dialogService)
        {
            _colorService = colorService;

            _dialogService = dialogService;

            _languageRepository = languageRepository;

            _emailService = emailService;

            _qRCodeRepository = qRCodeRepository;

            _localDBRepository = localDBRepository;

            _colorService.PropertyChanged += ColorServicePropertyChanged;

            LoadedCommand = new RelayCommand<object>(Loaded);

            SendToEmailCommand = new RelayCommand<object>(SendToEmail);

            GenerateQRCodeCommand = new RelayCommand<object>(GenerateQRCode);

            ExportImageCommand = new RelayCommand<object>(ExportImage);

            OpenEmailDialogHostCommand = new RelayCommand<object>(OpenEmailDialogHost);

            ShareByEmailCommand = new RelayCommand<object>(ShareByEmail);

            RemoveEmailCommand = new RelayCommand<string>(RemoveEmail);

            CloseEmailDialogHostCommand = new RelayCommand<object>(CloseEmailDialogHost);

            PrintCommand = new RelayCommand<object>(Print);

            CopyImageCommand = new RelayCommand<object>(CopyImage);

            AddEmailsCommand = new RelayCommand<object>(AddEmails);

            DeleteAllCommand = new RelayCommand<object>(DeleteAll);

            To = new ObservableCollection<string>();

            SelectedEmail = new Email();

            GenerateQRCode();

            ChangeTransitioner();
        }

        private void DeleteAll(object obj)
        {
            if (!To.Any())
            {
                return;
            }

            To.Clear();
        }

        private void RemoveEmail(string email)
        {
            _ = To.Remove(email);
        }

        public ICommand AddEmailsCommand { get; set; }
        public ICommand DeleteAllCommand { get; set; }

        public BaseTheme BaseTheme
        {
            get => _baseTheme;
            set => Set(ref _baseTheme, value);
        }

        public string Body
        {
            get => _body;
            set => Set(ref _body, value);
        }

        public ICommand CloseEmailDialogHostCommand { get; set; }
        public ICommand CopyImageCommand { get; set; }

        public string Emails
        {
            get => _emails;
            set => Set(ref _emails, value);
        }

        public ICommand ExportImageCommand { get; set; }

        public string Foreground
        {
            get => _foreground;
            set => Set(ref _foreground, value);
        }

        public string Comment
        {
            get => _comment;
            set
            {
                _ = Set(ref _comment, value);
                GenerateQRCode();
            }
        }

        public string From
        {
            get => _from;
            set => Set(ref _from, value);
        }

        public ICommand GenerateQRCodeCommand { get; set; }

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => Set(ref _imageSource, value);
        }

        public bool Is24Hours
        {
            get => _is24Hours;
            set => Set(ref _is24Hours, value);
        }

        public bool IsConfiguredEmail
        {
            get => _isConfiguredEmail;
            set => Set(ref _isConfiguredEmail, value);
        }

        public bool IsOpenEmailDialogHost
        {
            get => _isOpenEmailDialogHost;
            set
            {
                _ = Set(ref _isOpenEmailDialogHost, value);
                _dialogService.IsOpenDialogHost = value;
            }
        }

        public ICommand LoadedCommand { get; set; }
        public ICommand OpenEmailDialogHostCommand { get; set; }
        public ICommand PrintCommand { get; set; }

        public Email SelectedEmail
        {
            get => _selectedEmail;
            set => Set(ref _selectedEmail, value);
        }

        public DateTime SelectedEndDate
        {
            get => _selectedEndDate;
            set => Set(ref _selectedEndDate, value);
        }

        public DateTime SelectedEndTime
        {
            get => _selectedEndTime;
            set => Set(ref _selectedEndTime, value);
        }

        public int SelectedIndexTransitioner
        {
            get => _selectedIndexTransitioner;
            set => Set(ref _selectedIndexTransitioner, value);
        }

        public DateTime SelectedStartDate
        {
            get => _selectedStartDate;
            set => Set(ref _selectedStartDate, value);
        }

        public DateTime SelectedStartTime
        {
            get => _selectedStartTime;
            set => Set(ref _selectedStartTime, value);
        }

        public ICommand SendToEmailCommand { get; set; }
        public ICommand ShareByEmailCommand { get; set; }
        public ICommand RemoveEmailCommand { get; set; }

        public string Subject
        {
            get => _subject;
            set => Set(ref _subject, value);
        }

        public string Text
        {
            get => _text;
            set
            {
                _ = Set(ref _text, value);

                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                GenerateQRCode();
            }
        }

        public ObservableCollection<string> To { get; set; }

        public async void ExportImage(object obj)
        {
            try
            {
                _qRCodeRepository.ExportImage(GenerateText());
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void AddEmails(object obj)
        {
            string[] emails = Emails.Split(',');

            Emails = string.Empty;

            for (int i = 0; i < emails.Length; i++)
            {
                if (!emails[i].Replace(" ", "").IsEmailValid())
                {
                    continue;
                }

                if (To.Contains(emails[i]))
                {
                    continue;
                }

                To.Add(emails[i]);
            }
        }

        private async void ChangeTransitioner()
        {
            await Task.Delay(1000);

            SelectedIndexTransitioner = 1;

            await Task.Delay(1000);
        }

        private void CloseEmailDialogHost(object obj)
        {
            IsOpenEmailDialogHost = false;
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

        private async void CopyImage(object obj)
        {
            try
            {
                Clipboard.SetImage(_qRCodeRepository.GerarQRCode(178, 178, GenerateText()));

                OpenCustomMessageBox(header: Properties.Resources.Well_Done,
                                     message: Properties.Resources.Copied_image,
                                     cancel: async () => await CloseDialog());

                await CloseDialog(1500);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void GenerateQRCode(object obj = null)
        {
            try
            {
                ImageSource = _qRCodeRepository.GerarQRCode(178, 178, GenerateText());
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private string GenerateText()
        {
            Debug.WriteLine(Comment?.PadRight(20, '\0'));
            return $"@{SelectedStartDate:dd/MM/yyyy}|{SelectedStartTime:HH:mm}|{SelectedEndDate:dd/MM/yyyy}|{SelectedEndTime:HH:mm}|{Comment?.PadRight(20, ' ')}#";
        }

        private void GetIs24Hours()
        {
            Is24Hours = _languageRepository.GetIs24Hours();
        }

        private void GetSelectedEmail()
        {
            Email email = _localDBRepository.FindOne(SelectedEmail) as Email;

            if (!(email is Email))
            {
                IsConfiguredEmail = false;

                return;
            }

            IsConfiguredEmail = true;

            SelectedEmail = email;

            _emailService.SelectedEmail = SelectedEmail;
        }

        private void Loaded(object obj)
        {
            SelectedStartDate = DateTime.Now;

            SelectedStartTime = DateTime.Now;

            SelectedEndDate = DateTime.Now.AddHours(1);

            SelectedEndTime = DateTime.Now.AddHours(1);

            _ = GenerateText();

            GenerateQRCode();

            GetSelectedEmail();

            GetIs24Hours();
        }

        private void OpenEmailDialogHost(object obj)
        {
            IsOpenEmailDialogHost = true;
        }

        private async void Print(object obj)
        {
            try
            {
                _qRCodeRepository.Print(GenerateText());
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void SendToEmail(object obj)
        {
            try
            {
                string emails = string.Empty;

                for (int i = 0; i < To.Count; i++)
                {
                    emails += $"\n{To[i].Replace(" ", "")}";
                }

                OpenCustomMessageBox(header: Properties.Resources.Wait,
                                     message: string.Format(CultureInfo.CurrentCulture, Properties.Resources.Sending_email_to, emails)
                                     );

                await Task.Run(() =>
                {
                    MailMessage mail = new MailMessage
                    {
                        From = new MailAddress(SelectedEmail.Address)
                    };

                    foreach (string email in To)
                    {
                        mail.To.Add(email);
                    }

                    mail.Subject = SelectedEmail.Subject;

                    mail.Body = SelectedEmail.Body;

                    mail.IsBodyHtml = SelectedEmail.IsBodyHtml;

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();

                    Guid photoID = Guid.NewGuid();

                    string photolocation = $"{Path.GetTempPath()}/{photoID}.jpg";

                    encoder.Frames.Add(BitmapFrame.Create(_qRCodeRepository.GerarQRCode(178, 178, GenerateText())));

                    using (FileStream filestream = new FileStream(photolocation, FileMode.Create))
                    {
                        encoder.Save(filestream);
                    }

                    mail.Attachments.Add(new Attachment(photolocation));

                    using SmtpClient smtp = new SmtpClient(SelectedEmail.SMTPMailServer.SMTPSettings)
                    {
                        EnableSsl = SelectedEmail.SMTPMailServer.EnableSsl,
                        Port = SelectedEmail.SMTPMailServer.Port,
                        DeliveryMethod = SelectedEmail.SMTPMailServer.SmtpDeliveryMethod,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(SelectedEmail.Address, SelectedEmail.Password)
                    };

                    smtp.Send(mail);
                });

                _ = _localDBRepository.Upsert(new QRCodeModel
                {
                    StartDate = SelectedStartDate,
                    StartTime = SelectedStartTime,
                    EndDate = SelectedEndDate,
                    EndTime = SelectedEndTime,
                    CreatedAt = DateTime.Now,
                    To = To,
                    From = SelectedEmail.Address
                });

                OpenCustomMessageBox(header: Properties.Resources.Well_Done,
                                     message: string.Format(CultureInfo.CurrentCulture, Properties.Resources.Email_sent_successfully_to, emails),
                                     ok: async () => await CloseDialog());

                await CloseDialog(3000);

                Emails = string.Empty;
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void ShareByEmail(object obj)
        {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            Guid photoID = Guid.NewGuid();

            string photolocation = $"{Path.GetTempPath()}/{photoID}.jpg";

            encoder.Frames.Add(BitmapFrame.Create(_qRCodeRepository.GerarQRCode(178, 178, GenerateText())));

            using (FileStream filestream = new FileStream(photolocation, FileMode.Create))
            {
                encoder.Save(filestream);
            }

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(From),
                Subject = Subject,
                IsBodyHtml = SelectedEmail.IsBodyHtml,
                Body = Body
            };

            mailMessage.Attachments.Add(new Attachment(photolocation));

            string filename = $"{Path.GetTempPath()}/mymessage.eml";

            mailMessage.Save(filename);

            _ = Process.Start(filename);
        }
    }
}