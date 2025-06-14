// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using System.Threading.Channels;
using ExpertBridge.Api.Models.IPC;
using ExpertBridge.Core.Entities.ManyToManyRelationships.PostTags;
using ExpertBridge.Core.Entities.ManyToManyRelationships.UserInterests;
using ExpertBridge.Core.Entities.Tags;
using ExpertBridge.Core.Responses;
using ExpertBridge.Data.DatabaseContexts;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.DomainServices;

/// <summary>
///     Provides methods to manage and associate tags with posts and user profiles.
/// </summary>
public sealed class TaggingService
{
    /// <summary>
    ///     Represents the database context used to interact with the underlying database
    ///     for performing data-related operations, including accessing and manipulating
    ///     entities such as Users, Profiles, Tags, Posts, and more.
    /// </summary>
    private readonly ExpertBridgeDbContext _dbContext;

    /// <summary>
    ///     Represents a channel writer used for publishing messages related to user interest updates.
    ///     This allows asynchronous communication of changes to user profiles, such as updated tags or interests.
    /// </summary>
    private readonly ChannelWriter<UserInterestsUpdatedMessage> _userInterestsChannel;

    /// <summary>
    /// Represents the channel writer responsible for handling and dispatching
    /// messages containing user interest processing data, ensuring proper
    /// communication and task coordination within the system.
    /// </summary>
    private readonly ChannelWriter<UserInterestsProsessingMessage> _userInterestsProcessingChannel;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TaggingService" /> class.
    /// </summary>
    /// <param name="dbContext">The database context used for data operations.</param>
    /// <param name="userInterestsChannel">The channel used for publishing user interest update messages.</param>
    /// <param name="userInterestsProcessingChannel">The channel used for processing user interest messages.</param>
    public TaggingService(
        ExpertBridgeDbContext dbContext,
        Channel<UserInterestsUpdatedMessage> userInterestsChannel,
        Channel<UserInterestsProsessingMessage> userInterestsProcessingChannel)
    {
        _dbContext = dbContext;
        _userInterestsChannel = userInterestsChannel;
        _userInterestsProcessingChannel = userInterestsProcessingChannel.Writer;
    }


    /// <summary>
    ///     Adds the provided raw tags to a post and updates the related user profile.
    /// </summary>
    /// <param name="postId">The unique identifier of the post to which the tags will be added.</param>
    /// <param name="authorId">The unique identifier of the author of the post.</param>
    /// <param name="tags">The categorizer response containing the tags to be added to the post.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddRawTagsToPostAsync(
        string postId,
        string authorId,
        PostCategorizerResponse tags,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(postId);
        ArgumentNullException.ThrowIfNull(tags);

        var tagList = tags.Tags.ToList();

        var englishNames = tagList.Select(t => t.EnglishName).ToList();
        var arabicNames = tagList.Select(t => t.ArabicName).ToList();

        var existingTags = await _dbContext.Tags
            .AsNoTracking()
            .Where(t => englishNames.Contains(t.EnglishName) || arabicNames.Contains(t.ArabicName))
            .ToListAsync(cancellationToken); // materialize the query to use it in next calculations.

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

        await _dbContext.AddRangeAsync(newTags, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken); // Save new tags to generate their IDs

        var post = await _dbContext.Posts.FirstAsync(p => p.Id == postId, cancellationToken);

        var tagsToAdd = newTags.Concat(existingTags);

        await AddTagsToPostInternalAsync(post.Id, tagsToAdd.Select(t => t.Id), cancellationToken);
        await AddTagsToUserProfileInternalAsync(authorId, tagsToAdd.Select(t => t.Id), cancellationToken);

        post.Language = tags.Language;
        post.IsTagged = true;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _userInterestsChannel.WriteAsync(new UserInterestsUpdatedMessage { UserProfileId = authorId },
            cancellationToken);
    }


    /// <summary>
    ///     Associates a collection of tag IDs with a specific post by creating PostTag relationships in the database.
    /// </summary>
    /// <param name="postId">The identifier of the post to which the tags will be added.</param>
    /// <param name="tagIds">A collection of tag IDs that need to be associated with the post.</param>
    /// <param name="cancellationToken">
    ///     A token to observe while waiting for the task to complete, enabling cancellation of the
    ///     operation.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddTagsToPostInternalAsync(
        string postId,
        IEnumerable<string> tagIds,
        CancellationToken cancellationToken = default)
    {
        var existingTagIds = await _dbContext.PostTags
            .Where(pt => pt.PostId == postId && tagIds.Contains(pt.TagId))
            .Select(pt => pt.TagId)
            .ToListAsync(cancellationToken);

        var newPostTags = tagIds
            .Where(tagId => !existingTagIds.Contains(tagId))
            .Select(tagId => new PostTag { PostId = postId, TagId = tagId });

        await _dbContext.PostTags.AddRangeAsync(newPostTags, cancellationToken);
    }

    /// <summary>
    ///     Adds a collection of tags to a specified post by associating them with the post using their identifiers.
    /// </summary>
    /// <param name="postId">The identifier of the post to which the tags will be added.</param>
    /// <param name="tags">A collection of tags to be associated with the post.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddTagsToPostAsync(string postId, IEnumerable<Tag> tags)
    {
        await AddTagsToPostInternalAsync(postId, tags.Select(t => t.Id));
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    ///     Adds tags to a user profile within a transaction. This method does not commit
    ///     changes to the database, as it is intended to be part of a larger unit of work.
    ///     So it should not be called outside directly.
    /// </summary>
    /// <param name="profileId">The unique identifier of the user's profile.</param>
    /// <param name="tagIds">A collection of tag IDs to associate with the user profile.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task AddTagsToUserProfileInternalAsync(
        string profileId,
        IEnumerable<string> tagIds,
        CancellationToken cancellationToken = default)
    {
        var existingTagIds = await _dbContext.UserInterests
            .Where(ui => ui.ProfileId == profileId && tagIds.Contains(ui.TagId))
            .Select(pt => pt.TagId)
            .ToListAsync(cancellationToken);

        var newUserInterests = tagIds
            .Where(tagId => !existingTagIds.Contains(tagId))
            .Select(tagId => new UserInterest { ProfileId = profileId, TagId = tagId });

        await _dbContext.UserInterests.AddRangeAsync(newUserInterests, cancellationToken);
    }

    /// <summary>
    ///     Associates the specified tags with a user profile.
    /// </summary>
    /// <param name="profileId">The unique identifier of the user profile to which the tags will be added.</param>
    /// <param name="tags">A collection of tags to associate with the user profile.</param>
    public async Task AddTagsToUserProfileAsync(string profileId, IEnumerable<Tag> tags) =>
        await AddTagsToUserProfileAsync(profileId, tags.Select(t => t.Id));

    /// <summary>
    ///     Associates the specified set of tag identifiers with a user profile.
    ///     This is the method that will be called all the time.
    ///     Whether the code calls this or the other overload, eventually
    ///     this method is the one to be called, and it's the one responsible
    ///     for commiting changes to DB.
    /// </summary>
    /// <param name="profileId">The unique identifier of the user profile to which the tags will be added.</param>
    /// <param name="tagIds">A collection of tag identifiers to associate with the user profile.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddTagsToUserProfileAsync(string profileId, IEnumerable<string> tagIds)
    {
        await AddTagsToUserProfileInternalAsync(profileId, tagIds);
        await _dbContext.SaveChangesAsync();

        await _userInterestsChannel.WriteAsync(new UserInterestsUpdatedMessage { UserProfileId = profileId });
    }
}
