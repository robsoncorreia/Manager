using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Util;
using FC.Domain.Model.Device;
using FC.Domain.Model.Project;
using FC.Domain.Model.ZXT600;
using FC.Domain.Repository;
using FC.Domain.Repository.Gateway;
using FC.Domain.Repository.Zwave;
using FC.Domain.Service;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace FC.Manager.ViewModel.Project.Device.Zwave.Config.Flex
{
    public class ZXT600ConfigViewModel : ProjectViewModel
    {
        private int _Battery = -1;
        private bool _BuiltinIREmitterControl;
        private int _CheckIrCodeLearningStatus = -1;
        private bool _IsBlaster360;
        private bool _IsExternalIr;
        private bool _IsOpenDialogHostModel;
        private bool _IsSwing;
        private PackIconKind _PackIconBattery = PackIconKind.BatteryOff;
        private ZXTCodeModel _SelectedBrand;
        private int _SelectedIndexAutoReportCondition = -1;
        private int _SelectedIndexAutoReportConditionTimeInterval = -1;
        private int _SelectedIndexCalibrate = -1;
        private int _SelectedIndexCode = -1;
        private int codeBrand;
        private Brush _ForegroundBattery;

        public ZXT600ConfigViewModel(IFrameNavigationService navigationService,
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
            SelectedProjectModel = _projectService.SelectedProject;
            LearnCommand = new RelayCommand<object>(Learn);
            SetModelCommand = new RelayCommand<object>(SetModel);
            SetAutoReportConditionCommand = new RelayCommand<object>(SetAutoReportCondition);
            SetSwingCommand = new RelayCommand<object>(SetSwing);
            GetModelCommand = new RelayCommand<object>(GetModel);
            GetAutoReportConditionCommand = new RelayCommand<object>(GetAutoReportCondition);
            GetSwingCommand = new RelayCommand<object>(GetSwing);
            GetBatteryCommand = new RelayCommand<object>(GetBattery);
            SetBlaster360Command = new RelayCommand<object>(SetBlaster360);
            SetExternalIrCommand = new RelayCommand<object>(SetExternalIr);
            SelectionChangedBrandCommand = new RelayCommand<object>(SelectionChangedBrand);
            GetBlaster360Command = new RelayCommand<object>(GetBlaster360);
            GetExternalIrCommand = new RelayCommand<object>(GetExternalIr);
            GetCheckIrCodeLearningStatusCommand = new RelayCommand<object>(GetCheckIrCodeLearningStatus);
            GetBuiltinIREmitterCommand = new RelayCommand<object>(GetBuiltinIREmitter);
            SetBuiltinIREmitterControlCommand = new RelayCommand<object>(SetBuiltinIREmitter);
            SetCalibrateTemperatureReadingCommand = new RelayCommand<object>(SetCalibrateTemperatureReading);
            GetCalibrateTemperatureReadingCommand = new RelayCommand<object>(GetCalibrateTemperatureReading);
            GetAutoReportConditionTimeIntervalCommand = new RelayCommand<object>(GetAutoReportConditionTimeInterval);
            SetAutoReportConditionTimeIntervalCommand = new RelayCommand<object>(SetAutoReportConditionTimeInterval);
            Codes = new ObservableCollection<ZXTCodeModel>(JsonConvert.DeserializeObject<IList<ZXTCodeModel>>(Domain.Properties.Resources.ZXT600CODES));
            BrandsFound = new ObservableCollection<ZXTCodeModel>();
            GetMapping();

            SelectedProjectModel = _projectService.SelectedProject;

            WeakReferenceMessenger.Default.Register<ProjectModel>(this, (r, project) =>
            {
                SelectedProjectModel = _projectService.SelectedProject;
            });
        }

        public Brush ForegroundBattery
        {
            get => _ForegroundBattery;
            set => SetProperty(ref _ForegroundBattery, value);
        }

        public int Battery
        {
            get => _Battery;
            set
            {
                _ = SetProperty(ref _Battery, value);

                SetPackIconBattery(value);
            }
        }

        #region Collection

        public ObservableCollection<ZXTCodeModel> BrandsFound { get; set; }

        public ObservableCollection<ZXTCodeModel> Codes { get; set; }

        public IList<ZXTIRLearningMappingModel> IRsLearn { get; set; }

        #endregion Collection

        public int CheckIrCodeLearningStatus
        {
            get => _CheckIrCodeLearningStatus;
            set => SetProperty(ref _CheckIrCodeLearningStatus, value);
        }

        public bool IsBlaster360
        {
            get => _IsBlaster360;
            set => SetProperty(ref _IsBlaster360, value);
        }

        public bool IsBuiltinIREmitterControl
        {
            get => _BuiltinIREmitterControl;
            set => SetProperty(ref _BuiltinIREmitterControl, value);
        }

        public bool IsExternalIr
        {
            get => _IsExternalIr;
            set => SetProperty(ref _IsExternalIr, value);
        }

        public bool IsOpenDialogHostBrand
        {
            get => _IsOpenDialogHostModel;
            set => SetProperty(ref _IsOpenDialogHostModel, value);
        }

        public bool IsSwing
        {
            get => _IsSwing;
            set => SetProperty(ref _IsSwing, value);
        }

        public ICommand LearnCommand { get; set; }

        public PackIconKind PackIconBattery
        {
            get => _PackIconBattery;
            set => SetProperty(ref _PackIconBattery, value);
        }

        public ZXTCodeModel SelectedBrand
        {
            get => _SelectedBrand;
            set => SetProperty(ref _SelectedBrand, value);
        }

        public int SelectedIndexAutoReportCondition
        {
            get => _SelectedIndexAutoReportCondition;
            set => SetProperty(ref _SelectedIndexAutoReportCondition, value);
        }

        public int SelectedIndexAutoReportConditionTimeInterval
        {
            get => _SelectedIndexAutoReportConditionTimeInterval;
            set => SetProperty(ref _SelectedIndexAutoReportConditionTimeInterval, value);
        }

        public int SelectedIndexCalibrate
        {
            get => _SelectedIndexCalibrate;
            set => SetProperty(ref _SelectedIndexCalibrate, value);
        }

        public int SelectedIndexCode
        {
            get => _SelectedIndexCode;
            set => SetProperty(ref _SelectedIndexCode, value);
        }

        private async void Learn(object obj)
        {
            try
            {
                if (obj is not ZXTIRLearningMappingModel mappingModel)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                if (!await _zwaveRepository.Learn(SelectedProjectModel, mappingModel))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => Learn(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => Learn(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        #region Set Method

        private void SelectionChangedBrand(object obj)
        {
            if (SelectedBrand == null)
            {
                IsOpenDialogHostBrand = false;
                return;
            }

            SelectedBrand.SelectedIndexCode = SelectedBrand.Codes.IndexOf(codeBrand);

            SelectedIndexCode = Codes.IndexOf(SelectedBrand);

            IsOpenDialogHostBrand = false;
        }

        private async void SetAutoReportCondition(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: SelectedProjectModel,
                                                          parameter: 30,
                                                          size: 1,
                                                          value: SelectedIndexAutoReportCondition,
                                                          isPingGateway: false,
                                                          isPingZwave: false))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetAutoReportCondition(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetAutoReportCondition(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private async void SetAutoReportConditionTimeInterval(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: SelectedProjectModel,
                                                          parameter: 34,
                                                          size: 1,
                                                          value: SelectedIndexAutoReportConditionTimeInterval,
                                                          isPingGateway: false,
                                                          isPingZwave: false))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetAutoReportConditionTimeInterval(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetAutoReportConditionTimeInterval(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private async void SetBlaster360(object obj)
        {
            try
            {
                await _zwaveRepository.SetBlaster360(selectedProject: SelectedProjectModel,
                                                     isBlaster360: IsBlaster360,
                                                     pingGateway: false,
                                                     pingZwave: false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void SetBuiltinIREmitter(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: SelectedProjectModel,
                                                          parameter: 32,
                                                          size: 1,
                                                          value: IsBuiltinIREmitterControl ? 255 : 0,
                                                          isPingGateway: false,
                                                          isPingZwave: false))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => SetBuiltinIREmitter(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetBuiltinIREmitter(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private async void SetCalibrateTemperatureReading(object obj)
        {
            try
            {
                if (obj is not TemperatureOffsetValueEnum temperature)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: SelectedProjectModel,
                                                          parameter: 37,
                                                          size: 1,
                                                          value: (int)temperature,
                                                          isPingGateway: false,
                                                          isPingZwave: false))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetCalibrateTemperatureReading(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetCalibrateTemperatureReading(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private async void SetExternalIr(object obj)
        {
            try
            {
                await _zwaveRepository.SetExternalIr(selectedProject: SelectedProjectModel,
                                                     isExternalIr: IsExternalIr,
                                                     pingGateway: false,
                                                     pingZwave: false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void SetModel(object obj)
        {
            try
            {
                if (obj is not int code)
                {
                    return;
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                if (await _zwaveRepository.SetZwaveConfig(selectedProject: SelectedProjectModel,
                                                  parameter: 27,
                                                  size: 2,
                                                  value: code,
                                                  isPingGateway: false,
                                                  isPingZwave: false))
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => SetModel(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => SetModel(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
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

        private async void SetSwing(object obj)
        {
            try
            {
                _ = await _zwaveRepository.SetZwaveConfig(selectedProject: SelectedProjectModel,
                                                      parameter: 33,
                                                      size: 1,
                                                      value: IsSwing ? 1 : 0);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        #endregion Set Method

        #region Get Method

        private async void GetAutoReportCondition(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                int @return = await _zwaveRepository.GetZwaveConfig(selectedProject: SelectedProjectModel,
                                                                    parameter: 30);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetAutoReportCondition(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                SelectedIndexAutoReportCondition = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetAutoReportCondition(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private async void GetAutoReportConditionTimeInterval(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                int @return = await _zwaveRepository.GetZwaveConfig(selectedProject: SelectedProjectModel,
                                                                    parameter: 34,
                                                                    pingGateway: false,
                                                                    pingZwave: false);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetAutoReportConditionTimeInterval(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                SelectedIndexAutoReportConditionTimeInterval = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetAutoReportConditionTimeInterval(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private readonly CountDownTimer count = new();

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

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

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

        private async void GetBlaster360(object obj)
        {
            try
            {
                IsBlaster360 = await _zwaveRepository.GetBlaster360(selectedProject: SelectedProjectModel,
                                                                    pingGateway: false,
                                                                    pingZwave: false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetBuiltinIREmitter(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                int @return = await _zwaveRepository.GetZwaveConfig(selectedProject: SelectedProjectModel,
                                                                    parameter: 32);

                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetBuiltinIREmitter(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                IsBuiltinIREmitterControl = @return == 255;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetBuiltinIREmitter(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private async void GetCalibrateTemperatureReading(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                int @return = await _zwaveRepository.GetZwaveConfig(selectedProject: SelectedProjectModel,
                                                                    parameter: 37);
                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetCalibrateTemperatureReading(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                TemperatureOffsetValueEnum @enum = (TemperatureOffsetValueEnum)@return;

                SelectedIndexCalibrate = Array.IndexOf(Enum.GetValues(@enum.GetType()), @enum);

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetCalibrateTemperatureReading(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private async void GetCheckIrCodeLearningStatus(object obj)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                int @return = await _zwaveRepository.GetZwaveConfig(selectedProject: SelectedProjectModel,
                                                                    parameter: 37);
                if (@return < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetCheckIrCodeLearningStatus(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                CheckIrCodeLearningStatus = @return;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetCheckIrCodeLearningStatus(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        private async void GetExternalIr(object obj)
        {
            try
            {
                IsExternalIr = await _zwaveRepository.GetExternalIr(selectedProject: SelectedProjectModel,
                                                                    pingGateway: false,
                                                                    pingZwave: false);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void GetMapping()
        {
            IRsLearn = new List<ZXTIRLearningMappingModel>
            {
               new  ZXTIRLearningMappingModel{
                   StorageLocation = 0,
                   Name = Domain.Properties.Resources.Off
               },
               new  ZXTIRLearningMappingModel{
                   StorageLocation = 1,
                   Name = Domain.Properties.Resources.On__Resume_,
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 2,
                   Name = Domain.Properties.Resources._17C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 3,
                   Name = Domain.Properties.Resources._18C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 4,
                   Name = Domain.Properties.Resources._19C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 5,
                   Name = Domain.Properties.Resources._20C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 6,
                   Name = Domain.Properties.Resources._21C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 7,
                   Name = Domain.Properties.Resources._22C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 8,
                   Name = Domain.Properties.Resources._23C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 9,
                   Name = Domain.Properties.Resources._24C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 10,
                   Name = Domain.Properties.Resources._25C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 11,
                   Name = Domain.Properties.Resources._26C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 12,
                   Name = Domain.Properties.Resources._27C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 13,
                   Name = Domain.Properties.Resources._28C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 14,
                   Name = Domain.Properties.Resources._29C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 15,
                   Name = Domain.Properties.Resources._30C_COOL
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 16,
                   Name = Domain.Properties.Resources._17C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 17,
                   Name = Domain.Properties.Resources._18C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 18,
                   Name = Domain.Properties.Resources._19C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 19,
                   Name = Domain.Properties.Resources._20C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 20,
                   Name = Domain.Properties.Resources._21C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 21,
                   Name = Domain.Properties.Resources._22C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 22,
                   Name = Domain.Properties.Resources._23C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 23,
                   Name = Domain.Properties.Resources._24C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 24,
                   Name = Domain.Properties.Resources._25C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 25,
                   Name = Domain.Properties.Resources._26C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 26,
                   Name = Domain.Properties.Resources._27C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 27,
                   Name = Domain.Properties.Resources._28C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 28,
                   Name = Domain.Properties.Resources._29C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 29,
                   Name = Domain.Properties.Resources._30C_HEAT
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 30,
                   Name = Domain.Properties.Resources.DRY_MODE
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 31,
                   Name = Domain.Properties.Resources.AUTO_MODE
               },
                new  ZXTIRLearningMappingModel{
                   StorageLocation = 32,
                   Name = Domain.Properties.Resources.FAN_MODE
               },
            };
        }

        private async void GetModel(object obj = null)
        {
            try
            {
                if (BrandsFound.Count > 0)
                {
                    BrandsFound.Clear();
                }

                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     message: Domain.Properties.Resources.Checking_configured_code,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                codeBrand = await _zwaveRepository.GetZwaveConfig(selectedProject: SelectedProjectModel,
                                                                    parameter: 27,
                                                                    pingGateway: false,
                                                                    pingZwave: false);

                if (codeBrand < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                         message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                         textButtonCustom: Domain.Properties.Resources.Try_again,
                                         custom: () => GetModel(obj),
                                         textButtonCancel: Domain.Properties.Resources.Close,
                                         cancel: async () => await CloseDialog());
                    return;
                }

                ZXTCodeModel[] codesModel = Codes.Where(x => x.Codes.Contains(codeBrand)).ToArray();

                if (codesModel == null)
                {
                    await CloseDialog();
                    return;
                }

                if (codesModel.Length > 1)
                {
                    IsOpenDialogHostBrand = codesModel.Length > 1;

                    foreach (ZXTCodeModel brand in codesModel)
                    {
                        BrandsFound.Add(brand);
                    }
                    await CloseDialog();
                    return;
                }

                if (codesModel.FirstOrDefault(x => x.Codes.Contains(codeBrand)) is not ZXTCodeModel code)
                {
                    await CloseDialog();
                    return;
                }

                code.SelectedIndexCode = code.Codes.IndexOf(codeBrand);

                SelectedIndexCode = Codes.IndexOf(code);

                await CloseDialog();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async void GetSwing(object obj = null)
        {
            try
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Wait,
                                     cancel: async () => await CloseDialog(),
                                     textButtonCancel: Domain.Properties.Resources.Cancel,
                                     isProgressBar: true);

                int swing = await _zwaveRepository.GetZwaveConfig(selectedProject: SelectedProjectModel,
                                                                  parameter: 33);

                if (swing < 0)
                {
                    OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: string.Format(Domain.Properties.Resources._0__did_not_respond, SelectedProjectModel.SelectedGateway.SelectedZwaveDevice.Name),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     custom: () => GetSwing(obj),
                                     textButtonCancel: Domain.Properties.Resources.Close,
                                     cancel: async () => await CloseDialog());
                    return;
                }

                IsSwing = swing == 1;

                count.SetTime(1000);

                count.Start();

                await Task.Run(() => { while (count.IsRunnign && !IsCanceled) { } });

                await CloseDialog();
            }
            catch (Exception ex)
            {
                OpenCustomMessageBox(header: Domain.Properties.Resources.Error,
                                     message: IsCanceled ? Domain.Properties.Resources.Task_canceled : ex.Message,
                                     cancel: async () => await CloseDialog(),
                                     custom: () => GetSwing(obj),
                                     textButtonCustom: Domain.Properties.Resources.Try_again,
                                     textButtonCancel: Domain.Properties.Resources.Close);
            }
        }

        #endregion Get Method

        #region Set Command

        public ICommand SelectionChangedBrandCommand { get; set; }
        public ICommand SetAutoReportConditionCommand { get; set; }
        public ICommand SetAutoReportConditionTimeIntervalCommand { get; set; }
        public ICommand SetBlaster360Command { get; set; }
        public ICommand SetBuiltinIREmitterControlCommand { get; set; }
        public ICommand SetCalibrateTemperatureReadingCommand { get; set; }
        public ICommand SetExternalIrCommand { get; set; }
        public ICommand SetModelCommand { get; set; }
        public ICommand SetSwingCommand { get; set; }

        #endregion Set Command

        #region Get Command

        public ICommand GetAutoReportConditionCommand { get; set; }
        public ICommand GetAutoReportConditionTimeIntervalCommand { get; set; }
        public ICommand GetBatteryCommand { get; set; }
        public ICommand GetBlaster360Command { get; set; }
        public ICommand GetBuiltinIREmitterCommand { get; set; }
        public ICommand GetCalibrateTemperatureReadingCommand { get; set; }
        public ICommand GetCheckIrCodeLearningStatusCommand { get; set; }
        public ICommand GetExternalIrCommand { get; set; }
        public ICommand GetModelCommand { get; set; }
        public ICommand GetSwingCommand { get; set; }

        #endregion Get Command
    }
}