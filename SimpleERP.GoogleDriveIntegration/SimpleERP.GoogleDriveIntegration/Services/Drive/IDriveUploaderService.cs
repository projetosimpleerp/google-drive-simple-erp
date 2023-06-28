namespace SimpleERP.GoogleDriveIntegration.Services.Drive
{
    public interface IDriveUploaderService
    {
        public bool UploadFile(string filePath, string credentialsFilePath, string destinationFolderId = null);
    }
}
