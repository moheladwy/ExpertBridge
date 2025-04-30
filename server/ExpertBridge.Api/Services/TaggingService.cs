// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Api.Responses;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Entities.Posts;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Services
{
    public class TaggingService
    {
        private readonly ExpertBridgeDbContext _dbContext;
        private readonly ChannelWriter<UserInterestsUpdatedMessage> _channel;

        public TaggingService(
            ExpertBridgeDbContext dbContext,
            Channel<UserInterestsUpdatedMessage> channel)
        {
            _dbContext = dbContext;
            _channel = channel.Writer;
        }

        // FUNCTIONAL PROGRAMMING IS MAD!
        // FUNCTIONAL PROGRAMMING IS MAD!
        // FUNCTIONAL PROGRAMMING IS MAD!
        public async Task AddRawTagsToPostAsync(string postId, string authorId, PostCategorizerResponse tags)
        {
            ArgumentNullException.ThrowIfNull(postId);
            ArgumentNullException.ThrowIfNull(tags);

            var tagList = tags.Tags.ToList();

            var englishNames = tagList.Select(t => t.EnglishName).ToList();
            var arabicNames = tagList.Select(t => t.ArabicName).ToList();

            var existingTags = await _dbContext.Tags
                .AsNoTracking()
                .Where(t => englishNames.Contains(t.EnglishName) || arabicNames.Contains(t.ArabicName))
                .ToListAsync(); // materialize the query to use it in next calculations.

            var newRawTags = tagList
                .Where(t => !existingTags
                    .Any(et => et.EnglishName == t.EnglishName || et.ArabicName == t.ArabicName))
                .ToList(); // this LINQ is only client side, thus we can use Any().

            var newTags = newRawTags.Select(tag => new Tag
            {
                EnglishName = tag.EnglishName,
                ArabicName = tag.ArabicName,
                Description = tag.Description
            }).ToList();

            await _dbContext.AddRangeAsync(newTags);
            await _dbContext.SaveChangesAsync(); // Save new tags to generate their IDs

            var post = await _dbContext.Posts.FirstAsync(p => p.Id == postId);

            var tagsToAdd = newTags.Concat(existingTags);

            await AddTagsToPostInternalAsync(post.Id, tagsToAdd.Select(t => t.Id));
            await AddTagsToUserProfileInternalAsync(authorId, tagsToAdd.Select(t => t.Id));

            post.Language = tags.Language;
            post.IsTagged = true;

            await _dbContext.SaveChangesAsync();

            await _channel.WriteAsync(new UserInterestsUpdatedMessage
            {
                UserProfileId = authorId,
            });
        }

        /// <summary>
        /// This method should not be called from outside because it's just
        /// a step in a bigger Unit of Work. It does not commit the changes to DB.
        /// </summary>
        private async Task AddTagsToPostInternalAsync(string postId, IEnumerable<string> tagIds)
        {
            var existingTagIds = await _dbContext.PostTags
                .Where(pt => pt.PostId == postId && tagIds.Contains(pt.TagId))
                .Select(pt => pt.TagId)
                .ToListAsync();

            var newPostTags = tagIds
                .Where(tagId => !existingTagIds.Contains(tagId))
                .Select(tagId => new PostTag
                {
                    PostId = postId,
                    TagId = tagId
                });

            await _dbContext.PostTags.AddRangeAsync(newPostTags);
        }

        public async Task AddTagsToPostAsync(string postId, IEnumerable<Tag> tags)
        {
            await AddTagsToPostInternalAsync(postId, tags.Select(t => t.Id));
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// This method should not be called from outside because it's just
        /// a step in a bigger Unit of Work. It does not commit the changes.
        /// </summary>
        private async Task AddTagsToUserProfileInternalAsync(string profileId, IEnumerable<string> tagIds)
        {
            var existingTagIds = await _dbContext.UserInterests
                .Where(ui => ui.ProfileId == profileId && tagIds.Contains(ui.TagId))
                .Select(pt => pt.TagId)
                .ToListAsync();

            var newUserInterests = tagIds
                .Where(tagId => !existingTagIds.Contains(tagId))
                .Select(tagId => new UserInterest
                {
                    ProfileId= profileId,
                    TagId = tagId
                });

            await _dbContext.UserInterests.AddRangeAsync(newUserInterests);
        }

        public async Task AddTagsToUserProfileAsync(string profileId, IEnumerable<Tag> tags)
        {
            await AddTagsToUserProfileInternalAsync(profileId, tags.Select(t => t.Id));
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddTagsToUserProfileAsync(string profileId, IEnumerable<string> tagIds)
        {
            await AddTagsToUserProfileInternalAsync(profileId, tagIds);
            await _dbContext.SaveChangesAsync();
        }
    }
}
