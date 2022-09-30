using FC.Domain._Base;
using FC.Domain.Repository.Util;
using System;
using System.ComponentModel;

namespace FC.Domain.Model
{
    public enum BuildGateway
    {
        [Description(UtilGateway.RELEASE)]
        Release,

        [Description(UtilGateway.DEBUG)]
        Debug,

        [Description(UtilGateway.RELEASEINTEGRATION)]
        ReleaseIntegration,

        [Description(UtilGateway.DEBUGINTEGRATION)]
        DebugIntegration,
    }

    public class FirmwareModel : ModelBase
    {
        private string _version;

        public string Name { get; set; }

        public string Version
        {
            get => _version;
            set
            {
                _version = value;
                VersionName = value.Replace("_", ".").Replace("/", "");
            }
        }

        public string VersionName { get; set; }

        public DateTime LastModified { get; set; }

        public override string ToString()
        {
            return $"{{T: FirmwareModel, Name:{Name}, Version: {Version}, LastModified: {LastModified}}}";
        }
    }

    public class BuildModel : ModelBase
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
}