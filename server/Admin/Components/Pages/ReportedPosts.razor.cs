using Core.Entities.Posts;
using Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

namespace Admin.Components.Pages;

public sealed partial class ReportedPosts : ComponentBase
{
    private readonly ExpertBridgeDbContext _dbContext;
    private List<Post>? reportedPosts;

    public ReportedPosts(ExpertBridgeDbContext dbContext)
    {
        _dbContext = dbContext;
        reportedPosts = [];
    }

    protected override async Task OnInitializedAsync()
    {
        reportedPosts = await GetReportedPosts();
        await base.OnInitializedAsync();
    }

    private async Task<List<Post>> GetReportedPosts()
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(p => p.IsDeleted)
            .Include(p => p.Author)
            .Include(p => p.Comments)
            .Include(p => p.Votes)
            .Include(p=> p.Medias)
            .ToListAsync();

        return posts;
    }
}
