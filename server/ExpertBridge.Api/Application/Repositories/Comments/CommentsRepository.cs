// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Api.Application.Interfaces.Repositories;
using ExpertBridge.Api.Core.Entities.Comments;
using ExpertBridge.Api.Data.DatabaseContexts;

namespace ExpertBridge.Api.Application.Repositories.Comments;

public class CommentsRepository(ExpertBridgeDbContext db) : IEntityRepository<Comment>
{
    public async Task<Comment?> GetByIdAsync(string id) => throw new NotImplementedException();

    public async Task<Comment?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public async Task<IEnumerable<Comment>> GetAllAsync() => throw new NotImplementedException();

    public async Task<Comment?> GetFirstAsync(Expression<Func<Comment, bool>> predicate) => throw new NotImplementedException();

    public async Task<Comment?> GetFirstAsNoTrackingAsync(Expression<Func<Comment, bool>> predicate) => throw new NotImplementedException();

    public async Task AddAsync(Comment entity) => throw new NotImplementedException();

    public async Task UpdateAsync(Comment entity) => throw new NotImplementedException();

    public async Task DeleteAsync(string id) => throw new NotImplementedException();
}
