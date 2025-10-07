// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Admin.ViewModels;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;
using Radzen.Blazor;

namespace ExpertBridge.Admin.Components.Pages;

public partial class DeletedTags
{
  private readonly ExpertBridgeDbContext _dbContext;
  private readonly HybridCache _cache;
  private readonly NotificationService _notificationService;
  public List<PostTagsViewModel> deletedTags;
  private RadzenDataGrid<PostTagsViewModel>? grid;
  private bool isLoading = true;

  public DeletedTags(ExpertBridgeDbContext dbContext, HybridCache cache, NotificationService notificationService)
  {
    _dbContext = dbContext;
    _cache = cache;
    _notificationService = notificationService;
    deletedTags = [];
  }

  protected override async Task OnInitializedAsync()
  {
    try
    {
      isLoading = true;
      var key = "DeletedTags";

      // Note: Tag entity does not implement ISoftDeletable
      // So this will always return an empty list
      // This page is created for consistency with the UI structure
      deletedTags = await _cache.GetOrCreateAsync(key,
      async (cancellationToken) =>
      {
        // Since Tags don't support soft deletion, return empty list
        return await Task.FromResult(new List<PostTagsViewModel>());
      });
    }
    finally
    {
      isLoading = false;
    }
    await base.OnInitializedAsync();
  }
}
