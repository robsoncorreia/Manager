#define ZWAVE

using FC.Domain.Model;
using FC.Domain.Properties;
using FC.Domain.Util;
using Parse;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Markup;

namespace FC.Manager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsSoftwareAdministrator { get; private set; }

        public App()
        {
            Settings.Default.Reload();
            string language = ((LanguageEnum)Settings.Default.language).GetEnumDescription();
            CultureInfo culture = new(language);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            try
            {
                FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            }
            catch (Exception)
            {
            }

            ParseClient.Initialize(new ParseClient.Configuration
            {
#if DEBUG
                ApplicationId = "APPLICATION_ID",
                Server = "http://localhost:1337/parse/"
#else
                ApplicationId = "APPLICATION_ID",
                Server = "http://localhost:1337/parse/"
#endif
            });
            Settings.Default.isCurrentProcessAdmin = IsCurrentProcessAdmin();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                Settings.Default.Save();
            }
            finally
            {
                base.OnExit(e);
            }
        }

        //public bool IsCurrentProcessAdmin()
        //{
        //    using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        //    var principal = new System.Security.Principal.WindowsPrincipal(identity);
        //    return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        //}

        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsUserAnAdmin();

        private static bool IsCurrentProcessAdmin()
        {
            return IsUserAnAdmin();
        }

        private void AppStartup(object sender, StartupEventArgs e)
        {
            const string LICENSEMANAGERS = "SoftwareAdministrator";

            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i] == LICENSEMANAGERS)
                {
                    IsSoftwareAdministrator = true;
                }
            }
        }
    }
}