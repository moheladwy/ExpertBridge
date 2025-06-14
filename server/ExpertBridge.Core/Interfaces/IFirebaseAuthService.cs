// Licensed to the.NET Foundation under one or more agreements.
// The.NET Foundation licenses this file to you under the MIT license.

using FirebaseAdmin.Auth;

namespace ExpertBridge.Core.Interfaces;

public interface IFirebaseAuthService
{
    Task<string> RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
    Task<FirebaseToken?> VerifyIdTokenAsync(string idToken);
}
