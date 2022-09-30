using CommunityToolkit.Mvvm.Messaging;
using FC.Domain._Base;
using FC.Domain.Model.Project;

namespace FC.Domain.Service
{
    public interface IProjectService
    {
        ProjectModel SelectedProject { get; set; }
    }

    public class ProjectService : ModelBase, IProjectService
    {
        private ProjectModel _selectedProject;

        public ProjectModel SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                _ = WeakReferenceMessenger.Default.Send(value);
            }
        }

        public ProjectService()
        {
            SelectedProject = new ProjectModel();
        }
    }
}