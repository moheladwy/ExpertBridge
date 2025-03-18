// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Core.Interfaces.Services;

namespace ExpertBridge.Api.Application.Repositories.Posts;

public class PostsCacheRepository(
    ICacheService cache,
    PostsRepository postRepository
    ) : IEntityRepository<Core.Entities.Posts.Post>
{
    public async Task<Core.Entities.Posts.Post?> GetByIdAsync(string id) => throw new NotImplementedException();

    public async Task<Core.Entities.Posts.Post?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public async Task<IEnumerable<Core.Entities.Posts.Post>> GetAllAsync() => throw new NotImplementedException();

    public async Task<Core.Entities.Posts.Post?> GetFirstAsync(Expression<Func<Core.Entities.Posts.Post, bool>> predicate) => throw new NotImplementedException();

    public async Task<Core.Entities.Posts.Post?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.Posts.Post, bool>> predicate) => throw new NotImplementedException();

    public async Task AddAsync(Core.Entities.Posts.Post entity) => throw new NotImplementedException();

    public async Task UpdateAsync(Core.Entities.Posts.Post entity) => throw new NotImplementedException();

    public async Task DeleteAsync(string id) => throw new NotImplementedException();
}
