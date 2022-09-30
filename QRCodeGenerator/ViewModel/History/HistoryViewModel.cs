using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using QRCodeGenerator.Model;
using QRCodeGenerator.Repository;
using QRCodeGenerator.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QRCodeGenerator.ViewModel.History
{
    public class HistoryViewModel : FlexViewModelBase
    {
        private readonly IColorService _colorService;
        private readonly ILocalDBRepository _localDBRepository;
        private BaseTheme _baseTheme = (BaseTheme)Properties.Settings.Default.baseTheme;
        private string _foreground = Properties.Settings.Default.foreground;

        public HistoryViewModel(ITaskService taskService,
                                IDialogService dialogService,
                                ILocalDBRepository localDBRepository,
                                IColorService colorService) : base(taskService, dialogService)
        {
            _colorService = colorService;

            _localDBRepository = localDBRepository;

            QRCodes = new ObservableCollection<QRCodeModel>();

            LoadedCommand = new RelayCommand<object>(Loaded);

            ClearAllAsyncCommand = new RelayCommand<object>(ClearAllAsync);

            _colorService.PropertyChanged += ColorServicePropertyChanged;
        }

        public BaseTheme BaseTheme
        {
            get => _baseTheme;
            set => Set(ref _baseTheme, value);
        }

        public ICommand ClearAllAsyncCommand { get; set; }

        public string Foreground
        {
            get => _foreground;
            set => Set(ref _foreground, value);
        }

        public ICommand LoadedCommand { get; set; }
        public ObservableCollection<QRCodeModel> QRCodes { get; set; }

        private async void ClearAll()
        {
            try
            {
                _ = _localDBRepository.ClearAll();
                QRCodes.Clear();
                await CloseDialog(2000);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void ClearAllAsync(object obj)
        {
            OpenCustomMessageBox(header: Properties.Resources.Clear_All,
                                 message: Properties.Resources.Do_you_want_to_delete_the_entire_history_,
                                 textButtonCancel: Properties.Resources.No,
                                 custom: () => ClearAll(),
                                 textButtonCustom: Properties.Resources.Yes,
                                 cancel: async () => await CloseDialog());
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

        private void GetAll()
        {
            if (QRCodes.Any())
            {
                QRCodes.Clear();
            }

            foreach (QRCodeModel qrcode in _localDBRepository.GetAll())
            {
                QRCodes.Add(qrcode);
            }
        }

        private void Loaded(object obj)
        {
            GetAll();
        }
    }
}