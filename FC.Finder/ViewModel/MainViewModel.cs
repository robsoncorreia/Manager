using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Service;
using System.Windows.Input;

namespace FC.Finder.ViewModel
{
    public class MainViewModel : ObservableRecipient
    {
        public string Version { get; set; } = $"{Properties.Resources.AppName} {Properties.Resources.AppVersion}";

        public ICommand LoadedCommand { get; set; }

        public MainViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;

            LoadedCommand = new RelayCommand(Loaded);
        }

        private void Loaded()
        {
            _navigationService.NavigateTo(AppConstants.GATEWAY);
        }

        private readonly IFrameNavigationService _navigationService;
    }
}