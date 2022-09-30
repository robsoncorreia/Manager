using FC.Domain.Model.Project;

namespace FC.Domain.Service
{
    public interface ISoftwareAdministratorService
    {
        AmbienceImageModel SelectedAmbienceModel { get; set; }
    }

    public class SoftwareAdministratorService : ISoftwareAdministratorService
    {
        public AmbienceImageModel SelectedAmbienceModel { get; set; }
    }
}