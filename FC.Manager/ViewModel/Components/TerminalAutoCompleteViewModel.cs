using CommunityToolkit.Mvvm.ComponentModel;

namespace FC.Manager.ViewModel.Components
{
    public class TerminalAutoCompleteViewModel : ObservableRecipient
    {
        private bool _IsChecked = Domain.Properties.Settings.Default.terminalAutoComplete;

        public bool IsChecked
        {
            get => _IsChecked;
            set
            {
                _ = SetProperty(ref _IsChecked, value);
                Domain.Properties.Settings.Default.terminalAutoComplete = value;
            }
        }
    }
}