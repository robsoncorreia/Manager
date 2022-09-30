using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.Device;
using FC.Domain.Model.Project;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace FC.Manager.ViewModel.Project.Gateway.Zwave.Test
{
    public class ZXT600TestViewModel : ProjectViewModel
    {
        #region Fields

        private int _Battery;
        private Brush _ForegroundBattery;
        private PackIconKind _PackIconBattery = PackIconKind.BatteryOff;
        private string _RoomTemperature;

        #endregion Fields

        public ZXT600TestViewModel(IFrameNavigationService navigationService,
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
            ThermostatModeSetCommand = new RelayCommand<object>(ThermostatModeSet);
            ThermostatFanSetCommand = new RelayCommand<object>(ThermostatFanSet);
            SetTemperatureCommand = new RelayCommand<object>(SetTemperature);
            GetTemperatureCommand = new RelayCommand<object>(GetTemperature);
            GetBatteryCommand = new RelayCommand<object>(GetBattery);

            SelectedProjectModel = _projectService.SelectedProject;
            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = project;
            });
        }

        public static int MaximumTemperature { get; } = 40;

        public static int MinimumTemperature { get; } = 1;

        public int Battery
        {
            get => _Battery;
            set
            {
                _ = SetProperty(ref _Battery, value);
                SetPackIconBattery(value);
            }
        }

        public Brush ForegroundBattery
        {
            get => _ForegroundBattery;
            set => SetProperty(ref _ForegroundBattery, value);
        }

        public ICommand GetBatteryCommand { get; set; }

        public ICommand GetTemperatureCommand { get; set; }

        public PackIconKind PackIconBattery
        {
            get => _PackIconBattery;
            set => SetProperty(ref _PackIconBattery, value);
        }

        public string RoomTemperature
        {
            get => _RoomTemperature;
            set => SetProperty(ref _RoomTemperature, value);
        }

        public ICommand SetTemperatureCommand { get; set; }

        public ICommand ThermostatFanSetCommand { get; set; }

        public ICommand ThermostatModeSetCommand { get; set; }

        private async void GetBattery(object obj = null)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     message: Domain.Properties.Resources.Checking_the_battery_level,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                if (await _zwaveRepository.GetBattery(SelectedProjectModel) is not int battery)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetBattery(),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                Battery = battery;

                await Task.Run(() => { while (count.IsRunnign) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetBattery(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private readonly CountDownTimer count = new();

        private async void GetTemperature(object obj = null)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Checking_ambient_temperature,
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     cancel: async () => await CloseDialog(),
                                     isProgressBar: true);

                if (await _zwaveRepository.GetMultiLevelSensorStatus(SelectedProjectModel,
                                                                       SensorTypeEnum.Temp,
                                                                       TemperatureScaleEnum.Celsius,
                                                                       pingGateway: false,
                                                                       pingZwave: false) is not JObject json)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetTemperature(),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                RoomTemperature = json.Value<int>(UtilZwave.VALUE).ToString().Length == 3 ? json.Value<int>(UtilZwave.VALUE).ToString().Insert(2, ".") : json.Value<int>(UtilZwave.VALUE).ToString();

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign) {; } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void SetPackIconBattery(int value)
        {
            if (value > 90)
            {
                PackIconBattery = PackIconKind.Battery100;
                ForegroundBattery = new SolidColorBrush(Colors.GreenYellow);
            }
            else if (value > 80)
            {
                ForegroundBattery = new SolidColorBrush(Colors.GreenYellow);
                PackIconBattery = PackIconKind.Battery90;
            }
            else if (value > 70)
            {
                ForegroundBattery = new SolidColorBrush(Colors.GreenYellow);
                PackIconBattery = PackIconKind.Battery80;
            }
            else if (value > 60)
            {
                ForegroundBattery = new SolidColorBrush(Colors.GreenYellow);
                PackIconBattery = PackIconKind.Battery70;
            }
            else if (value > 50)
            {
                ForegroundBattery = new SolidColorBrush(Colors.GreenYellow);
                PackIconBattery = PackIconKind.Battery60;
            }
            else if (value > 40)
            {
                ForegroundBattery = new SolidColorBrush(Colors.GreenYellow);
                PackIconBattery = PackIconKind.Battery50;
            }
            else if (value > 30)
            {
                ForegroundBattery = new SolidColorBrush(Colors.GreenYellow);
                PackIconBattery = PackIconKind.Battery40;
            }
            else if (value > 20)
            {
                ForegroundBattery = new SolidColorBrush(Colors.Yellow);
                PackIconBattery = PackIconKind.Battery30;
            }
            else if (value > 10)
            {
                ForegroundBattery = new SolidColorBrush(Colors.Red);
                PackIconBattery = PackIconKind.Battery20;
            }
            else if (value > 0)
            {
                ForegroundBattery = new SolidColorBrush(Colors.Red);
                PackIconBattery = PackIconKind.Battery10;
            }
            else if (value < 0)
            {
                ForegroundBattery = new SolidColorBrush(Colors.Red);
                PackIconBattery = PackIconKind.BatteryOff;
            }
        }

        private async void SetTemperature(object obj = null)
        {
            try
            {
                if (obj is not string @char)
                {
                    return;
                }

                if (@char == "+")
                {
                    ++SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.RoomTemperature;
                    return;
                }

                if (@char == "-")
                {
                    --SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.RoomTemperature;
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: string.Format(Domain.Properties.Resources.Changing_temperature_to__0__C, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.RoomTemperature),
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (!await _zwaveRepository.ThermostatTemperatureSet(selectedProject: SelectedProjectModel,
                                                                temperature: SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.RoomTemperature,
                                                                pingGateway: false,
                                                                pingZwave: false))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetTemperature("Enviar"),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void ThermostatFanSet(object obj = null)
        {
            try
            {
                if (obj is not ThermostatFanEnum thermostatFan)
                {
                    return;
                }
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: string.Format(Domain.Properties.Resources.Changing_fan_mode_to__0_, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.ThermostatFans.FirstOrDefault(x => (ThermostatFanEnum)x.Code == thermostatFan).Name),
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (!await _zwaveRepository.ThermostatFanSet(selectedProject: SelectedProjectModel,
                                                        thermostatFan: thermostatFan,
                                                        pingGateway: false,
                                                        pingZwave: false))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                        message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                        textButtonCustom: Domain.Properties.Resources.Try_again,
                                        custom: () => ThermostatFanSet(obj),
                                        textButtonCancel: Domain.Properties.Resources.Close,
                                        cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

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

        private async void ThermostatModeSet(object obj = null)
        {
            try
            {
                if (obj is not ThermostatModeEnum thermostatMode)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: string.Format(Domain.Properties.Resources.Changing_mode_to__0_, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.ThermostatModes.FirstOrDefault(x => (ThermostatModeEnum)x.Code == thermostatMode).Name),
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel);

                if (!await _zwaveRepository.ThermostatModeSet(selectedProject: SelectedProjectModel,
                                                         thermostatMode: thermostatMode,
                                                         pingGateway: false,
                                                         pingZwave: false))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                       message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                       textButtonCustom: Domain.Properties.Resources.Try_again,
                                       custom: () => ThermostatModeSet(obj),
                                       textButtonCancel: Domain.Properties.Resources.Close,
                                       cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
    }
}