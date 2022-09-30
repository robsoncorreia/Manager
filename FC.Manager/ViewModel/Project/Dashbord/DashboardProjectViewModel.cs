using CommunityToolkit.Mvvm.Input;
using FC.Domain._Util;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Dashbord
{
    public class DashboardProjectViewModel : ProjectViewModelBase, IDropTarget
    {
        private readonly INetworkService _internetService;
        private int _countArchived;
        private int _countRecicleBin;
        private string _filter = string.Empty;
        private bool _isOpenProjectFilter;
        private bool _isReloading;
        private int _selectedIndexAmbienceOrderBy;
        private int _selectedIndexOrderBy = 0;
        private int _selectedIndexProject = -1;

        public DashboardProjectViewModel(IFrameNavigationService navigationService,
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

            CreateCommand = new RelayCommand<object>(Create);

            LoadedCommand = new RelayCommand<object>(Loaded);

            ReloadCommand = new RelayCommand<object>(Reload);

            OpenRecicleBinCommand = new RelayCommand<object>(OpenRecicleBin);

            OpenArchivedCommand = new RelayCommand<object>(OpenArchived);

            EventCommand = new RelayCommand<object>(Event);

            DeleteAsyncCommand = new RelayCommand<object>(DeleteAsync);

            SelectionChangedOrderByCommand = new RelayCommand<object>(SelectionChangedOrderBy);

            ArchiveAsyncCommand = new RelayCommand<object>(ArchiveAsync);

            DeleteAllAsyncCommand = new RelayCommand<object>(DeleteAllAsync);

            OpenProjectFilterCommand = new RelayCommand<object>(OpenProjectFilter);

            CloseProjectFilterCommand = new RelayCommand<object>(CloseProjectFilter);

            SelectionChangedCommand = new RelayCommand<object>(SelectionChanged);
            RenameCommand = new RelayCommand<object>(Rename);
        }

        private async void Rename(object obj)
        {
            try
            {
                if (obj is not ProjectModel project)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Updating,
                                     message: Domain.Properties.Resources.Updating_The_Project);

                await _projectRepository.Rename(project);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        #region ICommand

        public ICommand SelectionChangedOrderByCommand { get; set; }

        public ICommand ArchiveAsyncCommand { get; set; }

        public ICommand RenameCommand { get; set; }

        public ICommand CloseProjectFilterCommand { get; set; }

        public ICommand DeleteAllAsyncCommand { get; set; }

        public ICommand DeleteAsyncCommand { get; set; }

        public ICommand EventCommand { get; set; }

        public ICommand SelectionChangedCommand { get; set; }

        public ICommand OpenArchivedCommand { get; set; }

        public ICommand OpenProjectFilterCommand { get; set; }

        public ICommand OpenRecicleBinCommand { get; set; }

        #endregion ICommand

        #region Properties

        public int CountArchived
        {
            get => _countArchived;
            set => SetProperty(ref _countArchived, value);
        }

        public int CountRecicleBin
        {
            get => _countRecicleBin;
            set => SetProperty(ref _countRecicleBin, value);
        }

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

        public bool IsOpenProjectFilter
        {
            get => _isOpenProjectFilter;
            set => SetProperty(ref _isOpenProjectFilter, value);
        }

        public bool IsReloading
        {
            get => _isReloading;
            set => SetProperty(ref _isReloading, value);
        }

        public int SelectedIndexAmbienceOrderBy
        {
            get => _selectedIndexAmbienceOrderBy;
            set => SetProperty(ref _selectedIndexAmbienceOrderBy, value);
        }

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

        #endregion Properties

        #region Collections

        public ObservableCollection<ProjectModel> Projects { get; set; }

        public ICollectionView ProjectsCollectionView => CollectionViewSource.GetDefaultView(Projects);

        #endregion Collections

        #region Methods

        private void SelectionChanged(object obj)
        {
            if (obj is not ProjectModel project)
            {
                return;
            }

            _projectService.SelectedProject = project;

            _navigationService.NavigateTo(Domain.Properties.Resources.Detail_Project);
        }

        private async Task Archive(object obj)
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

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Archiving_project);

                await _projectRepository.Archive(project);

                _ = Projects.Remove(project);

                await CloseDialog();

                await CountArchivedAsync();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ArchiveAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Archive,
                                 message: Domain.Properties.Resources.Do_you_want_to_archive_the_project_,
                                 custom: async () => await Archive(obj),
                                 textButtonCustom: Domain.Properties.Resources._Archive,
                                 cancel: async () => await CloseDialog());
        }

        private void CloseProjectFilter(object obj)
        {
            IsOpenProjectFilter = false;
        }

        private async Task CountArchivedAsync()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                CountArchived = await _projectRepository.CountArchived();
            }
            catch (Exception)
            {
            }
        }

        private async Task CountRecicleBinAsync()
        {
            if (!await _internetService.IsInternet())
            {
                throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
            }
            CountRecicleBin = await _projectRepository.CountRecicleBinAsync();
        }

        private void Create(object obj = null)
        {
            _navigationService.NavigateTo(AppConstants.CREATEPROJECT);
        }

        private async Task Delete(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Sending_the_project_to_the_trash);

                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                await _projectRepository.Delete(obj as ProjectModel);

                _ = Projects.Remove(obj as ProjectModel);

                await CountRecicleBinAsync();

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task DeleteAll()
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Sending_all_projects_to_trash);

                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                await _projectRepository.DeleteAll(Projects);

                await CountRecicleBinAsync();

                await CloseDialog(0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAllAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Do_you_want_to_send_all_projects_to_trash_,
                                 textButtonCustom: Domain.Properties.Resources._Delete_All,
                                 custom: async () => await DeleteAll(),
                                 cancel: async () => await CloseDialog()); ;
        }

        private void DeleteAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Do_you_want_to_send_the_project_to_the_trash_,
                                 custom: async () => await Delete(obj),
                                 textButtonCustom: Domain.Properties.Resources._Delete,
                                 cancel: async () => await CloseDialog());
        }

        private void Event(object obj)
        {
        }

        private async Task GetAll()
        {
            if (!await _internetService.IsInternet())
            {
                throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
            }

            await _projectRepository.GetAll(Projects);
        }

        private readonly CountDownTimer count = new();

        private async Task GetProjects()
        {
            try
            {
                count.SetTime(1000);

                Filter = string.Empty;

                OpenCustomMessageBox(header: Domain.Properties.Resources.Projects,
                                     message: Domain.Properties.Resources.Loading_all_projects,
                                     isProgressBar: true,
                                     textButtonCancel: Domain.Properties.Resources._Cancel,
                                     cancel: async () => await CloseDialog());

                Projects.Clear();

                CountArchived = 0;

                CountRecicleBin = 0;

                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                count.Start();

                await CountRecicleBinAsync();

                await CountArchivedAsync();

                await GetAll();

                await Task.Run(() => { while (count.IsRunnign) { }; });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void Loaded(object obj)
        {
            await GetProjects();
            SortProjects();
        }

        private void OpenArchived(object obj)
        {
            _navigationService.NavigateTo(Domain.Properties.Resources.Archived_Project);
        }

        private void OpenProjectFilter(object obj)
        {
            IsOpenProjectFilter = true;
        }

        private void OpenRecicleBin(object obj)
        {
            _navigationService.NavigateTo(Domain.Properties.Resources.Project_trash);
        }

        private async void Reload(object obj)
        {
            IsReloading = true;
            await GetProjects();
            IsReloading = false;
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

            SaveProjectsOrder(Projects);
        }

        #endregion Methods

        #region DragDrop

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo is null)
            {
                return;
            }

            if (dropInfo.Data is ProjectModel)
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

            if (dropInfo.Data is ProjectModel model)
            {
                Projects.RemoveAt(dropInfo.DragInfo.SourceIndex);

                if (dropInfo.InsertIndex == 0)
                {
                    Projects.Insert(0, model);
                    SaveProjectsOrder(Projects);
                    return;
                }

                Projects.Insert(dropInfo.InsertIndex - 1, model);
                SaveProjectsOrder(Projects);
                return;
            }
        }

        private void SaveProjectsOrder(ObservableCollection<ProjectModel> projects)
        {
            _configurationRepository.ConfigurationApp.ProjectsOrder = projects.Select(x => x.ParseObject?.ObjectId).ToArray();
            _ = _configurationRepository.Update();
        }

        private void SortProjects()
        {
            if (!Projects.Any())
            {
                return;
            }

            if (_configurationRepository.ConfigurationApp.ProjectsOrder is null)
            {
                return;
            }

            foreach (string objectId in _configurationRepository.ConfigurationApp.ProjectsOrder)
            {
                ProjectModel project = Projects.FirstOrDefault(x => x.ParseObject?.ObjectId == objectId);

                if (project != null)
                {
                    _ = Projects.Remove(project);
                    Projects.Add(project);
                }
            }
        }

        #endregion DragDrop
    }
}