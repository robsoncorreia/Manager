using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.RecicleBin
{
    public class RecicleBinProjectViewModel : ProjectViewModelBase
    {
        private string _filter = string.Empty;
        private readonly INetworkService _internetService;
        private bool _isOpenFilterDialogHost;

        public RecicleBinProjectViewModel(IFrameNavigationService navigationService,
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
                                 INetworkService internetService) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            _internetService = internetService;

            Projects = new ObservableCollection<ProjectModel>();

            LoadedCommand = new RelayCommand<object>(Loaded);

            NavigateBeforeCommand = new RelayCommand<object>(NavigateBefore);

            RestoreProjectCommand = new RelayCommand<object>(RestoreProjectAsync);

            RestoreAllAsyncCommand = new RelayCommand<object>(RestoreAllAsync);

            PermanentlyDeleteProjectAsyncCommand = new RelayCommand<object>(PermanentlyDeleteProjectAsync);

            PermanentlyDeleteAllProjectsAsyncCommand = new RelayCommand<object>(PermanentlyDeleteAllProjectsAsync);

            SelectionChangedOrderByCommand = new RelayCommand<object>(SelectionChangedOrderBy);

            OpenFilterDialogHostCommand = new RelayCommand<object>(OpenFilterDialogHost);

            CloseFilterDialogHostCommand = new RelayCommand<object>(CloseFilterDialogHost);

            ReloadCommand = new RelayCommand(Reload);
        }

        #region Properties

        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;

                ProjectsCollectionView.Filter = w =>
                {
                    using ProjectModel model = w as ProjectModel;

                    return model?.Name?.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
                };
            }
        }

        public bool IsOpenFilterDialogHost
        {
            get => _isOpenFilterDialogHost;
            set => SetProperty(ref _isOpenFilterDialogHost, value);
        }

        #endregion Properties

        #region ICommand

        public ICommand CloseFilterDialogHostCommand { get; set; }

        public ICommand OpenFilterDialogHostCommand { get; set; }

        public ICommand PermanentlyDeleteAllProjectsAsyncCommand { get; set; }

        public ICommand PermanentlyDeleteProjectAsyncCommand { get; set; }

        public ICommand RestoreAllAsyncCommand { get; set; }

        public ICommand RestoreProjectCommand { get; set; }

        public ICommand SelectionChangedOrderByCommand { get; set; }

        #endregion ICommand

        #region Collection

        public ObservableCollection<ProjectModel> Projects { get; set; }

        public ICollectionView ProjectsCollectionView => CollectionViewSource.GetDefaultView(Projects);

        #endregion Collection

        #region Method

        private void CloseFilterDialogHost(object obj)
        {
            IsOpenFilterDialogHost = false;
        }

        private async void GetDeletedProjects()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                Filter = string.Empty;

                if (Projects.Any())
                {
                    Projects.Clear();
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Getting_deleted_projects,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await _projectRepository.GetAllDeleted(Projects);

                await Task.Run(() => { while (count.IsRunnign) { }; });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            GetDeletedProjects();
        }

        private void NavigateBefore(object obj = null)
        {
            _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
        }

        private void OpenFilterDialogHost(object obj)
        {
            IsOpenFilterDialogHost = true;
        }

        private async Task PermanentlyDeleteAllProjects()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Deleting,
                                     message: Domain.Properties.Resources.Permanently_deleting_all_projects);

                await _projectRepository.PermanentlyDeleteAllProjects(Projects);

                IsOpenDialogHost = false;

                Projects.Clear();

                _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void PermanentlyDeleteAllProjectsAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete_All,
                                 message: Domain.Properties.Resources.Are_you_sure_you_want_to_permanently_delete_all_projects_,
                                 textButtonCustom: Domain.Properties.Resources._Delete_All,
                                 custom: async () => await PermanentlyDeleteAllProjects(),
                                 textButtonCancel: Domain.Properties.Resources._Cancel,
                                 cancel: async () => await CloseDialog());
        }

        private async Task PermanentlyDeleteProject(object obj)
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                if (obj is not ProjectModel project)
                {
                    return;
                }

                count.SetTime(2000);

                count.Start();

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Deleting_project);

                await _projectRepository.PermanentlyDeleteProject(project);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                IsOpenDialogHost = false;

                _ = Projects.Remove(project);

                if (Projects.Count == 0)
                {
                    _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void PermanentlyDeleteProjectAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Do_you_want_to_permanently_delete_project_,
                                 textButtonCustom: Domain.Properties.Resources._Delete,
                                 custom: async () => await PermanentlyDeleteProject(obj),
                                 textButtonCancel: Domain.Properties.Resources._Cancel,
                                 cancel: async () => await CloseDialog());
        }

        private void Reload()
        {
            GetDeletedProjects();
        }

        private async Task RestoreAll()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Restoring_all_projects,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                await _projectRepository.RestoreAll(Projects);

                IsOpenDialogHost = false;

                NavigateBefore();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RestoreAllAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Restore,
                                 message: Domain.Properties.Resources.Want_to_restore_all_projects_,
                                 custom: async () => await RestoreAll(),
                                 textButtonCustom: Domain.Properties.Resources.Restore,
                                 textButtonCancel: Domain.Properties.Resources._Cancel,
                                 cancel: async () => await CloseDialog());
        }

        private readonly CountDownTimer count = new();

        private async void RestoreProject(ProjectModel projectModel)
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Restoring,
                                     message: Domain.Properties.Resources.Restoring_project,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await _projectRepository.Restore(projectModel);

                await Task.Run(() => { while (count.IsRunnign) { }; });

                _ = Projects.Remove(projectModel);

                await CloseDialog();

                if (Projects.Count == 0)
                {
                    _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void RestoreProjectAsync(object obj)
        {
            if (obj is not ProjectModel project)
            {
                return;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Restore,
                                 message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Do_You_Want_Restore_Project, project.Name),
                                 textButtonCustom: Domain.Properties.Resources._Restore,
                                 custom: () => RestoreProject(project),
                                 textButtonCancel: Domain.Properties.Resources._Cancel,
                                 cancel: async () => await CloseDialog());
        }

        private void SelectionChangedOrderBy(object obj)
        {
            if (!Projects.Any())
            {
                return;
            }

            if (obj is not ProjectOrderByEnum)
            {
                return;
            }

            ProjectModel[] arrayProject = new ProjectModel[Projects.Count];

            switch ((ProjectOrderByEnum)obj)
            {
                case ProjectOrderByEnum.CreatedAt:

                    Projects.OrderByDescending(x => x.ParseObject?.CreatedAt).ToList().CopyTo(arrayProject);
                    break;

                case ProjectOrderByEnum.UpdatedAt:
                    Projects.OrderByDescending(x => x.ParseObject?.UpdatedAt).ToList().CopyTo(arrayProject);
                    break;

                case ProjectOrderByEnum.Name:
                    Projects.OrderBy(x => x.Name).ToList().CopyTo(arrayProject);
                    break;
            }

            Projects.Clear();

            foreach (ProjectModel project in arrayProject)
            {
                Projects.Add(project);
            }
        }

        #endregion Method
    }
}