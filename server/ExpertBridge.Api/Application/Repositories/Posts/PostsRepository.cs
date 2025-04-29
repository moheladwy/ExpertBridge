// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Api.Application.Interfaces.Repositories;
using ExpertBridge.Api.Data.DatabaseContexts;

namespace ExpertBridge.Api.Application.Repositories.Posts;

public class PostsRepository(ExpertBridgeDbContext db) : IEntityRepository<Core.Entities.Posts.Post>
{
    public Task<Core.Entities.Posts.Post?> GetByIdAsync(string id) => throw new NotImplementedException();

    public Task<Core.Entities.Posts.Post?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public Task<IEnumerable<Core.Entities.Posts.Post>> GetAllAsync() => throw new NotImplementedException();

    public Task<Core.Entities.Posts.Post?> GetFirstAsync(Expression<Func<Core.Entities.Posts.Post, bool>> predicate) => throw new NotImplementedException();

    public Task<Core.Entities.Posts.Post?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.Posts.Post, bool>> predicate) => throw new NotImplementedException();

    public Task AddAsync(Core.Entities.Posts.Post entity) => throw new NotImplementedException();

    public Task UpdateAsync(Core.Entities.Posts.Post entity) => throw new NotImplementedException();

    public Task DeleteAsync(string id) => throw new NotImplementedException();
}
