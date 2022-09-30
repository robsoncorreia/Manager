using Google.Apis.Drive.v3;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FC.Domain.Service
{
    public class GoogleDriveService : IGoogleDriveService
    {
        public async Task<IList<Google.Apis.Drive.v3.Data.File>> GetFiles(DriveService servico, string path = "FC.CONFIG.Z")
        {
            FilesResource.ListRequest request = servico.Files.List();
            request.Fields = "files(id, name)";
            Google.Apis.Drive.v3.Data.FileList resultado = await request.ExecuteAsync();
            Google.Apis.Drive.v3.Data.File output = resultado.Files.FirstOrDefault(x => x.Name == path);
            request.Q = $"parents in '{output.Id}'";
            resultado = await request.ExecuteAsync();
            return resultado.Files;
        }

        public async Task<byte[]> GetFiles(DriveService driveService, string path, string fileName)
        {
            FilesResource.ListRequest requestList = driveService.Files.List();
            requestList.Fields = "files(id, name)";
            Google.Apis.Drive.v3.Data.FileList resultado = await requestList.ExecuteAsync();
            Google.Apis.Drive.v3.Data.File output = resultado.Files.FirstOrDefault(x => x.Name == path);
            requestList.Q = $"parents in '{output.Id}'";
            resultado = await requestList.ExecuteAsync();
            IList<Google.Apis.Drive.v3.Data.File> filesInDir = resultado.Files;
            if (!filesInDir.Any())
            {
                return null;
            }

            Google.Apis.Drive.v3.Data.File file = filesInDir.FirstOrDefault(x => x.Name.Contains(fileName));

            if (file is null)
            {
                return null;
            }

            string pathTemp = Path.GetTempPath() + file.Name;

            using (FileStream fileStream = new(pathTemp, FileMode.Create))
            {
                FilesResource.GetRequest request = driveService.Files.Get(file.Id);

                _ = await request.DownloadAsync(fileStream);
            }

            return File.ReadAllBytes(pathTemp);
        }
    }

    public interface IGoogleDriveService
    {
        Task<IList<Google.Apis.Drive.v3.Data.File>> GetFiles(DriveService servico, string path = "Changelog");

        Task<byte[]> GetFiles(DriveService driveService, string path, string fileName);
    }
}