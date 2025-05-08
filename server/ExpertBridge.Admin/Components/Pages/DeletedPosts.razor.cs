using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Data.Queries;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Admin.Components.Pages;

public sealed partial class DeletedPosts : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private List<PostResponse>? reportedPosts;

    public DeletedPosts(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
        reportedPosts = [];
    }

    protected override async Task OnInitializedAsync()
    {
        reportedPosts = await GetReportedPosts();
        await base.OnInitializedAsync();
    }

    private async Task<List<PostResponse>> GetReportedPosts()
    {
        var posts = await _dbContext.Posts
            .IgnoreQueryFilters()
            .FullyPopulatedPostQuery(p => p.IsDeleted)
            .SelectPostResponseFromFullPost(null)
            .ToListAsync();

        return posts;
    }
}
