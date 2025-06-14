// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Core.Entities.Media;
using ExpertBridge.Core.Requests;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.DomainServices
{
    public class MediaAttachmentService
    {
        // No DbContext injected here if we pass it as a parameter,
        // which aligns better with the service being stateless regarding the UoW context.

        public async Task<List<TMedia>> ProcessAndAttachMediaAsync<TEntity, TMedia>(
            ICollection<MediaObjectRequest> mediaRequests,
            TEntity parentEntity,
            Func<MediaObjectRequest, TEntity, TMedia> createMediaEntityFunc,
            ExpertBridgeDbContext dbContext)
            where TMedia : MediaObject
            where TEntity : class
        {
            if (mediaRequests == null || !mediaRequests.Any())
            {
                return new List<TMedia>();
            }

            var mediaEntities = new List<TMedia>();
            foreach (var mediaReq in mediaRequests)
            {
                var mediaEntity = createMediaEntityFunc(mediaReq, parentEntity);
                mediaEntities.Add(mediaEntity);
            }

            // Add to DbContext, but don't save
            await dbContext.Set<TMedia>().AddRangeAsync(mediaEntities);

            var keys = mediaEntities.Select(m => m.Key).ToList();
            if (keys.Any())
            {
                var grants = await dbContext.MediaGrants
                    .Where(grant => keys.Contains(grant.Key))
                    .ToListAsync(); // Fetch to modify

                foreach (var grant in grants)
                {
                    grant.IsActive = true;
                    grant.OnHold = false;
                    grant.ActivatedAt = DateTime.UtcNow;
                    // dbContext will track these changes
                }
            }
            return mediaEntities;
        }
    }
}
