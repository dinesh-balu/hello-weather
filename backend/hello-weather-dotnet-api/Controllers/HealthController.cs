using Microsoft.AspNetCore.Mvc;

namespace HelloWeatherApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet("healthz")]
        public IActionResult GetHealth()
        {
            return Ok(new { status = "ok" });
        }
    }
}
