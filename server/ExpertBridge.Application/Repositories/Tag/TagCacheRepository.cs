// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Linq.Expressions;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;

namespace ExpertBridge.Application.Repositories.Tag;

public class TagCacheRepository(
    TagRepository tagRepository,
    ICacheService cache)
    : IEntityRepository<Core.Entities.Tags.Tag>
{
    public async Task<Core.Entities.Tags.Tag?> GetByIdAsync(string id) => throw new NotImplementedException();

    public async Task<Core.Entities.Tags.Tag?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public async Task<IEnumerable<Core.Entities.Tags.Tag>> GetAllAsync() => throw new NotImplementedException();

    public async Task<Core.Entities.Tags.Tag?> GetFirstAsync(Expression<Func<Core.Entities.Tags.Tag, bool>> predicate) => throw new NotImplementedException();

    public async Task<Core.Entities.Tags.Tag?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.Tags.Tag, bool>> predicate) => throw new NotImplementedException();

    public async Task AddAsync(Core.Entities.Tags.Tag entity) => throw new NotImplementedException();

    public async Task UpdateAsync(Core.Entities.Tags.Tag entity) => throw new NotImplementedException();

    public async Task DeleteAsync(string id) => throw new NotImplementedException();
}
