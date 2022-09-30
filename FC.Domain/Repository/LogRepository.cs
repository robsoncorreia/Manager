using FC.Domain._Util;
using FC.Domain.Model;
using FC.Domain.Service;
using Parse;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FC.Domain.Repository
{
    public interface ILogRepository
    {
        Task<bool> SaveError<T>(T obj, string appName, string appVersion);

        Task SaveLog(LogModel logModel);
    }

    public class LogRepository : ILogRepository
    {
        private readonly IParseService _parseService;

        public LogRepository(IParseService parseService)
        {
            _parseService = parseService;
        }

#if DEBUG
        private static readonly string ENV = Properties.Resources.DEV;
#else
        private readonly static string ENV = Properties.Resources.PROD;
#endif

        public async Task<bool> SaveError<T>(T obj, string appName, string appVersion)
        {
            try
            {
                if (obj is not Exception)
                {
                    return false;
                }

                Exception ex = obj as Exception;

                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                using StreamWriter outputFile = new(Path.Combine(docPath, $"{appName}.{appVersion}.{ENV}.txt"), true);

                await outputFile.WriteLineAsync($"---{DateTime.Now:hh:mm:ss MM/dd/yyyy}---").ConfigureAwait(true);

                await outputFile.WriteLineAsync(ex.ToString()).ConfigureAwait(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task SaveLog(LogModel logModel)
        {
            if (logModel is null)
            {
                throw new ArgumentNullException(nameof(logModel));
            }

            _parseService.IsSendingToCloud = true;

            ParseObject logParseObject = new(AppConstants.LOGCLASS)
            {
                [AppConstants.LOGDESCRIPTIONDATABASE] = logModel.Description,
                [AppConstants.LOGERRORCODEDATABASE] = (int)logModel.ErrorCode,
                [AppConstants.LOGMANUFACTURERKEYDATABASE] = logModel.ManufacturerKey,
                [AppConstants.LOGPRODUCTKEYDATABASE] = logModel.ProductKey,
                [AppConstants.LOGFIRMWAREVERSIONDATABASE] = logModel.FirmwareVersion,
                [AppConstants.LOGCUSTOMIDDATABASE] = logModel.CustomId,
            };

            await logParseObject.SaveAsync();

            _parseService.IsSendingToCloud = true;
        }
    }
}