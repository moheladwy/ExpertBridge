// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Api.Application.Interfaces.Repositories;
using ExpertBridge.Api.Core.Entities.Tags;
using ExpertBridge.Api.Data.DatabaseContexts;

namespace ExpertBridge.Api.Application.Repositories.Tags;

public class TagsRepository(ExpertBridgeDbContext db) : IEntityRepository<Tag>
{
    public async Task<Tag?> GetByIdAsync(string id) => throw new NotImplementedException();

    public async Task<Tag?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public async Task<IEnumerable<Tag>> GetAllAsync() => throw new NotImplementedException();

    public async Task<Tag?> GetFirstAsync(Expression<Func<Tag, bool>> predicate) => throw new NotImplementedException();

    public async Task<Tag?> GetFirstAsNoTrackingAsync(Expression<Func<Tag, bool>> predicate) => throw new NotImplementedException();

    public async Task AddAsync(Tag entity) => throw new NotImplementedException();

    public async Task UpdateAsync(Tag entity) => throw new NotImplementedException();

    public async Task DeleteAsync(string id) => throw new NotImplementedException();
}
