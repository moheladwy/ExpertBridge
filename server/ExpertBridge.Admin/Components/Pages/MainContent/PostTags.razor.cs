using ExpertBridge.Admin.Components.SubModules;
using ExpertBridge.Admin.ViewModels;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;
using Radzen.Blazor;

namespace ExpertBridge.Admin.Components.Pages.MainContent;

public partial class PostTags
{
    private readonly HybridCache _cache;
    private readonly ExpertBridgeDbContext _dbContext;
    private readonly DialogService _dialogService;
    private readonly NotificationService _notificationService;
    private RadzenDataGrid<PostTagsViewModel> _grid;
    private bool _isLoading = true;
    private List<PostTagsViewModel> _postTags;

    public PostTags(
        ExpertBridgeDbContext dbContext,
        HybridCache cache,
        DialogService dialogService,
        NotificationService notificationService)
    {
        _dbContext = dbContext;
        _cache = cache;
        _dialogService = dialogService;
        _notificationService = notificationService;
        _postTags = [];
    }

    public int TotalActiveTags
    {
        get { return _postTags.Count(pt => pt.IsUsedAnywhere); }
    }

    public int TotalInactiveTags
    {
        get { return _postTags.Count(pt => !pt.IsUsedAnywhere); }
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _isLoading = true;
            const string key = "AllTags";
            _postTags = await _cache.GetOrCreateAsync(key,
                async cancellationToken =>
                {
                    return await _dbContext.Tags
                        .AsNoTracking()
                        .Include(tag => tag.PostTags)
                        .Include(tag => tag.ProfileTags)
                        .Include(tag => tag.JobPostingTags)
                        .Select(tag => new PostTagsViewModel
                        {
                            TagId = tag.Id,
                            EnglishName = tag.EnglishName,
                            ArabicName = tag.ArabicName ?? string.Empty,
                            Description = tag.Description,
                            PostCount = tag.PostTags.Count,
                            UserInterestCount = tag.ProfileTags.Count,
                            JobPostingCount = tag.JobPostingTags.Count
                        })
                        .OrderByDescending(tag => tag.PostCount + tag.UserInterestCount + tag.JobPostingCount)
                        .ToListAsync(cancellationToken);
                });
        }
        finally
        {
            _isLoading = false;
        }

        await base.OnInitializedAsync();
    }

    private async Task OpenEditDialog(PostTagsViewModel tag)
    {
        var parameters = new Dictionary<string, object>
        {
            { "TagId", tag.TagId },
            { "EnglishName", tag.EnglishName },
            { "ArabicName", tag.ArabicName },
            { "Description", tag.Description }
        };

        var result = await _dialogService.OpenAsync<EditTagDialog>("Edit Tag",
            parameters,
            new DialogOptions
            {
                Width = "600px",
                Height = "auto",
                Resizable = true,
                Draggable = true,
                CloseDialogOnOverlayClick = false
            });

        if (result is bool { } success && success)
        {
            // Refresh the data
            await RefreshData();
            _notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = "Tag updated successfully",
                Duration = 4000
            });
        }
    }

    private async Task DeleteTag(PostTagsViewModel tag)
    {
        var confirmed = await _dialogService.Confirm(
            $"Are you sure you want to delete the tag '{tag.EnglishName}'? This will remove all associations with posts, job postings, and user interests.",
            "Delete Tag",
            new ConfirmOptions { OkButtonText = "Delete", CancelButtonText = "Cancel", AutoFocusFirstElement = true });

        if (confirmed == true)
        {
            try
            {
                // Find the tag with all its relations
                var tagEntity = await _dbContext.Tags
                    .Include(t => t.PostTags)
                    .Include(t => t.ProfileTags)
                    .Include(t => t.JobPostingTags)
                    .FirstOrDefaultAsync(t => t.Id == tag.TagId);

                if (tagEntity == null)
                {
                    _notificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Tag not found",
                        Duration = 4000
                    });
                    return;
                }

                // Remove all PostTag relations
                if (tagEntity.PostTags.Count > 0)
                {
                    _dbContext.PostTags.RemoveRange(tagEntity.PostTags);
                }

                // Remove all UserInterest (ProfileTags) relations
                if (tagEntity.ProfileTags.Count > 0)
                {
                    _dbContext.UserInterests.RemoveRange(tagEntity.ProfileTags);
                }

                // Remove all JobPostingTag relations
                if (tagEntity.JobPostingTags.Count > 0)
                {
                    _dbContext.JobPostingTags.RemoveRange(tagEntity.JobPostingTags);
                }

                // Now delete the tag itself
                _dbContext.Tags.Remove(tagEntity);

                // Save changes
                await _dbContext.SaveChangesAsync();

                // Refresh the data
                await RefreshData();

                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = $"Tag '{tag.EnglishName}' and all its associations have been deleted successfully",
                    Duration = 4000
                });
            }
            catch (Exception ex)
            {
                _notificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"Failed to delete tag: {ex.Message}",
                    Duration = 4000
                });
            }
        }
    }

    private async Task RefreshData()
    {
        try
        {
            _isLoading = true;
            // Clear cache
            await _cache.RemoveAsync("AllTags");

            const string key = "AllTags";
            _postTags = await _cache.GetOrCreateAsync(key,
                async cancellationToken =>
                {
                    return await _dbContext.Tags
                        .AsNoTracking()
                        .Include(tag => tag.PostTags)
                        .Include(tag => tag.ProfileTags)
                        .Include(tag => tag.JobPostingTags)
                        .Select(tag => new PostTagsViewModel
                        {
                            TagId = tag.Id,
                            EnglishName = tag.EnglishName,
                            ArabicName = tag.ArabicName ?? string.Empty,
                            Description = tag.Description,
                            PostCount = tag.PostTags.Count,
                            UserInterestCount = tag.ProfileTags.Count,
                            JobPostingCount = tag.JobPostingTags.Count
                        })
                        .OrderByDescending(tag => tag.PostCount + tag.UserInterestCount + tag.JobPostingCount)
                        .ToListAsync(cancellationToken);
                });
        }
        finally
        {
            _isLoading = false;
        }
    }
}
