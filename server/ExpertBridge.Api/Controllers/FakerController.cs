using ExpertBridge.Application.DataGenerator;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FakerController(
    ExpertBridgeDbContext _dbContext
    )
    : ControllerBase
{
    [HttpGet("generate")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return NoContent();
        Generator.SeedDatabase(_dbContext);

        return Ok();
    }
}
