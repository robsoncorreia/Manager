using FC.Domain._Base;
using FC.Domain.Model.IfThen;
using FC.Domain.Model.License;
using FC.Domain.Model.User;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FC.Domain.Model.Project
{
    public enum ProjectOrderByEnum
    {
        [Description("Create At")]
        CreatedAt,

        [Description("Updated At")]
        UpdatedAt,

        [Description("Name")]
        Name
    }

    public class ProjectModel : ModelBase
    {
        public int Id { get; set; }

        public ObservableCollection<AmbienceModel> AmbiencesModel { get; set; }

        public ParseUser DeletedBy { get; internal set; }

        public ObservableCollection<GatewayModel> Devices { get; set; }

        public IList<string> FiledFor { get; set; }

        public bool IsDeleted
        {
            get => _isDeleted;
            set
            {
                if (Equals(_isDeleted, value))
                {
                    return;
                }
                _isDeleted = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<LicenseModel> Licenses { get; set; }

        public ObservableCollection<ParseUserCustom> MasterUsers { get; set; }

        public IEnumerable<ParseObject> ModulesParseObjects { get; internal set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public ParseObject ParseObject { get; set; }

        public string PrimaryColor
        {
            get => _primaryColor;
            set
            {
                _primaryColor = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ParseUserCustom> Programmers { get; set; }

        public AmbienceModel SelectedAmbienceModel
        {
            get => _selectedAmbienceModel;
            set
            {
                _selectedAmbienceModel = value;
                NotifyPropertyChanged();
            }
        }

        public GatewayModel SelectedGateway
        {
            get => _selectedDeviceModel;
            set
            {
                _selectedDeviceModel = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexAmbience
        {
            get => _selectedIndexAmbience;
            set
            {
                _selectedIndexAmbience = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexAmbienceGroup
        {
            get => _selectedIndexAmbienceGroup;
            set
            {
                _selectedIndexAmbienceGroup = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexDeviceModel
        {
            get => _selectedIndexDeviceModel;
            set
            {
                _selectedIndexDeviceModel = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexLicense
        {
            get => _selectedIndexLicense;
            set
            {
                _selectedIndexLicense = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedIndexModule
        {
            get => _selectedIndexModule;
            set
            {
                _selectedIndexModule = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<AmbienceModel> UnlinkedAmbiencesModel { get; set; }

        public ObservableCollection<ParseUserCustom> UserFoundSearch { get; set; }

        public ObservableCollection<ParseUserCustom> Users { get; set; }

        public string Version { get; set; } = "1.0";

        public ProjectModel()
        {
            UsersList = new List<string>();

            MasterUsersList = new List<string>();

            ProgrammersList = new List<string>();

            AmbiencesModel = new ObservableCollection<AmbienceModel>();

            DeletedAmbiences = new ObservableCollection<AmbienceModel>();

            ArchivedAmbiences = new ObservableCollection<AmbienceModel>();

            UnlinkedAmbiencesModel = new ObservableCollection<AmbienceModel>();

            Programmers = new ObservableCollection<ParseUserCustom>();

            MasterUsers = new ObservableCollection<ParseUserCustom>();

            Users = new ObservableCollection<ParseUserCustom>();

            UserFoundSearch = new ObservableCollection<ParseUserCustom>();

            Licenses = new ObservableCollection<LicenseModel>();

            Devices = new ObservableCollection<GatewayModel>();

            SelectedGateway = new GatewayModel();

            FiledFor = new List<string>();

            AllUsers = new List<ParseUser>();
        }

        public override string ToString()
        {
            return $"{Name} {Properties.Resources.CreateAt.ToLower()} {ParseObject?.CreatedAt:MM/dd/yyyy}";
        }

        private double _blue = 50;
        private double _green = 50;
        private bool _isDeleted;

        private LicenseModel _license;
        private string _name;
        private string _primaryColor = "#505050";
        private double _red = 50;
        private AmbienceModel _selectedAmbienceModel;
        private GatewayModel _selectedDeviceModel;
        private int _selectedIndexAmbience;
        private int _selectedIndexAmbienceGroup;
        private int _selectedIndexDeviceModel;
        private int _selectedIndexLicense;

        #region Colors

        public ObservableCollection<AmbienceModel> ArchivedAmbiences { get; set; }
        public ParseUser ArchivedBy { get; internal set; }

        public double Blue
        {
            get => _blue;
            set
            {
                _blue = value;
                NotifyPropertyChanged();
                ConvertColors();
            }
        }

        public double Green
        {
            get => _green;
            set
            {
                _green = value;
                NotifyPropertyChanged();
                ConvertColors();
            }
        }

        public IEnumerable<ParseUser> AllUsers { get; set; }

        public LicenseModel License
        {
            get => _license;
            set
            {
                _license = value;
                NotifyPropertyChanged();
            }
        }

        public double Red
        {
            get => _red;
            set
            {
                _red = value;
                NotifyPropertyChanged();
                ConvertColors();
            }
        }

        public ParseRole RoleUsers { get; set; }
        public ObservableCollection<AmbienceModel> DeletedAmbiences { get; set; }
        public IList<string> ProgrammersList { get; set; }
        public IList<string> MasterUsersList { get; set; }
        public IList<string> UsersList { get; set; }
        public ParseObject LicenseParseObject { get; set; }
        public IfThenModel SelectedIfThen { get; set; }
        //public IfThenModel SelectedSchedule { get; set; }

        public void ConvertColors(string primaryColor)
        {
            try
            {
                if (string.IsNullOrEmpty(primaryColor))
                {
                    throw new ArgumentException(Properties.Resources.Primary_Color_Null, nameof(primaryColor));
                }

                if (primaryColor.Length != 7)
                {
                    return;
                }

                if (double.TryParse(Convert.ToInt32(primaryColor.Substring(1, 2), 16).ToString(), out double red))
                {
                    Red = red;
                }
                if (double.TryParse(Convert.ToInt32(primaryColor.Substring(3, 2), 16).ToString(), out double green))
                {
                    Green = green;
                }
                if (double.TryParse(Convert.ToInt32(primaryColor.Substring(5, 2), 16).ToString(), out double blue))
                {
                    Blue = blue;
                }
            }
            catch (Exception)
            {
            }
        }

        private void ConvertColors()
        {
            PrimaryColor = $"#{(int)Red:X2}{(int)Green:X2}{(int)Blue:X2}";
        }

        #endregion Colors

        private int _selectedIndexModule;
    }
}