using CommunityToolkit.Mvvm.Input;
using FC.Domain.Model;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Updater.View.Components.Terminal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace FC.Updater.ViewModel.Gateway
{
    public class GatewayViewModel : FlexViewModelBase
    {
        private string _filterGateways;
        private bool _isBottomDrawerOpen;
        private bool _isGroupByName = true;

        private bool _isSelectable;

        private int _selectedIndexTransitioner;

        private bool isFirstSelectAll;

        public GatewayViewModel(IFrameNavigationService navigationService,
                                IProjectService projectService,
                                IUDPRepository udpRepository,
                                ITcpRepository tcpRepository,
                                IIRRepository irRepository,
                                IUserRepository userService,
                                IProjectRepository projectRepository,
                                ILocalDBRepository localDBRepository,
                                ILogRepository logRepository,
                                IUserRepository userRepository,
                                ISerialRepository serialRepository,
                                ICommandRepository commandRepository,
                                IIPCommandRepository ipCommandRepository,
                                IGatewayService gatewayService,
                                ISettingsRepository configurationRepository,
                                ITaskService taskService,
                                IParseService parseService,
                                IRFRepository rfRepository,
                                IZwaveRepository zwaveRepository,
                                IRelayRepository relayTestRepository,
                                IGatewayRepository gatewayRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository)
        {
            LoadedCommand = new RelayCommand<object>(Loaded);

            ReloadCommand = new RelayCommand<object>(Reload);

            SelectedChangedGatewayCommand = new RelayCommand<object>(SelectedChangedGateway);

            Gateways = _gatewayService.Gateways;

            UpdateCommand = new RelayCommand<object>(Update);

            GetInfoCommand = new RelayCommand<object>(GetInfo);

            CustomUpdateCommand = new RelayCommand<object>(CustomUpdate);

            UpdateSelectedCommand = new RelayCommand<object>(UpdateSelected);

            GetBuildCommand = new RelayCommand<object>(GetBuild);

            OpenTerminalCommand = new RelayCommand<object>(OpenTerminal);

            SelectionChangedFirmwareCommand = new RelayCommand<object>(SelectionChangedFirmware);

            GetGatewaysVersionsCommand = new RelayCommand<object>(GetGatewaysVersions);

            SelectAllCommand = new RelayCommand<object>(SelectAll);

            MultiSelectionCommand = new RelayCommand<object>(MultiSelection);

            GetFirmwareInfoPICCommand = new RelayCommand<object>(GetFirmwareInfoPIC);

            OpenChangelogCommand = new RelayCommand<object>(OpenChangelog);

            UpdatePICCommad = new RelayCommand<object>(UpdatePIC);
        }

        public ICommand CustomUpdateCommand { get; set; }

        public string Error => string.Empty;

        public string FilterGateways
        {
            get => _filterGateways;
            set
            {
                if (Equals(_filterGateways, value))
                {
                    return;
                }

                _ = SetProperty(ref _filterGateways, value);

                GatewaysCollectionView.Filter = w =>
                {
                    using GatewayModel model = w as GatewayModel;

                    return model.Name.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
                };
            }
        }

        public ObservableCollection<GatewayModel> Gateways { get; set; }

        public ICollectionView GatewaysCollectionView => CollectionViewSource.GetDefaultView(Gateways);

        public ICommand GetBuildCommand { get; set; }

        public ICommand GetFirmwareInfoPICCommand { get; set; }

        public ICommand GetGatewaysVersionsCommand { get; set; }

        public ICommand GetInfoCommand { get; set; }

        public bool IsBottomDrawerOpen
        {
            get => _isBottomDrawerOpen;
            set => SetProperty(ref _isBottomDrawerOpen, value);
        }

        public bool IsGroupByName
        {
            get => _isGroupByName;
            set
            {
                if (Equals(_isGroupByName, value))
                {
                    return;
                }

                _isGroupByName = value;

                _ = SetProperty(ref _isGroupByName, value);

                GroupByName(value);
            }
        }

        public bool IsSelectable
        {
            get => _isSelectable;
            set
            {
                if (Equals(_isSelectable, value))
                {
                    return;
                }
                _ = SetProperty(ref _isSelectable, value);
            }
        }

        public ICommand MultiSelectionCommand { get; set; }

        public ICommand OpenChangelogCommand { get; set; }

        public ICommand OpenTerminalCommand { get; set; }

        public ICommand SelectAllCommand { get; set; }

        public ICommand SelectedChangedGatewayCommand { get; set; }

        public int SelectedIndexTransitioner
        {
            get => _selectedIndexTransitioner;
            set => SetProperty(ref _selectedIndexTransitioner, value);
        }

        public ICommand SelectionChangedFirmwareCommand { get; set; }

        public ICommand UpdatePICCommad { get; set; }

        public ICommand UpdateSelectedCommand { get; set; }

        private static void OpenTerminal()
        {
            foreach (object item in System.Windows.Application.Current.Windows)
            {
                if (item is TerminalWindow terminalWindow)
                {
                    terminalWindow.Close();
                }
            }

            TerminalWindow termWindow = new TerminalWindow();

            termWindow.Show();
        }

        private async void ChangeTransitioner()
        {
            await System.Threading.Tasks.Task.Delay(1000);

            SelectedIndexTransitioner = 1;

            await System.Threading.Tasks.Task.Delay(1000);

            Reload();
        }

        private async void CustomUpdate(object obj)
        {
            try
            {
                await _gatewayRepository.CustomUpdate(SelectedGateway);

                LastCommandSend = _gatewayService.LastCommandSend;
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void GetBuild(object obj = null)
        {
            try
            {
                await _gatewayRepository.GetBuild(SelectedGateway);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void GetFirmwareInfoPIC(object obj)
        {
            try
            {
                await _rfRepository.GetFirmwareInfoPICAsync(SelectedGateway, ProtocolTypeEnum.UDP);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void GetGatewaysVersions(object obj)
        {
            try
            {
                if (!(obj is ICollection<object> temp))
                {
                    return;
                }

                if (!temp.Any())
                {
                    return;
                }

                OpenCustomMessageBox(message: Domain.Properties.Resources.Getting_versions_of_gateways,
                                     header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog());

                IEnumerable<GatewayModel> list = temp.OfType<GatewayModel>();

                for (int i = 0; i < list.Count(); i++)
                {
                    await _gatewayRepository.GetUID(list.ElementAt(i));
                }

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void GetInfo(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Getting_information_from_the_gateway,
                                     cancel: async () => await CloseDialog());

                await _gatewayRepository.GetGatewayInfo(SelectedGateway);

                await _gatewayRepository.GetFirmawes(SelectedGateway);

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void GroupByName(bool value)
        {
            GatewaysCollectionView.GroupDescriptions.Clear();

            if (value)
            {
                GatewaysCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Name"));
            }
        }

        private void Loaded(object obj)
        {
            SelectedIndexTransitioner = 0;

            ChangeTransitioner();

            if (Gateways.Any())
            {
                Gateways.Clear();
            }

            GroupByName(IsGroupByName);
        }

        private void MultiSelection(object obj)
        {
            IsSelectable = !IsSelectable;
        }

        private async void OpenChangelog(object obj)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = $"{Domain.Properties.Resources.PDF_File} (*.pdf)|*.pdf",
                    FileName = $"FC.UPDATER.{Domain.Properties.Resources.Changelog.ToUpper()}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, Properties.Resources.FC_UPDATER_CHANGELOG);

                    Process process = new Process();

                    process.StartInfo.FileName = saveFileDialog.FileName;

                    _ = process.Start();

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void OpenTerminal(object obj)
        {
            OpenTerminal();
        }

        private async void Reload(object obj = null)
        {
            ReplaySubject<string> replay = new ReplaySubject<string>();

            ReplaySubject<int> replayProgressBarValue = new ReplaySubject<int>();

            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Finding_Gateways_Network,
                                     rx: replay,
                                     textButtonCancel: Domain.Properties.Resources.Stop_search,
                                     cancel: async () => await CloseDialog(),
                                     isRXProgressBarVisibible: true,
                                     rxProgressBarValue: replayProgressBarValue);

                await _gatewayRepository.GetGateways(GatewayModelEnum.ANY,
                                                     address: IPAddress.Broadcast,
                                                     replay: replay,
                                                     rxProgressBarValue: replayProgressBarValue);

                if (!Gateways.Any())
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Finding_Gateways_Network,
                                         message: Domain.Properties.Resources.Could_not_find_any_gateway_on_the_network__Search_again_,
                                         textButtonCustom: Domain.Properties.Resources._Try_again,
                                         custom: () => Reload(),
                                         textButtonCancel: Domain.Properties.Resources._Close,
                                         cancel: async () => await CloseDialog());

                    return;
                }
                await CloseDialog();
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
            finally
            {
                replay.Dispose();

                replayProgressBarValue.Dispose();
            }
        }

        private void SelectAll(object obj)
        {
            if (!Gateways.Any())
            {
                return;
            }

            isFirstSelectAll = !isFirstSelectAll;

            for (int i = 0; i < Gateways.Count; i++)
            {
                Gateways[i].IsSelected = isFirstSelectAll;
            }
        }

        private async void SelectedChangedGateway(object obj)
        {
            try
            {
                if (!(obj is GatewayModel temp) || IsSelectable)
                {
                    return;
                }

                SelectedGateway = temp;

                _gatewayService.SelectedGateway = temp;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Getting_information_from_the_gateway,
                                     cancel: async () => await CloseDialog());

                await _gatewayRepository.GetFirmawes(SelectedGateway);

                await _gatewayRepository.GetBuild(SelectedGateway);

                IsBottomDrawerOpen = true;

                await _gatewayRepository.GetUID(SelectedGateway);

                if (SelectedGateway.IsSupportsRF)
                {
                    await _rfRepository.GetFirmwareInfoPICAsync(SelectedGateway, ProtocolTypeEnum.UDP);
                }

                if (SelectedGateway.Build != -1)
                {
                    SelectedGateway.Build = int.Parse(SelectedGateway.Build.ToString().Substring(0, 8));
                }

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void SelectionChangedFirmware(object obj)
        {
            try
            {
                if (!(obj is bool mouseOver))
                {
                    return;
                }

                if (!mouseOver)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Get_builds,
                                     cancel: async () => await CloseDialog());

                await _gatewayRepository.GetBuild(SelectedGateway);

                await CloseDialog(1000);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void Update(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Updating,
                                     cancel: async () => await CloseDialog());

                await _gatewayRepository.UpdateFirmawe(SelectedGateway);

                LastCommandSend = _gatewayService.LastCommandSend;

                await CloseDialog(2000);
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async void UpdatePIC(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Updating_Firmware,
                                     cancel: async () => await CloseDialog());

                await _rfRepository.UpdatePICAsync(SelectedGateway, ProtocolTypeEnum.UDP);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private async Task UpdateSelected(IEnumerable<GatewayModel> list)
        {
            try
            {
                foreach (GatewayModel gateway in list)
                {
                    await _gatewayRepository.UpdateFirmawe(gateway);
                }

                await CloseDialog();
            }
            catch (Exception ex)
            {
                await ShowError(ex);
            }
        }

        private void UpdateSelected(object obj)
        {
            if (!(obj is ICollection<object> temp))
            {
                return;
            }

            if (!temp.Any())
            {
                return;
            }

            IEnumerable<GatewayModel> list = temp.OfType<GatewayModel>();

            string mes = string.Empty;

            foreach (GatewayModel gateway in list)
            {
                mes += gateway.ToString() + '\n';
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Update_selected_gateways_to_the_latest_version,
                                 message: mes,
                                 custom: async () => await UpdateSelected(list),
                                 textButtonCustom: Domain.Properties.Resources.Update,
                                 cancel: async () => await CloseDialog());
        }
    }
}