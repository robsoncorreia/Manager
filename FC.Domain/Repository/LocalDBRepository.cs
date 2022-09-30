using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Model.Configution;
using FC.Domain.Model.Device;
using FC.Domain.Model.FlexCloudClone;
using FC.Domain.Model.IR;
using FC.Domain.Model.Project;
using FC.Domain.Model.ZXT600;
using LiteDB;
using Parse;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FC.Domain.Repository
{
    public interface ILocalDBRepository
    {
        List<CommonIP> GetAllMostRepeatedIP();

        bool Delete<T>(T obj);

        bool DeleteAll(string key);

        RadioModel FindOne(GatewayModel selectedDevice, RadioModel radio);

        ConfigurationApp FindOneConfigurationApp();

        ObservableCollection<CommandModel> GetAllCommands();

        void Update(GatewayModel selectedDevice, RadioModel radio);

        object Update(object obj);

        void SaveMostRepeatedIP(CommonIP commonIP);

        void DeleteSmartSearch();
    }

    public class LocalDBRepository : ILocalDBRepository
    {
        private readonly LiteDatabase _db;

        public LocalDBRepository()
        {
            _db = new(connectionString);
        }

        public bool Delete<T>(T obj)
        {
            if (obj is IRModel)
            {
                using IRModel irModel = obj as IRModel;
                ILiteCollection<IRModel> liteCollection = _db.GetCollection<IRModel>(IRMODEL);
                return liteCollection.Delete(irModel.IRModelID);
            }
            else if (obj is RadioModel)
            {
                using RadioModel model = obj as RadioModel;
                ILiteCollection<RadioModel> liteCollection = _db.GetCollection<RadioModel>(RadioModel.CLASS_NAME);
                return liteCollection.Delete(model.Id);
            }
            else if (obj is CommandModel temp)
            {
                ILiteCollection<CommandModel> liteCollection = _db.GetCollection<CommandModel>(AppConstants.LITEDBCOMMANDDATABASE);
                return liteCollection.Delete(temp.Id);
            }

            return false;
        }

        public bool DeleteAll(string key)
        {
            try
            {
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (key == nameof(CommandModel))
                {
                    ILiteCollection<CommandModel> collection = _db.GetCollection<CommandModel>(AppConstants.LITEDBCOMMANDDATABASE);

                    IEnumerable<CommandModel> commands;

                    if (ParseUser.CurrentUser is null)

                    {
                        commands = collection.Find(x => x.ParseUserObjectId.Equals("notFound"));

                        foreach (CommandModel command in commands)
                        {
                            _ = collection.Delete(command.Id);
                        }

                        return true;
                    }

                    commands = collection.Find(x => x.ParseUserObjectId.Equals(ParseUser.CurrentUser.ObjectId));

                    foreach (CommandModel command in commands)
                    {
                        _ = collection.Delete(command.Id);
                    }

                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public ConfigurationApp FindOneConfigurationApp()
        {
            if (ParseUser.CurrentUser == null)
            {
                return new ConfigurationApp();
            }

            ILiteCollection<ConfigurationApp> collection = _db.GetCollection<ConfigurationApp>("configurationApp");

            return collection.FindOne(Query.EQ(nameof(ConfigurationApp.ParseUserCurrentUserObjectId), ParseUser.CurrentUser.ObjectId)) ?? new ConfigurationApp();
        }

        public ObservableCollection<CommandModel> GetAllCommands()
        {
            string objectId = "notFound";

            if (ParseUser.CurrentUser?.ObjectId is not null)
            {
                objectId = ParseUser.CurrentUser?.ObjectId;
            }

            ILiteCollection<CommandModel> collection = _db.GetCollection<CommandModel>(AppConstants.LITEDBCOMMANDDATABASE);

            IEnumerable<CommandModel> commands = collection.Find(x => x.ParseUserObjectId.Equals(objectId));

            return commands.Any() ? new ObservableCollection<CommandModel>(commands) : new ObservableCollection<CommandModel>();
        }

        private readonly object _locker = new();

        public object Update(object obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            lock (_locker)
            {
                if (obj is ConfigurationApp)
                {
                    ILiteCollection<ConfigurationApp> collection = _db.GetCollection<ConfigurationApp>("configurationApp");

                    using ConfigurationApp configurationApp = obj as ConfigurationApp;

                    if (ParseUser.CurrentUser is null)
                    {
                        return null;
                    }

                    ConfigurationApp configuration = collection.FindOne(Query.EQ(nameof(ConfigurationApp.ParseUserCurrentUserObjectId), ParseUser.CurrentUser.ObjectId));

                    if (configuration is null)
                    {
                        configurationApp.ParseUserCurrentUserObjectId = ParseUser.CurrentUser.ObjectId;
                        return collection.Upsert(configurationApp);
                    }
                    else
                    {
                        return collection.Update(configurationApp);
                    }
                }
                else if (obj is CommandModel temp)
                {
                    temp.ParseUserObjectId ??= "notFound";

                    ILiteCollection<CommandModel> commands = _db.GetCollection<CommandModel>(AppConstants.LITEDBCOMMANDDATABASE);
                    try
                    {
                        return commands.Upsert(temp);
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                }
                else if (obj is RadioModel radio)
                {
                    ILiteCollection<RadioModel> rfs = _db.GetCollection<RadioModel>(RadioModel.CLASS_NAME);

                    RadioModel result = rfs.FindOne(x => x.MacAddress.Equals(radio.MacAddress) &&
                                                    x.MemoryId == radio.MemoryId);

                    if (result is null)
                    {
                        radio.ParseUserCurrentUserObjectId = ParseUser.CurrentUser.ObjectId;
                        return rfs.Insert(radio);
                    }
                    else
                    {
                        radio.ParseUserCurrentUserObjectId = result.ParseUserCurrentUserObjectId;
                        radio.Id = result.Id;
                        return rfs.Update(radio);
                    }
                }
                else if (obj is ProjectModel)
                {
                    using ProjectModel model = obj as ProjectModel;

                    ILiteCollection<ProjectModel> projects = _db.GetCollection<ProjectModel>("Project");

                    ProjectModel result = projects.FindOne(x => x.Id == model.Id);

                    if (result is null)
                    {
                        return projects.Insert(model);
                    }
                    else
                    {
                        model.Id = result.Id;
                        return projects.Update(model);
                    }
                }
                else if (obj is ZXTCodeModel)
                {
                    using ZXTCodeModel model = obj as ZXTCodeModel;

                    ILiteCollection<ZXTCodeModel> codes = _db.GetCollection<ZXTCodeModel>(ZXTCodeModel.CLASS_NAME);

                    ZXTCodeModel result = codes.FindOne(x => x.Id == model.Id);

                    if (result is null)
                    {
                        return codes.Insert(model);
                    }
                    else
                    {
                        model.Id = result.Id;
                        return codes.Update(model);
                    }
                }
                else
                {
                    throw new Exception(Properties.Resources.Type_Not_Supported_Method);
                }
            }
        }

        public void Update(GatewayModel selectedDevice, RadioModel radio)
        {
            if (selectedDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedDevice));
            }

            if (radio is null)
            {
                throw new ArgumentNullException(nameof(radio));
            }

            ILiteCollection<RadioModel> rfs = _db.GetCollection<RadioModel>(RadioModel.CLASS_NAME);

            radio.MacAddress = selectedDevice.LocalMacAddress;

            RadioModel result = rfs.FindOne(x => x.MacAddress == selectedDevice.LocalMacAddress &&
                                            x.TypeRF == radio.TypeRF &&
                                            x.MemoryId == radio.MemoryId);

            if (result is null)
            {
                _ = rfs.Insert(radio);
            }
            else
            {
                radio.Id = result.Id;
                _ = rfs.Update(radio);
            }
        }

        public RadioModel FindOne(GatewayModel selectedDevice, RadioModel radio)
        {
            if (selectedDevice is null)
            {
                throw new ArgumentNullException(nameof(selectedDevice));
            }

            if (radio is null)
            {
                throw new ArgumentNullException(nameof(radio));
            }

            ILiteCollection<RadioModel> rfs = _db.GetCollection<RadioModel>(RadioModel.CLASS_NAME);

            return rfs.FindOne(x => x.MacAddress == selectedDevice.LocalMacAddress && x.MemoryId == radio.MemoryId && radio.TypeRF == radio.TypeRF) is RadioModel temp
                ? temp
                : null;
        }

        public void SaveMostRepeatedIP(CommonIP commonIP)
        {
            ILiteCollection<CommonIP> collection = _db.GetCollection<CommonIP>(nameof(CommonIP));

            _ = collection.Insert(commonIP);
        }

        public List<CommonIP> GetAllMostRepeatedIP()
        {
            ILiteCollection<CommonIP> commands = _db.GetCollection<CommonIP>(nameof(CommonIP));

            IEnumerable<CommonIP> query = commands.FindAll();

            return query.Any() ? new List<CommonIP>(query) : new List<CommonIP>();
        }

        public void DeleteSmartSearch()
        {
            ILiteCollection<CommonIP> commands = _db.GetCollection<CommonIP>(nameof(CommonIP));

            IEnumerable<CommonIP> query = commands.FindAll();

            if (query.Any())
            {
                foreach (CommonIP item in query)
                {
                    _ = commands.Delete(item.ID);
                }
            }
        }

        public const string IRMODEL = "IRModel";
        private static readonly string fullPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\000004.db";
        private readonly string connectionString = @$"Filename={fullPath}; Connection=Shared;";
    }
}