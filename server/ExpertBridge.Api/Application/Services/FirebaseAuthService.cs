// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Configurations;
using ExpertBridge.Api.Core.Interfaces.Services;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Options;

namespace ExpertBridge.Api.Application.Services;

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly FirebaseAuth _auth = FirebaseAuth.DefaultInstance;
    private readonly HttpClient httpClient;

    public FirebaseAuthService(IOptions<FirebaseCredentials> credentials)
    {
        httpClient = new HttpClient { BaseAddress = new Uri(credentials.Value.AuthenticationTokenUri) };
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
        var response = await httpClient.PostAsJsonAsync("",request);
        var authToken = await response.Content.ReadFromJsonAsync<AuthToken>();
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
