namespace Core.Interfaces.Services;
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T entity);
    Task UpdateAsync<T>(string key, T entity);
    Task RemoveAsync(string key);
}
