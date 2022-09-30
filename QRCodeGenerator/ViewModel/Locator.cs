using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using QRCodeGenerator.Repository;
using QRCodeGenerator.Service;
using QRCodeGenerator.View.Component;
using QRCodeGenerator.ViewModel.Generator;
using QRCodeGenerator.ViewModel.History;
using QRCodeGenerator.ViewModel.Settings;
using QRCodeGenerator.ViewModel.SetupEmail;
using QRCodeGenerator.ViewModel.Tab;

namespace QRCodeGenerator.ViewModel
{
    public class Locator
    {
        public Locator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<IFrameNavigationService, FrameNavigationService>();
            SimpleIoc.Default.Register<ITaskService, TaskService>();
            SimpleIoc.Default.Register<IQRCodeRepository, QRCodeRepository>();
            SimpleIoc.Default.Register<IMailRepository, MailRepository>();
            SimpleIoc.Default.Register<IEmailService, EmailService>();
            SimpleIoc.Default.Register<ILanguageRepository, LanguageRepository>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IColorService, ColorService>();
            SimpleIoc.Default.Register<CustomMessageBoxUserControl>();
            SimpleIoc.Default.Register<CustomMessageBoxViewModel>();
            SimpleIoc.Default.Register<ILocalDBRepository, LocalDBRepository>();
        }

        public GeneratorViewModel Generator
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<GeneratorViewModel>())
                {
                    SimpleIoc.Default.Register<GeneratorViewModel>();
                }
                return ServiceLocator.Current.GetInstance<GeneratorViewModel>();
            }
        }

        public HistoryViewModel History
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<HistoryViewModel>())
                {
                    SimpleIoc.Default.Register<HistoryViewModel>();
                }

                return ServiceLocator.Current.GetInstance<HistoryViewModel>();
            }
        }

        public MainViewModel MainView
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<MainViewModel>())
                {
                    SimpleIoc.Default.Register<MainViewModel>();
                }

                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SettingsViewModel Settings
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<SettingsViewModel>())
                {
                    SimpleIoc.Default.Register<SettingsViewModel>();
                }

                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        public SetupEmailViewModel SetupEmail
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<SetupEmailViewModel>())
                {
                    SimpleIoc.Default.Register<SetupEmailViewModel>();
                }

                return ServiceLocator.Current.GetInstance<SetupEmailViewModel>();
            }
        }

        public TabViewModel Tab
        {
            get
            {
                if (!SimpleIoc.Default.ContainsCreated<TabViewModel>())
                {
                    SimpleIoc.Default.Register<TabViewModel>();
                }

                return ServiceLocator.Current.GetInstance<TabViewModel>();
            }
        }

        public static void Cleanup()
        {
        }
    }
}