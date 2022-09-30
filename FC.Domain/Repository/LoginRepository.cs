using FC.Domain._Util;
using FC.Domain.Service;
using Parse;
using System;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface ILoginRepository
    {
        Task<ParseUser> Login(string email, string password);

        Task LogOutAsync();

        Task RegisterUser(string userName, string password, string email);

        Task ResetPassword(string email);
    }

    public class LoginRepository : ILoginRepository
    {
        private readonly ITaskService _taskService;
        private readonly IParseService _parseService;

        public LoginRepository(ITaskService taskService, IParseService parseService)
        {
            _taskService = taskService;

            _parseService = parseService;
        }

        public async Task<ParseUser> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException();
            }

            _taskService.CancellationTokenSource = new System.Threading.CancellationTokenSource();

            return await ParseUser.LogInAsync(email, password, _taskService.CancellationTokenSource.Token);
        }

        public async Task LogOutAsync()
        {
            await ParseUser.LogOutAsync();
        }

        public async Task RegisterUser(string userName, string password, string email)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException();
            }

            ParseUser parseUser = new()
            {
                Username = userName,
                Password = password,
                Email = email,
            };

            parseUser[AppConstants.PARSEPICTUREUSER] = AppConstants.DEFAULTPICTUREUSER;

            await parseUser.SignUpAsync();
        }

        public async Task ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException($"'{nameof(email)}' não pode ser nulo ou vazio", nameof(email));
            }

            _taskService.CancellationTokenSource = new System.Threading.CancellationTokenSource();

            _parseService.IsSendingToCloud = true;

            await ParseUser.RequestPasswordResetAsync(email, _taskService.CancellationTokenSource.Token);

            _parseService.IsSendingToCloud = true;
        }
    }
}