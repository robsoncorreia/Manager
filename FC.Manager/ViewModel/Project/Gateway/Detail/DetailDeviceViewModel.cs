using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.Project;
using FC.Domain.Model.User;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Detail
{
    public class DetailDeviceViewModel : ProjectViewModelBase, IDropTarget
    {
        public DetailDeviceViewModel(IFrameNavigationService navigationService,
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
                                 IGatewayRepository gatewayRepository,
                                 IDialogService dialogService,
                                 ILoginRepository loginRepository,
                                 IScheduleService scheduleService,
                                 INetworkService inetworkService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _scheduleService = scheduleService;

            _inetworkService = inetworkService;

            LoadedCommand = new RelayCommand<object>(Loaded);

            UnloadedCommand = new RelayCommand<object>(Unloaded);

            NavigateBeforeCommand = new RelayCommand<object>(NavigateBefore);

            SyncGatewayCommand = new RelayCommand<object>(SyncGateway);

            RebootCommand = new RelayCommand<object>(Reboot);

            _gatewayService.PropertyChanged += GatewayServicePropertyChanged;

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        private async void Reboot(object obj)
        {
            using ReplaySubject<string> rx = new();
            using CountDownTimer timer = new();

            try
            {
                if (_gatewayService.IsSendingToGateway)
                {
                    return;
                }

                timer.SetTime(15000);
                timer.Start();
                timer.TimeChanged += () => rx.OnNext($"{timer.TimeLeftMsStr}");
                timer.CountDownFinished += () => IsOpenDialogHost = false;
                timer.StepMs = 77;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Reboot_,
                                     rx: rx);

                await _gatewayRepository.Reboot(SelectedProjectModel.SelectedGateway);

                rx.Dispose();

                timer.Dispose();

                IsActiveSnackbar = false;

                SyncGateway();
            }
            catch (Exception ex)
            {
                ShowError(ex);

                rx.Dispose();

                timer.Dispose();
            }
            finally
            {
                rx.Dispose();

                timer.Dispose();
            }
        }

        public ICommand SyncGatewayCommand { get; set; }
        public ICommand RebootCommand { get; set; }

        private readonly IScheduleService _scheduleService;

        private readonly INetworkService _inetworkService;

        private async void SyncGateway(object obj = null)
        {
            ReplaySubject<string> replay = new();

            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Finding,
                                     isProgressBar: true,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog(),
                                     rx: replay);

                if (!await _gatewayRepository.FindGateway(gateway: SelectedProjectModel.SelectedGateway,
                                                          replay: replay,
                                                          isSendingToGateway: true))
                {
                    if (IsCanceled)
                    {
                        OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                             message: $"{Domain.Properties.Resources.Task_canceled}",
                                             textButtonCancel: Domain.Properties.Resources.Close,
                                             cancel: async () => await CloseDialog());
                        return;
                    }
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: Domain.Properties.Resources.Gateway_not_found,
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SyncGateway(obj),
                                         textButtonCancel: Domain.Properties.Resources._Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                await _gatewayRepository.GetGatewayInfo(selectedGateway: SelectedProjectModel.SelectedGateway,
                                                        isSendingToGateway: true);

                _gatewayService.IsPrimary = SelectedProjectModel.SelectedGateway.IsPrimary;

                if (!await _inetworkService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                await _gatewayRepository.Update(SelectedProjectModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            finally
            {
                replay.Dispose();
            }
        }

        private void GatewayServicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GatewayService.IsPrimary))
            {
                PrimaryZwaveCollapse(((GatewayService)sender).IsPrimary);
            }
        }

        private void PrimaryZwaveCollapse(bool isPrimary)
        {
            if (tabControl.Items.Cast<TabItem>().FirstOrDefault(x => x.Header.ToString() == Domain.Properties.Resources.Primary_Z_Wave) is not TabItem tabItem)
            {
                return;
            }
            tabItem.Visibility = isPrimary ? Visibility.Visible : Visibility.Collapsed;
        }

        public override void Unloaded(object obj)
        {
            base.Unloaded(obj);
            SelectedProjectModel.SelectedGateway.Dispose();
            _scheduleService.Source = new Uri(AppConstants.LISTSCHEDULEPAGE, UriKind.Relative);
        }

        private TabControl tabControl = null;

        private void GetTabs(object obj = null)
        {
            if (obj is not TabControl control)
            {
                return;
            }
            tabControl = control;

            _gatewayService.TabControl = control;

            List<TabItem> tabs = tabControl.Items.Cast<TabItem>().Select(x => { x.Visibility = Visibility.Visible; return x; }).ToList();

            IList<string> collapseds = new List<string>();

            if (!SelectedProjectModel.SelectedGateway.IsSupportsRelay)
            {
                collapseds.Add(Domain.Properties.Resources.Relay);
            }

            if (!SelectedProjectModel.SelectedGateway.IsSupportsSerial)
            {
                collapseds.Add(Domain.Properties.Resources.Serial);
            }

            if (!SelectedProjectModel.SelectedGateway.IsSupportsRF)
            {
                collapseds.Add(Domain.Properties.Resources.Radio_433MHz);
                collapseds.Add(Domain.Properties.Resources.RTS);
            }

            if (!SelectedProjectModel.SelectedGateway.IsSupportVoiceAssistant)
            {
                collapseds.Add(Domain.Properties.Resources.Voice_Assistant);
            }

            if (!SelectedProjectModel.SelectedGateway.IsSupportsIR)
            {
                collapseds.Add(Domain.Properties.Resources.IR);
            }
            if (!SelectedProjectModel.SelectedGateway.IsSupportdIfthen)
            {
                collapseds.Add(Domain.Properties.Resources.Secondary_Z_Wave);
                collapseds.Add(Domain.Properties.Resources.If_Then);
                collapseds.Add(Domain.Properties.Resources.Primary_Z_Wave);
                collapseds.Add(Domain.Properties.Resources.Scheduling);
            }

            if (!SelectedProjectModel.SelectedGateway.IsPrimary)
            {
                collapseds.Add(Domain.Properties.Resources.Primary_Z_Wave);
            }

            foreach (TabItem tab in tabs.Where(x => collapseds.Contains(x.Tag)))
            {
                tab.Visibility = Visibility.Collapsed;
            }

            SelectedProjectModel.SelectedGateway.SelectedIndexTabControl = tabs.IndexOf(tabs.FirstOrDefault(x => x.Visibility == Visibility.Visible));
        }

        private void Loaded(object obj)
        {
            SortTabs(obj);

            SelectedProjectModel = _projectService.SelectedProject;

            GetTabs(obj);
        }

        private void NavigateBefore(object obj)
        {
            _navigationService.NavigateTo(Domain.Properties.Resources.Detail_Project);
        }

        #region DragDrop

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                return;
            }

            if (dropInfo.Data is ParseUserCustom)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }

            if (dropInfo.Data is TabItem && dropInfo.TargetCollection != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
                return;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                return;
            }

            if (dropInfo.TargetCollection is null)
            {
                return;
            }

            if (dropInfo.Data is ParseUserCustom custom)
            {
                if (((IList<ParseUserCustom>)dropInfo.TargetCollection).FirstOrDefault(x => x.ParseUser.ObjectId == custom.ParseUser.ObjectId) != null)
                {
                    return;
                }

                ((IList<ParseUserCustom>)dropInfo.TargetCollection).Insert(dropInfo.InsertIndex, custom);
                return;
            }

            if (dropInfo.Data is TabItem item)
            {
                ((ItemCollection)dropInfo.TargetCollection).RemoveAt(dropInfo.DragInfo.SourceIndex);

                if (dropInfo.InsertIndex == 0)
                {
                    ((ItemCollection)dropInfo.TargetCollection).Insert(0, item);
                    SelectedProjectModel.SelectedGateway.SelectedIndexTabControl = 0;
                    SaveTabOrder((ItemCollection)dropInfo.TargetCollection);
                    return;
                }

                ((ItemCollection)dropInfo.TargetCollection).Insert(dropInfo.InsertIndex - 1, item);
                SelectedProjectModel.SelectedGateway.SelectedIndexTabControl = dropInfo.InsertIndex - 1;
                SaveTabOrder((ItemCollection)dropInfo.TargetCollection);
                return;
            }
        }

        private void SaveTabOrder(ItemCollection targetCollection)
        {
            _configurationRepository.ConfigurationApp.OrderDetailDeviceTabs = targetCollection.Cast<TabItem>().Select(x => x.Name);
            _ = _configurationRepository.Update();
        }

        private void SortTabs(object obj)
        {
            if (obj is null)
            {
                return;
            }

            List<TabItem> tabs = ((TabControl)obj).Items.Cast<TabItem>().ToList();

            ((TabControl)obj).Items.Clear();

            if (_configurationRepository.ConfigurationApp.OrderDetailDeviceTabs is null)
            {
                foreach (TabItem item in tabs)
                {
                    _ = ((TabControl)obj).Items.Add(item);
                }

                SaveTabOrder(((TabControl)obj).Items);

                return;
            }

            if (tabs.Count != _configurationRepository.ConfigurationApp.OrderDetailDeviceTabs.Count())
            {
                foreach (TabItem item in tabs)
                {
                    _ = ((TabControl)obj).Items.Add(item);
                }

                SaveTabOrder(((TabControl)obj).Items);
                return;
            }

            foreach (string name in _configurationRepository.ConfigurationApp.OrderDetailDeviceTabs)
            {
                if (tabs.FirstOrDefault(x => x.Name == name) is TabItem tab)
                {
                    _ = ((TabControl)obj).Items.Add(tab);
                }
                else
                {
                    foreach (TabItem item in tabs)
                    {
                        _ = ((TabControl)obj).Items.Add(item);
                    }

                    SaveTabOrder(((TabControl)obj).Items);

                    break;
                }
            }
        }

        #endregion DragDrop
    }
}