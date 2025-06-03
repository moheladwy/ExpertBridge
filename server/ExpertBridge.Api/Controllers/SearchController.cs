using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpertBridge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ExpertBridgeDbContext _dbContext;

    public SearchController(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("posts?query={query}")]
    public async Task<List<PostResponse>> SearchPosts([FromQuery] string query)
    {
        throw new NotImplementedException("Search functionality is not yet implemented.");
    }
}
