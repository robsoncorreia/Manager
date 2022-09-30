using QRCodeGenerator.Service;

namespace QRCodeGenerator.ViewModel.Tab
{
    public class TabViewModel : FlexViewModelBase
    {
        private readonly IColorService _colorService;
        private string _primarySoftwareColor = Properties.Settings.Default.primarySoftwareColor;

        public TabViewModel(ITaskService taskService,
                            IDialogService dialogService,
                            IColorService colorService) : base(taskService, dialogService)
        {
            _dialogService.PropertyChanged += DialogServicePropertyChanged;

            _colorService = colorService;

            _colorService.PropertyChanged += ColorServicePropertyChanged;
        }

        public string PrimarySoftwareColor
        {
            get => _primarySoftwareColor;
            set => Set(ref _primarySoftwareColor, value);
        }

        private void ColorServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_colorService.PrimarySoftwareColor))
            {
                PrimarySoftwareColor = ((ColorService)sender).PrimarySoftwareColor;
            }
        }

        private void DialogServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_dialogService.IsOpenDialogHost))
            {
                IsTabEnable = !((DialogService)sender).IsOpenDialogHost;
            }
        }
    }
}