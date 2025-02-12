using System.Linq.Expressions;

namespace ExpertBridge.Core.Interfaces.Repositories;

public interface IEntityRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id);
    Task<T?> GetByIdAsNoTrackingAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetFirstAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetFirstAsNoTrackingAsync(Expression<Func<T, bool>> predicate);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
}
