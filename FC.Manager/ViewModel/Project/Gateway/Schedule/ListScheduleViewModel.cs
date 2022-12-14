using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.Device;
using FC.Domain.Model.IfThen;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.IfThen
{
    public class ListScheduleViewModel : ProjectViewModelBase
    {
        private readonly IIfThenRepository _ifThenRepository;
        private readonly IScheduleService _scheduleService;

        public ListScheduleViewModel(IFrameNavigationService navigationService,
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
                                 IIfThenRepository ifThenRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _scheduleService = scheduleService;

            _ifThenRepository = ifThenRepository;

            LoadedCommand = new RelayCommand<object>(Loaded);

            DeleteAsyncCommand = new RelayCommand<object>(DeleteAsync);

            NewCommand = new RelayCommand<object>(New);

            EnableCommand = new RelayCommand<object>(Enable);

            ReloadCommand = new RelayCommand<object>(GetSchedules);

            RenameCommand = new RelayCommand<object>(Rename);

            IfThenModels = new ObservableCollection<IfThenModel>();

            SelectionChangedCommand = new RelayCommand<object>(SelectionChanged);

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        private async void Rename(object obj)
        {
            try
            {
                if (obj is not IfThenModel ifThen)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                        isProgressBar: true,
                                        message: Domain.Properties.Resources.Renaming);

                await _ifThenRepository.Update(SelectedProjectModel, ifThen);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete_schedule,
                                 message: Domain.Properties.Resources.Do_you_want_to_delete_the_schedule_,
                                 custom: () => Delete(obj),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 textButtonCancel: Domain.Properties.Resources.Cancel,
                                 cancel: async () => await CloseDialog());
        }

        private async void Enable(object obj)
        {
            try
            {
                if (obj is not IfThenModel ifThen)
                {
                    return;
                }
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                        isProgressBar: true,
                                        message: ifThen.IsEnabled ? $"{Domain.Properties.Resources.Enabling}" : $"{Domain.Properties.Resources.Disabling}"); ;

                await _ifThenRepository.RuleIdEnabled(SelectedProjectModel, ifThen);

                await _ifThenRepository.Update(SelectedProjectModel, ifThen);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public ICommand DeleteAsyncCommand { get; set; }
        public ObservableCollection<IfThenModel> IfThenModels { get; set; }
        public ICommand NewCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand EnableCommand { get; set; }
        public ICommand RenameCommand { get; set; }

        private async void Delete(object obj)
        {
            try
            {
                if (obj is not IfThenModel ifThen)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Deleting,
                                     isProgressBar: true);

                await _ifThenRepository.GetIfThen(SelectedProjectModel, ifThen);

                bool isDeleted = await _ifThenRepository.DeleteIfThenFromGataway(SelectedProjectModel, ifThen);

                if (isDeleted)
                {
                    await _ifThenRepository.DeleteIfThenFromCloud(SelectedProjectModel, ifThen);
                }

                _ = IfThenModels.Remove(ifThen);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetSchedules(object obj = null)
        {
            DateTime dateTime = DateTime.Now;

            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading,
                                     isProgressBar: true,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());

                await _ifThenRepository.GetIfThens(SelectedProjectModel, IfThenModels, IfthenType.Schedule);

                await Task.Delay(2000 - (DateTime.Now - dateTime).Milliseconds);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            if (obj is not Page view)
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            GetSchedules();
        }

        private void New(object obj)
        {
            _projectService.SelectedProject.SelectedIfThen = new IfThenModel { IfthenType = IfthenType.Schedule };
            _scheduleService.Source = new Uri(AppConstants.CREATESCHEDULEPAGE, UriKind.Relative);
        }

        private void SelectionChanged(object obj)
        {
            if (obj is not IfThenModel selectedSchedule)
            {
                return;
            }

            _projectService.SelectedProject.SelectedIfThen = selectedSchedule;
            _scheduleService.Source = new Uri(AppConstants.CREATESCHEDULEPAGE, UriKind.Relative);
        }
    }
}