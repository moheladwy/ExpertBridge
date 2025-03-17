using System.Linq.Expressions;
using ExpertBridge.Api.Core.Interfaces.Repositories;
using ExpertBridge.Api.Core.Interfaces.Services;

namespace ExpertBridge.Api.Application.Repositories.Post;

public class PostCacheRepository(
    ICacheService cache,
    PostRepository postRepository
    ) : IEntityRepository<Core.Entities.Post.Post>
{
    public async Task<Core.Entities.Post.Post?> GetByIdAsync(string id) => throw new NotImplementedException();

    public async Task<Core.Entities.Post.Post?> GetByIdAsNoTrackingAsync(string id) => throw new NotImplementedException();

    public async Task<IEnumerable<Core.Entities.Post.Post>> GetAllAsync() => throw new NotImplementedException();

    public async Task<Core.Entities.Post.Post?> GetFirstAsync(Expression<Func<Core.Entities.Post.Post, bool>> predicate) => throw new NotImplementedException();

    public async Task<Core.Entities.Post.Post?> GetFirstAsNoTrackingAsync(Expression<Func<Core.Entities.Post.Post, bool>> predicate) => throw new NotImplementedException();

    public async Task AddAsync(Core.Entities.Post.Post entity) => throw new NotImplementedException();

    public async Task UpdateAsync(Core.Entities.Post.Post entity) => throw new NotImplementedException();

    public async Task DeleteAsync(string id) => throw new NotImplementedException();
}
