// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Application.DomainServices;
using ExpertBridge.Contract.Queries;
using ExpertBridge.Contract.Responses;
using ExpertBridge.Core.Entities;
using ExpertBridge.Core.Entities.Comments;
using ExpertBridge.Core.Entities.JobPostings;
using ExpertBridge.Core.Entities.ModerationReports;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Data.DatabaseContexts;
using ExpertBridge.Notifications;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;

namespace ExpertBridge.Admin.Components.Pages.ModerationReports;

public partial class ModerationReportDetail : ComponentBase
{
    private CommentResponse? _commentResponse;
    private bool _contentNotFound;
    private bool _isLoading = true;
    private bool _isLoadingContent = true;
    private JobPostingResponse? _jobPostingResponse;
    private PostResponse? _postResponse;
    private ModerationReport? _report;

    [Parameter] public required string Id { get; set; }

    [Inject] private ExpertBridgeDbContext DbContext { get; set; }

    [Inject] private HybridCache Cache { get; set; }

    [Inject] private DialogService DialogService { get; set; }

    [Inject] private NotificationService NotificationService { get; set; }

    [Inject] private NavigationManager NavigationManager { get; set; }

    [Inject] private ILogger<ModerationReportDetail> Logger { get; set; }

    [Inject] private NotificationFacade NotificationFacade { get; set; }

    [Inject] private ModerationReportService ModerationReportService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadReportAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isLoading)
        {
            await LoadReportAsync();
        }
    }

    private async Task LoadReportAsync()
    {
        _isLoading = true;
        _isLoadingContent = true;
        _contentNotFound = false;

        try
        {
            var key = $"admin:moderation-reports:{Id}";
            _report = await Cache.GetOrCreateAsync<ModerationReport?>(key,
                async cancellationToken =>
                {
                    return await DbContext.ModerationReports
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.Id == Id, cancellationToken);
                });

            if (_report != null)
            {
                await LoadContentAsync();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading moderation report {ReportId}", Id);
            ShowNotification(NotificationSeverity.Error, "Error", "Failed to load moderation report");
        }
        finally
        {
            _isLoading = false;
            _isLoadingContent = false;
        }
    }

    private async Task LoadContentAsync()
    {
        if (_report == null)
        {
            return;
        }

        try
        {
            switch (_report.ContentType)
            {
                case ContentTypes.Post:
                    await LoadPostAsync();
                    break;
                case ContentTypes.Comment:
                    await LoadCommentAsync();
                    break;
                case ContentTypes.JobPosting:
                    await LoadJobPostingAsync();
                    break;
                case ContentTypes.Profile:
                case ContentTypes.Message:
                case ContentTypes.Video:
                case ContentTypes.Image:
                case ContentTypes.File:
                default:
                    _contentNotFound = true;
                    break;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading content for report {ReportId}", Id);
            _contentNotFound = true;
        }
    }

    private async Task LoadPostAsync()
    {
        if (_report == null)
        {
            return;
        }

        try
        {
            var key = $"admin:posts:{_report.ContentId}";
            var post = await Cache.GetOrCreateAsync<Post?>(key, async cancellationToken =>
            {
                return await DbContext.Posts
                    .IgnoreQueryFilters()
                    .FullyPopulatedPostQuery()
                    .FirstOrDefaultAsync(p => p.Id == _report.ContentId, cancellationToken);
            });

            if (post is null)
            {
                _contentNotFound = true;
            }
            else
            {
                _postResponse = post.SelectPostResponseFromFullPost(_report.AuthorId);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading post {PostId}", _report.ContentId);
            _contentNotFound = true;
        }
    }

    private async Task LoadCommentAsync()
    {
        if (_report == null)
        {
            return;
        }

        try
        {
            var key = $"admin:comments:{_report.ContentId}";
            var comment = await Cache.GetOrCreateAsync<Comment?>(key, async cancellationToken =>
            {
                return await DbContext.Comments
                    .IgnoreQueryFilters()
                    .FullyPopulatedCommentQuery()
                    .FirstOrDefaultAsync(c => c.Id == _report.ContentId, cancellationToken);
            });

            if (comment is null)
            {
                _contentNotFound = true;
            }
            else
            {
                _commentResponse = comment.SelectCommentResponseFromFullComment(_report.AuthorId);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading comment {CommentId}", _report.ContentId);
            _contentNotFound = true;
        }
    }

    private async Task LoadJobPostingAsync()
    {
        if (_report == null)
        {
            return;
        }

        try
        {
            var key = $"admin:job-postings:{_report.ContentId}";
            var jobPost = await Cache.GetOrCreateAsync<JobPosting?>(key, async cancellationToken =>
            {
                return await DbContext.JobPostings
                    .IgnoreQueryFilters()
                    .FullyPopulatedJobPostingQuery()
                    .FirstOrDefaultAsync(jp => jp.Id == _report.ContentId, cancellationToken);
            });

            if (jobPost is null)
            {
                _contentNotFound = true;
            }
            else
            {
                _jobPostingResponse = jobPost.SelectJopPostingResponseFromFullJobPosting(_report.AuthorId);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading job posting {JobPostingId}", _report.ContentId);
            _contentNotFound = true;
        }
    }

    private async Task ToggleResolution()
    {
        if (_report == null)
        {
            return;
        }

        try
        {
            await ModerationReportService.ToggleResolution(_report);

            switch (_report.ContentType)
            {
                // Invalidate cache
                case ContentTypes.Post:
                    await Cache.RemoveAsync($"admin:posts:{_report.ContentId}");
                    break;
                case ContentTypes.Comment:
                    await Cache.RemoveAsync($"admin:comments:{_report.ContentId}");
                    break;
                case ContentTypes.JobPosting:
                    await Cache.RemoveAsync($"admin:job-postings:{_report.ContentId}");
                    break;
                case ContentTypes.Profile:
                case ContentTypes.Message:
                case ContentTypes.Video:
                case ContentTypes.Image:
                case ContentTypes.File:
                default:
                    break;
            }

            await Cache.RemoveAsync("admin:moderation-reports:all");
            await Cache.RemoveAsync($"admin:moderation-reports:{Id}");

            _report.IsResolved = !_report.IsResolved;
            var message = _report.IsResolved ? "Report marked as resolved" : "Report marked as unresolved";
            ShowNotification(NotificationSeverity.Success, "Success", message);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error toggling resolution for report {ReportId}", _report.Id);
            ShowNotification(NotificationSeverity.Error, "Error", "Failed to update report resolution status");
        }
    }

    private async Task ToggleIsNegative()
    {
        if (_report == null)
        {
            return;
        }

        var confirmationMessage = $"Are you sure you want to mark this {_report.ContentType} as {(_report.IsNegative ?
                                      $"Clean and undelete the {_report.ContentType}" :
                                      $"Negative and delete the {_report.ContentType}")}?";
        var confirmed = await DialogService.Confirm(
            confirmationMessage,
            "Confirm",
            new ConfirmOptions { OkButtonText = "Confirm", CancelButtonText = "Cancel" }
        );

        if (confirmed == true)
        {
            try
            {
                await Cache.RemoveAsync("admin:moderation-reports:all");
                await ModerationReportService.ToggleNegative(_report);

                if (_report.ContentType == ContentTypes.Post && _postResponse != null)
                {
                    await Cache.RemoveAsync($"admin:posts:{_postResponse.Id}");
                    await NotificationFacade.NotifyPostRestoredAsync(
                        new Post
                        {
                            Id = _postResponse.Id,
                            Title = _postResponse.Title,
                            Content = _postResponse.Content,
                            AuthorId = _report.AuthorId
                        }, _report);
                }
                else if (_report.ContentType == ContentTypes.Comment && _commentResponse != null)
                {
                    await Cache.RemoveAsync($"admin:comments:{_report.ContentId}");
                    await NotificationFacade.NotifyCommentRestoredAsync(
                        new Comment
                        {
                            Id = _commentResponse.Id,
                            Content = _commentResponse.Content,
                            AuthorId = _report.AuthorId
                        }, _report);
                }
                else if (_report.ContentType == ContentTypes.JobPosting && _jobPostingResponse != null)
                {
                    await Cache.RemoveAsync($"admin:job-postings:{_report.ContentId}");
                    await NotificationFacade.NotifyPostRestoredAsync(
                        new JobPosting
                        {
                            Id = _jobPostingResponse.Id,
                            Title = _jobPostingResponse.Title,
                            Content = _jobPostingResponse.Content,
                            AuthorId = _report.AuthorId
                        }, _report);
                }

                await Cache.RemoveAsync("admin:moderation-reports:all");
                await Cache.RemoveAsync($"admin:moderation-reports:{Id}");

                _report.IsNegative = !_report.IsNegative;
                var message = _report.IsNegative ?
                    $"{_report.ContentType} marked as negative and deleted" :
                    $"{_report.ContentType} marked as clean and restored";
                ShowNotification(NotificationSeverity.Success, "Success", message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error toggling negative flag for report {ReportId}", _report.Id);
                ShowNotification(NotificationSeverity.Error, "Error", "Failed to update report negative status");
            }
        }
    }

    private void NavigateBack()
    {
        NavigationManager.NavigateTo("/moderation-reports");
    }

    private static BadgeStyle GetContentTypeBadgeStyle(ContentTypes contentType)
    {
        return contentType switch
        {
            ContentTypes.Post => BadgeStyle.Success,
            ContentTypes.Comment => BadgeStyle.Info,
            ContentTypes.JobPosting => BadgeStyle.Warning,
            ContentTypes.Profile => BadgeStyle.Secondary,
            ContentTypes.Message => BadgeStyle.Primary,
            ContentTypes.Video => BadgeStyle.Danger,
            ContentTypes.Image => BadgeStyle.Light,
            ContentTypes.File => BadgeStyle.Dark,
            _ => BadgeStyle.Light
        };
    }

    private static BadgeStyle GetReportedByBadgeStyle(ReportedBy reportedBy)
    {
        return reportedBy switch
        {
            ReportedBy.Ai => BadgeStyle.Info,
            ReportedBy.User => BadgeStyle.Warning,
            ReportedBy.Admin => BadgeStyle.Danger,
            _ => BadgeStyle.Light
        };
    }

    private List<(string Name, double Value)> GetAiScores()
    {
        if (_report == null)
        {
            return [];
        }

        return
        [
            ("Toxicity", _report.Toxicity),
            ("Severe Toxicity", _report.SevereToxicity),
            ("Obscene", _report.Obscene),
            ("Threat", _report.Threat),
            ("Insult", _report.Insult),
            ("Identity Attack", _report.IdentityAttack),
            ("Sexual Explicit", _report.SexualExplicit)
        ];
    }

    private static string GetScoreColor(double score)
    {
        return score switch
        {
            < 0.3 => "var(--rz-success)",
            < 0.5 => "var(--rz-warning)",
            < 0.7 => "#ff9800", // Orange
            _ => "var(--rz-danger)"
        };
    }

    private static string GetScoreBarStyle(double score)
    {
        var color = score switch
        {
            < 0.3 => "var(--rz-success)",
            < 0.5 => "var(--rz-warning)",
            < 0.7 => "#ff9800",
            _ => "var(--rz-danger)"
        };

        return $"height: 8px; --rz-series-0: {color};";
    }

    private void ShowNotification(NotificationSeverity severity, string summary, string detail)
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = severity, Summary = summary, Detail = detail, Duration = 4000
        });
    }
}
