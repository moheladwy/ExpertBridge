using System.Linq.Expressions;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Data.DatabaseContexts;

namespace ExpertBridge.Api.Application.Repositories.Post;

public class PostRepository(ExpertBridgeDbContext db) : IEntityRepository<Core.Entities.Post.Post>
{
    public Task<Core.Entities.Post.Post?> GetByIdAsync(string id) => throw new NotImplementedException();

    public Task<Core.Entities.Post.Post?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public Task<IEnumerable<Core.Entities.Post.Post>> GetAllAsync() => throw new NotImplementedException();

    public Task<Core.Entities.Post.Post?> GetFirstAsync(Expression<Func<Core.Entities.Post.Post, bool>> predicate) => throw new NotImplementedException();

    public Task<Core.Entities.Post.Post?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.Post.Post, bool>> predicate) => throw new NotImplementedException();

    public Task AddAsync(Core.Entities.Post.Post entity) => throw new NotImplementedException();

    public Task UpdateAsync(Core.Entities.Post.Post entity) => throw new NotImplementedException();

    public Task DeleteAsync(string id) => throw new NotImplementedException();
}
