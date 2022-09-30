using FC.Domain._Util;
using FC.Domain.Model.License;
using FC.Domain.Model.User;
using FC.Domain.Service;
using Parse;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface IUserRepository
    {
        Task<ParseUserCustom> FindOwnerUserByEmail(string emailOwner, CancellationTokenSource cancellationTokenSource);

        Task FindUserByEmail<TFoundList>(TFoundList foundList, string email);

        Task<ParseUserCustom> FindUserByEmail(string email);

        Task<ParseUserCustom> GetUserByObjectId(string objectId, CancellationTokenSource cts);

        Task GetUsers(ObservableCollection<ParseUserCustom> addedUsers, LicenseModel selectedItemLicenseModel, CancellationTokenSource cts);

        Task<bool> IsSoftwareAdministrator(CancellationTokenSource cts);
    }

    public class UserRepository : IUserRepository
    {
        public UserRepository(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<ParseUserCustom> FindOwnerUserByEmail(string emailOwner, CancellationTokenSource cancellationTokenSource)
        {
            if (string.IsNullOrEmpty(emailOwner))
            {
                throw new System.ArgumentException(Properties.Resources.Email_Field_Empty, nameof(emailOwner));
            }

            if (cancellationTokenSource is null)
            {
                throw new System.ArgumentNullException(nameof(cancellationTokenSource));
            }

            ParseUser parseUser = await ParseUser.Query.WhereEqualTo(AppConstants.PARSEUSEREMAIL, emailOwner)
                                                       .FirstOrDefaultAsync();
            using ParseUserCustom parseUserCustom = new()
            {
                ParseUser = parseUser
            };

            return parseUserCustom;
        }

        public async Task FindUserByEmail<TFoundList>(TFoundList foundList, string email)
        {
            if (foundList is ObservableCollection<ParseUserCustom>)
            {
                ObservableCollection<ParseUserCustom> _foundList = foundList as ObservableCollection<ParseUserCustom>;

                IEnumerable<ParseUser> parseUsers = await ParseUser.Query.WhereEqualTo(AppConstants.PARSEUSEREMAIL, email)
                                                                         .FindAsync();
                foreach (ParseUser parseUser in parseUsers)
                {
                    if (_foundList.Where(x => x.ParseUser.ObjectId == parseUser.ObjectId).Any())
                    {
                        continue;
                    }

                    using ParseUserCustom user = new()
                    {
                        ParseUser = parseUser
                    };

                    _foundList.Add(user);
                }
            }
        }

        public async Task<ParseUserCustom> FindUserByEmail(string email)
        {
            ;

            ParseUser parseUser = await ParseUser.Query.WhereEqualTo(AppConstants.EMAILPARSEUSER, email).FirstOrDefaultAsync(_taskService.CancellationTokenSource.Token);

            if (parseUser is null)
            {
                return null;
            }

            using ParseUserCustom user = new()
            {
                ParseUser = parseUser
            };

            return user;
        }

        public async Task<ParseUserCustom> GetUserByObjectId(string objectId, CancellationTokenSource cts)
        {
            ParseUser parseUser = await ParseUser.Query.WhereEqualTo(AppConstants.PARSEOBJECTID, objectId).FirstOrDefaultAsync(cts.Token);

            using ParseUserCustom user = new();
            user.ParseUser = parseUser;
            return user;
        }

        public async Task GetUsers(ObservableCollection<ParseUserCustom> addedUsers, LicenseModel selectedItemLicenseModel, CancellationTokenSource cts)
        {
            if (addedUsers is null)
            {
                throw new System.ArgumentNullException(nameof(addedUsers));
            }

            if (selectedItemLicenseModel is null)
            {
                throw new System.ArgumentNullException(nameof(selectedItemLicenseModel));
            }

            if (cts is null)
            {
                throw new System.ArgumentNullException(nameof(cts));
            }

            IEnumerable<ParseUser> response = await (from user in ParseUser.Query
                                                     where selectedItemLicenseModel.UserProjects.Contains(user.ObjectId)
                                                     select user).FindAsync();

            selectedItemLicenseModel?.UserProjects.Clear();

            foreach (ParseUser item in response)
            {
                using ParseUserCustom user = new();
                user.IsAdd = false;
                addedUsers.Add(user);
            }
        }

        public async Task<bool> IsSoftwareAdministrator(CancellationTokenSource cts)
        {
            ParseRole queryRole = await (from role in ParseRole.Query
                                         where role.Name == AppConstants.SOFTWAREADMINISTRATORROLE
                                         select role).FirstAsync();

            IEnumerable<ParseUser> users = await queryRole.Users.Query.FindAsync();

            foreach (ParseUser user in users)
            {
                if (ParseUser.CurrentUser.ObjectId == user.ObjectId)
                {
                    return true;
                }
            }

            return false;
        }

        private readonly ITaskService _taskService;
    }
}