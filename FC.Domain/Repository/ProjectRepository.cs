using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Project;
using FC.Domain.Model.User;
using FC.Domain.Service;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface IProjectRepository
    {
        Task Archive(ProjectModel selectedProjectModel);

        Task<int> CountArchived();

        Task<int> CountRecicleBinAsync();

        Task Delete(ProjectModel project);

        Task GetAll(ObservableCollection<ProjectModel> projects);

        Task GetAllDeleted(ObservableCollection<ProjectModel> projects);

        Task GetArchivedProjects(ObservableCollection<ProjectModel> projects);

        Task GetProjectUsers(ProjectModel project);

        Task Insert(ProjectModel project);

        Task LinkLicenseWithProject(ProjectModel selectedProjectModel);

        Task PermanentlyDeleteAllProjects(ObservableCollection<ProjectModel> projects);

        Task PermanentlyDeleteProject(ProjectModel projectModel);

        Task Restore(ProjectModel projectModel);

        Task RestoreAll(ObservableCollection<ProjectModel> projects);

        Task Unarchive(ProjectModel projectModel);

        Task UnarchiveAll(ObservableCollection<ProjectModel> projects);

        Task UnLinkProjectLicense(ProjectModel selectedProjectModel);

        Task Update(ProjectModel project);

        Task DeleteAll(ObservableCollection<ProjectModel> projects);

        Task GetProjectInfo(ProjectModel selectedProjectModel);

        Task Rename(ProjectModel project);
    }

    public class ProjectRepository : IProjectRepository
    {
        public ProjectRepository(ITaskService taskService, IParseService parseService)
        {
            _parseService = parseService;
            _taskService = taskService;
        }

        public async Task Archive(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            if (selectedProjectModel.FiledFor.FirstOrDefault(x => x == ParseUser.CurrentUser.ObjectId) is null)
            {
                selectedProjectModel.FiledFor.Add(ParseUser.CurrentUser.ObjectId);
            }

            selectedProjectModel.ParseObject[AppConstants.PROJECTFILEDFOR] = selectedProjectModel.FiledFor;

            _taskService.CancellationTokenSource = new CancellationTokenSource();

            await selectedProjectModel.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);
        }

        public async Task<int> CountArchived()
        {
            //if (!Application.InternetGetConnectedState())
            //{
            //    throw new Exception(Properties.Resources.Without_Internet_Connection);
            //}

            ParseQuery<ParseObject> query = ParseObject.GetQuery(AppConstants.PROJECTCLASS);

            return await query.WhereContains(AppConstants.PROJECTFILEDFOR,
                                             ParseUser.CurrentUser.ObjectId).CountAsync();
        }

        public async Task<int> CountRecicleBinAsync()
        {
            ParseQuery<ParseObject> query = ParseObject.GetQuery(AppConstants.PROJECTCLASS);

            return await query.WhereEqualTo(AppConstants.PROJECTISDELETED, true)
                              .CountAsync()
                              ;
        }

        public async Task Delete(ProjectModel project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource(10000);

            project.ParseObject[AppConstants.PROJECTISDELETED] = true;

            project.ParseObject[AppConstants.PROJECTDELETEDBY] = ParseUser.CurrentUser;

            await project.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);
        }

        public async Task GetAll(ObservableCollection<ProjectModel> projects)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            ;

            ParseQuery<ParseObject> query = ParseObject.GetQuery(AppConstants.PROJECTCLASS)//ProjectFlex
                                                       .Include(AppConstants.PROJECTLICENSE)//license
                                                       .Include(AppConstants.PROJECTUSERSROLE);//usersRole

            IEnumerable<ParseObject> result = await query.WhereNotEqualTo(AppConstants.PROJECTISDELETED, true)//isDeleted
                                                         .WhereNotEqualTo(AppConstants.PROJECTUSERSROLE, null)//usersRole
                                                         .WhereNotContainedIn(AppConstants.PROJECTFILEDFOR, new string[] { ParseUser.CurrentUser.ObjectId })//filedFor
                                                         .FindAsync(_taskService.CancellationTokenSource.Token);

            foreach (ParseObject parseObject in result)
            {
                using ProjectModel projectModel = parseObject.ParseObjectToProject();

                if (projectModel is null)
                {
                    continue;
                }

                projects.Add(projectModel);
            }
        }

        public async Task GetAllDeleted(ObservableCollection<ProjectModel> projects)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            if (projects.Any())
            {
                projects.Clear();
            }

            ParseQuery<ParseObject> query = ParseObject.GetQuery(AppConstants.PROJECTCLASS)
                                                       .Include(AppConstants.PROJECTLICENSE)
                                                       .Include(AppConstants.PROJECTDELETEDBY)
                                                       .Include(AppConstants.PROJECTPROGRAMMERSUSERSROLE)
                                                       .Include(AppConstants.PROJECTMASTERUSERSROLE)
                                                       .Include(AppConstants.PROJECTUSERSROLE);

            IEnumerable<ParseObject> result = await query.WhereEqualTo(AppConstants.PROJECTISDELETED, true).FindAsync();

            foreach (ParseObject parseObject in result)
            {
                if (parseObject.ParseObjectToProject() is ProjectModel project)
                {
                    projects.Add(project);
                }
            }
        }

        public async Task GetArchivedProjects(ObservableCollection<ProjectModel> projects)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            projects.Clear();

            ParseQuery<ParseObject> query = ParseObject.GetQuery(AppConstants.PROJECTCLASS);

            query = query.Include(AppConstants.PROJECTARCHIVEDBY);

            IEnumerable<ParseObject> result = await query.WhereContains(AppConstants.PROJECTFILEDFOR, ParseUser.CurrentUser.ObjectId).FindAsync();

            foreach (ParseObject parseObject in result)
            {
                if (parseObject.ParseObjectToProject() is ProjectModel project)
                {
                    projects.Add(project);
                }
            }
        }

        public async Task GetProjectUsers(ProjectModel project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            project.AllUsers = await project.RoleUsers.Users.Query.FindAsync();

            if (project.Users.Any())
            {
                project.Users.Clear();
            }

            if (project.MasterUsers.Any())
            {
                project.MasterUsers.Clear();
            }

            if (project.Programmers.Any())
            {
                project.Programmers.Clear();
            }

            foreach (ParseUser user in project.AllUsers)
            {
                if (project.UsersList.Contains(user.ObjectId))
                {
                    using ParseUserCustom parseUserCustom = new()
                    {
                        ParseUser = user
                    };
                    project.Users.Add(parseUserCustom);
                }

                if (project.MasterUsersList.Contains(user.ObjectId))
                {
                    using ParseUserCustom parseUserCustom = new()
                    {
                        ParseUser = user
                    };
                    project.MasterUsers.Add(parseUserCustom);
                }

                if (project.ProgrammersList.Contains(user.ObjectId))
                {
                    using ParseUserCustom parseUserCustom = new()
                    {
                        ParseUser = user
                    };
                    project.Programmers.Add(parseUserCustom);
                }
            }
        }

        public async Task Insert(ProjectModel project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            project.ProjectToParseObject();

            await project.ParseObject.SaveAsync();

            ParseACL projectACL = new();

            ParseACL aclRoleProject = new()
            {
                PublicReadAccess = true
            };

            string usersRoleName = $"{AppConstants.PROJECTUSERSROLE}{project.ParseObject.ObjectId}";

            aclRoleProject.SetRoleWriteAccess(usersRoleName, true);

            ParseRole roleUsers = new(usersRoleName, aclRoleProject);

            foreach (ParseUserCustom programmer in project.Programmers)
            {
                roleUsers.Users.Add(programmer.ParseUser);
            }

            foreach (ParseUserCustom masterUser in project.MasterUsers)
            {
                roleUsers.Users.Add(masterUser.ParseUser);
            }

            foreach (ParseUserCustom user in project.Users)
            {
                roleUsers.Users.Add(user.ParseUser);
            }

            await roleUsers.SaveAsync();

            projectACL.SetRoleReadAccess(roleUsers, true);

            projectACL.SetRoleWriteAccess(roleUsers, true);

            project.ParseObject.ACL = projectACL;

            project.ParseObject[AppConstants.PROJECTUSERSROLE] = roleUsers;

            await project.ParseObject.SaveAsync();

            project.RoleUsers = roleUsers;
        }

        public async Task LinkLicenseWithProject(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            ParseObject selectedLicense = selectedProjectModel.Licenses.ElementAt(selectedProjectModel.SelectedIndexLicense).ParseObject;

            selectedLicense[AppConstants.LICENSEPROJECT] = selectedProjectModel.ParseObject;

            _taskService.CancellationTokenSource = new CancellationTokenSource();

            await selectedLicense.SaveAsync(_taskService.CancellationTokenSource.Token);

            selectedProjectModel.ParseObject[AppConstants.PROJECTLICENSE] = selectedLicense;

            await selectedProjectModel.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

            selectedProjectModel.License = selectedProjectModel.Licenses.ElementAt(selectedProjectModel.SelectedIndexLicense);

            selectedProjectModel.License.ParseObject = selectedLicense;
        }

        public async Task PermanentlyDeleteAllProjects(ObservableCollection<ProjectModel> projects)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            ProjectModel[] temp = projects.ToArray();

            for (int i = 0; i < temp.Length; i++)
            {
                await PermanentlyDeleteProject(temp[i]);
                _ = projects.Remove(temp[i]);
            }
        }

        public async Task PermanentlyDeleteProject(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            IEnumerable<ParseObject> groups = await selectedProject.ParseObject.GetRelation<ParseObject>(AppConstants.PROJECTFLOORS).Query
                                                          .FindAsync()
                                                          ;

            await ParseObject.DeleteAllAsync(groups);

            IEnumerable<ParseObject> ambiences = await ParseObject.GetQuery(AppConstants.AMBIENCECLASS)
                                             .WhereEqualTo(AppConstants.AMBIENCEPROJECT, selectedProject.ParseObject)
                                             .FindAsync()
                                             ;

            await ParseObject.DeleteAllAsync(ambiences);

            IEnumerable<ParseObject> devices = await ParseObject.GetQuery(AppConstants.GATEWAYCLASSNAME)
                                           .WhereEqualTo(AppConstants.DEVICEPROJECT, selectedProject.ParseObject)
                                           .FindAsync()
                                           ;

            await ParseObject.DeleteAllAsync(devices);

            await selectedProject.ParseObject.DeleteAsync();

            if (selectedProject.RoleUsers is not null)
            {
                await selectedProject.RoleUsers.DeleteAsync();
            }
        }

        public async Task Restore(ProjectModel projectModel)
        {
            if (projectModel is null)
            {
                throw new ArgumentNullException(nameof(projectModel));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource();

            projectModel.ParseObject[AppConstants.PROJECTISDELETED] = false;

            await projectModel.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

            projectModel.IsDeleted = false;
        }

        public async Task RestoreAll(ObservableCollection<ProjectModel> projects)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            IEnumerable<ProjectModel> parseObjects = projects.Select(x => { x.ParseObject[AppConstants.PROJECTISDELETED] = false; return x; });

            _taskService.CancellationTokenSource = new CancellationTokenSource();

            foreach (ProjectModel parseObject in parseObjects)
            {
                await parseObject.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);
            }
        }

        public async Task Unarchive(ProjectModel projectModel)
        {
            if (projectModel is null)
            {
                throw new ArgumentNullException(nameof(projectModel));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource();

            projectModel.FiledFor.RemoveAt(projectModel.FiledFor.IndexOf(ParseUser.CurrentUser.ObjectId));

            projectModel.ParseObject[AppConstants.PROJECTFILEDFOR] = projectModel.FiledFor;

            await projectModel.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);
        }

        public async Task UnarchiveAll(ObservableCollection<ProjectModel> projects)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            foreach (ProjectModel project in projects)
            {
                await Unarchive(project);
            }

            projects.Clear();
        }

        public async Task UnLinkProjectLicense(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource();

            selectedProjectModel.ParseObject[AppConstants.PROJECTLICENSE] = null;

            await selectedProjectModel.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

            selectedProjectModel.License.ParseObject[AppConstants.LICENSEPROJECT] = null;

            await selectedProjectModel.License.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

            selectedProjectModel.License = null;
        }

        public async Task Update(ProjectModel project)
        {
            if (project is null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            _taskService.CancellationTokenSource = new CancellationTokenSource(60000);

            ParseRelation<ParseUser> usersRelationUsers = project.RoleUsers.GetRelation<ParseUser>(AppConstants.USERSPARSEROLE);

            ParseQuery<ParseUser> usersRelationQueryUsers = usersRelationUsers.Query;

            IEnumerable<ParseUser> usersRoleUsers = await usersRelationQueryUsers.FindAsync(_taskService.CancellationTokenSource.Token);

            foreach (ParseUser user in usersRoleUsers)
            {
                usersRelationUsers.Remove(user);
            }

            foreach (ParseUserCustom user in project.Users)
            {
                project.RoleUsers.Users.Add(user.ParseUser);
            }

            foreach (ParseUserCustom user in project.MasterUsers)
            {
                project.RoleUsers.Users.Add(user.ParseUser);
            }

            foreach (ParseUserCustom user in project.Programmers)
            {
                project.RoleUsers.Users.Add(user.ParseUser);
            }

            await project.RoleUsers.SaveAsync(_taskService.CancellationTokenSource.Token);

            project.ProjectToParseObject();

            await project.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);
        }

        public async Task DeleteAll(ObservableCollection<ProjectModel> projects)
        {
            if (projects is null)
            {
                throw new ArgumentNullException(nameof(projects));
            }

            ProjectModel[] temp = projects.ToArray();

            for (int i = 0; i < temp.Length; i++)
            {
                _taskService.CancellationTokenSource = new CancellationTokenSource(10000);

                temp[i].ParseObject[AppConstants.PROJECTISDELETED] = true;

                temp[i].ParseObject[AppConstants.PROJECTDELETEDBY] = ParseUser.CurrentUser;

                await temp[i].ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);

                _ = projects.Remove(temp[i]);
            }
        }

        public async Task GetProjectInfo(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            _parseService.IsSendingToCloud = true;

            IEnumerable<ParseObject> devices = await selectedProjectModel.ParseObject.GetRelation<ParseObject>("gateways").Query
                                                                                                     .Include(AppConstants.GATEWAYREMOTEACCESSSTANDALONE)
                                                                                                     .Include("commands")
                                                                                                     .FindAsync();

            if (selectedProjectModel.Devices.Any())
            {
                selectedProjectModel.Devices.Clear();
            }

            foreach (ParseObject parseObject in devices)
            {
                if (parseObject.ParseObjectToGateway() is GatewayModel deviceModel)
                {
                    selectedProjectModel.Devices.Add(deviceModel);
                }
            }

            IEnumerable<ParseObject> ambiences = await selectedProjectModel.ParseObject.GetRelation<ParseObject>("ambiences").Query
                                                                                                        .WhereNotEqualTo(AppConstants.AMBIENCEISDELETED, true)
                                                                                                        .WhereNotContainedIn(AppConstants.AMBIENCEFILEDFOR, new string[] { ParseUser.CurrentUser.ObjectId })
                                                                                                        .FindAsync();

            if (selectedProjectModel.AmbiencesModel.Any())
            {
                selectedProjectModel.AmbiencesModel.Clear();
            }

            foreach (ParseObject parseObject in ambiences)
            {
                if (parseObject.ParseObjectToAmbience() is AmbienceModel ambience)
                {
                    selectedProjectModel.AmbiencesModel.Add(ambience);
                }
            }
        }

        public async Task Rename(ProjectModel project)
        {
            _taskService.CancellationTokenSource = new CancellationTokenSource(6000);
            project.ParseObject[AppConstants.PROJECTNAME] = project.Name;
            await project.ParseObject.SaveAsync(_taskService.CancellationTokenSource.Token);
        }

        private readonly ITaskService _taskService;

        private readonly IParseService _parseService;
    }
}