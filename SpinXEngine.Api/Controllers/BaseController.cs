using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SpinXEngine.Api.Controllers
{
    /// <summary>
    /// This is the Base Controller for LineManager Serve
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [Authorize]
    public class BaseController : ControllerBase
    {
        [AllowAnonymous]
        [MapToApiVersion("1.0")]
        [HttpGet]
        public IActionResult Heartbeat()
        {
            // Logger.Info("Heartbeat");
            return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
        }
    }
}
