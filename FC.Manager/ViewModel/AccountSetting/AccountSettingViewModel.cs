using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model.License;
using FC.Domain.Service;
using Microsoft.Win32;
using Parse;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace FC.Manager.ViewModel.AccountSetting
{
    public class AccountSettingViewModel : ObservableRecipient
    {
        public ICommand EditImageProfileCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ObservableCollection<LicenseModel> Licenses { get; set; }
        public ICommand LoadedCommand { get; set; }

        public ParseUser ParseUser
        {
            get => _parseUser;
            set => SetProperty(ref _parseUser, value);
        }

        public double ProgressImageProfile
        {
            get => _progressImageProfile;
            set
            {
                if (Equals(_progressImageProfile, value))
                {
                    return;
                }
                _ = SetProperty(ref _progressImageProfile, value);
            }
        }

        public Visibility ProgressVisibility
        {
            get => _progressVisibility;
            set
            {
                if (Equals(_progressVisibility, value))
                {
                    return;
                }
                _ = SetProperty(ref _progressVisibility, value);
            }
        }

        public int SelectedIndexLicenseModel
        {
            get => _selectedIndexLicenseModel;
            set
            {
                if (Equals(_selectedIndexLicenseModel, value))
                {
                    return;
                }
                _ = SetProperty(ref _selectedIndexLicenseModel, value);
            }
        }

        public ICommand SelectedLicenseCommand { get; set; }

        public LicenseModel SelectedLicenseModel
        {
            get => _selectedLicenseModel;
            set
            {
                if (Equals(_selectedLicenseModel, value))
                {
                    return;
                }
                _ = SetProperty(ref _selectedLicenseModel, value);
            }
        }

        public object Source
        {
            get => _source;
            set
            {
                if (Equals(_source, value))
                {
                    return;
                }
                _ = SetProperty(ref _source, value);
            }
        }

        public string SourcePicture
        {
            get => _sourcePicture;
            set => SetProperty(ref _sourcePicture, value);
        }

        public AccountSettingViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            LoadedCommand = new RelayCommand(Loaded);

            GoBackCommand = new RelayCommand(GoBack);

            EditImageProfileCommand = new RelayCommand(EditImageProfile);

            SelectedLicenseCommand = new RelayCommand(SelectedLicense);

            Licenses = new ObservableCollection<LicenseModel>();
        }

        private async void EditImageProfile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Image files (*.png;*.jpeg,*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _ = File.ReadAllText(openFileDialog.FileName);
            }

            if (string.IsNullOrEmpty(openFileDialog.FileName))
            {
                return;
            }

            byte[] b = File.ReadAllBytes(openFileDialog.FileName);

            string fileName = Regex.Replace(ParseUser.CurrentUser.Username, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);

            ParseFile file = new($"{ParseUser.CurrentUser.Username}.png", b);

            await file.SaveAsync(new Progress<ParseUploadProgressEventArgs>(p =>
            {
                ProgressVisibility = Visibility.Visible;
                ProgressImageProfile = p.Progress * 100;
            }));

            ProgressVisibility = Visibility.Collapsed;
            ProgressImageProfile = 0;

            ParseUser.CurrentUser["imageProfileFile"] = file;

            await ParseUser.CurrentUser.SaveAsync();

            ParseFile applicantResumeFile = ParseUser.CurrentUser.Get<ParseFile>("imageProfileFile");
            ParseUser.CurrentUser["picture"] = applicantResumeFile.Url.AbsoluteUri;
            await ParseUser.CurrentUser.SaveAsync();
            ParseUser = ParseUser.CurrentUser;
            GetSourcePicture();
        }

        private void GetSourcePicture()
        {
            if (ParseUser.ContainsKey("imageProfileFile"))
            {
                SourcePicture = ParseUser.Get<ParseFile>("imageProfileFile").Url.ToString();
            }

            SourcePicture ??= "/FC.Domain;component/Assets/User/user.png";
        }

        private void GoBack()
        {
            _navigationService.NavigateTo(AppConstants.DASHBOARD);
        }

        private void Loaded()
        {
            ParseUser = ParseUser.CurrentUser;

            GetSourcePicture();
        }

        private void SelectedLicense()
        {
            SelectedLicenseModel = Licenses[SelectedIndexLicenseModel];
        }

        #region Fields

        private readonly IFrameNavigationService _navigationService;
        private ParseUser _parseUser;
        private double _progressImageProfile;
        private Visibility _progressVisibility = Visibility.Collapsed;
        private int _selectedIndexLicenseModel;
        private LicenseModel _selectedLicenseModel;
        private object _source;
        private string _sourcePicture;

        #endregion Fields
    }
}