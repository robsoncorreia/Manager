using CommunityToolkit.Mvvm.Input;
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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FC.Manager.ViewModel.Project.Create
{
    public class CreateProjectViewModel : ProjectViewModelBase, IDropTarget
    {
        private readonly INetworkService _internetService;

        public CreateProjectViewModel(IFrameNavigationService navigationService,
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

            CreateCommand = new RelayCommand<object>(Create);

            NavigateBeforeCommand = new RelayCommand<object>(NavigateBefore);

            RemoveUserCommand = new RelayCommand<object>(RemoveUser);

            SelectColorCommand = new RelayCommand<object>(SelectColor);

            FindUserByEmailCommand = new RelayCommand<object>(FindUserByEmail);

            TextBoxEnterCommand = new RelayCommand<object>(TextBoxEnter);

            LoadedCommand = new RelayCommand<object>(Loaded);
        }

        private void TextBoxEnter(object obj)
        {
            if (obj is not string text)
            {
                return;
            }
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            Create(obj);
        }

        public ICommand FindUserByEmailCommand { get; set; }
        public ICommand TextBoxEnterCommand { get; set; }

        private async void Create(object obj)
        {
            try
            {
                if (!await _internetService.IsInternet())
                {
                    throw new Exception(Domain.Properties.Resources.Without_Internet_Connection);
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Creating,
                                     message: Domain.Properties.Resources.Creating_Project);

                await _projectRepository.Insert(SelectedProjectModel);

                await CloseDialog();

                _projectService.SelectedProject = SelectedProjectModel;

                _navigationService.NavigateTo(Domain.Properties.Resources.Detail_Project);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void FindUserByEmail(object obj)
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

        private void Loaded(object obj)
        {
            SelectedProjectModel = new ProjectModel();

            using ParseUserCustom user = new()
            {
                IsAdministratorProject = true,
                ParseUser = ParseUser.CurrentUser
            };

            SelectedProjectModel.Programmers.Add(user);
        }

        private void NavigateBefore(object obj)
        {
            _navigationService.NavigateTo(Domain.Properties.Resources.Dashboard_Project);
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

        private void SelectColor(object obj)
        {
            if (obj is not SolidColorBrush brush)
            {
                return;
            }
            SelectedProjectModel.Red = brush.Color.R;
            SelectedProjectModel.Green = brush.Color.G;
            SelectedProjectModel.Blue = brush.Color.B;
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
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            ParseUserCustom msp = (ParseUserCustom)dropInfo.Data;

            if (((IList<ParseUserCustom>)dropInfo.TargetCollection).FirstOrDefault(x => x.ParseUser.ObjectId == msp.ParseUser.ObjectId) != null)
            {
                return;
            }

            ((IList<ParseUserCustom>)dropInfo.TargetCollection).Insert(dropInfo.InsertIndex, msp);
        }

        #endregion DragDrop
    }
}