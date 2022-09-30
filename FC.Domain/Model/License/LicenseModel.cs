using FC.Domain._Base;
using FC.Domain._Util;
using FC.Domain.Model.Project;
using FC.Domain.Model.User;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FC.Domain.Model.License
{
    public enum AppEmail
    {
        [Description("Outlook")]
        Outlook,

        [Description("Email Windows 10")]
        EmailWindows10
    }

    public enum GroupByLog
    {
        [Description("Type")]
        Type,

        [Description("E-mail")]
        Email,

        [Description("Name")]
        Name,

        [Description("None")]
        None
    }

    public enum LicenseFilterTypeDateEnum
    {
        [Description("CreateAt")]
        CreateAt,

        [Description("Update")]
        Update,

        [Description("Expiration")]
        Expiration,
    }

    public enum LicenseTypeEnum
    {
        [Description("Commercial")]
        Commercial = 1,

        [Description("Development")]
        Development = 2
    }

    public enum LicenseTypeLog
    {
        [Description("Create")]
        Create,

        [Description("Update")]
        Update,

        [Description("Delete")]
        Delete,

        [Description("Updated")]
        Updated,

        [Description("Created")]
        Created,

        Deleted
    }

    public class LicenseModel : ModelBase
    {
        public IList<string> AppUsers
        {
            get => _appUsers;
            set
            {
                _appUsers = value;
                NotifyPropertyChanged();
            }
        }

        public ParseUserCustom CreateBy { get; set; }

        public DateTime? ExpirationDate
        {
            get => _expirationDate;
            set
            {
                _expirationDate = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (Equals(_isSelected, value))
                {
                    return;
                }
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsWriteAccess
        {
            get => _isWriteAccess;
            set
            {
                _isWriteAccess = value;
                NotifyPropertyChanged();
            }
        }

        public string LicenseKey
        {
            get => _licenseKey;
            set
            {
                _licenseKey = value;
                NotifyPropertyChanged();
            }
        }

        public LicenseTypeEnum LicenseType
        {
            get => _licenseType;
            set
            {
                _licenseType = value;
                NotifyPropertyChanged();
            }
        }

        public ParseUserCustom Owner
        {
            get => _userOwnerLicense;
            set
            {
                _userOwnerLicense = value;
                NotifyPropertyChanged();
            }
        }

        public ParseObject ParseObject { get; set; }

        public ProjectModel Project { get; set; }

        public ParseObject ProjectParseObject { get; set; }

        public int SelectedIndexType
        {
            get => _selectedIndexType;
            set
            {
                _selectedIndexType = value;
                NotifyPropertyChanged();
            }
        }

        public int SessionsPerUser
        {
            get => _sessionsPerUser;
            set
            {
                if (value < int.Parse(AppConstants.MINIMUMSESSIONSPERUSER) || Equals(value, _sessionsPerUser))
                {
                    return;
                }

                _sessionsPerUser = value;

                NotifyPropertyChanged();
            }
        }

        public int UserLimit
        {
            get => userLimit;
            set
            {
                if (value < int.Parse(AppConstants.MINIMUMUSERLIMIT) || Equals(userLimit, value))
                {
                    return;
                }
                userLimit = value;
                NotifyPropertyChanged();
            }
        }

        public IList<string> UserProjects
        {
            get => _userProjects;
            set
            {
                _userProjects = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<ParseUserCustom> UsersProjectList { get; set; }

        public LicenseModel()
        {
            UsersProjectList = new ObservableCollection<ParseUserCustom>();

            Owner = new ParseUserCustom();
        }

        private IList<string> _appUsers = new List<string>();

        private DateTime? _expirationDate = DateTime.Now.AddYears(30);

        private bool _isSelected;

        private bool _isWriteAccess;

        private string _licenseKey = Guid.NewGuid().ToString();

        private LicenseTypeEnum _licenseType;

        private int _selectedIndexType;

        private int _sessionsPerUser = int.Parse(AppConstants.MINIMUMSESSIONSPERUSER);

        private ParseUserCustom _userOwnerLicense;

        private IList<string> _userProjects = new List<string>();

        private int userLimit = int.Parse(AppConstants.MINIMUMUSERLIMIT);
    }
}