namespace ExpertBridge.Api.Core.Interfaces.Services;

public interface IFirebaseService
{
    Task<string> RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
}
