using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using QRCodeGenerator.Service;
using QRCodeGenerator.Util;
using System.Windows.Input;

namespace QRCodeGenerator.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string Version => $"{Properties.Resources.QRCode_Generator} {Properties.Resources.AppVersion}";

        public ICommand LoadedCommand { get; set; }

        public MainViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            LoadedCommand = new RelayCommand(Loaded);
        }

        private bool _isOpenDialogHost;

        public bool IsOpenDialogHost
        {
            get => _isOpenDialogHost;
            set => Set(ref _isOpenDialogHost, value);
        }

        private void Loaded()
        {
            _navigationService.NavigateTo(AppConstants.TAB);
        }

        private readonly IFrameNavigationService _navigationService;
    }
}