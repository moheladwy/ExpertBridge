using ExpertBridge.Admin.ViewModels;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen.Blazor;

namespace ExpertBridge.Admin.Components.Pages;

public partial class PostTags
{
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly HybridCache _cache;
    public List<PostTagsViewModel> postTags;
    public int TotalActiveTags => postTags.Count(pt => pt.PostCount > 0);
    public int TotalInactiveTags => postTags.Count(pt => pt.PostCount == 0);
    private RadzenDataGrid<PostTagsViewModel>? grid;
    private bool isLoading = true;

    public PostTags(ExpertBridgeDbContext dbContext, HybridCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
        postTags = [];
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            var key = "AllPostTags";
            postTags = await _cache.GetOrCreateAsync(key,
            async (cancellationToken) =>
            {
                return await _dbContext.Tags
                    .AsNoTracking()
                    .Include(tag => tag.PostTags)
                    .Where(tag => tag.PostTags.Any())
                    .Select(tag => new PostTagsViewModel
                    {
                        TagId = tag.Id,
                        EnglishName = tag.EnglishName,
                        ArabicName = tag.ArabicName ?? string.Empty,
                        Description = tag.Description,
                        PostCount = tag.PostTags.Count
                    })
                    .OrderByDescending(tag => tag.PostCount)
                    .ToListAsync(cancellationToken);
            });
        }
        finally
        {
            isLoading = false;
        }
        await base.OnInitializedAsync();
    }
}
