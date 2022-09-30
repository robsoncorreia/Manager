using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Project;
using FC.Domain.Model.User;
using FC.Domain.Service;
using Parse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface IVoiceAssistantRepository
    {
        Task GetRemoteAccessStandaloneCommand(GatewayModel gateway);

        Task Remove(GatewayModel selectedGateway, Model.FCC.VoiceAssistantCommandModel voiceAssistantCommand);

        Task<bool> UpdateCommand(ProjectModel selectedProject);

        Task<bool> UpdateUsers(ProjectModel selectedProjectModel);

        Task UpdateUser(ParseUserCustom user);

        Task<bool> RemoveUser(ParseUserCustom custom);

        Task DeleteAllCommandsFromCloud(ProjectModel selectedProjectModel);

        Task CopyAllUserToCloud(ProjectModel selectedProjectModel);

        Task RemoveAllUsers(ProjectModel selectedProjectModel);
    }

    public class VoiceAssistantRepository : IVoiceAssistantRepository
    {
        private readonly IParseService _parseService;

        public VoiceAssistantRepository(IParseService parseService)
        {
            _parseService = parseService;
        }

        public async Task Remove(GatewayModel selectedGateway, Model.FCC.VoiceAssistantCommandModel voiceAssistantCommand)
        {
            if (selectedGateway is null)
            {
                throw new ArgumentNullException(nameof(selectedGateway));
            }

            if (voiceAssistantCommand is null)
            {
                throw new ArgumentNullException(nameof(voiceAssistantCommand));
            }
            _parseService.IsSendingToCloud = true;

            await voiceAssistantCommand.ParseObject.DeleteAsync();

            _ = selectedGateway.RemoteAccessStandaloneModel.CommandsCloud.Remove(voiceAssistantCommand);

            _parseService.IsSendingToCloud = false;
        }

        public async Task<bool> UpdateCommand(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            try
            {
                _parseService.IsSendingToCloud = true;

                if (selectedProject is null)
                {
                    throw new ArgumentNullException(nameof(selectedProject));
                }

                await selectedProject.RemoteAccessStandaloneToParseObject();

                await selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject.SaveAsync();

                selectedProject.SelectedGateway.ParseObject[AppConstants.GATEWAYREMOTEACCESSSTANDALONE] = selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject;

                await selectedProject.SelectedGateway.ParseObject.SaveAsync();

                _parseService.IsSendingToCloud = false;

                return true;
            }
            catch (ParseException ex)
            {
                if (ex.Code == ParseException.ErrorCode.ObjectNotFound)
                {
                    selectedProject.SelectedGateway.ParseObject = null;
                }

                return false;
            }
            catch (Exception)
            {
                _parseService.IsSendingToCloud = false;

                return false;
            }
        }

        public async Task GetRemoteAccessStandaloneCommand(GatewayModel gateway)
        {
            if (gateway is null)
            {
                throw new ArgumentNullException(nameof(gateway));
            }

            if (gateway.RemoteAccessStandaloneModel.ParseObject is null)
            {
                return;
            }

            if (gateway.RemoteAccessStandaloneModel.CommandsCloud.Any())
            {
                gateway.RemoteAccessStandaloneModel.CommandsCloud.Clear();
            }

            if (gateway.RemoteAccessStandaloneModel.Users.Any())
            {
                gateway.RemoteAccessStandaloneModel.Users.Clear();
            }

            _parseService.IsSendingToCloud = true;

            IEnumerable<ParseObject> commands = await gateway.RemoteAccessStandaloneModel.ParseObject.GetRelation<ParseObject>("commands").Query.FindAsync();

            IEnumerable<ParseObject> users = await gateway.RemoteAccessStandaloneModel.ParseObject.GetRelation<ParseObject>(AppConstants.REMOTEACCESSSTANDALONEUSERS).Query.Include("user").FindAsync();

            foreach (ParseObject command in commands)
            {
                gateway.RemoteAccessStandaloneModel.CommandsCloud.Add(command.ParseObjectToRemoteAccessStandaloneCommand());
            }

            foreach (ParseObject user in users)
            {
                gateway.RemoteAccessStandaloneModel.Users.Add(user.ParseObjectToRemoteAccessStandaloneUsers());
            }

            _parseService.IsSendingToCloud = false;
        }

        public async Task<bool> UpdateUsers(ProjectModel project)
        {
            try
            {
                if (project is null)
                {
                    return false;
                }

                if (project is null)
                {
                    throw new ArgumentNullException(nameof(project));
                }

                project.SelectedGateway.RemoteAccessStandaloneModel.ParseObject ??= new ParseObject(AppConstants.REMOTEACCESSSTANDALONECLASSNAME);

                foreach (ParseUserCustom user in project.SelectedGateway.RemoteAccessStandaloneModel.Users)
                {
                    user.ParseObject ??= new ParseObject(AppConstants.REMOTEACCESSSTANDALONEUSERSCLASSNAME);
                    user.ParseObject["user"] = user.ParseUser;
                    IDictionary<string, object> dictionary = new Dictionary<string, object>
                {
                    { "google", user.IsGoogleAssistant },
                    { "amazon", user.IsAmazonAssistant }
                };

                    user.ParseObject["assistants"] = dictionary;

                    await user.ParseObject.SaveAsync();
                }

                ParseRelation<ParseObject> relation = project.SelectedGateway.RemoteAccessStandaloneModel.ParseObject.GetRelation<ParseObject>(AppConstants.REMOTEACCESSSTANDALONEUSERS);

                foreach (ParseObject user in project.SelectedGateway.RemoteAccessStandaloneModel.Users.Select(x => x.ParseObject))
                {
                    relation.Add(user);
                }

                await project.SelectedGateway.RemoteAccessStandaloneModel.ParseObject.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task UpdateUser(ParseUserCustom user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _parseService.IsSendingToCloud = true;

            IDictionary<string, object> dictionary = new Dictionary<string, object>
            {
                { "google", user.IsGoogleAssistant },
                { "amazon", user.IsAmazonAssistant }
            };

            user.ParseObject["assistants"] = dictionary;

            await user.ParseObject.SaveAsync();

            _parseService.IsSendingToCloud = false;
        }

        public async Task<bool> RemoveUser(ParseUserCustom custom)
        {
            if (custom is null)
            {
                return false;
            }
            try
            {
                await custom.ParseObject.DeleteAsync();
                custom.ParseObject = null;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task DeleteAllCommandsFromCloud(ProjectModel selectedProjectModel)
        {
            if (selectedProjectModel is null)
            {
                throw new ArgumentNullException(nameof(selectedProjectModel));
            }

            _parseService.IsSendingToCloud = true;

            IEnumerable<ParseObject> temp = selectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.Select(x => x?.ParseObject);

            await ParseObject.DeleteAllAsync(temp);

            selectedProjectModel.SelectedGateway.RemoteAccessStandaloneModel.CommandsCloud.Clear();

            _parseService.IsSendingToCloud = false;
        }

        public async Task CopyAllUserToCloud(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            if (!selectedProject.Users.Any())
            {
                return;
            }

            _parseService.IsSendingToCloud = true;

            selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject ??= new ParseObject(AppConstants.REMOTEACCESSSTANDALONECLASSNAME);

            IList<ParseUserCustom> temp = new List<ParseUserCustom>();

            foreach (ParseUserCustom user in selectedProject.Users)
            {
                if (selectedProject.SelectedGateway.RemoteAccessStandaloneModel.Users.FirstOrDefault(x => x.ParseObject.ObjectId == user.ParseObject.ObjectId) is null)
                {
                    temp.Add(user);
                }
            }

            foreach (ParseUserCustom user in temp)
            {
                user.ParseObject ??= new ParseObject(AppConstants.REMOTEACCESSSTANDALONEUSERSCLASSNAME);
                user.ParseObject["user"] = user.ParseUser;
                IDictionary<string, object> dictionary = new Dictionary<string, object>
                {
                    { "google", user.IsGoogleAssistant },
                    { "amazon", user.IsAmazonAssistant }
                };

                user.ParseObject["assistants"] = dictionary;

                await user.ParseObject.SaveAsync();
            }

            ParseRelation<ParseObject> relation = selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject.GetRelation<ParseObject>(AppConstants.REMOTEACCESSSTANDALONEUSERS);

            foreach (ParseObject user in temp.Select(x => x.ParseObject))
            {
                relation.Add(user);
            }

            await selectedProject.SelectedGateway.RemoteAccessStandaloneModel.ParseObject.SaveAsync();

            foreach (ParseUserCustom user in temp)
            {
                selectedProject.SelectedGateway.RemoteAccessStandaloneModel.Users.Add(user);
            }

            _parseService.IsSendingToCloud = false;
        }

        public async Task RemoveAllUsers(ProjectModel selectedProject)
        {
            if (selectedProject is null)
            {
                throw new ArgumentNullException(nameof(selectedProject));
            }

            _parseService.IsSendingToCloud = true;

            IEnumerable<ParseObject> temp = selectedProject.SelectedGateway.RemoteAccessStandaloneModel.Users.Select(x => x?.ParseObject);

            await ParseObject.DeleteAllAsync(temp);

            selectedProject.SelectedGateway.RemoteAccessStandaloneModel.Users.Clear();

            foreach (ParseUserCustom user in selectedProject.Users)
            {
                user.ParseObject = null;
            }

            _parseService.IsSendingToCloud = false;
        }
    }
}