using Microsoft.AspNetCore.Mvc;
using SimpleERP.GoogleDriveIntegration.Services.Drive;

namespace SimpleERP.GoogleDriveIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : Controller
    {
        [HttpGet]
        [Route("check")]
        public IActionResult Check()
        {
            return Ok("Online");
        }
    }
}
