using Microsoft.AspNetCore.Mvc;
using SimpleERP.GoogleDriveIntegration.Services.Drive;

namespace SimpleERP.GoogleDriveIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriveController : Controller
    {
        private readonly IDriveUploaderService _driveUploaderService;

        public DriveController(IDriveUploaderService driveUploaderService)
        {
            _driveUploaderService = driveUploaderService;
        }

        [HttpPost]
        [Route("Upload")]
        public IActionResult Upload()
        {
            string credentialsPath = "F:\\Projects\\SimpleERP\\google-drive-simple-erp\\simpleerp-390702-ec66d24e8cfb.json";
            string filePath = "F:\\Animes\\Wallpaper\\Wallpaper_Desktop\\wp2016699.jpg";
            string destinationFolderId = "1JWV3R3xNp4migCZV00GBPO4rb9c3t0v_";

            return _driveUploaderService.UploadFile(filePath, credentialsPath, destinationFolderId) ? 
                 Ok(): BadRequest();
        }
    }
}
