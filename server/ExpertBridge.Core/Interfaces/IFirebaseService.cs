namespace ExpertBridge.Core.Interfaces;

public interface IFirebaseService
{
    Task<string> RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
}
