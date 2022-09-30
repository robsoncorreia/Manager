using QRCodeGenerator.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QRCodeGenerator.Service
{
    public interface IFrameNavigationService
    {
        event PropertyChangedEventHandler PropertyChanged;

        string CurrentPageKey { get; }

        Frame CustomFrame { get; set; }

        void Configure(string key, Uri pageType);

        void GoBack();

        void NavigateTo(string pageKey);
    }

    public class FrameNavigationService : IFrameNavigationService, INotifyPropertyChanged
    {
        private readonly List<string> _historic;
        private readonly Dictionary<string, Uri> _pagesByKey;
        private string _currentPageKey;

        public FrameNavigationService()
        {
            _pagesByKey = new Dictionary<string, Uri>();
            _historic = new List<string>();
            Configure(Properties.Resources.DashBoard, new Uri(AppConstants.DASHBOARDPAGE, UriKind.Relative));
            Configure(AppConstants.TAB, new Uri(AppConstants.TABPAGE, UriKind.Relative));
            Configure(Properties.Resources.Dashboard_Project, new Uri(AppConstants.DASHBOARDPROJECTPAGE, UriKind.Relative));
            Configure(AppConstants.CREATEPROJECT, new Uri(AppConstants.CREATEPROJECTPAGE, UriKind.Relative));
            Configure(Properties.Resources.Detail_Project, new Uri(AppConstants.DETAILPROJECTPAGE, UriKind.Relative));
            Configure(AppConstants.LOGIN, new Uri(AppConstants.LOGINPAGE, UriKind.Relative));
            Configure(AppConstants.CONFIGURATION, new Uri(AppConstants.CONFIGURATIONPAGE, UriKind.Relative));
            Configure(Properties.Resources.Project, new Uri(AppConstants.PROJECTPAGE, UriKind.Relative));
            Configure(AppConstants.GATEWAY, new Uri(AppConstants.GATEWAYPAGE, UriKind.Relative));
            Configure(AppConstants.LICENSEMANAGER, new Uri(AppConstants.LICENSEMANAGERPAGE, UriKind.Relative));
            Configure(AppConstants.CREATELICENSE, new Uri(AppConstants.CREATELICENSEPAGE, UriKind.Relative));
            Configure(AppConstants.EDITLICENSE, new Uri(AppConstants.EDITLICENSEPAGE, UriKind.Relative));
            Configure(AppConstants.LOGLICENSE, new Uri(AppConstants.LOGLICENSEPAGE, UriKind.Relative));
            Configure(AppConstants.HISTORYLICENSE, new Uri(AppConstants.HISTORYLICENSEPAGE, UriKind.Relative));
            Configure(AppConstants.ACCOUNTSETTING, new Uri(AppConstants.ACCOUNTSETTINGPAGE, UriKind.Relative));
            Configure(AppConstants.DASHBOARDLICENSE, new Uri(AppConstants.DASHBOARDLICENSEPAGE, UriKind.Relative));
            Configure(AppConstants.CREATEAMBIENCE, new Uri(AppConstants.CREATEAMBIENCEPAGE, UriKind.Relative));
            Configure(AppConstants.DETAILAMBIENCE, new Uri(AppConstants.DETAILAMBIENCEPAGE, UriKind.Relative));
            Configure(AppConstants.ADDMODULE, new Uri(AppConstants.ADDMODULEPAGE, UriKind.Relative));
            Configure(AppConstants.DETAILMODULE, new Uri(AppConstants.DETAILMODULEPAGE, UriKind.Relative));
            Configure(Properties.Resources.Project_trash, new Uri(AppConstants.RECICLEBINPROJECTPAGE, UriKind.Relative));
            Configure(AppConstants.RECICLEBINAMBIENCE, new Uri(AppConstants.RECICLEBINAMBIENCEPAGE, UriKind.Relative));
            Configure(Properties.Resources.Archived_Project, new Uri(AppConstants.ARCHIVEDPROJECTPAGE, UriKind.Relative));
            Configure(AppConstants.ARCHIVEDAMBIENCE, new Uri(AppConstants.ARCHIVEDAMBIENCEPAGE, UriKind.Relative));
            Configure(AppConstants.ADDDEVICE, new Uri(AppConstants.ADDDEVICEPAGE, UriKind.Relative));
            Configure(Properties.Resources.Detail_Device, new Uri(AppConstants.DETAILGATEWAYPAGE, UriKind.Relative));
            Configure(AppConstants.SOFTWAREADMINISTRATOR, new Uri(AppConstants.SOFTWAREADMINISTRATORPAGE, UriKind.Relative));
            Configure(AppConstants.CREATEGROUPAMBIENCE, new Uri(AppConstants.CREATEGROUPAMBIENCEPAGE, UriKind.Relative));
            Configure(AppConstants.GENERATOR, new Uri(AppConstants.GENERATORPAGE, UriKind.Relative));
            Configure(AppConstants.SETUPEMAIL, new Uri(AppConstants.SETUPEMAILPAGE, UriKind.Relative));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentPageKey
        {
            get => _currentPageKey;

            private set
            {
                if (_currentPageKey == value)
                {
                    return;
                }
                _currentPageKey = value;
                OnPropertyChanged(nameof(CurrentPageKey));
            }
        }

        public Frame CustomFrame { get; set; }
        public object Parameter { get; private set; }

        public void Configure(string key, Uri pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(key))
                {
                    _pagesByKey[key] = pageType;
                }
                else
                {
                    _pagesByKey.Add(key, pageType);
                }
            }
        }

        public void GoBack()
        {
            if (_historic.Count > 1)
            {
                _historic.RemoveAt(_historic.Count - 1);
                NavigateTo(_historic.Last(), null);
            }
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public virtual void NavigateTo(string pageKey, object parameter)
        {
            lock (_pagesByKey)
            {
                if (!_pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No such page: {0} ", pageKey), "pageKey");
                }

                Frame frame = CustomFrame ?? GetDescendantFromName(Application.Current.MainWindow, "MainFrame") as Frame;

                if (frame != null)
                {
                    frame.Source = _pagesByKey[pageKey];
                }
                Parameter = parameter;
                _historic.Add(pageKey);
                CurrentPageKey = pageKey;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);

            if (count < 1)
            {
                return null;
            }

            for (int i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement frameworkElement)
                {
                    if (frameworkElement.Name == name)
                    {
                        return frameworkElement;
                    }

                    frameworkElement = GetDescendantFromName(frameworkElement, name);

                    if (frameworkElement != null)
                    {
                        return frameworkElement;
                    }
                }
            }
            return null;
        }
    }
}