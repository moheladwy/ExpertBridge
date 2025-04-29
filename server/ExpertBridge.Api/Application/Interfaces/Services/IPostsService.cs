// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Responses;

namespace ExpertBridge.Api.Application.Interfaces.Services;

public interface IPostsService
{
    Task<PostResponse> GetByIdAsync(string id);
}
