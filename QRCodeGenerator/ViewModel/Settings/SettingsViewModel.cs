using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using QRCodeGenerator.Model;
using QRCodeGenerator.Repository;
using QRCodeGenerator.Service;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace QRCodeGenerator.ViewModel.Settings
{
    public class SettingsViewModel : FlexViewModelBase
    {
        private readonly IColorService _colorService;
        private readonly ILanguageRepository _languageRepository;

        private readonly ILocalDBRepository _localDBRepository;
        private BaseTheme _baseTheme = (BaseTheme)Properties.Settings.Default.baseTheme;
        private string _foreground = Properties.Settings.Default.foreground;
        private int _selectedIndexLanguages;

        public SettingsViewModel(ITaskService taskService,
                                 IDialogService dialogService,
                                 ILanguageRepository languageRepository,
                                 ILocalDBRepository localDBRepository,
                                 IColorService colorService) : base(taskService, dialogService)
        {
            _colorService = colorService;

            _localDBRepository = localDBRepository;

            _dialogService = dialogService;

            _languageRepository = languageRepository;

            ChangeLanguageCommand = new RelayCommand<object>(ChangeLanguage);

            ClearAllHistoryAsyncCommand = new RelayCommand<object>(ClearAllHistoryAsync);

            SelectionChangedColorCommand = new RelayCommand<object>(SelectionChangedColor);

            Languages = _languageRepository.Languages;

            SelectedIndexLanguages = Properties.Settings.Default.language;

            DefautColors = _colorService.DefautColors;

            _colorService.PropertyChanged += ColorServicePropertyChanged;
        }

        public BaseTheme BaseTheme
        {
            get => _baseTheme;
            set => Set(ref _baseTheme, value);
        }

        public ICommand ChangeLanguageCommand { get; set; }
        public ICommand ClearAllHistoryAsyncCommand { get; set; }
        public IList<SolidColorBrush> DefautColors { get; set; }

        public string Foreground
        {
            get => _foreground;
            set => Set(ref _foreground, value);
        }

        public IList<LanguageModel> Languages { get; set; }

        public string PrimarySoftwareColor
        {
            get => Properties.Settings.Default.primarySoftwareColor;
            set
            {
                Properties.Settings.Default.primarySoftwareColor = value;
                Properties.Settings.Default.Save();
            }
        }

        public int SelectedIndexLanguages
        {
            get => _selectedIndexLanguages;
            set => Set(ref _selectedIndexLanguages, value);
        }

        public ICommand SelectionChangedColorCommand { get; set; }

        private void ChangeLanguage(object obj)
        {
            _languageRepository.ChangeLanguage(SelectedIndexLanguages);

            OpenCustomMessageBox(header: Properties.Resources.Close,
                                 message: Properties.Resources.To_make_the_changes_effective__it_is_necessary_to_close_and_reopen_the_software__Do_you_want_to_close_the_software_,
                                 custom: () => Close(),
                                 textButtonCustom: Properties.Resources.Yes,
                                 textButtonCancel: Properties.Resources.No,
                                 cancel: async () => await CloseDialog());
        }

        private void ClearAllHistoryAsync(object obj)
        {
            OpenCustomMessageBox(header: Properties.Resources.Clear_All,
                                 message: Properties.Resources.Do_you_want_to_delete_the_entire_history_,
                                 textButtonCancel: Properties.Resources.No,
                                 custom: () => ClearHistoryAll(),
                                 textButtonCustom: Properties.Resources.Yes,
                                 cancel: async () => await CloseDialog());
        }

        private async void ClearHistoryAll()
        {
            try
            {
                _ = _localDBRepository.ClearAll();
                await CloseDialog(2000);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void Close()
        {
            Application.Current.Shutdown();
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

        private void SelectionChangedColor(object obj)
        {
            if (!(obj is SolidColorBrush color))
            {
                return;
            }

            _colorService.PrimarySoftwareColor = color.ToString();
        }
    }
}