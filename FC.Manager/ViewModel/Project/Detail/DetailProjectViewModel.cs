using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Project;
using FC.Domain.Model.User;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using GongSolutions.Wpf.DragDrop;
using Parse;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace FC.Manager.ViewModel.Project.Detail
{
    public class DetailProjectViewModel : ProjectViewModelBase, IDropTarget
    {
        private string _filterAmbiences;
        private string _filterDevices;
        private readonly INetworkService _internetService;
        private bool _isGroupBy;
        private bool _isOpenDetailProjetFilter;
        private int _selectedIndexGroupOrderBy;

        public DetailProjectViewModel(IFrameNavigationService navigationService,
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

            LoadedCommand = new RelayCommand<object>(Loaded);

            CreateAmbienceCommand = new RelayCommand<object>(GoToCreateAmbience);

            OpenAmbienceCommand = new RelayCommand<object>(OpenAmbience);

            FindUserByEmailCommand = new RelayCommand(async () => await FindUserByEmail());

            NavigateBeforeCommand = new RelayCommand<object>(NavigateBefore);

            UpdateProjectCommand = new RelayCommand<object>(UpdateProject);

            RemoveUserCommand = new RelayCommand<object>(RemoveUser);

            DeleteProjectAsyncCommand = new RelayCommand<object>(DeleteProjectAsync);

            ArchiveProjectCommand = new RelayCommand<object>(ArchiveProject);

            OpenRecicleBinAmbienceCommand = new RelayCommand<object>(OpenRecicleBinAmbience);

            OpenArchivedCommand = new RelayCommand<object>(OpenArchived);

            UnLinkAsyncCommand = new RelayCommand<object>(UnLinkAsync);

            LinkLicenseWithProjectAsyncCommand = new RelayCommand<object>(LinkLicenseWithProjectAsync);

            GoToAddIPDeviceCommand = new RelayCommand<object>(GoToAddIPDevice);

            OpenGatewayCommand = new RelayCommand<object>(OpenGateway);

            SelectColorCommand = new RelayCommand<object>(SelectColor);

            CleanAllMasterUsersCommand = new RelayCommand<object>(CleanAllHouseOwners);

            GetDevicesCommand = new RelayCommand<object>(GetDevices);

            CleanAllUsersCommand = new RelayCommand<object>(CleanAllHomeResidents);

            ClearAllUsersFoundCommand = new RelayCommand<object>(ClearAllUsersFound);

            SelectionChangedAmbienceOrderByCommand = new RelayCommand<object>(SelectionChangedAmbienceOrderBy);

            DeleteDeviceAsyncCommand = new RelayCommand<object>(DeleteDeviceAsync);

            OpenDetailProjetFilterCommand = new RelayCommand<object>(OpenDetailProjetFilter);

            CloseDetailProjetFilterCommand = new RelayCommand<object>(CloseDetailProjetFilter);

            RenameGatewayCommand = new RelayCommand<object>(RenameGateway);

            SyncGatewayCommand = new RelayCommand<object>(SyncGateway);

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        private async void SyncGateway(object obj)
        {
            if (obj is not GatewayModel gateway)
            {
                return;
            }

            ReplaySubject<string> replay = new();

            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Finding,
                                     isProgressBar: true,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog(),
                                     rx: replay);

                if (!await _gatewayRepository.FindGateway(gateway: gateway,
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

                await _gatewayRepository.GetGatewayInfo(selectedGateway: gateway,
                                                        isSendingToGateway: true);

                _gatewayService.IsPrimary = SelectedProjectModel.SelectedGateway.IsPrimary;

                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                await _gatewayRepository.Update(gateway);

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

        private async void RenameGateway(object obj)
        {
            try
            {
                if (obj is not GatewayModel gateway)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                await _gatewayRepository.Rename(gateway);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public ICollectionView AmbiencesModelCollectionView => CollectionViewSource.GetDefaultView(SelectedProjectModel.AmbiencesModel);

        public ICommand ArchiveProjectCommand { get; set; }

        public ICommand CleanAllMasterUsersCommand { get; set; }

        public ICommand CleanAllUsersCommand { get; set; }

        public ICommand ClearAllUsersFoundCommand { get; set; }

        public ICommand CloseDetailProjetFilterCommand { get; set; }

        public ICommand RenameGatewayCommand { get; set; }
        public ICommand SyncGatewayCommand { get; set; }

        public ICommand CreateAmbienceCommand { get; set; }

        public ICommand DeleteDeviceAsyncCommand { get; set; }

        public ICommand DeleteProjectAsyncCommand { get; set; }

        public ICollectionView DevicesCollectionView => CollectionViewSource.GetDefaultView(SelectedProjectModel.Devices);

        public string FilterAmbiences
        {
            get => _filterAmbiences;
            set
            {
                _filterAmbiences = value;

                AmbiencesModelCollectionView.Filter = w =>
                {
                    using AmbienceModel model = w as AmbienceModel;

                    return model.Name.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
                };
            }
        }

        public string FilterDevices
        {
            get => _filterDevices;
            set
            {
                _filterDevices = value;

                DevicesCollectionView.Filter = w =>
                {
                    using GatewayModel model = w as GatewayModel;

                    return model.Name.IndexOf(value, StringComparison.OrdinalIgnoreCase) != -1;
                };
            }
        }

        public ICommand FindUserByEmailCommand { get; set; }

        public ICommand GetDevicesCommand { get; set; }

        public ICommand GoToAddIPDeviceCommand { get; set; }

        public ICommand OpenGatewayCommand { get; set; }

        public bool IsGroupBy
        {
            get => _isGroupBy;
            set
            {
                if (Equals(_isGroupBy, value))
                {
                    return;
                }

                _ = SetProperty(ref _isGroupBy, value);

                if (_isGroupBy)
                {
                    AmbiencesModelCollectionView.GroupDescriptions.Clear();
                    AmbiencesModelCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("AmbienceGroupModel.Name"));
                    AmbiencesModelCollectionView.SortDescriptions.Add(new SortDescription("AmbienceGroupModel.Name", ListSortDirection.Ascending));
                }
                else
                {
                    AmbiencesModelCollectionView.GroupDescriptions.Clear();
                }
            }
        }

        public bool IsOpenDetailProjetFilter
        {
            get => _isOpenDetailProjetFilter;
            set => SetProperty(ref _isOpenDetailProjetFilter, value);
        }

        public ICommand LinkLicenseWithProjectAsyncCommand { get; set; }

        public ICollectionView MasterUsersCollectionView => CollectionViewSource.GetDefaultView(SelectedProjectModel.MasterUsers);

        public ICommand OpenAmbienceCommand { get; set; }

        public ICommand OpenArchivedCommand { get; set; }

        public ICommand OpenDetailProjetFilterCommand { get; set; }

        public ICommand OpenRecicleBinAmbienceCommand { get; set; }

        public ICollectionView ProgrammersCollectionView => CollectionViewSource.GetDefaultView(SelectedProjectModel.Programmers);

        public int SelectedIndexGroupOrderBy
        {
            get => _selectedIndexGroupOrderBy;
            set => SetProperty(ref _selectedIndexGroupOrderBy, value);
        }

        public ICommand SelectionChangedAmbienceOrderByCommand { get; set; }

        public ICommand UnLinkAsyncCommand { get; set; }

        public ICollectionView UnlinkedAmbiencesModelCollectionView => CollectionViewSource.GetDefaultView(SelectedProjectModel.UnlinkedAmbiencesModel);

        public ICommand UpdateProjectCommand { get; set; }

        public ICollectionView UsersCollectionView => CollectionViewSource.GetDefaultView(SelectedProjectModel.Users);

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
                throw new ArgumentNullException(nameof(dropInfo));
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
                    SelectedIndexTabControl = 0;
                    SaveTabOrder((ItemCollection)dropInfo.TargetCollection);
                    return;
                }

                ((ItemCollection)dropInfo.TargetCollection).Insert(dropInfo.InsertIndex - 1, item);
                SelectedIndexTabControl = dropInfo.InsertIndex - 1;
                SaveTabOrder((ItemCollection)dropInfo.TargetCollection);
                return;
            }
        }

        private async void ArchiveProject(object obj)
        {
            try
            {
                await _projectRepository.Archive(SelectedProjectModel);
                _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void CleanAllHomeResidents(object obj)
        {
            SelectedProjectModel.Users.Clear();
        }

        private void CleanAllHouseOwners(object obj)
        {
            SelectedProjectModel.MasterUsers.Clear();
        }

        private void ClearAll(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            SelectedProjectModel.Devices.Clear();
        }

        private void ClearAllUsersFound(object obj)
        {
            SelectedProjectModel.UserFoundSearch.Clear();
        }

        private void CloseDetailProjetFilter(object obj)
        {
            IsOpenDetailProjetFilter = false;
        }

        private async void DeleteDevice(GatewayModel deviceModel)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Deleting,
                                     message: Domain.Properties.Resources.Deleting_device,
                                     cancel: async () => await CloseDialog());

                await _gatewayRepository.Delete(deviceModel);

                _ = SelectedProjectModel.Devices.Remove(deviceModel);

                await CloseDialog(0);

                if (SelectedProjectModel.AmbiencesModel.Count <= 0)
                {
                    return;
                }

                foreach (AmbienceModel ambience in SelectedProjectModel.AmbiencesModel)
                {
                    if (ambience.Devices.FirstOrDefault(x => x.ParseObject.ObjectId == deviceModel.ParseObject.ObjectId) != null)
                    {
                        _ = ambience.Devices.Remove(ambience.Devices.FirstOrDefault(x => x.ParseObject.ObjectId == deviceModel.ParseObject.ObjectId));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteDeviceAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                 message: Domain.Properties.Resources.Are_you_sure_you_want_to_delete_device_,
                                 custom: () => DeleteDevice(obj as GatewayModel),
                                 textButtonCustom: Domain.Properties.Resources.Delete,
                                 cancel: async () => await CloseDialog());
        }

        private async void DeleteProject()
        {
            try
            {
                await _projectRepository.Delete(SelectedProjectModel);
                _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteProjectAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Updating,
                                 message: Domain.Properties.Resources.Updating_Ambience_Information,
                                 cancel: async () => await CloseDialog(),
                                 textButtomOk: Domain.Properties.Resources._Delete,
                                 ok: () => DeleteProject());
        }

        private async Task FindUserByEmail()
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    return;
                }

                ParseUserCustom user = await _userService.FindUserByEmail(Email);

                if (user is null)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Not_Found,
                                         message: Domain.Properties.Resources.No_User_Found,
                                         ok: async () => await CloseDialog());

                    await CloseDialog(1000);

                    return;
                }

                bool isAdded = SelectedProjectModel.UserFoundSearch.FirstOrDefault(x => x?.ParseUser?.ObjectId == user?.ParseUser?.ObjectId) != null;

                if (!isAdded)
                {
                    SelectedProjectModel.UserFoundSearch.Add(user);
                }
            }
            catch (NullReferenceException ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: Domain.Properties.Resources.Without_Internet_Connection,
                                     ok: async () => await CloseDialog());

                _ = await _logRepository.SaveError(ex, Properties.Resources.AppName, Properties.Resources.AppVersion);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private readonly CountDownTimer count = new();

        private async void GetDevices(object obj = null)
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                FilterDevices = string.Empty;

                if (SelectedProjectModel.Devices.Any())
                {
                    SelectedProjectModel.Devices.Clear();
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     isProgressBar: true,
                                     message: Domain.Properties.Resources.Getting_project_gateways);

                count.SetTime(2000);

                count.Start();

                await _gatewayRepository.GetAll(SelectedProjectModel);

                await Task.Run(() =>
                {
                    while (count.IsRunnign)
                    {
                    };
                });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task GetProjectInfo()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                await _projectRepository.GetProjectInfo(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task GetProjectUsers()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                await _projectRepository.GetProjectUsers(SelectedProjectModel);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void GoToAddIPDevice(object obj = null)
        {
            _navigationService.NavigateTo(AppConstants.ADDDEVICE);
        }

        private void GoToCreateAmbience(object obj)
        {
            _navigationService.NavigateTo(AppConstants.CREATEAMBIENCE);
        }

        private void OpenGateway(object obj)
        {
            if (obj is not GatewayModel gateway)
            {
                return;
            }

            OpenCustomMessageBox(header: Domain.Properties.Resources.Loading);

            _projectService.SelectedProject.SelectedGateway = gateway;

            _navigationService.NavigateTo(Domain.Properties.Resources.Detail_Device);
        }

        private async void LinkLicenseWithProject()
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Linking,
                                     message: Domain.Properties.Resources.Linking_Project_License,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     ok: async () => await CloseDialog());

                await _projectRepository.LinkLicenseWithProject(SelectedProjectModel);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void LinkLicenseWithProjectAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Link_License,
                                 message: Domain.Properties.Resources.Do_You_Link_Selected_License_Project,
                                 cancel: async () => await CloseDialog(),
                                 textButtomOk: Domain.Properties.Resources.Link,
                                 ok: () => LinkLicenseWithProject());
        }

        private async void Loaded(object obj)
        {
            SortTabs(obj);

            OpenCustomMessageBox(header: Domain.Properties.Resources.Loading,
                                 isProgressBar: true,
                                 message: Domain.Properties.Resources.Loading_Project);

            SelectedProjectModel = _projectService.SelectedProject;

            SelectedProjectModel.SelectedIndexDeviceModel = -1;

            ClearAll(SelectedProjectModel);

            await GetProjectUsers();

            await GetProjectInfo();

            await CloseDialog();
        }

        private void NavigateBefore(object obj)
        {
            _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
        }

        private void OpenAmbience(object obj)
        {
            if (obj is null)
            {
                return;
            }

            if (obj is not AmbienceModel)
            {
                return;
            }

            if (SelectedProjectModel is null)
            {
                return;
            }

            if (SelectedProjectModel.SelectedIndexAmbience < 0)
            {
                return;
            }

            SelectedProjectModel.SelectedAmbienceModel = SelectedProjectModel.AmbiencesModel.ElementAt(SelectedProjectModel.SelectedIndexAmbience);

            _navigationService.NavigateTo(AppConstants.DETAILAMBIENCE);
        }

        private void OpenArchived(object obj)
        {
            _navigationService.NavigateTo(AppConstants.ARCHIVEDAMBIENCE);
        }

        private void OpenDetailProjetFilter(object obj)
        {
            IsOpenDetailProjetFilter = true;
        }

        private void OpenRecicleBinAmbience(object obj)
        {
            _navigationService.NavigateTo(AppConstants.RECICLEBINAMBIENCE);
        }

        private void RemoveUser(object obj)
        {
            if (obj is null)
            {
                return;
            }

            if (obj is not object[])
            {
                return;
            }

            if ((string)((object[])obj)[1] == "LbProgrammer")
            {
                if (SelectedProjectModel.Programmers.Count <= 1)
                {
                    ShowSnackbar(contentSnackbar: string.Format(CultureInfo.InvariantCulture, Domain.Properties.Resources.List_Must_Least_Contain_N_User, 1));
                    return;
                }

                _ = SelectedProjectModel.Programmers.Remove((ParseUserCustom)((object[])obj)[0]);
            }
            else if ((string)((object[])obj)[1] == "LbMasterUsers")
            {
                _ = SelectedProjectModel.MasterUsers.Remove((ParseUserCustom)((object[])obj)[0]);
            }
            else if ((string)((object[])obj)[1] == "LbUsers")
            {
                _ = SelectedProjectModel.Users.Remove((ParseUserCustom)((object[])obj)[0]);
            }
            else if ((string)((object[])obj)[1] == "LbUserFoundSearch")
            {
                _ = SelectedProjectModel.UserFoundSearch.Remove((ParseUserCustom)((object[])obj)[0]);
            }
        }

        private void SaveTabOrder(ItemCollection targetCollection)
        {
            _configurationRepository.ConfigurationApp.OrderProjectDetailTabs = targetCollection.Cast<TabItem>().Select(x => x.Name);
            _ = _configurationRepository.Update();
        }

        private void SelectColor(object obj)
        {
            if (obj is SolidColorBrush brush)
            {
                SelectedProjectModel.Red = brush.Color.R;
                SelectedProjectModel.Green = brush.Color.G;
                SelectedProjectModel.Blue = brush.Color.B;
            }
        }

        private void SelectionChangedAmbienceOrderBy(object obj)
        {
            AmbienceModel[] arrayAmbiences = new AmbienceModel[SelectedProjectModel.AmbiencesModel.Count];

            switch ((AmbienceOrderByEnum)obj)
            {
                case AmbienceOrderByEnum.CreatedAt:
                    SelectedProjectModel.AmbiencesModel.OrderByDescending(x => x.ParseObject?.CreatedAt).ToList().CopyTo(arrayAmbiences);
                    break;

                case AmbienceOrderByEnum.UpdatedAt:
                    SelectedProjectModel.AmbiencesModel.OrderByDescending(x => x.ParseObject?.UpdatedAt).ToList().CopyTo(arrayAmbiences);
                    break;

                case AmbienceOrderByEnum.Name:
                    SelectedProjectModel.AmbiencesModel.OrderBy(x => x.Name).ToList().CopyTo(arrayAmbiences);
                    break;
            }

            SelectedProjectModel.AmbiencesModel.Clear();

            foreach (AmbienceModel ambience in arrayAmbiences)
            {
                SelectedProjectModel.AmbiencesModel.Add(ambience);
            }
        }

        private void SortTabs(object obj)
        {
            if (obj is null)
            {
                return;
            }
            if (obj is not TabControl)
            {
                return;
            }

            TabControl control = obj as TabControl;

            List<TabItem> tabs = control.Items.Cast<TabItem>().ToList();

            control.Items.Clear();

            if (_configurationRepository.ConfigurationApp.OrderProjectDetailTabs is null)
            {
                foreach (TabItem item in tabs)
                {
                    _ = control.Items.Add(item);
                }

                SaveTabOrder(control.Items);

                return;
            }

            if (_configurationRepository.ConfigurationApp.OrderProjectDetailTabs.Count() != tabs.Count)
            {
                foreach (TabItem item in tabs)
                {
                    _ = control.Items.Add(item);
                }

                SaveTabOrder(control.Items);

                return;
            }

            foreach (string name in _configurationRepository.ConfigurationApp.OrderProjectDetailTabs)
            {
                TabItem tab = tabs.FirstOrDefault(x => x.Name == name);

                if (tab != null)
                {
                    _ = control.Items.Add(tab);
                }
                else
                {
                    control.Items.Clear();

                    foreach (TabItem item in tabs)
                    {
                        _ = control.Items.Add(item);
                    }

                    SaveTabOrder(control.Items);

                    break;
                }
            }
        }

        private async void UnLink()
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Unlinking,
                                     message: Domain.Properties.Resources.Unlinking_Project_License,
                                     cancel: async () => await CloseDialog());

                await _projectRepository.UnLinkProjectLicense(SelectedProjectModel);

                OpenCustomMessageBox(header: Domain.Properties.Resources.Well_Done,
                                     message: Domain.Properties.Resources.Unlinked_Project_License,
                                     ok: async () => await CloseDialog());

                if (SelectedProjectModel.Licenses.Any())
                {
                    SelectedProjectModel.Licenses.Clear();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void UnLinkAsync(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Unlink,
                                 message: Domain.Properties.Resources.You_Want_Unlink_License,
                                 cancel: async () => await CloseDialog(),
                                 textButtomOk: Domain.Properties.Resources._Unlink,
                                 ok: () => UnLink());
        }

        private async void UpdateProject(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Updating,
                                     message: Domain.Properties.Resources.Updating_The_Project);

                await _projectRepository.Update(SelectedProjectModel);

                await CloseDialog();
            }
            catch (ParseException ex)
            {
                switch (ex.Code)
                {
                    case ParseException.ErrorCode.ObjectNotFound:
                        _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
                        ShowError(ex);
                        break;

                    default:
                        ShowError(ex);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}