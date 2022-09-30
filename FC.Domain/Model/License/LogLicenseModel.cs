using FC.Domain._Base;
using FC.Domain.Model.User;
using Parse;
using System.ComponentModel;

namespace FC.Domain.Model.License
{
    public enum FilterLogEmun
    {
        [Description("Name")]
        Name,

        [Description("Key")]
        Key,

        [Description("Email")]
        Email
    }

    public class LogLicenseModel : ModelBase
    {
        public LicenseModel License { get; set; }
        public ParseObject LicenseParseObject { get; set; }

        public LicenseTypeLog LicenseTypeLog
        {
            get => _licenseTypeLog;
            set
            {
                _licenseTypeLog = value;
                NotifyPropertyChanged();
            }
        }

        public ParseObject ParseObject { get; set; }
        public ParseUserCustom UpdatedBy { get; set; }
        private LicenseTypeLog _licenseTypeLog;
    }
}