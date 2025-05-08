using ExpertBridge.Core.Queries;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
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
        reportedPosts = await GetDeletedPosts();
        await base.OnInitializedAsync();
    }

    private async Task<List<PostResponse>> GetDeletedPosts()
    {
        var posts = await _dbContext.Posts
            .IgnoreQueryFilters()
            .FullyPopulatedPostQuery(p => p.IsDeleted)
            .SelectPostResponseFromFullPost(null)
            .ToListAsync();

        return posts;
    }
}
