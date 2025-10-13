// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Data.DatabaseContexts;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Radzen;

namespace ExpertBridge.Admin.Components.SubModules;

public partial class EditTagDialog
{
    private string errorMessage = string.Empty;

    private bool isSaving;

    [Parameter] public string TagId { get; set; } = string.Empty;

    [Parameter] public string EnglishName { get; set; } = string.Empty;

    [Parameter] public string ArabicName { get; set; } = string.Empty;

    [Parameter] public string Description { get; set; } = string.Empty;

    [Inject] private ExpertBridgeDbContext DbContext { get; set; } = default!;

    [Inject] private HybridCache Cache { get; set; } = default!;

    [Inject] private DialogService DialogService { get; set; } = default!;

    private async Task SaveTag()
    {
        try
        {
            isSaving = true;
            errorMessage = string.Empty;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(EnglishName))
            {
                errorMessage = "English name is required.";
                return;
            }

            // Find the tag
            var tag = await DbContext.Tags
                .FirstOrDefaultAsync(t => t.Id == TagId);

            if (tag == null)
            {
                errorMessage = "Tag not found.";
                return;
            }

            // Update the tag
            tag.EnglishName = EnglishName.Trim();
            tag.ArabicName = string.IsNullOrWhiteSpace(ArabicName) ? null : ArabicName.Trim();
            tag.Description = string.IsNullOrWhiteSpace(Description) ? null! : Description.Trim();

            await DbContext.SaveChangesAsync();

            // Clear the cache
            await Cache.RemoveAsync("AllPostTags");

            // Close the dialog with success result
            DialogService.Close(true);
        }
        catch (Exception ex)
        {
            errorMessage = $"Error saving tag: {ex.Message}";
        }
        finally
        {
            isSaving = false;
        }
    }

    private void Cancel() => DialogService.Close(false);
}
