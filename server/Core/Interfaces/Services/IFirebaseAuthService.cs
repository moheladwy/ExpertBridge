using FirebaseAdmin.Auth;

namespace Core.Interfaces.Services;

public interface IFirebaseAuthService
{
    Task<string> RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
    Task<FirebaseToken?> VerifyIdTokenAsync(string idToken);
}
