using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace SimpleERP.GoogleDriveIntegration.Services.Drive
{
    public class DriveUploaderService : IDriveUploaderService
    {
        private DriveService GetDriveService(string credentialsFilePath)
        {
            GoogleCredential credential;

            using (var stream = new FileStream(credentialsFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(new[] { DriveService.Scope.DriveFile });
            }

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "SimpleERP Drive Integration"
            });

            return service;
        }

        public bool UploadFile(string filePath, string credentialsFilePath, string destinationFolderId = null)
        {
            try
            {
                var service = GetDriveService(credentialsFilePath);

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(filePath),
                    Parents = !string.IsNullOrEmpty(destinationFolderId) ? new[] { destinationFolderId } : null
                };

                FilesResource.CreateMediaUpload request;
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    request = service.Files.Create(fileMetadata, stream, "image/jpeg");
                    request.Fields = "id";
                    request.Upload();
                }

                var uploadedFile = request.ResponseBody;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }

        }
    }
}
