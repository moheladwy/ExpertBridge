// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Api.Core.Entities.ManyToManyRelationships.ProfileTags;
using ExpertBridge.Api.Core.Entities.Posts;
using ExpertBridge.Api.Core.Entities.Tags;
using ExpertBridge.Api.Data.DatabaseContexts;
using ExpertBridge.Api.Responses;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.Tags;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Services
{
    public class TaggingService
    {
        private readonly ExpertBridgeDbContext _dbContext;

        public TaggingService(ExpertBridgeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddRawTagsToPostAsync(string postId, string authorId, PostCategorizerResponse tags)
        {
            List<Tag> rawTags = [];

            tags.Tags.ForEach(async tag =>
            {
                var existingTag = await _dbContext.Tags
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Name == tag.Tag);

                if (existingTag == null)
                {
                    existingTag = new Tag
                    {
                        Name = tag.Tag,
                        Description = tag.Description
                    };
                    await _dbContext.Tags.AddAsync(existingTag);
                }

                rawTags.Add(existingTag);
            });

            var post = await _dbContext.Posts.FirstAsync(p => p.Id == postId);

            await AddTagsToPostInternalAsync(post.Id, rawTags);
            await AddTagsToUserProfileInternalAsync(authorId, rawTags);

            post.Language = tags.Language;
            post.IsTagged = true;

            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// This method should not be called from outside because it's just
        /// a step in a bigger Unit of Work. It does not commit the changes.
        /// </summary>
        private async Task AddTagsToPostInternalAsync(string postId, IEnumerable<Tag> tags)
        {
            await _dbContext.PostTags.AddRangeAsync(tags.Select(tag => new PostTag
            {
                PostId = postId,
                Tag = tag
            }));
        }

        public async Task AddTagsToPostAsync(string postId, IEnumerable<Tag> tags)
        {
            await AddTagsToPostInternalAsync(postId, tags);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// This method should not be called from outside because it's just
        /// a step in a bigger Unit of Work. It does not commit the changes.
        /// </summary>
        private async Task AddTagsToUserProfileInternalAsync(string profileId, IEnumerable<Tag> tags)
        {
            await _dbContext.ProfileTags.AddRangeAsync(tags.Select(tag => new ProfileTag
            {
                ProfileId = profileId,
                Tag = tag
            }));
        }

        public async Task AddTagsToUserProfileAsync(string profileId, IEnumerable<Tag> tags)
        {
            await AddTagsToUserProfileInternalAsync(profileId, tags);
            await _dbContext.SaveChangesAsync();
        }
    }
}
