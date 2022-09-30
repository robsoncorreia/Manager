using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.IpCommand;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using FC.Domain.Util;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace FC.Manager.ViewModel.Project.Device.Edit
{
    public class IPCommandDetailDeviceViewModel : ProjectViewModelBase
    {
        private readonly CountDownTimer count = new();
        private IpCommandModel _ipCommandModel;
        private bool isTerminalEnable;

        public IPCommandDetailDeviceViewModel(IFrameNavigationService navigationService,
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
                                 ILoginRepository loginRepository) : base(navigationService, projectService, udpRepository, tcpRepository, irRepository, userService, projectRepository, localDBRepository, logRepository, userRepository, serialRepository, commandRepository, ipCommandRepository, gatewayService, configurationRepository, taskService, parseService, rfRepository, zwaveRepository, relayTestRepository, gatewayRepository, dialogService, loginRepository)
        {
            IpCommandModel = new IpCommandModel();

            PlayCommand = new RelayCommand<object>(Play);

            SaveInGatewayCommand = new RelayCommand<object>(SaveInGateway);

            SelectedIPCommand = new RelayCommand<object>(SelectedIP);

            DeleteAwaitCommand = new RelayCommand<object>(DeleteAwait);

            DeleteAllAwaitCommand = new RelayCommand<object>(DeleteAllAwait);

            NewCommand = new RelayCommand<object>(New);

            ExportToExcelCommand = new RelayCommand<object>(ExportToExcel);

            GetAllIPsGatewayCommand = new RelayCommand(async () => await GetAllIPsGateway());

            LoadedCommand = new RelayCommand<object>(Loaded);

            ExportToExcelGatewayListCommand = new RelayCommand<object>(ExportToExcelGatewayList);

            IsTerminalEnable = Domain.Properties.Settings.Default.enableTerminal;

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        public ICommand DeleteAllAwaitCommand { get; set; }
        public ICommand DeleteAwaitCommand { get; set; }
        public ICommand ExportToExcelCommand { get; set; }
        public ICommand ExportToExcelGatewayListCommand { get; set; }
        public ICommand GetAllIPsGatewayCommand { get; set; }

        public IpCommandModel IpCommandModel
        {
            get => _ipCommandModel;
            set => SetProperty(ref _ipCommandModel, value);
        }

        public bool IsTerminalEnable
        {
            get => isTerminalEnable;
            set => SetProperty(ref isTerminalEnable, value);
        }

        public ICommand NewCommand { get; set; }

        public ICommand PlayCommand { get; set; }

        public ICommand SaveInGatewayCommand { get; set; }

        public ICommand SelectedIPCommand { get; set; }

        private async Task Delete(IpCommandModel ipCommand)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Deleting_IP_command);

                await _ipCommandRepository.Delete(SelectedProjectModel.SelectedGateway, ipCommand);

                count.SetTime(1000);

                count.Restart();

                count.Start();

                count.CountDownFinished = async () =>
                {
                    await CloseDialog();
                };

                IpCommandModel = new IpCommandModel();

                SelectedProjectModel.SelectedGateway.SelectedIndexIPCommand = -1;
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
                                     message: Domain.Properties.Resources.Deleting_all_IP_commands);

                await _ipCommandRepository.DeleteAll(SelectedProjectModel.SelectedGateway);

                count.SetTime(1000);

                count.Restart();

                count.Start();

                count.CountDownFinished = async () =>
                {
                    await CloseDialog();
                };

                IpCommandModel = new IpCommandModel();

                SelectedProjectModel.SelectedGateway.SelectedIndexIPCommand = -1;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void DeleteAllAwait(object obj)
        {
            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete_All,
                                                        message: Domain.Properties.Resources.Delete_All_IP_Commands,
                                                        textButtonCustom: Domain.Properties.Resources.Delete,
                                                        textButtonCancel: Domain.Properties.Resources.Close,
                                                        cancel: async () => await CloseDialog(),
                                                        custom: async () => await DeleteAll());
        }

        private void DeleteAwait(object obj)
        {
            if (obj is not IpCommandModel temp)
            {
                return;
            }

            SelectedProjectModel.SelectedGateway.SelectedIndexIPCommand = SelectedProjectModel.SelectedGateway.IpCommands.IndexOf(temp);

            IpCommandModel = temp;

            OpenCustomMessageBox(header: Domain.Properties.Resources.Delete,
                                                        message: $"{Domain.Properties.Resources.Delete_IP_command} {((IpCommandModel)obj).MemoryId}",
                                                        textButtonCustom: Domain.Properties.Resources.Delete,
                                                        custom: async () => await Delete(temp),
                                                        textButtonCancel: Domain.Properties.Resources._Cancel,
                                                        cancel: async () => await CloseDialog());
        }

        private void ExportToExcel(List<IpCommandModel> ipCommands)
        {
            string[] lines = ExtensionMethods.ExportToExcel(ipCommands.ToList());

            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Excel file (*.csv)|*.csv",
                FileName = $"IR-{DateTime.Now:hhmmssddMMyyyy}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllLines(saveFileDialog.FileName, lines);

                Process process = new();

                process.StartInfo.FileName = saveFileDialog.FileName;

                _ = process.Start();

                process.WaitForExit();
            }
        }

        private async void ExportToExcel(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Exported_To,
                                     message: Domain.Properties.Resources.Excel_File_Created_List);

                ExportToExcel(SelectedProjectModel.SelectedGateway.IpCommands.ToList());

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void ExportToExcelGatewayList(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Exported_To,
                                     message: Domain.Properties.Resources.Excel_File_Created_List);

                string[] lines = ExtensionMethods.ExportToExcel(SelectedProjectModel.SelectedGateway.IpCommands.ToList());

                SaveFileDialog saveFileDialog = new()
                {
                    Filter = "Excel file (*.csv)|*.csv",
                    FileName = $"{Domain.Properties.Resources.Ip_Command}-{DateTime.Now:hhmmssddMMyyyy}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllLines(saveFileDialog.FileName, lines);
                }

                count.Reset();

                count.SetTime(4000);

                count.Start();

                count.CountDownFinished = () => { IsActiveSnackbar = false; };

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.File_saved;

                action = () =>
                {
                    OpenFile(saveFileDialog);
                    ActionContentSnackbar = null;
                    IsActiveSnackbar = false;
                };

                ActionContentSnackbar = Domain.Properties.Resources.Open;

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task GetAllIPsGateway()
        {
            try
            {
                IpCommandModel = new IpCommandModel();

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Loading,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog(),
                                     isProgressBar: true);

                await _ipCommandRepository.GetAll(selectedGateway: SelectedProjectModel.SelectedGateway,
                                                  isSendingToGateway: true);

                _gatewayService.IsSendingToGateway = false;

                count.Reset();

                count.SetTime(1000);

                count.Start();

                count.CountDownFinished = async () =>
                {
                    await CloseDialog();
                };

                SelectedProjectModel.SelectedGateway.SelectedIndexIPCommand = -1;
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: ex.Message,
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: async () => await GetAllIPsGateway(),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
            }
        }

        private async void Loaded(object obj)
        {
            SelectedProjectModel = _projectService.SelectedProject;

            if (obj is not UserControl view)
            {
                return;
            }

            if (!view.IsVisible)
            {
                return;
            }

            if (SelectedProjectModel.SelectedGateway.IpCommands.Any())
            {
                return;
            }

            await GetAllIPsGateway();
        }

        private void New(object obj)
        {
            IpCommandModel = new IpCommandModel();
            SelectedProjectModel.SelectedGateway.SelectedIndexIPCommand = -1;
        }

        private async void Play(object obj)
        {
            if (obj is not IpCommandModel ipCommand)
            {
                return;
            }

            ipCommand.IsSending = true;

            try
            {
                SelectedProjectModel.SelectedGateway.SelectedIndexIPCommand = SelectedProjectModel.SelectedGateway.IpCommands.IndexOf(ipCommand);

                IpCommandModel = ipCommand;

                IsActiveSnackbar = true;

                ContentSnackbar = Domain.Properties.Resources.Sending;

                await _ipCommandRepository.PlayMemory(selectedGateway: SelectedProjectModel.SelectedGateway,
                                                      selectedIpCommand: ipCommand);
                IsActiveSnackbar = false;

                ContentSnackbar = string.Format(Domain.Properties.Resources.Send_Sucess, Domain.Properties.Resources.Sent);

                IsActiveSnackbar = true;

                count.Reset();

                count.Start();

                count.SetTime(2000);

                count.CountDownFinished = () => { IsActiveSnackbar = false; };

                await CloseDialog();
            }
            catch (Exception ex)
            {
                IsActiveSnackbar = false;
                ShowError(ex);
            }
            finally
            {
                ipCommand.IsSending = false;
            }
        }

        private async void SaveInGateway(object obj)
        {
            try
            {
                _ = await _ipCommandRepository.SaveAsync(SelectedProjectModel.SelectedGateway, IpCommandModel);

                IpCommandModel = new IpCommandModel();

                SelectedProjectModel.SelectedGateway.SelectedIndexIPCommand = -1;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SelectedIP(object obj)
        {
            if (obj is not IpCommandModel temp)
            {
                return;
            }
            temp.CopyPropertiesTo(IpCommandModel);
        }
    }
}