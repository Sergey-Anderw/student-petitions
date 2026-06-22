using Microsoft.AspNetCore.Mvc;

namespace StudentPetitions.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            application = "StudentPetitions.Api"
        });
    }
}
