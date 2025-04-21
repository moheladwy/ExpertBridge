using ExpertBridge.Api.Settings;
using ExpertBridge.Api.Core.Interfaces.Services;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.Application.Services;

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseAuth _auth = FirebaseAuth.DefaultInstance;
    private readonly HttpClient _httpClient;
    private readonly FirebaseSettings _firebaseConfig;
    private readonly FirebaseAuthSettings _authSettings;

    public FirebaseAuthService(
        IOptions<FirebaseSettings> firebaseConfig,
        IOptions<FirebaseAuthSettings> authSettings)
    {
        _firebaseConfig = firebaseConfig.Value;
        _authSettings = authSettings.Value;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_authSettings.TokenUri)
        };
    }

    public async Task<string> RegisterAsync(string email, string password)
    {
        var userArgs = new UserRecordArgs
        {
            Email = email,
            Password = password,
            Disabled = false,
            DisplayName = email,
            EmailVerified = true
        };

        var userRecord = await _auth.CreateUserAsync(userArgs);
        return userRecord.Uid;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        var request = new { email, password, returnSecureToken = true };
        var response = await _httpClient.PostAsJsonAsync("", request);
        response.EnsureSuccessStatusCode();

        var authToken = await response.Content.ReadFromJsonAsync<AuthToken>()
            ?? throw new InvalidOperationException("Failed to parse authentication token");

        return authToken.IdToken;
    }

    public async Task<FirebaseToken?> VerifyIdTokenAsync(string idToken)
    {
        try
        {
            return await _auth.VerifyIdTokenAsync(idToken, true);
        }
        catch
        {
            return null; // Invalid session
        }
    }
}
