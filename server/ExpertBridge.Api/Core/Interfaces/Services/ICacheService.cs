// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ExpertBridge.Api.Core.Interfaces.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T entity);
    Task UpdateAsync<T>(string key, T entity);
    Task RemoveAsync(string key);
}
