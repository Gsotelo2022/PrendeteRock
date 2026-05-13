using Microsoft.AspNetCore.Mvc;

namespace PrendeteRock.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "¡Bienvenido a PrendeteRock API!", status = "healthy" });
        }
    }
}
