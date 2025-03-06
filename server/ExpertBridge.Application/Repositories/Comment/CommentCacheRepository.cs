using System.Linq.Expressions;
using ExpertBridge.Core.Interfaces.Repositories;
using ExpertBridge.Core.Interfaces.Services;

namespace ExpertBridge.Application.Repositories.Comment;

public class CommentCacheRepository(
    ICacheService cache,
    CommentRepository commentRepository
    ) : IEntityRepository<Core.Entities.Comment.Comment>
{
    public async Task<Core.Entities.Comment.Comment?> GetByIdAsync(string id) => throw new NotImplementedException();

    public async Task<Core.Entities.Comment.Comment?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public async Task<IEnumerable<Core.Entities.Comment.Comment>> GetAllAsync() => throw new NotImplementedException();

    public async Task<Core.Entities.Comment.Comment?> GetFirstAsync(Expression<Func<Core.Entities.Comment.Comment, bool>> predicate) => throw new NotImplementedException();

    public async Task<Core.Entities.Comment.Comment?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.Comment.Comment, bool>> predicate) => throw new NotImplementedException();

    public async Task AddAsync(Core.Entities.Comment.Comment entity) => throw new NotImplementedException();

    public async Task UpdateAsync(Core.Entities.Comment.Comment entity) => throw new NotImplementedException();

    public async Task DeleteAsync(string id) => throw new NotImplementedException();
}
