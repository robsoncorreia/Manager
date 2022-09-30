using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace FC.Domain.Service
{
    public class GoogleApiService : IGoogleApiService
    {
        public DriveService LoginServiceAccountCredential()
        {
            string filePath = Path.GetTempPath() + "fc-config-f5a18effa839.p12";

            string[] Scopes = new[] { DriveService.Scope.DriveFile, DriveService.Scope.Drive };

            string serviceAccountEmail = "fc-config-z@fc-config.iam.gserviceaccount.com";

            File.WriteAllBytes(filePath, Properties.Resources.fc_config_f5a18effa839);

            X509Certificate2 certificate = new(filePath, "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new(
               new ServiceAccountCredential.Initializer(serviceAccountEmail)
               {
                   Scopes = Scopes
               }.FromCertificate(certificate));

            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "FC.CONFIG.Z",
            });
        }
    }

    public interface IGoogleApiService
    {
        public DriveService LoginServiceAccountCredential();
    }
}