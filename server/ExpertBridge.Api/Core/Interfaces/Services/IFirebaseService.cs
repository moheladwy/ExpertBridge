// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Interfaces.Services;

public interface IFirebaseService
{
    Task<string> RegisterAsync(string email, string password);
    Task<string> LoginAsync(string email, string password);
}
