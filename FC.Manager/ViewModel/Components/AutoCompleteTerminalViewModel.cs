using GalaSoft.MvvmLight;

namespace FC.Manager.ViewModel.Components
{
    public class AutoCompleteTerminalViewModel : ViewModelBase
    {
        private bool _IsChecked = Domain.Properties.Settings.Default.autoCompleteTerminal;

        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                Set(ref _IsChecked, value);
                Domain.Properties.Settings.Default.autoCompleteTerminal = value;
                Domain.Properties.Settings.Default.Save();
            }
        }

    }
}
