using System.Net.Http.Json;
using ExpertBridge.Application.Configurations;
using ExpertBridge.Core.Interfaces.Services;
using FirebaseAdmin.Auth;

namespace ExpertBridge.Application.Services;

public class FirebaseService(HttpClient httpClient) : IFirebaseService
{
    private readonly FirebaseAuth _auth = FirebaseAuth.DefaultInstance;

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
        var response = await httpClient.PostAsJsonAsync("", request);
        var authToken = await response.Content.ReadFromJsonAsync<AuthToken>();
        return authToken.IdToken;
    }
}
