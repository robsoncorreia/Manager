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

namespace FC.Manager.ViewModel.Project.Archived
{
    public class ArchivedProjectViewModel : ProjectViewModelBase
    {
        private string _filter = string.Empty;

        private readonly INetworkService _internetService;

        private bool _isOpenFilterDialogHost;

        private int _selectedIndexOrderBy;

        private int _selectedIndexProject;

        public ArchivedProjectViewModel(IFrameNavigationService navigationService,
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

            UnarchiveCommand = new RelayCommand<object>(UnarchiveAsync);

            UnarchiveAllAsyncCommand = new RelayCommand<object>(UnarchiveAllAsync);

            PermanentlyDeleteAllProjectsAsyncCommand = new RelayCommand<object>(PermanentlyDeleteAllProjectsAsync);

            SelectionChangedOrderByCommand = new RelayCommand<object>(SelectionChangedOrderBy);

            PermanentlyDeleteProjectAsyncCommand = new RelayCommand<object>(PermanentlyDeleteProjectAsync);

            OpenFilterDialogHostCommand = new RelayCommand<object>(OpenFilterDialogHost);

            CloseFilterDialogHostCommand = new RelayCommand<object>(CloseFilterDialogHost);

            ReloadCommand = new RelayCommand<object>(Reload);
        }

        public ICommand CloseFilterDialogHostCommand { get; set; }

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

        public ICommand OpenFilterDialogHostCommand { get; set; }

        public ICommand PermanentlyDeleteAllProjectsAsyncCommand { get; set; }

        public ICommand PermanentlyDeleteProjectAsyncCommand { get; set; }

        public ObservableCollection<ProjectModel> Projects { get; set; }

        public ICollectionView ProjectsCollectionView => CollectionViewSource.GetDefaultView(Projects);

        public int SelectedIndexOrderBy
        {
            get => _selectedIndexOrderBy;
            set => SetProperty(ref _selectedIndexOrderBy, value);
        }

        public int SelectedIndexProject
        {
            get => _selectedIndexProject;
            set => SetProperty(ref _selectedIndexProject, value);
        }

        public ICommand SelectionChangedOrderByCommand { get; set; }

        public ICommand UnarchiveAllAsyncCommand { get; set; }

        public ICommand UnarchiveCommand { get; set; }

        private void CloseFilterDialogHost(object obj)
        {
            IsOpenFilterDialogHost = false;
        }

        private readonly CountDownTimer count = new();

        private async void GetArchivedProjects()
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
                                     message: Domain.Properties.Resources.Getting_archived_projects,
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await _projectRepository.GetArchivedProjects(Projects);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Loaded(object obj)
        {
            GetArchivedProjects();
        }

        private void NavigateBefore(object obj)
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
                                     message: Domain.Properties.Resources.Permanently_deleting_all_projects,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await _projectRepository.PermanentlyDeleteAllProjects(Projects);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

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

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Deleting_project,
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await _projectRepository.PermanentlyDeleteProject((ProjectModel)obj);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                _ = Projects.Remove((ProjectModel)obj);

                if (Projects.Any())
                {
                    IsOpenDialogHost = false;
                    return;
                }

                _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void PermanentlyDeleteProjectAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.The_project_will_be_permanently_deleted,
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 custom: async () => await PermanentlyDeleteProject(obj),
                                 cancel: async () => await CloseDialog());
        }

        private void Reload(object obj)
        {
            GetArchivedProjects();
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

        private async void Unarchive(ProjectModel projectModel)
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Unarchiving_project,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await _projectRepository.Unarchive(projectModel);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                IsOpenDialogHost = false;

                _ = Projects.Remove(projectModel);

                if (Projects.Any())
                {
                    return;
                }

                _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task UnarchiveAll()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Unarchiving_all_projects,
                                     cancel: async () => await CloseDialog());

                count.SetTime(2000);

                count.Start();

                await _projectRepository.UnarchiveAll(Projects);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    }
                });

                IsOpenDialogHost = false;

                _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void UnarchiveAllAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Unarchive,
                                 message: Domain.Properties.Resources.Do_you_want_to_unarchive_all_projects_,
                                 textButtonCustom: Domain.Properties.Resources.Unarchive_all,
                                 custom: async () => await UnarchiveAll(),
                                 cancel: async () => await CloseDialog());
        }

        private void UnarchiveAsync(object obj)
        {
            if (obj is null)
            {
                return;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Restore,
                                 message: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.Do_You_Want_Restore_Project, ((ProjectModel)obj).Name),
                                 textButtonCustom: Domain.Properties.Resources._Unarchive,
                                 custom: () => Unarchive(obj as ProjectModel),
                                 cancel: async () => await CloseDialog());
        }
    }
}