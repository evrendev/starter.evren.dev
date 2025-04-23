using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvrenDev.PublicApi.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/version")]
public class VersionController : ControllerBase
{

    [HttpGet]
    public IActionResult GetAll()
    {
        var version = Assembly.GetExecutingAssembly()?.GetName()?.Version?.ToString();
        var buildTime = System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);

        return Ok(new
        {
            version,
            buildTime = buildTime.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }
}
