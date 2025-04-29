// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ExpertBridge.Api.Requests;
using ExpertBridge.Api.Responses;
using Refit;

namespace ExpertBridge.Api.HttpClients
{
    public interface IPostCategroizerClient
    {
        [Post("/categorize")]
        Task<ApiResponse<PostCategorizerResponse>> GetPostTagsAsync([Body] PostCategorizerRequest request);
    }
}
